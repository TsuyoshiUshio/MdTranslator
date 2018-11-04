using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MdTranslatorLibrary
{
    public class RestAPICallException : Exception
    {
        public string StatusCode { get; set; }
        public HttpRequestMessage RequestMessage { get; set; }

        public RestAPICallException()
        {
        }

        public RestAPICallException(string message) : base(message)
        {
        }

        public RestAPICallException(string message, Exception inner) : base(message, inner)
        {

        }

        public RestAPICallException(string statusCode, string message, HttpRequestMessage requestMessage) : base(message)
        {
            StatusCode = statusCode;
            RequestMessage = requestMessage;
        }
    }
}
