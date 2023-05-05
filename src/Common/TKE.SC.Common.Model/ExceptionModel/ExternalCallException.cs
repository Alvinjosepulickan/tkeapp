using System;
using System.Net.Http;
using TKE.SC.Common.Model;

namespace TKE.SC.Common.Model.ExceptionModel
{
    /// <summary>
    /// ExternalCallException
    /// </summary>
    public class ExternalCallException : Exception
    {
        /// <summary>
        /// Exception Message
        /// </summary>
        public string ExceptionMessage { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Http Response Message
        /// </summary>
        public HttpResponseMessage HttpResponseMessage { get; set; }
        /// <summary>
        /// Http Request Message
        /// </summary>
        public HttpRequestMessage HttpRequestMessage { get; set; }

        public ExternalCallException(ResponseMessage responseMessage)
        {
            Description = responseMessage.Description;
            ExceptionMessage = responseMessage.Message;
            HttpResponseMessage = responseMessage.HttpResponseMessage;
            HttpRequestMessage = responseMessage.HttpRequestMessage;
        }
    }
}
