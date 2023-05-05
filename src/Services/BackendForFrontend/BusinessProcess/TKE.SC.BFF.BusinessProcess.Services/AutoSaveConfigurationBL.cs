/************************************************************************************************************
************************************************************************************************************
    File Name     :   AutoSaveConfigurationBL.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/


using Configit.Configurator.Server.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;
namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class AutoSaveConfigurationBL:IAutoSaveConfiguration
    {
        /// <summary>
        /// Variable Collection
        /// </summary>
        #region Variables
        private readonly IAutoSaveConfigurationDL _autoSaveConfigurationDL;
        private readonly IConfigure _configure;
        private readonly ILogger _logger;
        #endregion


        /// <summary>
        /// Constructor for AutoSaveConfiguration
        /// </summary>
        /// <param Name="logger"></param>
        /// <param Name="autoSaveConfigurationDL"></param>
        /// <param Name="configure"></param>
        public AutoSaveConfigurationBL(ILogger<AutoSaveConfigurationBL> logger, IAutoSaveConfigurationDL autoSaveConfigurationDL, IConfigure configure)
        {
            _autoSaveConfigurationDL = autoSaveConfigurationDL;
            _configure = configure;
            Utility.SetLogger(logger);
        }


        /// <summary>
        /// Business layer method for auto save 
        /// </summary>
        /// <param Name="autoSaveRequest"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> AutoSaveConfiguration(AutoSaveConfiguration autoSaveRequest)
        {
            try
            {
                int result = _autoSaveConfigurationDL.AutoSaveConfiguration(autoSaveRequest);
                return new ResponseMessage { StatusCode = Constant.SUCCESS };

            }
            catch (Exception ex)
            {
                return new ResponseMessage { StatusCode = Constant.BADREQUEST};
            }
        }
        
        /// <summary>
        /// Business layer method for deleting auto save data
        /// </summary>
        /// <param Name="userName"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> DeleteAutoSaveConfigurationByUser(string sessionId)
        {
            try
            {
                var userName = _configure.GetUserId(sessionId);
                int result = _autoSaveConfigurationDL.DeleteAutoSaveConfigurationByUser(userName);
                if (result.Equals(1))
                {
                    return new ResponseMessage { StatusCode = Constant.SUCCESS };
                }
                else
                {
                    return new ResponseMessage { StatusCode = Constant.INTERNALSERVERERROR };
                }
                
            }
            catch (Exception ex)
            {
                return new ResponseMessage { StatusCode = Constant.BADREQUEST};
            }
        }

        /// <summary>
        /// Business layer method for fetching auto save data by username
        /// </summary>
        /// <param Name="userName"></param>
        /// <returns></returns>
        public  async Task<ResponseMessage> GetAutoSaveConfigurationByUser(string sessionId)
        {
            try
            {
                var userName = _configure.GetUserId(sessionId);
                //Call to DB
                var autoSaveDataString = _autoSaveConfigurationDL.GetAutoSaveConfigurationByUser(userName);
                if (autoSaveDataString.CreatedOn.Equals(Convert.ToDateTime("1 / 1 / 0001 12:00:00 AM")))
                {
                    return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = null };
                }
                var autoSaveDataJObject = new JArray();
                autoSaveDataJObject.Add(JObject.FromObject(autoSaveDataString));
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = autoSaveDataJObject };


            }
            catch (Exception ex)
            {
                var response = JObject.FromObject(ex);
                return new ResponseMessage { StatusCode = Constant.BADREQUEST, Response = response };
            }
        }
    }
}
