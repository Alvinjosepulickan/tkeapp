using System;
using System.Net.Http;

namespace TKE.SC.Common.Caching.Helper
{
    public class CacheException : Exception
    {
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Exception Message
        /// </summary>
        public string ExceptionMessage { get; set; }
        /// <summary>
        /// Http Response Message
        /// </summary>
        public HttpResponseMessage HttpResponseMessage { get; set; }
        /// <summary>
        /// Http Request Message
        /// </summary>
        public HttpRequestMessage HttpRequestMessage { get; set; }
        /// <summary>
        /// Cache Exception
        /// </summary>
        /// <param name="responseMessage"></param>
        public CacheException(ResponseMessage responseMessage)
        {
            Code = responseMessage.StatusCode.ToString();
            ExceptionMessage = responseMessage.Message;
            HttpResponseMessage = responseMessage.HttpResponseMessage;
            HttpRequestMessage = responseMessage.HttpRequestMessage;
        }
    }
}
