/************************************************************************************************************
************************************************************************************************************
    File Name     :   IGroupConfiguration.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model;
using System.Threading.Tasks;
using TKE.SC.Common.Model.ViewModel;
using Newtonsoft.Json.Linq;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IGroupConfiguration
    {
        /// <summary>
        /// Get group configuration details by Group Id
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="cr"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetGroupConfigurationDetailsByGroupId(int groupConfigurationId, JObject variableAssignments, string sessionId, string selectedTab);

        /// <summary>
        /// Save Group Configuration
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="userName"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        Task<ResponseMessage> SaveGroupConfiguration(int buildingId, string sessionId, JObject variableAssignments);

        /// <summary>
        /// Update Group Configuration
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        Task<ResponseMessage> UpdateGroupConfiguration(int buildingId, int groupConfigurationId, JObject variableAssignments);

        /// <summary>
        /// Get group configuration by Building Id
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetGroupConfigurationByBuildingId(string buildingId);

        /// <summary>
        /// Delete group configuration
        /// </summary>
        /// <param Name="GroupId"></param>
        /// <returns></returns>
        Task<ResponseMessage> DeleteGroupConfiguration(int GroupId);

        /// <summary>
                /// Start Group Configure
                /// </summary>
                /// <param Name="configureRequest"></param>
                /// <param Name="BuildingId"> </param>
                /// <param Name="SessionId"></param>
                /// <returns></returns>
        Task<ResponseMessage> StartGroupConfigure(JObject variableAssignments, int BuildingId, int groupId, string SessionId, string selectedTab);

        /// <summary>
        /// Interface for Save Entrance Configuration method in the Business Layer
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="groupHallFixturesData"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> SaveGroupHallFixture(int groupId, GroupHallFixturesData groupHallFixturesData, string sessionId, int is_Saved);

        /// <summary>
        /// StartUnitHallFixtureConfigure
        /// </summary>
        /// <param Name="consoleId"></param>
        /// <param Name="groupId"></param>
        /// <param Name="SessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> AddorChangeGroupConsole(int consoleId, int isChange, int groupId, string SessionId, string fixtureSelected, bool isReset, GroupHallFixturesData groupConsole = null, bool isSave = false);


        /// <summary>
        /// StartGroupHallFixtureConfigureBL
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> StartGroupHallFixtureConfigureBL(JObject variableAssignments, int groupConfigurationId, string sessionId);

        /// <summary>
        /// DeleteGroupHallFixtureConfigure
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        Task<ResponseMessage> DeleteGroupHallFixtureConsole(int groupId, int consoleId, string fixtureType, string sessionId);

        Task<ResponseMessage> DuplicateGroupConfigurationById(List<int> GroupId, int buildingID);

        /// <summary>
        /// DisplayVariablesValuesResponse
        /// </summary>
        /// <param Name="varibleAssignments"></param>
        /// <returns></returns>
        List<DisplayVariableAssignmentsValues> DisplayVariablesValuesResponse(JObject varibleAssignments);

        /// <summary>
        /// GetLogHistoryTableForConsole
        /// </summary>
        /// <param name="lstConsoleHistory"></param>
        /// <returns></returns>
        List<LogHistoryTable> GetLogHistoryTableForConsole(List<ConsoleHistory> lstConsoleHistory);

        /// <summary>
        /// GetGroupHallFixtureSectionTab
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetGroupConfigurationSectionTab(int groupId);

    }
}
