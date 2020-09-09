using System.Collections.Generic;
using Cyrus.ApiClient.ApiCallers;

namespace Cyrus.ApiClient.ApiCallerContexts
{
    internal class TestApiCallerContext : ApiCallerContext
    {
        protected override IEnumerable<IApiCaller> GetApiCallers()
        {
            yield return new TestApiCaller();
        }
    }
}
