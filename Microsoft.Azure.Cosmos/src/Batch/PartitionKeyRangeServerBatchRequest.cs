﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;

    internal sealed class PartitionKeyRangeServerBatchRequest : ServerBatchRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionKeyRangeServerBatchRequest"/> class.
        /// </summary>
        /// <param name="partitionKeyRangeId">The partition key range id associated with all requests.</param>
        /// <param name="maxBodyLength">Maximum length allowed for the request body.</param>
        /// <param name="maxOperationCount">Maximum number of operations allowed in the request.</param>
        /// <param name="serializer">Serializer to serialize user provided objects to JSON.</param>
        public PartitionKeyRangeServerBatchRequest(
            string partitionKeyRangeId,
            int maxBodyLength,
            int maxOperationCount,
            CosmosSerializer serializer)
            : base(maxBodyLength, maxOperationCount, serializer)
        {
            this.PartitionKeyRangeId = partitionKeyRangeId;
        }

        /// <summary>
        ///  Gets the PartitionKeyRangeId that applies to all operations in this request.
        /// </summary>
        public string PartitionKeyRangeId { get; }

        /// <summary>
        /// Creates an instance of <see cref="PartitionKeyRangeServerBatchRequest"/>.
        /// In case of direct mode requests, all the operations are expected to belong to the same PartitionKeyRange.
        /// The body of the request is populated with operations till it reaches the provided maxBodyLength.
        /// </summary>
        /// <param name="partitionKeyRangeId">The partition key range id associated with all requests.</param>
        /// <param name="operations">Operations to be added into this batch request.</param>
        /// <param name="maxBodyLength">Desired maximum length of the request body.</param>
        /// <param name="maxOperationCount">Maximum number of operations allowed in the request.</param>
        /// <param name="ensureContinuousOperationIndexes">Whether to stop adding operations to the request once there is non-continuity in the operation indexes.</param>
        /// <param name="serializer">Serializer to serialize user provided objects to JSON.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>A newly created instance of <see cref="PartitionKeyRangeServerBatchRequest"/>.</returns>
        public static async Task<Tuple<PartitionKeyRangeServerBatchRequest, ArraySegment<ItemBatchOperation>>> CreateAsync(
            string partitionKeyRangeId,
            ArraySegment<ItemBatchOperation> operations,
            int maxBodyLength,
            int maxOperationCount,
            bool ensureContinuousOperationIndexes,
            CosmosSerializer serializer,
            CancellationToken cancellationToken)
        {
            PartitionKeyRangeServerBatchRequest request = new PartitionKeyRangeServerBatchRequest(partitionKeyRangeId, maxBodyLength, maxOperationCount, serializer);
            ArraySegment<ItemBatchOperation> pendingOperations = await request.CreateBodyStreamAsync(operations, cancellationToken, ensureContinuousOperationIndexes);
            return new Tuple<PartitionKeyRangeServerBatchRequest, ArraySegment<ItemBatchOperation>>(request, pendingOperations);
        }
    }
}
