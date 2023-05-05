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
    public class DesignAutomationController : Controller
    {
        #region Variables
        private readonly IDesignAutomation _daconfiguration;
        private readonly IConfigure _configure;
        #endregion
        public DesignAutomationController(IDesignAutomation daconfiguration, IConfigure configure, Microsoft.Extensions.Logging.ILogger<ObomController> logger)
        {
            _daconfiguration = daconfiguration;
            _configure = configure;
            Utility.SetLogger(logger);
        }
        /// <summary>
        /// API to get the DA Response
        /// </summary>
        /// <param name="configurationDetails"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDAResponse([FromBody] ConfigurationDetails configurationDetails)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _daconfiguration.GetDAResponse(configurationDetails, sessionId).ConfigureAwait(true);
            Utility.LogEnd(methodBeginTime);
            return Ok(response.Response);
        }
        /// <summary>
        /// API to Get the DA Status
        /// </summary>
        /// <param name="configurationDetails"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetStatus")]
        public async Task<IActionResult> GetDAStatus([FromBody] ConfigurationDetails configurationDetails)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _daconfiguration.GetDAStatus(configurationDetails, sessionId).ConfigureAwait(true);
            Utility.LogEnd(methodBeginTime);
            return Ok(response.Response);
        }
    }
}
