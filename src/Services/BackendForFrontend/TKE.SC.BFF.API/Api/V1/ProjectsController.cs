using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.Helper;

namespace TKE.SC.BFF.APIController
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        /// <summary>
        /// Variables Collection
        /// </summary>
        #region Variables
        private readonly IProject _project;
        //private readonly IOracleCRMOD _oracleCRMODbl;
        private readonly IConfigure _configure;


        #endregion

        /// <summary>
        /// Constructor for Initializing the Product details Business Logic object
        /// </summary>
        /// <param Name="paramSettings"></param>
        /// <param Name="logger"></param>
        public ProjectsController(IProject project, ILogger<ProjectsController> logger)
        {
            _project = project;
            Utility.SetLogger(logger);
        }


        /// <summary>
        /// This method will get the project details for the entered oppportunityid and versionid
        /// </summary>
        /// <param Name="OpportunityId"></param>
        /// <param Name="versionId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProjectDetails(string projectId, string versionId, int parentVersionId)       
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            if (string.IsNullOrEmpty(projectId))
            {
                var projectList= await _project.GetListOfProjectsDetailsBl(sessionId).ConfigureAwait(false);
                Utility.LogEnd(methodBeginTime);
                return Ok(projectList.Response);
            }
            var projDetails = await _project.GetProjectDetails(projectId, versionId, sessionId, parentVersionId, true).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(projDetails);
        }

      


        /// <summary>
        /// CreateProjects
        /// </summary>
        /// <param Name="variablesValues"></param>
        /// <returns></returns>
        [Route("{projectId}")]
        [HttpPost]
        public async Task<IActionResult> CreateProjects([FromRoute]string projectId)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            var ProjDetailObj = await _project.CreateProjectsBL(sessionId, projectId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(ProjDetailObj.Response);
        }

        /// <summary>
        /// SaveAndUpdateMiniProjects
        /// </summary>
        /// <param Name="variablesValues"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveAndUpdateMiniProjects([FromBody] JObject variablesValues)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            var ProjDetailObj = await _project.SaveAndUpdateMiniProjectsBL(sessionId, variablesValues).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(ProjDetailObj.ResponseArray);
        }

        /// <summary>
        /// This method is used for deleting a project
        /// </summary>
        /// <param Name="projectId"></param>
        /// <param Name="versionId"></param>
        /// <returns></returns>
        [Route("{projectId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProjectById([FromRoute] string projectId, string versionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Convert.ToString(Request.Headers[Constant.SESSIONID]);
            var ProjectRespObj = await _project.DeleteProjectById(projectId, versionId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(ProjectRespObj.ResponseArray);
        }

        /// <summary>
        /// DuplicateQuoteByQuoteId
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        [Route("{projectId}/quote/{quoteId}")]
        [HttpPost]
        public async Task<IActionResult> DuplicateQuoteByQuoteId([FromRoute] string projectId, [FromRoute] string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var projDetails = await _project.DuplicateQuotesByQuoteId(projectId, quoteId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(projDetails.ResponseArray);
        }

        /// <summary>
        /// AddToPrimaryQuotes
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        [Route("primary/{quoteId}")]
        [HttpPut]
        public async Task<IActionResult> AddToPrimaryQuotes([FromRoute] string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var projDetails = await _project.AddToPrimaryQuote(quoteId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(projDetails.Response);
        }

        /// <summary>
        /// AddQuoteForProject
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="versionId"></param>
        /// <returns></returns>
        [Route("{projectId}/version")]
        [HttpPost]
        public async Task<IActionResult> AddQuoteForProject([FromRoute] string projectId)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var projDetails = await _project.AddQuoteForProject(projectId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(projDetails.Response);
        }
    }
}
