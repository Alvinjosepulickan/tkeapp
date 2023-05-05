using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TKE.SC.Common.Model.HttpClientModel
{
    public class HttpClientRequestModel
    {
        public HTTPMETHODTYPE Method { get; set; }
        public string BaseUrl { get; set; }
        public string EndPoint { get; set; }
        //TODO:Add all the std headers
        public IDictionary<string, string> RequestHeaders { get; set; }
        public JObject RequestBody { get; set; }
        public IDictionary<string, string> BodyToEncode { get; set; }
        public string ContentType { get; set; }
        public string Proxy { get; set; }
        public string RequestString { get; set; }
        public string  PostAs { get; set; }
    }

    public enum HTTPMETHODTYPE
    {
        GET,
        POST,
        PUT,
        DELETE
    }
}
