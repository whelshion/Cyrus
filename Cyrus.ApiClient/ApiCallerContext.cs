using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace Cyrus.ApiClient
{
    internal abstract class ApiCallerContext : IApiCallerContext
    {
        private readonly List<CallInfo> _callInfos = new List<CallInfo>();
        private readonly Dictionary<string, object> _context = new Dictionary<string, object>();

        public object this[string key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }

        public virtual async Task LogAsync()
        {
            var directory = $"{Directory.GetCurrentDirectory()}/logs/{DateTime.Today:yyyyMMdd}";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            var filePath = Path.Combine(directory, $"{DateTime.Now.Ticks}_{GetType().Name}.log");
            var logContent = ToLogContent(_context, _callInfos);
            await File.WriteAllTextAsync(filePath, logContent.ToString());
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            foreach (var apiCaller in GetApiCallers())
            {
                var response = await apiCaller.ExecuteAsync(this, cancellationToken);
                _callInfos.Add(new CallInfo
                {
                    Name = apiCaller.GetType().FullName,
                    Request = new ApiRequest
                    {
                        Body = response.Request.Body,
                        Files = response.Request.Files,
                        Method = response.Request.Method,
                        Parameters = response.Request.Parameters,
                        Resource = response.Request.Resource
                    },
                    Response = new ApiResponse
                    {
                        Content = response.Content,
                        ContentLength = response.ContentLength,
                        ErrorMessage = response.ErrorMessage,
                        IsSuccessful = response.IsSuccessful,
                        RawBytes = response.RawBytes,
                        StatusCode = response.StatusCode,
                        StatusDescription = response.StatusDescription
                    }
                });
            }
        }

        public T Value<T>(string key)
        {
            var value = GetValue(key);
            if (value == null)
                return default;
            return (T)value;
        }

        protected abstract IEnumerable<IApiCaller> GetApiCallers();

        protected virtual string ToLogContent(IReadOnlyDictionary<string, object> context, IReadOnlyList<CallInfo> callInfos)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            return new StringBuilder(4000)
                .AppendLine("-- Context --")
                .AppendLine(JsonConvert.SerializeObject(context, Formatting.Indented, jsonSerializerSettings))
                .AppendLine("-- Detail --")
                .AppendLine(JsonConvert.SerializeObject(callInfos, Formatting.Indented, jsonSerializerSettings))
                .ToString();
        }

        private object GetValue(string key)
        {
            _context.TryGetValue(key, out var value);
            return value;
        }

        private void SetValue(string key, object value)
        {
            if (_context.ContainsKey(key))
                _context[key] = value;
            else
                _context.Add(key, value);
        }
    }

    internal class ApiRequest
    {
        public RequestBody Body { get; set; }
        public List<FileParameter> Files { get; set; }
        public Method Method { get; set; }
        public List<Parameter> Parameters { get; set; }
        public string Resource { get; set; }
    }

    internal class ApiResponse
    {
        public string Content { get; set; }
        public string ContentEncoding { get; set; }
        public long ContentLength { get; set; }
        public string ContentType { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccessful { get; set; }
        public byte[] RawBytes { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }
    }

    internal class CallInfo
    {
        public string Name { get; set; }
        public ApiRequest Request { get; set; }
        public ApiResponse Response { get; set; }
    }
}
