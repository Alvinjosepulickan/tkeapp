/************************************************************************************************************
************************************************************************************************************
    File Name     :   IOpeningLocation
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model;
using System.Threading.Tasks;
using TKE.SC.Common.Model.ViewModel;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IOpeningLocation
    {
        /// <summary>
        /// method to update opening location
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="openingLocation"></param>
        /// <returns></returns>
        Task<ResponseMessage> UpdateOpeningLocation(OpeningLocations openingLocation, string sessionId);

        /// <summary>
        /// method to get opening location
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetOpeningLocationByGroupId(int GroupConfigurationId,string sessionId);

    }
}
