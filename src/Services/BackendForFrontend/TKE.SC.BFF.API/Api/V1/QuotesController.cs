using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.Helper;

namespace TTKE.SC.BFF.Api.V1
{

    [Microsoft.AspNetCore.Authorization.Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {

        #region Variables
        private readonly IBuildingConfiguration _buildingConfiguration;
        private readonly IFieldDrawingAutomation _fieldDrawingAutomation;
        private readonly IProject _project;
        private readonly IOzBL _oz;
        private readonly IReleaseInfo _releaseInfo;
        #endregion

        public QuotesController(IBuildingConfiguration building, IProject project, IOzBL oz, IFieldDrawingAutomation fieldDrawingAutomation, Microsoft.Extensions.Logging.ILogger<QuotesController> logger, IReleaseInfo releaseInfo)
        {
            _buildingConfiguration = building;
            _fieldDrawingAutomation = fieldDrawingAutomation;
            _project = project;
            _oz = oz;
            _releaseInfo = releaseInfo;
            Utility.SetLogger(logger);
        }
    
   
        /// <summary>
        /// Get List of configurations for a quoteId
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <returns></returns>
        [Route("{quoteId}/configurations")]
        [HttpGet]
        public async Task<IActionResult> GetListOfConfigurationForProject(string quoteId)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var UserRespObj = await _buildingConfiguration.GetListOfConfigurationForProject(quoteId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(UserRespObj.Response);
        }

        /// <summary>
        /// QuickConfigurationSummary
        /// </summary>
        /// <param Name="type"></param>
        /// <param Name="Id"></param>
        /// <returns></returns>
        [Route("quicksummary/{type}/{Id}")]
        [HttpGet]
        public async Task<IActionResult> QuickConfigurationSummary(string type, string id)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            var UserRespObj = await _buildingConfiguration.QuickConfigurationSummary(type, id, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(UserRespObj.Response);
        }


        /// <summary>
		///  This is the initial method for GetFieldDrawingsByProjectId
		/// </summary>
		/// <param Name="projectId"></param>
		/// <returns></returns>
		[Route("{quoteId}/drawings")]
        [HttpGet]
        public async Task<IActionResult> GetFieldDrawingsByProjectId([FromRoute] string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _fieldDrawingAutomation.GetFieldDrawingsByProjectId(quoteId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(response.Response);
        }


        /// <summary>
        /// Method for saving send to coordination data
        /// </summary>
        /// <param Name="sendToCoordinationObj"></param>
        /// <param Name="quoteId"></param>
        /// <returns></returns>
        [Route("{quoteId}/coordination/questionnaire"), HttpPost]
        public async Task<IActionResult> SaveSendToCoordination([FromBody] Newtonsoft.Json.Linq.JObject sendToCoordinationObj, string quoteId = null)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var startGrpResponseObj = await _fieldDrawingAutomation.SaveSendToCoordination(quoteId, sendToCoordinationObj, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(startGrpResponseObj.ResponseArray);
        }

        /// <summary>
        /// This method is to get the group details and its Coordination questions
        /// </summary>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        [Route("{quoteId}/coordination/questionnaire")]
        [HttpGet]
        public async Task<IActionResult> GetSendToCoordinationByProjectId([FromRoute] string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            var response = await _fieldDrawingAutomation.GetSendToCoordinationByProjectId(quoteId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(response.Response);
        }

        /// <summary>
        /// Check for enabling the send to coordination button
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        [Route("{quoteId}/coordination/status")]
        [HttpGet]
        public async Task<IActionResult> GetSendToCoordinationStatus([FromRoute] string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            var response = await _fieldDrawingAutomation.GetSendToCoordinationStatus(quoteId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(response.Response);
        }

        /// <summary>
        /// This method saves configuration to an external system
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <returns></returns>
        [Route("{quoteId}/view"), HttpPost]
        public async Task<IActionResult> SaveConfigurationToExternalSystem([FromRoute]string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Convert.ToString(Request.Headers[Constant.SESSIONID]);
            var projDetails = await _project.SaveConfigurationToView(quoteId, string.Empty, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(projDetails);
        }

        
        /// <summary>
        /// This method will generate the configuration request for VIEW
        /// </summary>
        /// <param Name="OpportunityId"></param>
        /// <returns></returns>
        [Route("{quoteId}/view"), HttpGet]
        public async Task<IActionResult> GenerateViewConfigurationRequest([FromRoute]string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            // var sessionId = Convert.ToString(Request.Headers[Constant.SESSIONID]);
            var projDetails = await _project.SaveProjectInfo1(quoteId, string.Empty, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(projDetails.Response);
        }

        /// <summary>
        ///  Booking Request Payload Generation for OZ API
        /// </summary>
        /// <param Name="quoteId"></param>
        [Route("{quoteId}/coordination/request"), HttpGet]
        public async Task<IActionResult> GenerateOzRequestBody(string quoteId)
        {
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var requestBodyObj = await _oz.GenerateBookingRequestPayload(quoteId, sessionId).ConfigureAwait(false);

            Utility.LogEnd(methodBeginTime);
            return Ok(requestBodyObj);
        }

        [Route("{quoteId}/coordination"), HttpPost]
        public async Task<IActionResult> BookingRequest([FromRoute] string quoteId)
        {
            var sessionId = Utility.GetSessionId(User);
            var methodBeginTime = Utility.LogBegin();
            var bookingRequestObj = await _oz.BookingRequest(quoteId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(bookingRequestObj);
        }

        [Route("{quoteId}/release")]
        [HttpGet]
        public async Task<IActionResult> GetReleaseInfo([FromRoute]string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _releaseInfo.GetProjectReleaseInfo(quoteId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(response.Response);
        }

        /// <summary>
        /// API to get Unit configuration datapoints for the release info popup
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        [Route("{quoteId}/release/group/{groupId}")]
        [HttpGet]
        public async Task<IActionResult> GetReleaseToManufactureByGroupId([FromRoute]int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _releaseInfo.GetGroupReleaseInfo(groupId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(response.Response);
        }

        /// <summary>
        /// API to save the release to manufacturing popup details and change the group status to released if conditions satisfies.
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="variableAssignments"></param>
        /// <returns></returns>
        [Route("{quoteId}/release/group/{groupId}")]
        [HttpPost]
        public async Task<IActionResult> SaveUpdatReleaseInfoDetails([FromRoute] int groupId, [FromBody] JObject variableAssignments)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _releaseInfo.SaveUpdatReleaseInfoDetails(groupId, variableAssignments, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(response.ResponseArray);
        }
    }
}
