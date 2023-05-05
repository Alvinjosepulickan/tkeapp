/************************************************************************************************************
************************************************************************************************************
   File Name     :   GroupLayoutBL.cs 
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.UIModel;
using System.Text.RegularExpressions;
using TKE.SC.Common;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class GroupLayoutBL : IGroupLayout
    {
        #region Variables
        private readonly IGroupLayoutDL _groupLayoutdl;
        private readonly IConfigure _configure;
        #endregion

        /// <summary>
        /// Constructor for GroupLayoutBL
        /// </summary>
        /// <param Name="groupLayoutdl"></param>
        /// <param Name="configure"></param>
        public GroupLayoutBL(ILogger<GroupLayoutBL> logger, IGroupLayoutDL groupLayoutdl, IConfigure configure)
        {
            _groupLayoutdl = groupLayoutdl;
            _configure = configure;
            Utility.SetLogger(logger);
        }

        /// <summary>
        /// Method for Save Group Layout
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="userName"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveGroupLayout(int groupId, string sessionId, JObject variableAssignments)
        {
            var methodBeginTime = Utility.LogBegin();
            var groupContantsDictionary = Utility.GetVariableMapping(Constant.GROUPMAPPERVARIABLES, Constant.GROUPMAPPERCONFIGURATION);
            var groupLayout = Utility.DeserializeObjectValue<GroupLayoutSave>(Utility.SerializeObjectValue(variableAssignments));
            List<DisplayVariableAssignmentsValues> displayVariableAssignments = groupLayout.DisplayVariableAssignments;
            var groupVariableListData = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            var groupData = VariableValuesResult(groupVariableListData.ToList());
            List<ConfigVariable> listOfGroupVariables = groupData.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();

            List<ConfigVariable> unitVariableAssignment = groupData.Where(oh => Regex.IsMatch(oh.VariableId, Constant.PARAMETERS_LAYOUT_BANKTYPE)).Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();
            //Convert List to Json
            List<ConfigVariable> unitVariableAssignmentForHallRiser = groupData.Where(oh => oh.VariableId.Contains(Constant.QUAD) & oh.VariableId.Contains(Constant.PARAMETERSVALUES)).Select(
               variableAssignment => new ConfigVariable
               {
                   VariableId = variableAssignment.VariableId,
                   Value = variableAssignment.Value
               }).ToList<ConfigVariable>();

            List<ConfigVariable> unitVariableAssignmentForDoor = groupData.Where(oh => oh.VariableId.Contains(Constant.DOOR) & oh.VariableId.Contains(Constant.PARAMETERS_SP)).Select(
              variableAssignment => new ConfigVariable
              {
                  VariableId = variableAssignment.VariableId,
                  Value = variableAssignment.Value
              }).ToList<ConfigVariable>();

            //Control Location
            List<ConfigVariable> unitVariableAssignmentForControlLocation = groupData.Where(oh => (oh.VariableId.Contains(Constant.PARAMETERSSPCONTROLROOM) ||
            oh.VariableId.Contains(Constant.FIXTURESTRATEGY_SP) || oh.VariableId.StartsWith(groupContantsDictionary[Constant.PARAMETERSNXVMDISTANCEFLOOR]) ||
            oh.VariableId.Contains(Constant.HALLSTATIONPARAM))).Select(
              variableAssignment => new ConfigVariable
              {
                  VariableId = variableAssignment.VariableId,
                  Value = variableAssignment.Value
              }).ToList<ConfigVariable>();
            var fixtureStrategy = true;
            foreach (var variableAssignment in unitVariableAssignmentForControlLocation)
            {
                if (variableAssignment.VariableId.Contains(groupContantsDictionary[Constant.FIXTURESTRATEGYVRAIBLEID]))
                {
                    fixtureStrategy = false;
                }
            }
            if (fixtureStrategy)
            {
                var variableAssignmentz = new Line();
                var emptyVariableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(variableAssignmentz));
                var configureRequest = _configure.CreateConfigurationRequestWithTemplate(emptyVariableAssignments, Constant.GROUPDEFAULTSCLMCALL);
                var configureResponseJObj =
                    await _configure.ConfigurationBl(configureRequest, configureRequest.PackagePath, sessionId).ConfigureAwait(false);
                var configureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
                var configureResponseArgument = configureResponse.Arguments;
                var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
                //set cache defaults
                _configure.SetGroupDefaultsCache(sessionId, configureResponseArgumentJObject);
                var configureRequestDictionary = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();

                var crossPackagevariableDictionary = new Dictionary<string, string>();

                var crossPackageVariableId = (JObject.Parse(File.ReadAllText(Constant.CROSSPACKAGEVARIABLEMAPPING)));
                JToken crossPackageVariables;
                crossPackageVariables = crossPackageVariableId[Constant.DEFAULTVALUES];
                crossPackagevariableDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(crossPackageVariables));

                var val = (from val1 in configureRequestDictionary
                           from val2 in crossPackagevariableDictionary
                           where Utility.CheckEquals(val1.Key.ToString(), val2.Key.ToString())
                           select new VariableAssignment
                           {
                               VariableId = val1.Key,
                               Value = val1.Value
                           }).Distinct().ToList();
                var fixtureStrategyDefaultValue = val.Where(oh => oh.VariableId.StartsWith(Constant.PARAMETERS_SP)).Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();
                unitVariableAssignmentForControlLocation.AddRange(fixtureStrategyDefaultValue);
            }

            var hasConflictsFlag = _configure.GetCacheValuesForConflictManagement(sessionId, Constant.GROUP);
            var userName = _configure.GetUserId(sessionId);
            var unitMappingValues = await _configure.GetUnitName(variableAssignments, groupId, sessionId, Constant.GROUPLAYOUTCONFIGURATION, displayVariableAssignments).ConfigureAwait(false);
            var result = _groupLayoutdl.SaveGroupLayout(groupId, unitVariableAssignment, unitVariableAssignmentForHallRiser, unitVariableAssignmentForDoor, userName, unitMappingValues, unitVariableAssignmentForControlLocation, displayVariableAssignments);
            var response = JArray.FromObject(result);

            if (result[0].Result == 1)
            {
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = result[0].Message,
                    ResponseArray = response
                });
            }
        }

        /// <summary>
        /// This method is used to duplicate a group 
        /// </summary>
        /// <param Name="unitID"></param>
        /// <param Name="groupID"></param>
        /// <param Name="carPositionList"></param>
        /// <param Name="operation"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> DuplicateGroupLayoutById(List<int> unitID, int groupID, List<CarPosition> carPositionList, Operation operation)
        {
            var methodBeginTime = Utility.LogBegin();
            ResultUnitConfiguration res = new ResultUnitConfiguration();
            List<ResultUnitConfiguration> lstResult = new List<ResultUnitConfiguration>();
            var unitIDDataTable = Utility.GetGroupIDDataTable(unitID);
            var carPositionDataTable = Utility.GetCarPositionDataTable(carPositionList);
            var result = _groupLayoutdl.DuplicateGroupLayoutById(unitIDDataTable, groupID, carPositionDataTable);
            ResultUnitConfiguration resultUnitConfiguration = new ResultUnitConfiguration();
            if (result != null && result.Tables.Count > 0)
            {
                if (result.Tables[0].Columns.Contains(Constant.RESULT))
                {
                    resultUnitConfiguration.result = 0;
                    resultUnitConfiguration.groupConfigurationId = groupID;
                    resultUnitConfiguration.message = Constant.UNITNAMEREPEATING;
                    string unitNames = string.Empty;
                    resultUnitConfiguration.isDuplicateNameError = true;
                    resultUnitConfiguration.carPositionsWithDuplicateNames = new List<string>();
                    var carPositionWithDuplicateName = (from DataRow dRow in result.Tables[1].Rows
                                                        select new { location = Convert.ToString(dRow[Constant.CARPOSITIONLOCATION]) }
                                                   );
                    foreach (var duplicatePosition in carPositionWithDuplicateName)
                    {
                        resultUnitConfiguration.carPositionsWithDuplicateNames.Add(Constant.PARAMETERlAYOUT + duplicatePosition.location);
                    }
                    resultUnitConfiguration.carPositionsWithDuplicateNames.Distinct();
                    foreach (DataRow row in result.Tables[1].Rows)
                    {
                        unitNames += unitNames.Equals(string.Empty) ? Convert.ToString(row[Constant.UNITDESIGNATION]) : Constant.COMMA + Convert.ToString(row[Constant.UNITDESIGNATION]);
                    }
                    resultUnitConfiguration.message = resultUnitConfiguration.message.Replace(Constant.EQUALTO, unitNames);
                    JArray responses = new JArray();
                    responses.Add(JObject.FromObject(JsonConvert.DeserializeObject(Utility.SerializeObjectValue(resultUnitConfiguration))));
                    Utility.LogEnd(methodBeginTime);
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = resultUnitConfiguration.message,
                        Description = Constant.DUPLICATEBUILDINGEQUIPMENTCONSOLEERRORMSG,
                        ResponseArray = responses
                    });

                }
                if (result.Tables.Count == 1)
                {
                    var unitList = (from DataRow row in result.Tables[0].Rows
                                    select new { UnitId = Convert.ToInt32(row[Constant.UNITIDCOLUMNID]), UnitName = Convert.ToString(row[Constant.UNITNAMECOLUMNNAME]) }).ToList();
                    switch (operation.ToString().ToUpper())
                    {
                        case Constant.OVERWRITE:
                            foreach (var unit in unitList)
                            {
                                resultUnitConfiguration.message = Constant.UNIT + unit.UnitName + Constant.OVERWRITTENSUCCESSFULLY;
                                resultUnitConfiguration.result = 1;
                                lstResult.Add(resultUnitConfiguration);
                            }
                            break;
                        default:
                            foreach (var unit in unitList)
                            {
                                resultUnitConfiguration.message = Constant.UNIT + unit.UnitName + Constant.CREATEDSUCCESSFULLY;
                                resultUnitConfiguration.result = 1;
                                lstResult.Add(resultUnitConfiguration);
                            }
                            break;
                    }
                }
                else
                {
                    resultUnitConfiguration.message = Constant.UNITS;
                    foreach (DataTable table in result.Tables)
                    {
                        if (table.Columns.Contains(Constant.UNITIDCOLUMNID))
                        {
                            var unitList = (from DataRow row in table.Rows
                                            select new { UnitId = Convert.ToInt32(row[Constant.UNITIDCOLUMNID]), UnitName = Convert.ToString(row[Constant.UNITNAMECOLUMNNAME]) }).ToList();
                            if (unitList.Count > 0)
                            {
                                resultUnitConfiguration.result = 1;
                                foreach (var unit in unitList)
                                {
                                    resultUnitConfiguration.message += unit.UnitName + Constant.COMMA + Constant.SPACECHAR;
                                }
                            }
                        }
                    }
                    switch (operation.ToString().ToUpper())
                    {
                        case Constant.OVERWRITE:
                            resultUnitConfiguration.message = resultUnitConfiguration.message.Substring(0, resultUnitConfiguration.message.Length - 2);
                            resultUnitConfiguration.message += Constant.OVERWRITTENSUCCESSFULLY;
                            break;
                        default:
                            resultUnitConfiguration.message = resultUnitConfiguration.message.Substring(0, resultUnitConfiguration.message.Length - 2);
                            resultUnitConfiguration.message += Constant.CREATEDSUCCESSFULLY;
                            break;
                    }
                    lstResult.Add(resultUnitConfiguration);
                }
            }

            var responseArray = JArray.FromObject(lstResult);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = responseArray };
        }

        /// <summary>
        /// Method for Update Group Layout
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="userName"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> UpdateGroupLayout(int groupId, string sectionTab, string sessionId, JObject varibleAssignments)
        {
            var methodBeginTime = Utility.LogBegin();
            var cachedDisplayVariableAssignments = new List<DisplayVariableAssignmentsValues>();
            var changeInFloorPlan = false;
            var unitsAddedOrRemoved = false;   /// if units are added
            var changeInDisplayVariableAssignments = false;
            var groupContantsDictionary = Utility.GetVariableMapping(Constant.GROUPMAPPERVARIABLES, Constant.GROUPMAPPERCONFIGURATION);
            var groupLayout = Utility.DeserializeObjectValue<GroupLayoutSave>(Utility.SerializeObjectValue(varibleAssignments));
            List<DisplayVariableAssignmentsValues> displayVariableAssignments = groupLayout.DisplayVariableAssignments;
            if(displayVariableAssignments!=null)
            {
                cachedDisplayVariableAssignments = await _configure.GetSetDisplayVariableAssignmentsForGroup(cachedDisplayVariableAssignments, groupId, sessionId);
                var setDisplayVariableAssignmentsToCache = await _configure.GetSetDisplayVariableAssignmentsForGroup(displayVariableAssignments, groupId, sessionId);

            }
            var groupVariableListData = Utility.DeserializeObjectValue<Line>(varibleAssignments.ToString()).VariableAssignments;
            //added cache variables
            var crossPackagevariableAssignments = new List<VariableAssignment>();
            var getCrossPackageValues = _configure.GetCrosspackageVariableAssignments(sessionId, Constant.GROUPCONFIGURATION);
            if (!string.IsNullOrEmpty(getCrossPackageValues))
            {
                crossPackagevariableAssignments = Utility.DeserializeObjectValue<List<VariableAssignment>>(getCrossPackageValues);
            }
            var newControlLocationParam = (from a in groupVariableListData where a.VariableId.Equals(groupContantsDictionary[Constant.CONTROLLERLOCATION_SP]) select a?.Value);
            if(newControlLocationParam.FirstOrDefault() != null && newControlLocationParam.FirstOrDefault().ToString() != Constant.REMOTE)
            {
                var newListData = crossPackagevariableAssignments.Select(x => x).ToList();
                foreach (var itemId in crossPackagevariableAssignments)
                {
                    if ( itemId.VariableId.Contains(groupContantsDictionary[Constant.PARMETERSXDIMENSIONVALUES]) ||
                                itemId.VariableId.Contains(groupContantsDictionary[Constant.PARAMETERSYDIMENSIONVALUES]) )
                    {
                        newListData.Remove(itemId);                        
                    }
                }
                crossPackagevariableAssignments = newListData;

            }
            var countOfCrossPackagevariables = crossPackagevariableAssignments.Where(x => Regex.IsMatch(x.VariableId, Constant.PARAMETERS_LAYOUT_BANKTYPE) && x.Value.Equals(Constants.TRUEVALUES)).ToList().Count();
            var countOfGroupVariables = groupVariableListData.Where(x => Regex.IsMatch(x.VariableId, Constant.PARAMETERS_LAYOUT_BANKTYPE) && x.Value.Equals(Constants.TRUEVALUES)).ToList().Count();
            if (countOfCrossPackagevariables != countOfGroupVariables)  /// if units are added or removed
            {
                unitsAddedOrRemoved = true;
            }
            foreach (var userAssignment in groupVariableListData)
            {
                foreach(var cachedData in crossPackagevariableAssignments)
                {
                    if(Regex.IsMatch(userAssignment.VariableId, Constant.PARAMETERS_LAYOUT_BANKTYPE) && Regex.IsMatch(cachedData.VariableId, Constant.PARAMETERS_LAYOUT_BANKTYPE) &&
                        Utility.CheckEquals(userAssignment.VariableId.ToString(),cachedData.VariableId.ToString()) &&
                        !Utility.CheckEquals(userAssignment.Value.ToString(), cachedData.Value.ToString()))
                    {
                        changeInFloorPlan = true;
                        break;
                    }
                    
                }
                if (changeInFloorPlan)
                {
                    break;
                }
            }
                foreach (var userAssignment in displayVariableAssignments)
            {
                foreach (var cachedData in cachedDisplayVariableAssignments)
                {
                    if (Regex.IsMatch(userAssignment.VariableId, Constant.PARAMETERS_LAYOUT_BANKTYPE) && Regex.IsMatch(cachedData.VariableId, Constant.PARAMETERS_LAYOUT_BANKTYPE) &&
                        Utility.CheckEquals(userAssignment.VariableId.ToString(), cachedData.VariableId.ToString()) && cachedData.Value.ToString().Equals(Constant.TRUEVALUES) &&
                        !Utility.CheckEquals(userAssignment.Value.ToString(), cachedData.Value.ToString()))
                    {
                        changeInDisplayVariableAssignments = true;
                        break;
                    }

                }
                if (changeInDisplayVariableAssignments)
                {
                    break;
                }
            }
            var configAssignments = groupVariableListData;
            foreach (var varAssign in configAssignments)
            {
                if (varAssign.VariableId.EndsWith(Constant.ACROSSTHEHALLDISTANCEPARAMETER) || varAssign.VariableId.EndsWith(Constant.BANKOFFSETPARAMETER) ||
                    varAssign.VariableId.EndsWith(Constant.PARAMETERSYDIMENSIONVALUES) || varAssign.VariableId.EndsWith(Constant.PARMETERSXDIMENSIONVALUES))
                {
                    varAssign.Value = Convert.ToInt32(varAssign.Value) * 12;
                }
            }

            var assignments = new Line() { VariableAssignments = configAssignments };
            //generate cross package variable assignments
            crossPackagevariableAssignments = _configure.GenerateVariableAssignmentsForUnitConfiguration(crossPackagevariableAssignments, assignments);
            var variableAssignmentz = new Line();
            variableAssignmentz.VariableAssignments = crossPackagevariableAssignments;

            if (variableAssignmentz?.VariableAssignments != null && variableAssignmentz.VariableAssignments.Any())
            {
                groupVariableListData = variableAssignmentz.VariableAssignments;
            }
            ConflictsStatus isEditFlow = ConflictsStatus.Valid;
            var getVariables = _configure.GetCacheVariablesForConflictChanges(null, sessionId);
            var listOfChangedVariables = _configure.CheckConflict(Utility.DeserializeObjectValue<List<VariableAssignment>>(Utility.SerializeObjectValue(groupLayout.VariableAssignments)), getVariables);
            if (listOfChangedVariables.Any())
            {
                var crossPackageVariableId = (JObject.Parse(File.ReadAllText(Constant.CROSSPACKAGEVARIABLEMAPPING)));
                var crossPackageVariables = crossPackageVariableId["GroupToUnit"];
                var crossPackagevariableDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(crossPackageVariables));
                foreach (var buildingVariable in listOfChangedVariables)
                {
                    if (crossPackagevariableDictionary.ContainsKey(buildingVariable.VariableId))
                    {
                        isEditFlow = ConflictsStatus.NeedValidation;
                    }
                }
            }
            // Adding Variables to Cache
            _configure.SetCrosspackageVariableAssignments(groupVariableListData.ToList(), sessionId, Constant.GROUPCONFIGURATION);

            var floorPlanDistanceVariables = (JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES))[Constant.FLOORPLANDISTANCEPARAMETERS]).ToList();
            var groupData = VariableValuesResult(groupVariableListData.ToList());
            List<ConfigVariable> listOfGroupVariables = groupData.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();
            List<ConfigVariable> unitVariableAssignment = groupData.Where(oh => Regex.IsMatch(oh.VariableId, Constant.PARAMETERS_LAYOUT_BANKTYPE) ||
            floorPlanDistanceVariables.Contains(oh.VariableId)).Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();
            List<ConfigVariable> unitVariableAssignmentForHallRiser = groupData.Where(oh => oh.VariableId.Contains(Constant.QUAD) && oh.VariableId.Contains(Constant.PARAMETERSVALUES)).Select(
              variableAssignment => new ConfigVariable
              {
                  VariableId = variableAssignment.VariableId,
                  Value = variableAssignment.Value
              }).ToList<ConfigVariable>();
            List<ConfigVariable> unitVariableAssignmentForDoor = groupData.Where(oh => oh.VariableId.Contains(Constant.DOOR) && oh.VariableId.Contains(Constant.PARAMETERS_SP)).Select(
              variableAssignment => new ConfigVariable
              {
                  VariableId = variableAssignment.VariableId,
                  Value = variableAssignment.Value
              }).ToList<ConfigVariable>();

            List<ConfigVariable> unitVariableAssignmentForControlLocation = groupData.Where(oh => (oh.VariableId.StartsWith(Constant.PARAMETERS_SP) ||
            oh.VariableId.StartsWith(groupContantsDictionary[Constant.PARAMETERSNXVMDISTANCEFLOOR])) && !oh.VariableId.Contains(groupContantsDictionary[Constant.TOTALBUILDINGFLOORTOFLOORHEIGHT])).Select(
              variableAssignment => new ConfigVariable
              {
                  VariableId = variableAssignment.VariableId,
                  Value = variableAssignment.Value
              }).ToList<ConfigVariable>();

            var fixtureStrategy = true;
            foreach (var variableAssignment in unitVariableAssignmentForControlLocation)
            {
                if (variableAssignment.VariableId.Contains(groupContantsDictionary[Constant.FIXTURESTRATEGYVRAIBLEID]))
                {
                    fixtureStrategy = false;
                }
            }
            if (fixtureStrategy)
            {
                var emptyStringvariableAssignment = new Line();
                var emptyVariableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(emptyStringvariableAssignment));
                var configureRequest = _configure.CreateConfigurationRequestWithTemplate(emptyVariableAssignments, Constant.GROUPDEFAULTSCLMCALL);
                var configureResponseJObj =
                    await _configure.ConfigurationBl(configureRequest, configureRequest.PackagePath, sessionId).ConfigureAwait(false);
                // configuration object values 
                var configureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
                var configureResponseArgument = configureResponse.Arguments;
                var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
                // adding defaults to cache
                _configure.SetGroupDefaultsCache(sessionId, configureResponseArgumentJObject);
                var configureRequestDictionary = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();


                //
                var crossPackagevariableDictionary = new Dictionary<string, string>();

                var crossPackageVariableId = (JObject.Parse(File.ReadAllText(Constant.CROSSPACKAGEVARIABLEMAPPING)));
                JToken crossPackageVariables;
                //
                crossPackageVariables = crossPackageVariableId[Constant.DEFAULTVALUES];
                crossPackagevariableDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(crossPackageVariables));

                var value = (from val1 in configureRequestDictionary
                             from val2 in crossPackagevariableDictionary
                             where Utility.CheckEquals(val1.Key.ToString(), val2.Key.ToString())
                             select new VariableAssignment
                             {
                                 VariableId = val1.Key,
                                 Value = val1.Value
                             }).Distinct().ToList();
                var fixtureStrategyDefaultValue = value.Where(oh => oh.VariableId.StartsWith(Constant.PARAMETERS_SP)).Select(
              variableAssignment => new ConfigVariable
              {
                  VariableId = variableAssignment.VariableId,
                  Value = variableAssignment.Value
              }).ToList<ConfigVariable>();
                unitVariableAssignmentForControlLocation.AddRange(fixtureStrategyDefaultValue);
            }

            var hasConflictsFlag = _configure.GetCacheValuesForConflictManagement(sessionId, Constant.GROUP);
            if (hasConflictsFlag)
            {
                isEditFlow = ConflictsStatus.InValid;
            }
            if (unitVariableAssignmentForControlLocation.Any() && unitVariableAssignmentForDoor.Any())
            {
                var hallStationToDoorsMapping = (JObject.Parse(File.ReadAllText(Constant.HALLSTATIONTODOORSMAPPING)));
                var hallStationToDoorsMappingDictionary = Utility.DeserializeObjectValue<Dictionary<string, List<string>>>(Utility.SerializeObjectValue(hallStationToDoorsMapping));
                foreach (var hallStation in unitVariableAssignmentForControlLocation)
                {
                    if (!String.IsNullOrEmpty(hallStation.Value.ToString()) && hallStation.Value.ToString().ToUpper().Equals(Constant.TRUEVALUES) && hallStation.VariableId.Contains("R_SP"))
                    {
                        var door = (from data in hallStationToDoorsMappingDictionary
                                    where data.Key.Equals(hallStation.VariableId)
                                    select data.Value).FirstOrDefault().ToList();
                        var dic = (from data in unitVariableAssignmentForDoor
                                   where door.Contains(data.VariableId)
                                   select data).ToList();
                        var flag = 0;
                        foreach (var doorsInCache in dic)
                        {
                            if (!doorsInCache.Value.Equals(String.Empty) && !doorsInCache.Value.Equals(Constant.NR))
                            {
                                flag = 1;
                            }
                        }
                        if (flag == 0)
                        {
                            hallStation.Value = Constant.FALSEVALUES;
                        }
                    }
                }
            }
            var HallStationToUnitsMapping = Utility.DeserializeObjectValue<Dictionary<string, List<string>>>((JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES))["HallStationToUnitsMapping"].ToString()));
            if (unitVariableAssignmentForControlLocation.Any() && unitVariableAssignment.Any())
            {
                foreach (var hallstation in unitVariableAssignmentForControlLocation)
                {
                    if (!String.IsNullOrEmpty(hallstation.Value.ToString()) && (hallstation.Value.Equals(Constant.TRUEVALUES)))
                    {
                        var banks = (from data in HallStationToUnitsMapping
                                     where data.Key.Equals(hallstation.VariableId)
                                     select data.Value).FirstOrDefault().ToList();
                        var bankDataInCache = (from data in unitVariableAssignment
                                               where banks.Contains(data.VariableId)
                                               select data).ToList();
                        var flag = 0;
                        foreach (var banksInCache in bankDataInCache)
                        {
                            if (banksInCache.Value.Equals(Constant.TRUEVALUES))
                            {
                                flag = 1;
                            }
                        }
                        if (flag == 0)
                        {
                            hallstation.Value = Constant.FALSEVALUES;
                        }
                    }
                }
            }
            var userName = _configure.GetUserId(sessionId);
            var unitMappingValues = await _configure.GetUnitName(varibleAssignments, groupId, sessionId, Constant.GROUPLAYOUTCONFIGURATION, displayVariableAssignments).ConfigureAwait(false);
            if(changeInFloorPlan || changeInDisplayVariableAssignments)
            {
                unitVariableAssignmentForHallRiser = new List<ConfigVariable>();
                unitVariableAssignmentForControlLocation = unitVariableAssignmentForControlLocation.Where(x => !x.VariableId.Contains("HS") && !x.VariableId.Contains("Control")).ToList();
                unitVariableAssignmentForDoor.ForEach(x=>x.Value = Constant.NR);
            }

            var groupConflictVariableAssignments = Utility.DeserializeObjectValue<Line>(varibleAssignments.ToString()).VariableAssignments;
            var listConflictVariables = groupConflictVariableAssignments.Where(x => x.VariableId.Contains(Constants.FIXTURESTRATEGY_SP)).ToList();
            if (listConflictVariables.Count() == 0)
            {
                listConflictVariables = groupConflictVariableAssignments.Where(x => x.VariableId.Contains(Constants.PARAMETERSCONTROLLERLOCATION_SP)).ToList();
            }
            var groupConflictVariables = new List<VariableAssignment>();
            if (listConflictVariables.Any())
            {
                groupConflictVariables = (from val1 in groupConflictVariableAssignments
                                          where !val1.VariableId.Contains(Constants.FRONTREARDOORTYPEANDHAND_SP)
                                          select new VariableAssignment
                                          {
                                              VariableId = val1.VariableId,
                                              Value = val1.Value
                                          }).ToList();
            }
            else
            {
                groupConflictVariables = (from val1 in groupConflictVariableAssignments
                                          select new VariableAssignment
                                          {
                                              VariableId = val1.VariableId,
                                              Value = val1.Value
                                          }).ToList();
            }
            if (sectionTab == Constants.FLOORPLANLAYOUT)
            {
                if (!unitsAddedOrRemoved)
                {
                    groupConflictVariables = (from val1 in groupConflictVariableAssignments
                                              where !val1.VariableId.Contains(Constants.FRONTREARDOORTYPEANDHAND_SP)
                                              select new VariableAssignment
                                              {
                                                  VariableId = val1.VariableId,
                                                  Value = val1.Value
                                              }).ToList();
                }
            }
            if (!groupConflictVariables.Any(x => x.VariableId.Contains(Constants.FRONTREARDOORTYPEANDHAND_SP)) && unitVariableAssignmentForDoor.Any() && sectionTab.ToUpper().Equals(Constant.DOORTAB))
            {
                var removedDoorVariables = (from var in unitVariableAssignmentForDoor
                                            where var.VariableId.Contains(Constants.FRONTREARDOORTYPEANDHAND_SP)
                                            select new VariableAssignment
                                            {
                                                VariableId = var.VariableId,
                                                Value = var.Value
                                            }).ToList();
                groupConflictVariables.AddRange(removedDoorVariables);
             }
            List<Result> conflictResponse = _configure.SaveConflictsValues(groupId, groupConflictVariables, Constants.GROUPENTITY);
            var result = _groupLayoutdl.UpdateGroupLayout(groupId, sectionTab, unitVariableAssignment, unitVariableAssignmentForHallRiser, unitVariableAssignmentForDoor, userName, unitMappingValues, unitVariableAssignmentForControlLocation, displayVariableAssignments, isEditFlow);
            var response = JArray.FromObject(result);
            var unitMappingListValues = new List<UnitMappingValues>();
            int eleValue = 1;

            foreach (var item in unitVariableAssignment)
            {
                if (item.VariableId.Contains(Constant.PARAMETERS_LAYOUT_B) && Utility.CheckEquals(item.Value.ToString().ToUpper(), Constant.TRUEVALUES))
                {
                    var vel = item.VariableId.Split(Constant.DOTCHAR)[1];
                    var newUnitMapValues = new UnitMappingValues()
                    {
                        ElevatorName = vel,
                        ElevatorValue = eleValue++
                    };
                    if (newUnitMapValues != null)
                    {
                        unitMappingListValues.Add(newUnitMapValues);
                    }
                }
            }
            var val = unitMappingListValues;
            _configure.SetCacheMappingValues(unitMappingListValues, sessionId);
            if (result[0].Result == 1)
            {
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, Message = result[0].Message, ResponseArray = response };
            }
            else
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = result[0].Message,
                    Description = Constant.UPDATEGROUPLAYOUTERROR,
                    ResponseArray = response
                });
            }
        }

        /// <summary>
        /// this method to create group configuration request body
        /// </summary>
        /// <param Name="varibleAssignments"></param>
        /// <returns></returns>
        public List<DisplayVariableAssignmentsValues> DisplayVariablesValuesResponse(JObject varibleAssignments)
        {
            var methodBeginTime = Utility.LogBegin();
            var objLine = Utility.DeserializeObjectValue<List<DisplayVariableAssignmentsValues>>(varibleAssignments[Constant.DISPLAYVARIABLEASSIGNMENTS]?.ToString());
            Utility.LogEnd(methodBeginTime);
            return objLine;
        }

        /// <summary>
        /// this method to create group configuration request body
        /// </summary>
        /// <param Name="varibleAssignments"></param>
        /// <returns></returns>
        public List<DisplayVariableAssignmentsValues> DisplayVariablesValuesResponseUpdate(JObject varibleAssignments)
        {
            var methodBeginTime = Utility.LogBegin();
            var objLine = Utility.DeserializeObjectValue<List<DisplayVariableAssignmentsValues>>(varibleAssignments[Constant.DISPLAYVARIABLEASSIGNMENTS]?.ToString());
            Utility.LogEnd(methodBeginTime);
            return objLine;
        }

        /// <summary>
        /// VariableValuesResult
        /// </summary>
        /// <param Name="variableAssignmentsListValues"></param>
        /// <returns></returns>
        public List<VariableAssignment> VariableValuesResult(List<VariableAssignment> variableAssignmentsListValues)
        {
            var methodBeginTime = Utility.LogBegin();
            var positionNumber = 1;
            if (variableAssignmentsListValues != null && variableAssignmentsListValues.Any())
            {
                // for true values 
                foreach (var variableAssignmentValue in variableAssignmentsListValues)
                {
                    if (variableAssignmentValue.VariableId.Contains(Constant.PARAMETERS_LAYOUT_B) && Utility.CheckEquals(variableAssignmentValue.Value.ToString(), Constant.TRUEVALUES))
                    {
                        var ValuesData = variableAssignmentValue.VariableId.Split(Constant.DOTCHAR).Last().ToUpper();
                        if (ValuesData.Contains(Constant.BANKTWOPOSITION))
                        {
                            var valChangd = Constant.BANKTWOPOSITION + positionNumber;
                            variableAssignmentValue.VariableId = variableAssignmentValue.VariableId.Replace(ValuesData, valChangd);
                            positionNumber++;
                        }

                    }
                }
                // for false values 
                foreach (var variableAssignmentValue in variableAssignmentsListValues)
                {
                    if (variableAssignmentValue.VariableId.Contains(Constant.PARAMETERS_LAYOUT_B) && Utility.CheckEquals(variableAssignmentValue.Value.ToString(), Constant.FALSEVALUES))
                    {
                        var ValuesData = variableAssignmentValue.VariableId.Split(Constant.DOTCHAR).Last().ToUpper();
                        if (ValuesData.Contains(Constant.BANKTWOPOSITION))
                        {
                            var valChangd = Constant.BANKTWOPOSITION + positionNumber;
                            variableAssignmentValue.VariableId = variableAssignmentValue.VariableId.Replace(ValuesData, valChangd);
                            positionNumber++;
                        }

                    }
                }
            }
            Utility.LogEnd(methodBeginTime);
            return variableAssignmentsListValues;

        }
    }
}