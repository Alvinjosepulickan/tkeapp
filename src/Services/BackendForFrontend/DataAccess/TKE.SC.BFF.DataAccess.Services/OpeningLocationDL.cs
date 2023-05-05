/************************************************************************************************************
    File Name     :   OpeningLocationDL
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
********************************************************************************************/
using Configit.Configurator.Server.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.Common.Database;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class OpeningLocationDL : IOpeningLocationDL
    {
        private readonly IConfiguration _configuration;
        private readonly string _environment;
        private readonly ICacheManager _cpqCacheManager;
        /// <summary>
        /// Constructor For OpeningLocationDL
        /// </summary>
        /// <param Name="logger"></param>
        public OpeningLocationDL(ILogger<OpeningLocationDL> logger, ICacheManager cpqCacheManager, IConfiguration iConfig)
        {
            Utility.SetLogger(logger);
            _configuration = iConfig;
            _environment = Constant.DEV;
            _cpqCacheManager = cpqCacheManager;
        }

        /// <summary>
        /// This method is used to update opening location
        /// </summary>
        /// <param Name="openingLocation"></param>
        /// <param Name="changeLogForOpenings"></param>
        /// <returns></returns>
        public int UpdateOpeningLocation(OpeningLocations openingLocation, List<LogHistoryTable> changeLogForOpenings)
        {
            var methodBeginTime = Utility.LogBegin();
            var openingLocationDataTable = Utility.CreateDataTableForUpdateOpeningLocation(openingLocation);
            DataTable historyTable = Utility.GenerateDataTableForHistoryTable(changeLogForOpenings);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForUpdateOpeningLocation(openingLocation, openingLocationDataTable, historyTable);
            int resultForUpdateOpeningLocation = CpqDatabaseManager.ExecuteNonquery(Constant.SPUPDATEOPENINGLOCATION, lstSqlParameter);
            Utility.LogEnd(methodBeginTime);
            return resultForUpdateOpeningLocation;
        }

        /// <summary>
        /// This method is to get opening location
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="val"></param>
        /// <returns></returns>
        public OpeningLocations GetOpeningLocationBygroupId(int GroupConfigurationId, List<VariableAssignment> val, List<ConfigVariable> mapperVariables, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var groupContantsDictionary = Utility.VariableMapper(Constant.GROUPMAPPERVARIABLESMAPPERPATH, Constant.GROUPMAPPERCONFIGURATION);
            DataTable variableMapperAssignment = Utility.GenerateVariableMapperDataTable(mapperVariables);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@GROUPCONFIGRATIONID, Value = GroupConfigurationId},
                new SqlParameter()  { ParameterName = Constant.VARIABLEMAPPERDATATABLE,Value = variableMapperAssignment,Direction = ParameterDirection.Input }
            };
            OpeningLocations openinglocation = new OpeningLocations();
            DataSet dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETOPENINGLOCATIONBYID, sqlParameters);
            if (dataSet != null && dataSet.Tables.Count >= 1)
            {
                var openinglocationlist = (from DataRow row in dataSet.Tables[0].Rows
                                           select new { groupConfigurationId = Convert.ToInt32(row[Constant.GRPID]), buildingRise = Convert.ToDouble(row[Constant.BuildingRise]), userName = row[Constant.USERNAMEVARIABLE].ToString(), noOfFloors = Convert.ToInt32(row[Constant.FLOOR]) }).Distinct();
                foreach (var location in openinglocationlist)
                {
                    openinglocation.GroupConfigurationId = location.groupConfigurationId;
                    openinglocation.BuildingRise = location.buildingRise;
                    openinglocation.UserName = location.userName;
                    openinglocation.NoOfFloors = location.noOfFloors;
                }
                var unitList = (from DataRow row in dataSet.Tables[0].Rows
                                select new
                                {
                                    unitId = Convert.ToInt32(row[Constant.UNITIDLOWERCASE])
                                    ,
                                    UnitName = row[Constant.NAME].ToString()
                                    ,
                                    TravelFeet = Convert.ToInt32(row[Constant.TRAVEL_FEET])
                                    ,
                                    TravelInch = Convert.ToDecimal(row[Constant.TRAVEL_INCH])
                                    ,
                                    NoOfFloors = Convert.ToInt32(row[Constant.NOOFFLOORS])
                                    ,
                                    OccupiedSpaceBelow = Convert.ToBoolean(row[Constant.OCCUPIEDSPACEBELOW])
                                    ,
                                    FrontOpening = Convert.ToInt32(row[Constant.FRONTOPENING])
                                    ,
                                    RearOpening = Convert.ToInt32(row[Constant.REAROPENING])
                                    ,
                                    SideOpening = Convert.ToInt32(row[Constant.SIDEOPENING])
                                }).ToList().Distinct();
                openinglocation.Units = new List<UnitData>();
                foreach (var units in unitList)
                {
                    var unit = new UnitData
                    {
                        UnitId = units.unitId,
                        UnitName = units.UnitName,
                        Travel = new TypicalFloor
                        {
                            feet = units.TravelFeet,
                            inch = units.TravelInch
                        },
                        OcuppiedSpace = units.OccupiedSpaceBelow,
                        NoOfFloors = units.NoOfFloors,
                        FrontOpening = units.FrontOpening,
                        RearOpening = units.RearOpening,
                        SideOpening = units.SideOpening
                    };
                    var doorList = (from DataRow row in dataSet.Tables[0].Rows
                                    select new
                                    {
                                        unitId = Convert.ToInt32(row[Constant.UNITIDLOWERCASE]),
                                        DoorType = row[Constant.DOORTYPE].ToString(),
                                        DoorValue = row[Constant.DOORVALUE].ToString()
                                    }).ToList().Distinct();
                    var doors = (from door in doorList
                                 where door.unitId == units.unitId
                                 select new
                                 {
                                     UnitId = door.unitId,
                                     DoorType = door.DoorType,
                                     DoorValue = door.DoorValue
                                 });
                    unit.OpeningDoors = new OpeningDoors();
                    foreach (var door in doors)
                    {
                        if (!string.IsNullOrEmpty(door.DoorValue))
                        {
                            if (door.DoorType.Contains(groupContantsDictionary[Constant.FRONTDOORTYPEANDHAND]))
                            {
                                unit.OpeningDoors.Front = true;
                            }
                            if (door.DoorType.Contains(groupContantsDictionary[Constant.REARDOORTYPEANDHAND]) && !Utility.CheckEquals(door.DoorValue, Constant.NR))
                            {
                                unit.OpeningDoors.Rear = true;
                            }
                            if (door.DoorType.Contains(groupContantsDictionary[Constant.SIDEDOORTYPEANDHAND]))
                            {
                                unit.OpeningDoors.Side = true;
                            }
                        }
                            
                    }
                    unit.OpeningsAssigned = new List<OpeningsAssigned>();
                    var openingdetails = (from DataRow row in dataSet.Tables[0].Rows
                                          where unit.UnitId == Convert.ToInt32(row[Constant.UNITIDLOWERCASE])
                                          select new
                                          {
                                              unitId = Convert.ToInt32(row[Constant.UNITIDLOWERCASE]),
                                              FloorDesignation = row[Constant.FLOORDESIGNATION].ToString(),
                                              ElevationFeet = Convert.ToInt32(row[Constant.ELEVAIONFEET]),
                                              ElevationInch = Convert.ToDecimal(row[Constant.ELEVAIONINCH]),
                                              Front = Convert.ToBoolean(row[Constant.FRONTDOOR]),
                                              Rear = Convert.ToBoolean(row[Constant.REARDOOR]),
                                              Side = Convert.ToBoolean(row[Constant.SIDEDOOR]),
                                              MainEgress = Convert.ToBoolean(row[Constant.MAINEGRESS]),
                                              AlternateEgress = Convert.ToBoolean(row[Constant.ALTERNATEEGRESS])

                                          }).ToList().Distinct();
                    int floorNumber = 0;
                    foreach (var openings in openingdetails)
                    {
                        floorNumber += 1;
                        var opening = new OpeningsAssigned
                        {
                            FloorDesignation = openings.FloorDesignation,
                            FloorNumber = floorNumber,
                            Elevation = new TypicalFloor
                            {
                                feet = openings.ElevationFeet,
                                inch = openings.ElevationInch
                            },
                            Front = openings.Front,
                            Rear = openings.Rear,
                            Side = openings.Side,
                            MainEgress = openings.MainEgress,
                            AlternateEgress = openings.AlternateEgress                            
                        };
                        if (opening.MainEgress)
                        {
                            opening.Front = true;
                        }
                        unit.OpeningsAssigned.Add(opening);
                    }
                    openinglocation.Units.Add(unit);
                }
                var displayvariableAssignment = (from DataRow row in dataSet.Tables[1].Rows
                                                 select new
                                                 {
                                                     variableAssignmentString = row[Constant.MAPPEDLOCATIONJSON].ToString()
                                                 }).ToList().Distinct();
                foreach (var variableAssignment in displayvariableAssignment)
                {
                    if (!string.IsNullOrEmpty(variableAssignment.variableAssignmentString))
                    {
                        openinglocation.DisplayVariableAssignments = Utility.DeserializeObjectValue<List<DisplayVariableAssignmentsValues>>(variableAssignment.variableAssignmentString);
                    }
                }
                openinglocation.Units.OrderBy(x => x.UnitId);
                var doorDetails = val.Where(oh => oh.VariableId.Contains(Constant.DOOR)).Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList();
                doorDetails.OrderBy(x => x.VariableId);
                for (int indexInOpeningLocationObject = 0; indexInOpeningLocationObject < openinglocation.Units.Count; indexInOpeningLocationObject++)
                {
                    foreach (var door in doorDetails)
                    {
                        if (door.VariableId.Contains(Convert.ToString(indexInOpeningLocationObject + 1)))
                        {
                            if (door.VariableId.Contains(Constant.FRONT.ToLower()))
                            {
                                openinglocation.Units[indexInOpeningLocationObject].OpeningDoors.Front = true;
                            }
                            if (door.VariableId.Contains(Constant.REAR.ToLower()) && !Utility.CheckEquals(door.Value.ToString(), Constant.NR))
                            {
                                openinglocation.Units[indexInOpeningLocationObject].OpeningDoors.Rear = true;
                            }
                        }
                    }
                }
                if (dataSet != null && dataSet.Tables.Count > 2)
                {
                    if (dataSet.Tables[2] != null && dataSet.Tables[2].Rows.Count > 0)
                    {
                        var conflictVaribales = new List<string>();
                        foreach (DataRow item in dataSet.Tables[2].Rows)
                        {
                            conflictVaribales.Add(item[Constants.CONFLICTVARIABLEIDDATA].ToString());
                        }
                        openinglocation.VariableIds = conflictVaribales != null && conflictVaribales.Any() ? conflictVaribales : new List<string>();
                    }
                    if(dataSet.Tables[3] != null && dataSet.Tables[3].Rows.Count > 0)
                    {
                        var UHFExists = (from DataRow dRow in dataSet.Tables[3].Rows
                                        select Convert.ToInt32(dRow["UnitHallFixturesExist"])).FirstOrDefault() >0 ? true : false;
                        _cpqCacheManager.SetCache(sessionId, _environment, GroupConfigurationId.ToString(), Constants.UHFEXISTSFLAG, Utility.SerializeObjectValue(UHFExists));

                    }
                    if (dataSet.Tables[4] != null && dataSet.Tables[4].Rows.Count > 0)
                    {
                        var saveOpeningLocation = (from DataRow dRow in dataSet.Tables[4].Rows
                                         select Convert.ToInt32(dRow["saveOpeningLocation"])).FirstOrDefault() > 0 ? true : false;
                        _cpqCacheManager.SetCache(sessionId, _environment, GroupConfigurationId.ToString(), Constants.SAVEOPENINGLOCATIONSFLAG, Utility.SerializeObjectValue(saveOpeningLocation));

                    }
                }
            }
            openinglocation.Units.OrderBy(x => x.UnitId);
            Utility.LogTrace(Utility.SerializeObjectValue(openinglocation));
            Utility.LogEnd(methodBeginTime);
            return openinglocation;
        }

        /// <summary>
        /// This method is used to get the permision by Rolename
        /// </summary>
        /// <param Name="Id"></param>
        /// <param Name="roleName"></param>
        /// <returns></returns>
        public List<string> GetPermissionByRole(int id, string roleName)
        {
            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@_ID, Value = id},
                new SqlParameter() { ParameterName = Constant.@ROLENAME, Value = roleName},
                new SqlParameter() { ParameterName = Constant.@ENTITY, Value = "Group"},
            };
            List<string> permission = new List<string>();
            DataSet ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETPERMISSIONBYROLENAME, sqlParameters);
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
        /// UpdateGroupConflictStatus
        /// </summary>
        /// <param name="configurationId"></param>
        /// <param name="conflictVariables"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public Result UpdateGroupConflictStatus(int configurationId, bool conflictStatusFlag)
        {
            var methodStartTime = Utility.LogBegin();
            Result result = new Result();
            var resultBuildingConfiguration = new Result();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
               new SqlParameter() { ParameterName = Constants.GROUPINGID_LOWERCASE ,Value = configurationId},
               new SqlParameter() { ParameterName = Constants.CONFLICTSTATUSFLAG ,Value=conflictStatusFlag},
               new SqlParameter() { ParameterName = Constants.RESULT_LOWCASE,Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int},
            };
            int results = CpqDatabaseManager.ExecuteNonquery(Constants.UPDATEGROUPCONFLICTSTATUS, sqlParameters);
            result.message = "Success";
            result.result = results;
            Utility.LogEnd(methodStartTime);
            return result;
        }
    }
}
