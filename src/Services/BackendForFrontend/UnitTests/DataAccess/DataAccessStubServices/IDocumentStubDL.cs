using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Interfaces;

namespace TKE.SC.BFF.UnitTests.DataAccess.DataAccessStubServices
{
    class IDocumentStubDL : IDocumentDL
    {
        public async Task<HttpResponseMessage> SendRequestToDocumentGenerator(JObject documentGenerationrequestBody)
        {
            return new HttpResponseMessage {StatusCode= new System.Net.HttpStatusCode { } };
        }
    }
}
