//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Azure.Core.Pipeline;
    using Microsoft.Azure.Cosmos.Handlers;
    using Microsoft.Azure.Cosmos.Query;
    using Microsoft.Azure.Documents;

    internal class PipelineClientContextCore : CosmosClientContext
    {
        private readonly HttpPipeline pipeline;

        internal PipelineClientContextCore(
            CosmosClient client,
            CosmosClientOptions clientOptions,
            CosmosSerializer userJsonSerializer,
            CosmosSerializer defaultJsonSerializer,
            CosmosSerializer sqlQuerySpecSerializer,
            CosmosResponseFactory cosmosResponseFactory,
            RequestInvokerHandler requestHandler,
            DocumentClient documentClient,
            IDocumentQueryClient documentQueryClient)
        {
            this.Client = client;
            this.ClientOptions = clientOptions;
            this.CosmosSerializer = userJsonSerializer;
            this.PropertiesSerializer = defaultJsonSerializer;
            this.SqlQuerySpecSerializer = sqlQuerySpecSerializer;
            this.ResponseFactory = cosmosResponseFactory;
            this.RequestHandler = requestHandler;
            this.DocumentClient = documentClient;
            this.DocumentQueryClient = documentQueryClient;

            this.pipeline = HttpPipelineBuilder.Build(new CosmosPipelineClientOptions(requestHandler));
        }

        internal override CosmosClient Client { get; }

        internal override DocumentClient DocumentClient { get; }

        internal override IDocumentQueryClient DocumentQueryClient { get; }

        internal override CosmosSerializer CosmosSerializer { get; }

        internal override CosmosSerializer PropertiesSerializer { get; }

        internal override CosmosSerializer SqlQuerySpecSerializer { get; }

        internal override CosmosResponseFactory ResponseFactory { get; }

        internal override RequestInvokerHandler RequestHandler { get; }

        internal override CosmosClientOptions ClientOptions { get; }

        internal override Uri CreateLink(
            string parentLink,
            string uriPathSegment,
            string id)
        {
            int parentLinkLength = parentLink?.Length ?? 0;
            string idUriEscaped = Uri.EscapeUriString(id);

            StringBuilder stringBuilder = new StringBuilder(parentLinkLength + 2 + uriPathSegment.Length + idUriEscaped.Length);
            if (parentLinkLength > 0)
            {
                stringBuilder.Append(parentLink);
                stringBuilder.Append("/");
            }

            stringBuilder.Append(uriPathSegment);
            stringBuilder.Append("/");
            stringBuilder.Append(idUriEscaped);
            return new Uri(stringBuilder.ToString(), UriKind.Relative);
        }

        internal override Task<T> ProcessResourceOperationAsync<T>(Uri resourceUri, ResourceType resourceType, OperationType operationType, RequestOptions requestOptions, ContainerCore cosmosContainerCore, PartitionKey? partitionKey, Stream streamPayload, Action<RequestMessage> requestEnricher, Func<ResponseMessage, T> responseCreator, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        internal override async Task<ResponseMessage> ProcessResourceOperationStreamAsync(Uri resourceUri, ResourceType resourceType, OperationType operationType, RequestOptions requestOptions, ContainerCore cosmosContainerCore, PartitionKey? partitionKey, Stream streamPayload, Action<RequestMessage> requestEnricher, CancellationToken cancellationToken)
        {
            RequestMessage requestMessage = this.RequestHandler.CreateRequestMessage(resourceUri, resourceType, operationType, requestOptions, cosmosContainerCore, partitionKey, streamPayload, requestEnricher);
            // Should populate/generate in some smart way
            requestMessage.ClientRequestId = Guid.NewGuid().ToString();
            // Should just return the Response, but it would involve changing the API in many places
            global::Azure.Response response = await this.pipeline.SendRequestAsync(requestMessage, cancellationToken);
            return response as ResponseMessage;
        }

        internal override void ValidateResource(string id)
        {
            this.DocumentClient.ValidateResource(id);
        }
    }
}
