using System.Collections.Generic;
using System.Threading.Tasks;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
    public interface IOzDL
    {
        public EquipmentandDrawing GetEquipmentAndDrawingForOZ(string opportunityId);
        public int GetBranchId(string name);
        public Dictionary<string, string> GetProjectIdVersionId(string quoteId);
        /// <summary>
        /// SaveSentToCoordinationWorkflowstatusforQuote
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <returns></returns>
        int SaveSentToCoordinationWorkflowstatusforQuote(string quoteId);

        Task<string> GetOzToken(string sessionId);
        Task<ResponseMessage> BookCoOrdination(string quoteId, string sessionId, OzBookingRequest ozBookingRequest);
    }
}
