/************************************************************************************************************
************************************************************************************************************
    File Name     :   GenerateTokenBL.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GenrateTokenResponse = TKE.SC.Common.Model.UIModel.GenrateTokenResponse;
using UserInfo = TKE.SC.Common.Model.UserInfo;
using UserInfoUI = TKE.SC.Common.Model.UIModel.UserInfo;
using Utility = TKE.SC.BFF.BusinessProcess.Helpers.Utility;
using Microsoft.Extensions.Localization;
using System.Reflection;
using Microsoft.Extensions.Logging;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class GenerateTokenBL : IGenerateToken
    {
        /// <summary>
        /// Variable Collection
        /// </summary>
        #region Variables

        private readonly DistributedCacheEntryOptions _cacheOptions;
        private readonly IConfigurationSection _configuration;
        private readonly IConfiguration _generalConfiguration;
        private readonly JsonSerializer _serializer;
        private readonly string _environment;
        private readonly ICacheManager _cpqCacheManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<GenerateTokenBL> _localizer;

        #endregion

        /// <summary>
        ///  Constructor for the class GenerateTokenBL
        /// </summary>
        /// <param Name="configuration"></param>
        /// <param Name="serializer"></param>
        /// <param Name="cpqCacheManager"></param>
        /// <param Name="httpContextAccessor"></param>
        /// <param Name="localizer"></param>
        /// <param Name="logger"></param>
        public GenerateTokenBL(IConfiguration configuration,
              JsonSerializer serializer, 
             ICacheManager cpqCacheManager, IHttpContextAccessor httpContextAccessor, IStringLocalizer<GenerateTokenBL> localizer,ILogger<GenerateTokenBL> logger)
        {
            _serializer = serializer;
            _configuration = configuration.GetSection(Constant.PARAMSETTINGS);
            _generalConfiguration = configuration;          
            _cpqCacheManager = cpqCacheManager;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
            _cacheOptions = null;
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
            }
            Utility.SetLogger(logger);
        }

        /// <summary>
        /// Validity Token Method
        /// </summary>
        /// <param Name="uid"></param>
        /// <param Name="timestamp"></param>
        /// <param Name="signature"></param>
        /// <param Name="distributedCache"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> ValidityToken(string locale=null,
                  string uid = null, string sessionId = null, string persona = null, bool isProdchk = false)
        {
            var methodBeginTime = Utility.LogBegin();
            var responseObj = new ResponseMessage();
            var userDetail = new UserInfoUI();
            var tokenAttributes = new GenrateTokenResponse();
            var userInfoList = new List<UserInfo>();
            var entitlements = new List<string>();
            var sessionID = sessionId;
            
            if (string.IsNullOrEmpty(sessionID))
            {

                var newSessionId = Guid.NewGuid().ToString();
                //create key bunch
                _cpqCacheManager.SetCache(newSessionId, _environment, Constant.KEYBUNCH,
                    string.Empty);
                sessionID = newSessionId;
                _cpqCacheManager.SetCache(sessionID, _environment, Constant.LOCALEWORD, locale);
            }
            else
            {
                var status = _cpqCacheManager.GetCache(sessionID, _environment, Constant.CPQ);
                if (string.IsNullOrEmpty(status))
                {
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = Constant.UNAUTHORIZED,
                        Message = Constant.SESSIONEXPIRYMESSAGE + Constant.HYPHEN + locale.Split(Constant.UNDERSCORECHAR)[0]

                    });
                }

                var cachedPersona = _cpqCacheManager.GetCache(sessionID, _environment, Constant.PERSONA);
                var cachedProdchk = _cpqCacheManager.GetCache(sessionID, _environment, Constant.ISPRODUCTIONCHECK);
                bool prodChk = Convert.ToBoolean(cachedProdchk);
                if (Utility.CheckEquals(persona, cachedPersona))
                {
                    var cachedCserDetail = _cpqCacheManager.GetCache(sessionID, _environment,
                        Constant.USERINFOCPQ);
                    userDetail = Utility.DeserializeObjectValue<UserInfoUI>(cachedCserDetail);
                        var userDetailStringValues = Utility.SerializeObjectValue(userDetail);
                        _cpqCacheManager.SetCache(sessionID, _environment, Constant.USERINFOCPQ,
                            userDetailStringValues);
                    entitlements = Utility.DeserializeObjectValue<List<string>>(
                        _cpqCacheManager.GetCache(sessionID, _environment, Constant.ENTITLEMENT));

                    var token = JObject.Parse(GetAuthToken(sessionID, userDetail, uid, entitlements));
                    tokenAttributes = new GenrateTokenResponse
                    {
                        Token = token,
                        SessionId = sessionID,
                        Persona = persona,
                        UserInfo = userDetail,
                        IsProdChk = prodChk
                    };
                    responseObj.StatusCode = Constant.SUCCESS;
                    responseObj.Response = JObject.FromObject(tokenAttributes, _serializer);
                    return responseObj;
                }
            }
            _cpqCacheManager.SetCache(sessionID, _environment, Constant.PERSONA, persona);
            _cpqCacheManager.SetCache(sessionID, _environment, Constant.ISPRODUCTIONCHECK, Convert.ToString(isProdchk));
            if (!string.IsNullOrEmpty(uid))
            {
                _cpqCacheManager.SetCache(sessionID, _environment, Constant.UID,
                    uid);
            }
            if (Utility.CheckEquals(persona, Constant.PUBLICCUSTOMER))
            {
                userInfoList = new List<UserInfo>
                {
                    new UserInfo()
                };
            }
            userDetail.UserId = uid;
            var userDetailString = Utility.SerializeObjectValue(userDetail);
            _cpqCacheManager.SetCache(sessionID, _environment, Constant.USERINFOCPQ, userDetailString);
            var jwtToken = JObject.Parse(GetAuthToken(sessionID, userDetail, uid, entitlements));
            tokenAttributes = new GenrateTokenResponse
            {
                Token = jwtToken,
                SessionId = sessionID,
                Persona = persona,
                UserInfo = userDetail,
                IsProdChk = isProdchk
            };
            responseObj.StatusCode = Constant.SUCCESS;
            responseObj.Response = JObject.FromObject(tokenAttributes, _serializer);
            Utility.LogEnd(methodBeginTime);
            return responseObj;
        }

        /// <summary>
        /// Getting Authorization token to be returned for a valid user
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="distributedCache"></param>
        /// <param Name="userDetails"></param>
      
        /// <param Name="entitlements"></param>
        /// <returns></returns>
        public string GetAuthToken(string sessionId, UserInfoUI userDetails,
            string tkeUuid, List<string> entitlements = null)
        {
            var methodBeginTime = Utility.LogBegin();
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.CPQ, Constant.CPQ);
            var unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds +
                                int.Parse(_configuration[Constant.TTL]);
            Utility.LogEnd(methodBeginTime);
            return JsonConvert.SerializeObject(new
            {
                tokeyType = Constant.BEARER,
                accessToken = string.Empty,
                ttl = unixTimestamp
            });
        }

        /// <summary>
        /// Extends session for logged in user
        /// </summary>
        /// <param Name="sessionId"></param>
        public JObject ExtendSession(string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var persona = _cpqCacheManager.GetCache(sessionId, _environment, Constant.PERSONA);
            _cpqCacheManager.ExtendUserSession(_environment, sessionId);
            var unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds +
                                int.Parse(_configuration[Constant.TTL]);
            Utility.LogEnd(methodBeginTime);
            return JObject.FromObject(new
            {
                tokeyType = Constant.BEARER,
                sessionId = sessionId,
                persona = persona,
                ttl = unixTimestamp
            });
        }

        /// <summary>
        /// UI Appsettings
        /// </summary>
        /// <returns></returns>
        public List<IConfigurationSection> UIAppsettings()
        {
            var methodBeginTime = Utility.LogBegin();
            Utility.LogEnd(methodBeginTime);
            return _generalConfiguration.GetSection(Constant.UISETTINGS).GetChildren().ToList();
        }

    }
}
