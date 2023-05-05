/************************************************************************************************************
************************************************************************************************************
    File Name     :   BuildingConfigurationDL class 
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
using System.Linq;
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Database;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using System.IO;
using Microsoft.Extensions.Configuration;
using Configit.Configurator.Server.Common;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class BuildingConfigurationDL : IBuildingConfigurationDL
    {
        #region Variables
        string message = string.Empty;
        int resultBuildingElevation = 0;
        private readonly IProject _projectDl;
        private readonly IConfigure _configure;
        private readonly IConfigurationSection _configuration;
        private readonly string _environment;
        private readonly ICacheManager _cpqCacheManager;
        #endregion

        /// <summary>
        /// Constructor for BuildingConfigurationDL
        /// </summary>
        /// <param name="logger"></param>
        public BuildingConfigurationDL(ILogger<BuildingConfigurationDL> logger, IProject projectDl, IConfigure configure, ICacheManager cpqCacheManager, IConfiguration configuration)
        {
            Utility.SetLogger(logger);
            _projectDl = projectDl;
            _configure = configure;
            _configuration = configuration.GetSection(Constant.PARAMSETTINGS);
            _environment = _configuration[Constant.ENVIRONMENT];
            _cpqCacheManager = cpqCacheManager;
        }
        /// <summary>
        /// Get List Of Buildings For Project by project Id
        /// </summary>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        public List<ListOfConfiguration> GetListOfConfigurationForProject(string quoteId, DataTable configVariables,string sessionId)
        {
            var methodStartTime = Utility.LogBegin();
            var userDetails = _cpqCacheManager.GetCache(sessionId, _environment, Constants.USERDETAILSCPQ);
            var userData = JObject.Parse(userDetails);
            var viewUser = (bool)userData.SelectToken("isViewUser");
            var sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@QUOTEIDSPPARAMETER ,Value=quoteId,Direction = ParameterDirection.Input },
                new SqlParameter(){ParameterName =Constant.CONSTANTMAPPERLIST ,Value = configVariables }
            };

            DataSet dataSet = new DataSet();
            List<ListOfConfiguration> ListConfiguration = new List<ListOfConfiguration>();
            var saleStage = string.Empty;
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETBUILDINGCONFIGFORPROJECT, sp);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[2].Rows.Count > 0)
            {
                var saleStageValue = (from DataRow dRow in dataSet.Tables[2].Rows
                                      select new
                                      {
                                          WorkFlowStage = Convert.ToString(dRow[Constants.WORKFLOWSTAGE])
                                      }).Distinct();
                saleStage = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(saleStageValue.FirstOrDefault()))[Constants.WORKFLOWSTAGEPASCAL].ToString();
            }
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                var productNameMapperJson = Utility.VariableMapper(Constants.PRODUCTSELECTIONCONSTANTTEMPLATEPATH, Constants.PROJECTCOMMONNAME);

                var buildingConfigList = (from DataRow dRow in dataSet.Tables[0].Rows
                                          select new
                                          {
                                              id = Convert.ToInt32(dRow[Constant.ID]),
                                              buildingName = Convert.ToString(dRow[Constant.BUILDINGNAME]),
                                              //BuildingConflictCheck = Convert.ToString(dRow[Constant.BUILDINGCONFLICTCHECK]), 
                                              BuildingEquipmentStatus = Convert.ToString(dRow[Constant.BUILDINGEQUIPMENTSTATUS]),
                                              buildingStatusKey = Convert.ToString(dRow["BuildingStatusKey"]),
                                              buildingStatusName = Convert.ToString(dRow["BuildingStatusName"]),
                                              buildingStatusDisplayName = Convert.ToString(dRow["BuildingStatusDisplayName"]),
                                              buildingStatusDescription = Convert.ToString(dRow["BuildingStatusDescription"])
                                          }).Distinct();


                foreach (var building in buildingConfigList)
                {
                    ListOfConfiguration configurationList = new ListOfConfiguration()
                    {
                        BuildingName = building.buildingName,
                        Id = building.id,
                        BuildingStatus = new Status()
                        {
                            StatusKey = building.buildingStatusKey,
                            StatusName = building.buildingStatusName,
                            DisplayName = building.buildingStatusDisplayName,
                            Description = building.buildingStatusDescription
                        },
                        BuildingEquipmentStatus = building.BuildingEquipmentStatus,
                    };
                    var groupList = (from DataRow dRow in dataSet.Tables[0].Rows
                                     select new
                                     {
                                         id = Convert.ToInt32(dRow[Constant.ID]),
                                         groupId = Convert.ToInt32(dRow[Constant.GROUPID]),
                                         //GroupConflictCheck = Convert.ToString(dRow[Constant.GROUPCONFLICTCHECK]), 
                                         groupName = Convert.ToString(dRow[Constant.GROUPNAME]),
                                         NeedsValidation = Convert.ToBoolean(dRow[Constant.NEEDSVALIDATION]),
                                         groupStatusKey = Convert.ToString(dRow["GroupStatusKey"]),
                                         groupStatusName = Convert.ToString(dRow["GroupStatusName"]),
                                         groupStatusDisplayName = Convert.ToString(dRow["GroupStatusDisplayName"]),
                                         groupStatusDescription = Convert.ToString(dRow["GroupStatusDescription"]),
                                         productCategory = Convert.ToString(dRow[Constant.PRODUCTCATEGORY_LOWERCASE]),
                                     }).ToList();

                    groupList = groupList.Where(x => x.id.Equals(building.id)).Distinct().ToList();
                    configurationList.Groups = new List<GrupConfiguration>();
                    foreach (var group in groupList)
                    {
                        if (group.groupId > 0)
                        {
                            GrupConfiguration groupConfiguration = new GrupConfiguration()
                            {
                                groupId = group.groupId,
                                groupName = group.groupName,
                                NeedsValidation = group.NeedsValidation,
                                productCategory = group.productCategory,
                                groupStatus = new Status()
                                {
                                    StatusKey = group.groupStatusKey,
                                    StatusName = group.groupStatusName,
                                    DisplayName = group.groupStatusDisplayName,
                                    Description = group.groupStatusDescription
                                },
                            };



                            var unitList = (from DataRow dRow in dataSet.Tables[0].Rows
                                            select new
                                            {
                                                groupId = Convert.ToInt32(dRow[Constant.GROUPID]),
                                                unitId = Convert.ToInt32(dRow[Constant.UNITID]),
                                                UnitConflictCheck = Convert.ToString(dRow[Constant.UNITSCONFLICTCHECK]),
                                                unitName = Convert.ToString(dRow[Constant.UNITNAME]),
                                                productName = Convert.ToString(dRow[Constant.PRODUCT]),
                                                price = (dRow[Constant.PRICE_CAMELCASE] != DBNull.Value) ? Convert.ToDecimal(dRow[Constant.PRICE_CAMELCASE]) : 0,
                                                capacity = Convert.ToString(dRow[Constant.CAPACITY]),
                                                speed = Convert.ToString(dRow[Constant.SPEED]),
                                                landings = Convert.ToInt32(dRow[Constant.LANDINGS]),
                                                frontOpening = Convert.ToInt32(dRow[Constant.FRONTOPENING]),
                                                rearOpening = Convert.ToInt32(dRow[Constant.REAROPENING]),
                                                setId = Convert.ToInt32(dRow[Constant.SETCONFIGURATIONID]),
                                                SetName = (dRow[Constant.SETNAME]).ToString(),
                                                unitStatusKey = Convert.ToString(dRow["UnitStatusKey"]),
                                                unitStatusName = Convert.ToString(dRow["UnitStatusName"]),
                                                unitStatusDisplayName = Convert.ToString(dRow["UnitStatusDisplayName"]),
                                                unitStatusDescription = Convert.ToString(dRow["UnitStatusDescription"]),
                                                Description = dRow[Constant.DESCRIPTION].ToString(),
                                                UEID = dRow[Constant.UEID].ToString(),
                                                FactoryJobId = Convert.ToString(dRow[Constants.FACTORYJOBID]),
                                                CreatedOn = Convert.ToDateTime(dRow[Constant.CREATEDON]),
                                                UnitPosition = Convert.ToString(dRow[Constant.UNITPOSITION])
                                            }).ToList();
                            unitList = unitList.Where(x => x.groupId.Equals(groupConfiguration.groupId)).Distinct().ToList();
                            groupConfiguration.Units = new List<Unit>();
                            foreach (var unit in unitList)
                            {
                                if (unit.unitId > 0)
                                {
                                    Unit unitConfig = new Unit()
                                    {
                                        unitId = unit.unitId,
                                        unitName = unit.unitName,
                                        Product = productNameMapperJson.ContainsKey(unit.productName)? productNameMapperJson[unit.productName]: unit.productName,
                                        ProductId = unit.productName,
                                        price = unit.price,
                                        capacity = unit.capacity,
                                        speed = unit.speed,
                                        Landings = unit.landings,
                                        FrontOpenings = unit.frontOpening,
                                        RearOpening = unit.rearOpening,
                                        SetId = unit.setId,
                                        SetName = unit.SetName,
                                        Status = new Status()
                                        {
                                            StatusKey = unit.unitStatusKey,
                                            StatusName = unit.unitStatusName,
                                            DisplayName = unit.unitStatusDisplayName,
                                            Description = unit.unitStatusDescription
                                        },
                                        Description = unit.Description,
                                        UEID = unit.UEID,
                                        Factory=new Factory()
                                        {
                                            FactoryJobId=unit.FactoryJobId,
                                        },
                                        CreatedOn = unit.CreatedOn,
                                    };
                                    if ((!viewUser && Utilities.CheckEquals(saleStage, Constants.ORDER)) || viewUser)
                                    {
                                        unitConfig.Factory.IsReadOnly = true;
                                    }
                                    else
                                    {
                                        unitConfig.Factory.IsReadOnly = false;
                                    }

                                    if (Utility.CheckEquals(unitConfig.Product, Constant.TWIN))
                                    {
                                        unitConfig.UnitPosition = unit.UnitPosition;
                                    }
                                    groupConfiguration.Units.Add(unitConfig);
                                    switch ((UnitConflictsStatus)Enum.Parse(typeof(UnitConflictsStatus), unit.UnitConflictCheck.Replace(" ", "")))
                                    {
                                        case UnitConflictsStatus.UNIT_INV:
                                            unitConfig.ConflictsStatus = ConflictsStatus.InValid;
                                            break;
                                        case UnitConflictsStatus.UNIT_VAL:
                                            unitConfig.ConflictsStatus = ConflictsStatus.Valid;
                                            break;
                                        case UnitConflictsStatus.UNIT_NV:
                                            unitConfig.ConflictsStatus = ConflictsStatus.NeedValidation;
                                            break;
                                        case UnitConflictsStatus.InValid:
                                            unitConfig.ConflictsStatus = ConflictsStatus.InValid;
                                            break;
                                        case UnitConflictsStatus.Valid:
                                            unitConfig.ConflictsStatus = ConflictsStatus.Valid;
                                            break;
                                        case UnitConflictsStatus.NeedValidation:
                                            unitConfig.ConflictsStatus = ConflictsStatus.NeedValidation;
                                            break;
                                        case UnitConflictsStatus.UNIT_CNV:
                                            unitConfig.ConflictsStatus = ConflictsStatus.InValid;
                                            break;
                                    }
                                    unitConfig.Status = unitConfig.Status;

                                }
                            }
                            var unitsInGroup = new List<Unit>();
                            var independentUnits = (from units in groupConfiguration.Units
                                                    orderby units.CreatedOn ascending
                                                    where units.SetId > 0
                                                    group units by units.SetId into q
                                                    where q.Count() == 1
                                                    select q).ToList();
                            var independentUnitsinGroup = new List<Unit>();
                            independentUnitsinGroup = (from units in groupConfiguration.Units
                                                       from independentUnit in independentUnits
                                                       orderby units.CreatedOn ascending
                                                       where units.SetId == independentUnit.Key
                                                       select units).ToList();

                            unitsInGroup = (from units in groupConfiguration.Units
                                                //from independentUnit in independentUnits
                                            orderby units.CreatedOn ascending
                                            where units.SetId == 0
                                            select units).ToList();
                            independentUnitsinGroup.AddRange(unitsInGroup);
                            unitsInGroup = independentUnitsinGroup;
                            foreach (var independentUnit in unitsInGroup)
                            {
                                groupConfiguration.Units.RemoveAll(x => x.SetId == independentUnit.SetId);
                            }
                            groupConfiguration.Units = (from units in groupConfiguration.Units
                                                            //from independentUnit in independentUnits
                                                        orderby units.SetId, units.CreatedOn ascending
                                                        where units.SetId > 0
                                                        select units).ToList();

                            groupConfiguration.Units.AddRange(unitsInGroup);


                            configurationList.Groups.Add(groupConfiguration);
                        }
                    }
                    ListConfiguration.Add(configurationList);
                }
            }
            Utility.LogEnd(methodStartTime);
            return ListConfiguration;
        }

        /// <summary>
        /// Get Building Details by building Id
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        public List<ConfigVariable> GetBuildingConfigurationById(int buildingId)
        {
            var methodStartTime = Utility.LogBegin();
            List<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@_ID, Value = buildingId }
            };
            List<ConfigVariable> lstBldng = new List<ConfigVariable>();
            DataSet dataSet = new DataSet();
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETBUILDINGCONFIGBYID, sqlParameters);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                lstBldng = JsonConvert.DeserializeObject<List<ConfigVariable>>(dataSet.Tables[0].Rows[0][Constant.BUILDINGJSON].ToString());
            }
            Utility.LogEnd(methodStartTime);
            return lstBldng;
        }

        /// <summary>
        /// Get Permission By Role of User
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        public List<string> GetPermissionByRole(int id, string roleName)
        {
            var methodStartTime = Utility.LogBegin();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@_ID, Value = id },
                new SqlParameter() { ParameterName = Constant.@ROLENAME, Value = roleName },
                new SqlParameter() { ParameterName = Constant.@ENTITY, Value = "Building" },
            };
            List<string> permission = new List<string>();
            DataSet ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETPERMISSIONBYROLENAME, sqlParameters);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dRow in ds.Tables[0].Rows)
                {
                    permission.Add(Convert.ToString(dRow[Constant.PERMISSIONKEY]));
                }

            }
            Utility.LogEnd(methodStartTime);
            return permission;
        }

        /// <summary>
        /// Save and Update Building details
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="userId"></param>
        /// <param Name="projectId"></param>
        /// <param Name="BuildingName"></param>
        /// <param Name="bldVariablejson"></param>
        /// <returns></returns>
        public List<Result> SaveBuildingConfigurationForProject(int buildingId, string userId, string quoteId, string buildingName, string bldVariablejson, ConflictsStatus isEditFlow, bool hasConflictsFlag, List<ConfigVariable> mapperVariables)
        {
            var methodStartTime = Utility.LogBegin();
            Result result = new Result();
            List<Result> lstResult = new List<Result>();
            DataTable variableMapperAssignment = Utility.GenerateVariableMapperDataTable(mapperVariables);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForAddandUpdateBuilding(buildingId, userId, quoteId, buildingName, bldVariablejson, isEditFlow, hasConflictsFlag, variableMapperAssignment);

            if (buildingId == 0)
            {

                int resultForSaveBuilding = CpqDatabaseManager.ExecuteNonquery(Constant.SPADDBUILDINGSFORPROJECT, lstSqlParameter);
                if (resultForSaveBuilding == -1)
                {
                    result.result = -1;
                    result.buildingId = 0;
                    result.message = Constant.BUILDINGNAMEEXISTS;
                }
                else if (resultForSaveBuilding == -2)
                {
                    result.result = -1;
                    result.buildingId = 0;
                    result.message = Constant.BUILDINGSAVEISSUES;
                }
                else
                {
                    result.result = 1;
                    result.buildingId = resultForSaveBuilding;
                    result.message = Constant.BUILDINGSAVEMESSAGE;
                }
                lstResult.Add(result);
            }
            else
            {

                int resultForUpdateBuilding = CpqDatabaseManager.ExecuteNonquery(Constant.SPUPDATEBUILDINGSFORPROJECT, lstSqlParameter);

                if (resultForUpdateBuilding == -1)
                {
                    result.result = -1;
                    result.buildingId = 0;
                    result.message = Constant.BUILDINGNAMEEXISTS;
                }
                else
                {
                    result.result = 1;
                    result.buildingId = buildingId;
                    result.message = Constant.UPDATEMESSAGE;
                }
                lstResult.Add(result);
            }
            Utility.LogEnd(methodStartTime);
            return lstResult;
        }

        /// <summary>
        /// Save Building Elevation Details
        /// </summary>
        /// <param Name="dtBuildingElevation"></param>
        /// <returns></returns>
        public List<Result> SaveBuildingElevation(DataTable dtBuildingElevation)
        {
            var methodStartTime = Utility.LogBegin();
            Result result = new Result();
            List<Result> lstResult = new List<Result>();



            IList<SqlParameter> lstSqlParameter = Utility.sqlParameterForBuildingElevation(dtBuildingElevation);

            resultBuildingElevation = CpqDatabaseManager.ExecuteNonquery(Constant.SPINSERTBUILDINGELEVATION, lstSqlParameter);
            if (resultBuildingElevation.Equals(1))
            {
                result.message = Constant.BUILDINGELEVATIONSUCCESSMESSAGE;
                lstResult.Add(result);
                var response = JArray.FromObject(lstResult);
                Utility.LogEnd(methodStartTime);
                return lstResult;
            }
            else
            {
                result.message = Constant.BUILDINGELEVATIONERRORMESSAGE;
                lstResult.Add(result);
                var response = JArray.FromObject(lstResult);
                Utility.LogEnd(methodStartTime);
                return lstResult;
            }
        }

        /// <summary>
        /// Autosave Building Elevation
        /// </summary>
        /// <param Name="dtBuildingElevation"></param>
        /// <returns></returns>
        public List<Result> AutoSaveBuildingElevation(DataTable dtBuildingElevation)
        {
            var methodStartTime = Utility.LogBegin();
            Result result = new Result();
            List<Result> lstResult = new List<Result>();



            IList<SqlParameter> lstSqlParameter = Utility.sqlParameterForBuildingElevation(dtBuildingElevation);

            resultBuildingElevation = CpqDatabaseManager.ExecuteNonquery(Constant.SPAUTOSAVEBUILDINGELEVATION, lstSqlParameter);

            if (resultBuildingElevation.Equals(1))
            {
                result.message = Constant.AUTOSAVEBUILDINGELESUCCESSMSG;
                lstResult.Add(result);
                var response = JArray.FromObject(lstResult);
                Utility.LogEnd(methodStartTime);
                return lstResult;
            }
            else
            {
                result.message = Constant.AUTOSAVEBUILDINGELEERRORMSG;
                lstResult.Add(result);
                var response = JArray.FromObject(lstResult);
                Utility.LogEnd(methodStartTime);
                return lstResult;
            }
        }

        /// <summary>
        /// Update Building Elevation
        /// </summary>
        /// <param Name="dtBuildingElevation"></param>
        /// <returns></returns>
        public List<Result> UpdateBuildingElevation(DataTable dtBuildingElevation, List<ConfigVariable> mapperVariables)
        {
            var methodStartTime = Utility.LogBegin();
            Result result = new Result();
            List<Result> lstResult = new List<Result>();
            DataTable variableMapperAssignment = Utility.GenerateVariableMapperDataTable(mapperVariables);
            IList<SqlParameter> lstSqlParameter = Utility.sqlParameterForBuildingElevation(dtBuildingElevation);
            SqlParameter sqlParameters = new SqlParameter()
            {
                ParameterName = Constant.VARIABLEMAPPERDATATABLE,
                Value = variableMapperAssignment,
                Direction = ParameterDirection.Input
            };
            lstSqlParameter.Add(sqlParameters);
            resultBuildingElevation = CpqDatabaseManager.ExecuteNonquery(Constant.SPUPDATEBUILDINGELEVATION, lstSqlParameter);

            if (resultBuildingElevation == 1)
            {
                result.message = Constant.BUILDINGELEVATIONUPDATEMSG;
                lstResult.Add(result);
                var response = JArray.FromObject(lstResult);
                Utility.LogEnd(methodStartTime);
                return lstResult;
            }
            else
            {
                result.message = Constant.BUILDINGELEVATIONUPDATEERRORMSG;
                lstResult.Add(result);
                var response = JArray.FromObject(lstResult);
                Utility.LogEnd(methodStartTime);
                return lstResult;
            }
        }

        /// <summary>
        /// Get Building Elevation details by BuildingId
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        public List<BuildingElevation> GetBuildingElevationById(int buildingId, List<ConfigVariable> mapperVariables, string sessionId)
        {
            var methodStartTime = Utility.LogBegin();
            List<BuildingElevation> buildingElevation = new List<BuildingElevation>();
            BuildingElevation building = new BuildingElevation();
            DataTable variableMapperAssignment = Utility.GenerateVariableMapperDataTable(mapperVariables);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@_ID, Value = buildingId }
                ,new SqlParameter() { ParameterName =  Constant.VARIABLEMAPPERDATATABLE,Value=variableMapperAssignment,Direction = ParameterDirection.Input }
            };
            DataSet dsBuildingElevation = new DataSet();
            dsBuildingElevation = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETBUILDINGELEVATIONBYID, sqlParameters);
            if (dsBuildingElevation != null && dsBuildingElevation.Tables.Count > 0 && dsBuildingElevation.Tables[0].Rows.Count > 0)
            {
                List<BuildingElevationData> lstBuildingElevationData = new List<BuildingElevationData>();

                foreach (DataRow row in dsBuildingElevation.Tables[0].Rows)
                {
                    BuildingElevationData buildingElevationData;

                    building.buildingConfigurationId = Convert.ToInt32(row["Id"]);
                    building.noOFFloor = Convert.ToInt32(row["numberOfFloor"]);
                    building.buildingRiseValue = Convert.ToDecimal(row["buildingRise"]); 
                    building.AvgRoofHeight = Convert.ToDecimal(row[Constants.AVGROOFHEIGHT]);


                    if (!String.IsNullOrEmpty(row["FloorDesignation"].ToString()))
                    {
                        buildingElevationData = new BuildingElevationData();
                        buildingElevationData.alternateEgress = Convert.ToBoolean(row["alternateEgress"]);
                        buildingElevationData.mainEgress = Convert.ToBoolean(row["mainEgress"]);
                        buildingElevationData.FloorNumber = Convert.ToInt32(row["FloorNumber"]);

                        buildingElevationData.floorDesignation = row["FloorDesignation"].ToString();

                        TypicalFloor elevationTypicalFloor = new TypicalFloor()
                        {
                            feet = Convert.ToInt32(row["elevationFeet"]),
                            inch = Convert.ToDecimal(row["elevationInch"]),
                        };
                        buildingElevationData.elevation = elevationTypicalFloor;

                        TypicalFloor floorToFloorTypical = new TypicalFloor()
                        {
                            feet = Convert.ToInt32(row["floorToFloorHeightFeet"]),
                            inch = Convert.ToDecimal(row["floorToFloorHeightInch"]),
                        };
                        buildingElevationData.floorToFloorHeight = floorToFloorTypical;
                        lstBuildingElevationData.Add(buildingElevationData);
                    }


                }
                if(dsBuildingElevation.Tables[1].Rows.Count>0)
                {
                    var isEditFlag = (Int32)dsBuildingElevation.Tables[1].Rows[0][Constant.ISEDITFLAG];
                    _cpqCacheManager.SetCache(sessionId, _environment, buildingId.ToString(), Constants.BUILDINGELEVATIONISEDITFLAG, Utility.SerializeObjectValue(isEditFlag));
                }
                building.buildingElevation = lstBuildingElevationData;
            }

            buildingElevation.Add(building);
            Utility.LogEnd(methodStartTime);
            return buildingElevation;
        }

        /// <summary>
        /// Delete Building Details by buildingConfigurationId
        /// </summary>
        /// <param Name="buildingConfigurationId"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public List<Result> DeleteBuildingConfigurationById(int buildingConfigurationId, string userId)
        {
            var methodStartTime = Utility.LogBegin();
            Result resultBuildingConfiguration = new Result();
            List<Result> lstResult = new List<Result>();



            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
              new SqlParameter() { ParameterName = Constant.@BUILDINGCONFIGID,Value=buildingConfigurationId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int},
              new SqlParameter() { ParameterName = Constant.@MODIFIEDBY,Value=userId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar,Size=100},
              new SqlParameter() { ParameterName = Constant.@RESULT,Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int},
            };
            int result = CpqDatabaseManager.ExecuteNonquery(Constant.SPDELETEBUILDINGCONFIGBYID, sqlParameters);

            if (result == 1)
            {
                resultBuildingConfiguration.buildingId = buildingConfigurationId;
                resultBuildingConfiguration.message = Constant.BUILDINGDELETIONSUCCESSMSG;
                lstResult.Add(resultBuildingConfiguration);
            }
            else
            {
                resultBuildingConfiguration.buildingId = buildingConfigurationId;
                resultBuildingConfiguration.message = Constant.BUILDINGDELETIONERRORMSG;
                lstResult.Add(resultBuildingConfiguration);
            }
            Utility.LogEnd(methodStartTime);
            return lstResult;
        }

        /// <summary>
        /// Delete Building Elevation Details by buildingConfigurationId
        /// </summary>
        /// <param Name="buildingConfigurationId"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public List<Result> DeleteBuildingElevationById(int buildingConfigurationId, string userId)
        {
            var methodStartTime = Utility.LogBegin();
            Result resultBuildingConfiguration = new Result();
            List<Result> lstResult = new List<Result>();


            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
               new SqlParameter() { ParameterName = Constant.@BUILDINGCONFIGID ,Value=buildingConfigurationId},
               new SqlParameter() { ParameterName = Constant.@MODIFIEDBY ,Value=userId}
            };

            int result = CpqDatabaseManager.ExecuteNonquery(Constant.SPDELETEBUILDINGELEVATIONBYID, sqlParameters, string.Empty);

            if (result == -1)
            {
                resultBuildingConfiguration.message = Constant.BUILDINGELEVATIONDELETIONSUCCESSMSG;
                lstResult.Add(resultBuildingConfiguration);
            }
            else
            {
                resultBuildingConfiguration.message = Constant.BUILDINGELEVATIONDELETIONERRORMSG;
                lstResult.Add(resultBuildingConfiguration);
            }
            Utility.LogEnd(methodStartTime);
            return lstResult;
        }

        /// <summary>
        /// Generate Building Name Automatically by projectId
        /// </summary>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        public int GenerateBuildingName(string quoteId)
        {
            var methodStartTime = Utility.LogBegin();
            var sqlParam = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@PROJECTID,Value=0,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
                ,new SqlParameter() { ParameterName = Constant.@QUOTEIDSPPARAMETERLATEST,Value=quoteId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar }
            };
            var buildingCount = CpqDatabaseManager.ExecuteScalar(Constant.SPGETNUMBEROFBUILDINGS, sqlParam);
            Utility.LogEnd(methodStartTime);
            return buildingCount;
        }

        /// <summary>
        /// Get Product Category By SetId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetProductCategoryBySetId(int id, string type)
        {
            var methodBeginTime = Utility.LogBegin();
            string productCategory = string.Empty;
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.@_ID, id),
                new SqlParameter(Constant.@TYPE, type),
            };

            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETPRODUCTCATEGORYBYGROUPID, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                productCategory = Convert.ToString(ds.Tables[0].Rows[0][Constant.GROUPCONFIGURATIONVALUE]);
            }
            Utility.LogEnd(methodBeginTime);
            return productCategory;
        }

        /// <summary>
        ///  Get Summary Details Based on BuildingId/groupId/setId
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="groupId"></param>
        /// <param Name="setId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public QuickSummary QuickConfigurationSummary(string opportunityId, int buildingId, int groupId, int setId, string sessionId)
        {
            var methodStartTime = Utility.LogBegin();
            QuickSummary quickSummaryList = new QuickSummary();
            OpportunityDetails opportunityDetails = new OpportunityDetails();
            UnitsTable units = new UnitsTable();
            UnitConfigurationDetails1 unitConfigurationDetails = new UnitConfigurationDetails1();
            DataSet summarySet = new DataSet();
            var hallStationVariables = JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES))[Constants.HALLSTATIONS].ToList();
            var fireServiceVariables = Utility.VariableMapper(Constant.GROUPMAPPERVARIABLES, Constant.FIRESERVICEHALLSTATION);
            IList<SqlParameter> sqlParam = new List<SqlParameter>()
            {
               new SqlParameter() { ParameterName = Constant.@QUOTEIDSPPARAMETER ,Value=opportunityId},
               new SqlParameter() { ParameterName = Constant.@BUILDINGCONFIGID ,Value=buildingId},
               new SqlParameter() { ParameterName = Constant.@GRPCONFIGID ,Value=groupId},
                new SqlParameter() { ParameterName = Constant.SETID ,Value=setId}
            };
            summarySet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETQUICKSUMMARYCONFIGBYID, sqlParam);
            if (summarySet != null && summarySet.Tables.Count > 0)
            {
                if (summarySet.Tables[0].Rows.Count > 0)
                {

                    opportunityDetails.projectDetails = (from DataRow row in summarySet.Tables[0].Rows
                                                         where summarySet.Tables[0].Columns.Contains(Constant.NUMBEROFBUILDINGS)
                                                         select new ProjectDetails
                                                         {
                                                             numberOfBuildings = Convert.ToInt32(row[Constant.NUMBEROFBUILDINGS])
                                                         }).FirstOrDefault();

                    quickSummaryList.project = new OpportunityDetails();
                    quickSummaryList.project.projectDetails = new ProjectDetails();
                    quickSummaryList.project.projectDetails = opportunityDetails.projectDetails;
                    quickSummaryList.project.Id = Convert.ToString(summarySet.Tables[0].Rows[0][Constant.QUOTEID]);

                    {
                        quickSummaryList.building = (from DataRow row in summarySet?.Tables[0].Rows
                                                     where summarySet.Tables[0].Columns.Contains(Constant.BLDGID)
                                                     select new BuildingDetails
                                                     {
                                                         id = Convert.ToInt32(row[Constant.BLDGID]),
                                                         name = Convert.ToString(row[Constant.BLDGNAME]),
                                                         numberOfGroups = Convert.ToInt32(row[Constant.NUMBEROFGROUPS])
                                                     }).FirstOrDefault();
                    }
                    {
                        quickSummaryList.group = (from DataRow row in summarySet?.Tables[0].Rows
                                                  where summarySet.Tables[0].Columns.Contains(Constant.GROUPID)
                                                  select new GroupDetails
                                                  {
                                                      id = Convert.ToInt32(row[Constant.GROUPID]),
                                                      name = Convert.ToString(row[Constant.GRPNAME]),
                                                      numberOfUnits = Convert.ToInt32(row[Constant.NUMBEROFUNITS]),
                                                  }).FirstOrDefault();
                        if (groupId > 0)
                        {
                            quickSummaryList.group.unitLayoutDetails = (from DataRow row in summarySet?.Tables[0].Rows
                                                                        where summarySet.Tables[0].Columns.Contains(Constant.UNITID)
                                                                        select new UnitLayOutDetails
                                                                        {
                                                                            unitDesignation = Convert.ToString(row[Constant.UNITSDESIGNATION]),
                                                                            displayCarPosition = Convert.ToString(row[Constant.DISPLAYCARPOSITION]),
                                                                        }).ToList();
                            if (summarySet?.Tables[2].Rows.Count > 0)
                            {
                                 var hallStationDetails = (from DataRow row in summarySet?.Tables[2].Rows
                                                                              where summarySet.Tables[2].Columns.Contains(Constant.CONTROLLOCATIONTYPE)
                                                                              select new ConfigVariable
                                                                              {
                                                                                  VariableId = Convert.ToString(row[Constant.CONTROLLOCATIONTYPE]),
                                                                                  Value = Convert.ToString(row[Constant.CONTROLLOCATIONVALUE])
                                                                              }).ToList();
                                quickSummaryList.group.hallStationDetails = new List<HallStationDetails>();
                                foreach(var hallStation in hallStationVariables)
                                {
                                    
                                        quickSummaryList.group.hallStationDetails.Add(new HallStationDetails
                                        {
                                            hallStationVariables = new ConfigVariable
                                            {
                                                VariableId = hallStation.ToString(),
                                                Value = (from x in hallStationDetails 
                                                        where x.VariableId.Equals(hallStation.ToString()) 
                                                        select (x.Value.ToString().Equals(Constants.True))).FirstOrDefault()
                                            },
                                            key = (hallStation.ToString().Split(Constants.DOTCHAR)[1].ToString()).Split(Constants.UNDERSCORECHAR)[0].ToString()
                                        });
                                    
                                    
                                }
                            }
                            

                            if (!String.IsNullOrEmpty(quickSummaryList?.group?.unitLayoutDetails?.FirstOrDefault().displayCarPosition))
                            {
                                foreach (var Unitdoordetails in quickSummaryList.group.unitLayoutDetails)
                                {
                                    DoorOpenings doorOpenings = new DoorOpenings();

                                    doorOpenings.frontDoor = (from DataRow row in summarySet?.Tables[0].Rows
                                                              where summarySet.Tables[0].Columns.Contains(Constant.UNITID) && Utility.CheckEquals(Convert.ToString(row[Constant.UNITDESIGNATION]), Unitdoordetails.unitDesignation)
                                                              select new Front
                                                              {
                                                                  isSelected = false,
                                                                  doorTypeHand = (Convert.ToString(row[Constant.FRONTDOOR])) == String.Empty ? Constant.RIGHT : Convert.ToString(row[Constant.FRONTDOOR])

                                                              }).FirstOrDefault();


                                    doorOpenings.rearDoor = (from DataRow row in summarySet?.Tables[0].Rows
                                                             where summarySet.Tables[0].Columns.Contains(Constant.UNITID) && Utility.CheckEquals(Convert.ToString(row[Constant.UNITDESIGNATION]), Unitdoordetails.unitDesignation)
                                                             select new Front
                                                             {
                                                                 isSelected = false,
                                                                 doorTypeHand = Convert.ToString(row[Constant.REARDOOR])

                                                             }).FirstOrDefault();
                                    doorOpenings.leftSideDoor = (from DataRow row in summarySet?.Tables[0].Rows
                                                                 where summarySet.Tables[0].Columns.Contains(Constant.UNITID) && Utility.CheckEquals(Convert.ToString(row[Constant.UNITDESIGNATION]), Unitdoordetails.unitDesignation)
                                                                 select new Front
                                                                 {
                                                                     isSelected = false,

                                                                 }).FirstOrDefault();
                                    doorOpenings.rightSideDoor = (from DataRow row in summarySet?.Tables[0].Rows
                                                                  where summarySet.Tables[0].Columns.Contains(Constant.UNITID) && Utility.CheckEquals(Convert.ToString(row[Constant.UNITDESIGNATION]), Unitdoordetails.unitDesignation)
                                                                  select new Front
                                                                  {
                                                                      isSelected = false,

                                                                  }).FirstOrDefault();

                                    if (!string.IsNullOrEmpty(doorOpenings.frontDoor.doorTypeHand))
                                    {
                                        doorOpenings.frontDoor.isSelected = true;
                                    }
                                    if (!string.IsNullOrEmpty(doorOpenings.rearDoor.doorTypeHand))
                                    {
                                        doorOpenings.rearDoor.isSelected = true;
                                    }
                                    Unitdoordetails.doorOpenings = doorOpenings;
                                }
                            }
                            var fireSreviceHallStation = new ConfigVariable();
                            if (summarySet?.Tables[3].Rows.Count > 0)
                            {
                                 fireSreviceHallStation = (from DataRow row in summarySet?.Tables[3].Rows
                                                              where summarySet.Tables[3].Columns.Contains(Constant.FIRESERVICEHALLSTATION)
                                                              select new ConfigVariable
                                                              {
                                                                  VariableId = Convert.ToString(row[Constant.FIRESERVICEHALLSTATION]),
                                                                  Value = true
                                                              }).FirstOrDefault();
                            }
                            foreach(var hallStation in fireServiceVariables.Keys)
                            {
                                if (hallStation.Equals(fireSreviceHallStation?.VariableId, StringComparison.OrdinalIgnoreCase))
                                {
                                    var fireService = new HallStationDetails
                                    {
                                        hallStationVariables = new ConfigVariable
                                        {
                                            VariableId = fireServiceVariables[fireSreviceHallStation.VariableId],
                                            Value = true
                                        },
                                        key = (fireServiceVariables[fireSreviceHallStation.VariableId].ToString().Split(Constants.DOTCHAR)[1].ToString()).Split(Constants.UNDERSCORECHAR)[0].ToString()
                                    };
                                    quickSummaryList.group.hallStationDetails.Add(fireService);
                                }
                            }
                        }                            
                       
                    }
                    if (setId > 0)
                    {
                        var productNameMapperJson = Utility.VariableMapper(Constants.PRODUCTSELECTIONCONSTANTTEMPLATEPATH, Constants.PROJECTCOMMONNAME);
                        units = (from DataRow row in summarySet.Tables[0].Rows
                                 where summarySet.Tables[0].Columns.Contains(Constant.UNITID)
                                 select new UnitsTable
                                 {
                                     model = productNameMapperJson.ContainsKey(Convert.ToString(row[Constant.MODEL]))? productNameMapperJson[Convert.ToString(row[Constant.MODEL])]: Convert.ToString(row[Constant.MODEL])
                                 }).FirstOrDefault();

                        units.selectedUnits = (from DataRow row in summarySet.Tables[0].Rows
                                               where summarySet.Tables[0].Columns.Contains(Constant.UNITID) && Convert.ToBoolean(row[Constant.UNITCURRENTLYCONFIGURED]) == true
                                               select new SelectedUnits
                                               {

                                                   ueid = Convert.ToString(row[Constant.UEID]),
                                                   unitid = Convert.ToInt32(row[Constant.UNITID]),
                                                   unitname = Convert.ToString(row[Constant.UNITNAME])

                                               }).Distinct().ToList();
                        var ceScreenList = JObject.Parse(File.ReadAllText(Constants.UNITCONSTANTMAPPERTEMPLATE));
                        var ceScreenParameter = ceScreenList[Constants.CESCREENLIST].ToList();
                        if (!ceScreenParameter.Contains(units.model))
                        {
                            unitConfigurationDetails = (from DataRow row in summarySet.Tables[0].Rows
                                                        where summarySet.Tables[0].Columns.Contains(Constant.UNITID)
                                                        select new UnitConfigurationDetails1
                                                        {
                                                            capacity = Convert.ToString(row[Constant.CAPACITY]),
                                                            speed = Convert.ToString(row[Constant.SPEED]),
                                                            status = Convert.ToString(row[Constant.STATUS]),
                                                            width = Convert.ToString(row[Constant.WIDTH]),
                                                            depth = Convert.ToString(row[Constant.DEPTH]),
                                                            pitDepth = Convert.ToString(row[Constant.PITDEPTH]),
                                                            overHead = Convert.ToString(row[Constant.OVERHEAD]),
                                                            dimensionSelection = (Convert.ToString(row[Constant.DIMENSIONSELECTION]) == String.Empty ? Constant.MINIMUM : Convert.ToString(row[Constant.DIMENSIONSELECTION])),
                                                            machineType = Convert.ToString(row[Constant.MACHINETYPE]),
                                                            motorTypeSize = Convert.ToString(row[Constant.MOTORTYPESIZE]),
                                                            availableFinishWeight = Convert.ToString(row[Constant.AVGFINISHWEIGHT]),
                                                            grossLoadOnJacks = Convert.ToString(row[Constant.GROSSLOADONJACK]),
                                                            grossLoadOnPowerUnit = Convert.ToString(row[Constant.GROSSLOADONPOWER])
                                                        }).FirstOrDefault();
                        }
                        units.unitDetails = unitConfigurationDetails;

                        units.openingDetails = (from DataRow row in summarySet.Tables[1].Rows
                                                where summarySet.Tables[1].Columns.Contains(Constant.UNITID)
                                                select new OpeningDetail
                                                {
                                                    travel = new TypicalFloor
                                                    {
                                                        feet = row[Constant.TRAVELFEET] == DBNull.Value ? 0 : Convert.ToInt32(row[Constant.TRAVELFEET]),
                                                        inch = row[Constant.TRAVELINCH] == DBNull.Value ? 0 : Convert.ToDecimal(row[Constant.TRAVELINCH])
                                                    },
                                                    frontOpenings = Convert.ToInt32(row[Constant.FRONTOPENINGS]),
                                                    rearOpenings = Convert.ToInt32(row[Constant.REAROPENINGS]),
                                                    floorsServed = Convert.ToInt32(row[Constant.FLOORSSERVED])
                                                }).FirstOrDefault();

                        units.unitLayoutDetails = (from DataRow row in summarySet.Tables[0].Rows
                                                   where summarySet.Tables[0].Columns.Contains(Constant.UNITID)
                                                   select new UnitLayOutDetails
                                                   {
                                                       unitDesignation = Convert.ToString(row[Constant.UNITSDESIGNATION]),
                                                       displayCarPosition = Convert.ToString(row[Constant.DISPLAYCARPOSITION]),
                                                       unitCurrentlyConfigured = Convert.ToBoolean(row[Constant.UNITCURRENTLYCONFIGURED])
                                                   }).ToList();

                        foreach (var Unitdoordetails in units.unitLayoutDetails)
                        {
                            DoorOpenings doorOpenings = new DoorOpenings();

                            doorOpenings.frontDoor = (from DataRow row in summarySet.Tables[0].Rows
                                                      where summarySet.Tables[0].Columns.Contains(Constant.UNITID) && Utility.CheckEquals(Convert.ToString(row[Constant.UNITDESIGNATION]), Unitdoordetails.unitDesignation)
                                                      select new Front
                                                      {
                                                          isSelected = false,
                                                          doorTypeHand = (Convert.ToString(row[Constant.FRONTDOOR])) == String.Empty ? Constant.RIGHT : Convert.ToString(row[Constant.FRONTDOOR])

                                                      }).FirstOrDefault();


                            doorOpenings.rearDoor = (from DataRow row in summarySet.Tables[0].Rows
                                                     where summarySet.Tables[0].Columns.Contains(Constant.UNITID) && Utility.CheckEquals(Convert.ToString(row[Constant.UNITDESIGNATION]), Unitdoordetails.unitDesignation)
                                                     select new Front
                                                     {
                                                         isSelected = false,
                                                         doorTypeHand = Convert.ToString(row[Constant.REARDOOR])

                                                     }).FirstOrDefault();
                            doorOpenings.leftSideDoor = (from DataRow row in summarySet.Tables[0].Rows
                                                         where summarySet.Tables[0].Columns.Contains(Constant.UNITID) && Utility.CheckEquals(Convert.ToString(row[Constant.UNITDESIGNATION]), Unitdoordetails.unitDesignation)
                                                         select new Front
                                                         {
                                                             isSelected = false,

                                                         }).FirstOrDefault();
                            doorOpenings.rightSideDoor = (from DataRow row in summarySet.Tables[0].Rows
                                                          where summarySet.Tables[0].Columns.Contains(Constant.UNITID) && Utility.CheckEquals(Convert.ToString(row[Constant.UNITDESIGNATION]), Unitdoordetails.unitDesignation)
                                                          select new Front
                                                          {
                                                              isSelected = false,

                                                          }).FirstOrDefault();

                            if (!string.IsNullOrEmpty(doorOpenings.frontDoor.doorTypeHand))
                            {
                                doorOpenings.frontDoor.isSelected = true;
                            }
                            if (!string.IsNullOrEmpty(doorOpenings.rearDoor.doorTypeHand))
                            {
                                doorOpenings.rearDoor.isSelected = true;
                            }
                            Unitdoordetails.doorOpenings = doorOpenings;
                        }
                        quickSummaryList.units = units;
                    }
                    _cpqCacheManager.SetCache(sessionId, _environment, groupId.ToString(), Utility.SerializeObjectValue(quickSummaryList.group));
                }
                else
                {
                    quickSummaryList.building = new BuildingDetails();
                    quickSummaryList.group = new GroupDetails();
                    quickSummaryList.units = new UnitsTable();
                    quickSummaryList.units.unitDetails = new UnitConfigurationDetails1();
                    quickSummaryList.project = new OpportunityDetails();
                    quickSummaryList.project.Id = opportunityId;
                }
            }
            Utility.LogEnd(methodStartTime);
            return quickSummaryList;
        }

        private object GetCrosspackageVariableAssignments(string sessionId, string gROUPCONFIGURATION)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if Group Exists
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        public bool CheckGroupExists(int buildingId)
        {
            var methodStartTime = Utility.LogBegin();
            IList<SqlParameter> sqlParam = new List<SqlParameter>()
            {
               new SqlParameter() { ParameterName = Constant.@BUILDINGCONFIGID ,Value=buildingId},
               new SqlParameter() { ParameterName = Constant.RESULT ,Value=buildingId,Direction = ParameterDirection.Output}
            };
            Utility.LogEnd(methodStartTime);
            return (Convert.ToBoolean(CpqDatabaseManager.ExecuteNonquery(Constant.SPCHECKIFGROUPEXISTFORABUILDING, sqlParam)));
        }

        /// <summary>
        /// Duplicate Building Configuration By Id
        /// </summary>
        /// <param Name="buildingIDDataTable"></param>
        /// <param Name="quoteId"></param>
        /// <returns></returns>
        public DataSet DuplicateBuildingConfigurationById(DataTable buildingIDDataTable, string quoteId)
        {
            var methodStartTime = Utility.LogBegin();
            List<Result> lstResult = new List<Result>();
            IList<SqlParameter> listOfInputs;
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
            listOfInputs = new List<SqlParameter>()
                {
                    new SqlParameter() { ParameterName = Constant.BUILDINGIDLIST,Value=buildingIDDataTable,Direction = ParameterDirection.Input },
                    new SqlParameter() { ParameterName = Constant.QUOTEIDSPPARAMETERLATEST,Value=quoteId,Direction = ParameterDirection.Input},
                    new SqlParameter() { ParameterName = Constant.@RESULT,Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int},
                    new SqlParameter() { ParameterName =  Constant.VARIABLEMAPPERDATATABLE,Value=variableMapperAssignment,Direction = ParameterDirection.Input }
            };
            Utility.LogEnd(methodStartTime);
            return CpqDatabaseManager.ExecuteDataSet(Constant.SPDUPLICATEBUILDINGCONFIGURATIONBYBUILDINGID, listOfInputs);
        }

        /// <summary>
        /// Get Building Equipment isDisabled
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        public bool GetBuildingConfigurationSectionTab(int buildingId)
        {
            var methodStartTime = Utility.LogBegin();
            bool isDisabled = false;
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
               new SqlParameter() { ParameterName = Constant.BUILDINGID_CAMELCASE ,Value=buildingId}
            };
            DataSet dataSet = new DataSet();
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.FNGETBUILDINGEQUIPMENTBYBUILDINGID, sqlParameters);
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    isDisabled = Convert.ToBoolean(dataSet.Tables[0].Rows[0][Constant.ISDISABLED]);
                }
            }
            Utility.LogEnd(methodStartTime);
            return isDisabled;
        }

        /// <summary>
        /// Get Log History of Building
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="lastDate"></param>
        /// <returns></returns>
        public LogHistoryResponse GetLogHistoryBuilding(int BuildingId, string lastDate)
        {
            var methodStartTime = Utility.LogBegin();
            LogHistoryResponse response = new LogHistoryResponse();
            response.Data = new List<Data>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
               new SqlParameter() { ParameterName = Constant.BUILDINGID ,Value=BuildingId}
            };
            DataSet dataSetForBuild = new DataSet();
            if (!string.IsNullOrEmpty(lastDate))
            {
                var culture = System.Globalization.CultureInfo.CurrentCulture;
                sqlParameters.Add(new SqlParameter(Constant.DATE, DateTime.ParseExact(lastDate, Constant.MMDDYYYYFORMAT, culture)));
            }
            else
            {
                sqlParameters.Add(new SqlParameter(Constant.DATE, DBNull.Value));
            }
            dataSetForBuild = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETLOGHISTORYBUILDING, sqlParameters);
            if (dataSetForBuild != null)
            {
                if (dataSetForBuild.Tables.Count > 0)
                {
                    if (dataSetForBuild.Tables[0].Rows.Count > 0)
                    {
                        var lstDate = (from DataRow row in dataSetForBuild.Tables[0].Rows
                                       select new
                                       {
                                           BuildingId = Convert.ToInt32(row[Constant.BUILDINGIDCOLUMNNAME]),
                                           date = Convert.ToDateTime(row[Constant.CREATEDON]).ToString(Constant.MMDDYYYYFORMAT)
                                       }).Distinct();
                        var LogHistory = (from DataRow row in dataSetForBuild.Tables[0].Rows
                                          select new
                                          {
                                              BuildingId = Convert.ToInt32(row[Constant.BUILDINGIDCOLUMNNAME]),
                                              date = Convert.ToDateTime(row[Constant.CREATEDON]).ToString(Constant.MMDDYYYYFORMAT),
                                              variableId = Convert.ToString(row[Constant.VARIABLEID]),
                                              currentValue = Convert.ToString(row[Constant.CURRENTVALUE]),
                                              previousValue = Convert.ToString(row[Constant.PREVIOUSVALUE]),
                                              user = Convert.ToString(row[Constant.CREATEDBY]),
                                              time = Convert.ToDateTime(row[Constant.CREATEDON]).ToString("hh:mm tt")

                                          }).Distinct();
                        List<Data> lstData = new List<Data>();
                        if (lstDate.Any())
                        {
                            foreach (var varDate in lstDate)
                            {
                                Data data = new Data();
                                data.Date = varDate.date;
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
                if (dataSetForBuild.Tables.Count > 1)
                {
                    if (dataSetForBuild.Tables[1].Rows.Count > 0)
                    {
                        var designation = Convert.ToString(dataSetForBuild.Tables[1].Rows[0][Constant.DESIGNATION]);
                        response.Description = designation;
                        response.Section = Constant.BUILDING;
                    }
                }
                if (dataSetForBuild.Tables.Count > 2)
                {
                    if (dataSetForBuild.Tables[2].Rows.Count > 0)
                    {
                        var showLoadMore = Convert.ToBoolean(dataSetForBuild.Tables[2].Rows[0][Constant.SHOWLOADMORE]);
                        response.ShowLoadMore = showLoadMore;
                    }
                }
            }

            Utility.LogEnd(methodStartTime);
            return response;
        }

        /// <summary>
        /// Get Quote Details
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <returns></returns>
        public DataSet GetQuoteDetails(string quoteId)
        {
            var methodStartTime = Utility.LogBegin();
            var quoteDetails = new DataSet();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
               new SqlParameter() { ParameterName = Constant.QUOTEIDSPPARAMETERLATEST ,Value=quoteId}
            };
            quoteDetails = CpqDatabaseManager.ExecuteDataSet(Constant.USPGETQUOTEDETAILS, sqlParameters);
            Utility.LogEnd(methodStartTime);
            return quoteDetails;
        }

        /// <summary>
        /// Get Permission For Configuration
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <param Name="roleName"></param>
        /// <returns></returns>
        public List<Permissions> GetPermissionForConfiguration(string quoteId, string roleName)
        {
            var methodStartTime = Utility.LogBegin();
            List<Permissions> lstPermission = new List<Permissions>();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
               new SqlParameter() { ParameterName = Constant.@_ID ,Value = quoteId},
               new SqlParameter() { ParameterName = Constant.@ROLENAME ,Value=roleName},
               new SqlParameter() { ParameterName = Constant.@ENTITY ,Value="ListofConfiguration"}
            };
            DataSet dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETPERMISSIONBYROLENAME, sqlParameters);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dRow in dataSet.Tables[0].Rows)
                {
                    var permission = new Permissions()
                    {
                        Entity = Convert.ToString(dRow[Constant.ENTITYNAME]),
                        PermissionKey = Convert.ToString(dRow[Constant.PERMISSIONKEY]),
                        BuildingStatus = Convert.ToString(dRow[Constant.BUILDINGSTATUS]),
                        GroupStatus = Convert.ToString(dRow[Constant.GROUPSTATUS_CAMEL]),
                        UnitStatus = Convert.ToString(dRow[Constant.UNITSTATUS])
                    };
                    lstPermission.Add(permission);
                }

            }
            Utility.LogEnd(methodStartTime);
            return lstPermission;
        }


        public async Task<ResponseMessage> GetListOfConfigurationForQuote(string quoteId, string sessionId)
        {

            var methodStartTime = Utility.LogBegin();
            var sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@QUOTEIDSPPARAMETER ,Value=quoteId,Direction = ParameterDirection.Input }
            };

            DataSet dataSet = new DataSet();
            ListofConfigurationForQuote ListConfiguration = new ListofConfigurationForQuote()
            {
                Configuration = new Configuration()
                {
                    Buildings = new List<Buildings>(),
                    Permissions = new List<string>()
                },
                ProjectDetails = new ProjectDetail()
            };
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETBUILDINGCONFIGFORPROJECT, sp);
            if (dataSet?.Tables?.Count > 0)
            {
                var j = dataSet.Tables[0].Rows.Count;

                var buildingConfigList = (from DataRow dRow in dataSet.Tables[0].Rows
                                          select new
                                          {
                                              id = Convert.ToInt32(dRow[Constant.ID]),
                                              buildingName = Convert.ToString(dRow[Constant.BUILDINGNAME]),
                                              //BuildingConflictCheck = Convert.ToString(dRow[Constant.BUILDINGCONFLICTCHECK]), 
                                              BuildingEquipmentStatus = Convert.ToString(dRow[Constant.BUILDINGEQUIPMENTSTATUS]),
                                              buildingStatusKey = Convert.ToString(dRow["BuildingStatusKey"]),
                                              buildingStatusName = Convert.ToString(dRow["BuildingStatusName"]),
                                              buildingStatusDisplayName = Convert.ToString(dRow["BuildingStatusDisplayName"]),
                                              buildingStatusDescription = Convert.ToString(dRow["BuildingStatusDescription"])
                                          }).Distinct();
                foreach (var building in buildingConfigList)
                {
                    Buildings configurationList = new Buildings()
                    {
                        BuildingName = building.buildingName,
                        Id = building.id,
                        BuildingStatus = new Status()
                        {
                            StatusKey = building.buildingStatusKey,
                            StatusName = building.buildingStatusName,
                            DisplayName = building.buildingStatusDisplayName,
                            Description = building.buildingStatusDescription
                        },
                        BuildingEquipmentStatus = building.BuildingEquipmentStatus,
                    };
                    var groupList = (from DataRow dRow in dataSet.Tables[0].Rows
                                     select new
                                     {
                                         id = Convert.ToInt32(dRow[Constant.ID]),
                                         groupId = Convert.ToInt32(dRow[Constant.GROUPID]),
                                         //GroupConflictCheck = Convert.ToString(dRow[Constant.GROUPCONFLICTCHECK]), 
                                         groupName = Convert.ToString(dRow[Constant.GROUPNAME]),
                                         NeedsValidation = Convert.ToBoolean(dRow[Constant.NEEDSVALIDATION]),
                                         groupStatusKey = Convert.ToString(dRow["GroupStatusKey"]),
                                         groupStatusName = Convert.ToString(dRow["GroupStatusName"]),
                                         groupStatusDisplayName = Convert.ToString(dRow["GroupStatusDisplayName"]),
                                         groupStatusDescription = Convert.ToString(dRow["GroupStatusDescription"])
                                     }).ToList();
                    groupList = groupList.Where(x => x.id.Equals(building.id)).Distinct().ToList();
                    configurationList.Groups = new List<Groups>();
                    foreach (var group in groupList)
                    {
                        if (group.groupId > 0)
                        {
                            Groups groupConfiguration = new Groups()
                            {
                                GroupId = group.groupId,
                                GroupName = group.groupName,
                                GroupStatus = new Status()
                                {
                                    StatusKey = group.groupStatusKey,
                                    StatusName = group.groupStatusName,
                                    DisplayName = group.groupStatusDisplayName,
                                    Description = group.groupStatusDescription
                                },
                            };

                            var unitList = (from DataRow dRow in dataSet.Tables[0].Rows
                                            select new
                                            {
                                                groupId = Convert.ToInt32(dRow[Constant.GROUPID]),
                                                unitId = Convert.ToInt32(dRow[Constant.UNITID]),
                                                UnitConflictCheck = Convert.ToString(dRow[Constant.UNITSCONFLICTCHECK]),
                                                unitName = Convert.ToString(dRow[Constant.UNITNAME]),
                                                productName = Convert.ToString(dRow[Constant.PRODUCT]),
                                                capacity = Convert.ToString(dRow[Constant.CAPACITY]),
                                                speed = Convert.ToString(dRow[Constant.SPEED]),
                                                landings = Convert.ToInt32(dRow[Constant.LANDINGS]),
                                                frontOpening = Convert.ToInt32(dRow[Constant.FRONTOPENING]),
                                                rearOpening = Convert.ToInt32(dRow[Constant.REAROPENING]),
                                                setId = Convert.ToInt32(dRow[Constant.SETCONFIGURATIONID]),
                                                SetName = (dRow[Constant.SETNAME]).ToString(),
                                                unitStatusKey = Convert.ToString(dRow["UnitStatusKey"]),
                                                unitStatusName = Convert.ToString(dRow["UnitStatusName"]),
                                                unitStatusDisplayName = Convert.ToString(dRow["UnitStatusDisplayName"]),
                                                unitStatusDescription = Convert.ToString(dRow["UnitStatusDescription"]),
                                                Description = dRow[Constant.DESCRIPTION].ToString(),
                                                UEID = dRow[Constant.UEID].ToString(),
                                                FactoryJobId= Convert.ToString(dRow[Constants.FACTORYJOBID]),
                                                CreatedOn = Convert.ToDateTime(dRow[Constant.CREATEDON])
                                            }).ToList();
                            unitList = unitList.Where(x => x.groupId.Equals(groupConfiguration.GroupId)).Distinct().ToList();
                            groupConfiguration.Units = new List<UnitsForConfiguration>();
                            foreach (var unit in unitList)
                            {
                                if (unit.unitId > 0)
                                {
                                    UnitsForConfiguration unitConfig = new UnitsForConfiguration()
                                    {
                                        UnitId = unit.unitId,
                                        UnitName = unit.unitName,
                                        Product = unit.productName,
                                        Capacity = unit.capacity,
                                        Speed = unit.speed,
                                        Landings = unit.landings,
                                        FrontOpenings = unit.frontOpening,
                                        RearOpening = unit.rearOpening,
                                        SetId = unit.setId,
                                        SetName = unit.SetName,
                                        Status = new Status()
                                        {
                                            StatusKey = unit.unitStatusKey,
                                            StatusName = unit.unitStatusName,
                                            DisplayName = unit.unitStatusDisplayName,
                                            Description = unit.unitStatusDescription
                                        },
                                        Description = unit.Description,
                                        Ueid = unit.UEID,
                                        Factory = new Factory()
                                        {
                                            FactoryJobId = unit.FactoryJobId,
                                        },
                                        CreatedOn = unit.CreatedOn,
                                    };
                                    groupConfiguration.Units.Add(unitConfig);

                                    unitConfig.Status = unitConfig.Status;

                                }
                            }
                            var unitsInGroup = new List<UnitsForConfiguration>();
                            var independentUnits = (from units in groupConfiguration.Units
                                                    orderby units.CreatedOn ascending
                                                    where units.SetId > 0
                                                    group units by units.SetId into q
                                                    where q.Count() == 1
                                                    select q).ToList();
                            var independentUnitsinGroup = new List<UnitsForConfiguration>();
                            independentUnitsinGroup = (from units in groupConfiguration.Units
                                                       from independentUnit in independentUnits
                                                       orderby units.CreatedOn ascending
                                                       where units.SetId == independentUnit.Key
                                                       select units).ToList();

                            unitsInGroup = (from units in groupConfiguration.Units
                                                //from independentUnit in independentUnits
                                            orderby units.CreatedOn ascending
                                            where units.SetId == 0
                                            select units).ToList();
                            independentUnitsinGroup.AddRange(unitsInGroup);
                            unitsInGroup = independentUnitsinGroup;
                            foreach (var independentUnit in unitsInGroup)
                            {
                                groupConfiguration.Units.RemoveAll(x => x.SetId == independentUnit.SetId);
                            }
                            groupConfiguration.Units = (from units in groupConfiguration.Units
                                                            //from independentUnit in independentUnits
                                                        orderby units.SetId, units.CreatedOn ascending
                                                        where units.SetId > 0
                                                        select units).ToList();

                            groupConfiguration.Units.AddRange(unitsInGroup);


                            configurationList.Groups.Add(groupConfiguration);
                        }
                    }
                    ListConfiguration.Configuration.Buildings.Add(configurationList);
                }
                var projectdata = (from DataRow dRow in dataSet.Tables[1].Rows
                                   select new
                                   {
                                       Opportunityid = Convert.ToString(dRow[Constant.OPPORTUNITYIDVARIABLE]),
                                       VersionId = Convert.ToString(dRow[Constant.VERSIONID])
                                   }).Distinct();
                foreach (var version in projectdata)
                {
                    var val = await _projectDl.CreateProjectsBL(sessionId, version.Opportunityid).ConfigureAwait(false);
                    var responseData = Utility.DeserializeObjectValue<CreateProjectResponseObject>(Utility.SerializeObjectValue(val.Response));
                    var variableValuesData = Utility.DeserializeObjectValue<VariableDetails>(Utility.SerializeObjectValue(responseData.VariableDetails));
                    var projectDisplayDetails = Utility.DeserializeObjectValue<ProjectDisplayDetails>(Utility.SerializeObjectValue(responseData.ProjectDisplayDetails));
                    ListConfiguration.ProjectDetails = new ProjectDetail()
                    {
                        Id = version.Opportunityid,
                        OpportunityName = variableValuesData?.ProjectName,
                        SalesStage = variableValuesData?.SalesStage,
                        VersionId = version.VersionId,
                        AccountAddress = new AccountEntity()
                        {
                            AccountName = variableValuesData?.AccountName,
                            AccountAddressCity = variableValuesData?.City,
                            AccountAddressState = variableValuesData?.State,
                            AccountAddressCountry = variableValuesData?.Country,
                            AccountAddressAddressZipCode = variableValuesData?.ZipCode,
                            AccountAddressStreetAddress = variableValuesData?.AddressLine1,
                            AccountAddressStreetAddress2 = variableValuesData?.AddressLine2
                        },
                        //BookingDate = projectDisplayDetails?.ContractBookedDate,
                        //ProposedDate = projectDisplayDetails?.ProposedDate,
                        //CreatedDate= projectDisplayDetails?.ContractBookedDate
                    };
                }
            }
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = JObject.FromObject(ListConfiguration) };
        }
        /// <summary>
        /// GetBuildingConflicts
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public List<string> GetBuildingConflicts(int buildingId)
        {
            var methodStartTime = Utility.LogBegin();
            List<string> lstConflictVariables = new List<string>();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
               new SqlParameter() { ParameterName = Constant.@_ID ,Value = buildingId},
               new SqlParameter() { ParameterName = Constant.@ENTITY ,Value="Building"},              
            };
            DataSet dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETBUILDINGCONFLICTS, sqlParameters);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dRow in dataSet.Tables[0].Rows)
                {
                    lstConflictVariables.Add(Convert.ToString(dRow[Constant.CONFLICTVARIABLEID]));
                }

            }
            Utility.LogEnd(methodStartTime);
            return lstConflictVariables;
        }

        /// <summary>
        /// GetBuildingVariablesWithUnitByGroupId
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="configVariables"></param>
        /// <returns></returns>
        public List<UnitVariables> GetBuildingVariables(int groupid, DataTable configVariables)
        {
            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> parameterList = new List<SqlParameter>();
            List<UnitVariables> buildingVariables = new List<UnitVariables>();
            DataSet dataSet = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID_LOWERCASE, groupid),
                new SqlParameter(Constant.CONSTANTMAPPERLIST ,configVariables),
                new SqlParameter(Constant.TYPE, Constants.BUILDING)
            };
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETBUILDINGGROUPUNITVARIABLEASSIGNMENTS, param);
            if (dataSet != null && dataSet.Tables.Count > 1 && dataSet.Tables[0].Rows.Count > 0)
            {
                buildingVariables = (from DataRow row in dataSet.Tables[0].Rows
                                    select new UnitVariables
                                    {
                                        groupConfigurationId = Convert.ToInt32(row[Constant.FDAGROUPCONFIGURATIONID]),
                                        name = Convert.ToString(row[Constant.FDAUNITNAME]),
                                        unitId = Convert.ToInt32(row[Constant.FDAUNITID]),
                                        MappedLocation = Convert.ToString(row[Constant.MAPPEDLOCATION]),
                                        VariableId = Convert.ToString(row[Constant.VARIABLEID]),
                                        Value = Convert.ToString(row[Constant.VALUE]),
                                    }).ToList();
               
            }
            Utility.LogEnd(methodBeginTime);
            return buildingVariables;
        }
    }
}