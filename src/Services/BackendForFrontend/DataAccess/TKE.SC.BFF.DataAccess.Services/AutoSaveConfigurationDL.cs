/************************************************************************************************************
************************************************************************************************************
    File Name     :   AutoSaveConfigurationDL class 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Database;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class AutoSaveConfigurationDL : IAutoSaveConfigurationDL
    {

        /// <summary>
        /// Constructor for AutoSaveConfigurationDL
        /// </summary>
        /// <param Name="logger"></param>
        public AutoSaveConfigurationDL(ILogger<AutoSaveConfigurationDL> logger)
        {
            Utility.SetLogger(logger);

        }
        /// <summary>
        /// data layer method for auto save
        /// </summary>
        /// <param Name="autoSaveRequest"></param>
        /// <returns></returns>
        public int AutoSaveConfiguration(AutoSaveConfiguration autoSaveRequest)
        {
            var methodStartTime = Utility.LogBegin();
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForAutoSaveConfiguration(autoSaveRequest);
            int resultForAutoSaveConfiguration = CpqDatabaseManager.ExecuteNonquery(Constant.SPAUTOSAVECONFIGURATION, lstSqlParameter);
            Utility.LogEnd(methodStartTime);
            return resultForAutoSaveConfiguration;
        }


        /// <summary>
        /// data layer method for deleting auto save configuration
        /// </summary>
        /// <param Name="userName"></param>
        /// <returns></returns>
        public int DeleteAutoSaveConfigurationByUser(string userName)
        {
            var methodStartTime = Utility.LogBegin();
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForDeletingAutoSaveConfiguration(userName);
            _ = CpqDatabaseManager.ExecuteNonquery(Constant.SPDELETEAUTOSAVECONFIGURATION, lstSqlParameter);
            int result = 1;
            Utility.LogEnd(methodStartTime);
            return result;
        }


        /// <summary>
        /// data layer method for fetchingauto save data
        /// </summary>
        /// <param Name="userName"></param>
        /// <returns></returns>
        public AutoSaveConfiguration GetAutoSaveConfigurationByUser(string userName)
        {
            var methodStartTime = Utility.LogBegin();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@USERNAME, Value = userName }
            };       
            AutoSaveConfiguration autoSaveConfigurationResult = new AutoSaveConfiguration();
            DataSet dataSet = new DataSet();
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETAUTOSAVECONFIGBYID, sqlParameters);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                autoSaveConfigurationResult.RequestMessage = JObject.FromObject(JsonConvert.DeserializeObject(dataSet.Tables[0].Rows[0][Constant.REQUESTMESSAGE].ToString()));
                autoSaveConfigurationResult.CreatedBy = dataSet.Tables[0].Rows[0][Constant.CREATEDBY].ToString();
                autoSaveConfigurationResult.CreatedOn = Convert.ToDateTime(dataSet.Tables[0].Rows[0][Constant.CREATEDON].ToString());
            }
            Utility.LogEnd(methodStartTime);
            return autoSaveConfigurationResult;
        }
    }
}
