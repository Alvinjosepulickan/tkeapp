using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IOzBL
    {
        Task<ResponseMessage> BookingRequest(string quoteId, string sessionId);
        Task<OzBookingRequest> GenerateBookingRequestPayload(string quoteId, string sessionId);
    }
}
