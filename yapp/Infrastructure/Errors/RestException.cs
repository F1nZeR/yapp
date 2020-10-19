using System;
using System.Net;

namespace yapp.Infrastructure.Errors
{
    public class RestException : Exception
    {
        public RestException(HttpStatusCode code, string message = null)
            : base(message)
        {
            Code = code;
            // todo: add more info for rest errors
        }

        public HttpStatusCode Code { get; }
    }
}
