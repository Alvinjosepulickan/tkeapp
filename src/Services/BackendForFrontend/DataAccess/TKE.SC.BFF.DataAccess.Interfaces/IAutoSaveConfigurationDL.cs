/************************************************************************************************************
************************************************************************************************************
    File Name     :   IAutoSaveConfigurationDL class 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model;
using System.Threading.Tasks;
using Configit.Configurator.Server.Common;
using TKE.SC.Common.Model.ViewModel;
using System.Data;

namespace TKE.SC.BFF.DataAccess.Interfaces
{
    public interface IAutoSaveConfigurationDL
    {
        /// <summary>
        /// data layer interface method for autosave
        /// </summary>
        /// <returns></returns>
        int AutoSaveConfiguration(AutoSaveConfiguration autoSaveRequest);

        /// <summary>
        /// data layer interface method for deleting auto save data
        /// </summary>
        /// <param Name="userName"></param>
        /// <returns></returns>
        int DeleteAutoSaveConfigurationByUser(string userName);

        /// <summary>
        /// data layer interface method for fetching auto save data
        /// </summary>
        /// <param Name="userName"></param>
        /// <returns></returns>
        AutoSaveConfiguration GetAutoSaveConfigurationByUser(string userName);
    }
}
