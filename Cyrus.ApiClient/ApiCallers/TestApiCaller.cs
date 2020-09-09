using System.Threading.Tasks;
using RestSharp;

namespace Cyrus.ApiClient.ApiCallers
{
    internal class TestApiCaller : ApiCaller
    {
        protected override void PostResponse(IRestResponse restResponse, IApiCallerContext context)
        {
            // var result = JObject.Parse(restResponse.Content);
            // context["name"] = result.Value<string>("name");
        }

        protected override void PreRequest(IRestRequest restRequest, IApiCallerContext context)
        {
            restRequest.Method = Method.GET;
            restRequest.Resource = "order/{id}";
            restRequest.AddUrlSegment("id", 1);
        }

        protected override Task Authenticate(IRestClient restClient)
        {
            return base.Authenticate(restClient);
        }
    }
}
