/************************************************************************************************************
************************************************************************************************************
    File Name     :   IConfiguratorServiceDL class 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using TKE.SC.Common.Model;
using Configit.Configurator.Server.Common;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
    public interface IConfiguratorService
    {
        /// <summary>
        /// Interface for Sublines API Data layer
        /// </summary>
        /// <param Name="request"></param>
        /// <param Name="packagePath"></param>
        /// <returns></returns>
        Task<ResponseMessage> RequestConfigurations(SublinesRequest request, string packagePath);

        /// <summary>
        /// Interface for Price API Data layer
        /// </summary>
        /// <param Name="request"></param>
        /// <param Name="packagePath"></param>
        /// <returns></returns>
        Task<ResponseMessage> Price(PriceRequest request, string packagePath);

        /// <summary>
        /// Interface for Configure API Data layer
        /// </summary>
        /// <param Name="request"></param>
        /// <param Name="packagePath"></param>
        /// <returns></returns>
        Task<ResponseMessage> Configure(ConfigureRequest request, string packagePath);

        /// <summary>
        /// RequestEmptyCarWeightCalculation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="packagePath"></param>
        /// <returns></returns>
        Task<ResponseMessage> RequestEmptyCarWeightCalculation(ConfigureRequest request, string endPoint);
    }
}
