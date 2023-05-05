/************************************************************************************************************
************************************************************************************************************
    File Name     :   IAutoSaveConfiguration.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

using System.Threading.Tasks;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IAutoSaveConfiguration
    {
        /// <summary>
        /// Business layer interface method for autosave
        /// </summary>
        /// <param Name="autoSave"></param>
        /// <returns></returns>
        Task<ResponseMessage> AutoSaveConfiguration(AutoSaveConfiguration autoSaveRequest);

        /// <summary>
        /// Business layer Interface method for deleting auto save data by username
        /// </summary>
        /// <param Name="userName"></param>
        /// <returns></returns>
        Task<ResponseMessage> DeleteAutoSaveConfigurationByUser(string sessionId);


        /// <summary>
        /// business layer Interface method for fetching auto save data by username
        /// </summary>
        /// <param Name="userName"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetAutoSaveConfigurationByUser(string sessionId);

    }
}
