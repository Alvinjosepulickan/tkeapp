using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TKE.SC.BFF.Helper;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.BFF.Controllers;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using TKE.SC.Common.Model;
using Newtonsoft.Json;

namespace TKE.SC.BFF.APIController
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        #region Variables
        private readonly IFieldDrawingAutomation _fieldDrawingAutomation;
        private readonly IGroupConfiguration _groupconfiguration;
        private readonly IGroupLayout _groupLayout;
        private readonly IConfigure _configure;
        private readonly IOpeningLocation _openingLocation;
        private readonly IDesignAutomation _daconfiguration;
        #endregion

        public GroupsController(IGroupLayout groupLayout, IFieldDrawingAutomation fieldDrawingAutomation, IGroupConfiguration group, IConfigure configure, IOpeningLocation openingLocation, ILogger<GroupsController> logger, IDesignAutomation daconfiguration)
        {
            _fieldDrawingAutomation = fieldDrawingAutomation;
            _groupconfiguration = group;
            _openingLocation = openingLocation;
            _configure = configure;
            _groupLayout = groupLayout;
            _daconfiguration = daconfiguration;
            Utility.SetLogger(logger);
        }
        /// <summary>
        /// This is the initial method which will return FDA Assigments and Output Types
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        [Route("{groupId}/drawings/layout"), HttpGet]
        public async Task<IActionResult> StartFieldDrawingConfigure([FromRoute] int groupid = 0)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var startGrpResponseObj = await _fieldDrawingAutomation.StartFieldDrawingConfigure(null, groupid, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(startGrpResponseObj.Response);
        }

        /// <summary>
        ///  This is the initial method for ChangeFieldDrawingConfigure
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="groupid"></param>
        /// <returns></returns>
        [Route("{groupId}/drawings/layout/configure"), HttpPost]
        public async Task<IActionResult> ChangeFieldDrawingConfigure([FromBody] JObject variableAssignments = null, int groupid = 0)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var startGrpResponseObj = await _fieldDrawingAutomation.ChangeFieldDrawingConfigure(variableAssignments, groupid, true, sessionId, null, null).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(startGrpResponseObj.Response);
        }

        /// <summary>
        ///  This is the initial method for GetRequestQueueByGroupId
        /// </summary>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        [Route("{groupId}/drawings/history")]
        [HttpGet]
        public async Task<IActionResult> GetRequestQueueByGroupId([FromRoute] int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            var response = await _fieldDrawingAutomation.GetRequestQueueByGroupId(groupId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(response.Response);
        }


        /// <summary>
        /// This is the initial method for SaveFieldDrawingConfigure
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="groupid"></param>
        /// <returns></returns>
        [Route("{groupId}/drawings/layout"), HttpPost]
        public async Task<IActionResult> SaveFieldDrawingConfigure([FromBody] JObject variableAssignments = null, [FromRoute]int groupid = 0)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var saveFdaResponseObj = await _fieldDrawingAutomation.SaveFieldDrawingConfigure(groupid, variableAssignments, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(saveFdaResponseObj.Response);
        }


        /// <summary>
        ///  This is the initial method for GetLockeGroups
        /// </summary>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        [Route("{groupId}/lock"), HttpPut]
        public async Task<IActionResult> UpdateLockedGroupsByProjectId([FromRoute] int groupid, [FromBody] OnlyString islocked)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var islock = islocked.islock;
            var responseUpdated = await _fieldDrawingAutomation.UpdateLockedGroupsByProjectId(sessionId, groupid, islock).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(responseUpdated.ResponseArray);
        }

        /// <summary>
        ///  This is the initial method for Lift Designer
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="groupid"></param>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        [Route("{groupId}/drawings/request"), HttpPost]
        public async Task<IActionResult> RequestFieldDrawingConfigure([FromBody] JObject variableAssignments = null, [FromRoute] int groupid = 0)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var startGrpResponseObj = await _fieldDrawingAutomation.RequestLDDrawings(groupid, variableAssignments, sessionId).ConfigureAwait(false);
            var quoteId = _fieldDrawingAutomation.GetQuoteIdFromCache(sessionId);
            var configurationDetails = new ConfigurationDetails { GroupId = groupid,QuoteId=quoteId };
            await _daconfiguration.GetDAResponse(configurationDetails, sessionId,variableAssignments).ConfigureAwait(true);
            Utility.LogEnd(methodBeginTime);
            return Ok(startGrpResponseObj.ResponseArray);
        }

        [Route("{groupId}/drawings/status"), HttpGet]
        public async Task<IActionResult> RefreshFieldDrawingConfigure([FromRoute]int groupid = 0)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var configurationDetails = new ConfigurationDetails { GroupId = groupid };
            await _daconfiguration.GetDAStatus(configurationDetails, sessionId).ConfigureAwait(false);
            var startGrpResponseObj = await _fieldDrawingAutomation.RefreshFieldDrawingConfigure(groupid, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(startGrpResponseObj.Response);
        }

        /// <summary>
        /// SaveUpdateFieldDrawingStatus
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [Route("/drawings/status"), HttpPut]
        public async Task<IActionResult> SaveUpdateFieldDrawingStatus([FromBody]JObject variableAssignments)
        {
            var methodBeginTime = Utility.LogBegin();
            var startGrpResponseObj = await _fieldDrawingAutomation.SaveUpdateFieldDrawingStatus(variableAssignments).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(startGrpResponseObj.Response);
        }

        /// <summary>
        /// GetGroupConfigurationDetailsByGroupId
        /// </summary>
        /// <param name="groupConfigurationId"></param>
        /// <param name="variableAssignments"></param>
        /// <returns></returns>
        [Route("{groupId}")]      //here section id is the selected tab
        [HttpPost]
        public async Task<IActionResult> GetGroupConfigurationDetailsByGroupId([FromRoute] int groupid, [FromQuery] string selectedTab,
            [FromBody] JObject variableAssignments = null)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            ResponseMessage variableAssignmentObj = new ResponseMessage();
            //var selectedTab = variableAssignments[Constant.SECTIONTABS].ToString();
            if (Utility.CheckEquals(selectedTab, Constant.OPENINGLOCATIONSTAB))
            {
                variableAssignmentObj = await _openingLocation.GetOpeningLocationByGroupId(groupid, sessionId).ConfigureAwait(false);
            }
            else
            {
                //to fix the issue where UI sends null as the string "null"
                if(Utility.CheckEquals(selectedTab, "null"))
                {
                    selectedTab = string.Empty;
                }
                variableAssignmentObj = await _groupconfiguration.GetGroupConfigurationDetailsByGroupId(groupid, variableAssignments, sessionId, selectedTab).ConfigureAwait(false);
            }
            Utility.LogEnd(methodBeginTime);
            return Ok(variableAssignmentObj.Response);
        }

        /// <summary>
        /// SaveGroupConfigurationDetails
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="groupConfiguration"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<IActionResult> SaveGroupConfigurationDetails([FromBody] GroupConfigurationSave groupConfiguration)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            ResponseMessage response;
            if (string.IsNullOrEmpty(groupConfiguration.Operation.ToString()))
            {
                groupConfiguration.Operation = Operation.Save;
            }
            switch (groupConfiguration.Operation.ToString().ToUpper())
            {
                case Constant.DUPLICATEOPERATION:
                    response = await _groupconfiguration.DuplicateGroupConfigurationById(groupConfiguration.GroupIDs, groupConfiguration.buildingID).ConfigureAwait(false);
                    break;
                default:
                    response = await _groupconfiguration.SaveGroupConfiguration(groupConfiguration.buildingID, sessionId, JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(groupConfiguration))).ConfigureAwait(false);
                    break;
            }
            Utility.LogEnd(methodBeginTime);
            return Ok(response.ResponseArray);

        }

        /// <summary>
        /// This method will return      configuration details by BuildingId
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [HttpGet]
        public Task<ResponseMessage> GetGroupConfigurationByBuildingId([FromQuery] string buildingId)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            Utility.LogEnd(methodBeginTime);
            return _groupconfiguration.GetGroupConfigurationByBuildingId(buildingId);
        }

        /// <summary>
        /// UpdateGroupConfigurationDetails
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="variableAssignments"></param>
        /// <returns></returns>
        //[Route("{groupId}​​")]
        [HttpPut]
        public async Task<IActionResult> UpdateGroupConfigurationDetails([FromQuery]int buildingId, [FromQuery]int groupId, [FromBody] JObject variableAssignments)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _groupconfiguration.UpdateGroupConfiguration(buildingId, groupId, variableAssignments).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(response.ResponseArray);
        }

        /// <summary>
        /// This method is used to start configuring a group
        /// </summary>
        /// <param Name="modelNumber"></param>
        /// <param Name="parentCode"></param>
        /// <param Name="BuildingId"></param>
        /// <param Name="groupId"></param>
        /// <param Name="selectedTab"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="locale"></param>
        /// <returns></returns>
        [Route("{groupId}"), HttpGet]
        public async Task<IActionResult> StartGroupConfigure(int buildingId, [FromRoute] int groupId, string selectedTab)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var startGrpResponseObj = await _groupconfiguration.StartGroupConfigure(new JObject(new JProperty("VariableAssignments", "")), buildingId, groupId, sessionId, selectedTab).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(startGrpResponseObj.Response);
        }

        /// <summary>
        /// this method is used to change the configuration of a group
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="groupId"></param>
        /// <param Name="selectedTab"></param>
        /// <returns></returns>
        [Route("{groupId}/configure")]
        [HttpPost]
        public async Task<IActionResult> ChangeGroupConfigure([FromBody] JObject variableAssignments, [FromRoute] int groupId, string selectedTab)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var displayVariableAssignmentsResponse = variableAssignments[Constant.DISPLAYVARIABLEASSIGNMENTS].ToString();
            var response = await _configure.ChangeGroupConfigure(variableAssignments, groupId,
                sessionId, selectedTab, Utility.DeserializeObjectValue<List<DisplayVariableAssignmentsValues>>(displayVariableAssignmentsResponse),true).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(JObject.FromObject(response));
        }

        /// <summary>
        /// This Method is used to delete Group Configuration By GroupId
        /// </summary>
        /// <param Name="GroupId"></param>
        /// <returns></returns>
        [Route("{groupId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteGroupConfiguration([FromRoute] int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var UserRespObj = await _groupconfiguration.DeleteGroupConfiguration(groupId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(UserRespObj.ResponseArray);
        }

        /// <summary>
        /// To Save Group Hall Fixtures
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="groupHallFixturesData"></param>
        /// <returns></returns>
        [Route("{groupId}/grouphall")]
        [HttpPut]
        public async Task<IActionResult> SaveGroupHallFixture([FromRoute] int groupId, [FromQuery] int sectionId, int isSaved, [FromBody] GroupHallFixturesData groupHallFixturesData)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            var response = await _groupconfiguration.SaveGroupHallFixture(groupId, groupHallFixturesData, sessionId, isSaved).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(response.ResponseArray);
        }

        /// <summary>
        /// StartGroupConsole
        /// </summary>
        /// <param Name="consoleId"></param>
        /// <param Name="groupId"></param>
        /// <param Name="variableAssignments"></param>
        /// <returns></returns>
        [Route("{groupId}/grouphall"), HttpGet]
        public async Task<IActionResult> StartGroupConsole([FromQuery] int consoleId, [FromQuery] int sectionId, int groupId, [FromQuery] string fixtureSelected)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            int isChange = 0;
            var startGrpResponseObj = await _groupconfiguration.AddorChangeGroupConsole(consoleId, isChange, groupId, sessionId, fixtureSelected, false).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(startGrpResponseObj.Response);
        }


        /// <summary>
        /// DeleteGroupHallFixtureConfigure
        /// </summary>
        /// <returns></returns>
        [Route("{groupId}/grouphall"), HttpDelete]
        public async Task<IActionResult> DeleteGroupHallFixtureConfigure([FromRoute]int groupId, [FromQuery] string fixtureType, [FromQuery] int consoleId)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var BuildingRespObj = await _groupconfiguration.DeleteGroupHallFixtureConsole(groupId, consoleId, fixtureType, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(BuildingRespObj.ResponseArray);
        }

        /// <summary>
        /// GetStartGroupHallFixture controller
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        [Route("{groupId}/grouphallfixtures")]
        [HttpGet]
        public async Task<IActionResult> GetStartGroupHallFixture([FromRoute] int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            var response = await _groupconfiguration.StartGroupHallFixtureConfigureBL(null, groupId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(response.Response);
        }

        /// <summary>
        /// ChangeGroupHallFixtureConfigure
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="groupHallFixturesData"></param>
        /// <returns></returns>
        [Route("{groupId}/grouphallfixtures"), HttpPost]
        public async Task<IActionResult> ChangeGroupHallFixtureConfigure(int groupId, [FromBody] GroupHallFixturesData groupHallFixturesData)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            int consoleId = Convert.ToInt32(groupHallFixturesData.GroupHallFixtureConsoleId);
            string fixtureType = groupHallFixturesData.FixtureType;
            int isChange = 1;
            var startGrpResponseObj = await _groupconfiguration.AddorChangeGroupConsole(consoleId, isChange, groupId, sessionId, fixtureType, false, groupHallFixturesData).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(startGrpResponseObj.Response);
        }

        /// <summary>
        /// Method for saving group layout details
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="groupLayout"></param>
        /// <returns></returns>
        [Route("{groupId}/grouplayout")]
        [HttpPost]
        public async Task<IActionResult> SaveGroupLayoutDetails([FromRoute] int groupId, [FromBody] GroupLayoutSave groupLayout)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            ResponseMessage response;
            if (string.IsNullOrEmpty(groupLayout.Operation.ToString()))
            {
                groupLayout.Operation = Operation.Save;
            }
            switch (groupLayout.Operation.ToString().ToUpper())
            {
                case Constant.DUPLICATEOPERATION:
                    response = await _groupLayout.DuplicateGroupLayoutById(groupLayout.UnitID, groupLayout.GroupID, groupLayout.CarPosition, groupLayout.Operation).ConfigureAwait(false);
                    break;
                default:
                    response = await _groupLayout.SaveGroupLayout(groupId, sessionId, Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(groupLayout))).ConfigureAwait(false);
                    break;
            }
            Utility.LogEnd(methodBeginTime);
            return Ok(response.ResponseArray);

        }

        /// <summary>
        /// Method for updating GroupLayoutDetails
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="groupLayout"></param>
        /// <returns></returns>
        [Route("{groupId}/grouplayout")]
        [HttpPut]
        public async Task<IActionResult> UpdateGroupLayoutDetails([FromRoute] int groupId, [FromBody] GroupLayoutSave groupLayout)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            ResponseMessage response;
            if (string.IsNullOrEmpty(groupLayout.Operation.ToString()))
            {
                groupLayout.Operation = Operation.Update;
            }
            switch (groupLayout.Operation.ToString().ToUpper())
            {
                case Constant.OVERWRITE:
                    response = await _groupLayout.DuplicateGroupLayoutById(groupLayout.UnitID, groupLayout.GroupID, groupLayout.CarPosition, groupLayout.Operation).ConfigureAwait(false);
                    break;
                default:
                    response = await _groupLayout.UpdateGroupLayout(groupId, groupLayout.SectionTab, sessionId, Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(groupLayout))).ConfigureAwait(false);
                    break;
            }
            Utility.LogEnd(methodBeginTime);
            return Ok(response.ResponseArray);
        }

        /// <summary>
        /// Method for updating opening location details
        /// </summary>
        /// <param Name="openingLocation"></param>
        /// <returns></returns>
        [Route("{groupId}/openinglocation"), HttpPut]
        public async Task<IActionResult> UpdateOpeningLocation([FromBody] OpeningLocations openingLocation)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _openingLocation.UpdateOpeningLocation(openingLocation, sessionId).ConfigureAwait(false);
            Utility.LogEnd((methodBeginTime));
            return Ok(response.ResponseArray);
        }


        /// <summary>
        /// This method will  return Opening Location By group configuration Id
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <returns></returns>
        [Route("{groupId}/openinglocation")]
        [HttpGet]
        public async Task<IActionResult> GetopeninglocationByGoupId( int groupConfigurationId)
        {
            var methodBeginTime = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            //var sessionId = Request.Headers[Constant.SESSIONID].ToString();
            var UserRespObj = await _openingLocation.GetOpeningLocationByGroupId(groupConfigurationId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return Ok(UserRespObj.Response);
        }


    }
}
