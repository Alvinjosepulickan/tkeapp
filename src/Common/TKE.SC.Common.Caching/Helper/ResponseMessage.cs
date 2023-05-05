using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http;

namespace TKE.SC.Common.Caching.Helper
{
    public class ResponseMessage
    {
        /// <summary>
        /// Name of the StatusCode
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// Name of the ErrorCode
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// Name of the Message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Response object
        /// </summary>
        public JObject Response { get; set; }
        /// <summary>
        /// Request Uri
        /// </summary>
        public string RequestUri { get; set; }
        /// <summary>
        /// Http Response Message
        /// </summary>
        public HttpResponseMessage HttpResponseMessage { get; set; }
        /// <summary>
        /// Http Request Message
        /// </summary>
        public HttpRequestMessage HttpRequestMessage { get; set; }
    }
}
