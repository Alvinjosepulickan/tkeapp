/************************************************************************************************************
************************************************************************************************************
   File Name     :   GroupConfigurationBL.cs 
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
using TKE.SC.Common;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class GroupConfigurationBL : IGroupConfiguration
    {
        /// <summary>
        /// Variable Collection
        /// </summary>
        #region Variables
        private readonly ICacheManager _cpqCacheManager;
        private readonly IGroupConfigurationDL _groupdl;
        private readonly IConfigure _configure;
        private readonly string _environment;
        private readonly IOpeningLocationDL _openingLocationdl;
        #endregion

        /// <summary>
        /// Constructor for GroupConfiguationBL
        /// </summary>
        /// <param name="utility"></param>
        /// <param name="groupdl"></param>
        /// <param name="configure"></param>
        /// <param name="cpqCacheManager"></param>
        /// <param name="logger"></param>
        public GroupConfigurationBL(IGroupConfigurationDL groupdl, IConfigure configure, ICacheManager cpqCacheManager, ILogger<GroupConfigurationBL> logger, IOpeningLocationDL openingLocationDl)
        {
            _groupdl = groupdl;
            _configure = configure;
            _environment = Constant.DEV;
            _cpqCacheManager = cpqCacheManager;
            _openingLocationdl = openingLocationDl;
        Utility.SetLogger(logger);
        }

        /// <summary>
        /// Get Group Configuration Details by GroupId Method
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="cr"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetGroupConfigurationDetailsByGroupId(int groupConfigurationId, JObject variableAssignments, string sessionId, string selectedTab)
        {
            JObject response = new JObject();
            var methodBeginTime = Utility.LogBegin();

            var unitContantsDictionary = Utility.GetVariableMapping(Constant.CUSTOMENGINEEREDCONSTANTMAPPERPATH, Constant.CUSTOMENGINEEREDVARIABLES);
            List<ConfigVariable> lstConfigVariable = new List<ConfigVariable>();
            foreach (var variable in unitContantsDictionary)
            {
                ConfigVariable configVariable = new ConfigVariable() { VariableId = variable.Key, Value = variable.Value };
                lstConfigVariable.Add(configVariable);
            }
            var dtVariables = Utility.DeserializeObjectValue<DataTable>(Utility.SerializeObjectValue(lstConfigVariable));
            //Getting the Product category
            var productCategory = _groupdl.GetProductCategoryByGroupId(groupConfigurationId, Constant.GROUPLOWERCASE, dtVariables);

            var groupContantsDictionary = Utility.GetVariableMapping(Constant.GROUPMAPPERVARIABLES, Constant.GROUPMAPPERCONFIGURATION);
            if (string.IsNullOrEmpty(productCategory))
                productCategory = Constant.PRODUCTELEVATOR;

            if (Utility.CheckEquals(productCategory, Constant.PRODUCTELEVATOR))
            {
                if (string.IsNullOrEmpty(selectedTab))
                {
                    selectedTab = Constant.FLOORPLANTAB;
                }
                var mainUiResponse = JObject.Parse(File.ReadAllText(Constant.GROUPCONFIGURATIONSMAINUIRESPONSE));
                var mainUiConfigurationResponseObj = mainUiResponse.ToObject<ConfigurationResponse>();
                var groupConfigurationResponse = new UIMappingBuildingConfigurationResponse();
                if (Utility.CheckEquals(selectedTab, Constant.GROUPHALLFIXTURETAB))
                {
                    var responseData = StartGroupHallFixtureConfigureBL(variableAssignments, groupConfigurationId, sessionId);
                    var serializedData = Utility.DeserializeObjectValue<ConfigurationResponse>(Utility.SerializeObjectValue(responseData.Result.Response));
                    foreach (var items in mainUiConfigurationResponseObj.Sections)
                    {
                        if (Utility.CheckEquals(items.Id, selectedTab))
                        {
                            var mainSectionValues = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(serializedData.Sections));
                            items.sections = Utility.DeserializeObjectValue<IList<SectionsValues>>(Utility.SerializeObjectValue(mainSectionValues));
                            items.AllOpeningsSelected = serializedData.AllOpeningsSelected;
                            break;
                        }
                    }
                    groupConfigurationResponse.Sections = mainUiConfigurationResponseObj.Sections;
                    groupConfigurationResponse.ConflictAssignments = serializedData.ConflictAssignments;
                    groupConfigurationResponse.EnrichedData = serializedData.EnrichedData;
                    groupConfigurationResponse.Permissions = serializedData.Permissions;
                    groupConfigurationResponse.ReadOnly = serializedData.ReadOnly;
                    _cpqCacheManager.SetCache(sessionId, _environment, Constant.INTERNALPREVIOUSGROUPCONFLICTSVALUES, Utility.SerializeObjectValue(groupConfigurationResponse.ConflictAssignments));
                    _cpqCacheManager.SetCache(sessionId, _environment, Constant.PREVIOUSGROUPCONFLICTSVALUES, Utility.SerializeObjectValue(groupConfigurationResponse.ConflictAssignments));
                    
                    var conflictStatusFlag = false;
                    if (groupConfigurationResponse.ConflictAssignments != null)
                    {
                        conflictStatusFlag = true;
                    }
                    var resultConflictStatusUpdate = _openingLocationdl.UpdateGroupConflictStatus(groupConfigurationId, conflictStatusFlag);
                    response = Utility.FilterNullValues(groupConfigurationResponse);
                }
                else
                {
                    var configurationRequest = CreateGroupConfigurationRequest(variableAssignments);
                    var lstConfigureVariable = _groupdl.GetGroupConfigurationDetailsByGroupId(groupConfigurationId, selectedTab, sessionId);
                    lstConfigureVariable.ConfigVariable = lstConfigureVariable.ConfigVariable.Where(x => !Utility.CheckEquals(x.VariableId, groupContantsDictionary[Constant.PRODUCTCATEGORY]) && !Utility.CheckEquals(x.VariableId, groupContantsDictionary[Constant.GROUPDESIGNATION])).ToList();
                    var displayVariablesValuesResponse = lstConfigureVariable.DisplayVariableAssignmentsValues;
                    configurationRequest.Line.VariableAssignments = lstConfigureVariable.ConfigVariable.Select(
                        variableAssignment => new VariableAssignment
                        {
                            VariableId = variableAssignment.VariableId,
                            Value = variableAssignment.Value
                        }).ToList<VariableAssignment>();
                    variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configurationRequest.Line));
                    var selectedNewTab = Utility.GetGroupLayoutTab(selectedTab);
                    if (Utility.CheckEquals(selectedNewTab.ToUpper(), Constant.GROUPLAYOUTCONFIGURATION))
                    {
                        //Call to DB
                        var buildingVariableAssignments = _groupdl.GetBuildingVariableAssignments(groupConfigurationId);
                        var variableAssignmentsForGroup = _configure.GeneratevariableAssignmentsForCrosspackageDependecy(Constant.GROUPLAYOUTCONFIGURATION, buildingVariableAssignments);
                        var variableAssignmentsList = Utility.DeserializeObjectValue<List<VariableAssignment>>(Utility.SerializeObjectValue(variableAssignmentsForGroup));
                        foreach (var groupVariableAssignment in variableAssignmentsList)
                        {
                            if (groupVariableAssignment.VariableId.Contains(Constants.BUILDING_CONFIGURATION + Constants.DOT))
                            {
                                var updatedVariableId = groupVariableAssignment.VariableId.ToString().Replace(Constants.BUILDING_CONFIGURATION + Constants.DOT, Constants.ELEVATOR001);
                                groupVariableAssignment.VariableId = updatedVariableId;
                            }
                        }
                        var unitMappingListValues = new List<UnitMappingValues>();
                        int elevatorNumber = 1;

                        foreach (var item in configurationRequest.Line.VariableAssignments)
                        {
                            if (item.VariableId.Contains(Constant.PARAMETERS_LAYOUT_B, StringComparison.InvariantCultureIgnoreCase) && Utility.CheckEquals(item.Value.ToString(), Constant.TRUEVALUES))
                            {
                                var vel = item.VariableId.Split(Constant.DOTCHAR)[1];
                                var newUnitMapValues = new UnitMappingValues()
                                {
                                    ElevatorName = vel,
                                    ElevatorValue = elevatorNumber++
                                };
                                if (newUnitMapValues != null)
                                {
                                    unitMappingListValues.Add(newUnitMapValues);
                                }
                            }
                        }
                        variableAssignmentsList = _configure.GenerateVariableAssignmentsForUnitConfiguration(variableAssignmentsList, configurationRequest.Line);
                        if (lstConfigureVariable.UpdatedTotalNumberOfFloors > 0)
                        {
                            foreach (var item in variableAssignmentsList)
                            {
                                if (item.VariableId.Contains(Constant.BLANDINGS))
                                {
                                    item.Value = lstConfigureVariable.UpdatedTotalNumberOfFloors;
                                }
                            }
                        }

                        var val = unitMappingListValues;
                        var lineObject = new Line();
                        lineObject.VariableAssignments = variableAssignmentsList;
                        _configure.GetCacheVariablesForConflictChanges(variableAssignmentsList, sessionId);
                        variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(lineObject));
                        _configure.GetCacheVariablesForConflictChanges(lineObject.VariableAssignments.ToList(), sessionId);
                        _configure.SetCrosspackageVariableAssignments(variableAssignmentsList, sessionId, Constant.GROUPCONFIGURATION);
                        _configure.SetCacheMappingValues(unitMappingListValues, sessionId);

                        _cpqCacheManager.RemoveCache(sessionId, _environment, Constant.INTERNALPREVIOUSGROUPCONFLICTSVALUES,null);
                        response = await _configure.ChangeGroupConfigure(variableAssignments, groupConfigurationId, sessionId, selectedTab, displayVariablesValuesResponse);
                        var grouphallFixtureResponse = Utility.DeserializeObjectValue<UIMappingBuildingConfigurationResponse>(Utility.SerializeObjectValue(response));
                        grouphallFixtureResponse.ReadOnly = _groupdl.CheckUnitConfigured(groupConfigurationId);

                        if (lstConfigureVariable.UpdatedTotalNumberOfFloors > 0)
                        {
                            var assignments = Utility.DeserializeObjectValue<Line>(Utility.SerializeObjectValue(variableAssignments));
                            foreach (var item in assignments.VariableAssignments)
                            {
                                if (item.VariableId.Contains(Constant.BLANDINGS))
                                {
                                    var resolvedConflictsValues = new ConflictMgmtList()
                                    {
                                        VariableId = item.VariableId,
                                        Value = item.Value
                                    };
                                    grouphallFixtureResponse.ConflictAssignments.ResolvedAssignments.Add(resolvedConflictsValues);
                                    break;
                                }
                            }
                        }
                        var enrichedData = JObject.Parse(File.ReadAllText(Constant.ELEVATORENRICHMENTTEMPLATE));
                        if (Utility.CheckEquals(selectedTab, Constant.GROUPCONFIGURATION))
                        {
                            enrichedData = JObject.Parse(File.ReadAllText(Constant.GROUPPOPUPENRICHMENTTEMPLATE));
                        }
                        grouphallFixtureResponse.EnrichedData = enrichedData;
                        var roleName = _configure.GetRoleName(sessionId);
                        var permissions = _groupdl.GetPermissionByRole(groupConfigurationId, roleName);
                        grouphallFixtureResponse.Permissions = permissions;
                        // conflicts
                        var getCrossPackageValues = _configure.GetCrosspackageVariableAssignments(sessionId, Constant.GROUPCONFIGURATION);
                        var userVariableAssignments = lineObject.VariableAssignments.ToList();
                        if (!string.IsNullOrEmpty(getCrossPackageValues))
                        {
                            userVariableAssignments = Utility.DeserializeObjectValue<List<VariableAssignment>>(getCrossPackageValues);
                        }
                        var conflictVariablesData = new List<ConflictMgmtList>();
                        foreach (var userVariables in userVariableAssignments)
                        {
                            if (lstConfigureVariable.VariableIds != null && lstConfigureVariable.VariableIds.Any())
                            {
                                foreach (var conflictVariables in lstConfigureVariable.VariableIds)
                                {
                                    if (Utility.CheckEquals(userVariables.VariableId, conflictVariables))
                                    {
                                        var conflictVal = new ConflictMgmtList()
                                        {
                                            VariableId = conflictVariables,
                                            Value = userVariables.Value
                                        };
                                        conflictVariablesData.Add(conflictVal);
                                    }
                                }
                            }
                        }
                        if (conflictVariablesData.Any())
                        {
                            var internalConflicts = (from val1 in conflictVariablesData
                                                     from val2 in lstConfigureVariable.VariableIds
                                                     where !Utility.CheckEquals(val1.VariableId, val2)
                                                     select new ConflictMgmtList()
                                                     {
                                                         VariableId = val2,
                                                         Value = string.Empty
                                                     }).ToList();
                            if (internalConflicts != null && internalConflicts.Any())
                            {
                                conflictVariablesData.AddRange(internalConflicts);
                            }
                        }
                        else
                        {
                            if (lstConfigureVariable.VariableIds != null && lstConfigureVariable.VariableIds.Any())
                            {
                                foreach (var conflictItems in lstConfigureVariable.VariableIds)
                                {
                                    var latestConflict = new ConflictMgmtList()
                                    {
                                        VariableId = conflictItems,
                                        Value = string.Empty
                                    };
                                    conflictVariablesData.Add(latestConflict);
                                }
                            }                            
                        }
                        if (grouphallFixtureResponse.ConflictAssignments.PendingAssignments.Any())
                        {
                            var previousConflicts = grouphallFixtureResponse.ConflictAssignments.PendingAssignments.Select(a => a.VariableId).ToList();
                            var filterConflictWithPreviousVariables = conflictVariablesData.Where(a => !previousConflicts.Contains(a.VariableId));
                            if (filterConflictWithPreviousVariables != null && filterConflictWithPreviousVariables.Any())
                            {
                                var filteredDuplivateConflicts = filterConflictWithPreviousVariables.Union(filterConflictWithPreviousVariables).ToList();
                                grouphallFixtureResponse.ConflictAssignments.PendingAssignments.AddRange(filteredDuplivateConflicts);
                            }
                        }
                        else
                        {
                            grouphallFixtureResponse.ConflictAssignments.PendingAssignments.AddRange(conflictVariablesData);
                        }
                        var filteredPendingConflictsVariables = grouphallFixtureResponse.ConflictAssignments.PendingAssignments.GroupBy(x => x.VariableId).Select(s => s.FirstOrDefault()).ToList();
                        if (filteredPendingConflictsVariables != null && filteredPendingConflictsVariables.Any())
                        {
                            grouphallFixtureResponse.ConflictAssignments.PendingAssignments = filteredPendingConflictsVariables;
                        }
                        grouphallFixtureResponse.ConflictAssignments.ResolvedAssignments.Distinct().ToList();
                        _cpqCacheManager.SetCache(sessionId, _environment, Constant.INTERNALPREVIOUSGROUPCONFLICTSVALUES, Utility.SerializeObjectValue(grouphallFixtureResponse.ConflictAssignments.PendingAssignments));
                        _cpqCacheManager.SetCache(sessionId, _environment, Constant.PREVIOUSGROUPCONFLICTSVALUES, Utility.SerializeObjectValue(grouphallFixtureResponse.ConflictAssignments));
                        response = Utility.FilterNullValues(grouphallFixtureResponse);
                    }
                }
            }
            else if (Utility.NonConfigurableProducts.Contains(productCategory))
            {
                var groupVariableAssignments = _groupdl.GetGroupInformationByGroupId(groupConfigurationId);
                response = await _configure.GroupInfoConfigure(sessionId, groupVariableAssignments);
            }
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = response
            };
        }

        /// <summary>
        /// This method used to call database method for save group configuration details
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="GroupName"></param>
        /// <param Name="userName"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>

        public async Task<ResponseMessage> SaveGroupConfiguration(int buildingId, string sessionId, JObject variableAssignments)
        {
            var methodBeginTime = Utility.LogBegin();
            List<ResultGroupConfiguration> resultGroupConfigurations = new List<ResultGroupConfiguration>();
            JArray response = new JArray();
            var groupContantsDictionary = Utility.GetVariableMapping(Constant.GROUPMAPPERVARIABLES, Constant.GROUPMAPPERCONFIGURATION);
            var groupData = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            List<ConfigVariable> listOfGroupVariables = groupData.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();

            List<ConfigVariable> groupNameList = groupData.Where(oh => Utility.CheckEquals(oh.VariableId, (groupContantsDictionary[Constant.GROUPDESIGNATION]))).Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();

            string groupName = groupNameList[groupNameList.Count() - 1].Value.ToString();
            var grpVariablejson = JsonConvert.SerializeObject(listOfGroupVariables);
            var userName = _configure.GetUserId(sessionId);

            var productCategory = listOfGroupVariables.Where(x => Utility.CheckEquals(x.VariableId, groupContantsDictionary[Constant.PRODUCTCATEGORY])).Select(y => y.Value).FirstOrDefault();
            var noOfUnits = listOfGroupVariables.Where(x => Utility.CheckEquals(x.VariableId, Constant.NOOFUNITS)).Select(y => y.Value).FirstOrDefault();

            resultGroupConfigurations = _groupdl.SaveGroupConfiguration(buildingId, groupName, userName, grpVariablejson, Convert.ToString(productCategory), Convert.ToInt32(noOfUnits));

            response = JArray.FromObject(resultGroupConfigurations);
            if (resultGroupConfigurations[0].Result == 1)
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
                    Message = resultGroupConfigurations[0].Message,
                    Description = resultGroupConfigurations[0].Message
                });
            }
        }

        /// <summary>
        /// This method used to call database method for updating group configuration details
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> UpdateGroupConfiguration(int buildingId, int groupConfigurationId, JObject variableAssignments)
        {
            var methodBeginTime = Utility.LogBegin();
            var groupData = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            List<ConfigVariable> lstvariableassignment = groupData.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();

            List<ConfigVariable> groupNameList = groupData.Where(oh => Utility.CheckEquals(oh.VariableId, Constant.GROUPDESIGNATION)).Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();

            string groupName = groupNameList[groupNameList.Count() - 1].Value.ToString();

            var grpVariablejson = JsonConvert.SerializeObject(lstvariableassignment);
            List<Result> conflictResponse = _configure.SaveConflictsValues(buildingId, Utility.DeserializeObjectValue<List<VariableAssignment>>(grpVariablejson), Constants.GROUPENTITY);
            var result = _groupdl.UpdateGroupConfiguration(buildingId, groupName, groupConfigurationId, grpVariablejson);
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
                    Description = result[0].Message
                });
            }
        }

        /// <summary>
        /// Get group configuration by BuildingId method
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        public Task<ResponseMessage> GetGroupConfigurationByBuildingId(string buildingId)
        {
            var methodBeginTime = Utility.LogBegin();
            Utility.LogEnd(methodBeginTime);
            return _groupdl.GetGroupConfigurationByBuildingId(buildingId);
        }

        /// <summary>
        /// Delete group configuration method
        /// </summary>
        /// <param Name="GroupId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> DeleteGroupConfiguration(int GroupId)
        {
            var methodBeginTime = Utility.LogBegin();
            List<GroupResult> reslist = new List<GroupResult>();
            GroupResult res = new GroupResult();

            if (GroupId == 0)
            {
                res.Message = Constant.SOMETHINGWENTWRONGMSG;
                reslist.Add(res);
                var responseBuildingConfiguration = JArray.FromObject(reslist);
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.NOTFOUND,
                    Message = Constant.INVALIDGROUPID,
                    ResponseArray = responseBuildingConfiguration
                });

            }
            else if (GroupId == -1)
            {
                res.Message = Constant.SOMETHINGWENTWRONGMSG;
                reslist.Add(res);
                var responseBuildingConfiguration = JArray.FromObject(reslist);
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.INVALIDGROUPID,
                    ResponseArray = responseBuildingConfiguration
                });
            }
            reslist = _groupdl.DeleteGroupConfiguration(GroupId);
            var response = JArray.FromObject(reslist);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
        }

        /// <summary>
        /// Method to call the configuration details
        /// </summary>
        /// <param Name="configurationRequest"></param>
        /// <param Name="projectId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> StartGroupConfigure(JObject variableAssignments, int buildingId, int groupId, string sessionId, string selectedTab)
        {
            var methodBeginTime = Utility.LogBegin();
            var configurationRequest = CreateGroupConfigurationRequest(variableAssignments);
            var groupContantsDictionary = Utility.GetVariableMapping(Constant.GROUPMAPPERVARIABLES, Constant.GROUPMAPPERCONFIGURATION);

            var displayVariablesValuesResponse = DisplayVariablesValuesResponse(variableAssignments);
            foreach (var variable in displayVariablesValuesResponse)
            {
                variable.UnitDesignation = string.Empty;
            }

            if (!string.IsNullOrEmpty(selectedTab) && selectedTab.ToUpper().Equals(Constant.GROUPCONFIGURATION))
            {
                int groupCount = _groupdl.GenerateGroupName(buildingId);
                string GroupName = Constant.G + Convert.ToInt32(groupCount + 1);
                ConfigVariable configVariable = new ConfigVariable();
                List<ConfigVariable> lstConfigureVariable = new List<ConfigVariable>();
                configVariable.VariableId = groupContantsDictionary[Constant.GROUPDESIGNATION];
                configVariable.Value = GroupName;
                lstConfigureVariable.Add(configVariable);

                List<VariableAssignment> lstvariableassignment = lstConfigureVariable.Select(
                        variableAssignment => new VariableAssignment
                        {
                            VariableId = variableAssignment.VariableId,
                            Value = variableAssignment.Value
                        }).ToList<VariableAssignment>();
                configurationRequest.Line.VariableAssignments = lstvariableassignment;
            }
            else
            {
                if (!string.IsNullOrEmpty(selectedTab) && selectedTab.ToUpper().Equals(Constant.GROUPLAYOUTCONFIGURATION))
                {
                    //var localVariableAssignmentValues = JArray.Parse(File.ReadAllText(Constant.VARIABLEASSIGNMENTVALUESDATA));
                    //var localVariableAssignmentValuesResponseObj = localVariableAssignmentValues.ToObject<List<VariableAssignment>>();
                    var defaultVariableAssignments = JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES))[Constant.DEFAULTVARIABLEASSIGNMENTS].Select
                        (x => new VariableAssignment { VariableId = (string)x["variableId"], Value = (string)x["value"] }).ToList();
                    configurationRequest.Line.VariableAssignments = defaultVariableAssignments;
                }
            }
            variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configurationRequest.Line));

            var response = await _configure.ChangeGroupConfigure(variableAssignments, groupId,
            sessionId, selectedTab, displayVariablesValuesResponse);

            var mainResponse = Utility.DeserializeObjectValue<UIMappingBuildingConfigurationResponse>(Utility.SerializeObjectValue(response));
            var enrichedData = JObject.Parse(File.ReadAllText(Constant.ELEVATORENRICHMENTTEMPLATE));
            if (selectedTab.ToUpper().Equals(Constant.GROUPCONFIGURATION))
            {
                enrichedData = JObject.Parse(File.ReadAllText(Constant.GROUPPOPUPENRICHMENTTEMPLATE));
            }
            mainResponse.EnrichedData = enrichedData;
            var rolename = _configure.GetRoleName(sessionId);
            var permission = _groupdl.GetPermissionByRole(groupId, rolename);
            mainResponse.Permissions = permission;
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = Utility.FilterNullValues(mainResponse) };
        }

        /// <summary>
        /// this method to create group configuration request body
        /// </summary>
        /// <param Name="varibleAssignments"></param>
        /// <returns></returns>
        private ConfigurationRequest CreateGroupConfigurationRequest(JObject varibleAssignments)
        {
            var methodBeginTime = Utility.LogBegin();
            var stubReqbody = JObject.Parse(System.IO.File.ReadAllText(Constant.GROUPCONFIGURATIONREQESTBODYSTUBPATH)).ToString();
            var configurationRequest = Utility.DeserializeObjectValue<ConfigurationRequest>(stubReqbody);
            configurationRequest.Date = DateTime.Now;
            var objLine = Utility.DeserializeObjectValue<Line>(varibleAssignments.ToString());
            configurationRequest.Line.VariableAssignments = objLine.VariableAssignments;
            Utility.LogEnd(methodBeginTime);
            return configurationRequest;
        }

        /// <summary>
        /// Save Group Hall Fixture Configuration
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="groupHallFixturesData"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveGroupHallFixture(int groupId, GroupHallFixturesData groupHallFixturesData, string sessionId, int is_Saved)
        {
            var methodBeginTime = Utility.LogBegin();
            var groupContantsDictionary = Utility.GetVariableMapping(Constant.GROUPMAPPERVARIABLES, Constant.GROUPCONSTANTMAPPER);
            var constantMapper = JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES));
            var userId = _configure.GetUserId(sessionId);
            if (constantMapper[Constant.CONSOLEWITHSWITCHVARIABLES].Select(x => x.ToString()).ToList().Contains(groupHallFixturesData.FixtureType))
            {
                var consoleTemplateResponse = JObject.Parse(File.ReadAllText(Constant.GROUPHALLFIXTURECONSOLEPATH)).ToObject<ConfigurationResponse>();
            
                var switchVariables = (from consoles in consoleTemplateResponse.Sections
                                   where consoles.Id.Equals(groupHallFixturesData.FixtureType)
                                   select consoles.Variables).FirstOrDefault();
                foreach (var varAssign in switchVariables)
                {
                    if (Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(constantMapper[Constant.SWITCHVARIABLESTOBEREMOVED])).Contains(varAssign.Id))
                    {
                        var varAssignForConsole = new ConfigVariable { VariableId = varAssign.Id, Value = "TRUE" };
                        groupHallFixturesData.VariableAssignments.Add(varAssignForConsole);
                    }

                }
            }

            foreach (var variable in groupHallFixturesData.VariableAssignments)
            {
                if (Utility.CheckEquals(variable.VariableId, groupContantsDictionary[Constant.FIREBOXCONSTANT]) && variable.Value.Equals(false))
                {

                    variable.Value = Constant.NR;
                }
            }
            var oldConsoleData = _configure.SetCacheGroupHallFixtureConsoles(null, sessionId, groupId);
            var isHallStation=false;
            if (!groupHallFixturesData.FixtureType.Equals(Constant.AGILEHALLSTATION))
            {
                isHallStation = true;
            }
            else
            {
                isHallStation = false;
            }
            var  lstConsoleHistory = GetConsoleHistoriesGroupHallFixture(oldConsoleData, groupHallFixturesData, isHallStation);
            var lstConsoleHistoryData = lstConsoleHistory.Where(x => !(Utility.CheckEquals(x.PresentValue, x.PreviousValue) || x.PresentValue.Equals(x.PreviousValue))).ToList();
            if (lstConsoleHistoryData != null && lstConsoleHistoryData.Any())
            {
                lstConsoleHistory = lstConsoleHistoryData;
            }
            var historyTable = GetLogHistoryTableForConsole(lstConsoleHistory);
            var result = _groupdl.SaveGroupHallFixture(groupId, groupHallFixturesData, userId, is_Saved, historyTable);
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
                    Description = result[0].Message,
                    ResponseArray = response
                });
            }
        }

        /// <summary>
        /// StartGroupHallFixtureConfigureBL
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> StartGroupHallFixtureConfigureBL(JObject variableAssignments, int groupConfigurationId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            string sectionTab = Constant.GROUPHALLFIXTURE;
            int groupId = groupConfigurationId;
            var groupHallFixtureConfigurations = new List<GroupHallFixtures>();
            var variableAssignmentz = new Line();
            var cachedConfigurations = _cpqCacheManager.GetCache(sessionId, _environment, groupId.ToString(), Constant.CURRENTGROUPCONFIGURATION);
            //fetching configuration from clm if cached configuration is null
            if (string.IsNullOrEmpty(cachedConfigurations))
            {
                var groupLayout = _groupdl.GetGroupConfigurationDetailsByGroupId(groupId, Constant.GROUPLAYOUTCONFIGURATION, sessionId);
                if (groupLayout != null)
                {
                    if (groupLayout.ConfigVariable != null && groupLayout.ConfigVariable.Any())
                    {
                        variableAssignmentz.VariableAssignments = (from value in groupLayout.ConfigVariable
                                                                   select new VariableAssignment()
                                                                   {
                                                                       VariableId = value.VariableId,
                                                                       Value = value.Value
                                                                   }).ToList();
                    }

                }
                variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(variableAssignmentz));
                var configureRequest = _configure.CreateConfigurationRequestWithTemplate(variableAssignments, Constant.GROUPCONFIGURATIONNAME);
                var configureResponseJObj =
                    await _configure.ConfigurationBl(configureRequest, configureRequest.PackagePath, sessionId).ConfigureAwait(false);
                var configureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
                var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponse.Arguments));
                // adding defaults to cache
                _configure.SetGroupDefaultsCache(sessionId, configureResponseArgumentJObject);
                cachedConfigurations = Utility.SerializeObjectValue(configureResponseArgumentJObject[Constant.CONFIGURATION]);
                _cpqCacheManager.SetCache(sessionId, _environment, groupId.ToString(), Constant.CURRENTGROUPCONFIGURATION, cachedConfigurations);
            }
            var configureRequestDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(cachedConfigurations);
            var crossPackagevariableDictionary = new Dictionary<string, string>();
            var crossPackageVariableId = (JObject.Parse(File.ReadAllText(Constant.VARIABLEDICTIONARY)));
            JToken crossPackageVariables;
            crossPackageVariables = crossPackageVariableId[Constant.DEFAULTVALUES];
            crossPackagevariableDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(crossPackageVariables));
            //GettingMapper json for mapping group configuration details
            var groupConfigurationMapperDictionary = JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES))[Constant.GROUPCONFIGURATIONVARIABLEMAPPER].ToObject<Dictionary<string, string>>();
            //Fetchng hallstation variables from configurations
            var hallstations = (from hallStation in configureRequestDictionary.Where(x => x.Key.Contains(groupConfigurationMapperDictionary[Constant.HALLSTATION], StringComparison.OrdinalIgnoreCase)).ToList()
                                select new ConfigVariable
                                {
                                    VariableId = hallStation.Key,
                                    Value = hallStation.Value
                                }).Distinct().ToList();
            hallstations = hallstations.Where(x => x.Value.Equals(Constant.TRUEVALUES)).ToList();
            //validations for opening location and grouphallfixture tab
            ValidationForGroupHallFixture(groupId, hallstations, methodBeginTime);
            //fetching Fixture startegy variables from Configurations
            var fixtureStrategyList = configureRequestDictionary.Where(x => x.Key.Contains(groupConfigurationMapperDictionary[Constant.FIXTURESTRATEGY], StringComparison.OrdinalIgnoreCase)).ToList();
            var fixtureStrategy = Convert.ToString(fixtureStrategyList.Count > 0 ? fixtureStrategyList[0].Value : Constant.ETA);

            var val = (from val1 in configureRequestDictionary
                       from val2 in crossPackagevariableDictionary
                       where Utility.CheckEquals(val1.Key.ToString(), val2.Key.ToString())
                       select new VariableAssignment
                       {
                           VariableId = val1.Key,
                           Value = val1.Value
                       }).Distinct().ToList();

            if (string.IsNullOrEmpty(fixtureStrategy))
            {
                fixtureStrategy = val.Where(oh => oh.VariableId.Contains(Constant.FIXTURESTRATEGY)).Select(
                    variableAssignment => new ConfigVariable
                    {
                        VariableId = variableAssignment.VariableId,
                        Value = variableAssignment.Value
                    }).ToList()?[0].Value.ToString();
            }
            var doors = val.Where(oh => oh.VariableId.Contains(Constant.DOOR)).Select(
                    variableAssignment => new ConfigVariable
                    {
                        VariableId = variableAssignment.VariableId,
                        Value = variableAssignment.Value
                    }).ToList<ConfigVariable>();
            if (Convert.ToString(groupConfigurationId) != null)
            {
                if (groupConfigurationId != 0)
                {
                    List<ConfigVariable> lstConfigureVariable = new List<ConfigVariable>();
                    if (Utility.CheckEquals(sectionTab, Constant.GROUPHALLFIXTURE))
                    {
                        var userName = _configure.GetUserId(sessionId);
                        var unitDetails = _groupdl.GetUnitDetails(groupConfigurationId, doors);
                        _cpqCacheManager.SetCache(sessionId, _environment, Constant.UNITDETAILSCPQ, Utility.SerializeObjectValue(unitDetails));
                        var consoleTemplate = JObject.Parse(File.ReadAllText(Constant.GROUPHALLFIXTURECONSOLEPATH));
                        var consoleTemplateObj = consoleTemplate.ToObject<ConfigurationResponse>();
                        groupHallFixtureConfigurations = _groupdl.GetGroupHallFixturesData(groupConfigurationId, userName, fixtureStrategy, hallstations);

                        groupHallFixtureConfigurations = _configure.SetCacheGroupHallFixtureConsoles(groupHallFixtureConfigurations, sessionId, groupConfigurationId);
                    }
                }
            }

            var response = await _configure.StartGroupHallFixtures(sessionId, sectionTab, fixtureStrategy, variableAssignments, groupHallFixtureConfigurations);
            var grouphallFixtureResponse = Utility.DeserializeObjectValue<ConfigurationResponse>(Utility.SerializeObjectValue(response.Response));

            //Adding internal conflicts
            var listOfGroupHallFixtureConflicts = groupHallFixtureConfigurations[0].VariableIds;
            if (listOfGroupHallFixtureConflicts != null && listOfGroupHallFixtureConflicts.Any())
            {
                var conflictVariablesData = new List<ConflictMgmtList>();
                foreach (var conflictItems in listOfGroupHallFixtureConflicts)
                {
                    var latestConflict = new ConflictMgmtList()
                    {
                        VariableId = conflictItems,
                        Value = string.Empty
                    };
                    conflictVariablesData.Add(latestConflict);
                }
                if (conflictVariablesData.Any())
                {
                    if (grouphallFixtureResponse.ConflictAssignments != null)
                    {                       
                        grouphallFixtureResponse.ConflictAssignments.PendingAssignments.AddRange(conflictVariablesData);
                        grouphallFixtureResponse.ConflictAssignments.PendingAssignments.Distinct();
                    }
                    else
                    {
                        grouphallFixtureResponse.ConflictAssignments = new ConflictManagement()
                        {
                            PendingAssignments = conflictVariablesData,
                            ResolvedAssignments = new List<ConflictMgmtList>()
                        };
                    }
                    var filteredPendingConflictsVariables = grouphallFixtureResponse.ConflictAssignments.PendingAssignments.GroupBy(x => x.VariableId).Select(s => s.FirstOrDefault()).ToList();
                    if (filteredPendingConflictsVariables != null && filteredPendingConflictsVariables.Any())
                    {
                        var conflictMgmtLists = filteredPendingConflictsVariables.Where(x => !Utility.CheckEquals(x.VariableId, "groupHallFixtures")).ToList();
                        grouphallFixtureResponse.ConflictAssignments.PendingAssignments = conflictMgmtLists.Any()?conflictMgmtLists:filteredPendingConflictsVariables;
                    }
                }
            }
            var enrichedData = JObject.Parse(File.ReadAllText(Constant.ELEVATORENRICHMENTTEMPLATE));
            grouphallFixtureResponse.EnrichedData = enrichedData;
            var rolename = _configure.GetRoleName(sessionId);
            var permission = _groupdl.GetPermissionByRole(groupConfigurationId, rolename);
            grouphallFixtureResponse.Permissions = permission;
            response.Response = Utility.FilterNullValues(grouphallFixtureResponse);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = JObject.FromObject(response.Response) };
        }

        /// <summary>
        ///  Add or Change GroupConsoles
        /// </summary>
        /// <param Name="consoleId"></param>
        /// <param Name="groupId"></param>
        /// <param Name="SessionId"></param>
        /// <param Name="fixtureSelected"></param>
        /// <param Name="objGroup"></param>
        /// <param Name="isSave"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> AddorChangeGroupConsole(int consoleId, int isChange, int groupId, string SessionId, string fixtureSelected, bool isReset, GroupHallFixturesData objGroup = null, bool isSave = false)
        {
            var methodBeginTime = Utility.LogBegin();
            var userId = _configure.GetUserId(SessionId);
            List<UnitMappingValues> lstunits = new List<UnitMappingValues>();
            var groupContantsDictionary = Utility.GetVariableMapping(Constant.GROUPMAPPERVARIABLES, Constant.GROUPCONSTANTMAPPER);
            var fixtureType = fixtureSelected;
            var variableAssignmentz = new Line();
            JObject variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(variableAssignmentz));
            var configureRequest = _configure.CreateConfigurationRequestWithTemplate(variableAssignments, Constant.GROUPCONFIGURATIONNAME);
            var configureResponseJObj =
                await _configure.ConfigurationBl(configureRequest, configureRequest.PackagePath, SessionId).ConfigureAwait(false);
            var configureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = configureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            // adding defaults to cache
            _configure.SetGroupDefaultsCache(SessionId, configureResponseArgumentJObject);
            var configureRequestDictionary = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();

            var crossPackagevariableDictionary = new Dictionary<string, string>();

            var crossPackageVariableId = (JObject.Parse(File.ReadAllText(Constant.VARIABLEDICTIONARY)));
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
            List<ConfigVariable> dbMapperVariables = new List<ConfigVariable>();
            dbMapperVariables.Add(new ConfigVariable()
            {
                VariableId = Constant.FIXTURESTRATEGY,
                Value = groupContantsDictionary[Constant.FIXTURESTRATEGYVRAIBLEID]
            });
            var fixtureStrategy = _groupdl.GetGroupFixtureStrategy(groupId);
            if (string.IsNullOrEmpty(fixtureStrategy))
            {
                fixtureStrategy = val.Where(oh => oh.VariableId.Contains(groupContantsDictionary[Constant.FIXTURESTRATEGYVRAIBLEID])).Select(
                    variableAssignment => new ConfigVariable
                    {
                        VariableId = variableAssignment.VariableId,
                        Value = variableAssignment.Value
                    }).ToList()?[0].Value.ToString();
            }
            var doors = val.Where(oh => oh.VariableId.Contains(Constant.DOOR)).Select(
                    variableAssignment => new VariableAssignment
                    {
                        VariableId = variableAssignment.VariableId,
                        Value = variableAssignment.Value
                    }).ToList<VariableAssignment>();
            var groupHallFixtureConsoles = _configure.SetCacheGroupHallFixtureConsoles(null, SessionId, groupId);
            List<HallStations> HallStationLocation = new List<HallStations>();
            foreach (var console in groupHallFixtureConsoles)
            {
                if (console.GroupHallFixtureType != Constants.AGILEHALLSTATION)
                {
                    if (console.HallStations != null && console.HallStations.Any())
                    {
                        foreach (var hall in console.HallStations)
                        {
                            var hallStation = (from hallStationData in HallStationLocation select hallStationData.HallStationName).ToList();

                            if (hallStation.Contains(hall.HallStationName))
                            {
                                continue;
                            }
                            else
                            {
                                HallStations hallStationData = new HallStations()
                                {
                                    HallStationId = hall.HallStationId,
                                    HallStationName = hall.HallStationName,
                                    NoOfFloors = hall.NoOfFloors,
                                    openingDoors = hall.openingDoors,
                                    openingsAssigned = new List<GroupHallFixtureLocations>()
                                };
                                foreach (var opening in hall.openingsAssigned)
                                {
                                    GroupHallFixtureLocations groupHallFixtureLocation = new GroupHallFixtureLocations();
                                    groupHallFixtureLocation.HallStationName = opening.HallStationName;
                                    if (opening.Front != null)
                                    {
                                        groupHallFixtureLocation.Front = new LandingOpening
                                        {
                                            Value = opening.Front.Value,
                                            InCompatible = opening.Front.InCompatible,
                                            NotAvailable = opening.Front.NotAvailable
                                        };
                                    }
                                    if (opening.Rear != null)
                                    {
                                        groupHallFixtureLocation.Rear = new LandingOpening
                                        {
                                            Value = opening.Rear.Value,
                                            InCompatible = opening.Rear.InCompatible,
                                            NotAvailable = opening.Rear.NotAvailable
                                        };
                                    }
                                    groupHallFixtureLocation.UnitId = opening.UnitId;
                                    groupHallFixtureLocation.FloorNumber = opening.FloorNumber;
                                    groupHallFixtureLocation.FloorDesignation = opening.FloorDesignation;
                                    hallStationData.openingsAssigned.Add(groupHallFixtureLocation);
                                }
                                HallStationLocation.Add(hallStationData);
                            }
                        }

                    }
                }
            }

            if (isChange == 0)
            {
                var consoleToRemove = (from console in groupHallFixtureConsoles
                                       where (console.VariableAssignments == null || console.GroupHallFixtureLocations == null) && console.IsController == false
                                       select console).ToList();
                if (consoleToRemove.FirstOrDefault() != null)
                {
                    foreach (var console in consoleToRemove)
                    {
                        groupHallFixtureConsoles.Remove(console);
                    }
                }
            }


            var unitDetails = _configure.GetUnitDetails(SessionId);
            if (groupHallFixtureConsoles == null)
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.CACHEEMPTY
                });
            }
            foreach (var console in groupHallFixtureConsoles)
            {
                if (console.ConsoleId == 0)
                {
                    console.GroupHallFixtureType = fixtureType;
                }
            }
            List<List<UnitDetailsValues>> lstSelectedFrontFloorNumber = new List<List<UnitDetailsValues>>();
            foreach (var console in groupHallFixtureConsoles)
            {
                if (console.GroupHallFixtureType != Constants.AGILEHALLSTATION)
                {
                    if (console.HallStations != null && console.HallStations.Any())
                    {
                        foreach (var hallData in console.HallStations)
                        {
                            var hallStationDataValues = (from hallStation in HallStationLocation select hallStation.HallStationName).ToList();

                            if (hallStationDataValues.Contains(hallData.HallStationName))
                            {
                                continue;
                            }
                            else
                            {
                                HallStations hallStationData = new HallStations()
                                {
                                    HallStationId = hallData.HallStationId,
                                    HallStationName = hallData.HallStationName,
                                    NoOfFloors = hallData.NoOfFloors,
                                    openingDoors = hallData.openingDoors,
                                    openingsAssigned = new List<GroupHallFixtureLocations>()
                                };
                                foreach (var opening in hallData.openingsAssigned)
                                {
                                    GroupHallFixtureLocations groupHallFixtureLocation = new GroupHallFixtureLocations();
                                    groupHallFixtureLocation.HallStationName = opening.HallStationName;
                                    if (opening.Front != null)
                                    {
                                        groupHallFixtureLocation.Front = new LandingOpening
                                        {
                                            Value = opening.Front.Value,
                                            InCompatible = opening.Front.InCompatible,
                                            NotAvailable = opening.Front.NotAvailable
                                        };
                                    }
                                    if (opening.Rear != null)
                                    {
                                        groupHallFixtureLocation.Rear = new LandingOpening
                                        {
                                            Value = opening.Rear.Value,
                                            InCompatible = opening.Rear.InCompatible,
                                            NotAvailable = opening.Rear.NotAvailable
                                        };
                                    }
                                    groupHallFixtureLocation.UnitId = opening.UnitId;
                                    groupHallFixtureLocation.FloorNumber = opening.FloorNumber;
                                    groupHallFixtureLocation.FloorDesignation = opening.FloorDesignation;
                                    hallStationData.openingsAssigned.Add(groupHallFixtureLocation);
                                }
                                HallStationLocation.Add(hallStationData);
                            }
                        }
                    }
                }
            }
            if (Utility.Equals(fixtureStrategy, Constant.ETA_AND_ETD))
            {

                if (Utility.HallStationConsoles.Contains(fixtureType))
                {
                    lstSelectedFrontFloorNumber = (from varGroupHallFixtureConsole in groupHallFixtureConsoles
                                                   where varGroupHallFixtureConsole.GroupHallFixtureType.ToUpper().Contains(Constant.AGILEHALLSTATION.ToUpper()) ||
                                                    varGroupHallFixtureConsole.GroupHallFixtureType.ToUpper().Contains(Constant.TRADITIONALHALLSTATION.ToUpper())
                                                   select varGroupHallFixtureConsole.UnitDetails
                                                  ).ToList();

                }
                else
                {
                    lstSelectedFrontFloorNumber = (from varGroupHallFixtureConsole in groupHallFixtureConsoles
                                                   where varGroupHallFixtureConsole.GroupHallFixtureType.ToUpper().Contains(fixtureType.ToUpper())
                                                   select varGroupHallFixtureConsole.UnitDetails
                                                   ).ToList();
                }
            }
            else
            {
                lstSelectedFrontFloorNumber = (from varGroupHallFixtureConsole in groupHallFixtureConsoles
                                               where varGroupHallFixtureConsole.GroupHallFixtureType.ToUpper().Equals(fixtureType.ToUpper())
                                               select varGroupHallFixtureConsole.UnitDetails
                                                   ).ToList();

            }
            var newConsole = new GroupHallFixtures();
            newConsole.GroupHallFixtureType = fixtureType;
            if (objGroup == null)
            {
                CreateNewGroupHallFixtureConsole(groupHallFixtureConsoles, fixtureType, consoleId, ref newConsole, lstSelectedFrontFloorNumber, unitDetails, SessionId, HallStationLocation, groupId, fixtureStrategy, userId);
            }
            else
            {
                ChangeGroupHallFixtureConsole(groupHallFixtureConsoles, fixtureType, consoleId, ref newConsole, lstSelectedFrontFloorNumber, unitDetails, SessionId, HallStationLocation, groupId, fixtureStrategy, userId, objGroup, fixtureSelected);
            }
            var res = await _configure.GroupHallFixtureConsoleConfigureBl(newConsole, SessionId, fixtureType, isSave, fixtureStrategy, groupId);

            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = res
            };
        }

        /// <summary>
        /// This method is used to delete a group Hall fixture console
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="consoleId"></param>
        /// <param Name="fixtureType"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> DeleteGroupHallFixtureConsole(int groupId, int consoleId, string fixtureType, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            if (groupId == 0 || consoleId == 0 || fixtureType.Equals(string.Empty))
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.INVALIDINPUT,
                    Description = Constant.INVALIDINPUT
                });
            }
            var userId = _configure.GetUserId(sessionId);
            var oldConsoleData = _configure.SetCacheGroupHallFixtureConsoles(null, sessionId, groupId);
            var grpConsole = new GroupHallFixtures();
            var cachedConsoles = (from console in oldConsoleData
                                  where console.GroupHallFixtureType != null && console.GroupHallFixtureType.Equals(fixtureType)
                                  && console.ConsoleId.Equals(consoleId)
                                  select console).ToList();
            if (cachedConsoles.Count > 0)
            {
                grpConsole = cachedConsoles[0];
            }
            var historyTable = new List<LogHistoryTable>();
            var varassignment = Utility.GetLandingOpeningAssignmentSelectedForGroupHallFixture(grpConsole.UnitDetails);
            if (grpConsole.VariableAssignments.Count > 0)
            {
                var groupHallFixtureConsoleTemplate = JObject.Parse(File.ReadAllText(Constant.GROUPHALLFIXTURECONSOLEPATH));
                var groupHallFixtureConsoleTemplateObj = groupHallFixtureConsoleTemplate.ToObject<ConfigurationResponse>();
                var lstvariables = Utility.GetAllSectionVariables(groupHallFixtureConsoleTemplateObj.Sections);
                var enrichedData = JObject.Parse(File.ReadAllText(Constant.ELEVATORENRICHMENTTEMPLATE));
                var groupContantsDictionary = enrichedData[Constant.VARIABLEVALUES];
                foreach (var assignment in grpConsole.VariableAssignments)
                {
                    if (!String.IsNullOrEmpty(assignment.VariableId.ToString()))
                    {
                        var needVariables = Utility.GetTokens(assignment.VariableId, groupContantsDictionary, false);
                        var currentPropertyCollection = needVariables.Select(x => (JProperty)x).Where(x => x.Name == Constant.PROPERTIES).Select(x => x.Value).FirstOrDefault();
                        var displayName = currentPropertyCollection.Children().Where(x => x.Children<JProperty>().Any(y => y.Name == Constant.SECTIONID && y.Value.ToString() == Constant.FDADISPLAYNAME)).SelectMany(x => x.Children<JProperty>().Select(y => y.Name == Constant.VALUE ? y.Value.ToString() : Constant.SPACE)).FirstOrDefault(x => !string.IsNullOrEmpty(x));
                        var history = new LogHistoryTable()
                        {
                            VariableId = string.Concat(fixtureType.Replace(Constant.UNDERSCORE, string.Empty), string.Empty, Constant.UNDERSCORE, string.Empty, displayName, Constant.OPENROUNDBRACKET, varassignment, Constant.CLOSEDROUNDBRACKET),
                            UpdatedValue = string.Empty,
                            PreviuosValue = assignment.Value != null ? assignment.Value.ToString() : string.Empty

                        };
                        if (!history.PreviuosValue.Equals(history.UpdatedValue))
                        {
                            historyTable.Add(history);
                        }
                    }
                }
            }
            var result = _groupdl.DeleteGroupHallFixtureConsole(groupId, consoleId, fixtureType, historyTable, userId);
            var response = JArray.FromObject(result);
            var objresult = result.Count > 0 ? result[0].result : 0;
            if (objresult == 1)
            {
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.NOTFOUND,
                    ResponseArray = response
                });
            }

        }

        /// <summary>
        /// This method is used to duplicate group
        /// </summary>
        /// <param Name="GroupId"></param>
        /// <param Name="buildingID"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> DuplicateGroupConfigurationById(List<int> GroupId, int buildingID)
        {

            var methodBeginTime = Utility.LogBegin();
            ResultGroupConfiguration res = new ResultGroupConfiguration();
            List<ResultGroupConfiguration> lstResult = new List<ResultGroupConfiguration>();

            var groupIDDataTable = Utility.GetBuildingIDDataTable(GroupId);
            var result = _groupdl.DuplicateGroupConfigurationById(groupIDDataTable, buildingID);
            if (result != null && result.Tables.Count > 0)
            {
                foreach (DataTable table in result.Tables)
                {
                    if (table.Columns.Contains(Constant.GROUPIDCOLUMNNAME))
                    {
                        var buildingList = (from DataRow row in table.Rows
                                            select new { GroupId = Convert.ToInt32(row[Constant.GROUPIDCOLUMNNAME]), GroupName = Convert.ToString(row[Constant.GROUPNAMECOLUMNNAME]) }).ToList();
                        if (buildingList.Count > 0)
                        {
                            foreach (var building in buildingList)
                            {
                                ResultGroupConfiguration resultBuildingConfiguration = new ResultGroupConfiguration();
                                resultBuildingConfiguration.GroupConfigurationId = building.GroupId;
                                resultBuildingConfiguration.Message = Constant.GROUP + building.GroupName + Constant.CREATEDSUCCESSFULLY;
                                resultBuildingConfiguration.Result = 1;
                                lstResult.Add(resultBuildingConfiguration);
                            }

                        }
                    }
                }
            }
            var responseArray = JArray.FromObject(lstResult);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = responseArray };
        }

        /// <summary>
        /// this method to create group configuration request body
        /// </summary>
        /// <param Name="varibleAssignments"></param>
        /// <returns></returns>
        public List<DisplayVariableAssignmentsValues> DisplayVariablesValuesResponse(JObject varibleAssignments)
        {
            var methodBeginTime = Utility.LogBegin();
            var objLine = new List<DisplayVariableAssignmentsValues>();
            var displayValues = varibleAssignments[Constant.DISPLAYVARIABLEASSIGNMENTS];
            if (displayValues != null)
            {
                objLine = Utility.DeserializeObjectValue<List<DisplayVariableAssignmentsValues>>(varibleAssignments[Constant.DISPLAYVARIABLEASSIGNMENTS]?.ToString());
            }
            Utility.LogEnd(methodBeginTime);
            return objLine;
        }

        /// <summary>
        /// This method is used to get the console Histories for group hall fixture
        /// </summary>
        /// <param Name="cachedConsoleData"></param>
        /// <param Name="unitHallFixtureData"></param>
        /// <param Name="is_HallStation"></param>
        /// <returns></returns>
        private List<ConsoleHistory> GetConsoleHistoriesGroupHallFixture(List<GroupHallFixtures> cachedConsoleData, GroupHallFixturesData unitHallFixtureData, bool isHallStation)
        {
            var methodBeginTime = Utility.LogBegin();
            var lstConsoleHistory = new List<ConsoleHistory>();
            var cachedConsoles = (from console in cachedConsoleData
                                  where console.GroupHallFixtureType != null && Utility.CheckEquals(console.GroupHallFixtureType, unitHallFixtureData.FixtureType) && !console.ConsoleId.Equals(0)
                                  select console).ToList();
            if (unitHallFixtureData.GroupHallFixtureLocations != null && !isHallStation)
            {
                foreach (var entrancelocation in unitHallFixtureData.GroupHallFixtureLocations)
                {
                    foreach (var opening in entrancelocation.Assignments)
                    {
                        var unitDetails = (from console in cachedConsoles
                                           from location in console.UnitDetails
                                           where location.UnitId.Equals(entrancelocation.UnitId)
                                           select location.UniDesgination).ToList();
                        var floorDetails = (from console in cachedConsoles
                                            from location in console.UnitDetails
                                            from units in location.UnitGroupValues
                                            where units.FloorDesignation.Equals(opening.FloorDesignation)
                                            select units.FloorNumber).ToList();
                        var FrontConsole = (from console in cachedConsoles
                                            from location in console.UnitDetails
                                            from units in location.UnitGroupValues
                                            where location.UnitId == entrancelocation.UnitId && units.FloorDesignation == opening.FloorDesignation
                                            && units.Front.Value.Equals(true)
                                            select console).ToList();
                        if (FrontConsole.Count > 0)
                        {
                            FrontConsole[0].VariableAssignments = FrontConsole[0].VariableAssignments.Where(x => !x.VariableId.Equals(String.Empty)).ToList();
                            if (FrontConsole[0].VariableAssignments.Count > 0)
                            {
                                foreach (var variable in FrontConsole[0].VariableAssignments)
                                {
                                    foreach (var cachevariable in unitHallFixtureData.VariableAssignments)
                                    {

                                        if (Utility.CheckEquals(variable.VariableId, cachevariable.VariableId))
                                        {
                                            if (opening.Front)
                                            {
                                                var consolehistory = new ConsoleHistory()
                                                {
                                                    Console = unitHallFixtureData.ConsoleName.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE),
                                                    Parameter = cachevariable.VariableId,
                                                    UnitId = unitDetails.Count > 0 ? unitDetails[0] : string.Empty,
                                                    FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                                    Opening = Constant.F,
                                                    PresentValue = cachevariable.Value != null ? cachevariable.Value.ToString() : string.Empty,
                                                    PreviousValue = variable.Value != null ? variable.Value.ToString() : String.Empty
                                                };
                                                lstConsoleHistory.Add(consolehistory);
                                            }
                                            else if (unitHallFixtureData.GroupHallFixtureConsoleId.Equals(FrontConsole[0].ConsoleId.ToString()))
                                            {
                                                var consolehistory = new ConsoleHistory()
                                                {
                                                    Console = unitHallFixtureData.ConsoleName.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE),
                                                    Parameter = cachevariable.VariableId,
                                                    UnitId = unitDetails.Count > 0 ? unitDetails[0] : string.Empty,
                                                    FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                                    Opening = Constant.F,
                                                    PresentValue = string.Empty,
                                                    PreviousValue = variable.Value != null ? variable.Value.ToString() : String.Empty
                                                };
                                                lstConsoleHistory.Add(consolehistory);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (opening.Front)
                                {

                                    foreach (var variable in unitHallFixtureData.VariableAssignments)
                                    {
                                        var consolehistory = new ConsoleHistory()
                                        {
                                            Console = unitHallFixtureData.ConsoleName.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE),
                                            Parameter = variable.VariableId,
                                            UnitId = unitDetails.Count > 0 ? unitDetails[0] : string.Empty,
                                            FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                            Opening = Constant.F,
                                            PresentValue = variable.Value != null ? variable.Value.ToString() : string.Empty,
                                            PreviousValue = String.Empty
                                        };
                                        lstConsoleHistory.Add(consolehistory);
                                    }

                                }
                            }

                        }
                        else
                        {
                            if (opening.Front)
                            {
                                foreach (var variable in unitHallFixtureData.VariableAssignments)
                                {


                                    var consolehistory = new ConsoleHistory()
                                    {
                                        Console = unitHallFixtureData.ConsoleName,
                                        Parameter = variable.VariableId,
                                        UnitId = unitDetails.Count > 0 ? unitDetails[0] : string.Empty,
                                        FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                        Opening = Constant.F,
                                        PresentValue = variable.Value != null ? variable.Value.ToString() : String.Empty,
                                        PreviousValue = String.Empty
                                    };
                                    lstConsoleHistory.Add(consolehistory);
                                }
                            }
                        }
                        var RearConsole = (from console in cachedConsoles
                                           from location in console.UnitDetails
                                           from units in location.UnitGroupValues
                                           where location.UnitId.Equals(entrancelocation.UnitId) && units.FloorDesignation.Equals(opening.FloorDesignation) && units.Rear.Value.Equals(true)
                                           select console).ToList();
                        if (RearConsole.Count > 0)
                        {
                            RearConsole[0].VariableAssignments = RearConsole[0].VariableAssignments.Where(x => !x.VariableId.Equals(string.Empty)).ToList();
                            if (RearConsole[0].VariableAssignments.Count > 0)
                            {
                                foreach (var variable in RearConsole[0].VariableAssignments)
                                {
                                    foreach (var cachevariable in unitHallFixtureData.VariableAssignments)
                                    {
                                        if (Utility.CheckEquals(variable.VariableId, cachevariable.VariableId))
                                        {

                                            if (opening.Rear)
                                            {
                                                var consolehistory = new ConsoleHistory()
                                                {
                                                    Console = unitHallFixtureData.ConsoleName,
                                                    Parameter = cachevariable.VariableId,
                                                    UnitId = unitDetails.Count > 0 ? unitDetails[0] : string.Empty,
                                                    FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                                    Opening = Constant.R,
                                                    PresentValue = cachevariable.Value != null ? cachevariable.Value.ToString() : string.Empty,
                                                    PreviousValue = variable.Value != null ? variable.Value.ToString() : String.Empty
                                                };
                                                lstConsoleHistory.Add(consolehistory);
                                            }
                                            else if (unitHallFixtureData.GroupHallFixtureConsoleId.Equals(RearConsole[0].ConsoleId))
                                            {
                                                var consolehistory = new ConsoleHistory()
                                                {
                                                    Console = unitHallFixtureData.ConsoleName,
                                                    Parameter = cachevariable.VariableId,
                                                    UnitId = unitDetails.Count > 0 ? unitDetails[0] : string.Empty,
                                                    FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                                    Opening = Constant.R,
                                                    PresentValue = string.Empty,
                                                    PreviousValue = variable.Value != null ? variable.Value.ToString() : String.Empty
                                                };
                                                lstConsoleHistory.Add(consolehistory);
                                            }

                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (var variable in unitHallFixtureData.VariableAssignments)
                                {
                                    if (opening.Rear)
                                    {

                                        var consolehistory = new ConsoleHistory()
                                        {
                                            Console = unitHallFixtureData.ConsoleName,
                                            Parameter = variable.VariableId,
                                            UnitId = unitDetails.Count > 0 ? unitDetails[0] : string.Empty,
                                            FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                            Opening = Constant.R,
                                            PresentValue = variable.Value != null ? variable.Value.ToString() : string.Empty,
                                            PreviousValue = String.Empty
                                        };
                                        lstConsoleHistory.Add(consolehistory);
                                    }
                                }
                            }

                        }
                        else
                        {
                            if (opening.Rear)
                            {
                                foreach (var variable in unitHallFixtureData.VariableAssignments)
                                {

                                    var consolehistory = new ConsoleHistory()
                                    {
                                        Console = unitHallFixtureData.ConsoleName,
                                        Parameter = variable.VariableId,
                                        UnitId = unitDetails.Count > 0 ? unitDetails[0] : string.Empty,
                                        FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                        Opening = Constant.R,
                                        PresentValue = variable.Value != null ? variable.Value.ToString() : String.Empty,
                                        PreviousValue = String.Empty
                                    };
                                    lstConsoleHistory.Add(consolehistory);
                                }
                            }

                        }
                    }

                }
            }
            else if (unitHallFixtureData.GroupHallFixtureLocations != null && isHallStation.Equals(true))
            {
                foreach (var entrancelocation in unitHallFixtureData.GroupHallFixtureLocations)
                {
                    foreach (var opening in entrancelocation.Assignments)
                    {
                        var hallStationDetails = (from console in cachedConsoles
                                                  from location in console.HallStations
                                                  where location.HallStationName.Equals(entrancelocation.HallStationName)
                                                  select location.HallStationName).ToList();
                        var floorDetails = (from console in cachedConsoles
                                            from location in console.HallStations
                                            from units in location.openingsAssigned
                                            where units.FloorDesignation.Equals(opening.FloorDesignation)
                                            select units.FloorNumber).ToList();
                        var FrontConsole = (from console in cachedConsoles
                                            from location in console.HallStations
                                            from units in location.openingsAssigned
                                            where location.HallStationName == entrancelocation.HallStationName && units.FloorDesignation == opening.FloorDesignation
                                            && units.Front != null && units.Front.Value.Equals(true)
                                            select console).ToList();
                        if (FrontConsole.Count > 0)
                        {
                            FrontConsole[0].VariableAssignments = FrontConsole[0].VariableAssignments.Where(x => !x.VariableId.Equals(string.Empty)).ToList();
                            if (FrontConsole[0].VariableAssignments.Count > 0)
                            {
                                foreach (var variable in FrontConsole[0].VariableAssignments)
                                {
                                    foreach (var cachevariable in unitHallFixtureData.VariableAssignments)
                                    {

                                        if (Utility.CheckEquals(variable.VariableId, cachevariable.VariableId))
                                        {
                                            if (opening.Front)
                                            {
                                                var consolehistory = new ConsoleHistory()
                                                {
                                                    Console = unitHallFixtureData.ConsoleName.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE),
                                                    Parameter = cachevariable.VariableId,
                                                    HallStationName = hallStationDetails.Count > 0 ? hallStationDetails[0] : string.Empty,
                                                    FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                                    Opening = Constant.F,
                                                    PresentValue = cachevariable.Value != null ? cachevariable.Value.ToString() : string.Empty,
                                                    PreviousValue = variable.Value != null ? variable.Value.ToString() : String.Empty
                                                };
                                                lstConsoleHistory.Add(consolehistory);
                                            }
                                            else if (unitHallFixtureData.GroupHallFixtureConsoleId.Equals(FrontConsole[0].ConsoleId.ToString()))
                                            {
                                                var consolehistory = new ConsoleHistory()
                                                {
                                                    Console = unitHallFixtureData.ConsoleName.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE),
                                                    Parameter = cachevariable.VariableId,
                                                    HallStationName = hallStationDetails.Count > 0 ? hallStationDetails[0] : string.Empty,
                                                    FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                                    Opening = Constant.F,
                                                    PresentValue = string.Empty,
                                                    PreviousValue = variable.Value != null ? variable.Value.ToString() : String.Empty
                                                };
                                                lstConsoleHistory.Add(consolehistory);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (opening.Front)
                                {

                                    foreach (var variable in unitHallFixtureData.VariableAssignments)
                                    {
                                        var consolehistory = new ConsoleHistory()
                                        {
                                            Console = unitHallFixtureData.ConsoleName.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE),
                                            Parameter = variable.VariableId,
                                            HallStationName = hallStationDetails.Count > 0 ? hallStationDetails[0] : string.Empty,
                                            FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                            Opening = Constant.F,
                                            PresentValue = variable.Value != null ? variable.Value.ToString() : string.Empty,
                                            PreviousValue = String.Empty
                                        };
                                        lstConsoleHistory.Add(consolehistory);
                                    }

                                }
                            }

                        }
                        else
                        {
                            if (opening.Front)
                            {
                                foreach (var variable in unitHallFixtureData.VariableAssignments)
                                {


                                    var consolehistory = new ConsoleHistory()
                                    {
                                        Console = unitHallFixtureData.ConsoleName,
                                        Parameter = variable.VariableId,
                                        HallStationName = hallStationDetails.Count > 0 ? hallStationDetails[0] : string.Empty,
                                        FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                        Opening = Constant.F,
                                        PresentValue = variable.Value != null ? variable.Value.ToString() : String.Empty,
                                        PreviousValue = String.Empty
                                    };
                                    lstConsoleHistory.Add(consolehistory);
                                }
                            }
                        }
                        var RearConsole = (from console in cachedConsoles
                                           from location in console.UnitDetails
                                           from units in location.UnitGroupValues
                                           where location.UnitId.Equals(entrancelocation.UnitId) && units.FloorDesignation.Equals(opening.FloorDesignation) && units.Rear.Value.Equals(true)
                                           select console).ToList();
                        if (RearConsole.Count > 0)
                        {
                            RearConsole[0].VariableAssignments = RearConsole[0].VariableAssignments.Where(x => !x.VariableId.Equals(string.Empty)).ToList();
                            if (RearConsole[0].VariableAssignments.Count > 0)
                            {
                                foreach (var variable in RearConsole[0].VariableAssignments)
                                {
                                    foreach (var cachevariable in unitHallFixtureData.VariableAssignments)
                                    {
                                        if (Utility.CheckEquals(variable.VariableId, cachevariable.VariableId))
                                        {

                                            if (opening.Rear)
                                            {
                                                var consolehistory = new ConsoleHistory()
                                                {
                                                    Console = unitHallFixtureData.ConsoleName,
                                                    Parameter = cachevariable.VariableId,
                                                    HallStationName = hallStationDetails.Count > 0 ? hallStationDetails[0] : string.Empty,
                                                    FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                                    Opening = Constant.R,
                                                    PresentValue = cachevariable.Value != null ? cachevariable.Value.ToString() : string.Empty,
                                                    PreviousValue = variable.Value != null ? variable.Value.ToString() : String.Empty
                                                };
                                                lstConsoleHistory.Add(consolehistory);
                                            }
                                            else if (unitHallFixtureData.GroupHallFixtureConsoleId.Equals(RearConsole[0].ConsoleId))
                                            {
                                                var consolehistory = new ConsoleHistory()
                                                {
                                                    Console = unitHallFixtureData.ConsoleName,
                                                    Parameter = cachevariable.VariableId,
                                                    HallStationName = hallStationDetails.Count > 0 ? hallStationDetails[0] : string.Empty,
                                                    FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                                    Opening = Constant.R,
                                                    PresentValue = string.Empty,
                                                    PreviousValue = variable.Value != null ? variable.Value.ToString() : String.Empty
                                                };
                                                lstConsoleHistory.Add(consolehistory);
                                            }

                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (var variable in unitHallFixtureData.VariableAssignments)
                                {
                                    if (opening.Rear)
                                    {

                                        var consolehistory = new ConsoleHistory()
                                        {
                                            Console = unitHallFixtureData.ConsoleName,
                                            Parameter = variable.VariableId,
                                            HallStationName = hallStationDetails.Count > 0 ? hallStationDetails[0] : string.Empty,
                                            FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                            Opening = Constant.R,
                                            PresentValue = variable.Value != null ? variable.Value.ToString() : string.Empty,
                                            PreviousValue = String.Empty
                                        };
                                        lstConsoleHistory.Add(consolehistory);
                                    }
                                }
                            }

                        }
                        else
                        {
                            if (opening.Rear)
                            {
                                foreach (var variable in unitHallFixtureData.VariableAssignments)
                                {

                                    var consolehistory = new ConsoleHistory()
                                    {
                                        Console = unitHallFixtureData.ConsoleName,
                                        Parameter = variable.VariableId,
                                        HallStationName = hallStationDetails.Count > 0 ? hallStationDetails[0] : string.Empty,
                                        FloorNumber = floorDetails.Count > 0 ? floorDetails[0] : 0,
                                        Opening = Constant.R,
                                        PresentValue = variable.Value != null ? variable.Value.ToString() : String.Empty,
                                        PreviousValue = String.Empty
                                    };
                                    lstConsoleHistory.Add(consolehistory);
                                }
                            }

                        }
                    }

                }
            }
            Utility.LogEnd(methodBeginTime);
            return lstConsoleHistory;

        }

        /// <summary>
        /// GetLogHistoryTableForConsole
        /// </summary>
        /// <param Name="lstConsoleHistory"></param>
        /// <returns></returns>
        public List<LogHistoryTable> GetLogHistoryTableForConsole(List<ConsoleHistory> lstConsoleHistory)
        {
            var methodBeginTime = Utility.LogBegin();
            List<LogHistoryTable> logHistoryTable = new List<LogHistoryTable>();
            var enrichedData = JObject.Parse(File.ReadAllText(Constants.ELEVATORENRICHMENTTEMPLATE));
            var enrichedDataVariables = enrichedData[Constant.VARIABLES];
            var isHallStation=false;
            if (lstConsoleHistory != null && lstConsoleHistory.Any())
            {
                if (!lstConsoleHistory.FirstOrDefault().Console.Contains(Constant.AGILEHALLSTATIONS)&& !lstConsoleHistory.FirstOrDefault().Console.Contains(Constants.OPENINGLOCATIONPASCALCASE))
                {
                    isHallStation = true;
                }
                else
                {
                    isHallStation = false;
                }
                foreach (var consolehistory in lstConsoleHistory)
                {
                    var lstSameConsoleHistory = (from x in lstConsoleHistory
                                                 where (consolehistory.Parameter.Equals(x.Parameter) || Utility.CheckEquals(consolehistory.Parameter, x.Parameter)) && (consolehistory.PresentValue.Equals(x.PresentValue)
                                                 || Utility.CheckEquals(consolehistory.PresentValue, x.PresentValue)) && (consolehistory.PreviousValue.Equals(x.PreviousValue)
                                                 || Utility.CheckEquals(consolehistory.PreviousValue, x.PreviousValue))
                                                 select x).ToList();
                    if (lstSameConsoleHistory.Count > 0 && isHallStation.Equals(false))
                    {

                        List<UnitDetailsValues> lstUnitDetails = new List<UnitDetailsValues>();
                        foreach (var sameConsoleHistory in lstSameConsoleHistory)
                        {
                            var unitDetails = new UnitDetailsValues()
                            {
                                UniDesgination = sameConsoleHistory.UnitId,
                                UnitGroupValues = new List<GroupHallFixtureLocations>()
                            };
                            var lstSameUnit = lstSameConsoleHistory.Where(x => x.UnitId.Equals(sameConsoleHistory.UnitId)).ToList();
                            List<GroupHallFixtureLocations> lstfixtureAssignment = new List<GroupHallFixtureLocations>();
                            foreach (var sameunit in lstSameUnit)
                            {
                                var entranceLocation = new GroupHallFixtureLocations()
                                {
                                    FloorNumber = sameunit.FloorNumber,
                                    UnitDesignation = sameunit.UnitId
                                };
                                var lstsameFloor = lstSameUnit.Where(x => x.FloorNumber.Equals(sameunit.FloorNumber)).ToList();
                                var Front = new LandingOpening()
                                {
                                    Value = lstsameFloor.Where(x => x.Opening.Equals(Constant.F)).ToList().Count > 0
                                };
                                var Rear = new LandingOpening()
                                {
                                    Value = lstsameFloor.Where(x => x.Opening.Equals(Constant.R)).ToList().Count > 0
                                };
                                entranceLocation.Front = Front;
                                entranceLocation.Rear = Rear;
                                lstfixtureAssignment.Add(entranceLocation);
                                lstSameUnit = lstSameUnit.Where(x => !x.FloorNumber.Equals(sameunit.FloorNumber)).ToList();

                            }
                            unitDetails.UnitGroupValues.AddRange(lstfixtureAssignment);

                            lstUnitDetails.Add(unitDetails);
                            lstSameConsoleHistory = lstSameConsoleHistory.Where(x => !x.UnitId.Equals(sameConsoleHistory.UnitId)).ToList();
                        }
                        lstConsoleHistory = (from x in lstConsoleHistory
                                             where !((consolehistory.Parameter.Equals(x.Parameter) || Utility.CheckEquals(consolehistory.Parameter, x.Parameter)) && (consolehistory.PresentValue.Equals(x.PresentValue) || Utility.CheckEquals(consolehistory.PresentValue, x.PresentValue)) && (consolehistory.PreviousValue.Equals(x.PreviousValue) || Utility.CheckEquals(consolehistory.PreviousValue, x.PreviousValue)))
                                             select x).ToList();
                        lstUnitDetails = (from x in lstUnitDetails
                                          where x.UnitGroupValues.Count > 0
                                          select x).ToList();
                        var frontRearAssignment = Utility.GetLandingOpeningAssignmentSelectedForGroupHallFixture(lstUnitDetails);
                        var needVariables = Utility.GetTokens(consolehistory.Parameter, enrichedDataVariables, false);
                        var displayName = string.Empty;
                        if (needVariables != null && needVariables.Any())
                        {
                            var currentPropertyCollection = needVariables.Select(x => (JProperty)x).Where(x => x.Name == Constant.PROPERTIES).Select(x => x.Value).FirstOrDefault();
                            displayName = currentPropertyCollection.Children().Where(x => x.Children<JProperty>().Any(y => y.Name == Constant.IDPARAM && y.Value.ToString() == Constant.DISPLAYNAME)).SelectMany(x => x.Children<JProperty>().Select(y => y.Name == Constant.VALUE ? y.Value.ToString() : "")).FirstOrDefault(x => !string.IsNullOrEmpty(x));

                        }


                        var loghistory = new LogHistoryTable()
                        {
                            VariableId = consolehistory.Console + Constant.HYPHENWITHSPACE + displayName + Constant.OPENROUNDBRACKET + frontRearAssignment + Constant.CLOSEDROUNDBRACKET,
                            PreviuosValue = consolehistory.PreviousValue,
                            UpdatedValue = consolehistory.PresentValue
                        };
                        logHistoryTable.Add(loghistory);
                    }
                    else if (lstSameConsoleHistory.Count > 0 && isHallStation.Equals(true))
                    {

                        List<UnitDetailsValues> lstUnitDetails = new List<UnitDetailsValues>();
                        List<HallStations> lstHallStationDetails = new List<HallStations>();
                        foreach (var sameConsoleHistory in lstSameConsoleHistory)
                        {
                            var hallStationDetails = new HallStations()
                            {
                                HallStationName = sameConsoleHistory.HallStationName,
                                openingsAssigned = new List<GroupHallFixtureLocations>()
                            };
                            var lstSameUnit = lstSameConsoleHistory.Where(x => x.HallStationName.Equals(sameConsoleHistory.HallStationName)).ToList();
                            List<GroupHallFixtureLocations> lstfixtureAssignment = new List<GroupHallFixtureLocations>();
                            foreach (var sameunit in lstSameUnit)
                            {
                                var entranceLocation = new GroupHallFixtureLocations()
                                {
                                    FloorNumber = sameunit.FloorNumber,
                                    HallStationName = sameunit.HallStationName
                                };
                                var lstsameFloor = lstSameUnit.Where(x => x.FloorNumber.Equals(sameunit.FloorNumber)).ToList();
                                var Front = new LandingOpening()
                                {
                                    Value = lstsameFloor.Where(x => x.Opening.Equals(Constant.F)).ToList().Count > 0
                                };
                                var Rear = new LandingOpening()
                                {
                                    Value = lstsameFloor.Where(x => x.Opening.Equals(Constant.R)).ToList().Count > 0
                                };
                                entranceLocation.Front = Front;
                                entranceLocation.Rear = Rear;
                                lstfixtureAssignment.Add(entranceLocation);
                                lstSameUnit = lstSameUnit.Where(x => !x.FloorNumber.Equals(sameunit.FloorNumber)).ToList();

                            }
                            hallStationDetails.openingsAssigned.AddRange(lstfixtureAssignment);

                            lstHallStationDetails.Add(hallStationDetails);
                            lstSameConsoleHistory = lstSameConsoleHistory.Where(x => !x.HallStationName.Equals(sameConsoleHistory.HallStationName)).ToList();
                        }
                        lstConsoleHistory = (from x in lstConsoleHistory
                                             where !((consolehistory.Parameter.Equals(x.Parameter) || Utility.CheckEquals(consolehistory.Parameter, x.Parameter)) && (consolehistory.PresentValue.Equals(x.PresentValue) || Utility.CheckEquals(consolehistory.PresentValue, x.PresentValue)) && (consolehistory.PreviousValue.Equals(x.PreviousValue) || Utility.CheckEquals(consolehistory.PreviousValue, x.PreviousValue)))
                                             select x).ToList();
                        lstHallStationDetails = (from x in lstHallStationDetails
                                                 where x.openingsAssigned.Count > 0
                                                 select x).ToList();
                        var frontRearAssignment = Utility.GetOpeningForHallStation(lstHallStationDetails);
                        var needVariables = Utility.GetTokens(consolehistory.Parameter, enrichedDataVariables, false);
                        var displayName = string.Empty;
                        if (needVariables != null && needVariables.Any())
                        {
                            var currentPropertyCollection = needVariables.Select(x => (JProperty)x).Where(x => x.Name == Constant.PROPERTIES).Select(x => x.Value).FirstOrDefault();
                            displayName = currentPropertyCollection.Children().Where(x => x.Children<JProperty>().Any(y => y.Name == Constant.IDPARAM && y.Value.ToString() == Constant.DISPLAYNAME)).SelectMany(x => x.Children<JProperty>().Select(y => y.Name == Constant.VALUE ? y.Value.ToString() : "")).FirstOrDefault(x => !string.IsNullOrEmpty(x));

                        }
                        var loghistory = new LogHistoryTable()
                        {
                            VariableId = consolehistory.Console + Constant.HYPHENWITHSPACE + displayName + Constant.OPENROUNDBRACKET + frontRearAssignment + Constant.CLOSEDROUNDBRACKET,
                            PreviuosValue = consolehistory.PreviousValue,
                            UpdatedValue = consolehistory.PresentValue
                        };
                        logHistoryTable.Add(loghistory);
                    }

                }
            }
            Utility.LogEnd(methodBeginTime);
            return logHistoryTable;

        }

        /// <summary>
        /// Constructor for GetBuildingConfigurationSectionTab
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetGroupConfigurationSectionTab(int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            Dictionary<string, bool> isDisabled = _groupdl.GetGroupConfigurationSectionTab(groupId, null);
            var stubMainSubSectionResponse = JObject.Parse(File.ReadAllText(Constant.GROUPCONFIGURATIONTABSTUBPATH));
            var stubUnitConfigurationMainResponseObj = stubMainSubSectionResponse.ToObject<GroupTab>();
            stubUnitConfigurationMainResponseObj.Sections.Where(c => c.Id == Constant.GROUPHALLFIXTURELOWERCASE).FirstOrDefault().IsDisabled = isDisabled[Constant.GROUPHALLFIXTURES];
            stubUnitConfigurationMainResponseObj.Sections.Where(c => c.Id == Constant.OPENINGLOCATIONLOWERCASE).FirstOrDefault().IsDisabled = isDisabled[Constant.OPENINGLOCATIONS];

            var response = JObject.FromObject(stubUnitConfigurationMainResponseObj);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = response };

        }

        private List<int> GetFrontSelectedFloors(List<GroupHallFixtures> groupHallFixtureConsoles, int consoleId)
        {
            List<int> frontSelected = new List<int>();
            foreach (var console in groupHallFixtureConsoles)
            {
                if (console.ConsoleId != consoleId && (console.GroupHallFixtureType.Equals(Constant.AGILEHALLSTATION) ||
                    console.GroupHallFixtureType.Equals(Constant.TRADITIONALHALLSTATION)))
                {
                    if (console.HallStations != null)
                    {
                        foreach (var hallStation in console.HallStations)
                        {
                            foreach (var location in hallStation.openingsAssigned)
                            {
                                if (location.Front != null && location.Front.Value.Equals(true))
                                {
                                    frontSelected.Add(location.FloorNumber);
                                }
                            }
                        }
                    }
                }
            }
            return frontSelected;
        }

        private List<int> GetRearSelectedFloors(List<GroupHallFixtures> groupHallFixtureConsoles, int consoleId)
        {
            List<int> rearSelected = new List<int>();
            foreach (var console in groupHallFixtureConsoles)
            {
                if (console.ConsoleId != consoleId && (console.GroupHallFixtureType.Equals(Constant.AGILEHALLSTATION) ||
                    console.GroupHallFixtureType.Equals(Constant.TRADITIONALHALLSTATION)))
                {
                    if (console.HallStations != null)
                    {
                        foreach (var hallStation in console.HallStations)
                        {
                            foreach (var location in hallStation.openingsAssigned)
                            {
                                if (location.Rear != null && location.Rear.Value.Equals(true))
                                {
                                    rearSelected.Add(location.FloorNumber);
                                }
                            }
                        }
                    }
                }
            }
            return rearSelected;
        }

        private void CreateNewGroupHallFixtureConsole(List<GroupHallFixtures> groupHallFixtureConsoles, string fixtureType, int consoleId, ref GroupHallFixtures newConsole, List<List<UnitDetailsValues>> lstSelectedFrontFloorNumber, string unitDetails, string SessionId, List<HallStations> HallStationLocation, int groupId, string fixtureStrategy, string userId)
        {

            var maxConsoleId = (from console in groupHallFixtureConsoles
                                where console.GroupHallFixtureType == fixtureType
                                select console.ConsoleId).ToList().Count();
            var groupConsole = groupHallFixtureConsoles.Where(x => x.ConsoleId.Equals(consoleId) && x.GroupHallFixtureType.Equals(fixtureType)).ToList();
            var isController = false;
            groupHallFixtureConsoles.Add(newConsole);
            newConsole = new GroupHallFixtures();
            newConsole.GroupHallFixtureType = fixtureType;
            var groupHallFixtureConsole = (from consoles in groupHallFixtureConsoles
                                           where consoles.ConsoleId == consoleId && consoles.GroupHallFixtureType == fixtureType
                                           select consoles).ToList();

            if (groupHallFixtureConsole != null && groupHallFixtureConsole.Any())
            {
                var newGroupLocation = Utility.DeserializeObjectValue<List<UnitDetailsValues>>(unitDetails);
                if (consoleId == 0)
                {
                    var firePhoneJack = -1;
                    if (fixtureType.Equals(Constants.FIREPHONEJACKCONSOLE))
                    {
                        firePhoneJack = (from consoles in groupHallFixtureConsoles
                                         where consoles.ConsoleId > 0 && consoles.GroupHallFixtureType == fixtureType
                                         select consoles).ToList().Count();
                    }

                    var gHFDefaults = JObject.Parse(File.ReadAllText(Constants.GROUPHALLFIXTURECONSOLEDEFAULTVALUES)).ToString();
                    var defaultGHFVariables = Utility.DeserializeObjectValue<Dictionary<string, Dictionary<string, string>>>(gHFDefaults);
                    var defaultVariablesDict = (from consoleTypes in defaultGHFVariables
                                                where consoleTypes.Key != null && consoleTypes.Key.Equals(groupHallFixtureConsole[0].GroupHallFixtureType)
                                                select consoleTypes.Value).FirstOrDefault();
                    List<ConfigVariable> defaultVarAssignment = new List<ConfigVariable>();
                    if (defaultVariablesDict != null)
                    {
                        foreach (var dictKey in defaultVariablesDict.Keys)
                        {
                            defaultVarAssignment.Add(new ConfigVariable { VariableId = dictKey, Value = defaultVariablesDict[dictKey] });
                        }
                    }
                    var fixtureName = fixtureType.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE);
                    fixtureName = fixtureName + Constant.EMPTYSPACE;
                    newConsole.ConsoleId = maxConsoleId + 1;
                    newConsole.ConsoleName = newConsole.ConsoleId == 1 ? fixtureName : fixtureName + newConsole.ConsoleId;
                    newConsole.AssignOpenings = true;
                    newConsole.IsController = false;
                    newConsole.Openings = groupHallFixtureConsole[0].Openings;
                    newConsole.NoOfFloor = groupHallFixtureConsole[0].NoOfFloor;
                    newConsole.VariableAssignments = defaultVarAssignment;
                    newConsole.GroupHallFixtureType = groupHallFixtureConsole[0].GroupHallFixtureType;

                    Dictionary<string, List<int>> frontSelected = new Dictionary<string, List<int>>();
                    Dictionary<string, List<int>> rearSelected = new Dictionary<string, List<int>>();
                    if (fixtureType.Equals(Constants.HALLCALLREGISTRATION) || fixtureType.Equals(Constants.HALLCALLLOCKOUT))
                    {
                        SetIncompatiblePropertyForMutuallyExclusiveConsoles(groupHallFixtureConsoles, fixtureType, consoleId, frontSelected, rearSelected);
                    }
                    else if (fixtureStrategy == Constants.ETA_AND_ETD && (fixtureType.Equals(Constants.AGILEHALLSTATION) || fixtureType == Constants.TRADITIONALHALLSTATION))
                    {
                        SetIncompatiblePropertyForMutuallyExclusiveDefaultConsoles(groupHallFixtureConsoles, fixtureType, consoleId, frontSelected, rearSelected);
                    }
                    else
                    {
                        foreach (var console in groupHallFixtureConsoles)
                        {
                            if (console.ConsoleId != consoleId && console.GroupHallFixtureType.Equals(fixtureType) && !console.GroupHallFixtureType.Equals(Constants.AGILEHALLSTATION))
                            {
                                foreach (var hallStation in console.HallStations)
                                {
                                    if (hallStation.HallStationId.Contains(Constants.FRONTHALLSTATION))
                                    {
                                        if (!frontSelected.ContainsKey(hallStation.HallStationId))
                                        {
                                            frontSelected.Add(hallStation.HallStationId, new List<int>());
                                        }
                                        foreach (var opening in hallStation.openingsAssigned)
                                        {
                                            if (opening.Front.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase))
                                            {
                                                frontSelected[hallStation.HallStationId].Add(opening.FloorNumber);
                                            }

                                        }
                                    }
                                    if (hallStation.HallStationId.Contains(Constants.REARHALLSTATION))
                                    {
                                        if (!rearSelected.ContainsKey(hallStation.HallStationId))
                                        {
                                            rearSelected.Add(hallStation.HallStationId, new List<int>());
                                        }
                                        foreach (var opening in hallStation.openingsAssigned)
                                        {
                                            if (opening.Rear.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase))
                                            {
                                                rearSelected[hallStation.HallStationId].Add(opening.FloorNumber);
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (Utility.Equals(fixtureType, Constant.FIRESERVICE) || Utility.Equals(fixtureType, Constant.EMERGENCYPOWER) || Utility.Equals(fixtureType, Constants.ELEVATORCOMMUNICATIONFAILUREJEWELCONSOLE))
                    {
                        var mainEgress = newGroupLocation[0].MainEgress;
                        var alternateEgress = newGroupLocation[0].AlternateEgress;
                        if (!Utility.Equals(fixtureType, Constants.ELEVATORCOMMUNICATIONFAILUREJEWELCONSOLE))
                        {
                            HallStationLocation = (from consoles in groupHallFixtureConsoles
                                                   where consoles.GroupHallFixtureType.Equals(fixtureType)
                                                   select consoles).ToList().FirstOrDefault().HallStations;
                        }

                        foreach (var HallStation in HallStationLocation)
                        {
                            foreach (var opening in HallStation.openingsAssigned)
                            {
                                if (opening.Front != null)
                                {
                                    opening.Front.NotAvailable = !(opening.FloorDesignation == alternateEgress);
                                    if (Utility.Equals(fixtureType, Constants.ELEVATORCOMMUNICATIONFAILUREJEWELCONSOLE) && opening.FloorDesignation == alternateEgress)
                                    {
                                        opening.Front.NotAvailable = true;
                                    }
                                    else if (Utility.Equals(fixtureType, Constants.ELEVATORCOMMUNICATIONFAILUREJEWELCONSOLE) && opening.FloorDesignation == mainEgress)
                                    {
                                        opening.Front.NotAvailable = false;
                                    }
                                }
                                if (opening.Rear != null)
                                {
                                    opening.Rear.NotAvailable = !(opening.FloorDesignation == mainEgress || opening.FloorDesignation == alternateEgress);
                                    if (Utility.Equals(fixtureType, Constants.ELEVATORCOMMUNICATIONFAILUREJEWELCONSOLE) && opening.FloorDesignation == alternateEgress)
                                    {
                                        opening.Rear.NotAvailable = true;
                                    }
                                    else if (Utility.Equals(fixtureType, Constants.ELEVATORCOMMUNICATIONFAILUREJEWELCONSOLE) && opening.FloorDesignation == mainEgress)
                                    {
                                        opening.Rear.NotAvailable = false;
                                    }
                                }
                            }
                        }
                    }

                    if (fixtureType != Constants.AGILEHALLSTATION)
                    {
                        foreach (var hallstation in HallStationLocation)
                        {
                            foreach (var station in hallstation.openingsAssigned)
                            {
                                if (station.Front != null)
                                {
                                    var values = (from keysValues in frontSelected.Keys
                                                  where String.Equals(keysValues, hallstation.HallStationId, StringComparison.OrdinalIgnoreCase)
                                                  select frontSelected[keysValues]).ToList().FirstOrDefault();
                                    station.Front.InCompatible = values != null ? values.Contains(station.FloorNumber) ? true : false : false;
                                    station.Front.Value = station.Front.NotAvailable == false && (firePhoneJack == 0);
                                }
                                if (station.Rear != null)
                                {
                                    var values = (from keysValues in rearSelected.Keys
                                                  where String.Equals(keysValues, hallstation.HallStationId, StringComparison.OrdinalIgnoreCase)
                                                  select rearSelected[keysValues]).ToList().FirstOrDefault();
                                    station.Rear.InCompatible = values != null ? values.Contains(station.FloorNumber) ? true : false : false;
                                    station.Rear.Value = station.Rear.NotAvailable == false && (firePhoneJack == 0);
                                }
                            }
                        }
                    }

                    newConsole.FixtureAssignments = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(HallStationLocation.Distinct()));
                    if (fixtureType.Equals(Constants.AGILEHALLSTATION))
                    {
                        var agileData = (from x in groupHallFixtureConsoles
                                         where x.GroupHallFixtureType.Equals(Constants.AGILEHALLSTATION)
                                         select x).FirstOrDefault();
                        var frontFloors = (from x in groupHallFixtureConsoles
                                           where x.GroupHallFixtureType.Equals(Constants.AGILEHALLSTATION) && x.ConsoleId != consoleId
                                           from opening in x.GroupHallFixtureLocations
                                           where opening.Front.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                           select opening.FloorNumber).ToList();
                        var rearFloors = (from x in groupHallFixtureConsoles
                                          where x.GroupHallFixtureType.Equals(Constants.AGILEHALLSTATION) && x.ConsoleId != consoleId
                                          from opening in x.GroupHallFixtureLocations
                                          where opening.Rear.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                          select opening.FloorNumber).ToList();
                        foreach (var location in agileData.GroupHallFixtureLocations)
                        {
                            if (frontSelected.Values.SelectMany(x => x).ToList().Contains(location.FloorNumber) || frontFloors.Contains(location.FloorNumber))
                            {
                                location.Front.InCompatible = true;
                            }
                            if (rearSelected.Values.SelectMany(x => x).ToList().Contains(location.FloorNumber) || rearFloors.Contains(location.FloorNumber))
                            {
                                location.Rear.InCompatible = true;
                            }
                        }
                        var detailsList = new List<HallStations>();
                        agileData.GroupHallFixtureLocations.ForEach(x => x.Front.Value = false) ;
                        agileData.GroupHallFixtureLocations.ForEach(x => x.Rear.Value = false);
                        var details = new HallStations
                        {
                            HallStationId = Constants.ZERO,
                            HallStationName = Constants.LANDINGS,
                            NoOfFloors = agileData.GroupHallFixtureLocations.Count(),
                            openingDoors = new OpeningDoors
                            {
                                Front = agileData.Openings.Front,
                                Rear = agileData.Openings.Rear
                            },
                            openingsAssigned = (from x in agileData.GroupHallFixtureLocations orderby Convert.ToInt32(x.FloorNumber) select x).ToList()

                        };
                        detailsList.Add(details);
                        newConsole.FixtureAssignments = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(detailsList));
                    }
                    newConsole.UnitDetails = new List<UnitDetailsValues>();


                    newConsole.HallStations = HallStationLocation;
                    newConsole.HallStations = (from hallName in newConsole.HallStations orderby hallName.HallStationName select hallName).ToList();
                    groupHallFixtureConsoles.Add(newConsole);
                    groupHallFixtureConsoles = _configure.SetCacheGroupHallFixtureConsoles(groupHallFixtureConsoles, SessionId, groupId);
                }
                else
                {

                    var consoleList = (from hallfixture in groupHallFixtureConsole where hallfixture.ConsoleId == consoleId select hallfixture).ToList();
                    consoleList = (from console in consoleList where console.GroupHallFixtureType == fixtureType select console).ToList();
                    newConsole = consoleList[0];



                    newGroupLocation = Utility.DeserializeObjectValue<List<UnitDetailsValues>>(unitDetails);


                    if (Utility.Equals(fixtureType, Constant.FIRESERVICE) || Utility.Equals(fixtureType, Constant.EMERGENCYPOWER) || Utility.Equals(fixtureType, Constants.ELEVATORCOMMUNICATIONFAILUREJEWELCONSOLE))
                    {
                        HallStationLocation = (from consoles in groupHallFixtureConsoles
                                               where consoles.ConsoleId.Equals(consoleId) &&
                                               consoles.GroupHallFixtureType.Equals(fixtureType)
                                               select consoles.HallStations).FirstOrDefault();

                        var mainEgress = newGroupLocation[0].MainEgress;
                        var alternateEgress = newGroupLocation[0].AlternateEgress;
                        foreach (var floor in HallStationLocation)
                        {
                            foreach (var opening in floor.openingsAssigned)
                            {
                                if (opening.Front != null)
                                {
                                    opening.Front.NotAvailable = !(opening.FloorDesignation == alternateEgress);
                                    if (Utility.Equals(fixtureType, Constants.ELEVATORCOMMUNICATIONFAILUREJEWELCONSOLE) && opening.FloorDesignation == alternateEgress)
                                    {
                                        opening.Front.NotAvailable = true;
                                    }
                                    else if (Utility.Equals(fixtureType, Constants.ELEVATORCOMMUNICATIONFAILUREJEWELCONSOLE) && opening.FloorDesignation == mainEgress)
                                    {
                                        opening.Front.NotAvailable = false;
                                    }
                                }

                                if (opening.Rear != null)
                                {
                                    opening.Rear.NotAvailable = !(opening.FloorDesignation == alternateEgress);
                                    if (Utility.Equals(fixtureType, Constants.ELEVATORCOMMUNICATIONFAILUREJEWELCONSOLE) && opening.FloorDesignation == alternateEgress)
                                    {
                                        opening.Rear.NotAvailable = true;
                                    }
                                    else if (Utility.Equals(fixtureType, Constants.ELEVATORCOMMUNICATIONFAILUREJEWELCONSOLE) && opening.FloorDesignation == mainEgress)
                                    {
                                        opening.Rear.NotAvailable = false;
                                    }
                                }

                            }
                        }
                    }

                    foreach (var unit in newConsole.UnitDetails)
                    {
                        unit.openingsAssigned = (Utility.DeserializeObjectValue<List<Opening>>(Utility.SerializeObjectValue(unit.UnitGroupValues)));
                    }
                    newConsole.ConsoleId = consoleId;
                    newConsole.AssignOpenings = true;
                    if (consoleId > 1)
                    {
                        newConsole.IsController = false;
                    }
                    HallStationLocation = (from consoles in groupHallFixtureConsoles
                                           where consoles.ConsoleId.Equals(consoleId) && consoles.GroupHallFixtureType.Equals(fixtureType)
                                           select consoles.HallStations).FirstOrDefault();

                    Dictionary<string, List<int>> frontSelected = new Dictionary<string, List<int>>();
                    Dictionary<string, List<int>> rearSelected = new Dictionary<string, List<int>>();
                    if (fixtureType.Equals(Constants.HALLCALLREGISTRATION) || fixtureType.Equals(Constants.HALLCALLLOCKOUT))
                    {
                        SetIncompatiblePropertyForMutuallyExclusiveConsoles(groupHallFixtureConsoles, fixtureType, consoleId, frontSelected, rearSelected);
                    }
                    else if (fixtureStrategy.Equals(Constants.ETA_AND_ETD) && (fixtureType.Equals(Constants.AGILEHALLSTATION) || fixtureType.Equals(Constants.TRADITIONALHALLSTATION)))
                    {
                        SetIncompatiblePropertyForMutuallyExclusiveDefaultConsoles(groupHallFixtureConsoles, fixtureType, consoleId, frontSelected, rearSelected);
                    }
                    else
                    {
                        if (fixtureType != Constants.AGILEHALLSTATION)
                        {
                            foreach (var console in groupHallFixtureConsoles)
                            {
                                if (console.ConsoleId != consoleId && console.GroupHallFixtureType.Equals(fixtureType) && console.HallStations != null)
                                {
                                    foreach (var hallStation in console.HallStations)
                                    {
                                        if (hallStation.HallStationId.Contains(Constants.FRONTHALLSTATION))
                                        {
                                            if (!frontSelected.ContainsKey(hallStation.HallStationId))
                                            {
                                                frontSelected.Add(hallStation.HallStationId, new List<int>());
                                            }
                                            foreach (var opening in hallStation.openingsAssigned)
                                            {
                                                if (opening.Front.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    frontSelected[hallStation.HallStationId].Add(opening.FloorNumber);
                                                }

                                            }
                                        }
                                        if (hallStation.HallStationId.Contains(Constants.REARHALLSTATION))
                                        {
                                            if (!rearSelected.ContainsKey(hallStation.HallStationId))
                                            {
                                                rearSelected.Add(hallStation.HallStationId, new List<int>());
                                            }
                                            foreach (var opening in hallStation.openingsAssigned)
                                            {
                                                if (opening.Rear.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    rearSelected[hallStation.HallStationId].Add(opening.FloorNumber);
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                            foreach (var hallStations in HallStationLocation)
                            {
                                foreach (var location in hallStations.openingsAssigned)
                                {
                                    if (location.Front != null)
                                    {
                                        var values = (from keysValues in frontSelected.Keys
                                                      where String.Equals(keysValues, hallStations.HallStationId, StringComparison.OrdinalIgnoreCase)
                                                      select frontSelected[keysValues]).ToList().FirstOrDefault();
                                        location.Front.InCompatible = values != null ? values.Contains(location.FloorNumber) ? true : false : false;
                                    }
                                    if (location.Rear != null)
                                    {
                                        var values = (from keysValues in rearSelected.Keys
                                                      where String.Equals(keysValues, hallStations.HallStationId, StringComparison.OrdinalIgnoreCase)
                                                      select rearSelected[keysValues]).ToList().FirstOrDefault();
                                        location.Rear.InCompatible = values != null ? values.Contains(location.FloorNumber) ? true : false : false;
                                    }
                                }
                            }
                        }
                    }
                    if (fixtureType != Constants.AGILEHALLSTATION)
                    {
                        foreach (var hallstation in HallStationLocation)
                        {
                            foreach (var station in hallstation.openingsAssigned)
                            {
                                if (station.Front != null)
                                {
                                    var values = (from keysValues in frontSelected.Keys
                                                  where String.Equals(keysValues, hallstation.HallStationId, StringComparison.OrdinalIgnoreCase)
                                                  select frontSelected[keysValues]).ToList().FirstOrDefault();
                                    station.Front.InCompatible = values != null ? values.Contains(station.FloorNumber) ? true : false : false;

                                }
                                if (station.Rear != null)
                                {
                                    var values = (from keysValues in rearSelected.Keys
                                                  where String.Equals(keysValues, hallstation.HallStationId, StringComparison.OrdinalIgnoreCase)
                                                  select rearSelected[keysValues]).ToList().FirstOrDefault();
                                    station.Rear.InCompatible = values != null ? values.Contains(station.FloorNumber) ? true : false : false;

                                }
                            }
                        }
                        newConsole.FixtureAssignments = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(HallStationLocation.Distinct()));
                    }
                    else
                    {
                        var agileData = (from x in groupHallFixtureConsoles
                                         where x.GroupHallFixtureType.Equals(Constants.AGILEHALLSTATION) && x.ConsoleId == consoleId
                                         select x).FirstOrDefault();
                        var frontFloors = (from x in groupHallFixtureConsoles
                                           where x.GroupHallFixtureType.Equals(Constants.AGILEHALLSTATION) && x.ConsoleId != consoleId && x.ConsoleId != 0
                                           from opening in x.GroupHallFixtureLocations
                                           where opening.Front.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                           select opening.FloorNumber).ToList();
                        var rearFloors = (from x in groupHallFixtureConsoles
                                          where x.GroupHallFixtureType.Equals(Constants.AGILEHALLSTATION) && x.ConsoleId != consoleId && x.ConsoleId != 0
                                          from opening in x.GroupHallFixtureLocations
                                          where opening.Rear.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                          select opening.FloorNumber).ToList();
                        foreach (var location in agileData.GroupHallFixtureLocations)
                        {
                            if (frontSelected.Values.SelectMany(x => x).ToList().Contains(location.FloorNumber) || frontFloors.Contains(location.FloorNumber))
                            {
                                location.Front.InCompatible = true;
                            }
                            if (rearSelected.Values.SelectMany(x => x).ToList().Contains(location.FloorNumber) || rearFloors.Contains(location.FloorNumber))
                            {
                                location.Rear.InCompatible = true;
                            }
                        }
                        var detailsList = new List<HallStations>();
                        var details = new HallStations
                        {
                            HallStationId = Constants.ZERO,
                            HallStationName = Constants.LANDINGS,
                            NoOfFloors = agileData.GroupHallFixtureLocations.Count(),
                            openingDoors = new OpeningDoors
                            {
                                Front = agileData.Openings.Front,
                                Rear = agileData.Openings.Rear
                            },
                            openingsAssigned = (from x in agileData.GroupHallFixtureLocations orderby Convert.ToInt32(x.FloorNumber) select x).ToList()

                        };
                        detailsList.Add(details);
                        newConsole.FixtureAssignments = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(detailsList));
                    }
                    newConsole.UnitDetails = new List<UnitDetailsValues>();
                    var cacheFixtures = _cpqCacheManager.GetCache(userId, _environment, groupId.ToString(), Constant.MAINFIXTUREASSIGNAMENT + consoleId);
                    _cpqCacheManager.SetCache(userId, _environment, groupId.ToString(), Constant.MAINFIXTUREASSIGNAMENT + consoleId, Utility.SerializeObjectValue(newConsole.FixtureAssignments));

                    _cpqCacheManager.SetCache(SessionId, _environment, Constant.FIXTUREASSIGNAMENT, Utility.SerializeObjectValue(newGroupLocation));

                }


            }
        }

        /// <summary>
        /// ChangeGroupHallFixtureConsole
        /// </summary>
        /// <param name="groupHallFixtureConsoles"></param>
        /// <param name="fixtureType"></param>
        /// <param name="consoleId"></param>
        /// <param name="newConsole"></param>
        /// <param name="lstSelectedFrontFloorNumber"></param>
        /// <param name="unitDetails"></param>
        /// <param name="SessionId"></param>
        /// <param name="HallStationLocation"></param>
        /// <param name="groupId"></param>
        /// <param name="fixtureStrategy"></param>
        /// <param name="userId"></param>
        /// <param name="objGroup"></param>
        /// <param name="fixtureSelected"></param>
        private void ChangeGroupHallFixtureConsole(List<GroupHallFixtures> groupHallFixtureConsoles, string fixtureType, int consoleId, ref GroupHallFixtures newConsole, List<List<UnitDetailsValues>> lstSelectedFrontFloorNumber, string unitDetails, string SessionId, List<HallStations> HallStationLocation, int groupId, string fixtureStrategy, string userId, GroupHallFixturesData objGroup, string fixtureSelected)
        {
            newConsole = groupHallFixtureConsoles.Where(x => x.ConsoleId.Equals(consoleId) && x.GroupHallFixtureType.Equals(fixtureType)).ToList()[0];

            var cachedfixture = _configure.GetUnitfixtureDetails(SessionId);
            var cachedFixtureDetails = new List<UnitDetailsValues>();

            newConsole.ConsoleId = Convert.ToInt32(objGroup.GroupHallFixtureConsoleId);
            newConsole.ConsoleName = objGroup.ConsoleName;
            newConsole.VariableAssignments = new List<ConfigVariable>();
            newConsole.GroupHallFixtureType = objGroup.FixtureType;
            if (objGroup.VariableAssignments != null && objGroup.VariableAssignments.Count != 0)
            {
                foreach (var varAssignment in objGroup.VariableAssignments)
                {

                    var newAssignment = new ConfigVariable()
                    {
                        VariableId = varAssignment.VariableId,
                        Value = varAssignment.Value
                    };
                    newConsole.VariableAssignments.Add(newAssignment);
                }
            }
            newConsole.IsController = objGroup.IsController;


            List<HallStations> HallDetails = new List<HallStations>();
            if (!fixtureType.Equals(Constants.AGILEHALLSTATION))
            {
                foreach (var hallFixtureLocation in objGroup.GroupHallFixtureLocations)
                {
                    HallStations StationHall = new HallStations
                    {
                        HallStationName = hallFixtureLocation.HallStationName,
                        HallStationId = hallFixtureLocation.HallStationId,

                        openingsAssigned = new List<GroupHallFixtureLocations>()
                    };
                    foreach (var opening in hallFixtureLocation.Assignments)
                    {
                        GroupHallFixtureLocations GHFL = new GroupHallFixtureLocations
                        {
                            FloorNumber = opening.FloorNumber,
                            FloorDesignation = opening.FloorDesignation
                        };
                        if (StationHall.HallStationName.Contains(Constants.F))
                        {
                            if (opening.Front != null)
                            {
                                GHFL.Front = new LandingOpening
                                {
                                    Value = opening.Front,
                                    NotAvailable = !opening.Front
                                };
                            }
                        }
                        if (StationHall.HallStationName.Contains(Constants.R))
                        {
                            if (opening.Rear != null)
                            {
                                GHFL.Rear = new LandingOpening
                                {
                                    Value = opening.Rear,
                                    NotAvailable = !opening.Rear
                                };
                            }
                        }
                        StationHall.openingsAssigned.Add(GHFL);
                    }
                    if (StationHall.HallStationName.Contains(Constants.F))
                    {
                        StationHall.NoOfFloors = (from opening in StationHall.openingsAssigned where opening.Front.Value.Equals(true) select opening).ToList().Count();
                    }
                    else if (StationHall.HallStationName.Contains(Constants.R))
                    {
                        StationHall.NoOfFloors = (from opening in StationHall.openingsAssigned where opening.Rear.Value.Equals(true) select opening).ToList().Count();

                    }
                    HallDetails.Add(StationHall);
                }
                newConsole.FixtureAssignments = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(HallDetails.Distinct()));
            }
            else
            {

                newConsole.FixtureAssignments = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(objGroup.GroupHallFixtureLocations));
            }
                
            newConsole.UnitDetails = new List<UnitDetailsValues>();
            var cacheFixtures = _cpqCacheManager.GetCache(userId, _environment, groupId.ToString(), Constant.MAINFIXTUREASSIGNAMENT + consoleId);
            if (!string.IsNullOrEmpty(cacheFixtures))
            {
                var mainCacheFixtures = Utility.DeserializeObjectValue<List<HallStations>>(cacheFixtures);
                var hallData = (from maincache in mainCacheFixtures
                                where !HallDetails.Any(x => x.HallStationId.ToLower() == maincache.HallStationId.ToLower())
                                select maincache).Distinct().ToList();
            }
            newConsole.FixtureAssignments = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(HallDetails));
            _cpqCacheManager.SetCache(userId, _environment, groupId.ToString(), Constant.MAINFIXTUREASSIGNAMENT + consoleId, Utility.SerializeObjectValue(newConsole.FixtureAssignments));


        }
        /// <summary>
        /// ValidationForGroupHallFixture
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="hallstations"></param>
        /// <param name="methodBeginTime"></param>
        private void ValidationForGroupHallFixture(int groupId, List<ConfigVariable> hallstations, DateTime methodBeginTime)
        {
            Dictionary<string, bool> isDisabled = _groupdl.GetGroupConfigurationSectionTab(groupId, hallstations);
            if (isDisabled[Constant.OPENINGLOCATIONS].Equals(true))
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.ERRORMSGFORRISERANDOPENING,
                    Description = Constant.ERRORMSGFORRISERANDOPENING
                });
            }
            else if (isDisabled[Constant.GROUPHALLFIXTURES].Equals(true))
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.ERRORMSGFORGROUPOPENING,
                    Description = Constant.ERRORMSGFORGROUPOPENING
                });
            }
            else if ((isDisabled[Constant.OPENINGLOCATIONS].Equals(true)) && (isDisabled[Constant.GROUPHALLFIXTURES].Equals(false)))
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.ERRORMSFFORRISERLOCATION,
                    Description = Constant.ERRORMSFFORRISERLOCATION
                });
            }
        }

        /// <summary>
        /// Method for setting incompatible property for Hall Call Lockout and Hall Call Registration consoles
        /// </summary>
        /// <param name="groupHallFixtureConsoles"></param>
        /// <param name="fixtureType"></param>
        /// <param name="consoleId"></param>
        /// <param name="frontSelected"></param>
        /// <param name="rearSelected"></param>
        private void SetIncompatiblePropertyForMutuallyExclusiveConsoles(List<GroupHallFixtures> groupHallFixtureConsoles, string fixtureType, int consoleId, Dictionary<string, List<int>> frontSelected, Dictionary<string, List<int>> rearSelected)
        {
            List<string> fixtureTypesToCheck = new List<string>();
            if (fixtureType.Equals(Constants.HALLCALLREGISTRATION) || fixtureType.Equals(Constants.HALLCALLLOCKOUT))
            {
                fixtureTypesToCheck.Add(Constants.HALLCALLREGISTRATION);
                fixtureTypesToCheck.Add(Constants.HALLCALLLOCKOUT);
            }
            foreach (var console in groupHallFixtureConsoles)
            {
                var flag = console.ConsoleId == consoleId ? (console.GroupHallFixtureType == fixtureType ? false : true) : true;
                if (flag && fixtureTypesToCheck.Contains(console.GroupHallFixtureType) && console.ConsoleId != 0)
                {
                    foreach (var hallStation in console.HallStations)
                    {
                        if (hallStation.HallStationId.Contains(Constants.FRONTHALLSTATION))
                        {
                            if (!frontSelected.ContainsKey(hallStation.HallStationId))
                            {
                                frontSelected.Add(hallStation.HallStationId, new List<int>());
                            }
                            foreach (var opening in hallStation.openingsAssigned)
                            {
                                if (opening.Front.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase))
                                {
                                    frontSelected[hallStation.HallStationId].Add(opening.FloorNumber);
                                }

                            }
                        }
                        if (hallStation.HallStationId.Contains(Constants.REARHALLSTATION))
                        {
                            if (!rearSelected.ContainsKey(hallStation.HallStationId))
                            {
                                rearSelected.Add(hallStation.HallStationId, new List<int>());
                            }
                            foreach (var opening in hallStation.openingsAssigned)
                            {
                                if (opening.Rear.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase))
                                {
                                    rearSelected[hallStation.HallStationId].Add(opening.FloorNumber);
                                }

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method for setting incompatible property for AGILE and Traditional consoles
        /// </summary>
        /// <param name="groupHallFixtureConsoles"></param>
        /// <param name="fixtureType"></param>
        /// <param name="consoleId"></param>
        /// <param name="frontSelected"></param>
        /// <param name="rearSelected"></param>
        private void SetIncompatiblePropertyForMutuallyExclusiveDefaultConsoles(List<GroupHallFixtures> groupHallFixtureConsoles, string fixtureType, int consoleId, Dictionary<string, List<int>> frontSelected, Dictionary<string, List<int>> rearSelected)
        {
            List<string> fixtureTypesToCheck = new List<string>();

            if (fixtureType.Equals(Constants.TRADITIONALHALLSTATION))
            {
                var hallstations = (from consoles in groupHallFixtureConsoles where consoles.GroupHallFixtureType == Constants.TRADITIONALHALLSTATION select consoles.HallStations).FirstOrDefault();
                foreach (var hallStation in hallstations)
                {
                    if (hallStation.HallStationId.Contains(Constants.FRONTHALLSTATION))
                    {
                        if (!frontSelected.ContainsKey(hallStation.HallStationId))
                        {
                            frontSelected.Add(hallStation.HallStationId, new List<int>());
                        }
                    }
                    if (hallStation.HallStationId.Contains(Constants.REARHALLSTATION))
                    {
                        if (!rearSelected.ContainsKey(hallStation.HallStationId))
                        {
                            rearSelected.Add(hallStation.HallStationId, new List<int>());
                        }
                    }
                }

                foreach (var console in groupHallFixtureConsoles)
                {
                    if (console.GroupHallFixtureType.Equals(Constants.AGILEHALLSTATION))
                    {
                        foreach (var location in console.GroupHallFixtureLocations)
                        {
                            if (location.Front.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase))
                            {
                                foreach (var selection in frontSelected.Keys)
                                {
                                    frontSelected[selection].Add(Convert.ToInt32(location.FloorNumber));
                                }
                            }
                            if (location.Rear.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase))
                            {
                                foreach (var selection in rearSelected.Keys)
                                {
                                    rearSelected[selection].Add(Convert.ToInt32(location.FloorNumber));
                                }
                            }
                        }
                    }
                    else if (console.GroupHallFixtureType == Constants.TRADITIONALHALLSTATION && (console.ConsoleId != 0 && console.ConsoleId != consoleId))
                    {
                        foreach (var hallStation in console.HallStations)
                        {
                            if (hallStation.HallStationId.Contains(Constants.FRONTHALLSTATION))
                            {
                                foreach (var opening in hallStation.openingsAssigned)
                                {
                                    if (opening.Front.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase))
                                    {
                                        frontSelected[hallStation.HallStationId].Add(opening.FloorNumber);
                                    }

                                }
                            }
                            if (hallStation.HallStationId.Contains(Constants.REARHALLSTATION))
                            {
                                foreach (var opening in hallStation.openingsAssigned)
                                {
                                    if (opening.Rear.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase))
                                    {
                                        rearSelected[hallStation.HallStationId].Add(opening.FloorNumber);
                                    }

                                }
                            }
                        }
                    }
                }
            }
            else if (fixtureType.Equals(Constants.AGILEHALLSTATION))
            {
                foreach (var console in groupHallFixtureConsoles)
                {
                    if (console.GroupHallFixtureType == Constants.TRADITIONALHALLSTATION)
                    {
                        foreach (var hallStation in console.HallStations)
                        {
                            if (hallStation.HallStationId.Contains(Constants.FRONTHALLSTATION))
                            {
                                if (!frontSelected.ContainsKey(hallStation.HallStationId))
                                {
                                    frontSelected.Add(hallStation.HallStationId, new List<int>());
                                }
                                foreach (var opening in hallStation.openingsAssigned)
                                {
                                    if (opening.Front.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase))
                                    {
                                        frontSelected[hallStation.HallStationId].Add(opening.FloorNumber);
                                    }

                                }
                            }
                            if (hallStation.HallStationId.Contains(Constants.REARHALLSTATION))
                            {
                                if (!rearSelected.ContainsKey(hallStation.HallStationId))
                                {
                                    rearSelected.Add(hallStation.HallStationId, new List<int>());
                                }
                                foreach (var opening in hallStation.openingsAssigned)
                                {
                                    if (opening.Rear.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase))
                                    {
                                        rearSelected[hallStation.HallStationId].Add(opening.FloorNumber);
                                    }

                                }
                            }
                        }
                    }
                }

            }
        }
    }
}