/************************************************************************************************************
************************************************************************************************************
    File Name     :   IGroupConfigurationDL class 
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
using Newtonsoft.Json.Linq;
using System.Data;
using Configit.Configurator.Server.Common;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
    public interface IGroupConfigurationDL
    {

        /// <summary>
        /// Interface to get group configuration details by GroupConfigurationId
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <returns></returns>
        GroupLayout GetGroupConfigurationDetailsByGroupId(int groupConfigurationId, string selectTab, string sessionId);

        /// <summary>
        /// Interface to save group configuration
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="groupName"></param>
        /// <param name="userName"></param>
        /// <param name="grpVariablejson"></param>
        /// <param name="productCategory"></param>
        /// <param name="numberOfUnits"></param>
        /// <returns></returns>
        List<ResultGroupConfiguration> SaveGroupConfiguration(int buildingId, string groupName, string userName, string grpVariablejson, string productCategory, int numberOfUnits);
        /// <summary>
        /// Interface to update group configuration
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="GroupName"></param>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="grpVariablejson"></param>
        /// <param Name="unitVariableAssignment"></param>
        /// <returns></returns>
        List<ResultGroupConfiguration> UpdateGroupConfiguration(int buildingId, string groupName, int groupConfigurationId, string grpVariablejson);


        /// <summary>
        /// Interface to get group configuration by BuildingId
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetGroupConfigurationByBuildingId(string buildingId);

        /// <summary>
        /// Interface to delete group configuration by GroupId
        /// </summary>
        /// <param Name="GroupId"></param>
        /// <returns></returns>
        List<GroupResult> DeleteGroupConfiguration(int GroupId);

        /// <summary>
        /// Interface to generate group Name by BuildingId
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        int GenerateGroupName(int buildingId);

        /// <summary>
        /// Interface to generate product category by Id
        /// </summary>
        /// <param Name="productCategoryId"></param>
        /// <returns></returns>
        string GenerateProductCategory(int productCategoryId);

        /// <summary>
        /// interface to generate Floor designation and floor number by group Id
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        List<BuildingElevationData> GetFloorDesignationFloorNumberByGroupId(int groupId);

        /// <summary>
        /// interface to get Group Values by Variable Name
        /// </summary>
        /// <param Name = "VariableName" ></ param >
        /// < returns ></ returns >
        string GetGroupValues(string VariableName);

        /// <summary>
        /// Interface to SaveEntranceConfiguration
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="groupHallFixturesData"></param>
        /// <param Name="userId"></param>
        /// <param Name="is_Saved"></param>
        /// <returns></returns>
        List<ResultGroupConfiguration> SaveGroupHallFixture(int groupId, GroupHallFixturesData groupHallFixturesData, string userId, int is_Saved, List<LogHistoryTable> historyTable);

        /// <summary>
        /// GetUnitHallFixturesData
        /// </summary>
        /// <param Name="UnitId"></param>
        /// <param Name="groupid"></param>
        /// <param Name="userName"></param>
        /// <param Name="FixtureStrategy"></param>
        /// <returns></returns>
        List<GroupHallFixtures> GetGroupHallFixturesData(int groupid, string userName, string fixtureStrategy, List<ConfigVariable> hallStations);

        /// <summary>
        /// get Unit related data for group hall fixture
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="doorDetails"></param>
        /// <returns></returns>
        List<UnitDetailsValues> GetUnitDetails(int groupConfigurationId, List<ConfigVariable> doorDetails);
        /// <summary>
        /// GetGroupFixtureStrategy
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        string GetGroupFixtureStrategy(int groupConfigurationId);

        /// <summary>
        /// GetGroupHallFixturesTypesList
        /// </summary>
        /// <param Name="fixtureStrategy"></param>
        /// <returns></returns>
        //List<string> GetGroupHallFixturesTypesList(string FixtureStrategy);

        /// <summary>
        /// DeleteGroupHallFixtureConfigurationById
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="consoleId"></param>
        /// <param Name="fixtureType"></param>
        /// <returns></returns>
        List<ResultSetConfiguration> DeleteGroupHallFixtureConsole(int groupId, int consoleId, string fixtureType, List<LogHistoryTable> historyTable, string userId);

        /// <summary>
        /// check if Unit in a group is configured
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        bool CheckUnitConfigured(int groupId);
        /// <summary>
        /// CheckProductSelected
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <returns></returns>
        bool CheckProductSelected(int groupConfigurationId);
        /// <summary>
        /// Duplicate group Configuration by Id
        /// </summary>
        /// <param Name="groupIDDataTable"></param>
        /// <param Name="buildingID"></param>
        /// <returns></returns>
        DataSet DuplicateGroupConfigurationById(DataTable groupIDDataTable, int buildingID);
        /// <summary>
        /// get all variable assignments for building;
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <returns></returns>
        List<ConfigVariable> GetBuildingVariableAssignments(int groupConfigurationId);
        /// <summary>
        /// GetLogHistoryGroup
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="lastDate"></param>
        /// <returns></returns>
        LogHistoryResponse GetLogHistoryGroup(int groupId, string lastDate);
        /// <summary>
        /// Get Permission by role
        /// </summary>
        /// <param Name="Id"></param>
        /// <param Name="roleName"></param>
        /// <returns></returns>
        List<string> GetPermissionByRole(int id, string roleName);

        /// <summary>
        /// GetBuildingConfigurationSectionTab
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        Dictionary<string, bool> GetGroupConfigurationSectionTab(int groupId, List<ConfigVariable> hallStations);

        /// <summary>
        /// Get Product Category By groupId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetProductCategoryByGroupId(int id, string type, DataTable configVariables);

        /// <summary>
        /// Get Group Info Variables By GroupId
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        List<ConfigVariable> GetGroupInformationByGroupId(int groupId);
        /// <summary>
        /// SaveBuildingConflicts
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="conflictVariables"></param>
        /// <returns></returns>
        List<Result> SaveBuildingConflicts(int buildingId, List<VariableAssignment> conflictVariables, string entityType);
        /// <summary>
        /// GetBuildingGroupVariablesWithUnitByGroupId
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="configVariables"></param>
        /// <returns></returns>
        List<UnitVariables> GetGroupVariables(int groupid, DataTable configVariables);
    }
}
