/************************************************************************************************************
************************************************************************************************************
    File Name     :   ConfiguratorServiceDL class 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Configit.Configurator.Server.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.HttpClientModel;
using TKE.SC.Common;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class ConfiguratorServiceDL : IConfiguratorService
    {
        /// <summary>
        /// Variables Collections
        /// </summary>
        #region Variables

        private IConfigurationSection _configuration;
        private readonly IConfiguration _iconfiguration;
        private readonly string _environment;
        private readonly ICacheManager _cpqCacheManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;

        #endregion

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param Name="configuration"></param>
        /// <param Name="cpqCacheManager"></param>
        /// <param Name="httpContextAccessor"></param>
        /// <param Name="clientFactory"></param>
        /// <param Name="logger"></param>
        public ConfiguratorServiceDL(IConfiguration configuration, ICacheManager cpqCacheManager,
            IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, ILogger<ConfiguratorServiceDL> logger)  /*,IAccessToken wrapper)*/
        {
            _configuration = configuration?.GetSection(Constant.PARAMSETTINGS);
            _cpqCacheManager = cpqCacheManager;
            if (_configuration != null)
            {
                _environment = _configuration[Constant.ENVIRONMENT];
            }
            _httpContextAccessor = httpContextAccessor;
            _iconfiguration = configuration;
            _clientFactory = clientFactory;
            Utility.SetLogger(logger);
            // _wrapper = wrapper;
        }


        /// <inheritdoc />
        /// <summary>
        /// Data layer for Sublines API
        /// </summary>
        /// <param Name="request"></param>
        /// <param Name="storageCredentials"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> RequestConfigurations(SublinesRequest request, string packagePath)
        {
            var methodBeginTime = Utility.LogBegin();
            var packagePathValue = WebUtility.UrlEncode(packagePath);




            var response = await Utility.InitializeHttpRequest(_clientFactory.CreateClient(), packagePathValue, Constant.POST, new Uri(_configuration[Constant.CONFIGURATORURL] + Constant.SLASH)
            , "configurator/v1/sublines" + "?" + Constant.PACKAGEPATH + Constant.EQUALTO + _configuration["ConnectMachine"] + packagePathValue.ToLower() + Constant.SLASH + packagePathValue
            , JObject.FromObject(request), null).ConfigureAwait(false);
            var result = await Utility.MapResponse(response).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                Utility.LogEnd(methodBeginTime);
                return result;
            }
            Utility.LogEnd(methodBeginTime);
            throw new ConfiguratorServiceException(result) { Description = Constants.CONFIGSERVICEEXCEPTION };
        }

        /// <summary>
        /// RequestEmptyCarWeightCalculation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="packagePath"></param>
        /// <param name="sessionId"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> RequestEmptyCarWeightCalculation(ConfigureRequest request, string endPoint)
        {
            var methodBeginTime = Utility.LogBegin();
            var carWeightSettings = Utility.GetSection(_configuration, Constant.SYSTEMVALIDATIONSETTINGS);

            var requestObject = new HttpClientRequestModel()
            {
                BaseUrl = Utility.GetPropertyValue(carWeightSettings, Constant.BASEURL),
                EndPoint = Utility.GetPropertyValue(carWeightSettings, endPoint),
                //This is a hack to overcome the datatype issue
                RequestBody = JObject.Parse(Utility.SerializeObjectValue(JObject.FromObject(request, new Newtonsoft.Json.JsonSerializer()))),
                Method = HTTPMETHODTYPE.POST,
                Proxy = Utility.GetPropertyValue(carWeightSettings, Constant.PROXYURI),
            };
            // This is a hack to overcome the datatype issue
            requestObject.RequestBody["line"]["quantity"]["value"] = (int)requestObject.RequestBody["line"]["quantity"]["value"];
            var apiResponse = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(false);
            var result = await Utility.MapResponse(apiResponse).ConfigureAwait(false);
            if (apiResponse.IsSuccessStatusCode)
            {
                Utility.LogEnd(methodBeginTime);
                return result;
            }
            Utility.LogEnd(methodBeginTime);
            throw new ConfiguratorServiceException(result) { Description = Constants.CONFIGSERVICEEXCEPTION };
        }


        /// <inheritdoc />
        /// <summary>
        /// Data layer for Configure API
        /// </summary>
        /// <param Name="request"></param>
        /// <param Name="packagePath"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> Configure(ConfigureRequest request, string packagePath)
        {
            var methodBeginTime = Utility.LogBegin();
            var response = await ConfigureOrPrice(JObject.FromObject(request), packagePath, Constant.CONFIGURE).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return response;
        }

        /// <summary>
        /// common method for configure and price call
        /// </summary>
        /// <param Name="request"></param>
        /// <param Name="packagePath"></param>
        /// <param Name="configureOrPrice"></param>
        /// <returns></returns>
        private async Task<ResponseMessage> ConfigureOrPrice(JObject request, string packagePath, string configureOrPrice)
        {
            var methodBeginTime = Utility.LogBegin();
            var packagePathValue = WebUtility.UrlEncode(packagePath);
            HttpResponseMessage response;
            //string configuration = string.Empty;
            //if (Utility.CheckEquals(packagePath, "SysVal_Evo200"))
            //{
            //    configuration = _configuration[Constant.CONFIGURATORSERVICESYSTEMVALIDATIONURL];
            //}
            //else
            //{
            //    configuration = _configuration[Constant.CONFIGURATORURL];
            //}
            response = await Utility.InitializeHttpRequest(_clientFactory.CreateClient(), packagePathValue, Constant.POST, new Uri(_configuration[Constant.CONFIGURATORURL] + Constant.SLASH)
                    , configureOrPrice + "?" + Constant.PACKAGEPATH + Constant.EQUALTO + packagePathValue
                    , JObject.FromObject(request), null).ConfigureAwait(false);
            var result = await Utility.MapResponse(response).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                Utility.LogEnd(methodBeginTime);
                return result;
            }
            else
            {
                response = await Utility.InitializeHttpRequest(_clientFactory.CreateClient(), packagePathValue, Constant.POST, new Uri(_configuration[Constant.CONFIGURATORURL] + Constant.SLASH)
                    , configureOrPrice + "?" + Constant.PACKAGEPATH + Constant.EQUALTO + packagePathValue
                    , JObject.FromObject(request), null).ConfigureAwait(false);
                result = await Utility.MapResponse(response).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    Utility.LogEnd(methodBeginTime);
                    return result;
                }
            }
            Utility.LogEnd(methodBeginTime);
            throw new ConfiguratorServiceException(result) { Description = Constants.CONFIGSERVICEEXCEPTION };
        }

        /// <inheritdoc />
        /// <summary>
        /// Data layer for Price API
        /// </summary>
        /// <param Name="request"></param>
        /// <param Name="storageCredentials"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> Price(PriceRequest request, string packagePath)
        {
            var methodBeginTime = Utility.LogBegin();
            var response = await ConfigureOrPrice(JObject.FromObject(request), packagePath, Constant.PRICE).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return response;
        }
    }
}