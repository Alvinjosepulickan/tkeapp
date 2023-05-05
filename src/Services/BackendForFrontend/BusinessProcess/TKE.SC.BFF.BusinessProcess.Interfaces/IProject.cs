/************************************************************************************************************
************************************************************************************************************
    File Name     :   IProject.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Configit.Configurator.Server.Common;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IProject
    {
        /// <summary>
        /// Get list of projects for user
        /// </summary>
        /// <param Name="userId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetListOfProjectsForUser(int userId);

        /// <summary>
        /// to get opportunity details
        /// </summary>
        /// <param Name="oppId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetOpportunityData(string oppId, string sessionId);
        /// <summary>
        /// Search User
        /// </summary>
        /// <param Name="userName"></param>
        /// <returns></returns>
        Task<ResponseMessage> SearchUser(string userName);
        /// <summary>
        /// CreateProjectsBL
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> CreateProjectsBL(string sessionId, string projectId, int versionId = 1);
        /// <summary>
        /// SaveAndUpdateMiniProjectsBL
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="variablesValues"></param>
        /// <returns></returns>
        Task<ResponseMessage> SaveAndUpdateMiniProjectsBL(string sessionId, JObject variablesValues);
        /// <summary>
        /// getListOfProjectsDetailsBl
        /// </summary>
        /// <param Name="userId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetListOfProjectsDetailsBl(string sessionId);

        /// <summary>
        /// to get project info
        /// </summary>
        /// <param Name="OpportunityId"></param>
        /// <param Name="versionId"></param>
        /// <param Name="defineSymbol"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetProjectDetails(string opportunityId, string versionId, string sessionId, int parentVersionId = 0, bool refreshCache = false);

        /// <summary>
        /// Post project info to view system
        /// </summary>
        /// <param Name="OpportunityId"></param>
        /// <param Name="versionId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>


        Task<ResponseMessage> SaveProjectInfo1(string opportunityId, string versionId, string sessionId);

        Task<ResponseMessage> SaveConfigurationToView(string opportunityId, string versionId, string sessionId);

        /// <summary>
        /// GetProjectInfo
        /// </summary>
        /// <param name="opportunityId"></param>
        /// <param name="versionId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetProjectInfo(string opportunityId, string versionId, string sessionId, int parentVersionId = 0);
        /// <summary>
        /// delete project
        /// </summary>
        /// <param Name="projectId"></param>
        /// <param Name="versionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> DeleteProjectById(string projectId, string versionId, string sessionId);

        /// <summary>
        /// GetProjectInfoForViewuser
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="versionId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetProjectInfoForViewuser(string projectId, string versionId, string sessionId);

        /// <summary>
        /// AddQuoteForProjectId
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="versionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> AddQuoteForProject(string opportunityId, string sessionId);
        /// <summary>
        /// AddToPrimaryQuoteBL
        /// </summary>
        /// <param name="opportunityId"></param>
        /// <param name="versionId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> AddToPrimaryQuote(string quoteId, string sessionId);

        /// <summary>
        /// DuplicateQuotesByQuoteId
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="quoteId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> DuplicateQuotesByQuoteId(string projectId, string quoteId, string sessionId);
        /// <summary>
        /// Generate variable assignments for Prdoucttree call
        /// </summary>
        /// <param name="setVariableAssignment"></param>
        /// <returns></returns>
        List<VariableAssignment> GenerateVariableAssignmentsForProductTree(SetVariableAssignment setVariableAssignment);
    }
}
