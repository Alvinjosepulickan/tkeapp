/************************************************************************************************************
************************************************************************************************************
    File Name     :   UnitConfigurationBL.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Configit.Configurator.Server.Common;
using Hangfire;
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
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.CommonModel;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class UnitConfigurationBL : IUnitConfiguration
    {
        #region Variables
        /// <summary>
        /// object IUnitConfigurationDL
        /// </summary>
        private readonly IUnitConfigurationDL _unitConfigurationdl;
        /// <summary>
        /// object IConfigure
        /// </summary>
        private readonly IConfigure _configure;
        /// <summary>
        /// object projectsdl
        /// </summary>
        private readonly IProject _projectDl;
        /// <summary>
        /// object ICacheManager
        /// </summary>
        private readonly ICacheManager _cpqCacheManager;
        /// <summary>
        /// string environment
        /// </summary>
        private readonly string _environment;
        /// <summary>
        /// product selection
        /// </summary>
        private readonly IProductSelection _productSelection;
        #endregion

        /// <summary>
        /// Constructor for GroupLayoutBL
        /// </summary>
        /// <param Name="unitdl"></param>
        /// <param Name="configure"></param>
        /// <param Name="utility"
        public UnitConfigurationBL(IConfigure configure, IUnitConfigurationDL unitdl, ILogger<UnitConfigurationBL> logger, IProject _project, ICacheManager cpqCacheManager, IProductSelection productSelection)
        {
            _configure = configure;
            _unitConfigurationdl = unitdl;
            _projectDl = _project;
            _cpqCacheManager = cpqCacheManager;
            _environment = Constant.DEV;
            _productSelection = productSelection;
            Utility.SetLogger(logger);
        }

        /// <summary>
        ///  Unit configuration start
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="setId"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="sectionTab"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> StartUnitConfigure(JObject variableAssignments, int groupConfigurationId, int setId, string sessionId, string sectionTab, int unitId)
        {
            var methodBegin = Utility.LogBegin();
            var unitCommonMapper = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.UNITCOMMONMAPPER);
            var unitMapperVariablesOtherEquipment = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constants.OTHEREQUIPMENT);
            ConfigurationResponse unitResponse = new ConfigurationResponse();
            List<string> conflictDetails = new List<string>();
            var configurationRequest = CreateUnitConfigurationRequest(variableAssignments);
            var mainCategory = string.Empty;
            var controlLanding = String.Empty;
            var unitMapperVariablesGeneralInformation = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.GENERALINFOMAPPER);
            var unitMapperVariables = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.UNITCOMMONMAPPER);

            var unitContantsDictionary = Utility.GetVariableMapping(Constant.CUSTOMENGINEEREDCONSTANTMAPPERPATH, Constant.CUSTOMENGINEEREDVARIABLES);

            List<ConfigVariable> lstConfigVariable = new List<ConfigVariable>();
            foreach (var variable in unitContantsDictionary)
            {
                ConfigVariable configVariable = new ConfigVariable() { VariableId = variable.Key, Value = variable.Value };
                lstConfigVariable.Add(configVariable);
            }

            var dtVariables = Utility.DeserializeObjectValue<DataTable>(Utility.SerializeObjectValue(lstConfigVariable));


            //Getting the Product category
            var productCategory = _unitConfigurationdl.GetProductCategoryByGroupId(groupConfigurationId, Constant.GROUPLOWERCASE, dtVariables);
            if (string.IsNullOrEmpty(productCategory))
                productCategory = Constant.PRODUCTELEVATOR;


            if (Utility.CheckEquals(productCategory, Constant.PRODUCTELEVATOR))
            {
                var productType = _unitConfigurationdl.GetProductType(setId);
                var entranceConflictExists = _unitConfigurationdl.GetConflictsData(setId).Where(x => x.Contains("entrances")).Count() > 0 ? true : false;
                if (Utility.ElevatorProducts.Contains(productType))
                {
                    //If Products will be Elevator
                    List<UnitNames> lstunits = new List<UnitNames>();
                    var unitDetails = new List<ConfigVariable>();
                    _configure.SetCacheProductType(new List<VariableAssignment> { new VariableAssignment { VariableId = "ProductType", Value = productType } }, sessionId, setId);
                    // to select required configurationtype
                    if (string.IsNullOrEmpty(sectionTab))
                    {
                        sectionTab = Constant.GENERALINFORMATION;
                    }
                    var entranceConfigurations = new List<EntranceConfigurations>();
                    var unitHallFixtureConfigurations = new List<UnitHallFixtures>();
                    var fixtureStrategy = _unitConfigurationdl.GetFixtureStrategy(groupConfigurationId);
                    _cpqCacheManager.SetCache(sessionId, _environment, setId.ToString(), Constants.FIXTURESTRATEGYOFCURRENTSET, fixtureStrategy);
                    if (Convert.ToString(groupConfigurationId) != null)
                    {
                        if (groupConfigurationId != 0)
                        {
                            if (Utility.CheckEquals(sectionTab, Constant.ENTRANCES))
                            {
                                //Setting control landing in cache if entrance conflict exists
                                if (entranceConflictExists)
                                {
                                    var unitDataForSet = _unitConfigurationdl.GetUnitsByGroupId(groupConfigurationId, setId).Select(x => x.Unitid).ToList();
                                    var unitDetailsForProduct = await _productSelection.GetUnitVariableAssignments(unitDataForSet, sessionId, Constants.ENTRANCE).ConfigureAwait(false);
                                    JObject variableAssignmentsForProduct = unitDetailsForProduct.Response;
                                    var _ = await _configure.GetAvailableProducts(variableAssignmentsForProduct, sessionId, null, unitDataForSet, null).ConfigureAwait(false);
                                    controlLanding = _cpqCacheManager.GetCache(_configure.GetUserId(sessionId), string.Join("_", unitDataForSet), "CONTROLLDG");
                                }

                                var username = _configure.GetUserId(sessionId);
                                entranceConfigurations = _unitConfigurationdl.GetEntranceConfiguration(setId, groupConfigurationId, controlLanding, username);

                                entranceConfigurations = _configure.SetCacheEntranceConsoles(entranceConfigurations, sessionId, setId);
                            }
                            if (Utility.CheckEquals(sectionTab, Constant.UNITHALLFIXTURE))
                            {
                                var userName = _configure.GetUserId(sessionId);
                                unitHallFixtureConfigurations = _unitConfigurationdl.GetUnitHallFixturesData(setId, groupConfigurationId, userName, fixtureStrategy, sessionId);
                                unitHallFixtureConfigurations = _configure.SetCacheUnitHallFixtureConsoles(unitHallFixtureConfigurations, sessionId, setId);
                            }

                            var lstConfigureVariable = _unitConfigurationdl.GetUnitConfigurationByGroupId(groupConfigurationId, setId, sectionTab);
                            unitDetails = lstConfigureVariable.listOfConfigVariables;
                            conflictDetails = lstConfigureVariable.conflictListOfStrings;
                            var varRearOpen = lstConfigureVariable.listOfConfigVariables.Where(x => x.VariableId.Contains(Constant.REAROPEN) || x.VariableId.Contains(Constant.FIXTURESTRATEGY_SP)).ToList();
                            lstunits = _unitConfigurationdl.GetUnitsByGroupId(groupConfigurationId, setId);
                            _configure.SetCacheUnitsList(lstunits, sessionId, setId);

                            //needed code, dont remove
                            if (Utility.CheckEquals(sectionTab, Constant.CARFIXTURE))
                            {
                                int totalOpening = 0;
                                var openingsParamList = lstConfigureVariable.listOfConfigVariables.Where(x => x.VariableId.Contains(unitMapperVariables[Constant.OPENFRONT]) || x.VariableId.Contains(unitMapperVariables[Constant.OPENREAR])).ToList();
                                if (openingsParamList.Any() || openingsParamList != null)
                                {
                                    foreach (var variables in openingsParamList)
                                    {
                                        totalOpening += Convert.ToInt32(variables.Value);
                                    }
                                }
                                ConfigVariable totalOpenings = new ConfigVariable();
                                totalOpenings.VariableId = Constant.TOTALOPENINGS_SP;
                                totalOpenings.Value = totalOpening;
                                lstConfigureVariable.listOfConfigVariables.Add(totalOpenings);
                            }

                            //Converting ConfigureVariable to VariableAssignments
                            List<VariableAssignment> lstvariableassignment = lstConfigureVariable.listOfConfigVariables.Select(
                             variableAssignment => new VariableAssignment
                             {
                                 VariableId = variableAssignment.VariableId,
                                 Value = variableAssignment.Value
                             }).ToList<VariableAssignment>();

                            configurationRequest.Line.VariableAssignments = lstvariableassignment;
                        }
                    }
                    //Get all the variableId's
                    var result = _unitConfigurationdl.GetDetailsForTP2SummaryScreen(setId);
                    var listOfsavedvariables = new List<ConfigVariable>();
                    foreach (var item in result.VariableAssignments)
                    {
                        if (item.VariableAssignments != null)
                        {
                            listOfsavedvariables.AddRange(item.VariableAssignments);
                        }
                    }
                    if (Utility.CheckEquals(sectionTab, Constants.TRACTIONHOISTWAYEQUIPMENT))
                    {
                        var additionalAssignments = _unitConfigurationdl.GetDetailsForHoistwayWiring(null, setId, sessionId, unitId);
                        listOfsavedvariables.AddRange(additionalAssignments);
                    }
                    var variableAssignmentz = configurationRequest.Line.VariableAssignments;
                    var completeVariableAssignments = Utility.DeserializeObjectValue<List<VariableAssignment>>(Utility.SerializeObjectValue(_configure.GeneratevariableAssignmentsForCrosspackageDependecy("Unit", listOfsavedvariables)));
                    completeVariableAssignments.AddRange(variableAssignmentz);
                    var travel = result.TravelVariableAssignments;
                    if (travel != null)
                    {
                        completeVariableAssignments.Add(travel);
                    }
                    if (Utility.CheckEquals(sectionTab, Constants.TRACTIONHOISTWAYEQUIPMENT))
                    {
                        var hoistwayVariables = _unitConfigurationdl.GetDetailsForHoistwayWiring(Constant.True, setId, sessionId, unitId);
                        var carposValue = (from val in listOfsavedvariables where val.VariableId.Contains(Constants.CARPOS) select val.Value)?.FirstOrDefault();
                        foreach (var variable in hoistwayVariables)
                        {
                            if (variable.VariableId.Contains(carposValue.ToString()))
                            {
                                variable.Value = Constant.TRUEVALUES;
                            }
                        }
                        var newListHoistway = new List<VariableAssignment> { new VariableAssignment {VariableId=unitCommonMapper[Constants.CONTROLCLOSET],Value=Constants.BELOW}
                        ,new VariableAssignment{VariableId=unitCommonMapper[Constants.HOISTWAYENTRY],Value=Constants.OPPMACH } };
                        var hoistwayWiring = Utility.DeserializeObjectValue<List<VariableAssignment>>(Utility.SerializeObjectValue(hoistwayVariables));
                        hoistwayWiring.AddRange(newListHoistway);
                        completeVariableAssignments.AddRange(hoistwayWiring);
                    }
                    foreach (var val in completeVariableAssignments)
                    {
                        if (val.VariableId.Equals(unitMapperVariablesOtherEquipment[Constants.ADDITIONALWIRING]))
                        {
                            val.Value = Convert.ToDouble(val.Value) * 12;
                        }
                    }
                    if (sectionTab != Constants.GENERALINFORMATIONVALUE && sectionTab != Constants.GENERALINFORMATION)
                    {
                        var valCache = _configure.SetCacheHoistwayDimensions(null, sessionId, setId);
                        if (valCache != null)
                        {
                            completeVariableAssignments.AddRange(valCache);
                        }
                    }
                    var getValues = await _configure.GetDefaultValues(sessionId, Constants.DEFAULTGROUPCONFIGVALUES, Constant.GROUPCONFIGURATIONNAME).ConfigureAwait(false);
                    var configAssignmentsWithDefaults = Utility.DeserializeObjectValue<Line>(Utility.SerializeObjectValue(variableAssignments)).VariableAssignments.ToList();
                    var getDefaultValues = (from val1 in getValues
                                            from val2 in configAssignmentsWithDefaults
                                            where !string.IsNullOrEmpty(val2.VariableId) && !Utility.CheckEquals(val1.VariableId, val2.VariableId)
                                            select val1).Distinct().ToList();
                    foreach (var item in getDefaultValues)
                    {
                        configAssignmentsWithDefaults.Add(item);
                    }
                    var lineVariables = new Line()
                    {
                        VariableAssignments = configAssignmentsWithDefaults
                    };
                    var responseObjValue = _configure.GetCrossPackageVariableDefaultValues(Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(lineVariables)), groupConfigurationId, sessionId);
                    var resulValue = responseObjValue.Result;
                    var configureRequestDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(resulValue);
                    var crossPackageFrontValue = (from value in completeVariableAssignments where value.VariableId.ToLower() == unitMapperVariablesGeneralInformation[Constant.DEFAULTFRONTHANDVARIABLE].ToLower() select value).FirstOrDefault();
                    var crossPackageRearValue = (from value in completeVariableAssignments where value.VariableId.ToLower() == unitMapperVariablesGeneralInformation[Constant.DEFAULTREARHANDVARIABLE].ToLower() select value).FirstOrDefault();
                    if (crossPackageFrontValue == null)
                    {
                        var defaultVal = (from valData in configureRequestDictionary where valData.Key.ToLower() == unitMapperVariablesGeneralInformation[Constant.FRONTHANDVARIABLE].ToLower() select valData).FirstOrDefault();
                        if (defaultVal.Key != null)
                        {
                            var varAssignmentDefault = new VariableAssignment
                            {
                                VariableId = unitMapperVariablesGeneralInformation[Constant.DEFAULTFRONTHANDVARIABLE],
                                Value = defaultVal.Value
                            };
                            completeVariableAssignments.Add(varAssignmentDefault);
                        }

                    }
                    var controlLocation = (from valds in completeVariableAssignments where valds.VariableId.Equals(unitCommonMapper[Constants.PARAMETERCONTROLLOCATION]) select valds)?.FirstOrDefault();
                    var blandingVar = (from valds in completeVariableAssignments where valds.VariableId.Equals(unitCommonMapper[Constants.BLANDINGVARIABLE]) select valds)?.FirstOrDefault();
                    var datvl = (from valds in completeVariableAssignments where valds.VariableId.Equals(unitCommonMapper[Constants.CONTROLROOMQUAD]) select valds)?.FirstOrDefault();
                    if (controlLocation?.Value.ToString() == Constants.CONTROLLOCATIONADJACENT && blandingVar != null)
                    {
                        completeVariableAssignments.Remove(blandingVar);
                        if (datvl == null)
                        {
                            var varAssignmentDefault = new VariableAssignment
                            {
                                VariableId = unitCommonMapper[Constants.CONTROLROOMQUAD],
                                Value = Constants.ADJACENTDEFAULTVALUE
                            };
                            completeVariableAssignments.Add(varAssignmentDefault);

                        }
                    }
                    if (controlLocation?.Value.ToString() == Constants.CONTROLLOCATIONREMOTE)
                    {
                        if (datvl == null)
                        {
                            var varAssignmentDefault = new VariableAssignment
                            {
                                VariableId = unitCommonMapper[Constants.CONTROLROOMQUAD],
                                Value = Constants.REMOTEDEFAULTVALUE
                            };
                            completeVariableAssignments.Add(varAssignmentDefault);
                        }
                    }
                    if (crossPackageRearValue == null)
                    {
                        var defaultVal = (from valData in configureRequestDictionary where valData.Key.ToLower() == unitMapperVariablesGeneralInformation[Constant.REARHANDVARIABLE].ToLower() select valData).FirstOrDefault();
                        if (defaultVal.Key != null)
                        {
                            var varAssignmentDefault = new VariableAssignment
                            {
                                VariableId = unitMapperVariablesGeneralInformation[Constant.DEFAULTREARHANDVARIABLE],
                                Value = defaultVal.Value
                            };
                            completeVariableAssignments.Add(varAssignmentDefault);
                        }
                    }
                    completeVariableAssignments = completeVariableAssignments.GroupBy(variable => variable.VariableId).Select(g => g.First()).ToList();
                    completeVariableAssignments = completeVariableAssignments.Where(variable => !variable.VariableId.Equals(Constants.UNITSINSET)).ToList();
                    _configure.SetCrosspackageVariableAssignments(completeVariableAssignments, sessionId, string.Concat(Constant.UNITCONFIGURATION, setId));
                    configurationRequest.Line.VariableAssignments = completeVariableAssignments;
                    //Getting the List Of Units
                    lstunits = _unitConfigurationdl.GetUnitsByGroupId(groupConfigurationId, setId);
                    variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configurationRequest.Line));
                    var unitHallFixturesConfigurationValues = unitHallFixtureConfigurations.Distinct().ToList();
                    _configure.GetCacheVariablesForConflictChanges(completeVariableAssignments, sessionId);
                    ResponseMessage response = new ResponseMessage();
                    switch (sectionTab.ToUpper())
                    {
                        // Unit Hall Fixtures
                        case Constant.UNITHALLFIXTURE:
                            response = await _configure.StartUnitHallFixtures(sessionId, sectionTab, fixtureStrategy, productType, setId, variableAssignments, lstunits, unitHallFixturesConfigurationValues).ConfigureAwait(false);
                            break;
                        default:

                            response = await _configure.StartUnitConfigure(sessionId, sectionTab, fixtureStrategy, setId, productType, controlLanding, variableAssignments, lstunits, entranceConfigurations, unitHallFixturesConfigurationValues, groupConfigurationId, unitId).ConfigureAwait(false);
                            break;
                    }
                    //response = await _configure.StartUnitConfigure(sessionId, sectionTab, fixtureStrategy, setId, productType, variableAssignments, lstunits, entranceConfigurations, unitHallFixturesConfigurationValues, groupConfigurationId).ConfigureAwait(false);
                    unitResponse = Utility.DeserializeObjectValue<ConfigurationResponse>(Utility.SerializeObjectValue(response.Response));
                    var numberOfVariable = unitDetails.Count();
                    if (Utility.CheckEquals(unitDetails[(numberOfVariable) - 1].VariableId, Constant.NUMBEROFFLOORSUNITPACKAGEVARIABLE))
                    {
                        var variable = new ConflictMgmtList()
                        {
                            VariableId = Constant.NUMBEROFFLOORSUNITPACKAGEVARIABLE,
                            Value = unitDetails[(numberOfVariable) - 1].Value
                        };

                        unitResponse.ConflictAssignments.ResolvedAssignments.Add(variable);
                    }
                    unitResponse.ConfigurationStatus = Constant.ISCOMPLETED;
                    unitResponse.SystemValidateStatus = new Status()
                    {
                        StatusKey = Constant.STATUSKEYUNIT_INC,
                        StatusName = Constant.INCOMPLETESTATUS,
                        DisplayName = Constant.INCOMPLETESTATUS,
                        Description = Constant.INCOMPLETEDESCRIPTIONSTATUS
                    };
                    unitResponse.ConfiguratorStatus = new Status()
                    {
                        StatusKey = Constant.STATUSKEYUNIT_INC,
                        StatusName = Constant.INCOMPLETESTATUS,
                        DisplayName = Constant.INCOMPLETESTATUS,
                        Description = Constant.INCOMPLETEDESCRIPTIONSTATUS
                    };

                    var userid = _configure.GetUserId(sessionId);
                    var systemsValuesDBResult = _unitConfigurationdl.GetLatestSystemsValValues(setId, Constant.INCOMPLETESTATUSCODE, userid, null, Constant.REFRESHTYPE, result.UnitDetails, sessionId);
                    if (!string.IsNullOrEmpty(systemsValuesDBResult?.SystemValidateStatus?.StatusName))
                    {
                        unitResponse.ConflictAssignments.ValidationAssignments = systemsValuesDBResult.ConflictAssignments.ValidationAssignments;
                        unitResponse.SystemValidateStatus = systemsValuesDBResult.SystemValidateStatus;

                        var buildingEquipmentStatusCached = _cpqCacheManager.GetCache(sessionId, _environment, setId.ToString(), Constants.BUILDINGEQUIPMENTSTATUSKEY);
                        if (!String.IsNullOrEmpty(buildingEquipmentStatusCached))
                        {
                            var cachedVariable = Utility.DeserializeObjectValue<JObject>(buildingEquipmentStatusCached)[Constants.BUILDINGSTATUS].ToString();
                            if (Utility.CheckEquals(cachedVariable, Constants.BUILDINGEQUIPMENTCOMPLETESTATUSKEY))
                            {
                                unitResponse.BuildingEquipmentConfigured = true;
                            }
                            else
                            {
                                unitResponse.BuildingEquipmentConfigured = false;
                            }

                        }
                    }

                    _configure.GetConflictCacheValues(sessionId, unitResponse);
                }
                else if (Utility.CustomEngineeredProducts.Contains(productType))
                {
                    //If Products will be NCP
                    List<ConfigVariable> configVariables = _unitConfigurationdl.GetVariableAssignmentsBySetId(setId, dtVariables);
                    if (configVariables.Count() > 0)
                    {
                        //Converting ConfigureVariable to VariableAssignments
                        List<VariableAssignment> lstvariableassignment = configVariables.Select(
                         variableAssignment => new VariableAssignment
                         {
                             VariableId = variableAssignment.VariableId,
                             Value = variableAssignment.Value
                         }).ToList<VariableAssignment>();


                        configurationRequest.Line.VariableAssignments = lstvariableassignment;

                        variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configurationRequest.Line));
                    }
                    unitResponse = await _configure.StartCustomEngineeredProductConfigure(sessionId, productType, variableAssignments, setId);
                }
            }
            else if (Utility.NonConfigurableProducts.Contains(productCategory))
            {
                List<ConfigVariable> configVariables = _unitConfigurationdl.GetVariableAssignmentsBySetId(setId, dtVariables);
                if (configVariables.Count() > 0)
                {
                    //Converting ConfigureVariable to VariableAssignments
                    List<VariableAssignment> lstvariableassignment = configVariables.Select(
                     variableAssignment => new VariableAssignment
                     {
                         VariableId = variableAssignment.VariableId,
                         Value = variableAssignment.Value
                     }).ToList<VariableAssignment>();


                    configurationRequest.Line.VariableAssignments = lstvariableassignment;

                    variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configurationRequest.Line));
                }
                unitResponse = await _configure.StartNonConfigurableProductConfigure(sessionId, productCategory, variableAssignments, setId);
            }
            var conflictVariablesData = new List<ConflictMgmtList>();
            if (conflictDetails != null && conflictDetails.Any())
            {
                foreach (var val1 in conflictDetails)
                {
                    var ConflictList = new ConflictMgmtList();
                    ConflictList.VariableId = val1;
                    ConflictList.Value = string.Empty;
                    conflictVariablesData.Add(ConflictList);
                }
            }
            unitResponse.ConflictAssignments.PendingAssignments = new List<ConflictMgmtList>();
            unitResponse.ConflictAssignments.PendingAssignments.AddRange(conflictVariablesData);

            var rolename = _configure.GetRoleName(sessionId);
            var permission = _unitConfigurationdl.GetPermissionByRole(setId, rolename);
            unitResponse.Permissions = permission;

            Utility.LogEnd(methodBegin);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = Utility.FilterNullValues(unitResponse) };
        }

        /// <summary>
        /// Change Unit configuration 
        /// </summary>
        /// <param name="variableAssignments"></param>
        /// <param name="sessionId"></param>
        /// <param name="sectionTab"></param>
        /// <param name="setId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> ChangeUnitConfigure(JObject variableAssignments, string sessionId, string sectionTab, int setId, int unitId = 0)
        {
            var methodBeginTime = Utility.LogBegin();
            JObject unitResponse = new JObject();

            var unitContantsDictionary = Utility.GetVariableMapping(Constant.CUSTOMENGINEEREDCONSTANTMAPPERPATH, Constant.CUSTOMENGINEEREDVARIABLES);
            List<ConfigVariable> lstConfigVariable = new List<ConfigVariable>();
            foreach (var variable in unitContantsDictionary)
            {
                ConfigVariable configVariable = new ConfigVariable() { VariableId = variable.Key, Value = variable.Value };
                lstConfigVariable.Add(configVariable);
            }
            var configVariables = Utility.DeserializeObjectValue<DataTable>(Utility.SerializeObjectValue(lstConfigVariable));
            //Getting the Product category
            var productCategory = _unitConfigurationdl.GetProductCategoryByGroupId(setId, Constant.SETLOWERCASE, configVariables);
            if (string.IsNullOrEmpty(productCategory))
                productCategory = Constant.PRODUCTELEVATOR;


            if (Utility.CheckEquals(productCategory, Constant.PRODUCTELEVATOR))
            {
                string productType = _unitConfigurationdl.GetProductType(setId);
                if (Utility.ElevatorProducts.Contains(productType))
                {
                    //If Products will be Elevator
                    var response = await _configure.ChangeUnitConfigureBl(variableAssignments, sessionId, sectionTab, setId, unitId);
                    unitResponse = Utility.FilterNullValues(response);
                }
                else if (Utility.CustomEngineeredProducts.Contains(productType))
                {
                    //If Products will be NCP
                    var rolename = _configure.GetRoleName(sessionId);
                    var permissions = _unitConfigurationdl.GetPermissionByRole(setId, rolename);
                    var response = await _configure.StartCustomEngineeredProductConfigure(sessionId, productType, variableAssignments, setId, permissions);
                    unitResponse = Utility.FilterNullValues(response);
                }
            }
            else if (Utility.NonConfigurableProducts.Contains(productCategory))
            {
                var rolename = _configure.GetRoleName(sessionId);
                var permissions = _unitConfigurationdl.GetPermissionByRole(setId, rolename);
                var response = await _configure.StartNonConfigurableProductConfigure(sessionId, productCategory, variableAssignments, setId, permissions);
                unitResponse = Utility.FilterNullValues(response);
            }
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = unitResponse };
        }

        /// <summary>
        /// Method for save cab interior details
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="UnitId"></param>
        /// <param Name="userName"></param>
        /// <param Name="variableAssignments"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveUnitConfiguration(int setId, JObject variableAssignments, string sessionId, int unitId)
        {
            var methodBegin = Utility.LogBegin();
            List<ResultSetConfiguration> resultSetConfigurations = new List<ResultSetConfiguration>();

            var unitContantsDictionary = Utility.GetVariableMapping(Constant.CUSTOMENGINEEREDCONSTANTMAPPERPATH, Constant.CUSTOMENGINEEREDVARIABLES);
            List<ConfigVariable> lstConfigVariable = new List<ConfigVariable>();
            foreach (var variable in unitContantsDictionary)
            {
                ConfigVariable configVariable = new ConfigVariable() { VariableId = variable.Key, Value = variable.Value };
                lstConfigVariable.Add(configVariable);
            }
            var dtVariables = Utility.DeserializeObjectValue<DataTable>(Utility.SerializeObjectValue(lstConfigVariable));
            //Getting the Product category
            var productCategory = _unitConfigurationdl.GetProductCategoryByGroupId(setId, Constant.SETLOWERCASE, dtVariables);
            if (string.IsNullOrEmpty(productCategory))
                productCategory = Constant.PRODUCTELEVATOR;
            var userId = _configure.GetUserId(sessionId);
            var groupData = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            List<ConfigVariable> listOfGroupVariables = groupData.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();

            if (Utility.CheckEquals(productCategory, Constant.PRODUCTELEVATOR))
            {
                string productType = _unitConfigurationdl.GetProductType(setId);
                if (Utility.ElevatorProducts.Contains(productType))
                {
                    //If Products will be Elevator
                    var historyTable = new List<LogHistoryTable>();
                    /**Basic Log**/
                    var carcallcutoutassignment = listOfGroupVariables.Where(x => x.VariableId.Contains(Constant.CARCALLOUT)).Select(y => y.Value).ToList();
                    if (carcallcutoutassignment.Count > 0)
                    {
                        if (carcallcutoutassignment[0].Equals(Constant.NR))
                        {

                            var EntranceAssignment = _unitConfigurationdl.GetCarCallCutoutOpenings(setId);
                            if (EntranceAssignment.IsSaved)
                            {
                                var assignment = Utility.GetLandingOpeningAssignmentSelected(EntranceAssignment.FixtureAssignments);
                            }
                        }
                    }
                    /**Basic Log**/
                    resultSetConfigurations = _unitConfigurationdl.SaveUnitConfigurationDL(setId, listOfGroupVariables, userId, historyTable, unitId);
                }
                else if (Utility.CustomEngineeredProducts.Contains(productType))
                {
                    //If Products will be NCP
                    resultSetConfigurations = _unitConfigurationdl.SaveNonConfigurableUnitConfigurationDL(setId, listOfGroupVariables, userId, dtVariables);
                }
            }
            else if (Utility.NonConfigurableProducts.Contains(productCategory))
            {
                resultSetConfigurations = _unitConfigurationdl.SaveNonConfigurableUnitConfigurationDL(setId, listOfGroupVariables, userId, dtVariables);
            }
            // get the conflicts
            List<Result> conflictResponse = _configure.SaveConflictsValues(setId, Utility.DeserializeObjectValue<List<VariableAssignment>>(Utility.SerializeObjectValue(listOfGroupVariables)), Constants.UNITENTITY);

            var response = JArray.FromObject(resultSetConfigurations);
            if (resultSetConfigurations[0].result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.UNITCONFIGURATIONSAVEERRORMESSAGE,
                    Description = resultSetConfigurations[0].message
                });
            }
        }

        /// <summary>
        /// Save Entrance Configuration
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="entranceConfigurationData"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveEntranceConfiguration(int setId, EntranceConfigurationData entranceConfigurationData, string sessionId, int is_Saved)
        {
            var methodBegin = Utility.LogBegin();
            var userId = _configure.GetUserId(sessionId);
            /**Basic Log**/
            var entranceConsoles = _configure.SetCacheEntranceConsoles(null, sessionId, setId);
            entranceConsoles = entranceConsoles.Where(x => !x.EntranceConsoleId.Equals(0)).ToList();
            var lstConsoleHistory = GetConsoleHistoriesEntrance(entranceConsoles, entranceConfigurationData);
            lstConsoleHistory = lstConsoleHistory.Where(x => !Utility.CheckEquals(x.PresentValue, x.PreviousValue)).ToList();
            List<LogHistoryTable> logHistoryTable = GetLogHistoryTableForConsole(lstConsoleHistory, Constant.ENTRANCES);
            /**Basic Log**/
            var result = _unitConfigurationdl.SaveEntranceConfiguration(setId, entranceConfigurationData, userId, is_Saved, false, logHistoryTable);
            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.UNITCONFIGURATIONSAVEERRORMESSAGE,
                    Description = result[0].message
                });
            }

        }

        /// <summary>
        /// Method for save cab interior details
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="userName"></param>
        /// <param Name="variableAssignments"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> UpdateUnitConfiguration(int setId, JObject variableAssignments, string sessionId, int unitId)
        {
            var methodBegin = Utility.LogBegin();
            List<ConfigVariable> listOfGroupVariables = new List<ConfigVariable>();
            List<LogHistoryTable> historyTable = new List<LogHistoryTable>();
            List<ResultSetConfiguration> result = new List<ResultSetConfiguration>();
            ConflictsStatus isEditFlow = ConflictsStatus.Valid;
            var userId = _configure.GetUserId(sessionId);

            var groupData = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            listOfGroupVariables = groupData.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();

            var unitContantsDictionary = Utility.GetVariableMapping(Constant.CUSTOMENGINEEREDCONSTANTMAPPERPATH, Constant.CUSTOMENGINEEREDVARIABLES);
            List<ConfigVariable> lstConfigVariable = new List<ConfigVariable>();
            foreach (var variable in unitContantsDictionary)
            {
                ConfigVariable configVariable = new ConfigVariable() { VariableId = variable.Key, Value = variable.Value };
                lstConfigVariable.Add(configVariable);
            }
            var dtVariables = Utility.DeserializeObjectValue<DataTable>(Utility.SerializeObjectValue(lstConfigVariable));
            //Getting the Product category
            var productCategory = _unitConfigurationdl.GetProductCategoryByGroupId(setId, Constant.SETLOWERCASE, dtVariables);
            if (string.IsNullOrEmpty(productCategory))
                productCategory = Constant.PRODUCTELEVATOR;


            if (Utility.CheckEquals(productCategory, Constant.PRODUCTELEVATOR))
            {
                string productType = _unitConfigurationdl.GetProductType(setId);
                if (Utility.ElevatorProducts.Contains(productType))
                {
                    //If Products Will be Elevator
                    historyTable = new List<LogHistoryTable>();
                    /**Basic Log**/
                    var carcallcutoutassignment = listOfGroupVariables.Where(x => x.VariableId.Contains(Constant.CARCALLOUT)).Select(y => y.Value).ToList();
                    if (carcallcutoutassignment.Count > 0)
                    {
                        if (carcallcutoutassignment[0].Equals(Constant.NR))
                        {

                            var EntranceAssignment = _unitConfigurationdl.GetCarCallCutoutOpenings(setId);
                            if (EntranceAssignment.IsSaved)
                            {
                                var assignment = Utility.GetLandingOpeningAssignmentSelected(EntranceAssignment.FixtureAssignments);
                                var loghistory = new LogHistoryTable()
                                {
                                    VariableId = Constant.CARCALLCUTOUTKEYSWITCH + "(" + assignment + ")",
                                    PreviuosValue = Constant.True,
                                    UpdatedValue = string.Empty
                                };
                                historyTable.Add(loghistory);
                            }
                        }
                    }
                    /**Basic Log**/
                    var hasConflictsFlag = _configure.GetCacheValuesForConflictManagement(sessionId, Constant.UNIT);

                    // need to get the value for the control location and fixture type
                    // and assign it 

                    if (hasConflictsFlag)
                    {
                        isEditFlow = ConflictsStatus.InValid;
                    }
                    result = _unitConfigurationdl.UpdateUnitConfigurationDL(setId, listOfGroupVariables, userId, isEditFlow, historyTable, unitId);
                }
                else if (Utility.CustomEngineeredProducts.Contains(productType))
                {
                    //If Products will be NCP
                    result = _unitConfigurationdl.SaveNonConfigurableUnitConfigurationDL(setId, listOfGroupVariables, userId, dtVariables);
                }
            }
            else if (Utility.NonConfigurableProducts.Contains(productCategory))
            {
                result = _unitConfigurationdl.SaveNonConfigurableUnitConfigurationDL(setId, listOfGroupVariables, userId, dtVariables);
            }

            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.UNITCONFIGURATIONUPDATEERRORMESSAGE,
                    Description = result[0].message

                }); ;
            }
        }

        /// <summary>
        /// Method for save cab interior details
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveCabInteriorDetails(int groupConfigurationId, string productName, JObject variableAssignments, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            var userId = _configure.GetUserId(sessionId);
            var groupData = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            List<ConfigVariable> listOfGroupVariables = groupData.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();

            var result = _unitConfigurationdl.SaveCabInteriorDetails(groupConfigurationId, productName, listOfGroupVariables, userId);
            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = result[0].message,
                    Description = result[0].message,

                    ResponseArray = response
                }); ;
            }

        }

        /// <summary>
        /// Method for save cab interior details
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> UpdateCabInteriorDetails(int groupConfigurationId, string productName, JObject variableAssignments, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            var userId = _configure.GetUserId(sessionId);
            var groupData = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            List<ConfigVariable> listOfGroupVariables = groupData.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();

            var result = _unitConfigurationdl.UpdateCabInteriorDetails(groupConfigurationId, productName, listOfGroupVariables, userId);
            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = result[0].message,
                    Description = result[0].message,
                    ResponseArray = response
                }); ;
            }

        }

        /// <summary>
        /// Method for save hoistway traction equipment
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveHoistwayTractionEquipment(int groupConfigurationId, string productName, JObject variableAssignments, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            var userId = _configure.GetUserId(sessionId);
            var groupData = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            List<ConfigVariable> listOfGroupVariables = groupData.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();

            //Convert List to Json

            var result = _unitConfigurationdl.SaveHoistwayTractionEquipment(groupConfigurationId, productName, listOfGroupVariables, userId);
            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = result[0].message,
                    Description = result[0].message,
                    ResponseArray = response
                }); ;
            }

        }

        /// <summary>
        /// Method for update hoistway traction equipment
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> UpdateHoistwayTractionEquipment(int groupConfigurationId, string productName, JObject variableAssignments, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            var userId = _configure.GetUserId(sessionId);
            var groupData = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            List<ConfigVariable> listOfGroupVariables = groupData.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();

            var result = _unitConfigurationdl.UpdateHoistwayTractionEquipment(groupConfigurationId, productName, listOfGroupVariables, userId);
            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = result[0].message,
                    Description = result[0].message,
                    ResponseArray = response
                });
            }

        }
        /// <summary>
        ///  method to save general information
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveGeneralInformation(int groupConfigurationId, string productName, JObject variableAssignments, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            var userid = _configure.GetUserId(sessionId);
            var generalInformation = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            List<ConfigVariable> listOfGeneralInformationVariables = generalInformation.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();

            var result = _unitConfigurationdl.SaveGeneralInformation(groupConfigurationId, productName, listOfGeneralInformationVariables, userid);
            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage
                {
                    StatusCode = Constant.SUCCESS,
                    ResponseArray = response
                };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = result[0].message,
                    Description = Constant.BADREQUESTMSG
                }); ;
            }

        }
        /// <summary>
        /// method for updating general information
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> UpdateGeneralInformation(int groupConfigurationId, string productName, JObject variableAssignments, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            var userid = _configure.GetUserId(sessionId);
            var generalInformation = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            List<ConfigVariable> listOfGeneralInformationVariables = generalInformation.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();

            var result = _unitConfigurationdl.UpdateGeneralInformation(groupConfigurationId, productName, listOfGeneralInformationVariables, userid);
            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage
                {
                    StatusCode = Constant.SUCCESS,
                    ResponseArray = response
                };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = result[0].message,
                    Description = Constant.BADREQUESTMSG
                }); ;
            }


        }

        /// <summary>
        /// Save entrances BL method
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveEntrances(int groupConfigurationId, string productName, JObject variableAssignments, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            var userid = _configure.GetUserId(sessionId);
            var entrances = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            List<ConfigVariable> listOfEntranceVariables = entrances.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();

            var result = _unitConfigurationdl.SaveEntrances(groupConfigurationId, productName, listOfEntranceVariables, userid);
            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage
                {
                    StatusCode = Constant.SUCCESS,
                    ResponseArray = response
                };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = result[0].message,
                    Description = Constant.BADREQUESTMSG
                }); ;
            }
        }

        /// <summary>
        /// Save entrances BL method
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> UpdateEntrances(int groupConfigurationId, string productName, JObject variableAssignments, string sessionId)
        {

            var methodBegin = Utility.LogBegin();
            var userid = _configure.GetUserId(sessionId);
            var entrances = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            List<ConfigVariable> listOfEntranceVariables = entrances.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<ConfigVariable>();

            var result = _unitConfigurationdl.UpdateEntrances(groupConfigurationId, productName, listOfEntranceVariables, userid);
            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage
                {
                    StatusCode = Constant.SUCCESS,
                    ResponseArray = response
                };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = result[0].message,
                    Description = Constant.BADREQUESTMSG
                }); ;
            }

        }

        /// <summary>
        /// this method to create group configuration request body
        /// </summary>
        /// <param Name="varibleAssignments"></param>
        /// <returns></returns>
        private ConfigurationRequest CreateUnitConfigurationRequest(JObject varibleAssignments)
        {
            var methodBegin = Utility.LogBegin();
            var stubReqbody = JObject.Parse(System.IO.File.ReadAllText(Constant.UNITCONFIGURATIONREQESTBODYSTUBPATH)).ToString();
            var configurationRequest = Utility.DeserializeObjectValue<ConfigurationRequest>(stubReqbody);
            configurationRequest.Date = DateTime.Now;
            var objLine = Utility.DeserializeObjectValue<Line>(varibleAssignments.ToString());
            configurationRequest.Line.VariableAssignments = objLine.VariableAssignments;
            Utility.LogEnd(methodBegin);
            return configurationRequest;
        }

        /// <summary>
        /// method to edit Unit designation
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="UnitId"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="unit"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> EditUnitDesignation(int groupId, int unitId, string sessionId, UnitDesignation unit)
        {
            var methodBegin = Utility.LogBegin();
            var userid = _configure.GetUserId(sessionId);
            if (string.IsNullOrEmpty(unit.Description) && string.IsNullOrEmpty(unit.Designation))
            {

                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.UNITDESIGNATIONPARAMETERERRORMESSAGE1,
                    Description = Constant.UNITDESIGNATIONPARAMETERERRORDESCRIPTION,
                });


            }
            var result = _unitConfigurationdl.EditUnitDesignation(groupId, unitId, userid, unit);
            if (result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage
                {
                    StatusCode = Constant.SUCCESS
                ,
                    Response = JObject.FromObject(JsonConvert.DeserializeObject(Constant.UNITDESIGNATIONMESSAGE))
                };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.UNITDESCRIPTIONERRORMESSAGE1,
                    Description = Constant.UNITDESCRIPTIONERRORMESSAGE1
                });


            }


        }

        /// <summary> 
        /// method to configure entrance
        /// </summary>
        /// <param Name="consoleId"></param>
        /// <param Name="setId"></param>
        /// <param Name="SessionId"></param>
        /// <param Name="objEntranceConfiguration"></param>
        /// <param Name="isSave"></param>
        /// <param Name="isReset"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> StartEntranceConfigure(int consoleId, int setId, string SessionId, EntranceConfigurationData objEntranceConfiguration = null, bool isSave = false, bool isReset = false)
        {
            var methodBegin = Utility.LogBegin();
            var entranceConsoles = _configure.SetCacheEntranceConsoles(null, SessionId, setId);
            if (entranceConsoles == null)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.CACHEEMPTY
                });
            }
            var newConsole = new EntranceConfigurations();
            //Edit/add entrance configuration console
            if (objEntranceConfiguration == null)
            {
                newConsole = StartEntranceConfigurationConsoleBL(entranceConsoles, consoleId, SessionId, setId);
                if (consoleId == 0)
                {
                    entranceConsoles.Add(newConsole);
                    _configure.SetCacheEntranceConsoles(entranceConsoles, SessionId, setId);
                }
            }
            //change entrance configuration console
            else
            {
                newConsole = ChangeEntranceConfigurationConsoleBL(entranceConsoles, objEntranceConfiguration);
            }
            var res = await _configure.EntranceConsoleConfigureBl(newConsole, SessionId, isSave, setId).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = res
            };

        }

        /// <summary>
        /// GetDetailsForTP2SummaryScreen
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="UnitId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetDetailsForTP2SummaryScreen(int unitId, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            var cr = new ConfigureRequest();
            var unitMapperVariables = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.UNITCOMMONMAPPER);
            var result = _unitConfigurationdl.GetDetailsForTP2SummaryScreen(unitId);
            var manufacturingCommentsTable = (from data in result.FloorMatrixTable
                                              where data.Id.Equals("manufacturingCommentsTable")
                                              select data).ToList();
            var cachedUserDetail = _cpqCacheManager.GetCache(sessionId, _environment, Constant.USERDETAILSCPQ);
            var isViewUser = false;
            if (!string.IsNullOrEmpty(cachedUserDetail))
            {
                isViewUser = Utility.DeserializeObjectValue<User>(cachedUserDetail).IsViewUser;
            }
            // to get the response as expected
            var sectionTab = Constant.SUMMARYTAB;

            // to get the unitConfiguration response
            //for building
            //for group
            var unitVaribaleAssignmentValues = result.VariableAssignments.Where(s => s.Id.Equals(Constant.UNITVARIABLESLIST)).FirstOrDefault();
            var buildingVaribaleAssignmentValues = result.VariableAssignments.Where(s => s.Id.Equals(Constant.BUILDINGVARIABLESLIST)).FirstOrDefault();
            var consoleVariables = (from variables in result.OpeningVariableAssginments
                                    select variables.VariableAssigned);
            foreach (var consoleVar in consoleVariables)
            {
                if (consoleVar.VariableId.Contains(Constant.SALESUIPARAMETER))
                {
                    if (consoleVar.VariableId.Contains(Constant.ELEVATOR))
                    {
                        var id = consoleVar.VariableId.Split(Constant.DOT).ToList().Skip(3);
                        consoleVar.VariableId = Constant.ELEVATOR + Constant.DOT + String.Join(Constant.DOT, id);
                    }
                    else
                    {
                        var id = consoleVar.VariableId.Split(Constant.DOT).ToList().Skip(2);
                        consoleVar.VariableId = Constant.ELEVATOR + Constant.DOT + String.Join(Constant.DOT, id);
                    }
                }
                if (consoleVar.VariableId.Contains(Constant.HALLFINPARAM))
                {
                    consoleVar.VariableId = Constant.HALLFINVARIABLEID;
                }
            }
            foreach (var buildingVariable in buildingVaribaleAssignmentValues.VariableAssignments)
            {
                if (buildingVariable.VariableId.Contains(Constant.BUILDING_CONFIGURATION))
                {
                    buildingVariable.VariableId = buildingVariable.VariableId.Replace(Constant.BUILDING_CONFIGURATION, Constant.ELEVATOR_CONFIGURATION);
                }
            }
            //Converting ConfigureVariable to VariableAssignments

            List<VariableAssignment> lstvariableassignment = unitVaribaleAssignmentValues.VariableAssignments.Select(
                variableAssignment => new VariableAssignment
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<VariableAssignment>();
            lstvariableassignment.AddRange(buildingVaribaleAssignmentValues.VariableAssignments.Select(
                variableAssignment => new VariableAssignment
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<VariableAssignment>()
                );
            lstvariableassignment.AddRange(consoleVariables.Select(
                variableAssignment => new VariableAssignment
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<VariableAssignment>()
                );
            var region = isViewUser ? Constants.NOTEQUALTOCANADA : Constants.CANADACAMELCASE;
            lstvariableassignment.Add(new VariableAssignment { VariableId = unitMapperVariables[Constant.ELEVSYSPARAMETER], Value = Constant.EVO });
            lstvariableassignment.Add(new VariableAssignment { VariableId = unitMapperVariables[Constant.REGIONPARAMETER], Value = region });
            cr.Line = new Line();
            var cachedHoistwayDimensions = _configure.SetCacheHoistwayDimensions(null, sessionId, unitId);
            lstvariableassignment.Add((from variable in cachedHoistwayDimensions
                                      where variable.VariableId.Contains("OVHEAD")
                                      select variable).FirstOrDefault());
            cr.Line.VariableAssignments = lstvariableassignment;

            var lineVariableAssignment = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(cr.Line));
            var unitValues = result.UnitDetails;
            var roleName = _configure.GetRoleName(sessionId);
            var permissions = _unitConfigurationdl.GetPermissionByRole(unitId, roleName, Constants.DISCOUNT_ENTITY);
            var unitResponseObj = await _configure.SummaryUnitConfigureBl(lineVariableAssignment, unitValues, sessionId, sectionTab, result.OpeningVariableAssginments, unitId, result.GroupUnitInfo, result.PriceAndDiscountData, manufacturingCommentsTable).ConfigureAwait(false);
            var response = Utility.DeserializeObjectValue<UnitSummaryUIModel>(Utility.SerializeObjectValue(unitResponseObj));
            if (result.CustomPriceLine.Count > 0)
            {
                decimal totalEstimationPrice = 0;
                decimal totalPrice = 0;
                foreach (var priceLine in result.CustomPriceLine)
                {
                    if (priceLine.PriceValue.ToList().Any())
                    {
                        totalPrice += priceLine.PriceValue.ToList()[0].Value.totalPrice;
                        if (response.PriceValue.ContainsKey(priceLine.PriceValue.ToList()[0].Key))
                        {
                            response.PriceValue[priceLine.PriceValue.ToList()[0].Key] = priceLine.PriceValue.ToList()[0].Value;
                        }
                        else
                        {
                            response.PriceValue.Add(priceLine.PriceValue.ToList()[0].Key, priceLine.PriceValue.ToList()[0].Value);
                        }
                    }
                    foreach (var priceSection in response.PriceSections)
                    {
                        if (Utilities.CheckEquals(priceSection.Section, priceLine.priceKeyInfo.Section))
                        {
                            priceSection.PriceKeyInfo.Add(priceLine.priceKeyInfo);
                        }
                    }
                }
                //Get the base unit details of the set
                response.Units = GetUnitsBasePrice(response.Units, sessionId);
                foreach (var unit in response.Units)
                {
                    unit.Price += totalPrice;
                    totalEstimationPrice = totalEstimationPrice + unit.Price;
                }
                _cpqCacheManager.SetCache(sessionId, _environment, Constants.TOTALCUSTOMPRICE, Convert.ToString(totalPrice));
                if (response.PriceValue.ContainsKey(Constants.TOTALPRICEVALUES))
                {
                    response.PriceValue[Constants.TOTALPRICEVALUES].totalPrice = totalEstimationPrice;
                    response.PriceValue[Constants.TOTALPRICEVALUES].unitPrice = totalEstimationPrice;
                }
            }
            response.Permissions = permissions;
            var corporateAssistance = result?.PriceAndDiscountData?.Where(discountType => Utility.CheckEquals(discountType.VariableForUnit.VariableId, Constants.CORPORATEASSISTANCE)).Select(values => values.VariableForUnit.Value).FirstOrDefault();
            decimal corporateAssistanceValue = string.IsNullOrEmpty(Convert.ToString(corporateAssistance)) ? 0 : Convert.ToDecimal(corporateAssistance);
            var strategicDiscount = result?.PriceAndDiscountData?.Where(discountType => discountType.VariableForUnit.VariableId.Equals(Constants.STRATEGICDISCOUNT)).Select(values => values.VariableForUnit.Value).FirstOrDefault();
            decimal strategicDiscountValue = string.IsNullOrEmpty(Convert.ToString(strategicDiscount)) ? 0 : Convert.ToDecimal(strategicDiscount);
            decimal totalDiscount = corporateAssistanceValue + strategicDiscountValue;
            if (totalDiscount > 0)
            {
                response = AdjustDiscounts(totalDiscount, response);
            }
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.PRICESECTION, Utility.SerializeObjectValue(response));
            await SavePriceForTP2SummaryScreen(unitId, null, sessionId, response.Units);
            Utility.LogEnd(methodBegin);
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Message = Constant.UNITDESIGNATIONMESSAGE,
                Response = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(response))
            };
        }

        /// <summary>
        /// Method to get the base unit details of the set
        /// </summary>
        /// <param name="units"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        private List<UnitNames> GetUnitsBasePrice(List<UnitNames> units, string sessionId)
        {
            var setUnitsDataCache = _cpqCacheManager.GetCache(sessionId, _environment, Constants.SETUNITSDATA);
            if (setUnitsDataCache != null)
            {
                return JsonConvert.DeserializeObject<List<UnitNames>>(setUnitsDataCache);
            }
            else
            {
                return units;
            }
        }

        private UnitSummaryUIModel AdjustDiscounts(decimal totalDiscount, UnitSummaryUIModel response)
        {
            foreach (var mainUnitItems in response.Units)
            {
                var priceWithDiscount = mainUnitItems.Price - totalDiscount;
                mainUnitItems.Price = priceWithDiscount;
            }
            var totalPrice = response.Units.Sum(x => x.Price);
            response.PriceValue[Constants.TOTALPRICEVALUES].totalPrice = totalPrice;
            response.PriceValue[Constants.TOTALPRICEVALUES].unitPrice = totalPrice;
            return response;
        }

        /// <summary>
        /// Save HallLantern Configuration
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="unitHallFixtureConfigurationData"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveUnitHallFixtureConfiguration(int setId, UnitHallFixtureData unitHallFixtureConfigurationData, string sessionId, int is_Saved)
        {
            var methodBegin = Utility.LogBegin();
            var userId = _configure.GetUserId(sessionId);

            /**Adding Switch Variables For Saving in DB**/
            var constantMapper = JObject.Parse(File.ReadAllText(Constant.UNITSVARIABLESMAPPERPATH));
            var productType = _configure.SetCacheProductType(null, sessionId, setId).FirstOrDefault().Value.ToString();
            if (constantMapper[Constant.CONSOLEWITHSWITCHVARIABLES].Select(x => x.ToString()).ToList().Contains(unitHallFixtureConfigurationData.FixtureType))
            {
                var unitHallFixtureConsolesResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITHALLFIXTURECONSOLESPATH, Constant.EVOLUTION200))).ToObject<ConfigurationResponse>();
                if (productType.Equals(Constant.ENDURA_100))
                {
                    unitHallFixtureConsolesResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITHALLFIXTURECONSOLESPATH, Constant.END100))).ToObject<ConfigurationResponse>();
                }
                var switchVariables = (from consoles in unitHallFixtureConsolesResponse.Sections
                                       where consoles.Id.Equals(unitHallFixtureConfigurationData.FixtureType)
                                       select consoles.Variables).FirstOrDefault();
                foreach (var varAssign in switchVariables)
                {
                    if (Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(constantMapper[Constant.SWITCHVARIABLESTOBEREMOVED])).Contains(varAssign.Id))
                    {
                        var varAssignForConsole = new ConfigVariable { VariableId = varAssign.Id, Value = "TRUE" };
                        unitHallFixtureConfigurationData.VariableAssignments.Add(varAssignForConsole);
                    }

                }

            }

            /**Basic Log**/
            var entranceConsoles = _configure.SetCacheUnitHallFixtureConsoles(null, sessionId, setId);
            var lstConsoleHistory = GetConsoleHistoriesUnitHallFixture(entranceConsoles, unitHallFixtureConfigurationData);
            lstConsoleHistory = lstConsoleHistory.Where(x => !Utility.CheckEquals(x.PresentValue, x.PreviousValue)).ToList();
            List<LogHistoryTable> logHistoryTable = GetLogHistoryTableForConsole(lstConsoleHistory, Constant.UNITHALLFIXTURE);
            /**Basic Log**/

            var result = _unitConfigurationdl.SaveUnitHallFixtureConfiguration(setId, unitHallFixtureConfigurationData, userId, is_Saved, logHistoryTable);
            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.UNITCONFIGURATIONSAVEERRORMESSAGE,
                    Description = result[0].message
                });

            }
        }

        /// <summary>
        /// Start Unit HallFixture Configuration
        /// </summary>
        /// <param name="consoleId"></param>
        /// <param name="isChange"></param>
        /// <param name="setId"></param>
        /// <param name="SessionId"></param>
        /// <param name="fixtureSelected"></param>
        /// <param name="isReset"></param>
        /// <param name="objEntranceConfiguration"></param>
        /// <param name="isSave"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> StartUnitHallFixtureConfigure(int consoleId, int isChange, int setId, string SessionId, string fixtureSelected, bool isReset, UnitHallFixtureData objEntranceConfiguration = null, bool isSave = false)
        {
            var methodBegin = Utility.LogBegin();
            var userName = _configure.GetUserId(SessionId);

            var fixtureType = fixtureSelected.Contains(Constant.DOTCHAR) ? fixtureSelected.Split(Constant.DOTCHAR)[3] : fixtureSelected;
            var unitHallFixtureConsoles = _configure.SetCacheUnitHallFixtureConsoles(null, SessionId, setId);
            if (unitHallFixtureConsoles == null)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.CACHEEMPTY
                });
            }
            if (isChange == 0)
            {
                var consoleToRemove = (from console in unitHallFixtureConsoles
                                       where (console.VariableAssignments.Count == 0 || console.UnitHallFixtureLocations == null) && console.IsController == false
                                       && console.ConsoleId != 0
                                       select console).ToList();
                foreach (var console in unitHallFixtureConsoles)
                {
                    var sumOfFront = 0;
                    var sumOfRear = 0;
                    sumOfFront = (from openings in console.UnitHallFixtureLocations
                                  select Convert.ToBoolean(openings.Front.Value) ? 1 : 0).ToList().Sum();
                    sumOfRear = (from openings in console.UnitHallFixtureLocations
                                 select Convert.ToBoolean(openings.Rear.Value) ? 1 : 0).ToList().Sum();
                    var consolesThatCanHaveZeroOpenings = new List<String>();
                    consolesThatCanHaveZeroOpenings.Add("Hall_Lantern");
                    consolesThatCanHaveZeroOpenings.Add("Hall_PI");
                    consolesThatCanHaveZeroOpenings.Add("Combo_Hall_Lantern/PI");
                    consolesThatCanHaveZeroOpenings.Add("Hall_Elevator_Designation_Plate");
                    if (sumOfFront == 0 && sumOfRear == 0 && console.ConsoleId > 0 && !consolesThatCanHaveZeroOpenings.Contains(console.UnitHallFixtureType))
                    {
                        consoleToRemove.Add(console);
                    }
                    if (sumOfFront == 0 && sumOfRear == 0 && console.ConsoleId > 1 && consolesThatCanHaveZeroOpenings.Contains(console.UnitHallFixtureType))
                    {
                        consoleToRemove.Add(console);
                    }
                }
                if (consoleToRemove.FirstOrDefault() != null)
                {
                    foreach (var console in consoleToRemove)
                    {
                        unitHallFixtureConsoles.Remove(console);
                    }
                }
            }

            foreach (var console in unitHallFixtureConsoles)
            {
                if (console.ConsoleId == 0)
                {
                    console.UnitHallFixtureType = fixtureType;
                }
            }

            var lstSelectedFrontFloorNumber = (from varUnitHallFixtureConsole in unitHallFixtureConsoles
                                               where varUnitHallFixtureConsole.UnitHallFixtureType.ToUpper().Equals(fixtureType.ToUpper())
                                               from EntranceLocations in varUnitHallFixtureConsole.UnitHallFixtureLocations
                                               where EntranceLocations.Front.Value.Equals(true) && varUnitHallFixtureConsole.ConsoleId != consoleId
                                               select EntranceLocations.FloorNumber).ToList();
            var lstSelectedRearFloorNumber = (from varUnitHallFixtureConsole in unitHallFixtureConsoles
                                              where varUnitHallFixtureConsole.UnitHallFixtureType.ToUpper().Equals(fixtureType.ToUpper())
                                              from EntranceLocations in varUnitHallFixtureConsole.UnitHallFixtureLocations
                                              where EntranceLocations.Rear.Value.Equals(true) && varUnitHallFixtureConsole.ConsoleId != consoleId
                                              select EntranceLocations.FloorNumber).ToList();
            var newConsole = new UnitHallFixtures();
            if (objEntranceConfiguration == null)
            {
                var maxConsoleId = (from entrance in unitHallFixtureConsoles
                                    where entrance.UnitHallFixtureType == fixtureType
                                    select entrance.ConsoleId).ToList().Max();
                var unitHallFixtureConsole = (from consoles in unitHallFixtureConsoles
                                              where consoles.ConsoleId == 0
                                              select consoles).ToList();
                if (unitHallFixtureConsole != null && unitHallFixtureConsole.Any())
                {
                    var selectedLocations = (from consoles in unitHallFixtureConsoles
                                               where consoles.UnitHallFixtureType == fixtureType
                                               select consoles).ToList().FirstOrDefault().UnitHallFixtureLocations;
                    var newentranceLocation = new List<EntranceLocations>();
                    foreach(var location in selectedLocations)
                    {
                        newentranceLocation.Add(new EntranceLocations
                        {
                            Front = new LandingOpening
                            {
                                Value = location.Front.Value,
                                InCompatible = location.Front.InCompatible,
                                NotAvailable = location.Front.NotAvailable
                            },
                            Rear = new LandingOpening
                            {
                                Value = location.Rear.Value,
                                InCompatible = location.Rear.InCompatible,
                                NotAvailable = location.Rear.NotAvailable
                            },
                            FloorDesignation = location.FloorDesignation,
                            FloorNumber = location.FloorNumber
                        });
                    }

                    foreach (var varLocation in newentranceLocation)
                    {
                        varLocation.Front.InCompatible = false;
                        varLocation.Rear.InCompatible = false;
                        varLocation.Front.Value = false;
                        varLocation.Rear.Value = false;
                        if (lstSelectedFrontFloorNumber.Count > 0)
                        {
                            if (lstSelectedFrontFloorNumber.Contains(varLocation.FloorNumber))
                            {
                                varLocation.Front.InCompatible = true;
                            }

                        }
                        if (lstSelectedRearFloorNumber.Count > 0)
                        {
                            if (lstSelectedRearFloorNumber.Contains(varLocation.FloorNumber))
                            {
                                varLocation.Rear.InCompatible = true;
                            }
                        }

                    }
                    if (consoleId == 0)
                    {
                        var productType = _configure.SetCacheProductType(null, SessionId, setId).FirstOrDefault().Value.ToString();
                        var defaults = JObject.Parse(File.ReadAllText(string.Format(Constants.UNITHALLFIXTURECONSOLEDEFAULTVALUES, Constant.EVOLUTION200))).ToString();
                        if (productType.Equals(Constant.EVO_200))
                        {
                            defaults = JObject.Parse(File.ReadAllText(string.Format(Constants.UNITHALLFIXTURECONSOLEDEFAULTVALUES, Constant.EVOLUTION200))).ToString();
                        }
                        else if (productType.Equals(Constant.ENDURA_100))
                        {
                            defaults = JObject.Parse(File.ReadAllText(string.Format(Constants.UNITHALLFIXTURECONSOLEDEFAULTVALUES, Constants.END100))).ToString();
                        }
                        var defaultUHFVariables = Utility.DeserializeObjectValue<Dictionary<string, Dictionary<string, string>>>(defaults);
                        newConsole.ConsoleId = maxConsoleId + 1;
                        var fixtureName = fixtureType.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE);
                        fixtureName = fixtureName + Constant.EMPTYSPACE;
                        var defaultVariablesDict = (from consoleTypes in defaultUHFVariables
                                                    where consoleTypes.Key.Equals(unitHallFixtureConsole[0].UnitHallFixtureType)
                                                    select consoleTypes.Value).FirstOrDefault();
                        List<ConfigVariable> defaultVarAssignment = new List<ConfigVariable>();
                        if (defaultVariablesDict != null)
                        {
                            foreach (var dictKey in defaultVariablesDict.Keys)
                            {
                                defaultVarAssignment.Add(new ConfigVariable { VariableId = dictKey, Value = defaultVariablesDict[dictKey] });
                            }
                        }

                        newConsole.ConsoleName = newConsole.ConsoleId == 1 ? fixtureName : fixtureName + newConsole.ConsoleId;
                        newConsole.UnitHallFixtureLocations = newentranceLocation;
                        newConsole.AssignOpenings = true;
                        newConsole.IsController = false;
                        newConsole.Openings = unitHallFixtureConsole[0].Openings;
                        newConsole.NoOfFloor = unitHallFixtureConsole[0].NoOfFloor;
                        newConsole.VariableAssignments = defaultVarAssignment;
                        newConsole.UnitHallFixtureType = unitHallFixtureConsole[0].UnitHallFixtureType;
                        if (newConsole.UnitHallFixtureType.Equals(Constant.HOISTWAYACCESS))
                        {
                            var frontFloors = (from varUnitHallFixtureConsole in unitHallFixtureConsoles
                                               where varUnitHallFixtureConsole.UnitHallFixtureType.ToUpper().Equals(fixtureType.ToUpper())
                                               from EntranceLocations in varUnitHallFixtureConsole.UnitHallFixtureLocations
                                               where EntranceLocations.Front.Value.Equals(true) && varUnitHallFixtureConsole.ConsoleId != consoleId
                                               select EntranceLocations.FloorNumber).ToList();
                            var firstFloorFront = frontFloors.Any() ? frontFloors.Min() : -1;
                            var lastFloorFront = frontFloors.Any() ? frontFloors.Max() : -1;
                            var rearFloors = (from varUnitHallFixtureConsole in unitHallFixtureConsoles
                                              where varUnitHallFixtureConsole.UnitHallFixtureType.ToUpper().Equals(fixtureType.ToUpper())
                                              from EntranceLocations in varUnitHallFixtureConsole.UnitHallFixtureLocations
                                              where EntranceLocations.Rear.Value.Equals(true) && varUnitHallFixtureConsole.ConsoleId != consoleId
                                              select EntranceLocations.FloorNumber).ToList();
                            var firstFloorRear = rearFloors.Any() ? rearFloors.Min() : -1;
                            var lastFloorRear = rearFloors.Any() ? rearFloors.Max() : -1;
                            for (int floor = 0; floor < (newConsole.UnitHallFixtureLocations.Count); floor++)
                            {
                                if (!newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(firstFloorFront) && !newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(lastFloorFront))
                                {
                                    newConsole.UnitHallFixtureLocations[floor].Front.NotAvailable = true;
                                }
                                if (!newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(firstFloorRear) && !newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(lastFloorRear))
                                {
                                    newConsole.UnitHallFixtureLocations[floor].Rear.NotAvailable = true;
                                }
                                if (newConsole.ConsoleId == 1 && (newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(firstFloorFront) ||
                                           !newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(lastFloorFront)))
                                {
                                    newConsole.UnitHallFixtureLocations[floor].Front.NotAvailable = true;
                                }
                                if (newConsole.ConsoleId == 1 && (newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(firstFloorRear) ||
                                    !newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(lastFloorRear)))
                                {
                                    newConsole.UnitHallFixtureLocations[floor].Rear.NotAvailable = true;
                                }
                            }
                        }
                        unitHallFixtureConsoles.Add(newConsole);
                        if (newConsole.UnitHallFixtureType.Equals(Constant.HALLLANTERNCAMELCASE) || newConsole.UnitHallFixtureType.Equals(Constant.HALLPICAMELCASE) || newConsole.UnitHallFixtureType.Equals(Constant.COMBOHALLLANTERNPICAMELCASE))
                        {
                            SetIncompatiblePropertyForMutuallyExclusiveConsoles(newConsole, unitHallFixtureConsoles);
                        }
                        unitHallFixtureConsoles = _configure.SetCacheUnitHallFixtureConsoles(unitHallFixtureConsoles, SessionId, setId);
                    }
                    else
                    {

                        newConsole = (from consoles in unitHallFixtureConsoles
                                      where consoles.ConsoleId == consoleId && consoles.UnitHallFixtureType == fixtureType
                                      select consoles).ToList()[0];
                        var newentranceLocations = newConsole.UnitHallFixtureLocations.Distinct().ToList();
                        var lastFloor = newConsole.UnitHallFixtureLocations.Count;


                        foreach (var varLocation in newentranceLocations)
                        {
                            varLocation.Front.InCompatible = false;
                            varLocation.Rear.InCompatible = false;
                            bool has = lstSelectedFrontFloorNumber.Any(floor => floor == varLocation.FloorNumber);
                            if (has)
                            {
                                varLocation.Front.InCompatible = true;
                            }
                            has = lstSelectedRearFloorNumber.Any(floor => floor == varLocation.FloorNumber);
                            if (has)
                            {
                                varLocation.Rear.InCompatible = true;
                            }
                        }
                        if (newConsole.UnitHallFixtureType.Equals(Constant.HALLLANTERNCAMELCASE) || newConsole.UnitHallFixtureType.Equals(Constant.HALLPICAMELCASE) || newConsole.UnitHallFixtureType.Equals(Constant.COMBOHALLLANTERNPICAMELCASE))
                        {
                            SetIncompatiblePropertyForMutuallyExclusiveConsoles(newConsole, unitHallFixtureConsoles);
                        }
                        if (Utility.CheckEquals(newConsole.UnitHallFixtureType, Constant.HOISTWAYACCESS))
                        {
                            var frontFloors = (from floors in newConsole.UnitHallFixtureLocations
                                               where !floors.Front.NotAvailable.Equals(true)
                                               select floors.FloorNumber).ToList();
                            var firstFloorFront = frontFloors.Any() ? frontFloors.Min() : -1;
                            var lastFloorFront = frontFloors.Any() ? frontFloors.Max() : -1;
                            var rearFloors = (from floors in newConsole.UnitHallFixtureLocations
                                              where !floors.Rear.NotAvailable.Equals(true)
                                              select floors.FloorNumber).ToList();
                            var firstFloorRear = rearFloors.Any() ? rearFloors.Min() : -1;
                            var lastFloorRear = rearFloors.Any() ? rearFloors.Max() : -1;
                            for (int floor = 0; floor < (newConsole.UnitHallFixtureLocations.Count); floor++)
                            {
                                if (!newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(firstFloorFront) && !newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(lastFloorFront))
                                {
                                    newConsole.UnitHallFixtureLocations[floor].Front.NotAvailable = true;
                                }
                                if (!newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(firstFloorRear) && !newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(lastFloorRear))
                                {
                                    newConsole.UnitHallFixtureLocations[floor].Rear.NotAvailable = true;
                                }
                                if (newConsole.ConsoleId == 1 && (newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(firstFloorFront) ||
                                    newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(lastFloorFront)) &&
                                    (newConsole.UnitHallFixtureLocations[floor].Front.Value.Equals(true)))
                                {
                                    newConsole.UnitHallFixtureLocations[floor].Front.NotAvailable = true;
                                }
                                if (newConsole.ConsoleId == 1 && (newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(firstFloorRear) ||
                                    newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(lastFloorRear)) &&
                                    (newConsole.UnitHallFixtureLocations[floor].Rear.Value.Equals(true)))
                                {
                                    newConsole.UnitHallFixtureLocations[floor].Rear.NotAvailable = true;
                                }
                            }

                        }


                        if (isReset)
                        {
                            newConsole.VariableAssignments = new List<ConfigVariable>();
                            var result = _unitConfigurationdl.ResetUnitHallFixtureConsole(setId, consoleId, fixtureSelected, userName);
                            result[0].VariableAssignments = new List<ConfigVariable>();
                            var lastFloorOfResult = result[0].UnitHallFixtureLocations.Count;
                            if (Utility.CheckEquals(result[0].UnitHallFixtureType, Constant.HOISTWAYACCESS) && result[0].IsController.Equals(true))
                            {
                                result[0].UnitHallFixtureLocations[0].Front.InCompatible = true;
                                result[0].UnitHallFixtureLocations[0].Rear.InCompatible = true;
                                result[0].UnitHallFixtureLocations[lastFloorOfResult - 1].Front.InCompatible = true;
                                result[0].UnitHallFixtureLocations[lastFloorOfResult - 1].Rear.InCompatible = true;

                            }
                            List<UnitHallFixtures> newConsoles = new List<UnitHallFixtures>();
                            UnitHallFixtures newConsoleData = new UnitHallFixtures();
                            if (result.Count() > 0)
                            {
                                foreach (var console in unitHallFixtureConsoles)
                                {
                                    if (console.ConsoleId != consoleId || console.UnitHallFixtureType != fixtureType)
                                    {
                                        newConsoles.Add(console);
                                    }
                                }
                                newConsoles.Add(result[0]);
                            }

                            unitHallFixtureConsoles = _configure.SetCacheUnitHallFixtureConsoles(newConsoles, SessionId, setId);

                            var lstSelectedFrontFloorNumber1 = (from varConsole in unitHallFixtureConsoles
                                                                where varConsole.ConsoleId != consoleId && varConsole.UnitHallFixtureType.ToUpper().Equals(fixtureType.ToUpper())

                                                                from locations in varConsole.UnitHallFixtureLocations
                                                                where locations.Front.Value.Equals(true)
                                                                select locations.FloorNumber).ToList();
                            var lstSelectedRearFloorNumber1 = (from varUnitHallFixtureConsole in unitHallFixtureConsoles
                                                               where varUnitHallFixtureConsole.ConsoleId != consoleId &&
                                                                varUnitHallFixtureConsole.UnitHallFixtureType.ToUpper().Equals(fixtureType.ToUpper())
                                                               from EntranceLocations in varUnitHallFixtureConsole.UnitHallFixtureLocations
                                                               where EntranceLocations.Rear.Value.Equals(true)
                                                               select EntranceLocations.FloorNumber).ToList();

                            List<EntranceLocations> newentranceLocation1 = new List<EntranceLocations>();
                            foreach (var consoleLocation in unitHallFixtureConsoles)
                            {
                                if (consoleLocation.UnitHallFixtureType == fixtureType && consoleLocation.ConsoleId == consoleId)
                                {
                                    newentranceLocation1 = consoleLocation.UnitHallFixtureLocations;
                                }
                            }

                            foreach (var varLocation in newentranceLocation1)
                            {

                                if (lstSelectedFrontFloorNumber1.Count > 0)
                                {
                                    if (lstSelectedFrontFloorNumber1.Contains(varLocation.FloorNumber))
                                    {
                                        varLocation.Front.InCompatible = true;
                                    }

                                }
                                if (lstSelectedRearFloorNumber1.Count > 0)
                                {
                                    if (lstSelectedRearFloorNumber1.Contains(varLocation.FloorNumber))
                                    {
                                        varLocation.Rear.InCompatible = true;
                                    }
                                }

                            }
                            newConsole.UnitHallFixtureLocations = newentranceLocation1;

                        }

                    }
                }

            }
            else
            {
                newConsole = unitHallFixtureConsoles.Where(x => x.ConsoleId.Equals(consoleId) && x.UnitHallFixtureType.Equals(fixtureSelected)).ToList()[0];
                newConsole.ConsoleId = Convert.ToInt32(objEntranceConfiguration.ConsoleId);
                newConsole.ConsoleName = objEntranceConfiguration.ConsoleName;
                newConsole.VariableAssignments = new List<ConfigVariable>();
                newConsole.UnitHallFixtureType = objEntranceConfiguration.FixtureType;
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


                foreach (var entranceLocation in newConsole.UnitHallFixtureLocations)
                {
                    foreach (var entrancedata in objEntranceConfiguration.FixtureLocations)
                    {
                        if (entranceLocation.FloorNumber == entrancedata.FloorNumber)
                        {
                            entranceLocation.Front.Value = entrancedata.Front;
                            entranceLocation.Rear.Value = entrancedata.Rear;
                            if (lstSelectedFrontFloorNumber.Count > 0)
                            {
                                if (lstSelectedFrontFloorNumber.Contains(entranceLocation.FloorNumber))
                                {
                                    entranceLocation.Front.InCompatible = true;
                                }
                            }
                            if (lstSelectedRearFloorNumber.Count > 0)
                            {
                                if (lstSelectedRearFloorNumber.Contains(entranceLocation.FloorNumber))
                                {
                                    entranceLocation.Rear.InCompatible = true;
                                }
                            }
                        }
                    }
                }
                if (newConsole.UnitHallFixtureType.Equals(Constant.HALLLANTERNCAMELCASE) || newConsole.UnitHallFixtureType.Equals(Constant.HALLPICAMELCASE) || newConsole.UnitHallFixtureType.Equals(Constant.COMBOHALLLANTERNPICAMELCASE))
                {
                    SetIncompatiblePropertyForMutuallyExclusiveConsoles(newConsole, unitHallFixtureConsoles);
                }
                var lastFloor = newConsole.UnitHallFixtureLocations.Count;
                if (Utility.CheckEquals(newConsole.UnitHallFixtureType, Constant.HOISTWAYACCESS))
                {
                    var frontFloors = (from floors in newConsole.UnitHallFixtureLocations
                                       where floors.Front.Value.Equals(true)
                                       select floors.FloorNumber).ToList();
                    var firstFloorFront = frontFloors.Any() ? frontFloors.Min() : -1;
                    var lastFloorFront = frontFloors.Any() ? frontFloors.Max() : -1;
                    var rearFloors = (from floors in newConsole.UnitHallFixtureLocations
                                      where floors.Rear.Value.Equals(true)
                                      select floors.FloorNumber).ToList();
                    var firstFloorRear = rearFloors.Any() ? rearFloors.Min() : -1;
                    var lastFloorRear = rearFloors.Any() ? rearFloors.Max() : -1;
                    for (int floor = 0; floor < (newConsole.UnitHallFixtureLocations.Count); floor++)
                    {
                        if (!newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(firstFloorFront) && !newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(lastFloorFront))
                        {
                            newConsole.UnitHallFixtureLocations[floor].Front.NotAvailable = true;
                        }
                        if (!newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(firstFloorRear) && !newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(lastFloorRear))
                        {
                            newConsole.UnitHallFixtureLocations[floor].Rear.NotAvailable = true;
                        }
                        if (newConsole.ConsoleId == 1 && (newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(firstFloorFront) ||
                            newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(lastFloorFront)))
                        {
                            newConsole.UnitHallFixtureLocations[floor].Front.NotAvailable = true;
                        }
                        if (newConsole.ConsoleId == 1 && (newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(firstFloorRear) ||
                            newConsole.UnitHallFixtureLocations[floor].FloorNumber.Equals(lastFloorRear)))
                        {
                            newConsole.UnitHallFixtureLocations[floor].Rear.NotAvailable = true;
                        }
                    }

                }
            }



            var res = await _configure.UnitHallFixtureConsoleConfigureBl(newConsole, SessionId, fixtureType, setId, isSave).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = res
            };


        }

        /// <summary>
        /// DeleteEntranceConsole
        /// </summary>
        /// <param Name="consoleId"></param>
        /// <param Name="setId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> DeleteEntranceConsole(int consoleId, int setId, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            if (setId == 0 || consoleId == 0)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Description = Constant.SOMETHINGWENTWRONGMSG
                });
            }
            var userid = _configure.GetUserId(sessionId);

            /**Basic Log**/
            var entranceConsoles = _configure.SetCacheEntranceConsoles(null, sessionId, setId);
            var entranceConsole = new EntranceConfigurations();
            if (entranceConsoles != null)
            {
                var consoles = (from varconsole in entranceConsoles
                                where varconsole.EntranceConsoleId.Equals(consoleId)
                                select varconsole).ToList();
                if (consoles.Count > 0)
                {
                    entranceConsole = consoles[0];
                }
            }

            var historyTable = new List<LogHistoryTable>();
            var frontRearAssignment = entranceConsole.FixtureLocations.Count > 0 ? Utility.GetLandingOpeningAssignmentSelected(entranceConsole.FixtureLocations) : string.Empty;
            if (entranceConsole.VariableAssignments.Count > 0)
            {
                var enrichedData = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.EVOLUTION200)));
                var enrichedDataVariables = enrichedData[Constant.VARIABLES];
                entranceConsole.VariableAssignments = entranceConsole.VariableAssignments.Where(x => !x.VariableId.Equals(string.Empty)).ToList();
                foreach (var assignment in entranceConsole.VariableAssignments)
                {
                    var needVariables = Utility.GetTokens(assignment.VariableId, enrichedDataVariables, false);
                    var currentPropertyCollection = needVariables.Select(x => (JProperty)x).Where(x => x.Name == Constant.PROPERTIES).Select(x => x.Value).FirstOrDefault();
                    var displayName = currentPropertyCollection.Children().Where(x => x.Children<JProperty>().Any(y => y.Name == Constant.IDPARAM && y.Value.ToString() == Constant.DISPLAYNAME)).SelectMany(x => x.Children<JProperty>().Select(y => y.Name == Constant.VALUE ? y.Value.ToString() : "")).FirstOrDefault(x => !string.IsNullOrEmpty(x));
                    var history = new LogHistoryTable()
                    {
                        VariableId = Constant.ENTRANCE_CONFIGURATION + " - " + displayName + "(" + frontRearAssignment + ")",
                        UpdatedValue = string.Empty,
                        PreviuosValue = assignment.Value != null ? assignment.Value.ToString() : string.Empty

                    };
                    if (!history.UpdatedValue.Equals(history.PreviuosValue))
                    {
                        historyTable.Add(history);
                    }

                }
            }
            /**Basic Log**/


            var result = _unitConfigurationdl.DeleteEntranceConsole(consoleId, setId, historyTable, userid);
            var response = JArray.FromObject(result);
            var objresult = result.Count > 0 ? result[0].result : 0;
            if (objresult == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.NOTFOUND,
                    Description = Constant.DELETEENTRANCECONSOLEERRORMSG,
                    Message = result[0].message
                });

            }


        }

        /// <summary>
        /// Method to delete Unit hall fiture consoles
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="consoleId"></param>
        /// <param Name="fixtureType"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> DeleteUnitHallFixtureConsole(int setId, int consoleId, string fixtureType, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            if (setId == 0 || consoleId == 0 || fixtureType.Equals(string.Empty))
            {

                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Description = Constant.DELETEUNITHALLFIXTURECONSOLEERRORMSG
                });

            }

            /**Basic Log**/
            var entranceConsoles = _configure.SetCacheUnitHallFixtureConsoles(null, sessionId, setId);
            var unithallfixtureconsole = new UnitHallFixtures();
            if (entranceConsoles != null)
            {
                var consoles = (from varconsole in entranceConsoles
                                where varconsole.ConsoleId.Equals(consoleId) && Utility.CheckEquals(varconsole.UnitHallFixtureType, fixtureType)
                                select varconsole).ToList();
                if (consoles.Count > 0)
                {
                    unithallfixtureconsole = consoles[0];
                }
            }
            var product = _configure.SetCacheProductType(null, sessionId, setId);
            var historyTable = new List<LogHistoryTable>();
            var frontRearAssignment = unithallfixtureconsole.UnitHallFixtureLocations.Count > 0 ? Utility.GetLandingOpeningAssignmentSelected(unithallfixtureconsole.UnitHallFixtureLocations) : string.Empty;
            if (unithallfixtureconsole.VariableAssignments.Count > 0)
            {
                var enrichedData = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.EVOLUTION200)));
                if (product != null && Utility.CheckEquals(product[0].Value.ToString(), Constants.EVO_100))
                {
                    enrichedData = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constants.EVOLUTION100)));
                }
                if (product != null && Utility.CheckEquals(product[0].Value.ToString(), Constants.ENDURA_100))
                {
                    enrichedData = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constants.END100)));
                }
                var enrichedDataVariables = enrichedData[Constant.VARIABLES];
                unithallfixtureconsole.VariableAssignments = unithallfixtureconsole.VariableAssignments.Where(x => !x.VariableId.Equals(string.Empty)).ToList();
                foreach (var assignment in unithallfixtureconsole.VariableAssignments)
                {
                    var needVariables = Utility.GetTokens(assignment.VariableId, enrichedDataVariables, false);
                    var currentPropertyCollection = needVariables.Select(x => (JProperty)x).Where(x => x.Name == Constant.PROPERTIES).Select(x => x.Value).FirstOrDefault();
                    var displayName = currentPropertyCollection?.Children().Where(x => x.Children<JProperty>().Any(y => y.Name == Constant.IDPARAM && y.Value.ToString() == Constant.DISPLAYNAME)).SelectMany(x => x.Children<JProperty>().Select(y => y.Name == Constant.VALUE ? y.Value.ToString() : "")).FirstOrDefault(x => !string.IsNullOrEmpty(x));
                    var history = new LogHistoryTable()
                    {
                        VariableId = unithallfixtureconsole.ConsoleName + " - " + displayName + "(" + frontRearAssignment + ")",
                        UpdatedValue = string.Empty,
                        PreviuosValue = assignment.Value != null ? assignment.Value.ToString() : string.Empty

                    };
                    if (!history.PreviuosValue.Equals(history.UpdatedValue))
                    {
                        historyTable.Add(history);
                    }

                }
            }

            /**Basic Log**/

            var userId = _configure.GetUserId(sessionId);
            var result = _unitConfigurationdl.DeleteUnitHallFixtureConsole(setId, consoleId, fixtureType, historyTable, userId);
            var response = JArray.FromObject(result);
            var objresult = result.Count > 0 ? result[0].result : 0;
            if (objresult == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.NOTFOUND,
                    Description = Constant.NOTFOUNDERRORMSG
                });
            }

        }

        /// <summary>
        /// method to save car call out key openings
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="carcallCutoutData"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveCarCallCutoutKeyswitchOpenings(int setId, CarcallCutoutData carcallCutoutData, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            var userId = _configure.GetUserId(sessionId);
            /**Basic Log**/
            var logHistory = new List<ConsoleHistory>();
            var EntranceAssignment = _unitConfigurationdl.GetCarCallCutoutOpenings(setId);
            List<EntranceLocations> previousCarcutout = new List<EntranceLocations>();
            if (EntranceAssignment != null)
            {
                previousCarcutout = EntranceAssignment.FixtureAssignments;
            }

            foreach (var location in carcallCutoutData.EntranceLocations)
            {
                var front = previousCarcutout.Where(x => x.FloorNumber.Equals(location.FloorNumber) && !(location.Front.Equals(x.Front.Value))).ToList();
                if (front.Count > 0)
                {
                    var consolehistory = new ConsoleHistory()
                    {
                        Console = Constant.CARCALLCUTOUTKEYSWITCH,
                        Parameter = string.Empty,
                        FloorNumber = front[0].FloorNumber,
                        Opening = Constant.F,
                        PreviousValue = !EntranceAssignment.IsSaved ? string.Empty : front[0].Front.Value.ToString(),
                        PresentValue = location.Front.ToString()

                    };
                    logHistory.Add(consolehistory);
                }
                var rear = previousCarcutout.Where(x => x.FloorNumber.Equals(location.FloorNumber) && !(location.Rear.Equals(x.Rear.Value))).ToList();
                if (rear.Count > 0)
                {
                    var consolehistory = new ConsoleHistory()
                    {
                        Console = Constant.CARCALLCUTOUTKEYSWITCH,
                        Parameter = string.Empty,
                        FloorNumber = rear[0].FloorNumber,
                        Opening = Constant.R,
                        PreviousValue = !EntranceAssignment.IsSaved ? string.Empty : rear[0].Front.Value.ToString(),
                        PresentValue = location.Rear.ToString()

                    };
                    logHistory.Add(consolehistory);
                }
            }
            var logHistoryTable = GetLogHistoryTableForConsole(logHistory);
            /**Basic Log**/
            var result = _unitConfigurationdl.SaveCarCallCutoutKeyswitchOpenings(setId, carcallCutoutData, userId, logHistoryTable);
            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.UNITCONFIGURATIONSAVEERRORMESSAGE,
                    Description = result[0].message
                });
            }

        }

        /// <summary>
        /// method to assgin openings in carcallcutout
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> StartCarCallCutoutAssignOpenings(int setId)
        {
            var methodBegin = Utility.LogBegin();
            var openingAssignments = _unitConfigurationdl.GetCarCallCutoutOpenings(setId);
            var newConsole = new Sections()
            {
                Id = "1",
                Name = Constant.CARCALLCUTOUTKEYSWITCHESCONSOLE,
                FixtureLocations = openingAssignments,
            };

            var newConsoleFiltered = Utility.FilterNullValues(newConsole);
            var result = JObject.FromObject(newConsoleFiltered);
            Utility.LogEnd(methodBegin);
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = result
            };

        }

        /// <summary>
        /// GetConsoleHistoriesUnitHallFixture
        /// </summary>
        /// <param Name="cachedConsoleData"></param>
        /// <param Name="unitHallFixtureData"></param>
        /// <returns></returns>
        private List<ConsoleHistory> GetConsoleHistoriesUnitHallFixture(List<UnitHallFixtures> cachedConsoleData, UnitHallFixtureData unitHallFixtureData)
        {
            var methodBegin = Utility.LogBegin();
            var lstConsoleHistory = new List<ConsoleHistory>();
            var cachedConsoles = (from console in cachedConsoleData
                                  where console.UnitHallFixtureType != null && console.UnitHallFixtureType.Equals(unitHallFixtureData.FixtureType)
                                  select console).ToList();
            foreach (var entrancelocation in unitHallFixtureData.FixtureLocations)
            {
                var FrontConsole = (from console in cachedConsoles
                                    from location in console.UnitHallFixtureLocations
                                    where location.FloorNumber.Equals(entrancelocation.FloorNumber) && location.Front.Value.Equals(true)
                                    select console).ToList();
                if (FrontConsole.Count > 0)
                {
                    FrontConsole[0].VariableAssignments = FrontConsole[0].VariableAssignments.Where(x => !x.VariableId.Equals("")).ToList();
                    if (FrontConsole[0].VariableAssignments.Count > 0)
                    {
                        foreach (var variable1 in FrontConsole[0].VariableAssignments)
                        {
                            foreach (var variable2 in unitHallFixtureData.VariableAssignments)
                            {
                                if (Utility.CheckEquals(variable1.VariableId, variable2.VariableId))
                                {
                                    if (entrancelocation.Front.Equals(true))
                                    {
                                        var consolehistory = new ConsoleHistory()
                                        {
                                            Console = unitHallFixtureData.FixtureType.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE),
                                            Parameter = variable2.VariableId,
                                            FloorNumber = entrancelocation.FloorNumber,
                                            Opening = Constant.F,
                                            PresentValue = variable2.Value != null ? variable2.Value.ToString() : string.Empty,
                                            PreviousValue = variable1.Value != null ? variable1.Value.ToString() : String.Empty
                                        };
                                        lstConsoleHistory.Add(consolehistory);
                                    }
                                    else if (unitHallFixtureData.ConsoleId.Equals(Convert.ToString(FrontConsole[0].ConsoleId)))
                                    {
                                        var consolehistory = new ConsoleHistory()
                                        {
                                            Console = unitHallFixtureData.FixtureType.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE),
                                            Parameter = variable2.VariableId,
                                            FloorNumber = entrancelocation.FloorNumber,
                                            Opening = Constant.F,
                                            PresentValue = string.Empty,
                                            PreviousValue = variable1.Value != null ? variable1.Value.ToString() : String.Empty
                                        };
                                        lstConsoleHistory.Add(consolehistory);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var variable2 in unitHallFixtureData.VariableAssignments)
                        {
                            if (entrancelocation.Front.Equals(true))
                            {
                                var consolehistory = new ConsoleHistory()
                                {
                                    Console = unitHallFixtureData.FixtureType.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE),
                                    Parameter = variable2.VariableId,
                                    FloorNumber = entrancelocation.FloorNumber,
                                    Opening = Constant.F,
                                    PresentValue = variable2.Value != null ? variable2.Value.ToString() : string.Empty,
                                    PreviousValue = string.Empty
                                };
                                lstConsoleHistory.Add(consolehistory);
                            }
                        }
                    }

                }
                else
                {
                    if (entrancelocation.Front)
                    {
                        foreach (var variable1 in unitHallFixtureData.VariableAssignments)
                        {
                            var consolehistory = new ConsoleHistory()
                            {
                                Console = unitHallFixtureData.FixtureType.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE),
                                Parameter = variable1.VariableId,
                                FloorNumber = entrancelocation.FloorNumber,
                                Opening = Constant.F,
                                PresentValue = variable1.Value != null ? variable1.Value.ToString() : String.Empty,
                                PreviousValue = String.Empty
                            };
                            lstConsoleHistory.Add(consolehistory);
                        }
                    }
                }
                var RearConsole = (from console in cachedConsoles
                                   from location in console.UnitHallFixtureLocations
                                   where location.FloorNumber.Equals(entrancelocation.FloorNumber) && location.Rear.Value.Equals(true)
                                   select console).ToList();
                if (RearConsole.Count > 0)
                {
                    RearConsole[0].VariableAssignments = RearConsole[0].VariableAssignments.Where(x => !x.VariableId.Equals("")).ToList();
                    if (RearConsole[0].VariableAssignments.Count > 0)
                    {
                        foreach (var variable1 in RearConsole[0].VariableAssignments)
                        {
                            foreach (var variable2 in unitHallFixtureData.VariableAssignments)
                            {
                                if (Utility.CheckEquals(variable1.VariableId, variable2.VariableId))
                                {
                                    if (entrancelocation.Rear)
                                    {
                                        var consolehistory = new ConsoleHistory()
                                        {
                                            Console = unitHallFixtureData.FixtureType.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE),
                                            Parameter = variable2.VariableId,
                                            FloorNumber = entrancelocation.FloorNumber,
                                            Opening = Constant.R,
                                            PresentValue = variable2.Value != null ? variable2.Value.ToString() : string.Empty,
                                            PreviousValue = variable1.Value != null ? variable1.Value.ToString() : String.Empty
                                        };
                                        lstConsoleHistory.Add(consolehistory);
                                    }
                                    else if (unitHallFixtureData.ConsoleId.Equals(Convert.ToString(RearConsole[0].ConsoleId)))
                                    {
                                        var consolehistory = new ConsoleHistory()
                                        {
                                            Console = unitHallFixtureData.FixtureType.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE),
                                            Parameter = variable2.VariableId,
                                            FloorNumber = entrancelocation.FloorNumber,
                                            Opening = Constant.R,
                                            PresentValue = string.Empty,
                                            PreviousValue = variable1.Value != null ? variable1.Value.ToString() : String.Empty
                                        };
                                        lstConsoleHistory.Add(consolehistory);
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var variable2 in unitHallFixtureData.VariableAssignments)
                        {
                            if (entrancelocation.Rear)
                            {
                                var consolehistory = new ConsoleHistory()
                                {
                                    Console = unitHallFixtureData.FixtureType.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE),
                                    Parameter = variable2.VariableId,
                                    FloorNumber = entrancelocation.FloorNumber,
                                    Opening = Constant.R,
                                    PresentValue = variable2.Value != null ? variable2.Value.ToString() : string.Empty,
                                    PreviousValue = string.Empty
                                };
                                lstConsoleHistory.Add(consolehistory);
                            }
                        }
                    }

                }
                else
                {
                    if (entrancelocation.Rear)
                    {
                        foreach (var variable1 in unitHallFixtureData.VariableAssignments)
                        {
                            var consolehistory = new ConsoleHistory()
                            {
                                Console = unitHallFixtureData.FixtureType.Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE),
                                Parameter = variable1.VariableId,
                                FloorNumber = entrancelocation.FloorNumber,
                                Opening = Constant.R,
                                PresentValue = variable1.Value != null ? variable1.Value.ToString() : String.Empty,
                                PreviousValue = String.Empty
                            };
                            lstConsoleHistory.Add(consolehistory);
                        }
                    }

                }
            }
            Utility.LogEnd(methodBegin);
            return lstConsoleHistory;
        }

        /// <summary>
        /// GetConsoleHistoriesEntrance
        /// </summary>
        /// <param Name="entranceConsoles"></param>
        /// <param Name="entranceConfigurationData"></param>
        /// <returns></returns>
        public List<ConsoleHistory> GetConsoleHistoriesEntrance(List<EntranceConfigurations> entranceConsoles, EntranceConfigurationData entranceConfigurationData)
        {
            var methodBegin = Utility.LogBegin();
            var lstConsoleHistory = new List<ConsoleHistory>();
            foreach (var entrancelocation in entranceConfigurationData.EntranceLocations)
            {
                var FrontConsole = (from console in entranceConsoles
                                    from location in console.FixtureLocations
                                    where location.FloorNumber == entrancelocation.FloorNumber && location.Front.Value.Equals(true)
                                    select console).ToList();
                if (FrontConsole.Count > 0)
                {
                    FrontConsole[0].VariableAssignments = FrontConsole[0].VariableAssignments.Where(x => !x.VariableId.Equals("")).ToList();
                    if (FrontConsole[0].VariableAssignments.Count > 0)
                    {
                        foreach (var variable1 in FrontConsole[0].VariableAssignments)
                        {
                            foreach (var variable2 in entranceConfigurationData.VariableAssignments)
                            {
                                if (Utility.CheckEquals(variable1.VariableId, variable2.VariableId))
                                {
                                    if (entrancelocation.Front)
                                    {
                                        var consolehistory = new ConsoleHistory()
                                        {
                                            Console = Constant.ENTRANCE_CONFIGURATION,
                                            Parameter = variable2.VariableId,
                                            FloorNumber = entrancelocation.FloorNumber,
                                            Opening = Constant.F,
                                            PresentValue = variable2.Value != null ? variable2.Value.ToString() : string.Empty,
                                            PreviousValue = variable1.Value != null ? variable1.Value.ToString() : String.Empty
                                        };
                                        lstConsoleHistory.Add(consolehistory);
                                    }
                                    else if (entranceConfigurationData.EntranceConsoleId.Equals(Convert.ToString(FrontConsole[0].EntranceConsoleId)))
                                    {
                                        var consolehistory = new ConsoleHistory()
                                        {
                                            Console = Constant.ENTRANCE_CONFIGURATION,
                                            Parameter = variable2.VariableId,
                                            FloorNumber = entrancelocation.FloorNumber,
                                            Opening = Constant.F,
                                            PresentValue = string.Empty,
                                            PreviousValue = variable1.Value != null ? variable1.Value.ToString() : String.Empty
                                        };
                                        lstConsoleHistory.Add(consolehistory);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (entrancelocation.Front)
                        {
                            foreach (var variable2 in entranceConfigurationData.VariableAssignments)
                            {
                                var consolehistory = new ConsoleHistory()
                                {
                                    Console = Constant.ENTRANCE_CONFIGURATION,
                                    Parameter = variable2.VariableId,
                                    FloorNumber = entrancelocation.FloorNumber,
                                    Opening = Constant.F,
                                    PresentValue = variable2.Value != null ? variable2.Value.ToString() : string.Empty,
                                    PreviousValue = string.Empty
                                };
                                lstConsoleHistory.Add(consolehistory);
                            }
                        }
                    }
                }
                else
                {
                    if (entrancelocation.Front)
                    {
                        foreach (var variable1 in entranceConfigurationData.VariableAssignments)
                        {
                            var consolehistory = new ConsoleHistory()
                            {
                                Console = Constant.ENTRANCE_CONFIGURATION,
                                Parameter = variable1.VariableId,
                                FloorNumber = entrancelocation.FloorNumber,
                                Opening = Constant.F,
                                PresentValue = variable1.Value != null ? variable1.Value.ToString() : String.Empty,
                                PreviousValue = String.Empty
                            };
                            lstConsoleHistory.Add(consolehistory);
                        }
                    }
                }
                var RearConsole = (from console in entranceConsoles
                                   from location in console.FixtureLocations
                                   where location.FloorNumber == entrancelocation.FloorNumber && location.Rear.Value.Equals(true)
                                   select console).ToList();
                if (RearConsole.Count > 0)
                {
                    RearConsole[0].VariableAssignments = RearConsole[0].VariableAssignments.Where(x => !x.VariableId.Equals("")).ToList();
                    if (RearConsole[0].VariableAssignments.Count > 0)
                    {
                        foreach (var variable1 in RearConsole[0].VariableAssignments)
                        {
                            foreach (var variable2 in entranceConfigurationData.VariableAssignments)
                            {
                                if (Utility.CheckEquals(variable1.VariableId, variable2.VariableId))
                                {
                                    if (entrancelocation.Rear)
                                    {
                                        var consolehistory = new ConsoleHistory()
                                        {
                                            Console = Constant.ENTRANCE_CONFIGURATION,
                                            Parameter = variable2.VariableId,
                                            FloorNumber = entrancelocation.FloorNumber,
                                            Opening = Constant.R,
                                            PresentValue = variable2.Value != null ? variable2.Value.ToString() : string.Empty,
                                            PreviousValue = variable1.Value != null ? variable1.Value.ToString() : String.Empty
                                        };
                                        lstConsoleHistory.Add(consolehistory);
                                    }
                                    else if (entranceConfigurationData.EntranceConsoleId.Equals(Convert.ToString(RearConsole[0].EntranceConsoleId)))
                                    {
                                        var consolehistory = new ConsoleHistory()
                                        {
                                            Console = Constant.ENTRANCE_CONFIGURATION,
                                            Parameter = variable2.VariableId,
                                            FloorNumber = entrancelocation.FloorNumber,
                                            Opening = Constant.R,
                                            PresentValue = string.Empty,
                                            PreviousValue = variable1.Value != null ? variable1.Value.ToString() : String.Empty
                                        };
                                        lstConsoleHistory.Add(consolehistory);
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var variable2 in entranceConfigurationData.VariableAssignments)
                        {
                            if (entrancelocation.Rear)
                            {
                                var consolehistory = new ConsoleHistory()
                                {
                                    Console = Constant.ENTRANCE_CONFIGURATION,
                                    Parameter = variable2.VariableId,
                                    FloorNumber = entrancelocation.FloorNumber,
                                    Opening = Constant.R,
                                    PresentValue = variable2.Value != null ? variable2.Value.ToString() : string.Empty,
                                    PreviousValue = string.Empty
                                };
                                lstConsoleHistory.Add(consolehistory);
                            }
                        }
                    }

                }
                else
                {
                    if (entrancelocation.Rear)
                    {
                        foreach (var variable1 in entranceConfigurationData.VariableAssignments)
                        {
                            var consolehistory = new ConsoleHistory()
                            {
                                Console = Constant.ENTRANCE_CONFIGURATION,
                                Parameter = variable1.VariableId,
                                FloorNumber = entrancelocation.FloorNumber,
                                Opening = Constant.R,
                                PresentValue = variable1.Value != null ? variable1.Value.ToString() : String.Empty,
                                PreviousValue = string.Empty
                            };
                            lstConsoleHistory.Add(consolehistory);
                        }
                    }

                }
            }
            Utility.LogEnd(methodBegin);
            return lstConsoleHistory;
        }

        /// <summary>
        /// GetLogHistoryTableForConsole
        /// </summary>
        /// <param Name="lstConsoleHistory"></param>
        /// <returns></returns>
        public List<LogHistoryTable> GetLogHistoryTableForConsole(List<ConsoleHistory> lstConsoleHistory, string section = "")
        {
            var methodBegin = Utility.LogBegin();
            List<LogHistoryTable> logHistoryTable = new List<LogHistoryTable>();
            var enrichedData = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.EVOLUTION200)));
            var enrichedDataVariables = enrichedData[Constant.VARIABLES];
            foreach (var consolehistory in lstConsoleHistory)
            {
                var needVariables = Utility.GetTokens(consolehistory.Parameter, enrichedDataVariables, false);
                var displayName = string.Empty;
                if (needVariables != null && needVariables.Any())
                {
                    var currentPropertyCollection = needVariables.Select(x => (JProperty)x).Where(x => x.Name == Constant.PROPERTIES).Select(x => x.Value).FirstOrDefault();
                    displayName = currentPropertyCollection.Children().Where(x => x.Children<JProperty>().Any(y => y.Name == Constant.IDPARAM && y.Value.ToString() == Constant.DISPLAYNAME)).SelectMany(x => x.Children<JProperty>().Select(y => y.Name == Constant.VALUE ? y.Value.ToString() : "")).FirstOrDefault(x => !string.IsNullOrEmpty(x));

                }
                var lstSameConsoleHistory = (from x in lstConsoleHistory
                                             where (Utility.CheckEquals(consolehistory.Parameter, x.Parameter) || x.Parameter.Equals(consolehistory.Parameter)) && (consolehistory.PresentValue.Equals(x.PresentValue) || Utility.CheckEquals(consolehistory.PresentValue, x.PresentValue)) && (consolehistory.PreviousValue.Equals(x.PreviousValue) || Utility.CheckEquals(consolehistory.PreviousValue, x.PreviousValue))
                                             select x).ToList();
                if (lstSameConsoleHistory.Count > 0)
                {
                    List<EntranceLocations> lstEntranceLocation = new List<EntranceLocations>();
                    foreach (var sameConsoleHistory in lstSameConsoleHistory)
                    {
                        var entranceLocation = new EntranceLocations()
                        {
                            FloorNumber = sameConsoleHistory.FloorNumber
                        };
                        var lstsameFloor = lstSameConsoleHistory.Where(x => x.FloorNumber.Equals(sameConsoleHistory.FloorNumber)).ToList();
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
                        lstSameConsoleHistory = lstSameConsoleHistory.Where(x => !x.FloorNumber.Equals(sameConsoleHistory.FloorNumber)).ToList();
                        lstEntranceLocation.Add(entranceLocation);
                    }
                    lstConsoleHistory = (from x in lstConsoleHistory
                                         where !((Utility.CheckEquals(consolehistory.Parameter, x.Parameter) || x.Parameter.Equals(consolehistory.Parameter)) && (consolehistory.PresentValue.Equals(x.PresentValue) || Utility.CheckEquals(consolehistory.PresentValue, x.PresentValue)) && (consolehistory.PreviousValue.Equals(x.PreviousValue) || Utility.CheckEquals(consolehistory.PreviousValue, x.PreviousValue)))
                                         select x).ToList();
                    var frontRearAssignment = Utility.GetLandingOpeningAssignmentSelected(lstEntranceLocation);
                    var loghistory = new LogHistoryTable()
                    {
                        VariableId = consolehistory.Console + " - " + displayName + "(" + frontRearAssignment + ")",
                        PreviuosValue = consolehistory.PreviousValue,
                        UpdatedValue = consolehistory.PresentValue
                    };
                    logHistoryTable.Add(loghistory);
                }
            }
            Utility.LogEnd(methodBegin);
            return logHistoryTable;

        }

        /// <summary>
        /// GetSystemValidationForUnit
        /// </summary>
        /// <param Name="UnitId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetSystemValidationForUnit(int setId, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            var configureRequest = new ConfigureRequest();
            var userid = _configure.GetUserId(sessionId);
            var result = _unitConfigurationdl.GetDetailsForUnits(setId);
            var statusFlag = _unitConfigurationdl.GetSystemsValValues(setId, userid);
            var systemsValuesDBResult = _unitConfigurationdl.GetLatestSystemsValValues(setId, "Unit_ValInp", userid, null, Constant.REFRESHTYPE, result.UnitDetails, sessionId);
            // to get the response as expected
            var sectionTab = Constant.SUMMARYTAB;

            var unitaas = result.UnitLevelVariables;

            // to get the unitConfiguration response
            var unitVaribaleAssignmentValues = result.VariableAssignments.Where(s => s.Id.Equals(Constant.UNITVARIABLESLIST)).FirstOrDefault();
            //Converting ConfigureVariable to VariableAssignments
            List<VariableAssignment> lstvariableassignment = unitVaribaleAssignmentValues.VariableAssignments.Select(
                variableAssignment => new VariableAssignment
                {
                    VariableId = variableAssignment.VariableId,
                    Value = variableAssignment.Value
                }).ToList<VariableAssignment>();

            configureRequest.Line = new Line();

            configureRequest.Line.VariableAssignments = lstvariableassignment;
            var lineVariableAssignment = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureRequest.Line));
            var unitValues = result.UnitDetails;
            // need add the asynchronous job type
            var mainResponseValidations = BackgroundJob.Enqueue(() => GetSystemValidationMainValues(lstvariableassignment, unitValues, sessionId, sectionTab, setId, userid, unitaas, unitValues));
            //var unitResponseObj = await _configure.SystemValidationForUnitBl(lstvariableassignment, unitValues, sessionId, sectionTab).ConfigureAwait(false);                
            var conflictCacheValues = _configure.GetConflictCacheValues(sessionId, null);
            var mainResponse = conflictCacheValues;
            var configurationConflictExistCached = _cpqCacheManager.GetCache(sessionId, _environment, setId.ToString(), Constants.CONFIGURATIONCONFLICTS);
            if (!String.IsNullOrEmpty(configurationConflictExistCached))
            {
                var cachedVariable = Utility.DeserializeObjectValue<JObject>(configurationConflictExistCached)[Constants.BUILDINGSTATUS].ToString();
                if (cachedVariable.ToUpper().Equals(Constants.TRUEVALUES))
                {
                    mainResponse.ConfigurationConflictExists = true;
                }
                else
                {
                    mainResponse.ConfigurationConflictExists = false;
                }
            }
            foreach (var unit in unitValues)
            {
                if (!string.IsNullOrEmpty(unit.ProductName))
                {
                    switch (unit.ProductName.ToUpper())
                    {
                        case Constant.EVO_200:
                            unit.ProductName = Constant.EVOLUTION_200;
                            break;
                        case Constant.EVO_100:
                            unit.ProductName = Constant.EVOLUTION_100;
                            break;
                        default:
                            break;
                    }
                }
            }
            mainResponse.Units = JsonConvert.DeserializeObject<List<UnitNames>>(JsonConvert.SerializeObject(unitValues));
            mainResponse.ConfigurationStatus = "Completed";
            if (statusFlag != null)
            {
                mainResponse.SystemValidateStatus = statusFlag;
                if (mainResponse.SystemValidateStatus.StatusId == 0 && string.IsNullOrEmpty(mainResponse.SystemValidateStatus.StatusName))
                {
                    mainResponse.SystemValidateStatus.StatusName = string.Empty;
                }
                mainResponse.ConfiguratorStatus = statusFlag;
                if (mainResponse.ConfiguratorStatus.StatusId == 0 && string.IsNullOrEmpty(mainResponse.ConfiguratorStatus.StatusName))
                {
                    mainResponse.ConfiguratorStatus.StatusName = string.Empty;
                }
            }
            else
            {
                mainResponse.SystemValidateStatus = new Status()
                {
                    StatusKey = Constant.STATUSKEYUNIT_SVINP,
                    StatusName = Constant.INPROGRESSSTATUSCODE,
                    DisplayName = Constant.INPROGRESSSTATUS,
                    Description = Constant.INPROGRESSDESCRIPTIONSTATUS
                };
                mainResponse.ConfiguratorStatus = new Status()
                {
                    StatusKey = Constant.STATUSKEYUNIT_SVINP,
                    StatusName = Constant.INPROGRESSSTATUSCODE,
                    DisplayName = Constant.INPROGRESSSTATUS,
                    Description = Constant.INPROGRESSDESCRIPTIONSTATUS
                };
            }
            _configure.GetConflictCacheValues(sessionId, mainResponse);
            Utility.LogEnd(methodBegin);
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = Utility.FilterNullValues(mainResponse)
            };


        }

        /// <summary>
        /// GetLatestSystemValidationForUnit
        /// </summary>
        /// <param Name="UnitId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetLatestSystemValidationForUnit(int setId, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            var userid = _configure.GetUserId(sessionId);
            var result = _unitConfigurationdl.GetDetailsForTP2SummaryScreen(setId);
            var conflictCacheValues = _configure.GetConflictCacheValues(sessionId, null);

            var mainResponse = conflictCacheValues;
            var systemsValuesDBResult = _unitConfigurationdl.GetLatestSystemsValValues(setId, "Unit_ValInp", userid, null, Constant.REFRESHTYPE, result.UnitDetails, sessionId);
            if (mainResponse.ConflictAssignments == null)
            {
                mainResponse.ConflictAssignments = new ConflictManagement();
            }
            var conflictValues = mainResponse.ConflictAssignments;
            conflictValues.ValidationAssignments = systemsValuesDBResult.ConflictAssignments.ValidationAssignments;
            mainResponse.ConflictAssignments = conflictValues;
            mainResponse.SystemValidateStatus = systemsValuesDBResult.SystemValidateStatus;
            mainResponse.ConfiguratorStatus = systemsValuesDBResult.SystemValidateStatus;
            var unitValues = result.UnitDetails;
            foreach (var unit in unitValues)
            {
                if (!string.IsNullOrEmpty(unit.ProductName))
                {
                    switch (unit.ProductName.ToUpper())
                    {
                        case Constant.EVO_200:
                            unit.ProductName = Constant.EVOLUTION_200;
                            break;
                        default:
                            break;
                    }
                }
            }
            mainResponse.Units = JsonConvert.DeserializeObject<List<UnitNames>>(JsonConvert.SerializeObject(unitValues));
            mainResponse.ConfigurationStatus = "Completed";
            var buildingEquipmentStatusCached = _cpqCacheManager.GetCache(sessionId, _environment, setId.ToString(), Constants.BUILDINGEQUIPMENTSTATUSKEY);
            if (!String.IsNullOrEmpty(buildingEquipmentStatusCached))
            {
                var cachedVariable = Utility.DeserializeObjectValue<JObject>(buildingEquipmentStatusCached)[Constants.BUILDINGSTATUS].ToString();
                if (cachedVariable.Equals(Constants.BUILDINGEQUIPMENTCOMPLETESTATUSKEY))
                {
                    mainResponse.BuildingEquipmentConfigured = true;
                }
                else
                {
                    mainResponse.BuildingEquipmentConfigured = false;
                }

            }
            var configurationConflictExistCached = _cpqCacheManager.GetCache(sessionId, _environment, setId.ToString(), Constants.CONFIGURATIONCONFLICTS);
            if (!String.IsNullOrEmpty(configurationConflictExistCached))
            {
                var cachedVariable = Utility.DeserializeObjectValue<JObject>(configurationConflictExistCached)[Constants.BUILDINGSTATUS].ToString();
                if (cachedVariable.ToUpper().Equals(Constants.TRUEVALUES))
                {
                    mainResponse.ConfigurationConflictExists = true;
                }
                else
                {
                    mainResponse.ConfigurationConflictExists = false;
                }
            }
            _configure.GetConflictCacheValues(sessionId, mainResponse);
            Utility.LogEnd(methodBegin);
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = Utility.FilterNullValues(mainResponse)
            };
        }

        /// <summary>
        /// GetSystemValidationMainValues
        /// </summary>
        /// <param Name="lstVariableAssignment"></param>
        /// <param Name="unitValues"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="sectionTab"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetSystemValidationMainValues(List<VariableAssignment> lstVariableAssignment, List<UnitDetailsForTP2> unitValues, string sessionId, string sectionTab, int setId, string userId, List<UnitVariablesDetailsForTP2> unitVariablesDetailsForTP2s, List<UnitDetailsForTP2> UnitDetails)
        {
            var methodBegin = Utility.LogBegin();
            var statusKey = Constant.SYSVALUNIT_INV;
            var (mainResponse, systemVariables) = await _configure.SystemValidationForUnitBl(lstVariableAssignment, unitValues, sessionId, sectionTab, unitVariablesDetailsForTP2s).ConfigureAwait(false);
            var systemValidationConflicts = Utility.DeserializeObjectValue<List<SystemValidationKeyValues>>(Utility.SerializeObjectValue(mainResponse.ResponseArray));

            if (systemValidationConflicts != null && systemValidationConflicts.Any())
            {
                int idVlaue = 1;
                foreach (var item in systemValidationConflicts)
                {
                    item.Id = idVlaue;
                    if (string.IsNullOrEmpty(item.SystemValidKeys))
                    {
                        item.SystemValidKeys = string.Empty;
                        item.SystemValidValues = string.Empty;
                        item.StatusKey = Constant.SYSVALUNIT_VAL;
                    }
                    else
                    {
                        item.StatusKey = Constant.SYSVALUNIT_INV;
                    }
                    idVlaue++;
                }

                var statusFinder = systemValidationConflicts.Where(s => s.StatusKey.Equals(Constant.SYSVALUNIT_INV)).FirstOrDefault();
                if (statusFinder != null && !string.IsNullOrEmpty(statusFinder.StatusKey))
                {
                    statusKey = Constant.SYSVALUNIT_INV;
                }
                else
                {
                    statusKey = Constant.SYSVALUNIT_VAL;
                }
            }
            var savethevalue = _unitConfigurationdl.GetLatestSystemsValValues(setId, statusKey, userId, systemValidationConflicts, string.Empty, UnitDetails, sessionId, systemVariables);

            Utility.LogEnd(methodBegin);
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = Utility.FilterNullValues(mainResponse)
            };

        }

        /// <summary>
        /// SavePriceForTP2SummaryScreen
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="priceDetails"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SavePriceForTP2SummaryScreen(int setId, UnitSummaryUIModel priceDetails, string sessionId, List<UnitNames> unitPrices)
        {
            var methodBegin = Utility.LogBegin();
            var userId = _configure.GetUserId(sessionId);
            List<ConfigVariable> listOfLeadTimes = new List<ConfigVariable>();
            List<ConfigVariable> listOfVariables = new List<ConfigVariable>();
            var historyTable = new List<LogHistoryTable>();
            if (priceDetails != null)
            {
                listOfVariables = priceDetails.PriceValue.Select(
                variableAssignment => new ConfigVariable
                {
                    VariableId = variableAssignment.Key,
                    Value = variableAssignment.Value.totalPrice
                }).ToList<ConfigVariable>();
                foreach (var details in priceDetails.UserVariables)
                {
                    listOfVariables.Add(new ConfigVariable { VariableId = details.Key, Value = details.Value });
                }

                var priceKeys = (from keys in priceDetails.Sections[0].Variables
                                 select keys.ItemNumber).ToList();
                var unitMappingObjValues = JObject.Parse(File.ReadAllText(Constant.PRICEDETAILS));
                var priceValues = Utility.DeserializeObjectValue<List<PriceValuesDetails>>(Utility.SerializeObjectValue(unitMappingObjValues[Constant.PRICEKEYDETAILS]));
                var getFilterValues = (from values in priceValues
                                       from keys in priceKeys
                                       where Utility.CheckEquals(values.ItemNumber.Replace(Constant.HYPHENCHAR, Constant.UNDERSCORECHAR), keys)
                                       select values).ToList();
                var sectionToBatchmapper = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(unitMappingObjValues[Constants.SECTIONTOBATCH]));
                var tp2Price = _cpqCacheManager.GetCache(sessionId, _environment, Constant.PRICESECTION);
                if (tp2Price == null)
                {
                    throw new CustomException(new ResponseMessage()
                    {
                        StatusCode = Constant.INTERNALSERVERERROR,
                        Message = Constant.TP2SUMMARYERROR
                    });
                }
                var unitsTp2Response = Utility.DeserializeObjectValue<UnitSummaryUIModel>(tp2Price);
                foreach (var section in unitsTp2Response.PriceSections)
                {
                    if (sectionToBatchmapper.ContainsKey(section.Section))
                    {
                        foreach (var priceKey in section.PriceKeyInfo)
                        {
                            if (priceKey.ItemNumber.Contains(Constants.CUSTOMPRICEKEY, StringComparison.OrdinalIgnoreCase))
                            {

                                getFilterValues.Add(new PriceValuesDetails()
                                {
                                    BatchNo = sectionToBatchmapper[section.Section],
                                    LeadTime = priceKey.LeadTime
                                });
                            }
                        }
                    }
                }
                var manufacturingLeadTime = (from price in getFilterValues
                                             select Convert.ToInt32(price.LeadTime)).ToList().Max();
                Dictionary<string, Int32> leadTimePerBatch = new Dictionary<string, int>() { { Constant.BATCH1LEADTIME, 0 }, { Constant.BATCH2LEADTIME, 0 },
                { Constant.BATCH3LEADTIME, 0 }, { Constant.BATCH4LEADTIME, 0 }, { Constant.BATCH5LEADTIME, 0 } };
                foreach (var price in getFilterValues)
                {
                    if (price.BatchNo.Equals("1") && Convert.ToInt32(price.LeadTime) > leadTimePerBatch[Constant.BATCH1LEADTIME])
                    {
                        leadTimePerBatch[Constant.BATCH1LEADTIME] = Convert.ToInt32(price.LeadTime);
                    }
                    else if (price.BatchNo.Equals("2") && Convert.ToInt32(price.LeadTime) > leadTimePerBatch[Constant.BATCH2LEADTIME])
                    {
                        leadTimePerBatch[Constant.BATCH2LEADTIME] = Convert.ToInt32(price.LeadTime);
                    }
                    else if (price.BatchNo.Equals("3") && Convert.ToInt32(price.LeadTime) > leadTimePerBatch[Constant.BATCH3LEADTIME])
                    {
                        leadTimePerBatch[Constant.BATCH3LEADTIME] = Convert.ToInt32(price.LeadTime);
                    }
                    else if (price.BatchNo.Equals("4") && Convert.ToInt32(price.LeadTime) > leadTimePerBatch[Constant.BATCH4LEADTIME])
                    {
                        leadTimePerBatch[Constant.BATCH4LEADTIME] = Convert.ToInt32(price.LeadTime);
                    }
                    else if (price.BatchNo.Equals("5") && Convert.ToInt32(price.LeadTime) > leadTimePerBatch[Constant.BATCH5LEADTIME])
                    {
                        leadTimePerBatch[Constant.BATCH5LEADTIME] = Convert.ToInt32(price.LeadTime);
                    }
                }
                listOfLeadTimes = leadTimePerBatch.Select(
                    variableAssignment => new ConfigVariable
                    {
                        VariableId = variableAssignment.Key,
                        Value = variableAssignment.Value
                    }).ToList<ConfigVariable>();
                listOfLeadTimes.Add(new ConfigVariable { VariableId = Constant.MANUFACTURINGLEADTIME, Value = manufacturingLeadTime });
                listOfLeadTimes = (from leadTime in listOfLeadTimes
                                   where !Utility.CheckEquals(leadTime.Value.ToString(), "0")
                                   select leadTime).ToList();
            }

            if (unitPrices.Count == 0)
            {
                var priceResponse = Utility.DeserializeObjectValue<UnitSummaryUIModel>(_cpqCacheManager.GetCache(sessionId, _environment, Constant.PRICESECTION));
                unitPrices = priceResponse.Units;
            }

            var result = _unitConfigurationdl.SavePriceValuesDL(setId, listOfVariables, listOfLeadTimes, userId, historyTable, unitPrices);
            var response = JArray.FromObject(result);
            if (result[0].result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.PRICEDETAILSSAVEERROR,
                    Description = result[0].message
                });
            }

        }

        private List<EntranceLocations> UpdatePropertyForEntranceControllerConsole(List<EntranceLocations> listEntranceLocation, Dictionary<string, List<int>> dictSelectedFloor, int controlFloorNumber)
        {
            var lstSelectedFrontFloorNumber = dictSelectedFloor[Constant.FRONTOPENING];
            var lstSelectedRearFloorNumber = dictSelectedFloor[Constant.REAROPENING];
            foreach (var varLocation in listEntranceLocation)
            {
                if (varLocation.FloorNumber == controlFloorNumber)
                {
                    varLocation.Front.InCompatible = lstSelectedFrontFloorNumber.Count > 0 && lstSelectedFrontFloorNumber.Contains(varLocation.FloorNumber);
                    varLocation.Rear.InCompatible = lstSelectedRearFloorNumber.Count > 0 && lstSelectedRearFloorNumber.Contains(varLocation.FloorNumber);
                }
                else
                {
                    varLocation.Front.NotAvailable = true;
                    varLocation.Rear.NotAvailable = true;
                }
            }
            return listEntranceLocation;
        }
        private List<EntranceLocations> UpdatePropertyForEntranceConsole(List<EntranceLocations> listEntranceLocation, Dictionary<string, List<int>> dictSelectedFloor, EntranceLocations controlFloorDetails, int consoleId)
        {
            var controlFloorNumber = controlFloorDetails != null ? controlFloorDetails.FloorNumber : 0;
            var lstSelectedFrontFloorNumber = dictSelectedFloor[Constant.FRONTOPENING];
            var lstSelectedRearFloorNumber = dictSelectedFloor[Constant.REAROPENING];
            foreach (var varLocation in listEntranceLocation)
            {
                if (varLocation.FloorNumber == controlFloorNumber)
                {
                    varLocation.Front.NotAvailable = controlFloorDetails != null && controlFloorDetails.Front.Value.Equals(true);
                    varLocation.Rear.NotAvailable = controlFloorDetails != null && controlFloorDetails.Rear.Value.Equals(true);
                }
                else
                {
                    if (lstSelectedFrontFloorNumber.Count > 0)
                    {
                        if (consoleId == 0)
                        {
                            varLocation.Front.InCompatible = lstSelectedFrontFloorNumber.Contains(varLocation.FloorNumber);
                            varLocation.Front.Value = false; // !(lstSelectedFrontFloorNumber.Contains(varLocation.FloorNumber)) && !varLocation.Front.NotAvailable;
                        }
                        else
                        {
                            varLocation.Front.InCompatible = lstSelectedFrontFloorNumber.Contains(varLocation.FloorNumber);
                        }
                    }
                    if (lstSelectedRearFloorNumber.Count > 0)
                    {
                        if (consoleId == 0)
                        {
                            varLocation.Rear.InCompatible = lstSelectedRearFloorNumber.Contains(varLocation.FloorNumber);
                            varLocation.Rear.Value = !(lstSelectedRearFloorNumber.Contains(varLocation.FloorNumber)) && !varLocation.Rear.NotAvailable;
                        }
                        else
                        {
                            varLocation.Rear.InCompatible = lstSelectedRearFloorNumber.Contains(varLocation.FloorNumber);
                        }
                    }
                }

            }
            return listEntranceLocation;
        }
        private EntranceConfigurations StartEntranceConfigurationConsoleBL(List<EntranceConfigurations> entranceConsoles, int consoleId, string SessionId, int setId)
        {
            var lstSelectedFrontFloorNumber = (from varEntranceConsole in entranceConsoles
                                               from EntranceLocations in varEntranceConsole.FixtureLocations
                                               where EntranceLocations.Front.Value.Equals(true) && varEntranceConsole.EntranceConsoleId != consoleId
                                               select EntranceLocations.FloorNumber).ToList();
            var lstSelectedRearFloorNumber = (from varEntranceConsole in entranceConsoles
                                              from EntranceLocations in varEntranceConsole.FixtureLocations
                                              where EntranceLocations.Rear.Value.Equals(true) && varEntranceConsole.EntranceConsoleId != consoleId
                                              select EntranceLocations.FloorNumber).ToList();
            Dictionary<string, List<int>> dictSelectedFloor = new Dictionary<string, List<int>>
            {
                { Constant.FRONTOPENING, lstSelectedFrontFloorNumber },
                { Constant.REAROPENING, lstSelectedRearFloorNumber }
            };
            var maxConsoleId = (from entrance in entranceConsoles select entrance.EntranceConsoleId).ToList().Max();
            var entranceConsole = entranceConsoles.Where(x => x.EntranceConsoleId.Equals(consoleId)).ToList().FirstOrDefault();
            var controlFloorNumber = (from varEntranceConsole in entranceConsoles
                                      from EntranceLocations in varEntranceConsole.FixtureLocations
                                      where (EntranceLocations.Rear.Value.Equals(true) || EntranceLocations.Front.Value.Equals(true)) && varEntranceConsole.IsController == true
                                      select EntranceLocations.FloorNumber).ToList().FirstOrDefault();
            EntranceLocations controlFloorDetails = (from varEntranceConsole in entranceConsoles
                                                     from EntranceLocations in varEntranceConsole.FixtureLocations
                                                     where (EntranceLocations.FloorNumber.Equals(controlFloorNumber)) && varEntranceConsole.IsController == true
                                                     select EntranceLocations).ToList().FirstOrDefault();

            var newConsole = new EntranceConfigurations();
            if (entranceConsole != null)
            {
                var newentranceLocation = entranceConsole.FixtureLocations;
                if (entranceConsole.IsController)
                {
                    newentranceLocation = UpdatePropertyForEntranceControllerConsole(entranceConsole.FixtureLocations, dictSelectedFloor, controlFloorNumber);
                }
                else
                {
                    newentranceLocation = UpdatePropertyForEntranceConsole(entranceConsole.FixtureLocations, dictSelectedFloor, controlFloorDetails, consoleId);
                }
                if (consoleId == 0)
                {
                    var productType = _configure.SetCacheProductType(null, SessionId, setId).FirstOrDefault().Value.ToString();
                    string defaultVariables = string.Empty;
                    defaultVariables = JObject.Parse(File.ReadAllText(string.Format(Constants.ENTRANCESCONSOLEDEFAULTVALUES, Constant.EVOLUTION200))).ToString();
                    if (productType.Equals(Constant.EVO_200))
                    {
                        defaultVariables = JObject.Parse(File.ReadAllText(string.Format(Constants.ENTRANCESCONSOLEDEFAULTVALUES, Constant.EVOLUTION200))).ToString();
                    }
                    else if (productType.Equals(Constant.ENDURA_100))
                    {
                        defaultVariables = JObject.Parse(File.ReadAllText(string.Format(Constants.ENTRANCESCONSOLEDEFAULTVALUES, Constants.END100))).ToString();
                    }
                    else if (productType.Equals(Constant.MODEL_EVO100))
                    {
                        defaultVariables = JObject.Parse(File.ReadAllText(string.Format(Constants.ENTRANCESCONSOLEDEFAULTVALUES, Constants.EVOLUTION100))).ToString();
                    }
                    var defaultEntranceConfigurationValues = Utility.DeserializeObjectValue<Dictionary<string, Dictionary<string, string>>>(defaultVariables);
                    List<ConfigVariable> defaultValues = new List<ConfigVariable>();
                    if (defaultEntranceConfigurationValues.ContainsKey(Constants.NONCONTROLLERCONSOLE))
                    {
                        foreach (var defaults in defaultEntranceConfigurationValues[Constants.NONCONTROLLERCONSOLE])
                        {
                            defaultValues.Add(new ConfigVariable { VariableId = defaults.Key, Value = defaults.Value });
                        }
                    }
                    newConsole.EntranceConsoleId = maxConsoleId + 1;
                    newConsole.ConsoleName = entranceConsole.IsController ? Constant.ENTRANCECONSOLE + maxConsoleId : Constant.ENTRANCECONSOLE + (maxConsoleId + 1);
                    newConsole.FixtureLocations = newentranceLocation;
                    newConsole.AssignOpenings = true;
                    newConsole.IsController = false;
                    newConsole.ProductName = entranceConsoles[0].ProductName;
                    newConsole.Openings = entranceConsoles[0].Openings;
                    newConsole.NoOfFloor = entranceConsoles[0].NoOfFloor;
                    newConsole.VariableAssignments = defaultValues;
                }
                else
                {
                    newConsole = entranceConsole;
                    newConsole.FixtureLocations = newentranceLocation;

                }
            }
            return newConsole;
        }
        private EntranceConfigurations ChangeEntranceConfigurationConsoleBL(List<EntranceConfigurations> entranceConsoles, EntranceConfigurationData objEntranceConfiguration)
        {
            var newConsole = new EntranceConfigurations();
            //List of selected front landing 
            var lstSelectedFrontFloorNumber = (from varEntranceConsole in entranceConsoles
                                               from EntranceLocations in varEntranceConsole.FixtureLocations
                                               where EntranceLocations.Front.Value.Equals(true) && varEntranceConsole.EntranceConsoleId.Equals(objEntranceConfiguration.EntranceConsoleId)
                                               select EntranceLocations.FloorNumber).ToList();
            //List of selected rear landing
            var lstSelectedRearFloorNumber = (from varEntranceConsole in entranceConsoles
                                              from EntranceLocations in varEntranceConsole.FixtureLocations
                                              where EntranceLocations.Rear.Value.Equals(true) && varEntranceConsole.EntranceConsoleId.Equals(objEntranceConfiguration.EntranceConsoleId)
                                              select EntranceLocations.FloorNumber).ToList();
            //control landing
            var controlFloorNumber = (from varEntranceConsole in entranceConsoles
                                      from EntranceLocations in varEntranceConsole.FixtureLocations
                                      where (EntranceLocations.Rear.Value.Equals(true) || EntranceLocations.Front.Value.Equals(true)) && varEntranceConsole.IsController == true
                                      select EntranceLocations.FloorNumber).ToList().FirstOrDefault();
            //control landing details
            EntranceLocations controlFloorDetails = (from varEntranceConsole in entranceConsoles
                                                     from EntranceLocations in varEntranceConsole.FixtureLocations
                                                     where (EntranceLocations.FloorNumber.Equals(controlFloorNumber)) && varEntranceConsole.IsController == true
                                                     select EntranceLocations).ToList().FirstOrDefault();
            newConsole = entranceConsoles.Where(x => x.EntranceConsoleId.Equals(Convert.ToInt32(objEntranceConfiguration.EntranceConsoleId))).ToList().FirstOrDefault();
            newConsole.VariableAssignments = objEntranceConfiguration.VariableAssignments;
            foreach (var entranceLocation in newConsole.FixtureLocations)
            {
                foreach (var entrancedata in objEntranceConfiguration.EntranceLocations)
                {
                    if (entranceLocation.FloorNumber == entrancedata.FloorNumber)
                    {
                        entranceLocation.Front.Value = entrancedata.Front;
                        entranceLocation.Rear.Value = entrancedata.Rear;
                        if (entranceLocation.FloorNumber == controlFloorNumber)
                        {
                            entranceLocation.Front.InCompatible = controlFloorDetails.Front.Value.Equals(true);
                            entranceLocation.Front.NotAvailable = controlFloorDetails.Front.Value.Equals(true);
                            entranceLocation.Rear.InCompatible = controlFloorDetails.Rear.Value.Equals(true);
                            entranceLocation.Rear.NotAvailable = controlFloorDetails.Rear.Value.Equals(true);
                        }
                    }
                }
            }
            return newConsole;
        }

        public async Task<ResponseMessage> ValidateUnitHallFixtureConsoles(int setId, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            Utility.LogEnd(methodBegin);
            var hallLanternVariableValue = String.Empty;
            var unitHallFixtureConsoles = _configure.SetCacheUnitHallFixtureConsoles(null, sessionId, setId);
            var hallLanternVariable = _cpqCacheManager.GetCache(sessionId, _environment, "HALLLANTERNVARIABLE");
            var fixtureStrategy = _cpqCacheManager.GetCache(sessionId, _environment, setId.ToString(), Constants.FIXTURESTRATEGYOFCURRENTSET);
            var carRidingLanternQuantity = _cpqCacheManager.GetCache(sessionId, _environment, setId.ToString(), Constants.CARRIDINGLANTERNQUANT);
            if (!String.IsNullOrEmpty(fixtureStrategy))
            {
                fixtureStrategy = fixtureStrategy.ToString();
            }
            var flag = 0;
            var noOfHallLanternConsoles = 0;
            if (!String.IsNullOrEmpty(hallLanternVariable))
            {
                var cachedVariable = Utility.DeserializeObjectValue<JObject>(hallLanternVariable);
                var setIdOfCachedData = Convert.ToInt32(cachedVariable[Constant.SETID]);
                if (setId.Equals(setIdOfCachedData))
                {
                    hallLanternVariableValue = cachedVariable[Constant.VALUE].ToString();
                }
            }
            foreach (var subsection in unitHallFixtureConsoles)
            {
                if (!String.IsNullOrEmpty(subsection.UnitHallFixtureType) && 
                   (subsection.UnitHallFixtureType.Equals(Constant.HALLLANTERNCONSOLE) || subsection.UnitHallFixtureType.Equals(Constant.COMBOHALLLANTERNPICAMELCASE)))
                {
                    noOfHallLanternConsoles += 1;
                }
                if (!String.IsNullOrEmpty(subsection.UnitHallFixtureType) && (subsection.UnitHallFixtureType.Equals(Constant.BRAILLECONSOLE) || subsection.UnitHallFixtureType.Equals(Constant.ELEVATORFLOORBRAILLECONSOLE) ||
               subsection.UnitHallFixtureType.Equals(Constant.HALLLANTERNCONSOLE) || subsection.UnitHallFixtureType.Equals(Constant.COMBOHALLLANTERNPICAMELCASE)) ||
               hallLanternVariableValue.Equals(Constant.NR))
                {
                    var consolesToCheck = new List<string>();
                    if (!String.IsNullOrEmpty(subsection.UnitHallFixtureType) && subsection.UnitHallFixtureType.Equals(Constant.BRAILLECONSOLE))
                    {
                        consolesToCheck.Add(Constant.BRAILLECONSOLE);
                    }
                    else if (!String.IsNullOrEmpty(hallLanternVariableValue) && hallLanternVariableValue.Equals(Constant.NR))
                    {
                        consolesToCheck.Add(Constant.HALLLANTERNCONSOLE);
                        consolesToCheck.Add(Constant.COMBOHALLLANTERNPICAMELCASE);
                    }
                    else if (!String.IsNullOrEmpty(carRidingLanternQuantity) && Convert.ToInt32(carRidingLanternQuantity) == 0 && subsection.UnitHallFixtureType.Equals(Constant.HALLLANTERNCONSOLE))
                    {
                        consolesToCheck.Add(Constant.HALLLANTERNCONSOLE);
                        consolesToCheck.Add(Constant.COMBOHALLLANTERNPICAMELCASE);
                    }
                    var consolesPerSubsection = (from console in unitHallFixtureConsoles
                                                 where console.UnitHallFixtureType != null && consolesToCheck.Contains(console.UnitHallFixtureType)
                                                 select console).ToList();
                    var lstSelectedFrontOpenings = (from console in unitHallFixtureConsoles
                                                    where console.UnitHallFixtureType != null && consolesToCheck.Contains(console.UnitHallFixtureType)
                                                    from location in console.UnitHallFixtureLocations
                                                    where location.Front.Value.Equals(true)
                                                    select location.FloorNumber).Distinct().ToList();
                    var lstSelectedRearOpenings = (from console in unitHallFixtureConsoles
                                                   where console.UnitHallFixtureType != null && consolesToCheck.Contains(console.UnitHallFixtureType)
                                                   from location in console.UnitHallFixtureLocations
                                                   where location.Rear.Value.Equals(true)
                                                   select location.FloorNumber).Distinct().ToList();
                    var FrontOpenings = consolesPerSubsection.Count > 0 ? consolesPerSubsection[0].UnitHallFixtureLocations.Where(x => x.Front.NotAvailable.ToString().ToUpper().Equals(Constant.FALSEVALUES)).Count() : 0;
                    var RearOpenings = consolesPerSubsection.Count > 0 ? consolesPerSubsection[0].UnitHallFixtureLocations.Where(x => x.Rear.NotAvailable.ToString().ToUpper().Equals(Constant.FALSEVALUES)).Count() : 0;
                    var isFront = consolesPerSubsection.Count > 0 && consolesPerSubsection[0].Openings.Front;
                    var isRear = consolesPerSubsection.Count > 0 && consolesPerSubsection[0].Openings.Rear;
                    var AllOpeningsSelected = isFront ? isRear ? lstSelectedFrontOpenings.Count == FrontOpenings && lstSelectedRearOpenings.Count == RearOpenings
                                                       : lstSelectedFrontOpenings.Count == FrontOpenings : !isRear || lstSelectedRearOpenings.Count == RearOpenings;
                    if (AllOpeningsSelected.Equals(false))
                    {
                        flag = 1;
                        break;
                    }
                }
            }

            if (!String.IsNullOrEmpty(hallLanternVariableValue) && hallLanternVariableValue.Equals(Constant.NR) && noOfHallLanternConsoles == 0 &&
                !String.IsNullOrEmpty(fixtureStrategy) && !fixtureStrategy.Equals(Constant.ETD))
            {
                flag = 1;
            }
            else if (!String.IsNullOrEmpty(carRidingLanternQuantity) && Convert.ToInt32(carRidingLanternQuantity) == 0 && noOfHallLanternConsoles == 0)
            {
                flag = 1;
            }

            if (flag == 1)
            {
                Utility.LogEnd(methodBegin);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.UNITHALLFIXTUREVALIDATIONERRORMESSAGE,
                    Description = Constant.UNITHALLFIXTUREVALIDATIONERRORMESSAGE
                });
            }
            else
            {
                List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
                ResultSetConfiguration result = new ResultSetConfiguration() { result = 1, setId = setId, message = Constant.UNITCONFIGURATIONSAVEMESSAGE };
                lstResult.Add(result);
                var res = JArray.FromObject(lstResult);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = res };
            }
        }

        /// <summary>
        /// ChangeTP2SummaryDetails
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> ChangeTP2SummaryDetails(int unitId, string sessionId, UnitSummaryUIModel requestBody)
        {
            var methodBegin = Utility.LogBegin();
            var mainResponse = new UnitSummaryUIModel();
            var unitResponseObj = _cpqCacheManager.GetCache(sessionId, _environment, Constant.PRICESECTION);
            if (string.IsNullOrEmpty(unitResponseObj))
            {
                var res = await GetDetailsForTP2SummaryScreen(unitId, sessionId).ConfigureAwait(false);
                mainResponse = Utility.DeserializeObjectValue<UnitSummaryUIModel>(Utility.SerializeObjectValue(res.Response));
            }
            else
            {
                mainResponse = Utility.DeserializeObjectValue<UnitSummaryUIModel>(unitResponseObj);
            }
            decimal.TryParse(requestBody.UserVariables[Constants.CORPORATEASSISTANCE], out decimal corporateAssistance);
            decimal.TryParse(requestBody.UserVariables[Constants.STRATEGICDISCOUNT], out decimal strategicDiscount);
            var totalValue = corporateAssistance + strategicDiscount;
            var priceBeforeDiscount = (from units in mainResponse.Units
                                       select units.Price).ToList().Min();
            if (totalValue > priceBeforeDiscount)
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constants.INTERNALSERVERERROR,
                    Message = Constants.SAVEDISCOUNTERRORMESSAGE
                });
            }
            if (totalValue > 0)
            {
                mainResponse.Units = GetUnitsBasePrice(mainResponse.Units, sessionId);
                var totalCustomPrice = _cpqCacheManager.GetCache(sessionId, _environment, Constants.TOTALCUSTOMPRICE);
                foreach (var unit in mainResponse.Units)
                {
                    unit.Price += !string.IsNullOrEmpty(totalCustomPrice) ? decimal.Parse(totalCustomPrice) : 0;
                }
                mainResponse = AdjustDiscounts(totalValue, mainResponse);
            }
            mainResponse.UserVariables = requestBody.UserVariables;
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.PRICESECTION, Utility.SerializeObjectValue(mainResponse));
            Utility.LogEnd(methodBegin);
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Message = Constant.UNITDESIGNATIONMESSAGE,
                Response = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(mainResponse))
            };


        }

        private void SetIncompatiblePropertyForMutuallyExclusiveConsoles(UnitHallFixtures console, List<UnitHallFixtures> cachedConsoles)
        {
            var lstSelectedFrontFloorNumber = (from varUnitHallFixtureConsole in cachedConsoles
                                               where varUnitHallFixtureConsole.UnitHallFixtureType.ToUpper().Equals(console.UnitHallFixtureType.ToUpper())
                                               from EntranceLocations in varUnitHallFixtureConsole.UnitHallFixtureLocations
                                               where EntranceLocations.Front.Value.Equals(true) && varUnitHallFixtureConsole.ConsoleId != console.ConsoleId
                                               select EntranceLocations.FloorNumber).ToList();
            var lstSelectedRearFloorNumber = (from varUnitHallFixtureConsole in cachedConsoles
                                              where varUnitHallFixtureConsole.UnitHallFixtureType.ToUpper().Equals(console.UnitHallFixtureType.ToUpper())
                                              from EntranceLocations in varUnitHallFixtureConsole.UnitHallFixtureLocations
                                              where EntranceLocations.Rear.Value.Equals(true) && varUnitHallFixtureConsole.ConsoleId != console.ConsoleId
                                              select EntranceLocations.FloorNumber).ToList();
            if (console.UnitHallFixtureType.ToUpper().Equals(Constant.HALL_PI) || console.UnitHallFixtureType.ToUpper().Equals(Constant.HALLLANTERN))
            {
                lstSelectedFrontFloorNumber.AddRange(from varUnitHallFixtureConsole in cachedConsoles
                                                     where varUnitHallFixtureConsole.UnitHallFixtureType.ToUpper().Equals(Constant.COMBO_HALL_LANTERN_PI)
                                                     from EntranceLocations in varUnitHallFixtureConsole.UnitHallFixtureLocations
                                                     where EntranceLocations.Front.Value.Equals(true)
                                                     select EntranceLocations.FloorNumber);
                lstSelectedRearFloorNumber.AddRange(from varUnitHallFixtureConsole in cachedConsoles
                                                    where varUnitHallFixtureConsole.UnitHallFixtureType.ToUpper().Equals(Constant.COMBO_HALL_LANTERN_PI)
                                                    from EntranceLocations in varUnitHallFixtureConsole.UnitHallFixtureLocations
                                                    where EntranceLocations.Rear.Value.Equals(true)
                                                    select EntranceLocations.FloorNumber);
            }
            if (console.UnitHallFixtureType.ToUpper().Equals(Constant.COMBO_HALL_LANTERN_PI))
            {
                lstSelectedFrontFloorNumber.AddRange(from varUnitHallFixtureConsole in cachedConsoles
                                                     where varUnitHallFixtureConsole.UnitHallFixtureType.ToUpper().Equals(Constant.HALLLANTERN) ||
                                                     varUnitHallFixtureConsole.UnitHallFixtureType.ToUpper().Equals(Constant.HALL_PI)
                                                     from EntranceLocations in varUnitHallFixtureConsole.UnitHallFixtureLocations
                                                     where EntranceLocations.Front.Value.Equals(true)
                                                     select EntranceLocations.FloorNumber);
                lstSelectedRearFloorNumber.AddRange(from varUnitHallFixtureConsole in cachedConsoles
                                                    where varUnitHallFixtureConsole.UnitHallFixtureType.ToUpper().Equals(Constant.HALLLANTERN) ||
                                                    varUnitHallFixtureConsole.UnitHallFixtureType.ToUpper().Equals(Constant.HALL_PI)
                                                    from EntranceLocations in varUnitHallFixtureConsole.UnitHallFixtureLocations
                                                    where EntranceLocations.Rear.Value.Equals(true)
                                                    select EntranceLocations.FloorNumber);
            }

            foreach (var varLocation in console.UnitHallFixtureLocations)
            {
                if (lstSelectedFrontFloorNumber.Count > 0)
                {
                    if (lstSelectedFrontFloorNumber.Contains(varLocation.FloorNumber))
                    {
                        varLocation.Front.InCompatible = true;
                    }
                }
                if (lstSelectedRearFloorNumber.Count > 0)
                {
                    if (lstSelectedRearFloorNumber.Contains(varLocation.FloorNumber))
                    {
                        varLocation.Rear.InCompatible = true;
                    }
                }
            }
        }



        public async Task<ResponseMessage> SaveCustomPriceLine(int setId, string sessionId, List<CustomPriceLine> customPriceLine)
        {
            var methodBegin = Utility.LogBegin();
            var tp2Price = _cpqCacheManager.GetCache(sessionId, _environment, Constant.PRICESECTION);
            if (tp2Price == null)
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR,
                    Message = Constant.TP2SUMMARYERROR
                });
            }
            var unitsTp2Response = Utility.DeserializeObjectValue<UnitSummaryUIModel>(tp2Price);
            var saveCustomPriceResponse = _unitConfigurationdl.SaveCustomPriceLine(setId, _configure.GetUserId(sessionId), customPriceLine);
            unitsTp2Response = AssignCustomPriceLineForSave(unitsTp2Response, saveCustomPriceResponse, sessionId);
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.PRICESECTION, Utility.SerializeObjectValue(unitsTp2Response));
            Utility.LogEnd(methodBegin);
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Message = Constant.UNITDESIGNATIONMESSAGE,
                Response = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(unitsTp2Response))
            };


        }

        public async Task<ResponseMessage> EditCustomPriceLine(int setId, string sessionId, CustomPriceLine customPriceLine)
        {
            var methodBegin = Utility.LogBegin();
            if (customPriceLine != null)
            {
                customPriceLine.UserId = _configure.GetUserId(sessionId);
            }
            var tp2Price = _cpqCacheManager.GetCache(sessionId, _environment, Constant.PRICESECTION);
            if (tp2Price == null)
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR,
                    Message = Constant.TP2SUMMARYERROR
                });
            }
            var unitsTp2Response = Utility.DeserializeObjectValue<UnitSummaryUIModel>(tp2Price);
            var editCustomPriceResponse = _unitConfigurationdl.EditCustomPriceLine(setId, sessionId, customPriceLine);
            unitsTp2Response = AssignCustomPriceLine(unitsTp2Response, editCustomPriceResponse, sessionId);
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.PRICESECTION, Utility.SerializeObjectValue(unitsTp2Response));
            Utility.LogEnd(methodBegin);
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Message = Constant.UNITDESIGNATIONMESSAGE,
                Response = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(unitsTp2Response))
            };


        }

        private UnitSummaryUIModel AssignCustomPriceLine(UnitSummaryUIModel unitsTp2Response, CustomPriceLine upsertResponse, string sessionId)
        {
            decimal totalPrice = 0;
            decimal priceValue = 0;
            decimal totalEstimationPrice = 0;
            if (upsertResponse.PriceValue.ToList().Any())
            {
                totalPrice = totalPrice + upsertResponse.PriceValue.ToList()[0].Value.quantity * upsertResponse.PriceValue.ToList()[0].Value.unitPrice;
                var priceKey = unitsTp2Response.PriceValue.Where(x => Utilities.CheckEquals(x.Key, upsertResponse.PriceValue.ToList()[0].Key)).ToList();
                if (priceKey.Any())
                {
                    priceValue = priceValue + priceKey[0].Value.totalPrice;
                    unitsTp2Response.PriceValue[priceKey[0].Key] = upsertResponse.PriceValue.ToList()[0].Value;
                }
                else
                {
                    unitsTp2Response.PriceValue.Add(upsertResponse.PriceValue.ToList()[0].Key, upsertResponse.PriceValue.ToList()[0].Value);
                }
            }
            foreach (var priceSection in unitsTp2Response.PriceSections)
            {
                if (Utilities.CheckEquals(priceSection.Section, upsertResponse.priceKeyInfo.Section))
                {
                    priceSection.PriceKeyInfo = priceSection.PriceKeyInfo.Where(x => x.PriceKeyId != upsertResponse.priceKeyInfo.PriceKeyId).ToList();
                    priceSection.PriceKeyInfo.Add(upsertResponse.priceKeyInfo);
                }
            }
            foreach (var unit in unitsTp2Response.Units)
            {
                unit.Price = unit.Price + totalPrice;
                unit.Price = unit.Price - priceValue;
                totalEstimationPrice = totalEstimationPrice + unit.Price;
            }
            var totalCustomPrice = GetTotalCustomPrice(unitsTp2Response);
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.TOTALCUSTOMPRICE, Convert.ToString(totalCustomPrice));
            if (unitsTp2Response.PriceValue.ContainsKey(Constants.TOTALPRICEVALUES))
            {
                unitsTp2Response.PriceValue[Constants.TOTALPRICEVALUES].totalPrice = totalEstimationPrice;
                unitsTp2Response.PriceValue[Constants.TOTALPRICEVALUES].unitPrice = totalEstimationPrice;
            }
            return unitsTp2Response;
        }
        public async Task<ResponseMessage> DeleteCustomPriceLine(int setId, string sessionId, int priceLineId)
        {
            var methodBegin = Utility.LogBegin();
            var tp2Price = _cpqCacheManager.GetCache(sessionId, _environment, Constant.PRICESECTION);
            if (tp2Price == null)
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR,
                    Message = Constant.TP2SUMMARYERROR
                });
            }
            var unitsTp2Response = Utility.DeserializeObjectValue<UnitSummaryUIModel>(tp2Price);
            var deleteCustomPriceResponse = _unitConfigurationdl.DeleteCustomPriceLine(setId, priceLineId);

            foreach (var priceSection in unitsTp2Response.PriceSections)
            {
                var priceKeyData = priceSection.PriceKeyInfo.Where(x => x.PriceKeyId == priceLineId).ToList();
                if (priceKeyData.Any())
                {
                    var priceKey = unitsTp2Response.PriceValue.Where(x => Utilities.CheckEquals(x.Key, priceKeyData[0].ItemNumber)).ToList();
                    if (priceKey.Any())
                    {
                        decimal totalPrice = 0;
                        foreach (var unit in unitsTp2Response.Units)
                        {
                            unit.Price = unit.Price - priceKey[0].Value.totalPrice;
                            totalPrice = totalPrice + unit.Price;
                        }
                        unitsTp2Response.PriceValue["totalPrice"].unitPrice = totalPrice;
                        unitsTp2Response.PriceValue["totalPrice"].totalPrice = totalPrice;
                    }
                }
                priceSection.PriceKeyInfo = (from priceKeyInfo in priceSection.PriceKeyInfo
                                             where priceKeyInfo.PriceKeyId != priceLineId
                                             select priceKeyInfo).ToList();
            }
            var totalCustomPrice = GetTotalCustomPrice(unitsTp2Response);
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.TOTALCUSTOMPRICE, Convert.ToString(totalCustomPrice));
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.PRICESECTION, Utility.SerializeObjectValue(unitsTp2Response));
            Utility.LogEnd(methodBegin);
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Message = Constant.UNITDESIGNATIONMESSAGE,
                Response = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(unitsTp2Response))
            };


        }

        /// <summary>
        /// Method to get the Total custom price fro all sections
        /// </summary>
        /// <param name="unitsTp2Response"></param>
        /// <returns></returns>

        private decimal GetTotalCustomPrice(UnitSummaryUIModel unitsTp2Response)
        {
            decimal totalPrice = 0;
            foreach (var priceSection in unitsTp2Response.PriceSections)
            {
                if (priceSection != null)
                {
                    foreach (var priceKey in priceSection.PriceKeyInfo)
                    {
                        if (priceKey.PriceKeyId > 0 && unitsTp2Response.PriceValue != null)
                        {
                            foreach (var priceValue in unitsTp2Response.PriceValue.ToList())
                            {
                                if (Utility.CheckEquals(priceValue.Key, priceKey.ItemNumber))
                                {
                                    totalPrice += priceValue.Value.totalPrice;
                                }
                            }
                        }
                    }
                }
            }
            return totalPrice;
        }

        private UnitSummaryUIModel AssignCustomPriceLineForSave(UnitSummaryUIModel unitsTp2Response, List<CustomPriceLine> upsertResponse, string sessionId)
        {
            decimal totalPrice = 0;
            decimal totalEstimationPrice = 0;
            foreach (var priceLine in upsertResponse)
            {
                if (priceLine.PriceValue.ToList().Any())
                {
                    totalPrice = totalPrice + priceLine.PriceValue.ToList()[0].Value.quantity * priceLine.PriceValue.ToList()[0].Value.unitPrice;
                    if (unitsTp2Response.PriceValue.ContainsKey(priceLine.PriceValue.ToList()[0].Key))
                    {
                        unitsTp2Response.PriceValue[priceLine.PriceValue.ToList()[0].Key] = priceLine.PriceValue.ToList()[0].Value;
                    }
                    else
                    {
                        unitsTp2Response.PriceValue.Add(priceLine.PriceValue.ToList()[0].Key, priceLine.PriceValue.ToList()[0].Value);
                    }
                }
                foreach (var priceSection in unitsTp2Response.PriceSections)
                {
                    if (Utilities.CheckEquals(priceSection.Section, priceLine.priceKeyInfo.Section))
                    {
                        priceSection.PriceKeyInfo.Add(priceLine.priceKeyInfo);
                    }
                }
            }
            foreach (var unit in unitsTp2Response.Units)
            {
                unit.Price = unit.Price + totalPrice;
                totalEstimationPrice = totalEstimationPrice + unit.Price;
            }
            var totalCustomPrice = GetTotalCustomPrice(unitsTp2Response);
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.TOTALCUSTOMPRICE, Convert.ToString(totalCustomPrice));
            if (unitsTp2Response.PriceValue.ContainsKey(Constants.TOTALPRICEVALUES))
            {
                unitsTp2Response.PriceValue[Constants.TOTALPRICEVALUES].totalPrice = totalEstimationPrice;
                unitsTp2Response.PriceValue[Constants.TOTALPRICEVALUES].unitPrice = totalEstimationPrice;
            }
            return unitsTp2Response;
        }
        /// <summary>
        ///  create Update Factory Job ID
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="sessionId"></param>
        /// <param name="factoryJobId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> CreateUpdateFactoryJobId(int unitId, string sessionId, string factoryJobId)
        {
            var methodBegin = Utility.LogBegin();
            var userid = _configure.GetUserId(sessionId);
            if (string.IsNullOrEmpty(factoryJobId))
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constants.FACTORYJOBERRORMESSAGE,
                    Description = Constants.FACTORYJOBERRORDESCRIPTION,
                });
            }
            var result = _unitConfigurationdl.CreateUpdateFactoryJobId(unitId, userid, factoryJobId);
            if (result == 1)
            {
                Utility.LogEnd(methodBegin);
                return new ResponseMessage
                {
                    StatusCode = Constant.SUCCESS
                ,
                    Response = JObject.FromObject(JsonConvert.DeserializeObject(Constants.FACTORYJOBIDMESSAGE))
                };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constants.FACTORYJOBERRORMESSAGE1,
                    Description = Constants.FACTORYJOBERRORMESSAGE1
                });


            }
        }
    }
}
