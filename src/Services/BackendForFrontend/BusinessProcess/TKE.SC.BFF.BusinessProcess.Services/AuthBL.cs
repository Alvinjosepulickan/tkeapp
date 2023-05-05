/************************************************************************************************************
************************************************************************************************************
    File Name     :   AuthBL.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class AuthBL : IAuth
    {
        /// <summary>
        /// Variable Collection
        /// </summary>
        #region Variables
        private readonly ICacheManager _cpqCacheManager;
        private readonly IConfigurationSection _configuration;
        private readonly string _environment;
        private readonly IAuthDL _authdl;

        #endregion

        /// <summary>
        /// AuthBL
        /// </summary>
        /// <param Name="configuration"></param>
        /// <param Name="cpqCacheManager"></param>
        /// <param Name="authdl"></param>
        /// <param Name="logger"></param>
        public AuthBL(IConfiguration configuration, ICacheManager cpqCacheManager, IAuthDL authdl, ILogger<AuthBL> logger)
        {
            _cpqCacheManager = cpqCacheManager;
            _configuration = configuration.GetSection(Constant.PARAMSETTINGS);
            _environment = _configuration[Constant.ENVIRONMENT];
            _authdl = authdl;
            Utility.SetLogger(logger);
        }


        /// <summary>
        /// Constructor for the class GetUserDetails
        /// </summary>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public Task<ResponseMessage> GetUserDetails(User userDetailsModel, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var res = _authdl.GetUserDetails(userDetailsModel);
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.USERDETAILSCPQ, res.Result.Response.ToString());
            Utility.LogEnd(methodBeginTime);
            return res;
        }

    }
}
