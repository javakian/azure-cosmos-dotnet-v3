//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using global::Azure.Core.Pipeline;
    using Microsoft.Azure.Cosmos.Handlers;

    internal sealed class CosmosPipelineClientOptions : ClientOptions
    {
        public CosmosPipelineClientOptions(RequestInvokerHandler requestInvokerHandler)
        {
            this.Transport = new CosmosPipelineTransport(requestInvokerHandler);
        }
    }
}
