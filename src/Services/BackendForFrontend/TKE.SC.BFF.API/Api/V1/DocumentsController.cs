using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TKE.SC.BFF.Helper;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TKE.SC.BFF.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
	{
        #region Variables

        private readonly IConfigure _configure;
        private readonly ILogHistory _loghistory;
        private readonly IDocument _document;
        #endregion
        public DocumentsController( IConfigure configure, ILogHistory logHistory, ILogger<DocumentsController> logger, IDocument document)
        {
            _configure = configure;
            _loghistory = logHistory;
            _document = document;
            Utility.SetLogger(logger);
        }
        /// <summary>
        /// Generate Orderform Document
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns>Stream of PDF content</returns>
        [Route("orderform/unit/{unitId}")]
        [HttpPost]
        public async Task<IActionResult> GetDataForDocumentGeneration([FromRoute] int unitId,[FromQuery]int setId)
        {

            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _document.GetDataForDocumentGeneration(setId, unitId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return new FileStreamResult(response.Content.ReadAsStreamAsync().Result,"application/pdf");
        }

        /// <summary>
        /// Get M-File Vault/Documents Folder Location
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Route("vault/{projectId}")]
        [HttpGet]
        public async Task<IActionResult> GetVaultLocation([FromRoute]string projectId)
        {
            var methodBegin = Utility.LogBegin();
            var response = await _document.GetVaultLocation(projectId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response);
        }

    }
}
