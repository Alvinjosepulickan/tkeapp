/************************************************************************************************************
************************************************************************************************************
    File Name     :   ConfigurationServicesStubDL class 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Configit.Configurator.Server.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.BFF.Test.Common;
using TKE.SC.BFF.BusinessProcess.Helpers;
using Constant = TKE.SC.BFF.DataAccess.Helpers.Constant;

namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    public class ConfigurationServicesStubDL : IConfiguratorService
    {
        /// <inheritdoc />
        /// <summary>
        /// Data layer for Sublines API
        /// </summary>
        /// <param name="request"></param>
        /// <param name="storageCredentials"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> RequestConfigurations(SublinesRequest request, string packagePath)
        {
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Data layer for Configure API
        /// </summary>
        /// <param name="request"></param>
        /// <param name="packagePath"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> Configure(ConfigureRequest request, string packagePath)
        {
            var getResponse = File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.STARTGROUPCONFIGURATIONRESPONSEBODY));
            if (packagePath == "buildingconfiguration")
            {
                var getResponse1 = File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.STARTBUILDINGCONFIGRESPONSEBODY));
                getResponse = getResponse1;
            }
            if (packagePath == "productselection")
            {
                var getResponse1 = File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.CHANGEPRODUCTSELECTIONSTUBRESPONSEBODY));
                getResponse = getResponse1;
            }
            if (packagePath == "unitvalidation")
            {
                var getResponse1 = File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.UNITCONFIGURATIONRESPONSE));
                getResponse = getResponse1;
            }
            // var getResponse = File.ReadAllText(AppGatewayJsonFilePath.CHANGECONFIGUREOUTPUT);
            if (request == null)
            {
                return new ResponseMessage
                {
                    StatusCode = BFF.BusinessProcess.Helpers.Constant.BADREQUEST,
                    Response = JObject.Parse(getResponse)
                };
            }
            return new ResponseMessage
            {
                StatusCode = BFF.BusinessProcess.Helpers.Constant.SUCCESS,
                Response = JObject.Parse(getResponse)
            };
        }

        /// <summary>
        /// common method for configure and price call
        /// </summary>
        /// <param name="request"></param>
        /// <param name="packagePath"></param>
        /// <param name="configureOrPrice"></param>

        
        /// <returns></returns>
        private async Task<ResponseMessage> ConfigureOrPrice(JObject request, string packagePath, string configureOrPrice)
        {
            var getResponse = File.ReadAllText(AppGatewayJsonFilePath.CHANGECONFIGUREOUTPUT);
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Data layer for Price API
        /// </summary>
        /// <param name="request"></param>
        /// <param name="storageCredentials"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> Price(PriceRequest request, string packagePath)
        {
            return null;
        }

        Task<ResponseMessage> IConfiguratorService.RequestConfigurations(SublinesRequest request, string packagePath)
        {
            throw new NotImplementedException();
        }

        Task<ResponseMessage> IConfiguratorService.Price(PriceRequest request, string packagePath)
        {
            throw new NotImplementedException();
        }


        async Task<ResponseMessage> IConfiguratorService.Configure(ConfigureRequest request, string packagePath)
        {
            var getResponse = File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.STARTGROUPCONFIGURATIONRESPONSEBODY));
            if (packagePath == "buildingconfiguration")
            {
                var getResponse1 = File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.STARTBUILDINGCONFIGRESPONSEBODY));
                getResponse = getResponse1;
            }
            if (packagePath == "productselection")
            {
                var getResponse1 = File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.CHANGEPRODUCTSELECTIONSTUBRESPONSEBODY));
                getResponse = getResponse1;
            }
            if (packagePath == "unitvalidation")
            {
                var getResponse1 = File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.UNITCONFIGURATIONRESPONSE));
                getResponse = getResponse1;
            }
            // var getResponse = File.ReadAllText(AppGatewayJsonFilePath.CHANGECONFIGUREOUTPUT);
            if (request == null)
            {
                return new ResponseMessage
                {
                    StatusCode = BFF.BusinessProcess.Helpers.Constant.BADREQUEST,
                    Response = JObject.Parse(getResponse)
                };
            }
            return new ResponseMessage
            {
                StatusCode = BFF.BusinessProcess.Helpers.Constant.SUCCESS,
                Response = JObject.Parse(getResponse)
            };
        }
        //Task<ResponseMessage> IConfiguratorService.Configure(ConfigureRequest request, string packagePath)
        //{
        //    var methodBeginTime = Utility.LogBegin();
        //    var response = await ConfigureOrPrice(JObject.FromObject(request), packagePath, Constant.CONFIGURE).ConfigureAwait(false);
        //    Utility.LogEnd(methodBeginTime);
        //    return response;
        //}
        //private async Task<ResponseMessage> ConfigureOrPrice(JObject request, string packagePath, string configureOrPrice)
        //{
        //    var methodBeginTime = Utility.LogBegin();
        //    var packagePathValue = WebUtility.UrlEncode(packagePath);
        //    HttpResponseMessage response;
        //    //string configuration = string.Empty;
        //    //if (Utility.CheckEquals(packagePath, "SysVal_Evo200"))
        //    //{
        //    //    configuration = _configuration[Constant.CONFIGURATORSERVICESYSTEMVALIDATIONURL];
        //    //}
        //    //else
        //    //{
        //    //    configuration = _configuration[Constant.CONFIGURATORURL];
        //    //}
        //    response = await Utility.InitializeHttpRequest(_clientFactory.CreateClient(), packagePathValue, Constant.POST, new Uri(_configuration[Constant.CONFIGURATORURL] + Constant.SLASH)
        //            , configureOrPrice + "?" + Constant.PACKAGEPATH + Constant.EQUALTO + packagePathValue
        //            , JObject.FromObject(request), null).ConfigureAwait(false);
        //    var result = await Utility.MapResponse(response).ConfigureAwait(false);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        Utility.LogEnd(methodBeginTime);
        //        return result;
        //    }
        //    else
        //    {
        //        response = await Utility.InitializeHttpRequest(_clientFactory.CreateClient(), packagePathValue, Constant.POST, new Uri(_configuration[Constant.CONFIGURATORURL] + Constant.SLASH)
        //            , configureOrPrice + "?" + Constant.PACKAGEPATH + Constant.EQUALTO + packagePathValue
        //            , JObject.FromObject(request), null).ConfigureAwait(false);
        //        result = await Utility.MapResponse(response).ConfigureAwait(false);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            Utility.LogEnd(methodBeginTime);
        //            return result;
        //        }
        //    }
        //    Utility.LogEnd(methodBeginTime);
        //    throw new ConfiguratorServiceException(result) { Description = Constants.CONFIGSERVICEEXCEPTION };
        //}



        Task<ResponseMessage> IConfiguratorService.RequestEmptyCarWeightCalculation(ConfigureRequest request, string endPoint)
        {
            throw new NotImplementedException();
        }
    }
}
