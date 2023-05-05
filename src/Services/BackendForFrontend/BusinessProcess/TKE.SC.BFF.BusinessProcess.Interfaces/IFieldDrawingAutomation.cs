using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IFieldDrawingAutomation
    {
        Task<ResponseMessage> StartFieldDrawingConfigure(JObject variableAssignments, int groupId, string sessionId);

        Task<ResponseMessage> GetFieldDrawingsByProjectId(string projectId, string sessionId);

        Task<ResponseMessage> GetRequestQueueByGroupId(int groupId);

        Task<ResponseMessage> SaveFieldDrawingConfigure(int groupId, JObject variableAssignments, string sessionId);

        Task<ResponseMessage> UpdateLockedGroupsByProjectId(string sessionId, int groupid, string islock);

        Task<ResponseMessage> RequestLDDrawings(int groupId, JObject variableAssignments, string sessionId);

        Task<ResponseMessage> SaveSendToCoordination(string projectId, JObject sendToCoordinationObj, string sessionId);

        Task<ResponseMessage> GetSendToCoordinationByProjectId(string projectId);

        Task<ResponseMessage> RefreshFieldDrawingConfigure(int groupId, string sessionId);

        Task<ResponseMessage> ChangeFieldDrawingConfigure(JObject variableAssignments, int groupId, bool isChange, string sessionId, List<UnitLayOutDetails> lstUnitLayoutDetails, string QuoteId = "");
        Task<ResponseMessage> GetSendToCoordinationStatus(string projectId);
        string GetLDResponseJson(int groupId);

        Task<ResponseMessage> SaveUpdateFieldDrawingStatus(JObject variableAssignments);

        string GetQuoteIdFromCache(string sessionId);
    }
}
