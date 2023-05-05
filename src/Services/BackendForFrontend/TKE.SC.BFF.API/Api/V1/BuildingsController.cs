using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.Helper;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.Api.V1
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BuildingsController : ControllerBase
    {
        #region Variables
        private readonly IBuildingConfiguration _buildingConfiguration;
        private readonly IBuildingEquipment _buildingEquipment;
        private readonly IConfigure _configure;
        int buildingId = 0;

        #endregion

        /// <summary>
        /// BuildingConfigurationController
        /// </summary>
        /// <param Name="building"></param>
        /// <param Name="configure"></param>
        /// <param Name="logger"></param>
        public BuildingsController(IBuildingConfiguration building, IBuildingEquipment buildingEquipment, IConfigure configure, Microsoft.Extensions.Logging.ILogger<BuildingsController> logger)
        {
            _buildingConfiguration = building;
            _buildingEquipment = buildingEquipment;
            _configure = configure;
            Utility.SetLogger(logger);
        }

        /// <summary>
        /// SaveBuildingConfigurationForProject
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <param Name="buildingConfiguration"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveBuildingConfigurationForProject( [FromBody] BuildingConfiguration buildingConfiguration)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            string quoteId = buildingConfiguration.QuoteId;
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            Utility.LogTrace(Request.Host.ToString() + Request.Path.ToString());
            ResponseMessage response;
            if (string.IsNullOrEmpty(Convert.ToString(buildingConfiguration.Operation)))
            {
                buildingConfiguration.Operation = Operation.Save;
            }
            switch (buildingConfiguration.Operation.ToString().ToUpper())
            {
                case Constant.DUPLICATEOPERATION:
                    response = await _buildingConfiguration.DuplicateBuildingConfigurationById(buildingConfiguration.BuildingIDs, quoteId).ConfigureAwait(false);
                    break;
                default:
                    response = await _buildingConfiguration.SaveBuildingConfigurationForProject(buildingConfiguration.BuildingIDs.FirstOrDefault(), sessionId, quoteId, JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(buildingConfiguration))).ConfigureAwait(false);
                    break;
            }
            Utility.LogEnd(methodStartTime);
            return Ok(response.ResponseArray);
        }


        /// <summary>
        /// update Building Configuration
        /// </summary>
        /// <param Name="projectId"></param>
        /// <param Name="BuildingId"></param>
        /// <param Name="variableAssignments"></param>
        /// <returns></returns>
        [HttpPut]   
        //parameters may be taken from json body please modify the parameters in such case
        public async Task<IActionResult> UpdateBuildingConfigurationForProject([FromBody] BuildingConfiguration buildingConfiguration)
        {   
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var BuildingRespObj = await _buildingConfiguration.SaveBuildingConfigurationForProject(buildingConfiguration.BuildingIDs.FirstOrDefault(), sessionId, buildingConfiguration.QuoteId, JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(buildingConfiguration))).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(BuildingRespObj.ResponseArray);
        }

        /// <summary>
        /// Save Building elevation details
        /// </summary>
        /// <param Name="buildingElevation"></param>    
        /// <returns></returns>
        [Route("{buildingId}/elevation")]
        [HttpPost]
        public async Task<IActionResult> SaveBuildingElevation([FromBody] BuildingElevation buildingElevation)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var BuildingElevationRespObj = await _buildingConfiguration.SaveBuildingElevation(buildingElevation).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(BuildingElevationRespObj.ResponseArray);
        }

        /// <summary>
        /// Update the Building Elevation
        /// </summary>
        /// <param Name="buildingElevation"></param>
        /// <returns></returns>
        [Route("{buildingId}/elevation")]
        [HttpPut]
        public async Task<IActionResult> UpdateBuildingElevation([FromBody] BuildingElevation buildingElevation)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var userId = _configure.GetUserId(sessionId);
            buildingElevation.modifiedBy.UserId = userId;
            var BuildingElevationRespObj = await _buildingConfiguration.UpdateBuildingElevation(buildingElevation).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(BuildingElevationRespObj.ResponseArray);
        }

        /// <summary>
        /// Get building elevation details
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        [Route("{buildingId}/elevation")]
        [HttpGet]
        public async Task<IActionResult> GetBuildingElevationById(int buildingId)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var UserRespObj = await _buildingConfiguration.GetBuildingElevationById(buildingId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(UserRespObj.ResponseArray);
        }

        /// <summary>
        /// Delete Building configuration By BuildingId
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        [Route("{buildingId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteBuildingConfigurationById([FromRoute] int buildingId)
        {
            var methodStartTime = Utility.LogBegin();
            string userId = Utility.GetClaim(User, JwtClaimTypes.PreferredUserName);
            var response = await _buildingConfiguration.DeleteBuildingConfigurationById(buildingId, userId).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(response.ResponseArray);
        }

        [HttpGet]
        public async Task<IActionResult> StartBuildingConfigure(string quoteId, int buildingId )
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            Utility.LogTrace(Constant.STARTBUILDINGCONFIGUREURL + Request.Host.ToString() + Request.Path.ToString());
            var startBldgResponseObj = await _buildingConfiguration.StartBuildingConfigure(null, quoteId, buildingId, sessionId, false).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(startBldgResponseObj.Response);
        }

        /// <summary>
        /// Change configure
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <returns></returns>
        [Route("configure")]
        [HttpPost]
        public async Task<IActionResult> ChangeBuildingConfigure([FromBody] JObject variableAssignments)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var (response, startconfigureresponse) = await _configure.ChangeBuildingConfigure(variableAssignments,
              sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(JObject.FromObject(response));
        }

        /// <summary>
        /// Get Building Tabs for getting the isDisabled1
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        [Route("{buildingId}/configuration-sections")]
        [HttpGet]
        public async Task<IActionResult> GetBuildingConfigurationSectionTab(int buildingId)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var UserRespObj = await _buildingConfiguration.GetBuildingConfigurationSectionTab(buildingId).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(UserRespObj.Response);
        }

        /// <summary>
        /// Get/Start API for Building Equipment
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>

        [Route("{buildingId}/equipment"), HttpGet]
        public async Task<IActionResult> StartBuildingEquipmentConfigure([FromRoute] int buildingId)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var startGrpResponseObj = await _buildingEquipment.StartBuildingEquipmentConfigure(new JObject(new JProperty("VariableAssignments", "")), buildingId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(startGrpResponseObj.Response);
        }

        /// <summary>
        /// ChangeBuildingEquipmentConfigure
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>

        [Route("{buildingId}/equipment/configure")]
        [HttpPost]
        public async Task<IActionResult> ChangeBuildingEquipmentConfigure([FromBody] JObject variableAssignments, int buildingId = 0)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            var response = await _configure.ChangeBuildingEquipmentConfigureBl(variableAssignments, sessionId, buildingId).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(JObject.FromObject(response));
        }

        /// <summary>
        /// SaveAssignGroups
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="consoleId"></param>
        /// <param Name="variableAssignments"></param>
        /// <returns></returns>

        [Route("{buildingId}/equipment/consoles")]
        [HttpPost]
        public async Task<IActionResult> SaveAssignGroups([FromRoute] int buildingId, [FromBody] List<BuildingEquipmentData> variableAssignments)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            var response = await _buildingEquipment.SaveAssignGroups(buildingId, variableAssignments, sessionId, Constant.IS_SAVE).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(response.ResponseArray);
        }

        /// <summary>
        /// To Update Assigned Groups
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="consoleId"></param>
        /// <param Name="variableAssignments"></param>
        /// <returns></returns>

        [Route("{buildingId}/equipment/consoles/{consoleId}")]
        [HttpPut]
        public async Task<IActionResult> UpdateAssignGroups([FromRoute] int consoleId, [FromRoute] int buildingId, [FromBody] List<BuildingEquipmentData> variableAssignments)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            var response = await _buildingEquipment.SaveAssignGroups(buildingId, variableAssignments, sessionId, Constant.IS_UPDATE).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(response.ResponseArray);
        }

        /// <summary>
        /// To Duplicate Building Equipment Console
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="consoleId"></param>
        /// <returns></returns>

        [Route("{buildingId}/equipment/consoles/{consoleId}/duplicate")]
        [HttpPost]
        public async Task<IActionResult> DuplicateBuildingEquipmentConsole([FromRoute] int consoleId, [FromRoute] int buildingId)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            var response = await _buildingEquipment.DuplicateBuildingEquipmentConsole(buildingId, consoleId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(response.ResponseArray);
        }


        /// <summary>
        /// API for DeleteBuildingEquipmentConsole 
        /// </summary>
        /// <param Name="consoleId"></param>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>

        [Route("{buildingId}/equipment/consoles/{consoleId}"), HttpDelete]
        public async Task<IActionResult> DeleteBuildingEquipmentConsole(int consoleId, int buildingId)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            var buildingRespObj = await _buildingEquipment.DeleteBuildingEquipmentConsole(buildingId, consoleId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(buildingRespObj.ResponseArray);
        }

        /// <summary>
        /// To Save Building Equipments
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="consoleId"></param>
        /// <param Name="variableAssignments"></param>
        /// <returns></returns>

        [Route("{buildingId}/equipment")]
        [HttpPost]
        public async Task<IActionResult> SaveBuildingEquipmentConfiguration([FromRoute] int buildingId, [FromBody] JObject variableAssignments, [FromQuery] bool saveDraft)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            var response = await _buildingEquipment.SaveBuildingEquipmentConfiguration(buildingId, variableAssignments, sessionId, Constant.IS_SAVE, saveDraft).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(response.ResponseArray);
        }

        /// <summary>
        /// UpdateBuildingEquipment
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="variableAssignments"></param>
        /// <returns></returns>

        [Route("{buildingId}/equipment")]
        [HttpPut]
        public async Task<IActionResult> UpdateBuildingEquipmentConfiguration([FromRoute] int buildingId, [FromBody] JObject variableAssignments)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            var response = await _buildingEquipment.SaveBuildingEquipmentConfiguration(buildingId, variableAssignments, sessionId, Constant.IS_UPDATE, true).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(response.ResponseArray);
        }

        /// <summary>
        /// StartBuildingEquipmentConsole
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="consoleId"></param>
        /// <param Name="variableAssignments"></param>
        /// <returns></returns>

        [Route("{buildingId}/equipment/consoles/{consoleId}"), HttpGet]
        public async Task<IActionResult> StartBuildingEquipmentConsole([FromRoute] int buildingId, [FromRoute] int consoleId)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var startGrpResponseObj = await _buildingEquipment.StartBuildingEquipmentConsole(consoleId, buildingId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(startGrpResponseObj.Response);
        }

        /// <summary>
        /// ChangeBuildingEquipmentConsoleConfigure
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="objUnitHallFixtureConfigurationData"></param>
        /// <returns></returns>
       
        [Route("{buildingId}/equipment/console/configure"), HttpPost]
        public async Task<IActionResult> ChangeBuildingEquipmentConsoleConfigure(int buildingId, [FromBody] BuildingEquipmentData objUnitHallFixtureConfigurationData)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            int consoleId = Convert.ToInt32(objUnitHallFixtureConfigurationData.ConsoleId);
            var startGrpResponseObj = await _buildingEquipment.StartBuildingEquipmentConsole(consoleId, buildingId, sessionId, objUnitHallFixtureConfigurationData).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(startGrpResponseObj.Response);
        }

        [Route("{buildingId}/reset"), HttpGet]
        public async Task<IActionResult> ResetBuildingConfigure([FromRoute] int buildingId)
        {
            var methodStartTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var resetBldgResponseObj = await _buildingConfiguration.StartBuildingConfigure(null, null, buildingId, sessionId, true).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return Ok(resetBldgResponseObj.Response);
        }
    }
}
