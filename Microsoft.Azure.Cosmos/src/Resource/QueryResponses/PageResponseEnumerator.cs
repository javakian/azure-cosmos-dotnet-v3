// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ------------------------------------------------------------

namespace Microsoft.Azure.Cosmos
{
    using System;
    using System.Collections.Generic;
    
    using System.Threading.Tasks;
    using global::Azure;

    internal static class PageResponseEnumerator
    {
        public static IEnumerable<global::Azure.Response<T>> CreateEnumerable<T>(Func<string, Page<T>> pageFunc)
        {
            string nextLink = null;
            do
            {
                Page<T> pageResponse = pageFunc(nextLink);
                foreach (T item in pageResponse.Values)
                {
                    yield return new global::Azure.Response<T>(pageResponse.GetRawResponse(), item);
                }
                nextLink = pageResponse.ContinuationToken;
            }
            while (nextLink != null);
        }

        public static AsyncCollection<T> CreateAsyncEnumerable<T>(Func<string, Task<Page<T>>> pageFunc)
        {
            return new FuncAsyncCollection<T>((continuationToken, pageSizeHint) => pageFunc(continuationToken));
        }

        public static AsyncCollection<T> CreateAsyncEnumerable<T>(Func<string, int?, Task<Page<T>>> pageFunc)
        {
            return new FuncAsyncCollection<T>(pageFunc);
        }

        internal class FuncAsyncCollection<T> : AsyncCollection<T>
        {
            private readonly Func<string, int?, Task<Page<T>>> pageFunc;

            public FuncAsyncCollection(Func<string, int?, Task<Page<T>>> pageFunc)
            {
                this.pageFunc = pageFunc;
            }

            public override async IAsyncEnumerable<Page<T>> ByPage(string continuationToken = null, int? pageSizeHint = 0)
            {
                do
                {
                    Page<T> pageResponse = await this.pageFunc(continuationToken, pageSizeHint).ConfigureAwait(false);
                    yield return pageResponse;
                    continuationToken = pageResponse.ContinuationToken;
                }
                while (continuationToken != null);
            }
        }
    }
}
