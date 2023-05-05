using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.Helper;
using TKE.SC.Common;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.APIController
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ObomController : Controller
    {
        #region Variables
        private readonly IObom _obomconfiguration;
        private readonly IConfigure _configure;
        #endregion
        public ObomController(IObom obomconfiguration, IConfigure configure, Microsoft.Extensions.Logging.ILogger<ObomController> logger)
        {
            _obomconfiguration = obomconfiguration;
            _configure = configure;
            Utility.SetLogger(logger);
        }
        [HttpGet]
        public async Task<IActionResult> GETOBOMResponse([FromBody] ConfigurationDetails configurationDetails)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var UserRespObj = await _obomconfiguration.GETOBOMResponse( configurationDetails, sessionId).ConfigureAwait(true);
            byte[] dataBuffer = Encoding.UTF8.GetBytes(Convert.ToString((UserRespObj.XmlDocument[0])));


            var stream = new MemoryStream(dataBuffer);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(stream)
                //Content = new ByteArrayContent(stream.ToArray())
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = "OBOM.xml"
                };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/xml");

            Utilities.LogEnd(methodBegin);
            return new FileStreamResult(result.Content.ReadAsStreamAsync().Result, "application/xml") { FileDownloadName = "OBOM.xml" };
        }
    }
}