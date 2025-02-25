﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------
//
using Azure.Identity;

using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensibility;

using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.PowerShell.Authenticators.Identity
{
    internal class MsalConfidentialClient : MsalClientBase<IConfidentialClientApplication>
    {
        internal readonly string _clientSecret;
        internal readonly bool _includeX5CClaimHeader;
        internal readonly IX509Certificate2Provider _certificateProvider;
        private readonly Func<string> _assertionCallback;
        private readonly Func<CancellationToken, Task<string>> _asyncAssertionCallback;
        private readonly Func<AppTokenProviderParameters, Task<AppTokenProviderResult>> _appTokenProviderCallback;

        internal string RedirectUrl { get; }

        /// <summary>
        /// For mocking purposes only.
        /// </summary>
        protected MsalConfidentialClient()
        { }

        public MsalConfidentialClient(CredentialPipeline pipeline, string tenantId, string clientId, string clientSecret, string redirectUrl, TokenCredentialOptions options)
            : base(pipeline, tenantId, clientId, options)
        {
            _clientSecret = clientSecret;
            RedirectUrl = redirectUrl;
        }

        public MsalConfidentialClient(CredentialPipeline pipeline, string tenantId, string clientId, IX509Certificate2Provider certificateProvider, bool includeX5CClaimHeader, TokenCredentialOptions options)
            : base(pipeline, tenantId, clientId, options)
        {
            _includeX5CClaimHeader = includeX5CClaimHeader;
            _certificateProvider = certificateProvider;
        }

        public MsalConfidentialClient(CredentialPipeline pipeline, string tenantId, string clientId, Func<string> assertionCallback, TokenCredentialOptions options)
            : base(pipeline, tenantId, clientId, options)
        {
            _assertionCallback = assertionCallback;
        }

        public MsalConfidentialClient(CredentialPipeline pipeline, string tenantId, string clientId, Func<CancellationToken, Task<string>> assertionCallback, TokenCredentialOptions options)
            : base(pipeline, tenantId, clientId, options)
        {
            _asyncAssertionCallback = assertionCallback;
        }

        public MsalConfidentialClient(CredentialPipeline pipeline, string tenantId, string clientId, Func<AppTokenProviderParameters, Task<AppTokenProviderResult>> appTokenProviderCallback, TokenCredentialOptions options)
            : base(pipeline, tenantId, clientId, options)
        {
            _appTokenProviderCallback = appTokenProviderCallback;
        }

        internal string RegionalAuthority { get; } = EnvironmentVariables.AzureRegionalAuthorityName;

        protected override ValueTask<IConfidentialClientApplication> CreateClientAsync(bool enableCae, bool async, CancellationToken cancellationToken)
        {
            return CreateClientCoreAsync(enableCae, async, cancellationToken);
        }

        protected virtual async ValueTask<IConfidentialClientApplication> CreateClientCoreAsync(bool enableCae, bool async, CancellationToken cancellationToken)
        {
            string[] clientCapabilities =
                enableCae ? cp1Capabilities : Array.Empty<string>();

            ConfidentialClientApplicationBuilder confClientBuilder = ConfidentialClientApplicationBuilder.Create(ClientId)
                .WithHttpClientFactory(new HttpPipelineClientFactory(Pipeline.HttpPipeline))
                .WithLogging(LogMsal, enablePiiLogging: IsSupportLoggingEnabled);

            // Special case for using appTokenProviderCallback, authority validation and instance metadata discovery should be disabled since we're not calling the STS
            // The authority matches the one configured in the CredentialOptions.
            if (_appTokenProviderCallback != null)
            {
                confClientBuilder.WithAppTokenProvider(_appTokenProviderCallback)
                    .WithAuthority(AuthorityHost.AbsoluteUri, TenantId, false)
                    .WithInstanceDiscovery(false);
            }
            else
            {
                confClientBuilder.WithAuthority(AuthorityHost.AbsoluteUri, TenantId);
                if (DisableInstanceDiscovery)
                {
                    confClientBuilder.WithInstanceDiscovery(false);
                }
            }

            if (clientCapabilities.Length > 0)
            {
                confClientBuilder.WithClientCapabilities(clientCapabilities);
            }

            if (_clientSecret != null)
            {
                confClientBuilder.WithClientSecret(_clientSecret);
            }

            if (_assertionCallback != null)
            {
                confClientBuilder.WithClientAssertion(_assertionCallback);
            }

            if (_asyncAssertionCallback != null)
            {
                confClientBuilder.WithClientAssertion(_asyncAssertionCallback);
            }

            if (_certificateProvider != null)
            {
                X509Certificate2 clientCertificate = await _certificateProvider.GetCertificateAsync(async, cancellationToken).ConfigureAwait(false);
                confClientBuilder.WithCertificate(clientCertificate);
            }

            // When the appTokenProviderCallback is set, meaning this is for managed identity, the regional authority is not relevant.
            if (_appTokenProviderCallback == null && !string.IsNullOrEmpty(RegionalAuthority))
            {
                confClientBuilder.WithAzureRegion(RegionalAuthority);
            }

            if (!string.IsNullOrEmpty(RedirectUrl))
            {
                confClientBuilder.WithRedirectUri(RedirectUrl);
            }

            return confClientBuilder.Build();
        }

        public virtual async ValueTask<AuthenticationResult> AcquireTokenForClientAsync(
            string[] scopes,
            string tenantId,
            bool enableCae,
            bool async,
            CancellationToken cancellationToken)
        {
            var result = await AcquireTokenForClientCoreAsync(scopes, tenantId, enableCae, async, cancellationToken).ConfigureAwait(false);
            LogAccountDetails(result);
            return result;
        }

        public virtual async ValueTask<AuthenticationResult> AcquireTokenForClientCoreAsync(
            string[] scopes,
            string tenantId,
            bool enableCae,
            bool async,
            CancellationToken cancellationToken)
        {
            IConfidentialClientApplication client = await GetClientAsync(enableCae, async, cancellationToken).ConfigureAwait(false);

            var builder = client
                .AcquireTokenForClient(scopes)
                .WithSendX5C(_includeX5CClaimHeader);

            if (!string.IsNullOrEmpty(tenantId))
            {
                builder.WithAuthority(AuthorityHost.AbsoluteUri, tenantId);
            }
            return await builder
                .ExecuteAsync(async, cancellationToken)
                .ConfigureAwait(false);
        }

        public virtual async ValueTask<AuthenticationResult> AcquireTokenSilentAsync(
            string[] scopes,
            AuthenticationAccount account,
            string tenantId,
            string redirectUri,
            bool enableCae,
            bool async,
            CancellationToken cancellationToken)
        {
            var result = await AcquireTokenSilentCoreAsync(scopes, account, tenantId, redirectUri, enableCae, async, cancellationToken).ConfigureAwait(false);
            LogAccountDetails(result);
            return result;
        }

        public virtual async ValueTask<AuthenticationResult> AcquireTokenSilentCoreAsync(
            string[] scopes,
            AuthenticationAccount account,
            string tenantId,
            string redirectUri,
            bool enableCae,
            bool async,
            CancellationToken cancellationToken)
        {
            IConfidentialClientApplication client = await GetClientAsync(enableCae, async, cancellationToken).ConfigureAwait(false);

            var builder = client.AcquireTokenSilent(scopes, account);
            if (!string.IsNullOrEmpty(tenantId))
            {
                builder.WithAuthority(AuthorityHost.AbsoluteUri, tenantId);
            }
            return await builder
                .ExecuteAsync(async, cancellationToken)
                .ConfigureAwait(false);
        }

        public virtual async ValueTask<AuthenticationResult> AcquireTokenByAuthorizationCodeAsync(
            string[] scopes,
            string code,
            string tenantId,
            string redirectUri,
            bool enableCae,
            bool async,
            CancellationToken cancellationToken)
        {
            var result = await AcquireTokenByAuthorizationCodeCoreAsync(scopes, code, tenantId, redirectUri, enableCae, async, cancellationToken).ConfigureAwait(false);
            LogAccountDetails(result);
            return result;
        }

        public virtual async ValueTask<AuthenticationResult> AcquireTokenByAuthorizationCodeCoreAsync(
            string[] scopes,
            string code,
            string tenantId,
            string redirectUri,
            bool enableCae,
            bool async,
            CancellationToken cancellationToken)
        {
            IConfidentialClientApplication client = await GetClientAsync(enableCae, async, cancellationToken).ConfigureAwait(false);

            var builder = client.AcquireTokenByAuthorizationCode(scopes, code);

            if (!string.IsNullOrEmpty(tenantId))
            {
                builder.WithAuthority(AuthorityHost.AbsoluteUri, tenantId);
            }
            return await builder
                .ExecuteAsync(async, cancellationToken)
                .ConfigureAwait(false);
        }

        public virtual async ValueTask<AuthenticationResult> AcquireTokenOnBehalfOfAsync(
            string[] scopes,
            string tenantId,
            UserAssertion userAssertionValue,
            bool enableCae,
            bool async,
            CancellationToken cancellationToken)
        {
            var result = await AcquireTokenOnBehalfOfCoreAsync(scopes, tenantId, userAssertionValue, enableCae, async, cancellationToken).ConfigureAwait(false);
            LogAccountDetails(result);
            return result;
        }

        public virtual async ValueTask<AuthenticationResult> AcquireTokenOnBehalfOfCoreAsync(
            string[] scopes,
            string tenantId,
            UserAssertion userAssertionValue,
            bool enableCae,
            bool async,
            CancellationToken cancellationToken)
        {
            IConfidentialClientApplication client = await GetClientAsync(enableCae, async, cancellationToken).ConfigureAwait(false);

            var builder = client
                .AcquireTokenOnBehalfOf(scopes, userAssertionValue)
                .WithSendX5C(_includeX5CClaimHeader);

            if (!string.IsNullOrEmpty(tenantId))
            {
                builder.WithAuthority(AuthorityHost.AbsoluteUri, tenantId);
            }
            return await builder
                .ExecuteAsync(async, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
