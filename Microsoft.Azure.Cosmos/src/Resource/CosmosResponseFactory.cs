//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::Azure.Core;
    using Microsoft.Azure.Cosmos.Scripts;

    internal class CosmosResponseFactory
    {
        /// <summary>
        /// Cosmos JSON converter. This allows custom JSON parsers.
        /// </summary>
        private readonly CosmosSerializer cosmosSerializer;

        /// <summary>
        /// This is used for all meta data types
        /// </summary>
        private readonly CosmosSerializer propertiesSerializer;

        internal CosmosResponseFactory(
            CosmosSerializer defaultJsonSerializer,
            CosmosSerializer userJsonSerializer)
        {
            this.propertiesSerializer = defaultJsonSerializer;
            this.cosmosSerializer = userJsonSerializer;
        }

        internal FeedResponse<T> CreateQueryFeedResponse<T>(
            ResponseMessage cosmosResponseMessage)
        {
            //Throw the exception
            cosmosResponseMessage.EnsureSuccessStatusCode();

            QueryResponse queryResponse = cosmosResponseMessage as QueryResponse;
            if (queryResponse != null)
            {
                return QueryResponse<T>.CreateResponse<T>(
                    cosmosQueryResponse: queryResponse,
                    jsonSerializer: this.cosmosSerializer);
            }

            return ReadFeedResponse<T>.CreateResponse<T>(
                       cosmosResponseMessage,
                       this.cosmosSerializer);
        }

        internal IReadOnlyList<T> CreateQueryPageResponse<T>(
            ResponseMessage cosmosResponseMessage)
        {
            //Throw the exception
            cosmosResponseMessage.EnsureSuccessStatusCode();

            using (cosmosResponseMessage)
            {
                IReadOnlyList<T> resources = default(IReadOnlyList<T>);
                if (cosmosResponseMessage.Content != null)
                {
                    CosmosFeedResponseUtil<T> response = this.cosmosSerializer.FromStream<CosmosFeedResponseUtil<T>>(cosmosResponseMessage.Content);
                    resources = response.Data;
                }

                return resources;
            }
        }

        internal Task<ItemResponse<T>> CreateItemResponseAsync<T>(
            Task<ResponseMessage> cosmosResponseMessageTask)
        {
            return this.ProcessMessageAsync(cosmosResponseMessageTask, (cosmosResponseMessage) =>
            {
                T item = this.ToObjectInternal<T>(cosmosResponseMessage, this.cosmosSerializer);
                return new ItemResponse<T>(
                    cosmosResponseMessage.StatusCode,
                    cosmosResponseMessage.CosmosHeaders,
                    item,
                    cosmosResponseMessage.Diagnostics);
            });
        }

        internal Task<global::Azure.Response<T>> CreateItemResponseAsync<T>(
            Task<global::Azure.Response> cosmosResponseMessageTask)
        {
            return this.ProcessMessageAsync(cosmosResponseMessageTask, (cosmosResponseMessage) =>
            {
                T item = this.ToObjectInternal<T>(cosmosResponseMessage, this.cosmosSerializer);
                return new global::Azure.Response<T>(cosmosResponseMessage, item);
            });
        }

        internal Task<ContainerResponse> CreateContainerResponseAsync(
            Container container,
            Task<ResponseMessage> cosmosResponseMessageTask)
        {
            return this.ProcessMessageAsync(cosmosResponseMessageTask, (cosmosResponseMessage) =>
            {
                ContainerProperties containerProperties = this.ToObjectInternal<ContainerProperties>(cosmosResponseMessage, this.propertiesSerializer);
                return new ContainerResponse(
                    cosmosResponseMessage.StatusCode,
                    cosmosResponseMessage.CosmosHeaders,
                    containerProperties,
                    container);
            });
        }

        internal Task<UserResponse> CreateUserResponseAsync(
            User user,
            Task<ResponseMessage> cosmosResponseMessageTask)
        {
            return this.ProcessMessageAsync(cosmosResponseMessageTask, (cosmosResponseMessage) =>
            {
                UserProperties userProperties = this.ToObjectInternal<UserProperties>(cosmosResponseMessage, this.propertiesSerializer);
                return new UserResponse(
                    cosmosResponseMessage.StatusCode,
                    cosmosResponseMessage.CosmosHeaders,
                    userProperties,
                    user);
            });
        }

        internal Task<PermissionResponse> CreatePermissionResponseAsync(
            Permission permission,
            Task<ResponseMessage> cosmosResponseMessageTask)
        {
            return this.ProcessMessageAsync(cosmosResponseMessageTask, (cosmosResponseMessage) =>
            {
                PermissionProperties permissionProperties = this.ToObjectInternal<PermissionProperties>(cosmosResponseMessage, this.propertiesSerializer);
                return new PermissionResponse(
                    cosmosResponseMessage.StatusCode,
                    cosmosResponseMessage.CosmosHeaders,
                    permissionProperties,
                    permission);
            });
        }

        internal Task<DatabaseResponse> CreateDatabaseResponseAsync(
            Database database,
            Task<ResponseMessage> cosmosResponseMessageTask)
        {
            return this.ProcessMessageAsync(cosmosResponseMessageTask, (cosmosResponseMessage) =>
            {
                DatabaseProperties databaseProperties = this.ToObjectInternal<DatabaseProperties>(cosmosResponseMessage, this.propertiesSerializer);
                return new DatabaseResponse(
                    cosmosResponseMessage.StatusCode,
                    cosmosResponseMessage.CosmosHeaders,
                    databaseProperties,
                    database);
            });
        }

        internal Task<ThroughputResponse> CreateThroughputResponseAsync(
            Task<ResponseMessage> cosmosResponseMessageTask)
        {
            return this.ProcessMessageAsync(cosmosResponseMessageTask, (cosmosResponseMessage) =>
            {
                ThroughputProperties throughputProperties = this.ToObjectInternal<ThroughputProperties>(cosmosResponseMessage, this.propertiesSerializer);
                return new ThroughputResponse(
                    cosmosResponseMessage.StatusCode,
                    cosmosResponseMessage.CosmosHeaders,
                    throughputProperties);
            });
        }

        internal Task<StoredProcedureExecuteResponse<T>> CreateStoredProcedureExecuteResponseAsync<T>(Task<ResponseMessage> cosmosResponseMessageTask)
        {
            return this.ProcessMessageAsync(cosmosResponseMessageTask, (cosmosResponseMessage) =>
            {
                T item = this.ToObjectInternal<T>(cosmosResponseMessage, this.cosmosSerializer);
                return new StoredProcedureExecuteResponse<T>(
                    cosmosResponseMessage.StatusCode,
                    cosmosResponseMessage.CosmosHeaders,
                    item);
            });
        }

        internal Task<StoredProcedureResponse> CreateStoredProcedureResponseAsync(Task<ResponseMessage> cosmosResponseMessageTask)
        {
            return this.ProcessMessageAsync(cosmosResponseMessageTask, (cosmosResponseMessage) =>
            {
                StoredProcedureProperties cosmosStoredProcedure = this.ToObjectInternal<StoredProcedureProperties>(cosmosResponseMessage, this.propertiesSerializer);
                return new StoredProcedureResponse(
                    cosmosResponseMessage.StatusCode,
                    cosmosResponseMessage.CosmosHeaders,
                    cosmosStoredProcedure);
            });
        }

        internal Task<TriggerResponse> CreateTriggerResponseAsync(Task<ResponseMessage> cosmosResponseMessageTask)
        {
            return this.ProcessMessageAsync(cosmosResponseMessageTask, (cosmosResponseMessage) =>
            {
                TriggerProperties triggerProperties = this.ToObjectInternal<TriggerProperties>(cosmosResponseMessage, this.propertiesSerializer);
                return new TriggerResponse(
                    cosmosResponseMessage.StatusCode,
                    cosmosResponseMessage.CosmosHeaders,
                    triggerProperties);
            });
        }

        internal Task<UserDefinedFunctionResponse> CreateUserDefinedFunctionResponseAsync(Task<ResponseMessage> cosmosResponseMessageTask)
        {
            return this.ProcessMessageAsync(cosmosResponseMessageTask, (cosmosResponseMessage) =>
            {
                UserDefinedFunctionProperties settings = this.ToObjectInternal<UserDefinedFunctionProperties>(cosmosResponseMessage, this.propertiesSerializer);
                return new UserDefinedFunctionResponse(
                    cosmosResponseMessage.StatusCode,
                    cosmosResponseMessage.CosmosHeaders,
                    settings);
            });
        }

        internal async Task<T> ProcessMessageAsync<T>(Task<ResponseMessage> cosmosResponseTask, Func<ResponseMessage, T> createResponse)
        {
            using (ResponseMessage message = await cosmosResponseTask)
            {
                return createResponse(message);
            }
        }

        internal async Task<T> ProcessMessageAsync<T>(Task<global::Azure.Response> cosmosResponseTask, Func<global::Azure.Response, T> createResponse)
        {
            using (global::Azure.Response message = await cosmosResponseTask)
            {
                return createResponse(message);
            }
        }

        internal T ToObjectInternal<T>(ResponseMessage cosmosResponseMessage, CosmosSerializer jsonSerializer)
        {
            //Throw the exception
            cosmosResponseMessage.EnsureSuccessStatusCode();

            if (cosmosResponseMessage.Content == null)
            {
                return default(T);
            }

            return jsonSerializer.FromStream<T>(cosmosResponseMessage.Content);
        }

        internal T ToObjectInternal<T>(global::Azure.Response response, CosmosSerializer jsonSerializer)
        {
            //Throw the exception
            //TODO : Add helper?
            if (response.Status < 200 || response.Status >= 300)
            {
                string message = $"Response status code does not indicate success: {response.Status} Reason: ({response.ReasonPhrase}).";

                throw new CosmosException(
                        response,
                        message);
            }

            if (response.ContentStream == null)
            {
                return default(T);
            }

            return jsonSerializer.FromStream<T>(response.ContentStream);
        }
    }
}