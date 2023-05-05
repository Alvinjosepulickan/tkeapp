using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
    public interface IFieldDrawingAutomationDL
    {
        List<ConfigVariable> GetFieldDrawingAutomationByGroupId(int groupid, string projectId, DataTable ConfigVariables);

        List<UnitLayOutDetails> GetFieldDrawingAutomationLayoutByGroupId(int groupid, string projectId, DataTable ConfigVariables);

        GroupDetailsForDrawingDetails GetFieldDrawingsByProjectId(string OpportunityId, DataTable dtVariables);

        RequestHistory GetRequestQueueByGroupId(int groupId);

        Task<string> GenerateWrapperToken();

        Task<string> RequestLayouts(JObject json, string token);

        Task GetLayoutStatus(int groupId, string projectId, string userName, string ReferenceId, int IntegratedProcessId, string WrapperToken);

        int SaveReferenceId(int groupId, string projectId, string userName, string statusId, int intergratedSystemId, string referenceId);

        List<Reference> CheckRequestIdByFDAIntegrationId(int fieldDrawingIntegrationMasterId);

        List<ConfigVariable> GetLiftDesignerByGroupId(int groupid, string storedProcedureName);

        ResultGroupConfiguration SaveFdaByGroupId(int groupid, string quoteId, string createdBy, List<ConfigVariable> FieldDrawingAutomationDataTable, List<ConfigVariable> groupVariableAssignment);
        List<ResultGroupConfiguration> UpdateFdaDrawingMethodByGroupId(int groupid, int drawingMethod);

        List<ResultGroupConfiguration> UpdateLockPropertyForGroups(string projectId, int groupId, string islock);

        List<RecurringJobData> CheckHangFireRecurringJob(int IntegratedProcessId);

        List<ResultGroupConfiguration> SaveSendToCoordination(string projectId, string createdBy, List<SendToCoordinationData> coordinationData);

        GroupDetailsForSendToCoordination GetSendToCoordinationByProjectId(string OpportunityId);

        List<FDAOutputTypes> GetOutputTypesForXMGeneration(int groupid);

        FieldDrawingStatus GetLayoutRequestIdWithStatus(int groupid, string quoteId);

        GroupDetailsForDrawingDetails GetFieldDrawingsByGroupId(int groupid, string quoteId, DataTable dtVariables);

        void UpdateFDARequestStatusByFieldDrawingId(int fieldDrawingId);

        string GetGroupStatusByGroupId(int groupid);

        string GetProjectStatusByGroupId(string quoteId);

        string GetFieldDrawingStatusByGroupId(int groupId, string quoteId);

        string GetQuoteIdByGroupId(int groupId);

        /// <summary>
        /// GetPermissionForFDA
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        List<Permissions> GetPermissionForFDA(string quoteId, string roleName, string entity);

        /// <summary>
        /// GetSendToCoordinationStatus
        /// </summary>
        /// <param name="qouteId"></param>
        /// <returns></returns>
        SendToCoordinationStatus GetSendToCoordinationStatus(string qouteId);

        List<UnitVariables> GetUnitsVariablesWithUnitDetailsByGroupId(int groupid, DataTable dtVariables);

        List<UnitVariables> GetXMLVariablesWithUnitByGroupId(int groupid, DataTable dtVariables);

        ProjectDet GetOpportunityAndVersionByQuoteId(string quoteId);
        int GetBuildingId(int groupId);
        string GetLDResponseJson(int groupId);
        int SaveVariableArguments(int groupId, List<UnitVariables> listVariablesArgument,string userName);
        int UpdateStatusForFDA(string guid, string statusId);
        /// <summary>
        /// GetGroupInformation
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        List<GroupInfo> GetGroupInformation(int groupid);
    }
}
