/************************************************************************************************************
************************************************************************************************************
    File Name     :   AccessBL.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/


using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class AccessBL : IAccess
    {
        /// <summary>
        /// Variable Collection
        /// </summary>
        #region Variables
        private readonly IConfigurationSection _configuration;
        private readonly string _environment;
        private readonly ICacheManager _cpqCacheManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        /// <summary>
        /// Constructor for the class AccessBL
        /// </summary>
        /// <param Name="configuration"></param>
        /// <param Name="cpqCacheManager"></param>
        /// <param Name="httpContextAccessor"></param>
        /// <param Name="logger"></param>
        public AccessBL(IConfiguration configuration, ICacheManager cpqCacheManager,
            IHttpContextAccessor httpContextAccessor, ILogger<AccessBL> logger)
        {
            _configuration = configuration?.GetSection(Constant.PARAMSETTINGS);
            if (_configuration == null)
            {
                return;
            }

            _cpqCacheManager = cpqCacheManager;
            _httpContextAccessor = httpContextAccessor;
            _environment = _configuration[Constant.ENVIRONMENT];
            if (_httpContextAccessor.HttpContext != null)
            {
                var sessionId = _httpContextAccessor?.HttpContext?.Request?.Headers[Constant.SESSIONID];
                var isProdChk = _cpqCacheManager.GetCache(sessionId, _environment, Constant.ISPRODUCTIONCHECK);
                bool isProductionChk = Convert.ToBoolean(isProdChk);
                _configuration = isProductionChk ? configuration?.GetSection(Constant.PRODUCTIONCHECKSETTINGS) : configuration?.GetSection(Constant.PARAMSETTINGS);
                if (_configuration == null)
                {
                    return;
                }
                return;
            }
            Utility.SetLogger(logger);
        }
    }
}
