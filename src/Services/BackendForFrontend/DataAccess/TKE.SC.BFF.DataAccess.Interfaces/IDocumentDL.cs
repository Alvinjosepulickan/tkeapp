using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
	public interface IDocumentDL
	{
		Task<HttpResponseMessage> SendRequestToDocumentGenerator(JObject documentGenerationrequestBody);
	}
}
