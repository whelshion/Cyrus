using System;
using System.Threading.Tasks;
using Cyrus.ApiClient.ApiCallerContexts;

namespace Cyrus.ApiClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IApiCallerContext context = new TestApiCallerContext();
            await context.RunAsync();
            await context.LogAsync();
            Console.ReadKey();
        }
    }
}
