using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using TKE.SC.Common.Model;


namespace TKE.SC.Common.Model.ExceptionModel
{
    /// <summary>
    /// CustomException
    /// </summary>
    public class CustomException : Exception
    {
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// ExceptionMessage
        /// </summary>
        public string ExceptionMessage { get; set; }
        /// <summary>
        /// ExceptionDescription
        /// </summary>
        public string ExceptionDescription { get; set; }
        /// <summary>
        /// Http Resposne Message
        /// </summary>
        public HttpResponseMessage HttpResponseMessage { get; set; }
        /// <summary>
        /// Http Request Message
        /// </summary>
        public HttpRequestMessage HttpRequestMessage { get; set; }
        /// <summary>
        /// ResonseArray
        /// </summary>
        public JArray ResponseArray { get; set; }
        /// <summary>
        /// CustomException
        /// </summary>
        /// <param Name="responseMessage"></param>
        public CustomException(ResponseMessage responseMessage)
        {
            Code = responseMessage.StatusCode.ToString();
            ExceptionMessage = responseMessage.Message;
            ExceptionDescription = responseMessage?.Description;
            HttpResponseMessage = responseMessage.HttpResponseMessage;
            HttpRequestMessage = responseMessage.HttpRequestMessage;
            ResponseArray = responseMessage.ResponseArray;
        }
    }
}
