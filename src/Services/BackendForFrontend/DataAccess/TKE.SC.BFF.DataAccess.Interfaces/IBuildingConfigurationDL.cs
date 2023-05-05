/************************************************************************************************************
************************************************************************************************************
    File Name     :   IBuildingConfigurationDL class 
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
using Configit.Configurator.Server.Common;
using TKE.SC.Common.Model.ViewModel;
using System.Data;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
    public interface IBuildingConfigurationDL
    {
        /// <summary>
        /// Interface to get list of configuration for project using projectId
        /// </summary>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        List<ListOfConfiguration> GetListOfConfigurationForProject(string quoteId, DataTable configVariables,string sessionId);

        /// <summary>
        /// Interface to get building configuration by Id using BuildingId
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        List<ConfigVariable> GetBuildingConfigurationById(int buildingId);

        /// <summary>
        /// Interface to save building configuration for project
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="userId"></param>
        /// <param Name="projectId"></param>
        /// <param Name="BuildingName"></param>
        /// <param Name="bldVariablejson"></param>
        /// <returns></returns>
        List<Result> SaveBuildingConfigurationForProject(int buildingId, string userId, string projectId, string buildingName, string bldVariablejson, ConflictsStatus isEditFlow,bool hasConflictsFlag, List<ConfigVariable> mapperVariablesForSP);


        /// <summary>
        /// Interface to save building elevation
        /// </summary>
        /// <param Name="dtBuildingElevation"></param>
        /// <returns></returns>
        List<Result> SaveBuildingElevation(DataTable dtBuildingElevation);

        /// <summary>
        /// Interface to autosave building elevation
        /// </summary>
        /// <param Name="dtBuildingElevation"></param>
        /// <returns></returns>
        List<Result> AutoSaveBuildingElevation(DataTable dtBuildingElevation);

        /// <summary>
        /// Interface to update building elevation
        /// </summary>
        /// <param Name="dtBuildingElevation"></param>
        /// <returns></returns>
        List<Result> UpdateBuildingElevation(DataTable dtBuildingElevation, List<ConfigVariable> mapperVariables);

        /// <summary>
        /// Interface to get building elevation by using BuildingId
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        List<BuildingElevation> GetBuildingElevationById(int buildingId, List<ConfigVariable> mapperVariables, string sessionId);

        /// <summary>
        /// Interface to delete building configuration using buildingConfigurationId
        /// </summary>
        /// <param Name="buildingConfigurationId"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        List<Result> DeleteBuildingConfigurationById(int buildingConfigurationId, string userId);

        /// <summary>
        /// Interface to delete building elevation using buildingConfigurationId
        /// </summary>
        /// <param Name="buildingConfigurationId"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        List<Result> DeleteBuildingElevationById(int buildingConfigurationId, string userId);

        /// <summary>
        /// Interface to generate building Name using projectId
        /// </summary>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        int GenerateBuildingName(string quoteId);

        /// <summary>
        /// Interface to get Quick Summary
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="groupId"></param>
        /// <param Name="setId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        QuickSummary QuickConfigurationSummary(string opportunityId, int buildingId, int groupId, int setId, string sessionId);

        /// <summary>
        /// check if group exists for a building
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        bool CheckGroupExists(int buildingId);


        /// <summary>
        /// DuplicateBuildingConfigurationById
        /// </summary>
        /// <param Name="buildingIDDataTable"></param>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        DataSet DuplicateBuildingConfigurationById(DataTable buildingIDDataTable, string quoteId);

        /// <summary>
        /// GetBuildingConfigurationSectionTab
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        bool GetBuildingConfigurationSectionTab(int buildingId);

        /// <summary>
        /// GetLogHistoryUnit
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="lastDate"></param>
        /// <returns></returns>
        LogHistoryResponse GetLogHistoryBuilding(int BuildingId, string lastDate);
        /// <summary>
        /// to get quoteId related data
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <returns></returns>
        DataSet GetQuoteDetails(string quoteId);

        /// <summary>
        /// Get Permission by rolename
        /// </summary>
        /// <param Name="Id"></param>
        /// <param Name="roleName"></param>
        /// <returns></returns>
        List<string> GetPermissionByRole(int id, string roleName);
        /// <summary>
        /// Get Permission list for configuration
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <param Name="roleName"></param>
        /// <returns></returns>
        List<Permissions> GetPermissionForConfiguration(string quoteId, string roleName);

        /// <summary>
        /// Get ListOfConfiguration For Quote
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetListOfConfigurationForQuote(string quoteId, string sessionId);

        /// <summary>
        /// Get ProductCategory By SetId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetProductCategoryBySetId(int id, string type);
        /// <summary>
        /// GetBuildingConflicts
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        List<string> GetBuildingConflicts(int buildingId);
        /// <summary>
        /// GetBuildingVariablesWithUnitByGroupId
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="configVariables"></param>
        /// <returns></returns>
        List<UnitVariables> GetBuildingVariables(int groupid, DataTable configVariables);
    }
}

