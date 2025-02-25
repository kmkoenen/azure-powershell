// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

namespace Microsoft.Azure.Management.Storage
{
    using Microsoft.Rest.Azure;
    using Models;

    /// <summary>
    /// ObjectReplicationPoliciesOperations operations.
    /// </summary>
    public partial interface IObjectReplicationPoliciesOperations
    {
        /// <summary>
        /// List the object replication policies associated with the storage account.
        /// </summary>
        /// <remarks>
        /// List the object replication policies associated with the storage account.
        /// </remarks>
        /// <param name='resourceGroupName'>
        /// The name of the resource group within the user&#39;s subscription. The name is case insensitive.
        /// </param>
        /// <param name='accountName'>
        /// The name of the storage account within the specified resource group. Storage account names must be between 3 and 24 characters in length and use numbers and lower-case letters only.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.Azure.CloudException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        System.Threading.Tasks.Task<Microsoft.Rest.Azure.AzureOperationResponse<System.Collections.Generic.IEnumerable<ObjectReplicationPolicy>>> ListWithHttpMessagesAsync(string resourceGroupName, string accountName, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Get the object replication policy of the storage account by policy ID.
        /// </summary>
        /// <remarks>
        /// Get the object replication policy of the storage account by policy ID.
        /// </remarks>
        /// <param name='resourceGroupName'>
        /// The name of the resource group within the user&#39;s subscription. The name is case insensitive.
        /// </param>
        /// <param name='accountName'>
        /// The name of the storage account within the specified resource group. Storage account names must be between 3 and 24 characters in length and use numbers and lower-case letters only.
        /// </param>
        /// <param name='objectReplicationPolicyId'>
        /// For the destination account, provide the value &#39;default&#39;. Configure the policy on the destination account first. For the source account, provide the value of the policy ID that is returned when you download the policy that was defined on the destination account. The policy is downloaded as a JSON file.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.Azure.CloudException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        System.Threading.Tasks.Task<Microsoft.Rest.Azure.AzureOperationResponse<ObjectReplicationPolicy>> GetWithHttpMessagesAsync(string resourceGroupName, string accountName, string objectReplicationPolicyId, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Create or update the object replication policy of the storage account.
        /// </summary>
        /// <remarks>
        /// Create or update the object replication policy of the storage account.
        /// </remarks>
        /// <param name='resourceGroupName'>
        /// The name of the resource group within the user&#39;s subscription. The name is case insensitive.
        /// </param>
        /// <param name='accountName'>
        /// The name of the storage account within the specified resource group. Storage account names must be between 3 and 24 characters in length and use numbers and lower-case letters only.
        /// </param>
        /// <param name='objectReplicationPolicyId'>
        /// For the destination account, provide the value &#39;default&#39;. Configure the policy on the destination account first. For the source account, provide the value of the policy ID that is returned when you download the policy that was defined on the destination account. The policy is downloaded as a JSON file.
        /// </param>
        /// <param name='properties'>
        /// The object replication policy set to a storage account. A unique policy ID will be created if absent.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.Azure.CloudException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        System.Threading.Tasks.Task<Microsoft.Rest.Azure.AzureOperationResponse<ObjectReplicationPolicy>> CreateOrUpdateWithHttpMessagesAsync(string resourceGroupName, string accountName, string objectReplicationPolicyId, ObjectReplicationPolicy properties, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Deletes the object replication policy associated with the specified storage account.
        /// </summary>
        /// <remarks>
        /// Deletes the object replication policy associated with the specified storage account.
        /// </remarks>
        /// <param name='resourceGroupName'>
        /// The name of the resource group within the user&#39;s subscription. The name is case insensitive.
        /// </param>
        /// <param name='accountName'>
        /// The name of the storage account within the specified resource group. Storage account names must be between 3 and 24 characters in length and use numbers and lower-case letters only.
        /// </param>
        /// <param name='objectReplicationPolicyId'>
        /// For the destination account, provide the value &#39;default&#39;. Configure the policy on the destination account first. For the source account, provide the value of the policy ID that is returned when you download the policy that was defined on the destination account. The policy is downloaded as a JSON file.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.Azure.CloudException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        System.Threading.Tasks.Task<Microsoft.Rest.Azure.AzureOperationResponse> DeleteWithHttpMessagesAsync(string resourceGroupName, string accountName, string objectReplicationPolicyId, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> customHeaders = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

    }
}