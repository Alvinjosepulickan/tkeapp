    using Configit.Configurator.Server.Common;
    using IronXL;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
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
        public class ObomBL : IObom
        {
            #region Variables
            //private readonly ILogger _logger;
            private readonly IObomDL _obomdl;
            private readonly IConfigure _configure;
            string parentCode = null, locale = null,
                 modelNumber = null;

            private IConfiguration _configuration;
            /// <summary>
            /// object ICacheManager
            /// </summary>
            private readonly ICacheManager _cpqCacheManager;
            /// <summary>
            /// string
            /// </summary>
            private readonly string _environment;
            private IConfiguration _iconfig;
            private readonly IFieldDrawingAutomation _fieldDrawingAutomation;
            #endregion

            public ObomBL(IObomDL obomdl, IConfigure configure, IConfiguration iConfig, ICacheManager cpqCacheManager, IConfiguration configuration, IFieldDrawingAutomation fieldDrawingAutomation)
            {
                _obomdl = obomdl;
                _configure = configure;
                _configuration = iConfig;
                _cpqCacheManager = cpqCacheManager;
                _environment = Constant.DEV;
                _iconfig = configuration?.GetSection(Constant.PARAMSETTINGS);
                _fieldDrawingAutomation = fieldDrawingAutomation;
            }

            /// <summary>
            /// method used to create xml for obom
            /// </summary>
            /// <param name="configurationDetails"></param>
            /// <param name="sessionId"></param>7
            /// <returns></returns>
            public async Task<ResponseMessage> GETOBOMResponse(ConfigurationDetails configurationDetails, string sessionId)
            {
                var methodBeginTime = Utility.LogBegin();
                var response = new ResponseMessage()
                {
                    XmlDocument = new List<XDocument>()
                };

                var projectStatusMapper = JObject.Parse(File.ReadAllText(Constants.OBOMVARIABLEMAPPERPATH));
                var obomProjectStatusMapper = projectStatusMapper.ContainsKey(Constants.OBOM) ? projectStatusMapper[Constants.OBOM].ToObject<List<string>>() : new List<string>();
                var crossPackageVariableAssignments = await GetBuildingVariableAssignmentsForOBOM(configurationDetails, sessionId).ConfigureAwait(false);
                _cpqCacheManager.SetCache(sessionId, _environment, Constants.OBOMVARIABLEASSIGNMENTS + Convert.ToString(configurationDetails.GroupId), Utility.SerializeObjectValue(crossPackageVariableAssignments));
                if (obomProjectStatusMapper.Where(x => Utilities.CheckEquals(x, crossPackageVariableAssignments.QuoteStatus)).Any())
                {
                    var productSelectionMapperVariables = Utilities.VariableMapper(Constants.OBOMVARIABLEMAPPERPATH, Constants.VARIABLESKEY);
                    foreach (var building in crossPackageVariableAssignments.BuildingData)
                    {
                        foreach (var group in building.GroupData)
                        {
                            foreach (var set in group.SetData)
                            {
                                foreach (var setvariableAssignment in set.VariableAssignment)
                                {
                                    if (!set.SetConfigurationVariables.ContainsKey(setvariableAssignment.VariableId))
                                    {
                                        set.SetConfigurationVariables[setvariableAssignment.VariableId] = setvariableAssignment.Value;
                                    }
                                }
                                foreach (var groupVariable in group.GroupConfigurationVariables)
                                {
                                    if (groupVariable.Key.Contains("ELEVATOR0", StringComparison.OrdinalIgnoreCase))
                                    {
                                        foreach (var selectedUnit in set.SelectedUnits)
                                        {
                                            if (groupVariable.Key.Contains(selectedUnit, StringComparison.OrdinalIgnoreCase) && !set.SetConfigurationVariables.ContainsKey(groupVariable.Key))
                                            {
                                                set.SetConfigurationVariables[groupVariable.Key] = groupVariable.Value;
                                            }
                                        }
                                    }
                                    else if (!set.SetConfigurationVariables.ContainsKey(groupVariable.Key))
                                    {
                                        set.SetConfigurationVariables[groupVariable.Key] = groupVariable.Value;
                                    }
                                }

                                foreach (var setVariableAssignment in set.SystemValidationVariables)
                                {
                                    if (!set.SetConfigurationVariables.ContainsKey(setVariableAssignment.VariableId))
                                    {
                                        set.SetConfigurationVariables[setVariableAssignment.VariableId] = setVariableAssignment.Value;
                                    }
                                }

                            }
                            foreach (var unit in group.UnitDataForObom)
                            {
                                if (unit.SetId > 0)
                                {
                                    var configurationData = group.SetData.Where(x => x.SetId == unit.SetId).ToList();
                                    var variableAssignments = new List<VariableAssignment>();
                                    unit.Characteristics.Add(new Characteristics()
                                    {
                                        ClassName = "ORDER",
                                        CharacName = "FC_CUSTOMER",
                                        Value = "TK Elevator Corporation"
                                    });
                                    unit.Characteristics.Add(new Characteristics()
                                    {
                                        ClassName = "ORDER",
                                        CharacName = "QUOTEID",
                                        Value = crossPackageVariableAssignments.QuoteId
                                    });
                                    unit.Characteristics.Add(new Characteristics()
                                    {
                                        ClassName = "ORDER",
                                        CharacName = "PROJECTID",
                                        Value = crossPackageVariableAssignments.OpportunityId
                                    });
                                    unit.Characteristics.Add(new Characteristics()
                                    {
                                        ClassName = "ORDER",
                                        CharacName = "VERSIONID",
                                        Value = crossPackageVariableAssignments.VersionId
                                    });
                                    unit.Characteristics.Add(new Characteristics()
                                    {
                                        ClassName = "ORDER",
                                        CharacName = "ProjectStatus",
                                        Value = crossPackageVariableAssignments.ProjectStatus
                                    });
                                    unit.Characteristics.Add(new Characteristics()
                                    {
                                        ClassName = "ORDER",
                                        CharacName = "ProjectName",
                                        Value = crossPackageVariableAssignments.Name
                                    });
                                    unit.Characteristics.Add(new Characteristics()
                                    {
                                        ClassName = "ORDER",
                                        CharacName = "OrderStatus",
                                        Value = crossPackageVariableAssignments.QuoteStatus
                                    });
                                    unit.Characteristics.Add(new Characteristics()
                                    {
                                        ClassName = "ORDER",
                                        CharacName = "BRANCH",
                                        Value = crossPackageVariableAssignments.Branch
                                    });
                                    unit.Characteristics.Add(new Characteristics()
                                    {
                                        ClassName = "ORDER",
                                        CharacName = "CUSTOMERACCOUNT",
                                        Value = crossPackageVariableAssignments.AccountName
                                    });
                                    unit.Characteristics.Add(new Characteristics()
                                    {
                                        ClassName = "ORDER",
                                        CharacName = "CUSTOMERCONTACT",
                                        Value = crossPackageVariableAssignments.CustomerNumber
                                    });
                                    unit.Characteristics.Add(new Characteristics()
                                    {
                                        ClassName = "ORDER",
                                        CharacName = "SITEADDRESS",
                                        Value = crossPackageVariableAssignments.Address
                                    });

                                    if (configurationData.Any())
                                    {
                                        foreach (var configVariable in configurationData[0].SetConfigurationVariables)
                                        {
                                            variableAssignments.Add(new VariableAssignment() { VariableId = configVariable.Key, Value = configVariable.Value });
                                        }
                                        var unitDetails = new UnitDetailsForOBOM()
                                        {
                                            Unitid = unit.UnitId,
                                            UnitName = unit.UnitName,
                                            UEID = unit.UEID,
                                            Characteristics = unit.Characteristics,
                                            ElevatorLocation = unit.Location,
                                            SpecMemoVersion = unit.SpecMemoVersion,
                                            QuoteId = crossPackageVariableAssignments.QuoteId,
                                            GroupId = group.GroupId,
                                            OpportunityId = crossPackageVariableAssignments.OpportunityId,
                                            VersionId = crossPackageVariableAssignments.VersionId
                                        };
                                        var obomVariableAssignmentsList = variableAssignments.Where(x => !x.VariableId.Contains(Constants.FLOORMATRIX)).ToList();
                                        var variableAssignmentList = new List<VariableAssignment>();



                                        foreach (var variable in obomVariableAssignmentsList)
                                        {
                                            var variableVale = variable.VariableId.Split(Constants.DOT);
                                            var variableValue = variableVale[variableVale.Count() - 1];
                                            variableAssignmentList.Add(
                                                new VariableAssignment()
                                                {
                                                    VariableId = Constants.PARAMETERSVARIABLE + variableValue,
                                                    Value = variable.Value
                                                }
                                                );
                                        }
                                        obomVariableAssignmentsList = variableAssignments.Where(x => x.VariableId.Contains(Constants.FLOORMATRIX, StringComparison.OrdinalIgnoreCase)).ToList();
                                        foreach (var variable in obomVariableAssignmentsList)
                                        {
                                            var variableVale = variable.VariableId.Split(Constants.DOT);
                                            var variableValue = variableVale[variableVale.Count() - 1];
                                            var variableValueList = variable.VariableId.Split(Constants.DOT);
                                            foreach (var value in variableValueList)
                                            {
                                                if (value.Contains(Constants.LANDING))
                                                {
                                                    variableAssignmentList.Add(
                                                    new VariableAssignment()
                                                    {
                                                        VariableId = string.Format(productSelectionMapperVariables[Constants.LANDINGPARAMETER], value, variableValue),
                                                        Value = variable.Value
                                                    }
                                                    );
                                                }
                                            }

                                        }
                                        List<ObomVariables> obomvariableAssignments = new List<ObomVariables>();

                                        var obomVariableAssignments = new List<ObomVariables>();

                                        for (int i = building.NumberOfLanding + 1; i <= 200; i++)
                                        {
                                            var outOfIndex = Convert.ToString(i);
                                            variableAssignmentList.RemoveAll(unit => unit.VariableId.Contains(outOfIndex));
                                        }




                                        variableAssignmentList = variableAssignmentList.GroupBy(x => x.VariableId).Select(x => x.LastOrDefault()).ToList();
                                        //var missedVariables = Utility.DeserializeObjectValue<List<VariableAssignment>>(JArray.Parse(System.IO.File.ReadAllText(string.Format(Constants.OBOMTEMPORARYREQUESTBODYPATH, "RequestPayload"))).ToString());
                                        //variableAssignmentList.AddRange(missedVariables);
                                        obomVariableAssignments = new List<ObomVariables>();
                                        var parallelTaskList = new List<Task<ResponseMessage>>();
                                        var mapperData = JObject.Parse(File.ReadAllText(Constants.OBOMVARIABLEMAPPERPATH));

                                        var electricalsystemPackagePath = mapperData[Constants.ELECTRICALCALCULATION].ToObject<Dictionary<string, Dictionary<string, string>>>();
                                        variableAssignmentList.AddRange(mapperData.ContainsKey(Constants.ELECTRICALCALCULATIONHARDCODEDDVARIABLES) ? mapperData[Constants.ELECTRICALCALCULATIONHARDCODEDDVARIABLES].ToObject<List<VariableAssignment>>() : new List<VariableAssignment>());
                                        var electricalCalculationPackagePathMapper = electricalsystemPackagePath.ContainsKey(unit.ElevatorName) ? electricalsystemPackagePath[unit.ElevatorName] : new Dictionary<string, string>();

                                        var threeDParameterslist = Utility.DeserializeObjectValue<List<string>>(System.IO.File.ReadAllText(Constants.THREEDPARAMTERLIST));
                                        var threeDvariableList = new List<VariableAssignment>();
                                        foreach (var variable in variableAssignmentList)
                                        {
                                            var threeDParameterList = threeDParameterslist.Where(x => variable.VariableId.EndsWith(x, StringComparison.OrdinalIgnoreCase)).ToList();
                                            foreach (var threeDParameter in threeDParameterList)
                                            {
                                                foreach (var unitOpening in unit.OpeningLocation)
                                                {
                                                    if (unitOpening.Front || unitOpening.Rear)
                                                    {
                                                        var landing = Constants.LANDING + Convert.ToString(unitOpening.FloorNumber).PadLeft(3, Convert.ToChar(Constants.ZERO));
                                                        var threeDVariable = new VariableAssignment()
                                                        {
                                                            VariableId = string.Format(productSelectionMapperVariables[Constants.LANDINGPARAMETER], landing, threeDParameter),
                                                            Value = variable.Value
                                                        };
                                                        if (!variableAssignmentList.Where(x => x.VariableId.Contains(threeDVariable.VariableId, StringComparison.OrdinalIgnoreCase)).Any())
                                                        {
                                                            threeDvariableList.Add(threeDVariable);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        variableAssignmentList.AddRange(threeDvariableList);
                                        foreach (var packagePath in electricalCalculationPackagePathMapper)
                                        {
                                            var configurationRequest = CreateConfigurationRequestWithTemplate(packagePath.Key);
                                            configurationRequest.Line.VariableAssignments = variableAssignmentList;
                                            configurationRequest.Settings.IncludeSections = null;
                                            var configurationResponse = await _configure.GetByDefaultOrRulevaluesFromPackage(configurationRequest, sessionId).ConfigureAwait(false);
                                            var configurationResponseData = Utilities.DeserializeObjectValue<Dictionary<string, object>>(configurationResponse[Constant.CONFIGURATION].ToString());
                                            foreach (var configVariable in configurationResponseData)
                                            {
                                                if (!variableAssignmentList.Where(x => Utilities.CheckEquals(x.VariableId, configVariable.Key)).ToList().Any())
                                                {
                                                    variableAssignmentList.Add(new VariableAssignment() { VariableId = configVariable.Key, Value = configVariable.Value });
                                                }
                                            }
                                        }







                                        if (crossPackageVariableAssignments.IsQuoteReleased)
                                        {
                                            var obomPackagePathMapper = mapperData.ContainsKey(unit.ElevatorName) ? mapperData[unit.ElevatorName].ToObject<Dictionary<string, string>>() : new Dictionary<string, string>();
                                            foreach (var packagePath in obomPackagePathMapper)
                                            {
                                                parallelTaskList.Add(OBOMPAckageCall(packagePath.Value, new List<ObomVariables>(), variableAssignmentList
                                                , new Line(), sessionId, building.NumberOfLanding, true));
                                            }

                                            var obomResponseArray = await Task.WhenAll(parallelTaskList).ConfigureAwait(false);

                                            foreach (var obomResponse in obomResponseArray)
                                            {
                                                obomVariableAssignments.AddRange(Utility.DeserializeObjectValue<List<ObomVariables>>
                                                (Utility.SerializeObjectValue(obomResponse.ResponseArray)));
                                            }
                                        }
                                        response.XmlDocument.Add(GenerateXmlForOBOM(obomVariableAssignments, unitDetails, variableAssignmentList, building.NumberOfLanding, sessionId, crossPackageVariableAssignments.QuoteStatus));

                                    }
                                }
                            }
                        }



                    }
                    response.QuoteId = crossPackageVariableAssignments.QuoteId;
                }
                Utility.LogEnd(methodBeginTime);
                return response;
            }


            public async Task<ObomVariableAssignment> GetBuildingVariableAssignmentsForOBOM(ConfigurationDetails configurationDetails, string sessionId)
            {
                var methodBeginTime = Utility.LogBegin();
                var databaseResponse = _obomdl.GetvariableAssignmentsForObom(configurationDetails.GroupId);
                var crossPackageVariableAssignments = Utilities.GenerateObomVariablesAssignments(databaseResponse);
                if (!crossPackageVariableAssignments.OpportunityId.Contains(Constants.SC, StringComparison.OrdinalIgnoreCase))
                {
                    var viewData = _cpqCacheManager.GetCache(crossPackageVariableAssignments.OpportunityId + crossPackageVariableAssignments.VersionId, _environment, Constant.VIEWDATA);
                    if (viewData != null)
                    {
                        var jObjectDictionry = (JObject.Parse(File.ReadAllText(Constant.VIEWVARIABLEMAPPING)));
                        var viewVariables = jObjectDictionry[Constant.VIEWVARIABLES];
                        var variablesDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(viewVariables));
                        // Contact Variables Data
                        var contactVariables = jObjectDictionry[Constant.CONTACTVRBLES];
                        var variablesDictionary_Contact = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(contactVariables));
                        // gCVariables data
                        var gCVariables = jObjectDictionry[Constant.GCVARIABLES];
                        // ownerVariables data
                        var ownerVariables = jObjectDictionry[Constant.OWNERVARIABLES];
                        // architectVariables data
                        var architectVariables = jObjectDictionry[Constant.ARCHITECHVRBLES];
                        // billingVariables data
                        var billingVariables = jObjectDictionry[Constant.BILLINGVRBLES];
                        // BuildingVariables data
                        var BuildingVariables = jObjectDictionry[Constant.BUILDINGVRBLES];
                        var variablesDictionary_Building = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(BuildingVariables));
                        // quoteVariables data
                        var quoteVariables = jObjectDictionry[Constant.QUOTEVRBLES];
                        // building Info Variables data
                        var buildingInfoVariables = jObjectDictionry[Constant.BUILDINGINFOVRBLES];
                        // Getting Units List Data 
                        var viewDataJObject = Utility.DeserializeObjectValue<JObject>(viewData);
                        string address = (string)viewDataJObject.SelectToken(variablesDictionary_Building[Constant.ADDRESSLINE1])
                            + System.Environment.NewLine
                            + (string)viewDataJObject.SelectToken(variablesDictionary_Building[Constant.ADDRESSLINE2])
                            + System.Environment.NewLine
                            + (string)viewDataJObject.SelectToken(variablesDictionary_Building[Constant.CITY])
                            + System.Environment.NewLine
                            + (string)viewDataJObject.SelectToken(variablesDictionary_Building[Constant.STATE])
                            + System.Environment.NewLine
                            + (string)viewDataJObject.SelectToken(variablesDictionary_Building[Constant.COUNTRY])
                            + System.Environment.NewLine
                            + (string)viewDataJObject.SelectToken(variablesDictionary_Building[Constant.ZIPCODE]);
                        crossPackageVariableAssignments.Name = (string)viewDataJObject.SelectToken(variablesDictionary_Building[Constant.ACCOUNTNME]);
                        crossPackageVariableAssignments.AccountName = (string)viewDataJObject.SelectToken(variablesDictionary_Building[Constant.ACCOUNTNME]);
                        crossPackageVariableAssignments.CustomerNumber = (string)viewDataJObject.SelectToken(variablesDictionary_Contact[Constant.MOBILEPHONE]);
                        crossPackageVariableAssignments.Address = address;
                        crossPackageVariableAssignments.Branch = (string)viewDataJObject.SelectToken(variablesDictionary[Constant.BRANCH]);
                        crossPackageVariableAssignments.Country = (string)viewDataJObject.SelectToken(variablesDictionary_Building[Constant.COUNTRY]);
                        if (!string.IsNullOrEmpty(crossPackageVariableAssignments.Country) && crossPackageVariableAssignments.Country.Contains(Constants.US, StringComparison.OrdinalIgnoreCase))
                        {
                            crossPackageVariableAssignments.Country = Constants.US;
                        }
                    }
                }
                var productSelectionMapperVariables = Utilities.VariableMapper(Constants.OBOMVARIABLEMAPPERPATH, Constants.VARIABLESKEY);
                foreach (var building in crossPackageVariableAssignments.BuildingData)
                {
                    building.ConfigurationVariables.Add
                        (new VariableAssignment()
                        {
                            VariableId = productSelectionMapperVariables[Constants.BUILDINCITY],
                            Value = crossPackageVariableAssignments.Country
                        });
                    var buildingVaribaleAssignmentValues = building.ConfigurationVariables;
                    var lineVariable = new Line();
                    lineVariable.VariableAssignments = buildingVaribaleAssignmentValues;
                    var configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineVariable), Constant.BUILDINGNAME);
                    configRequest.Settings.IncludeSections = new List<string>();
                    var configResponse = await _configure.GetByDefaultOrRulevaluesFromPackage(configRequest, sessionId).ConfigureAwait(false);
                    building.BuildingConfigurationVariables = Utilities.DeserializeObjectValue<Dictionary<string, object>>(configResponse[Constant.CONFIGURATION].ToString());
                    foreach (var buildingvariableAssignment in buildingVaribaleAssignmentValues)
                    {
                        if (!building.BuildingConfigurationVariables.ContainsKey(buildingvariableAssignment.VariableId))
                        {
                            building.BuildingConfigurationVariables.Add(buildingvariableAssignment.VariableId, buildingvariableAssignment.Value);
                        }
                    }
                    building.GroupData = await GetGroupVariableAssignmentsForObom(building.GroupData, building, sessionId).ConfigureAwait(false);
                }
                Utility.LogEnd(methodBeginTime);
                return crossPackageVariableAssignments;
            }

            public async Task<List<GroupData>> GetGroupVariableAssignmentsForObom(List<GroupData> groupData, BuildingData buildingData, string sessionId)
            {
                var methodBeginTime = Utility.LogBegin();
                var productSelectionMapperVariables = Utilities.VariableMapper(Constants.OBOMVARIABLEMAPPERPATH, Constants.VARIABLESKEY);
                foreach (var group in groupData)
                {
                    var buildingVariableAssignments = new List<ConfigVariable>();
                    foreach (var buildingVariable in buildingData.BuildingConfigurationVariables)
                    {
                        buildingVariableAssignments.Add(new ConfigVariable() { VariableId = buildingVariable.Key, Value = buildingVariable.Value });
                    }
                    var groupCrossPackageVariables = _configure.GeneratevariableAssignmentsForCrosspackageDependecy(Constant.GROUPLAYOUTCONFIGURATION, buildingVariableAssignments);
                    var groupVaribaleAssignmentValues = group.VariableAssignment;
                    foreach (var buildingVariable in groupCrossPackageVariables)
                    {
                        groupVaribaleAssignmentValues.Add(new VariableAssignment() { VariableId = buildingVariable.VariableId, Value = buildingVariable.Value });
                    }
                    var lineVariable = new Line()
                    {
                        VariableAssignments = groupVaribaleAssignmentValues
                    };
                    var configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineVariable), Constant.GROUPCONFIGURATIONNAME);
                    configRequest.Settings.IncludeSections = new List<string>();
                    var configResponse = await _configure.GetByDefaultOrRulevaluesFromPackage(configRequest, sessionId).ConfigureAwait(false);
                    var configVariableDictionary = Utilities.DeserializeObjectValue<Dictionary<string, object>>(configResponse[Constant.CONFIGURATION].ToString());
                    configVariableDictionary.Remove("Parameters_SP.frontDoorTypeAndHand_SP");
                    configVariableDictionary.Remove("Parameters_SP.rearDoorTypeAndHand_SP");
                    foreach (var variable in configVariableDictionary.ToList())
                    {
                        group.GroupConfigurationVariables[variable.Key] = variable.Value;
                    }
                    if (group.GroupConfigurationVariables == null)
                    {
                        group.GroupConfigurationVariables = new Dictionary<string, object>();
                    }
                    foreach (var variableId in groupVaribaleAssignmentValues)
                    {
                        if (string.IsNullOrEmpty(variableId.VariableId) && (!group.GroupConfigurationVariables.ContainsKey(variableId.VariableId)))
                        {
                            group.GroupConfigurationVariables[variableId.VariableId] = variableId.Value;
                        }
                    }

                    foreach (var variableId in group.FDAVariableAssignments)
                    {
                        if (string.IsNullOrEmpty(variableId.VariableId) && (!group.GroupConfigurationVariables.ContainsKey(variableId.VariableId)))
                        {
                            group.GroupConfigurationVariables[variableId.VariableId] = variableId.Value;
                        }
                    }
                    foreach (var buildingVariable in buildingData.BuildingConfigurationVariables)
                    {
                        if (!string.IsNullOrEmpty(buildingVariable.Key) && (!group.GroupConfigurationVariables.ContainsKey(buildingVariable.Key)))
                        {
                            group.GroupConfigurationVariables[buildingVariable.Key] = buildingVariable.Value;
                        }
                    }
                    group.GroupConfigurationVariables.Remove(productSelectionMapperVariables[Constants.FRONTDOORTYPEMAP]);
                    group.GroupConfigurationVariables.Remove(productSelectionMapperVariables[Constants.REARDOORTYPEMAP]);
                    group.SetData = await GetSetVariableAssignmentsForObom(group.SetData, group, sessionId).ConfigureAwait(false);
                }
                Utility.LogEnd(methodBeginTime);
                return groupData;
            }


            public async Task<List<SetData>> GetSetVariableAssignmentsForObom(List<SetData> setData, GroupData groupData, string sessionId)
            {
                var methodBeginTime = Utility.LogBegin();
                var configRequest = new ConfigurationRequest();
                foreach (var set in setData)
                {
                    var groupVariableAssignments = new List<ConfigVariable>();
                    foreach (var groupVariable in groupData.GroupConfigurationVariables)
                    {
                        if (!groupVariable.Key.Contains("ELEVATOR", StringComparison.OrdinalIgnoreCase))
                        {
                            groupVariableAssignments.Add(new ConfigVariable() { VariableId = groupVariable.Key, Value = groupVariable.Value });
                        }
                        else if (set.SelectedUnits.Any())
                        {
                            foreach (var selectedUnit in set.SelectedUnits)
                            {
                                if (!groupVariable.Key.Contains(selectedUnit, StringComparison.OrdinalIgnoreCase))
                                {
                                    groupVariableAssignments.Add(new ConfigVariable() { VariableId = groupVariable.Key.Replace(selectedUnit, "ELEVATOR"), Value = groupVariable.Value });
                                }
                            }
                        }
                    }
                    var groupCrossPackageVariable = _configure.GeneratevariableAssignmentsForCrosspackageDependecy("Unit", groupVariableAssignments);
                    foreach (var groupVariable in groupCrossPackageVariable)
                    {
                        set.VariableAssignment.Add(new VariableAssignment() { VariableId = groupVariable.VariableId, Value = groupVariable.Value });
                    }
                    var lineVariable = new Line()
                    {
                        VariableAssignments = set.VariableAssignment
                    };
                    if (groupData.IsNcp)
                    {
                        var productCategory = groupData.VariableAssignment.Where(x => Utility.CheckEquals(x.VariableId, "productCategory")).ToList();
                        if (productCategory.Any())
                        {
                            configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineVariable), Convert.ToString(productCategory[0].Value), new List<VariableAssignment>(), set.ProductSelected);
                        }
                    }
                    else
                    {
                        configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineVariable), Constant.UNITCONFIG, new List<VariableAssignment>(), set.ProductSelected);
                    }
                    configRequest.Settings.IncludeSections = new List<string>();
                    var configResponse = await _configure.GetByDefaultOrRulevaluesFromPackage(configRequest, sessionId).ConfigureAwait(false);

                    var setConfigVariableDictionary = Utilities.DeserializeObjectValue<Dictionary<string, object>>(configResponse[Constant.CONFIGURATION].ToString());

                    foreach (var setVariableAssignment in setConfigVariableDictionary)
                    {
                        if (!set.SetConfigurationVariables.ContainsKey(setVariableAssignment.Key))
                        {
                            set.SetConfigurationVariables.Add(setVariableAssignment.Key, setVariableAssignment.Value);
                        }
                    }
                    foreach (var setvariableAssignment in set.VariableAssignment)
                    {
                        if (!set.SetConfigurationVariables.ContainsKey(setvariableAssignment.VariableId))
                        {
                            set.SetConfigurationVariables[setvariableAssignment.VariableId] = setvariableAssignment.Value;
                        }
                    }
                    foreach (var groupVariable in groupData.GroupConfigurationVariables)
                    {
                        if (groupVariable.Key.Contains("ELEVATOR0", StringComparison.OrdinalIgnoreCase))
                        {
                            foreach (var selectedUnit in set.SelectedUnits)
                            {
                                if (groupVariable.Key.Contains(selectedUnit, StringComparison.OrdinalIgnoreCase) && !set.SetConfigurationVariables.ContainsKey(groupVariable.Key))
                                {
                                    set.SetConfigurationVariables[groupVariable.Key] = groupVariable.Value;
                                }
                            }
                        }
                        else if (!set.SetConfigurationVariables.ContainsKey(groupVariable.Key))
                        {
                            set.SetConfigurationVariables[groupVariable.Key] = groupVariable.Value;
                        }
                    }

                    foreach (var setVariableAssignment in set.SystemValidationVariables)
                    {
                        if (!set.SetConfigurationVariables.Where(x => x.Key.Contains(setVariableAssignment.VariableId, StringComparison.OrdinalIgnoreCase)).ToList().Any())
                        {
                            set.SetConfigurationVariables[setVariableAssignment.VariableId] = setVariableAssignment.Value;
                        }
                    }

                }
                Utility.LogEnd(methodBeginTime);
                return setData;
            }
            /// <summary>
            /// Generate request body for  obom packages and fetch the response
            /// </summary>
            /// <param name="packagePath"></param>
            /// <param name="obomVariableAssignments"></param>
            /// <param name="variableAssignments"></param>
            /// <param name="lineVariable"></param>
            /// <param name="sessionId"></param>
            /// <param name="numberOfFloors"></param>
            /// <returns></returns>
            public async Task<ResponseMessage> OBOMPAckageCall(string packagePath, List<ObomVariables> obomVariableAssignments, List<VariableAssignment> variableAssignments, Line lineVariable, string sessionId, int numberOfFloors, bool isObom)
            {
                var methodBeginTime = Utility.LogBegin();
                var configRequest = CreateConfigurationRequestWithTemplate(packagePath);
                var variables = variableAssignments;
                //includeSection.Add(Constants.FLOORMATRIX);
                //try
                //{
                //    var p = Utility.DeserializeObjectValue<List<ObomVariableAssignmentsTemp>>(JArray.Parse(System.IO.File.ReadAllText(string.Format(Constants.OBOMTEMPORARYREQUESTBODYPATH, packagePath))).ToString());
                //    variables = new List<VariableAssignment>();
                //    foreach (var item in p[0].GlobalParameters.ToList())
                //    {
                //        variables.Add(new VariableAssignment()
                //        {
                //            VariableId = "Parameters." + item.Key,
                //            Value = item.Value.Value
                //        });
                //    }
                //    int number = 0;
                //    foreach (var item in p[0].FloorMatrixParameters)
                //    {
                //        foreach (var subItemtem in item)
                //        {
                //            number += 1;
                //            foreach (var subItem in subItemtem.ToList())
                //            {
                //                variables.Add(new VariableAssignment()
                //                {
                //                    VariableId = string.Format("FloorMatrix.LANDING{0}.Parameters.", "00" + Convert.ToString(number)) + subItem.Key,
                //                    Value = subItem.Value
                //                });
                //            }
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{

                //}
                configRequest.Line.VariableAssignments = variables;
                configRequest.Settings.IncludeSections = null;
                var obomonfigCall = await _configure.OBOMConfigureBl(configRequest, sessionId, configRequest.PackagePath).ConfigureAwait(false);
                // add 
                var configurationResponse = Utilities.DeserializeObjectValue<OBOMResponse>(Utility.SerializeObjectValue(obomonfigCall.Response));
                if (isObom)
                {
                    Utility.LogInfo(Utility.SerializeObjectValue(configurationResponse));
                }
                //filter the floorMatrix Section
                if (configurationResponse == null || configurationResponse.Sections == null)
                {
                    return new ResponseMessage() { ResponseArray = JArray.FromObject(new List<ObomVariables>()) };
                }
                var floorMatrixResponse = configurationResponse.Sections.Where(unit => unit.Id.Equals(Constants.FLOORMATRIX)).ToList();
                //var floorMatrixResponse = new List<SectionsValues>();
                //Remove Parameters and floormatrix section from the response
                configurationResponse.Sections = configurationResponse.Sections.Except(configurationResponse.Sections.Where(unit => unit.Id.Contains(Constant.PARAMETERSVALUES))).ToList();
                configurationResponse.Sections = configurationResponse.Sections.Except(configurationResponse.Sections.Where(unit => unit.Id.Contains(Constants.FLOORMATRIX))).ToList();

                foreach (var section in configurationResponse.Sections)
                {
                    var bomParentList = FetchVariableAssignmentsForXMLGeneration(Utility.DeserializeObjectValue<List<SectionsValues>>(Utility.SerializeObjectValue(section.sections)), new List<ObomVariables>(), 0);
                    foreach (var bomParent in bomParentList)
                    {
                        if (isObom)
                        {
                            if (bomParent?.XMLVariables != null && !bomParent.XMLVariables.ContainsKey(Constants.FOREACH))
                            {
                                obomVariableAssignments.Add(bomParent);
                            }
                        }
                        else if (bomParent?.XMLVariables != null)
                        {
                            obomVariableAssignments.Add(bomParent);
                        }

                    }
                }
                //call package for each per landing variable
                if (isObom && floorMatrixResponse.Any())
                {
                    var parallelTaskList = new List<Task<ResponseMessage>>();
                    int floorNumber = 0;
                    foreach (var floor in floorMatrixResponse)
                    {
                        for (int floorNumberValue = 0; floorNumberValue < numberOfFloors; floorNumberValue++)
                        {
                            if (floorNumberValue < floor.sections.Count())
                            {
                                floorNumber++;
                                var section = PerLandingVariables(floor.sections[floorNumberValue], new List<VariableAssignment>());
                                var variableList = new List<VariableAssignment>();
                                variableList.AddRange(variables);
                                variableList.AddRange(section);
                                var lineObject = new Line()
                                {
                                    VariableAssignments = variableList
                                };
                                var perLandingConfigurationRequest = CreateConfigurationRequestWithTemplate(packagePath);
                                perLandingConfigurationRequest.Line.VariableAssignments = variableList;
                                perLandingConfigurationRequest.Settings.IncludeSections = null;
                                parallelTaskList.Add(_configure.OBOMConfigureBl(perLandingConfigurationRequest, sessionId, perLandingConfigurationRequest.PackagePath));


                            }
                        }

                    }
                    var obomResponseArray = await Task.WhenAll(parallelTaskList).ConfigureAwait(false);
                    floorNumber = 0;
                    foreach (var response in obomResponseArray)
                    {
                        floorNumber = floorNumber + 1;
                        var perlnadingConfigurationResponse = Utility.DeserializeObjectValue<OBOMResponse>(Utility.SerializeObjectValue(response.Response));
                        perlnadingConfigurationResponse.Sections = perlnadingConfigurationResponse.Sections.Except(perlnadingConfigurationResponse.Sections.Where(unit => unit.Id.Contains(Constant.PARAMETERSVALUES))).ToList();
                        perlnadingConfigurationResponse.Sections = perlnadingConfigurationResponse.Sections.Except(perlnadingConfigurationResponse.Sections.Where(unit => unit.Id.Contains(Constants.FLOORMATRIX))).ToList();
                        if (isObom)
                        {
                            Utility.LogInfo(Utility.SerializeObjectValue(perlnadingConfigurationResponse));
                        }
                        foreach (var landingSection in perlnadingConfigurationResponse.Sections)
                        {
                            var bomParentList = FetchVariableAssignmentsForXMLGeneration(Utility.DeserializeObjectValue<List<SectionsValues>>(Utility.SerializeObjectValue(landingSection.sections)), new List<ObomVariables>(), floorNumber);
                            foreach (var bomParent in bomParentList)
                            {
                                if (bomParent.XMLVariables != null && bomParent.XMLVariables.ContainsKey(Constants.FOREACH))
                                {
                                    obomVariableAssignments.Add(bomParent);
                                }
                            }
                        }
                    }
                }
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage() { ResponseArray = JArray.FromObject(obomVariableAssignments), Message = packagePath };
            }
            /// <summary>
            /// generate variable assignments for obom package call
            /// </summary>
            /// <param name="databaseResponse"></param>
            /// <returns></returns>
            public List<VariableAssignment> GenearteVariableAssignmentsFromDataset(Dictionary<string, object> configVariables)
            {
                var methodBeginTime = Utility.LogBegin();
                var variableAssignments = new List<VariableAssignment>();
                var variableList = configVariables.ToList();
                foreach (KeyValuePair<string, object> kvp in configVariables)
                {
                    variableAssignments.Add(new VariableAssignment { VariableId = kvp.Key, Value = kvp.Value });
                }
                Utility.LogEnd(methodBeginTime);
                return variableAssignments;
            }


            /// <summary>
            /// method used to filter the vt package response for obom xml generation
            /// </summary>
            /// <param name="configurationResponse"></param>
            /// <param name="obomvariableAssignments"></param>
            /// <param name="floorNumber"></param>
            /// <returns></returns>
            public List<ObomVariables> FetchVariableAssignmentsForXMLGeneration(List<SectionsValues> configurationResponse, List<ObomVariables> obomvariableAssignments, int floorNumber)
            {
                var methodBeginTime = Utility.LogBegin();

                if (configurationResponse != null)
                {
                    foreach (var section in configurationResponse)
                    {
                        var obomVariables = new ObomVariables();
                        if (section != null && section.Variables.Any())
                        {
                            obomVariables.Characteristics = new Dictionary<string, Object>();
                            obomVariables.XMLVariables = new Dictionary<string, Object>();
                            obomVariables.Child = new List<ObomVariables>();
                            if (floorNumber > 0)
                            {
                                obomVariables.Characteristics[Constants.LANDING] = Convert.ToString(floorNumber).PadLeft(3, Convert.ToChar(Constants.ZERO));
                            }
                            foreach (var variable in section.Variables)
                            {
                                if (variable != null && variable.Values != null && variable.Id != null)
                                {
                                    obomVariables.VariableId = section.Id;
                                    foreach (var variableValue in variable.Values)
                                    {
                                        if (variableValue != null && !(string.IsNullOrEmpty(variableValue.Assigned)) && variableValue.value != null)
                                        {
                                            if (variable.Id.EndsWith(Constants.PARTNUM))
                                            {
                                                foreach (var property in variable.Values)
                                                {
                                                    if (!string.IsNullOrEmpty(property.Assigned))
                                                    {
                                                        obomVariables.PartNumber = Convert.ToString(property.value);
                                                    }
                                                }
                                            }
                                            else if (variable.Id.EndsWith(Constants.QTY))
                                            {
                                                obomVariables.Quantity = Convert.ToInt32(variableValue.value);
                                            }
                                            else if (variable.Id.EndsWith(Constants.DESC))
                                            {
                                                foreach (var property in variable.Values)
                                                {
                                                    if (!string.IsNullOrEmpty(property.Assigned))
                                                    {
                                                        obomVariables.Description = Convert.ToString(property.value);
                                                        object characteresticValue;
                                                        try
                                                        {
                                                            characteresticValue = Math.Round(Convert.ToDecimal(property.value), 3);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            characteresticValue = property.value;
                                                        }
                                                        obomVariables.Characteristics["ItemTextLine1"] = characteresticValue;
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                var variableList = variable.Id.Split(Constants.DOT);
                                                foreach (var property in variable.Values)
                                                {
                                                    if (!string.IsNullOrEmpty(property.Assigned))
                                                    {
                                                        object characteresticValue;
                                                        try
                                                        {
                                                            characteresticValue = Math.Round(Convert.ToDecimal(property.value), 3);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            characteresticValue = property.value;
                                                        }
                                                        obomVariables.Characteristics[variableList[variableList.Count() - 1]] = characteresticValue;
                                                    }
                                                }

                                            }
                                        }
                                    }


                                }

                            }
                            foreach (var property in section.Properties)
                            {
                                obomVariables.XMLVariables[property.Id] = property.Value;
                            }
                        }
                        if (section != null && section.sections != null && section.sections.Any())
                        {
                            obomVariables.Child = new List<ObomVariables>();
                            var obomvariableAssignments1 = new List<ObomVariables>();
                            obomVariables.Child.AddRange(FetchVariableAssignmentsForXMLGeneration(Utility.DeserializeObjectValue<List<SectionsValues>>(Utility.SerializeObjectValue(section.sections)), obomvariableAssignments1, floorNumber));
                            //obomVariables.Child = obomVariables.Child.GroupBy(x => new { x.PartNumber }).Select(y => y.First()).ToList(); ;
                        }

                        obomvariableAssignments.Add(obomVariables);
                    }
                }
                Utility.LogEnd(methodBeginTime);
                return obomvariableAssignments;
            }
            /// <summary>
            /// method used to generate xml for obom
            /// </summary>
            /// <param name="obomVariableAssignments"></param>
            /// <param name="UnitDetails"></param>
            /// <param name="variableAssignments"></param>
            /// <param name="numberOfFloors"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public XDocument GenerateXmlForOBOM(List<ObomVariables> obomVariableAssignments, UnitDetailsForOBOM UnitDetails, List<VariableAssignment> variableAssignments, int numberOfFloors, string sessionId, string quoteStatus)
            {
                var methodBeginTime = Utility.LogBegin();
                XDocument xDoc = new XDocument();
                _configuration = new ConfigurationBuilder().SetBasePath(System.IO.Directory.GetCurrentDirectory()).AddJsonFile(Constant.APPSETTINGS).Build();
                string sourceEnv = Convert.ToString(_configuration.GetSection(Constant.PARAMSETTINGS).GetSection(Constants.SOURCEENV).Value);
                string targetEnv = Convert.ToString(_configuration.GetSection(Constant.PARAMSETTINGS).GetSection(Constants.TARGETENV).Value);
                string sendingTypeValue = Convert.ToString(_configuration.GetSection(Constant.PARAMSETTINGS).GetSection(Constants.SENDINGTYPE).Value);
                string plantValue = Convert.ToString(_configuration.GetSection(Constant.PARAMSETTINGS).GetSection(Constants.PLANT).Value);

                var userName = _configure.GetUserId(sessionId);

                XElement documentName = new XElement("SpecMemoAndOBOM_CreateAndUpdate_Request");

                XElement messageHeader = new XElement("MessageHeader");
                XElement uuid = new XElement("UUID", Guid.NewGuid().ToString());
                var createdDate = DateTime.Now.ToString("yyyy-MM-dd") + "T";
                XElement creationDateTime = new XElement("CreationDateTime", createdDate + DateTime.Now.ToString("HH:mm:ss.mmmmmmm"));
                XElement senderBusinessSystemID = new XElement("SenderBusinessSystemID", sourceEnv);
                XElement recipientBusinessSystemID = new XElement("RecipientBusinessSystemID", targetEnv);
                XElement trackingID = new XElement("TrackingID", "54321");


                XElement elevator = new XElement("Elevator");
                XElement uniqueEquipmentID = new XElement("UniqueEquipmentID", UnitDetails.UEID);
                XElement changeNumber = new XElement("ChangeNumber", "MyProjectId");
                XElement sourceSystem = new XElement("SourceSystem", "PC");
                XElement mfgtProjectID = new XElement("MfgtProjectID", UnitDetails.OpportunityId);

                XElement mfgtWBSNO = new XElement("MfgtWBSNO", "MyWbsNo");
                XElement plant = new XElement("Plant", plantValue);
                XElement sendUser = new XElement("SenderUser", userName);
                XElement sendingType = new XElement("SendingType", sendingTypeValue);
                XElement specMemo = GenerateSpecmemo(variableAssignments, numberOfFloors, UnitDetails);
                //XElement specMemoVersion = new XElement("SpecMemoVersion", UnitDetails.SpecMemoVersion);
                XElement characteristics = new XElement("Characteristics");
                XElement className = new XElement("ClassName", "XXXX");
                XElement characName = new XElement("CharacName", "XXXX");
                XElement value = new XElement("Value", 1);

                characteristics.Add(className);
                characteristics.Add(characName);
                characteristics.Add(value);


                elevator.Add(uniqueEquipmentID);
                elevator.Add(changeNumber);
                elevator.Add(sourceSystem);
                elevator.Add(mfgtProjectID);
                elevator.Add(mfgtWBSNO);
                elevator.Add(plant);
                elevator.Add(sendUser);
                elevator.Add(sendingType);
                elevator.Add(specMemo);
                foreach (var obomVaiable in obomVariableAssignments)
                {
                    if (obomVaiable.Quantity > 0)
                    {
                        XElement item = new XElement("Item");
                        XElement componentItem = new XElement("ComponentItem", obomVaiable.XMLVariables != null && obomVaiable.XMLVariables.ContainsKey("item_num") ? obomVaiable.XMLVariables["item_num"] : 0);
                        XElement componentMaterialNo = new XElement("ComponentMaterialNo", obomVaiable.PartNumber);
                        XElement desiredQuantity = new XElement("DesiredQuantity", obomVaiable.Quantity);
                        XElement uom = new XElement("UOM", obomVaiable.XMLVariables != null && obomVaiable.XMLVariables.ContainsKey("qty_units") ? obomVaiable.XMLVariables["qty_units"] : string.Empty);
                        XElement ctoorETO = new XElement("CTOorETO", "CTO");
                        XElement modelVersion = new XElement("ModelVersion", obomVaiable.XMLVariables != null && obomVaiable.XMLVariables.ContainsKey("revision") ? obomVaiable.XMLVariables["revision"] : obomVaiable.XMLVariables.ContainsKey("eco_rev") ? obomVaiable.XMLVariables["eco_rev"] : string.Empty);
                        XElement revision = new XElement("Revision", obomVaiable.XMLVariables.ContainsKey("revision") ? obomVaiable.XMLVariables["revision"] : obomVaiable.XMLVariables.ContainsKey("mco_rev") ? obomVaiable.XMLVariables["mco_rev"] : obomVaiable.XMLVariables.ContainsKey("eco_rev") ? obomVaiable.XMLVariables["eco_rev"] : string.Empty);

                        //XElement characteristic = new XElement("Characteristic");
                        //XElement ClassName = new XElement("DesiredQuantity", variable.Characteristics);
                        //XElement CharacNames = new XElement("CharacName", variable.Characteristics);
                        //XElement values = new XElement("Value", variable.Characteristics);
                        //XElement bomParent = new XElement("BomParent");
                        int numberOfBomChildren = 0;
                        var bomChildList = new List<XElement>();
                        if (obomVaiable.Child != null && obomVaiable.Child.Any())
                        {
                            bomChildList = GenerateBOMChild(obomVaiable.Child, obomVaiable.PartNumber);
                        }
                        item.Add(componentItem);
                        item.Add(componentMaterialNo);
                        item.Add(desiredQuantity);
                        item.Add(uom);
                        item.Add(ctoorETO);
                        item.Add(modelVersion);
                        item.Add(revision);
                        if (obomVaiable.Characteristics != null)
                        {
                            foreach (var charac in obomVaiable.Characteristics)
                            {
                                if (!Utility.CheckEquals(charac.Key, "bomgroup") && !Utility.CheckEquals(charac.Key, "showas") && !string.IsNullOrEmpty(Convert.ToString(charac.Value)) && !Utility.CheckEquals(charac.Key, "name") && !Utility.CheckEquals(charac.Key, "fullyqualifiedname") && !Utility.CheckEquals(charac.Key, "foreach") && !Utility.CheckEquals(charac.Key, "itemnum") && !Utility.CheckEquals(charac.Key, "show"))
                                {
                                    XElement characteristic = new XElement("Characteristic");
                                    XElement classNames = (Convert.ToString(charac.Key).ToUpper()).Contains("LANDING") ? new XElement("ClassName", Convert.ToString(charac.Key).ToUpper()) : new XElement("ClassName", "XXXX");
                                    XElement characNames = new XElement("CharacName", Convert.ToString(charac.Key).ToUpper().Contains("LANDING") ? Convert.ToString(charac.Key).ToUpper() : charac.Key);
                                    XElement values = new XElement("Value", charac.Value);
                                    characteristic.Add(classNames);
                                    characteristic.Add(characNames);
                                    characteristic.Add(values);
                                    item.Add(characteristic);
                                }
                            }
                        }
                        if (obomVaiable.XMLVariables != null)
                        {
                            foreach (var charac in obomVaiable.XMLVariables)
                            {
                                if (Utility.CheckEquals(charac.Key, "legacy_sec"))
                                {
                                    XElement characteristic = new XElement("Characteristic");
                                    XElement classNames = new XElement("ClassName", "XXXX");
                                    XElement characNames = new XElement("CharacName", charac.Key.ToUpper());
                                    XElement values = new XElement("Value", charac.Value);
                                    characteristic.Add(classNames);
                                    characteristic.Add(characNames);
                                    characteristic.Add(values);
                                    item.Add(characteristic);
                                }
                            }
                        }
                        if (bomChildList != null)
                        {
                            foreach (var child in bomChildList)
                            {
                                if (!Utilities.CheckEquals(Utility.SerializeObjectValue(item),
                                    Utility.SerializeObjectValue(child)) && !(Utility.SerializeObjectValue(item).Contains(Utility.SerializeObjectValue(child))))
                                {
                                    item.Add(child);
                                }
                            }
                        }
                        elevator.Add(item);
                    }
                }


                //specMemo= GenerateSpecmemo(variableAssignments,numberOfFloors);
                //specMemo.Add(characteristics);
                messageHeader.Add(uuid);
                messageHeader.Add(creationDateTime);
                messageHeader.Add(senderBusinessSystemID);
                messageHeader.Add(recipientBusinessSystemID);
                messageHeader.Add(trackingID);



                documentName.Add(messageHeader);
                documentName.Add(elevator);
                xDoc.Add(documentName);
                bool exists = System.IO.Directory.Exists(quoteStatus);

                if (!exists)
                    System.IO.Directory.CreateDirectory(quoteStatus);
                xDoc.Save(quoteStatus + "\\" + Convert.ToString(UnitDetails.UEID) + ".xml");

                Utility.LogEnd(methodBeginTime);
                //File.WriteAllText("foo.xml", Utility.SerializeObjectValue(xDoc));
                return xDoc;
            }


            /// <summary>
            /// Method used to filter per landing variables from vt package response
            /// </summary>
            /// <param name="floor"></param>
            /// <param name="variableList"></param>
            /// <returns></returns>
            public List<VariableAssignment> PerLandingVariables(SectionsGroupValues floor, List<VariableAssignment> variableList)
            {
                var methodBeginTime = Utility.LogBegin();
                if (floor != null && floor.Properties != null && floor.Variables != null)
                {
                    foreach (var variable in floor.Variables)
                    {
                        foreach (var property in variable.Properties)
                        {
                            if (Utility.CheckEquals(property.Id, Constants.FOREACHVALUE))
                            {

                                foreach (var value in variable.Values)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(value.Assigned)))
                                    {
                                        var landingVariable = Convert.ToString(property.Value).Replace(Constants.OPENINGSQUAREBRACKET, string.Empty);
                                        variableList.Add(new VariableAssignment()
                                        {
                                            VariableId = landingVariable.Replace(Constants.CLOSINGSQUAREBRACKET, string.Empty),
                                            Value = value.value
                                        });
                                    }
                                }
                            }
                        }
                    }
                    if (floor.sections != null && floor.sections.Count > 0)
                    {
                        var subSectionVariables = new List<VariableAssignment>();
                        foreach (var section in floor.sections)
                        {
                            subSectionVariables = PerLandingVariables(Utility.DeserializeObjectValue<SectionsGroupValues>(Utility.SerializeObjectValue(section)), new List<VariableAssignment>());
                            variableList.AddRange(subSectionVariables);
                        }
                    }
                }
                Utility.LogEnd(methodBeginTime);
                return variableList;
            }


            /// <summary>
            /// method used to generate per specmemo for OBOM xml
            /// </summary>
            /// <param name="variableAssignments"></param>
            /// <param name="numberOfFloors"></param>
            /// <param name="UnitDetails"></param>
            /// <returns></returns>
            public XElement GenerateSpecmemo(List<VariableAssignment> variableAssignments, int numberOfFloors, UnitDetailsForOBOM UnitDetails)
            {
                var methodBeginTime = Utility.LogBegin();
                var twoDParameterslist = Utility.DeserializeObjectValue<List<string>>(System.IO.File.ReadAllText(Constants.TWODPARAMTERLIST));
                var threeDParameterslist = Utility.DeserializeObjectValue<List<string>>(System.IO.File.ReadAllText(Constants.THREEDPARAMTERLIST));
                XElement specMemo = new XElement("SpecMemo");
                variableAssignments = variableAssignments.Where(x => !x.VariableId.Contains("B1P", StringComparison.OrdinalIgnoreCase) && !x.VariableId.Contains("B2P", StringComparison.OrdinalIgnoreCase)
                && !x.VariableId.Contains("A01", StringComparison.OrdinalIgnoreCase) && !x.VariableId.Contains("flag", StringComparison.OrdinalIgnoreCase)
                && !x.VariableId.Contains("invalid", StringComparison.OrdinalIgnoreCase)).ToList();

                //variableAssignments = variableAssignments.Where(x => x.VariableId.Contains("LANDING") && x.VariableId.Contains("Parameter") ).ToList();
                //variableAssignments=variableAssignments.Where(x=>!x.VariableId )
                XElement specMemoVersion = new XElement("SpecMemoVersion", UnitDetails.SpecMemoVersion);
                specMemo.Add(specMemoVersion);
                var specMemoList = new List<SpecMemo>();
                var quoteId = new SpecMemo()
                {
                    ClassName = "ELEVATOR",
                    CharacName = "QUOTEID",
                    Value = UnitDetails.QuoteId
                };
                var groupId = (new SpecMemo()
                {
                    ClassName = "ELEVATOR",
                    CharacName = "GROUPID",
                    Value = UnitDetails.GroupId
                });
                foreach (var variable in variableAssignments)
                {
                    if (!(variable.VariableId.ToUpper().StartsWith("ELEVATOR0") && !variable.VariableId.ToUpper().StartsWith(UnitDetails.ElevatorLocation)))
                    {
                        var specMemoItem = new SpecMemo();
                        specMemoItem.ClassName = "ELEVATOR";
                        var variableIdSplitList = variable.VariableId.Split("."); if (variable.VariableId.Contains("LANDING"))
                        {
                            foreach (var intermediateValue in variableIdSplitList)
                            {
                                if (intermediateValue.StartsWith("LANDING") && char.IsDigit(intermediateValue[intermediateValue.Length - 1]))
                                {
                                    specMemoItem.ClassName = intermediateValue;
                                }
                            }
                        }

                        specMemoItem.CharacName = variableIdSplitList[variableIdSplitList.Count() - 1];
                        specMemoItem.Value = variable.Value;
                        if (variable.VariableId.Contains(".HWYWID", StringComparison.OrdinalIgnoreCase) && Convert.ToInt32(variable.Value) < 12)
                        {
                            specMemoItem.Value = Convert.ToInt32(variable.Value) * 12;
                        }
                        if (variable.VariableId.Contains(".HWYDEP", StringComparison.OrdinalIgnoreCase) && Convert.ToInt32(variable.Value) < 12)
                        {
                            specMemoItem.Value = Convert.ToInt32(variable.Value) * 12;
                        }
                        if (variable.VariableId.Contains(".PITDEPTH", StringComparison.OrdinalIgnoreCase) && Convert.ToInt32(variable.Value) < 12)
                        {
                            specMemoItem.Value = Convert.ToInt32(variable.Value) * 12;
                        }
                        if (variable.VariableId.Contains(".OVHEAD", StringComparison.OrdinalIgnoreCase) && Convert.ToInt32(variable.Value) < 12)
                        {
                            specMemoItem.Value = Convert.ToInt32(variable.Value) * 12;
                        }
                        specMemoList.Add(specMemoItem);
                    }
                }
                var twoDParametersList = specMemoList.Where(x => Utility.CheckEquals(x.ClassName, "ELEVATOR")).ToList();
                var threeDParameters = specMemoList.Where(x => !Utility.CheckEquals(x.ClassName, "ELEVATOR")).ToList();
                foreach (var parameter in threeDParameterslist)
                {
                    twoDParametersList = twoDParametersList.Where(x => !Utility.CheckEquals(parameter, x.CharacName)).ToList();
                }
                foreach (var parameter in twoDParameterslist)
                {
                    threeDParameters = threeDParameters.Where(x => !Utility.CheckEquals(parameter, x.CharacName)).ToList();
                }
                specMemoList = twoDParametersList;
                specMemoList.AddRange(threeDParameters);

                var specMemoSortedList = new List<SpecMemo>();


                specMemoList = specMemoList.GroupBy(x => new { x.ClassName, x.CharacName }).Select(y => y.First()).ToList();
                specMemoList = specMemoList.OrderBy(x => x.ClassName).ThenBy(y => y.CharacName).ToList();
                //specMemoList = specMemoList.ToList().Distinct(); ;

                specMemoSortedList = specMemoList.Where(x => Utility.CheckEquals(x.ClassName, "ELEVATOR")).ToList();
                var specMemoSortedListSP = specMemoSortedList.Where(x => x.CharacName.Contains("_SP", StringComparison.OrdinalIgnoreCase)).ToList();
                var specMemoSortedListEngineeringParamaeter = specMemoSortedList.Where(x => !x.CharacName.Contains("_SP", StringComparison.OrdinalIgnoreCase)).ToList();
                var spParameters = specMemoSortedListSP;
                spParameters.AddRange(specMemoSortedListEngineeringParamaeter);
                for (int landing = 1; landing <= numberOfFloors; landing++)
                {
                    switch (Convert.ToString(landing).Length)
                    {
                        case 1:
                            specMemoSortedList = specMemoList.Where(x => Utility.CheckEquals(x.ClassName, "LANDING00" + Convert.ToString(landing))).ToList();
                            specMemoSortedListSP = specMemoSortedList.Where(x => x.CharacName.Contains("_SP")).ToList();
                            specMemoSortedListEngineeringParamaeter = specMemoSortedList.Where(x => !x.CharacName.Contains("_SP")).ToList();
                            spParameters.AddRange(specMemoSortedListSP);
                            spParameters.AddRange(specMemoSortedListEngineeringParamaeter);
                            break;
                        case 2:
                            specMemoSortedList = specMemoList.Where(x => Utility.CheckEquals(x.ClassName, "LANDING0" + Convert.ToString(landing))).ToList();
                            specMemoSortedListSP = specMemoSortedList.Where(x => x.CharacName.Contains("_SP")).ToList();
                            specMemoSortedListEngineeringParamaeter = specMemoSortedList.Where(x => !x.CharacName.Contains("_SP")).ToList();
                            spParameters.AddRange(specMemoSortedListSP);
                            spParameters.AddRange(specMemoSortedListEngineeringParamaeter);
                            break;
                        default:
                            specMemoSortedList = specMemoList.Where(x => Utility.CheckEquals(x.ClassName, "LANDING" + Convert.ToString(landing))).ToList();
                            specMemoSortedListSP = specMemoSortedList.Where(x => x.CharacName.Contains("_SP")).ToList();
                            specMemoSortedListEngineeringParamaeter = specMemoSortedList.Where(x => !x.CharacName.Contains("_SP")).ToList();
                            spParameters.AddRange(specMemoSortedListSP);
                            spParameters.AddRange(specMemoSortedListEngineeringParamaeter);
                            break;

                    }
                }
                specMemoList = new List<SpecMemo>();
                foreach (var characteristic in UnitDetails.Characteristics)
                {
                    specMemoList.Add(new SpecMemo()
                    {
                        ClassName = characteristic.ClassName,
                        CharacName = characteristic.CharacName,
                        Value = characteristic.Value
                    }
                    );
                }
                specMemoList.AddRange(spParameters);
                specMemoList.Add(quoteId);
                specMemoList.Add(groupId);
                specMemoList.OrderByDescending(x => x.ClassName).Where(y => y.ClassName.Contains("_SP"));
                for (int i = numberOfFloors + 1; i <= 200; i++)
                {
                    var outOfIndex = Convert.ToString(i);
                    specMemoList.RemoveAll(unit => unit.ClassName.Contains(outOfIndex));
                    //specMemoList.RemoveAll(li);
                }
                var specMemos = specMemoList;
                specMemoList = new List<SpecMemo>();
                specMemoList.AddRange(specMemos);
                foreach (var specMemoItem in specMemoList)
                {
                    XElement characteristics = new XElement("Characteristics");
                    XElement className = new XElement("ClassName", specMemoItem.ClassName.ToUpper());

                    XElement characName = new XElement("CharacName", specMemoItem.CharacName.ToUpper());
                    XElement value = new XElement("Value", RoundObjectValue(specMemoItem.Value));
                    characteristics.Add(className);
                    characteristics.Add(characName);
                    characteristics.Add(value);
                    specMemo.Add(characteristics);
                }
                Utility.LogEnd(methodBeginTime);
                return specMemo;
            }


            private object RoundObjectValue(object value)
            {
                try
                {
                    return Math.Round(Convert.ToDecimal(value), 3);
                }
                catch (Exception ex)
                {
                    return value;
                }
            }
            /// <summary>
            /// method used to generate BOM child for OBOM xml
            /// </summary>
            /// <param name="obomVariableAssignments"></param>
            /// <param name="partNumber"></param>
            /// <returns></returns>
            public List<XElement> GenerateBOMChild(List<ObomVariables> obomVariableAssignments, object partNumber)
            {
                var methodBeginTime = Utility.LogBegin();
                List<XElement> bomItem = new List<XElement>();
                if (obomVariableAssignments.Any())
                {
                    XElement bomParent = new XElement("BOMParent");
                    XElement materialInternalID = new XElement("MaterialInternalID", partNumber);

                    bomParent.Add(materialInternalID);
                    foreach (var item in obomVariableAssignments)
                    {
                        if (!string.IsNullOrEmpty(item.VariableId) && !item.VariableId.EndsWith("Module") && item.Quantity > 0)
                        {
                            XElement bomChild = new XElement("BOMChild");
                            XElement itemNumber = new XElement("ItemNumber", item.XMLVariables.ContainsKey("item_num") ? item.XMLVariables["item_num"] : 0);

                            XElement materialInternalID2 = new XElement("MaterialInternalID", item.PartNumber);

                            XElement itemCategory = new XElement("ItemCategory", "X");
                            XElement uom = new XElement("UOM", item.XMLVariables.ContainsKey("qty_units") ? item.XMLVariables["qty_units"] : string.Empty);
                            XElement desiredQuantity = new XElement("DesiredQuantity", item.Quantity);
                            //XElement ctoorETO = new XElement("CTOorETO", "CTO");
                            XElement itemTextLine1 = new XElement("ItemTextLine1", item.Description);
                            XElement componentScrapPercent = new XElement("ComponentScrapPercent", "0");
                            XElement revision = new XElement("Revision", item.XMLVariables.ContainsKey("revision") ? item.XMLVariables["revision"] : item.XMLVariables.ContainsKey("mco_rev") ? item.XMLVariables["mco_rev"] : item.XMLVariables.ContainsKey("eco_rev") ? item.XMLVariables["eco_rev"] : string.Empty);

                            bomChild.Add(itemNumber);
                            bomChild.Add(materialInternalID2);
                            bomChild.Add(itemCategory);
                            bomChild.Add(uom);
                            bomChild.Add(desiredQuantity);
                            //bomChild.Add(ctoorETO);
                            bomChild.Add(itemTextLine1);
                            bomChild.Add(componentScrapPercent);
                            bomChild.Add(revision);
                            foreach (var charac in item.Characteristics)
                            {
                                if (!Utility.CheckEquals(charac.Key, "bomgroup") && !Utility.CheckEquals(charac.Key, "showas") && !string.IsNullOrEmpty(Convert.ToString(charac.Value)) && !Utility.CheckEquals(charac.Key, "name") && !Utility.CheckEquals(charac.Key, "fullyqualifiedname") && !Utility.CheckEquals(charac.Key, "foreach") && !Utility.CheckEquals(charac.Key, "itemnum") && !Utility.CheckEquals(charac.Key, "show"))
                                {
                                    XElement characteristic = new XElement("Characteristic");
                                    XElement className = Convert.ToString(charac.Key).ToUpper().Contains("LANDING") ? new XElement("ClassName", Convert.ToString(charac.Key).ToUpper()) : new XElement("ClassName", "XXXX");
                                    XElement characName = new XElement("CharacName", charac.Key.ToUpper());
                                    XElement value = new XElement("Value", charac.Value);
                                    characteristic.Add(className);
                                    characteristic.Add(characName);
                                    characteristic.Add(value);
                                    bomChild.Add(characteristic);
                                }
                            }
                            foreach (var charac in item.XMLVariables)
                            {
                                if (Utility.CheckEquals(charac.Key, "legacy_sec"))
                                {
                                    XElement characteristic = new XElement("Characteristic");
                                    XElement classNames = Utilities.CheckEquals(Convert.ToString(charac.Key), "FloorNumber") ? new XElement("ClassName", "FloorNumber") : new XElement("ClassName", "XXXX");
                                    XElement characNames = new XElement("CharacName", charac.Key.ToUpper());
                                    XElement values = new XElement("Value", charac.Value);
                                    characteristic.Add(classNames);
                                    characteristic.Add(characNames);
                                    characteristic.Add(values);
                                    bomChild.Add(characteristic);
                                }
                            }
                            bomParent.Add(bomChild);
                        }

                    }
                    bomItem.Add(bomParent);
                    foreach (var item in obomVariableAssignments)
                    {
                        if (item.Child != null && item.Child.Any() && item.Quantity > 0)
                        {
                            bomItem.AddRange(GenerateBOMChild(item.Child, item.PartNumber));
                        }

                    }
                }
                Utility.LogEnd(methodBeginTime);
                return bomItem.Distinct().ToList();
            }


            public ConfigurationRequest CreateConfigurationRequestWithTemplate(string packageName)
            {
                var methodBeginTime = Utility.LogBegin();
                var stubReqbody = JObject.Parse(System.IO.File.ReadAllText(Constants.OBOMREQESTBODYSTUBPATH)).ToString();
                var configurationRequest = Utility.DeserializeObjectValue<ConfigurationRequest>(stubReqbody);
                configurationRequest.PackagePath = packageName;
                configurationRequest.Line.ProductId = packageName;
                Utility.LogEnd(methodBeginTime);
                return configurationRequest;
            }

            public async Task<ResponseMessage> GenerateExcelForStatusReport(ConfigurationDetails configurationDetails)
            {
                var methodBeginTime = Utility.LogBegin();
                if (!(System.IO.Directory.Exists("StatusReport")))
                    System.IO.Directory.CreateDirectory("StatusReport");
                var excelWorkBook = new WorkBook();
                var configurationDetailsForStatusReport = _obomdl.GetConfigurationDetailsForStatusReport(configurationDetails.QuoteId);
                if (System.IO.File.Exists(@"StatusReport" + "\\" + configurationDetails.QuoteId + ".xlsx"))
                {
                    excelWorkBook = WorkBook.Load(@"StatusReport" + "\\" + configurationDetails.QuoteId + ".xlsx");
                }
                else
                {
                    excelWorkBook = WorkBook.Create(ExcelFileFormat.XLS);
                }
                WorkSheet xlsSheet = excelWorkBook.CreateWorkSheet(DateTime.Now.Day + Constants.HYPHEN + DateTime.Now.ToString("MMM") + Constants.HYPHEN + DateTime.Now.Year + Constants.UNDERSCORE + DateTime.Now.Hour + Constants.HYPHEN + DateTime.Now.Minute);
                //Add data and styles to the new worksheet



                xlsSheet["A1"].Value = "QuoteId";
                xlsSheet["A1"].Style.Font.Bold = true;
                xlsSheet["B1"].Value = "Quote Status";
                xlsSheet["B1"].Style.Font.Bold = true;
                xlsSheet["C1"].Value = "Quote Deleted";
                xlsSheet["C1"].Style.Font.Bold = true;
                xlsSheet["D1"].Value = "Building Name";
                xlsSheet["D1"].Style.Font.Bold = true;
                xlsSheet["E1"].Value = "Building Id";
                xlsSheet["E1"].Style.Font.Bold = true;
                xlsSheet["F1"].Value = "Building Status";
                xlsSheet["F1"].Style.Font.Bold = true;
                xlsSheet["G1"].Value = "Building Deleted";
                xlsSheet["G1"].Style.Font.Bold = true;
                xlsSheet["H1"].Value = "Group Name";
                xlsSheet["H1"].Style.Font.Bold = true;
                xlsSheet["I1"].Value = "Group Id";
                xlsSheet["I1"].Style.Font.Bold = true;
                xlsSheet["J1"].Value = "Group Status";
                xlsSheet["J1"].Style.Font.Bold = true;
                xlsSheet["K1"].Value = "Group Deleted";
                xlsSheet["K1"].Style.Font.Bold = true;
                xlsSheet["L1"].Value = "Unit Designation";
                xlsSheet["L1"].Style.Font.Bold = true;
                xlsSheet["M1"].Value = "Unit Id";
                xlsSheet["M1"].Style.Font.Bold = true;
                xlsSheet["N1"].Value = "Unit Factory Job Id";
                xlsSheet["N1"].Style.Font.Bold = true;
                xlsSheet["O1"].Value = "UEID";
                xlsSheet["O1"].Style.Font.Bold = true;
                xlsSheet["P1"].Value = "Unit Deleted";
                xlsSheet["P1"].Style.Font.Bold = true;
                xlsSheet["Q1"].Value = "Unit Location";
                xlsSheet["Q1"].Style.Font.Bold = true;
                xlsSheet["R1"].Value = "Product Name";
                xlsSheet["R1"].Style.Font.Bold = true;
                xlsSheet["S1"].Value = "SetId";
                xlsSheet["S1"].Style.Font.Bold = true;
                xlsSheet["T1"].Value = "Unit Status";
                xlsSheet["T1"].Style.Font.Bold = true;
                ////Enter values to the cells from A3 to A5
                int counter = 1;

                foreach (var building in configurationDetailsForStatusReport.BuildingStatusReport)
                {
                    if (!building.GroupStatusReport.Any())
                    {
                        counter += 1;
                        var counterString = Convert.ToString(counter);
                        xlsSheet["A" + counterString].Value = configurationDetailsForStatusReport.QuoteId;
                        xlsSheet["B" + counterString].Value = configurationDetailsForStatusReport.QuoteStatus;
                        xlsSheet["C" + counterString].Value = configurationDetailsForStatusReport.QuoteIsDeleted;
                        xlsSheet["D" + counterString].Value = building.BuildingName;
                        xlsSheet["E" + counterString].Value = building.BuildingId;
                        xlsSheet["F" + counterString].Value = building.BuildingStatus;
                        xlsSheet["G" + counterString].Value = building.BuildingIsDeleted;
                        xlsSheet["H" + counterString].Value = string.Empty;
                        xlsSheet["I" + counterString].Value = string.Empty;
                        xlsSheet["J" + counterString].Value = string.Empty;
                        xlsSheet["K" + counterString].Value = string.Empty;
                        xlsSheet["L" + counterString].Value = string.Empty;
                        xlsSheet["M" + counterString].Value = string.Empty;
                        xlsSheet["N" + counterString].Value = string.Empty;
                        xlsSheet["O" + counterString].Value = string.Empty;
                        xlsSheet["P" + counterString].Value = string.Empty;
                        xlsSheet["Q" + counterString].Value = string.Empty;
                        xlsSheet["R" + counterString].Value = string.Empty;
                        xlsSheet["S" + counterString].Value = string.Empty;
                        xlsSheet["T" + counter].Value = string.Empty;
                    }
                    foreach (var group in building.GroupStatusReport)
                    {
                        if (!group.UnitStatusReport.Any())
                        {
                            counter += 1;
                            var counterString = Convert.ToString(counter);
                            xlsSheet["A" + counterString].Value = configurationDetailsForStatusReport.QuoteId;
                            xlsSheet["B" + counterString].Value = configurationDetailsForStatusReport.QuoteStatus;
                            xlsSheet["C" + counterString].Value = configurationDetailsForStatusReport.QuoteIsDeleted;
                            xlsSheet["D" + counterString].Value = building.BuildingName;
                            xlsSheet["E" + counterString].Value = building.BuildingId;
                            xlsSheet["F" + counterString].Value = building.BuildingStatus;
                            xlsSheet["G" + counterString].Value = building.BuildingIsDeleted;
                            xlsSheet["H" + counterString].Value = group.GroupName;
                            xlsSheet["I" + counterString].Value = group.GroupId;
                            xlsSheet["J" + counterString].Value = group.GroupStatus;
                            xlsSheet["K" + counterString].Value = group.GroupIsDeleted;
                            xlsSheet["L" + counterString].Value = string.Empty;
                            xlsSheet["M" + counterString].Value = string.Empty;
                            xlsSheet["N" + counterString].Value = string.Empty;
                            xlsSheet["O" + counterString].Value = string.Empty;
                            xlsSheet["P" + counterString].Value = string.Empty;
                            xlsSheet["Q" + counterString].Value = string.Empty;
                            xlsSheet["R" + counterString].Value = string.Empty;
                            xlsSheet["S" + counterString].Value = string.Empty;
                            xlsSheet["T" + counter].Value = string.Empty;
                        }
                        foreach (var unit in group.UnitStatusReport)
                        {
                            counter += 1;
                            var counterString = Convert.ToString(counter);
                            xlsSheet["A" + counterString].Value = configurationDetailsForStatusReport.QuoteId;
                            xlsSheet["B" + counterString].Value = configurationDetailsForStatusReport.QuoteStatus;
                            xlsSheet["C" + counterString].Value = configurationDetailsForStatusReport.QuoteIsDeleted;
                            xlsSheet["D" + counterString].Value = building.BuildingName;
                            xlsSheet["E" + counterString].Value = building.BuildingId;
                            xlsSheet["F" + counterString].Value = building.BuildingStatus;
                            xlsSheet["G" + counterString].Value = building.BuildingIsDeleted;
                            xlsSheet["H" + counterString].Value = group.GroupName;
                            xlsSheet["I" + counterString].Value = group.GroupId;
                            xlsSheet["J" + counterString].Value = group.GroupStatus;
                            xlsSheet["K" + counterString].Value = group.GroupIsDeleted;
                            xlsSheet["L" + counterString].Value = unit.UnitName;
                            xlsSheet["M" + counterString].Value = unit.UnitId;
                            xlsSheet["N" + counterString].Value = unit.FactoryJobId;
                            xlsSheet["O" + counterString].Value = unit.UEID;
                            xlsSheet["P" + counterString].Value = unit.UnitIsDeleted;
                            xlsSheet["Q" + counterString].Value = unit.UnitLocation;
                            xlsSheet["R" + counterString].Value = unit.ProductName;
                            xlsSheet["S" + counterString].Value = unit.SetId;
                            xlsSheet["T" + counter].Value = unit.UnitStatus;
                        }
                    }
                }


                //Save the excel file
                excelWorkBook.SaveAs(@"StatusReport" + "\\" + configurationDetails.QuoteId + ".xlsx");
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage();
            }

        }
    }