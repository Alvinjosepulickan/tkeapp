using Configit.Configurator.Server.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Database;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.HttpClientModel;
using TKE.SC.Common.Model.UIModel;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace TKE.SC.BFF.DataAccess.Services
{
	public class DocumentDL: IDocumentDL
	{
        /// <summary>
        /// configuration object
        /// </summary>
        private IConfiguration _configuration;
        public DocumentDL(ILogger<UnitConfigurationDL> logger, IConfiguration iConfig)
        {
            Utility.SetLogger(logger);
            _configuration = iConfig;
        }
        public async Task<HttpResponseMessage> SendRequestToDocumentGenerator(JObject documentGenerationrequestBody)
        {
            var methodStartTime = Utility.LogBegin();
            var documentGenerationSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constant.DOCUMENTGENERATIONSETTINGS);
            var requestObject = new HttpClientRequestModel()
            {
                Method = HTTPMETHODTYPE.POST,
                BaseUrl = Utility.GetPropertyValue(documentGenerationSettings, Constant.BASEURL),
                EndPoint = Utility.GetPropertyValue(documentGenerationSettings, Constant.DOCUMENTGENERATIONAPIROUTE),
                RequestBody = JObject.FromObject(documentGenerationrequestBody)
            };
            var response = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return response;
        }
    }
}
