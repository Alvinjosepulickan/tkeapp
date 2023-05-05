using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TKE.SC.Common.Model;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IReleaseInfo
    {
        Task<ResponseMessage> GetProjectReleaseInfo(string quoteId,string sessionId);

        Task<ResponseMessage> GetGroupReleaseInfo(int groupId,string sessionId);

        Task<ResponseMessage> SaveUpdatReleaseInfoDetails(int groupid, JObject variableAssignments, string sessionId);
    }
}