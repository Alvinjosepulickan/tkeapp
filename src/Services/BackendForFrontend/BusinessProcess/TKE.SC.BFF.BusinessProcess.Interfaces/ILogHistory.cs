using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.Common.Model;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface ILogHistory
    {
        /// <summary>
        /// GetLogHistory
        /// </summary>
        /// <param Name="requestBody"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetLogHistory(LogHistoryRequestBody requestBody);
    }
}
