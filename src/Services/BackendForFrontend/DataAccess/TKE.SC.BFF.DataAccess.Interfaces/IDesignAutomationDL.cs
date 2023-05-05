using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Configit.TKE.OrderBom.WebApi;
using Configit.TKE.DesignAutomation.Models;
using Configit.TKE.DesignAutomation.Services;
using Configit.TKE.DesignAutomation.WebApi;
using Configit.TKE.OrderBom.CLMPlatform;
using Configit.TKE.OrderBom.Models;
using Configit.TKE.OrderBom.Services;
using Configit.TKE.OrderBom.WebApi.Models;
using Configit.TKE.DesignAutomation.WebApi.Models;
using Configit.TKE.DesignAutomation.Services.Models;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
//using Configit.TKE.DesignAutomation.WebApi.Models;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
    public interface IDesignAutomationDL
    {
        Task<ResponseMessage> GetOBOMResponseForDA(CreateBomRequest requestBody, ConfigurationDetails configurationDetails, string sessionId, string packagePath);
        Task<List<string>> GetDefaultExportTypes();
        Task<ExportTypeResponse[]> GetAvailableExportTypes();
        Task<SubmitBomResponse> GetSubmitBOMResponse(SubmitBomRequest obomResponse, ConfigurationDetails configurationDetails, string sessionId, string packagePath);
        Task<AutomationTaskDetailsReference> GetJobStatus(string jodId);
        int SaveUpdateJobDetailsForDA(int groupId,List<DaJobDetails> daJobDetailsList, string hangfireJobId, string outputLocation="");
        List<DaJobIdDetails> GetJobIdForDA(int groupId, string sessionId,ref string hangFireJobId, out string daStatus);
        int SaveUpdateHangFireJobDetailsForDA(int groupId,string qouteId,string hangfireJobStatus, string hangfireJobId);
    }
}
