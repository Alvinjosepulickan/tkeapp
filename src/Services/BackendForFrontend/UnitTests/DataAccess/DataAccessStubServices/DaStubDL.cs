using Configit.TKE.DesignAutomation.Services.Models;
using Configit.TKE.DesignAutomation.WebApi.Models;
using Configit.TKE.OrderBom.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.BFF.Helper;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;

namespace TKE.SC.BFF.UnitTests.DataAccess.DataAccessStubServices
{
    class DaStubDL : IDesignAutomationDL
    {
        public Task<ExportTypeResponse[]> GetAvailableExportTypes()
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetDefaultExportTypes()
        {
            throw new NotImplementedException();
        }

        public List<string> GetJobIdForDA(int groupId, string sessionId)
        {
            var listJobIds = new List<string>
            {
                "6eee25f6-6282-4170-b533-002f6cbf45ce"
            };
            return listJobIds;
        }

        public async Task<AutomationTaskDetails> GETJobStatus(string jodId)
        {
            if (jodId != null)
            {
                var response = new AutomationTaskDetails
                {
                    Id = "6eee25f6-6282-4170-b533-002f6cbf45ce",
                    PercentageCompleted = 100,
                    Status = Configit.Grid.JobStatus.Succeeded
                };
                return response;
            }
            else
            {
                throw new CustomException(new ResponseMessage { StatusCode = Constant.BADREQUEST });
            }
        }

        public async Task<ResponseMessage> GETOBOMResponse(CreateBomRequest requestBody)
        {
            throw new NotImplementedException();
        }

        public async Task<SubmitBomResponse> GETSubmitBOMResponse(SubmitBomRequest obomResponse)
        {
            throw new NotImplementedException();
        }

        public int SaveJobIdForDA(int groupId, string jobId, string outputLocation)
        {
            throw new NotImplementedException();
        }
    }
}
