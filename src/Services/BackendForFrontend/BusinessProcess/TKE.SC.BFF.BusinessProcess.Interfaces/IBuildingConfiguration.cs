/************************************************************************************************************
************************************************************************************************************
   File Name     :   IBuildingConfiguration.cs 
   Created By    :   Infosys LTD
   Created On    :   01-JAN-2020
   Modified By   :
   Modified On   :
   Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IBuildingConfiguration
    {
        /// <summary>
        /// Interface for GetListOfConfigurationForProject
        /// </summary>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetListOfConfigurationForProject(string quoteId,string sessionId);

        /// <summary>
        /// Interface for GetBuildingConfigurationById
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="cr"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetBuildingConfigurationById(int buildingId, JObject variableAssignments, string sessionId);


        /// <summary>
        /// Interface for SaveBuildingConfigurationForProject
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="userId"></param>
        /// <param Name="projectId"></param>
        /// <param Name="BuildingName"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        Task<ResponseMessage> SaveBuildingConfigurationForProject(int buildingId, string sessionId, string quoteId, JObject variableAssignments);



        /// <summary>
        /// Interface for SaveBuildingElevation
        /// </summary>
        /// <param Name="buildingElevationDatas"></param>
        /// <returns></returns>
        Task<ResponseMessage> SaveBuildingElevation(BuildingElevation buildingElevationDatas);

        /// <summary>
        /// Interface for UpdateBuildingElevation
        /// </summary>
        /// <param Name="buildingElevationDatas"></param>
        /// <returns></returns>
        Task<ResponseMessage> UpdateBuildingElevation(BuildingElevation buildingElevationDatas);

        /// <summary>
        /// Interface for AutoSaveBuildingElevation
        /// </summary>
        /// <param Name="buildingElevationDatas"></param>
        /// <returns></returns>
        Task<ResponseMessage> AutoSaveBuildingElevation(BuildingElevation buildingElevationDatas);

        /// <summary>
        /// Interface for GetBuildingElevationById
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetBuildingElevationById(int buildingId,string sessionId);

        /// <summary>
        /// Interface for DeleteBuildingConfigurationById
        /// </summary>
        /// <param Name="buildingConfigurationId"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        Task<ResponseMessage> DeleteBuildingConfigurationById(int buildingConfigurationId, string userId);

        /// <summary>
        /// Interface for DeleteBuildingElevationById
        /// </summary>
        /// <param Name="buildingConfigurationId"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        Task<ResponseMessage> DeleteBuildingElevationById(int buildingConfigurationId, string userId);

        /// <summary>
        /// Interface for StartBuildingConfigure
        /// </summary>
        /// <param Name="cr"></param>
        /// <param Name="projectId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> StartBuildingConfigure(JObject variableAssignments, string quoteId, int buildingId, string sessionId, bool isReset);

        /// <summary>
        ///  Interface for Quick Summary
        /// </summary>
        /// <param Name="type"></param>
        /// <param Name="Id"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> QuickConfigurationSummary(string type, string Id, string sessionId);

        /// <summary>
        /// DuplicateBuildingConfigurationById
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        Task<ResponseMessage> DuplicateBuildingConfigurationById(List<int> buildingId, string quoteId);

        /// <summary>
        /// GetBuildingConfigurationSectionTab
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetBuildingConfigurationSectionTab(int buildingId);


        Task<ResponseMessage> GetListOfConfigurationForQuote(string quoteId, string sessionId);

        ConfigurationRequest CreateBuildingConfigurationRequest(JObject varibleAssignments);
    }
}
