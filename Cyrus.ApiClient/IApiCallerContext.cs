using System.Threading;
using System.Threading.Tasks;

namespace Cyrus.ApiClient
{
    internal interface IApiCallerContext
    {
        object this[string key] { get; set; }

        Task LogAsync();

        Task RunAsync(CancellationToken cancellationToken = default);

        T Value<T>(string key);
    }
}