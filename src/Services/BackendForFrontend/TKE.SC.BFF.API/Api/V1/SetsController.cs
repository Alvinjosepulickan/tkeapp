using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.Controllers;
using TKE.SC.BFF.Helper;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.APIController
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class SetsController : Controller
    {
        #region Variables
        private readonly IUnitConfiguration _unitConfiguration;
        private readonly IConfigure _configure;
        private readonly ILogHistory _loghistory;
        #endregion

        public SetsController(IUnitConfiguration unitConfiguration, IConfigure configure, ILogHistory logHistory, ILogger<SetsController> logger)
        {
            _unitConfiguration = unitConfiguration;
            _configure = configure;
            _loghistory = logHistory;
            Utility.SetLogger(logger);
        }

        /// <summary>
        /// Get and Start API
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="setId"></param>
        /// <param Name="variableAssignments"></param>
        /// <returns></returns>
        [Route("{setId}/intiate"), HttpPost]
        public async Task<IActionResult> StartUnitConfigure([FromQuery] int groupid,[FromRoute] int setId,
          [FromBody] JObject variableAssignments,[FromQuery] int unitId)
        {
            var sectionTab = variableAssignments[Constant.SECTIONTABS].ToString();

            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var startUnitResponseObj = await _unitConfiguration.StartUnitConfigure(variableAssignments, groupid, setId, sessionId, sectionTab,unitId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(startUnitResponseObj.Response);
        }

        /// <summary>
        /// Change UnitConfiguration
        /// </summary>
        /// <param name="variableAssignments"></param>
        /// <param name="sectionTab"></param>
        /// <returns></returns>
        [Route("{setId}/configure")]
        [HttpPost]
        public async Task<IActionResult> ChangeUnitConfigure([FromRoute] int setId, [FromBody] JObject variableAssignments, string sectionTab,[FromQuery] int unitId)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var changeUnitResponse = await _unitConfiguration.ChangeUnitConfigure(variableAssignments, sessionId, sectionTab, setId,unitId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(changeUnitResponse.Response);
        }

        /// <summary>
        /// Save Unit Configuration
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="variableAssignments"></param>
        /// <returns></returns>
        [Route("{setId}/unit")]
        [HttpPost]
        public async Task<IActionResult> SaveUnitConfiguration([FromQuery] int sectionId, [FromRoute] int setId, [FromBody] JObject variableAssignments,[FromQuery] int unitId)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.SaveUnitConfiguration(setId, variableAssignments, sessionId, unitId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.ResponseArray);
        }

        /// <summary>
        /// Start Entrance Console Configuration with Console Id
        /// </summary>
        /// <param Name="consoleId"></param>
        /// <param Name="setId"></param>
        /// <returns></returns>
        [Route("{setId}/entrances"), HttpGet]
        public async Task<IActionResult> StartEntranceConfigure([FromQuery] int consoleId, [FromRoute] int setId)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var startGrpResponseObj = await _unitConfiguration.StartEntranceConfigure(consoleId, setId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(startGrpResponseObj.Response);
        }

        /// <summary>
        /// Save Entrance Console Configuration
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="entranceConfigurationData"></param>
        /// <returns></returns>
        [Route("{setId}/entrances"), HttpPost]
        public async Task<IActionResult> SaveEntranceConfiguration([FromRoute] int setId, [FromBody] EntranceConfigurationData entranceConfigurationData)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.SaveEntranceConfiguration(setId, entranceConfigurationData, sessionId, Constant.IS_SAVE).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.ResponseArray);

        }

        /// <summary>
        /// Update Entrance Console Configuration
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="entranceConfigurationData"></param>
        /// <returns></returns>
        [Route("{setId}/entrances"), HttpPut]
        public async Task<IActionResult> UpdateEntranceConfiguration([FromRoute] int setId, [FromBody] EntranceConfigurationData entranceConfigurationData)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.SaveEntranceConfiguration(setId, entranceConfigurationData, sessionId, Constant.IS_UPDATE).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.ResponseArray);
        }
        
        /// <summary>
        /// Configure Entrance Console Configuration
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="objEntranceConfigurationData"></param>
        /// <returns></returns>
        [Route("{setId}/entrances/configure"), HttpPost]
        public async Task<IActionResult> ChangeEntranceConfigure([FromRoute] int setId, [FromBody] EntranceConfigurationData objEntranceConfigurationData)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            int consoleId = Convert.ToInt32(objEntranceConfigurationData.EntranceConsoleId);
            var startGrpResponseObj = await _unitConfiguration.StartEntranceConfigure(consoleId, setId, sessionId, objEntranceConfigurationData).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(startGrpResponseObj.Response);
        }
        
        /// <summary>
        /// Delete Entrance Console Configuration 
        /// </summary>
        /// <param name="consoleId"></param>
        /// <param name="setId"></param>
        /// <returns></returns>
        [Route("{setId}/entrances"), HttpDelete]
        public async Task<IActionResult> DeleteEntranceConfigure([FromQuery] int consoleId, [FromRoute] int setId)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var startGrpResponseObj = await _unitConfiguration.DeleteEntranceConsole(consoleId, setId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(startGrpResponseObj.ResponseArray);
        }

        /// <summary>
        /// Save UnitHall Configuration
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="entranceConfigurationData"></param>
        /// <returns></returns>
        [Route("{setId}/unithall"), HttpPost]
        public async Task<IActionResult> SaveUnitHallFixtureConfiguration([FromRoute] int setId, [FromBody] UnitHallFixtureData unitHallFixtureConfigurationData)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.SaveUnitHallFixtureConfiguration(setId, unitHallFixtureConfigurationData, sessionId, Constant.IS_SAVE).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.ResponseArray);
        }

        [Route("{setId}/unithall"), HttpGet]
        public async Task<IActionResult> StartUnitHallFixtureConfigure([FromQuery] int consoleId, int setId, [FromQuery] string fixtureSelected)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            int isChange = 0;
            var startGrpResponseObj = await _unitConfiguration.StartUnitHallFixtureConfigure(consoleId, isChange, setId, sessionId, fixtureSelected, false).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(startGrpResponseObj.Response);
        }

        /// <summary>
        /// DeleteUnitHallFixtureConfigure
        /// </summary>
        /// <returns></returns>
        [Route("{setId}/unithall"), HttpDelete]
        public async Task<IActionResult> DeleteUnitHallFixtureConfigure([FromRoute] int setId, [FromQuery] int consoleId, [FromQuery] string fixtureSelected)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var BuildingRespObj = await _unitConfiguration.DeleteUnitHallFixtureConsole(setId, consoleId, fixtureSelected, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(BuildingRespObj.ResponseArray);
        }

        /// <summary>
        /// GetDetailsForTP2SummaryScreen
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        [Route("{setId}/tp2summary")]
        [HttpGet]
        public async Task<IActionResult> GetDetailsForTP2SummaryScreen([FromRoute] int setId)
        {

            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.GetDetailsForTP2SummaryScreen(setId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.Response);
        }

        // <summary>
        /// Method for saving the final price from TP2 summary screen
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="priceDetails"></param>
        /// <returns></returns>
        [Route("{setId}/tp2summary")]
        [HttpPost]
        public async Task<IActionResult> SavePriceForTP2SummaryScreen([FromRoute] int setId, [FromBody] UnitSummaryUIModel requestBody)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.SavePriceForTP2SummaryScreen(setId, requestBody, sessionId, new List<UnitNames>()).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.ResponseArray);
        }

        /// <summary>
        /// ChangeUnitHallFixtureConfigure
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="objEntranceConfigurationData"></param>
        /// <returns></returns>
        [Route("{setId}/unithallfixtures/configure"), HttpPost]
        public async Task<IActionResult> ChangeUnitHallFixtureConfigure([FromRoute] int setId, [FromBody] UnitHallFixtureData objUnitHallFixtureConfigurationData)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            int consoleId = Convert.ToInt32(objUnitHallFixtureConfigurationData.ConsoleId);
            string fixtureType = objUnitHallFixtureConfigurationData.FixtureType;
            int isChange = 1;
            var startGrpResponseObj = await _unitConfiguration.StartUnitHallFixtureConfigure(consoleId, isChange, setId, sessionId, fixtureType, false, objUnitHallFixtureConfigurationData).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(startGrpResponseObj.Response);
        }

        /// <summary>
        /// SaveCarCallCutoutKeyswitchOpenings
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="carcallCutoutData"></param>
        /// <returns></returns>
        [Route("{setId}/carcallcutoutkeyswitch")]
        [HttpPost]
        public async Task<IActionResult> SaveCarCallCutoutKeyswitchOpenings([FromRoute] int setId, [FromBody] CarcallCutoutData carcallCutoutData)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.SaveCarCallCutoutKeyswitchOpenings(setId, carcallCutoutData, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.ResponseArray);
        }
        /// <summary>
        /// StartCarCallCutoutAssignOpenings
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        [Route("{setId}/carcallcutoutkeyswitch"), HttpGet]
        public async Task<IActionResult> StartCarCallCutoutAssignOpenings([FromRoute] int setId)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var startGrpResponseObj = await _unitConfiguration.StartCarCallCutoutAssignOpenings(setId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(startGrpResponseObj.Response);
        }
       

        /// <summary>
        /// System Validations
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        [Route("{setId}/validate")]
        [HttpGet]
        public async Task<IActionResult> GetSystemValidationForUnit([FromRoute] int setId)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.GetSystemValidationForUnit(setId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.Response);
        }

        /// <summary>
        /// Fetch Status
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        [Route("{setId}/status")]
        [HttpGet]
        public async Task<IActionResult> RefreshSystemValidationForUnit([FromRoute] int setId)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.GetLatestSystemValidationForUnit(setId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.Response);
        }




        /// <summary>
        /// This method is used for editing the Unit designation for a particular Unit
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="UnitId"></param>
        /// <param Name="unit"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> EditUnitDesignation([FromQuery] int groupId, [FromQuery] int unitId, [FromBody] UnitDesignation unit)
        {

            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.EditUnitDesignation(groupId, unitId, sessionId, unit).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.Response);

        }

        [Route("{setId}/unithallfixtures/validate"), HttpGet]
        public async Task<IActionResult> ValidateUnitHallFixtureConsoles([FromRoute] int setId)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.ValidateUnitHallFixtureConsoles(setId, sessionId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.ResponseArray);

        }



        /// <summary>
        /// GetDetailsForTP2SummaryScreen
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        [Route("{setId}/tp2summary/configure")]
        [HttpPost]
        public async Task<IActionResult> GetDetailsForTP2SummaryScreen([FromRoute] int setId, [FromBody] UnitSummaryUIModel requestBody)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.ChangeTP2SummaryDetails(setId, sessionId, requestBody).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.Response);
        }

        /// <summary>
        /// api to save custom price line
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="customPriceLine"></param>
        /// <returns></returns>
        [Route("{setId}/tp2summary/customPriceLine")]
        [HttpPost]
        public async Task<IActionResult> SaveCustomPriceLine([FromRoute] int setId, [FromBody] List<CustomPriceLine> customPriceLine )
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.SaveCustomPriceLine(setId, sessionId, customPriceLine).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.Response);
        }
        /// <summary>
        /// api to update custom price line
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="customPriceLine"></param>
        /// <returns></returns>
        [Route("{setId}/tp2summary/customPriceLine")]
        [HttpPut]
        public async Task<IActionResult> EditCustomPriceLine([FromRoute] int setId, [FromBody] CustomPriceLine customPriceLine)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.EditCustomPriceLine(setId, sessionId, customPriceLine).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.Response);
        }
        /// <summary>
        /// api to delete custom price line
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="priceLineId"></param>
        /// <returns></returns>
        [Route("{setId}/tp2summary/customPriceLine/{priceLineId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomPriceLine([FromRoute] int setId, [FromRoute]  int priceLineId)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.DeleteCustomPriceLine(setId, sessionId, priceLineId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.Response);
        }
        /// <summary>
        /// api to edit Factory Job ID
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="factoryJobId"></param>
        /// <returns></returns>
        [Route("unit/{unitId}/factoryNumber/{factoryJobId}")]
        [HttpPut]
        public async Task<IActionResult> CreateUpdateFactoryJobId([FromRoute] int unitId, [FromRoute] string factoryJobId)
        {

            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var response = await _unitConfiguration.CreateUpdateFactoryJobId(unitId, sessionId,factoryJobId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(response.Response);

        }
    }
}
