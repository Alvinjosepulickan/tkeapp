using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Database;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class BuildingEquipmentDL : IBuildingEquipmentDL
    {
        /// <summary>
        /// object ICacheManager
        /// </summary>
        private readonly ICacheManager _cpqCacheManager;
        /// <summary>
        /// string environment
        /// </summary>
        private readonly string _environment;

        /// <summary>
        /// Constructor for BuildingEquipmentDL
        /// </summary>
        /// <param Name="logger"></param>
        public BuildingEquipmentDL(ILogger<BuildingEquipmentDL> logger, ICacheManager cpqCacheManager)
        {
            Utility.SetLogger(logger);
            _cpqCacheManager = cpqCacheManager;
            _environment = Constant.DEV;
        }
        /// <summary>
        /// Data Access for GetBuildingEquipmentConfigurationByBuildingId
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        public List<ConfigVariable> GetBuildingEquipmentConfigurationByBuildingId(int buildingId, DataTable configVariables)
        {
            var methodStartTime = Utility.LogBegin();
            var buildingEquipmentContantsDictionary = Utility.VariableMapper(Constants.BUILDINGMAPPERVARIABLESMAPPERPATH, Constants.BUILDINGEQUIPMENTVARIABLES);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.BUILDINGID ,Value=buildingId }
               ,new SqlParameter() { ParameterName = Constant.BUILDINGEQUIPMENTVARIABLESLIST ,Value=configVariables }
            };
            List<ConfigVariable> listGroup = new List<ConfigVariable>();
            List<UnitNames> lstUnits = new List<UnitNames>();
            DataSet dataSet = new DataSet();
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETBUILDINGEQUIPMENTCONFIGURATIONBYID, sqlParameters);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    ConfigVariable configVariable = new ConfigVariable
                    {
                        VariableId = Convert.ToString(dataSet.Tables[0].Rows[i][Constant.VARIABLETYPE]),
                        Value = Convert.ToString(dataSet.Tables[0].Rows[i][Constant.VALUE])

                    };
                    listGroup.Add(configVariable);
                }
                if (dataSet.Tables[1].Rows.Count > 0)
                {
                    listGroup.AddRange(new List<ConfigVariable>
                    {
                    new ConfigVariable {VariableId = buildingEquipmentContantsDictionary[Constants.TOPLANDINGELEVATION], Value = (dataSet.Tables[1].Rows[0][Constants.TOPFLOORELEVATION]) },
                    new ConfigVariable {VariableId = buildingEquipmentContantsDictionary[Constants.BUTTOMLANDINGELEVATION], Value = (dataSet.Tables[1].Rows[0][Constants.BOTTOMFLOORELEVATION])},
                    new ConfigVariable {VariableId = buildingEquipmentContantsDictionary[Constants.MAINEGRESSELEVATION], Value = (dataSet.Tables[1].Rows[0][Constants.MAINEGRESSELEVATIONS]) }
                    });
                }
            }
            Utility.LogEnd(methodStartTime);
            return listGroup;
        }

        /// <summary>
        /// Data Access for GetBuildingEquipmentConsoles
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="username"></param>
        /// <returns></returns>
        public List<BuildingEquipmentData> GetBuildingEquipmentConsoles(int buildingId, string username, string sessionId)
        {
            var methodStartTime = Utility.LogBegin();
            List<BuildingEquipmentData> objBuildingEquipmentConfigurationData = new List<BuildingEquipmentData>();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.BUILDINGID, Value = buildingId },
                new SqlParameter() { ParameterName = Constant.@USERNAME, Value = username }
            };
            List<BuildingEquipmentData> listGroup = new List<BuildingEquipmentData>();
            List<UnitNames> lstUnits = new List<UnitNames>();
            List<ConfiguredGroups> lstConfiguredGroups = new List<ConfiguredGroups>();
            DataSet dataSet = new DataSet();
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETBUILDINGEQUIPMENTCONSOLECONFIGURATION, sqlParameters);
            if (dataSet != null && dataSet.Tables.Count > 0)
            {

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    List<BuildingEquipmentData> buildingEquipmentConsoles = new List<BuildingEquipmentData>();
                    var buildingEquipmentConsoleList = (from DataRow row in dataSet.Tables[0].Rows
                                                        select new
                                                        {
                                                            entranceConsoleId = Convert.ToInt32(row[Constant.CONSOLENUMBER]),
                                                            ConsoleNumber = Convert.ToInt32(row[Constant.CONSOLENUMBER]),
                                                            ConsoleName = row[Constant.CONSOLENAME].ToString(),
                                                            islobby = Convert.ToBoolean(row[Constant.ISLOBBY]),
                                                            isController = Convert.ToBoolean(row[Constant.ISCONTROLLER]),
                                                            AssignedGroups = Convert.ToInt32(row[Constant.ASSIGNEDGROUPS]),
                                                            AssignedUnits = Convert.ToInt32(row[Constant.ASSIGNEDUNITS]),
                                                        }).Distinct();


                    foreach (var console in buildingEquipmentConsoleList)
                    {

                        BuildingEquipmentData buildingEquipmentConsole = new BuildingEquipmentData()
                        {
                            ConsoleId = console.entranceConsoleId,
                            ConsoleNumber = console.ConsoleNumber,
                            ConsoleName = console.ConsoleName,
                            IsLobby = console.islobby,
                            IsController = console.isController,
                            AssignedGroups = console.AssignedGroups,
                            AssignedUnits = console.AssignedUnits,
                        };
                        var variableList = (from DataRow rows in dataSet.Tables[1].Rows
                                            select new
                                            {
                                                entranceConsoleId = Convert.ToInt32(rows[Constant.CONSOLENUMBER]),
                                                variableType = "",
                                                variablevalue = "",
                                            }).Distinct();

                        DataColumnCollection columns = dataSet.Tables[1].Columns;
                        if (columns.Contains(Constant.VARIABLETYPE) && columns.Contains(Constant.VALUE))
                        {
                            var variableListValues = (from DataRow rows in dataSet.Tables[1].Rows
                                                      select new
                                                      {
                                                          entranceConsoleId = Convert.ToInt32(rows[Constant.CONSOLENUMBER]),
                                                          variableType = rows[Constant.VARIABLETYPE] != DBNull.Value ? rows[Constant.VARIABLETYPE].ToString() : "",
                                                          variablevalue = rows[Constant.VALUE] != DBNull.Value ? rows[Constant.VALUE]?.ToString() : "",

                                                      }).Distinct();
                            variableList = variableListValues;
                        }

                        variableList = variableList?.Where(x => x.entranceConsoleId.Equals(buildingEquipmentConsole.ConsoleId)).Distinct().ToList();


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
                        buildingEquipmentConsole.VariableAssignments = variableAssignments;


                        buildingEquipmentConsole.lstConfiguredGroups = new List<ConfiguredGroups>();

                        if (dataSet.Tables[2].Rows.Count > 0)
                        {

                            var buildingEquipmentGroupList = (from DataRow row1 in dataSet.Tables[2].Rows
                                                              select new
                                                              {
                                                                  consoleId = Convert.ToInt32(row1[Constant.CONSOLENUMBER]),
                                                                  groupId = Convert.ToInt32(row1[Constant.GROUPIDLOWERCASE]),
                                                                  groupName = row1[Constant.GROUPNAMELOWERCASE].ToString(),
                                                                  is_Checked = Convert.ToBoolean(row1[Constant.IS_CHECKED]),
                                                                  totalUnits = Convert.ToInt32(row1[Constant.TOTALUNITS]),
                                                                  totalGroups = Convert.ToInt32(row1[Constant.TOTALGROUPS])

                                                              }).Distinct();

                            foreach (var consoles in buildingEquipmentGroupList)
                            {
                                if (consoles.consoleId == console.entranceConsoleId)
                                {
                                    var configuredUnits = new ConfiguredGroups()
                                    {
                                        consoleId = consoles.consoleId,
                                        groupId = consoles.groupId,
                                        groupName = consoles.groupName,
                                        totalGroups = consoles.totalGroups,
                                        noOfUnits = consoles.totalUnits,
                                        isChecked = consoles.is_Checked,
                                        InCompatible = true
                                    };
                                    buildingEquipmentConsole.lstConfiguredGroups.Add(configuredUnits);
                                }
                            }
                        }

                        buildingEquipmentConsoles.Add(buildingEquipmentConsole);
                    }

                    if (buildingEquipmentConsoles.Count > 0)
                    {

                        var varlstConfiguredGroups = buildingEquipmentConsoles[0].lstConfiguredGroups;

                        BuildingEquipmentData objEntranceConsole = new BuildingEquipmentData()
                        {
                            ConsoleId = 0,
                            VariableAssignments = new List<ConfigVariable>(),
                            ConsoleName = Constant.CONSOLENAMELOWERCASE,
                            IsController = false,
                            IsLobby = true,
                            AssignedGroups = 0,
                            AssignedUnits = 0,
                            lstConfiguredGroups = varlstConfiguredGroups,
                            lstExistingGroups = new List<BuildingEquipmentGroupDetails>(),
                            lstFutureGroup = new List<BuildingEquipmentGroupDetails>(),

                        };
                        buildingEquipmentConsoles.Add(objEntranceConsole);
                    }


                    objBuildingEquipmentConfigurationData = buildingEquipmentConsoles;

                }
                foreach (var console in objBuildingEquipmentConfigurationData)
                {
                    if (dataSet.Tables[3].Rows.Count > 0)
                    {
                        var buildingEquipmentConsoleList = (from DataRow row in dataSet.Tables[3].Rows
                                                            select new
                                                            {
                                                                entranceConsoleId = Convert.ToInt32(row[Constant.CONSOLENUMBER]),
                                                                groupCategoryId = Convert.ToInt32(row[Constant.GROUPCATEGORYID]),
                                                                groupCategoryName = row[Constant.GROUPCATEGORYNAME].ToString(),
                                                                groupName = row[Constant.GROUPNAME].ToString(),
                                                                noOfUnits = Convert.ToInt32(row[Constant.NOOFUNITS]),

                                                            }).Distinct();

                        console.lstExistingGroups = new List<BuildingEquipmentGroupDetails>();
                        console.lstFutureGroup = new List<BuildingEquipmentGroupDetails>();

                        foreach (var consoles in buildingEquipmentConsoleList)
                        {

                            if (console.ConsoleId == consoles.entranceConsoleId)
                            {
                                if (consoles.groupCategoryId.Equals(1))
                                {
                                    var listExistingGroup = new BuildingEquipmentGroupDetails()
                                    {
                                        consoleId = console.ConsoleId,
                                        groupName = consoles.groupName,
                                        noOfUnits = consoles.noOfUnits
                                    };
                                    console.lstExistingGroups.Add(listExistingGroup);
                                }
                                else
                                {
                                    var listFutureGroup = new BuildingEquipmentGroupDetails()
                                    {
                                        consoleId = console.ConsoleId,
                                        groupName = consoles.groupName,
                                        noOfUnits = consoles.noOfUnits
                                    };
                                    console.lstFutureGroup.Add(listFutureGroup);
                                }

                            }

                        }

                    }
                }
                if(dataSet.Tables[4].Rows.Count > 0)
                {
                    var lobbyRecallSwitchData = (from DataRow row in dataSet.Tables[4].Rows
                                                 select new
                                                 {
                                                     groupId = Convert.ToInt32(row[Constant.GROUPID]),
                                                     lobbyRecallSwitch = String.IsNullOrEmpty(Convert.ToString(row[Constant.VARIABLEVALUE]))?false: Convert.ToString(row[Constant.VARIABLEVALUE]).Equals("TRUE")? true:false
                                                 }).Distinct();
                    _cpqCacheManager.SetCache(sessionId, _environment, buildingId.ToString(), Constant.LOBBYRECALLSWITCHVARAIBLES, Utility.SerializeObjectValue(lobbyRecallSwitchData));
                }
            }
            foreach (var console in objBuildingEquipmentConfigurationData)
            {
                console.ConsoleId = console.ConsoleNumber;
            }
            Utility.LogEnd(methodStartTime);
            return objBuildingEquipmentConfigurationData;
        }

        /// <summary>
        /// Save Assign Groups
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="consoleId"></param>
        /// <param Name="buildingEquipmentConfigurationData"></param>
        /// <param Name="userId"></param>
        /// <param Name="is_Saved"></param>
        /// <param Name="historyTable"></param>
        /// <returns></returns>
        public List<Result> SaveAssignGroups(int buildingId, int consoleId, BuildingEquipmentData buildingEquipmentConfigurationData, string userId, int is_Saved, List<LogHistoryTable> historyTable)
        {
            var methodStartTime = Utility.LogBegin();
            Result result = new Result();
            List<Result> lstResult = new List<Result>();
            DataTable assignedGroupsDataTable = new DataTable();
            DataTable existingGroupsDataTable = new DataTable(); ;
            DataTable futureGroupsDataTable = new DataTable(); ;
            DataTable logHistoryTable = new DataTable();

            DataTable consoleDataTable = Utility.GenerateDataTableForAssignGroups(buildingEquipmentConfigurationData);
            DataTable consoleConfigurationDataTable = Utility.GenerateDataTableForAssignGroupsConfiguration(buildingEquipmentConfigurationData.VariableAssignments, Convert.ToInt32(buildingEquipmentConfigurationData.ConsoleId));

            assignedGroupsDataTable = Utility.GenerateDataTableForAssignedGroups(buildingEquipmentConfigurationData.ConfiguredGroups, Convert.ToInt32(buildingEquipmentConfigurationData.ConsoleId));
            existingGroupsDataTable = Utility.GenerateDataTableForExistingGroups(buildingEquipmentConfigurationData.ExistingGroups, Convert.ToInt32(buildingEquipmentConfigurationData.ConsoleId));
            futureGroupsDataTable = Utility.GenerateDataTableForFutureGroups(buildingEquipmentConfigurationData.FutureGroups, Convert.ToInt32(buildingEquipmentConfigurationData.ConsoleId));

            logHistoryTable = Utility.GenerateDataTableForHistoryTable(historyTable);

            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSaveAssignedGroupsConfiguration(buildingId, Convert.ToInt32(buildingEquipmentConfigurationData.ConsoleId), consoleDataTable, consoleConfigurationDataTable, assignedGroupsDataTable, existingGroupsDataTable, futureGroupsDataTable, userId, logHistoryTable);
            int resultForSaveUnitConfiguration = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVEASSIGNEDGROUPS, lstSqlParameter);

            if (resultForSaveUnitConfiguration > 0 && is_Saved.Equals(0))
            {
                result.result = 1;
                result.buildingId = resultForSaveUnitConfiguration;
                if (buildingEquipmentConfigurationData.ConsoleName.Contains(Constant.LOBBYPANEL))
                {
                    result.message = Constant.SAVEBUILDINGEQUIPMENTCONSOLESUCCESSMSG;
                }
                else
                {
                    result.message = Constant.SAVEBUILDINGEQUIPMENTSUCCESSMSG;
                }
            }
            else if (resultForSaveUnitConfiguration > 0 && is_Saved.Equals(1))
            {
                result.result = 1;
                result.buildingId = resultForSaveUnitConfiguration;
                result.message = Constant.UPDATEBUILDINGEQUIPMENTCONSOLESUCCESSMSG;
            }
            else if (resultForSaveUnitConfiguration == 0)
            {
                result.result = 0;
                result.buildingId = resultForSaveUnitConfiguration;
                result.message = Constant.SAVEBUILDINGEQUIPMENTCONSOLEERRORMSG;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodStartTime);
            return lstResult;
        }

        /// <summary>
        /// Duplicate Building Equipment Console
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="consoleId"></param>
        /// <param Name="userId"></param>
        public List<Result> DuplicateBuildingEquipmentConsole(int buildingId, int consoleId, string userId)
        {
            var methodStartTime = Utility.LogBegin();
            Result result = new Result();
            List<Result> lstResult = new List<Result>();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.BUILDINGID, Value = buildingId},
                new SqlParameter() { ParameterName = Constant.CONSOLEID, Value = consoleId},
                new SqlParameter() { ParameterName = Constant.USERID, Value = userId},
                new SqlParameter() { ParameterName = Constant.RESULT, Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int }
            };
            int Result = CpqDatabaseManager.ExecuteNonquery(Constant.DUPLICATEBUILDINGEQUIPMENTCONSOLE, sqlParameters);
            if (Result > 0)
            {
                result.result = 1;
                result.buildingId = buildingId;
                result.message = Constant.DUPLICATEBUILDINGEQUIPMENTSUCCESSMSG;
            }
            else if (Result == 0)
            {
                result.result = 0;
                result.buildingId = buildingId;
                result.message = Constant.DUPLICATEBUILDINGEQUIPMENTCONSOLEERRORMSG;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodStartTime);
            return lstResult;

        }

        /// <summary>
        /// Duplicate Building Equipment Console
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="consoleId"></param>
        /// <param Name="userId"></param>
        /// // <param Name="logHistory"></param>
        public List<Result> DeleteBuildingEquipmentConsole(int buildingId, int consoleId, string userId, List<LogHistoryTable> logHistory)
        {
            var methodStartTime = Utility.LogBegin();
            Result result = new Result();
            List<Result> lstResult = new List<Result>();
            DataTable historyTable = Utility.GenerateDataTableForHistoryTable(logHistory);
            List<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.BUILDINGID, Value = buildingId},
                new SqlParameter() { ParameterName = Constant.CONSOLEID, Value = consoleId},
                new SqlParameter() { ParameterName = Constant.USERID, Value = userId},
                new SqlParameter() { ParameterName = @"historyTable", Value = historyTable},
                new SqlParameter() { ParameterName = Constant.RESULT, Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int }
            };
            int Result = CpqDatabaseManager.ExecuteNonquery(Constant.DELETEBUILDINGEQUIPMENTCONSOLE, sqlParameters);
            if (Result > 0)
            {
                result.result = 1;
                result.buildingId = buildingId;
                result.message = Constant.DELETEBUILDINGEQUIPMENTSUCCESSMSG;
            }
            else if (Result == 0)
            {
                result.result = 0;
                result.buildingId = buildingId;
                result.message = Constant.DELETEBUILDINGEQUIPMENTCONSOLEERRORMSG;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodStartTime);
            return lstResult;

        }

        /// <summary>
        /// Save Building Equipment Configuration
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="buildingEquipmentConfigurationData"></param>
        /// <param Name="userId"></param>
        /// // <param Name="is_Saved"></param>
        public List<Result> SaveBuildingEquipmentConfiguration(int buildingId, List<ConfigVariable> buildingEquipmentConfigurationData, string userId, int is_Saved)
        {
            var methodStartTime = Utility.LogBegin();
            Result result = new Result();
            List<Result> lstResult = new List<Result>();

            DataTable configurationDataTable = Utility.GenerateDataTableForUnitConfiguration(buildingEquipmentConfigurationData);

            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSaveBuildingEquipmentConfiguration(buildingId, configurationDataTable, userId);
            int resultForSaveUnitConfiguration = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVEBUILDINGEQUIPMENTCONFIGURATION, lstSqlParameter);

            if (resultForSaveUnitConfiguration > 0 && is_Saved.Equals(0))
            {
                result.result = 1;
                result.buildingId = resultForSaveUnitConfiguration;
                result.message = Constant.SAVEBUILDINGEQUIPMENTSUCCESSMSG;
            }
            else if (resultForSaveUnitConfiguration > 0 && is_Saved.Equals(1))
            {
                result.result = 1;
                result.buildingId = resultForSaveUnitConfiguration;
                result.message = Constant.UPDATEBUILDINGEQUIPMENTSUCCESSMSG;
            }
            else if (resultForSaveUnitConfiguration == 0)
            {
                result.result = 0;
                result.buildingId = resultForSaveUnitConfiguration;
                result.message = Constant.SAVEBUILDINGEQUIPMENTERRORMSG;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodStartTime);
            return lstResult;
        }
    }
}
