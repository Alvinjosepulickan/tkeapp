/************************************************************************************************************
    File Name     :   IProjectsDL
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
********************************************************************************************/
using System.Collections.Generic;
using System.Threading.Tasks;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
    public interface IProjectsDL
    {       
        /// <summary>
        /// method to get list of projects for user
        /// </summary>
        /// <param Name="userId"></param>
        /// <returns></returns>
        List<Projects> GetListOfProjectsForUser(int userId);

        /// <summary>
        /// to get opportunity details
        /// </summary>
        /// <param Name="oppId"></param>
        /// <returns></returns>
        OpportunityEntity GetOpportunityData(string oppId);
        /// <summary>
        /// method to search user
        /// </summary>
        /// <param Name="userName"></param>
        /// <returns></returns>
        List<User> SearchUser(string userName);
        /// <summary>
        /// GetMiniProjectValues
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="userDetails"></param>
        /// <returns></returns>

        CreateProjectResponseObject GetMiniProjectValues(string sessionId, string userDetails, string projectId, CreateProjectResponseObject enrichedData, int versionId = 1);

        /// <summary>
        /// SaveAndUpdateMiniProjectValues
        /// </summary>
        /// <param Name="unitVariableAssignment"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        List<ResultProjectSave> SaveAndUpdateMiniProjectValues(VariableDetails enrichedData, string userName, bool isAddQuote = false);

        /// <summary>
        /// GetListOfProjectsDetailsDl-
        /// </summary>
        /// <param Name="userName"></param>
        /// <returns></returns>
        List<ProjectResponseDetails> GetListOfProjectsDetailsDl(User user);
        /// <summary>
        /// to get project Info
        /// </summary>
        /// <param Name="OpportunityId"></param>
        /// <param Name="versionId"></param>
        /// <param Name="defineSymbol"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetProjectInfo(string opportunityId, string versionId);
        /// <summary>
        /// to get variables and values
        /// </summary>
        /// <param Name="oppId"></param>
        /// <returns></returns>



        ViewExportDetails GetVariablesAndValuesForView1(string opportunityId, List<string> exportJsonBuildingVariables, List<string> exportJsoneqmntConsoleVariables, List<string> exportJsonEqmntConfgnVariables, List<string> exportJsonControlLocationVariables, List<string> exportJsonUnitConfigurationVariables, List<BuildingVariableAssignment> defaultVtPackageValues);

        Task<ResponseMessage> SaveConfigurationToView(ResponseMessage requestBody);
        /// <summary>
        /// GenerateQuoteId
        /// </summary>
        /// <param name="viewDetails"></param>
        /// <param name="userName"></param>
        /// <param name="parentVersionId"></param>
        /// <returns></returns>
        List<ResultProjectSave> GenerateQuoteId(ViewProjectDetails viewDetails, string userName, int parentVersionId = 0);
        /// <summary>
        /// delete project DL
        /// </summary>
        /// <param Name="projectId"></param>
        /// <param Name="versionId"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        List<ResultProjectDelete> DeleteProjectById(string projectId, string versionId, string userId);

        /// <summary>
        /// get vt package variables by quoteId
        /// </summary>
        /// <param Name="OpportunityId"></param>
        /// <returns></returns>
        List<BuildingVariableAssignment> Getvariablevalues(string opportunityId);

        /// <summary>
        /// Get Permission by role
        /// </summary>
        /// <param Name="roleName"></param>
        /// <returns></returns>
        List<Permissions> GetPermissionByRole(string roleName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rolename"></param>
        /// <param name="projectid"></param>
        /// <returns></returns>
        List<string> GetPermissionForProjectScreen(string rolename, string projectid);
        /// <summary>
        /// SetQuoteToPrimaryDL
        /// </summary>
        /// <param name="enrichedData"></param>
        /// <param name="userName"></param>
        /// <param name="isAddQuote"></param>
        /// <returns></returns>
        ResultProjectSave SetQuoteToPrimaryDL(string userName, string quoteId);

        /// <summary>
        /// GetDuplicateQuoteByProjectIdDL
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="quoteId"></param>
        /// <param name="userName"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        List<ResultProjectSave> GetDuplicateQuoteByProjectIdDL(string projectId, string quoteId, string userName, string country);
    }
}
