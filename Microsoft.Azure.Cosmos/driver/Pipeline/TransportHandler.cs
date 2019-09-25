//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.Handlers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;

    internal class TransportHandler
    {
        private readonly IAuthorizationTokenProvider authorizationTokenProvider;
        private readonly Func<DocumentServiceRequest, IStoreModel> getStoreModel;
        private readonly Action<DocumentServiceRequest, DocumentServiceResponse> captureSession;

        public TransportHandler(
            IAuthorizationTokenProvider authorizationTokenProvider,
            Func<DocumentServiceRequest, IStoreModel> getStoreModel,
            Action<DocumentServiceRequest, DocumentServiceResponse> captureSession)
        {
            if (authorizationTokenProvider == null)
            {
                throw new ArgumentNullException(nameof(authorizationTokenProvider));
            }

            if (getStoreModel == null)
            {
                throw new ArgumentNullException(nameof(getStoreModel));
            }

            if (captureSession == null)
            {
                throw new ArgumentNullException(nameof(captureSession));
            }

            this.authorizationTokenProvider = authorizationTokenProvider;
            this.getStoreModel = getStoreModel;
            this.captureSession = captureSession;
        }

        public async Task<DocumentServiceResponse> SendAsync(
            string verb,
            DocumentServiceRequest request,
            CancellationToken cancellationToken)
        {
            using (new ActivityScope(Guid.NewGuid()))
            {
                return await this.ProcessMessageAsync(verb, request, cancellationToken);
            }
        }

        internal Task<DocumentServiceResponse> ProcessMessageAsync(
            string verb,
            DocumentServiceRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            //TODO: extrace auth into a separate handler
            string authorization = this.authorizationTokenProvider.GetUserAuthorizationToken(
                request.ResourceAddress,
                PathsHelper.GetResourcePath(request.ResourceType),
                verb,
                request.Headers,
                AuthorizationTokenType.PrimaryMasterKey,
                payload: out _);

            request.Headers[HttpConstants.HttpHeaders.Authorization] = authorization;

            IStoreModel storeProxy = this.getStoreModel(request);
            if (request.OperationType == OperationType.Upsert)
            {
                return this.ProcessUpsertAsync(storeProxy, request, cancellationToken);
            }
            else
            {
                return storeProxy.ProcessMessageAsync(request, cancellationToken);
            }
        }

        private async Task<DocumentServiceResponse> ProcessUpsertAsync(IStoreModel storeProxy, DocumentServiceRequest serviceRequest, CancellationToken cancellationToken)
        {
            DocumentServiceResponse response = await storeProxy.ProcessMessageAsync(serviceRequest, cancellationToken);
            this.captureSession(serviceRequest, response);
            return response;
        }
    }
}
