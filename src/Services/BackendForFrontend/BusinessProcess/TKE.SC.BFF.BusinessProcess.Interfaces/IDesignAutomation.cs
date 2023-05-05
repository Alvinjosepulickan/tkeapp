using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IDesignAutomation
    {
        Task<ResponseMessage> GetDAResponse(ConfigurationDetails configurationDetails, string sessionId, JObject variableAssignments=null);
        Task<ResponseMessage> GetDAStatus(ConfigurationDetails configurationDetails, string sessionId); 
    }
}
