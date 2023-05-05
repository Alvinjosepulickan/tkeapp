/************************************************************************************************************
************************************************************************************************************
    File Name     :   GroupLayoutDL class 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Database;
using Microsoft.Extensions.Logging;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using System.IO;
using Newtonsoft.Json.Linq;
using TKE.SC.Common;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class GroupLayoutDL : IGroupLayoutDL
    {
        /// <summary>
        /// constructor method for Group Layout
        /// </summary>
        /// <param Name="logger"></param>
        public GroupLayoutDL(ILogger<GroupLayoutDL> logger)
        {
            Utility.SetLogger(logger);
        }

        /// <summary>
        /// Method for save group layout
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="unitVariableAssignment"></param>
        /// <param Name="unitVariableAssignmentForHallRiser"></param>
        /// <param Name="unitVariableAssignmentForDoor"></param>
        /// <param Name="userName"></param>
        /// <param Name="unitMappingValues"></param>
        /// <param Name="unitVariableAssignmentForControlLocation"></param>
        /// <param Name="displayVariableAssignments"></param>
        /// <returns></returns>
        public List<ResultGroupConfiguration> SaveGroupLayout(int groupId, List<ConfigVariable> unitVariableAssignment, List<ConfigVariable> unitVariableAssignmentForHallRiser, List<ConfigVariable> unitVariableAssignmentForDoor, string userName, List<UnitMappingValues> unitMappingValues, List<ConfigVariable> unitVariableAssignmentForControlLocation, List<DisplayVariableAssignmentsValues> displayVariableAssignments)
        {
            var methodBeginTime = Utility.LogBegin();
            ResultGroupConfiguration result = new ResultGroupConfiguration();
            List<ResultGroupConfiguration> lstResult = new List<ResultGroupConfiguration>();
            unitVariableAssignment = (from unit in unitVariableAssignment
                                      orderby unit.VariableId
                                      select unit).ToList();
            List<ConfigVariable> fdaData = (from unit in unitVariableAssignment
                                            where Utility.CheckEquals(unit.VariableId, Constant.ACROSSTHEHALLDISTANCE)
                                            || Utility.CheckEquals(unit.VariableId, Constant.BANKOFFSET)
                                            select unit).ToList();
            var fdaJsonData = JsonConvert.SerializeObject(fdaData);
            unitVariableAssignment = unitVariableAssignment.Except(unitVariableAssignment.Where(unit => unit.VariableId == Constant.ACROSSTHEHALLDISTANCE
                                    || unit.VariableId == Constant.BANKOFFSET)).ToList();
            DataTable unitDataTable = Utility.GenerateDataTableForUnitTable(unitVariableAssignment, displayVariableAssignments);
            DataTable unitDataTableForHallRiser = Utility.GenerateDataTableForSaveGroupLayout(unitVariableAssignmentForHallRiser, unitMappingValues);
            DataTable unitDataTableForDoor = Utility.GenerateDataTableForSaveGroupLayout(unitVariableAssignmentForDoor, unitMappingValues);
            DataTable unitDataTableForControlLocation = Utility.GenerateDataTableForSaveGroupLayout(unitVariableAssignmentForControlLocation, new List<UnitMappingValues>());
            ConflictsStatus isEditFlow = ConflictsStatus.Valid;
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSavingGroupLayout(groupId, unitDataTable, unitDataTableForHallRiser, unitDataTableForDoor
                                , userName, unitDataTableForControlLocation, isEditFlow, fdaJsonData, Constants.SAVEGROUPLAYOUT);
            var resultForSaveGroupLayout = CpqDatabaseManager.ExecuteDataSet(Constant.SPSAVEGROUPLAYOUT, lstSqlParameter);
            if (resultForSaveGroupLayout.Tables.Count == 1)
            {
                result.Result = 1;
                result.GroupConfigurationId = groupId;
                result.Message = Constant.GROUPLAYOUTSAVEMESSAGE;
            }
            else if (resultForSaveGroupLayout.Tables.Count > 1)
            {
                var resultValue = (from DataRow row in resultForSaveGroupLayout.Tables[0].Rows
                                   select new
                                   {
                                       returnValue = Convert.ToInt32(row[Constant.RETURNVALUE])
                                   }).FirstOrDefault();
                if (resultValue.returnValue.Equals(-2))
                {
                    result.Result = -2;
                    result.GroupConfigurationId = groupId;
                    result.Message = Constant.GROUPLAYOUTCONTROLROOMERRORMESSAGE;
                }
                else
                {
                    result.Result = 0;
                    result.GroupConfigurationId = groupId;
                    result.Message = Constant.ERRORSAVEMESSAGE;
                }
            }
            else
            {
                result.Result = -1;
                result.GroupConfigurationId = groupId;
                result.Message = Constant.ERRORSAVEMESSAGE;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBeginTime);
            return lstResult;
        }

        /// <summary>
        /// Method for Update Group Layout
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="unitVariableAssignment"></param>
        /// <param Name="unitVariableAssignmentForHallRiser"></param>
        /// <param Name="unitVariableAssignmentForDoor"></param>
        /// <param Name="userName"></param>
        /// <param Name="unitMappingValues"></param>
        /// <param Name="unitVariableAssignmentForControlLocation"></param>
        /// <param Name="displayVariableAssignments"></param>
        /// <param Name="isEditFlow"></param>
        /// <returns></returns>
        public List<ResultGroupConfiguration> UpdateGroupLayout(int groupId, string sectionTab, List<ConfigVariable> unitVariableAssignment, List<ConfigVariable> unitVariableAssignmentForHallRiser, List<ConfigVariable> unitVariableAssignmentForDoor, string userName, List<UnitMappingValues> unitMappingValues, List<ConfigVariable> unitVariableAssignmentForControlLocation, List<DisplayVariableAssignmentsValues> displayVariableAssignments, ConflictsStatus isEditFlow)
        {
            var methodBeginTime = Utility.LogBegin();
            ResultGroupConfiguration result = new ResultGroupConfiguration();
            List<ResultGroupConfiguration> lstResult = new List<ResultGroupConfiguration>();
            var floorPlanDistanceVariables = (JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES))[Constant.FLOORPLANDISTANCEPARAMETERS]).ToList();
            var groupConstantsDictionary = Utility.VariableMapper(Constant.GROUPMAPPERVARIABLES, Constant.GROUPMAPPERCONFIGURATION);
            List<ConfigVariable> fdaData = (from unit in unitVariableAssignment
                                            where floorPlanDistanceVariables.Contains(unit.VariableId)
                                            select unit).ToList();
            var fdaJsonData = JsonConvert.SerializeObject(fdaData);
            unitVariableAssignment = unitVariableAssignment.Except(unitVariableAssignment.Where(unit => floorPlanDistanceVariables.Contains(unit.VariableId))).ToList();
            DataTable unitDataTable = Utility.GenerateDataTableForUnitTable(unitVariableAssignment, displayVariableAssignments);
            DataTable unitDataTableForHallRiser = Utility.GenerateDataTableForSaveGroupLayout(unitVariableAssignmentForHallRiser, unitMappingValues);
            DataTable unitDataTableForDoor = Utility.GenerateDataTableForSaveGroupLayout(unitVariableAssignmentForDoor, unitMappingValues);
            DataTable unitDataTableForControlLocation = Utility.GenerateDataTableForSaveGroupLayout(unitVariableAssignmentForControlLocation, new List<UnitMappingValues>());
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSavingGroupLayout(groupId, unitDataTable, unitDataTableForHallRiser, unitDataTableForDoor, userName, unitDataTableForControlLocation, isEditFlow, fdaJsonData, sectionTab);
            
            var resultForSaveGroupLayout = CpqDatabaseManager.ExecuteDataSet(Constant.SPSAVEGROUPLAYOUT, lstSqlParameter);
            if (resultForSaveGroupLayout.Tables.Count == 1)
            {
                result.Result = 1;
                result.GroupConfigurationId = groupId;
                result.Message = Constant.GROUPLAYOUTUPDATEMESSAGE;
            }
            else if (resultForSaveGroupLayout.Tables.Count > 1)
            {
                if(resultForSaveGroupLayout.Tables[0].Columns.Contains(Constant.RETURNVALUE))
                { 
                var resultValue = (from DataRow row in resultForSaveGroupLayout.Tables[0].Rows
                                  select new
                                  {
                                      returnValue = Convert.ToInt32(row[Constant.RETURNVALUE])
                                  }).FirstOrDefault();
                
                    result.Result = -2;
                    result.GroupConfigurationId = groupId;
                    result.Message = Constant.GROUPLAYOUTCONTROLROOMERRORMESSAGE;
                }
                else
                {
                    result.Result = 0;
                    result.GroupConfigurationId = groupId;
                    result.Message = Constant.UNITNAMEREPEATING;
                    string unitNames = string.Empty;
                    result.IsDuplicateNameError = true;
                    result.CarPositionsWithDuplicateNames = new List<string>();
                    var carPositionWithDuplicateName = (from DataRow dRow in resultForSaveGroupLayout.Tables[1].Rows
                                                        select new { location = Convert.ToString(dRow[Constant.LOCATION]) }
                                                   );
                    foreach (var duplicatePosition in carPositionWithDuplicateName)
                    {
                        result.CarPositionsWithDuplicateNames.Add(Constant.PARAMETERS + duplicatePosition.location);
                    }
                    result.CarPositionsWithDuplicateNames.Distinct();
                    foreach (DataRow row in resultForSaveGroupLayout.Tables[1].Rows)
                    {
                        unitNames += unitNames.Equals(string.Empty) ? Convert.ToString(row[Constant.UNITDESIGNATION]) : Constant.COMA + Convert.ToString(row[Constant.UNITDESIGNATION]);
                    }
                    result.Message = result.Message.Replace(Constant.EQUALTO, unitNames);
                }
            }
            else
            {
                result.Result = -1;
                result.GroupConfigurationId = groupId;
                result.Message = Constant.ERRORSAVEMESSAGE;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBeginTime);
            return lstResult;
        }

        /// <summary>
        /// This function is used for duplicating or overwriting units
        /// </summary>
        /// <param Name="unitIDDataTable"></param>
        /// <param Name="groupID"></param>
        /// <param Name="carPositionDataTable"></param>
        /// <returns></returns>
        public DataSet DuplicateGroupLayoutById(DataTable unitIDDataTable, int groupID, DataTable carPositionDataTable)
        {
            var methodBeginTime = Utility.LogBegin();
            List<Result> lstResult = new List<Result>();
            IList<SqlParameter> sqlParameters;
            sqlParameters = new List<SqlParameter>()
                {
                    new SqlParameter() { ParameterName = Constant.UNITIDLIST,Value=unitIDDataTable,Direction = ParameterDirection.Input },
                    new SqlParameter() { ParameterName = Constant.GROUPINGID,Value=groupID,Direction = ParameterDirection.Input},
                    new SqlParameter() { ParameterName = Constant.CARPOSITIONLIST,Value=carPositionDataTable,Direction = ParameterDirection.Input},
                    new SqlParameter() { ParameterName = Constant.@RESULT,Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int},
                };
            Utility.LogEnd(methodBeginTime);
            return CpqDatabaseManager.ExecuteDataSet(Constant.SPDUPLICATEUNIT, sqlParameters);
        }

        /// <summary>
        /// Method for Update Group Layout
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="unitVariableAssignment"></param>
        /// <param Name="unitVariableAssignmentForHallRiser"></param>
        /// <param Name="unitVariableAssignmentForDoor"></param>
        /// <param Name="userName"></param>
        /// <param Name="unitMappingValues"></param>
        /// <param Name="unitVariableAssignmentForControlLocation"></param>
        /// <param Name="displayVariableAssignments"></param>
        /// <returns></returns>
        public List<ResultGroupConfiguration> UpdateGroupLayout(int groupId, List<ConfigVariable> unitVariableAssignment, List<ConfigVariable> unitVariableAssignmentForHallRiser, List<ConfigVariable> unitVariableAssignmentForDoor, string userName, List<UnitMappingValues> unitMappingValues, List<ConfigVariable> unitVariableAssignmentForControlLocation, List<DisplayVariableAssignmentsValues> displayVariableAssignments)
        {
            throw new NotImplementedException();
        }
    }
}
