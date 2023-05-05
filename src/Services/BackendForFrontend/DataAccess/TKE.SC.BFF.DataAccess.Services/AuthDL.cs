/************************************************************************************************************
************************************************************************************************************
    File Name     :   AuthDL class 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Database;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class AuthDL : IAuthDL
    {
        /// <summary>
        /// Constructor for Auth DL
        /// </summary>
        /// <param Name="logger"></param>
        public AuthDL(ILogger<AuthDL> logger)
        {
            Utility.SetLogger(logger);

        }
      
        public async Task<ResponseMessage> GetUserDetails(User userDetailsModel)
        {
            var methodStartTime = Utility.LogBegin();
            userDetailsModel.Location = new Location();
            userDetailsModel.Role = new Role();
            List<SqlParameter> lstSqlParameter = new List<SqlParameter>();

            DataSet dataSet = new DataSet();
            DataTable groupGuidDataTable = Utility.DataTableForGuids(userDetailsModel.Groups);
            lstSqlParameter = Utility.SqlParameterForGetUserDetails(groupGuidDataTable);
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETUSERDETAILS, lstSqlParameter);

            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                //user.Id = Convert.ToInt32(dataSet.Tables[0].Rows[0]["Id"]);
                userDetailsModel.Location.id = dataSet.Tables[0].Rows[0][Constant.LOCATIONID].ToString();
                userDetailsModel.Location.Name = dataSet.Tables[0].Rows[0][Constant.LOCATIONNAME].ToString();
                userDetailsModel.Location.city = dataSet.Tables[0].Rows[0][Constant.CITY].ToString().Trim();
                userDetailsModel.Location.state = dataSet.Tables[0].Rows[0][Constant.STATE].ToString().Trim();
                userDetailsModel.Location.country = dataSet.Tables[0].Rows[0][Constant.COUNTRY].ToString().Trim();
                userDetailsModel.Role.id = dataSet.Tables[1].Rows[0][Constant.ROLEKEY].ToString();
                userDetailsModel.Role.name = dataSet.Tables[1].Rows[0][Constant.USERROLENAME].ToString();
                if (!Utility.CheckEquals(userDetailsModel.Location.country, Constant.CANADA))
                {
                    userDetailsModel.IsViewUser = true;
                }
                var response = JObject.FromObject(userDetailsModel);
                Utility.LogEnd(methodStartTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, Message = Constant.REQUESTGENERATEDSUCCESSFULLY, Response = response, IsSuccessStatusCode = true };

            }
            else
            {
                Utility.LogEnd(methodStartTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.NOTFOUND,
                    Message = Constant.USERNOTFOUND,
                });
            }
        }
    }
}
