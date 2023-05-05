/************************************************************************************************************
************************************************************************************************************
    File Name     :   GroupConfigurationDL class 
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
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Database;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.UIModel;
using System.IO;
using TKE.SC.Common;
using TKE.SC.Common.Caching;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;

namespace TKE.SC.BFF.DataAccess.Services
{

    public class GroupConfigurationDL : IGroupConfigurationDL
    {

        private readonly Utility _utility;
        private readonly ICacheManager _cpqCacheManager;
        private readonly string _environment;

        /// <summary>
        /// Constructor method for GroupConfigurationDL
        /// </summary>
        /// <param Name="logger"></param>
        public GroupConfigurationDL(ILogger<GroupConfigurationDL> logger, ICacheManager cpqCacheManager)
        {
            _cpqCacheManager = cpqCacheManager;
            _environment = Constant.DEV;
            Utility.SetLogger(logger);
        }

        /// <summary>
        /// To Get Group Configuration Details by GroupConfigurationId
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <returns></returns>
        public GroupLayout GetGroupConfigurationDetailsByGroupId(int groupConfigurationId, string selectedTab, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var groupMapperVariables = Utility.VariableMapper(Constant.GROUPMAPPERVARIABLES, Constant.GROUPMAPPER);
            GroupLayout groupLayout = new GroupLayout();
            List<ConfigVariable> listGroup = new List<ConfigVariable>();
            var fdaData = new List<ConfigVariable>();
            IList<SqlParameter> parameterList = new List<SqlParameter>();
            DataSet dataSet = new DataSet();
            if (selectedTab == Constant.GROUPCONFIGURATION)
            {
                SqlParameter param = new SqlParameter(Constant.@_ID, groupConfigurationId);
                parameterList.Add(param);
                dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETGROUPCONFIGBYGROUPID, parameterList);
                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    listGroup = JsonConvert.DeserializeObject<List<ConfigVariable>>(dataSet.Tables[0].Rows[0][Constant.GROUPJSON].ToString());
                }
            }
            else
            {
                SqlParameter param = new SqlParameter(Constant.@_ID, groupConfigurationId);
                parameterList.Add(param);
                DisplayVariableAssignmentsValues mappedLocation = new DisplayVariableAssignmentsValues();
                List<DisplayVariableAssignmentsValues> mappedLocations = new List<DisplayVariableAssignmentsValues>();
                dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETGROUPLAYOUTCONFIGURATIONID, parameterList);
                var unitDesignation = new Dictionary<string, string>();
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    StringBuilder jsonBuilder = new StringBuilder();
                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {

                        if (!(Convert.ToString(dataSet.Tables[0].Rows[i][Constant.UNITJSON])).Equals(Constant.EMPTYSTRING))
                        {
                            jsonBuilder.Append(Convert.ToString(dataSet.Tables[0].Rows[i][Constant.UNITJSON]) + Constant.COMA);
                            mappedLocation = Utility.DeserializeObjectValue<DisplayVariableAssignmentsValues>(Convert.ToString(dataSet.Tables[0].Rows[i][Constant.UNITJSON]));
                        }
                        if (!(Convert.ToString(dataSet.Tables[0].Rows[i][Constant.UNITDesignation])).Equals(Constant.EMPTYSTRING))
                        {
                            var mappedLocationPosition = Convert.ToString(dataSet.Tables[0].Rows[i][Constant.MAPPEDLOCATION]);
                            if (!unitDesignation.ContainsKey(mappedLocationPosition))
                            {
                                unitDesignation.Add(mappedLocationPosition, Convert.ToString(dataSet.Tables[0].Rows[i][Constant.UNITDesignation]));
                            }
                        }
                        if (!(Convert.ToString(dataSet.Tables[0].Rows[i][Constant.DISPLAYJSON])).Equals(Constant.EMPTYSTRING))
                        {
                            mappedLocations = Utility.DeserializeObjectValue<List<DisplayVariableAssignmentsValues>>(dataSet.Tables[0].Rows[i][Constant.DISPLAYJSON].ToString());
                        }
                        if (!(Convert.ToString(dataSet.Tables[0].Rows[i][Constant.HALLRISERJSON])).Equals(Constant.EMPTYSTRING))
                        {
                            jsonBuilder.Append(Convert.ToString(dataSet.Tables[0].Rows[i][Constant.HALLRISERJSON]) + Constant.COMA);
                        }
                        if (!(Convert.ToString(dataSet.Tables[0].Rows[i][Constant.DOORJSON])).Equals(Constant.EMPTYSTRING))
                        {
                            jsonBuilder.Append(Convert.ToString(dataSet.Tables[0].Rows[i][Constant.DOORJSON]) + Constant.COMA);
                        }
                    }
                    foreach (var location in mappedLocations)
                    {
                        var positionList = location.VariableId.Split(Constant.DOT);
                        if (unitDesignation.ContainsKey(positionList[positionList.Count() - 1]))
                        {
                            location.UnitDesignation = unitDesignation[positionList[positionList.Count() - 1]];
                            location.Value = Constant.TRUE_UPPERCASE;
                        }
                        else
                        {
                            location.UnitDesignation = string.Empty;
                            location.Value = Constant.FALSE_UPPERCASE;
                        }
                    }
                    int bankTwoposition = 1;
                    foreach (var location in mappedLocations)
                    {
                        if (Convert.ToBoolean(location.Value))
                        {
                            if (location.VariableId.Contains(Constant.B2P))
                            {
                                location.MappedTo = location.MappedTo.Substring(0, location.MappedTo.Length - 1) + Convert.ToString(bankTwoposition);
                                bankTwoposition += 1;
                            }
                        }
                    }
                    if (dataSet.Tables[1].Rows.Count > 0)
                    {
                        for (int i = 0; i < dataSet.Tables[1].Rows.Count; i++)
                        {
                            jsonBuilder.Append(Convert.ToString(dataSet.Tables[1].Rows[i][Constant.CONTROLLOCATIONJSON]) + Constant.COMA);
                        }
                    }
                    string jsonData = Constant.OPENINGSQUAREBRACKET + jsonBuilder.ToString() + Constant.CLOSINGSQUAREBRACKET;
                    if (dataSet.Tables[2].Rows.Count > 0)
                    {
                        groupLayout.UpdatedTotalNumberOfFloors = (Int32)dataSet.Tables[2].Rows[0][Constant.TOTALNUMBEROFFLOORS];
                    }
                    if (dataSet.Tables[3].Rows.Count > 0)
                    {
                        listGroup.AddRange(JsonConvert.DeserializeObject<List<ConfigVariable>>(Convert.ToString(dataSet.Tables[3].Rows[0][Constant.GROUPJSON])));
                        listGroup = listGroup.GroupBy(group => group.VariableId).Select(g => g.Last()).ToList();
                        fdaData = listGroup;
                        fdaData = fdaData.Except(fdaData.Where(unit => unit.VariableId == groupMapperVariables[Constant.GROUPDESGN])).ToList();
                    }
                    listGroup = JsonConvert.DeserializeObject<List<ConfigVariable>>(jsonData.Replace(Constant.CLOSINGSQUAREBRACKETWITHCOMA, Constant.CLOSINGSQUAREBRACKET).ToString());
                    listGroup.AddRange(fdaData);
                    listGroup = listGroup.GroupBy(group => group.VariableId).Select(g => g.First()).ToList();
                    groupLayout.DisplayVariableAssignmentsValues = mappedLocations;

                    if(dataSet.Tables[4].Rows.Count>0)
                    {
                        var editFlag = (Int32)dataSet.Tables[4].Rows[0][Constant.ISEDITFLOW];
                        _cpqCacheManager.SetCache(sessionId, _environment, groupConfigurationId.ToString(), Constants.EDITFLAGFORGROUP, Utility.SerializeObjectValue(editFlag));
                    }
                    if (dataSet.Tables[5] != null && dataSet.Tables[5].Rows.Count > 0)
                    {
                        var conflictVaribales = new List<string>();
                        foreach (DataRow item in dataSet.Tables[5].Rows)
                        {
                            conflictVaribales.Add(item[Constants.CONFLICTVARIABLEIDDATA].ToString());
                        }
                        groupLayout.VariableIds = conflictVaribales != null && conflictVaribales.Any() ? conflictVaribales : new List<string>();
                    }
                    if (dataSet.Tables[6].Rows.Count > 0)
                    {
                        var editFlag = (Int32)dataSet.Tables[6].Rows[0]["entrancesExist"];
                        _cpqCacheManager.SetCache(sessionId, _environment, groupConfigurationId.ToString(), "EntrancesExist", Utility.SerializeObjectValue(editFlag));
                    }
                }
            }
            groupLayout.ConfigVariable = listGroup;
            Utility.LogEnd(methodBeginTime);
            return groupLayout;
        }

        /// <summary>
        /// This method is used to save group configuration and FloorPlan
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="GroupName"></param>
        /// <param Name="userName"></param>
        /// <param Name="grpVariablejson"></param>
        /// <returns></returns>
        public List<ResultGroupConfiguration> SaveGroupConfiguration(int buildingId, string groupName, string userName, string grpVariablejson, string productCategory, int numberOfUnits)
        {
            var methodBeginTime = Utility.LogBegin();
            ResultGroupConfiguration result = new ResultGroupConfiguration();
            List<ResultGroupConfiguration> lstResult = new List<ResultGroupConfiguration>();
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSavingGroupConfiguration(buildingId, groupName, userName, grpVariablejson);
            int resultForSaveGroupConfiguration = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVEGROUPCONFIGURATION, lstSqlParameter);
            if (resultForSaveGroupConfiguration > 0)
            {
                if (string.IsNullOrEmpty(productCategory))
                    productCategory = Constant.PRODUCTELEVATOR;

                if (Utility.CheckEquals(productCategory, Constant.PRODUCTELEVATOR))
                {
                    result.Result = 1;
                    result.GroupConfigurationId = resultForSaveGroupConfiguration;
                    result.Message = Constant.GROUPSAVEMESSAGE;
                    result.Description = Constant.GROUPSAVEMESSAGE;
                }
                else if (Utility.NonConfigurableProducts.Contains(productCategory))
                {
                    IList<SqlParameter> lstNCPSqlParameter = Utility.SqlParameterForSavingUnitsForNCP(resultForSaveGroupConfiguration, numberOfUnits, userName);
                    DataSet ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPSAVEUNITSFORNONCONFIGURABLEPRODUCTS, lstNCPSqlParameter);
                    result.Result = 1;
                    result.GroupConfigurationId = resultForSaveGroupConfiguration;

                    foreach (DataTable table in ds.Tables)
                    {
                        if (table.Columns.Contains(Constant.SETIDPASCALCASE))
                        {
                            if (!Convert.ToBoolean(table.Rows.Count))
                            {
                                throw new CustomException(new ResponseMessage
                                {
                                    StatusCode = Constant.BADREQUEST,
                                    Message = Constant.ERRORREQUESTMESSAGE,
                                    Description = Constant.ERRORREQUESTMESSAGE,
                                });
                            }

                            result.SetId = table.Rows[0].Field<int>(Constant.SETIDPASCALCASE);
                            break;
                        }
                    }

                    result.Message = Constant.GROUPSAVEMESSAGE;
                    result.Description = Constant.GROUPSAVEMESSAGE;
                }
            }
            else if (resultForSaveGroupConfiguration == 0)
            {
                result.Result = 0;
                result.GroupConfigurationId = resultForSaveGroupConfiguration;
                result.Message = Constant.ERRORSAVEMESSAGE;
                result.Description = Constant.ERRORSAVEMESSAGE;
            }
            else if (resultForSaveGroupConfiguration == -2)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.ERRORBUILDINGIDINCORRECTMESSAGE,
                    Description = Constant.ERRORBUILDINGIDINCORRECTMESSAGE,
                });
            }
            else
            {
                result.Result = -1;
                result.GroupConfigurationId = resultForSaveGroupConfiguration;
                result.Message = Constant.ERRORGROUPNAMEXISTS;
                result.Description = Constant.ERRORGROUPNAMEXISTS;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBeginTime);
            return lstResult;
        }

        /// <summary>
        /// This method is used to update Group configuration details
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="GroupName"></param>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="grpVariablejson"></param>
        /// <returns></returns>
        public List<ResultGroupConfiguration> UpdateGroupConfiguration(int buildingId, string groupName, int groupConfigurationId, string grpVariablejson)
        {
            var methodBeginTime = Utility.LogBegin();
            ResultGroupConfiguration result = new ResultGroupConfiguration();
            List<ResultGroupConfiguration> lstResult = new List<ResultGroupConfiguration>();
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForUpdatingGroupConfiguration(buildingId, groupName, groupConfigurationId, grpVariablejson);
            int resultForUpdateGroupConfiguration = CpqDatabaseManager.ExecuteNonquery(Constant.SPUPDATEGROUPCONFIGURATION, lstSqlParameter);
            if (resultForUpdateGroupConfiguration == 1)
            {
                result.Result = resultForUpdateGroupConfiguration;
                result.GroupConfigurationId = groupConfigurationId;
                result.Message = Constant.GROUPUPDATEMESSAGE;
                result.Description = Constant.GROUPUPDATEMESSAGE;
            }
            else if (resultForUpdateGroupConfiguration == -1)
            {
                result.Result = resultForUpdateGroupConfiguration;
                result.GroupConfigurationId = 0;
                result.Message = Constant.ERRORGROUPNAMEXISTS;
                result.Description = Constant.ERRORGROUPNAMEXISTS;
            }
            else
            {
                result.Result = resultForUpdateGroupConfiguration;
                result.GroupConfigurationId = 0;
                result.Message = Constant.ERRORUPDATEMESSAGE;
                result.Description = Constant.ERRORUPDATEMESSAGE;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBeginTime);
            return lstResult;
        }

        /// <summary>
        /// To Delete Floor Plan by GroupConfigurationId
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> DeleteFloorPlan(string GroupConfigurationId)
        {
            int Msg = 0;
            string Message = string.Empty;
            List<FloorPlanDetails> lstFloor = new List<FloorPlanDetails>();


            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            SqlParameter parameter1 = new SqlParameter(Constant.GROUPCONFIGID, GroupConfigurationId);
            SqlParameter parameter2 = new SqlParameter(Constant.ISDELETED, true);
            sqlParameters.Add(parameter1);
            sqlParameters.Add(parameter2);
            string result = CpqDatabaseManager.ExecuteScalarForReturnString(Constant.SPDELETEFLOORPLAN, sqlParameters);

            if (result != string.Empty)
            {
                Msg = 1;
                Message = Constant.DELETEFLOORPLANSUCCESSMSG;

                List<ResultFloor> resultFloorList = new List<ResultFloor>();
                ResultFloor resultFloor = new ResultFloor();
                resultFloor.groupConfigurationId = result;
                resultFloorList.Add(resultFloor);
                var response = JArray.FromObject(resultFloorList);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response, Message = Message };
            }
            else
            {
                Msg = 0;
                Message = Constant.DELETEFLOORPLANERRORMSG;
                return new ResponseMessage
                {
                    StatusCode = Constant.UNAUTHORIZED,
                    ResponseArray = JArray.FromObject(lstFloor)
                };
            }
        }

        /// <summary>
        /// To Get Group Configuration By BuildingId
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetGroupConfigurationByBuildingId(string buildingId)
        {
            var methodBeginTime = Utility.LogBegin();
            GroupConfiguration groupConfiguration;
            List<GroupConfiguration> lstGroupConfiguration = new List<GroupConfiguration>();


            List<SqlParameter> sp = new List<SqlParameter>();
            SqlParameter param = new SqlParameter(Constant.BUILDINGID, buildingId);
            sp.Add(param);
            SqlDataReader dr = CpqDatabaseManager.ExecuteReader(Constant.SPGETGROUPCONFIGBYBUILDINGID, sp);
            DataTable dtGroupList = new DataTable();
            dtGroupList.Load(dr);

            if (dtGroupList.Rows.Count > 0)
            {
                foreach (DataRow row in dtGroupList.Rows)
                {
                    groupConfiguration = new GroupConfiguration()
                    {
                        Id = row[Constant.ID].ToString(),
                        GroupName = row[Constant.GROUPNAMELOWERCASE].ToString(),
                        ProductCategory = row[Constant.PRODUCTCATEGORY].ToString(),
                        controlLocation = row[Constant.CONTROLLOCATION].ToString(),
                        FixtureStrategy = row[Constant.FIXTURESTRATEGYVARIABLE].ToString(),
                        CreatedOn = Convert.ToDateTime(row[Constant.CREATEDON]),
                        CreatedBy = new User()
                        {
                            Id = Convert.ToInt32(row[Constant.CREATEDBY]),
                            FirstName = row[Constant.CREATEDBYFN].ToString(),
                            LastName = row[Constant.CREATEDBYLN].ToString(),
                        },
                        ModifiedOn = Convert.ToDateTime(row[Constant.MODIFIEDONCRM]),
                        ModifiedBy = new User()
                        {
                            Id = Convert.ToInt32(row[Constant.MODIFIEDBY_CAMELCASE]),
                            FirstName = row[Constant.MODIFIEDBYFN].ToString(),
                            LastName = row[Constant.MODIFIEDBYLN].ToString(),
                        },
                    };
                    lstGroupConfiguration.Add(groupConfiguration);
                }
                var response = JArray.FromObject(lstGroupConfiguration);
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.NOTFOUND,
                    Message = Constant.BUILDINGNOTFOUND,
                    ResponseArray = JArray.FromObject(lstGroupConfiguration)
                });
            }
        }

        /// <summary>
        /// To Save Floor Plan 
        /// </summary>
        /// <param Name="floorPlanData"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveFloorPlan(FloorPlan floorPlanData)
        {

            int Msg;
            string Message = "";
            List<ResultElevation> reslist = new List<ResultElevation>();
            ResultElevation res = new ResultElevation();
            if (floorPlanData == null)
            {
                Msg = 0;
                Message = Constant.SOMEFIELDSMISSINGMSG;
                res.result = Msg;
                reslist.Add(res);
                var response = JArray.FromObject(reslist);
                return new ResponseMessage { StatusCode = Constant.UNAUTHORIZED, ResponseArray = response, Message = Message };
            }



            List<SqlParameter> parameterList = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.GROUPCONFIGID,Value=floorPlanData.groupConfigurationId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar},
                new SqlParameter() { ParameterName = Constant.SELECTEDELEVATORS,Value=floorPlanData.selectedElevators,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar},
                new SqlParameter() { ParameterName = Constant.@USERID,Value=floorPlanData.createdBy.UserId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
            };

            int result = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVEFLOORPLAN, parameterList);

            if (result == -1)
            {
                Msg = 1;
                Message = Constant.SAVEFLOORPLANSUCCESSMSG;
                res.result = Msg;
                reslist.Add(res);
                var response = JArray.FromObject(reslist);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response, Message = Message };
            }
            else
            {
                Msg = 0;
                Message = Constant.SAVEFLOORPLANERRORMSG;
                res.result = Msg;
                reslist.Add(res);
                var response = JArray.FromObject(reslist);
                return new ResponseMessage { StatusCode = Constant.UNAUTHORIZED, ResponseArray = response, Message = Message };
            }
        }

        /// <summary>
        /// To Update Floor Plan 
        /// </summary>
        /// <param Name="floorPlanData"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> UpdateFloorPlan(FloorPlan floorPlanData)
        {

            int Msg;
            string Message = "";
            List<ResultElevation> resultElevations = new List<ResultElevation>();
            ResultElevation elevation = new ResultElevation();
            if (floorPlanData == null)
            {
                Msg = 0;
                Message = Constant.SOMEFIELDSMISSINGMSG;
                elevation.result = Msg;
                resultElevations.Add(elevation);
                var response = JArray.FromObject(resultElevations);
                return new ResponseMessage { StatusCode = Constant.UNAUTHORIZED, ResponseArray = response, Message = Message };
            }




            IList<SqlParameter> parameterList = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.GROUPCONFIGID,Value=floorPlanData.groupConfigurationId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar},
                new SqlParameter() { ParameterName = Constant.SELECTEDELEVATORS,Value=floorPlanData.selectedElevators,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar},
                new SqlParameter() { ParameterName = Constant.@USERID,Value=floorPlanData.modifiedBy.UserId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
            };

            int result = CpqDatabaseManager.ExecuteNonquery(Constant.SPUPDATEFLOORPLAN, parameterList);

            if (result == -1)
            {
                Msg = 1;
                Message = Constant.UPDATEFLOORPLANSUCCESSMSG;
                elevation.result = Msg;
                resultElevations.Add(elevation);
                var response = JArray.FromObject(resultElevations);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response, Message = Message };
            }
            else
            {
                Msg = 0;
                Message = Constant.UPDATEFLOORPLANERRORMSG;
                elevation.result = Msg;
                resultElevations.Add(elevation);
                var response = JArray.FromObject(resultElevations);
                return new ResponseMessage { StatusCode = Constant.UNAUTHORIZED, ResponseArray = response, Message = Message };
            }

        }

        /// <summary>
        /// To Delete Group Configuration Details By GroupId
        /// </summary>
        /// <param Name="GroupId"></param>
        /// <returns></returns>
        public List<GroupResult> DeleteGroupConfiguration(int GroupId)
        {
            var methodBeginTime = Utility.LogBegin();
            string Message = string.Empty;
            List<GroupResult> lstgroup = new List<GroupResult>();
            GroupResult resultGroupConfiguration = new GroupResult();


            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForDeleteGroup(GroupId);
            int result = CpqDatabaseManager.ExecuteNonquery(Constant.SPDELETEGROUPBYID, lstSqlParameter);

            if (result == 1)
            {
                resultGroupConfiguration.Message = Constant.DELETEGROUPCONFIGSUCCESSMSG;
                lstgroup.Add(resultGroupConfiguration);
            }
            else
            {
                resultGroupConfiguration.Message = Constant.DELETEGROUPCONFIGERRORMSG;
                lstgroup.Add(resultGroupConfiguration);
            }
            Utility.LogEnd(methodBeginTime);
            return lstgroup;
        }

        /// <summary>
        /// To Generate Group Name by BuildingId
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        public int GenerateGroupName(int buildingId)
        {
            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> sqlParam = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.BUILDINGID, Value = buildingId }
            };
            var groupCount = CpqDatabaseManager.ExecuteScalar(Constant.SPGETNUMBEROFGROUPS, sqlParam);
            Utility.LogEnd(methodBeginTime);
            return groupCount;
        }

        /// <summary>
        /// This function is used to fetch the product category by  productCategoryId
        /// </summary>
        /// <param Name="productCategoryId"></param>
        /// <returns></returns>
        public string GenerateProductCategory(int productCategoryId)
        {
            var methodBeginTime = Utility.LogBegin();


            List<SqlParameter> sqlParam = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@PRODUCTCATEGORYID, Value = productCategoryId }
            };
            var productCategory = CpqDatabaseManager.ExecuteScalarForReturnString(Constant.SPGETPRODUCTCATEGORY, sqlParam);

            Utility.LogEnd(methodBeginTime);
            return productCategory;

        }

        /// <summary>
        /// This function is used to fetch the floor designation by groupId
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        public List<BuildingElevationData> GetFloorDesignationFloorNumberByGroupId(int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            List<BuildingElevationData> lstBuildingElevationData = new List<BuildingElevationData>();


            List<SqlParameter> sqlParam = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@_ID, Value = groupId }
            };
            var ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETFLOORDESIGNATIONBYGROUPID, sqlParam);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                var NoOfFloors = Convert.ToInt32(ds.Tables[0].Rows[0][Constant.NUMBEROFFLOOR].ToString());
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var buildingElevationData = new BuildingElevationData()
                    {
                        floorDesignation = row[Constant.FLOORDESIGNATION].ToString(),
                        FloorNumber = NoOfFloors,
                    };
                    lstBuildingElevationData.Add(buildingElevationData);
                    if (NoOfFloors > 0)
                    {
                        NoOfFloors--;
                    }
                }
            }
            Utility.LogEnd(methodBeginTime);
            return lstBuildingElevationData;
        }

        /// <summary>
        /// Method is used to return the Vt package variable Id of a variable from GetGroupVariables in the database
        /// </summary>
        /// <param Name="VariableName"></param>
        /// <returns></returns>
        public string GetGroupValues(string VariableName)
        {
            var methodBeginTime = Utility.LogBegin();


            List<SqlParameter> sqlParam = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.VARIABLENAME, Value = VariableName }
            };
            var Value = CpqDatabaseManager.ExecuteScalarForReturnString(Constant.SPGETGROUPVARIABLEDETAILS, sqlParam);

            Utility.LogEnd(methodBeginTime);
            return Value;
        }

        /// <summary>
        /// Method for update Unit configuration
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="groupHallFixturesData"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public List<ResultGroupConfiguration> SaveGroupHallFixture(int groupId, GroupHallFixturesData groupHallFixturesData, string userId, int is_Saved, List<LogHistoryTable> logHistoryTable)
        {
            var methodBeginTime = Utility.LogBegin();
            bool is_HallStation = true;
            ResultGroupConfiguration result = new ResultGroupConfiguration();
            List<ResultGroupConfiguration> lstResult = new List<ResultGroupConfiguration>();
            DataTable groupHallFixtureConsoleInfoDataTable = Utility.GenerateGroupHallFixtureConsoleDataTable(groupHallFixturesData);
            DataTable entranceConfigurationDataTable = Utility.GenerateEntranceConfigurationDataTable(groupHallFixturesData.VariableAssignments, Convert.ToInt32(groupHallFixturesData.GroupHallFixtureConsoleId));
            DataTable groupHallFixtureLocationDataTable = Utility.GenerateGroupHallFixtureLocationDataTable(groupHallFixturesData.GroupHallFixtureLocations, Convert.ToInt32(groupHallFixturesData.GroupHallFixtureConsoleId), is_HallStation);
            DataTable historyTable = Utility.GenerateDataTableForHistoryTable(logHistoryTable);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSaveGroupHallFixture(groupId, Convert.ToInt32(groupHallFixturesData.GroupHallFixtureConsoleId), groupHallFixtureConsoleInfoDataTable, entranceConfigurationDataTable, groupHallFixtureLocationDataTable, userId, historyTable);
            int resultForSaveGroupHallFixture = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVEGROUPHALLFIXTURE, lstSqlParameter);
            if (resultForSaveGroupHallFixture > 0 && is_Saved.Equals(0))
            {
                result.Result = 1;
                result.GroupConfigurationId = resultForSaveGroupHallFixture;
                result.Message = Constant.GROUPHALLFIXTURESAVEMESSAGE;
                result.Description = Constant.GROUPHALLFIXTURESAVEMESSAGE;
            }
            else if (resultForSaveGroupHallFixture > 0 && is_Saved.Equals(1))
            {
                result.Result = 1;
                result.GroupConfigurationId = resultForSaveGroupHallFixture;
                result.Message = Constant.GROUPHALLFIXTUREUPDATEMESSAGE;
            }
            else if (resultForSaveGroupHallFixture == 0)
            {
                result.Result = 0;
                result.GroupConfigurationId = resultForSaveGroupHallFixture;
                result.Message = Constant.GROUPHALLFIXTURESAVEERRORMESSAGE;
                result.Description = Constant.GROUPHALLFIXTURESAVEERRORMESSAGE;
            }
            else if (resultForSaveGroupHallFixture.Equals(-1))
            {
                result.Result = resultForSaveGroupHallFixture;
                result.GroupConfigurationId = groupId;
                result.Message = Constant.GROUPHALLFIXTURESAVETRADITIONALAGILEERRORMESSAGE;
                result.Description = Constant.GROUPHALLFIXTURESAVETRADITIONALAGILEERRORMESSAGE;
            }
            else if (resultForSaveGroupHallFixture.Equals(-2))
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.GROUPHALLFIXTURESAVEFIRESERVICEERRORMESSAGE,
                    Description = Constant.GROUPHALLFIXTURESAVEFIRESERVICEERRORMESSAGE
                });
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBeginTime);
            return lstResult;
        }

        /// <summary>
        /// To Get Saved Group Hall Fixture Data
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="userName"></param>
        /// <param Name="FixtureStrategy"></param>
        /// <returns></returns>
        public List<GroupHallFixtures> GetGroupHallFixturesData(int groupid, string userName, string fixtureStrategy, List<ConfigVariable> hallStation)
        {
            var methodBeginTime = Utility.LogBegin();
            var gHFDefaults = JObject.Parse(File.ReadAllText(Constants.GROUPHALLFIXTURECONSOLEDEFAULTVALUES)).ToString();
            var defaultGHFVariables = Utility.DeserializeObjectValue<Dictionary<string, Dictionary<string, string>>>(gHFDefaults);
            DataTable defaultVariablesGHF = Utility.GenerateDataTableForDefaultConsoleVariables(defaultGHFVariables);
            List<GroupHallFixtures> objGroupHallFixtureConfigurationData = new List<GroupHallFixtures>();
            var hallStationTables = Utility.GenerateDataTableForUnitConfiguration(hallStation);
            IList<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@GROUPCONFIGRATIONID, Value = groupid },
                new SqlParameter() { ParameterName = Constant.@FIXTURESTRATEGY, Value = fixtureStrategy },
                new SqlParameter() { ParameterName = Constant.@USERNAME, Value = userName },
                new SqlParameter(){ParameterName = Constant.HALLSTATIONVARIABLES, Value = hallStationTables},
                new SqlParameter(){ParameterName = Constant.DEFAULTVARIABLES, Value = defaultVariablesGHF}
            };
            DataSet ds = new DataSet();
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETGROUPHALLFIXTURE, sp);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var unit = (from DataRow row in ds.Tables[0].Rows
                                select new
                                {
                                    // need to change the table row names
                                    UnitId = Convert.ToInt32(row[Constant.UNITIDLOWERCASE]),
                                    FloorDesignation = row[Constant.FLOORDESIGNATION].ToString()
                                }).Distinct();
                    List<GroupHallFixtures> groupHallFixtureConsoles = new List<GroupHallFixtures>();
                    var groupHallFixtureConsoleList = (from DataRow row in ds.Tables[0].Rows
                                                       select new
                                                       {
                                                           // need to change the table row names
                                                           ConsoleId = Convert.ToInt32(row[Constant.UNITHALLFIXTURECONSOLEID]),
                                                           ConsoleName = row[Constant.UNITHALLFIXTURECONSOLENAME].ToString(),
                                                           isController = Convert.ToBoolean(row[Constant.ISCONTROLLER]),
                                                           FrontOpening = Convert.ToBoolean(row[Constant.FRONTOPENING]),
                                                           RearOpening = Convert.ToBoolean(row[Constant.REAROPENING]),
                                                           FixtureType = row[Constant.FIXTURETYPEVARIABLE].ToString(),
                                                       }).Distinct();
                    foreach (var console in groupHallFixtureConsoleList)
                    {
                        var opening = new Openings()
                        {
                            Front = console.FrontOpening,
                            Rear = console.RearOpening
                        };
                        GroupHallFixtures groupHallFixtureConsole = new GroupHallFixtures()
                        {
                            ConsoleId = console.ConsoleId,
                            ConsoleName = console.ConsoleName,
                            AssignOpenings = !console.isController,
                            IsController = console.isController,
                            Openings = opening,
                            GroupHallFixtureType = console.FixtureType,
                        };
                        var variableList = (from DataRow row in ds.Tables[0].Rows
                                            select new
                                            {
                                                // Constant.GROUPHALLFIXTURECONSOLEID neeed to change in the table
                                                ConsoleId = Convert.ToInt32(row[Constant.UNITHALLFIXTURECONSOLEID]),
                                                variableType = row[Constant.VARIABLETYPE].ToString() == null ? null : row[Constant.VARIABLETYPE].ToString(),
                                                variablevalue = row[Constant.VARIABLEVALUE].ToString() == null ? null : row[Constant.VARIABLEVALUE].ToString(),
                                                groupFixtureType = row[Constant.FIXTURETYPEVARIABLE].ToString(),
                                            }).Distinct();
                        variableList = variableList.Where(x => x.groupFixtureType.Equals(groupHallFixtureConsole.GroupHallFixtureType)).Distinct().ToList();
                        variableList = variableList.Where(x => x.ConsoleId == groupHallFixtureConsole.ConsoleId).ToList();
                        List<ConfigVariable> variableAssignments = new List<ConfigVariable>();
                        foreach (var assignments in variableList)
                        {
                            ConfigVariable variableAssignment = new ConfigVariable()
                            {
                                VariableId = assignments.variableType,
                                Value = assignments.variablevalue
                            };
                            variableAssignments.Add(variableAssignment);
                        }
                        groupHallFixtureConsole.VariableAssignments = variableAssignments;
                        var groupConsoleList = (from DataRow row in ds.Tables[0].Rows
                                                select new
                                                {
                                                    // need to change row Name to group hall fixture
                                                    ConsoleId = Convert.ToInt32(row[Constant.UNITHALLFIXTURECONSOLEID]),
                                                    floorNumber = Convert.ToInt32(row[Constant.FLOORNUMBER]),
                                                    floorDesignation = row[Constant.FLOORDESIGNATION].ToString(),
                                                    front = Convert.ToBoolean(row[Constant.FRONT]),
                                                    rear = Convert.ToBoolean(row[Constant.REAR]),
                                                    openingFront = Convert.ToBoolean(row[Constant.OPENINGFRONT]),
                                                    openingRear = Convert.ToBoolean(row[Constant.OPENINGREAR]),
                                                    groupHallFixtureType = row[Constant.FIXTURETYPEVARIABLE].ToString(),
                                                    hallStationName = row[Constant.HALLSTATIONNAME].ToString()
                                                }).Distinct();
                        groupConsoleList = groupConsoleList.Where(x => x.ConsoleId.Equals(groupHallFixtureConsole.ConsoleId) && x.groupHallFixtureType.Equals(groupHallFixtureConsole.GroupHallFixtureType)).Distinct().ToList();
                        List<GroupHallFixtureLocations> groupLocations = new List<GroupHallFixtureLocations>();
                        foreach (var grouplocation in groupConsoleList)
                        {
                            GroupHallFixtureLocations groupHallLocation = new GroupHallFixtureLocations()
                            {
                                FloorNumber = grouplocation.floorNumber,
                                FloorDesignation = grouplocation.floorDesignation

                            };
                            LandingOpening landingOpening = new LandingOpening()
                            {
                                InCompatible = false,
                                NotAvailable = !grouplocation.openingFront,
                                Value = grouplocation.front
                            };
                            groupHallLocation.Front = landingOpening;
                            landingOpening = new LandingOpening()
                            {
                                InCompatible = false,
                                NotAvailable = !grouplocation.openingRear,
                                Value = grouplocation.rear
                            };
                            groupHallLocation.Rear = landingOpening;
                            groupLocations.Add(groupHallLocation);
                        }
                        groupHallFixtureConsole.GroupHallFixtureLocations = groupLocations;
                        groupHallFixtureConsoles.Add(groupHallFixtureConsole);
                        // For units level
                        var getunitGroupConsoleList = (from DataRow row in ds.Tables[0].Rows
                                                       select new
                                                       {
                                                           // need to change row Name to group hall fixture
                                                           ConsoleId = Convert.ToInt32(row[Constant.UNITHALLFIXTURECONSOLEID]),
                                                           unitId = Convert.ToInt32(row[Constant.UNITID]),
                                                           designation = Convert.ToString(row[Constant.DESIGNATION]),
                                                           floorNumber = Convert.ToInt32(row[Constant.FLOORNUMBER]),
                                                           floorDesignation = row[Constant.FLOORDESIGNATION].ToString(),
                                                           front = Convert.ToBoolean(row[Constant.FRONT]),
                                                           rear = Convert.ToBoolean(row[Constant.REAR]),
                                                           openingFront = Convert.ToBoolean(row[Constant.OPENINGFRONT]),
                                                           openingRear = Convert.ToBoolean(row[Constant.OPENINGREAR]),
                                                           groupHallFixtureType = row[Constant.FIXTURETYPEVARIABLE].ToString(),
                                                           hallStationName = row[Constant.HALLSTATIONNAME].ToString()
                                                       });
                        getunitGroupConsoleList = getunitGroupConsoleList.Where(x => x.ConsoleId.Equals(groupHallFixtureConsole.ConsoleId) && x.groupHallFixtureType.Equals(groupHallFixtureConsole.GroupHallFixtureType)).Distinct().ToList();
                        List<UnitDetailsValues> groupUnitLocationsValues = new List<UnitDetailsValues>();
                        var unitIdValues = getunitGroupConsoleList.Select(a => new { a.unitId, a.designation, a.groupHallFixtureType, a.hallStationName }).Distinct().OrderBy(a => a.designation).ToList();
                        foreach (var item in unitIdValues)
                        {
                            var unitValuesDetails = new UnitDetailsValues()
                            {
                                UnitGroupValues = new List<GroupHallFixtureLocations>(),
                                UnitId = item.unitId,
                                UniDesgination = item.designation,
                                HallStationName = item.hallStationName,
                            };
                            foreach (var grouplocation in getunitGroupConsoleList)
                            {
                                if (item.unitId == grouplocation.unitId)
                                {
                                    GroupHallFixtureLocations groupHallLocation = new GroupHallFixtureLocations()
                                    {
                                        FloorNumber = grouplocation.floorNumber,
                                        FloorDesignation = grouplocation.floorDesignation,
                                        UnitDesignation = grouplocation.designation

                                    };
                                    LandingOpening landingOpening = new LandingOpening()
                                    {
                                        InCompatible = false,
                                        NotAvailable = !grouplocation.openingFront,
                                        Value = grouplocation.front
                                    };
                                    groupHallLocation.Front = landingOpening;
                                    landingOpening = new LandingOpening()
                                    {
                                        InCompatible = false,
                                        NotAvailable = !grouplocation.openingRear,
                                        Value = grouplocation.rear
                                    };
                                    groupHallLocation.Rear = landingOpening;
                                    groupHallLocation.HallStationName = grouplocation.hallStationName;
                                    unitValuesDetails.UnitGroupValues.Add(groupHallLocation);
                                }
                            }
                            if (unitValuesDetails.UniDesgination != null && unitValuesDetails.UnitGroupValues.Any())
                            {
                                groupUnitLocationsValues.Add(unitValuesDetails);
                            }
                        }
                        groupHallFixtureConsole.UnitDetails = (from locations in groupUnitLocationsValues
                                                               orderby locations.UnitId
                                                               select locations).ToList();
                        groupHallFixtureConsoles.Add(groupHallFixtureConsole);
                    }
                    if (groupHallFixtureConsoles.Count > 0)
                    {
                        var opening = groupHallFixtureConsoles[0].Openings;
                        var lstlocation = new List<GroupHallFixtureLocations>();
                        var varGroupHallFixtureLocation = groupHallFixtureConsoles[0].GroupHallFixtureLocations;
                        foreach (var location in varGroupHallFixtureLocation)
                        {
                            var varlocation = new GroupHallFixtureLocations()
                            {
                                FloorNumber = location.FloorNumber,
                                FloorDesignation = location.FloorDesignation,
                                Front = new LandingOpening()
                                {
                                    Value = false,
                                    InCompatible = false,
                                    NotAvailable = location.Front.NotAvailable
                                },
                                Rear = new LandingOpening()
                                {
                                    Value = false,
                                    InCompatible = false,
                                    NotAvailable = location.Rear.NotAvailable
                                }
                            };
                            lstlocation.Add(varlocation);
                        }
                    }
                    // need to check the code and remove the duplicates
                    objGroupHallFixtureConfigurationData = groupHallFixtureConsoles.Distinct().ToList();
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    var HallAvailableData = (from DataRow row in ds.Tables[1].Rows
                                             select new
                                             {
                                                 FloorDesignation = row[Constant.FLOORDESIGNATION].ToString(),
                                                 FrontAvailable = Convert.ToBoolean(row[Constant.FRONT]),
                                                 RearAvailable = Convert.ToBoolean(row[Constant.REAR]),
                                                 HallStationName = row[Constant.HALLSTATIONNAME].ToString()
                                             }
                                    );

                    foreach (var Console in objGroupHallFixtureConfigurationData)
                    {
                       if(Console.GroupHallFixtureType!= Constants.AGILEHALLSTATION)
                        {
                            var hallStationData = (from Units in Console.UnitDetails select Units.HallStationName).ToList().Distinct();
                            foreach (var hallstation in hallStationData)
                            {
                                var unitsData = (from Unit in Console.UnitDetails where Unit.HallStationName.Equals(hallstation) select Unit).ToList();
                                List<GroupHallFixtureLocations> groupHallFixtureLocations = new List<GroupHallFixtureLocations>();
                                foreach (var unitValues in unitsData)
                                {
                                    foreach (var location in unitValues.UnitGroupValues.Distinct())
                                    {
                                        if (hallstation.Contains(Constant.F) && location.HallStationName.Equals(hallstation))
                                        {
                                            var locationPresent = (from locationData in groupHallFixtureLocations
                                                                   where locationData.FloorDesignation.Equals(location.FloorDesignation) && locationData.Front.Value.Equals(location.Front.Value)
                                                                   select locationData).ToList();
                                            if (locationPresent.Count == 0)
                                            {
                                                var frontDataAvailable = (from availableData in HallAvailableData
                                                                          where Utility.CheckEquals(availableData.HallStationName, hallstation)
                                                                              && availableData.FloorDesignation.Equals(location.FloorDesignation)
                                                                          select availableData.FrontAvailable).ToList().FirstOrDefault();
                                                var varlocation = new GroupHallFixtureLocations()
                                                {
                                                    FloorNumber = location.FloorNumber,
                                                    FloorDesignation = location.FloorDesignation,
                                                    Front = new LandingOpening()
                                                    {
                                                        Value = location.Front.Value,
                                                        InCompatible = false,
                                                        NotAvailable = !frontDataAvailable
                                                    }
                                                };
                                                groupHallFixtureLocations.Add(varlocation);
                                            }
                                        }
                                        if (hallstation.Contains(Constant.R) && Utility.CheckEquals(location.HallStationName, hallstation))
                                        {
                                            var locationPresent = (from locationData in groupHallFixtureLocations
                                                                   where locationData.FloorDesignation.Equals(location.FloorDesignation) && locationData.Rear.Value.Equals(location.Rear.Value)
                                                                   select locationData).ToList();
                                            if (locationPresent.Count == 0)
                                            {
                                                var reardataAvailable = (from availableData in HallAvailableData
                                                                         where Utility.CheckEquals(availableData.HallStationName, hallstation)
                                                                         && availableData.FloorDesignation.Equals(location.FloorDesignation)
                                                                         select availableData.RearAvailable).ToList()[0];
                                                var varlocation = new GroupHallFixtureLocations()
                                                {
                                                    FloorNumber = location.FloorNumber,
                                                    FloorDesignation = location.FloorDesignation,
                                                    Rear = new LandingOpening()
                                                    {
                                                        Value = location.Rear.Value,
                                                        InCompatible = false,
                                                        NotAvailable = !reardataAvailable
                                                    }
                                                };
                                                groupHallFixtureLocations.Add(varlocation);
                                            }
                                        }
                                    }
                                }
                                HallStations hallStations = new HallStations()
                                {
                                    HallStationId = hallstation,
                                    HallStationName = (hallstation.Split(Constant.DOT)[1]).Split(Constant.UNDERSCORE)[0],
                                    openingDoors = unitsData[0].openingDoors,
                                    NoOfFloors = groupHallFixtureLocations.Count,
                                    openingsAssigned = groupHallFixtureLocations.OrderBy(x => x.FloorNumber).ToList(),
                                };
                                if (Console.HallStations == null)
                                {
                                    Console.HallStations = new List<HallStations>();
                                }
                                Console.HallStations.Add(hallStations);
                                Console.GroupHallFixtureLocations = groupHallFixtureLocations;
                            }
                        }

                       else if(ds.Tables[2].Rows.Count > 0)
                       {
                            var agileAvailabeData = (from DataRow row in ds.Tables[2].Rows
                                                      select new
                                                      {
                                                          FloorDesignation = row[Constant.FLOORDESIGNATION].ToString(),
                                                          FrontAvailable = Convert.ToBoolean(row[Constant.FRONT]),
                                                          RearAvailable = Convert.ToBoolean(row[Constant.REAR])
                                                      });

                            foreach (var location in Console.GroupHallFixtureLocations)
                            {
                                location.Front.NotAvailable = !(from availableData in agileAvailabeData
                                                               where availableData.FloorDesignation.Equals(location.FloorDesignation, StringComparison.OrdinalIgnoreCase)
                                                               select availableData.FrontAvailable).FirstOrDefault();
                                location.Rear.NotAvailable = !(from availableData in agileAvailabeData
                                                                where availableData.FloorDesignation.Equals(location.FloorDesignation, StringComparison.OrdinalIgnoreCase)
                                                                select availableData.RearAvailable).FirstOrDefault();
                            }                              
                       }
                            
                        
                    }

                }

                if (ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0)
                {
                    var conflictVaribales = new List<string>();
                    foreach (DataRow item in ds.Tables[3].Rows)
                    {
                        conflictVaribales.Add(item[Constants.CONFLICTVARIABLEIDDATA].ToString());
                    }
                    objGroupHallFixtureConfigurationData[0].VariableIds = (conflictVaribales != null && conflictVaribales.Any() ? conflictVaribales : new List<string>());
                }
            }
            Utility.LogEnd(methodBeginTime);
            return objGroupHallFixtureConfigurationData;
        }

        /// <summary>
        /// To Get Unit Details
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="doorDetails"></param>
        /// <returns></returns>
        public List<UnitDetailsValues> GetUnitDetails(int groupConfigurationId, List<ConfigVariable> doorDetails)
        {
            var methodBeginTime = Utility.LogBegin();
            List<UnitDetailsValues> unitsObject = new List<UnitDetailsValues>();
            IList<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@GROUPCONFIGRATIONID, Value = groupConfigurationId}
            };
            DataSet ds = new DataSet();
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETUNITDETAILSFORGROUPHALLFIXTURE, sp);

            if (ds.Tables.Count > 0)
            {
                var mainEgress = string.Empty;
                var alternateEgress = string.Empty;
                int noofFloor = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    mainEgress = Convert.ToString(ds.Tables[0].Rows[0][Constant.MAINEGRESS]);
                    alternateEgress = Convert.ToString(ds.Tables[0].Rows[0][Constant.ALTERNATEEGRESS]);
                    noofFloor = Convert.ToInt32(ds.Tables[0].Rows[0][Constant.NOOFFLOORS]);
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    //Fetching UnitDetails from dataset
                    var unitDetails = (from DataRow row in ds.Tables[1].Rows
                                       select new
                                       {
                                           unitId = Convert.ToInt32(row[Constant.UNITID]),
                                           unitDesignation = Convert.ToString(row[Constant.UNITDesignation]),
                                           occupiedSpaceBelow = Convert.ToBoolean(row[Constant.OCCUPIEDSPACEBELOW]),
                                           front = Convert.ToBoolean(row[Constant.FRONTOPENING]),
                                           rear = Convert.ToBoolean(row[Constant.REAROPENING]),

                                       }).Distinct();
                    //Fetching Landing and opening unitwise details from dataset
                    var openingDetails = (from DataRow row in ds.Tables[1].Rows
                                          select new
                                          {
                                              unitId = Convert.ToInt32(row[Constant.UNITID]),
                                              floorDesignation = Convert.ToString(row[Constant.FLOORDESIGNATION]),
                                              floorNumber = Convert.ToInt32(row[Constant.FLOORNUMBER]),
                                              front = Convert.ToBoolean(row[Constant.FRONT]),
                                              rear = Convert.ToBoolean(row[Constant.REAR]),

                                          }).Distinct();
                    //Looping through the unit details in the group
                    foreach (var unit in unitDetails)
                    {
                        var openingDetailsFiltered = openingDetails.Where(x => x.unitId.Equals(unit.unitId)).ToList().Distinct();
                        var openingsAssigned = new List<Opening>();
                        var groupHallFixtureLocation = new List<GroupHallFixtureLocations>();
                        //looping throght each landing openings in the Unit
                        foreach (var openings in openingDetailsFiltered)
                        {
                            openingsAssigned.Add(new Opening()
                            {
                                FloorDesignation = openings.floorDesignation,
                                FloorNumber = openings.floorNumber,
                                Front = new Compatible()
                                {
                                    NotAvailable = !openings.front,
                                    Value = false
                                },
                                Rear = new Compatible()
                                {
                                    NotAvailable = !openings.rear,
                                    Value = false
                                }

                            });
                            groupHallFixtureLocation.Add(new GroupHallFixtureLocations()
                            {
                                FloorDesignation = openings.floorDesignation,
                                FloorNumber = openings.floorNumber,
                                Front = new LandingOpening()
                                {
                                    NotAvailable = !openings.front,
                                    Value = false
                                },
                                Rear = new LandingOpening()
                                {
                                    NotAvailable = !openings.rear,
                                    Value = false
                                }
                            });
                        }
                        unitsObject.Add(new UnitDetailsValues()
                        {
                            UnitId = unit.unitId,
                            UniDesgination = unit.unitDesignation,
                            unitName = unit.unitDesignation,
                            OccupiedSpaceBelow = unit.occupiedSpaceBelow,
                            openingDoors = new OpeningDoors()
                            {
                                Front = unit.front,
                                Rear = unit.rear
                            },
                            MainEgress = Convert.ToString(mainEgress),
                            AlternateEgress = Convert.ToString(alternateEgress),
                            openingsAssigned = openingsAssigned,
                            UnitGroupValues = groupHallFixtureLocation
                        });
                    }
                }
            }
            Utility.LogEnd(methodBeginTime);
            return unitsObject;
        }

        /// <summary>
        /// GetGroupFixtureStrategy
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <returns></returns>
        public string GetGroupFixtureStrategy(int groupConfigurationId)
        {
            var methodBeginTime = Utility.LogBegin();

           
            IList<SqlParameter> sqlParam = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName=Constant.GROUPINGID,Value=groupConfigurationId},
                 
            };
            var fixtureStrategy = CpqDatabaseManager.ExecuteScalarForReturnString(Constant.SPGETFIXTURESTRATEGY, sqlParam);

            Utility.LogEnd(methodBeginTime);
            return fixtureStrategy;

        }

        /// <summary>
        /// DeleteGroupHallFixtureConfigurationById
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="consoleId"></param>
        /// <param Name="fixtureType"></param>
        /// <param Name="logHistoryTable"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public List<ResultSetConfiguration> DeleteGroupHallFixtureConsole(int groupId, int consoleId, string fixtureType, List<LogHistoryTable> logHistoryTable, string userId)
        {
            var methodBeginTime = Utility.LogBegin();
            ResultSetConfiguration result = new ResultSetConfiguration();
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
            DataTable historyTable = Utility.GenerateDataTableForHistoryTable(logHistoryTable);
            IList<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.GROUPINGID, Value = groupId},
                new SqlParameter() { ParameterName = Constant.CONSOLEID, Value = consoleId},
                new SqlParameter() { ParameterName = Constant.FIXTURETYPE, Value = fixtureType},
                new SqlParameter() { ParameterName = Constant.HISTORYTABLE, Value = historyTable},
                new SqlParameter() { ParameterName = Constant.FDACREATEDDBY, Value = userId},
                new SqlParameter() { ParameterName = Constant.RESULT, Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int},
            };
            int Result = CpqDatabaseManager.ExecuteNonquery(Constant.SPDELETEGROUPHALLFIXTURECONFIGBYID, sp);
            if (Result > 0)
            {
                result.result = 1;
                result.setId = groupId;
                result.message = Constant.DELETEGROUPHALLFIXTURECONSOLESUCCESSMSG;
                result.description = Constant.DELETEGROUPHALLFIXTURECONSOLESUCCESSMSG;
            }
            else if (Result == 0)
            {
                result.result = 0;
                result.setId = groupId;
                result.message = Constant.DELETEGROUPHALLFIXTURECONSOLEERRORMSG;
                result.description = Constant.DELETEGROUPHALLFIXTURECONSOLEERRORMSG;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBeginTime);
            return lstResult;
        }

        /// <summary>
        /// This function is used to check if product is configured for atleast for one Unit in the group
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        public bool CheckUnitConfigured(int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> sqlParam = new List<SqlParameter>()
            {
               new SqlParameter() { ParameterName = Constant.@GRPCONFIGID ,Value=groupId},
               new SqlParameter() { ParameterName = Constant.RESULT ,Value=groupId,Direction = ParameterDirection.Output}
            };
            Utility.LogEnd(methodBeginTime);
            return (Convert.ToBoolean(CpqDatabaseManager.ExecuteNonquery(Constant.SPCHECKIFUNITCONFIGURED, sqlParam)));
        }

        /// <summary>
        /// This function is used to check if product is configured for atleast for one Unit in the group
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        public bool CheckProductSelected(int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> sqlParam = new List<SqlParameter>()
            {
               new SqlParameter() { ParameterName = Constant.@GRPCONFIGID ,Value=groupId},
               new SqlParameter() { ParameterName = Constant.RESULT ,Value=groupId,Direction = ParameterDirection.Output}
            };
            Utility.LogEnd(methodBeginTime);
            return (Convert.ToBoolean(CpqDatabaseManager.ExecuteNonquery(Constant.SPCHECKPRODUCTSELECTED, sqlParam)));
        }

        /// <summary>
        /// This function is usedd to duplication a list of groups to another building or same building
        /// </summary>
        /// <param Name="groupIDDataTable"></param>
        /// <param Name="buildingID"></param>
        /// <returns></returns>
        public DataSet DuplicateGroupConfigurationById(DataTable groupIDDataTable, int buildingID)
        {
            var methodBeginTime = Utility.LogBegin();
            List<Result> lstResult = new List<Result>();
            IList<SqlParameter> sp1;
            var duplicateContantsDictionary = Utility.VariableMapper(Constant.DUPLICATECONSTANTMAPPERPATH, Constant.CONSTANTMAPPER);
            //creating variableMapper to send to StoredProcedures
            List<ConfigVariable> mapperVariables = new List<ConfigVariable>();
            foreach (var variable in duplicateContantsDictionary)
            {
                mapperVariables.Add(new ConfigVariable()
                {
                    VariableId = variable.Key,
                    Value = variable.Value
                });
            }
            var variableMapperAssignment = Utility.GenerateVariableMapperDataTable(mapperVariables);
            sp1 = new List<SqlParameter>()
            {
                    new SqlParameter() { ParameterName = Constant.GROUPIDLIST,Value=groupIDDataTable,Direction = ParameterDirection.Input },
                    new SqlParameter() { ParameterName = Constant.BUILDINGID,Value=buildingID,Direction = ParameterDirection.Input},
                    new SqlParameter() { ParameterName = Constant.@RESULT,Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int},
                    new SqlParameter() { ParameterName =  Constant.VARIABLEMAPPERDATATABLE,Value=variableMapperAssignment,Direction = ParameterDirection.Input }

            };
            Utility.LogEnd(methodBeginTime);
            return CpqDatabaseManager.ExecuteDataSet(Constant.SPDUPLICATEGROUP, sp1);
        }

        /// <summary>
        /// This funtion is used to fetch the building related variables which are required for configuring a group
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <returns></returns>
        public List<ConfigVariable> GetBuildingVariableAssignments(int groupConfigurationId)
        {
            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@_ID, Value = groupConfigurationId }
            };
            List<ConfigVariable> lstBldng = new List<ConfigVariable>();
            DataSet ds = new DataSet();
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETBUILDINGCONFIGBYGROUPID, sp);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lstBldng = JsonConvert.DeserializeObject<List<ConfigVariable>>(ds.Tables[0].Rows[0][Constant.BUILDINGJSON].ToString());
            }
            Utility.LogEnd(methodBeginTime);
            return lstBldng;
        }

        /// <summary>
        /// This function is used to fetch Log History for a group
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="lastDate"></param>
        /// <returns></returns>
        public LogHistoryResponse GetLogHistoryGroup(int groupId, string lastDate)
        {
            var methodBeginTime = Utility.LogBegin();
            LogHistoryResponse response = new LogHistoryResponse();
            response.Data = new List<Data>();
            IList<SqlParameter> sp = new List<SqlParameter>();
            DataSet ds = new DataSet();
            SqlParameter param = new SqlParameter(Constant.GROUPCONID, groupId);
            sp.Add(param);
            if (lastDate != null && !string.IsNullOrEmpty(lastDate))
            {
                var culture = System.Globalization.CultureInfo.CurrentCulture;
                param = new SqlParameter(Constant.DATE, DateTime.ParseExact(lastDate, Constant.DATEFORMAT, culture));
            }
            else
            {
                param = new SqlParameter(Constant.DATE, DBNull.Value);
            }
            sp.Add(param);
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETLOGHISTORYGROUP, sp);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        var lstDate = (from DataRow row in ds.Tables[0].Rows
                                       select new
                                       {
                                           GroupId = Convert.ToInt32(row[Constant.GROUPID]),
                                           date = Convert.ToDateTime(row[Constant.CREATEDON]).ToString(Constant.MMDDYYYYFORMAT)
                                       }).Distinct();
                        var LogHistory = (from DataRow row in ds.Tables[0].Rows
                                          select new
                                          {
                                              GroupId = Convert.ToInt32(row[Constant.GROUPID]),
                                              date = Convert.ToDateTime(row[Constant.CREATEDON]).ToString(Constant.MMDDYYYYFORMAT),
                                              variableId = Convert.ToString(row[Constant.VARIABLEID]),
                                              currentValue = Convert.ToString(row[Constant.CURRENTVALUE]),
                                              previousValue = Convert.ToString(row[Constant.PREVIOUSVALUE]),
                                              user = Convert.ToString(row[Constant.CREATEDBY]),
                                              time = Convert.ToDateTime(row[Constant.CREATEDON]).ToString(Constant.TIMEFORMAT)
                                          }).Distinct();
                        List<Data> lstData = new List<Data>();
                        if (lstDate.Any())
                        {
                            foreach (var varDate in lstDate)
                            {
                                Data data = new Data()
                                {
                                    Date = varDate.date
                                };
                                List<LogParameters> lstLogparameters = new List<LogParameters>();
                                var filteresHistory = LogHistory.Where(x => x.date.Equals(varDate.date));
                                if (filteresHistory.Any())
                                {
                                    foreach (var loghistory in filteresHistory)
                                    {
                                        LogParameters logparameter = new LogParameters()
                                        {
                                            VariableId = loghistory.variableId,
                                            Name = loghistory.variableId,
                                            UpdatedValue = loghistory.currentValue,
                                            PreviousValue = loghistory.previousValue,
                                            User = loghistory.user,
                                            Role = string.Empty,
                                            Time = loghistory.time
                                        };
                                        lstLogparameters.Add(logparameter);
                                    }
                                }
                                data.LogParameters = lstLogparameters;
                                lstData.Add(data);
                            }
                        }
                        response.Data = lstData;
                    }
                }
                if (ds.Tables.Count > 1)
                {
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        var designation = Convert.ToString(ds.Tables[1].Rows[0][Constant.DESIGNATION]);
                        response.Description = designation;
                        response.Section = Constant.GROUP;
                    }
                }
                if (ds.Tables.Count > 2)
                {
                    if (ds.Tables[2].Rows.Count > 0)
                    {
                        var showLoadMore = Convert.ToBoolean(ds.Tables[2].Rows[0][Constant.SHOWLOADMORE]);
                        response.ShowLoadMore = showLoadMore;
                    }
                }
            }
            Utility.LogEnd(methodBeginTime);
            return response;
        }

        /// <summary>
        /// This function is used to fetch permision by role Name
        /// </summary>
        /// <param Name="Id"></param>
        /// <param Name="roleName"></param>
        /// <returns></returns>
        public List<string> GetPermissionByRole(int id, string roleName)
        {
            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@_ID, Value = id },
                new SqlParameter() { ParameterName = Constant.@ROLENAME, Value = roleName },
                new SqlParameter() { ParameterName = Constant.@ENTITY, Value = Constant.GROUPVARIABLENAME },
            };
            List<string> permission = new List<string>();
            DataSet ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETPERMISSIONBYROLENAME, sp);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var buildingConfigList = (from DataRow dRow in ds.Tables[0].Rows
                                          select new
                                          {
                                              permissions = Convert.ToString(dRow[Constant.PERMISSIONKEY]),
                                          }).Distinct().ToList();
                if (buildingConfigList != null && buildingConfigList.Any())
                {
                    permission = buildingConfigList.Select(x => x.permissions).ToList();
                }
            }
            Utility.LogEnd(methodBeginTime);
            return permission;
        }

        /// <summary>
        /// Get Building Equipment isDisabled
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        public Dictionary<string, bool> GetGroupConfigurationSectionTab(int groupId, List<ConfigVariable> hallstations)
        {
            var methodBeginTime = Utility.LogBegin();
            Dictionary<string, bool> IsDisable = new Dictionary<string, bool>();
            bool IsDisabledOpening = false;
            bool IsDisableGroupHallFixtures = false;
            var hallStationTable = Utility.GenerateDataTableForUnitConfiguration(hallstations);
            IList<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.GROUPID_CAMELCASE, Value = groupId },
                new SqlParameter() { ParameterName = Constant.HALLSTATIONVARIABLES, Value = hallStationTable },

            };
            DataSet ds = new DataSet();
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.GETGROUPCONFIGURATIONBYGROUPID, sp);

            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    IsDisableGroupHallFixtures = Convert.ToBoolean(ds.Tables[0].Rows[0][Constant.ISDISABLEDFORGHF]);
                    IsDisable[Constant.GROUPHALLFIXTURE] = IsDisableGroupHallFixtures;
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    IsDisabledOpening = Convert.ToBoolean(ds.Tables[1].Rows[0][Constant.ISDISABLEDFOROPENINGS]);
                    IsDisable[Constant.OPENINGLOCATIONS] = IsDisabledOpening;
                }
            }
            Utility.LogEnd(methodBeginTime);
            return IsDisable;
        }

        /// <summary>
        /// Get Product Category By GroupId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetProductCategoryByGroupId(int id, string type, DataTable configVariables)
        {
            var methodBeginTime = Utility.LogBegin();
            string productCategory = string.Empty;
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.@_ID, id),
                new SqlParameter(Constant.@TYPE, type),
                new SqlParameter(Constant.CONSTANTMAPPERLIST, configVariables)
            };

            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETPRODUCTCATEGORYBYGROUPID, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                productCategory = Convert.ToString(ds.Tables[0].Rows[0][Constant.GROUPCONFIGURATIONVALUE]);
            }
            Utility.LogEnd(methodBeginTime);
            return productCategory;
        }

        public List<ConfigVariable> GetGroupInformationByGroupId(int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            List<ConfigVariable> lstConfigVariables = new List<ConfigVariable>();
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPID_CAMELCASE, groupId)
            };

            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETGROUPINFODETAILSBYGROUPID, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lstConfigVariables = (from DataRow row in ds.Tables[0].Rows
                                      select new ConfigVariable
                                      {
                                          VariableId = Convert.ToString(row[Constant.VARIABLEID]),
                                          Value = Convert.ToString(row[Constant.VALUE]),
                                      }).ToList();
            }
            Utility.LogEnd(methodBeginTime);
            return lstConfigVariables;
        }

        /// <summary>
        /// SaveBuildingConflicts
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="conflictVariables"></param>
        /// <returns></returns>
        public List<Result> SaveBuildingConflicts(int configurationId, List<VariableAssignment> conflictVariables, string entityType)
        {
            var methodStartTime = Utility.LogBegin();
            List<Result> result = new List<Result>();
            var resultBuildingConfiguration = new Result();
            List<ConflictMgmtList> lstConflictVariables = new List<ConflictMgmtList>();
            var variableMapperAssignment = Utility.GenerateVariableMapperDataTableForConflict(conflictVariables);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
               new SqlParameter() { ParameterName = Constant.@_ID ,Value = configurationId},
               new SqlParameter() { ParameterName = Constant.@ENTITY ,Value=entityType},
               new SqlParameter() { ParameterName = Constant.LISTOFVARIABLEASSIGNMENT ,Value=variableMapperAssignment},
               new SqlParameter() { ParameterName = Constant.@RESULT,Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int},
            };
            int results = CpqDatabaseManager.ExecuteNonquery(Constant.SPGETBUILDINGCONFLICTS, sqlParameters);
            if (results == 1)
            {
                resultBuildingConfiguration.message = Constant.BUILDINGCONFLICTSVARIABLES;
                result.Add(resultBuildingConfiguration);
            }
            else
            {
                resultBuildingConfiguration.message = Constant.BUILDINGCONFLICTSVARIABLESISSUES;
                result.Add(resultBuildingConfiguration);
            }
            Utility.LogEnd(methodStartTime);
            return result;
        }

        public List<UnitVariables> GetGroupVariables(int groupid, DataTable configVariables)
        {
            var methodBeginTime = Utility.LogBegin();
            var fdaContantsDictionary = Utility.VariableMapper(Constant.FDAMAPPERVARIABLESMAPPERPATH, Constant.FIELDDRAWINGAUTOMATION);
            IList<SqlParameter> parameterList = new List<SqlParameter>();
            List<UnitVariables> lstUnitVariables = new List<UnitVariables>();
            DataSet dataSet = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID_LOWERCASE, groupid),
                new SqlParameter(Constant.CONSTANTMAPPERLIST ,configVariables),
                new SqlParameter(Constant.TYPE, Constants.GROUP)
            };
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETBUILDINGGROUPUNITVARIABLEASSIGNMENTS, param);
            if (dataSet != null && dataSet.Tables.Count > 3 && dataSet.Tables[0].Rows.Count > 0)
            {
                var groupVariables = (from DataRow row in dataSet.Tables[0].Rows
                                     select new UnitVariables
                                     {
                                         groupConfigurationId = Convert.ToInt32(row[Constant.FDAGROUPCONFIGURATIONID]),
                                         name = Convert.ToString(row[Constant.FDAUNITNAME]),
                                         unitId = Convert.ToInt32(row[Constant.FDAUNITID]),
                                         MappedLocation = Convert.ToString(row[Constant.MAPPEDLOCATION]),
                                         VariableId = Convert.ToString(row[Constant.VARIABLEID]),
                                         Value = Convert.ToString(row[Constant.VALUE]),
                                     }).ToList();
                var buildingElevationVariables = (from DataRow dRow in dataSet.Tables[1].Rows
                                                  select new BuildingElevationDetails
                                                  {
                                                      FloorNumber = Convert.ToString(dRow[Constant.FLOORNUMBERCAMELCASE]),
                                                      ElevationFeet = Convert.ToDecimal(dRow[Constant.ELEVAIONFEET]),
                                                      ElevationInch = Convert.ToDecimal(dRow[Constant.ELEVAIONINCH]),
                                                      FloorToFloorHeightFeet = Convert.ToDecimal(dRow[Constant.FLOORTOFLOORHEIGHTFEET]),
                                                      FloorToFloorHeightInch = Convert.ToDecimal(dRow[Constant.FLOORTOFLOORHEIGHTINCH])
                                                  }).Distinct().ToList().OrderBy(x => x.FloorNumber).ToList();
                var openingLocationVariables = (from DataRow dRow in dataSet.Tables[2].Rows
                                                select new OpeningLocationDetails
                                                {
                                                    UnitId = Convert.ToInt32(dRow[Constant.UNITID]),
                                                    TravelFeet = Convert.ToDecimal(dRow[Constant.TRAVELFEET]),
                                                    TravelInch = Convert.ToDecimal(dRow[Constant.TRAVEL_INCH]),
                                                    FloorNumber = Convert.ToString(dRow[Constant.FLOORNUMBER]),
                                                    OccupiedSpaceBelow = Convert.ToString(dRow[Constant.OCUPPIEDSPACEBELOW]),
                                                    Front = Convert.ToBoolean(dRow[Constant.FRONT]),
                                                    Rear = Convert.ToBoolean(dRow[Constant.REAR]),
                                                    FrontOpening = Convert.ToString(dRow[Constant.FRONTOPENING]),
                                                    RearOpening = Convert.ToString(dRow[Constant.REAROPENING]),
                                                }).Distinct().ToList();
                var unitVariables = (from DataRow dRow in dataSet.Tables[3].Rows
                                     select new UnitVariables
                                     {
                                         name = Convert.ToString(dRow[Constant.FDAUNITNAME]),
                                         unitId = Convert.ToInt32(dRow[Constant.FDAUNITID]),
                                         MappedLocation = Convert.ToString(dRow[Constant.MAPPEDLOCATION]),
                                     }).Distinct().ToList();
                lstUnitVariables.AddRange(groupVariables);
                lstUnitVariables.AddRange(CreateRequiredGroupVariables(groupid,buildingElevationVariables,openingLocationVariables,unitVariables,fdaContantsDictionary));
            }
            Utility.LogEnd(methodBeginTime);
            return lstUnitVariables;
        }

        public List<UnitVariables> CreateRequiredGroupVariables(int groupid, List<BuildingElevationDetails> buildingElevationVariables, List<OpeningLocationDetails> openingLocationVariables, List<UnitVariables> unitVariables, Dictionary<string,string> fdaContantsDictionary)
        {
            List<UnitVariables> listOfVariables = new List<UnitVariables>();
            int elevatorNumber = 0;
            foreach (var unit in unitVariables)
            {
                elevatorNumber++;
                var openingLocationForUnit = openingLocationVariables.Where(x => x.UnitId == unit.unitId).ToList();
                listOfVariables.Add(GenerateUnitVariable(groupid, unit, elevatorNumber, Constant.ELEVATORLEVEL, fdaContantsDictionary[Constant.TOTALNUMLANDINGS_SP], openingLocationForUnit.Count()));
                listOfVariables.Add(GenerateUnitVariable(groupid, unit, elevatorNumber, Constant.ELEVATORLEVEL, fdaContantsDictionary[Constant.OPNFRTP], openingLocationForUnit.Where(x => x.Front).ToList().Count()));
                listOfVariables.Add(GenerateUnitVariable(groupid, unit, elevatorNumber, Constant.ELEVATORLEVEL, fdaContantsDictionary[Constant.CWTSFTY], openingLocationForUnit[0].OccupiedSpaceBelow));
                listOfVariables.Add(GenerateUnitVariable(groupid, unit, elevatorNumber, Constant.ELEVATORLEVEL, fdaContantsDictionary[Constant.OPNREARP], openingLocationForUnit.Where(x => x.Rear).ToList().Count()));
                listOfVariables.Add(GenerateUnitVariable(groupid, unit, elevatorNumber, Constant.ELEVATORLEVEL, fdaContantsDictionary[Constant.TOPR], openingLocationForUnit.Where(x => x.Rear).ToList().Count()));
                listOfVariables.Add(GenerateUnitVariable(groupid, unit, elevatorNumber, Constant.ELEVATORLEVEL, fdaContantsDictionary[Constant.TOPF], openingLocationForUnit.Where(x => x.Front).Select(x => x.FloorNumber).Max()));
                listOfVariables.Add(GenerateUnitVariable(groupid, unit, elevatorNumber, Constant.ELEVATORLEVEL, fdaContantsDictionary[Constant.TOTOPN], openingLocationForUnit.Where(x => x.Front || x.Rear).Select(x => x.FloorNumber).Max()));
                listOfVariables.Add(GenerateUnitVariable(groupid, unit, elevatorNumber, Constant.ELEVATORLEVEL, fdaContantsDictionary[Constant.TRAVELCAPS], openingLocationForUnit[0].TravelFeet * 12 + openingLocationForUnit[0].TravelInch));
                foreach (var opening in openingLocationForUnit)
                {
                    var buildingElevationForFloor = buildingElevationVariables.Where(x => x.FloorNumber == opening.FloorNumber).ToList();
                    listOfVariables.Add(GenerateUnitVariable(groupid, unit, elevatorNumber, Constant.LANDINGLEVEL, fdaContantsDictionary[Constant.ENTF], opening.Front, opening));
                    listOfVariables.Add(GenerateUnitVariable(groupid, unit, elevatorNumber, Constant.LANDINGLEVEL, fdaContantsDictionary[Constant.ENTR], opening.Rear, opening));
                    listOfVariables.Add(GenerateUnitVariable(groupid, unit, elevatorNumber, Constant.LANDINGLEVEL, fdaContantsDictionary[Constant.ELEVATION], buildingElevationForFloor[0].ElevationFeet * 12 + buildingElevationForFloor[0].ElevationInch, opening));
                    listOfVariables.Add(GenerateUnitVariable(groupid, unit, elevatorNumber, Constant.LANDINGLEVEL, fdaContantsDictionary[Constant.FLRHTF], buildingElevationForFloor[0].FloorToFloorHeightFeet * 12 + buildingElevationForFloor[0].ElevationInch, opening));
                    listOfVariables.Add(GenerateUnitVariable(groupid, unit, elevatorNumber, Constant.LANDINGLEVEL, fdaContantsDictionary[Constant.FLRHTR], buildingElevationForFloor[0].FloorToFloorHeightFeet * 12 + buildingElevationForFloor[0].ElevationInch, opening));
                }
            }
            return listOfVariables;
        }

        public UnitVariables GenerateUnitVariable(int groupid, UnitVariables unit, int elevatorNumber, string type, string variableName, object value, OpeningLocationDetails openingDetails = null)
        {
            UnitVariables unitVariable = new UnitVariables()
            {
                groupConfigurationId = groupid,
                name = unit.name,
                unitId = unit.unitId,
                MappedLocation = unit.MappedLocation,
                VariableId = string.Empty,
                Value = string.Empty
            };
            if (type.Equals(Constant.ELEVATORLEVEL))
            {
                unitVariable.VariableId = Constant.ELEVATOR00CAPS + Convert.ToString(elevatorNumber) + variableName;
            }
            else if (type.Equals(Constant.LANDINGLEVEL))
            {
                unitVariable.VariableId = Constant.ELEVATOR00CAPS + Convert.ToString(elevatorNumber) + Constant.FLOORMATRIXLANDING + Convert.ToString(openingDetails?.FloorNumber).PadLeft(3, Convert.ToChar(Constants.ZERO)) + variableName;
            }
            unitVariable.Value = value;
            return unitVariable;
        }
    }
}




