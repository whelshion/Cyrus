using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RestSharp;
using RestSharp.Authenticators;

namespace Cyrus.ApiClient
{
    internal abstract class ApiCaller : IApiCaller
    {
        private static string _token;
        private readonly IRestClient _restClient;

        public ApiCaller()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();
            Configuration.Bind(AppSettings.Instance);
            _restClient = new RestClient(AppSettings.Instance.OpenApiOptions.BaseUrl);
        }

        protected IConfigurationRoot Configuration { get; }

        public async Task<IRestResponse> ExecuteAsync(IApiCallerContext context, CancellationToken cancellationToken = default)
        {
            await Authenticate(_restClient);
            var restRequest = new RestRequest();
            PreRequest(restRequest, context);
            var response = await _restClient.ExecuteAsync(restRequest, cancellationToken);
            PostResponse(response, context);
            return response;
        }

        protected virtual void PostResponse(IRestResponse restResponse, IApiCallerContext context)
        {
        }

        protected virtual void PreRequest(IRestRequest restRequest, IApiCallerContext context)
        {
        }

        protected virtual async Task Authenticate(IRestClient restClient)
        {
            if (string.IsNullOrEmpty(_token))
            {
                _token = "access_token";
            }
            restClient.Authenticator = new JwtAuthenticator(_token);
            await Task.CompletedTask;
        }
    }
}