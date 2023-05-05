/************************************************************************************************************
************************************************************************************************************
    File Name     :   IConfigureServices.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

using Configit.Configurator.Server.Common;
using System.Collections.Generic;
using TKE.SC.Common.Model.ViewModel;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IConfigureServices
    {
        /// <summary>
        /// Interface for characteristic Business Layer
        /// </summary>
        /// <param Name="modelNumber"></param>
        /// <param Name="locale"></param>
        /// <param Name="characteristics"></param>
        /// <param Name="mulesoftToken"></param>
        /// <returns></returns>
        //Task<ResponseMessage> ServiceCharacteristicValuesBl(string modelNumber, string locale,
        //    CharacteristicRequest characteristics, string sessionId);

        /// <summary>
        /// Interface for Sublines Business Layer
        /// </summary>
        /// <param Name="storageCredentials"></param>
        /// <param Name="request"></param>
        /// <returns></returns>
        //Task<ResponseMessage> ServiceSublinesBl(string packagePath, SublineRequest request);

        /// <summary>
        /// Interface for Price Business Layer
        /// </summary>
        /// <param Name="storageCredentials"></param>
        /// <param Name="request"></param>
        /// <returns></returns>
        //Task<ResponseMessage> ServicePriceBl(string packagePath, PricesRequest request, string brand = null);

        /// <summary>
        /// Interface for Configure Business Layer
        /// </summary>
        /// <param Name="storageCredentials"></param>
        /// <param Name="request"></param>
        /// <returns></returns>
        //Task<ResponseMessage> ServiceConfigurationBl(string packagePath, ConfigurationRequest request,
            //string sessionId, string brand);

        /// <summary>
        /// Interface for StartConfigure Business Layer
        /// </summary>
        /// <param Name="configureRequest"></param>
        /// <param Name="modelNumber"></param>
        /// <param Name="locale"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="mulesoftToken"></param>
        /// <param Name="distributedCache"></param>
        /// <returns></returns>
        //Task<ResponseMessage> ServiceStartConfigureBl(ServiceRequest configureRequest, string modelNumber,
        //    string locale, string sessionId,
        //    string parentCode, string AgoUuid, bool isChanged, bool retrieve = false, string brand = null,
        //    bool isInternal = false);

        /// <summary>
        /// Interface for Service Product Type Business Layer
        /// </summary>
        /// <param Name="modelNumber"></param>
        /// <param Name="locale"></param>
        /// <param Name="mulesoftToken"></param>
        /// <param Name="dealerId"></param>
        /// <returns></returns>
        //Task<ResponseMessage> ServiceProductTypeBl(string modelNumber, string locale,
            //string dealerLocationId, string persona, string brand = null);

        /// <summary>
        ///  Interface for Merging Service Product and Service List
        ///  </summary>
        ///  <param Name="modelNumber"></param>
        ///  <param Name="locale"></param>
        ///  <param Name="mulesoftToken"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="distributedCache"></param>
        /// <returns></returns>
        //Task<ResponseMessage> ServiceListWithProductTypeBl(string modelNumber, string locale,
        //    string sessionId, string parentCode, string brand = null);

        /// <summary>
        /// Interface for Merging Service Product and Service List
        /// </summary>
        /// <param Name="request"></param>
        /// <param Name="modelNumber"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="distributedCache"></param>
        /// <returns></returns>
        //Task<ResponseMessage> ServiceResetingBl(ServiceRequest request, string modelNumber, string locale,
        //    string sessionId, string parentCode, string brand = null);

        /// <summary>
        /// Sets the configuration after conflicts
        /// </summary>
        /// <param Name="isApplied"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="distributedCache"></param>
        /// <param Name="materialNumber"></param>
        /// <param Name="parentCode"></param>
        /// <returns></returns>
        //Task<ResponseMessage> SetServiceConfigurationBL(bool isApplied, string sessionId,
        //   string materialNumber, string parentCode, string locale);
        //Task<ServiceGroup> GetServiceProductTypeResponse(string sessionId,
        //    string parentCode, string modelNumber, string persona, string locale, string brand = null);
        /// <summary>
        /// generate base request
        /// </summary>
        /// <param Name="baseConfigureRequest"></param>
        /// <param Name="modelNumber"></param>
        /// <param Name="model"></param>
        /// <param Name="configServiceRequest"></param>
        /// <returns></returns>
        /// 

        ConfigurationRequest GetBaseConfigureRequest(ConfigurationRequest baseConfigureRequest,
            string model = null, bool configServiceRequest = false,
             string machinePackagePath = null);
        /// <summary>
        /// This method verifies the completeness of configuration and returns is complete or not
        /// </summary>
        /// <param Name="configureResponse"></param>
        /// <returns>returns whether the configuration  is complete or not</returns>
        bool IsComplete(ConfigureResponse configureResponse);

        /// <summary>
        /// generate base request
        /// </summary>
        /// <param Name="modelNumber"></param>
        /// <param Name="packagePath"></param>
        /// <param Name="discountFlag"></param>
        /// <param Name="model"></param>
        /// <param Name="configServiceRequest"></param>
        /// <returns></returns>

        //ConfigurationRequest GenerateBaseconfigureRequest(string modelNumber, string packagePath,
        //    bool discountFlag, string model,
        //   string dealerAccountNumber = null, string countryCode = null, string machinePackagePath = null);
    }
}
