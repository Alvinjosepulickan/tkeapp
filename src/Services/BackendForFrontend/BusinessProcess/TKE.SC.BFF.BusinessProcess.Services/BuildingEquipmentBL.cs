using Configit.Configurator.Server.Common;
using Microsoft.Extensions.Logging;
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
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.Common;

namespace TKE.SC.BFF.BusinessProcess.Services
{

    public class BuildingEquipmentBL : IBuildingEquipment
    {
        /// <summary>
        /// Variable Collection
        /// </summary>
        #region Variables
        private readonly IBuildingEquipmentDL _buildingEquipmentdl;
        private readonly IConfigure _configure;
        private readonly IBuildingConfigurationDL _buildingDl;
        private ILogger<BuildingEquipmentBL> buildingEquipmentBlLogger;
        private IBuildingEquipmentDL iBuilding;
        private IConfigure iConfigure;
        #endregion

        /// <summary>
        /// Constructor For BuildingEquipmentBL
        /// </summary>
        /// <param Name="buildingEquipmentDl"></param>
        /// <param Name="configure"></param>
        /// <param Name="buildingDl"></param>
        /// <param Name="logger"></param>
        public BuildingEquipmentBL(IBuildingEquipmentDL buildingEquipmentDl, IConfigure configure, IBuildingConfigurationDL buildingDl, ILogger<BuildingEquipmentBL> logger)
        {
            _buildingEquipmentdl = buildingEquipmentDl;
            _configure = configure;
            _buildingDl = buildingDl;
            Utility.SetLogger(logger);
        }

        /// <summary>
        /// BuildingEquipmentBL
        /// </summary>
        /// <param name="buildingEquipmentBlLogger"></param>
        /// <param name="iBuilding"></param>
        /// <param name="iConfigure"></param>
        public BuildingEquipmentBL(ILogger<BuildingEquipmentBL> buildingEquipmentBlLogger, IBuildingEquipmentDL iBuilding, IConfigure iConfigure)
        {
            this.buildingEquipmentBlLogger = buildingEquipmentBlLogger;
            this.iBuilding = iBuilding;
            this.iConfigure = iConfigure;
        }

        /// <summary>
        /// Business Logic for StartBuildingEquipment
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="BuildingId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> StartBuildingEquipmentConfigure(JObject variableAssignments, int buildingId, string sessionId)
        {
            var methodStartTime = Utility.LogBegin();
            List<BuildingEquipmentData> lstBldgEquipmentConsoleCnfgn = new List<BuildingEquipmentData>();
            var configurationRequest = CreateBuildingConfigurationRequest(variableAssignments);
            // to select required configurationtype
            var entranceConfigurations = new List<EntranceConfigurations>();
            if (Convert.ToString(buildingId) != null)
            {
                if (buildingId != 0)
                {
                    var buildingEquipmentContantsDictionary = Utility.GetVariableMapping(Constant.BUILDINGMAPPERVARIABLESMAPPERPATH, Constant.BUILDINGEQUIPMENTVARIABLES);
                    List<ConfigVariable> lstConfigVariable = new List<ConfigVariable>();
                    foreach (var variable in buildingEquipmentContantsDictionary)
                    {
                        ConfigVariable configVariable = new ConfigVariable() { VariableId = variable.Key, Value = variable.Value };
                        lstConfigVariable.Add(configVariable);
                    }

                    var dtConfigVariables = Utility.DeserializeObjectValue<DataTable>(Utility.SerializeObjectValue(lstConfigVariable));
                    string userName = _configure.GetUserId(sessionId);
                    //Get BuildingEquipmentConfiguration Variables
                    var lstConfigureVariable = _buildingEquipmentdl.GetBuildingEquipmentConfigurationByBuildingId(buildingId, dtConfigVariables);
                    //Get Building Equipment Console Data
                    lstBldgEquipmentConsoleCnfgn = _buildingEquipmentdl.GetBuildingEquipmentConsoles(buildingId, userName, sessionId);
                    //Setting the Console Data in Cache
                    lstBldgEquipmentConsoleCnfgn = _configure.SetCacheBuildingEquipmentConsoles(lstBldgEquipmentConsoleCnfgn, sessionId, buildingId, false);
                    //Converting ConfigureVariable to VariableAssignments
                    List<VariableAssignment> lstvariableassignment = lstConfigureVariable.Select(
                     variableAssignment => new VariableAssignment
                     {
                         VariableId = variableAssignment.VariableId,
                         Value = variableAssignment.Value
                     }).ToList<VariableAssignment>();

                    configurationRequest.Line.VariableAssignments = lstvariableassignment;
                    var fixtureStrategy = _configure.SetCacheFixtureStrategy(lstvariableassignment, sessionId, buildingId);
                }
            }

            variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configurationRequest.Line));
            var rolename = _configure.GetRoleName(sessionId);
            var permission = _buildingDl.GetPermissionByRole(buildingId, rolename);
            var response = await _configure.StartBuildingEquipmentConfigureBl(sessionId, permission, buildingId, variableAssignments, lstBldgEquipmentConsoleCnfgn).ConfigureAwait(false);
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = JObject.FromObject(response.Response) };
        }

        /// <summary>
        /// this method is used to create the building configuration stub request body
        /// </summary>
        /// <param Name="varibleAssignments"></param>
        /// <returns></returns>
        private ConfigurationRequest CreateBuildingConfigurationRequest(JObject varibleAssignments)
        {
            //creating req body
            var methodStartTime = Utility.LogBegin();
            var stubReqbody = JObject.Parse(System.IO.File.ReadAllText(Constant.BUILDINGCONFIGURATIONREQESTBODYSTUBPATH)).ToString();
            var configurationRequest = Utility.DeserializeObjectValue<ConfigurationRequest>(stubReqbody);
            configurationRequest.Date = DateTime.Now;
            if (varibleAssignments != null)
            {
                var objLine = Utility.DeserializeObjectValue<Line>(varibleAssignments.ToString());
                configurationRequest.Line.VariableAssignments = objLine.VariableAssignments;
            }
            Utility.LogEnd(methodStartTime);
            return configurationRequest;
        }

        /// <summary>
        /// SaveAssignGroups
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="unitHallFixtureConfigurationData"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="is_Saved"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveAssignGroups(int buildingId, List<BuildingEquipmentData> buildingEquipmentFixtureConfigurationDatas, string sessionId, int is_Saved)
        {
            var methodStartTime = Utility.LogBegin();
            var userId = _configure.GetUserId(sessionId);
            var response = new JArray();
            foreach (var buildingEquipmentFixtureConfigurationData in buildingEquipmentFixtureConfigurationDatas)
            {
                var consoleId = buildingEquipmentFixtureConfigurationData.ConsoleId;
                foreach (var variable in buildingEquipmentFixtureConfigurationData.VariableAssignments)
                {
                    if ((Utility.CheckEquals(variable.VariableId, Constant.FIREPHONEJACK) || Utility.CheckEquals(variable.VariableId, Constant.FIREBOX)) && variable.Value.Equals(false))
                    {
                        variable.Value = Constant.NR;
                    }
                }
                if (buildingEquipmentFixtureConfigurationData.ConsoleName.Contains(Constant.KEYWORDLOBBYPANEL))
                {
                    var smartRescue5 = (from variable in buildingEquipmentFixtureConfigurationData.VariableAssignments
                                        where variable.VariableId.Equals(Constant.PARAMSMARTRESCUEPHONE5)
                                        select variable).ToList();
                    if (smartRescue5[0].Value.Equals(true))
                    {
                        var groupsSelected = (from groups in buildingEquipmentFixtureConfigurationData.ConfiguredGroups
                                              where groups.isChecked.Equals(true) && groups.noOfUnits > 5
                                              select groups).ToList();
                        if (groupsSelected != null && groupsSelected.Any())
                        {
                            string joinedString = Constant.SPACE;
                            var groupNames = groupsSelected.Select(x => x.groupName).ToList();
                            joinedString = string.Join(", ", groupNames.ToArray());
                            var errorMessage = Constant.ERRORMESSAGE1SMARTRESCUEPHONE5 + joinedString + Constant.ERRORMESSAGE2SMARTRESCUEPHONE5;
                            Utility.LogEnd(methodStartTime);
                            throw new CustomException(new ResponseMessage
                            {
                                StatusCode = Constant.BADREQUEST,
                                Message = errorMessage
                            });
                        }
                    }
                }
                var buildingEquipmentConsoles = _configure.SetCacheBuildingEquipmentConsoles(null, sessionId, buildingId, false);
                var updatedConsoleHistory = GetConsoleHistories(buildingEquipmentFixtureConfigurationData, buildingEquipmentConsoles).Where(x => !(Utility.CheckEquals(x.PresentValue, x.PreviousValue))).ToList();
                updatedConsoleHistory = (from historyDetails in updatedConsoleHistory
                                         where !string.IsNullOrEmpty(historyDetails.PresentValue) || !string.IsNullOrEmpty(historyDetails.PreviousValue)
                                         select historyDetails).ToList();

                List<LogHistoryTable> logHistoryTable = GetLogHistoryTableForConsole(updatedConsoleHistory);
                var result = _buildingEquipmentdl.SaveAssignGroups(buildingId, consoleId, buildingEquipmentFixtureConfigurationData, userId, is_Saved, logHistoryTable);
                response = JArray.FromObject(result);
                if (result[0].result == 1)
                {
                    continue;
                }
                else
                {
                    Utility.LogEnd(methodStartTime);
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = Constant.SAVEBUILDINGEQUIPMENTCONSOLEERRORMSG,
                        Description = result[0].message
                    });
                }
            }
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
        }

        /// <summary>
        /// Duplicate Building Equipment Console
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="consoleId"></param>
        /// <param Name="sessionId"></param>
        public async Task<ResponseMessage> DuplicateBuildingEquipmentConsole(int buildingId, int consoleId, string sessionId)
        {
            var methodStartTime = Utility.LogBegin();
            var userId = _configure.GetUserId(sessionId);
            var userName = _configure.GetUserId(sessionId);
            var result = _buildingEquipmentdl.DuplicateBuildingEquipmentConsole(buildingId, consoleId, userId);
            var lstBldgEquipmentConsoleCnfgn = _buildingEquipmentdl.GetBuildingEquipmentConsoles(buildingId, userName, sessionId);
            var unitHallFixtureConsoles = _configure.SetCacheBuildingEquipmentConsoles(lstBldgEquipmentConsoleCnfgn, sessionId, buildingId, false);
            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodStartTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                Utility.LogEnd(methodStartTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Description = Constant.DUPLICATEBUILDINGEQUIPMENTCONSOLEERRORMSG,
                    Message = result[0].message
                });
            }
        }

        /// <summary>
        /// Delete Building Equipment Console
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="consoleId"></param>
        /// <param Name="sessionId"></param>
        public async Task<ResponseMessage> DeleteBuildingEquipmentConsole(int buildingId, int consoleId, string sessionId)
        {
            var methodStartTime = Utility.LogBegin();
            var userId = _configure.GetUserId(sessionId);
            /**Basic Log**/
            var buildingEquipmentConsoles = _configure.SetCacheBuildingEquipmentConsoles(null, sessionId, buildingId, false);
            var cachedConsole = (from cachedData in buildingEquipmentConsoles
                                 where cachedData.ConsoleId.Equals(consoleId)
                                 select cachedData).ToList();
            var lsthistory = new List<LogHistoryTable>();
            if (cachedConsole.Count > 0)
            {
                foreach (var assignment in cachedConsole[0].VariableAssignments)
                {
                    var displayName = string.Empty;
                    if (!string.IsNullOrEmpty(assignment.VariableId))
                    {
                        var enrichedData = JObject.Parse(File.ReadAllText(Constant.BUILDINGENRICHEDDATA));
                        var enrichedDataVariables = enrichedData[Constant.VARIABLES];
                        var needVariables = Utility.GetTokens(assignment.VariableId, enrichedDataVariables, false);
                        var currentPropertyCollection = needVariables.Select(x => (JProperty)x).Where(x => x.Name == Constant.PROPERTIES).Select(x => x.Value).FirstOrDefault();
                        displayName = currentPropertyCollection.Children().Where(x => x.Children<JProperty>().Any(y => y.Name == Constant.IDPARAM && y.Value.ToString() == Constant.DISPLAYNAME)).SelectMany(x => x.Children<JProperty>().Select(y => y.Name == Constant.VALUE ? y.Value.ToString() : "")).FirstOrDefault(x => !string.IsNullOrEmpty(x));
                    }                    
                    if (string.IsNullOrEmpty(displayName))
                    {
                        displayName = assignment.VariableId;
                    }   
                    var lstAssignmedgrp = cachedConsole[0].lstConfiguredGroups.Where(y => y.isChecked).Select(x => x.groupName).ToArray();
                    if (lstAssignmedgrp.Length > 0)
                    {
                        var strassignedgrp = string.Join(Constant.COMMA + Constant.EMPTYSPACE, lstAssignmedgrp);

                        var history = new LogHistoryTable()
                        {
                            VariableId = Constant.LOBBYPANEL + Constant.HYPHEN + displayName + "(" + strassignedgrp + ")",
                            UpdatedValue = string.Empty,
                            PreviuosValue = Convert.ToString(assignment.Value)
                        };
                        lsthistory.Add(history);
                    }
                }
            }
            lsthistory = lsthistory.Where(x => !x.UpdatedValue.Equals(x.PreviuosValue)).ToList();
            /**Basic Log**/
            var result = _buildingEquipmentdl.DeleteBuildingEquipmentConsole(buildingId, consoleId, userId, lsthistory);
            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodStartTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                Utility.LogEnd(methodStartTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.DELETEBUILDINGEQUIPMENTSUCCESSMSG,
                    Description = result[0].message
                });
            }
        }

        /// <summary>
        /// Start Building Equipment Console
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="consoleId"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="objEntranceConfiguration"></param>
        /// <param Name="isSave"></param>
        /// <param Name="editFlag"></param>
        public async Task<ResponseMessage> StartBuildingEquipmentConsole(int consoleId, int buildingId, string SessionId, BuildingEquipmentData objEntranceConfiguration = null, bool isSave = false, bool editFlag = false)
        {
            var methodStartTime = Utility.LogBegin();
            var newSmartRescueConsoles = new Sections()
            {
                Id = Constants.IDTWO,
                Name = Constants.SMARTRESCUEPHONESTANDALONENAME,
                isController = true,
                isLobby = false,
                sections = new List<SectionsValues>(),
                Variables = new List<Variables>()
            };
            var response = new JObject();
            var consoleIdList = ResolveDependentConsoles(consoleId);
            foreach (var id in consoleIdList)
            {
                consoleId = id;
                var unitHallFixtureConsoles = _configure.SetCacheBuildingEquipmentConsoles(null, SessionId, buildingId, false);
                if (unitHallFixtureConsoles == null)
                {
                    Utility.LogEnd(methodStartTime);
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = Constant.CACHEEMPTY
                    });
                }
                var islobby = true;
                var isChange = false;
                var consoleToRemove = (from console in unitHallFixtureConsoles
                                       where (console.VariableAssignments.Count == 0 && console.AssignedGroups == 0) && console.IsController == false
                                       && console.ConsoleId != 0
                                       select console).FirstOrDefault();

                if (consoleToRemove != null)
                {
                    unitHallFixtureConsoles.Remove(consoleToRemove);
                }


                var lstNotSelectedGroups = (from varEntranceConsole in unitHallFixtureConsoles
                                            where varEntranceConsole.ConsoleName.Contains("Lobby Panel") || varEntranceConsole.ConsoleName.Contains("Dummy Console")
                                            from AssignedGrps in varEntranceConsole.lstConfiguredGroups
                                            where AssignedGrps.isChecked.Equals(false)
                                            select AssignedGrps).ToList();
                lstNotSelectedGroups = lstNotSelectedGroups.GroupBy(x => x.groupId, (key, Group) => Group.First()).ToList();

                var lstSelectedGroups = (from varEntranceConsole in unitHallFixtureConsoles
                                         where varEntranceConsole.ConsoleId.Equals(0)
                                         from AssignedGrps in varEntranceConsole.lstConfiguredGroups
                                         where AssignedGrps.isChecked.Equals(true)
                                         select AssignedGrps).ToList();

                var numberOfLobby = (from varEntranceConsole in unitHallFixtureConsoles
                                     where varEntranceConsole.IsLobby.Equals(true)
                                     select varEntranceConsole).ToList();

                var newConsole = new BuildingEquipmentData();
                if (objEntranceConfiguration == null)
                {
                    var maxConsoleId = (from entrance in unitHallFixtureConsoles
                                        select entrance.ConsoleNumber).ToList().Max();
                    var entranceConsole = unitHallFixtureConsoles.Where(x => x.IsLobby.Equals(true)).ToList();
                    var isController = unitHallFixtureConsoles.Where(x => x.IsController == true && x.IsLobby.Equals(true)).ToList().Count();
                    var unitHallFixtureConsole = (from consoles in unitHallFixtureConsoles
                                                  where consoles.ConsoleId == 0
                                                  select consoles).ToList();
                    if (unitHallFixtureConsole != null && unitHallFixtureConsole.Any())
                    {
                        if (consoleId == 0)
                        {
                            newConsole.ConsoleId = maxConsoleId + 1;
                            newConsole.ConsoleName = Constant.LOBBYPANEL + (entranceConsole.Count().ToString());
                            if (entranceConsole.Count<2)
                            {
                                newConsole.ConsoleName = Constant.KEYWORDLOBBYPANEL;
                            }
                            newConsole.lstConfiguredGroups = lstNotSelectedGroups;
                            foreach (var grps in newConsole.lstConfiguredGroups)
                            {
                                grps.consoleId = newConsole.ConsoleId;
                            }
                            newConsole.lstExistingGroups = new List<BuildingEquipmentGroupDetails>();//unitHallFixtureConsoles[0].lstExistingGroups;
                            newConsole.lstFutureGroup = new List<BuildingEquipmentGroupDetails>(); //unitHallFixtureConsoles[0].lstFutureGroup;
                            newConsole.IsLobby = true;
                            newConsole.VariableAssignments = new List<ConfigVariable>();
                            unitHallFixtureConsoles.Add(newConsole);
                            unitHallFixtureConsoles = _configure.SetCacheBuildingEquipmentConsoles(unitHallFixtureConsoles, SessionId, buildingId, false);
                        }
                        else
                        {
                            editFlag = true;
                            newConsole = (from consoles in unitHallFixtureConsoles
                                          where consoles.ConsoleNumber == consoleId
                                          select consoles).ToList().FirstOrDefault();

                            if (newConsole != null)
                            {

                                var lstSelectedGroup = (from varEntranceConsole in unitHallFixtureConsoles
                                                        where varEntranceConsole.ConsoleNumber.Equals(newConsole.ConsoleNumber)
                                                        from AssignedGrps in varEntranceConsole.lstConfiguredGroups
                                                        where AssignedGrps.isChecked.Equals(true)
                                                        select AssignedGrps).ToList();
                                lstSelectedGroup = lstSelectedGroup.GroupBy(x => x.groupId, (key, Group) => Group.First()).ToList();
                                var lstNotSelectedGroups1 = (from varEntranceConsole in unitHallFixtureConsoles
                                                             where varEntranceConsole.ConsoleNumber.Equals(newConsole.ConsoleNumber)
                                                             from AssignedGrps in varEntranceConsole.lstConfiguredGroups
                                                             where AssignedGrps.isChecked.Equals(false)
                                                             select AssignedGrps).ToList();
                                if (newConsole.IsLobby.Equals(true))
                                {
                                    lstNotSelectedGroups1 = (from varEntranceConsole in unitHallFixtureConsoles
                                                             where varEntranceConsole.IsLobby.Equals(true)
                                                             from AssignedGrps in varEntranceConsole.lstConfiguredGroups
                                                             where AssignedGrps.isChecked.Equals(false)
                                                             select AssignedGrps).ToList();
                                }
                                lstNotSelectedGroups1 = lstNotSelectedGroups1.GroupBy(x => x.groupId, (key, Group) => Group.First()).ToList();
                                foreach (var grps in lstNotSelectedGroups1)
                                {
                                    grps.consoleId = newConsole.ConsoleId;
                                    lstSelectedGroup.Add(grps);
                                }
                                newConsole.lstConfiguredGroups = lstSelectedGroup;

                                if (newConsole.lstExistingGroups != null)
                                {
                                    var lstExistngGroups = (from varEntranceConsole in unitHallFixtureConsoles
                                                            where varEntranceConsole.ConsoleNumber.Equals(newConsole.ConsoleNumber)
                                                            from AssignedGrps in varEntranceConsole.lstExistingGroups
                                                            select AssignedGrps).ToList();
                                    newConsole.lstExistingGroups = lstExistngGroups;
                                }
                                else
                                {
                                    newConsole.lstExistingGroups = new List<BuildingEquipmentGroupDetails>();
                                }


                                if (newConsole.lstFutureGroup != null)
                                {
                                    var lstFutureGroups = (from varEntranceConsole in unitHallFixtureConsoles
                                                           where varEntranceConsole.ConsoleNumber.Equals(newConsole.ConsoleNumber)
                                                           from AssignedGrps in varEntranceConsole.lstFutureGroup
                                                           select AssignedGrps).ToList();

                                    newConsole.lstFutureGroup = lstFutureGroups;
                                }
                                else
                                {
                                    newConsole.lstFutureGroup = new List<BuildingEquipmentGroupDetails>();
                                }

                                islobby = newConsole.IsLobby;

                            }
                            else
                            {

                                newConsole = new BuildingEquipmentData();
                            }

                        }
                    }

                }
                else
                {
                    List<BuildingEquipmentData> lstBuildingData = new List<BuildingEquipmentData>();
                    lstBuildingData.Add(objEntranceConfiguration);
                    var newCachedConsole = _configure.SetCacheBuildingEquipmentConsoles(lstBuildingData, SessionId, buildingId, true);
                    unitHallFixtureConsoles = _configure.SetCacheBuildingEquipmentConsoles(null, SessionId, buildingId, false);
                    if (unitHallFixtureConsoles.Count() > 0)
                    {
                        foreach (var console in unitHallFixtureConsoles)
                        {
                            if (console.ConsoleNumber == 0 && console.ConsoleName != "consolename")
                            {
                                int count = unitHallFixtureConsoles.Count() - 1;
                                console.ConsoleNumber = count;
                            }
                        }


                        newConsole = unitHallFixtureConsoles.Where(x => x.ConsoleNumber.Equals(consoleId)).ToList()[0];
                        if (newConsole != null)
                        {

                            newConsole.ConsoleId = Convert.ToInt32(objEntranceConfiguration.ConsoleId);
                            newConsole.ConsoleName = objEntranceConfiguration.ConsoleName;
                            newConsole.VariableAssignments = new List<ConfigVariable>();
                            islobby = newConsole.IsLobby;
                            editFlag = true;
                            isChange = true;
                            if (objEntranceConfiguration.VariableAssignments != null || objEntranceConfiguration.VariableAssignments.Count != 0)
                            {
                                foreach (var varAssignment in objEntranceConfiguration.VariableAssignments)
                                {

                                    var newAssignment = new ConfigVariable()
                                    {
                                        VariableId = varAssignment.VariableId,
                                        Value = varAssignment.Value
                                    };
                                    newConsole.VariableAssignments.Add(newAssignment);
                                }
                            }
                        }
                        else
                        {
                            newConsole = new BuildingEquipmentData();
                        }
                    }
                    else
                    {
                        newConsole = new BuildingEquipmentData();
                    }
                }

                if (newConsole.ConsoleId != 0)
                {

                    response = await _configure.BuildingEquipmentConsoleConfigureBl(newConsole, SessionId, isSave, buildingId, editFlag, islobby, isChange).ConfigureAwait(false);
                    if (newConsole.ConsoleId.Equals(2) || newConsole.ConsoleId.Equals(3))
                    {
                        newSmartRescueConsoles.EnrichedData = response[Constants.ENRICHEDDATA].ToObject<JObject>();
                        newSmartRescueConsoles.sections.Add(Utility.DeserializeObjectValue<SectionsValues>(Utility.SerializeObjectValue(response)));
                        response = Utility.FilterNullValues(newSmartRescueConsoles);
                    }
                }
                else
                {
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Description = Constant.CONSOLEDOESNOTEXIST
                    });
                }
            }
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = response
            };

        }
        /// <summary>
        /// Save Building Equipment Configuration
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="Is_Saved"></param>
        public async Task<ResponseMessage> SaveBuildingEquipmentConfiguration(int buildingId, JObject variableAssignments, string sessionId, int Is_Saved, bool saveDraft)
        {
            var methodStartTime = Utility.LogBegin();
            var userId = _configure.GetUserId(sessionId);
            var buildingEquipmentData = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            List<ConfigVariable> listOfGroupVariables = buildingEquipmentData.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();
            var buildingEquipmentConsoles = _configure.SetCacheBuildingEquipmentConsoles(null, sessionId, buildingId, false);
            var result = _buildingEquipmentdl.SaveBuildingEquipmentConfiguration(buildingId, listOfGroupVariables, userId, Is_Saved);
            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodStartTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                Utility.LogEnd(methodStartTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Description = Constant.SAVEBUILDINGEQUIPMENTERRORMSG
                });
            }
        }

        /// <summary>
        /// Get Console Histories Entrance
        /// </summary>
        /// <param Name="entranceConsoles"></param>
        /// <param Name="entranceConfigurationData"></param>
        /// <returns></returns>
        public List<ConsoleHistory> GetConsoleHistories(BuildingEquipmentData consoleData, List<BuildingEquipmentData> cachedConsoleData)
        {
            var methodStartTime = Utility.LogBegin();
            var lstConsoleHistory = new List<ConsoleHistory>();
            var cachedConsole = (from cachedData in cachedConsoleData
                                 where cachedData.ConsoleId.Equals(consoleData.ConsoleId)
                                 select cachedData).ToList();
            var selectedGroups = (from groups in consoleData.ConfiguredGroups
                                  where groups.isChecked.Equals(true)
                                  select groups).ToList();
            var cachedselectedGroups = new List<ConfiguredGroups>();
            if (cachedConsole.Count() > 0 || cachedConsole.Any())
            {
                cachedselectedGroups = (from groups in cachedConsole.FirstOrDefault().lstConfiguredGroups
                                        where groups.isChecked.Equals(true)
                                        select groups).ToList();
            }
            foreach (var grp in selectedGroups)
            {
                var group = cachedselectedGroups.Where(x => x.groupId.Equals(grp.groupId)).ToList();
                if (group.Count > 0)
                {
                    foreach (var variable1 in consoleData.VariableAssignments)
                    {
                        foreach (var variable2 in cachedConsole.FirstOrDefault().VariableAssignments)
                        {
                            if (variable1.VariableId.Equals(variable2.VariableId))
                            {
                                var consolehistory = new ConsoleHistory()
                                {
                                    Console = consoleData.IsLobbyPanel ? Constant.LOBBYPANEL : consoleData.ConsoleName,
                                    Parameter = variable2.VariableId,
                                    GroupName = grp.groupName,
                                    PresentValue = variable1.Value != null ? variable1.Value.ToString() : string.Empty,
                                    PreviousValue = variable2.Value != null ? variable2.Value.ToString() : String.Empty
                                };
                                lstConsoleHistory.Add(consolehistory);
                            }
                            else if (variable2.VariableId.Equals(""))
                            {
                                var consolehistory = new ConsoleHistory()
                                {
                                    Console = consoleData.IsLobbyPanel ? Constant.LOBBYPANEL : consoleData.ConsoleName,
                                    Parameter = variable1.VariableId,
                                    GroupName = group[0].groupName,
                                    PresentValue = variable1.Value != null ? variable1.Value.ToString() : string.Empty,
                                    PreviousValue = String.Empty
                                };
                                lstConsoleHistory.Add(consolehistory);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var variable1 in consoleData.VariableAssignments)
                    {
                        var consolehistory = new ConsoleHistory()
                        {
                            Console = consoleData.IsLobbyPanel ? Constant.LOBBYPANEL : consoleData.ConsoleName,
                            Parameter = variable1.VariableId,
                            GroupName = grp.groupName,
                            PresentValue = variable1.Value != null ? variable1.Value.ToString() : string.Empty,
                            PreviousValue = String.Empty
                        };
                        lstConsoleHistory.Add(consolehistory);
                    }
                }
            }
            for (var i = cachedselectedGroups.Count - 1; i >= 0; i--)
            {
                var selectedgrp = selectedGroups.Where(x => x.groupId.Equals(cachedselectedGroups[i].groupId)).ToList();
                if (selectedgrp.Count > 0)
                {
                    cachedselectedGroups.Remove(cachedselectedGroups[i]);
                }
            }
            foreach (var grp in cachedselectedGroups)
            {
                foreach (var variable2 in cachedConsole.FirstOrDefault().VariableAssignments)
                {
                    if (!variable2.VariableId.Equals(""))
                    {
                        var consolehistory = new ConsoleHistory()
                        {
                            Console = consoleData.IsLobbyPanel ? Constant.LOBBYPANEL : consoleData.ConsoleName,
                            Parameter = variable2.VariableId,
                            GroupName = grp.groupName,
                            PresentValue = string.Empty,
                            PreviousValue = variable2.Value != null ? variable2.Value.ToString() : string.Empty
                        };
                        lstConsoleHistory.Add(consolehistory);
                    }
                }
            }
            Utility.LogEnd(methodStartTime);
            return lstConsoleHistory;
        }

        /// <summary>
        /// Get LogHistory Table For Console
        /// </summary>
        /// <param Name="lstConsoleHistory"></param>
        /// <returns></returns>
        public List<LogHistoryTable> GetLogHistoryTableForConsole(List<ConsoleHistory> lstConsoleHistory)
        {
            var methodStartTime = Utility.LogBegin();
            List<LogHistoryTable> logHistoryTable = new List<LogHistoryTable>();
            if (lstConsoleHistory != null && lstConsoleHistory.Any())
            {
                foreach (var consolehistory in lstConsoleHistory)
                {
                    var lstSameConsoleHistory = (from sameConsoleHistory in lstConsoleHistory
                                                 where (Utility.CheckEquals(consolehistory.Parameter, sameConsoleHistory.Parameter) || sameConsoleHistory.Parameter.Equals(consolehistory.Parameter)) &&
                                                 (consolehistory.PresentValue.Equals(sameConsoleHistory.PresentValue) || Utility.CheckEquals(consolehistory.PresentValue, sameConsoleHistory.PresentValue)) &&
                                                 (consolehistory.PreviousValue.Equals(sameConsoleHistory.PreviousValue) || Utility.CheckEquals(consolehistory.PreviousValue, sameConsoleHistory.PreviousValue))
                                                 select sameConsoleHistory).ToList();
                    var lstGroup = string.Empty;
                    lstConsoleHistory = (from consoleHistoryValues in lstConsoleHistory
                                         where !((Utility.CheckEquals(consolehistory.Parameter, consoleHistoryValues.Parameter) || consoleHistoryValues.Parameter.Equals(consolehistory.Parameter)) && (consolehistory.PresentValue.Equals(consoleHistoryValues.PresentValue) || Utility.CheckEquals(consolehistory.PresentValue, consoleHistoryValues.PresentValue)) && (consolehistory.PreviousValue.Equals(consoleHistoryValues.PreviousValue) || Utility.CheckEquals(consolehistory.PreviousValue, consoleHistoryValues.PreviousValue)))
                                         select consoleHistoryValues).ToList();
                    if (lstSameConsoleHistory.Count > 0)
                    {
                        var arrgrpName = lstSameConsoleHistory.Select(x => x.GroupName).ToArray();
                        lstGroup = string.Join(Constant.COMMA + Constant.EMPTYSPACE, arrgrpName);
                        var enrichedData = JObject.Parse(File.ReadAllText(Constant.BUILDINGENRICHEDDATA));
                        var enrichedDataVariables = enrichedData[Constant.VARIABLES];
                        var needVariables = Utility.GetTokens(consolehistory.Parameter, enrichedDataVariables, false);
                        var currentPropertyCollection = needVariables.Select(x => (JProperty)x).Where(x => x.Name == Constant.PROPERTIES).Select(x => x.Value).FirstOrDefault();
                        var displayName = currentPropertyCollection.Children().Where(x => x.Children<JProperty>().Any(y => y.Name == Constant.IDPARAM && y.Value.ToString() == Constant.DISPLAYNAME)).SelectMany(x => x.Children<JProperty>().Select(y => y.Name == Constant.VALUE ? y.Value.ToString() : "")).FirstOrDefault(x => !string.IsNullOrEmpty(x));
                        if (string.IsNullOrEmpty(displayName))
                        {
                            displayName = consolehistory.Parameter;
                        }
                        var loghistory = new LogHistoryTable()
                        {
                            VariableId = consolehistory.Console + " - " + displayName + "(" + lstGroup + ")",
                            PreviuosValue = consolehistory.PreviousValue,
                            UpdatedValue = consolehistory.PresentValue
                        };
                        logHistoryTable.Add(loghistory);
                    }
                }
            }
            Utility.LogEnd(methodStartTime);
            return logHistoryTable;
        }

        private List<int> ResolveDependentConsoles(int consoleId)
        {
            var consoleIdList = new List<int>();
            consoleIdList.Add(consoleId);
            if (consoleId.Equals(2))
            {
                consoleIdList.Add(3);
            }
            else if (consoleId.Equals(3))
            {
                consoleIdList.Add(2);
            }
            return consoleIdList;
        }
    }
}