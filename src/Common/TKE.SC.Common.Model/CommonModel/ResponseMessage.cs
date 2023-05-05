/************************************************************************************************************
************************************************************************************************************
    File Name     :   ResponseMessage.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using System.Collections.Generic;
using System.Net.Http;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace TKE.SC.Common.Model
{
    /// <summary>
    /// ResponseMessage
    /// </summary>
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
        /// request uri for getting error provider
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
        /// <summary>
        /// Error Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// IsSuccessStatusCode
        /// </summary>
        public bool IsSuccessStatusCode { get; set; }
        /// <summary>
        /// IsSuccessStatusCode
        /// </summary>
        public JArray ResponseArray { get; set; }
        /// <summary>
        /// QuoteId
        /// </summary>
        public string QuoteId { get; set; }
        /// <summary>
        /// ReferenceId
        /// </summary>
        public string ReferenceId { get; set; }

        public List<XDocument> XmlDocument { get; set; }
    }
}
