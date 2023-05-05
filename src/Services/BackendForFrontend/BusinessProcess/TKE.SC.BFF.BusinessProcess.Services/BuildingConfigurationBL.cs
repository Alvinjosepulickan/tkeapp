/************************************************************************************************************
************************************************************************************************************
   File Name     :   BuildingConfigurationBL.cs 
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
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.Common;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using System.Threading;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class BuildingConfigurationBL : IBuildingConfiguration
    {
        /// <summary>
        /// Variable Collection
        /// </summary>
        #region Variables
        private readonly IBuildingConfigurationDL _buildingDl;
        private readonly IProject _projectDl;
        private readonly IConfigure _configure;
        private readonly string _environment;
        private readonly ICacheManager _cpqCacheManager;
        #endregion
        /// <summary>
        /// BuildingConfigurationBL
        /// </summary>
        /// <param Name="buildingdl"></param>
        /// <param Name="configure"></param>
        /// <param Name="projectdl"></param>
        /// <param Name="logger"></param>
        public BuildingConfigurationBL(IBuildingConfigurationDL buildingdl, IConfigure configure, IProject projectdl, ILogger<BuildingConfigurationBL> logger, ICacheManager cpqCacheManager)
        {
            _buildingDl = buildingdl;
            _configure = configure;
            _projectDl = projectdl;
            _cpqCacheManager = cpqCacheManager;
            Utility.SetLogger(logger);

        }


        /// <summary>
        ///  GetListOfConfigurationForProject
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetListOfConfigurationForProject(string quoteId, string sessionId)
        {
            var methodStartTime = Utility.LogBegin();
            var unitMapperVariablesGeneralInformation = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.GENERALINFOMAPPER);
            var buildingContantsDictionary = Utility.GetVariableMapping(Constant.BUILDINGMAPPERVARIABLESMAPPERPATH, Constant.NCPCONFIGURATION);
            List<ConfigVariable> lstConfigVariable = new List<ConfigVariable>();
            foreach (var variable in buildingContantsDictionary)
            {
                ConfigVariable configVariable = new ConfigVariable() { VariableId = variable.Key, Value = variable.Value };
                lstConfigVariable.Add(configVariable);
            }

            var configVariables = Utility.DeserializeObjectValue<DataTable>(Utility.SerializeObjectValue(lstConfigVariable));



            var lstConfiguration = _buildingDl.GetListOfConfigurationForProject(quoteId, configVariables,sessionId);
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.LISTOFCONFIGURATION, Utility.SerializeObjectValue(lstConfiguration));
            var defaultValue = _configure.GetDefaultUnitValues(sessionId);
            var unitDefaultValue = defaultValue.Result;
            foreach (var buildings in lstConfiguration)
            {
                foreach (var group in buildings.Groups)
                {
                    if (Utility.CheckEquals(group.productCategory, Constants.ELEV))
                    {
                        foreach (var unit in group.Units)
                        {

                            if (Utility.CheckEquals(unit.Product, Constant.EVOLUTION_200)||Utility.CheckEquals(unit.Product, Constant.PRODUCTNAME))
                            {
                                if (string.IsNullOrEmpty(unit.capacity))
                                {
                                    unit.capacity = unitDefaultValue[unitMapperVariablesGeneralInformation[Constant.CAPACITY]].ToString();
                                }
                                if (string.IsNullOrEmpty(unit.speed) || (Utility.CheckEquals(unit.speed, "0")))
                                {
                                    unit.speed = unitDefaultValue[unitMapperVariablesGeneralInformation[Constant.SPEED]].ToString();
                                }
                            }
                        }
                    }
                }
            }
            var roleName = _configure.GetRoleName(sessionId);
            var lstPermissions = _buildingDl.GetPermissionForConfiguration(quoteId, roleName);
            List<string> permissionAdd = (from objpermission in lstPermissions
                                          where objpermission.BuildingStatus.Equals(string.Empty) &&
                                                objpermission.GroupStatus.Equals(string.Empty) &&
                                                objpermission.UnitStatus.Equals(string.Empty) &&
                                                objpermission.Entity.Equals(Constant.PERMISSIONTYPEBUILDING)
                                          select objpermission.PermissionKey).Distinct().ToList();
            foreach (var configuration in lstConfiguration)
            {
                var permissionBuilding = (from objpermission in lstPermissions
                                          where objpermission.BuildingStatus.Equals(configuration.BuildingStatus.StatusName) &&
                                          objpermission.GroupStatus.Equals(string.Empty) &&
                                          objpermission.UnitStatus.Equals(string.Empty) &&
                                          (objpermission.Entity.Equals(Constant.PERMISSIONTYPEBUILDING) || objpermission.Entity.Equals("Group"))
                                          select objpermission.PermissionKey).Distinct().ToList();
                configuration.Permissions = permissionBuilding;
                foreach (var groups in configuration.Groups)
                {
                    var permissionGroup = (from objpermission in lstPermissions
                                           where objpermission.BuildingStatus.Equals(configuration.BuildingStatus.StatusName) &&
                                                 objpermission.GroupStatus.Equals(groups.groupStatus.StatusName) &&
                                                 objpermission.UnitStatus.Equals(string.Empty) &&
                                                 objpermission.Entity.Equals(Constant.PERMISSIONTYPEGROUP)
                                           select objpermission.PermissionKey).Distinct().ToList();
                    groups.Permissions = permissionGroup;
                    foreach (var units in groups.Units)
                    {
                        var permissionUnit = (from objpermission in lstPermissions
                                              where objpermission.BuildingStatus.Equals(configuration.BuildingStatus.StatusName) &&
                                                    objpermission.GroupStatus.Equals(groups.groupStatus.StatusName) &&
                                                    (objpermission.UnitStatus.Equals(units.Status.StatusName) || objpermission.UnitStatus.Equals(string.Empty)) &&
                                                    objpermission.Entity.Equals(Constant.PERMISSIONTYPEUNIT)
                                              select objpermission.PermissionKey).Distinct().ToList();
                        units.Permissions = permissionUnit;
                    }
                }
            }
            var objConfiguration = new ConfigurationScreen()
            {
                ListOfConfiguration = lstConfiguration,
                Permissions = permissionAdd

            };
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(objConfiguration)) };
        }

        /// <summary>
        /// GetBuildingConfigurationById
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="cr"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetBuildingConfigurationById(int buildingId, JObject variableAssignments, string sessionId)
        {
            var methodStartTime = Utility.LogBegin();
            ConfigurationRequest cr = CreateBuildingConfigurationRequest(variableAssignments);
            //Call to DB
            List<ConfigVariable> lstConfigureVariable = _buildingDl.GetBuildingConfigurationById(buildingId);
            //Converting ConfigureVariable to VariableAssignments
            List<VariableAssignment> lstvariableassignment = lstConfigureVariable.Select(
                variableAssignment => new VariableAssignment
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<VariableAssignment>();
            cr.Line.VariableAssignments = lstvariableassignment;
            // added required variables assignments in the request
            variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(cr.Line));
            //Sending the Variable Assignment Value into VTPKG
            Utility.LogDebug(Constant.GETBUILDINGCONFIGDLINITIATE);
            var (response, configureresponse) = await _configure.ChangeBuildingConfigure(variableAssignments, sessionId).ConfigureAwait(false);
            var EndTime2 = DateTime.Now;
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = response };
        }

        /// <summary>
        /// Constructor class for SaveBuildingConfigurationForProject
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="userId"></param>
        /// <param Name="projectId"></param>
        /// <param Name="BuildingName"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveBuildingConfigurationForProject(int buildingId, string sessionId, string quoteId, JObject varibleAssignments)
        {
            var methodStartTime = Utility.LogBegin();
            var buildingContantsDictionary = Utility.GetVariableMapping(Constant.BUILDINGMAPPERVARIABLESMAPPERPATH, Constant.BUILDINGMAPPERCONFIGURATION);
            var buildingData = Utility.DeserializeObjectValue<Line>(varibleAssignments.ToString()).VariableAssignments;
            var isEditFlow = ConflictsStatus.Valid;
            // get the conflict values from cache
            var hasConflictsFlag = _configure.GetCacheValuesForConflictManagement(sessionId, Constant.BUILDING);
            // get the cache response
            var getVariables = _configure.GetCacheVariablesForConflictChanges(null, sessionId);
            var listOfChangedVariables = _configure.CheckConflict(buildingData.ToList(), getVariables);
            if (listOfChangedVariables.Any())
            {
                var crossPackageVariableId = (JObject.Parse(File.ReadAllText(Constant.CROSSPACKAGEVARIABLEMAPPING)));
                var crossPackageVariables = crossPackageVariableId[Constant.GROUPTOBUILDINGCROSSPACKAGEVARIABLE];
                var crossPackageVariableDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(crossPackageVariables));
                foreach (var crossVariable in crossPackageVariableDictionary)
                {
                    foreach (var buildingVariable in listOfChangedVariables)
                    {
                        if (Utility.CheckEquals(crossVariable.Value, buildingVariable.VariableId))
                        {
                            isEditFlow = ConflictsStatus.NeedValidation;
                        }
                    }
                }
                crossPackageVariables = crossPackageVariableId[Constant.BUILDINGTOUNITCROSSPACKAGEVARIABLE];
                crossPackageVariableDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(crossPackageVariables));
                crossPackageVariableDictionary.Remove(buildingContantsDictionary[Constants.BUILDINGNAMEVARIABLEID]);
                foreach (var buildingVariable in listOfChangedVariables)
                {
                    if (crossPackageVariableDictionary.ContainsKey(buildingVariable.VariableId))
                    {
                        isEditFlow = ConflictsStatus.NeedValidation;
                    }
                }
            }
            List<ConfigVariable> lstVariableAssignment = buildingData.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();
            //True and False Conditions
            foreach (var variableAssigment in lstVariableAssignment)
            {
                if (variableAssigment.Value.Equals(true))
                {
                    variableAssigment.Value = Constant.TRUEVALUES;
                }
                else if (variableAssigment.Value.Equals(false))
                {
                    variableAssigment.Value = Constant.FALSEVALUES;
                }
            }
            List<ConfigVariable> buildingNameList = lstVariableAssignment.Where(oh => oh.VariableId.Contains(buildingContantsDictionary[Constant.BUILDINGNAMEVARIABLEID])).Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();
            var buildingName = string.Empty;
            if (buildingNameList.Count > 0)
            {
                buildingName = buildingNameList[buildingNameList.Count() - 1].Value.ToString();
            }
            //Convert List to Json
            var bldVariableJson = JsonConvert.SerializeObject(lstVariableAssignment);
            var userId = _configure.GetUserId(sessionId);
            //creating variableMapper to send to StoredProcedures
            List<ConfigVariable> mapperVariables = new List<ConfigVariable>();
            mapperVariables.Add(new ConfigVariable()
            {
                VariableId = Constant.BUILDINGNAMEVARIABLEID,
                Value = buildingContantsDictionary[Constant.BUILDINGNAMEVARIABLEID]
            });
            mapperVariables.Add(new ConfigVariable()
            {
                VariableId = Constant.TOTALBUILDINGFLOORTOFLOORHEIGHT,
                Value = buildingContantsDictionary[Constant.TOTALBUILDINGFLOORTOFLOORHEIGHT]
            });
            List<Result> result = _buildingDl.SaveBuildingConfigurationForProject(buildingId, userId, quoteId, buildingName, bldVariableJson, isEditFlow, hasConflictsFlag, mapperVariables);
            var response = JArray.FromObject(result);
            // Adding the conflicts
            List<Result> conflictResponse = _configure.SaveConflictsValues(buildingId,listOfChangedVariables, Constants.BUILDINGENTITY);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodStartTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else if (result[0].result == -2)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.BUILDINGSAVEISSUES,
                    Description = Constant.BUILDINGSAVEISSUES
                });
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.BUILDINGNAMEALREADYEXISTS,
                    Description = Constant.BUILDINGNAMEALREADYEXISTS
                }); ; ;
            }

        }

        /// <summary>
        /// Constructor class for SaveBuildingElevation
        /// </summary>
        /// <param Name="buildingElevation"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveBuildingElevation(BuildingElevation buildingElevation)
        {
            var methodStartTime = Utility.LogBegin();
            List<ResultElevation> resList = new List<ResultElevation>();
            ResultElevation res = new ResultElevation();
            var query = buildingElevation.buildingElevation.GroupBy(x => x.floorDesignation)
                .Where(g => g.Count() > 1)
                .Select(y => new { Element = y.Key, Counter = y.Count() })
                .ToList();
            if (query.Count > 0)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.FLOORDESIGNATIONREPEATING,
                    Description = Constant.FLOORDESIGNATIONREPEATINGDESCRIPTION
                });
            }
            if (buildingElevation.buildingConfigurationId.Equals(0) || string.IsNullOrEmpty(buildingElevation.createdBy.UserId) || (buildingElevation.buildingElevation.Count().Equals(0)))
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Description = Constant.SOMEFIELDSAREMISSING
                });
            }
            buildingElevation.modifiedBy = new User()
            {
                UserId = ""
            };
            DataTable dtBuildingElevation = Utility.GetBuildingElevationDataTable(buildingElevation);
            List<Result> result = _buildingDl.SaveBuildingElevation(dtBuildingElevation);
            result[0].buildingId = buildingElevation.buildingConfigurationId;
            var response = JArray.FromObject(result);
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
        }

        /// <summary>
        /// Constructor class for UpdateBuildingElevation
        /// </summary>
        /// <param Name="buildingElevation"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> UpdateBuildingElevation(BuildingElevation buildingElevation)
        {
            var methodStartTime = Utility.LogBegin();
            var buildingContantsDictionary = Utility.GetVariableMapping(Constant.BUILDINGMAPPERVARIABLESMAPPERPATH, Constant.BUILDINGMAPPERCONFIGURATION);
            string Message = string.Empty;
            List<ResultElevation> resList = new List<ResultElevation>();
            ResultElevation res = new ResultElevation();
            var query = buildingElevation.buildingElevation.GroupBy(x => x.floorDesignation)
                .Where(g => g.Count() > 1)
                .Select(y => new { Element = y.Key, Counter = y.Count() })
                .ToList();
            if (query.Count > 0)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.FLOORDESIGNATIONREPEATING,
                    Description = Constant.FLOORDESIGNATIONREPEATINGDESCRIPTION
                });

            }
            if (buildingElevation.buildingConfigurationId.Equals(0) || string.IsNullOrEmpty(buildingElevation.modifiedBy.UserId) || (buildingElevation.buildingElevation.Count().Equals(0)))
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.SOMEFIELDSAREMISSING,
                    Description = Constant.SOMEFIELDSAREMISSINGDESCRIPTION
                });
            }
            DataTable dtBuildingElevation = Utility.GetBuildingElevationDataTable(buildingElevation);
            //creating variableMapper to send to StoredProcedures
            List<ConfigVariable> mapperVariables = new List<ConfigVariable>();
            mapperVariables.Add(new ConfigVariable()
            {
                VariableId = Constant.BUILDINGBLANDINGS,
                Value = buildingContantsDictionary[Constant.BUILDINGBLANDINGS]
            });
            mapperVariables.Add(new ConfigVariable()
            {
                VariableId = Constant.TOTALBUILDINGFLOORTOFLOORHEIGHT,
                Value = buildingContantsDictionary[Constant.TOTALBUILDINGFLOORTOFLOORHEIGHT]
            });
            List<ConfigVariable> mapperVariablesForConflicts = new List<ConfigVariable>();

            mapperVariablesForConflicts.Add(new ConfigVariable()
            {
                Value = string.Empty,
                VariableId = buildingContantsDictionary[Constant.BUILDINGBLANDINGS]
            });
            mapperVariablesForConflicts.Add(new ConfigVariable()
            {
                Value = string.Empty,
                VariableId = buildingContantsDictionary[Constant.TOTALBUILDINGFLOORTOFLOORHEIGHT]
            });
            var mainEgressFloor = buildingElevation.buildingElevation.Where(x => x.mainEgress.Equals(true)).Select(x => x.FloorNumber)?.FirstOrDefault();
            var alterEgressFloor = buildingElevation.buildingElevation.Where(x => x.alternateEgress.Equals(true)).Select(x => x.FloorNumber)?.FirstOrDefault();
            mapperVariablesForConflicts.Add(new ConfigVariable()
            {
                Value = mainEgressFloor != null ? mainEgressFloor : 0,
                VariableId = Constant.MAINEGRESS
            }); ;
            mapperVariablesForConflicts.Add(new ConfigVariable()
            {
                Value = alterEgressFloor != null ? alterEgressFloor : 0,
                VariableId = Constant.ALTERNATEEGRESS
            });
            var variableAssignmentsForConflicts = Utility.DeserializeObjectValue<List<VariableAssignment>>(Utility.SerializeObjectValue(mapperVariablesForConflicts));
            List<Result> conflictResponse = _configure.SaveConflictsValues(buildingElevation.buildingConfigurationId, variableAssignmentsForConflicts, Constants.BUILDINGENTITY);
            List<Result> result = _buildingDl.UpdateBuildingElevation(dtBuildingElevation, mapperVariables);
            result[0].buildingId = buildingElevation.buildingConfigurationId;

            var response = JArray.FromObject(result);
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
        }

        /// <summary>
        /// Constructor class for AutoSaveBuildingElevation
        /// </summary>
        /// <param Name="buildingElevation"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> AutoSaveBuildingElevation(BuildingElevation buildingElevation)
        {
            var methodStartTime = Utility.LogBegin();
            int msg;
            string Message = string.Empty;
            List<ResultElevation> resList = new List<ResultElevation>();
            ResultElevation res = new ResultElevation();
            if ((buildingElevation.buildingConfigurationId.Equals("")) || (buildingElevation.createdBy.UserId.Equals("")) || (buildingElevation.buildingElevation.Count() == 0))
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Description = Constant.SOMEFIELDSAREMISSING
                }); ;

            }


            buildingElevation.modifiedBy = new User();
            buildingElevation.modifiedBy.UserId = "";

            DataTable dtBuildingElevation = Utility.GetBuildingElevationDataTable(buildingElevation);

            List<Result> result = _buildingDl.AutoSaveBuildingElevation(dtBuildingElevation);

            var response = JArray.FromObject(result);
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
        }

        /// <summary>
        /// GetBuildingElevationById
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetBuildingElevationById(int buildingId, string sessionId)
        {
            var methodStartTime = Utility.LogBegin();
            var buildingContantsDictionary = Utility.GetVariableMapping(Constant.BUILDINGMAPPERVARIABLESMAPPERPATH, Constant.BUILDINGMAPPERCONFIGURATION);
            //creating variableMapper to send to StoredProcedures
            List<ConfigVariable> mapperVariables = new List<ConfigVariable>();
            mapperVariables.Add(new ConfigVariable()
            {
                VariableId = Constant.BUILDINGBLANDINGS,
                Value = buildingContantsDictionary[Constant.BUILDINGBLANDINGS]
            });
            mapperVariables.Add(new ConfigVariable()
            {
                VariableId = Constant.TOTALBUILDINGFLOORTOFLOORHEIGHT,
                Value = buildingContantsDictionary[Constant.TOTALBUILDINGFLOORTOFLOORHEIGHT]
            });
            mapperVariables.Add(new ConfigVariable()
            {
                VariableId = Constant.AVGHEIGHT,
                Value = buildingContantsDictionary[Constant.AVGHEIGHT]
            });
            List<BuildingElevation> lstBuildingElevation = _buildingDl.GetBuildingElevationById(buildingId, mapperVariables, sessionId);
            var enrichedData = JObject.Parse(File.ReadAllText(Constant.BUILDINGENRICHEDDATA));
            ConflictManagement conflictsAssignments = new ConflictManagement();
            var pendingAssignments = new List<ConflictMgmtList>();
            var resolvedAssignments = new List<ConflictMgmtList>();
            if (lstBuildingElevation.Any() && lstBuildingElevation[0].AvgRoofHeight > 0 && lstBuildingElevation[0].AvgRoofHeight < (lstBuildingElevation[0].buildingRiseValue + 10))
            {
                var conflictVariable = new ConflictMgmtList
                {
                    VariableId = buildingContantsDictionary[Constant.AVGHEIGHT],
                    VariableName = buildingContantsDictionary[Constant.AVGHEIGHT]
                };
                pendingAssignments.Add(conflictVariable);
            }
            conflictsAssignments.PendingAssignments = pendingAssignments;
            conflictsAssignments.ResolvedAssignments = resolvedAssignments;
            lstBuildingElevation[0].ConflictAssignments = new ConflictManagement();
            lstBuildingElevation[0].EnrichedData = new JObject();
            lstBuildingElevation[0].ConflictAssignments = conflictsAssignments;
            lstBuildingElevation[0].EnrichedData = enrichedData;
            var roleName = _configure.GetRoleName(sessionId);
            var permission = _buildingDl.GetPermissionByRole(buildingId, roleName);
            lstBuildingElevation[0].Permissions = permission;
            var isEditFlagCached = _cpqCacheManager.GetCache(sessionId, _environment, buildingId.ToString(), Constants.BUILDINGELEVATIONISEDITFLAG);
            if(!String.IsNullOrEmpty(isEditFlagCached))
            {
                lstBuildingElevation[0].IsEditFlag = Utility.DeserializeObjectValue<Int32>(isEditFlagCached) > 0 ? true : false;
            }
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(lstBuildingElevation)) };
        }

        /// <summary>
        /// Constructor class for DeleteBuildingConfigurationById
        /// </summary>
        /// <param Name="buildingConfigurationId"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> DeleteBuildingConfigurationById(int buildingConfigurationId, string userId)
        {
            var methodStartTime = Utility.LogBegin();
            if (IsValidBuildingNumber(buildingConfigurationId))
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Description = Constant.SOMEFIELDSAREMISSING
                }); ;
            }
            var resList = _buildingDl.DeleteBuildingConfigurationById(buildingConfigurationId, userId);
            var response = JArray.FromObject(resList);
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
        }

        /// <summary>
        /// Constructor class for DeleteBuildingElevationById
        /// </summary>
        /// <param Name="buildingConfigurationId"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> DeleteBuildingElevationById(int buildingConfigurationId, string userId)
        {
            var methodStartTime = Utility.LogBegin();
            if (IsValidBuildingNumber(buildingConfigurationId))
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Description = Constant.SOMEFIELDSAREMISSING
                });
            }
            var resList = _buildingDl.DeleteBuildingElevationById(buildingConfigurationId, userId);
            var response = JArray.FromObject(resList);
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
        }

        /// <summary>
        /// Constructor class for StartBuildingConfigure
        /// </summary>
        /// <param Name="configurationRequest"></param>
        /// <param Name="projectId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> StartBuildingConfigure(JObject variablaAssignments, string quoteId, int buildingId, string sessionId, bool isReset)
        {
            var methodStartTime = Utility.LogBegin();
            // get the constant Values 
            var buildingContantsDictionary = Utility.GetVariableMapping(Constant.BUILDINGMAPPERVARIABLESMAPPERPATH, Constant.BUILDINGMAPPERCONFIGURATION);
            var configurationRequest = CreateBuildingConfigurationRequest(variablaAssignments);
            var lstConfigureVariable = new List<ConfigVariable>();
            if (buildingId == 0)
            {
                Thread.Sleep(1000 * 8);
                //genarate the building names
                int buildingCount = _buildingDl.GenerateBuildingName(quoteId);
                string buildingName = "B" + (buildingCount + 1);
                ConfigVariable configVariable = new ConfigVariable();
                configVariable.VariableId = buildingContantsDictionary[Constant.BUILDINGNAMEVARIABLEID];
                configVariable.Value = buildingName;
                lstConfigureVariable.Add(configVariable);
                //Converting ConfigureVariable to VariableAssignments
                List<VariableAssignment> lstvariableassignment = lstConfigureVariable.Select(
                        variableAssignment => new VariableAssignment
                        {
                            VariableId = variableAssignment.VariableId,
                            Value = variableAssignment.Value
                        }).ToList<VariableAssignment>();
                configurationRequest.Line.VariableAssignments = lstvariableassignment;
                _configure.SetCrosspackageVariableAssignments(lstvariableassignment, sessionId, Constant.BUILDINGCONFIGURATION);
                var response = await _configure.StartBuildingConfigure(sessionId, configurationRequest).ConfigureAwait(false);
                // set cache for the variable values
                _configure.GetCacheVariablesForConflictChanges(configurationRequest.Line.VariableAssignments.ToList(), sessionId);
                Utility.LogTrace(Constant.STARTBUILDINGFILTEREDCONFIGRESPONSE + JsonConvert.SerializeObject(response));
                var mainResponse = Utility.DeserializeObjectValue<UIMappingBuildingConfigurationResponse>(Utility.SerializeObjectValue(response.Response));
                var enrichedData = JObject.Parse(File.ReadAllText(Constant.BUILDINGENRICHEDDATA));
                mainResponse.EnrichedData = enrichedData;
                var rolename = _configure.GetRoleName(sessionId);
                var permission = _buildingDl.GetPermissionByRole(-1, rolename);
                mainResponse.Permissions = permission;
                Utility.LogEnd(methodStartTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = Utility.FilterNullValues(mainResponse) };
            }
            else
            {
                //Call to DB
                if (isReset)
                {
                    var buildingName = (from item in _buildingDl.GetBuildingConfigurationById(buildingId)
                                        where item.VariableId.Equals(buildingContantsDictionary[Constant.BUILDINGNAMEVARIABLEID])
                                        select item.Value.ToString()).FirstOrDefault();
                    ConfigVariable configVariable = new ConfigVariable();
                    configVariable.VariableId = buildingContantsDictionary[Constant.BUILDINGNAMEVARIABLEID];
                    configVariable.Value = buildingName;
                    lstConfigureVariable.Add(configVariable);
                }
                else
                {
                    lstConfigureVariable = _buildingDl.GetBuildingConfigurationById(buildingId);
                }
                //Converting ConfigureVariable to VariableAssignments
                List<VariableAssignment> lstvariableassignment = lstConfigureVariable.Select(
                    variableAssignment => new VariableAssignment
                    {
                        VariableId = variableAssignment.VariableId,
                        Value = variableAssignment.Value
                    }).ToList<VariableAssignment>();
                _configure.SetCrosspackageVariableAssignments(lstvariableassignment, sessionId, Constant.BUILDINGCONFIGURATION);
                configurationRequest.Line.VariableAssignments = lstvariableassignment;
                // added required variables assignments in the request
                variablaAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configurationRequest.Line));
                //Sending the Variable Assignment Value into VTPKG
                var (response, configureresponse) = await _configure.ChangeBuildingConfigure(variablaAssignments, sessionId).ConfigureAwait(false);
                // set cache
                _configure.GetCacheVariablesForConflictChanges(configurationRequest.Line.VariableAssignments.ToList(), sessionId);
                response[Constant.READONLY] = _buildingDl.CheckGroupExists(buildingId);
                var mainResponse = Utility.DeserializeObjectValue<UIMappingBuildingConfigurationResponse>(Utility.SerializeObjectValue(response));
                var enrichedData = JObject.Parse(File.ReadAllText(Constant.BUILDINGENRICHEDDATA));
                //var conflictVariables = _buildingDl.GetBuildingConflicts(buildingId);
                //if(conflictVariables != null && conflictVariables.Any())
                //{
                //    var conflictsData = new List<ConflictMgmtList>();
                //    foreach (var requestVariables in configurationRequest.Line.VariableAssignments)
                //    {
                //        foreach (var conflictVariableId in conflictVariables)
                //        {
                //            if (Utility.CheckEquals(requestVariables.VariableId,conflictVariableId))
                //            {
                //                var conflicts = new ConflictMgmtList()
                //                {
                //                    VariableId = conflictVariableId,
                //                    Value = requestVariables.Value
                //                };
                //                conflictsData.Add(conflicts);
                //            }
                //        }
                //    }
                //    mainResponse.ConflictAssignments.PendingAssignments = conflictsData.Distinct().ToList();
                //}
                var rolename = _configure.GetRoleName(sessionId);
                var permission = _buildingDl.GetPermissionByRole(buildingId, rolename);
                mainResponse.Permissions = permission;
                mainResponse.EnrichedData = enrichedData;
                Utility.LogEnd(methodStartTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = Utility.FilterNullValues(mainResponse) };
            }
        }

        /// <summary>
        /// this method is used to create the building configuration stub request body
        /// </summary>
        /// <param Name="varibleAssignments"></param>
        /// <returns></returns>
        public ConfigurationRequest CreateBuildingConfigurationRequest(JObject varibleAssignments)
        {
            var methodStartTime = Utility.LogBegin();
            //creating req body
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
        /// Constructor class for QuickConfigurationSummary
        /// </summary>
        /// <param Name="type"></param>
        /// <param Name="Id"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> QuickConfigurationSummary(string type, string id, string sessionId)
        {
            var methodStartTime = Utility.LogBegin();
            //Call to DB
            QuickSummary quickSummaryList = new QuickSummary();
            var quoteId = string.Empty;
            string versionId = string.Empty;
            var opportunityId = string.Empty;
            var buildingId = 0;
            var groupId = 0;
            var setId = 0;
            switch (type.ToLower())
            {
                case Constant.SUMMARYQUOTEID:
                    quoteId = id;
                    break;
                case Constant.SUMMARYBUILDING:
                    buildingId = Convert.ToInt32(id);
                    break;
                case Constant.SUMMARYGROUP:
                    groupId = Convert.ToInt32(id);
                    break;
                case Constant.SUMMARYSET:
                    setId = Convert.ToInt32(id);
                    break;
            }
            if (!string.IsNullOrEmpty(quoteId) || buildingId > 0 || groupId > 0 || setId > 0)
            {
                quickSummaryList = _buildingDl.QuickConfigurationSummary(quoteId, buildingId, groupId, setId, sessionId);
                if (quickSummaryList != null)
                {
                    if (quickSummaryList.building == null)
                    {
                        quickSummaryList.building = new BuildingDetails();
                    }
                    if (quickSummaryList.group == null)
                    {
                        quickSummaryList.group = new GroupDetails();
                    }
                    if (quickSummaryList.units == null)
                    {
                        quickSummaryList.units = new UnitsTable();
                    }
                    if (quickSummaryList != null && quickSummaryList.project != null)
                    {
                        quoteId = quickSummaryList.project.Id;
                    }
                }
            }
            if (setId > 0)
            {
                var productCategory = _buildingDl.GetProductCategoryBySetId(setId, Constant.SETLOWERCASE);

                if (string.IsNullOrEmpty(productCategory))
                {
                    productCategory = Constant.PRODUCTELEVATOR;
                }

                switch (productCategory)
                {
                    case Constant.PRODUCTELEVATOR:
                        var ceScreenList = JObject.Parse(File.ReadAllText(Constants.UNITCONSTANTMAPPERTEMPLATE));
                        var ceScreenParameter = ceScreenList[Constants.CESCREENLIST].ToList();
                        if (!ceScreenParameter.Contains(quickSummaryList.units.model))
                        {
                            var variablesStubDetails = JObject.Parse(File.ReadAllText(Constant.UNITVARIABLESFORQUICKSUMMARY)).ToString();
                            if(quickSummaryList.units.model.Equals(Constants.ENDURA100_PRODUCT))
                            {
                                variablesStubDetails = JObject.Parse(File.ReadAllText(Constant.UNITVARIABLESFORQUICKSUMMARYENDURA)).ToString();
                            }
                            var value = Utility.DeserializeObjectValue<JObject>(variablesStubDetails);
                            var variablesDetails = Utility.DeserializeObjectValue<List<VariablesDetails>>(value[Constant.VARIABLEVALUES].ToString());
                            if (quickSummaryList.units.unitDetails.dimensionSelection.Equals(Constants.MINIMUM) || quickSummaryList.units.unitDetails.dimensionSelection.Equals(Constants.MAXIMUM))
                            {
                                var hoistwayGetCache = _configure.SetCacheHoistwayDimensions(null, sessionId, setId);
                                if (hoistwayGetCache != null && hoistwayGetCache.Any())
                                {
                                    var hoistwayCapacity = (from capacity in hoistwayGetCache where capacity.VariableId.Contains(Constant.CAPACITYPARAMETER) select capacity.Value).FirstOrDefault().ToString();
                                    var hoistwayCarSpeed = (from carspeed in hoistwayGetCache where carspeed.VariableId.Contains(Constant.CARSPEEDPARAMETER) select carspeed.Value).FirstOrDefault().ToString();
                                    var hoistwayWidth = (from width in hoistwayGetCache where width.VariableId.Contains(Constant.HYDWIDPARAMETER) select width.Value).FirstOrDefault().ToString();
                                    var hoistwayDepth = (from depth in hoistwayGetCache where depth.VariableId.Contains(Constant.HYDDEPPARAMETER) select depth.Value).FirstOrDefault().ToString();
                                    var hoistwayPitDepth = (from pitdepth in hoistwayGetCache where pitdepth.VariableId.Contains(Constant.PITDEPTHPARAMETER) select pitdepth.Value).FirstOrDefault().ToString();
                                    var hoistwayOvHead = (from overhead in hoistwayGetCache where overhead.VariableId.Contains(Constant.OVHEADPARAMETER) select overhead.Value).FirstOrDefault().ToString();
                                    if (!string.IsNullOrEmpty(hoistwayCapacity))
                                    {
                                        quickSummaryList.units.unitDetails.capacity = hoistwayCapacity;
                                    }
                                    if (!string.IsNullOrEmpty(hoistwayCarSpeed))
                                    {
                                        quickSummaryList.units.unitDetails.speed = hoistwayCarSpeed;
                                    }
                                    if (!string.IsNullOrEmpty(hoistwayWidth))
                                    {
                                        quickSummaryList.units.unitDetails.width = hoistwayWidth;
                                    }
                                    if (!string.IsNullOrEmpty(hoistwayDepth))
                                    {
                                        quickSummaryList.units.unitDetails.depth = hoistwayDepth;
                                    }
                                    if (!string.IsNullOrEmpty(hoistwayPitDepth))
                                    {
                                        quickSummaryList.units.unitDetails.pitDepth = hoistwayPitDepth;
                                    }
                                    if (!string.IsNullOrEmpty(hoistwayOvHead))
                                    {
                                        quickSummaryList.units.unitDetails.overHead = hoistwayOvHead;
                                    }
                                }
                            }
                            if (quickSummaryList.units.unitDetails.dimensionSelection.Equals(Constants.CUSTOM) && Utility.Equals(quickSummaryList.units.unitDetails.pitDepth, String.Empty) && quickSummaryList.units.model.Equals(Constants.EVO100_PRODUCT))
                            {
                                var hoistwayGetCache = _configure.SetCacheHoistwayDimensions(null, sessionId, setId);
                                if (hoistwayGetCache != null && hoistwayGetCache.Any())
                                {
                                    var hoistwayCapacity = (from capacity in hoistwayGetCache where capacity.VariableId.Contains(Constant.CAPACITYPARAMETER) select capacity.Value).FirstOrDefault().ToString();
                                    var hoistwayCarSpeed = (from carspeed in hoistwayGetCache where carspeed.VariableId.Contains(Constant.CARSPEEDPARAMETER) select carspeed.Value).FirstOrDefault().ToString();
                                    var hoistwayPitDepth = (from pitdepth in hoistwayGetCache where pitdepth.VariableId.Contains(Constant.PITDEPTHPARAMETER) select pitdepth.Value).FirstOrDefault().ToString();
                                    if (!string.IsNullOrEmpty(hoistwayCapacity))
                                    {
                                        quickSummaryList.units.unitDetails.capacity = hoistwayCapacity;
                                    }
                                    if (!string.IsNullOrEmpty(hoistwayCarSpeed))
                                    {
                                        quickSummaryList.units.unitDetails.speed = hoistwayCarSpeed;
                                    }
                                    if (!string.IsNullOrEmpty(hoistwayPitDepth))
                                    {
                                        quickSummaryList.units.unitDetails.pitDepth = hoistwayPitDepth;
                                    }
                                }
                            }
                            if (quickSummaryList.units.unitDetails.dimensionSelection.Equals(Constants.CUSTOM) && Utility.Equals(quickSummaryList.units.unitDetails.width, String.Empty) && quickSummaryList.units.model.Equals(Constants.ENDURA100_PRODUCT))
                            {
                                var hoistwayGetCache = _configure.SetCacheHoistwayDimensions(null, sessionId, setId);
                                if (hoistwayGetCache != null && hoistwayGetCache.Any())
                                {
                                    var hoistwayCapacity = (from capacity in hoistwayGetCache where capacity.VariableId.Contains(Constant.CAPACITYPARAMETER) select capacity.Value).FirstOrDefault().ToString();
                                    var hoistwayCarSpeed = (from carspeed in hoistwayGetCache where carspeed.VariableId.Contains(Constant.CARSPEEDPARAMETER) select carspeed.Value).FirstOrDefault().ToString();
                                    var hoistwayWidth = (from width in hoistwayGetCache where width.VariableId.Contains(Constant.HYDWIDPARAMETER) select width.Value).FirstOrDefault().ToString();
                                    if (!string.IsNullOrEmpty(hoistwayCapacity))
                                    {
                                        quickSummaryList.units.unitDetails.capacity = hoistwayCapacity;
                                    }
                                    if (!string.IsNullOrEmpty(hoistwayCarSpeed))
                                    {
                                        quickSummaryList.units.unitDetails.speed = hoistwayCarSpeed;
                                    }
                                    if (!string.IsNullOrEmpty(hoistwayWidth))
                                    {
                                        quickSummaryList.units.unitDetails.width = hoistwayWidth;
                                    }
                                }
                            }
                            if (variablesDetails != null && variablesDetails.Count > 0)
                            {
                                if (Convert.ToBoolean(!quickSummaryList?.units?.model.Equals(Constants.ENDURA100_PRODUCT)))
                                {
                                    foreach (var variable in variablesDetails)
                                    {
                                        variable.value = string.Empty;
                                        switch (variable.name.ToUpper())
                                        {
                                            case Constant.CAPACITY:
                                                variable.value = quickSummaryList.units.unitDetails.capacity;
                                                break;
                                            case Constant.SPEED:
                                                variable.value = quickSummaryList.units.unitDetails.speed;
                                                break;
                                            case Constant.WIDTH:
                                                variable.value = quickSummaryList.units.unitDetails.width;
                                                break;
                                            case Constant.DEPTH:
                                                variable.value = quickSummaryList.units.unitDetails.depth;
                                                if (string.IsNullOrEmpty(variable.value))
                                                {
                                                    var hoistwayGetCache = _configure.SetCacheHoistwayDimensions(null, sessionId, setId);
                                                    if (hoistwayGetCache != null && hoistwayGetCache.Any())
                                                    {
                                                        var hoistwayWidth = (from width in hoistwayGetCache where width.VariableId.Contains(Constant.HYDDEPPARAMETER) select width.Value)?.FirstOrDefault().ToString();
                                                        if (!string.IsNullOrEmpty(hoistwayWidth))
                                                        {
                                                            variable.value = hoistwayWidth;
                                                        }
                                                    }
                                                }
                                                break;
                                            case Constant.PITDEPTH:
                                                variable.value = quickSummaryList.units.unitDetails.pitDepth;
                                                break;
                                            case Constant.OVERHEAD:
                                                variable.value = quickSummaryList.units.unitDetails.overHead;
                                                break;
                                            case Constant.MACHINETYPE:
                                                variable.value = quickSummaryList.units.unitDetails.machineType;
                                                break;
                                            case Constant.MOTORTYPESIZE:
                                                variable.value = quickSummaryList.units.unitDetails.motorTypeSize;
                                                break;
                                            case Constant.AVGFINWEIGHT:
                                                variable.value = quickSummaryList.units.unitDetails.availableFinishWeight;
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var variable in variablesDetails)
                                    {
                                        variable.value = string.Empty;
                                        switch (variable.name.ToUpper())
                                        {
                                            case Constant.CAPACITY:
                                                variable.value = quickSummaryList.units.unitDetails.capacity;
                                                break;
                                            case Constant.SPEED:
                                                variable.value = quickSummaryList.units.unitDetails.speed;
                                                break;
                                            case Constant.WIDTH:
                                                variable.value = quickSummaryList.units.unitDetails.width;
                                                break;
                                            case Constant.DEPTH:
                                                variable.value = quickSummaryList.units.unitDetails.depth;
                                                if (string.IsNullOrEmpty(variable.value))
                                                {
                                                    var hoistwayGetCache = _configure.SetCacheHoistwayDimensions(null, sessionId, setId);
                                                    if (hoistwayGetCache != null && hoistwayGetCache.Any())
                                                    {
                                                        var hoistwayWidth = (from width in hoistwayGetCache where width.VariableId.Contains(Constant.HYDDEPPARAMETER) select width.Value)?.FirstOrDefault().ToString();
                                                        if (!string.IsNullOrEmpty(hoistwayWidth))
                                                        {
                                                            variable.value = hoistwayWidth;
                                                        }
                                                    }
                                                }
                                                break;
                                            case Constant.PITDEPTH:
                                                variable.value = quickSummaryList.units.unitDetails.pitDepth;
                                                break;
                                            case Constant.OVERHEAD:
                                                variable.value = quickSummaryList.units.unitDetails.overHead;
                                                break;
                                            case Constant.MACHINETYPE:
                                                variable.value = quickSummaryList.units.unitDetails.machineType;
                                                break;
                                            case Constant.MOTORTYPESIZE:
                                                variable.value = quickSummaryList.units.unitDetails.motorTypeSize;
                                                break;
                                            case Constant.AVGFINWEIGHT:
                                                variable.value = quickSummaryList.units.unitDetails.availableFinishWeight;
                                                break;
                                            case Constant.GROSSLOADJACK:
                                                variable.value = quickSummaryList.units.unitDetails.grossLoadOnJacks;
                                                break;
                                            case Constant.GROSSLOADPOWERUNIT:
                                                variable.value = quickSummaryList.units.unitDetails.grossLoadOnPowerUnit;
                                                break;
                                        }
                                    }
                                }

                                quickSummaryList.units.unitConfigurationDetails = new UnitConfigurationDetails()
                                {
                                    variables = variablesDetails
                                };
                                quickSummaryList.units.unitDetails = null;
                            }
                        }
                        else
                        {
                            quickSummaryList.units.unitConfigurationDetails = null;
                            quickSummaryList.units.unitDetails = null;
                        }
                        break;
                    case Constant.ESCLATORMOVINGWALK:
                        quickSummaryList.units.unitConfigurationDetails = null;
                        quickSummaryList.units.unitLayoutDetails = null;
                        quickSummaryList.units.openingDetails = null;
                        quickSummaryList.units.unitDetails = null;
                        break;
                }

            }
            var noOfBuildings = 0;
            if (quickSummaryList != null)
            {

                if (quickSummaryList.project != null)
                {
                    if (quickSummaryList.project.projectDetails != null)
                    {
                        noOfBuildings = quickSummaryList.project.projectDetails.numberOfBuildings;
                        quoteId = quickSummaryList.project.Id;
                    }
                }
            }
            if (!string.IsNullOrEmpty(quoteId))
            {
                var quoteData = _buildingDl.GetQuoteDetails(quoteId);
                if (quoteData != null && quoteData.Tables.Count > 0)
                {
                    var quoteDetails = (from DataRow row in quoteData.Tables[0].Rows
                                        select new
                                        {
                                            OppId = Convert.ToString(row[Constant.OPPORTUNITYIDCOLUMNNAME]),
                                            versionId = Convert.ToString(row[Constant.VERSIONIDCOLUMNNAME]),
                                            quoteId = Convert.ToString(row[Constant.QUOTEIDIDCOLUMNNAME])
                                        }).Distinct();
                    foreach (var quote in quoteDetails)
                    {
                        opportunityId = quote.OppId;
                        versionId = quote.versionId;
                    }
                    var opportunityDataResponse = await _projectDl.GetProjectDetails(opportunityId, versionId, sessionId).ConfigureAwait(false);
                    quickSummaryList.project = opportunityDataResponse.Response.ToObject<OpportunityDetails>();
                    quickSummaryList.project.projectDetails = new ProjectDetails()
                    {
                        numberOfBuildings = noOfBuildings
                    };
                    quickSummaryList.project.Id = opportunityId;
                    quickSummaryList.project.VersionId = versionId;
                    quickSummaryList.project.OpportunityId = opportunityId;
                    quickSummaryList.project.QuoteId = quoteId;
                }
            }

            var response = Utility.FilterNullValues(JObject.FromObject(quickSummaryList));
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = response };
        }

        /// <summary>
        /// Duplicate Building Configuration By Id
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="quoteId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> DuplicateBuildingConfigurationById(List<int> buildingId, string quoteId)
        {
            var methodStartTime = Utility.LogBegin();
            Result res = new Result();
            List<Result> lstResult = new List<Result>();

            var buildingIDDataTable = Utility.GetBuildingIDDataTable(buildingId);
            var result = _buildingDl.DuplicateBuildingConfigurationById(buildingIDDataTable, quoteId);
            if (result != null && result.Tables.Count > 0)
            {
                foreach (DataTable table in result.Tables)
                {
                    if (table.Columns.Contains(Constant.BUILDINGIDCOLUMNNAME))
                    {
                        var buildingList = (from DataRow row in table.Rows
                                            select new { BuildingId = Convert.ToInt32(row[Constant.BUILDINGIDCOLUMNNAME]), BuildingName = Convert.ToString(row[Constant.BUILDINGNAMECOLUMNNAME]) }).ToList();
                        if (buildingList.Count > 0)
                        {
                            foreach (var building in buildingList)
                            {
                                Result resultBuildingConfiguration = new Result()
                                {
                                    buildingId = building.BuildingId,
                                    message = Constant.BUILDING + building.BuildingName + Constant.CREATEDSUCCESSFULLY,
                                    result = 1
                                };
                                lstResult.Add(resultBuildingConfiguration);
                            }
                        }
                    }
                }
            }
            var responseArray = JArray.FromObject(lstResult);
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = responseArray };
        }

        /// <summary>
        /// Constructor for GetBuildingConfigurationSectionTab
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetBuildingConfigurationSectionTab(int buildingId)
        {
            var methodStartTime = Utility.LogBegin();
            //Call to DB
            Utility.LogDebug(Constant.GETBUILDINGEQUIPMENTFORPROJECTINITIATEDL);
            bool isDisabled = _buildingDl.GetBuildingConfigurationSectionTab(buildingId);
            // Main Stub 
            var stubMainSubSectionResponse = JObject.Parse(File.ReadAllText(Constant.BUILDINGCONFIGURATIONTABSTUBPATH));
            // setting stub data into an sectionsValues object
            var stubUnitConfigurationMainResponseObj = stubMainSubSectionResponse.ToObject<BuildingConfigurationTab>();
            stubUnitConfigurationMainResponseObj.sections.Where(c => c.id == Constant.BUILDINGEQUIPMENTLOWERCASE).FirstOrDefault().isDisabled = isDisabled;
            var response = JObject.FromObject(stubUnitConfigurationMainResponseObj);
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = response };
        }

        /// <summary>
        /// To Check if BuildingId is Valid
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        private bool IsValidBuildingNumber(int buildingId)
        {
            if (buildingId == 0 || string.Equals(buildingId, string.Empty))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<ResponseMessage> GetListOfConfigurationForQuote(string quoteId, string sessionId)
        {
            var methodStartTime = Utility.LogBegin();
            var listOfConfigurationresponse = await _buildingDl.GetListOfConfigurationForQuote(quoteId, sessionId).ConfigureAwait(false);
            var lstConfiguration = Utility.DeserializeObjectValue<ListofConfigurationForQuote>(Utility.SerializeObjectValue(listOfConfigurationresponse.Response));
            string versionId = lstConfiguration.ProjectDetails.VersionId;
            if (lstConfiguration.ProjectDetails.Id.StartsWith(Constant.SCUSER))
            {
                var viewdata = await _projectDl.GetProjectDetails(lstConfiguration.ProjectDetails.Id, lstConfiguration.ProjectDetails.VersionId, sessionId).ConfigureAwait(false);
                if (viewdata != null)
                {
                    var viewData = Utility.DeserializeObjectValue<OpportunityEntity>(Utility.SerializeObjectValue(viewdata.Response));
                    lstConfiguration.ProjectDetails = new ProjectDetail()
                    {
                        Id = viewData.Id,
                        VersionId = versionId,
                        AccountAddress = new AccountEntity()
                        {
                            AccountId = viewData.AccountAddress?.AccountId,
                            AccountName = viewData.AccountAddress?.AccountName,
                            AccountAddressAddressZipCode = viewData.AccountAddress?.AccountAddressAddressZipCode,
                            AccountAddressCity = viewData.AccountAddress?.AccountAddressCity,
                            AccountAddressCountry = viewData.AccountAddress?.AccountAddressCountry,
                            AccountAddressState = viewData.AccountAddress?.AccountAddressState,
                            AccountAddressStreetAddress = viewData.AccountAddress?.AccountAddressStreetAddress,
                            AccountAddressStreetAddress2 = viewData.AccountAddress?.AccountAddressStreetAddress2
                        },
                        BookingDate = viewData.BookingDate,
                        CreatedDate = viewData.CreatedDate,
                        ProposedDate = viewData.BookingDate,
                        OpportunityName = viewData.OpportunityName,
                        SalesStage = viewData.SalesStage
                    };
                }
            }
            var roleName = _configure.GetRoleName(sessionId);
            var lstPermissions = _buildingDl.GetPermissionForConfiguration(quoteId, roleName);
            List<string> permissionAdd = (from objpermission in lstPermissions
                                          where objpermission.BuildingStatus.Equals(string.Empty) &&
                                                objpermission.GroupStatus.Equals(string.Empty) &&
                                                objpermission.UnitStatus.Equals(string.Empty) &&
                                                objpermission.Entity.Equals(Constant.BUILDING)
                                          select objpermission.PermissionKey).Distinct().ToList();
            foreach (var configuration in lstConfiguration.Configuration.Buildings)
            {
                var permissionBuilding = (from objpermission in lstPermissions
                                          where objpermission.BuildingStatus.Equals(configuration.BuildingStatus) &&
                                          objpermission.GroupStatus.Equals(string.Empty) &&
                                          objpermission.UnitStatus.Equals(string.Empty) &&
                                          (objpermission.Entity.Equals(Constant.BUILDING) || objpermission.Entity.Equals(Constant.GROUP))
                                          select objpermission.PermissionKey).Distinct().ToList();
                configuration.Permissions = permissionBuilding;
                foreach (var groups in configuration.Groups)
                {
                    var permissionGroup = (from objpermission in lstPermissions
                                           where objpermission.BuildingStatus.Equals(configuration.BuildingStatus) &&
                                                 objpermission.GroupStatus.Equals(groups.GroupStatus) &&
                                                 objpermission.UnitStatus.Equals(string.Empty) &&
                                                 objpermission.Entity.Equals(Constant.GROUP)
                                           select objpermission.PermissionKey).Distinct().ToList();
                    groups.Permission = permissionGroup;
                    foreach (var units in groups.Units)
                    {
                        var permissionUnit = (from objpermission in lstPermissions
                                              where objpermission.BuildingStatus.Equals(configuration.BuildingStatus) &&
                                                    objpermission.GroupStatus.Equals(groups.GroupStatus) &&
                                                    objpermission.UnitStatus.Equals(units.Status) &&
                                                    objpermission.Entity.Equals(Constant.UNIT)
                                              select objpermission.PermissionKey).Distinct().ToList();
                        units.Permissions = permissionUnit;
                    }
                }
            }
            lstConfiguration.Configuration.Permissions = new List<string>();
            lstConfiguration.Configuration.Permissions.AddRange(permissionAdd);
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = JObject.FromObject(lstConfiguration) };
        }


    }
}