using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.Common.Model;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
	public interface IDocument
	{
		Task<HttpResponseMessage> GetDataForDocumentGeneration(int setId,int unitId, string sessionId);
		Task<string> GetVaultLocation(string projectId);

	}
}
