using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace Cyrus.ApiClient
{
    internal interface IApiCaller
    {
        Task<IRestResponse> ExecuteAsync(IApiCallerContext context, CancellationToken cancellationToken = default);
    }
}