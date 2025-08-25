using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignInApi.utils.hateaos;

namespace SignInApi.utils.responses
{
    public class ResponseBody<T>: Resource
    {
        public string Message { get; set; } = string.Empty;
        public string? Url { get; set; }
        public T? Body { get; set; }
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public DateTimeOffset Timestamp { get; set; }

    }
}