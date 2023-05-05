using Configit.Configurator.Server.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Globalization;
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
using Line = Configit.Configurator.Server.Common.Line;
using System.Text.RegularExpressions;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.BusinessProcess.Services
{

    public class ConfigureBL : IConfigure
    {
        #region Variables
        private readonly IGroupConfigurationDL _groupConfiguration;
        private readonly IConfigurationSection _configuration;
        private readonly IConfiguratorService _configuratorService;
        private readonly IUnitConfigurationDL _unitConfigurationDL;
        private readonly string _environment;
        private readonly IConfigureServices _configureService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuth _auth;
        private readonly IOpeningLocationDL _openingLocationdl;
        private static JsonSerializer Serializer { get; set; }
        private readonly ICacheManager _cpqCacheManager;
        private readonly IStringLocalizer<GenerateTokenBL> _localizer;
        #endregion

        /// <summary>
        /// Constructor for Initializing the Business Logic object
        /// </summary>
        /// <param Name="configuration"></param>
        /// <param Name="configureServices"></param>
        /// <param Name="configuratorService"></param>
        /// <param Name="auth"></param>
        /// <param Name="serializer"></param>
        /// <param Name="cpqCacheManager"></param>
        /// <param Name="httpContextAccessor"></param>
        /// <param Name="utility"></param>
        /// <param Name="localizer"></param>
        /// <param Name="groupConfiguration"></param>
        /// <param Name="unitConfigurationDL"></param>
        /// <param Name="logger"></param>
        public ConfigureBL(IConfiguration configuration, IConfigureServices configureServices,
            IConfiguratorService configuratorService, IAuth auth, JsonSerializer serializer,
             ICacheManager cpqCacheManager, IOpeningLocationDL openingLocationdl,
            IHttpContextAccessor httpContextAccessor, IStringLocalizer<GenerateTokenBL> localizer, IGroupConfigurationDL groupConfiguration, IUnitConfigurationDL unitConfigurationDL, ILogger<ConfigureBL> logger)
        {
            Utility.SetLogger(logger);
            _groupConfiguration = groupConfiguration;
            _unitConfigurationDL = unitConfigurationDL;
            _openingLocationdl = openingLocationdl;
            _configuration = configuration?.GetSection(Constant.PARAMSETTINGS);
            if (_configuration == null)
            {
                return;
            }
            Serializer = serializer;
            _environment = _configuration[Constant.ENVIRONMENT];
            _configuratorService = configuratorService;
            _cpqCacheManager = cpqCacheManager;
            _configureService = configureServices;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
            _auth = auth;
            if (_httpContextAccessor.HttpContext != null)
            {
                var sessionId = _httpContextAccessor?.HttpContext?.Request?.Headers[Constant.SESSIONID];
                var isProdChk = _cpqCacheManager.GetCache(sessionId, _environment, Constant.ISPRODUCTIONCHECK);
                bool isProductionChk = Convert.ToBoolean(isProdChk);
                _configuration = isProductionChk ? configuration?.GetSection(Constant.PRODUCTIONCHECKSETTINGS) : configuration?.GetSection(Constant.PARAMSETTINGS);
                if (_configuration == null)
                {
                    return;
                }
            }

        }

        /// <summary>
        /// Business logic for Sublines API
        /// </summary>
        /// <param Name="request"></param>
        /// <param Name="storageCredentials"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> RequestConfigurationBl(SublineRequest request, string packagePath, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var packagePathRequest = packagePath;
            if (string.IsNullOrEmpty(packagePathRequest))
            {
                packagePathRequest = request?.PackagePath;
            }
            var response = new SublineResponse
            {
                PackagePath = packagePathRequest,
                ParentCode = string.Format(CultureInfo.InvariantCulture, Guid.NewGuid().ToString()),
                Line = request?.Line,
                ConfigurationTimeStamp = new ConfigurationTimeStamp
                {
                    CreatedOn = DateTime.Now
                }
            };
            if (response.Line == null)
            {
                response.Line = new Line();
            }
            response.Line.PriceSheet = new PriceSheet
            {
                Id = Constant.ROOT,
                Totals = new PriceSummary
                {
                    Total = new ValueWithCurrency(),
                    Net = new ValueWithCurrency(),
                    Tax = new ValueWithCurrency()
                }
            };
            var responseObj = new ResponseMessage
            {
                Response = JObject.FromObject(response),
                StatusCode = Constant.SUCCESS
            };
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.SUBLINESRESPONSE,
                response.ParentCode, Utility.SerializeObjectValue(response));
            Utility.LogEnd(methodBeginTime);
            return responseObj;
        }

        /// <summary>
        /// Business logic for Configure API
        /// </summary>
        /// <param Name="request"></param>
        /// <param Name="packagePath"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> ConfigurationBl(ConfigurationRequest request, string packagePath, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var packagePathConfigure = string.Empty;
            switch (packagePath)
            {
                case Constant.BUILDINGPACKAGEPATH:
                    packagePathConfigure = _configuration[Constant.BUILDINGCONFIGURATIONPATH];
                    break;
                case Constant.GROUPPACKAGEPATH:
                    packagePathConfigure = _configuration[Constant.GROUPCONFIGURATIONPATH];
                    break;
                case Constant.UNITPACKAGEPATH:
                    packagePathConfigure = _configuration[Constant.UNITVALIDATIONPATH];
                    break;
                case Constant.PRODUCTSELECTIONPACKAGEPATH:
                    packagePathConfigure = _configuration[Constant.PRODUCTSELECTIONPATH];
                    break;
                case Constant.LIFTDESIGNERPACKAGEPATH:
                    packagePathConfigure = _configuration[Constant.LIFTDESIGNERPATH];
                    break;
                default:
                    packagePathConfigure = packagePath;
                    break;
            }
            if (request != null)
            {
                request.Settings.IncludeStateAndJustification = true;
            }
            var configurCallStartTime = Utility.LogBegin("Start of " + packagePathConfigure + " JSME call ");
            var responseObj = await _configuratorService.Configure(request, packagePathConfigure)
                .ConfigureAwait(false);
            Utility.LogEnd(configurCallStartTime, "End of " + packagePathConfigure + " JSME call ");
            Utility.LogEnd(methodBeginTime);
            return responseObj;
        }

        /// <inheritdoc />
        /// <summary>
        /// Business logic for StartConfigure API
        /// </summary>
        /// <param Name="configureRequest"></param>
        /// <param Name="modelNumber"></param>
        /// <param Name="locale"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="distributedCache"></param>
        /// <returns></returns>

        public async Task<ResponseMessage> StartBuildingConfigure(string sessionId, ConfigurationRequest configureRequest = null)
        {
            var methodBeginTime = Utility.LogBegin();
            var packagePath = configureRequest?.PackagePath;
            var modifiedOn = new DateTime();
            //checks whether the configuration is in cache for page reload
            var currentConfigurationCache = _cpqCacheManager.GetCache(sessionId, _environment,
            Constant.CURRENTMACHINECONFIGURATION);
            // get the constant Values 
            var buildingContantsDictionary = Utility.GetVariableMapping(Constant.BUILDINGMAPPERVARIABLESMAPPERPATH, Constant.BUILDINGMAPPERCONFIGURATION);
            if (configureRequest != null && (string.IsNullOrEmpty(packagePath)))
            {
                if (String.IsNullOrEmpty(currentConfigurationCache))
                {
                    Utility.LogEnd(methodBeginTime);
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = _localizer[Constant.SESSIONIDPARENTCODEISNOMOREVALID]
                    });
                }
                var currentConfiguration = Utility.DeserializeObjectValue<StartConfigureResponse>(currentConfigurationCache);
                if (currentConfiguration.Audits.ModifiedOn == null)
                {
                    currentConfiguration.Audits.ModifiedOn = new DateTime();
                }
                if (currentConfiguration != null && modifiedOn != default(DateTime) &&
                    DateTime.Compare(modifiedOn, (DateTime)currentConfiguration.Audits.ModifiedOn) > 0)
                {
                    currentConfiguration.Audits.ModifiedOn = modifiedOn;
                }
                var value = Utility.SerializeObjectValue(currentConfiguration);
                _cpqCacheManager.SetCache(sessionId, _environment, Constant.CURRENTMACHINECONFIGURATION, value);
                //Response for UI
                var updatedStartConfigureResponse = ViewModelResponseMapper(currentConfiguration);
                var updatedCurrentConfiguration = Utility.SerializeObjectValue(updatedStartConfigureResponse);
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage
                {
                    Response = JObject.Parse(updatedCurrentConfiguration),
                    StatusCode = Constant.SUCCESS
                };
            }
            //Gets the base configuration of the model
            var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest);
            baseConfigureRequest = GenerateIncludeSections(baseConfigureRequest, Constant.BUILDINGCONFIGURATION);
            var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, packagePath, sessionId).ConfigureAwait(false);
            Utility.LogTrace(Constant.STARTBUILDINGCONFIGVTPACKAGERESPONSE + JsonConvert.SerializeObject(baseConfigureResponseJObj));
            var baseConfigureResponse = baseConfigureResponseJObj.Response.ToObject<ConfigurationResponse>();
            // getting required Section Values from the configurator service response 
            var buildingConfiguration = baseConfigureResponse.Sections;
            var configureResponse = baseConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            // configuration object values 
            var configureResponseArgument = configureResponse.Arguments;
            // adding defaults to cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTBUILDINGCONFIGVALUES, Utility.SerializeObjectValue(configureResponseArgument));
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            var configureRequestDictionary = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();
            // gettinglocal stub values for get required variables
            var stubBuildingConfigurationdefaultResponse = JObject.Parse(File.ReadAllText(Constant.STARTBUILDINGCONFIGURATIONSTUBPATH));
            // setting stub data into an sectionsValues object
            var stubConfigurationResponseObj = stubBuildingConfigurationdefaultResponse.ToObject<SectionsValues>();

            var mainFillteredBuildingConfigResponse = Utility.MapVariables(Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(baseConfigureResponse)), Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(stubConfigurationResponseObj)));
            if (mainFillteredBuildingConfigResponse != null)
            {
                var sectionsData = new Sections();
                sectionsData = Utility.DeserializeObjectValue<Sections>(Utility.SerializeObjectValue(mainFillteredBuildingConfigResponse));
                buildingConfiguration = new List<Sections>();
                buildingConfiguration.Add(sectionsData);
            }

            var changesValues = CompareChangeInConfigResponse(baseConfigureResponse, Constant.BUILDING, configureRequestDictionary);
            var changeConflics = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(changesValues));
            // adding the conflicts to the cachce as pervious conflicts
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.PREVIOUSCONFLICTSVALUES, Utility.SerializeObjectValue(changesValues));
            var buildingResponse = Utility.FilterNullValues(buildingConfiguration.FirstOrDefault());
            buildingResponse[Constant.CONFLICTMANAGEMENT] = Utility.DeserializeObjectValue<JToken>(Utility.SerializeObjectValue(changeConflics));
            Utility.LogEnd(methodBeginTime);
            if (configureRequest != null)
            {
                return new ResponseMessage
                {
                    StatusCode = Constant.SUCCESS,
                    Response = Utility.FilterNullValues(buildingResponse)
                };
            }
            throw new CustomException(new ResponseMessage
            {
                StatusCode = Constant.BADREQUEST,
                Message = _localizer[Constant.REQUESTCANNOTBENULL]
            });
        }

        public async Task<ResponseMessage> TestStartConfigureBl(string parentCode, string modelNumber, string sessionId,
        ConfigurationRequest configureRequest = null, string locale = null)
        {
            var methodBeginTime = Utility.LogBegin();
            var packagePath = configureRequest?.PackagePath;
            var modifiedOn = new DateTime();
            var parentCodeValue = parentCode + Constant.UNDERSCORE + modelNumber;
            //checks whether the configuration is in cache for page reload
            var currentConfigurationCache = _cpqCacheManager.GetCache(sessionId, _environment,
            Constant.CURRENTMACHINECONFIGURATION, parentCodeValue);
            if (configureRequest != null && (string.IsNullOrEmpty(packagePath)))
            {
                if (String.IsNullOrEmpty(currentConfigurationCache))
                {
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = _localizer[Constant.SESSIONIDPARENTCODEISNOMOREVALID + locale?.Split(Constant.UNDERSCORECHAR)[0]].Value
                    });
                }
                //Settings the machine modified on or services modified on based on the latest modified time
                var currentConfiguration = Utility.DeserializeObjectValue<StartConfigureResponse>(currentConfigurationCache);
                if (currentConfiguration.Audits.ModifiedOn == null)
                {
                    currentConfiguration.Audits.ModifiedOn = new DateTime();
                }
                if (currentConfiguration != null && modifiedOn != default(DateTime) &&
                    DateTime.Compare(modifiedOn, (DateTime)currentConfiguration.Audits.ModifiedOn) > 0)
                {
                    currentConfiguration.Audits.ModifiedOn = modifiedOn;
                }
                //Checking Delta Price Flag
                var value = Utility.SerializeObjectValue(currentConfiguration);
                _cpqCacheManager.SetCache(sessionId, _environment, Constant.CURRENTMACHINECONFIGURATION, parentCodeValue, value);
                //Response for UI
                var updatedStartConfigureResponse = ViewModelResponseMapper(currentConfiguration);
                var updatedCurrentConfiguration = Utility.SerializeObjectValue(updatedStartConfigureResponse);
                return new ResponseMessage
                {
                    Response = JObject.Parse(updatedCurrentConfiguration),
                    StatusCode = Constant.SUCCESS
                };
            }
            //caching machine request to use in services for reverse mapping
            _cpqCacheManager.SetCache(sessionId, _environment,
                Constant.MACHINEREQUESTCPQ, parentCode + Constant.UNDERSCORE + modelNumber,
                Utility.SerializeObjectValue(configureRequest));
            //Gets the base configuration of the model
            var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest, modelNumber);
            var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, packagePath, sessionId).ConfigureAwait(false);
            var baseConfigureResponse = baseConfigureResponseJObj.Response.ToObject<BaseConfigureResponse>();
            baseConfigureResponse.TotPrice = 0;
            Utility.LogEnd(methodBeginTime);
            if (configureRequest != null)
            {
                return new ResponseMessage
                {
                    StatusCode = Constant.SUCCESS,
                    //Response = Utility.FilterNullValues(configResponse)
                    Response = Utility.FilterNullValues(baseConfigureResponse)
                };
            }
            throw new CustomException(new ResponseMessage
            {
                StatusCode = Constant.BADREQUEST,
                Message = _localizer[Constant.REQUESTCANNOTBENULL + locale?.Split(Constant.UNDERSCORECHAR)[0]].Value
            });
        }

        /// <summary>
        /// Mapper class for UI Response
        /// </summary>
        /// <param Name="startConfigureResponse"></param>
        /// <returns></returns>
        public ViewModelResponse ViewModelResponseMapper(StartConfigureResponse startConfigureResponse)
        {
            var methodBeginTime = Utility.LogBegin();
            var configureResponse = Utility.SerializeObjectValue(startConfigureResponse);
            var viewResponse = Utility.DeserializeObjectValue<ViewModelResponse>(configureResponse);
            (from section in viewResponse.Sections
             from variable in section.Variables
             from value in variable.Values
             where value.Assigned != null && !Utility.CheckEquals(
                     value.Assigned?.ToString().Trim().ToUpperInvariant(),
                     Constant.BYUSER)
             select value).ToList().ForEach(p => p.Assigned = ValueStateAssignedView.BySystem);
            var updatedPackagePath = viewResponse?.PackagePath?.Split(Constant.TILDE)[0].Split(Constant.SLASHCHAR).LastOrDefault();
            if (viewResponse != null)
            {
                viewResponse.PackagePath = updatedPackagePath;
            }
            var totalPriceValue = (
                from sections in startConfigureResponse.Sections
                from variables in sections.Variables
                from values in variables?.Values?.OfType<SingletonValue>()
                where (
                    Utility.CheckEquals(values.Assigned.ToString(), Constant.BYRULE) &&
                    Utility.CheckEquals(sections.Id, Constant.PRICEVARIABLES))
                select values).Select(value => value.Value.ToString()).ToList();
            if (totalPriceValue.Count > 0)
            {
                viewResponse.totPrice = totalPriceValue.Sum(x => Convert.ToDouble(x));
            }
            Utility.LogEnd(methodBeginTime);
            return viewResponse;
        }

        /// <summary>
        /// Fetching assigned option codes from configure response
        /// </summary>
        /// <param Name="sections"></param>
        /// <returns></returns>
        public static List<string> FetchAssignedOptionCodes(IReadOnlyList<Section> sections)
        {
            var methodBeginTime = Utility.LogBegin();
            Utility.LogEnd(methodBeginTime);
            return (from section in sections
                    from variable in section.Variables
                    from value in variable.Values
                    where value.Assigned != null && (Utility.CheckEquals(value.Assigned.ToString(), Constant.BYDEFAULT) ||
                                                     Utility.CheckEquals(value.Assigned.ToString(), Constant.BYRULE) ||
                                                     Utility.CheckEquals(value.Assigned.ToString(), Constant.BYUSER))
                    select value).OfType<SingletonValue>().Select(value => value.Value.ToString()).ToList();
        }

        /// <summary>
        /// Business logic for ChangeConfigure API
        /// </summary>
        /// <param Name="configureRequest"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="distributedCache"></param>
        /// <returns></returns>
        public async Task<Tuple<JObject, StartConfigureResponse>> ChangeBuildingConfigure(JObject variableAssignments, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var cachedData = GetCrosspackageVariableAssignments(sessionId, Constant.BUILDINGCONFIGURATION);
            var crossPackagevariableAssignments = new List<VariableAssignment>();
            var buildingContantsDictionary = Utility.GetVariableMapping(Constant.BUILDINGMAPPERVARIABLESMAPPERPATH, Constant.BUILDINGMAPPERCONFIGURATION);
            if (!(string.IsNullOrEmpty(cachedData)))
            {
                crossPackagevariableAssignments = Utility.DeserializeObjectValue<List<VariableAssignment>>(cachedData);
            }
            var variables = variableAssignments[Constant.VARIABLEASSIGNMENTS].Where(k => k.SelectToken(Constant.VALUE).ToString().Length > 0);
            var filteredVariables = new JObject();
            filteredVariables = new JObject(new JProperty(Constant.VARIABLEASSIGNMENTS, variables));
            var assignments = Utility.DeserializeObjectValue<Line>(Utility.SerializeObjectValue(filteredVariables));
            //Removing required fields from cache if building code is changed
            var oldBuildingCode = (from assignment in crossPackagevariableAssignments
                                   where assignment.VariableId.Contains(buildingContantsDictionary[Constant.BUILDINGCODE])
                                   select assignment.Value).FirstOrDefault();
            var newBuildingCode = assignments.VariableAssignments.Where(x => x.VariableId.Contains(buildingContantsDictionary[Constant.BUILDINGCODE])).ToList();
            if (newBuildingCode.Count() > 0)
            {
                if (oldBuildingCode != null)
                {
                    if (!oldBuildingCode.Equals(newBuildingCode.FirstOrDefault().Value))
                    {
                        var variablesToBeRemoved = JObject.Parse(File.ReadAllText(Constant.BUILDINGMAPPERVARIABLESMAPPERPATH))[Constant.BUILDINGVARIABLESFORBUILDINGCODED].ToList();
                        crossPackagevariableAssignments = crossPackagevariableAssignments.Where(x => !variablesToBeRemoved.Contains(x.VariableId)).ToList();
                    }
                }
                else
                {
                    var variablesToBeRemoved = JObject.Parse(File.ReadAllText(Constant.BUILDINGMAPPERVARIABLESMAPPERPATH))[Constant.BUILDINGVARIABLESFORBUILDINGCODED].ToList();
                    crossPackagevariableAssignments = crossPackagevariableAssignments.Where(x => !variablesToBeRemoved.Contains(x.VariableId)).ToList();
                }
                assignments.VariableAssignments = assignments.VariableAssignments.Where(x => !x.VariableId.Contains(Constants.ELEVBASE)).ToList();
            }
            //generate cross package variable assignments
            crossPackagevariableAssignments = GenerateVariableAssignmentsForUnitConfiguration(crossPackagevariableAssignments, assignments);
            var variableAssignmentz = new Line
            {
                VariableAssignments = crossPackagevariableAssignments
            };
            SetCrosspackageVariableAssignments(crossPackagevariableAssignments, sessionId, Constant.BUILDINGCONFIGURATION);
            variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(variableAssignmentz));
            var configureRequest = CreateConfigurationRequestWithTemplate(variableAssignments, Constant.BUILDINGNAME);
            var packagePath = configureRequest?.PackagePath;
            if (configureRequest == null)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = _localizer[Constant.REQUESTCANNOTBENULL + Constant.HYPHEN]
                });
            }
            //include section
            configureRequest = GenerateIncludeSections(configureRequest, Constant.BUILDINGCONFIGURATION);
            var configureResponseJObj =
                await ConfigurationBl(configureRequest, packagePath, sessionId).ConfigureAwait(false);
            var configureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
            // configuration object values 
            var configureResponseArgument = configureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            // adding defaults to cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTBUILDINGCONFIGVALUES, Utility.SerializeObjectValue(configureResponseArgumentJObject));
            var configureRequestDictionary = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();
            var baseConfigureResponse = configureResponseJObj.Response.ToObject<ConfigurationResponse>();
            // getting required Section Values from the configurator service response 
            var buildingConfiguration = baseConfigureResponse.Sections;
            // gettinglocal stub values for get required variables
            var stubConfigurationResponse = JObject.Parse(File.ReadAllText(Constant.STARTBUILDINGCONFIGURATIONSTUBPATH));
            // setting stub data into an sectionsValues object
            var stubConfigurationResponseObj = stubConfigurationResponse.ToObject<SectionsValues>();
            var mainFilteredBuildingConfigResponse = Utility.MapVariables(Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(baseConfigureResponse)), Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(stubConfigurationResponseObj)));
            if (mainFilteredBuildingConfigResponse != null)
            {
                var sectionsData = new Sections();
                sectionsData = Utility.DeserializeObjectValue<Sections>(Utility.SerializeObjectValue(mainFilteredBuildingConfigResponse));
                buildingConfiguration = new List<Sections>();
                buildingConfiguration.Add(sectionsData);
            }

            var changesValues = CompareChangeInConfigResponse(baseConfigureResponse, Constant.BUILDING, configureRequestDictionary);
            // adding the conflicts to the cachce as pervious conflicts
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.PREVIOUSCONFLICTSVALUES, Utility.SerializeObjectValue(changesValues));
            //Adding dynamic range validation message for Average Roof Height
            var enrichedData = JObject.Parse(File.ReadAllText(Constant.BUILDINGENRICHEDDATA));
            var buildingRiseValue = variableAssignmentz.VariableAssignments.Where(x => x.VariableId.Contains(buildingContantsDictionary[Constant.TOTALBUILDINGFLOORTOFLOORHEIGHT])).ToList();
            if (buildingRiseValue != null && buildingRiseValue.Any())
            {
                var avgHeightProperties = (enrichedData[Constant.VARIABLES][buildingContantsDictionary[Constant.AVGHEIGHT]][Constant.PROPERTIES]).ToObject<List<Properties>>();
                foreach (var variable in buildingConfiguration.FirstOrDefault().sections.FirstOrDefault().Variables)
                {
                    if (Utility.CheckEquals(variable.Id, buildingContantsDictionary[Constant.AVGHEIGHT]))
                    {
                        if (avgHeightProperties != null && avgHeightProperties.Any())
                        {
                            foreach (var prop in avgHeightProperties)
                            {
                                if (prop.Id.Equals(Constant.MINVALUESMALLCASE, StringComparison.OrdinalIgnoreCase))
                                {
                                    prop.Value = Convert.ToDecimal(buildingRiseValue.FirstOrDefault().Value) + 120;
                                }
                                variable.Properties.Add(prop);
                            }
                        }
                    }
                }
            }
            //Conflict management for Average Roof Height
            var averageHeightVariable = (from variable in variableAssignmentz.VariableAssignments
                                         where variable.VariableId.Equals(buildingContantsDictionary[Constant.AVGHEIGHT], StringComparison.OrdinalIgnoreCase)
                                         select variable).ToList();
            if (averageHeightVariable.Any() && Convert.ToDecimal(averageHeightVariable.FirstOrDefault().Value) > 0 && Convert.ToDecimal(averageHeightVariable.FirstOrDefault().Value) < (Convert.ToDecimal(buildingRiseValue.FirstOrDefault().Value) + 10))
            {
                var conflictVariable = new ConflictMgmtList
                {
                    VariableId = buildingContantsDictionary[Constant.AVGHEIGHT],
                    VariableName = buildingContantsDictionary[Constant.AVGHEIGHT]
                };
                changesValues.PendingAssignments.Add(conflictVariable);
            }
            var changeConflics = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(changesValues));
            var buildingResponse = Utility.FilterNullValues(buildingConfiguration.FirstOrDefault());
            buildingResponse[Constant.CONFLICTASSIGNMENTS] = Utility.DeserializeObjectValue<JToken>(Utility.SerializeObjectValue(changeConflics));
            Utility.LogEnd(methodBeginTime);
            return Tuple.Create(Utility.FilterNullValues(buildingResponse), configureResponse);
        }

        /// <summary>
        /// ChangeGroupConfiguration BL
        /// </summary>
        /// <param Name="configureRequest"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="parentCode"></param>
        /// <param Name="locale"></param>
        /// <param Name="selectedTab"></param>
        /// <param Name="modelNumber"></param>
        /// <returns></returns>
        public async Task<JObject> ChangeGroupConfigure(JObject variableAssignments, int groupId, string sessionId
            , string selectedTab, List<DisplayVariableAssignmentsValues> displayVariablesValuesResponse, bool getConflictCache = false)
        {
            var methodBeginTime = Utility.LogBegin();

            var groupContantsDictionary = Utility.GetVariableMapping(Constant.GROUPMAPPERVARIABLES, Constant.GROUPMAPPERCONFIGURATION);

            if (!string.IsNullOrEmpty(selectedTab) && Utility.CheckEquals(selectedTab, Constant.GROUPCONFIGURATION))
            {
                return GenerateGroupConfigurationPopUpResponse(variableAssignments, groupContantsDictionary);
            }

            var selectedTabValue = selectedTab.ToUpper();
            selectedTab = Utility.GetGroupLayoutTab(selectedTabValue);
            var getValues = await GetDefaultValues(sessionId, Constants.DEFAULTBUILDINGCONFIGVALUES, Constant.BUILDINGDEFAULTSCLMCALL).ConfigureAwait(false);
            var configAssignmentsWithoutDefaults = Utility.DeserializeObjectValue<Line>(Utility.SerializeObjectValue(variableAssignments)).VariableAssignments.ToList();
            // remove duplicates 
            var getDefaultValues = (from val1 in getValues
                                    from val2 in configAssignmentsWithoutDefaults
                                    where !string.IsNullOrEmpty(val2.VariableId) && !Utility.CheckEquals(val1.VariableId, val2.VariableId) && !val1.VariableId.Contains("Building_Landing")
                                    select val1).Distinct().ToList();
            foreach (var item in getDefaultValues)
            {
                configAssignmentsWithoutDefaults.Add(item);
            }
            var lineVariables = new Line()
            {
                VariableAssignments = configAssignmentsWithoutDefaults
            };
            var crossPackagevariableAssignments = SetGroupCrossPackage(sessionId, Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(lineVariables)), groupContantsDictionary, selectedTabValue);

            var configureRequest = GenerateDefaultGroupRequest(crossPackagevariableAssignments, selectedTab);

            // get mapping values
            var unitMappingObj = JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES))[Constant.UNITTABLE];
            // setting stub data into an sectionsValues object
            var unitMappingResponse = unitMappingObj.ToObject<Dictionary<string, string>>();

            // variables assginments mapping added for fixing parameters sp missing properties 
            //----------------------------------------------------------------------------
            var varaibelsValuesAdat = configureRequest.Line.VariableAssignments;
            var controlLocationValue = varaibelsValuesAdat.Where(x => x.VariableId.Equals(groupContantsDictionary[Constant.CONTROLLERLOCATION_SP])).ToList();
            if (Utility.CheckEquals(selectedTab.ToUpper(), Constant.GROUPLAYOUTCONFIGURATION.ToUpper()))
            {
                if (controlLocationValue.Any())
                {
                    var valueToSetCache = _cpqCacheManager.GetCache(sessionId, _environment, groupContantsDictionary[Constant.CONTROLLERLOCATION_SP]);
                    if (string.IsNullOrEmpty(valueToSetCache))
                    {
                        var controlLocationCacheValue = controlLocationValue[0].Value;
                        _cpqCacheManager.SetCache(sessionId, _environment, groupContantsDictionary[Constant.CONTROLLERLOCATION_SP], Convert.ToString(controlLocationCacheValue));
                        valueToSetCache = Convert.ToString(controlLocationCacheValue);
                    }
                    List<VariableAssignment> lstVariableAssignment = (List<VariableAssignment>)varaibelsValuesAdat;
                    var valueDtat = varaibelsValuesAdat.ToList();
                    foreach (var itemId in valueDtat)
                    {
                        if (!(Utility.CheckEquals(Convert.ToString(controlLocationValue[0].Value), valueToSetCache)))
                        {
                            if (itemId.VariableId.Contains(groupContantsDictionary[Constant.CONTROLROOMQUADSPVALUES]) || itemId.VariableId.Contains(groupContantsDictionary[Constant.CONTROLFLOOR])
                                || itemId.VariableId.Contains(groupContantsDictionary[Constant.PARMETERSXDIMENSIONVALUES]) ||
                                itemId.VariableId.Contains(groupContantsDictionary[Constant.PARAMETERSYDIMENSIONVALUES]) ||
                                itemId.VariableId.Contains(groupContantsDictionary[Constant.PARAMETERSNXVMDISTANCEFLOOR]))

                            {
                                lstVariableAssignment.Remove(itemId);
                            }
                        }
                        if (Utility.CheckEquals(Convert.ToString(controlLocationValue[0].Value), Constant.JAMB_MOUNTEDVALUE))
                        {
                            if (itemId.VariableId.Contains(groupContantsDictionary[Constant.CONTROLROOMQUADSPVALUES]) || itemId.VariableId.Contains(groupContantsDictionary[Constant.CONTROLFLOOR]))
                            {
                                lstVariableAssignment.Remove(itemId);
                            }
                        }
                        if (!Utility.CheckEquals(Convert.ToString(controlLocationValue[0].Value), Constant.OVERHEAD))
                        {
                            if (itemId.VariableId.Contains(groupContantsDictionary[Constant.PARAMETERSNXVMDISTANCEFLOOR]))
                            {
                                lstVariableAssignment.Remove(itemId);
                            }
                        }
                        if (!Utility.CheckEquals(Convert.ToString(controlLocationValue[0].Value), Constant.CONTROLLOCATIONREMOTE))
                        {
                            if (itemId.VariableId.Contains(groupContantsDictionary[Constant.PARMETERSXDIMENSIONVALUES]) ||
                                itemId.VariableId.Contains(groupContantsDictionary[Constant.PARAMETERSYDIMENSIONVALUES]))
                            {
                                lstVariableAssignment.Remove(itemId);
                            }
                        }
                    }
                    varaibelsValuesAdat = lstVariableAssignment;
                    foreach (var item in varaibelsValuesAdat)
                    {
                        if (item.VariableId.Contains(groupContantsDictionary[Constant.CONTROLFLOOR]))
                        {
                            item.Value = Convert.ToString(item.Value);
                        }
                    }
                    _cpqCacheManager.SetCache(sessionId, _environment, groupContantsDictionary[Constant.CONTROLLERLOCATION_SP], Convert.ToString(controlLocationValue[0].Value));
                    configureRequest.Line.VariableAssignments = varaibelsValuesAdat;
                }
            }
            //---------------------------------------------------------------------------
            var riserVariableData = configureRequest.Line.VariableAssignments.ToList();
            var hallStationsForQuantityParam = JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES))[Constant.HALLSTATIONSFORQUANTITYPARAM].ToList();
            var hallRiserVal = riserVariableData.Where(x => hallStationsForQuantityParam.Contains(x.VariableId) && x.Value.Equals(Constant.TRUEVALUES)).ToList();
            var hallStationQuantity = new VariableAssignment { VariableId = groupContantsDictionary[Constant.NOOFFRONTHALLSTATIONS], Value = hallRiserVal.Count };
            var hallRiserRearVal = riserVariableData.Where(x => x.VariableId.Contains("R_SP")).ToList();

            var mainGroupConfigurationResponse = new ConfigurationResponse
            {
                Sections = new List<Sections>()
            };
            var basesSecctionValues = new List<Sections>();
            var unitMappingListValues = new List<UnitMappingValues>();
            var packagePath = configureRequest?.PackagePath;
            if (!Utility.CheckEquals(selectedTabValue, Constant.FLOORPLANTAB))
            {
                List<VariableAssignment> lstOfVariables = configureRequest.Line.VariableAssignments.ToList();
                var laygrpszValue = (from vars in lstOfVariables
                                     where Regex.IsMatch(vars.VariableId, Constant.PARAMETERS_LAYOUT_BANKTYPE) && vars.Value.Equals(Constant.TRUEVALUES)
                                     select vars).ToList().Count();
                lstOfVariables.Add(new VariableAssignment { VariableId = groupContantsDictionary[Constant.GROUPLAYOUTSIZE], Value = laygrpszValue });
                configureRequest.Line.VariableAssignments = lstOfVariables;
            }
            if (Utility.CheckEquals(selectedTabValue.ToUpper(), Constant.DOORTAB))
            {
                GenerateConfigurationRequestForDoors(configureRequest);
            }
            if (Utility.CheckEquals(selectedTabValue.ToUpper(), Constant.RISERLOCATIONSTAB) && Convert.ToInt32(hallStationQuantity.Value) > 0)
            {
                List<VariableAssignment> lstOfVariables = configureRequest.Line.VariableAssignments.ToList();
                lstOfVariables.Add(hallStationQuantity);
                configureRequest.Line.VariableAssignments = lstOfVariables;
            }
            var variableAssignmentsValues = configureRequest.Line.VariableAssignments;
            var requiredFilterVariables = (from filterValues in variableAssignmentsValues
                                           where !Utility.CheckEquals(filterValues.VariableId, "ELEVATOR001.Parameters.ELEVBASE") && !filterValues.VariableId.Contains("ELEVATOR001.Parameters.IBC")
                                           && !filterValues.VariableId.Contains("Parameters.NOARROW") && !filterValues.VariableId.Contains("Parameters.SUMPTYP")
                                           select filterValues).ToList();
            if (requiredFilterVariables.Any())
            {
                configureRequest.Line.VariableAssignments = requiredFilterVariables;
            }

            var configureResponseJObj =
                await ConfigurationBl(configureRequest, packagePath, sessionId).ConfigureAwait(false);
            // configuration object values 
            var configureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();


            var configureResponseArgument = configureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            // adding defaults to cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTGROUPCONFIGVALUES, Utility.SerializeObjectValue(configureResponseArgumentJObject));
            var configureRequestDictionary = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();
            if (configureRequestDictionary.Any())
            {
                _cpqCacheManager.SetCache(sessionId, _environment, Convert.ToString(groupId), Constant.CURRENTGROUPCONFIGURATION, Utility.SerializeObjectValue(configureRequestDictionary));
            }
            if (configureRequestDictionary.ContainsKey(groupContantsDictionary[Constant.CONTROLLERLOCATION_SP]))
            {
                _cpqCacheManager.SetCache(sessionId, _environment, groupContantsDictionary[Constant.CONTROLLERLOCATION_SP], Convert.ToString(configureRequestDictionary[groupContantsDictionary[Constant.CONTROLLERLOCATION_SP]]));
            }
            var hallstationActualValues = JArray.Parse(JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES))[Constant.HALLSTATIONMAPPEROBJECT].ToString());
            var masterElevatorPosition = hallstationActualValues.ToObject<List<string>>();
            var variable = (from hallstationActualValue in masterElevatorPosition
                            from dictionaryvalue in configureRequestDictionary
                            where Utility.CheckEquals(dictionaryvalue.Key, hallstationActualValue)
                            select new VariableAssignment
                            {
                                VariableId = dictionaryvalue.Key,
                                Value = dictionaryvalue.Value
                            }).Distinct().ToList();
            var requiredValues = GenerateVariableAssignmentsForUnitConfiguration(variable, configureRequest.Line);
            GetCacheVariablesForConflictChanges(requiredValues, sessionId);
            var baseConfigureResponse = configureResponseJObj.Response.ToObject<ConfigurationResponse>();
            var stubGroupConfigurationResponseObj = new ConfigurationResponse();
            // Main Stub 
            var combinedSectionsStubResponse = JObject.Parse(File.ReadAllText(Constant.MAINUIRESPONSETEMPLATE));
            // setting stub data into an sectionsValues object
            var combinedGroupConfigurationMainResponseObj = combinedSectionsStubResponse.ToObject<ConfigurationResponse>();
            switch (selectedTabValue)
            {
                case Constant.FLOORPLANTAB:
                    var stubGroupConfigurationFloorPlanResponse = JObject.Parse(File.ReadAllText(Constant.FLOORPLANUIRESPONSETEMPLATE));
                    // setting stub data into an sectionsValues object
                    stubGroupConfigurationResponseObj = stubGroupConfigurationFloorPlanResponse.ToObject<ConfigurationResponse>();
                    break;
                case Constant.CONTROLROOMTAB:
                    var stubGroupConfigurationControlRoomResponse = JObject.Parse(File.ReadAllText(Constant.CONTROLROOMSUIESPONSE));
                    // setting stub data into an sectionsValues object
                    stubGroupConfigurationResponseObj = stubGroupConfigurationControlRoomResponse.ToObject<ConfigurationResponse>();
                    break;
                case Constant.DOORTAB:
                    var stubGroupConfigurationDoorsResponse = JObject.Parse(File.ReadAllText(Constant.DOORSUITEMPLATE));
                    // setting stub data into an sectionsValues object
                    stubGroupConfigurationResponseObj = stubGroupConfigurationDoorsResponse.ToObject<ConfigurationResponse>();
                    break;
                case Constant.RISERLOCATIONSTAB:
                    var stubGroupConfigurationRiserLocationResponse = JObject.Parse(File.ReadAllText(Constant.RISERLOCATIONSUITEMPLATE));
                    // setting stub data into an sectionsValues object
                    stubGroupConfigurationResponseObj = stubGroupConfigurationRiserLocationResponse.ToObject<ConfigurationResponse>();
                    break;
                // normal start and change configuration type
                case Constant.GROUPCONFIGURATION:
                    var stubGroupConfigurationResponse = JObject.Parse(File.ReadAllText(Constant.STARTGROUPCONFIGURATIONSTUBVALIDATEDATAPATH));
                    // setting stub data into an sectionsValues object
                    stubGroupConfigurationResponseObj = stubGroupConfigurationResponse.ToObject<ConfigurationResponse>();
                    break;
                //Opening location type
                case Constant.OPENINGLOCATION:
                    var openingLocationResponses = JObject.Parse(File.ReadAllText(Constant.PRODUCTSELECTIONUIRESPONSETEMPLATE));
                    // setting stub data into an sectionsValues object
                    stubGroupConfigurationResponseObj = openingLocationResponses.ToObject<ConfigurationResponse>();
                    break;
                //default will be normal configuration type
                default:
                    var stubDefaultGroupConfigurationResponse = JObject.Parse(File.ReadAllText(Constant.FLOORPLANUIRESPONSETEMPLATE));
                    // setting stub data into an sectionsValues object'
                    stubGroupConfigurationResponseObj = stubDefaultGroupConfigurationResponse.ToObject<ConfigurationResponse>();
                    break;
            }

            var elevatorConfigurations = JObject.Parse(File.ReadAllText(Constant.ELEVATORCONFIGURATIONS));
            for (int count = 1; count <= 8; count++)
            {
                var newElevatorConfigurations = JObject.Parse(elevatorConfigurations.ToString().Replace('#', Convert.ToChar(Convert.ToString(count))));
                var elecvatorSections = (Utility.GetTokens("sections", newElevatorConfigurations));
                stubGroupConfigurationResponseObj.Sections.FirstOrDefault().sections.Add(Utility.DeserializeObjectValue<SectionsValues>(elecvatorSections.FirstOrDefault().ToString()));

            }
            //// setting stub data into an sectionsValues object
            //stubGroupConfigurationResponseObj = stubGroupConfigurationResponse.ToObject<ConfigurationResponse>();
            // loing main building response and stub response
            // to add group validation to the main obj 
            var mainFilteredRespone = Utility.DeserializeObjectValue<ConfigurationResponse>(Utility.MapVariables(Utility.SerializeObjectValue(baseConfigureResponse), Utility.SerializeObjectValue(stubGroupConfigurationResponseObj)));
            if (mainFilteredRespone != null)
            {
                //    baseConfigureResponse.Sections = addingGroupValidationValues;
                if (Utility.CheckEquals(selectedTab.ToUpper(), Constant.GROUPLAYOUTCONFIGURATION.ToUpper()))
                {
                    var parameterSPValues = JObject.Parse(File.ReadAllText(Constant.GROUPLAYOUTSTUBFORPARAMETERSSP));
                    var spParametersValues = parameterSPValues.ToObject<Sections>();
                    var lstFloorDesignation = _groupConfiguration.GetFloorDesignationFloorNumberByGroupId(groupId);
                    foreach (var item in mainFilteredRespone.Sections)
                    {
                        if (item.Id != null && Utility.CheckEquals(item.Id.ToUpper(), Constant.CONTROLROOMID))
                        {
                            GenerateVariablesForControlRoom(configureRequest, controlLocationValue, groupContantsDictionary, item, lstFloorDesignation);
                        }
                    }
                }

                if (Utility.CheckEquals(selectedTab.ToUpper(), Constant.GROUPCONFIGURATION))
                {
                    var isGroupDesignationAvaliable = false;
                    foreach (var baseSectionValues in mainFilteredRespone.Sections)
                    {
                        if (Utility.CheckEquals(baseSectionValues.Id, Constant.PARAMETERS))
                        {
                            foreach (var baseConfigurationValues in baseSectionValues.sections)
                            {
                                if (Utility.CheckEquals(baseConfigurationValues.Id, Constant.PARAMETERS))
                                {
                                    var groupDesignValues = (from baseConfigResponse in baseConfigurationValues.sections
                                                             from variablesValues in baseConfigResponse.Variables
                                                             where variablesValues.Id.Contains(Constant.GROUPDESIGNATIONNAME)
                                                             select variablesValues).ToList();
                                    if (groupDesignValues == null || groupDesignValues.Count == 0)
                                    {
                                        isGroupDesignationAvaliable = true;
                                    }
                                }
                            }
                        }
                    }
                    if (isGroupDesignationAvaliable)
                    {
                        var stubGroupDesignationConfigurationResponseObj = new ConfigurationResponse();
                        var stubGroupDesignationConfigurationResponse = JObject.Parse(File.ReadAllText(Constant.GROUPDESGINATIONSTUBRESPONSEPATH));
                        // setting stub data into an sectionsValues object
                        stubGroupDesignationConfigurationResponseObj = stubGroupDesignationConfigurationResponse.ToObject<ConfigurationResponse>();
                        foreach (var baseSectionValues in stubGroupDesignationConfigurationResponseObj.Sections)
                        {
                            if (Utility.CheckEquals(baseSectionValues.Id, Constant.PARAMETERS))
                            {
                                foreach (var configurationValues in baseSectionValues.sections)
                                {
                                    if (Utility.CheckEquals(configurationValues.Id, Constant.PARAMETERS))
                                    {
                                        var groupDesignValues = (from stubConfigResponse in configurationValues.sections
                                                                 from variablesValues in stubConfigResponse.Variables
                                                                 where variablesValues.Id.Contains(Constant.GROUPDESIGNATIONNAME)
                                                                 select variablesValues).ToList();
                                        foreach (var item in groupDesignValues[0].Values)
                                        {
                                            if (Utility.CheckEquals(item.Assigned, Constant.BYUSER_CAMELCASE))
                                            {
                                                var objLine = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString());
                                                var objLineValues = objLine.VariableAssignments.Where(s => s.VariableId.Contains(Constant.PARAMETERSBASICINFOGRPDESG)).ToList();
                                                if (objLineValues != null && objLineValues.Any())
                                                {
                                                    var variableValues = (string)objLineValues[0]?.Value;
                                                    item.Name = variableValues;
                                                    item.value = variableValues;
                                                }
                                            }
                                        }
                                        foreach (var variablesMapping in configurationValues.sections[0].Variables)
                                        {
                                            if (variablesMapping.Id.Contains(Constant.GROUPDESIGNATIONNAME))
                                            {
                                                variablesMapping.Values = groupDesignValues[0].Values;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        baseConfigureResponse = stubGroupDesignationConfigurationResponseObj;
                    }
                }
                else
                {
                    // to filter elevators based on car position values
                    if (Utility.CheckEquals(selectedTab.ToUpper(), Constant.GROUPLAYOUTCONFIGURATION))
                    {
                        var valueData = new List<SectionsValues>();
                        var mainValueData = new List<SectionsValues>();
                        valueData = mainFilteredRespone.Sections.FirstOrDefault().sections.ToList();
                        var values = valueData.Where(i => !i.Id.ToUpper().Contains(Constant.ELEVATOR));
                        mainValueData = values.ToList();
                        foreach (var topItem in valueData)
                        {
                            if (topItem.Id.Contains(Constant.ELEVATOR))
                            {
                                //getting REAR OPEN VALUES
                                GenerateElevatorSectionsForGroup(topItem, mainValueData, unitMappingResponse, unitMappingListValues);
                            }
                        }
                        // adding filter sections values to the main response 
                        mainFilteredRespone.Sections.FirstOrDefault().sections = mainValueData;
                    }
                }
            }
            // getting all the values for Unit table 
            if (Utility.CheckEquals(selectedTab, Constant.GROUPLAYOUTCONFIGURATION) && unitMappingListValues.Any())
            {
                var updatedUnitMappingData = unitMappingListValues;
                var unitTableResponseObj = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(updatedUnitMappingData));
                _cpqCacheManager.SetCache(sessionId, _environment, Constant.UNITTABLEVALUES, Utility.SerializeObjectValue(unitTableResponseObj));
            }

            if (mainFilteredRespone != null)
            {
                basesSecctionValues = mainFilteredRespone.Sections.ToList<Sections>();


                foreach (var itemMainGroupValues in basesSecctionValues)
                {
                    if (Utility.CheckEquals(selectedTab, Constant.GROUPLAYOUTCONFIGURATION) && itemMainGroupValues.sections != null && itemMainGroupValues.sections.Any())
                    {
                        FormattingValuesForGroupResponse(itemMainGroupValues, groupContantsDictionary, selectedTab);
                    }

                    // if as to check group configuration with parameters values and selected tab as group configuration
                    //to add the stub variable of product category
                    // and to get values from SP.
                    var stubReqbody = JObject.Parse(File.ReadAllText(Constant.GROUPCONFIGURATIONPRODUCTCATEGORY));
                    Variables productCategory = stubReqbody.ToObject<Variables>();
                    if (itemMainGroupValues.Id.Contains(Constant.GROUPVALIDATIONPARAMETER, StringComparison.InvariantCultureIgnoreCase) && Utility.CheckEquals(selectedTab, Constant.GROUPCONFIGURATION))
                    {
                        itemMainGroupValues.Variables.Add(productCategory);
                        // add one foreach loop for mainitem variables
                        // in that get count of variable 
                        // add a variable i as 1
                        // add condition to check the i values is less then or equal to mainitem variable count 
                        // in condition to get SP related values based on i as product category Id and do i++ at the end of the condition
                        foreach (var item in itemMainGroupValues.Variables)
                        {
                            var count = item.Values.Count;
                            if (item.Id.Contains("BLANDINGS"))
                            {
                                var variableName = item.Id.Split(Constant.DOTCHAR)[2];
                                item.Id = _groupConfiguration.GetGroupValues(variableName);
                            }
                            else if (item.Id.Contains("GRPDESG"))
                            {
                                var variableName = item.Id.Split(Constant.DOTCHAR)[2];
                                item.Id = _groupConfiguration.GetGroupValues(variableName);
                            }
                            else if (item.Id.Contains("PRODUCTCATEGORY"))
                            {
                                var variableName = item.Id.Split(Constant.DOTCHAR)[3];
                                item.Id = _groupConfiguration.GetGroupValues(variableName);
                            }
                            if (Utility.CheckEquals(item.Id, Constant.PRODUCTCATEGORY))
                            {
                                for (int itemcount = 0; itemcount < count; itemcount++)
                                {
                                    var category = _groupConfiguration.GenerateProductCategory(itemcount);
                                    item.Values[itemcount].value = category;
                                }
                            }
                        }
                    }
                }
            }

            mainGroupConfigurationResponse.Sections = basesSecctionValues;
            var changesValues = CompareChangeInConfigResponse(baseConfigureResponse, Constant.GROUP, configureRequestDictionary);
            mainGroupConfigurationResponse.ConflictAssignments = changesValues;
            var groupResponseObject = new UIMappingBuildingConfigurationResponse();
            //Adding response to main sections response file
            if (Utility.CheckEquals(selectedTab, Constant.GROUPLAYOUTCONFIGURATION))
            {
                foreach (var sectionData in combinedGroupConfigurationMainResponseObj.Sections)
                {
                    if (Utility.CheckEquals(sectionData.Id, selectedTabValue))
                    {
                        var mainSectionValues = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(mainGroupConfigurationResponse.Sections.First().sections));
                        sectionData.sections = Utility.DeserializeObjectValue<IList<SectionsValues>>(Utility.SerializeObjectValue(mainSectionValues));
                        break;

                    }
                }

                groupResponseObject.Sections = combinedGroupConfigurationMainResponseObj.Sections;
            }
            else
            {
                groupResponseObject.Sections = mainGroupConfigurationResponse.Sections;
            }
            if (getConflictCache)
            {
                var internalConflicts = GetCacheValuesForInternalConflicts(sessionId, Constant.GROUP);
                if (internalConflicts != null && internalConflicts.Any())
                {
                    mainGroupConfigurationResponse.ConflictAssignments.PendingAssignments.AddRange(internalConflicts);
                }
            }
            mainGroupConfigurationResponse.ConflictAssignments.PendingAssignments.Distinct();
            mainGroupConfigurationResponse.ConflictAssignments.ResolvedAssignments.Distinct();
            groupResponseObject.ConflictAssignments = mainGroupConfigurationResponse.ConflictAssignments;
            // adding the conflicts to the cachce as pervious conflicts
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.PREVIOUSGROUPCONFLICTSVALUES, Utility.SerializeObjectValue(mainGroupConfigurationResponse.ConflictAssignments));
            if (Utility.CheckEquals(selectedTab, Constant.GROUPLAYOUTCONFIGURATION))
            {
                var datasdflow = JObject.Parse(JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES))[Constant.FLOORPLANRULESMAPPEROBJECT].ToString());
                groupResponseObject.FloorPlanRules = datasdflow;
                groupResponseObject.DisplayVariableAssignments = new List<DisplayVariableAssignmentsValues>();
                if (displayVariablesValuesResponse != null && displayVariablesValuesResponse.Any())
                {
                    groupResponseObject.DisplayVariableAssignments = displayVariablesValuesResponse;
                }
                else
                {
                    var displayLocations = JObject.Parse(File.ReadAllText(Constant.DISPLAYLOCATIONSGROUPLAYOUT));
                    var displayLocationsValues = Utility.DeserializeObjectValue<List<DisplayVariableAssignmentsValues>>(Utility.SerializeObjectValue(displayLocations["displayVariableAssignments"]));
                    groupResponseObject.DisplayVariableAssignments = displayLocationsValues;
                }
            }
            var setDisplayVariableAssignmentsToCache = GetSetDisplayVariableAssignmentsForGroup(groupResponseObject.DisplayVariableAssignments, groupId, sessionId);
            var editFlagCache = _cpqCacheManager.GetCache(sessionId, _environment, groupId.ToString(), Constants.EDITFLAGFORGROUP);
            if (!String.IsNullOrEmpty(editFlagCache))
            {
                groupResponseObject.IsEditFlow = (Utility.DeserializeObjectValue<Int32>(editFlagCache)) > 0 ? true : false;
            }
            var entranceExistsCache = _cpqCacheManager.GetCache(sessionId, _environment, groupId.ToString(), "EntrancesExist");
            if (!String.IsNullOrEmpty(entranceExistsCache))
            {
                groupResponseObject.EntranceConfigurationExists = (Utility.DeserializeObjectValue<Int32>(entranceExistsCache)) > 0 ? true : false;
            }
            var conflictResponse = _cpqCacheManager.GetCache(sessionId, _environment, Constant.INTERNALPREVIOUSGROUPCONFLICTSVALUES);
            if (!string.IsNullOrEmpty(conflictResponse))
            {
                var cacheConflitsResponse = Utility.DeserializeObjectValue<List<ConflictMgmtList>>(conflictResponse);
                if (groupResponseObject.ConflictAssignments != null && groupResponseObject.ConflictAssignments.PendingAssignments != null)
                {
                    groupResponseObject.ConflictAssignments.PendingAssignments.AddRange(cacheConflitsResponse);
                }
                else
                {
                    groupResponseObject.ConflictAssignments = new ConflictManagement()
                    {
                        PendingAssignments = cacheConflitsResponse,
                        ResolvedAssignments = new List<ConflictMgmtList>()
                    };
                }
                var filteredPendingConflictsVariables = groupResponseObject.ConflictAssignments.PendingAssignments.GroupBy(x => x.VariableId).Select(s => s.FirstOrDefault()).ToList();
                if (filteredPendingConflictsVariables != null && filteredPendingConflictsVariables.Any())
                {
                    groupResponseObject.ConflictAssignments.PendingAssignments = filteredPendingConflictsVariables;
                }
            }
            UpdateCacheWithAutoResolvedConflicts(groupResponseObject, variableAssignments, sessionId, selectedTabValue, groupContantsDictionary);
            Utility.LogEnd(methodBeginTime);
            return Utility.FilterNullValues(groupResponseObject);
        }



        /// <summary>
        /// Change UnitConfiguration BL
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="selectedTab"></param>
        /// <returns></returns>
        public async Task<JObject> ChangeUnitConfigureBl(JObject variableAssignments, string sessionId, string sectionTab, int setId, int unitId = 0)
        {
            var methodBeginTime = Utility.LogBegin();
            var unitMapperVariables = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.UNITCOMMONMAPPER);
            var unitMapperVariablesCabInterior = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.CABINTERIORMAPPER);
            var unitMapperVariablesOtherEquipment = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constants.OTHEREQUIPMENT);
            var unitMapperVariablesGeneralInformation = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.GENERALINFOMAPPER);
            var variableRearOpen = SetCacheRearOpenForUnitSet(null, sessionId, setId);

            //deriving groupid and the corresponding list of units from setId
            var dataSetForgroupIdBySetid = _unitConfigurationDL.GetTravelValue(setId);
            var derivedGroupIdBySetId = dataSetForgroupIdBySetid.GroupId;
            var lstunits = _unitConfigurationDL.GetUnitsByGroupId(derivedGroupIdBySetId, setId);
            List<VariableAssignment> lstvariableassignment = null;
            if (variableRearOpen != null && variableRearOpen.Any())
            {
                lstvariableassignment = variableRearOpen.Select(
                     variableAssignment => new VariableAssignment
                     {
                         VariableId = variableAssignment.VariableId,
                         Value = variableAssignment.Value
                     }).ToList<VariableAssignment>();
            }
            //getting cached variable assignments
            var cachedData = GetCrosspackageVariableAssignments(sessionId, string.Concat(Constant.UNITCONFIGURATION,setId));

            var crossPackagevariableAssignments = new List<VariableAssignment>();
            if (!string.IsNullOrEmpty(cachedData))
            {
                crossPackagevariableAssignments = Utility.DeserializeObjectValue<List<VariableAssignment>>(cachedData);
            }

            var assignments = Utility.DeserializeObjectValue<Line>(Utility.SerializeObjectValue(variableAssignments));
            if (sectionTab.Equals(Constants.TRACTIONHOISTWAYEQUIPMENT))
            {
                var additionalWiring = _cpqCacheManager.GetCache(sessionId, _environment, setId.ToString(), Constants.SETEXCEPTIONWIRINGDATA);
                var hoistwayCache = _cpqCacheManager.GetCache(sessionId, _environment, setId.ToString(), Constants.SETEXCEPTIONCARPOSITION);
                var additonalWiringCache = new List<UnitVariables>();
                if (additionalWiring != null)
                {
                    additonalWiringCache = Utility.DeserializeObjectValue<List<UnitVariables>>(additionalWiring);
                }
                else
                {
                    additonalWiringCache = new List<UnitVariables> { new UnitVariables { unitId = unitId, Value = 0, VariableId = Constants.UNITSINSET } };
                }
                var crossPackagevariableAssignmentsVal = Utility.DeserializeObjectValue<List<UnitVariables>>(hoistwayCache);
                var val = (from valD in crossPackagevariableAssignmentsVal where valD.unitId.Equals(unitId) select valD.MappedLocation)?.FirstOrDefault();
                var wiringFlag = assignments.VariableAssignments.Where(x => x.VariableId.Contains(Constants.UNITSINSET)).ToList().Count > 0 ? true : false;
                if (val != null)
                {
                    if (wiringFlag)
                    {
                        foreach (var valData in crossPackagevariableAssignments)
                        {
                            if (valData.VariableId.Contains(Constants.ADDITIONALWIRINGVALUE))
                            {
                                var wiringForUnit = (from data in additonalWiringCache where data.unitId.Equals(unitId) select data.Value);
                                if (wiringForUnit.Count() > 0)
                                {
                                    valData.Value = Convert.ToInt32(wiringForUnit.FirstOrDefault()) * 12;
                                }
                                else
                                {
                                    valData.Value = 0;
                                }
                            }
                            if (valData.VariableId.Contains(Constants.CARPOS))
                            {
                                valData.Value = val;
                            }
                            if (Regex.IsMatch(valData.VariableId, Constant.PARAMETERS_LAYOUT_BANKTYPE) && valData.VariableId.Contains(val.ToString()))
                            {
                                valData.Value = true;
                            }
                            else if (Regex.IsMatch(valData.VariableId, Constant.PARAMETERS_LAYOUT_BANKTYPE))
                            {
                                valData.Value = false;
                            }
                        }
                    }
                }
            }
            if (assignments != null)
            {
                foreach (var additionWiring in assignments.VariableAssignments)
                {
                    if (additionWiring != null)
                    {
                        if (additionWiring.VariableId.Equals(unitMapperVariablesOtherEquipment[Constants.ADDITIONALWIRING]))
                        {
                            additionWiring.Value = Convert.ToDouble(additionWiring.Value) * 12;
                            foreach (var additionWiringVal in assignments.VariableAssignments)
                            {
                                if (additionWiringVal.VariableId.Equals(Constants.UNITSINSET))
                                {
                                    assignments.VariableAssignments = assignments.VariableAssignments.Where(x => !x.VariableId.Equals(unitMapperVariablesOtherEquipment[Constants.ADDITIONALWIRING])).ToList();
                                }
                            }
                        }
                    }
                }
            }
            var getValues = await GetDefaultValues(sessionId, Constants.DEFAULTGROUPCONFIGVALUES, Constant.GROUPCONFIGURATIONNAME).ConfigureAwait(false);
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
            assignments.VariableAssignments = configAssignmentsWithDefaults;
            //generate cross package variable assignments
            crossPackagevariableAssignments = GenerateVariableAssignmentsForUnitConfiguration(crossPackagevariableAssignments, assignments);
            var variableAssignmentz = new Line();
            //Remove hoistwaydimension variable if the min or max is selected
            var stubRangeValues = JObject.Parse(File.ReadAllText(Constant.RANGEJOBJECT));
            var rangeValuesList = stubRangeValues[Constant.RANGEVALUES].ToList();
            var hoistway = crossPackagevariableAssignments.Where(x => x.VariableId.Contains(Constant.HOISTWAYDIMENSIONS)).ToList();
            if (hoistway.Count > 0)
            {
                var valdimension = Convert.ToString(hoistway.FirstOrDefault().Value);
                if (Utility.CheckEquals(valdimension, Constant.MINIMUM) || Utility.CheckEquals(valdimension, Constant.MINIMUM))
                {
                    crossPackagevariableAssignments = crossPackagevariableAssignments.Where(x => !rangeValuesList.Contains(x.VariableId)).ToList();
                }
            }
            variableAssignmentz.VariableAssignments = crossPackagevariableAssignments;
            SetCrosspackageVariableAssignments(crossPackagevariableAssignments, sessionId,string.Concat(Constant.UNITCONFIGURATION,setId));
            variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(variableAssignmentz));
            var productType = SetCacheProductType(null, sessionId, setId).FirstOrDefault().Value.ToString();
            var configureRequest = CreateConfigurationRequestWithTemplate(variableAssignments, Constant.UNITNAME, lstvariableassignment, productType);
            var mainGroupConfigurationResponse = new ConfigurationResponse
            {
                Sections = new List<Sections>()
            };
            var packagePath = configureRequest?.PackagePath;
            // needed code pls don't remove it
            if (configureRequest == null)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = _localizer[Constant.REQUESTCANNOTBENULL + Constant.HYPHEN]
                });
            }
            // adding required include section values
            configureRequest = GenerateIncludeSections(configureRequest, Constant.UNITCONFIGURATION, sectionTab, productType);
            // hoistway values
            var hoidtwayDimensionData = configureRequest.Line.VariableAssignments.Where(x => x.VariableId.Equals(unitMapperVariablesGeneralInformation[Constant.HOISTWAYDIMENSIONVARIABLE])).ToList();
            if (hoidtwayDimensionData != null && hoidtwayDimensionData.Any())
            {
                var travelVariableValues = _unitConfigurationDL.GetTravelValue(setId);
                if (travelVariableValues.TravelVariableAssignments.VariableId != null)
                {
                    var filteredVariablesData = configureRequest.Line.VariableAssignments.ToList();
                    var getTravelValues = new VariableAssignment()
                    {
                        VariableId = unitMapperVariables[Constant.TRAVELVARIABLEIDVALUE],
                        Value = travelVariableValues.TravelVariableAssignments.Value,
                    };
                    filteredVariablesData.Add(getTravelValues);
                    configureRequest.Line.VariableAssignments = filteredVariablesData;
                }
            }

            //Adding include sections in request body
            var configureResponseJObj =
                await ConfigurationBl(configureRequest, packagePath, sessionId).ConfigureAwait(false);
            var configureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
            List<Compartment> lstcompartments = new List<Compartment>();
            var stubCarFixtureCompartmentResponseObj = new CompartmentsData();
            var baseConfigureResponse = configureResponseJObj.Response.ToObject<ConfigurationResponse>();
            // configuration object values for conflict mapping
            var ActualConfigureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = ActualConfigureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            // adding defaults to cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTUNITCONFIGVALUES, Utility.SerializeObjectValue(configureResponseArgumentJObject));
            var configureRequestDictionary = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();
            var stubUnitConfigurationResponseObj = new ConfigurationResponse();
            var stubUnitConfigurationSubSectionResponseObj = new ConfigurationResponse();
            var stubUnitConfigurationMainResponseObj = new ConfigurationResponse();
            // Main Stub 
            var stubMainSubSectionResponse = JObject.Parse(File.ReadAllText(Constant.UNITTUIRESPONSETEMPLATE));
            // setting stub data into an sectionsValues object
            stubUnitConfigurationMainResponseObj = stubMainSubSectionResponse.ToObject<ConfigurationResponse>();
            // to select required configurationtype
            if (string.IsNullOrEmpty(sectionTab))
            {
                sectionTab = Constant.GENERALINFORMATION;
            }
            var sectionTabValue = sectionTab.ToUpper();
            //Entarnce Console configuratioon fetching from cache
            string currentProductType; //= productType.Equals(Constant.MODEL_EVO200) ? Constant.EVOLUTION200 : Constant.END100;
            switch (productType)
            {
                //Evolution200
                case Constant.MODEL_EVO200:
                    currentProductType = Constant.EVOLUTION200;
                    break;
                //Evolution100
                case Constant.MODEL_EVO100:
                    currentProductType = Constant.EVOLUTION__100;
                    break;
                //Endura100
                case Constant.ENDURA_100:
                    currentProductType = Constant.END100;
                    break;
                //default Evolution200
                default:
                    currentProductType = Constant.EVOLUTION200;
                    break;
            }
            var enrichTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, currentProductType)));
            switch (sectionTabValue)
            {
                // GeneralInformation
                case Constant.GENERALINFORMATION:
                    stubUnitConfigurationSubSectionResponseObj = GeneralInformationUnitConfigureBl(configureRequest, currentProductType, baseConfigureResponse, rangeValuesList);
                    break;
                // Cab Interior
                case Constant.CABINTERIOR:
                    stubUnitConfigurationSubSectionResponseObj = CabInteriorUnitConfigureBl(currentProductType, baseConfigureResponse, crossPackagevariableAssignments);
                    break;
                // Hoistway traction
                case Constant.HOISTWAYTRACTIONEQUIPMENT:
                    stubUnitConfigurationSubSectionResponseObj = OtherEquipmentUnitConfigureBL(productType, baseConfigureResponse);
                    break;
                // Entrance
                case Constant.ENTRANCES:
                    var entranceConsoles = SetCacheEntranceConsoles(null, sessionId, setId);
                    stubUnitConfigurationSubSectionResponseObj = EntrancesUnitConfigureBL(productType, baseConfigureResponse, entranceConsoles);
                    break;
                // CarFixtures
                case Constant.CARFIXTURE:

                    var fixtureStrategy = (from var in configureRequest.Line.VariableAssignments
                                           where var.VariableId.Equals(Constant.ELEVATORFIXTURESTRATEGY)
                                           select var.Value).ToList();

                    stubUnitConfigurationSubSectionResponseObj = await CarFixtureChangeUnitConfigureBL(sessionId, fixtureStrategy, productType, currentProductType, baseConfigureResponse,
                        configureRequest, setId, sectionTab, configureResponseJObj, stubUnitConfigurationMainResponseObj);

                    break;
                //default will be normal configuration type
                default:
                    var stubGeralInformationdefaultResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.GENERALINFORMATIONTEMPLATE, Constant.EVOLUTION200)));
                    //Sub Section Stub Path
                    var stubGeralInformationSubSectiondefaultResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.EVOLUTION200)));
                    if (Utility.CheckEquals(productType, Constant.ENDURA_100))
                    {
                        stubGeralInformationdefaultResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.GENERALINFORMATIONTEMPLATE, Constant.END100)));
                        stubGeralInformationSubSectiondefaultResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.END100)));
                    }
                    if (Utility.CheckEquals(productType, Constant.MODEL_EVO100))
                    {
                        stubGeralInformationdefaultResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.GENERALINFORMATIONTEMPLATE, Constant.EVOLUTION__100)));
                        stubGeralInformationSubSectiondefaultResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.EVOLUTION__100)));
                    }
                    // setting stub data into an sectionsValues object
                    stubUnitConfigurationResponseObj = stubGeralInformationdefaultResponse.ToObject<ConfigurationResponse>();
                    //Sub Sections Stub Path
                    stubUnitConfigurationSubSectionResponseObj = stubGeralInformationSubSectiondefaultResponse.ToObject<ConfigurationResponse>();
                    break;
            }

            List<VariableAssignment> varAssigns = new List<VariableAssignment>();
            foreach (var subSectionDetails in stubUnitConfigurationSubSectionResponseObj.Sections)
            {
                foreach (var subsection in subSectionDetails.sections)
                {
                    if (subsection.Id.Equals(Constants.OTHEREQUIPVALUE))
                    {
                        var lowestUnitId = 0;
                        if (unitId > 0)
                        {
                            lowestUnitId = unitId;
                        }
                        else
                        {
                            lowestUnitId = Convert.ToInt32((from openingData in lstunits select openingData?.Unitid)?.ToList()?.Min());
                        }
                        var values = new List<Values>();
                        foreach (var units in lstunits)
                        {
                            values.Add(new Values { value = units.Unitname, id = units.Unitid.ToString(), Name = units.Unitname, State = Constants.AVAILABLE, InCompatible = false, Assigned = lowestUnitId == units.Unitid ? Constants.BYUSER_LOWERCASE : null });
                        }
                        subsection.Variables.Add(new Variables { Id = Constants.UNITSINSET, Name = Constants.UNITSINSET, Values = values, ValueType = Constants.STRING });
                        if (Utility.CheckEquals(currentProductType, Constant.END100) || Utility.CheckEquals(currentProductType, Constant.EVOLUTION__100))
                        {
                            var variables = new List<Variables> { new Variables { Id = unitMapperVariablesOtherEquipment[Constants.ADDITIONALWIRING], Name =unitMapperVariablesOtherEquipment[Constants.ADDITIONALWIRING], ValueType = Constants.STRING,Values=new List<Values>
                            { new Values { value= currentProductType==Constant.END100?20:40,
                            Type=Constants.SINGLETONVALUE,InCompatible=true,Assigned=Constant.BYUSER_LOWERCASE,State=Constants.SELECTED,Justification=Constants.None}} } };
                            subsection.Variables = variables;
                        }
                    }
                    if (configureRequest.Line.VariableAssignments != null && configureRequest.Line.VariableAssignments.Any())
                    {

                        // getting values of hoist way
                        var hoistwayDimensionValues = configureRequest.Line.VariableAssignments.Where(x => x.VariableId.Contains(unitMapperVariablesGeneralInformation[Constant.HOISTWAYDIMENSIONVALUE])).ToList();
                        string customValues = null;
                        if (hoistwayDimensionValues != null && hoidtwayDimensionData.Any())
                        {
                            customValues = hoistwayDimensionValues?.FirstOrDefault().Value.ToString();
                        }
                        Object dataVal = null;
                        var vfdValue = (from variablesItem in subsection.Variables
                                        where variablesItem.Id.Equals(unitMapperVariablesOtherEquipment[Constants.VFDWIRING])
                                        select variablesItem?.Values)?.FirstOrDefault();
                        if(vfdValue!= null)
                        {
                            dataVal = (from variablesItem in subsection.Variables where variablesItem.Id.Equals(unitMapperVariablesOtherEquipment[Constants.VFDWIRING]) from variableData in variablesItem?.Values where variableData.Type.Equals(Constant.SINGLETONVALUE) && variableData.InCompatible.Equals(false) select variableData?.value)?.FirstOrDefault();
                        }
                        foreach (var variablesItem in subsection.Variables)
                        {
                            if (Utility.CheckEquals(variablesItem.Id, unitMapperVariablesCabInterior[Constant.BUMPERHEIGHT]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesCabInterior[Constants.HANDRAILHEIGHT]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesCabInterior[Constants.OTHERWEIGHT]))
                            {
                                if (variablesItem.Values != null)
                                {
                                    var minDatas = variablesItem.Values.Where(x => x.lower != null && x.InCompatible.Equals(false) && x.State.Equals(Constant.AVAILABLE))?.FirstOrDefault()?.lower;
                                    var maxDatas = variablesItem.Values.Where(x => x.upper != null && x.InCompatible.Equals(false) && x.State.Equals(Constant.AVAILABLE))?.LastOrDefault()?.upper;
                                    var getPropertiesVal = JArray.Parse(File.ReadAllText(string.Format(Constant.PROPERTYTEMPLATE, currentProductType)));
                                    var variableProperties = Utility.DeserializeObjectValue<IList<Properties>>(Utility.SerializeObjectValue(getPropertiesVal));
                                    if (variableProperties.Any())
                                    {
                                        variablesItem.Properties = variableProperties;
                                    }
                                    if (!variablesItem.Id.Equals(unitMapperVariablesCabInterior[Constants.OTHERWEIGHT]))
                                    {
                                        foreach (var item in variablesItem.Properties)
                                        {
                                            item.Value = GetPropertyValueForHeight(item.Id, minDatas, maxDatas, false);
                                        }
                                        if (productType.Equals(Constants.ENDURA_100) && variablesItem.Id.Equals(unitMapperVariablesCabInterior[Constants.HANDRAILHEIGHT]))
                                        {
                                            var minVal = (from val in variablesItem.Properties where val.Id.Equals(Constant.MINVALUE) select val)?.FirstOrDefault();
                                            variablesItem.Properties.Remove(minVal);
                                            var maxVal = (from val in variablesItem.Properties where val.Id.Equals(Constant.MAXVALUE) select val)?.FirstOrDefault();
                                            variablesItem.Properties.Remove(maxVal);
                                        }
                                    }
                                    else
                                    {
                                        foreach (var item in variablesItem.Properties)
                                        {
                                            item.Value = GetPropertyValueForHeight(item.Id, minDatas, maxDatas, Constants.OTHERWEIGHT);
                                        }
                                    }
                                }
                            }
                            if (Utility.CheckEquals(variablesItem.Id, unitMapperVariablesOtherEquipment[Constants.VFDWIRING]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesOtherEquipment[Constants.ADDITIONALWIRING]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesOtherEquipment[Constants.TOTALWIRINGS]))
                            {
                                var data = variablesItem.Values?.Where(x => x.Type.Equals(Constant.SINGLETONVALUE) && x.InCompatible.Equals(false))?.FirstOrDefault();
                                if (data != null)
                                {
                                    data.value = Convert.ToInt32(data.value) / 12;
                                }
                                if ((dataVal != null) && 
                                    (variablesItem.Id.Equals(unitMapperVariablesOtherEquipment[Constants.ADDITIONALWIRING]) 
                                    || variablesItem.Id.Equals(unitMapperVariablesOtherEquipment[Constants.TOTALWIRING])))
                                {
                                    GetRangeValidationForHoistway(variablesItem, dataVal);
                                }
                            }
                            if (Utility.CheckEquals(variablesItem.Id, unitMapperVariablesOtherEquipment[Constants.COUNTERWEIGHTSAFETY]))
                            {
                                var result = await GetDataToCheckOccupiedSpace(derivedGroupIdBySetId, variableAssignments, sessionId);
                                SetIncompatibilityForCWSFTY(result, variablesItem, lstunits);
                            }
                            if (Utility.CheckEquals(variablesItem.Id, unitMapperVariablesGeneralInformation[Constants.FRONTDOORHEIGHTMAP]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesGeneralInformation[Constants.FRONTDOORWIDTHMAP]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesGeneralInformation[Constants.FRONTDOORTYPEMAP]))
                            {
                                variablesItem.Properties = AddDynamicLayout(stubUnitConfigurationSubSectionResponseObj, variablesItem);
                            }
                            foreach (var rangeItem in rangeValuesList)
                            {
                                if (Utility.CheckEquals(variablesItem.Id, rangeItem.ToString()))
                                {
                                    // getting min and max values 
                                    object minData = null;
                                    object maxData = null;
                                    minData = variablesItem.Values.Where(x => x.lower != null && x.InCompatible.Equals(false) && x.State.Equals(Constant.AVAILABLE))?.FirstOrDefault()?.lower;
                                    maxData = variablesItem.Values.Where(x => x.upper != null && x.InCompatible.Equals(false) && x.State.Equals(Constant.AVAILABLE))?.LastOrDefault()?.upper;
                                    if (Utility.CheckEquals(productType,Constants.ENDURA_100) && variablesItem.Id.Contains(Constants.PITDEPTHVALUE))
                                    {
                                        minData = variablesItem.Values.Select(x => x.value)?.FirstOrDefault()?.ToString();
                                        maxData = variablesItem.Values.Select(x => x.value)?.LastOrDefault()?.ToString();
                                        if (customValues != null && !Utility.CheckEquals(customValues, Constant.CUSTOM))
                                        {
                                            var userData = variablesItem.Values.Where(x => x.Assigned == Constant.BYUSER_CAMELCASE)?.FirstOrDefault();
                                            if (userData!=null)
                                            {
                                                variablesItem.Values.Remove(userData);
                                            }
                                        }
                                    }
                                    var assignedValue = variablesItem.Values.Where(x => x.Assigned != null && x.Assigned.Equals(Constant.BYRULE_CAMELCASE)).ToList();
                                    object value = string.Empty;
                                    if (customValues == null || customValues == Constant.MINIMUM)
                                    {
                                        value = minData;
                                    }
                                    else
                                    {
                                        value = maxData;
                                    }
                                    if (assignedValue.Count.Equals(0))
                                    {
                                        if (customValues != Constant.CUSTOM)
                                        {
                                            variablesItem.Values.Add(CreateSingletonValue(value));
                                        }
                                    }
                                    else
                                    {
                                        minData = minData == null ? assignedValue[0].value : minData;
                                        maxData = maxData == null ? assignedValue[0].value : maxData;
                                    }
                                    var getProperties = JArray.Parse(File.ReadAllText(string.Format(Constant.PROPERTYTEMPLATE, currentProductType)));
                                    var variableProperties = Utility.DeserializeObjectValue<IList<Properties>>(Utility.SerializeObjectValue(getProperties));
                                    if (variableProperties.Any())
                                    {
                                        variablesItem.Properties = variableProperties;
                                    }
                                    string valueData = null;
                                    if (customValues == Constant.MAXIMUM)
                                    {
                                        valueData = Constant.MAXIMUM;
                                    }
                                    else if (customValues == Constant.CUSTOM)
                                    {
                                        valueData = Constant.CUSTOM;
                                    }
                                    else
                                    {
                                        valueData = Constant.MINIMUM;
                                    }
                                    foreach (var item in variablesItem.Properties)
                                    {
                                        if (!String.IsNullOrEmpty(customValues))
                                        {
                                            if (customValues.Equals(Constant.CUSTOM))
                                            {
                                                if (minData == null || maxData == null)
                                                {
                                                    var assignedByUserValue = variablesItem.Values.Where(x => x.Assigned != null && x.Assigned.Equals(Constant.BYRULE_CAMELCASE)).ToList();
                                                    if (assignedByUserValue.Count.Equals(0))
                                                    {
                                                        assignedByUserValue = variablesItem.Values.Where(x => x.Assigned != null && (x.Assigned.Equals(Constant.BYUSER_CAMELCASE) || Utility.CheckEquals(x.Assigned, Constant.BYDEFAULT_CAMELCASE))).ToList();
                                                    }
                                                    if (assignedByUserValue.Count > 0)
                                                    {
                                                        minData = minData == null ? assignedByUserValue[0]?.value : minData;
                                                        maxData = maxData == null ? assignedByUserValue[0]?.value : maxData;
                                                    }
                                                }
                                            }
                                        }
                                        // if it is custom selection we will map the min, max and is range flag to set the range
                                        item.Value = GetPropertyValue(item.Id, minData, maxData, valueData);

                                    }

                                    if (productType.Equals(Constants.ENDURA_100) && customValues != null && customValues.Equals(Constant.CUSTOM) && variablesItem.Id.Contains(Constants.PITDEPTHVALUE))
                                    {
                                        variablesItem.Properties = GetPropertyForCustomPitDepth();
                                    }
                                    var requiredValue = (from assignedValues in variablesItem.Values where assignedValues.Assigned != null select assignedValues.value).ToList().FirstOrDefault();
                                    VariableAssignment assignings = new VariableAssignment
                                    {
                                        VariableId = variablesItem.Id,
                                        Value = requiredValue == null ? string.Empty : requiredValue
                                    };
                                    varAssigns.Add(assignings);
                                }
                            }


                        }
                    }
                    // To set Cacacity,CarSpeed Values in Cache
                    foreach (var subsectionValue in subsection.Variables)
                    {
                        if (subsectionValue.Id.Contains(Constant.CARSPEEDVARIABLE) || subsectionValue.Id.Contains(Constant.CAPACITY)||subsectionValue.Id.Contains(unitMapperVariablesGeneralInformation[Constants.ENDURACARSPEEDVARIABLE]))
                        {
                            var requiredValue = (from assignedValues in subsectionValue.Values
                                                 where assignedValues.Assigned != null
                                                 select assignedValues.value).ToList().FirstOrDefault();

                            VariableAssignment assignings = new VariableAssignment
                            {
                                VariableId = subsectionValue.Id== unitMapperVariablesGeneralInformation[Constants.ENDURACARSPEEDVARIABLE] ? unitMapperVariablesGeneralInformation[Constants.SPEED] : subsectionValue.Id,
                                Value = requiredValue == null ? string.Empty : requiredValue
                            };
                            varAssigns.Add(assignings);
                        }
                    }
                }
            }

            if (varAssigns != null && varAssigns.Any())
            {
                var hoistwaycache = SetCacheHoistwayDimensions(varAssigns, sessionId, setId);
            }

            mainGroupConfigurationResponse.Sections = stubUnitConfigurationSubSectionResponseObj.Sections;
            foreach (var items in stubUnitConfigurationMainResponseObj.Sections)
            {
                if (items.Id.ToUpper() == sectionTab.ToString().ToUpper())
                {
                    var mainSectionValues = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(mainGroupConfigurationResponse.Sections.First().sections));
                    items.sections = Utility.DeserializeObjectValue<IList<SectionsValues>>(Utility.SerializeObjectValue(mainSectionValues));
                    break;
                }
            }
            // added conflicts 
            var conflictChangesValues = CompareChangeInConfigResponse(baseConfigureResponse, Constant.UNITCONFIGURATION, configureRequestDictionary);
            stubUnitConfigurationMainResponseObj.ConflictAssignments = conflictChangesValues;
            // flag values
            stubUnitConfigurationMainResponseObj.ConfigurationStatus = Constant.ISCOMPLETED;
            stubUnitConfigurationMainResponseObj.SystemValidateStatus = new Status()
            {
                StatusKey = Constant.STATUSKEYUNIT_INC,
                StatusName = Constant.INCOMPLETESTATUS,
                DisplayName = Constant.INCOMPLETESTATUS,
                Description = Constant.INCOMPLETEDESCRIPTIONSTATUS
            };
            stubUnitConfigurationMainResponseObj.ConfiguratorStatus = new Status()
            {
                StatusKey = Constant.STATUSKEYUNIT_INC,
                StatusName = Constant.INCOMPLETESTATUS,
                DisplayName = Constant.INCOMPLETESTATUS,
                Description = Constant.INCOMPLETEDESCRIPTIONSTATUS
            };

            var getData = (from val1 in configureRequestDictionary
                           where (val1.Key.ToString().Contains("CAPACITY") || val1.Key.ToString().Contains("CARSPEED"))
                           select new VariableAssignment
                           {
                               VariableId = val1.Key,
                               Value = val1.Value
                           }).Distinct().ToList();
            foreach (var a in getData)
            {
                if (a.VariableId.Contains("CAPACITY_INT"))
                {
                    getData = getData.Where(x => !x.VariableId.Contains("CAPACITY_INT")).ToList();
                }
            }
            var lineValue = new Line
            {
                VariableAssignments = getData
            };
            var UpdateCrossPackagevariableAssignments = GenerateVariableAssignmentsForUnitConfiguration(crossPackagevariableAssignments, lineValue);
            UpdateCrossPackagevariableAssignments.Distinct();
            SetCrosspackageVariableAssignments(UpdateCrossPackagevariableAssignments, sessionId, string.Concat(Constant.UNITCONFIGURATION, setId));
            // adding the conflicts to the cachce as pervious conflicts
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.PREVIOUSUNITCONFLICTSVALUES, Utility.SerializeObjectValue(conflictChangesValues));
            GetConflictCacheValues(sessionId, stubUnitConfigurationMainResponseObj);
            Utility.LogEnd(methodBeginTime);
            return Utility.FilterNullValues(stubUnitConfigurationMainResponseObj);
        }

        public static List<string> AddOptionList(List<IEnumerable<Property>> optionproperties, List<string> optionList, List<string> variableList)
        {
            var methodBeginTime = Utility.LogBegin();
            if (optionproperties.Count == 0)
            {
                return optionList;
            }
            optionproperties.ForEach(properties =>
            {
                foreach (var property in properties)
                {
                    if (property != null)
                    {
                        foreach (var option in property.Value.ToString()
                            .Split(Constant.COMMACHAR, Constant.EQUALTOCHAR)
                            .ToList().Where((c, i) => i % 2 != 0).ToList())
                        {
                            if (!optionList.Contains(option))
                            {
                                optionList.Add(option);
                            }
                        }
                        variableList?.AddRange(property.Value.ToString().Split(Constant.COMMACHAR, Constant.EQUALTOCHAR)
                            .ToList().Where((c, i) => i % 2 == 0).ToList());
                    }
                }
            }
            );
            Utility.LogEnd(methodBeginTime);
            return optionList;
        }

        /// <summary>
        /// Assigning basket property to Basket variables
        /// </summary>
        /// <param Name="sections"></param>
        /// <param Name="optionList"></param>
        /// <param Name="variableList"></param>
        public static void AssignBasketProperty(IReadOnlyList<Section> sections, List<string> optionList,
            List<string> variableList)
        {
            var methodBeginTime = Utility.LogBegin();
            var basketProp = new Property
            {
                Id = Constant.BASKETASSIGNEDPROP,
                Value = true,
                Type = PropertyType.Boolean
            };
            int i = 0;
            if (variableList?.Count > 0)
            {
                foreach (string variable in variableList)
                {
                    var propObj = (from groups in sections
                                   from variables in groups.Variables
                                   where variables.Id.Split(Constant.DOTCHAR).Last() == variable
                                   from values in variables.Values
                                   select values).OfType<SingletonValue>().ToList()
                        .First(p => p.Value.ToString() == optionList[i])
                        .Properties.ToList();
                    propObj.Add(basketProp);
                    (from groups in sections
                     from variables in groups.Variables
                     where variables.Id.Split(Constant.DOTCHAR).Last() == variable
                     from values in variables.Values
                     select values).OfType<SingletonValue>().ToList()
                        .First(p => p.Value.ToString() == optionList[i])
                        .Properties = propObj;
                    i++;
                }
            }
            Utility.LogEnd(methodBeginTime);
        }

        /// <inheritdoc />
        /// <summary>
        /// Business logic for StartGroupConfigure API
        /// </summary>
        /// <param Name="configureRequest"></param>
        /// <param Name="modelNumber"></param>
        /// <param Name="locale"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="distributedCache"></param>
        /// <returns></returns>     
        public async Task<ResponseMessage> StartGroupConfigureBl(string parentCode, string modelNumber, string sessionId, string selectedTab,
            ConfigurationRequest configureRequest = null, string locale = null)
        {
            var methodBeginTime = Utility.LogBegin();
            var mainGroupConfigurationResponse = new ConfigurationResponse
            {
                Sections = new List<Sections>()
            };
            var basesSecctionValues = new List<Sections>();
            var packagePath = configureRequest?.PackagePath;
            var modifiedOn = new DateTime();
            var parentCodeValue = parentCode + "_" + modelNumber;
            //checks whether the configuration is in cache for page reload
            var currentConfigurationCache = _cpqCacheManager.GetCache(sessionId, _environment,
            Constant.CURRENTMACHINECONFIGURATION, parentCodeValue);
            if (configureRequest != null && (string.IsNullOrEmpty(packagePath)))
            {
                if (String.IsNullOrEmpty(currentConfigurationCache))
                {
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = _localizer[Constant.SESSIONIDPARENTCODEISNOMOREVALID + locale?.Split(Constant.UNDERSCORECHAR)[0]].Value
                    });
                }
                var currentConfiguration = Utility.DeserializeObjectValue<StartConfigureResponse>(currentConfigurationCache);
                if (currentConfiguration.Audits.ModifiedOn == null)
                {
                    currentConfiguration.Audits.ModifiedOn = new DateTime();
                }
                if (currentConfiguration != null && modifiedOn != default(DateTime) &&
                    DateTime.Compare(modifiedOn, (DateTime)currentConfiguration.Audits.ModifiedOn) > 0)
                {
                    currentConfiguration.Audits.ModifiedOn = modifiedOn;
                }
                var value = Utility.SerializeObjectValue(currentConfiguration);
                _cpqCacheManager.SetCache(sessionId, _environment, Constant.CURRENTMACHINECONFIGURATION, parentCodeValue, value);
                //Response for UI
                var updatedStartConfigureResponse = ViewModelResponseMapper(currentConfiguration);
                var updatedCurrentConfiguration = Utility.SerializeObjectValue(updatedStartConfigureResponse);
                return new ResponseMessage
                {
                    Response = JObject.Parse(updatedCurrentConfiguration),
                    StatusCode = Constant.SUCCESS
                };
            }
            //Gets the base configuration of the model
            var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest, modelNumber);
            //Adding include sections
            baseConfigureRequest = GenerateIncludeSections(baseConfigureRequest, selectedTab);
            var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, packagePath, sessionId
                ).ConfigureAwait(false);
            var baseConfigureResponse = baseConfigureResponseJObj.Response.ToObject<ConfigurationResponse>();
            // configuration object values for conflict mapping
            var ActualConfigureResponse = baseConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = ActualConfigureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            // adding defaults to cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTUNITCONFIGVALUES, Utility.SerializeObjectValue(configureResponseArgumentJObject));
            var stubGroupConfigurationResponseObj = new ConfigurationResponse();
            // to select required configurationtype
            var selectedTabValue = selectedTab.ToUpper();
            switch (selectedTabValue)
            {
                // normal start and change configuration type
                case Constant.GROUPCONFIGURATION:
                    var stubGroupConfigurationResponse = JObject.Parse(File.ReadAllText(Constant.STARTGROUPCONFIGURATIONSTUBVALIDATEDATAPATH));
                    // setting stub data into an sectionsValues object
                    stubGroupConfigurationResponseObj = stubGroupConfigurationResponse.ToObject<ConfigurationResponse>();
                    break;
                // group Layout Configuration type
                case Constant.GROUPLAYOUTCONFIGURATION:
                    var stubGroupLayoutConfigurationResponses = JObject.Parse(File.ReadAllText(Constant.STARTGROUPLAYOUTCONFIGURATIONSTUBPATH));
                    // setting stub data into an sectionsValues object
                    stubGroupConfigurationResponseObj = stubGroupLayoutConfigurationResponses.ToObject<ConfigurationResponse>();
                    break;
                //default will be normal configuration type
                default:
                    var stubDefaultGroupConfigurationResponse = JObject.Parse(File.ReadAllText(Constant.STARTGROUPCONFIGURATIONSTUBVALIDATEDATAPATH));
                    // setting stub data into an sectionsValues object
                    stubGroupConfigurationResponseObj = stubDefaultGroupConfigurationResponse.ToObject<ConfigurationResponse>();
                    break;
            }
            // getting required Section Values from the configurator service response 
            // loing main building response and stub response
            var filteredBasesResponse = (from baseValue in baseConfigureResponse.Sections
                                         from stubValue in stubGroupConfigurationResponseObj.Sections
                                         where baseValue.Id.ToUpper() == stubValue.Id.ToUpper()
                                         select baseValue).ToList();
            if (filteredBasesResponse != null && filteredBasesResponse.Any())
            {
                basesSecctionValues = filteredBasesResponse;
                foreach (var mainGroupConfigResponse in basesSecctionValues)
                {
                    foreach (var localGroupConfigResponse in stubGroupConfigurationResponseObj.Sections)
                    {
                        if (mainGroupConfigResponse.Id.ToUpper() == localGroupConfigResponse.Id.ToUpper())
                        {
                            var mainSectionValues = mainGroupConfigResponse.sections[0].sections.ToList();
                            var localSectionValues = localGroupConfigResponse.sections[0].sections.ToList();
                            var filterMainSectionValues = (from mainSectionData in mainSectionValues
                                                           from localSectionData in localSectionValues
                                                           where mainSectionData.Id == localSectionData.Id
                                                           select mainSectionData).ToList();
                            mainGroupConfigResponse.sections[0].sections = filterMainSectionValues;
                            foreach (var mainitem in mainGroupConfigResponse.sections[0].sections)
                            {
                                foreach (var localitems in localSectionValues)
                                {
                                    if (mainitem.Id.ToUpper() == localitems.Id.ToUpper())
                                    {
                                        // linq query to select required variables from main response
                                        var filterVarValues = (from mainGroupConfigData in mainitem.Variables
                                                               from localGroupConfigData in localitems.Variables
                                                               where mainGroupConfigData.Id.ToUpper() == localGroupConfigData.Id.ToUpper()
                                                               select mainGroupConfigData).ToList();
                                        if (filterVarValues != null && filterVarValues.Any())
                                        {
                                            // looping the filter values to assign property values from stub
                                            foreach (var item in filterVarValues)
                                            {
                                                // created new list of properties to get and assign from Ilist or from readonly type
                                                var mainPropertiesValues = new List<Properties>();
                                                mainPropertiesValues = item.Properties.ToList();
                                                // getting property values
                                                var localPropertyValues = localitems.Variables.Where(x => x.Id.ToUpper().Equals(item.Id.ToUpper())).ToList();
                                                // null check and assigning property values to main build response
                                                if (localPropertyValues != null && localPropertyValues.Any())
                                                {
                                                    // getting all properties and assigning to mainconfiguration values
                                                    mainPropertiesValues.AddRange(localPropertyValues[0].Properties);
                                                    item.Properties = mainPropertiesValues;
                                                }
                                            }
                                            mainitem.Variables = filterVarValues;
                                            var sectionsMachtedValues = (from nn in mainitem.sections
                                                                         from aa in localitems.sections
                                                                         where nn.Id.ToUpper() == aa.Id.ToUpper()
                                                                         select nn).Distinct().ToList();
                                            mainitem.sections = sectionsMachtedValues;
                                        }
                                        else
                                        {
                                            if (mainitem.sections != null && mainitem.sections.Any())
                                            {
                                                // linq query to select required variables from main response
                                                var variableValues = (from mainGroupConfigData in mainitem.sections
                                                                      from sectionOfVariableCValues in mainGroupConfigData.Variables
                                                                      from localGroupConfigData in localitems.sections
                                                                      from localSectionVariableValues in localGroupConfigData.Variables
                                                                      where sectionOfVariableCValues.Id.ToUpper() == localSectionVariableValues.Id.ToUpper()
                                                                      select sectionOfVariableCValues).Distinct().ToList();
                                                if (variableValues != null && variableValues.Any())
                                                {
                                                    mainitem.sections[0].Variables = variableValues;
                                                    foreach (var item in mainitem.sections[0].Variables)
                                                    {
                                                        // created new list of properties to get and assign from Ilist or from readonly type
                                                        var mainPropertiesValues = new List<Properties>();
                                                        mainPropertiesValues = item.Properties.ToList();
                                                        // getting property values
                                                        var localPropertyValues = localitems.sections[0].Variables.Where(x => x.Id.ToUpper().Equals(item.Id.ToUpper())).ToList();
                                                        // null check and assigning property values to main build response
                                                        if (localPropertyValues != null && localPropertyValues.Any())
                                                        {
                                                            foreach (var items in localPropertyValues)
                                                            {
                                                                if (item.Id.ToUpper() == items.Id.ToUpper())
                                                                {
                                                                    // getting all properties and assigning to mainconfiguration values
                                                                    mainPropertiesValues.AddRange(items.Properties);
                                                                    item.Properties = mainPropertiesValues;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                mainitem.sections.Distinct();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            mainGroupConfigurationResponse.Sections = basesSecctionValues;
            if (configureRequest != null)
            {
                return new ResponseMessage
                {
                    StatusCode = Constant.SUCCESS,
                    //Response = Utility.FilterNullValues(configResponse)
                    Response = Utility.FilterNullValues(mainGroupConfigurationResponse)
                };
            }
            Utility.LogEnd(methodBeginTime);
            throw new CustomException(new ResponseMessage
            {
                StatusCode = Constant.BADREQUEST,
                Message = _localizer[Constant.REQUESTCANNOTBENULL + locale?.Split(Constant.UNDERSCORECHAR)[0]].Value
            });
        }

        /// <summary>
        /// Business logic for StartUnitConfigure API
        /// </summary>
        /// <param Name="parentCode"></param>
        /// <param Name="modelNumber"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="selectedTab"></param>
        /// <param Name="configureRequest"></param>
        /// <param Name="locale"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> StartUnitConfigure(string sessionId, string sectionTab, string fixtureStrategy, int setId, string productType, string controlLanding,
                      JObject variableAssignments = null, List<UnitNames> lstunits = null, List<EntranceConfigurations> entranceConsoles = null, List<UnitHallFixtures> unitHallFixtureConsoles = null, int groupConfigurationId = 0, int unitId = 0)
        {
            var methodBeginTime = Utility.LogBegin();
            var unitCommonMapper = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.UNITCOMMONMAPPER);
            var unitMapperVariablesGeneralInformation = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.GENERALINFOMAPPER);
            var unitMapperVariablesCabInterior = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.CABINTERIORMAPPER);
            var unitMapperVariablesOtherEquipment = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constants.OTHEREQUIPMENT);
            var unithallfixtureContantsDictionary = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.UNITHALLFIXTURECONSTANTMAPPER);
            var configureRequest = CreateConfigurationRequestWithTemplate(variableAssignments, Constant.UNITNAME, null, productType);
            var mainGroupConfigurationResponse = new ConfigurationResponse
            {
                Sections = new List<Sections>()
            };
            List<Compartment> lstcompartments = new List<Compartment>();
            var packagePath = configureRequest?.PackagePath;
            var modifiedOn = new DateTime();
            var currentConfigurationCache = _cpqCacheManager.GetCache(sessionId, _environment,
            Constant.CURRENTMACHINECONFIGURATION);
            if (configureRequest != null && (string.IsNullOrEmpty(packagePath)))
            {
                if (String.IsNullOrEmpty(currentConfigurationCache))
                {
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = _localizer[Constant.SESSIONIDPARENTCODEISNOMOREVALID]
                    });
                }
                var currentConfiguration = Utility.DeserializeObjectValue<StartConfigureResponse>(currentConfigurationCache);
                if (currentConfiguration.Audits.ModifiedOn == null)
                {
                    currentConfiguration.Audits.ModifiedOn = new DateTime();
                }
                if (currentConfiguration != null && modifiedOn != default(DateTime) &&
                    DateTime.Compare(modifiedOn, (DateTime)currentConfiguration.Audits.ModifiedOn) > 0)
                {
                    currentConfiguration.Audits.ModifiedOn = modifiedOn;
                }
                var value = Utility.SerializeObjectValue(currentConfiguration);
                _cpqCacheManager.SetCache(sessionId, _environment, Constant.CURRENTMACHINECONFIGURATION, value);
                //Response for UI
                var updatedStartConfigureResponse = ViewModelResponseMapper(currentConfiguration);
                var updatedCurrentConfiguration = Utility.SerializeObjectValue(updatedStartConfigureResponse);
                return new ResponseMessage
                {
                    Response = JObject.Parse(updatedCurrentConfiguration),
                    StatusCode = Constant.SUCCESS
                };
            }
            //Gets the base configuration of the model
            var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest);
            //Adding include sections
            baseConfigureRequest = GenerateIncludeSections(baseConfigureRequest, Constant.UNITCONFIGURATION, sectionTab, productType);
            //Remove hoistwaydimension variable if the min or max is selected
            var stubRangeValues = JObject.Parse(File.ReadAllText(Constant.UNITSVARIABLESMAPPERPATH));
            var rangeValuesList = stubRangeValues["RangeValues"].ToList();
            var varAssignments = (List<VariableAssignment>)baseConfigureRequest.Line.VariableAssignments;
            var hoistway = varAssignments.Where(x => x.VariableId.Contains(Constant.HOISTWAYDIMENSIONS)).ToList();
            if (hoistway.Count > 0)
            {
                var valdimension = Convert.ToString(hoistway.FirstOrDefault().Value);
                if (Utility.CheckEquals(valdimension, Constant.MINIMUM) || Utility.CheckEquals(valdimension, Constant.MAXIMUM))
                {
                    varAssignments = varAssignments.Where(x => !rangeValuesList.Contains(x.VariableId)).ToList();
                }
            }
            if (Utility.CheckEquals(sectionTab, Constants.TRACTIONHOISTWAYEQUIPMENT))
            {
                var valdimension = Constant.MINIMUM;
                if (hoistway.Count > 0)
                {
                    valdimension = Convert.ToString(hoistway.FirstOrDefault().Value);
                }
                if (!Utility.CheckEquals(valdimension, Constant.CUSTOM))
                {
                    var valCache = SetCacheHoistwayDimensions(null, sessionId, setId);
                    if (valCache != null)
                    {
                        valCache = valCache.Where(x => rangeValuesList.Contains(x.VariableId)).ToList();
                        varAssignments.AddRange(valCache);
                    }
                }
            }
            if (sectionTab.Equals(Constants.TRACTIONHOISTWAYEQUIPMENT))
            {
                varAssignments = varAssignments.GroupBy(group => group.VariableId).Select(g => g.First()).ToList();
            }
            baseConfigureRequest.Line.VariableAssignments = varAssignments;
            var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, packagePath, sessionId).ConfigureAwait(false);
            var baseConfigureResponse = baseConfigureResponseJObj.Response.ToObject<ConfigurationResponse>();
            // configuration object values for conflict mapping
            var configureResponse = baseConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = configureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            // adding defaults to cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTUNITCONFIGVALUES, Utility.SerializeObjectValue(configureResponseArgumentJObject));
            var configureRequestDictionary = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();
            var stubUnitConfigurationResponseObj = new ConfigurationResponse();
            var stubCarFixtureCompartmentResponseObj = new CompartmentsData();
            var stubUnitConfigurationSubSectionResponseObj = new ConfigurationResponse();
            var stubUnitConfigurationMainResponseObj = new ConfigurationResponse();
            // Main Stub 
            var stubMainSubSectionResponse = JObject.Parse(File.ReadAllText(Constant.UNITTUIRESPONSETEMPLATE));
            // setting stub data into an sectionsValues object
            stubUnitConfigurationMainResponseObj = stubMainSubSectionResponse.ToObject<ConfigurationResponse>();
            // to select required configurationtype
            if (string.IsNullOrEmpty(sectionTab))
            {
                sectionTab = Constant.GENERALINFORMATION;
            }
            var sectionTabValue = sectionTab.ToUpper();
            string currentProductType;

            switch (productType)
            {
                //Evolution200
                case Constant.MODEL_EVO200:
                    currentProductType = Constant.EVOLUTION200;
                    break;
                //Evolution100
                case Constant.MODEL_EVO100:
                    currentProductType = Constant.EVOLUTION__100;
                    break;
                //Endura100
                case Constant.ENDURA_100:
                    currentProductType = Constant.END100;
                    break;
                //default Evolution200
                default:
                    currentProductType = Constant.EVOLUTION200;
                    break;
            }
            var enrichTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, currentProductType)));
            switch (sectionTabValue)
            {
                // GeneralInformation
                case Constant.GENERALINFORMATION:
                    stubUnitConfigurationSubSectionResponseObj = GeneralInformationUnitConfigureBl(baseConfigureRequest, currentProductType, baseConfigureResponse, rangeValuesList);
                    break;
                // Cab Interior
                case Constant.CABINTERIOR:
                    stubUnitConfigurationSubSectionResponseObj = CabInteriorUnitConfigureBl(currentProductType, baseConfigureResponse, varAssignments);
                    break;
                // Hoistway traction
                case Constant.HOISTWAYTRACTIONEQUIPMENT:

                    stubUnitConfigurationSubSectionResponseObj = OtherEquipmentUnitConfigureBL(productType, baseConfigureResponse);
                    break;

                // Entrance
                case Constant.ENTRANCES:
                    var entrancesDBvalues = configureRequestDictionary.Any() ? configureRequestDictionary.Where(x => Utility.CheckEquals(x.Key, Constant.PARAMETERS_SPCONTROLLERLOCATION_SP)).Select(x => x.Value).FirstOrDefault() : string.Empty;
                    if (entrancesDBvalues == null)
                    {
                        entrancesDBvalues = Constant.CONTROLLOCATIONJAMBMOUNTED;
                        //entrancesDBvalues = string.Empty;
                    }
                    var isJambMounted = (!string.IsNullOrEmpty(entrancesDBvalues.ToString()) && Utility.CheckEquals(entrancesDBvalues.ToString(), Constant.JAMB_MOUNTEDVALUE)) ? true : false;

                    //fetching entrance configuration frm db
                    var username = GetUserId(sessionId);
                    var entranceConfigurationConsoles = _unitConfigurationDL.GetEntranceConfiguration(setId, groupConfigurationId, controlLanding, username, isJambMounted);
                    //Caching the entrance configuration
                    entranceConsoles = SetCacheEntranceConsoles(entranceConfigurationConsoles, sessionId, setId);
                    stubUnitConfigurationSubSectionResponseObj = EntrancesUnitConfigureBL(productType, baseConfigureResponse, entranceConfigurationConsoles);
                    //Caching Controloc value
                    var controlLocationValue = (from variable in configureRequestDictionary
                                                where (Utility.CheckEquals(variable.Key, Constants.CONTROLOCVARIABLE))
                                                select new VariableAssignment
                                                {
                                                    VariableId = variable.Key,
                                                    Value = variable.Value
                                                }).Distinct().ToList();
                    SetCacheControlLocationValues(controlLocationValue, sessionId, setId);
                    break;
                // Car Fixtures
                case Constant.CARFIXTURE:

                    stubUnitConfigurationSubSectionResponseObj = await CarFixtureStartUnitConfigureBL(sessionId, fixtureStrategy, productType, currentProductType, baseConfigureResponse,
                        configureRequest, setId, sectionTab);

                    break;
                //default will be normal configuration type
                default:
                    var stubGeralInformationdefaultResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.GENERALINFORMATIONTEMPLATE, Constant.EVOLUTION200)));
                    //Sub Section Stub Path
                    var stubGeralInformationSubSectiondefaultResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.EVOLUTION200)));
                    if (Utility.CheckEquals(productType, Constant.ENDURA_100))
                    {
                        stubGeralInformationdefaultResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.GENERALINFORMATIONTEMPLATE, Constant.END100)));
                        stubGeralInformationSubSectiondefaultResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.END100)));
                    }
                    if (Utility.CheckEquals(productType, Constant.MODEL_EVO100))
                    {
                        stubGeralInformationdefaultResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.GENERALINFORMATIONTEMPLATE, Constant.EVOLUTION__100)));
                        stubGeralInformationSubSectiondefaultResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.EVOLUTION__100)));
                    }
                    // setting stub data into an sectionsValues object
                    stubUnitConfigurationResponseObj = stubGeralInformationdefaultResponse.ToObject<ConfigurationResponse>();
                    //Sub Sections Stub Path
                    stubUnitConfigurationSubSectionResponseObj = stubGeralInformationSubSectiondefaultResponse.ToObject<ConfigurationResponse>();
                    break;
            }

            List<VariableAssignment> varAssigns = new List<VariableAssignment>();
            foreach (var subsectionDetails in stubUnitConfigurationSubSectionResponseObj.Sections)
            {
                foreach (var subsection in subsectionDetails.sections)
                {
                    if (Utility.CheckEquals(subsection.Id, Constants.OTHEREQUIPVALUE))
                    {
                        var lowestUnitId = 0;
                        if (unitId > 0)
                        {
                            lowestUnitId = unitId;
                        }
                        else
                        {
                            lowestUnitId = Convert.ToInt32((from openingData in lstunits select openingData?.Unitid)?.ToList()?.Min());
                        }
                        var values = new List<Values>();
                        foreach (var units in lstunits)
                        {
                            values.Add(new Values { value = units.Unitname, id = units.Unitid.ToString(), Name = units.Unitname, State = Constants.AVAILABLE, InCompatible = false, Assigned = lowestUnitId == units.Unitid ? Constants.BYUSER_LOWERCASE : null });
                        }
                        subsection.Variables.Add(new Variables { Id = Constants.UNITSINSET, Name = Constants.UNITSINSET, Values = values, ValueType = Constants.STRING });
                        if (Utility.CheckEquals(currentProductType, Constant.END100) || Utility.CheckEquals(currentProductType, Constant.EVOLUTION__100))
                        {
                            var variables = new List<Variables> { new Variables { Id = unitMapperVariablesOtherEquipment[Constants.ADDITIONALWIRING], Name =unitMapperVariablesOtherEquipment[Constants.ADDITIONALWIRING], ValueType = Constants.STRING,Values=new List<Values>
                            { new Values { value= currentProductType==Constant.END100?20:40,
                            Type=Constants.SINGLETONVALUE,InCompatible=true,Assigned=Constant.BYUSER_LOWERCASE,State=Constants.SELECTED,Justification=Constants.None}} } };
                            subsection.Variables = variables;
                        }
                    }
                    if (baseConfigureRequest.Line.VariableAssignments != null && baseConfigureRequest.Line.VariableAssignments.Any())
                    {
                        // getting values of hoist way
                        var hoidtwayDimensionValues = baseConfigureRequest.Line.VariableAssignments.Where(x => x.VariableId.Contains(Constant.HOISTWAYDIMENSIONS)).ToList();
                        if (hoidtwayDimensionValues != null && hoidtwayDimensionValues.Any())
                        {
                            object dataVal = null;
                            var vfdValue = (from variablesItem in subsection.Variables
                                            where variablesItem.Id.Equals(unitMapperVariablesOtherEquipment[Constants.VFDWIRING])
                                            select variablesItem?.Values)?.FirstOrDefault();
                            if (vfdValue != null)
                            {
                                dataVal = (from variablesItem in subsection?.Variables
                                           where variablesItem.Id.Equals(unitMapperVariablesOtherEquipment[Constants.VFDWIRING])
                                           from variableData
                                            in variablesItem?.Values
                                           where variableData.Type.Equals(Constant.SINGLETONVALUE) && variableData.InCompatible.Equals(false)
                                           select variableData?.value)?.FirstOrDefault();
                            }                               
                            var customValues = hoidtwayDimensionValues.FirstOrDefault().Value.ToString();
                            foreach (var variablesItem in subsection.Variables)
                            {
                                if (Utility.CheckEquals(variablesItem.Id, unitMapperVariablesCabInterior[Constant.BUMPERHEIGHT]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesCabInterior[Constants.HANDRAILHEIGHT]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesCabInterior[Constants.OTHERWEIGHT]))
                                {
                                    if (variablesItem.Values != null)
                                    {
                                        var minDatas = variablesItem.Values.Where(x => x.lower != null && x.InCompatible.Equals(false) && x.State.Equals(Constant.AVAILABLE))?.FirstOrDefault()?.lower;
                                        var maxDatas = variablesItem.Values.Where(x => x.upper != null && x.InCompatible.Equals(false) && x.State.Equals(Constant.AVAILABLE))?.LastOrDefault()?.upper;
                                        var getPropertiesVal = JArray.Parse(File.ReadAllText(string.Format(Constant.PROPERTYTEMPLATE, currentProductType)));
                                        var variableProperties = Utility.DeserializeObjectValue<IList<Properties>>(Utility.SerializeObjectValue(getPropertiesVal));
                                        if (variableProperties.Any())
                                        {
                                            variablesItem.Properties = variableProperties;
                                        }
                                        if (!variablesItem.Id.Equals(unitMapperVariablesCabInterior[Constants.OTHERWEIGHT]))
                                        {
                                            foreach (var item in variablesItem.Properties)
                                            {
                                                item.Value = GetPropertyValueForHeight(item.Id, minDatas, maxDatas, false);
                                            }
                                            if (productType.Equals(Constants.ENDURA_100) && variablesItem.Id.Equals(unitMapperVariablesCabInterior[Constants.HANDRAILHEIGHT]))
                                            {
                                                var minVal = (from val in variablesItem.Properties where val.Id.Equals(Constant.MINVALUE) select val)?.FirstOrDefault();
                                                variablesItem.Properties.Remove(minVal);
                                                var maxVal = (from val in variablesItem.Properties where val.Id.Equals(Constant.MAXVALUE) select val)?.FirstOrDefault();
                                                variablesItem.Properties.Remove(maxVal);
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in variablesItem.Properties)
                                            {
                                                item.Value = GetPropertyValueForHeight(item.Id, minDatas, maxDatas, Constants.OTHERWEIGHT);
                                            }
                                        }
                                    }
                                }

                                if (Utility.CheckEquals(variablesItem.Id, unitMapperVariablesOtherEquipment[Constants.VFDWIRING]) 
                                    || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesOtherEquipment[Constants.ADDITIONALWIRING]) 
                                    || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesOtherEquipment[Constants.TOTALWIRINGS]))
                                {
                                    var data = variablesItem.Values?.Where(x => x.Type.Equals(Constant.SINGLETONVALUE) && x.InCompatible.Equals(false))?.FirstOrDefault();
                                    if (data != null)
                                    {
                                        data.value = Convert.ToInt32(data.value) / 12;
                                    }
                                    if ((dataVal!= null) && 
                                        (variablesItem.Id.Equals(unitMapperVariablesOtherEquipment[Constants.ADDITIONALWIRING]) 
                                        || variablesItem.Id.Equals(unitMapperVariablesOtherEquipment[Constants.TOTALWIRING])))
                                    {
                                        GetRangeValidationForHoistway(variablesItem, dataVal);
                                    }
                                }
                                if (Utility.CheckEquals(variablesItem.Id, unitMapperVariablesOtherEquipment[Constants.COUNTERWEIGHTSAFETY]))
                                {
                                    var result = await GetDataToCheckOccupiedSpace(groupConfigurationId, variableAssignments, sessionId);
                                    SetIncompatibilityForCWSFTY(result, variablesItem, lstunits);
                                }
                                if (Utility.CheckEquals(variablesItem.Id, unitMapperVariablesGeneralInformation[Constants.FRONTDOORHEIGHTMAP]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesGeneralInformation[Constants.FRONTDOORWIDTHMAP]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesGeneralInformation[Constants.FRONTDOORTYPEMAP]))
                                {
                                    variablesItem.Properties = AddDynamicLayout(stubUnitConfigurationSubSectionResponseObj, variablesItem);
                                }
                                foreach (var rangeItem in rangeValuesList)
                                {
                                    if (Utility.CheckEquals(variablesItem.Id, rangeItem.ToString()))
                                    {
                                        // getting min and max values  
                                        object minData = null;
                                        object maxData = null;
                                        minData = variablesItem.Values.Where(x => x.lower != null && x.InCompatible.Equals(false) && x.State.Equals(Constant.AVAILABLE))?.FirstOrDefault()?.lower;
                                        maxData = variablesItem.Values.Where(x => x.upper != null && x.InCompatible.Equals(false) && x.State.Equals(Constant.AVAILABLE))?.LastOrDefault()?.upper;
                                        if (Utility.CheckEquals(productType, Constants.ENDURA_100) && variablesItem.Id.Contains(Constants.PITDEPTHVALUE))
                                        {
                                            minData = variablesItem.Values.Select(x => x.value)?.FirstOrDefault()?.ToString();
                                            maxData = variablesItem.Values.Select(x => x.value)?.LastOrDefault()?.ToString();
                                        }
                                        var assignedValue = variablesItem.Values.Where(x => x.Assigned != null && x.Assigned.Equals(Constant.BYRULE_CAMELCASE)).ToList();
                                        if (assignedValue.Count.Equals(0))
                                        {
                                            if (customValues.Equals(Constant.MINIMUM))
                                            {
                                                var value = new Values()
                                                {
                                                    Type = Constant.SINGLETONVALUE,
                                                    value = minData,
                                                    Assigned = Constant.BYRULE,
                                                    InCompatible = false,
                                                    State = Constant.AVAILABLE
                                                };
                                                variablesItem.Values.Add(value);
                                            }
                                            else if (customValues.Equals(Constant.MAXIMUM))
                                            {
                                                var value = new Values()
                                                {
                                                    Type = Constant.SINGLETONVALUE,
                                                    value = maxData,
                                                    Assigned = Constant.BYRULE,
                                                    InCompatible = false,
                                                    State = Constant.AVAILABLE
                                                };
                                                variablesItem.Values.Add(value);
                                            }
                                        }
                                        else
                                        {
                                            minData = minData == null ? assignedValue[0].value : minData;
                                            maxData = maxData == null ? assignedValue[0].value : maxData;
                                        }
                                        var getProperties = JArray.Parse(File.ReadAllText(string.Format(Constant.PROPERTYTEMPLATE, currentProductType)));
                                        var variableProperties = Utility.DeserializeObjectValue<IList<Properties>>(Utility.SerializeObjectValue(getProperties));
                                        if (variableProperties.Any())
                                        {
                                            variablesItem.Properties = variableProperties;


                                            foreach (var item in variablesItem.Properties)
                                            {
                                                if (!String.IsNullOrEmpty(customValues))
                                                {
                                                    if (customValues.Equals(Constant.CUSTOM))
                                                    {
                                                        if (minData == null || maxData == null)
                                                        {
                                                            var assignedByUserValue = variablesItem.Values.Where(x => x.Assigned != null && x.Assigned.Equals(Constant.BYUSER_CAMELCASE)).ToList();
                                                            if (assignedByUserValue.Count.Equals(0))
                                                            {
                                                                assignedByUserValue = variablesItem.Values.Where(x => x.Assigned != null && (x.Assigned.Equals(Constant.BYUSER_CAMELCASE) || Utility.CheckEquals(x.Assigned, Constant.BYDEFAULT_CAMELCASE))).ToList();
                                                            }
                                                            if (assignedByUserValue.Count > 0)
                                                            {
                                                                minData = minData == null ? assignedByUserValue[0].value : minData;
                                                                maxData = maxData == null ? assignedByUserValue[0].value : maxData;
                                                            }
                                                        }
                                                    }
                                                }
                                                // if it is custom selection we will map the min, max and is range flag to set the range
                                                item.Value = GetPropertyValue(item.Id, minData, maxData, customValues);
                                            }
                                            if (productType.Equals(Constants.ENDURA_100) && customValues != null && customValues.Equals(Constant.CUSTOM) && variablesItem.Id.Contains(Constants.PITDEPTHVALUE))
                                            {
                                                variablesItem.Properties = GetPropertyForCustomPitDepth();
                                            }
                                            var requiredValue = (from assignedValues in variablesItem.Values where assignedValues.Assigned != null select assignedValues.value).ToList().FirstOrDefault();
                                            VariableAssignment assignings = new VariableAssignment
                                            {
                                                VariableId = variablesItem.Id,
                                                Value = requiredValue == null ? string.Empty : requiredValue
                                            };
                                            varAssigns.Add(assignings);

                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Object dataVal = null;
                            var vfdValue = (from variablesItem in subsection.Variables
                                            where variablesItem.Id.Equals(unitMapperVariablesOtherEquipment[Constants.VFDWIRING])
                                            select variablesItem?.Values)?.FirstOrDefault();
                            if(vfdValue != null)
                            {
                               dataVal = (from variablesItem in subsection.Variables where variablesItem.Id.Equals(unitMapperVariablesOtherEquipment[Constants.VFDWIRING]) from variableData in variablesItem?.Values where variableData.Type.Equals(Constant.SINGLETONVALUE) && variableData.InCompatible.Equals(false) select variableData?.value)?.FirstOrDefault();
                            }
                            foreach (var variablesItem in subsection.Variables)
                            {
                                if (Utility.CheckEquals(variablesItem.Id, unitMapperVariablesCabInterior[Constant.BUMPERHEIGHT]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesCabInterior[Constants.HANDRAILHEIGHT]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesCabInterior[Constants.OTHERWEIGHT]))
                                {
                                    if (variablesItem.Values != null)
                                    {
                                        var minDatas = variablesItem.Values.Where(x => x.lower != null && x.InCompatible.Equals(false) && x.State.Equals(Constant.AVAILABLE))?.FirstOrDefault()?.lower;
                                        var maxDatas = variablesItem.Values.Where(x => x.upper != null && x.InCompatible.Equals(false) && x.State.Equals(Constant.AVAILABLE))?.LastOrDefault()?.upper;
                                        var getPropertiesVal = JArray.Parse(File.ReadAllText(string.Format(Constant.PROPERTYTEMPLATE, currentProductType)));
                                        var variableProperties = Utility.DeserializeObjectValue<IList<Properties>>(Utility.SerializeObjectValue(getPropertiesVal));
                                        if (variableProperties.Any())
                                        {
                                            variablesItem.Properties = variableProperties;
                                        }
                                        if (!variablesItem.Id.Equals(unitMapperVariablesCabInterior[Constants.OTHERWEIGHT]))
                                        {
                                            foreach (var item in variablesItem.Properties)
                                            {
                                                item.Value = GetPropertyValueForHeight(item.Id, minDatas, maxDatas, false);
                                            }
                                            if (productType.Equals(Constants.ENDURA_100) && variablesItem.Id.Equals(unitMapperVariablesCabInterior[Constants.HANDRAILHEIGHT]))
                                            {
                                                var minVal = (from val in variablesItem.Properties where val.Id.Equals(Constant.MINVALUE) select val)?.FirstOrDefault();
                                                variablesItem.Properties.Remove(minVal);
                                                var maxVal = (from val in variablesItem.Properties where val.Id.Equals(Constant.MAXVALUE) select val)?.FirstOrDefault();
                                                variablesItem.Properties.Remove(maxVal);
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in variablesItem.Properties)
                                            {
                                                item.Value = GetPropertyValueForHeight(item.Id, minDatas, maxDatas, Constants.OTHERWEIGHT);
                                            }
                                        }
                                    }


                                }
                                if (Utility.CheckEquals(variablesItem.Id, unitMapperVariablesOtherEquipment[Constants.VFDWIRING]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesOtherEquipment[Constants.ADDITIONALWIRING]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesOtherEquipment[Constants.TOTALWIRINGS]))
                                {
                                    var data = variablesItem.Values?.Where(x => x.Type.Equals(Constant.SINGLETONVALUE) && x.InCompatible.Equals(false))?.FirstOrDefault();
                                    if (data != null)
                                    {
                                        data.value = Convert.ToInt32(data.value) / 12;
                                    }
                                    if ((dataVal!=null) && (variablesItem.Id.Equals(unitMapperVariablesOtherEquipment[Constants.ADDITIONALWIRING]) || variablesItem.Id.Equals(unitMapperVariablesOtherEquipment[Constants.TOTALWIRING])))
                                    {
                                        GetRangeValidationForHoistway(variablesItem, dataVal);
                                    }
                                }
                                if (variablesItem.Id.Equals(unitMapperVariablesOtherEquipment[Constants.COUNTERWEIGHTSAFETY]))
                                {
                                    var result = await GetDataToCheckOccupiedSpace(groupConfigurationId, variableAssignments, sessionId);
                                    SetIncompatibilityForCWSFTY(result, variablesItem, lstunits);
                                }

                                if (Utility.CheckEquals(variablesItem.Id, unitMapperVariablesGeneralInformation[Constants.FRONTDOORHEIGHTMAP]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesGeneralInformation[Constants.FRONTDOORWIDTHMAP]) || Utility.CheckEquals(variablesItem.Id, unitMapperVariablesGeneralInformation[Constants.FRONTDOORTYPEMAP]))
                                {
                                    variablesItem.Properties = AddDynamicLayout(stubUnitConfigurationSubSectionResponseObj, variablesItem);
                                }
                                foreach (var rangeItem in rangeValuesList)
                                {
                                    if (Utility.CheckEquals(variablesItem.Id, rangeItem.ToString()))
                                    {
                                        // getting min and max values  
                                        object minData = null;
                                        object maxData = null;
                                        minData = variablesItem.Values.Where(x => x.lower != null && x.InCompatible.Equals(false) && x.State.Equals(Constant.AVAILABLE))?.FirstOrDefault()?.lower;
                                        maxData = variablesItem.Values.Where(x => x.upper != null && x.InCompatible.Equals(false) && x.State.Equals(Constant.AVAILABLE))?.LastOrDefault()?.upper;
                                        if (Utility.CheckEquals(productType, Constants.ENDURA_100) && variablesItem.Id.Contains(Constants.PITDEPTHVALUE))
                                        {
                                            minData = variablesItem.Values.Select(x => x.value)?.FirstOrDefault()?.ToString();
                                            maxData = variablesItem.Values.Select(x => x.value)?.LastOrDefault()?.ToString();
                                        }
                                        var assignedValue = variablesItem.Values.Where(x => x.Assigned != null && x.Assigned.Equals(Constant.BYRULE_CAMELCASE)).ToList();
                                        if (assignedValue.Count.Equals(0))
                                        {
                                            var value = new Values()
                                            {
                                                Type = Constant.SINGLETONVALUE,
                                                value = minData,
                                                Assigned = Constant.BYRULE,
                                                InCompatible = false,
                                                State = Constant.AVAILABLE
                                            };
                                            variablesItem.Values.Add(value);

                                        }
                                        else
                                        {
                                            minData = minData == null ? assignedValue[0].value : minData;
                                            maxData = maxData == null ? assignedValue[0].value : maxData;
                                        }
                                        var getProperties = JArray.Parse(File.ReadAllText(string.Format(Constant.PROPERTYTEMPLATE, currentProductType)));
                                        var variableProperties = Utility.DeserializeObjectValue<IList<Properties>>(Utility.SerializeObjectValue(getProperties));
                                        if (variableProperties.Any())
                                        {
                                            variablesItem.Properties = variableProperties;


                                            foreach (var item in variablesItem.Properties)
                                            {
                                                // if it is custom selection we will map the min, max and is range flag to set the range
                                                item.Value = GetPropertyValue(item.Id, minData, maxData, Constant.MINIMUM);
                                            }

                                            var requiredValue = (from assignedValues in variablesItem.Values where assignedValues.Assigned != null select assignedValues.value).ToList().FirstOrDefault();
                                            VariableAssignment assignings = new VariableAssignment
                                            {
                                                VariableId = variablesItem.Id,
                                                Value = requiredValue == null ? string.Empty : requiredValue
                                            };
                                            varAssigns.Add(assignings);

                                        }
                                    }
                                }


                            }
                        }
                    }

                    // To set Cacacity,CarSpeed Values in Cache
                    foreach (var subsectionValue in subsection.Variables)
                    {
                        if (subsectionValue.Id.Contains(Constant.CARSPEEDVARIABLE) || subsectionValue.Id.Contains(Constant.CAPACITY) || subsectionValue.Id.Contains(unitMapperVariablesGeneralInformation[Constants.ENDURACARSPEEDVARIABLE]))
                        {
                            var requiredValue = (from assignedValues in subsectionValue.Values
                                                 where assignedValues.Assigned != null
                                                 select assignedValues.value).ToList().FirstOrDefault();

                            VariableAssignment assignings = new VariableAssignment
                            {
                                VariableId = subsectionValue.Id == unitMapperVariablesGeneralInformation[Constants.ENDURACARSPEEDVARIABLE] ? unitMapperVariablesGeneralInformation[Constants.SPEED] : subsectionValue.Id,
                                Value = requiredValue == null ? string.Empty : requiredValue
                            };
                            varAssigns.Add(assignings);
                        }
                    }
                }
            }
            if (varAssigns != null && varAssigns.Any())
            {
                var hoistwaycache = SetCacheHoistwayDimensions(varAssigns, sessionId, setId);
            }

            mainGroupConfigurationResponse.Sections = stubUnitConfigurationSubSectionResponseObj.Sections;
            foreach (var items in stubUnitConfigurationMainResponseObj.Sections)
            {
                if (items.Id.ToUpper() == sectionTab.ToString().ToUpper())
                {
                    var mainSectionValues = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(mainGroupConfigurationResponse.Sections.First().sections));
                    items.sections = Utility.DeserializeObjectValue<IList<SectionsValues>>(Utility.SerializeObjectValue(mainSectionValues));
                    break;
                }
            }
            stubUnitConfigurationMainResponseObj.Units = lstunits.Distinct().ToList();

            // added enriched data in the main response 
            var enrichedData = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.EVOLUTION200)));
            if (Utility.CheckEquals(productType, Constant.ENDURA_100))
            {
                enrichedData = JObject.Parse(File.ReadAllText(Constant.UNITENRICHEDDATAENDURA100));
            }
            if (Utility.CheckEquals(productType, Constant.MODEL_EVO100))
            {
                enrichedData = JObject.Parse(File.ReadAllText(Constant.UNITENRICHEDDATAEVOLUTION100));
            }
            stubUnitConfigurationMainResponseObj.EnrichedData = enrichedData;
            // added conflicts 
            var conflictChangesValues = CompareChangeInConfigResponse(baseConfigureResponse, Constant.UNITCONFIGURATION, configureRequestDictionary);
            stubUnitConfigurationMainResponseObj.ConflictAssignments = conflictChangesValues;
            var getData = (from val1 in configureRequestDictionary
                           where (val1.Key.ToString().Contains("CAPACITY") || val1.Key.ToString().Contains("CARSPEED"))
                           select new VariableAssignment
                           {
                               VariableId = val1.Key,
                               Value = val1.Value
                           }).Distinct().ToList();
            foreach (var a in getData)
            {
                if (a.VariableId.Contains("CAPACITY_INT"))
                {
                    getData = getData.Where(x => !x.VariableId.Contains("CAPACITY_INT")).ToList();
                }
            }
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.PREVIOUSUNITCONFLICTSVALUES, Utility.SerializeObjectValue(conflictChangesValues));
            var crossPackagevariableAssignments = new List<VariableAssignment>();
            crossPackagevariableAssignments = baseConfigureRequest.Line.VariableAssignments.ToList();
            crossPackagevariableAssignments.AddRange(getData);
            crossPackagevariableAssignments.Distinct();
            SetCrosspackageVariableAssignments(crossPackagevariableAssignments, sessionId,string.Concat(Constant.UNITCONFIGURATION,setId));
            if (configureRequest != null)
            {
                return new ResponseMessage
                {
                    StatusCode = Constant.SUCCESS,
                    Response = Utility.FilterNullValues(stubUnitConfigurationMainResponseObj)
                };
            }
            Utility.LogEnd(methodBeginTime);
            throw new CustomException(new ResponseMessage
            {
                StatusCode = Constant.BADREQUEST,
                Message = _localizer[Constant.REQUESTCANNOTBENULL]
            });
        }

        public async Task<ResponseMessage> StartBuildingEquipmentConfigureBl(string sessionId, List<string> permission, int buildingId,
                      JObject variableAssignments = null, List<BuildingEquipmentData> BuildingEquipmentConsoles = null)
        {
            var methodBeginTime = Utility.LogBegin();
            var configureRequest = CreateConfigurationRequestWithTemplate(variableAssignments, Constant.BUILDINGNAME);
            var buildingEquipmentContantsDictionary = Utility.GetVariableMapping(Constants.BUILDINGMAPPERVARIABLESMAPPERPATH, Constants.BUILDINGEQUIPMENTVARIABLES);
            var mainGroupConfigurationResponse = new ConfigurationResponse
            {
                Sections = new List<Sections>()
            };
            var packagePath = configureRequest?.PackagePath;
            var modifiedOn = new DateTime();
            var currentConfigurationCache = _cpqCacheManager.GetCache(sessionId, _environment,
            Constant.BUILDINGEQUIPMENT);
            if (configureRequest != null && (string.IsNullOrEmpty(packagePath)))
            {
                if (String.IsNullOrEmpty(currentConfigurationCache))
                {
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = _localizer[Constant.SESSIONIDPARENTCODEISNOMOREVALID]
                    });
                }
                var currentConfiguration = Utility.DeserializeObjectValue<StartConfigureResponse>(currentConfigurationCache);
                if (currentConfiguration.Audits.ModifiedOn == null)
                {
                    currentConfiguration.Audits.ModifiedOn = new DateTime();
                }
                if (currentConfiguration != null && modifiedOn != default(DateTime) &&
                    DateTime.Compare(modifiedOn, (DateTime)currentConfiguration.Audits.ModifiedOn) > 0)
                {
                    currentConfiguration.Audits.ModifiedOn = modifiedOn;
                }
                var value = Utility.SerializeObjectValue(currentConfiguration);
                _cpqCacheManager.SetCache(sessionId, _environment, Constant.BUILDINGEQUIPMENT, value);
                //Response for UI
                var updatedStartConfigureResponse = ViewModelResponseMapper(currentConfiguration);
                var updatedCurrentConfiguration = Utility.SerializeObjectValue(updatedStartConfigureResponse);
                return new ResponseMessage
                {
                    Response = JObject.Parse(updatedCurrentConfiguration),
                    StatusCode = Constant.SUCCESS
                };
            }
            //Gets the base configuration of the model
            var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest);
            //Adding include sections
            baseConfigureRequest = GenerateIncludeSections(baseConfigureRequest, Constant.BUILDINGEQUIPMENT);
            var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, packagePath, sessionId).ConfigureAwait(false);
            var baseConfigureResponse = baseConfigureResponseJObj.Response.ToObject<ConfigurationResponse>();
            // configuration object values for conflict mapping
            var ActualConfigureResponse = baseConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = ActualConfigureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            // adding defaults to cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTBUILDINGEQUIPMENTCONFIGVALUES, Utility.SerializeObjectValue(configureResponseArgumentJObject));
            var stubUnitConfigurationResponseObj = new ConfigurationResponse();
            var stubUnitConfigurationSubSectionResponseObj = new ConfigurationResponse();
            var stubUnitConfigurationMainResponseObj = new ConfigurationResponse();
            //Sub Section Stub Path
            var BuildingEqipmentTemplate = JObject.Parse(File.ReadAllText(Constant.BUILDINGEQUIPMENTUITEMPLATE));
            stubUnitConfigurationSubSectionResponseObj = BuildingEqipmentTemplate.ToObject<ConfigurationResponse>();
            var filteredSection = Utility.MapVariables(Utility.SerializeObjectValue(baseConfigureResponse), Utility.SerializeObjectValue(stubUnitConfigurationSubSectionResponseObj));
            var filteredVariables = Utility.DeserializeObjectValue<Sections>(filteredSection);
            var emergencySwitchSelected = true;
            //BuildingEquipment Console Code
            foreach (var subsection in filteredVariables.sections[0].sections)
            {
                if (subsection.Id.Equals(Constant.PARAMETERLOBBYPANEL))
                {
                    var stubVariables = subsection.Variables;
                    if (BuildingEquipmentConsoles.Count > 0)
                    {
                        subsection.sections = new List<SectionsValues>();
                        foreach (var varEntrance in BuildingEquipmentConsoles)
                        {
                            var lstLobby = BuildingEquipmentConsoles.Where(x => x.ConsoleName.Contains(Constant.KEYWORDLOBBYPANEL)).ToList();
                            int count = 0;
                            foreach (var looby in lstLobby)
                            {
                                count++;
                                if (looby.IsLobby == true && looby.ConsoleName.Contains(Constant.KEYWORDLOBBYPANEL))
                                {
                                    if (looby.ConsoleId > 0)
                                    {
                                        var varSection = new SectionsValues()
                                        {
                                            assignOpenings = null,
                                            Id = looby.ConsoleNumber.ToString(),
                                            Name = Constant.BUILDINGEQUIPMENTCONSOLESECTION + looby.ConsoleName.Replace(Constant.EMPTYSPACE, string.Empty),
                                            isController = looby.IsController,
                                            IsDelete = true,
                                            isLobby = looby.IsLobby,
                                            AssignedGroups = looby.AssignedGroups,
                                            AssignedUnits = looby.AssignedUnits,
                                            Variables = new List<Variables>(),
                                            Properties = new List<PropertyDetailsValues>()
                                        };
                                        List<Variables> lstVariables = new List<Variables>();
                                        var newflag = (from val in looby.VariableAssignments
                                                       where val.Value.Equals(string.Empty)
                                                       select val).ToList();
                                        var flagnew = 0;
                                        if (looby.VariableAssignments.Count() != newflag.Count())
                                        {
                                            flagnew = 1;
                                        }
                                        if (looby.VariableAssignments.Count >= 1 && flagnew == 1)
                                        {
                                            foreach (var variableAssignment in looby.VariableAssignments)
                                            {
                                                var lstproperties = (from variable in stubVariables
                                                                     from property in variable.Properties
                                                                     where variable.Id.Equals(variableAssignment.VariableId)
                                                                     select property).ToList();
                                                var displayname = string.Empty;
                                                var sequence = string.Empty;
                                                if (lstproperties.Count > 0)
                                                {
                                                    var lstdisplayname = lstproperties.Where(x => x.Id == Constant.DISPLAYNAME).Select(x => x.Value).ToList();
                                                    if (lstdisplayname.Count > 0)
                                                    {
                                                        displayname = lstdisplayname[0].ToString();
                                                    }
                                                    var lstsequence = lstproperties.Where(x => x.Id == Constant.SEQUENCE).Select(x => x.Value).ToList();
                                                    if (lstsequence.Count > 0)
                                                    {
                                                        sequence = lstsequence[0].ToString();
                                                    }
                                                }
                                                var boolValue = variableAssignment.Value;
                                                if (boolValue.Equals(Constant.True))
                                                {
                                                    boolValue = true;
                                                }
                                                else if (boolValue.Equals(Constant.False))
                                                {
                                                    boolValue = false;
                                                }
                                                var varVariable = new Variables()
                                                {
                                                    Id = variableAssignment.VariableId,
                                                    Name = displayname,
                                                    Value = boolValue,
                                                    Sequence = sequence
                                                };
                                                lstVariables.Add(varVariable);
                                            }
                                            varSection.Variables = lstVariables;
                                        }
                                        else
                                        {
                                            foreach (var variables in stubVariables)
                                            {
                                                var displayname = string.Empty;
                                                var sequence = string.Empty;
                                                var lstdisplayname = variables.Properties.Where(x => x.Id == Constant.DISPLAYNAME).Select(x => x.Value).ToList();
                                                if (lstdisplayname.Count > 0)
                                                {
                                                    displayname = lstdisplayname[0].ToString();
                                                }
                                                var lstsequence = variables.Properties.Where(x => x.Id == Constant.SEQUENCE).Select(x => x.Value).ToList();
                                                if (lstsequence.Count > 0)
                                                {
                                                    sequence = lstsequence[0].ToString();
                                                }
                                                var varVariable = new Variables()
                                                {
                                                    Id = variables.Id,
                                                    Name = displayname,
                                                    Sequence = sequence,
                                                    Value = variables.Values.Count > 0 ? variables.Values[0].value : string.Empty
                                                };
                                                lstVariables.Add(varVariable);
                                            }
                                            varSection.Variables = lstVariables;
                                        }
                                        var stubProperties = JArray.Parse(File.ReadAllText(Constant.PROPERTIESTEMPLATE));
                                        List<PropertyDetailsValues> varProprties = stubProperties.ToObject<List<PropertyDetailsValues>>();
                                        foreach (var varProperty in varProprties)
                                        {
                                            switch (varProperty.Id)
                                            {
                                                case Constant.SEQUENCE:
                                                    varProperty.Value = count;
                                                    break;
                                                case Constant.SECTIONNAME:
                                                    varProperty.Value = looby.ConsoleName;
                                                    break;
                                            }
                                        }
                                        varSection.Properties = varProprties;
                                        subsection.sections.Add(varSection);
                                        //For checking emergency power switch selection
                                        var emergencySwitchValue = varSection.Variables.Where(x => x.Id.Equals(buildingEquipmentContantsDictionary[Constants.EMERGENCYPOWERSWITCHLOBBY], StringComparison.OrdinalIgnoreCase)).ToList().FirstOrDefault();
                                        if (emergencySwitchValue != null && emergencySwitchValue.Value.Equals(true))
                                        {
                                            emergencySwitchSelected = false;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                        subsection.Variables = new List<Variables>();
                    }
                    var lobbyRecallSwitchData = _cpqCacheManager.GetCache(sessionId, _environment, buildingId.ToString(), Constant.LOBBYRECALLSWITCHVARAIBLES);
                    if (!string.IsNullOrEmpty(lobbyRecallSwitchData))
                    {
                        subsection.LobbyRecallSwitchPerGroup = Utility.DeserializeObjectValue<List<LobbyRecallSwitchConfigured>>(lobbyRecallSwitchData);
                    }
                    //subsection.LobbyRecallSwitchPerGroup
                }
                else if (subsection.Id.Equals(Constants.SMARTRESCUEPHONESTANDALONE))
                {
                    var stubVariables = subsection.Variables;
                    if (BuildingEquipmentConsoles.Count > 0)
                    {
                        subsection.sections = new List<SectionsValues>();
                        var totalAssignedGroup = 0;
                        var totalAssignedUnit = 0;
                        var varSection = new SectionsValues()
                        {
                            assignOpenings = null,
                            Id = Constants.IDTWO,
                            Name = Constants.SMARTRESCUEPHONESTANDALONENAME,
                            isController = true,
                            isLobby = false,
                            Variables = new List<Variables>(),
                            Properties = new List<PropertyDetailsValues>()
                        };
                        List<Variables> lstVariables = new List<Variables>();
                        foreach (var varEntrance in BuildingEquipmentConsoles)
                        {
                            if (varEntrance.IsLobby == false && (varEntrance.ConsoleName.Equals(Constant.KEYWORDSMARTRESCUEPHONE5) || varEntrance.ConsoleName.Equals(Constant.KEYWORDSMARTRESCUEPHONE10)))
                            {
                                if (varEntrance.ConsoleId > 0)
                                {
                                    var newflag = (from val in varEntrance.VariableAssignments
                                                   where val.Value.Equals(string.Empty)
                                                   select val).ToList();
                                    var flagnew = 0;
                                    if (varEntrance.VariableAssignments.Count() != newflag.Count())
                                    {
                                        flagnew = 1;
                                    }
                                    if (varEntrance.VariableAssignments.Count >= 1 && flagnew == 1)
                                    {
                                        foreach (var variableAssignment in varEntrance.VariableAssignments)
                                        {
                                            var boolValue = variableAssignment.Value;
                                            if (boolValue.Equals(Constant.True))
                                            {
                                                boolValue = true;
                                            }
                                            else if (boolValue.Equals(Constant.False))
                                            {
                                                boolValue = false;
                                            }
                                            var varVariable = new Variables()
                                            {
                                                Id = variableAssignment.VariableId,
                                                Value = boolValue
                                            };
                                            lstVariables.Add(varVariable);
                                        }
                                    }
                                    else
                                    {
                                        var requiredVariables = stubVariables;
                                        if (varEntrance.ConsoleName.Equals(Constant.KEYWORDSMARTRESCUEPHONE5))
                                        {
                                            requiredVariables = stubVariables.Where(x => x.Id.Equals(Constants.SMARTRESCUEPHONE5SPPARAM)).ToList();
                                        }
                                        else
                                        {
                                            requiredVariables = stubVariables.Where(x => x.Id.Equals(Constants.SMARTRESCUEPHONE10SPPARAM)).ToList();
                                        }
                                        foreach (var variables in requiredVariables)
                                        {
                                            var varVariable = new Variables()
                                            {
                                                Id = variables.Id,
                                                Value = variables?.Values?.Count > 1 ? variables.Values[1].value : string.Empty
                                            };
                                            lstVariables.Add(varVariable);
                                        }
                                    }
                                    var stubProperties = JArray.Parse(File.ReadAllText(Constant.PROPERTIESTEMPLATE));
                                    List<PropertyDetailsValues> varProprties = stubProperties.ToObject<List<PropertyDetailsValues>>();
                                    foreach (var varProperty in varProprties)
                                    {
                                        switch (varProperty.Id)
                                        {
                                            case Constant.SEQUENCE:
                                                varProperty.Value = Constant.ONE;
                                                break;
                                            case Constant.SECTIONNAME:
                                                varProperty.Value = Constants.SMARTRESCUEPHONESTANDALONENAME;
                                                break;
                                        }
                                    }
                                    varSection.Properties = varProprties;
                                    totalAssignedGroup += varEntrance.AssignedGroups;
                                    totalAssignedUnit += varEntrance.AssignedUnits;
                                }
                            }
                        }
                        varSection.Variables = lstVariables;
                        if (totalAssignedUnit > 0)
                        {
                            varSection.AssignedGroups = totalAssignedGroup;
                            varSection.AssignedUnits = totalAssignedUnit;
                        }
                        subsection.sections.Add(varSection);
                        subsection.Variables = new List<Variables>();
                    }
                }
                else if (subsection.Id.Equals(Constants.THIRDPARTYINTERFACES))
                {
                    foreach (var sections in subsection.sections)
                    {
                        if (sections.Id.Equals(Constant.PARAMETERROBOTICCONTROLLERINTERFACE))
                        {
                            var stubVariables = sections.Variables;
                            if (BuildingEquipmentConsoles.Count > 0)
                            {
                                sections.sections = new List<SectionsGroupValues>();
                                foreach (var varEntrance in BuildingEquipmentConsoles)
                                {
                                    if (varEntrance.IsLobby == false && varEntrance.ConsoleName.Equals(Constant.KEYWORDROBOTICCONTROLLERINTERFACE))
                                    {
                                        if (varEntrance.ConsoleId > 0)
                                        {
                                            var varSection = new SectionsGroupValues()
                                            {
                                                assignOpenings = null,
                                                Id = Convert.ToString(varEntrance.ConsoleId),
                                                Name = Constant.BUILDINGEQUIPMENTCONSOLESECTION + varEntrance.ConsoleName.Replace(Constant.EMPTYSPACE, string.Empty),
                                                isController = varEntrance.IsController,
                                                isLobby = varEntrance.IsLobby,
                                                AssignedGroups = varEntrance.AssignedGroups,
                                                AssignedUnits = varEntrance.AssignedUnits,
                                                Variables = new List<Variables>(),
                                                Properties = new List<PropertyDetailsValues>()
                                            };
                                            List<Variables> lstVariables = new List<Variables>();
                                            var newflag = (from val in varEntrance.VariableAssignments
                                                           where val.Value.Equals(string.Empty)
                                                           select val).ToList();
                                            var flagnew = 0;
                                            if (varEntrance.VariableAssignments.Count() != newflag.Count())
                                            {
                                                flagnew = 1;
                                            }
                                            if (varEntrance.VariableAssignments.Count >= 1 && flagnew == 1)
                                            {
                                                foreach (var variableAssignment in varEntrance.VariableAssignments)
                                                {
                                                    var lstproperties = (from variable in stubVariables
                                                                         from property in variable.Properties
                                                                         where variable.Id.Equals(variableAssignment.VariableId)
                                                                         select property).ToList();
                                                    var displayname = string.Empty;
                                                    var sequence = string.Empty;
                                                    if (lstproperties.Count > 0)
                                                    {
                                                        var lstdisplayname = lstproperties.Where(x => x.Id == Constant.DISPLAYNAME).Select(x => x.Value).ToList();
                                                        if (lstdisplayname.Count > 0)
                                                        {
                                                            displayname = lstdisplayname[0].ToString();
                                                        }
                                                        var lstsequence = lstproperties.Where(x => x.Id == Constant.SEQUENCE).Select(x => x.Value).ToList();
                                                        if (lstsequence.Count > 0)
                                                        {
                                                            sequence = lstsequence[0].ToString();
                                                        }
                                                    }
                                                    var boolValue = variableAssignment.Value;
                                                    if (boolValue.Equals(Constant.True))
                                                    {
                                                        boolValue = true;
                                                    }
                                                    else if (boolValue.Equals(Constant.False))
                                                    {
                                                        boolValue = false;
                                                    }
                                                    var varVariable = new Variables()
                                                    {
                                                        Id = variableAssignment.VariableId,
                                                        Name = displayname,
                                                        Value = boolValue,
                                                        Sequence = sequence
                                                    };
                                                    lstVariables.Add(varVariable);
                                                }
                                                varSection.Variables = lstVariables;
                                            }
                                            else
                                            {
                                                foreach (var variables in stubVariables)
                                                {
                                                    var displayname = string.Empty;
                                                    var sequence = string.Empty;
                                                    var lstdisplayname = variables.Properties.Where(x => x.Id == Constant.DISPLAYNAME).Select(x => x.Value).ToList();
                                                    if (lstdisplayname.Count > 0)
                                                    {
                                                        displayname = lstdisplayname[0].ToString();
                                                    }
                                                    var lstsequence = variables.Properties.Where(x => x.Id == Constant.SEQUENCE).Select(x => x.Value).ToList();
                                                    if (lstsequence.Count > 0)
                                                    {
                                                        sequence = lstsequence[0].ToString();
                                                    }
                                                    var varVariable = new Variables()
                                                    {
                                                        Id = variables.Id,
                                                        Name = displayname,
                                                        Sequence = Constant.ONE,
                                                        Value = variables.Values.Count > 0 ? variables.Values[0].value : string.Empty
                                                    };
                                                    lstVariables.Add(varVariable);
                                                }
                                                varSection.Variables = lstVariables;
                                            }
                                            var stubProperties = JArray.Parse(File.ReadAllText(Constant.PROPERTIESTEMPLATE));
                                            List<PropertyDetailsValues> varProprties = stubProperties.ToObject<List<PropertyDetailsValues>>();
                                            foreach (var varProperty in varProprties)
                                            {
                                                switch (varProperty.Id)
                                                {
                                                    case Constant.SEQUENCE:
                                                        varProperty.Value = Constant.ONE;
                                                        break;
                                                    case Constant.SECTIONNAME:
                                                        varProperty.Value = varEntrance.ConsoleName;
                                                        break;
                                                }
                                            }
                                            varSection.Properties = varProprties;
                                            sections.sections.Add(varSection);
                                        }
                                        break;
                                    }
                                }
                                sections.Variables = new List<Variables>();
                            }
                        }
                        else if (sections.Id.Equals(Constant.PARAMETERBACNET))
                        {
                            var stubVariables = sections.Variables;
                            if (BuildingEquipmentConsoles.Count > 0)
                            {
                                sections.sections = new List<SectionsGroupValues>();
                                foreach (var varEntrance in BuildingEquipmentConsoles)
                                {
                                    if (varEntrance.IsLobby == false && varEntrance.ConsoleName.Equals(Constant.KEYWORDBACNET))
                                    {
                                        if (varEntrance.ConsoleId > 0)
                                        {
                                            var varSection = new SectionsGroupValues()
                                            {
                                                assignOpenings = null,
                                                Id = Convert.ToString(varEntrance.ConsoleId),
                                                Name = Constant.BUILDINGEQUIPMENTCONSOLESECTION + varEntrance.ConsoleName.Replace(Constant.EMPTYSPACE, string.Empty),
                                                isController = varEntrance.IsController,
                                                isLobby = varEntrance.IsLobby,
                                                AssignedGroups = varEntrance.AssignedGroups,
                                                AssignedUnits = varEntrance.AssignedUnits,
                                                Variables = new List<Variables>(),
                                                Properties = new List<PropertyDetailsValues>()
                                            };
                                            List<Variables> lstVariables = new List<Variables>();
                                            var newflag = (from val in varEntrance.VariableAssignments
                                                           where val.Value.Equals(string.Empty)
                                                           select val).ToList();
                                            var flagnew = 0;
                                            if (varEntrance.VariableAssignments.Count() != newflag.Count())
                                            {
                                                flagnew = 1;
                                            }
                                            if (varEntrance.VariableAssignments.Count >= 1 && flagnew == 1)
                                            {
                                                foreach (var variableAssignment in varEntrance.VariableAssignments)
                                                {
                                                    var lstproperties = (from variable in stubVariables
                                                                         from property in variable.Properties
                                                                         where variable.Id.Equals(variableAssignment.VariableId)
                                                                         select property).ToList();
                                                    var displayname = string.Empty;
                                                    var sequence = string.Empty;
                                                    if (lstproperties.Count > 0)
                                                    {
                                                        var lstdisplayname = lstproperties.Where(x => x.Id == Constant.DISPLAYNAME).Select(x => x.Value).ToList();
                                                        if (lstdisplayname.Count > 0)
                                                        {
                                                            displayname = lstdisplayname[0].ToString();
                                                        }
                                                        var lstsequence = lstproperties.Where(x => x.Id == Constant.SEQUENCE).Select(x => x.Value).ToList();
                                                        if (lstsequence.Count > 0)
                                                        {
                                                            sequence = lstsequence[0].ToString();
                                                        }
                                                    }
                                                    var boolValue = variableAssignment.Value;
                                                    if (boolValue.Equals(Constant.True))
                                                    {
                                                        boolValue = true;
                                                    }
                                                    else if (boolValue.Equals(Constant.False))
                                                    {
                                                        boolValue = false;
                                                    }
                                                    var varVariable = new Variables()
                                                    {
                                                        Id = variableAssignment.VariableId,
                                                        Name = displayname,
                                                        Value = boolValue,
                                                        Sequence = sequence
                                                    };
                                                    lstVariables.Add(varVariable);
                                                }
                                                varSection.Variables = lstVariables;
                                            }
                                            else
                                            {
                                                foreach (var variables in stubVariables)
                                                {
                                                    var displayname = string.Empty;
                                                    var sequence = string.Empty;
                                                    var lstdisplayname = variables.Properties.Where(x => x.Id == Constant.DISPLAYNAME).Select(x => x.Value).ToList();
                                                    if (lstdisplayname.Count > 0)
                                                    {
                                                        displayname = lstdisplayname[0].ToString();
                                                    }
                                                    var lstsequence = variables.Properties.Where(x => x.Id == Constant.SEQUENCE).Select(x => x.Value).ToList();
                                                    if (lstsequence.Count > 0)
                                                    {
                                                        sequence = lstsequence[0].ToString();
                                                    }
                                                    var varVariable = new Variables()
                                                    {
                                                        Id = variables.Id,
                                                        Name = displayname,
                                                        Sequence = Constant.ONE,
                                                        Value = variables.Values.Count > 0 ? variables.Values[0].value : ""
                                                    };
                                                    lstVariables.Add(varVariable);
                                                }
                                                varSection.Variables = lstVariables;
                                            }
                                            var stubProperties = JArray.Parse(File.ReadAllText(Constant.PROPERTIESTEMPLATE));
                                            List<PropertyDetailsValues> varProprties = stubProperties.ToObject<List<PropertyDetailsValues>>();
                                            foreach (var varProperty in varProprties)
                                            {
                                                switch (varProperty.Id)
                                                {
                                                    case Constant.SEQUENCE:
                                                        varProperty.Value = Constant.ONE;
                                                        break;
                                                    case Constant.SECTIONNAME:
                                                        varProperty.Value = Constants.BACNETNAME;
                                                        break;
                                                }
                                            }
                                            varSection.Properties = varProprties;
                                            sections.sections.Add(varSection);
                                        }
                                        break;
                                    }
                                }
                                sections.Variables = new List<Variables>();
                            }
                        }
                    }
                }

            }
            stubUnitConfigurationSubSectionResponseObj.Sections = Utility.DeserializeObjectValue<List<Sections>>(Utility.SerializeObjectValue(filteredVariables.sections));
            foreach (var items in filteredVariables.sections)
            {
                var mainSectionValues = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(stubUnitConfigurationSubSectionResponseObj.Sections[0].sections));
                items.sections = Utility.DeserializeObjectValue<IList<SectionsGroupValues>>(Utility.SerializeObjectValue(mainSectionValues));
                items.sections.Where(c => c.isLobby == false || true).FirstOrDefault().isLobby = null;
            }
            //condition for adding warning message if egress distance is greater than 75 feet from top or lowest landing
            var buildingConfigureResponse = baseConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var buildingConfigureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(buildingConfigureResponse.Arguments));
            var buildingConfigureDictionary = buildingConfigureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, string>>();
            var egressDistanceFlag = buildingConfigureDictionary[(buildingEquipmentContantsDictionary[Constants.EGRESSFLAG])].ToString();
            if (egressDistanceFlag != null && egressDistanceFlag.Equals(Constants.True, StringComparison.OrdinalIgnoreCase))
            {
                if (emergencySwitchSelected)
                {
                    stubUnitConfigurationMainResponseObj.EgressDistanceExceeded = true;
                }
            }
            //Checking whether all groups are assigned to any of the lobby panel, if no then throwing error
            var lobbyPanelConsoles = (from consoles in BuildingEquipmentConsoles
                                      where consoles.ConsoleName.Contains(Constant.KEYWORDLOBBYPANEL)
                                      select consoles.lstConfiguredGroups).ToList();
            if (lobbyPanelConsoles != null && lobbyPanelConsoles.Any())
            {
                var notSelectedGroups = (from groups in lobbyPanelConsoles
                                         from newGroups in groups
                                         where !newGroups.isChecked
                                         select newGroups.groupId).ToList();
                if (notSelectedGroups != null && notSelectedGroups.Any())
                {
                    stubUnitConfigurationMainResponseObj.AllGroupsNotAssigned = true;
                }
            }
            stubUnitConfigurationMainResponseObj.EnrichedData = JObject.Parse(File.ReadAllText(Constant.BUILDINGENRICHEDDATA));
            stubUnitConfigurationMainResponseObj.Permissions = permission;
            stubUnitConfigurationMainResponseObj.Sections = Utility.DeserializeObjectValue<List<Sections>>(Utility.SerializeObjectValue(filteredVariables.sections));
            if (configureRequest != null)
            {
                return new ResponseMessage
                {
                    StatusCode = Constant.SUCCESS,
                    //Response = Utility.FilterNullValues(configResponse)
                    Response = Utility.FilterNullValues(stubUnitConfigurationMainResponseObj)
                };
            }
            Utility.LogEnd(methodBeginTime);
            throw new CustomException(new ResponseMessage
            {
                StatusCode = Constant.BADREQUEST,
                Message = _localizer[Constant.REQUESTCANNOTBENULL]
            });
        }

        public async Task<JObject> ChangeBuildingEquipmentConfigureBl(JObject variableAssignments, string sessionId, int buildingId)
        {
            var methodBeginTime = Utility.LogBegin();
            var BuildingEquipmentConsoles = SetCacheBuildingEquipmentConsoles(null, sessionId, buildingId, false);
            var buildingEquipmentContantsDictionary = Utility.GetVariableMapping(Constants.BUILDINGMAPPERVARIABLESMAPPERPATH, Constants.BUILDINGEQUIPMENTVARIABLES);
            var fixtureStrategy = SetCacheFixtureStrategy(null, sessionId, buildingId);
            var configureRequest = CreateConfigurationRequestWithTemplate(variableAssignments, Constant.BUILDINGEQUIPMENT);
            var variableAssignmentList = Utility.DeserializeObjectValue<Line>(Utility.SerializeObjectValue(variableAssignments));
            foreach (var variable in variableAssignmentList.VariableAssignments)
            {
                if (fixtureStrategy.Any(x => x.VariableId == variable.VariableId))
                {
                    fixtureStrategy.Where(x => x.VariableId == variable.VariableId).First().Value = variable.Value;
                }
                else
                {
                    if (Utility.CheckEquals(variable.VariableId, Constant.INTERGROUPEMEGENCYPOWER))
                    {
                        variable.Value = (Convert.ToString(variable.Value).ToUpper());
                    }
                    fixtureStrategy.Add(variable);
                }

            }
            configureRequest.Line.VariableAssignments = fixtureStrategy;
            fixtureStrategy = SetCacheFixtureStrategy(fixtureStrategy, sessionId, buildingId);
            var mainGroupConfigurationResponse = new ConfigurationResponse
            {
                Sections = new List<Sections>()
            };
            var packagePath = configureRequest?.PackagePath;
            // needed code pls don't remove it
            if (configureRequest == null)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = _localizer[Constant.REQUESTCANNOTBENULL + Constant.HYPHEN]
                });
            }
            // adding required include section values
            configureRequest = GenerateIncludeSections(configureRequest, Constant.BUILDINGEQUIPMENT);
            //Entarnce Console configuratioon fetching from cache
            //Adding include sections in request body
            var configureResponseJObj =
                await ConfigurationBl(configureRequest, packagePath, sessionId).ConfigureAwait(false);
            var configureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var baseConfigureResponse = configureResponseJObj.Response.ToObject<ConfigurationResponse>();
            // configuration object values for conflict mapping
            var ActualConfigureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = ActualConfigureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            // adding defaults to cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTBUILDINGEQUIPMENTCONFIGVALUES, Utility.SerializeObjectValue(configureResponseArgumentJObject));
            var stubUnitConfigurationResponseObj = new ConfigurationResponse();
            var stubUnitConfigurationSubSectionResponseObj = new ConfigurationResponse();
            var stubUnitConfigurationMainResponseObj = new ConfigurationResponse();
            // Main Stub 
            //Sub Section Stub Path
            var BuildingEquipmentUITemplate = JObject.Parse(File.ReadAllText(Constant.BUILDINGEQUIPMENTUITEMPLATE));
            // setting stub data into an sectionsValues object
            stubUnitConfigurationSubSectionResponseObj = BuildingEquipmentUITemplate.ToObject<ConfigurationResponse>();
            var filteredSection = Utility.MapVariables(Utility.SerializeObjectValue(baseConfigureResponse), Utility.SerializeObjectValue(stubUnitConfigurationSubSectionResponseObj));
            var filteredVariables = Utility.DeserializeObjectValue<Sections>(filteredSection);
            //BuildingEquipment Console Code
            foreach (var subsection in filteredVariables.sections[0].sections)
            {
                if (subsection.Id.Equals(Constant.PARAMETERLOBBYPANEL))
                {
                    var stubVariables = subsection.Variables;
                    if (BuildingEquipmentConsoles.Count > 0)
                    {
                        subsection.sections = new List<SectionsValues>();
                        foreach (var varEntrance in BuildingEquipmentConsoles)
                        {
                            var lstLobby = BuildingEquipmentConsoles.Where(x => x.ConsoleName.Contains(Constant.KEYWORDLOBBYPANEL)).ToList();
                            int count = 0;
                            foreach (var looby in lstLobby)
                            {
                                count++;
                                if (looby.IsLobby == true && looby.ConsoleName.Contains(Constant.KEYWORDLOBBYPANEL))
                                {
                                    if (looby.ConsoleId > 0)
                                    {
                                        var varSection = new SectionsValues()
                                        {
                                            assignOpenings = null,
                                            Id = looby.ConsoleNumber.ToString(),
                                            Name = Constant.BUILDINGEQUIPMENTCONSOLESECTION + looby.ConsoleName.Replace(Constant.EMPTYSPACE, string.Empty),
                                            isController = looby.IsController,
                                            IsDelete = true,
                                            isLobby = looby.IsLobby,
                                            AssignedGroups = looby.AssignedGroups,
                                            AssignedUnits = looby.AssignedUnits,
                                            Variables = new List<Variables>(),
                                            Properties = new List<PropertyDetailsValues>()
                                        };
                                        List<Variables> lstVariables = new List<Variables>();
                                        var newflag = (from val in varEntrance.VariableAssignments
                                                       where val.Value.Equals(string.Empty)
                                                       select val).ToList();
                                        var flagnew = 0;
                                        if (varEntrance.VariableAssignments.Count() != newflag.Count())
                                        {
                                            flagnew = 1;
                                        }
                                        if (looby.VariableAssignments.Count >= 1 && flagnew == 1)
                                        {
                                            foreach (var variableAssignment in looby.VariableAssignments)
                                            {
                                                var lstproperties = (from variable in stubVariables
                                                                     from property in variable.Properties
                                                                     where variable.Id.Equals(variableAssignment.VariableId)
                                                                     select property).ToList();
                                                var displayname = string.Empty;
                                                var sequence = string.Empty;
                                                if (lstproperties.Count > 0)
                                                {
                                                    var lstdisplayname = lstproperties.Where(x => x.Id == Constant.DISPLAYNAME).Select(x => x.Value).ToList();
                                                    if (lstdisplayname.Count > 0)
                                                    {
                                                        displayname = lstdisplayname[0].ToString();
                                                    }
                                                    var lstsequence = lstproperties.Where(x => x.Id == Constant.SEQUENCE).Select(x => x.Value).ToList();
                                                    if (lstsequence.Count > 0)
                                                    {
                                                        sequence = lstsequence[0].ToString();
                                                    }
                                                }
                                                var boolValue = variableAssignment.Value;
                                                if (boolValue.Equals(Constant.True))
                                                {
                                                    boolValue = true;
                                                }
                                                else if (boolValue.Equals(Constant.False))
                                                {
                                                    boolValue = false;
                                                }
                                                var varVariable = new Variables()
                                                {
                                                    Id = variableAssignment.VariableId,
                                                    Name = displayname,
                                                    Value = boolValue,
                                                    Sequence = sequence
                                                };
                                                lstVariables.Add(varVariable);
                                            }
                                            varSection.Variables = lstVariables;
                                        }
                                        else
                                        {
                                            foreach (var variables in stubVariables)
                                            {
                                                var displayname = string.Empty;
                                                var sequence = string.Empty;
                                                var lstdisplayname = variables.Properties.Where(x => x.Id == Constant.DISPLAYNAME).Select(x => x.Value).ToList();
                                                if (lstdisplayname.Count > 0)
                                                {
                                                    displayname = lstdisplayname[0].ToString();
                                                }
                                                var lstsequence = variables.Properties.Where(x => x.Id == Constant.SEQUENCE).Select(x => x.Value).ToList();
                                                if (lstsequence.Count > 0)
                                                {
                                                    sequence = lstsequence[0].ToString();
                                                }
                                                var varVariable = new Variables()
                                                {
                                                    Id = variables.Id,
                                                    Name = displayname,
                                                    Sequence = sequence,
                                                    Value = variables.Values.Count > 0 ? variables.Values[0].value : string.Empty
                                                };
                                                lstVariables.Add(varVariable);
                                            }
                                            varSection.Variables = lstVariables;
                                        }
                                        var stubProperties = JArray.Parse(File.ReadAllText(Constant.PROPERTIESTEMPLATE));
                                        List<PropertyDetailsValues> varProprties = stubProperties.ToObject<List<PropertyDetailsValues>>();
                                        foreach (var varProperty in varProprties)
                                        {
                                            switch (varProperty.Id)
                                            {
                                                case Constant.SEQUENCE:
                                                    varProperty.Value = count;
                                                    break;
                                                case Constant.SECTIONNAME:
                                                    varProperty.Value = looby.ConsoleName;
                                                    break;
                                            }
                                        }
                                        varSection.Properties = varProprties;
                                        subsection.sections.Add(varSection);
                                    }
                                }
                            }
                            break;
                        }
                        subsection.Variables = new List<Variables>();
                    }
                }
                else if (subsection.Id.Equals(Constants.SMARTRESCUEPHONESTANDALONE))
                {
                    var stubVariables = subsection.Variables;
                    if (BuildingEquipmentConsoles.Count > 0)
                    {
                        subsection.sections = new List<SectionsValues>();
                        var totalAssignedGroup = 0;
                        var totalAssignedUnit = 0;
                        var varSection = new SectionsValues()
                        {
                            assignOpenings = null,
                            Id = Constants.IDTWO,
                            Name = Constants.SMARTRESCUEPHONESTANDALONENAME,
                            isController = true,
                            isLobby = false,
                            Variables = new List<Variables>(),
                            Properties = new List<PropertyDetailsValues>()
                        };
                        List<Variables> lstVariables = new List<Variables>();
                        foreach (var varEntrance in BuildingEquipmentConsoles)
                        {
                            if (varEntrance.IsLobby == false && (varEntrance.ConsoleName.Equals(Constant.KEYWORDSMARTRESCUEPHONE5) || varEntrance.ConsoleName.Equals(Constant.KEYWORDSMARTRESCUEPHONE10)))
                            {
                                if (varEntrance.ConsoleId > 0)
                                {
                                    var newflag = (from val in varEntrance.VariableAssignments
                                                   where val.Value.Equals(string.Empty)
                                                   select val).ToList();
                                    var flagnew = 0;
                                    if (varEntrance.VariableAssignments.Count() != newflag.Count())
                                    {
                                        flagnew = 1;
                                    }
                                    if (varEntrance.VariableAssignments.Count >= 1 && flagnew == 1)
                                    {
                                        foreach (var variableAssignment in varEntrance.VariableAssignments)
                                        {
                                            var boolValue = variableAssignment.Value;
                                            if (boolValue.Equals(Constant.True))
                                            {
                                                boolValue = true;
                                            }
                                            else if (boolValue.Equals(Constant.False))
                                            {
                                                boolValue = false;
                                            }
                                            var varVariable = new Variables()
                                            {
                                                Id = variableAssignment.VariableId,
                                                Value = boolValue
                                            };
                                            lstVariables.Add(varVariable);
                                        }
                                    }
                                    else
                                    {
                                        var requiredVariables = stubVariables;
                                        if (varEntrance.ConsoleName.Equals(Constant.KEYWORDSMARTRESCUEPHONE5))
                                        {
                                            requiredVariables = stubVariables.Where(x => x.Id.Equals(Constants.SMARTRESCUEPHONE5SPPARAM)).ToList();
                                        }
                                        else
                                        {
                                            requiredVariables = stubVariables.Where(x => x.Id.Equals(Constants.SMARTRESCUEPHONE10SPPARAM)).ToList();
                                        }
                                        foreach (var variables in requiredVariables)
                                        {
                                            var varVariable = new Variables()
                                            {
                                                Id = variables.Id,
                                                Value = variables?.Values?.Count > 0 ? variables.Values[1].value : string.Empty
                                            };
                                            lstVariables.Add(varVariable);
                                        }
                                    }
                                    var stubProperties = JArray.Parse(File.ReadAllText(Constant.PROPERTIESTEMPLATE));
                                    List<PropertyDetailsValues> varProprties = stubProperties.ToObject<List<PropertyDetailsValues>>();
                                    foreach (var varProperty in varProprties)
                                    {
                                        switch (varProperty.Id)
                                        {
                                            case Constant.SEQUENCE:
                                                varProperty.Value = Constant.ONE;
                                                break;
                                            case Constant.SECTIONNAME:
                                                varProperty.Value = Constants.SMARTRESCUEPHONESTANDALONENAME;
                                                break;
                                        }
                                    }
                                    varSection.Properties = varProprties;
                                    totalAssignedGroup += varEntrance.AssignedGroups;
                                    totalAssignedUnit += varEntrance.AssignedUnits;
                                }
                            }
                        }
                        varSection.Variables = lstVariables;
                        if (totalAssignedUnit > 0)
                        {
                            varSection.AssignedGroups = totalAssignedGroup;
                            varSection.AssignedUnits = totalAssignedUnit;
                        }
                        subsection.sections.Add(varSection);
                        subsection.Variables = new List<Variables>();
                    }
                }
                else if (subsection.Id.Equals(Constants.THIRDPARTYINTERFACES))
                {
                    foreach (var sections in subsection.sections)
                    {
                        if (sections.Id.Equals(Constant.PARAMETERROBOTICCONTROLLERINTERFACE))
                        {
                            var stubVariables = sections.Variables;
                            if (BuildingEquipmentConsoles.Count > 0)
                            {
                                sections.sections = new List<SectionsGroupValues>();
                                foreach (var varEntrance in BuildingEquipmentConsoles)
                                {
                                    if (varEntrance.IsLobby == false && varEntrance.ConsoleName.Equals(Constant.KEYWORDROBOTICCONTROLLERINTERFACE))
                                    {
                                        if (varEntrance.ConsoleId > 0)
                                        {
                                            var varSection = new SectionsGroupValues()
                                            {
                                                assignOpenings = null,
                                                Id = Convert.ToString(varEntrance.ConsoleId),
                                                Name = Constant.BUILDINGEQUIPMENTCONSOLESECTION + varEntrance.ConsoleName.Replace(Constant.EMPTYSPACE, string.Empty),
                                                isController = varEntrance.IsController,
                                                isLobby = varEntrance.IsLobby,
                                                AssignedGroups = varEntrance.AssignedGroups,
                                                AssignedUnits = varEntrance.AssignedUnits,
                                                Variables = new List<Variables>(),
                                                Properties = new List<PropertyDetailsValues>()
                                            };
                                            List<Variables> lstVariables = new List<Variables>();
                                            var newflag = (from val in varEntrance.VariableAssignments
                                                           where val.Value.Equals(string.Empty)
                                                           select val).ToList();
                                            var flagnew = 0;
                                            if (varEntrance.VariableAssignments.Count() != newflag.Count())
                                            {
                                                flagnew = 1;
                                            }
                                            if (varEntrance.VariableAssignments.Count >= 1 && flagnew == 1)
                                            {
                                                foreach (var variableAssignment in varEntrance.VariableAssignments)
                                                {
                                                    var lstproperties = (from variable in stubVariables
                                                                         from property in variable.Properties
                                                                         where variable.Id.Equals(variableAssignment.VariableId)
                                                                         select property).ToList();
                                                    var displayname = string.Empty;
                                                    var sequence = string.Empty;
                                                    if (lstproperties.Count > 0)
                                                    {
                                                        var lstdisplayname = lstproperties.Where(x => x.Id == Constant.DISPLAYNAME).Select(x => x.Value).ToList();
                                                        if (lstdisplayname.Count > 0)
                                                        {
                                                            displayname = lstdisplayname[0].ToString();
                                                        }
                                                        var lstsequence = lstproperties.Where(x => x.Id == Constant.SEQUENCE).Select(x => x.Value).ToList();
                                                        if (lstsequence.Count > 0)
                                                        {
                                                            sequence = lstsequence[0].ToString();
                                                        }
                                                    }
                                                    var boolValue = variableAssignment.Value;
                                                    if (boolValue.Equals(Constant.True))
                                                    {
                                                        boolValue = true;
                                                    }
                                                    else if (boolValue.Equals(Constant.False))
                                                    {
                                                        boolValue = false;
                                                    }
                                                    var varVariable = new Variables()
                                                    {
                                                        Id = variableAssignment.VariableId,
                                                        Name = displayname,
                                                        Value = boolValue,
                                                        Sequence = sequence
                                                    };
                                                    lstVariables.Add(varVariable);
                                                }
                                                varSection.Variables = lstVariables;
                                            }
                                            else
                                            {
                                                foreach (var variables in stubVariables)
                                                {
                                                    var displayname = string.Empty;
                                                    var sequence = string.Empty;
                                                    var lstdisplayname = variables.Properties.Where(x => x.Id == Constant.DISPLAYNAME).Select(x => x.Value).ToList();
                                                    if (lstdisplayname.Count > 0)
                                                    {
                                                        displayname = lstdisplayname[0].ToString();
                                                    }
                                                    var lstsequence = variables.Properties.Where(x => x.Id == Constant.SEQUENCE).Select(x => x.Value).ToList();
                                                    if (lstsequence.Count > 0)
                                                    {
                                                        sequence = lstsequence[0].ToString();
                                                    }
                                                    var varVariable = new Variables()
                                                    {
                                                        Id = variables.Id,
                                                        Name = displayname,
                                                        Sequence = Constant.ONE,
                                                        Value = variables.Values.Count > 0 ? variables.Values[0].value : string.Empty
                                                    };
                                                    lstVariables.Add(varVariable);
                                                }
                                                varSection.Variables = lstVariables;
                                            }
                                            var stubProperties = JArray.Parse(File.ReadAllText(Constant.PROPERTIESTEMPLATE));
                                            List<PropertyDetailsValues> varProprties = stubProperties.ToObject<List<PropertyDetailsValues>>();
                                            foreach (var varProperty in varProprties)
                                            {
                                                switch (varProperty.Id)
                                                {
                                                    case Constant.SEQUENCE:
                                                        varProperty.Value = Constant.ONE;
                                                        break;
                                                    case Constant.SECTIONNAME:
                                                        varProperty.Value = varEntrance.ConsoleName;
                                                        break;
                                                }
                                            }
                                            varSection.Properties = varProprties;
                                            sections.sections.Add(varSection);
                                        }
                                        break;
                                    }
                                }
                                sections.Variables = new List<Variables>();
                            }
                        }
                        else if (sections.Id.Equals(Constant.PARAMETERBACNET))
                        {
                            var stubVariables = sections.Variables;
                            if (BuildingEquipmentConsoles.Count > 0)
                            {
                                sections.sections = new List<SectionsGroupValues>();
                                foreach (var varEntrance in BuildingEquipmentConsoles)
                                {
                                    if (varEntrance.IsLobby == false && varEntrance.ConsoleName.Equals(Constant.KEYWORDBACNET))
                                    {
                                        if (varEntrance.ConsoleId > 0)
                                        {
                                            var varSection = new SectionsGroupValues()
                                            {
                                                assignOpenings = null,
                                                Id = Convert.ToString(varEntrance.ConsoleId),
                                                Name = Constant.BUILDINGEQUIPMENTCONSOLESECTION + varEntrance.ConsoleName.Replace(Constant.EMPTYSPACE, string.Empty),
                                                isController = varEntrance.IsController,
                                                isLobby = varEntrance.IsLobby,
                                                AssignedGroups = varEntrance.AssignedGroups,
                                                AssignedUnits = varEntrance.AssignedUnits,
                                                Variables = new List<Variables>(),
                                                Properties = new List<PropertyDetailsValues>()
                                            };
                                            List<Variables> lstVariables = new List<Variables>();
                                            var newflag = (from val in varEntrance.VariableAssignments
                                                           where val.Value.Equals(string.Empty)
                                                           select val).ToList();
                                            var flagnew = 0;
                                            if (varEntrance.VariableAssignments.Count() != newflag.Count())
                                            {
                                                flagnew = 1;
                                            }
                                            if (varEntrance.VariableAssignments.Count >= 1 && flagnew == 1)
                                            {
                                                foreach (var variableAssignment in varEntrance.VariableAssignments)
                                                {
                                                    var lstproperties = (from variable in stubVariables
                                                                         from property in variable.Properties
                                                                         where variable.Id.Equals(variableAssignment.VariableId)
                                                                         select property).ToList();
                                                    var displayname = string.Empty;
                                                    var sequence = string.Empty;
                                                    if (lstproperties.Count > 0)
                                                    {
                                                        var lstdisplayname = lstproperties.Where(x => x.Id == Constant.DISPLAYNAME).Select(x => x.Value).ToList();
                                                        if (lstdisplayname.Count > 0)
                                                        {
                                                            displayname = lstdisplayname[0].ToString();
                                                        }
                                                        var lstsequence = lstproperties.Where(x => x.Id == Constant.SEQUENCE).Select(x => x.Value).ToList();
                                                        if (lstsequence.Count > 0)
                                                        {
                                                            sequence = lstsequence[0].ToString();
                                                        }
                                                    }
                                                    var boolValue = variableAssignment.Value;
                                                    if (boolValue.Equals(Constant.True))
                                                    {
                                                        boolValue = true;
                                                    }
                                                    else if (boolValue.Equals(Constant.False))
                                                    {
                                                        boolValue = false;
                                                    }
                                                    var varVariable = new Variables()
                                                    {
                                                        Id = variableAssignment.VariableId,
                                                        Name = displayname,
                                                        Value = boolValue,
                                                        Sequence = sequence
                                                    };
                                                    lstVariables.Add(varVariable);
                                                }
                                                varSection.Variables = lstVariables;
                                            }
                                            else
                                            {
                                                foreach (var variables in stubVariables)
                                                {
                                                    var displayname = string.Empty;
                                                    var sequence = string.Empty;
                                                    var lstdisplayname = variables.Properties.Where(x => x.Id == Constant.DISPLAYNAME).Select(x => x.Value).ToList();
                                                    if (lstdisplayname.Count > 0)
                                                    {
                                                        displayname = lstdisplayname[0].ToString();
                                                    }
                                                    var lstsequence = variables.Properties.Where(x => x.Id == Constant.SEQUENCE).Select(x => x.Value).ToList();
                                                    if (lstsequence.Count > 0)
                                                    {
                                                        sequence = lstsequence[0].ToString();
                                                    }
                                                    var varVariable = new Variables()
                                                    {
                                                        Id = variables.Id,
                                                        Name = displayname,
                                                        Sequence = Constant.ONE,
                                                        Value = variables.Values.Count > 0 ? variables.Values[0].value : ""
                                                    };
                                                    lstVariables.Add(varVariable);
                                                }
                                                varSection.Variables = lstVariables;
                                            }
                                            var stubProperties = JArray.Parse(File.ReadAllText(Constant.PROPERTIESTEMPLATE));
                                            List<PropertyDetailsValues> varProprties = stubProperties.ToObject<List<PropertyDetailsValues>>();
                                            foreach (var varProperty in varProprties)
                                            {
                                                switch (varProperty.Id)
                                                {
                                                    case Constant.SEQUENCE:
                                                        varProperty.Value = Constant.ONE;
                                                        break;
                                                    case Constant.SECTIONNAME:
                                                        varProperty.Value = Constants.BACNETNAME;
                                                        break;
                                                }
                                            }
                                            varSection.Properties = varProprties;
                                            sections.sections.Add(varSection);
                                        }
                                        break;
                                    }
                                }
                                sections.Variables = new List<Variables>();
                            }
                        }
                    }
                }

            }
            stubUnitConfigurationSubSectionResponseObj.Sections = Utility.DeserializeObjectValue<List<Sections>>(Utility.SerializeObjectValue(filteredVariables.sections));
            foreach (var items in filteredVariables.sections)
            {
                var mainSectionValues = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(stubUnitConfigurationSubSectionResponseObj.Sections[0].sections));
                items.sections = Utility.DeserializeObjectValue<IList<SectionsGroupValues>>(Utility.SerializeObjectValue(mainSectionValues));
                items.sections.Where(c => c.isLobby == false || true).FirstOrDefault().isLobby = null;
            }
            var buildingConfigureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponse.Arguments));
            var buildingConfigureDictionary = buildingConfigureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, string>>();
            var egressDistanceFlag = buildingConfigureDictionary[(buildingEquipmentContantsDictionary[Constants.EGRESSFLAG])].ToString();
            if (egressDistanceFlag != null && egressDistanceFlag.Equals(Constants.True, StringComparison.OrdinalIgnoreCase))
            {
                stubUnitConfigurationMainResponseObj.EgressDistanceExceeded = true;
            }
            //Checking whether all groups are assigned to any of the lobby panel, if no then throwing error
            var lobbyPanelConsoles = (from consoles in BuildingEquipmentConsoles
                                      where consoles.ConsoleName.Contains(Constant.KEYWORDLOBBYPANEL)
                                      select consoles.lstConfiguredGroups).ToList();
            if (lobbyPanelConsoles != null && lobbyPanelConsoles.Any())
            {
                var notSelectedGroups = (from groups in lobbyPanelConsoles
                                         from newGroups in groups
                                         where !newGroups.isChecked
                                         select newGroups.groupId).ToList();
                if (notSelectedGroups != null && notSelectedGroups.Any())
                {
                    stubUnitConfigurationMainResponseObj.AllGroupsNotAssigned = true;
                }
            }
            stubUnitConfigurationMainResponseObj.Sections = Utility.DeserializeObjectValue<List<Sections>>(Utility.SerializeObjectValue(filteredVariables.sections));
            Utility.LogEnd(methodBeginTime);
            return Utility.FilterNullValues(stubUnitConfigurationMainResponseObj);
        }



        /// <summary>
        /// CreateConfigurationRequestWithTemplate
        /// </summary>
        /// <param Name="varibleAssignments"></param>
        /// <param Name="typeOfConfiguration"></param>
        /// <param Name="varRearOpen"></param>
        /// <returns></returns>

        public ConfigurationRequest CreateConfigurationRequestWithTemplate(JObject varibleAssignments, string typeOfConfiguration, List<VariableAssignment> varRearOpen = null, string productType = null)
        {
            var methodBeginTime = Utility.LogBegin();
            //creating req body
            String stubReqbody;
            JObject requestBodyAssignmentsObj = new JObject();
            var configurationRequest = new ConfigurationRequest();
            switch (typeOfConfiguration)
            {
                case Constant.GROUPCONFIGURATIONNAME:
                    configurationRequest = CreateConfigurationRequestForProducts(Constants.GROUPCONFIGURATIONREQESTBODYSTUBPATH, Constant.GROUPCONFIGURATIONPATH);

                    break;
                case Constant.UNITCONFIG:
                    if (productType.Equals(Constant.EVO_200))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(Constant.UNITCONFIGURATIONREQESTBODYSTUBPATH, Constant.UNITVALIDATIONPATH);
                    }
                    else if (productType.Equals(Constant.ENDURA_100))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(Constants.UNITCONFIGURATIONREQESTBODYSTUBPATHENDURA100, Constant.UNITVALIDATIONENDURA100PATH);
                    }
                    if (productType.Equals(Constant.EVO_100))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(Constants.UNITCONFIGURATIONREQESTBODYSTUBPATHFOREVO100, Constant.UNITVALIDATIONPATHEVO100PATH);
                    }
                    else if (productType.Equals(Constant.CEGEARLESS))
                    {
                        configurationRequest = Utility.GetRequestBodyWithAssignments(Constant.CUSTOMENGINEEREDREQESTBODYSTUBPATH, _configuration[Constant.GEARLESSPACKAGEID], _configuration[Constant.CUSTOMENGINEEREDGEARLESSPATH]);
                    }
                    else if (productType.Equals(Constant.CEGEARED))
                    {
                        configurationRequest = Utility.GetRequestBodyWithAssignments(Constant.CUSTOMENGINEEREDREQESTBODYSTUBPATH, _configuration[Constant.GEAREDPACKAGEID], _configuration[Constant.CUSTOMENGINEEREDGEAREDPATH]);
                    }
                    else if (productType.Equals(Constant.CEHYDRAULIC))
                    {
                        configurationRequest = Utility.GetRequestBodyWithAssignments(Constant.CUSTOMENGINEEREDREQESTBODYSTUBPATH, _configuration[Constant.HYDRAULICPACKAGEID], _configuration[Constant.CUSTOMENGINEEREDHYDRAULICPATH]);
                    }
                    else if (productType.Equals(Constant.SYNERGY))
                    {
                        configurationRequest = Utility.GetRequestBodyWithAssignments(Constant.CUSTOMENGINEEREDREQESTBODYSTUBPATH, _configuration[Constant.SYNERGYPACKAGEID], _configuration[Constant.SYNERGYPATH]);
                    }
                    break;
                case Constant.PRODUCTSELECTION:
                    configurationRequest = CreateConfigurationRequestForProducts(Constant.PRODUCTSELECTIONCLMREQUESTTEMPLATE, Constant.PRODUCTSELECTIONPATH);
                    break;
                case Constant.PRODUCTTREE:
                    configurationRequest = CreateConfigurationRequestForProducts(Constant.PRODUCTTREESTUBPATH, Constant.PRODUCTTREEPATH);
                    break;
                case Constant.PRODUCTSELECTIONINVALIDEVO200:
                    configurationRequest = CreateConfigurationRequestForProducts(Constant.PRODUCTSELECTIONINVALIDEVO200STUBPATH, Constant.PRODUCTSELECTIONPATH);
                    break;
                case Constant.PRODUCTSELECTIONUNITLEVELVALIDATION:
                    configurationRequest = CreateConfigurationRequestForProducts(Constant.PRODUCTSELECTIONUNITLEVELVALIDATIONREQUESTBODYPATHPATH, Constant.GROUPCONFIGURATIONPATH);
                    break;
                case Constant.PRODUCTSELECTIONGROUPLEVELVALIDATIONEVO200:
                    stubReqbody = JObject.Parse(System.IO.File.ReadAllText(Constant.PRODUCTSELECTIONGROUPLEVELVALIDATIONEVO200REQUESTBODYPATHPATH)).ToString();
                    configurationRequest = Utility.DeserializeObjectValue<ConfigurationRequest>(stubReqbody);
                    break;
                case Constant.GROUPDEFAULTSCLMCALL:
                    configurationRequest = CreateConfigurationRequestForProducts(Constant.GROUPCONFIGURATIONREQESTBODYSTUBPATH, Constant.GROUPCONFIGURATIONPATH);
                    break;
                case Constant.BUILDINGDEFAULTSCLMCALL:
                    configurationRequest = CreateConfigurationRequestForProducts(Constant.BUILDINGDEFAULTSCLMCALLFILEPATH, Constant.BUILDINGCONFIGURATIONPATH);
                    break;
                case Constant.UNITDEFAULTSCLMCALL:
                    if (!String.IsNullOrEmpty(productType) && productType.Equals(Constants.EVO_200))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(Constant.UNITDEFAULTSCLMCALLFILEPATH, Constant.UNITVALIDATIONPATH);
                    }
                    else if (!String.IsNullOrEmpty(productType) && productType.Equals(Constants.EVO_100))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(Constant.UNITDEFAULTSCLMCALLFILEPATHEVO100, Constant.UNITVALIDATIONPATHEVO100PATH);
                    }
                    else if (!String.IsNullOrEmpty(productType) && productType.Equals(Constants.ENDURA_100))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(String.Format(Constant.SYSTEMVALIDATIONBASEREQUEST, Constants.END100), Constant.UNITVALIDATIONENDURA100PATH);
                    }
                    break;
                case Constant.LIFTDESIGNERDEFAULTSCLMCALL:
                    if (productType!= null && productType.Equals(Constants.EVO_200))
                    {
                        configurationRequest = Utility.GetRequestBodyWithAssignments(Constant.CLMREQESTBODYSTUBPATH, _configuration[Constant.LDPACKAGEID], _configuration[Constant.LIFTDESIGNERPATH]);
                    }
                    else if (productType != null && productType.Equals(Constants.EVO_100))
                    {
                        configurationRequest = Utility.GetRequestBodyWithAssignments(Constant.CLMREQESTBODYSTUBPATH, _configuration[Constant.LDPACKAGEIDEVO100], _configuration[Constant.LIFTDESIGNERPATHEVO100]);
                    }
                    break;
                case Constant.LIFTDESIGNERHEATDEFAULTSCLMCALL:
                    if (productType != null && productType.Equals(Constants.EVO_200))
                    {
                        configurationRequest = Utility.GetRequestBodyWithAssignments(Constant.CLMREQESTBODYSTUBPATH, _configuration[Constant.LDHEATPACKAGEID], _configuration[Constant.LIFTDESIGNERPATH]);
                    }
                    else if (productType != null && productType.Equals(Constants.EVO_100))
                    {
                        configurationRequest = Utility.GetRequestBodyWithAssignments(Constant.CLMREQESTBODYSTUBPATH, _configuration[Constant.LDHEATPACKAGEID], _configuration[Constant.LIFTDESIGNERPATHEVO100]);
                    }
                    configurationRequest.Settings.IncludeSections = new List<string>();
                    break;
                case Constant.LIFTDESIGNERBRACKETDEFAULTSCLMCALL:
                    if (productType != null && productType.Equals(Constants.EVO_200))
                    {
                        configurationRequest = Utility.GetRequestBodyWithAssignments(Constant.CLMREQESTBODYSTUBPATH, _configuration[Constant.LDBRACKETPACKAGEID], _configuration[Constant.LIFTDESIGNERPATH]);
                    }
                    else if (productType != null && productType.Equals(Constants.EVO_100))
                    {
                        configurationRequest = Utility.GetRequestBodyWithAssignments(Constant.CLMREQESTBODYSTUBPATH, _configuration[Constant.LDBRACKETPACKAGEID], _configuration[Constant.LIFTDESIGNERPATHEVO100]);
                    }
                    configurationRequest.Settings.IncludeSections = new List<string>();
                    break;
                case Constant.LDVALIDATIONDEFAULTSCLMCALL:
                    configurationRequest = Utility.GetRequestBodyWithAssignments(Constant.CLMREQESTBODYSTUBPATH, _configuration[Constant.LDVALIDATIONPACKAGEID], _configuration[Constant.LDVALIDATIONPATH]);
                    break;
                case Constant.SYSTEMVALIDATIONSLINGCALL:
                    if(!String.IsNullOrEmpty(productType) && productType.Equals(Constants.EVO_200))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(string.Format(Constant.SYSTEMVALIDATIONBASEREQUEST, Constant.EVOLUTION200), Constant.SLINGVALIDATIONPATH);
                        configurationRequest = GetConfigurationMatrialValues(configurationRequest, Constant.CABVALIDATIONPATH);
                    }
                    else if(!String.IsNullOrEmpty(productType) && productType.Equals(Constants.EVO_100))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(string.Format(Constant.SYSTEMVALIDATIONBASEREQUEST, Constant.EVOLUTION__100), Constant.SLINGVALIDATIONPATH);
                        configurationRequest = GetConfigurationMatrialValues(configurationRequest, Constants.SLINGVALIDATIONSUBPATHEVO100);
                    }
                    else if (!String.IsNullOrEmpty(productType) && productType.Equals(Constants.ENDURA_100))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(string.Format(Constant.SYSTEMVALIDATIONBASEREQUEST, Constant.END100), Constants.SLINGVALIDATIONPATHEND100);
                        configurationRequest = GetConfigurationMatrialValues(configurationRequest, Constants.SLINGVALIDATIONSUBPATHEND100);
                    }
                    break;
                case Constant.SYSTEMVALIDATIONCABCALL:
                    if (!String.IsNullOrEmpty(productType) && productType.Equals(Constants.EVO_200))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(string.Format(Constant.SYSTEMVALIDATIONBASEREQUEST, Constant.EVOLUTION200), Constant.VALIDATIONPATH);
                        configurationRequest = GetConfigurationMatrialValues(configurationRequest, Constant.CABVALIDATIONPATH);
                    }
                    else if (!String.IsNullOrEmpty(productType) && productType.Equals(Constants.EVO_100))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(string.Format(Constant.SYSTEMVALIDATIONBASEREQUEST, Constant.EVOLUTION__100), Constants.CABVLAIDATIONPATHEVO100);
                        configurationRequest = GetConfigurationMatrialValues(configurationRequest, Constants.CABVALIDATIONSUBPATHEVO100);
                    }
                    else if (!String.IsNullOrEmpty(productType) && productType.Equals(Constants.ENDURA_100))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(string.Format(Constant.SYSTEMVALIDATIONBASEREQUEST, Constant.END100), Constants.CABVLAIDATIONPATHEND100);
                        configurationRequest = GetConfigurationMatrialValues(configurationRequest, Constants.CABVALIDATIONSUBPATHEND100);
                    }
                    break;
                case Constant.SYSTEMVALIDATIONEMPTYCALL:
                    if (!String.IsNullOrEmpty(productType) && productType.Equals(Constants.EVO_200))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(string.Format(Constant.SYSTEMVALIDATIONBASEREQUEST, Constant.EVOLUTION200), Constants.VALIDATIONPATH);
                        configurationRequest = GetConfigurationMatrialValues(configurationRequest, Constant.EMPTYVALIDATIONPATH);
                    }
                    else if (!String.IsNullOrEmpty(productType) && productType.Equals(Constants.EVO_100))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(string.Format(Constant.SYSTEMVALIDATIONBASEREQUEST, Constant.EVOLUTION__100), Constants.EMPTYVALIDATIONPATHEVO100);
                        configurationRequest = GetConfigurationMatrialValues(configurationRequest, Constants.EMPTYVALIDATIONPATHEVO100SUBPATH);
                    }
                    break;
                case Constant.SYSTEMVALIDATIONDUTYCALL:
                    if (!String.IsNullOrEmpty(productType) && productType.Equals(Constants.EVO_200))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(string.Format(Constant.SYSTEMVALIDATIONBASEREQUEST, Constant.EVOLUTION200), Constants.VALIDATIONPATH);
                        configurationRequest = GetConfigurationMatrialValues(configurationRequest, Constant.DUTYVALIDATIONPATH);
                    }
                    else if (!String.IsNullOrEmpty(productType) && productType.Equals(Constants.EVO_100))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(string.Format(Constant.SYSTEMVALIDATIONBASEREQUEST, Constant.EVOLUTION__100), Constants.DUTYVALIDATIONPATHEVO100);
                        configurationRequest = GetConfigurationMatrialValues(configurationRequest, Constants.DUTYVALIDATIONEVO100SUBPATH);
                    }
                    else if (!String.IsNullOrEmpty(productType) && productType.Equals(Constants.ENDURA_100))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(string.Format(Constant.SYSTEMVALIDATIONBASEREQUEST, Constant.END100), Constants.DUTYVALIDATIONPATHEND100);
                        configurationRequest = GetConfigurationMatrialValues(configurationRequest, Constants.DUTYVALIDATIONEND100SUBPATH);
                    }
                    break;
                case Constant.SYSTEMVALIDATIONJACKDUTYCALL:
                    if (!String.IsNullOrEmpty(productType) && productType.Equals(Constants.ENDURA_100))
                    {
                        configurationRequest = CreateConfigurationRequestForProducts(string.Format(Constant.SYSTEMVALIDATIONBASEREQUEST, Constant.END100), Constants.JACKDUTYVALIDATIONPATHEND100);
                        configurationRequest = GetConfigurationMatrialValues(configurationRequest, Constants.JACKDUTYVALIDATIONEND100SUBPATH);
                    }
                    break;
                case Constant.ESCLATORMOVINGWALK:
                    configurationRequest = Utility.GetRequestBodyWithAssignments(Constant.NCPCLMREQESTBODYSTUBPATH, _configuration[Constant.ESCALATORPACKAGEID], _configuration[Constant.ESCLATORPATH]);
                    break;
                case Constant.TWINELEVATOR:
                    configurationRequest = Utility.GetRequestBodyWithAssignments(Constant.NCPCLMREQESTBODYSTUBPATH, _configuration[Constant.TWINELEVATORPACKAGEID], _configuration[Constant.TWINELEVATORPATH]);
                    break;
                case Constant.OTHER:
                    configurationRequest = Utility.GetRequestBodyWithAssignments(Constant.NCPCLMREQESTBODYSTUBPATH, _configuration[Constant.OTHERSCREENPACKAGEID], _configuration[Constant.OTHERSCREENPATH]);
                    break;
                default:
                    configurationRequest = CreateConfigurationRequestForProducts(Constant.BUILDINGCONFIGURATIONREQESTBODYSTUBPATH, Constant.BUILDINGCONFIGURATIONPATH);
                    break;
            }
            configurationRequest.Date = DateTime.Now;
            if (!(typeOfConfiguration.Equals(Constant.PRODUCT_SELECTION) || typeOfConfiguration.Equals(Constant.PRODUCTSELECTIONINVALIDEVO200)))
            {
                if (varibleAssignments != null)
                {
                    var objLine = Utility.DeserializeObjectValue<Line>(varibleAssignments.ToString());
                    List<VariableAssignment> lstVariableAssignments = new List<VariableAssignment>();
                    if (objLine.VariableAssignments != null && objLine.VariableAssignments.Any())
                    {
                        lstVariableAssignments.AddRange(objLine.VariableAssignments);
                    }
                    if (varRearOpen != null)
                    {
                        lstVariableAssignments.AddRange(varRearOpen);
                    }
                    foreach (var obj in lstVariableAssignments)
                    {
                        if (obj.VariableId.Equals(Constant.REGEN))
                        {
                            if (obj.Value.Equals(true))
                            {
                                obj.Value = "True";
                            }
                            else if (obj.Value.Equals(false))
                            {
                                obj.Value = "False";
                            }
                        }
                        else if (obj.Value.Equals(true))
                        {
                            obj.Value = "TRUE";
                        }
                        else if (obj.Value.Equals(false))
                        {
                            obj.Value = "FALSE";
                        }
                    }
                    if (configurationRequest.Line != null)
                    {
                        configurationRequest.Line.VariableAssignments = lstVariableAssignments;
                    }
                }
            }
            Utility.LogEnd(methodBeginTime);
            return configurationRequest;
        }

        private ConfigurationRequest CreateConfigurationRequestForProducts(string requestPath, string packagePath)
        {
            var stubReqbody = JObject.Parse(System.IO.File.ReadAllText(requestPath)).ToString();
            var configurationRequest = Utility.DeserializeObjectValue<ConfigurationRequest>(stubReqbody);
            configurationRequest.PackagePath = _configuration[packagePath];
            return configurationRequest;
        }

        /// <summary>
        /// GetAvailableProducts
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="parentCode"></param>
        /// <param Name="locale"></param>
        /// <param Name="selectedTab"></param>
        /// <param Name="modelNumber"></param>
        /// <returns></returns>

        public async Task<JObject> GetAvailableProducts(JObject variableAssignments, string sessionId
          , string locale, List<int> unitId,
            string modelNumber = null)
        {
            var methodBeginTime = Utility.LogBegin();
            var groupVariablesForFlags = new List<string>();
            var groupVariablesWithValues = new List<VariableAssignment>();
            var groupVariableValues = new List<VariableAssignment>();
            var productConstantMapper = Utility.GetVariableMapping(Constants.PRODUCTSELECTIONCONSTANTTEMPLATEPATH, Constants.VARIABLESCAPS);
            ConfigurationRequest configureRequest;

            var GroupConfigurationRequest = CreateProductConfigurationRequest(variableAssignments);
            GroupConfigurationRequest = GenerateIncludeSections(GroupConfigurationRequest, "GROUPLAYOUTCONFIGURATION");
            var cachedUserDetail = _cpqCacheManager.GetCache(sessionId, _environment, Constant.USERDETAILSCPQ);
            User userDetail = new User();
            if (!string.IsNullOrEmpty(cachedUserDetail))
            {
                userDetail = Utility.DeserializeObjectValue<User>(cachedUserDetail);
            }
            var variableAssignmentList = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString());
            var variableListForFixtureStrategy = variableAssignmentList.VariableAssignments.ToList();
            var variableTemplate = JObject.Parse(File.ReadAllText(Constant.PRODUCTSELECTIONCONSTANTTEMPLATEPATH));
            var fixtureStrategyVale = variableListForFixtureStrategy.Where(x => Utilities.CheckEquals(x.VariableId, Convert.ToString(variableTemplate[Constants.FIXTURESTRATEGY]))).ToList();
            if (fixtureStrategyVale.Any())
            {
                variableListForFixtureStrategy.Add
                    (new VariableAssignment()
                    {
                        VariableId = Convert.ToString(variableTemplate[Constants.FIXTURESTRATEGYVARIABLE]),
                        Value = fixtureStrategyVale[0].Value
                    }
                    );
            }
            variableAssignmentList.VariableAssignments = variableListForFixtureStrategy;
            string product = Constant.PRODUCTSELECTION;
            configureRequest = CreateConfigurationRequestWithTemplate(JObject.FromObject(variableAssignmentList), product);

            var mainGroupConfigurationResponse = new ConfigurationResponse
            {
                Sections = new List<Sections>()
            };
            var packagePath = configureRequest?.PackagePath;
            var groupPackagePath = GroupConfigurationRequest?.PackagePath;
            GroupConfigurationRequest.Line.VariableAssignments = variableListForFixtureStrategy;
            var groupConfigureResponseJObj = await ConfigurationBl(GroupConfigurationRequest, groupPackagePath, sessionId).ConfigureAwait(false);
            var groupConfigureResponse = groupConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var groupConfigureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(groupConfigureResponse.Arguments));
            var groupConfigureDictionary = groupConfigureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();
            var groupVariablesForVfd = groupConfigureDictionary.Where(x => x.Key.Contains(productConstantMapper[Constants.VFDVARIABLE]))?.FirstOrDefault();
            if (groupVariablesForVfd.Value.Key != null)
            {
                var groupVariablesForVfdVal = new VariableAssignment { VariableId = groupVariablesForVfd.Value.Key, Value = groupVariablesForVfd.Value.Value };
                variableListForFixtureStrategy.Add(groupVariablesForVfdVal);
                GroupConfigurationRequest.Line.VariableAssignments = variableListForFixtureStrategy;
                groupConfigureResponseJObj = await ConfigurationBl(GroupConfigurationRequest, groupPackagePath, sessionId).ConfigureAwait(false);
                groupConfigureResponse = groupConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
                groupConfigureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(groupConfigureResponse.Arguments));
                groupConfigureDictionary = groupConfigureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();
            }
            _cpqCacheManager.SetCache(sessionId, string.Join("_", unitId), "PRODUCTVARIABLES", Utility.SerializeObjectValue(groupConfigureResponseArgumentJObject));


            var GroupVariables = variableTemplate[Constant.GROUPVARIABLESFORFLAGS].ToObject<List<string>>();
            var variableDictionary = variableTemplate[Constants.VARIABLESCAPS].ToObject<Dictionary<string, string>>();

            groupVariablesForFlags = groupConfigureDictionary.Keys.Where(x => GroupVariables.Any(y => Utility.CheckEquals(x, y))).ToList();
            groupVariablesWithValues = groupConfigureDictionary.Where(x => groupVariablesForFlags.Contains(x.Key) && !Utilities.CheckEquals(Convert.ToString(x.Value).ToUpper(), Constants.FALSEVALUES)).Select(x => new VariableAssignment
            {
                VariableId = x.Key,
                Value = x.Value,
            }).ToList();

            // needed code pls don't remove it
            if (configureRequest == null)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = _localizer[Constant.REQUESTCANNOTBENULL + Constant.HYPHEN + locale?.Split(Constant.UNDERSCORECHAR)[0]].Value
                });
            }
            //Adding include sections in request body
            configureRequest = GenerateIncludeSections(configureRequest, "productselection");

            //generating cross package dependancy for flag variables
            var flagConfigVariable = new List<ConfigVariable>();



            foreach (var flag in groupVariablesWithValues)
            {
                flagConfigVariable.Add(new ConfigVariable { VariableId = flag.VariableId, Value = flag.Value });
            }

            var variableAssignmentsForGroup = GeneratevariableAssignmentsForCrosspackageDependecy(Constant.GROUPVARIABLESFORFLAGS, flagConfigVariable);

            foreach (var id in variableAssignmentsForGroup)
            {
                groupVariableValues.Add(new VariableAssignment { VariableId = id.VariableId, Value = id.Value });
            }
            configureRequest.Line.VariableAssignments = groupVariableValues;

            var configureResponseJObj = await ConfigurationBl(configureRequest, packagePath, sessionId).ConfigureAwait(false);
            var configureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
            groupConfigureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponse.Arguments));
            groupConfigureDictionary = groupConfigureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();
            if (variableAssignmentList?.VariableAssignments != null)
            {
                groupVariablesWithValues.AddRange(variableAssignmentList?.VariableAssignments.Where(x => x.VariableId.Equals(variableDictionary[Constants.CONTROLLOCATION]) ||
                x.VariableId.Contains(variableDictionary[Constants.TOPF], StringComparison.OrdinalIgnoreCase) ||
                x.VariableId.Contains(variableDictionary[Constants.TOPR], StringComparison.OrdinalIgnoreCase)).ToList());
            }
            configureRequest.Line.VariableAssignments = groupVariablesWithValues;
            var productSelectionConfigureResponseJObj = await ConfigurationBl(configureRequest, packagePath, sessionId).ConfigureAwait(false);
            // adding defaults to cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTPRODUCTCONFIGVALUES, Utility.SerializeObjectValue(groupConfigureResponseArgumentJObject));
            var modelId = modelNumber; //Added by harshada
            if (configureResponse.PriceSheet != null)  //Added by Harshada
            {
                modelId = configureResponse.PriceSheet.Id;
            }

            //JambMounted console default control landing value
            var jambMountedLandingValueTemplate = JObject.Parse(File.ReadAllText(Constant.PRODUCTSELECTIONJAMBMOUNTEDLANDINGVALUE));

            jambMountedLandingValueTemplate = Utility.MapVariables(productSelectionConfigureResponseJObj.Response, jambMountedLandingValueTemplate);

            var controlLandingVariable = Utility.GetVariables(jambMountedLandingValueTemplate).FirstOrDefault();


            if (controlLandingVariable != null)
            {
                int controlLanding = 0;
                foreach (var value in (controlLandingVariable.ToObject<Variables>()).Values)
                {
                    if (Utility.CheckEquals(value.Assigned, Constant.BYRULE))
                    {
                        controlLanding = Convert.ToInt32(value.value);
                        break;
                    }
                }
                _cpqCacheManager.SetCache(GetUserId(sessionId), string.Join("_", unitId), "CONTROLLDG", controlLanding.ToString());
            }

            JObject openingLocationResponses = JObject.Parse(File.ReadAllText(Constant.PRODUCTSELECTIONUIRESPONSETEMPLATE));

            openingLocationResponses = Utility.MapVariables(configureResponseJObj.Response, openingLocationResponses);
            mainGroupConfigurationResponse = openingLocationResponses.ToObject<ConfigurationResponse>();

            var productNameMapperJson = Utility.GetVariableMapping(Constants.PRODUCTSELECTIONCONSTANTTEMPLATEPATH, Constants.PROJECTCOMMONNAME);
            var productUnavailableJson = Utility.GetVariableMapping(Constants.PRODUCTSELECTIONCONSTANTTEMPLATEPATH, Constants.PRODUCTUNAVAILABLE);
            var productTypeMapperVariables = Utility.GetVariableMapping(Constants.PRODUCTSELECTIONCONSTANTTEMPLATEPATH, Constants.PRODUCTTYPE);
            var customEngineeredProducts = Utility.GetVariableMapping(Constants.PRODUCTSELECTIONCONSTANTTEMPLATEPATH, Constants.CUSTOMENGINEEREDPRODUCTS);
            var hydraulicCustomEngineeredProducts = Utility.GetVariableMapping(Constants.PRODUCTSELECTIONCONSTANTTEMPLATEPATH, Constants.HYDRAULICCUSTOMENGINEEREDPRODUCTS);
            var variableAssignmentsList = (variableAssignments).ToObject<Line>();
            var productType = variableAssignmentsList?.VariableAssignments.Where(x => Utilities.CheckEquals(x.VariableId, Constants.PRODUCTTYPE)).FirstOrDefault();
            var floorToFloorHeight = variableAssignmentsList?.VariableAssignments.Where(x => Utilities.CheckEquals(x.VariableId, Constants.TRAVEL)).FirstOrDefault();
            var TRAVELHYDRAULIC = variableAssignmentsList?.VariableAssignments.Where(x => Utilities.CheckEquals(x.VariableId, Constants.TRAVELHYDRAULIC)).FirstOrDefault();
            foreach (var currentProduct in mainGroupConfigurationResponse.Sections[0].Variables[0].Values)
            {
                var productName = currentProduct.Name;
                var messages = JObject.Parse(File.ReadAllText(Constant.PRODUCTSELECTIONCONSTANTTEMPLATEPATH))[$"messages"][productName].ToObject<Dictionary<string, string>>();

                var justification = string.Empty;
                foreach (var messageItem in messages)
                {
                    if (groupConfigureDictionary.ToList().Where(x => x.Key.Contains(messageItem.Key, StringComparison.OrdinalIgnoreCase) && Utility.CheckEquals(x.Value.ToString().ToUpper(), Constant.TRUEVALUES)).Any() && !string.IsNullOrEmpty(messageItem.Value) && !Utilities.CheckEquals(justification, messageItem.Value) && !justification.Contains(messageItem.Value))
                    {
                        justification = string.IsNullOrEmpty(justification) ? messageItem.Value : justification + "\n" + messageItem.Value;
                    }
                }
                if (productTypeMapperVariables.ContainsKey(productName) && !Utilities.CheckEquals(Convert.ToString(productTypeMapperVariables[productName]), Convert.ToString(productType.Value)) && !string.IsNullOrEmpty(Convert.ToString(productType.Value)))
                {
                    currentProduct.InCompatible = true;
                    var mixedGroupError = Convert.ToString(JObject.Parse(File.ReadAllText(Constant.PRODUCTSELECTIONCONSTANTTEMPLATEPATH))[Constants.MIXEDGROUPERROR]);
                    justification = string.IsNullOrEmpty(justification) ? mixedGroupError : justification + "\n" + mixedGroupError;
                }
                if (customEngineeredProducts.ContainsKey(productName) && Convert.ToBoolean(floorToFloorHeight.Value))
                {
                    currentProduct.InCompatible = true;
                    justification = string.IsNullOrEmpty(justification) ? customEngineeredProducts[productName] : justification + Environment.NewLine + customEngineeredProducts[productName];
                }
                if (hydraulicCustomEngineeredProducts.ContainsKey(productName) && Convert.ToBoolean(TRAVELHYDRAULIC.Value))
                {
                    currentProduct.InCompatible = true;
                    justification = string.IsNullOrEmpty(justification) ? hydraulicCustomEngineeredProducts[productName] : justification + Environment.NewLine + hydraulicCustomEngineeredProducts[productName];
                }
                //Utility.CheckEquals(productName, "EVO_100") ||
                if (productUnavailableJson.ContainsKey(productName))
                {
                    currentProduct.InCompatible = true;
                    justification = productUnavailableJson[productName];
                }
                currentProduct.Name = productNameMapperJson.ContainsKey(currentProduct.Name) ? productNameMapperJson[currentProduct.Name] : currentProduct.Name;
                currentProduct.Justification = justification;
            }
            Utility.LogEnd(methodBeginTime);
            return Utility.FilterNullValues(mainGroupConfigurationResponse);
        }


        public ConfigurationRequest CreateProductConfigurationRequest(JObject varibleAssignments)
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
        /// Adding Custom Engineered Include Sections
        /// </summary>
        /// <returns></returns>
        public List<string> AddIncludeSections(string configurationPath)
        {
            List<string> includeList = new List<string>();
            var includeValues = JArray.Parse(File.ReadAllText(configurationPath));
            var valuesData = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(includeValues));

            foreach (var value in valuesData)
            {
                includeList.Add(value);
            }
            return includeList;
        }

        /// <summary>
        /// Generate Include Sections
        /// </summary>
        /// <param Name="configureRequest"></param>
        /// <param Name="selectedTab"></param>
        /// <returns></returns>
        public ConfigurationRequest GenerateIncludeSections(ConfigurationRequest configureRequest, string selectedTab, string sectionTab = null, string productType = null)
        {
            var methodBeginTime = Utility.LogBegin();
            var selectedTabsValue = selectedTab.ToUpper();
            var sectionTabValue = sectionTab?.ToUpper();
            var unitsIncludeSectionValues = new JArray();
            var includeSections = JObject.Parse(File.ReadAllText(string.Format(Constant.INCLUDESECTIONSTEMPLATE, Constant.EVOLUTION200)));
            // Unit configuration includes section values map based on section tab
            if (!string.IsNullOrEmpty(sectionTabValue) && Utility.CheckEquals(selectedTab, Constant.UNITCONFIGURATION))
            {
                if (productType != null && productType.Equals(Constant.ENDURA_100))
                {
                    includeSections = JObject.Parse(File.ReadAllText(string.Format(Constant.INCLUDESECTIONSTEMPLATE, Constant.END100)));
                }
                if (productType != null && productType.Equals(Constant.EVOLUTION_100))
                {
                    includeSections = JObject.Parse(File.ReadAllText(string.Format(Constant.INCLUDESECTIONSTEMPLATE, Constant.EVOLUTION__100)));
                }
                switch (sectionTabValue)
                {
                    case Constant.GENERALINFORMATION:
                        unitsIncludeSectionValues = (JArray)includeSections[Constant.GENERALINFORMATIONLOWER];
                        if (productType != null && productType.Equals(Constant.END100))
                        {
                            unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(string.Format(Constant.INCLUDESECTIONSTEMPLATE, Constant.END100)));
                        }
                        if (productType != null && productType.Equals(Constant.EVOLUTION_100))
                        {
                            unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(string.Format(Constant.INCLUDESECTIONSTEMPLATE, Constant.EVOLUTION__100)));
                        }
                        break;
                    case Constant.CABINTERIOR:
                        unitsIncludeSectionValues = JArray.FromObject(includeSections[Constant.CABINTERIORLOWER]);
                        if (productType != null && productType.Equals(Constant.END100))
                        {
                            unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(string.Format(Constant.INCLUDESECTIONSTEMPLATE, Constant.END100)));
                        }
                        if (productType != null && productType.Equals(Constant.EVOLUTION_100))
                        {
                            unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(string.Format(Constant.INCLUDESECTIONSTEMPLATE, Constant.EVOLUTION__100)));
                        }
                        break;
                    case Constant.HOISTWAYTRACTIONEQUIPMENT:
                        unitsIncludeSectionValues = JArray.FromObject(includeSections[Constant.TRACTIONHOISTWAYEQUIPMENTLOWER]);
                        if (productType != null && productType.Equals(Constant.END100))
                        {
                            unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(string.Format(Constant.INCLUDESECTIONSTEMPLATE, Constant.END100)));
                        }
                        if (productType != null && productType.Equals(Constant.EVOLUTION_100))
                        {
                            unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(string.Format(Constant.INCLUDESECTIONSTEMPLATE, Constant.EVOLUTION__100)));
                        }
                        break;
                    case Constant.ENTRANCES:
                        unitsIncludeSectionValues = JArray.FromObject(includeSections[Constant.ENTRANCESLOWERCASE]);
                        break;
                    case Constant.CARFIXTURE:
                        unitsIncludeSectionValues = JArray.FromObject(includeSections[Constant.CARFIXTURELOWER]);
                        if (productType != null && productType.Equals(Constant.END100))
                        {
                            unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(string.Format(Constant.INCLUDESECTIONSTEMPLATE, Constant.END100)));
                        }
                        if (productType != null && productType.Equals(Constant.EVOLUTION_100))
                        {
                            unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(string.Format(Constant.INCLUDESECTIONSTEMPLATE, Constant.EVOLUTION__100)));
                        }
                        break;
                    case Constant.SUMMARYTAB:
                        unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(Constant.SUMMARYINCLUDESECTIONVALUES));
                        var landingSectionValues = (JObject.Parse(File.ReadAllText(Constant.UNITSVARIABLESMAPPERPATH))[Constant.SUMMARYLANDINGINCLUDESECTIONVALUES]).ToList();
                        var noOfLandings = (from variableValues in configureRequest.Line.VariableAssignments
                                            where variableValues.VariableId.Contains(Constant.BLANDINGS)
                                            select variableValues.Value).FirstOrDefault();
                        var constantVariables = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.DEFAULTVARIABLEVALUE);
                        if (noOfLandings == null)
                        {
                            noOfLandings = constantVariables[constantVariables.Keys.Where(x => x.Contains(Constant.BLANDINGS)).FirstOrDefault()];
                        }
                        for (int floor = 1; floor <= Convert.ToInt32(noOfLandings); floor++)
                        {
                            var strFloornumber = "000";
                            switch (floor.ToString().Length)
                            {
                                case 1:
                                    strFloornumber = "00" + floor.ToString();
                                    break;
                                case 2:
                                    strFloornumber = "0" + floor.ToString();
                                    break;
                                case 3:
                                    strFloornumber = floor.ToString();
                                    break;
                            }
                            foreach (var landing in landingSectionValues)
                            {
                                unitsIncludeSectionValues.Add(landing.ToString().Replace("#", strFloornumber));
                            }
                        }
                        break;
                    case Constant.UNITHALLFIXTURE:
                        unitsIncludeSectionValues = JArray.FromObject(includeSections[Constant.UNITHALLFIXTURELOWER]);
                        break;
                    case Constant.BUILDINGEQUIPMENT:
                        unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(Constant.BUILDINGCONFIGURATIONINCLUDESECTIONVALUES));
                        break;
                    case Constant.FIELDDRAWINGAUTOMATION:
                        unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(Constant.FIELDDRAWINGAUTOMATIONINLUDESECTIONVALUES));
                        break;
                    case Constant.LIFTDESIGNER:
                        unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(Constant.LIFTDESIGNERINLUDESECTIONVALUES));
                        break;
                    case Constant.ESCLATORCONFIGURATION:
                        unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(Constant.NCPINLUDESECTIONVALUES));
                        break;
                    case Constant.TWINELEVATORCONFIGURATION:
                        unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(Constant.NCPINLUDESECTIONVALUES));
                        break;
                    case Constant.OTHERSCREENCONFIGURATION:
                        unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(Constant.NCPINLUDESECTIONVALUES));
                        break;
                    case Constant.CUSTOMENGINEEREDGEARLESSCONFIGURATION:
                        unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(Constant.CUSTOMENGINEERDINLUDESECTIONVALUES));
                        break;
                    case Constant.CUSTOMENGINEEREDGEAREDCONFIGURATION:
                        unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(Constant.CUSTOMENGINEERDINLUDESECTIONVALUES));
                        break;
                    case Constant.CUSTOMENGINEEREDHYDRAULICCONFIGURATION:
                        unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(Constant.CUSTOMENGINEERDINLUDESECTIONVALUES));
                        break;
                    case Constant.SYNERGYCONFIGURATION:
                        unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(Constant.CUSTOMENGINEERDINLUDESECTIONVALUES));
                        break;
                    default:
                        unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(Constant.INCLUDESECTIONSTEMPLATE));
                        break;
                }
            }
            List<string> includeList = new List<string>
            {
                Constant.PARAMETERSVALUES
            };
            switch (selectedTabsValue)
            {
                case Constant.GROUPLAYOUTCONFIGURATION:
                    includeList.Add(Constant.PARAMETERS_SP);
                    includeList.Add(Constant.ELEVATOR001.Split(Constant.DOTCHAR)[0]);
                    var groupIncludeValues = JArray.Parse(File.ReadAllText(Constant.ELEVATORLISTDATA));
                    var elevatorsValuesData = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(groupIncludeValues));
                    // logic to get only the required elevators values 
                    foreach (var item in configureRequest.Settings.IncludeSections)
                    {
                        foreach (var elevators in elevatorsValuesData)
                        {
                            if (item.Contains(Constant.ELEVATORSVALUE) && elevators.Contains(item))
                            {
                                includeList.Add(elevators);
                            }
                        }
                    }
                    break;
                case Constant.OPENINGLOCATION:
                    var openingIncludeValues = JArray.Parse(File.ReadAllText(Constant.GROUPLANDINGSLISTDATA));
                    includeList = openingIncludeValues.ToObject<List<string>>();
                    break;
                case Constant.PRODUCTSELECTION:
                    //var productsIncludeValues = JArray.Parse(File.ReadAllText(Constant.PRODUCTLISTDATA));
                    //includeList = productsIncludeValues.ToObject<List<string>>();
                    includeList = null;
                    break;
                case Constant.UNITCONFIGURATION:
                    var unitsValuesData = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(unitsIncludeSectionValues));
                    foreach (var unitsData in unitsValuesData)
                    {
                        includeList.Add(unitsData);
                    }
                    break;
                case Constant.BUILDINGCONFIGURATION:
                    var buildingIncludeValues = JArray.Parse(File.ReadAllText(Constant.BUILDINGCONFIGURATIONINCLUDESECTIONVALUES));
                    var buildingValuesData = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(buildingIncludeValues));
                    foreach (var buildingData in buildingValuesData)
                    {
                        includeList.Add(buildingData);
                    }
                    break;
                case Constant.BUILDINGEQUIPMENT:
                    var buildingEquipmentIncludeValues = JArray.Parse(File.ReadAllText(Constant.BUILDINGCONFIGURATIONINCLUDESECTIONVALUES));
                    var buildingEquipmentValuesData = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(buildingEquipmentIncludeValues));
                    foreach (var buildingEquipmentData in buildingEquipmentValuesData)
                    {
                        includeList.Add(buildingEquipmentData);
                    }
                    break;
                case Constant.FIELDDRAWINGAUTOMATION:
                    includeList = AddIncludeSections(Constant.FIELDDRAWINGAUTOMATIONINLUDESECTIONVALUES);
                    break;
                case Constant.LIFTDESIGNER:
                    includeList = AddIncludeSections(Constant.FIELDDRAWINGAUTOMATIONINLUDESECTIONVALUES);
                    break;
                case Constant.ESCLATORCONFIGURATION:
                    includeList = AddIncludeSections(Constant.NCPINLUDESECTIONVALUES);
                    break;
                case Constant.TWINELEVATORCONFIGURATION:
                    includeList = AddIncludeSections(Constant.NCPINLUDESECTIONVALUES);
                    break;
                case Constant.OTHERSCREENCONFIGURATION:
                    includeList = AddIncludeSections(Constant.NCPINLUDESECTIONVALUES);
                    break;
                case Constant.CUSTOMENGINEEREDGEARLESSCONFIGURATION:
                    includeList = AddIncludeSections(Constant.CUSTOMENGINEERDINLUDESECTIONVALUES);
                    break;
                case Constant.CUSTOMENGINEEREDGEAREDCONFIGURATION:
                    includeList = AddIncludeSections(Constant.CUSTOMENGINEERDINLUDESECTIONVALUES);
                    break;
                case Constant.CUSTOMENGINEEREDHYDRAULICCONFIGURATION:
                    includeList = AddIncludeSections(Constant.CUSTOMENGINEERDINLUDESECTIONVALUES);
                    break;
                case Constant.SYNERGYCONFIGURATION:
                    includeList = AddIncludeSections(Constant.CUSTOMENGINEERDINLUDESECTIONVALUES);
                    break;
                default:
                    includeList.Add(Constant.PARAMETERSVALUES);
                    break;
            }
            configureRequest.Settings.IncludeSections = includeList;
            Utility.LogEnd(methodBeginTime);
            return configureRequest;
        }

        /// <summary>
        /// Method to get UserId from cache
        /// </summary>
        /// <returns></returns>

        public string GetUserId(string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            //Getting user details from auth get user info api cache
            var cachedUserDetail = _cpqCacheManager.GetCache(sessionId, _environment, Constant.USERDETAILSCPQ);
            User userDetail = new User();
            if (!string.IsNullOrEmpty(cachedUserDetail))
            {
                userDetail = Utility.DeserializeObjectValue<User>(cachedUserDetail);
            }
            Utility.LogEnd(methodBeginTime);
            return userDetail.UserId;
        }

        public string GetUnitDetails(string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            Utility.LogEnd(methodBeginTime);
            return _cpqCacheManager.GetCache(sessionId, _environment, Constant.UNITDETAILSCPQ);
        }

        public string GetUnitfixtureDetails(string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            Utility.LogEnd(methodBeginTime);
            return _cpqCacheManager.GetCache(sessionId, _environment, Constant.FIXTUREASSIGNAMENT);
        }

        public string GetUserAddress(string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            Utility.LogEnd(methodBeginTime);
            //Getting user details from auth get user info api cache
            return _cpqCacheManager.GetCache(sessionId, _environment, Constant.USERADDRESS);
        }

        public string GetOpportunityData(string OppAndVersion, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            Utility.LogEnd(methodBeginTime);
            //Getting user details from auth get user info api cache
            return _cpqCacheManager.GetCache(OppAndVersion, _environment, Constant.USERADDRESS);
        }

        /// <summary>
        /// GetUnitName
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<List<UnitMappingValues>> GetUnitName(JObject variableAssignments, int groupId, string sessionId, string selectedTab, List<DisplayVariableAssignmentsValues> displayVariablesValuesResponse)
        {
            var methodBeginTime = Utility.LogBegin();
            var CachedUnitName = _cpqCacheManager.GetCache(sessionId, _environment, Constant.UNITTABLEVALUES);
            if (string.IsNullOrEmpty(CachedUnitName))
            {
                CachedUnitName = _cpqCacheManager.GetCache(sessionId, _environment, Constant.UNITTABLEVALUES);
            }
            var unitTable = Utility.DeserializeObjectValue<List<UnitMappingValues>>(CachedUnitName);
            Utility.LogEnd(methodBeginTime);
            return unitTable;
        }

        public async Task<List<UnitMappingValues>> GetCacheUnitsList(string sessionId, int setId)
        {
            var methodBeginTime = Utility.LogBegin();
            var CachedUnitName = _cpqCacheManager.GetCache(sessionId, _environment, Constant.LISTOFUNITS);
            var unitTable = Utility.DeserializeObjectValue<List<UnitMappingValues>>(CachedUnitName);
            Utility.LogEnd(methodBeginTime);
            return unitTable;
        }

        /// <summary>
        /// SetCacheMappingValues
        /// </summary>
        /// <param Name="unitMappingValues"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public List<UnitMappingValues> SetCacheMappingValues(List<UnitMappingValues> unitMappingValues, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var updatedUnitMappingData = unitMappingValues;
            var unitTableResponseObj = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(updatedUnitMappingData));
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.POSITIONVALUES, Utility.SerializeObjectValue(unitTableResponseObj));
            Utility.LogEnd(methodBeginTime);
            return updatedUnitMappingData;
        }

        /// <summary>
        /// getElevatorMappingValues
        /// </summary>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        public ConfigureRequest GetElevatorMappingValues(ConfigureRequest configureRequest)
        {
            var methodBeginTime = Utility.LogBegin();
            var includeElevatorList = new List<string>();
            // get all car postion values from the request  
            var masterElevatorPosition = JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES))[Constant.ELEVATORPOSITIONS].ToList();
            // changes related to offset elevators 
            var positionNumber = 1;
            // for true values 
            foreach (var variableAssignmentValue in configureRequest.Line.VariableAssignments)
            {
                if (variableAssignmentValue.VariableId.Contains(Constant.PARAMETERS_LAYOUT_B) && Utility.CheckEquals(variableAssignmentValue.Value.ToString(), Constant.TRUEVALUES))
                {
                    var ValuesData = variableAssignmentValue.VariableId.Split(Constant.DOTCHAR).Last().ToUpper();
                    if (ValuesData.Contains("B2P"))
                    {
                        var valChangd = "B2P" + positionNumber;
                        variableAssignmentValue.VariableId = variableAssignmentValue.VariableId.Replace(ValuesData, valChangd);
                        positionNumber++;
                    }
                }
            }
            // for false values 
            foreach (var variableAssignmentValue in configureRequest.Line.VariableAssignments)
            {
                if (variableAssignmentValue.VariableId.Contains(Constant.PARAMETERS_LAYOUT_B) && Utility.CheckEquals(variableAssignmentValue.Value.ToString(), Constant.FALSEVALUES))
                {
                    var ValuesData = variableAssignmentValue.VariableId.Split(Constant.DOTCHAR).Last().ToUpper();
                    if (ValuesData.Contains("B2P"))
                    {
                        var valChangd = "B2P" + positionNumber;
                        variableAssignmentValue.VariableId = variableAssignmentValue.VariableId.Replace(ValuesData, valChangd);
                        positionNumber++;
                    }
                }
            }
            var selectedCarPositionValues = (from varAssign in configureRequest.Line.VariableAssignments
                                             where varAssign.VariableId.Contains(Constant.PARAMETERS_LAYOUT_B) && varAssign.Value.ToString().ToUpper().Equals(Constant.TRUEVALUES)
                                             select varAssign.VariableId.Split(Constant.DOTCHAR).Last()).ToList();
            var orderElevatorPositions = selectedCarPositionValues.OrderBy(elevatorPosName => masterElevatorPosition.IndexOf(elevatorPosName)).ToList();
            var elevatorNumber = 1;
            foreach (var orderPositionValue in orderElevatorPositions)
            {
                foreach (var variableAssignmentValue in configureRequest.Line.VariableAssignments)
                {
                    if (!variableAssignmentValue.VariableId.Contains(Constant.PARAMETERS_LAYOUT_B) && variableAssignmentValue.VariableId.Contains(orderPositionValue))
                    {
                        variableAssignmentValue.VariableId = variableAssignmentValue.VariableId.Replace(orderPositionValue, Constant.ELEVATORSVALUE + elevatorNumber);
                    }
                }
                // Added to mapping required elevators at include section level
                includeElevatorList.Add(Constant.ELEVATORSVALUE + elevatorNumber);
                elevatorNumber++;
            }
            // returning update variable Assignments Values in configureRequest Body
            configureRequest.Settings.IncludeSections = includeElevatorList;
            Utility.LogEnd(methodBeginTime);
            return configureRequest;
        }

        /// <summary>
        /// SetCacheEntranceConsoles
        /// </summary>
        /// <param Name="unitMappingValues"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public List<EntranceConfigurations> SetCacheEntranceConsoles(List<EntranceConfigurations> objEntranceConfigurationData, string sessionId, int setId)
        {
            var methodBeginTime = Utility.LogBegin();
            var updatedEntranceConsoleData = objEntranceConfigurationData;
            var username = GetUserId(sessionId);
            if (setId != 0)
            {
                if (objEntranceConfigurationData != null)
                {
                    var entranceConsolesResponseObj = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(updatedEntranceConsoleData));
                    _cpqCacheManager.SetCache(username, _environment, Constant.ENTRANCECONSOLE, setId.ToString(), Utility.SerializeObjectValue(entranceConsolesResponseObj));
                }
                else
                {
                    updatedEntranceConsoleData = Utility.DeserializeObjectValue<List<EntranceConfigurations>>(_cpqCacheManager.GetCache(username, _environment, Constant.ENTRANCECONSOLE, setId.ToString()));
                }
            }
            Utility.LogEnd(methodBeginTime);
            return updatedEntranceConsoleData;
        }

        /// <summary>
        /// EntranceConsoleConfigureBl
        /// </summary>
        /// <param Name="entranceConsole"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<JObject> EntranceConsoleConfigureBl(EntranceConfigurations entranceConsole, string sessionId, bool isSave, int setId)
        {
            var methodBeginTime = Utility.LogBegin();
            var constantVariables = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constants.ENTRANCE);
            var productType = SetCacheProductType(null, sessionId, setId).FirstOrDefault().Value.ToString();
            var configureRequest = CreateConfigurationRequestWithTemplate(null, Constant.UNITNAME, null, productType);
            var unitList = SetCacheUnitsList(null, sessionId, setId);
            CreateEntranceConsoleVariableAssignment(ref configureRequest, entranceConsole, sessionId, setId);
            var mainGroupConfigurationResponse = new ConfigurationResponse()
            {
                Sections = new List<Sections>()
            };
            var packagePath = configureRequest?.PackagePath;
            //Gets the base configuration of the model
            var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest);
            //Generate include section
            baseConfigureRequest = GenerateIncludeSections(baseConfigureRequest, Constant.UNITCONFIGURATION, Constant.ENTRANCES, productType);

            var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, packagePath, sessionId).ConfigureAwait(false);
            var baseConfigureResponse = baseConfigureResponseJObj.Response.ToObject<ConfigurationResponse>();
            var currentProductType = Constant.EVOLUTION200;

            switch (productType)
            {
                //Evolution200
                case Constant.MODEL_EVO200:
                    currentProductType = Constant.EVOLUTION200;
                    break;
                //Evolution100
                case Constant.MODEL_EVO100:
                    currentProductType = Constant.EVOLUTION__100;
                    break;
                //Endura100
                case Constant.ENDURA_100:
                    currentProductType = Constant.END100;
                    break;
                //default Evolution200
                default:
                    currentProductType = Constant.EVOLUTION200;
                    break;
            }
            // configuration object values for conflict mapping
            var configureResponse = baseConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = configureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            // adding defaults to cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTUNITCONFIGVALUES, Utility.SerializeObjectValue(configureResponseArgumentJObject));
            if (!isSave)
            {
                var stubHoistwayTractionResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.ENTRANCESCONSOLEUIRESPONSETEMPLATE, Constant.EVOLUTION200)));
                if (productType.Equals(Constant.ENDURA_100))
                {
                    stubHoistwayTractionResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.ENTRANCESCONSOLEUIRESPONSETEMPLATE, Constant.END100)));
                }
                if (productType.Equals(Constant.EVOLUTION_100))
                {
                    stubHoistwayTractionResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.ENTRANCESCONSOLEUIRESPONSETEMPLATE, Constant.EVOLUTION__100)));
                }
                // setting stub data into an sectionsValues object
                var mainFilteredRespone = Utility.MapVariables(JObject.FromObject(baseConfigureResponse), stubHoistwayTractionResponse);
                var collection = Utility.GetVariables(mainFilteredRespone);
                var listOfVariable = Utility.DeserializeObjectValue<List<Variables>>(Utility.SerializeObjectValue(collection));
                listOfVariable.Where(x => x.Id.Equals(constantVariables[Constants.WALLTHICKNESS], StringComparison.OrdinalIgnoreCase)).ToList().ForEach(variablesItem =>
                {
                    if (variablesItem.Values != null)
                    {
                        var readOnly = (from val in variablesItem.Properties where val.Id.Equals(Constant.READONLY_LOWER) & val.Value != null select val.Value)?.FirstOrDefault()?.ToString();
                        if (!Utility.CheckEquals(Constants.True, readOnly))
                        {
                            var minDatas = variablesItem.Values.Where(x => x.lower != null && x.InCompatible.Equals(false) && x.State.Equals(Constant.AVAILABLE))?.FirstOrDefault()?.lower;
                            var maxDatas = variablesItem.Values.Where(x => x.upper != null && x.InCompatible.Equals(false) && x.State.Equals(Constant.AVAILABLE))?.LastOrDefault()?.upper;
                            var getPropertiesVal = JArray.Parse(File.ReadAllText(string.Format(Constant.PROPERTYTEMPLATE, currentProductType)));
                            var variableProperties = Utility.DeserializeObjectValue<IList<Properties>>(Utility.SerializeObjectValue(getPropertiesVal));
                            if (variableProperties.Any())
                            {
                                variablesItem.Properties = variableProperties;
                            }
                            foreach (var item in variablesItem.Properties)
                            {
                                item.Value = GetPropertyValueForWallThicknessEntrances(item.Id, Convert.ToDouble(minDatas), Convert.ToDouble(maxDatas), false);
                            }
                        }
                    }
                });
                var entranceLocation = new EntranceAssignment()
                {
                    Openings = entranceConsole.Openings,
                    FixtureAssignments = new List<EntranceLocations>()
                };
                foreach (var varLocation in entranceConsole.FixtureLocations)
                {
                    entranceLocation.FixtureAssignments.Add(varLocation);
                }
                var newConsole = new Sections()
                {
                    Id = entranceConsole.EntranceConsoleId.ToString(),
                    Name = entranceConsole.ConsoleName,
                    assignOpenings = entranceConsole.AssignOpenings,
                    isController = entranceConsole.IsController,
                    FixtureLocations = entranceLocation,
                    Variables = listOfVariable,
                    Units = unitList
                };
                Utility.LogEnd(methodBeginTime);
                return Utility.FilterNullValues(newConsole);
            }
            return null;
        }

        public void CreateEntranceConsoleVariableAssignment(ref ConfigurationRequest cr, EntranceConfigurations entranceConsole, string sessionId, int setId)
        {
            var methodBeginTime = Utility.LogBegin();
            var constantVariables = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constants.ENTRANCE);
            var lstVariableAssignments = new List<VariableAssignment>();
            //Adding control location value in the variable assignments fro controller console
            if (entranceConsole.IsController.Equals(true))
            {
                var controlLocationVariable = SetCacheControlLocationValues(null, sessionId, setId);
                lstVariableAssignments.Add(new VariableAssignment()
                {
                    VariableId = constantVariables[Constants.CONTROLLOCATIONVALUE],
                    Value = controlLocationVariable.FirstOrDefault().Value
                });
                lstVariableAssignments.Add(new VariableAssignment()
                {
                    VariableId = constantVariables[Constants.CONTROLHERE],
                    Value = true
                });
            }
            var enrichedData = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.EVOLUTION200)));
            var enrichedDataVariables = enrichedData[Constant.VARIABLES];
            var lstSelectedLandings = entranceConsole.FixtureLocations.Where(x => x.Front.Value.Equals(true) || x.Rear.Value.Equals(true)).ToList();
            entranceConsole.VariableAssignments = entranceConsole.VariableAssignments.Where(x => !x.VariableId.Equals(String.Empty)).ToList();
            foreach (var varAssignment in entranceConsole.VariableAssignments)
            {
                var needVariables = Utility.GetTokens(varAssignment.VariableId, enrichedDataVariables, false);
                var currentPropertyCollection = needVariables.Select(x => (JProperty)x).Where(x => x.Name == Constant.PROPERTIES).Select(x => x.Value).FirstOrDefault();

                if (lstSelectedLandings.Count > 0)
                {
                    foreach (var selecteLanding in lstSelectedLandings)
                    {
                        var strFloornumber = selecteLanding.FloorNumber.ToString().PadLeft(3, '0');
                        bool isFront = Convert.ToBoolean(selecteLanding.Front.Value);
                        bool isRear = Convert.ToBoolean(selecteLanding.Rear.Value);
                        if (isFront && currentPropertyCollection != null)
                        {
                            var landingVariable = currentPropertyCollection.Children().Where(x => x.Children<JProperty>().Any(y => y.Name == Constant.IDPARAM && (y.Value.ToString() == "LandingVariableFront" || y.Value.ToString() == "LandingVariable")))?.SelectMany(x => x.Children<JProperty>().Select(y => y.Name == Constant.VALUE ? y.Value.ToString() : "")).FirstOrDefault(x => !string.IsNullOrEmpty(x));
                            var variableId = landingVariable.Replace("#", strFloornumber);
                            var variableAssignments = new VariableAssignment()
                            {
                                VariableId = variableId,
                                Value = varAssignment.Value
                            };
                            lstVariableAssignments.Add(variableAssignments);
                        }
                        if (isRear && currentPropertyCollection != null)
                        {
                            var landingVariable = currentPropertyCollection.Children().Where(x => x.Children<JProperty>().Any(y => y.Name == Constant.IDPARAM && (y.Value.ToString() == "LandingVariableRear" || y.Value.ToString() == "LandingVariable"))).SelectMany(x => x.Children<JProperty>().Select(y => y.Name == Constant.VALUE ? y.Value.ToString() : "")).FirstOrDefault(x => !string.IsNullOrEmpty(x));
                            if (isFront)
                            {
                                landingVariable = currentPropertyCollection.Children().Where(x => x.Children<JProperty>().Any(y => y.Name == Constant.IDPARAM && y.Value.ToString() == "LandingVariableRear")).SelectMany(x => x.Children<JProperty>().Select(y => y.Name == Constant.VALUE ? y.Value.ToString() : "")).FirstOrDefault(x => !string.IsNullOrEmpty(x));
                            }
                            if (!String.IsNullOrEmpty(landingVariable))
                            {
                                var variableId = landingVariable.Replace("#", strFloornumber);
                                var variableAssignments = new VariableAssignment()
                                {
                                    VariableId = variableId,
                                    Value = varAssignment.Value
                                };
                                lstVariableAssignments.Add(variableAssignments);
                            }
                        }
                    }
                }
                var variableAssignment = new VariableAssignment()
                {
                    VariableId = varAssignment.VariableId,
                    Value = varAssignment.Value
                };
                lstVariableAssignments.Add(variableAssignment);
            }
            cr.Line.VariableAssignments = lstVariableAssignments;
            Utility.LogEnd(methodBeginTime);
        }

        /// <summary>
        /// Summary Unit Configure BL
        /// </summary>
        /// <param name="lineVariableAssignment"></param>
        /// <param name="unitValues"></param>
        /// <param name="sessionId"></param>
        /// <param name="sectionTab"></param>
        /// <param name="OpeningVariables"></param>
        /// <param name="setId"></param>
        /// <param name="groupUnitInfo"></param>
        /// <param name="priceAndDiscountData"></param>
        /// <returns></returns>
        public async Task<JObject> SummaryUnitConfigureBl(JObject lineVariableAssignment, List<UnitDetailsForTP2> unitValues, string sessionId, string sectionTab,
            List<OpeningVariables> OpeningVariables, int setId, List<UnitNames> groupUnitInfo, List<DiscountDataPerUnit> priceAndDiscountData,
            List<PriceSectionDetails> manufacturingCommentsTable)
        {
            var methodBeginTime = Utility.LogBegin();
            var productType = SetCacheProductType(null, sessionId, setId).FirstOrDefault().Value.ToString();
            //productType = Constant.EVO_200;
            var configureRequest = CreateConfigurationRequestWithTemplate(lineVariableAssignment, Constant.UNITNAME, null, productType);
            var constantVariables = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.DEFAULTVARIABLEVALUE);
            var unitMapperVariables = Utility.DeserializeObjectValue <List<string>>(JObject.Parse(File.ReadAllText(Constant.UNITSVARIABLESMAPPERPATH))["VariablesRequiredForTP2SummaryPriceValuesCall"].ToString());
            var cachedData = _cpqCacheManager.GetCache(sessionId, _environment, Constant.PREVIOUSCONFLICTSVALUES);
            var totalLandings = (from variableValues in configureRequest.Line.VariableAssignments
                                 where variableValues.VariableId.Contains(Constant.BLANDINGS)
                                 select variableValues.Value);
            var numsuspValue = (from variableValues in configureRequest.Line.VariableAssignments
                                where variableValues.VariableId.Contains(Constant.NUMSUSP)
                                select variableValues.Value);
            if (totalLandings.Count() == 0)
            {
                totalLandings = totalLandings.Append(constantVariables[constantVariables.Keys.Where(x => x.Contains(Constant.BLANDINGS)).FirstOrDefault()]);
            }
            Dictionary<string, decimal> unitDict = new Dictionary<string, decimal>();
            foreach (var unit in unitValues)
            {
                unitDict[unit.Ueid] = 0;
            }
            var lowestUnit = string.Empty;
            var lowestUnitOfGroup = (from unitDetails in groupUnitInfo
                                     where !String.IsNullOrEmpty(unitDetails.Ueid)
                                     orderby unitDetails.Ueid
                                     select unitDetails.Ueid).First();
            if (unitDict.ContainsKey(lowestUnitOfGroup))
            {
                lowestUnit = lowestUnitOfGroup;
            }
            var mainGroupConfigurationResponse = new ConfigurationResponse
            {
                Sections = new List<Sections>()
            };
            var packagePath = configureRequest?.PackagePath;
            // needed code pls don't remove it
            if (configureRequest == null)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = _localizer[Constant.REQUESTCANNOTBENULL + Constant.HYPHEN]
                });
            }
            // adding required include section values
            configureRequest = GenerateIncludeSections(configureRequest, Constant.UNITCONFIGURATION, sectionTab);
            // for summary Unit configuration values 
            configureRequest.Settings.Debug = true;
            configureRequest.Settings.IncludePriceLines = true;
            //Adding include sections in request body
            var configureResponseJObj =
                await ConfigurationBl(configureRequest, packagePath, sessionId).ConfigureAwait(false);
            var configureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
            List<string> priceKeys = new List<string>();
            Dictionary<String, int> priceDict = new Dictionary<string, int>();
            var argumentsResponse = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponse.Arguments));
            var argumentsResponseDictionary = argumentsResponse[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTUNITCONFIGVALUES, Utility.SerializeObjectValue(argumentsResponse));
            var pitDepth = (from arguments in argumentsResponseDictionary
                            where arguments.Key.Contains("PITDEPTH")
                            select new VariableAssignment { VariableId = arguments.Key, Value = arguments.Value }).ToList();
            pitDepth.AddRange((from variableValues in configureRequest.Line.VariableAssignments
                                        where unitMapperVariables.Contains(variableValues.VariableId)
                                        select variableValues));
            pitDepth.AddRange((from variableValues in argumentsResponseDictionary
                               where unitMapperVariables.Contains(variableValues.Key)
                               select new VariableAssignment { VariableId = variableValues.Key, Value = variableValues.Value}));
            foreach(var obj in pitDepth)
            {
                if(obj.Value.GetType().Name == "Double")
                {
                    obj.Value = Math.Round(Convert.ToDecimal(obj.Value), 2);
                }
            }
            foreach (var sectionValue in configureResponse.Sections)
            {
                foreach (var subsection in sectionValue.Sections)
                {
                    if (subsection.Id.Contains(Constant.PRICEKEY))
                    {
                        foreach (var priceVariable in subsection.Variables)
                        {
                            foreach (var assignedValue in priceVariable.Values)
                            {
                                if (assignedValue.Assigned != null && (assignedValue.Assigned.ToString().Equals(Constant.BYRULEUPPERCASE) || assignedValue.Assigned.ToString().Equals(Constant.BYDEFAULTUPPERCASE)))
                                {
                                    var keyValue = priceVariable.Id.ToString().Split(Constant.DOT).Last();
                                    priceKeys.Add(keyValue);
                                    if (priceDict.ContainsKey(keyValue))
                                    {
                                        priceDict[keyValue] += 1;
                                    }
                                    else
                                    {
                                        priceDict[keyValue] = 1;
                                    }
                                }
                            }


                        }

                    }
                    else
                    {
                        foreach (var childSection in subsection.Sections)
                        {
                            foreach (var subSectionValue in childSection.Sections)
                            {
                                foreach (var priceVariable in subSectionValue.Variables)
                                {
                                    foreach (var assignedValue in priceVariable.Values)
                                    {
                                        if (assignedValue.Assigned != null && (assignedValue.Assigned.ToString().Equals(Constant.BYRULEUPPERCASE) || assignedValue.Assigned.ToString().Equals(Constant.BYDEFAULTUPPERCASE)))
                                        {
                                            var keyValue = priceVariable.Id.ToString().Split(Constant.DOT).Last();
                                            priceKeys.Add(keyValue);
                                            if (priceDict.ContainsKey(keyValue))
                                            {
                                                priceDict[keyValue] += 1;
                                            }
                                            else
                                            {
                                                priceDict[keyValue] = 1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            priceKeys = priceKeys.Distinct().ToList();
            var cr = new ConfigureRequest();
            cr.Line = new Line() { VariableAssignments = pitDepth };
            var configureRequestForPriceValues = CreateConfigurationRequestWithTemplate(Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(cr.Line)), 
                Constant.UNITNAME, null, productType);
            configureRequestForPriceValues.GlobalArguments[Constant.VARIANTS] = priceKeys;
            configureRequestForPriceValues.Settings.IncludePriceLines = true;
            List<String> includeSectionValuesForPriceValuesRequest = Utility.DeserializeObjectValue<List<String>>(JArray.Parse(File.ReadAllText(Constant.SUMMARYINCLUDESECTIONVALUES)).ToString());
            includeSectionValuesForPriceValuesRequest.RemoveAt(includeSectionValuesForPriceValuesRequest.Count - 1);
            configureRequestForPriceValues.Settings.IncludeSections = includeSectionValuesForPriceValuesRequest;
            var configureResponseForPriceValuesJObj =
                await ConfigurationBl(configureRequestForPriceValues, packagePath, sessionId).ConfigureAwait(false);
            var unitMappingObjValues = JObject.Parse(File.ReadAllText(Constant.PRICEDETAILS));
            if (Utility.CheckEquals(productType, Constants.EVO_100))
            {
                unitMappingObjValues = JObject.Parse(File.ReadAllText(Constant.PRICEDETAILSEVO100));
            }
            if (Utility.CheckEquals(productType, Constants.ENDURA_100))
            {
                unitMappingObjValues = JObject.Parse(File.ReadAllText(Constant.PRICEDETAILSEND100));
            }
            var priceValues = Utility.DeserializeObjectValue<List<PriceValuesDetails>>(Utility.SerializeObjectValue(unitMappingObjValues[Constant.PRICEKEYDETAILS]));
            //foreach(var openingVariables in )
            var getFilterValues = (from values in priceValues
                                   from keys in priceKeys
                                   where Utility.CheckEquals(values.ItemNumber.Replace(Constant.HYPHENCHAR, Constant.UNDERSCORECHAR), keys)
                                   select values).ToList();
            var getPriceByGroup = getFilterValues.GroupBy(variable => variable.SectionName).ToList();
            // values to add 
            List<PriceSectionDetails> priceSectionValues = new List<PriceSectionDetails>();
            foreach (var item in getPriceByGroup)
            {
                var sectionvalue = new PriceSectionDetails
                {
                    Name = item.Key
                };
                foreach (var items in item)
                {
                    var keyinfoValue = new PriceValuesDetails()
                    {
                        Section = items.Section,
                        ComponentName = items.Component,
                        PartDescription = items.PartDescription,
                        ItemNumber = items.ItemNumber.Replace(Constant.HYPHENCHAR, Constant.UNDERSCORECHAR),
                        LeadTime = items.LeadTime
                    };
                    if (sectionvalue.PriceKeyInfo == null)
                    {
                        var priceDetails = new List<PriceValuesDetails>
                        {
                            keyinfoValue
                        };
                        sectionvalue.PriceKeyInfo = priceDetails;
                        sectionvalue.Section = keyinfoValue.Section;
                    }
                    else
                    {
                        sectionvalue.PriceKeyInfo.Add(keyinfoValue);
                    }
                }
                priceSectionValues.Add(sectionvalue);
            }
            //get the variables names 
            var getNames = JObject.Parse(File.ReadAllText(Constant.VARIABLENAMESFORPRICING));
            var mapPriceValues = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(getNames[Constant.PRICINGCROSSVARIABLES]));
            var basePriceResponseForTabLevel = (from val1 in mapPriceValues
                                                from val2 in priceSectionValues
                                                where Utility.CheckEquals(val1.Key, val2.Name)
                                                select new PriceSectionDetails
                                                {
                                                    Name = val2.Name,
                                                    Id = val1.Value,
                                                    Section = val2.PriceKeyInfo[0].Section,
                                                    PriceKeyInfo = val2.PriceKeyInfo
                                                }).ToList();
            var configureResponseForPriceValues = configureResponseForPriceValuesJObj.Response.ToObject<StartConfigureResponse>();
            // adding the price values as a dicitionary to map
            Dictionary<string, UnitPriceValues> priceDictionary = new Dictionary<string, UnitPriceValues>();
            foreach (var item in priceKeys.Distinct())
            {
                List<PriceLine> localDictionary = new List<PriceLine>();
                foreach (var priceLineSection in configureResponseForPriceValues.PriceSheet.PriceLines.ToList())
                {
                    if (Utility.CheckEquals(item, priceLineSection.Feature))
                    {
                        localDictionary.Add(priceLineSection);
                    }
                }
                // getting the Unit price and Total price of the price key values
                if (localDictionary != null && localDictionary.Any())
                {
                    var valuePrice = localDictionary.LastOrDefault();
                    var details = (from keys in getFilterValues
                                   where keys.ItemNumber.ToString().Replace(Constant.HYPHENCHAR, Constant.UNDERSCORECHAR).Equals(item)
                                   select keys).ToList();
                    List<int> count = new List<int>();
                    if (details!=null && details.Count>0 && details[0].Parameter2 != null)
                    {
                        if (details[0].Parameter2Value.Contains(Constant.GREATERTHAN))
                        {
                            count = (from openingData in OpeningVariables
                                     where openingData.VariableAssigned.VariableId.Contains(details[0].Parameter2)
                                     select openingData.TotalOpenings).ToList();
                        }
                        else if (details[0].Parameter3 != null)
                        {
                            foreach (var openingData in OpeningVariables)
                            {
                                foreach (var openingData2 in OpeningVariables)
                                {
                                    if (openingData.VariableAssigned.VariableId.Contains(details[0].Parameter2) &&
                                        openingData2.VariableAssigned.VariableId.Contains(details[0].Parameter3) &&
                                        openingData.TotalOpenings.Equals(openingData2.OpeningsAssigned) &&
                                        openingData.OpeningsAssigned.Equals(openingData2.OpeningsAssigned))
                                    {
                                        List<int> openings = new List<int> { openingData.TotalOpenings };
                                        count = openings;
                                        break;
                                    }
                                }
                            }

                        }
                        else
                        {
                            count = (from openingData in OpeningVariables
                                     where openingData.VariableAssigned.VariableId.Contains(details[0].Parameter2) &&
                                     openingData.VariableAssigned.Value.ToString().Contains(details[0].Parameter2Value)
                                     select openingData.TotalOpenings).ToList();
                        }
                    }
                    else if (details != null && details.Count > 0 && details[0].Parameter3 != null)
                    {
                        count = (from openingData in OpeningVariables
                                 where openingData.VariableAssigned.VariableId.Contains(details[0].Parameter3) && details[0].Parameter3Value.ToUpper().Contains(openingData.VariableAssigned.Value.ToString().ToUpper())
                                 select openingData.TotalOpenings).ToList();

                    }
                    else if (details != null && details.Count > 0 &&  details[0].Parameter4 != null)
                    {
                        count = (from openingData in OpeningVariables
                                 where openingData.VariableAssigned.VariableId.Contains(details[0].Parameter4) && details[0].Parameter4Value.ToUpper().Contains(openingData.VariableAssigned.Value.ToString().ToUpper())
                                 select openingData.TotalOpenings).ToList();

                    }
                    if (details != null && details.Count > 0 && priceDict[item] <= Convert.ToInt32(totalLandings.FirstOrDefault()))
                    {
                        var unitPriceValues = new UnitPriceValues()
                        {
                            unitPrice = (valuePrice.Result.Value),
                            quantity = !String.IsNullOrEmpty(details[0].Numsusp) && details[0].Numsusp.Equals(Constant.TRUEVALUES) ? Convert.ToInt32(numsuspValue.FirstOrDefault()) : (details[0].qty > 0 ? details[0].qty : priceDict[item] > 1 ? priceDict[item] : count.FirstOrDefault() != 0 ? count.FirstOrDefault() : 1),
                            totalPrice = !String.IsNullOrEmpty(details[0].Numsusp) && details[0].Numsusp.Equals(Constant.TRUEVALUES) ? (valuePrice.Result.Value) * Convert.ToInt32(numsuspValue.FirstOrDefault()) : details[0].qty > 0 ? (valuePrice.Result.Value) * details[0].qty : priceDict[item] > 1 ? (valuePrice.Result.Value) * priceDict[item] :
                                        count.FirstOrDefault() != 0 ? (valuePrice.Result.Value) * count.FirstOrDefault() : (valuePrice.Result.Value),
                            Unit = Constant.CURRENCYCODE
                        };
                        priceDictionary.Add(item, unitPriceValues);

                        //Adding prices for individual units
                        if (details != null && details.FirstOrDefault().GroupMaterial.Equals(Constant.TRUEVALUES))
                        {
                            if (String.IsNullOrEmpty(lowestUnit))
                            {
                                priceDictionary.Remove(details.FirstOrDefault().ItemNumber.Replace(Constant.HYPHENCHAR, Constant.UNDERSCORECHAR));
                                foreach (var priceSection in priceSectionValues)
                                {
                                    priceSection.PriceKeyInfo = priceSection.PriceKeyInfo.Where(x => !x.ItemNumber.Equals(details.FirstOrDefault().ItemNumber.Replace(Constant.HYPHENCHAR, Constant.UNDERSCORECHAR))).ToList();
                                }
                            }
                            else
                            {
                                unitDict[lowestUnit] += unitPriceValues.totalPrice;
                            }

                        }
                        else
                        {
                            var listOfUEID = unitDict.Keys.ToList();
                            foreach (var id in listOfUEID)
                            {
                                unitDict[id] += unitPriceValues.totalPrice;
                            }
                        }
                    }
                }
            }
            var totalPriceList = (from price in unitDict.Values.ToList()
                                  select price).ToList();
            // Total Price mapping 
            priceDictionary.Add(Constant.TOTALPRICE, new UnitPriceValues()
            {
                unitPrice = totalPriceList.Sum(),
                quantity = unitDict.Count,
                totalPrice = totalPriceList.Sum(),
                Unit = Constant.CURRENCYCODE
            });
            var configureResponseArgument = configureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            var stubUnitConfigurationMainResponseObj = new UnitSummaryUIModel();
            // setting stub data into an sectionsValues object
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
                        case Constant.ENDURA_100:
                            unit.ProductName = Constant.ENDURA_100;
                            break;
                        default:
                            break;
                    }
                }
            }
            stubUnitConfigurationMainResponseObj.Units = JsonConvert.DeserializeObject<List<UnitNames>>(JsonConvert.SerializeObject(unitValues));
            foreach (var unit in stubUnitConfigurationMainResponseObj.Units)
            {
                unit.Price = (from unitInfo in unitDict
                              where unitInfo.Key.Equals(unit.Ueid)
                              select unitInfo.Value).FirstOrDefault();
            }
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.SETUNITSDATA, Utility.SerializeObjectValue(stubUnitConfigurationMainResponseObj.Units));
            if (basePriceResponseForTabLevel != null && basePriceResponseForTabLevel.FirstOrDefault(x => x.Section.Equals(Constant.BASESECTION)) != null)
            {
                var lineAdjustmentKeys = basePriceResponseForTabLevel.Where(x => x.Section.Equals(Constant.BASESECTION)).FirstOrDefault();
                basePriceResponseForTabLevel = basePriceResponseForTabLevel.Where(x => !x.Section.Equals(Constant.BASESECTION)).ToList();
                var unitPriceBeforeAdjustment = stubUnitConfigurationMainResponseObj.Units.FirstOrDefault().Price;
                foreach (var lineAdjustmentData in lineAdjustmentKeys.PriceKeyInfo)
                {
                    unitPriceBeforeAdjustment = unitPriceBeforeAdjustment - priceDictionary[lineAdjustmentData.ItemNumber].totalPrice;
                }
                foreach (var priceSection in basePriceResponseForTabLevel)
                {
                    foreach (var lineAdjustmentData in lineAdjustmentKeys.PriceKeyInfo)
                    {
                        var newPriceKeyInfo = new PriceValuesDetails
                        {
                            ComponentName = lineAdjustmentData.ComponentName,
                            ItemNumber = lineAdjustmentData.ItemNumber + priceSection.PriceKeyInfo.FirstOrDefault().Section,
                            PartDescription = lineAdjustmentData.PartDescription,
                            Section = priceSection.PriceKeyInfo.FirstOrDefault().Section
                        };
                        priceSection.PriceKeyInfo.Add(newPriceKeyInfo);
                        CreateNewPriceKeyBasedOnProductLineAdjustment(newPriceKeyInfo, priceDictionary, basePriceResponseForTabLevel, unitPriceBeforeAdjustment);

                    }

                }
                foreach (var lineAdjustmentData in lineAdjustmentKeys.PriceKeyInfo)
                {
                    priceDictionary.Remove(lineAdjustmentData.ItemNumber);
                }
            }

            stubUnitConfigurationMainResponseObj.PriceSections = (from sections in basePriceResponseForTabLevel
                                                                  orderby Convert.ToInt32(sections.Section)
                                                                  select sections).ToList();
            stubUnitConfigurationMainResponseObj.PriceValue = priceDictionary;
            var listOfUnitIds = new List<int>();
            foreach (var unit in unitValues)
            {
                listOfUnitIds.Add(unit.UnitId);
            }
            stubUnitConfigurationMainResponseObj.Variables = new List<Variables>();
            var userInputVariables = JsonConvert.DeserializeObject<SectionsValues>(File.ReadAllText(string.Format(Constant.USERINPUTVARAIBLESTEMPLATE, Constant.EVOLUTION200)));
            foreach (var variable in userInputVariables.sections[0].Variables)
            {
                var discountValue = priceAndDiscountData?.LastOrDefault(data => Utility.CheckEquals(data.VariableForUnit.VariableId, variable.Id) && listOfUnitIds.Contains(data.Unitid
                    ))?.VariableForUnit?.Value;
                var properties = new List<Properties>();
                properties.Add(new Properties { Id = Constant.MAXVALUE, Type = "String", Value = totalPriceList.Sum() });
                properties.Add(new Properties { Id = Constant.RANGEVALIDATION, Type = "String", Value = Constant.RANGEMESSAGE + totalPriceList.Sum().ToString() });

                stubUnitConfigurationMainResponseObj.Variables.Add(new Variables
                {
                    Id = variable.Id,
                    Value = variable.Id.Equals(Constant.MANUFACTURINGCOMMENTS) ? String.Empty : (discountValue != null ? discountValue.ToString() : "0"),
                    Properties = variable.Id != Constant.MANUFACTURINGCOMMENTS ? properties : new List<Properties>()
                });

            }

            // adding manufacturing comments history in the response
            if (manufacturingCommentsTable.Count > 0)
            {
                stubUnitConfigurationMainResponseObj.ManufacturingCommentsHistory = new List<LogParameters>();
                foreach (var comments in manufacturingCommentsTable.FirstOrDefault().PriceKeyInfo)
                {

                    stubUnitConfigurationMainResponseObj.ManufacturingCommentsHistory.Add(new LogParameters
                    {
                        Time = comments.Section,
                        User = comments.ComponentName,
                        UpdatedValue = comments.PartDescription
                    });
                }
            }

            // added enriched data in the main response 
            var enrichedData = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.EVOLUTION200)));
            stubUnitConfigurationMainResponseObj.EnrichedData = enrichedData;
            stubUnitConfigurationMainResponseObj.ConfigurationStatus = stubUnitConfigurationMainResponseObj.Units?.FirstOrDefault().Status;
            // to get the conflicts cache if any
            var getCacheResponse = _cpqCacheManager.GetCache(sessionId, _environment, Constant.PREVIOUSUNITCONFLICTSVALUES);
            if (!string.IsNullOrEmpty(getCacheResponse))
            {
                var filterConflictResponse = Utility.DeserializeObjectValue<ConflictManagement>(getCacheResponse);
                stubUnitConfigurationMainResponseObj.ConflictAssignments = filterConflictResponse;
            }
            var mainResponseData = Utility.FilterNullValues(stubUnitConfigurationMainResponseObj);
            Utility.LogEnd(methodBeginTime);
            return mainResponseData;
        }

        private void CreateNewPriceKeyBasedOnProductLineAdjustment(PriceValuesDetails newPriceKeyInfo, Dictionary<string, UnitPriceValues> priceValuesResponse, List<PriceSectionDetails> priceSections, Decimal totalPrice)
        {
            var sectionNumber = newPriceKeyInfo.Section;
            var priceKeysOfSection = new List<String>();
            decimal subtotalOfSection = 0;
            var totalValueAddedByUser = (from userVariable in priceValuesResponse.Keys
                                         where newPriceKeyInfo.ItemNumber.Contains(userVariable)
                                         select priceValuesResponse[userVariable]).FirstOrDefault().unitPrice;
            foreach (var priceDetails in priceSections.Where(x => x.Section.Equals(sectionNumber)).FirstOrDefault().PriceKeyInfo)
            {
                priceKeysOfSection.Add(priceDetails.ItemNumber);
            }
            foreach (var pricekey in priceValuesResponse)
            {
                if (priceKeysOfSection.Contains(pricekey.Key.ToUpper()))
                {
                    subtotalOfSection += pricekey.Value.totalPrice;
                }
            }
            var newPricePerSection = (subtotalOfSection / totalPrice) * Convert.ToDecimal(totalValueAddedByUser);
            priceValuesResponse.Add(newPriceKeyInfo.ItemNumber, new UnitPriceValues { Unit = Constant.CURRENCYCODE, quantity = 1, totalPrice = newPricePerSection });
        }

        public List<UnitHallFixtures> SetCacheUnitHallFixtureConsoles(List<UnitHallFixtures> objUnitHallFixtureConfigurationData, string sessionId, int setId)
        {
            var methodBeginTime = Utility.LogBegin();
            var updatedUnitHallFixtureConsoleData = objUnitHallFixtureConfigurationData;
            if (setId != 0)
            {
                if (objUnitHallFixtureConfigurationData != null)
                {
                    var unitHallFixtureConsolesResponseObj = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(updatedUnitHallFixtureConsoleData));
                    _cpqCacheManager.SetCache(sessionId, _environment, Constant.UNITHALLFIXTURECONSOLE, setId.ToString(), Utility.SerializeObjectValue(unitHallFixtureConsolesResponseObj));
                }
                else
                {
                    updatedUnitHallFixtureConsoleData = Utility.DeserializeObjectValue<List<UnitHallFixtures>>(_cpqCacheManager.GetCache(sessionId,
                        _environment, Constant.UNITHALLFIXTURECONSOLE, setId.ToString()));
                }
            }
            Utility.LogEnd(methodBeginTime);
            return updatedUnitHallFixtureConsoleData;
        }

        public void CreateUnitHallFixtureConsoleVariableAssignment(ref ConfigurationRequest cr, UnitHallFixtures console, bool isSave, List<Variables> lstvariables)
        {
            var methodBeginTime = Utility.LogBegin();
            var lstVariableAssignments = new List<VariableAssignment>();
            if (isSave)
            {
                var lstSelectedLandings = console.UnitHallFixtureLocations.Where(x => x.Front.Value.Equals(true) || x.Rear.Value.Equals(true)).ToList();
                if (lstSelectedLandings.Count > 0)
                {
                    foreach (var selecteLanding in lstSelectedLandings)
                    {
                        var strFloornumber = "000";
                        switch (selecteLanding.FloorNumber.ToString().Length)
                        {
                            case 1:
                                strFloornumber = "00" + selecteLanding.FloorNumber.ToString();
                                break;
                            case 2:
                                strFloornumber = "0" + selecteLanding.FloorNumber.ToString();
                                break;
                            case 3:
                                strFloornumber = selecteLanding.FloorNumber.ToString();
                                break;
                        }
                        bool isFront = Convert.ToBoolean(selecteLanding.Front.Value);
                        bool isRear = Convert.ToBoolean(selecteLanding.Rear.Value);
                        foreach (var varAssignment in console.VariableAssignments)
                        {
                            if (isFront)
                            {
                                var lstproperties = (from variable in lstvariables
                                                     from property in variable.Properties
                                                     where variable.Id.Equals(varAssignment.VariableId) &&
                                                     (Utility.CheckEquals(property.Id, "LandingVariableFront") || Utility.CheckEquals(property.Id, "LandingVariable"))
                                                     select property.Value).ToList();
                                if (lstproperties.Count > 0)
                                {
                                    var variableId = lstproperties[0].ToString();
                                    variableId = variableId.Replace("#", strFloornumber);
                                    var variableAssignment = new VariableAssignment()
                                    {
                                        VariableId = variableId,
                                        Value = varAssignment.Value
                                    };
                                    if (variableAssignment.Value.Equals(true))
                                    {
                                        variableAssignment.Value = "TRUE";
                                    }
                                    else if (variableAssignment.Value.Equals(false))
                                    {
                                        variableAssignment.Value = "FALSE";
                                    }
                                    lstVariableAssignments.Add(variableAssignment);
                                }
                            }
                            if (isRear)
                            {
                                var lstproperties = (from variable in lstvariables
                                                     from property in variable.Properties
                                                     where variable.Id.Equals(varAssignment.VariableId) &&
                                                     (Utility.CheckEquals(property.Id, "LandingVariableRear") || Utility.CheckEquals(property.Id, "LandingVariable"))
                                                     select property.Value).ToList();
                                if (isFront)
                                {
                                    lstproperties = (from variable in lstvariables
                                                     from property in variable.Properties
                                                     where variable.Id.EndsWith(varAssignment.VariableId) &&
                                                     Utility.CheckEquals(property.Id, "LandingVariableRear")
                                                     select property.Value).ToList();
                                }
                                if (lstproperties.Count > 0)
                                {
                                    var variableId = lstproperties[0].ToString();
                                    variableId = variableId.Replace("#", strFloornumber);
                                    var variableAssignment = new VariableAssignment()
                                    {
                                        VariableId = variableId,
                                        Value = varAssignment.Value
                                    };
                                    if (variableAssignment.Value.Equals(true))
                                    {
                                        variableAssignment.Value = "TRUE";
                                    }
                                    else if (variableAssignment.Value.Equals(false))
                                    {
                                        variableAssignment.Value = "FALSE";
                                    }
                                    lstVariableAssignments.Add(variableAssignment);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var varAssignment in console.VariableAssignments)
                {
                    var variableAssignment = new VariableAssignment()
                    {
                        VariableId = varAssignment.VariableId,
                        Value = varAssignment.Value
                    };
                    if (variableAssignment.Value.Equals(true))
                    {
                        variableAssignment.Value = "TRUE";
                    }
                    else if (variableAssignment.Value.Equals(false))
                    {
                        variableAssignment.Value = "FALSE";
                    }
                    lstVariableAssignments.Add(variableAssignment);
                }
            }
            cr.Line.VariableAssignments = lstVariableAssignments;
            Utility.LogEnd(methodBeginTime);
        }

        public List<UnitNames> SetCacheUnitsList(List<UnitNames> listUnitsData, string sessionId, int setId)
        {
            var methodBeginTime = Utility.LogBegin();
            var updatedUnitsListData = listUnitsData;
            var username = GetUserId(sessionId);
            if (setId != 0)
            {
                if (listUnitsData != null)
                {
                    var listOfUnitsResponseObj = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(updatedUnitsListData));
                    _cpqCacheManager.SetCache(username, _environment, Constant.LISTOFUNITS, setId.ToString(), Utility.SerializeObjectValue(listOfUnitsResponseObj));
                }
                else
                {
                    updatedUnitsListData = Utility.DeserializeObjectValue<List<UnitNames>>(_cpqCacheManager.GetCache(username, _environment, Constant.LISTOFUNITS, setId.ToString()));
                }
            }
            Utility.LogEnd(methodBeginTime);
            return updatedUnitsListData;
        }

        /// <summary>
        /// EntranceConsoleConfigureBl
        /// </summary>
        /// <param Name="console"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="fixtureType"></param>
        /// <param Name="isSave"></param>
        /// <returns></returns>
        public async Task<JObject> UnitHallFixtureConsoleConfigureBl(UnitHallFixtures console, string sessionId, string fixtureType, int setId, bool isSave)
        {
            var methodBeginTime = Utility.LogBegin();
            var constantMapper = JObject.Parse(File.ReadAllText(Constant.UNITSVARIABLESMAPPERPATH));
            var unithallfixtureContantsDictionary = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.UNITHALLFIXTURECONSTANTMAPPER);
            var productType = SetCacheProductType(null, sessionId, setId).FirstOrDefault().Value.ToString();
            var configureRequest = CreateConfigurationRequestWithTemplate(null, Constant.UNITNAME, null, productType);
            var unitList = SetCacheUnitsList(null, sessionId, setId);
            var stubEntranceResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITHALLFIXTURECONSOLESPATH, Constant.EVOLUTION200)));
            if (productType.Equals(Constant.ENDURA_100))
            {
                stubEntranceResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITHALLFIXTURECONSOLESPATH, Constant.END100)));
            }
            if (productType.Equals(Constant.MODEL_EVO100))
            {
                stubEntranceResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITHALLFIXTURECONSOLESPATH, Constant.EVOLUTION__100)));
            }
            var stubUnitConfigurationResponseObj = stubEntranceResponse.ToObject<ConfigurationResponse>();
            var variablesList = Utility.GetVariables(stubEntranceResponse);
            var listOfVariables = Utility.DeserializeObjectValue<List<Variables>>(Utility.SerializeObjectValue(variablesList));
            CreateUnitHallFixtureConsoleVariableAssignment(ref configureRequest, console, isSave, listOfVariables);
            var mainGroupConfigurationResponse = new ConfigurationResponse
            {
                Sections = new List<Sections>()
            };
            var packagePath = configureRequest?.PackagePath;
            //Generate include section
            var unitsIncludeSection = JObject.Parse(File.ReadAllText(string.Format(Constant.INCLUDESECTIONSTEMPLATE, Constant.EVOLUTION200)));
            var unitsIncludeSectionValues = JArray.FromObject(unitsIncludeSection[Constant.UNITHALLFIXTURECONSOLES]);
            if (productType.Equals(Constant.ENDURA_100))
            {
                unitsIncludeSection = JObject.Parse(File.ReadAllText(string.Format(Constant.INCLUDESECTIONSTEMPLATE, Constant.END100)));
                unitsIncludeSectionValues = JArray.FromObject(unitsIncludeSection[Constant.UNITHALLFIXTURECONSOLES]);
            }
            if (productType.Equals(Constant.MODEL_EVO100))
            {
                unitsIncludeSection = JObject.Parse(File.ReadAllText(string.Format(Constant.INCLUDESECTIONSTEMPLATE, Constant.EVOLUTION__100)));
                unitsIncludeSectionValues = JArray.FromObject(unitsIncludeSection[Constant.UNITHALLFIXTURECONSOLES]);
            }
            var unitsValuesData = Utility.DeserializeObjectValue<List<IncludeSection>>(Utility.SerializeObjectValue(unitsIncludeSectionValues));
            var includeSectionList = (from varunitsValuesData in unitsValuesData
                                      from varentranceConsole in console.UnitHallFixtureLocations
                                      where (varunitsValuesData.ConsoleNumber.Equals(varentranceConsole.FloorNumber.ToString()))
                                      && ((varentranceConsole.Front.Value.Equals(true) || varentranceConsole.Rear.Value.Equals(true)))
                                      select varunitsValuesData.IncludeSections).ToList();
            List<string> includeListEntrance = new List<string>();
            if (isSave)
            {
                if (includeSectionList.Count > 0)
                {
                    foreach (var varincludeSectionList in includeSectionList)
                    {
                        includeListEntrance.AddRange(varincludeSectionList);
                    }
                }
                else
                {
                    includeSectionList = (from varunitsValuesData in unitsValuesData
                                          where varunitsValuesData.ConsoleNumber.Equals(Constant.ZERO)
                                          select varunitsValuesData.IncludeSections).ToList();
                    if (includeSectionList.Count > 0)
                    {
                        includeListEntrance = includeSectionList[0];
                    }
                }
            }
            else
            {
                includeSectionList = (from varunitsValuesData in unitsValuesData
                                      where varunitsValuesData.ConsoleNumber.Equals(Constant.ZERO)
                                      select varunitsValuesData.IncludeSections).ToList();
                if (includeSectionList.Count > 0)
                {
                    includeListEntrance = includeSectionList[0];
                }
            }
            configureRequest.Settings.IncludeSections = includeListEntrance;
            //Gets the base configuration of the model
            var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest);
            var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, packagePath, sessionId
                ).ConfigureAwait(false);
            var baseConfigureResponse = baseConfigureResponseJObj.Response.ToObject<ConfigurationResponse>();
            // configuration object values for conflict mapping
            var configureResponse = baseConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = configureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            // adding defaults to cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTUNITCONFIGVALUES, Utility.SerializeObjectValue(configureResponseArgumentJObject));
            if (!isSave)
            {
                var mainFilteredConfigResponse = Utility.MapVariables(Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(baseConfigureResponse)), Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(stubUnitConfigurationResponseObj)));
                var sectionsData = Utility.DeserializeObjectValue<Sections>(Utility.SerializeObjectValue(mainFilteredConfigResponse));
                var filteredVariables = (from section in sectionsData.sections
                                         where Utility.CheckEquals(section.Id, fixtureType)
                                         from variable in section.Variables
                                         select variable).Distinct().ToList();
                var entranceLocation = new EntranceAssignment()
                {
                    Openings = console.Openings,
                    FixtureAssignments = new List<EntranceLocations>()
                };
                foreach (var varLocation in console.UnitHallFixtureLocations)
                {
                    entranceLocation.FixtureAssignments.Add(varLocation);
                }
                var newConsole = new Sections()
                {
                    Id = console.ConsoleId.ToString(),
                    Name = console.ConsoleName,
                    assignOpenings = console.AssignOpenings,
                    isController = console.IsController,
                    fixtureType = console.UnitHallFixtureType,
                    FixtureLocations = entranceLocation,
                    Variables = filteredVariables == null ? null : filteredVariables,
                    Units = unitList
                };
                foreach (var consoleVariable in newConsole.Variables)
                {
                    if (Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(constantMapper[Constant.SWITCHVARIABLESTOBEREMOVED])).Contains(consoleVariable.Id))
                    {
                        newConsole.Variables = newConsole.Variables.Where(x => !x.Id.Equals(consoleVariable.Id)).ToList();
                    }
                }
                Utility.LogEnd(methodBeginTime);
                return Utility.FilterNullValues(newConsole);
            }
            //filtering main response based on stub response
            return null;
        }

        /// <summary>
        /// start group hall fixture
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="sectionTab"></param>
        /// <param name="fixtureStrategy"></param>
        /// <param name="variableAssignments"></param>
        /// <param name="groupHallFixtureConsoles"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> StartGroupHallFixtures(string sessionId, string sectionTab, string fixtureStrategy, JObject variableAssignments = null, List<GroupHallFixtures> groupHallFixtureConsoles = null)
        {
            var methodBeginTime = Utility.LogBegin();
            var groupContantsDictionary = Utility.GetVariableMapping(Constant.GROUPMAPPERVARIABLES, Constant.GROUPCONSTANTMAPPER);
            var groupConfigurationResponseObj = new ConfigurationResponse();
            var groupHallFixtureUiResponse = JObject.Parse(File.ReadAllText(Constant.GROUPHALLFIXTUREUITEMPLATE));
            var consoleTemplate = JObject.Parse(File.ReadAllText(Constant.GROUPHALLFIXTURECONSOLEPATH));
            var consoleResponse = consoleTemplate.ToObject<ConfigurationResponse>();
            var constantMapper = JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES));
            var currentFixtureStrategy = Constant.ETACONSOLES;
            if (fixtureStrategy.Equals(Constant.ETD))
            {
                currentFixtureStrategy = Constant.ETDCONSOLES;
            }
            else if (fixtureStrategy.Equals(Constant.ETA_AND_ETD))
            {
                currentFixtureStrategy = Constant.ETDETDCONSOLES;
            }

            GetConsolesForFixtureStrategy(currentFixtureStrategy, constantMapper, ref consoleResponse);
            groupConfigurationResponseObj = groupHallFixtureUiResponse.ToObject<ConfigurationResponse>();
            var totalConsoles = new ConfigurationResponse();
            // group dl call
            var fixtureTypesList = _unitConfigurationDL.GetGroupHallFixturesTypesList(fixtureStrategy);
            fixtureTypesList = fixtureTypesList.ConvertAll(d => d.ToUpper());
            List<string> fixturesStored = new List<string>();

            fixturesStored = groupHallFixtureConsoles.Where(x => x.ConsoleId != 0 && fixtureTypesList.Contains(x.GroupHallFixtureType.ToUpper())).Select(x => x.GroupHallFixtureType.ToUpper()).ToList();
            //adding the fixture sections for the resoponse
            consoleResponse.Sections = consoleResponse.Sections.Where(x => fixturesStored.Contains(x.Id.ToUpper())).ToList();
            //getting the order of consoles
            List<String> orderOfConsoles = groupHallFixtureConsoles.Distinct().Select(x => x.GroupHallFixtureType).ToList();

            foreach (var subsection in consoleResponse.Sections)
            {
                var currentSectionName = subsection.Id;
                subsection.sections = new List<SectionsValues>();
                foreach (var console in groupHallFixtureConsoles)
                {
                    if (console.ConsoleId > 0)
                    {
                        if (Utility.CheckEquals(currentSectionName, console.GroupHallFixtureType.ToString()))
                        {
                            var frontRearAssignment = Utility.GetLandingOpeningAssignmentSelectedForGroupHallFixture(console.UnitDetails).Replace(Constants.COLON, String.Empty);
                            var consoleSection = new SectionsValues();
                            consoleSection = Utility.DeserializeObjectValue<SectionsValues>(Utility.SerializeObjectValue(subsection));
                            consoleSection.Id = console.ConsoleId.ToString();
                            consoleSection.Name = subsection.Id + Constant.DOT + console.ConsoleName;
                            consoleSection.OpeningRange = console.GroupHallFixtureType != Constants.AGILEHALLSTATION ? Utility.GetOpeningForHallStation(console.HallStations) : frontRearAssignment == Constant.SPACE ? Constant.ZEROOPENINGS : frontRearAssignment;
                            consoleSection.assignOpenings = console.AssignOpenings;
                            consoleSection.IsDelete = !console.IsController;
                            consoleSection.fixtureType = console.GroupHallFixtureType;
                            consoleSection.IsHallStation = true;

                            var newflag = (from val in console.VariableAssignments
                                           where string.IsNullOrEmpty(Convert.ToString(val.Value))
                                           select val).ToList();
                            var flagnew = 0;
                            if (console.VariableAssignments.Count() != newflag.Count())
                            {
                                flagnew = 1;
                            }
                            if (console.VariableAssignments.Count >= 1 && flagnew == 1)
                            {
                                foreach (var consoleVariables in consoleSection.Variables)
                                {
                                    var variableValue = (from variable in console.VariableAssignments
                                                         where Utility.CheckEquals(variable.VariableId, consoleVariables.Id)
                                                         select variable.Value).FirstOrDefault();
                                    consoleVariables.Value = variableValue;
                                }
                            }
                            foreach (var consoleVariable in consoleSection.Variables)
                            {
                                if (Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(constantMapper[Constant.SWITCHVARIABLESTOBEREMOVED])).Contains(consoleVariable.Id))
                                {
                                    consoleSection.Variables = consoleSection.Variables.Where(x => !x.Id.Equals(consoleVariable.Id)).ToList();
                                }
                                if (consoleVariable.Id.Equals(groupContantsDictionary[Constants.SPAREQUANTITY], StringComparison.OrdinalIgnoreCase) && (consoleVariable.Value == null || consoleVariable.Value.ToString() == Constants.ZERO))
                                {
                                    consoleSection.Variables = consoleSection.Variables.Where(x => !x.Id.Equals(consoleVariable.Id)).ToList();
                                }
                            }
                            foreach (var varProperty in consoleSection.Properties)
                            {
                                switch (varProperty.Id)
                                {
                                    case Constant.SECTIONNAMEPASCAL:
                                        varProperty.Value = console.ConsoleName;
                                        break;
                                }
                            }
                            subsection.sections.Add(consoleSection);
                            subsection.sections = (from consoleData in subsection.sections orderby consoleData.Name select consoleData).ToList();
                        }
                    }
                }
                subsection.Variables = new List<Variables>();

            }
            foreach (var subsection in groupConfigurationResponseObj.Sections)
            {
                if (Utility.CheckEquals(subsection.Id, Constant.ADDFIXTURES))
                {
                    foreach (var item in subsection.Variables)
                    {
                        List<Values> listOfFixtures = new List<Values>();
                        foreach (var diffValues in item.Values)
                        {
                            if (fixtureTypesList.Contains(diffValues.id.ToUpper()))
                            {
                                listOfFixtures.Add(diffValues);
                            }
                        }
                        item.Values = listOfFixtures;
                    }
                }
                if (Utility.CheckEquals(subsection.Id, Constant.CONSOLES))
                {
                    subsection.sections = Utility.DeserializeObjectValue<IList<SectionsValues>>(Utility.SerializeObjectValue(consoleResponse.Sections));
                }
            }
            var enrichedData = JObject.Parse(File.ReadAllText(Constant.ELEVATORENRICHMENTTEMPLATE));
            groupConfigurationResponseObj.EnrichedData = enrichedData;
            groupConfigurationResponseObj.AllOpeningsSelected = GetAllOpeningsFlagForGroupHallFixtures(groupHallFixtureConsoles, fixtureStrategy);
            if (groupConfigurationResponseObj != null)
            {
                return new ResponseMessage
                {
                    StatusCode = Constant.SUCCESS,
                    Response = Utility.FilterNullValues(groupConfigurationResponseObj)
                };
            }
            Utility.LogEnd(methodBeginTime);
            throw new CustomException(new ResponseMessage
            {
                StatusCode = Constant.BADREQUEST,
                Message = _localizer[Constant.REQUESTCANNOTBENULL]
            });
        }
        /// <summary>
        /// SetCacheGroupHallFixtureConsoles
        /// </summary>
        /// <param Name="objGroupHallFixturesData"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        public List<GroupHallFixtures> SetCacheGroupHallFixtureConsoles(List<GroupHallFixtures> objGroupHallFixturesData, string sessionId, int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            var updatedGroupHallFixtureConsoleData = objGroupHallFixturesData;
            if (groupId != 0)
            {
                if (objGroupHallFixturesData != null)
                {
                    var groupHallFixtureConsolesResponseObj = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(updatedGroupHallFixtureConsoleData));
                    _cpqCacheManager.SetCache(sessionId, _environment, Constant.GROUPHALLFIXTURECONSOLE, groupId.ToString(), Utility.SerializeObjectValue(groupHallFixtureConsolesResponseObj));
                }
                else
                {
                    updatedGroupHallFixtureConsoleData = Utility.DeserializeObjectValue<List<GroupHallFixtures>>(_cpqCacheManager.GetCache(sessionId,
                        _environment, Constant.GROUPHALLFIXTURECONSOLE, groupId.ToString()));
                }
            }
            Utility.LogEnd(methodBeginTime);
            return updatedGroupHallFixtureConsoleData;
        }
        /// <summary>
        /// Creating GroupHallFixtureConsole VariableAssignments
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="groupConsole"></param>
        /// <param name="isSave"></param>
        /// <param name="lstvariables"></param>
        public void CreateGroupHallFixtureConsoleVariableAssignment(ref ConfigurationRequest cr, GroupHallFixtures groupConsole, bool isSave, List<Variables> lstvariables)
        {
            var methodBeginTime = Utility.LogBegin();
            var lstVariableAssignments = new List<VariableAssignment>();
            if (isSave)
            {
                var lstSelectedLandings = groupConsole.GroupHallFixtureLocations.Where(x => x.Front.Value.Equals(true) || x.Rear.Value.Equals(true)).ToList();
                if (lstSelectedLandings.Count > 0)
                {
                    foreach (var selecteLanding in lstSelectedLandings)
                    {

                        var strFloornumber = selecteLanding.FloorNumber.ToString().PadLeft(3, '0');
                        bool isFront = Convert.ToBoolean(selecteLanding.Front.Value);
                        bool isRear = Convert.ToBoolean(selecteLanding.Rear.Value);
                        foreach (var varAssignment in groupConsole.VariableAssignments)
                        {
                            if (isFront)
                            {
                                var lstproperties = (from variable in lstvariables
                                                     from property in variable.Properties
                                                     where variable.Id.Equals(varAssignment.VariableId) &&
                                                     (Utility.CheckEquals(property.Id, Constant.LANDINGVARIABLEFRONT) || Utility.CheckEquals(property.Id, Constant.LANDINGVARIABLE))
                                                     select property.Value).ToList();
                                if (lstproperties.Count > 0)
                                {
                                    var variableId = lstproperties[0].ToString();
                                    variableId = variableId.Replace("#", strFloornumber);
                                    var variableAssignment = new VariableAssignment()
                                    {
                                        VariableId = variableId,
                                        Value = varAssignment.Value
                                    };
                                    if (!string.IsNullOrEmpty(Convert.ToString(variableAssignment.Value)))
                                    {
                                        if (variableAssignment.Value.Equals(true))
                                        {
                                            variableAssignment.Value = Constant.TRUEVALUES;
                                        }
                                        else if (variableAssignment.Value.Equals(false))
                                        {
                                            variableAssignment.Value = Constant.FALSEVALUES;
                                        }
                                        lstVariableAssignments.Add(variableAssignment);
                                    }
                                }
                            }
                            if (isRear)
                            {
                                var lstproperties = (from variable in lstvariables
                                                     from property in variable.Properties
                                                     where variable.Id.Equals(varAssignment.VariableId) &&
                                                     (Utility.CheckEquals(property.Id, Constant.LANDINGVARIABLEREAR) || Utility.CheckEquals(property.Id, Constant.LANDINGVARIABLE))
                                                     select property.Value).ToList();
                                if (isFront)
                                {
                                    lstproperties = (from variable in lstvariables
                                                     from property in variable.Properties
                                                     where variable.Id.EndsWith(varAssignment.VariableId) &&
                                                     Utility.CheckEquals(property.Id, Constant.LANDINGVARIABLEREAR)
                                                     select property.Value).ToList();
                                }
                                if (lstproperties.Count > 0)
                                {
                                    var variableId = lstproperties[0].ToString();
                                    variableId = variableId.Replace("#", strFloornumber);
                                    var variableAssignment = new VariableAssignment()
                                    {
                                        VariableId = variableId,
                                        Value = varAssignment.Value
                                    };
                                    if (!string.IsNullOrEmpty(Convert.ToString(variableAssignment.Value)))
                                    {
                                        if (variableAssignment.Value.Equals(true))
                                        {
                                            variableAssignment.Value = Constant.TRUEVALUES;
                                        }
                                        else if (variableAssignment.Value.Equals(false))
                                        {
                                            variableAssignment.Value = Constant.FALSEVALUES;
                                        }
                                        lstVariableAssignments.Add(variableAssignment);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var varAssignment in groupConsole.VariableAssignments)
                {
                    var variableAssignment = new VariableAssignment()
                    {
                        VariableId = varAssignment.VariableId,
                        Value = varAssignment.Value
                    };
                    if (!string.IsNullOrEmpty(Convert.ToString(variableAssignment.Value)))
                    {
                        if (variableAssignment.Value.Equals(true))
                        {
                            variableAssignment.Value = Constant.TRUEVALUES;
                        }
                        else if (variableAssignment.Value.Equals(false))
                        {
                            variableAssignment.Value = Constant.FALSEVALUES;
                        }
                        lstVariableAssignments.Add(variableAssignment);
                    }

                }
            }
            cr.Line.VariableAssignments = lstVariableAssignments;
            Utility.LogEnd(methodBeginTime);
        }
        /// <summary>
        /// group hall fixture console value mapping
        /// </summary>
        /// <param name="groupConsole"></param>
        /// <param name="sessionId"></param>
        /// <param name="fixtureType"></param>
        /// <param name="isSave"></param>
        /// <param name="fixtureStrategy"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<JObject> GroupHallFixtureConsoleConfigureBl(GroupHallFixtures groupConsole, string sessionId, string fixtureType, bool isSave, string fixtureStrategy, int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            var groupContantsDictionary = Utility.GetVariableMapping(Constant.GROUPMAPPERVARIABLES, Constant.GROUPCONSTANTMAPPER);
            var constantMapper = JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES));
            var configureRequest = CreateConfigurationRequestWithTemplate(null, Constant.GROUPCONFIGURATIONNAME);
            var groupHallFixtureConsoleTemplate = JObject.Parse(File.ReadAllText(Constant.GROUPHALLFIXTURECONSOLEPATH));
            var groupHallFixtureConsoleTemplateObj = groupHallFixtureConsoleTemplate.ToObject<ConfigurationResponse>();
            var lstvariables = Utility.GetAllSectionVariables(groupHallFixtureConsoleTemplateObj.Sections);
            CreateGroupHallFixtureConsoleVariableAssignment(ref configureRequest, groupConsole, isSave, lstvariables);
            var packagePath = configureRequest?.PackagePath;
            VariableAssignment strategy = new VariableAssignment
            {
                VariableId = Constant.ELEVATORFIXTURESTRATEGY,
                Value = fixtureStrategy
            };
            List<VariableAssignment> va = new List<VariableAssignment>
            {
                strategy
            };
            va.AddRange(configureRequest.Line.VariableAssignments);
            configureRequest.Line.VariableAssignments = va;
            //Generate include section
            var groupIncludeSectionValues = JObject.Parse(File.ReadAllText(Constant.GROUPINLUDESECTIONTEMPATE));
            var groupValuesData = Utility.DeserializeObjectValue<List<IncludeSection>>(Utility.SerializeObjectValue(groupIncludeSectionValues[Constant.GROUPHALLFIXTURE.ToLower()]));
            var includeListEntrance = groupValuesData[0].IncludeSections.ToList();
            //for changing firebox value from False to NR
            if (Utility.CheckEquals(fixtureType, Constant.FIRESERVICE))
            {
                foreach (var variableInRequest in configureRequest.Line.VariableAssignments)
                {
                    if (variableInRequest.VariableId.Equals(groupContantsDictionary[Constant.FIREBOXCONSTANT]) && variableInRequest.Value.Equals(Constant.FALSEVALUES))
                    {
                        variableInRequest.Value = Constant.NR;
                    }
                }
            }
            configureRequest.Settings.IncludeSections = includeListEntrance;
            //Gets the base configuration of the model
            var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest);
            var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, packagePath, sessionId
                ).ConfigureAwait(false);
            var baseConfigureResponse = baseConfigureResponseJObj.Response.ToObject<ConfigurationResponse>();
            // defaults call
            var configureResponse = baseConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = configureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTGROUPCONFIGVALUES, Utility.SerializeObjectValue(configureResponseArgumentJObject));
            if (!isSave)
            {

                var mainFilteredConfigResponse = Utility.MapVariables(Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(baseConfigureResponse)), Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(groupHallFixtureConsoleTemplateObj)));
                var sectionsData = Utility.DeserializeObjectValue<Sections>(Utility.SerializeObjectValue(mainFilteredConfigResponse));
                var lstVariables = (from section in sectionsData.sections
                                    where Utility.CheckEquals(section.Id, fixtureType)
                                    from variable in section.Variables
                                    select variable).Distinct().ToList();

                //changing the value of Fire Key Box from NR to False
                if (Utility.CheckEquals(fixtureType, Constant.FIRESERVICE))
                {
                    foreach (var variables in lstVariables)
                    {
                        if (Utility.CheckEquals(variables.Id, groupContantsDictionary[Constant.FIREBOXCONSTANT]))
                        {
                            foreach (var values in variables.Values)
                            {
                                if (values.value.Equals(Constant.NR))
                                {
                                    values.value = false;
                                    values.Name = Constant.FALSEVALUES;
                                }
                            }
                        }
                    }
                }
                var layoutDetails = _cpqCacheManager.GetCache(sessionId, _environment, groupId.ToString());
                var groupLayoutDetails = new GroupDetails();
                if (layoutDetails != null)
                {
                    groupLayoutDetails = Utilities.DeserializeObjectValue<GroupDetails>(layoutDetails);

                }
                var newConsole = new Sections()
                {
                    Id = groupConsole.ConsoleId.ToString(),
                    fixtureType = fixtureType,
                    Name = groupConsole.ConsoleName,
                    assignOpenings = groupConsole.AssignOpenings,
                    isController = groupConsole.IsController,
                    UnitDetails = groupConsole.UnitDetails,
                    FixtureAssignments = groupConsole.FixtureAssignments,
                    Variables = lstVariables,
                    IsHallStation = fixtureType == Constants.AGILEHALLSTATION ? false : true,
                    UnitLayoutDetails = groupLayoutDetails?.unitLayoutDetails,
                    HallStationDetails = groupLayoutDetails?.hallStationDetails
                };
                foreach (var consoleVariable in newConsole.Variables)
                {
                    if (Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(constantMapper[Constant.SWITCHVARIABLESTOBEREMOVED])).Contains(consoleVariable.Id))
                    {
                        newConsole.Variables = newConsole.Variables.Where(x => !x.Id.Equals(consoleVariable.Id)).ToList();
                    }
                }
                Utility.LogEnd(methodBeginTime);
                return Utility.FilterNullValues(newConsole);
            }
            return null;
        }

        public async Task<JObject> BuildingEquipmentConsoleConfigureBl(BuildingEquipmentData entranceConsole, string sessionId, bool isSave, int buildingId, bool editFlag, bool islobby, bool isChange)
        {
            var methodBeginTime = Utility.LogBegin();
            var configureRequest = CreateConfigurationRequestWithTemplate(null, Constant.BUILDINGCONFIGURATION);
            var stubEntranceResponse = JObject.Parse(File.ReadAllText(Constant.BUILDINGEQUIPMENTCONSOLETEMPLATEPATH));
            if (islobby)
            {
                var stubResponse = JObject.Parse(File.ReadAllText(Constant.BUILDINGEQUIPMENTLOBBYPANELRESPONSE));
                stubEntranceResponse = stubResponse;
            }
            var stubUnitConfigurationResponseObj = stubEntranceResponse.ToObject<ConfigurationResponse>();

            var appSections = Utility.GetTokens(Constant.TOKENSECTIONS, stubEntranceResponse, false).ToList();
            var layoutSections = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(appSections));
            var appVariables = Utility.GetTokens(Constant.VARIABLES, layoutSections, false).ToList();
            var lstvariables = Utility.DeserializeObjectValue<List<Variables>>(Utility.SerializeObjectValue(appVariables));
            CreateBuildingEquipmentConsoleVariableAssignment(ref configureRequest, entranceConsole, isSave);
            var mainGroupConfigurationResponse = new ConfigurationResponse
            {
                Sections = new List<Sections>()
            };
            var packagePath = configureRequest?.PackagePath;
            //Generate include section
            var unitsIncludeSectionValues = JArray.Parse(File.ReadAllText(Constant.BUILDINGEQUIPMENTINCLUDESECTIONVALUES));
            var stubLobbyPanelConfigurationMainResponseObj = new ConfigurationResponse();
            var stubLobbySubSectionResponse = JObject.Parse(File.ReadAllText(Constant.BUILDINGEQUIPMENTLOBBYPANELSUBSECTIONS));
            var stubLobbySubSectionProperties = JArray.Parse(File.ReadAllText(Constant.BUILDINGEQUIPMENTPROPERTIESSTUB));
            // setting stub data into an sectionsValues object
            stubLobbyPanelConfigurationMainResponseObj = stubLobbySubSectionResponse.ToObject<ConfigurationResponse>();
            var unitsValuesData = Utility.DeserializeObjectValue<List<IncludeSection>>(Utility.SerializeObjectValue(unitsIncludeSectionValues));
            var includeSectionList = (from varunitsValuesData in unitsValuesData
                                      select varunitsValuesData.IncludeSections).ToList();
            List<string> includeListEntrance = new List<string>();
            foreach (var varincludeSectionList in includeSectionList)
            {
                includeListEntrance.AddRange(varincludeSectionList);
            }
            var buildingEquipmentContantsDictionary = Utility.GetVariableMapping(Constant.BUILDINGMAPPERVARIABLESMAPPERPATH, Constant.BUILDINGEQUIPMENTVARIABLES);
            foreach (var variableInRequest in configureRequest.Line.VariableAssignments)
            {
                foreach (var variableInStub in lstvariables)
                {
                    if (variableInRequest.VariableId == variableInStub.Id)
                    {
                        if ((variableInRequest.VariableId.Equals(buildingEquipmentContantsDictionary[Constant.FIREPHONEJACK]) || variableInRequest.VariableId.Equals(buildingEquipmentContantsDictionary[Constant.FIREBOX])) && variableInRequest.Value.Equals(Constant.FALSEVALUES))
                        {
                            variableInRequest.Value = Constant.NR;
                        }
                    }
                }
            }
            configureRequest.Settings.IncludeSections = includeListEntrance;
            //Gets the base configuration of the model
            var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest);
            var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, packagePath, sessionId
                ).ConfigureAwait(false);
            var baseConfigureResponse = baseConfigureResponseJObj.Response.ToObject<ConfigurationResponse>();
            // configuration object values for conflict mapping
            var configureResponse = baseConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = configureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            // adding defaults to cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTBUILDINGEQUIPMENTCONFIGVALUES, Utility.SerializeObjectValue(configureResponseArgumentJObject));
            if (!isSave)
            {
                var filteredBasesResponse = Utility.MapVariables(Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(baseConfigureResponse)),
                    Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(stubUnitConfigurationResponseObj)));
                var filteredSectionResponse = Utility.DeserializeObjectValue<ConfigurationResponse>(Utility.SerializeObjectValue(filteredBasesResponse));
                var layoutSectionsData = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(filteredSectionResponse.Sections));
                var appVariablesData = Utility.GetTokens(Constant.VARIABLES, layoutSectionsData, false).ToList();
                var lstvariablesData = Utility.DeserializeObjectValue<List<Variables>>(Utility.SerializeObjectValue(appVariables));
                var sectionsList = Utility.DeserializeObjectValue<List<Sections>>(Utility.SerializeObjectValue(layoutSectionsData));
                List<Variables> lstFilteredVariables = new List<Variables>();
                var newConsole = new Sections();
                if (editFlag.Equals(true) && islobby.Equals(false))
                {
                    if (entranceConsole.ConsoleName.Equals(Constant.KEYWORDSMARTRESCUEPHONE5) || entranceConsole.ConsoleName.Equals(Constant.KEYWORDSMARTRESCUEPHONE10))
                    {
                        var currentVariables = (from section in sectionsList[0].sections
                                                where section.Id.Contains(entranceConsole.ConsoleName.Split().First(), StringComparison.InvariantCultureIgnoreCase)
                                                select section.Variables).ToList();
                        lstFilteredVariables.AddRange(currentVariables.FirstOrDefault());
                    }
                    else
                    {
                        var currentVariables = (from section in sectionsList[0].sections
                                                where section.Id.Equals(Constants.THIRDPARTYINTERFACES)
                                                from sections in section.sections
                                                where sections.Id.Contains(entranceConsole.ConsoleName.Split().First(), StringComparison.InvariantCultureIgnoreCase)
                                                select sections.Variables).ToList();
                        lstFilteredVariables.AddRange(currentVariables.FirstOrDefault());

                    }
                    var newConsole1 = new Sections()
                    {
                        Id = entranceConsole.ConsoleId.ToString(),
                        Name = entranceConsole.ConsoleName,
                        isController = entranceConsole.IsController,
                        isLobby = entranceConsole.IsLobby,
                        configuredGroups = entranceConsole.lstConfiguredGroups,
                        sections = new List<SectionsValues>(),
                        Variables = new List<Variables>()
                    };
                    var prop = Utility.DeserializeObjectValue<IList<PropertyDetailsValues>>(Utility.SerializeObjectValue(stubLobbySubSectionProperties));
                    var newId = string.Empty;
                    if (entranceConsole.ConsoleName.Equals(Constant.KEYWORDBACNET))
                    {
                        newId = Constant.BACNETSECTION;
                        foreach (var properties in prop)
                        {
                            if (properties.Id.Equals(Constant.SECTIONNAME))
                            {
                                properties.Value = Constant.BACNETSECTIONS;
                            }
                        }
                    }
                    else if (entranceConsole.ConsoleName.Equals(Constant.KEYWORDROBOTICCONTROLLERINTERFACE))
                    {
                        newId = Constant.ROBOTICCONTROLLERSECTION;
                        foreach (var properties in prop)
                        {
                            if (properties.Id.Equals(Constant.SECTIONNAME))
                            {
                                properties.Value = Constant.ROBOTICCONTROLLERSECTIONS;
                            }
                        }
                    }
                    else if (entranceConsole.ConsoleName.Equals(Constant.KEYWORDSMARTRESCUEPHONE5))
                    {
                        newId = Constant.SMARTRESCUEPHONE5SECTION;
                        foreach (var properties in prop)
                        {
                            if (properties.Id.Equals(Constant.SECTIONNAME))
                            {
                                properties.Value = string.Empty;
                            }
                        }
                    }
                    else if (entranceConsole.ConsoleName.Equals(Constant.KEYWORDSMARTRESCUEPHONE10))
                    {
                        newId = Constant.SMARTRESCUEPHONE10SECTION;
                        foreach (var properties in prop)
                        {
                            if (properties.Id.Equals(Constant.SECTIONNAME))
                            {
                                properties.Value = string.Empty;
                            }
                        }
                    }
                    var lobbySections = new SectionsValues()
                    {
                        Id = newId,
                        Variables = new List<Variables>(),
                        sections = new List<SectionsGroupValues>(),
                        Properties = prop
                    };
                    var filterVariablesData = lstFilteredVariables.GroupBy(getEachValues => getEachValues.Id).Select(getFilterValues => getFilterValues.FirstOrDefault()).ToList();
                    lobbySections.Variables = filterVariablesData == null ? null : filterVariablesData;
                    if (entranceConsole.ConsoleName.Equals(Constant.KEYWORDSMARTRESCUEPHONE5))
                    {
                        lobbySections.Variables = filterVariablesData == null ? null : filterVariablesData.Where(x => x.Id.Equals(Constants.SMARTRESCUEPHONE5SPPARAM)).ToList();
                    }
                    else if (entranceConsole.ConsoleName.Equals(Constant.KEYWORDSMARTRESCUEPHONE10))
                    {
                        lobbySections.Variables = filterVariablesData == null ? null : filterVariablesData.Where(x => x.Id.Equals(Constants.SMARTRESCUEPHONE10SPPARAM)).ToList();
                    }
                    newConsole1.sections.Add(lobbySections);
                    newConsole = newConsole1;
                }
                else
                {
                    foreach (var subsectionlobby in filteredSectionResponse.Sections[0].sections)
                    {
                        //changing the value of Fire Phone Jack from NR to False
                        if (Utility.CheckEquals(subsectionlobby.Id, buildingEquipmentContantsDictionary[Constant.FEATURESPERPANELL]))
                        {
                            foreach (var variables in subsectionlobby.Variables)
                            {
                                if (Utility.CheckEquals(variables.Id, buildingEquipmentContantsDictionary[Constant.FIREPHONEJACK]) || Utility.CheckEquals(variables.Id, buildingEquipmentContantsDictionary[Constant.FIREBOX]))
                                {
                                    foreach (var values in variables.Values)
                                    {
                                        if (values.value.Equals(Constant.NR))
                                        {
                                            values.value = false;
                                            values.Name = Constant.FALSEVALUES;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    var newConsole1 = new Sections()
                    {
                        Id = entranceConsole.ConsoleId.ToString(),
                        Name = entranceConsole.ConsoleName,
                        isController = entranceConsole.IsController,
                        isLobby = entranceConsole.IsLobby,
                        configuredGroups = entranceConsole.lstConfiguredGroups,
                        existingGroups = entranceConsole.lstExistingGroups,
                        futureGroups = entranceConsole.lstFutureGroup,
                        sections = new List<SectionsValues>(),
                        Variables = new List<Variables>()
                    };
                    if (isChange)
                    {
                        var cachedConsole = SetCacheBuildingEquipmentConsoles(null, sessionId, buildingId, true);
                        if (cachedConsole.FirstOrDefault().ExistingGroups != null)
                        {
                            newConsole1.existingGroups = cachedConsole.FirstOrDefault().ExistingGroups;
                        }
                        if (cachedConsole.FirstOrDefault().FutureGroups != null)
                        {
                            newConsole1.futureGroups = cachedConsole.FirstOrDefault().FutureGroups;
                        }
                        if (cachedConsole.FirstOrDefault().ConfiguredGroups != null)
                        {
                            newConsole1.configuredGroups = Utility.DeserializeObjectValue<IList<ConfiguredGroups>>(Utility.SerializeObjectValue(cachedConsole.FirstOrDefault().ConfiguredGroups));
                        }
                    }
                    var lobbySections = new SectionsValues()
                    {
                        Id = Constant.LOBBYPANELSECTION,
                        Variables = new List<Variables>(),
                        sections = new List<SectionsGroupValues>(),
                        Properties = Utility.DeserializeObjectValue<IList<PropertyDetailsValues>>(Utility.SerializeObjectValue(stubLobbySubSectionProperties))
                    };
                    var mainSectionValues = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(filteredSectionResponse.Sections[0].sections));
                    lobbySections.sections = Utility.DeserializeObjectValue<IList<SectionsGroupValues>>(Utility.SerializeObjectValue(mainSectionValues));
                    newConsole1.sections.Add(lobbySections);
                    newConsole = newConsole1;
                }
                newConsole.EnrichedData = JObject.Parse(File.ReadAllText(Constant.BUILDINGENRICHEDDATA));
                var lobbyRecallSwitchData = _cpqCacheManager.GetCache(sessionId, _environment, buildingId.ToString(), Constant.LOBBYRECALLSWITCHVARAIBLES);
                if (!String.IsNullOrEmpty(lobbyRecallSwitchData))
                {
                    newConsole.LobbyRecallSwitchPerGroup = Utility.DeserializeObjectValue<List<LobbyRecallSwitchConfigured>>(lobbyRecallSwitchData);
                }
                Utility.LogEnd(methodBeginTime);
                return Utility.FilterNullValues(newConsole);
            }
            //filtering main response based on stub response
            return null;
        }

        public void CreateBuildingEquipmentConsoleVariableAssignment(ref ConfigurationRequest cr, BuildingEquipmentData entranceConsole, bool isSave)
        {
            var methodBeginTime = Utility.LogBegin();
            var lstVariableAssignments = new List<VariableAssignment>();
            if (!isSave)
            {
                if (entranceConsole.VariableAssignments != null)
                {
                    foreach (var varAssignment in entranceConsole.VariableAssignments)
                    {
                        var variableAssignment = new VariableAssignment()
                        {
                            VariableId = varAssignment.VariableId,
                            Value = varAssignment.Value
                        };
                        if (variableAssignment.Value.Equals(true))
                        {
                            variableAssignment.Value = Constant.TRUEVALUES;
                        }
                        else if (variableAssignment.Value.Equals(false))
                        {
                            variableAssignment.Value = Constant.FALSEVALUES;
                        }
                        lstVariableAssignments.Add(variableAssignment);
                    }
                }
                else
                {
                    entranceConsole.VariableAssignments = new List<ConfigVariable>();
                }
            }
            cr.Line.VariableAssignments = lstVariableAssignments;
            Utility.LogEnd(methodBeginTime);
        }

        /// <summary>
        /// Common Method for Get and Set Cache
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="variableAssignments"></param>
        /// <param name="packageName"></param>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public ConfigurationRequest GetAndSetCacheConfiguration(string sessionId, JObject variableAssignments, string packageName, string cacheKey)
        {
            var methodBeginTime = Utility.LogBegin();
            var configureRequest = CreateConfigurationRequestWithTemplate(variableAssignments, packageName);
            var mainGroupConfigurationResponse = new ConfigurationResponse
            {
                Sections = new List<Sections>()
            };
            var packagePath = configureRequest?.PackagePath;
            var modifiedOn = new DateTime();
            var currentConfigurationCache = _cpqCacheManager.GetCache(sessionId, _environment, cacheKey);
            if (configureRequest != null && (string.IsNullOrEmpty(packagePath)))
            {
                if (String.IsNullOrEmpty(currentConfigurationCache))
                {
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = _localizer[Constant.SESSIONIDPARENTCODEISNOMOREVALID]
                    });
                }
                var currentConfiguration = Utility.DeserializeObjectValue<StartConfigureResponse>(currentConfigurationCache);
                if (currentConfiguration.Audits.ModifiedOn == null)
                {
                    currentConfiguration.Audits.ModifiedOn = new DateTime();
                }
                if (currentConfiguration != null && modifiedOn != default(DateTime) &&
                    DateTime.Compare(modifiedOn, (DateTime)currentConfiguration.Audits.ModifiedOn) > 0)
                {
                    currentConfiguration.Audits.ModifiedOn = modifiedOn;
                }
                var value = Utility.SerializeObjectValue(currentConfiguration);
                _cpqCacheManager.SetCache(sessionId, _environment, cacheKey, value);
                //Response for UI
                var updatedStartConfigureResponse = ViewModelResponseMapper(currentConfiguration);
            }
            Utility.LogEnd(methodBeginTime);
            return configureRequest;
        }

        /// <summary>
        /// Method for Getting all the Dividers
        /// </summary>
        /// <param name="bank1Count"></param>
        /// <param name="bank2Count"></param>
        /// <param name="dividersections"></param>
        /// <returns></returns>
        public List<Sections> GetBeamWallDividers(int bank1Count, int bank2Count, Sections dividersections)
        {
            var methodBeginTime = Utility.LogBegin();
            //Generating Dividers
            List<Sections> allDividers = new List<Sections>();
            if (bank1Count > 1)
            {
                List<Sections> bank1Dividers = new List<Sections>();
                for (int i = 1; i <= (bank1Count - 1); i++)
                {
                    var sectionBank1 = GetDividerSections(dividersections, i, i);
                    bank1Dividers.Add(sectionBank1);
                }

                allDividers.AddRange(bank1Dividers);
            }

            if (bank2Count > 1)
            {
                List<Sections> bank2Dividers = new List<Sections>();
                for (int i = 1; i <= (bank2Count - 1); i++)
                {
                    var sectionBank2 = GetDividerSections(dividersections, allDividers.Count() + i, bank1Count + i);
                    bank2Dividers.Add(sectionBank2);
                }

                allDividers.AddRange(bank2Dividers);
            }
            Utility.LogEnd(methodBeginTime);
            return allDividers;
        }

        /// <summary>
        /// Method for UnitLayoutGeneration
        /// </summary>
        /// <param name="lstUnitLayoutDetails"></param>
        /// <param name="layoutSections"></param>
        /// <returns></returns>
        public List<SectionsValues> UnitLayoutGeneration(List<UnitLayOutDetails> lstUnitLayoutDetails, Sections layoutSections)
        {
            var methodBeginTime = Utility.LogBegin();
            List<SectionsValues> lstSectionSumpPit = new List<SectionsValues>();
            int count = 1;
            foreach (var unitLayout in lstUnitLayoutDetails)
            {
                SectionsValues sectionSumpPit = new SectionsValues() { Variables = new List<Variables>(), Layout = new List<UnitLayOutDetails>() };
                unitLayout.layoutVariables = new List<Variables>();
                sectionSumpPit.Layout.Add(unitLayout);
                foreach (var variable in layoutSections.sections.FirstOrDefault().Variables)
                {
                    Variables variables = new Variables() { Id = variable.Id.Replace(Convert.ToString(Constant.HASH), Convert.ToString(count)), Value = string.Empty };
                    sectionSumpPit.Variables.Add(variables);
                }
                lstSectionSumpPit.Add(sectionSumpPit);
                count++;
            }
            Utility.LogEnd(methodBeginTime);
            return lstSectionSumpPit;
        }

        /// <summary>
        /// Method for checking the Wall Warning Message 
        /// </summary>
        /// <param name="baseConfigureRequest"></param>
        /// <param name="unitConfigurationMainResponseObj"></param>
        /// <param name="bank1Count"></param>
        /// <param name="bank2Count"></param>
        public void CheckWallWarningMessage(ConfigurationRequest baseConfigureRequest, ConfigurationResponse unitConfigurationMainResponseObj, int bank1Count, int bank2Count)
        {
            var methodBeginTime = Utility.LogBegin();
            List<ConfigVariable> dbTypeAssignments = baseConfigureRequest.Line.VariableAssignments.Where(oh => oh.VariableId.Contains(Constant.DBTYP)).Select(
             variableAssignment => new ConfigVariable
             {
                 VariableId = variableAssignment.VariableId,
                 Value = variableAssignment.Value
             }).ToList<ConfigVariable>();

            if (!(dbTypeAssignments.Count().Equals(0)))
            {
                var beamWallBank1 = dbTypeAssignments.Where(x => Utility.beamWallBank1.Contains(x.VariableId.Split(Constant.DOT).FirstOrDefault()));
                var beamWallBank2 = dbTypeAssignments.Where(x => Utility.beamWallBank2.Contains(x.VariableId.Split(Constant.DOT).FirstOrDefault()));
                bool isWallBank1 = false;
                if (beamWallBank1.Count() >= 3)
                {
                    foreach (var groupWallBank1 in beamWallBank1)
                    {
                        if (groupWallBank1.Value.Equals(Constant.WALL))
                        {
                            isWallBank1 = true;
                            break;
                        }
                    }
                }
                bool isWallBank2 = false;
                if (beamWallBank2.Count() >= 3)
                {
                    foreach (var groupWallBank2 in beamWallBank2)
                    {
                        if (groupWallBank2.Value.Equals(Constant.WALL))
                        {
                            isWallBank2 = true;
                            break;
                        }
                    }
                }
                if (beamWallBank1.Count() >= 3 && beamWallBank2.Count() >= 3)
                {
                    if (isWallBank1 && isWallBank2)
                    {
                        unitConfigurationMainResponseObj.AllBeamsSelected = false;
                    }
                    else
                    {
                        unitConfigurationMainResponseObj.AllBeamsSelected = true;
                    }
                }
                else if (!(beamWallBank1.Count() >= 3) && beamWallBank2.Count() >= 3)
                {
                    if (isWallBank2)
                    {
                        unitConfigurationMainResponseObj.AllBeamsSelected = false;
                    }
                    else
                    {
                        unitConfigurationMainResponseObj.AllBeamsSelected = true;
                    }
                }
                else if (beamWallBank1.Count() >= 3 && !(beamWallBank2.Count() >= 3))
                {
                    if (isWallBank1)
                    {
                        unitConfigurationMainResponseObj.AllBeamsSelected = false;
                    }
                    else
                    {
                        unitConfigurationMainResponseObj.AllBeamsSelected = true;
                    }
                }
            }
            else
            {
                if ((bank1Count >= 4 && bank2Count >= 4) || (!(bank1Count >= 4) && bank2Count >= 4) || (bank1Count >= 4 && !(bank2Count >= 4)))
                {
                    unitConfigurationMainResponseObj.AllBeamsSelected = true;
                }
                else
                {
                    unitConfigurationMainResponseObj.AllBeamsSelected = false;
                }
            }
            Utility.LogEnd(methodBeginTime);
        }

        /// <summary>
        /// ChangeFieldDrawingConfigure BL
        /// </summary>
        /// <param Name="configureRequest"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="parentCode"></param>
        /// <param Name="locale"></param>
        /// <param Name="selectedTab"></param>
        /// <param Name="modelNumber"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> StartFieldDrawingConfigure(JObject variableAssignments, int groupId, bool isChange, string sessionId, List<UnitLayOutDetails> lstUnitLayoutDetails, string statusKey, string projectStatusKey, string drawingStausKey, List<string> permissions)
        {
            var methodBeginTime = Utility.LogBegin();

            if (isChange)
            {
                // Checking the Cache and Updating the Variables
                var crossPackagevariableAssignments = new List<VariableAssignment>();
                var getCrossPackageValues = GetCrosspackageVariableAssignments(sessionId, Constant.FIELDDRAWINGAUTOMATION);
                if (!string.IsNullOrEmpty(getCrossPackageValues))
                {
                    crossPackagevariableAssignments = Utility.DeserializeObjectValue<List<VariableAssignment>>(getCrossPackageValues);
                }
                var dbAssignments = Utility.DeserializeObjectValue<Line>(Utility.SerializeObjectValue(variableAssignments));
                //generate cross package variable assignments
                crossPackagevariableAssignments = GenerateVariableAssignmentsForUnitConfiguration(crossPackagevariableAssignments, dbAssignments);
                var variableAssignmentz = new Line
                {
                    VariableAssignments = crossPackagevariableAssignments
                };
                SetCrosspackageVariableAssignments(crossPackagevariableAssignments, sessionId, Constant.FIELDDRAWINGAUTOMATION);
                variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(variableAssignmentz));
            }

            var configureRequest = GetAndSetCacheConfiguration(sessionId, variableAssignments, Constant.GROUPCONFIGURATIONNAME, Constant.FIELDDRAWINGAUTOMATION);

            //Gets the base configuration of the model
            var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest);
            //Adding include sections
            baseConfigureRequest = GenerateIncludeSections(baseConfigureRequest, Constant.FIELDDRAWINGAUTOMATION);

            var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, configureRequest.PackagePath, sessionId
             ).ConfigureAwait(false);
            var baseConfigureResponse = baseConfigureResponseJObj.Response.ToObject<ConfigurationResponse>();
            var unitConfigurationMainResponseObj = new ConfigurationResponse();

            // UI Template
            var beamWallMainResponseTemplate = JObject.Parse(File.ReadAllText(Constant.STARTFIELDDRAWINGAUTOMATIONMAINTEMPLATEPATH));
            unitConfigurationMainResponseObj = beamWallMainResponseTemplate.ToObject<ConfigurationResponse>();
            var appSections = Utility.GetTokens(Constant.TOKENSECTIONS, beamWallMainResponseTemplate, false);

            //Dividers logic
            var beamWallSection = appSections.AsEnumerable().Where(s => Convert.ToString(s[Constant.TOKENID]).Equals(Constant.BEAMWALLSECTIONKEY, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            var dividersections = beamWallSection.ToObject<Sections>();

            if (lstUnitLayoutDetails == null)
            {
                try
                {
                    lstUnitLayoutDetails = SetCacheFieldDrawingAutomationLayoutDetails(lstUnitLayoutDetails, sessionId, groupId);
                }
                catch (Exception)
                {
                    return new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = Constant.NODATACACHE
                    };
                }
            }

            var bank1 = lstUnitLayoutDetails.Where(x => Utility.bank1CarPosition.Contains(x.displayCarPosition)).Count();
            var bank2 = lstUnitLayoutDetails.Where(y => Utility.bank2CarPosition.Contains(y.displayCarPosition)).Count();
            var allDividers = GetBeamWallDividers(bank1, bank2, dividersections);
            List<SectionsValues> sectionsValues = new List<SectionsValues>();
            sectionsValues = Utility.DeserializeObjectValue<List<SectionsValues>>(Utility.SerializeObjectValue(allDividers));

            //Unit Layout Generation
            var unitLayoutSections = appSections.AsEnumerable().Where(s => Convert.ToString(s[Constant.TOKENID]).Equals(Constant.LAYOUTSECTIONKEY, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            var layoutSections = unitLayoutSections.ToObject<Sections>();
            var lstSectionSumpPit = UnitLayoutGeneration(lstUnitLayoutDetails, layoutSections);

            //Warning Message Logic
            CheckWallWarningMessage(baseConfigureRequest, unitConfigurationMainResponseObj, bank1, bank2);

            //Binding the Response in to UI Template
            foreach (var allSection in unitConfigurationMainResponseObj.Sections)
            {
                if (Utility.CheckEquals(allSection.Id, Constant.BEAMWALLSECTIONKEY))
                {
                    allSection.sections = new List<SectionsValues>();
                    allSection.sections = sectionsValues;
                }

                if (Utility.CheckEquals(allSection.Id, Constant.LAYOUTSECTIONKEY))
                {
                    allSection.sections = new List<SectionsValues>();
                    allSection.sections = lstSectionSumpPit;
                }

                if (Utility.CheckEquals(allSection.Id, Constant.LAYOUTGENERATIONSETTINGSSECTIONKEY))
                {
                    var layoutGenerationAssignments = Utility.DeserializeObjectValue<Line>(Utility.SerializeObjectValue(variableAssignments));
                    foreach (var subsection in allSection.sections)
                    {

                        foreach (var variable in subsection.Variables)
                        {
                            foreach (var generationAssignments in layoutGenerationAssignments.VariableAssignments)
                            {
                                if (Utility.CheckEquals(generationAssignments.VariableId, variable.Id))
                                {
                                    foreach (var value in variable.Values)
                                    {
                                        if (Utility.CheckEquals(Convert.ToString(value.value), Convert.ToString(Convert.ToBoolean(Convert.ToInt32(generationAssignments.Value)))))
                                        //if ((Convert.ToBoolean(value.value) && Convert.ToBoolean(generationAssignments.Value)) ||
                                        //    (!Convert.ToBoolean(value.value) && !Convert.ToBoolean(generationAssignments.Value)))
                                        {
                                            value.Assigned = Constant.BYUSER_LOWERCASE;
                                        }
                                    }
                                }
                            }
                        }


                        if (Utility.CheckEquals(subsection.Id, Constant.DRAWINGSECTIONKEY))
                        {
                            foreach (var variable in subsection.Variables)
                            {
                                variable.Properties = new List<Properties>();
                                Properties displayProperty = new Properties() { Id = Constant.DISPLAYNAME, Type = Constant.STRING };
                                Properties property = new Properties() { Id = Constant.INFOMESSAGE, Type = Constant.ARRAYTYPE };
                                if (variable.Id.Equals(Constant.DRAWINGTYPESBASEPACKAGE))
                                {
                                    if (Utility.CheckEquals(projectStatusKey, Constant.PROJECTBIDAWARDED))
                                    {
                                        property.Value = new object[2] { Constant.BASEPACKAGESECTION1, Constant.BASEPACKAGESECTION2 };
                                    }
                                    else
                                    {
                                        property.Value = new object[3] { Constant.LAYOUTDRAWINGSSECTION1, Constant.LAYOUTDRAWINGSSECTION2, new object[2] { Constant.LAYOUTDRAWINGSSECTION3, new object[4] { Constant.LAYOUTDRAWINGSSUBSECTION1, Constant.LAYOUTDRAWINGSSUBSECTION2, Constant.LAYOUTDRAWINGSSUBSECTION3, Constant.LAYOUTDRAWINGSSUBSECTION4 } } };
                                        displayProperty.Value = Constant.LAYOUTDRAWINGS;
                                        variable.Properties.Add(displayProperty);
                                    }
                                }
                                else if (variable.Id.Equals(Constant.DRAWINGTYPESEXTERIORPACKAGE))
                                {
                                    property.Value = new object[2] { Constant.EXTERIORSECTION1, Constant.EXTERIORSECTION2 };
                                }
                                else if (variable.Id.Equals(Constant.DRAWINGTYPESINTERIORPACKAGE))
                                {
                                    property.Value = new object[2] { Constant.INTERIORSECTION1, Constant.INTERIORSECTION2 };
                                }
                                variable.Properties.Add(property);
                            }
                        }
                    }
                }
            }

            var mainFillteredBuildingConfigResponse = Utility.MapVariables(JObject.FromObject(baseConfigureResponse), JObject.FromObject(unitConfigurationMainResponseObj));
            //Required because as Utility.FilterNullValues function works with only with C# types 
            var response = Utility.DeserializeObjectValue<ConfigurationResponse>(Utility.SerializeObjectValue(mainFillteredBuildingConfigResponse));
            //Adding counter weight location parameters into separate array
            UnitLayoutVariablesMapping(response);
            response.GroupStatus = statusKey;
            response.DrawingStatus = drawingStausKey;
            response.Permissions = permissions;
            response.EnrichedData = JObject.Parse(File.ReadAllText(Constant.FDAENRICHEDDATA));

            //Getting the Template Data
            if (configureRequest != null)
            {
                return new ResponseMessage
                {
                    StatusCode = Constant.SUCCESS,
                    Response = Utility.FilterNullValues(response)
                };
            }
            Utility.LogEnd(methodBeginTime);
            throw new CustomException(new ResponseMessage
            {
                StatusCode = Constant.BADREQUEST,
                Message = _localizer[Constant.REQUESTCANNOTBENULL]
            });
        }

        public Sections GetDividerSections(Sections dividersections, int idCount, int VariableCount)
        {
            Sections section = new Sections();
            foreach (var divider in dividersections.sections)
            {
                section.Id = divider.Id.Replace(Convert.ToString(Constant.HASH), Convert.ToString(idCount));
                section.Variables = new List<Variables>();
                foreach (var variable in divider.Variables)
                {
                    Variables variables = new Variables() { Id = variable.Id.Replace(Convert.ToString(Constant.HASH), Convert.ToString(VariableCount)) };
                    section.Variables.Add(variables);
                }
            }
            return section;
        }

        /// <summary>
        /// SetCacheEntranceConsoles
        /// </summary>
        /// <param Name="unitMappingValues"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public List<BuildingEquipmentData> SetCacheBuildingEquipmentConsoles(List<BuildingEquipmentData> objBuildingEquipmentConfigurationData, string sessionId, int buildingId, bool isChange)
        {
            var methodBeginTime = Utility.LogBegin();
            var updatedBuildingEquipmentConsoleData = objBuildingEquipmentConfigurationData;
            var username = GetUserId(sessionId);
            if (buildingId != 0)
            {
                if (objBuildingEquipmentConfigurationData != null && isChange == false)
                {
                    var buildingEquipmentConsolesResponseObj = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(updatedBuildingEquipmentConsoleData));
                    _cpqCacheManager.SetCache(username, _environment, Constant.BUILDINGEQUIPMENTCONSOLE, buildingId.ToString(), Utility.SerializeObjectValue(buildingEquipmentConsolesResponseObj));
                }
                else if (objBuildingEquipmentConfigurationData != null && isChange == true)
                {
                    var buildingEquipmentConsolesResponseObj = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(updatedBuildingEquipmentConsoleData));
                    _cpqCacheManager.SetCache(username, _environment, Constant.BUILDINGEQUIPMENTCONSOLECHANGE, buildingId.ToString(), Utility.SerializeObjectValue(buildingEquipmentConsolesResponseObj));
                }
                else if (objBuildingEquipmentConfigurationData == null && isChange == true)
                {
                    updatedBuildingEquipmentConsoleData = Utility.DeserializeObjectValue<List<BuildingEquipmentData>>(_cpqCacheManager.GetCache(username, _environment, Constant.BUILDINGEQUIPMENTCONSOLECHANGE, buildingId.ToString()));
                }
                else
                {
                    updatedBuildingEquipmentConsoleData = Utility.DeserializeObjectValue<List<BuildingEquipmentData>>(_cpqCacheManager.GetCache(username, _environment, Constant.BUILDINGEQUIPMENTCONSOLE, buildingId.ToString()));
                }
            }
            Utility.LogEnd(methodBeginTime);
            return updatedBuildingEquipmentConsoleData;
        }

        public List<UnitLayOutDetails> SetCacheFieldDrawingAutomationLayoutDetails(List<UnitLayOutDetails> objBuildingEquipmentConfigurationData, string sessionId, int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            var updatedBuildingEquipmentConsoleData = objBuildingEquipmentConfigurationData;
            var username = GetUserId(sessionId);
            if (groupId != 0)
            {
                if (objBuildingEquipmentConfigurationData != null)
                {
                    var buildingEquipmentConsolesResponseObj = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(updatedBuildingEquipmentConsoleData));
                    _cpqCacheManager.SetCache(username, _environment, Constant.FIELDDRAWINGAUTOMATION, groupId.ToString(), Utility.SerializeObjectValue(buildingEquipmentConsolesResponseObj));
                }
                else
                {
                    var getcache = _cpqCacheManager.GetCache(username, _environment, Constant.FIELDDRAWINGAUTOMATION, groupId.ToString());
                    if (getcache != null && getcache != "null")
                    {
                        updatedBuildingEquipmentConsoleData = Utility.DeserializeObjectValue<List<UnitLayOutDetails>>(_cpqCacheManager.GetCache(username, _environment, Constant.FIELDDRAWINGAUTOMATION, groupId.ToString()));
                    }
                    else
                    {
                        var buildingEquipmentConsolesResponseObj = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(updatedBuildingEquipmentConsoleData));
                        _cpqCacheManager.SetCache(username, _environment, Constant.FIELDDRAWINGAUTOMATION, groupId.ToString(), Utility.SerializeObjectValue(buildingEquipmentConsolesResponseObj));
                    }
                }
            }
            Utility.LogEnd(methodBeginTime);
            return updatedBuildingEquipmentConsoleData;
        }


        public List<ConfigVariable> SetCacheRearOpenForUnitSet(List<ConfigVariable> variable, string sessionId, int setId)
        {
            var methodBeginTime = Utility.LogBegin();
            var updatedvariable = variable;
            var username = GetUserId(sessionId);
            if (setId != 0)
            {
                if (variable != null)
                {
                    var listOfUnitsResponseObj = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(updatedvariable));
                    _cpqCacheManager.SetCache(username, _environment, Constant.REAROPEN, setId.ToString(), Utility.SerializeObjectValue(listOfUnitsResponseObj));
                }
                else
                {
                    var updatedValue = _cpqCacheManager.GetCache(username, _environment, Constant.REAROPEN, setId.ToString());
                    if (!string.IsNullOrEmpty(updatedValue))
                    {
                        updatedvariable = Utility.DeserializeObjectValue<List<ConfigVariable>>(updatedValue);
                    }
                }
            }
            Utility.LogEnd(methodBeginTime);
            return updatedvariable;
        }

        /// <summary>
        /// Compare Change InConfig Response Method
        /// </summary>
        /// <param Name="previousConfiguration"></param>
        /// <param Name="currentConfiguration"></param>
        /// <returns></returns>
        public ConflictManagement CompareChangeInConfigResponse(ConfigurationResponse currentConfiguration, string conflictType, Dictionary<string, object> configureRequestDictionary)
        {
            var methodBeginTime = Utility.LogBegin();
            //// removed configurations from clm
            //// foreach of the removed values type 
            var removedChanges = new List<ConflictMgmtList>();
            foreach (var item in currentConfiguration.RemovedAssignments.variableAssignments)
            {
                var valujon = new ConflictMgmtList
                {
                    VariableId = item.Variable.Id,
                    Name = item.Variable.Name,
                    Value = item.Value.Value
                };
                removedChanges.Add(valujon);
            }
            // removed assignments
            //Resolved values
            var addedValuesConfiguration = (from configureArgumentValues in configureRequestDictionary
                                            from removedArgumentVariables in removedChanges
                                            where Utility.CheckEquals(configureArgumentValues.Key.ToString(), removedArgumentVariables.VariableId.ToString())
                                            select new ConflictMgmtList
                                            {
                                                VariableId = configureArgumentValues.Key,
                                                Value = configureArgumentValues.Value
                                            }).ToList();
            // pendingConfiguration 
            var pendingConfiguration = removedChanges.Where(x => !configureRequestDictionary.Keys.Contains(x.VariableId)).ToList();
            var conflictAssignments = new ConflictManagement
            {
                //AddedConfigurataions = addedChanges,
                ResolvedAssignments = addedValuesConfiguration,
                PendingAssignments = pendingConfiguration
            };
            Utility.LogEnd(methodBeginTime);
            return conflictAssignments;
        }

        public List<ConfigVariable> GeneratevariableAssignmentsForCrosspackageDependecy(string currentConfiguration, List<ConfigVariable> configVariables)
        {
            var methodBeginTime = Utility.LogBegin();
            var buildingContantsDictionary = Utility.GetVariableMapping(Constant.BUILDINGMAPPERVARIABLESMAPPERPATH, Constant.BUILDINGCONSTANTMAPPER);
            var crossPackageVariableId = (JObject.Parse(File.ReadAllText(Constant.CROSSPACKAGEVARIABLEMAPPING)));
            var crossPackagevariableDictionary = new Dictionary<string, string>();
            JToken crossPackageVariables;
            switch (currentConfiguration.ToUpper())
            {
                case Constant.GROUPLAYOUTCONFIGURATION:
                    crossPackageVariables = crossPackageVariableId["GroupToBuilding"];
                    crossPackagevariableDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(crossPackageVariables));
                    foreach (var crossVariable in crossPackagevariableDictionary)
                    {
                        foreach (var buildingVariable in configVariables)
                        {
                            if (Utility.CheckEquals(crossVariable.Value, buildingVariable.VariableId))
                            {
                                buildingVariable.VariableId = crossVariable.Key;
                            }
                        }
                    }
                    break;
                case Constant.GROUPVARIABLESFORFLAGS_UPPER:
                    crossPackageVariables = crossPackageVariableId["GroupToProduct"];
                    crossPackagevariableDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(crossPackageVariables));
                    foreach (var crossVariable in crossPackagevariableDictionary)
                    {
                        foreach (var buildingVariable in configVariables)
                        {
                            if (Utility.CheckEquals(crossVariable.Key, buildingVariable.VariableId))
                            {
                                buildingVariable.VariableId = crossVariable.Value;
                            }
                        }
                    }
                    return configVariables;
                default:
                    var carPositionToElevatorMapper = (JObject.Parse(File.ReadAllText(Constant.LOCATIONTOELEVQATORMAPPING)));
                    var elevatorDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(carPositionToElevatorMapper));
                    foreach (var dictionaryItem in elevatorDictionary)
                    {
                        foreach (var groupVariable in configVariables)
                        {
                            if (groupVariable.VariableId.Contains(dictionaryItem.Key))
                            {
                                groupVariable.VariableId = groupVariable.VariableId.Replace(dictionaryItem.Key, dictionaryItem.Value);
                            }
                        }
                    }
                    crossPackageVariables = crossPackageVariableId["BuildingToUnit"];
                    crossPackagevariableDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(crossPackageVariables));
                    crossPackageVariables = crossPackageVariableId["GroupToUnit"];
                    var variable = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(crossPackageVariables));
                    foreach (var item in variable)
                    {
                        crossPackagevariableDictionary.Add(item.Key, item.Value);
                    }
                    break;
            }
            var variables = (from variabble in configVariables
                             where crossPackagevariableDictionary.ContainsKey(variabble.VariableId)
                             select variabble).ToList();
            if (!variables.Any(x => x.VariableId.Equals(buildingContantsDictionary[Constants.BUILDINGYEAR])))
            {
                var ConfigVariable = new ConfigVariable()
                {
                    VariableId = buildingContantsDictionary[Constants.BUILDINGYEAR],
                    Value = Constants.YEAR
                };
                variables.Add(ConfigVariable);
            }

            if (!Utility.CheckEquals(Constant.GROUPLAYOUTCONFIGURATION, currentConfiguration.ToUpper()))
            {
                variables.ForEach(variabble => variabble.VariableId = crossPackagevariableDictionary[variabble.VariableId]);
            }
            variables = variables.Distinct().ToList();
            Utility.LogEnd(methodBeginTime);
            return variables;
        }

        /// <summary>
        /// get cached variable assignments
        /// </summary>
        /// <param Name="crosspackagevariableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public string GetCrosspackageVariableAssignments(string sessionId, string configurationType)
        {
            var methodBeginTime = Utility.LogBegin();
            Utility.LogEnd(methodBeginTime);
            return _cpqCacheManager.GetCache(sessionId, _environment, configurationType);
        }

        public int SetCrosspackageVariableAssignments(List<VariableAssignment> crosspackagevariableAssignments, string sessionId, string configurationType)
        {
            var methodBeginTime = Utility.LogBegin();
            _cpqCacheManager.SetCache(sessionId, _environment, configurationType, Utility.SerializeObjectValue(crosspackagevariableAssignments));
            Utility.LogEnd(methodBeginTime);
            return 0;
        }

        public List<VariableAssignment> GenerateVariableAssignmentsForUnitConfiguration(List<VariableAssignment> crossPackagevariableAssignments, Line assignments)
        {
            var methodBeginTime = Utility.LogBegin();

            crossPackagevariableAssignments.RemoveAll((y => assignments.VariableAssignments.Any(z => z.VariableId.Equals(y.VariableId))));
            crossPackagevariableAssignments.AddRange(assignments.VariableAssignments);
            Utility.LogEnd(methodBeginTime);
            return crossPackagevariableAssignments;
        }

        /// <summary>
        /// SetOrGetCacheForEditConflictFlow
        /// </summary>vf drw mmxzbbxbxbbxbx xhhj 
        /// <param Name="sessionId"></param>
        /// <param Name="configurationType"></param>
        /// <returns></returns>
        public string SetOrGetCacheForEditConflictFlow(string sessionId, bool isEditFlow)
        {
            var methodBeginTime = Utility.LogBegin();
            if (isEditFlow)
            {
                _cpqCacheManager.SetCache(sessionId, _environment, Constant.EDITCONFLITFLOWCACHEKEY, isEditFlow.ToString());
                return (Constant.EDITCONFLITFLOWCACHEKEY);
            }
            Utility.LogEnd(methodBeginTime);
            return _cpqCacheManager.GetCache(sessionId, _environment, Constant.EDITCONFLITFLOWCACHEKEY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="configurationType"></param>
        /// <returns></returns>
        public bool GetCacheValuesForConflictManagement(string sessionId, string conflictType)
        {
            var methodBeginTime = Utility.LogBegin();
            var hasConflictFlagValue = false;
            var getCacheResponse = string.Empty;
            if (!string.IsNullOrEmpty(conflictType))
            {
                switch (conflictType)
                {
                    case Constant.BUILDING:
                        getCacheResponse = _cpqCacheManager.GetCache(sessionId, _environment, Constant.PREVIOUSCONFLICTSVALUES);
                        break;
                    case Constant.GROUP:
                        getCacheResponse = _cpqCacheManager.GetCache(sessionId, _environment, Constant.PREVIOUSGROUPCONFLICTSVALUES);
                        break;
                    case Constant.UNIT:
                        getCacheResponse = _cpqCacheManager.GetCache(sessionId, _environment, Constant.PREVIOUSUNITCONFLICTSVALUES);
                        break;
                    default:
                        getCacheResponse = _cpqCacheManager.GetCache(sessionId, _environment, Constant.PREVIOUSCONFLICTSVALUES);
                        break;
                }
                if (!string.IsNullOrEmpty(getCacheResponse))
                {
                    var filterConflictResponse = Utility.DeserializeObjectValue<ConflictManagement>(getCacheResponse);
                    if (filterConflictResponse != null && filterConflictResponse.PendingAssignments.Any())
                    {
                        hasConflictFlagValue = true;
                    }
                }
            }
            Utility.LogEnd(methodBeginTime);
            return hasConflictFlagValue;
        }

        public List<VariableAssignment> SetCacheFixtureStrategy(List<VariableAssignment> variables, string sessionId, int buildingId)
        {
            var methodBeginTime = Utility.LogBegin();
            var username = GetUserId(sessionId);
            if (variables != null)
            {
                var fixtureStrategy = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(variables));
                _cpqCacheManager.SetCache(username, _environment, Constant.FIXTURESTRATEGY, buildingId.ToString(), Utility.SerializeObjectValue(fixtureStrategy));
                Utility.LogEnd(methodBeginTime);
                return variables;
            }
            else
            {
                var strategy = Utility.DeserializeObjectValue<List<VariableAssignment>>(_cpqCacheManager.GetCache(username, _environment, Constant.FIXTURESTRATEGY, buildingId.ToString()));
                Utility.LogEnd(methodBeginTime);
                return strategy;
            }
        }

        public List<VariableAssignment> GetCacheVariablesForConflictChanges(List<VariableAssignment> getVariables, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            if (getVariables != null && getVariables.Any())
            {
                var fixtureStrategy = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(getVariables));
                _cpqCacheManager.SetCache(sessionId, _environment, Constant.GETVARIABLESCACHE, Utility.SerializeObjectValue(fixtureStrategy));
                Utility.LogEnd(methodBeginTime);
                return getVariables;
            }
            else
            {
                var varAssignments = _cpqCacheManager.GetCache(sessionId, _environment, Constant.GETVARIABLESCACHE);
                if (varAssignments != null)
                {
                    getVariables = new List<VariableAssignment>();
                    getVariables = Utility.DeserializeObjectValue<List<VariableAssignment>>(varAssignments);
                }
                Utility.LogEnd(methodBeginTime);
                return getVariables;
            }
        }

        /// <summary>
        /// check the save request body to give conflict flag
        /// </summary>
        /// <param Name="buildingData"></param>
        /// <param Name="getVariables"></param>
        /// <returns></returns>
        public List<VariableAssignment> CheckConflict(List<VariableAssignment> previousValues, List<VariableAssignment> currentValues)
        {
            var methodBeginTime = Utility.LogBegin();
            List<VariableAssignment> filterValues = new List<VariableAssignment>();
            if (currentValues != null && currentValues.Any())
            {
                filterValues = (from val in previousValues
                                from val2 in currentValues
                                where (Utility.CheckEquals(val.VariableId, val2.VariableId) && !Utility.CheckEquals(Convert.ToString(val.Value), Convert.ToString(val2.Value)))
                                select val).ToList();
                var addedVariables = previousValues.Where(m => !currentValues.Any(y => y.VariableId == m.VariableId)).ToList();
                filterValues.AddRange(addedVariables);
            }
            Utility.LogEnd(methodBeginTime);
            return filterValues;
        }

        /// <summary>
        /// GetConflictAssignments
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public ConflictManagement GetConflictAssignments(string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var getConflictAssignmentsValues = new ConflictManagement();
            // to get the conflicts cache if any
            var getCacheResponse = _cpqCacheManager.GetCache(sessionId, _environment, Constant.PREVIOUSGROUPCONFLICTSVALUES);
            if (!string.IsNullOrEmpty(getCacheResponse) && !Utility.CheckEquals(getCacheResponse, "null"))
            {
                var filterConflictResponse = Utility.DeserializeObjectValue<ConflictManagement>(getCacheResponse);
                if (filterConflictResponse.PendingAssignments != null && filterConflictResponse.PendingAssignments.Any())
                {
                    var removeNullConflicts = (from a in filterConflictResponse.PendingAssignments
                                               where !string.IsNullOrEmpty(a.Value.ToString())
                                               select a).ToList();
                    filterConflictResponse.PendingAssignments = removeNullConflicts;
                }
                getConflictAssignmentsValues = filterConflictResponse;
            }
            Utility.LogEnd(methodBeginTime);
            return getConflictAssignmentsValues;
        }

        public List<EntranceLocations> SetCacheCarCallCutout(int setId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var userId = GetUserId(sessionId);
            var entranceLocations = _cpqCacheManager.GetCache(setId.ToString(), userId, _environment, Constant.CARCALLCUTOUTKEYSWITCHESCONSOLE);
            if (entranceLocations != null)
            {
                var lstentranceLocation = Utility.DeserializeObjectValue<List<EntranceLocations>>(entranceLocations);
                Utility.LogEnd(methodBeginTime);
                return lstentranceLocation;
            }
            else
            {
                var carcutout = _unitConfigurationDL.GetCarCallCutoutOpenings(setId);
                var cacheobj = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(carcutout.FixtureAssignments));
                _cpqCacheManager.SetCache(setId.ToString(), userId, _environment, Constant.CARCALLCUTOUTKEYSWITCHESCONSOLE, Utility.SerializeObjectValue(cacheobj));
                Utility.LogEnd(methodBeginTime);
                return carcutout.FixtureAssignments;
            }
        }

        public OpeningLocations SetCacheOpeningLocation(OpeningLocations openingLocationData, string sessionId, int groupConfigurationId)
        {
            var methodBeginTime = Utility.LogBegin();
            var updatedEntranceConsoleData = openingLocationData;
            var username = GetUserId(sessionId);
            if (groupConfigurationId != 0)
            {
                if (openingLocationData != null)
                {
                    var entranceConsolesResponseObj = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(updatedEntranceConsoleData));
                    _cpqCacheManager.SetCache(username, _environment, Constant.OPENINGLOCATION, groupConfigurationId.ToString(), Utility.SerializeObjectValue(entranceConsolesResponseObj));
                }
                else
                {
                    updatedEntranceConsoleData = Utility.DeserializeObjectValue<OpeningLocations>(_cpqCacheManager.GetCache(username, _environment, Constant.OPENINGLOCATION, groupConfigurationId.ToString()));
                }
            }
            Utility.LogEnd(methodBeginTime);
            return updatedEntranceConsoleData;
        }

        public async Task<JObject> GetByDefaultOrRulevaluesFromPackage(ConfigurationRequest configurationRequest, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            string packagePath = configurationRequest?.PackagePath;
            if (configurationRequest == null)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = _localizer[Constant.REQUESTCANNOTBENULL + Constant.HYPHEN]
                });
            }
            configurationRequest.Settings.Debug = true;
            var includeSections = new List<string>();
            includeSections.Add(string.Empty);
            configurationRequest.Settings.IncludeSections = includeSections;
            var configureResponseJObj =
             await ConfigurationBl(configurationRequest, packagePath, sessionId).ConfigureAwait(false);
            //configuration object values
            var configurationResponsea = Utility.DeserializeObjectValue<StartConfigureResponse>(Utility.SerializeObjectValue(configureResponseJObj.Response));
            Utility.LogEnd(methodBeginTime);
            return Utility.FilterNullValues(configurationResponsea.Arguments);
        }

        public async Task<ResponseMessage> SystemValidationUnitCall(List<VariableAssignment> lstVariableAssignment, string sessionId, List<UnitVariablesDetailsForTP2> unitVariablesDetailsForTP2)
        {
            // Unit configuration call to get the required arguments
            var unitRequest = new ConfigureRequest
            {
                Line = new Line()
            };
            unitRequest.Line.VariableAssignments = lstVariableAssignment;
            var unitRequestObj = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(unitRequest.Line));
            var unitConfigureRequest = CreateConfigurationRequestWithTemplate(unitRequestObj, Constant.UNITDEFAULTSCLMCALL, null, unitVariablesDetailsForTP2.FirstOrDefault().ProductName);
            var unitPackagePath = unitConfigureRequest?.PackagePath;
            //Gets the base configuration of the model
            unitConfigureRequest = _configureService.GetBaseConfigureRequest(unitConfigureRequest);
            //Adding include sections
            var unitConfigureResponseJObj = await ConfigurationBl(unitConfigureRequest, unitPackagePath, sessionId).ConfigureAwait(false);
            // configuration object values for conflict mapping
            var configureResponse = unitConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = configureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            // adding defaults to cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTUNITCONFIGVALUES, Utility.SerializeObjectValue(configureResponseArgumentJObject));
            return unitConfigureResponseJObj;
        }

        /// <summary>
        /// SystemValidationCabCall
        /// </summary>
        /// <param name="lstVariableAssignment"></param>
        /// <param name="sessionId"></param>
        /// <param name="unitConfigureResponseJObj"></param>
        /// <param name="unitVariablesDetailsForTP2"></param>
        /// <returns></returns>
        public async Task<Tuple<ResponseMessage, List<VariableAssignment>>> SystemValidationCabCall(List<VariableAssignment> lstVariableAssignment, string sessionId, ResponseMessage unitConfigureResponseJObj, List<UnitVariablesDetailsForTP2> unitVariablesDetailsForTP2)
        {
            var cabSlingVariabels = new List<VariableAssignment>();

            var unitConfigurationResponse = unitConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var unitConfigureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(unitConfigurationResponse.Arguments));
            var unitConfigureRequestDictionary = unitConfigureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();
            var sysValContantsDictionary = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONPARAMETERS, Constant.EVOLUTION200)));
            if (unitVariablesDetailsForTP2 != null && unitVariablesDetailsForTP2.FirstOrDefault().ProductName.Equals(Constants.EVO_200))
            {
                sysValContantsDictionary = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONPARAMETERS, Constant.EVOLUTION200)));
            }
            else if (unitVariablesDetailsForTP2 != null && unitVariablesDetailsForTP2.FirstOrDefault().ProductName.Equals(Constants.EVO_100))
            {
                sysValContantsDictionary = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONPARAMETERS, Constant.EVOLUTION__100)));
            }
            else if (unitVariablesDetailsForTP2 != null && unitVariablesDetailsForTP2.FirstOrDefault().ProductName.Equals(Constants.ENDURA_100))
            {
                sysValContantsDictionary = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONPARAMETERS, Constant.END100)));
            }

            var lstSysVariables = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(sysValContantsDictionary[Constant.CABCALL]));
            var unitCabSlingValues = (from stubValues in lstSysVariables
                                      from unitResponse in unitConfigureRequestDictionary
                                      where Utility.CheckEquals(unitResponse.Key.Substring(unitResponse.Key.IndexOf('.') + 1).ToString(), stubValues)
                                      select new VariableAssignment()
                                      {
                                          VariableId = stubValues,
                                          Value = unitResponse.Value
                                      }).Distinct().ToList();

            if (unitCabSlingValues != null && unitCabSlingValues.Any())
            {
                var cabRequiredVariablesObj = (JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONCABREQUESTBODYVARIABLES, Constant.EVOLUTION200))));
                if (unitVariablesDetailsForTP2 != null && unitVariablesDetailsForTP2.FirstOrDefault().ProductName.Equals(Constants.EVO_200))
                {
                    cabRequiredVariablesObj = (JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONCABREQUESTBODYVARIABLES, Constant.EVOLUTION200))));
                }
                else if (unitVariablesDetailsForTP2 != null && unitVariablesDetailsForTP2.FirstOrDefault().ProductName.Equals(Constants.EVO_100))
                {
                    cabRequiredVariablesObj = (JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONCABREQUESTBODYVARIABLES, Constant.EVOLUTION__100))));
                }
                else if (unitVariablesDetailsForTP2 != null && unitVariablesDetailsForTP2.FirstOrDefault().ProductName.Equals(Constants.ENDURA_100))
                {
                    cabRequiredVariablesObj = (JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONCABREQUESTBODYVARIABLES, Constant.END100))));
                }
                var cabRequiredVariables = Utility.DeserializeObjectValue<List<VariableAssignment>>(Utility.SerializeObjectValue(cabRequiredVariablesObj[Constant.SYSVALVARIABLEASSIGNMENTS]));
                var cabSlingRequiredValues = cabRequiredVariables.Where(a => !unitCabSlingValues.Any(x => x.VariableId == a.VariableId)).ToList();
                if (cabSlingRequiredValues != null && cabSlingRequiredValues.Any())
                {
                    cabSlingVariabels.AddRange(unitCabSlingValues);
                    cabSlingVariabels.AddRange(cabSlingRequiredValues);
                }
            }
            if (cabSlingVariabels.Any())
            {
                cabSlingVariabels.Distinct();
                foreach (var savedUnitVariables in lstVariableAssignment)
                {
                    foreach (var cabValues in cabSlingVariabels)
                    {
                        if (Utility.CheckEquals(savedUnitVariables.VariableId.Substring(savedUnitVariables.VariableId.IndexOf(Constant.DOTCHAR) + 1), cabValues.VariableId))
                        {
                            cabValues.Value = savedUnitVariables.Value;
                        }
                    }
                }
            }
            var cr = new ConfigureRequest
            {
                Line = new Line()
            };
            cr.Line.VariableAssignments = cabSlingVariabels;
            var lineVariableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(cr.Line));
            var cabConfigureRequest = CreateConfigurationRequestWithTemplate(lineVariableAssignments, Constant.SYSTEMVALIDATIONCABCALL, null, unitVariablesDetailsForTP2.FirstOrDefault().ProductName);
            var packagePath = cabConfigureRequest?.PackagePath;
            //Gets the base configuration of the model
            cabConfigureRequest = _configureService.GetBaseConfigureRequest(cabConfigureRequest);
            //Adding include sections
            if(unitVariablesDetailsForTP2.FirstOrDefault().ProductName.Equals(Constants.EVO_100) || 
                unitVariablesDetailsForTP2.FirstOrDefault().ProductName.Equals(Constants.ENDURA_100))
            {
                cabConfigureRequest.Settings.IncludeSections = new List<String>();
            }

            var cabConfigureResponseJObj = await ConfigurationBl(cabConfigureRequest, packagePath, sessionId
                ).ConfigureAwait(false);

            return Tuple.Create(cabConfigureResponseJObj, cabSlingVariabels);
        }

        /// <summary>
        /// SystemValidationSlingCall
        /// </summary>
        /// <param name="lineVariableAssignments"></param>
        /// <param name="sessionId"></param>
        /// <param name="cabConfigureResponseJObj"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SystemValidationSlingCall(JObject lineVariableAssignments, string sessionId, ResponseMessage cabConfigureResponseJObj, string productName)
        {
            var sysValContantsDictionary = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.SYSTEMVALIDATIONCONSTANTMAPPER);
            //var cabConfigureRequest = CreateConfigurationRequestWithTemplate(lineVariableAssignments, Constant.SYSTEMVALIDATIONSLINGCALL);
            // Main Stub 
            // setting stub data into an sectionsValues object
            /////////////////////////////////////////////
            /// sling Call
            /// 
            var slingConfigureRequest = CreateConfigurationRequestWithTemplate(lineVariableAssignments, Constant.SYSTEMVALIDATIONSLINGCALL, null, productName);
            //slingConfigureRequest = _configureService.GetBaseConfigureRequest(slingConfigureRequest);
            //Adding include sections
            //Cretae a HTTP call, using configurator service. calculat empty car weight
            slingConfigureRequest.Settings.IncludeSections = new List<String>();
            var slingConfigureResponseJObj = await ConfigurationBl(slingConfigureRequest, slingConfigureRequest?.PackagePath, sessionId).ConfigureAwait(false);
            //var slingConfigureResponseJObj = await _configuratorService.RequestEmptyCarWeightCalculation(slingConfigureRequest, sysValContantsDictionary[Constant.SLINGWEIGHTAPI]).ConfigureAwait(false);
            return slingConfigureResponseJObj;
        }

        /// <summary>
        /// SystemValidationEmptyCall
        /// </summary>
        /// <param name="lstVariableAssignment"></param>
        /// <param name="cabConfigureResponse"></param>
        /// <param name="unitConfigureRequestDictionary"></param>
        /// <returns></returns>
        public async Task<Tuple<ResponseMessage, List<VariableAssignment>>> SystemValidationEmptyCall(List<VariableAssignment> lstVariableAssignment, ConfigurationResponse cabConfigureResponse, Dictionary<string, object> unitConfigureRequestDictionary, string productName, string sessionId)
        {
            var emptyVariables = new List<VariableAssignment>();
            var sysValContantsDictionary = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.SYSTEMVALIDATIONCONSTANTMAPPER);
            // to get all the variable Id's
            // need to compare the stub to get Id's from first call and common variables 
            // add the remaining variables to the main request 
            var sysValContantsDictionaryParameters = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONPARAMETERS, Constant.EVOLUTION200)));
            if (!String.IsNullOrEmpty(productName) && productName.Equals(Constants.EVO_200))
            {
                sysValContantsDictionaryParameters = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONPARAMETERS, Constant.EVOLUTION200)));
            }
            else if (!String.IsNullOrEmpty(productName) && productName.Equals(Constants.EVO_100))
            {
                sysValContantsDictionaryParameters = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONPARAMETERS, Constant.EVOLUTION__100)));
            }
            var lstSysVariables2 = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(sysValContantsDictionaryParameters[Constant.EMPTYCALL]));
            //variables from vtpkg first call
            var varAssignment = (from section in cabConfigureResponse.Sections
                                 from Sections in section.sections
                                 from variable in Sections.Variables
                                 where lstSysVariables2.Contains(variable.Id)
                                 select variable).ToList();


            var lineVariableAssignment2 = (from val1 in varAssignment
                                           from val2 in val1.Values
                                           where (val2.Assigned != null) && (Utility.CheckEquals(val2.Assigned, Constant.BYUSER) || Utility.CheckEquals(val2.Assigned, Constant.BYDEFAULTVALUE) || Utility.CheckEquals(val2.Assigned, Constant.BYRULE_CAMELCASE))

                                           select new VariableAssignment()
                                           {
                                               VariableId = val1.Id,
                                               Value = val2.value
                                           }).ToList();
            //filtering variable ids
            // empty call variables mapping
            var emptyCabSligValues = (from stubValues in lstSysVariables2
                                      from unitResponse in unitConfigureRequestDictionary
                                      where Utility.CheckEquals(unitResponse.Key.Substring(unitResponse.Key.IndexOf(Constant.DOTCHAR) + 1).ToString(), stubValues)
                                      select new VariableAssignment()
                                      {
                                          VariableId = stubValues,
                                          Value = unitResponse.Value
                                      }).Distinct().ToList();

            if (emptyCabSligValues != null && emptyCabSligValues.Any())
            {
                var emptyRequiredVariablesObj = (JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONEMPTYREQUESTBODYVARIABLES, Constant.EVOLUTION200))));
                if (!String.IsNullOrEmpty(productName) && productName.Equals(Constants.EVO_200))
                {
                    emptyRequiredVariablesObj = (JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONEMPTYREQUESTBODYVARIABLES, Constant.EVOLUTION200))));
                }
                else if (!String.IsNullOrEmpty(productName) && productName.Equals(Constants.EVO_100))
                {
                    emptyRequiredVariablesObj = (JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONEMPTYREQUESTBODYVARIABLES, Constant.EVOLUTION__100))));
                }
                var emptyRequiredVariables = Utility.DeserializeObjectValue<List<VariableAssignment>>(Utility.SerializeObjectValue(emptyRequiredVariablesObj[Constant.SYSVALVARIABLEASSIGNMENTS]));
                var emptyRequiredValues = emptyRequiredVariables.Where(a => !emptyCabSligValues.Any(x => x.VariableId == a.VariableId)).ToList();
                if (emptyRequiredValues != null && emptyRequiredValues.Any())
                {
                    emptyVariables.AddRange(emptyCabSligValues);
                    emptyVariables.AddRange(emptyRequiredValues);
                }
            }
            if (emptyVariables.Any())
            {
                emptyVariables.Distinct();
                foreach (var savedUnitVariables in lstVariableAssignment)
                {
                    foreach (var emptyValues in emptyVariables)
                    {
                        if (Utility.CheckEquals(savedUnitVariables.VariableId.Substring(savedUnitVariables.VariableId.IndexOf(Constant.DOTCHAR) + 1), emptyValues.VariableId))
                        {
                            emptyValues.Value = savedUnitVariables.Value;
                        }
                    }
                }
            }
            if (lineVariableAssignment2 != null && lineVariableAssignment2.Any())
            {
                foreach (var mainResponseUnitVariables in lineVariableAssignment2)
                {
                    foreach (var emptyValues in emptyVariables)
                    {
                        if (Utility.CheckEquals(mainResponseUnitVariables.VariableId, emptyValues.VariableId))
                        {
                            emptyValues.Value = mainResponseUnitVariables.Value;
                        }
                    }
                }
            }
            var cr = new ConfigureRequest
            {
                Line = new Line()
            };
            cr.Line.VariableAssignments = emptyVariables;
            var lineVariableAssignments2 = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(cr.Line));
            var emptyConfigureRequest = CreateConfigurationRequestWithTemplate(lineVariableAssignments2, Constant.SYSTEMVALIDATIONEMPTYCALL, null, productName);
            ResponseMessage emptyConfigureResponseJObj = new ResponseMessage();
            if (!String.IsNullOrEmpty(productName) && productName.Equals(Constants.EVO_200))
            {
                emptyConfigureRequest = _configureService.GetBaseConfigureRequest(emptyConfigureRequest);
                //Gets the base configuration of the model
                emptyConfigureResponseJObj = await _configuratorService.RequestEmptyCarWeightCalculation(emptyConfigureRequest, sysValContantsDictionary[Constant.EMPTYCARWEIGHTAPI]).ConfigureAwait(false);
            }
            if(!String.IsNullOrEmpty(productName) && productName.Equals(Constants.EVO_100))
            {
                
                var lstSysVariablesForSlingCall = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(sysValContantsDictionaryParameters["SlingCall"]));
                var slingCallRequiredVariables = (from stubValues in lstSysVariablesForSlingCall
                                                  from unitResponse in unitConfigureRequestDictionary
                                                  where Utility.CheckEquals(unitResponse.Key.Substring(unitResponse.Key.IndexOf(Constant.DOTCHAR) + 1).ToString(), stubValues)
                                                  select new VariableAssignment()
                                                  {
                                                      VariableId = stubValues,
                                                      Value = unitResponse.Value
                                                  }).Distinct().ToList();
                var slingRequiredVariablesObj = (JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONSLINGREQUESTBODYVARIABLES))));
                var slingRequiredVariables = Utility.DeserializeObjectValue<List<VariableAssignment>>(Utility.SerializeObjectValue(slingRequiredVariablesObj[Constant.SYSVALVARIABLEASSIGNMENTS]));
                var slingRequiredVariablesToBeSent = slingRequiredVariables.Where(a => !slingCallRequiredVariables.Any(x => x.VariableId == a.VariableId)).ToList();
                List<VariableAssignment> variablesForSlingCall = new List<VariableAssignment>();
                if (slingRequiredVariablesToBeSent != null && slingCallRequiredVariables.Any())
                {
                    variablesForSlingCall.AddRange(slingRequiredVariablesToBeSent);
                    variablesForSlingCall.AddRange(slingCallRequiredVariables);
                }
                var cr2 = new ConfigureRequest
                {
                    Line = new Line()
                };
                cr2.Line.VariableAssignments = variablesForSlingCall;
                var lineVariableAssignmentsForSlingCall = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(cr2.Line));
                
                emptyConfigureResponseJObj = await SystemValidationSlingCall(lineVariableAssignmentsForSlingCall, sessionId, emptyConfigureResponseJObj, productName);
                var slingConfigureResponse = JObject.FromObject((emptyConfigureResponseJObj.Response.ToObject<StartConfigureResponse>()).Arguments)["Configuration"].ToObject<IDictionary<string, object>>();
                foreach(var varAssign in emptyVariables)
                {
                    var x = slingConfigureResponse.Where(x => x.Key.Equals(varAssign.VariableId))?.FirstOrDefault().Value;
                    if(x != null)
                    {
                        varAssign.Value = x;
                    }
                }
                var crForEmptyCall = new ConfigureRequest
                {
                    Line = new Line()
                };
                crForEmptyCall.Line.VariableAssignments = emptyVariables;
                var lineVariableAssignmentsForEmptyCall = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(crForEmptyCall.Line));
                var configureRequestForEmptyCall = CreateConfigurationRequestWithTemplate(lineVariableAssignmentsForEmptyCall, Constant.SYSTEMVALIDATIONEMPTYCALL, null, productName);
                var packagePath = emptyConfigureRequest?.PackagePath;
                configureRequestForEmptyCall.Settings.IncludeSections = new List<String>();
                emptyConfigureResponseJObj = await ConfigurationBl(configureRequestForEmptyCall, packagePath, sessionId).ConfigureAwait(false);
            }
           
            return Tuple.Create(emptyConfigureResponseJObj, emptyVariables);
        }

        public async Task<Tuple<ResponseMessage, List<VariableAssignment>>> SystemValidationDutyCall(string packagePath, string sessionId, /*ConfigurationRequest slingConfigureRequest, */ConfigurationRequest emptyConfigureRequest, List<VariableAssignment> lstVariableAssignment, ResponseMessage emptyConfigureResponseJObj, Dictionary<string, object> unitConfigureRequestDictionary, string productName)
        {
            var dutyVariabels = new List<VariableAssignment>();

            IDictionary<string, string> emptyConfigureResponse = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(productName) && productName.Equals(Constant.EVO_200))
            {
                emptyConfigureResponse = emptyConfigureResponseJObj.Response.ToObject<IDictionary<string, string>>();
            }
            else if (!String.IsNullOrEmpty(productName) && (productName.Equals(Constant.EVO_100) || productName.Equals(Constant.ENDURA_100)))
            {
                emptyConfigureResponse = JObject.FromObject((emptyConfigureResponseJObj.Response.ToObject<StartConfigureResponse>()).Arguments)["Configuration"].ToObject<IDictionary<string, string>>();
            }
            var filterEmptyConfigValues = Utility.DeserializeObjectValue</*ConfigurationResponse*/IDictionary<string, string>>(Utility.SerializeObjectValue(emptyConfigureResponse));
            // get reconfig Values 
            //emptyConfigureResponse = filterEmptyConfigValues;
            //third call
            var sysValContantsDictionary = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONPARAMETERS, Constant.EVOLUTION200)));
            if (!String.IsNullOrEmpty(productName) && productName.Equals(Constants.EVO_200))
            {
                sysValContantsDictionary = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONPARAMETERS, Constant.EVOLUTION200)));
            }
            else if (!String.IsNullOrEmpty(productName) && productName.Equals(Constants.EVO_100))
            {
                sysValContantsDictionary = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONPARAMETERS, Constant.EVOLUTION__100)));
            }
            else if (!String.IsNullOrEmpty(productName) && productName.Equals(Constants.ENDURA_100))
            {
                sysValContantsDictionary = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONPARAMETERS, Constant.END100)));
            }
            var lstSysVariables3 = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(sysValContantsDictionary[Constant.DUTYCALL]));

            var emptyVarAssignment = (from variable in filterEmptyConfigValues
                                      where lstSysVariables3.Any(x => x.Contains(variable.Key.ToUpper()))
                                      select variable).ToList();

            //taking only variables and values
            var lineVariableAssignment3 = (from val1 in emptyVarAssignment
                                           where (val1.Value != null)
                                           select new VariableAssignment()
                                           {
                                               VariableId = val1.Key,
                                               Value = val1.Value
                                           }).ToList();
            //filtering variable ids
            // empty call variables mapping
            var dutyCallValues = (from stubValues in lstSysVariables3
                                  from unitResponse in unitConfigureRequestDictionary
                                  where Utility.CheckEquals(unitResponse.Key.Substring(unitResponse.Key.IndexOf(Constant.DOTCHAR) + 1).ToString(), stubValues)
                                  select new VariableAssignment()
                                  {
                                      VariableId = stubValues,
                                      Value = unitResponse.Value
                                  }).Distinct().ToList();
            if (!dutyCallValues.Any(x => x.VariableId.Contains(Constants.ECARWT)))
            {
                var dutyECARWTValue = (from stubValues in lstSysVariables3
                                       from unitResponse in emptyConfigureResponse
                                       where stubValues.Contains(Constants.ECARWT) && unitResponse.Key.Contains(Constants.ECARWT)
                                       select new VariableAssignment()
                                       {
                                           VariableId = stubValues,
                                           Value = Convert.ToString(Math.Round(Convert.ToDecimal(unitResponse.Value), 2))
                                       }).Distinct().ToList();
                dutyCallValues.AddRange(dutyECARWTValue);
            }
            if (dutyCallValues != null && dutyCallValues.Any())
            {
                var dutyRequiredVariablesObj = (JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONDUTYREQUESTBODYVARIABLES, Constant.EVOLUTION200))));
                if (!String.IsNullOrEmpty(productName) && productName.Equals(Constants.EVO_200))
                {
                    dutyRequiredVariablesObj = (JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONDUTYREQUESTBODYVARIABLES, Constant.EVOLUTION200))));
                }
                else if (!String.IsNullOrEmpty(productName) && productName.Equals(Constants.EVO_100))
                {
                    dutyRequiredVariablesObj = (JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONDUTYREQUESTBODYVARIABLES, Constant.EVOLUTION__100))));
                }
                else if (!String.IsNullOrEmpty(productName) && productName.Equals(Constants.ENDURA_100))
                {
                    dutyRequiredVariablesObj = (JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONDUTYREQUESTBODYVARIABLES, Constant.END100))));
                }
                var dutyRequiredVariables = Utility.DeserializeObjectValue<List<VariableAssignment>>(Utility.SerializeObjectValue(dutyRequiredVariablesObj["variableAssignments"]));
                var dutyRequiredValues = dutyRequiredVariables.Where(a => !dutyCallValues.Any(x => x.VariableId == a.VariableId)).ToList();

                if (dutyRequiredValues != null && dutyRequiredValues.Any())
                {
                    dutyVariabels.AddRange(dutyCallValues);
                    dutyVariabels.AddRange(dutyRequiredValues);
                }
            }
            if (dutyVariabels.Any())
            {
                dutyVariabels.Distinct();
                foreach (var savedUnitVariables in lstVariableAssignment)
                {
                    foreach (var dutyValues in dutyVariabels)
                    {
                        if (Utility.CheckEquals(savedUnitVariables.VariableId.Substring(savedUnitVariables.VariableId.IndexOf(Constant.DOTCHAR) + 1), dutyValues.VariableId))
                        {
                            dutyValues.Value = savedUnitVariables.Value;
                        }
                    }
                }
            }
            if (lineVariableAssignment3 != null && lineVariableAssignment3.Any())
            {
                foreach (var mainResponseDutyVariables in lineVariableAssignment3)
                {
                    foreach (var emptyValues in dutyVariabels)
                    {
                        if (Utility.CheckEquals(mainResponseDutyVariables.VariableId, emptyValues.VariableId))
                        {
                            emptyValues.Value = mainResponseDutyVariables.Value;
                        }
                    }
                }
            }
            var cr = new ConfigureRequest
            {
                Line = new Line()
            };
            cr.Line.VariableAssignments = dutyVariabels;
            var lineVariableAssignments3 = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(cr.Line));
            var dutyConfigureRequest = CreateConfigurationRequestWithTemplate(lineVariableAssignments3, Constant.SYSTEMVALIDATIONDUTYCALL, null, productName);
            //Gets the base configuration of the model
            dutyConfigureRequest = _configureService.GetBaseConfigureRequest(dutyConfigureRequest);
            packagePath = dutyConfigureRequest?.PackagePath;
            if(!String.IsNullOrEmpty(productName) && (productName.Equals(Constants.EVO_100) || productName.Equals(Constants.ENDURA_100)))
            {
                dutyConfigureRequest.Settings.IncludeSections = new List<String>();
            }
            var dutyConfigureResponseJObj = await ConfigurationBl(dutyConfigureRequest, packagePath, sessionId
                ).ConfigureAwait(false);
            if(!String.IsNullOrEmpty(productName) && productName.Equals(Constants.ENDURA_100))
            {
                lstSysVariables3 = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(sysValContantsDictionary["JackDutyCall"]));
                var emptyVarAssignmentRequired = (from variable in filterEmptyConfigValues
                                          where lstSysVariables3.Any(x => x.Contains(variable.Key.ToUpper()))
                                          select variable).ToList();

                //taking only variables and values
                var lineVariableAssignmentRequired = (from val1 in emptyVarAssignmentRequired
                                               where (val1.Value != null)
                                               select new VariableAssignment()
                                               {
                                                   VariableId = val1.Key,
                                                   Value = val1.Value
                                               }).ToList();
                //filtering variable ids
                // empty call variables mapping
                var dutyCallValuesJack = (from stubValues in lstSysVariables3
                                      from unitResponse in unitConfigureRequestDictionary
                                      where Utility.CheckEquals(unitResponse.Key.Substring(unitResponse.Key.IndexOf(Constant.DOTCHAR) + 1).ToString(), stubValues)
                                      select new VariableAssignment()
                                      {
                                          VariableId = stubValues,
                                          Value = unitResponse.Value
                                      }).Distinct().ToList();
                if (!dutyCallValuesJack.Any(x => x.VariableId.Contains(Constants.ECARWT)))
                {
                    var dutyECARWTValue = (from stubValues in lstSysVariables3
                                           from unitResponse in emptyConfigureResponse
                                           where stubValues.Contains(Constants.ECARWT) && unitResponse.Key.Contains(Constants.ECARWT)
                                           select new VariableAssignment()
                                           {
                                               VariableId = stubValues,
                                               Value = Convert.ToString(Math.Round(Convert.ToDecimal(unitResponse.Value), 2))
                                           }).Distinct().ToList();
                    dutyCallValuesJack.AddRange(dutyECARWTValue);
                }
                if (dutyCallValuesJack != null && dutyCallValuesJack.Any())
                {
                    var dutyRequiredVariablesObj = (JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONJACKDUTYREQUESTBODYVARIABLES, Constant.END100))));
                    
                    var dutyRequiredVariables = Utility.DeserializeObjectValue<List<VariableAssignment>>(Utility.SerializeObjectValue(dutyRequiredVariablesObj["variableAssignments"]));
                    var dutyRequiredValues = dutyRequiredVariables.Where(a => !dutyCallValuesJack.Any(x => x.VariableId == a.VariableId)).ToList();

                    if (dutyRequiredValues != null && dutyRequiredValues.Any())
                    {
                        dutyVariabels.AddRange(dutyCallValuesJack);
                        dutyVariabels.AddRange(dutyRequiredValues);
                    }
                }
                if (dutyVariabels.Any())
                {
                    dutyVariabels.Distinct();
                    foreach (var savedUnitVariables in lstVariableAssignment)
                    {
                        foreach (var dutyValues in dutyVariabels)
                        {
                            if (Utility.CheckEquals(savedUnitVariables.VariableId.Substring(savedUnitVariables.VariableId.IndexOf(Constant.DOTCHAR) + 1), dutyValues.VariableId))
                            {
                                dutyValues.Value = savedUnitVariables.Value;
                            }
                        }
                    }
                }
                if (lineVariableAssignmentRequired != null && lineVariableAssignmentRequired.Any())
                {
                    foreach (var mainResponseDutyVariables in lineVariableAssignmentRequired)
                    {
                        foreach (var emptyValues in dutyVariabels)
                        {
                            if (Utility.CheckEquals(mainResponseDutyVariables.VariableId, emptyValues.VariableId))
                            {
                                emptyValues.Value = mainResponseDutyVariables.Value;
                            }
                        }
                    }
                }
                var crForJackCall = new ConfigureRequest
                {
                    Line = new Line()
                };
                crForJackCall.Line.VariableAssignments = dutyVariabels;
                var lineVariableAssignmentsJackCall = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(crForJackCall.Line));
                var dutyConfigureRequestJack = CreateConfigurationRequestWithTemplate(lineVariableAssignmentsJackCall, Constant.SYSTEMVALIDATIONJACKDUTYCALL, null, productName);
                //Gets the base configuration of the model
                dutyConfigureRequestJack = _configureService.GetBaseConfigureRequest(dutyConfigureRequestJack);
                packagePath = dutyConfigureRequestJack?.PackagePath;
                dutyConfigureRequestJack.Settings.IncludeSections = new List<String>();
                
                var dutyConfigureResponseJObjJack = await ConfigurationBl(dutyConfigureRequestJack, packagePath, sessionId
                ).ConfigureAwait(false);
            }
            return Tuple.Create(dutyConfigureResponseJObj, dutyVariabels);
        }


        /// <summary>
        /// SystemValidationForUnitBl
        /// </summary>
        /// <param Name="lstVariableAssignment"></param>
        /// <param Name="unitValues"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="sectionTab"></param>
        /// <returns></returns>
        public async Task<Tuple<ResponseMessage, List<VariableAssignment>>> SystemValidationForUnitBl(List<VariableAssignment> lstVariableAssignment, List<UnitDetailsForTP2> unitValues, string sessionId, string sectionTab, List<UnitVariablesDetailsForTP2> unitVariablesDetailsForTP2s)
        {
            var methodBeginTime = Utility.LogBegin();
            List<VariableAssignment> systemVariablesResponse = new List<VariableAssignment>();
            List<SystemValidationKeyValues> mainConflictsValues = new List<SystemValidationKeyValues>();
            if (unitVariablesDetailsForTP2s != null && unitVariablesDetailsForTP2s.Any())
            {
                foreach (var unitVariablesSet in unitVariablesDetailsForTP2s)
                {
                    List<VariableAssignment> lstvariableassignment = unitVariablesSet.VariablesDetails.Select(
                         variableAssignment => new VariableAssignment
                         {
                             VariableId = variableAssignment.VariableId,
                             Value = variableAssignment.Value
                         }).ToList<VariableAssignment>();
                    lstVariableAssignment = lstvariableassignment;
                    var cabSlingVariabels = new List<VariableAssignment>();
                    var emptyVariabels = new List<VariableAssignment>();
                    var dutyVariabels = new List<VariableAssignment>();

                    var unitConfigureResponseJObj = await SystemValidationUnitCall(lstvariableassignment, sessionId, unitVariablesDetailsForTP2s);

                    //configuration object values for conflict mapping
                    var actualConfigureResponse = unitConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
                    var unitConfigureResponseArgument = actualConfigureResponse.Arguments;
                    var unitConfigureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(unitConfigureResponseArgument));
                    var unitConfigureRequestDictionary = unitConfigureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();

                    var (cabConfigureResponseJObj, cabSlingVariable) = await SystemValidationCabCall(lstVariableAssignment, sessionId, unitConfigureResponseJObj, unitVariablesDetailsForTP2s);
                    var cr = new ConfigureRequest
                    {
                        Line = new Line()
                    };
                    cr.Line.VariableAssignments = cabSlingVariable;
                    systemVariablesResponse.AddRange(cabSlingVariable);
                    var lineVariableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(cr.Line));
                    var cabConfigureRequest = CreateConfigurationRequestWithTemplate(lineVariableAssignments, Constant.SYSTEMVALIDATIONCABCALL, null, unitVariablesDetailsForTP2s.FirstOrDefault().ProductName);
                    var packagePath = cabConfigureRequest?.PackagePath;

                    var cabConfigureResponse = cabConfigureResponseJObj.Response.ToObject<ConfigurationResponse>();
                    var stubUnitConfigurationMainResponseObj = new ConfigurationResponse();
                    //// Main Stub 
                    var stubMainSubSectionResponse = JObject.Parse(File.ReadAllText(Constant.UNITTUIRESPONSETEMPLATE));
                    //// setting stub data into an sectionsValues object
                    stubUnitConfigurationMainResponseObj = stubMainSubSectionResponse.ToObject<ConfigurationResponse>();
                    List<VariableAssignment> emptyVariable = new List<VariableAssignment>();
                    ResponseMessage emptyConfigureResponseJObj = new ResponseMessage();
                    if (unitVariablesDetailsForTP2s != null && !unitVariablesDetailsForTP2s.FirstOrDefault().ProductName.Equals(Constant.ENDURA_100))
                    {
                        (emptyConfigureResponseJObj, emptyVariable) = await SystemValidationEmptyCall(lstVariableAssignment, cabConfigureResponse, unitConfigureRequestDictionary, unitVariablesDetailsForTP2s.FirstOrDefault().ProductName, sessionId);

                    }
                    else
                    {
                        var sysValContantsDictionaryParameters = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONPARAMETERS, Constant.END100)));
                        var lstSysVariablesForSlingCall = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(sysValContantsDictionaryParameters["SlingCall"]));
                        var slingCallRequiredVariables = (from stubValues in lstSysVariablesForSlingCall
                                                          from unitResponse in unitConfigureRequestDictionary
                                                          where Utility.CheckEquals(unitResponse.Key.Substring(unitResponse.Key.IndexOf(Constant.DOTCHAR) + 1).ToString(), stubValues)
                                                          select new VariableAssignment()
                                                          {
                                                              VariableId = stubValues,
                                                              Value = unitResponse.Value
                                                          }).Distinct().ToList();
                        var slingRequiredVariablesObj = (JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONSLINGREQUESTBODYVARIABLE, Constant.END100))));
                        var slingRequiredVariables = Utility.DeserializeObjectValue<List<VariableAssignment>>(Utility.SerializeObjectValue(slingRequiredVariablesObj[Constant.SYSVALVARIABLEASSIGNMENTS]));
                        var slingRequiredVariablesToBeSent = slingRequiredVariables.Where(a => !slingCallRequiredVariables.Any(x => x.VariableId == a.VariableId)).ToList();
                        List<VariableAssignment> variablesForSlingCall = new List<VariableAssignment>();
                        if (slingRequiredVariablesToBeSent != null && slingCallRequiredVariables.Any())
                        {
                            variablesForSlingCall.AddRange(slingRequiredVariablesToBeSent);
                            variablesForSlingCall.AddRange(slingCallRequiredVariables);
                        }
                        var cr2 = new ConfigureRequest
                        {
                            Line = new Line()
                        };
                        cr2.Line.VariableAssignments = variablesForSlingCall;
                        var lineVariableAssignmentsForSlingCall = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(cr2.Line));
                        emptyConfigureResponseJObj = await SystemValidationSlingCall(lineVariableAssignmentsForSlingCall, sessionId, emptyConfigureResponseJObj, unitVariablesDetailsForTP2s.FirstOrDefault().ProductName);
                        emptyVariable = slingCallRequiredVariables;
                    }

                    //adding absent variables to request body
                    cr.Line.VariableAssignments = emptyVariable;
                    systemVariablesResponse.AddRange(emptyVariable);
                    var lineVariableAssignments2 = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(cr.Line));
                    ConfigurationRequest emptyConfigureRequest = new ConfigurationRequest();
                    if(unitVariablesDetailsForTP2s != null && !unitVariablesDetailsForTP2s.FirstOrDefault().ProductName.Equals(Constant.ENDURA_100))
                    {
                        emptyConfigureRequest = CreateConfigurationRequestWithTemplate(lineVariableAssignments2, Constant.SYSTEMVALIDATIONEMPTYCALL, null, unitVariablesDetailsForTP2s.FirstOrDefault().ProductName);
                    }
                    else
                    {
                        emptyConfigureRequest = CreateConfigurationRequestWithTemplate(lineVariableAssignments2, Constant.SYSTEMVALIDATIONSLINGCALL, null, unitVariablesDetailsForTP2s.FirstOrDefault().ProductName);
                    }
                    //Gets the base configuration of the model
                    emptyConfigureRequest = _configureService.GetBaseConfigureRequest(emptyConfigureRequest);
                    IDictionary<string,string> emptyConfigureResponse = new Dictionary<string, string>();
                    if (unitVariablesDetailsForTP2s != null && unitVariablesDetailsForTP2s.FirstOrDefault().ProductName.Equals(Constant.EVO_200))
                    {
                        emptyConfigureResponse = emptyConfigureResponseJObj.Response.ToObject<IDictionary<string, string>>();
                    }
                    else if(unitVariablesDetailsForTP2s!=null && (unitVariablesDetailsForTP2s.FirstOrDefault().ProductName.Equals(Constant.EVO_100) || 
                    unitVariablesDetailsForTP2s.FirstOrDefault().ProductName.Equals(Constant.ENDURA_100)))
                    {
                        emptyConfigureResponse = JObject.FromObject((emptyConfigureResponseJObj.Response.ToObject<StartConfigureResponse>()).Arguments)["Configuration"].ToObject<IDictionary<string, string>>();
                    }
                    packagePath = emptyConfigureRequest?.PackagePath;

                    var (dutyConfigureResponseJObj, dutyVariable) = await SystemValidationDutyCall(packagePath, sessionId, emptyConfigureRequest, lstVariableAssignment, emptyConfigureResponseJObj, unitConfigureRequestDictionary, unitVariablesDetailsForTP2s.FirstOrDefault().ProductName);
                    cr.Line.VariableAssignments = dutyVariable;
                    systemVariablesResponse.AddRange(dutyVariable);
                    var lineVariableAssignments3 = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(cr.Line));
                    var dutyConfigureRequest = CreateConfigurationRequestWithTemplate(lineVariableAssignments3, Constant.SYSTEMVALIDATIONDUTYCALL, null, unitVariablesDetailsForTP2s.FirstOrDefault().ProductName);
                    //Gets the base configuration of the model
                    dutyConfigureRequest = _configureService.GetBaseConfigureRequest(dutyConfigureRequest);
                    var dutyConfigureResponse = dutyConfigureResponseJObj.Response.ToObject<ConfigurationResponse>();
                    // configuration object values for conflict mapping
                    var actualDutyConfigureResponse = unitConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
                    var dutyConfigureResponseArgument = actualDutyConfigureResponse.Arguments;
                    var dutyConfigureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(dutyConfigureResponseArgument));

                    //Getting all system validation response variables for saving in DB
                    List<ResponseMessage> cabEmptyDutyResponseList = new List<ResponseMessage>();
                    cabEmptyDutyResponseList.Add(cabConfigureResponseJObj);
                    if (unitVariablesDetailsForTP2s!=null && unitVariablesDetailsForTP2s.FirstOrDefault().ProductName.Equals(Constants.EVO_100))
                    {
                        cabEmptyDutyResponseList.Add(emptyConfigureResponseJObj);
                    }
                    cabEmptyDutyResponseList.Add(dutyConfigureResponseJObj);
                    foreach (var responseList in cabEmptyDutyResponseList)
                    {
                        var configureResponse = responseList.Response.ToObject<StartConfigureResponse>();
                        var configureResponseArgument = configureResponse.Arguments;
                        var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
                        var configureRequestDictionary = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, string>>();
                        List<VariableAssignment> argumentsList = configureRequestDictionary.Select(
                               variableAssignment => new VariableAssignment
                               {
                                   VariableId = variableAssignment.Key,
                                   Value = variableAssignment.Value
                               }).ToList<VariableAssignment>();

                        argumentsList.RemoveAll(x => x.VariableId.Contains(Constant.ERRORVALUE));
                        systemVariablesResponse.AddRange(argumentsList);
                    }
                    if (unitVariablesDetailsForTP2s.FirstOrDefault().ProductName.Equals(Constants.EVO_200))
                    {
                        var configureRequestDictionary = emptyConfigureResponseJObj.Response.ToObject<IDictionary<string, string>>();
                        List<VariableAssignment> argumentsList = configureRequestDictionary.Select(
                               variableAssignment => new VariableAssignment
                               {
                                   VariableId = variableAssignment.Key,
                                   Value = variableAssignment.Value
                               }).ToList<VariableAssignment>();

                        argumentsList.RemoveAll(x => x.VariableId.Contains(Constant.ERRORVALUE));
                        systemVariablesResponse.AddRange(argumentsList);
                    }

                    // getting empty variables and also duty variables messages 
                    var sysValContantsDictionary = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONPARAMETERS, Constant.EVOLUTION200)));
                    if (unitVariablesDetailsForTP2s != null && unitVariablesDetailsForTP2s.FirstOrDefault().ProductName.Equals(Constants.EVO_200))
                    {
                        sysValContantsDictionary = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONPARAMETERS, Constant.EVOLUTION200)));
                    }
                    else if (unitVariablesDetailsForTP2s!= null && unitVariablesDetailsForTP2s.FirstOrDefault().ProductName.Equals(Constants.EVO_100))
                    {
                        sysValContantsDictionary = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATIONPARAMETERS, Constant.EVOLUTION__100)));
                    }
                    var requiredConflictFlags = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(sysValContantsDictionary["ValidationConflicts"]));
                    //variables from vtpkg first call
                    var listOfFlagVariables = new List<Variables>();

                    var emptyCallConflictsVariables = (from section in emptyConfigureResponse
                                                       where Utility.CheckEquals(section.Value.ToString(), Constant.TRUEVALUES)
                                                       select section);
                    if(unitVariablesDetailsForTP2s.FirstOrDefault().ProductName.Equals(Constant.EVO_100))
                    {
                        var emptyCallConflictsVariablesForEvo100 = (from section in emptyConfigureResponseJObj.Response.ToObject<ConfigurationResponse>().Sections
                                                           from variable in section.Variables
                                                           where Utility.CheckEquals(section.Id, "R100289459")
                                                           select variable).ToList();
                        if (emptyCallConflictsVariablesForEvo100.Any())
                        {
                            listOfFlagVariables.AddRange(emptyCallConflictsVariablesForEvo100);
                        }
                    }

                    var dutyCallConflictVariables = (from Sections in dutyConfigureResponse.Sections
                                                     from variable in Sections.Variables
                                                     where Utility.CheckEquals(Sections.Id, Constant.DUTYCALLMATERIALNAME)
                                                     select variable).ToList();
                    if (dutyCallConflictVariables.Any())
                    {
                        listOfFlagVariables.AddRange(dutyCallConflictVariables);
                    }
                    
                    List<SystemValidationKeyValues> systemConflictVariables = new List<SystemValidationKeyValues>();
                    if (listOfFlagVariables.Any())
                    {
                        var flagAssignedValues = (from conflictVariables in listOfFlagVariables
                                                  from valueFilter in conflictVariables.Values
                                                  where (valueFilter.Assigned != null) && Utility.CheckEquals(valueFilter.value.ToString(), Constant.TRUEVALUES)
                                                  select conflictVariables).Distinct().ToList();
                        if (flagAssignedValues != null && flagAssignedValues.Any())
                        {
                            foreach (var flagValues in requiredConflictFlags)
                            {
                                foreach (var systemsVaraibles in flagAssignedValues)
                                {
                                    if (systemsVaraibles.Id.Contains(flagValues))
                                    {
                                        var des = systemsVaraibles.Properties.Where(x => Utility.CheckEquals(x.Id, Constant.SYSVALDESCRIPTION)).ToList();
                                        var sysValusData = new SystemValidationKeyValues()
                                        {
                                            UnitId = unitVariablesSet.UnitId,
                                            SystemValidKeys = systemsVaraibles.Id.Split(Constant.DOTCHAR)[1],
                                            SystemValidValues = des[0].Value.ToString()
                                        };
                                        systemConflictVariables.Add(sysValusData);
                                    }
                                    // please don't remove this need to discuss and add the required changes 
                                    if (emptyCallConflictsVariables != null && emptyCallConflictsVariables.Any())
                                    {
                                        foreach (var item in emptyCallConflictsVariables)
                                        {
                                            if (item.Key.Contains(flagValues))
                                            {
                                                var des = systemsVaraibles.Properties.Where(x => Utility.CheckEquals(x.Id, Constant.SYSVALDESCRIPTION)).ToList();
                                                var sysValusData = new SystemValidationKeyValues()
                                                {
                                                    UnitId = unitVariablesSet.UnitId,
                                                    SystemValidKeys = item.Key,
                                                    SystemValidValues = des[0].Value.ToString()
                                                };
                                                systemConflictVariables.Add(sysValusData);
                                            }
                                        }
                                    }
                                }
                              

                            }
                        }
                        else
                        {
                            //added else part to handle the flow
                            var sysValusData = new SystemValidationKeyValues()
                            {
                                UnitId = unitVariablesSet.UnitId,
                                SystemValidKeys = string.Empty,
                                SystemValidValues = string.Empty
                            };
                            systemConflictVariables.Add(sysValusData);
                        }
                    }
                    else
                    {
                        var sysValusData = new SystemValidationKeyValues()
                        {
                            UnitId = unitVariablesSet.UnitId,
                            SystemValidKeys = string.Empty,
                            SystemValidValues = string.Empty
                        };
                        systemConflictVariables.Add(sysValusData);
                    }
                    mainConflictsValues.AddRange(systemConflictVariables.Distinct().ToList());
                }
            }
            Utility.LogEnd(methodBeginTime);
            return Tuple.Create(new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                ResponseArray = JArray.FromObject(mainConflictsValues)
            },
            systemVariablesResponse);
        }

        /// <summary>
        /// GetConflictCacheValues
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public ConfigurationResponse GetConflictCacheValues(string sessionId, ConfigurationResponse cacheSetForConflicts)
        {
            var methodBeginTime = Utility.LogBegin();
            var conflictsValues = new ConfigurationResponse();
            if (cacheSetForConflicts != null)
            {
                _cpqCacheManager.SetCache(sessionId, _environment, Constant.PREVIOUSUNITCONFLICTSVALUESFORVALIDATION, Utility.SerializeObjectValue(cacheSetForConflicts));
            }
            else
            {
                var getCacheResponse = _cpqCacheManager.GetCache(sessionId, _environment, Constant.PREVIOUSUNITCONFLICTSVALUESFORVALIDATION);
                if (!string.IsNullOrEmpty(getCacheResponse))
                {
                    conflictsValues = Utility.DeserializeObjectValue<ConfigurationResponse>(getCacheResponse);
                }
            }
            Utility.LogEnd(methodBeginTime);
            return conflictsValues;
        }

        private bool ShouldReconfigure(IDictionary<string, string> emptyConfigureResponse)
        {
            //loop throug key values nd find Weight Flags and any of them or true then return false
            // if ny flags are means dont need to reconfigure
            var reconfigFlags = _configuration["Paramsettins:ReconfigFlags"].Split(",").ToList();
            foreach (var flag in reconfigFlags)
            {

                if (!string.IsNullOrEmpty(emptyConfigureResponse[flag]) && Convert.ToBoolean(emptyConfigureResponse[flag]))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<ResponseMessage> ReConfigForSystemValidation(IDictionary<string, string> emptyConfigureResponse, ConfigurationRequest slingConfigureRequest, ConfigurationRequest emptyConfigureRequest, string packagePath, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var emptyValidationResponse = emptyConfigureResponse;
            var shoudlReconfig = ShouldReconfigure(emptyConfigureResponse);

            while (shoudlReconfig)
            {
                //TODO: Update input to with empty call response
                var slingConfigureResponseJObj = await ConfigurationBl(slingConfigureRequest, packagePath, sessionId).ConfigureAwait(false);
                var emptyConfigureResponseJObj = await ConfigurationBl(emptyConfigureRequest, packagePath, sessionId).ConfigureAwait(false);
                emptyValidationResponse = emptyConfigureResponseJObj.Response.ToObject<IDictionary<string, string>>();
                shoudlReconfig = ShouldReconfigure(emptyConfigureResponse);
            }

            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = JObject.FromObject(emptyValidationResponse)
            };
        }

        public string GetRoleName(string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var userInfoResponse = _cpqCacheManager.GetCache(sessionId, _environment, Constant.USERDETAILSCPQ);
            var user = !string.IsNullOrEmpty(userInfoResponse) ? Utility.DeserializeObjectValue<User>(userInfoResponse) : new User();
            string roleName = user.Role.name;
            Utility.LogEnd(methodBeginTime);
            return roleName;
        }

        public List<VariableAssignment> SetCacheHoistwayDimensions(List<VariableAssignment> variables, string sessionId, int setId)
        {
            var methodBeginTime = Utility.LogBegin();
            var hoistwayValues = variables;
            var username = GetUserId(sessionId);
            if (variables != null && variables.Any())
            {
                _cpqCacheManager.SetCache(username, _environment, Constant.HOISTWAYDIMENSIONS, setId.ToString(), Utility.SerializeObjectValue(hoistwayValues));
            }
            else
            {
                var hoistget = _cpqCacheManager.GetCache(username, _environment, Constant.HOISTWAYDIMENSIONS, setId.ToString());
                if (!string.IsNullOrEmpty(hoistget))
                {
                    hoistwayValues = Utility.DeserializeObjectValue<List<VariableAssignment>>(hoistget);
                }
            }
            Utility.LogEnd(methodBeginTime);
            return hoistwayValues;

        }

        /// <summary>
        /// Start method for Non Configurable Products
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="productCategory"></param>
        /// <param name="variableAssignments"></param>
        /// <param name="setId"></param>
        /// <returns></returns>
        public async Task<ConfigurationResponse> StartNonConfigurableProductConfigure(string sessionId, string productCategory,
                     JObject variableAssignments = null, int setId = 0, List<string> permissions = null)
        {
            var methodBeginTime = Utility.LogBegin();
            // Checking the Cache and Updating the Variables
            var configureRequest = UpdatingVariableAssignmentsInCache(productCategory, sessionId, setId, productCategory, variableAssignments);
            var variableId = Convert.ToString(JObject.Parse(File.ReadAllText(Constants.UNITCONSTANTMAPPERTEMPLATE))[Constants.FLOORSSERVED]);
            var numberOfFloorsVariable = configureRequest.Line.VariableAssignments.Where(x => Utility.CheckEquals(x.VariableId, variableId)).ToList();
            var numberOfFloor = numberOfFloorsVariable.Any() ? numberOfFloorsVariable[0].Value : 0;
            //Gets the base configuration of the model
            var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest);
            var configuration = GetConfigurationByProductCategoryForNCP(productCategory);
            //Adding include sections
            baseConfigureRequest = GenerateIncludeSections(baseConfigureRequest, configuration, productCategory);
            var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, configureRequest.PackagePath, sessionId).ConfigureAwait(false);
            var baseConfigureResponse = baseConfigureResponseJObj.Response.ToObject<ConfigurationResponse>();
            // UI Template
            var unitConfigurationResponseObj = GetUnitConfigurationByProductCategoryForNCP(productCategory);
            var mainFillteredBuildingConfigResponse = Utility.MapVariables(JObject.FromObject(baseConfigureResponse), JObject.FromObject(unitConfigurationResponseObj));
            //Required because as Utility.FilterNullValues function works with only with C# types 
            var response = Utility.DeserializeObjectValue<ConfigurationResponse>(Utility.SerializeObjectValue(mainFillteredBuildingConfigResponse));

            if (Utility.Equals(productCategory, Constant.ESCALATORMOVINGWALK))
            {
                productCategory = Constant.ESCALATORTYPE;
            }

            //Changing Max Value Property to Feet Input in Escalator screen
            if (Utility.CheckEquals(productCategory, Constant.ESCALATORTYPE))
            {
                foreach (var variable in response.Sections[0].Variables)
                {
                    if (Utility.CheckEquals(variable.Id, Constants.RISEPARAMETER))
                    {
                        foreach (var properties in variable.Properties)
                        {
                            if (Utility.CheckEquals(properties.Id, Constant.MAXVALUESMALLCASE))
                            {
                                int val = Convert.ToInt32(properties.Value) / 12;
                                properties.Value = val;
                            }
                        }
                    }
                }
            }

            // Adding Min,Max RangeValidation Requirement for Twin Elevator Fron and Rear Openings
            if (Utility.CheckEquals(productCategory, Constant.TWINELEVATOR) || Utility.CheckEquals(productCategory, Constant.OTHER))
            {
                foreach (var sectionVariables in response.Sections[0].Variables)
                {
                    if (Utility.CheckEquals(sectionVariables.Id, Convert.ToString(JObject.Parse(File.ReadAllText(Constants.UNITCONSTANTMAPPERTEMPLATE))[Constants.FRONTOPENING]))
                        || Utility.CheckEquals(sectionVariables.Id, Convert.ToString(JObject.Parse(File.ReadAllText(Constants.UNITCONSTANTMAPPERTEMPLATE))[Constants.REAROPENING])))
                    {
                        var minValuesString = 0;

                        var maxValuesString = numberOfFloor;
                        if (Utility.CheckEquals(productCategory, Constant.OTHER) && Utility.CheckEquals(sectionVariables.Id, Constants.PARAMETERFRONTOPENINGS)
                            || Utility.CheckEquals(productCategory, Constant.TWINELEVATOR) && Utility.CheckEquals(sectionVariables.Id, Constants.PARAMETERFRONTOPENINGS))
                        {
                            minValuesString = 1;
                        }
                        sectionVariables.Properties.Add(new Properties
                        {

                            Id = Constant.MINVALUESMALLCASE,
                            Value = minValuesString,
                            Type = Constant.NUMBER
                        }
                        );
                        sectionVariables.Properties.Add(new Properties
                        {
                            Id = Constant.MAXVALUESMALLCASE,
                            Value = maxValuesString,
                            Type = Constant.NUMBER
                        }
                        );
                        sectionVariables.Properties.Add(new Properties
                        {
                            Id = Constant.RANGEVALIDATION,
                            Value = string.Format(Constant.TWINELEVATORMESSAGE + minValuesString + Constant.AND + maxValuesString)
                        }
                       );
                    }

                }
            }

            // added enriched data in the main response 
            var uiResponse = AddEnrichedData(response, string.Format(Constant.NCPENRICHEDDATA, productCategory), permissions);
            Utility.LogEnd(methodBeginTime);
            return response;
        }



        /// <summary>
        /// Common Method for Updating Variable Assignments in Cache
        /// </summary>
        /// <param name="configurationName"></param>
        /// <param name="sessionId"></param>
        /// <param name="setId"></param>
        /// <param name="productType"></param>
        /// <param name="variableAssignments"></param>
        /// <returns></returns>
        public ConfigurationRequest UpdatingVariableAssignmentsInCache(string configurationName, string sessionId, int setId, string productType, JObject variableAssignments)
        {
            var crossPackagevariableAssignments = new List<VariableAssignment>();
            var getCrossPackageValues = GetCrosspackageVariableAssignments(sessionId + setId, productType);
            if (!string.IsNullOrEmpty(getCrossPackageValues))
            {
                crossPackagevariableAssignments = Utility.DeserializeObjectValue<List<VariableAssignment>>(getCrossPackageValues);
            }
            var assignments = Utility.DeserializeObjectValue<Line>(Utility.SerializeObjectValue(variableAssignments));
            //generate cross package variable assignments
            crossPackagevariableAssignments = GenerateVariableAssignmentsForUnitConfiguration(crossPackagevariableAssignments, assignments);
            var variableAssignmentz = new Line();
            variableAssignmentz.VariableAssignments = crossPackagevariableAssignments;
            SetCrosspackageVariableAssignments(crossPackagevariableAssignments, sessionId + setId, productType);
            variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(variableAssignmentz));

            var configureRequest = CreateConfigurationRequestWithTemplate(variableAssignments, configurationName, null, productType);
            var mainGroupConfigurationResponse = new ConfigurationResponse();
            mainGroupConfigurationResponse.Sections = new List<Sections>();
            var packagePath = configureRequest?.PackagePath;
            var modifiedOn = new DateTime();
            var currentConfigurationCache = _cpqCacheManager.GetCache(sessionId + setId, _environment, productType);
            if (configureRequest != null && (string.IsNullOrEmpty(packagePath)))
            {
                if (String.IsNullOrEmpty(currentConfigurationCache))
                {
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = _localizer[Constant.SESSIONIDPARENTCODEISNOMOREVALID]
                    });
                }
                var currentConfiguration = Utility.DeserializeObjectValue<StartConfigureResponse>(currentConfigurationCache);
                if (currentConfiguration.Audits.ModifiedOn == null)
                {
                    currentConfiguration.Audits.ModifiedOn = new DateTime();
                }
                if (currentConfiguration != null && modifiedOn != default(DateTime) &&
                    DateTime.Compare(modifiedOn, (DateTime)currentConfiguration.Audits.ModifiedOn) > 0)
                {
                    currentConfiguration.Audits.ModifiedOn = modifiedOn;
                }

                var value = Utility.SerializeObjectValue(currentConfiguration);
                _cpqCacheManager.SetCache(sessionId + setId, _environment, productType, value);
                //Response for UI
                var updatedStartConfigureResponse = ViewModelResponseMapper(currentConfiguration);
                var updatedCurrentConfiguration = Utility.SerializeObjectValue(updatedStartConfigureResponse);
            }
            return configureRequest;
        }

        /// <summary>
        /// Adding Enriched Data in Response
        /// </summary>
        /// <param name="configurationResponse"></param>
        /// <param name="configurationPath"></param>
        /// <returns></returns>
        public ConfigurationResponse AddEnrichedData(ConfigurationResponse configurationResponse, string configurationPath, List<string> permissions = null)
        {
            var enrichedData = JObject.Parse(File.ReadAllText(configurationPath));
            configurationResponse.EnrichedData = enrichedData;
            if (permissions != null)
            {
                configurationResponse.Permissions = permissions;
            }
            configurationResponse.ConflictAssignments = new ConflictManagement();
            configurationResponse.ConfiguratorStatus = new Status();
            configurationResponse.SystemValidateStatus = new Status();
            configurationResponse.Units = new List<UnitNames>();
            return configurationResponse;
        }


        /// <summary>
        /// Start Method for Custom Engineered Products
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="productType"></param>
        /// <param name="variableAssignments"></param>
        /// <param name="setId"></param>
        /// <returns></returns>
        public async Task<ConfigurationResponse> StartCustomEngineeredProductConfigure(string sessionId, string productType,
                   JObject variableAssignments = null, int setId = 0, List<string> permissions = null)
        {
            var methodBeginTime = Utility.LogBegin();
            // Checking the Cache and Updating the Variables
            var configureRequest = UpdatingVariableAssignmentsInCache(Constant.UNITCONFIG, sessionId, setId, productType, variableAssignments);
            //Gets the base configuration of the model
            var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest);
            //Getting the Configuration By Product Type
            var configuration = GetConfigurationByProductType(productType);
            //Adding include sections
            baseConfigureRequest = GenerateIncludeSections(baseConfigureRequest, configuration, productType);
            var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, configureRequest.PackagePath, sessionId).ConfigureAwait(false);
            var baseConfigureResponse = baseConfigureResponseJObj.Response.ToObject<ConfigurationResponse>();
            // UI Template
            var unitConfigurationResponseObj = GetUnitConfigurationByProductType(productType);
            var mainFillteredBuildingConfigResponse = Utility.MapVariables(JObject.FromObject(baseConfigureResponse), JObject.FromObject(unitConfigurationResponseObj));
            //Required because as Utility.FilterNullValues function works with only with C# types 
            var response = Utility.DeserializeObjectValue<ConfigurationResponse>(Utility.SerializeObjectValue(mainFillteredBuildingConfigResponse));
            // added enriched data in the main response 
            var uiResponse = AddEnrichedData(response, string.Format(Constant.CUSTOMENGINEEREDENRICHEDDATA, productType.Replace("CE_", "")), permissions);
            Utility.LogEnd(methodBeginTime);
            return response;
        }



        /// <summary>
        /// Method for Group Info
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="variableAssignments"></param>
        /// <returns></returns>
        public async Task<JObject> GroupInfoConfigure(string sessionId, List<ConfigVariable> variableAssignments = null)
        {
            var methodBeginTime = Utility.LogBegin();
            var groupInfoSectionsResponseObj = new ConfigurationResponse();
            // Sections Stub 
            var groupInfoResponse = JObject.Parse(File.ReadAllText(Constant.GROUPINFOUIRESPONSEPATH));
            // setting stub data into an sectionsValues object
            groupInfoSectionsResponseObj = groupInfoResponse.ToObject<ConfigurationResponse>();
            var lstVariables = Utility.GetVariables(groupInfoResponse.ToObject<JToken>());
            var variables = Utility.DeserializeObjectValue<List<Variables>>(Utility.SerializeObjectValue(lstVariables));

            if (variableAssignments != null)
            {
                foreach (var variable in variables)
                {
                    foreach (var assignment in variableAssignments)
                    {
                        if (!Utility.CheckEquals(assignment.VariableId, Constant.PRODUCTCATEGORY))
                        {
                            if (assignment.VariableId.Equals(variable.Id))
                            {
                                foreach (var value in variable.Values)
                                {
                                    if (!value.Type.Equals(Constant.INTERVALVALUE))
                                    {
                                        value.Name = Convert.ToString(assignment.Value);
                                        value.value = Convert.ToString(assignment.Value);
                                        value.Assigned = Constant.BYUSER_LOWERCASE;
                                    }
                                }
                            }
                        }
                        else if (Utility.CheckEquals(assignment.VariableId, Constant.PRODUCTCATEGORY))
                        {
                            foreach (var value in variable.Values)
                            {
                                if (!value.Type.Equals(Constant.INTERVALVALUE))
                                {
                                    if (value.value.Equals(assignment.Value))
                                    {
                                        value.Assigned = Constant.BYUSER_LOWERCASE;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            groupInfoSectionsResponseObj.Sections.FirstOrDefault().Variables = variables;
            var uiResponse = AddEnrichedData(groupInfoSectionsResponseObj, Constant.GROUPINFOENRICHEDDATA);
            Utility.LogEnd(methodBeginTime);
            return Utility.FilterNullValues(uiResponse);
        }

        /// <summary>
        /// Method for Setting up the Product Type in Cache
        /// </summary>
        /// <param name="variables"></param>
        /// <param name="sessionId"></param>
        /// <param name="setId"></param>
        /// <returns></returns>
        public List<VariableAssignment> SetCacheProductType(List<VariableAssignment> variables, string sessionId, int setId)
        {
            var methodBeginTime = Utility.LogBegin();
            var username = GetUserId(sessionId);
            if (variables != null)
            {
                var productType = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(variables));
                _cpqCacheManager.SetCache(username, _environment, Constant.PRODUCTTYPE, setId.ToString(), Utility.SerializeObjectValue(productType));
                Utility.LogEnd(methodBeginTime);
                return variables;
            }
            else
            {
                var productType = Utility.DeserializeObjectValue<List<VariableAssignment>>(_cpqCacheManager.GetCache(username, _environment, Constant.PRODUCTTYPE, setId.ToString()));
                Utility.LogEnd(methodBeginTime);
                return productType;
            }
        }

        /// <summary>
        /// UnitsConfigurationArgumentsResponse
        /// </summary>
        /// <param name="lstVariableAssignment"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> UnitsConfigurationArgumentsResponse(List<VariableAssignment> lstVariableAssignment, string sessionId, string productName)
        {
            var methodBegin = Utility.LogBegin();
            // Unit configuration call to get the required arguments
            var unitRequest = new ConfigureRequest
            {
                Line = new Line
                {
                    VariableAssignments = lstVariableAssignment
                }
            };
            var unitRequestObj = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(unitRequest.Line));
            var unitConfigureRequest = CreateConfigurationRequestWithTemplate(unitRequestObj, Constant.UNITDEFAULTSCLMCALL, null, productName);
            var unitPackagePath = unitConfigureRequest?.PackagePath;
            //Gets the base configuration of the model
            unitConfigureRequest = _configureService.GetBaseConfigureRequest(unitConfigureRequest);
            //Adding include sections
            var unitConfigureResponseJObj = await ConfigurationBl(unitConfigureRequest, unitPackagePath, sessionId).ConfigureAwait(false);
            // configuration object values for conflict mapping
            var ActualConfigureResponse = unitConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var unitConfigureResponseArgument = ActualConfigureResponse.Arguments;
            var unitConfigureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(unitConfigureResponseArgument));
            var unitConfigureRequestDictionary = unitConfigureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();
            Utility.LogEnd(methodBegin);
            return unitConfigureRequestDictionary;
        }

        /// <summary>
        /// Getting the Details Based on Product Type
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public static ConfigurationResponse GetUnitConfigurationByProductType(string productType)
        {
            var methodBegin = Utility.LogBegin();
            var stubUnitConfigurationSubSectionResponseObj = new ConfigurationResponse();
            switch (productType)
            {
                // Custom Engineered Gearless
                case Constant.CEGEARLESS:
                    stubUnitConfigurationSubSectionResponseObj = CheckNonConfigurableProductName(Constant.CUSTOMENGINEEREDGEARLESSSTUBPATH);
                    break;
                // Custom Engineered Geared
                case Constant.CEGEARED:
                    stubUnitConfigurationSubSectionResponseObj = CheckNonConfigurableProductName(Constant.CUSTOMENGINEEREDGEAREDSTUBPATH);
                    break;
                // Custom Engineered Hydraulic
                case Constant.CEHYDRAULIC:
                    stubUnitConfigurationSubSectionResponseObj = CheckNonConfigurableProductName(Constant.CUSTOMENGINEEREDHYDRAULICSTUBPATH);
                    break;
                // Synergy
                case Constant.SYNERGY:
                    stubUnitConfigurationSubSectionResponseObj = CheckNonConfigurableProductName(Constant.SYNERGYSTUBPATH);
                    break;
            }
            Utility.LogEnd(methodBegin);
            return stubUnitConfigurationSubSectionResponseObj;
        }

        /// <summary>
        /// Get Configuration By Product Type
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public static string GetConfigurationByProductType(string productType)
        {
            var methodBegin = Utility.LogBegin();
            var configuration = string.Empty;
            if (Utility.CheckEquals(productType, Constant.CEGEARLESS))
            {
                configuration = Constant.CUSTOMENGINEEREDGEARLESSCONFIGURATION;
            }
            else if (Utility.CheckEquals(productType, Constant.CEGEARED))
            {
                configuration = Constant.CUSTOMENGINEEREDGEAREDCONFIGURATION;
            }
            else if (Utility.CheckEquals(productType, Constant.CEHYDRAULIC))
            {
                configuration = Constant.CUSTOMENGINEEREDHYDRAULICCONFIGURATION;
            }
            else if (Utility.CheckEquals(productType, Constant.SYNERGY))
            {
                configuration = Constant.SYNERGYCONFIGURATION;
            }
            Utility.LogEnd(methodBegin);
            return configuration;
        }

        /// <summary>
        /// Get Configuration By productCategory
        /// </summary>
        /// <param name="productCategory"></param>
        /// <returns></returns>
        public static string GetConfigurationByProductCategoryForNCP(string productCategory)
        {
            var methodBegin = Utility.LogBegin();
            string configuration = string.Empty;
            if (Utility.CheckEquals(productCategory, Constant.ESCLATORMOVINGWALK))
            {
                configuration = Constant.ESCLATORCONFIGURATION;
            }
            else if (Utility.CheckEquals(productCategory, Constant.TWINELEVATOR))
            {
                configuration = Constant.TWINELEVATORCONFIGURATION;
            }
            else if (Utility.CheckEquals(productCategory, Constant.OTHER))
            {
                configuration = Constant.OTHERSCREENCONFIGURATION;
            }
            Utility.LogEnd(methodBegin);
            return configuration;
        }

        /// <summary>
        /// Get Unit Configuration By Product Category For NCP
        /// </summary>
        /// <param name="productCategory"></param>
        /// <returns></returns>
        public static ConfigurationResponse GetUnitConfigurationByProductCategoryForNCP(string productCategory)
        {
            var methodBegin = Utility.LogBegin();
            var stubUnitConfigurationSubSectionResponseObj = new ConfigurationResponse();
            switch (productCategory)
            {
                // Escalator
                case Constant.ESCLATORMOVINGWALK:
                    stubUnitConfigurationSubSectionResponseObj = CheckNonConfigurableProductName(Constant.ESCLATORSTUBPATH);
                    break;
                // Twin Elevator
                case Constant.TWINELEVATOR:
                    stubUnitConfigurationSubSectionResponseObj = CheckNonConfigurableProductName(Constant.TWINELEVATORSTUBPATH);
                    break;
                // Other
                case Constant.OTHER:
                    stubUnitConfigurationSubSectionResponseObj = CheckNonConfigurableProductName(Constant.OTHERSCREENSTUBPATH);
                    break;
            }
            Utility.LogEnd(methodBegin);
            return stubUnitConfigurationSubSectionResponseObj;
        }

        /// <summary>
        /// Method For Check Non Configurable productName
        /// </summary>
        /// <param name="productCategoryPath"></param>
        /// <returns></returns>0
        public static ConfigurationResponse CheckNonConfigurableProductName(string productCategoryPath)
        {
            var methodBegin = Utility.LogBegin();
            var stubUnitConfigurationSubSectionResponseObj = new ConfigurationResponse();
            stubUnitConfigurationSubSectionResponseObj = JObject.Parse(File.ReadAllText(productCategoryPath)).ToObject<ConfigurationResponse>();
            Utility.LogEnd(methodBegin);
            return stubUnitConfigurationSubSectionResponseObj;
        }

        public async Task<ConfigurationResponse> CarFixtureStartUnitConfigureBL(string sessionId, string fixtureStrategy, string productType, string currentProductType,
            ConfigurationResponse baseConfigureResponse, ConfigurationRequest configureRequest, int setId, string sectionTab)
        {
            var methodBeginTime = Utility.LogBegin();
            var unitConfigurationResponseObj = new ConfigurationResponse();
            List<Compartment> lstcompartments = new List<Compartment>();
            var constantMapper = JObject.Parse(File.ReadAllText(Constant.UNITSVARIABLESMAPPERPATH));

            var CarFixtureResponseTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.CARFIXTURETEMPLATE, currentProductType)));
            var carFixtureTemplateObj = CarFixtureResponseTemplate.ToObject<ConfigurationResponse>();
            // Enriched Template
            var EnrichedTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, currentProductType)));
            if (productType.Equals(Constant.ENDURA_100))
            {
                CarFixtureResponseTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.CARFIXTURETEMPLATE, Constant.END100)));
            }
            if (productType.Equals(Constant.MODEL_EVO100))
            {
                CarFixtureResponseTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.CARFIXTURETEMPLATE, Constant.EVOLUTION__100)));
            }

            // Parameter For ETA/ETD not Enabled for ENDURA model
            if (!Utility.CheckEquals(productType, Constant.ENDURA_100))
            {
                if (Utility.CheckEquals(fixtureStrategy, Constant.ETA))
                {
                    CarFixtureResponseTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.CARFIXTURETEMPLATE, currentProductType)));
                    // setting stub data into an sectionsValues object
                    carFixtureTemplateObj = CarFixtureResponseTemplate.ToObject<ConfigurationResponse>();

                    var consolesList = constantMapper[Constant.ETAMAPPER].Select(x => x.ToString()).ToList();

                    var subSectionValues = (from section in carFixtureTemplateObj.Sections[0].sections
                                            where consolesList.Contains(section.Id)
                                            select section).ToList();
                    carFixtureTemplateObj.Sections[0].sections = subSectionValues;

                }
                else if (Utility.CheckEquals(fixtureStrategy, Constant.ETD) || Utility.CheckEquals(fixtureStrategy, Constant.ETA_AND_ETD))
                {
                    CarFixtureResponseTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.CARFIXTURETEMPLATE, currentProductType)));
                    carFixtureTemplateObj = CarFixtureResponseTemplate.ToObject<ConfigurationResponse>();
                }
            }
            else
            {
                CarFixtureResponseTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.CARFIXTURETEMPLATE, currentProductType)));
                // setting stub data into an sectionsValues object
                carFixtureTemplateObj = CarFixtureResponseTemplate.ToObject<ConfigurationResponse>();

                var consolesList = constantMapper[Constant.ETAMAPPER].Select(x => x.ToString()).ToList();

                var subSectionValues = (from section in carFixtureTemplateObj.Sections[0].sections
                                        where consolesList.Contains(section.Id)
                                        select section).ToList();
                carFixtureTemplateObj.Sections[0].sections = subSectionValues;
            }

            string CarFixtureCompartmentTemplate = File.ReadAllText(string.Format(Constant.CARFIXTURECOMPARTMENTPATH, currentProductType));
            var CarFixtureCompartmentTemplateObj = JsonConvert.DeserializeObject<CompartmentsData>(CarFixtureCompartmentTemplate);

            //getting main Filter Section values
            var mainFilteredRespone = Utility.MapVariables(Utility.SerializeObjectValue(baseConfigureResponse), Utility.SerializeObjectValue(carFixtureTemplateObj));

            var filteredSection = Utility.DeserializeObjectValue<ConfigurationResponse>(mainFilteredRespone);

            var filteredSections = filteredSection.Sections;
            unitConfigurationResponseObj.Sections = filteredSections;

            // get the variables id's from Stub
            // get the enrichment data to update properties

            foreach (var subsectionDetails in unitConfigurationResponseObj.Sections)
            {
                foreach (var subsection in subsectionDetails.sections)
                {
                    // get the constant Variables
                    var carFixtureContantsDictionary = Utility.GetVariableMapping(string.Format(Constant.UNITSVARIABLESMAPPERPATH, Constant.EVOLUTION200), Constant.UNITCARFIXTUREMAPPERCONFIGURATION);

                    //Adding the variables Name & value into compartments section 
                    if (subsection.Id == Constant.COPANDLOCKEDCOMPARTMENT)
                    {
                        foreach (var variables in subsection.Variables)
                        {
                            foreach (var values in variables.Values)
                            {
                                if (values.Name != Constant.TRUEVALUES && values.Name != Constant.FALSEVALUES && values.Name != Constant.True && values.Name != Constant.False)
                                {
                                    Compartment cm = new Compartment
                                    {
                                        name = values.Name.ToString(),
                                        value = values.value.ToString()
                                    };
                                    lstcompartments.Add(cm);
                                }
                            }
                        }
                        subsection.Compartments = lstcompartments.GroupBy(x => x.name).Select(c => c.First()).ToList();
                        if(!string.IsNullOrEmpty(productType) && (Utility.CheckEquals(productType, Constant.MODEL_EVO100) || Utility.CheckEquals(productType, Constant.ENDURA_100)))
                        {
                            var item = subsection.Compartments.Where(x=> x.value.Equals("FACEPLT")).FirstOrDefault();

                            subsection.Compartments.Remove(item);

                            subsection.Compartments.Insert(0, item);
                        }
                       
                        foreach (var data in CarFixtureCompartmentTemplateObj.compartments)
                        {
                            if (subsection.Compartments.Count > 0)
                            {
                                subsection.Compartments.Where(c => c.name == data.id).FirstOrDefault().name = data.name;
                            }
                        }
                    }
                    if (subsection.Id == Constant.CAROPERATINGPANEL)
                    {
                        if (configureRequest.Line.VariableAssignments != null && configureRequest.Line.VariableAssignments.Count() > 0)
                        {
                            foreach (var variable in configureRequest.Line.VariableAssignments)
                            {
                                if (variable.VariableId.Contains(Constant.LOCKREG) && (variable.Value.Equals(Constant.CCREG) || variable.Value.Equals(Constant.LOCKOUT)))
                                {
                                    var counts = 0;
                                    counts = _unitConfigurationDL.GetCarCallcutoutSavedOpenings(setId);
                                    subsection.sections = new List<SectionsGroupValues>();
                                    var secValues = new SectionsGroupValues
                                    {
                                        Id = Constant.ONE,
                                        Name = Constant.ELEVATORCARCALLCUTOUTKEYSWITCHESCONSOLE,
                                        Quantity = counts
                                    };
                                    var stubProperties = JArray.Parse(File.ReadAllText(Constant.PROPERTIESTEMPLATE));
                                    List<PropertyDetailsValues> varProprties = stubProperties.ToObject<List<PropertyDetailsValues>>();
                                    foreach (var varProperty in varProprties)
                                    {
                                        switch (varProperty.Id)
                                        {
                                            case Constant.SEQUENCE:
                                                varProperty.Value = Constant.ONE;
                                                break;
                                            case Constant.SECTIONNAME:
                                                varProperty.Value = Constant.CARCALLCUTOUTKEYSWITCH;
                                                break;
                                        }
                                    }
                                    secValues.Properties = varProprties;
                                    subsection.sections.Add(secValues);
                                }
                            }
                        }
                    }
                    if (Utility.CheckEquals(sectionTab, Constant.CARFIXTURE))
                    {
                        if (Utility.CheckEquals(subsection.Id, Constant.CARRIDINGLANTERNQUANTITY))
                        {
                            foreach (var variable in subsection.Variables)
                            {
                                if (Utility.CheckEquals(variable.Id, carFixtureContantsDictionary[Constant.CARRIDINGLANTERNQUANTITYSP]))
                                {
                                    var valuelist = (from value in variable.Values
                                                     where (value.State.Equals(Constant.AVAILABLE) || value.State.Equals(Constant.UNAVAILABLE) || value.State.Equals(Constant.SELECTED))
                                                     select value).ToList();
                                    variable.Values = valuelist;
                                }
                            }
                        }
                    }

                    foreach (var subsectionvariables in subsection.Variables)
                    {
                        //Condition for changing string to boolean
                        foreach (var variableItemsValues in subsectionvariables.Values)
                        {
                            if (variableItemsValues.value != null)
                            {
                                if (variableItemsValues.value.Equals(Constant.TRUEVALUES))
                                {
                                    variableItemsValues.value = true;
                                }
                                else if (variableItemsValues.value.Equals(Constant.FALSEVALUES))
                                {
                                    variableItemsValues.value = false;
                                }
                            }
                        }
                    }
                }
            }

            return unitConfigurationResponseObj;


        }

        /// <summary>
        /// CarFixtureChangeUnitConfigureBL
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="fixtureStrategy"></param>
        /// <param name="productType"></param>
        /// <param name="baseConfigureResponse"></param>
        /// <param name="configureRequest"></param>
        /// <param name="setId"></param>
        /// <param name="sectionTab"></param>
        /// <param name="configureResponseJObj"></param>
        /// <param name="stubUnitConfigurationMainResponseObj"></param>
        /// <returns></returns>
        public async Task<ConfigurationResponse> CarFixtureChangeUnitConfigureBL(string sessionId, List<object> fixtureStrategy, string productType, string currentProductType,
            ConfigurationResponse baseConfigureResponse, ConfigurationRequest configureRequest, int setId, string sectionTab,
            ResponseMessage configureResponseJObj, ConfigurationResponse stubUnitConfigurationMainResponseObj)
        {
            var methodBeginTime = Utility.LogBegin();
            var UnitConfigurationResponseObj = new ConfigurationResponse();
            List<Compartment> lstcompartments = new List<Compartment>();
            var constantMapper = JObject.Parse(File.ReadAllText(Constant.UNITSVARIABLESMAPPERPATH));

            var CarFixtureResponseTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.CARFIXTURETEMPLATE, Constant.EVOLUTION200)));
            var CarFixtureResponseObj = CarFixtureResponseTemplate.ToObject<ConfigurationResponse>();

            //Enriched Template
            var EnrichedTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, currentProductType)));

            if (Utility.CheckEquals(productType, Constant.ENDURA_100))
            {
                CarFixtureResponseTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.CARFIXTURETEMPLATE, Constant.END100)));
            }
            if (Utility.CheckEquals(productType, Constant.MODEL_EVO100))
            {
                CarFixtureResponseTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.CARFIXTURETEMPLATE, Constant.EVOLUTION__100)));
            }

            // Parameter For ETA/ETD not Enabled for ENDURA model
            if (!Utility.CheckEquals(productType, Constant.ENDURA_100))
            {
                if (fixtureStrategy.FirstOrDefault().Equals(Constant.ETA))
                {
                    CarFixtureResponseTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.CARFIXTURETEMPLATE, currentProductType)));
                    // setting stub data into an sectionsValues object
                    CarFixtureResponseObj = CarFixtureResponseTemplate.ToObject<ConfigurationResponse>();

                    var consolesList = constantMapper[Constant.ETAMAPPER].Select(x => x.ToString()).ToList();

                    var subSectionValues = (from section in CarFixtureResponseObj.Sections[0].sections
                                            where consolesList.Contains(section.Id)
                                            select section).ToList();
                    CarFixtureResponseObj.Sections[0].sections = subSectionValues;
                }
                else if (fixtureStrategy.FirstOrDefault().Equals(Constant.ETD) || fixtureStrategy.FirstOrDefault().Equals(Constant.ETA_AND_ETD))
                {
                    CarFixtureResponseTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.CARFIXTURETEMPLATE, currentProductType)));
                    // setting stub data into an sectionsValues object
                    CarFixtureResponseObj = CarFixtureResponseTemplate.ToObject<ConfigurationResponse>();
                }
            }
            else
            {
                CarFixtureResponseTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.CARFIXTURETEMPLATE, currentProductType)));
                // setting stub data into an sectionsValues object
                CarFixtureResponseObj = CarFixtureResponseTemplate.ToObject<ConfigurationResponse>();

                var consolesList = constantMapper[Constant.ETAMAPPER].Select(x => x.ToString()).ToList();

                var subSectionValues = (from section in CarFixtureResponseObj.Sections[0].sections
                                        where consolesList.Contains(section.Id)
                                        select section).ToList();
                CarFixtureResponseObj.Sections[0].sections = subSectionValues;
            }

            string CarFixtureCompartmentTemplate = File.ReadAllText(string.Format(Constant.CARFIXTURECOMPARTMENTPATH, currentProductType));
            var CarFixtureCompartmentResponseObj = JsonConvert.DeserializeObject<CompartmentsData>(CarFixtureCompartmentTemplate);

            var filteredBasesResponse = (from baseValue in baseConfigureResponse.Sections
                                         from stubValue in CarFixtureResponseObj.Sections
                                         where baseValue.Id.ToUpper() == stubValue.Id.ToUpper()
                                         select baseValue).ToList();

            Utility.LogTrace("change Unit configuration Response filtering using recursive method started");
            var mainFilteredRespone = Utility.MapVariables(Utility.SerializeObjectValue(baseConfigureResponse), Utility.SerializeObjectValue(CarFixtureResponseObj));
            var filteredSection = Utility.DeserializeObjectValue<ConfigurationResponse>(mainFilteredRespone);
            var filteredSections = filteredSection.Sections;
            UnitConfigurationResponseObj.Sections = filteredSections;
            var filteredVariables = new List<Variables>();
            var sectionVariablesList = filteredSections.Where(x => x.Id.Equals(sectionTab)).FirstOrDefault().sections.ToList();
            foreach (var sectionVariablesListitem in sectionVariablesList)
            {
                if (sectionVariablesListitem.Id != null)
                {
                    filteredVariables.AddRange(sectionVariablesListitem.Variables);
                }
            }
            // get the constant Variables
            //var carFixtureContantsDictionary = Utility.VariableMapper(string.Format(Constant.UNITMAPPERVARIABLESMAPPERPATH, Constant.EVOLUTION200), Constant.UNITCARFIXTUREMAPPERCONFIGURATION);

            //For device limits- showing error Message and the selected devices in carfixture screen if Total selected device slots is greater than 9
            if (Utility.CheckEquals(sectionTab, Constant.CARFIXTURE))
            {
                var VtConfigureResponse = configureResponseJObj.Response.ToObject<ConfigurationResponse>();
                foreach (var sections in VtConfigureResponse.Sections)
                {
                    foreach (var secton in sections.sections)
                    {
                        if (secton.Id.Equals(Constant.ELEVATOR_PARAMETERS_SP))
                        {
                            foreach (var variable in secton.Variables)
                            {
                                if (Utility.CheckEquals(variable.Id, Constant.TOTALDEVICESLOTSSP))
                                {
                                    foreach (var value in variable.Values)
                                    {
                                        if (value.Type.Equals(Constant.SINGLETONVALUE))
                                        {
                                            if (Convert.ToInt32(value.value) > Constant.TOTALDEVICELIMIT)
                                            {
                                                foreach (var items in stubUnitConfigurationMainResponseObj.Sections)
                                                {
                                                    if (Utility.CheckEquals(items.Id, Constant.CARFIXTURE))
                                                    {
                                                        var valueList = new List<string>();
                                                        var devicelimitobj = new deviceLimit
                                                        {
                                                            message = Constant.DEVICESLOTERRORMESSAGE
                                                        };
                                                        foreach (var assignment in configureRequest.Line.VariableAssignments)
                                                        {
                                                            if (assignment.Value.Equals(Constant.FACEPLT))
                                                            {
                                                                foreach (var varible in filteredVariables)
                                                                {
                                                                    if (assignment.VariableId.Equals(varible.Id))
                                                                    {
                                                                        foreach (var prop in varible.Properties)
                                                                        {
                                                                            if (prop.Id.Equals(Constant.DISPLAYNAME))
                                                                            {
                                                                                valueList.Add(prop.Value.ToString());
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        devicelimitobj.variables = valueList;
                                                        items.deviceLimit = devicelimitobj;
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
            }

            List<VariableAssignment> varAssigns = new List<VariableAssignment>();
            foreach (var subSectionDetails in UnitConfigurationResponseObj.Sections)
            {
                foreach (var subsection in subSectionDetails.sections)
                {
                    // get the constant Variables
                    var carFixtureContantsDictionary = Utility.GetVariableMapping(string.Format(Constant.UNITSVARIABLESMAPPERPATH, Constant.EVOLUTION200), Constant.UNITCARFIXTUREMAPPERCONFIGURATION);

                    //Adding the variables Name & value into compartments section 
                    if (subsection.Id == Constant.COPANDLOCKEDCOMPARTMENT)
                    {
                        foreach (var variables in subsection.Variables)
                        {
                            foreach (var values in variables.Values)
                            {
                                if (values.Name != Constant.TRUEVALUES && values.Name != Constant.FALSEVALUES && values.Name != Constant.True && values.Name != Constant.False)
                                {
                                    Compartment cm = new Compartment
                                    {
                                        name = values.Name.ToString(),
                                        value = values.value.ToString()
                                    };
                                    lstcompartments.Add(cm);
                                }
                            }
                        }
                        subsection.Compartments = lstcompartments.GroupBy(x => x.name).Select(c => c.First()).ToList();
                        if (!string.IsNullOrEmpty(productType) && (Utility.CheckEquals(productType, Constant.MODEL_EVO100) || Utility.CheckEquals(productType, Constant.ENDURA_100)))
                        {
                            var item = subsection.Compartments.Where(x => x.value.Equals("FACEPLT")).FirstOrDefault();

                            subsection.Compartments.Remove(item);

                            subsection.Compartments.Insert(0, item);
                        }
                        foreach (var data in CarFixtureCompartmentResponseObj.compartments)
                        {
                            if (subsection.Compartments.Count > 0)
                            {
                                subsection.Compartments.Where(c => c.name == data.id).FirstOrDefault().name = data.name;
                            }
                        }
                    }

                    ///For adding CarcallCutoutKeyswitch console
                    if (subsection.Id == Constant.CAROPERATINGPANEL)
                    {
                        if (configureRequest.Line.VariableAssignments != null && configureRequest.Line.VariableAssignments.Count() > 0)
                        {
                            foreach (var variable in configureRequest.Line.VariableAssignments)
                            {
                                if (variable.VariableId.Contains(Constant.LOCKREG) && (variable.Value.Equals(Constant.CCREG) || variable.Value.Equals(Constant.LOCKOUT)))
                                {
                                    var counts = 0;
                                    counts = _unitConfigurationDL.GetCarCallcutoutSavedOpenings(setId);
                                    subsection.sections = new List<SectionsGroupValues>();
                                    var secValues = new SectionsGroupValues
                                    {
                                        Id = Constant.ONE,
                                        Name = Constant.ELEVATORCARCALLCUTOUTKEYSWITCHESCONSOLE,
                                        Quantity = counts
                                    };
                                    var stubProperties = JArray.Parse(File.ReadAllText(Constant.PROPERTIESTEMPLATE));
                                    List<PropertyDetailsValues> varProprties = stubProperties.ToObject<List<PropertyDetailsValues>>();
                                    foreach (var varProperty in varProprties)
                                    {
                                        switch (varProperty.Id)
                                        {
                                            case Constant.SEQUENCE:
                                                varProperty.Value = Constant.ONE;
                                                break;
                                            case Constant.SECTIONNAME:
                                                varProperty.Value = Constant.CARCALLCUTOUTKEYSWITCH;
                                                break;
                                        }
                                    }
                                    secValues.Properties = varProprties;
                                    subsection.sections.Add(secValues);
                                }
                            }
                        }
                    }

                    if (Utility.CheckEquals(sectionTab, Constant.CARFIXTURE))
                    {
                        if (Utility.CheckEquals(subsection.Id, Constant.CARRIDINGLANTERNQUANTITY))
                        {
                            foreach (var variable in subsection.Variables)
                            {
                                if (Utility.CheckEquals(variable.Id, carFixtureContantsDictionary[Constant.CARRIDINGLANTERNQUANTITYSP]))
                                {
                                    if (variable.Values != null)
                                    {
                                        var valuelist = (from value in variable.Values
                                                         where (value.State.Equals(Constant.AVAILABLE) || value.State.Equals(Constant.UNAVAILABLE) || value.State.Equals(Constant.SELECTED))
                                                         select value).ToList();
                                        variable.Values = valuelist;
                                    }
                                }
                            }
                        }
                    }

                    foreach (var subsectionvariables in subsection.Variables)
                    {
                        //Condition for changing string to boolean
                        if (subsectionvariables.Values != null)
                        {
                            foreach (var variableItemsValues in subsectionvariables.Values)
                            {

                                if (variableItemsValues.value != null)
                                {
                                    if (variableItemsValues.value.Equals(Constant.TRUEVALUES))
                                    {
                                        variableItemsValues.value = true;
                                    }
                                    else if (variableItemsValues.value.Equals(Constant.FALSEVALUES))
                                    {
                                        variableItemsValues.value = false;
                                    }
                                }
                            }
                        }
                    }

                }
            }
            return UnitConfigurationResponseObj;
        }

        /// <summary>
        /// StartUnitHallFixtures
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="sectionTab"></param>
        /// <param name="fixtureStrategy"></param>
        /// <param name="productType"></param>
        /// <param name="variableAssignments"></param>
        /// <param name="lstunits"></param>
        /// <param name="unitHallFixtureConsoles"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> StartUnitHallFixtures(string sessionId, string sectionTab, string fixtureStrategy, string productType, int setId,
              JObject variableAssignments = null, List<UnitNames> lstunits = null, List<UnitHallFixtures> unitHallFixtureConsoles = null)
        {
            var methodBeginTime = Utility.LogBegin();
            var constantMapper = JObject.Parse(File.ReadAllText(Constant.UNITSVARIABLESMAPPERPATH));
            var configureRequest = CreateConfigurationRequestWithTemplate(variableAssignments, Constant.UNITNAME, null, productType);
            var mainGroupConfigurationResponse = new ConfigurationResponse
            {
                Sections = new List<Sections>()
            };
            var packagePath = configureRequest?.PackagePath;
            var modifiedOn = new DateTime();
            var currentConfigurationCache = _cpqCacheManager.GetCache(sessionId, _environment,
            Constant.CURRENTMACHINECONFIGURATION);
            if (configureRequest != null && (string.IsNullOrEmpty(packagePath)))
            {
                if (String.IsNullOrEmpty(currentConfigurationCache))
                {
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = _localizer[Constant.SESSIONIDPARENTCODEISNOMOREVALID]
                    });
                }
                var currentConfiguration = Utility.DeserializeObjectValue<StartConfigureResponse>(currentConfigurationCache);
                if (currentConfiguration.Audits.ModifiedOn == null)
                {
                    currentConfiguration.Audits.ModifiedOn = new DateTime();
                }
                if (currentConfiguration != null && modifiedOn != default(DateTime) &&
                    DateTime.Compare(modifiedOn, (DateTime)currentConfiguration.Audits.ModifiedOn) > 0)
                {
                    currentConfiguration.Audits.ModifiedOn = modifiedOn;
                }
                var value = Utility.SerializeObjectValue(currentConfiguration);
                _cpqCacheManager.SetCache(sessionId, _environment, Constant.CURRENTMACHINECONFIGURATION, value);
                //Response for UI
                var updatedStartConfigureResponse = ViewModelResponseMapper(currentConfiguration);
                var updatedCurrentConfiguration = Utility.SerializeObjectValue(updatedStartConfigureResponse);
                return new ResponseMessage
                {
                    Response = JObject.Parse(updatedCurrentConfiguration),
                    StatusCode = Constant.SUCCESS
                };
            }
            //Gets the base configuration of the model
            var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest);
            //Adding include sections
            baseConfigureRequest = GenerateIncludeSections(baseConfigureRequest, Constant.UNITCONFIGURATION, sectionTab, productType);
            var varAssignments = (List<VariableAssignment>)baseConfigureRequest.Line.VariableAssignments;
            baseConfigureRequest.Line.VariableAssignments = varAssignments;
            var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, packagePath, sessionId).ConfigureAwait(false);
            var baseConfigureResponse = baseConfigureResponseJObj.Response.ToObject<ConfigurationResponse>();
            // configuration object values for conflict mapping
            var configureResponse = baseConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = configureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            // adding defaults to cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTUNITCONFIGVALUES, Utility.SerializeObjectValue(configureResponseArgumentJObject));
            var configureRequestDictionary = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();
            var stubUnitConfigurationResponseObj = new ConfigurationResponse();
            var stubUnitConfigurationSubSectionResponseObj = new ConfigurationResponse();
            var stubUnitConfigurationMainResponseObj = new ConfigurationResponse();

            var stubMainSubSectionResponse = JObject.Parse(File.ReadAllText(Constant.UNITMAINRESPONSE));
            stubUnitConfigurationMainResponseObj = stubMainSubSectionResponse.ToObject<ConfigurationResponse>();
            var unitHallFixtureUIResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITHALLFIXTUREPATH, Constant.EVOLUTION200)));
            var unitHallFixtureConsolesResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITHALLFIXTURECONSOLESPATH, Constant.EVOLUTION200)));
            var unitHallFixtureConsoleSectionResponse = unitHallFixtureConsolesResponse.ToObject<ConfigurationResponse>();
            if (fixtureStrategy.Equals(Constant.ETA))
            {
                var consolesList = constantMapper[Constant.ETACONSOLES].Select(x => x.ToString()).ToList();
                unitHallFixtureConsoleSectionResponse.Sections = (from section in unitHallFixtureConsoleSectionResponse.Sections
                                                                  where consolesList.Contains(section.Id)
                                                                  select section).ToList();
            }
            else if (fixtureStrategy.Equals(Constant.ETD) || fixtureStrategy.Equals(Constant.ETA_AND_ETD))
            {
                var consolesList = constantMapper[Constant.ETDORETAETDCONSOLES].Select(x => x.ToString()).ToList();
                unitHallFixtureConsoleSectionResponse.Sections = (from section in unitHallFixtureConsoleSectionResponse.Sections
                                                                  where consolesList.Contains(section.Id)
                                                                  select section).ToList();
            }
            //Getting CarRiding Lantern Value From DB to Cache
            var CarRidingLanternQuantity = _cpqCacheManager.GetCache(sessionId, _environment, setId.ToString(), Constants.CARRIDINGLANTERNQUANT);

            if (productType.Equals(Constant.ENDURA_100))
            {
                unitHallFixtureConsolesResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITHALLFIXTURECONSOLESPATH, Constant.END100)));
                unitHallFixtureUIResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITHALLFIXTUREPATH, Constant.END100)));
                if (CarRidingLanternQuantity != "0")
                {
                    unitHallFixtureConsolesResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITHALLFIXTURECONSOLESFORQUANTITYPATH, Constant.END100)));
                }

                unitHallFixtureConsoleSectionResponse = unitHallFixtureConsolesResponse.ToObject<ConfigurationResponse>();
                if (fixtureStrategy.Equals(Constant.ETA))
                {
                    var consolesList = constantMapper[Constant.ETACONSOLES].Select(x => x.ToString()).ToList();
                    unitHallFixtureConsoleSectionResponse.Sections = (from section in unitHallFixtureConsoleSectionResponse.Sections
                                                                      where consolesList.Contains(section.Id)
                                                                      select section).ToList();
                }
                else if (fixtureStrategy.Equals(Constant.ETD) || fixtureStrategy.Equals(Constant.ETA_AND_ETD))
                {
                    var consolesList = constantMapper[Constant.ETDORETAETDCONSOLES].Select(x => x.ToString()).ToList();
                    unitHallFixtureConsoleSectionResponse.Sections = (from section in unitHallFixtureConsoleSectionResponse.Sections
                                                                      where consolesList.Contains(section.Id)
                                                                      select section).ToList();
                }
            }

            
            if (productType.Equals(Constant.MODEL_EVO100))
            {
                unitHallFixtureConsolesResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITHALLFIXTURECONSOLESPATH, Constant.EVOLUTION__100)));
                unitHallFixtureUIResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITHALLFIXTUREPATH, Constant.EVOLUTION__100)));
                if (CarRidingLanternQuantity != "0")
                {
                    unitHallFixtureConsolesResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITHALLFIXTURECONSOLESFORQUANTITYPATH, Constant.EVOLUTION__100)));
                }

                unitHallFixtureConsoleSectionResponse = unitHallFixtureConsolesResponse.ToObject<ConfigurationResponse>();
                if (fixtureStrategy.Equals(Constant.ETA))
                {
                    var consolesList = constantMapper[Constant.ETACONSOLES].Select(x => x.ToString()).ToList();
                    unitHallFixtureConsoleSectionResponse.Sections = (from section in unitHallFixtureConsoleSectionResponse.Sections
                                                                      where consolesList.Contains(section.Id)
                                                                      select section).ToList();
                }
            }
            // setting stub data into an sectionsValues object
            stubUnitConfigurationResponseObj = unitHallFixtureUIResponse.ToObject<ConfigurationResponse>();
            //Sub Sections Stub Path
            stubUnitConfigurationSubSectionResponseObj = unitHallFixtureConsoleSectionResponse;
            var stubUnitConfigurationSubSectionResponseSample = new ConfigurationResponse
            {
                Sections = new List<Sections>()
            };
            var fixtureTypesList = _unitConfigurationDL.GenerateUnitHallFixturesList(fixtureStrategy);
            fixtureTypesList = fixtureTypesList.ConvertAll(d => d.ToUpper());
            List<string> fixturesStored = new List<string>();
            foreach (var varUnitHallFixtures in unitHallFixtureConsoles)
            {
                if (varUnitHallFixtures.ConsoleId != 0)
                {
                    fixturesStored.Add(varUnitHallFixtures.UnitHallFixtureType.ToUpper());
                }
            }
            foreach (var items in stubUnitConfigurationSubSectionResponseObj.Sections)
            {
                if (fixturesStored.Contains(items.Id.ToUpper()))
                {
                    if (fixtureTypesList.Contains(items.Id.ToUpper()))
                    {
                        stubUnitConfigurationSubSectionResponseSample.Sections.Add(items);
                    }
                }
            }
            List<String> OrderOfConsoles = new List<String>();
            foreach (var console in unitHallFixtureConsoles)
            {
                if (OrderOfConsoles.Contains(console.UnitHallFixtureType))
                {
                    continue;
                }
                else
                {
                    OrderOfConsoles.Add(console.UnitHallFixtureType);
                }
            }
            foreach (var subsection in stubUnitConfigurationSubSectionResponseSample.Sections)
            {
                if (unitHallFixtureConsoles.Count > 0)
                {
                    var currentSectionName = subsection.Id;
                    subsection.sections = new List<SectionsValues>();
                    foreach (var varUnitHallFixtures in unitHallFixtureConsoles)
                    {
                        if (varUnitHallFixtures.ConsoleId > 0)
                        {
                            if (currentSectionName.ToUpper() == varUnitHallFixtures.UnitHallFixtureType.ToUpper())
                            {
                                if (varUnitHallFixtures.ConsoleId > 0)
                                {
                                    var frontRearAssignment = Utility.GetLandingOpeningAssignmentSelectedForUnitHallFixture(varUnitHallFixtures.UnitHallFixtureLocations);
                                    if (String.IsNullOrEmpty(frontRearAssignment))
                                    {
                                        if (varUnitHallFixtures.Openings.Rear.Equals(false))
                                        {
                                            frontRearAssignment = Constant.ZEROFRONTOPENINGS;
                                        }
                                        else
                                        {
                                            frontRearAssignment = Constant.ZEROOPENINGS;
                                        }
                                    }
                                    var consoleSection = new SectionsValues();
                                    consoleSection = Utility.DeserializeObjectValue<SectionsValues>(Utility.SerializeObjectValue(subsection));
                                    consoleSection.sections = new List<SectionsGroupValues>();
                                    consoleSection.Id = varUnitHallFixtures.ConsoleId.ToString();
                                    consoleSection.Name = subsection.Id + Constant.DOT + varUnitHallFixtures.ConsoleName;
                                    consoleSection.OpeningRange = frontRearAssignment;
                                    consoleSection.assignOpenings = varUnitHallFixtures.AssignOpenings;
                                    consoleSection.IsDelete = !varUnitHallFixtures.IsController;
                                    consoleSection.fixtureType = varUnitHallFixtures.UnitHallFixtureType;
                                    var newflag = (from val in varUnitHallFixtures.VariableAssignments
                                                   where val.Value.Equals(string.Empty)
                                                   select val).ToList();
                                    var flagnew = 0;
                                    if (varUnitHallFixtures.VariableAssignments.Count() != newflag.Count())
                                    {
                                        flagnew = 1;
                                    }
                                    if (varUnitHallFixtures.VariableAssignments.Count >= 1 && flagnew == 1)
                                    {
                                        foreach (var consoleVariables in consoleSection.Variables)
                                        {
                                            var variableValue = (from variable in varUnitHallFixtures.VariableAssignments
                                                                 where Utility.CheckEquals(variable.VariableId, consoleVariables.Id)
                                                                 select variable.Value).FirstOrDefault();
                                            consoleVariables.Value = variableValue;
                                        }
                                    }
                                    foreach (var consoleVariable in consoleSection.Variables)
                                    {
                                        if (Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(constantMapper[Constant.SWITCHVARIABLESTOBEREMOVED])).Contains(consoleVariable.Id))
                                        {
                                            consoleSection.Variables = consoleSection.Variables.Where(x => !x.Id.Equals(consoleVariable.Id)).ToList();
                                        }
                                    }
                                    foreach (var varProperty in consoleSection.Properties)
                                    {
                                        switch (varProperty.Id)
                                        {
                                            case Constant.SEQUENCE:
                                                varProperty.Value = varUnitHallFixtures.ConsoleId;
                                                break;
                                            case Constant.SECTIONNAME:
                                                varProperty.Value = varUnitHallFixtures.ConsoleName;
                                                break;
                                        }
                                    }
                                    subsection.sections.Add(consoleSection);
                                }
                            }
                        }
                    }
                }
                subsection.Properties[0].Value = OrderOfConsoles.IndexOf(subsection.Id);
                subsection.Variables = new List<Variables>();
            }
            foreach (var subsection in stubUnitConfigurationResponseObj.Sections)
            {
                if (Utility.CheckEquals(subsection.Id, Constant.ADDFIXTURES))
                {
                    foreach (var item in subsection.Variables)
                    {
                        List<Values> listOfFixtures = new List<Values>();
                        foreach (var diffValues in item.Values)
                        {
                            if (fixtureTypesList.Contains(diffValues.id.ToUpper()))
                            {
                                listOfFixtures.Add(diffValues);
                            }
                        }
                        item.Values = listOfFixtures;
                    }
                }
                if (Utility.CheckEquals(subsection.Id, Constant.CONSOLES))
                {
                    subsection.sections = Utility.DeserializeObjectValue<IList<SectionsValues>>(Utility.SerializeObjectValue(stubUnitConfigurationSubSectionResponseSample.Sections));
                }
            }
            mainGroupConfigurationResponse.Sections = stubUnitConfigurationResponseObj.Sections;


            foreach (var items in stubUnitConfigurationMainResponseObj.Sections)
            {
                if (items.Id.ToUpper() == sectionTab.ToString().ToUpper())
                {
                    var mainSectionValues = Utility.DeserializeObjectValue<JArray>(Utility.SerializeObjectValue(mainGroupConfigurationResponse.Sections));
                    items.sections = Utility.DeserializeObjectValue<IList<SectionsValues>>(Utility.SerializeObjectValue(mainSectionValues));

                    //adding Lobby Recall Switch configured flag in the main response
                    var lobbyFlag = _cpqCacheManager.GetCache(sessionId, _environment, setId.ToString(), Constant.LOBBYRECALLSWITCHFLAG);
                    if (String.IsNullOrEmpty(lobbyFlag))
                    {
                        items.LobbyRecallSwitchConfigured = false;
                    }
                    else
                    {
                        var cachedVariable = Utility.DeserializeObjectValue<JObject>(lobbyFlag);
                        if (String.IsNullOrEmpty(cachedVariable[Constants.VALUE].ToString()))
                        {
                            items.LobbyRecallSwitchConfigured = false;
                        }
                        else
                        {
                            var lobbyFlagValue = bool.Parse(cachedVariable[Constants.VALUE].ToString());
                            items.LobbyRecallSwitchConfigured = lobbyFlagValue;
                        }

                    }

                    //adding CarRidingLanternQuantityflag in the main Response
                    var CarRidingQuantFlag = _cpqCacheManager.GetCache(sessionId, _environment, setId.ToString(), Constants.CARRIDINGLANTERNQUANT);
                    if (String.IsNullOrEmpty(CarRidingQuantFlag))
                    {
                        items.CarRidingQuantityFlag = false;
                    }
                    else
                    {
                        if ((productType.Equals(Constant.MODEL_EVO100) || productType.Equals(Constant.ENDURA_100)) && CarRidingQuantFlag != "0")
                        {
                            items.CarRidingQuantityFlag = true;
                        }
                        else
                        {
                            items.CarRidingQuantityFlag = false;
                        }

                    }
                    break;

                }
            }
            stubUnitConfigurationMainResponseObj.Units = lstunits.Distinct().ToList();

            // added enriched data in the main response 
            var enrichedData = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.EVOLUTION200)));
            if (productType.Equals(Constant.ENDURA_100))
            {
                enrichedData = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.END100)));
            }
            if (productType.Equals(Constant.MODEL_EVO100))
            {
                enrichedData = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.EVOLUTION__100)));
            }
            stubUnitConfigurationMainResponseObj.EnrichedData = enrichedData;
            // added conflicts 
            var conflictChangesValues = CompareChangeInConfigResponse(baseConfigureResponse, Constant.UNITCONFIGURATION, configureRequestDictionary);
            stubUnitConfigurationMainResponseObj.ConflictAssignments = conflictChangesValues;
            var getData = (from val1 in configureRequestDictionary
                           where (val1.Key.ToString().Contains("CAPACITY") || val1.Key.ToString().Contains("CARSPEED"))
                           select new VariableAssignment
                           {
                               VariableId = val1.Key,
                               Value = val1.Value
                           }).Distinct().ToList();
            foreach (var a in getData)
            {
                if (a.VariableId.Contains("CAPACITY_INT"))
                {
                    getData = getData.Where(x => !x.VariableId.Contains("CAPACITY_INT")).ToList();
                }
            }
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.PREVIOUSUNITCONFLICTSVALUES, Utility.SerializeObjectValue(conflictChangesValues));
            var crossPackagevariableAssignments = new List<VariableAssignment>();
            crossPackagevariableAssignments = baseConfigureRequest.Line.VariableAssignments.ToList();
            crossPackagevariableAssignments.AddRange(getData);
            crossPackagevariableAssignments.Distinct();
            SetCrosspackageVariableAssignments(crossPackagevariableAssignments, sessionId,string.Concat(Constant.UNITCONFIGURATION,setId));
            if (configureRequest != null)
            {
                return new ResponseMessage
                {
                    StatusCode = Constant.SUCCESS,
                    Response = Utility.FilterNullValues(stubUnitConfigurationMainResponseObj)
                };
            }
            Utility.LogEnd(methodBeginTime);
            throw new CustomException(new ResponseMessage
            {
                StatusCode = Constant.BADREQUEST,
                Message = _localizer[Constant.REQUESTCANNOTBENULL]
            });
        }

        public void GetConsolesForFixtureStrategy(string fixtureStrategy, JObject constantMapper, ref ConfigurationResponse configurationResponse)
        {
            var consolesList = constantMapper[fixtureStrategy].Select(x => x.ToString()).ToList();
            configurationResponse.Sections = (from section in configurationResponse.Sections
                                              where consolesList.Contains(section.Id)
                                              select section).ToList();
        }



        private List<VariableAssignment> SetGroupCrossPackage(string sessionId, JObject variableAssignments, IDictionary<string, string> groupContantsDictionary)
        {
            var crossPackagevariableAssignments = new List<VariableAssignment>();
            var getCrossPackageValues = GetCrosspackageVariableAssignments(sessionId, Constant.GROUPCONFIGURATION);
            if (!string.IsNullOrEmpty(getCrossPackageValues))
            {
                crossPackagevariableAssignments = Utility.DeserializeObjectValue<List<VariableAssignment>>(getCrossPackageValues);
            }
            var configAssignments = Utility.DeserializeObjectValue<Line>(Utility.SerializeObjectValue(variableAssignments)).VariableAssignments;
            foreach (var varAssign in configAssignments)
            {
                if (varAssign.VariableId.EndsWith(groupContantsDictionary[Constant.ACROSSTHEHALLDISTANCEPARAMETER]) || varAssign.VariableId.EndsWith(groupContantsDictionary[Constant.BANKOFFSETPARAMETER]))
                {
                    varAssign.Value = (Convert.ToDecimal(varAssign.Value) * 12);
                }
            }

            var assignments = new Line() { VariableAssignments = configAssignments };
            //generate cross package variable assignments
            crossPackagevariableAssignments = GenerateVariableAssignmentsForUnitConfiguration(crossPackagevariableAssignments, assignments);

            SetCrosspackageVariableAssignments(crossPackagevariableAssignments, sessionId, Constant.GROUPCONFIGURATION);
            return crossPackagevariableAssignments;
        }

        private ConfigurationRequest GenerateDefaultGroupRequest(List<VariableAssignment> crossPackagevariableAssignments, string selectedTab)
        {
            var variableAssignmentz = new Line
            {
                VariableAssignments = crossPackagevariableAssignments
            };

            var variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(variableAssignmentz));


            var configureRequest = CreateConfigurationRequestWithTemplate(variableAssignments, Constant.GROUPCONFIGURATIONNAME);

            // to update elevator positions to elevator names
            var elevatorUpdatePositionsValues = GetElevatorMappingValues(configureRequest);
            // mapping Update Elevator Values
            configureRequest.Line.VariableAssignments = elevatorUpdatePositionsValues.Line.VariableAssignments;

            // needed code pls don't remove it
            if (configureRequest == null)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = _localizer[Constant.REQUESTCANNOTBENULL + Constant.HYPHEN]
                });
            }
            //Adding include sections in request body
            configureRequest = GenerateIncludeSections(configureRequest, selectedTab);
            return configureRequest;
        }

        private JObject GenerateGroupConfigurationPopUpResponse(JObject variableAssignments, IDictionary<string, string> groupContantsDictionary)
        {
            var stubGroupCnfgnResponse = JObject.Parse(File.ReadAllText(Constant.ELEVATORPOPUPUITEMPLATE));
            // setting stub data into an sectionsValues object
            var stubGroupCnfgnResponseObj = stubGroupCnfgnResponse.ToObject<ConfigurationResponse>();


            var configureRequestValues = Utility.DeserializeObjectValue<Line>(Utility.SerializeObjectValue(variableAssignments)).VariableAssignments;

            if (configureRequestValues != null && configureRequestValues.Any())
            {
                foreach (var mainSections in stubGroupCnfgnResponseObj.Sections)
                {
                    foreach (var variables in mainSections.Variables)
                    {
                        foreach (var assignment in configureRequestValues)
                        {
                            if (variables.Id.Equals(groupContantsDictionary[Constant.GROUPDESIGNATION]))
                            {
                                if (variables.Id.Equals(assignment.VariableId))
                                {
                                    foreach (var value in variables.Values)
                                    {
                                        if (!String.IsNullOrEmpty(value.Type) && !value.Type.Equals(Constant.INTERVALVALUE))
                                        {
                                            value.Name = Convert.ToString(assignment.Value);
                                            value.value = Convert.ToString(assignment.Value);
                                            value.Assigned = Constant.BYUSER_LOWERCASE;
                                        }
                                    }
                                }
                            }
                        }
                    }


                }
            }
            return Utility.FilterNullValues(stubGroupCnfgnResponseObj);
        }

        private void GenerateVariablesForControlRoom(ConfigurationRequest configureRequest, List<VariableAssignment> controlLocationValue,
            IDictionary<string, string> groupContantsDictionary, Sections item, List<BuildingElevationData> lstFloorDesignation)
        {
            if (controlLocationValue.Count.Equals(0))
            {
                controlLocationValue.Add(new VariableAssignment
                {
                    VariableId = groupContantsDictionary[Constant.CONTROLLERLOCATION_SP],
                    Value = item.sections.Where(x => x.Id.Equals(Constant.CONTROLLOCATIONID)).FirstOrDefault().
                    Variables.Where(x => x.Id.Contains(groupContantsDictionary[Constant.CONTROLLERLOCATION_SP])).FirstOrDefault().Values.Where(x => x.Assigned != null).
                    FirstOrDefault().value
                });
            }


            var lstAssignedFloorNumber = configureRequest.Line.VariableAssignments.Where(assignment => assignment.VariableId.ToString().Contains(groupContantsDictionary[Constant.CONTROLFLOOR])).ToList();
            if (controlLocationValue.Any())
            {
                if (Utility.CheckEquals(controlLocationValue[0].Value.ToString(), Constant.CONTROLLOCATIONJAMBMOUNTED) ||
                    Utility.CheckEquals(controlLocationValue[0].Value.ToString(), Constant.CONTROLLOCATIONOVERHEAD))
                {
                    var removedControlFloor = item.sections.Where(x => x.Id.Equals(Constant.CONTROLLOCATIONID)).FirstOrDefault().Variables.Where(x => x.Id.Contains(groupContantsDictionary[Constant.PARAMETERSSPCONTROLFLOOR])).FirstOrDefault();
                    foreach (var sectionVariables in item.sections)
                    {
                        sectionVariables.Variables.Remove(removedControlFloor);
                    }
                    if (Utility.CheckEquals(controlLocationValue[0].Value.ToString(), Constant.CONTROLLOCATIONOVERHEAD) ||
                        Utility.CheckEquals(controlLocationValue[0].Value.ToString(), Constant.CONTROLLOCATIONREMOTE))
                    {
                        foreach (var sectionVariables in item.sections)
                        {
                            foreach (var itemVariables in sectionVariables.Variables)
                            {
                                if (itemVariables.Id.Equals(groupContantsDictionary[Constant.PARAMETERSNXVMDISTANCEFLOOR]))
                                {
                                    var minValuesString = (from prop in itemVariables.Properties
                                                           where prop.Id.Contains(Constant.MINVALUESMALLCASE)
                                                           select prop.Value).ToList().FirstOrDefault();
                                    var maxValuesString = (from prop in itemVariables.Properties
                                                           where prop.Id.Contains(Constant.MAXVALUESMALLCASE)
                                                           select prop.Value).ToList().FirstOrDefault();
                                    var minValues = Convert.ToInt32(minValuesString) / 12;
                                    var maxValues = Convert.ToInt32(maxValuesString) / 12;
                                    //foreach (var prop in itemVariables.Properties)
                                    //{
                                    //    if (prop.Id.Contains(Constant.MINVALUESMALLCASE) || prop.Id.Contains(Constant.MAXVALUESMALLCASE))
                                    //    {
                                    //        prop.Value = (Convert.ToInt32(prop.Value) / 12).ToString();
                                    //    }
                                    //}
                                    var constanval = Constant.GENERALINFORMATIONMESSAGE;
                                    itemVariables.Properties.Add(new Properties { Id = Constant.RANGEVALIDATION, Value = string.Format(constanval, minValues.ToString().Split('.')[0], '0', maxValues.ToString().Split('.')[0], '0') });

                                }
                            }
                        }
                    }

                }
                else
                {
                    foreach (var sectionVariables in item.sections)
                    {
                        foreach (var itemVariables in sectionVariables.Variables)
                        {
                            if (itemVariables.Id.Equals(groupContantsDictionary[Constant.PARMETERSXDIMENSIONVALUES]) ||
                                itemVariables.Id.Equals(groupContantsDictionary[Constant.PARAMETERSYDIMENSIONVALUES]))
                            {
                                var minValuesString = (from prop in itemVariables.Properties
                                                       where prop.Id.Contains(Constant.MINVALUESMALLCASE)
                                                       select prop.Value).ToList().FirstOrDefault();
                                var maxValuesString = (from prop in itemVariables.Properties
                                                       where prop.Id.Contains(Constant.MAXVALUESMALLCASE)
                                                       select prop.Value).ToList().FirstOrDefault();
                                var minValues = Convert.ToInt32(minValuesString) / 12;
                                var maxValues = Convert.ToInt32(maxValuesString) / 12;
                                foreach (var prop in itemVariables.Properties)
                                {
                                    if (prop.Id.Contains(Constant.MINVALUESMALLCASE) || prop.Id.Contains(Constant.MAXVALUESMALLCASE))
                                    {
                                        prop.Value = (Convert.ToInt32(prop.Value) / 12).ToString();
                                    }
                                }
                                var constanval = Constant.GENERALINFORMATIONMESSAGE;
                                itemVariables.Properties.Add(new Properties { Id = Constant.RANGEVALIDATION, Value = string.Format(constanval, minValues.ToString().Split('.')[0], '0', maxValues.ToString().Split('.')[0], '0') });

                            }
                        }
                    }

                }
                //Logic to tag floor designation and floor number
                foreach (var sectionVariables in item.sections)
                {
                    foreach (var itemVariables in sectionVariables.Variables)
                    {
                        if (itemVariables.Id.Contains(groupContantsDictionary[Constant.PARAMETERSSPCONTROLFLOOR]))
                        {
                            var lstValues = new List<Values>();
                            var maxFloorNumber = (from floors in lstFloorDesignation
                                                  orderby floors.FloorNumber
                                                  select floors).ToList().Last();
                            foreach (var itemValues in lstFloorDesignation)
                            {
                                Values values = new Values
                                {
                                    Name = itemValues.FloorNumber.ToString(),
                                    value = itemValues.FloorNumber,
                                    InCompatible = false,
                                    Justification = itemVariables.Values[0].Justification,
                                    Type = itemVariables.Values[0].Type
                                };
                                if (lstAssignedFloorNumber.Count > 0)
                                {
                                    if (itemValues.FloorNumber == Convert.ToInt32(lstAssignedFloorNumber[0].Value))// || Utility.CheckEquals(itemValues.FloorDesignation, lstAssignedFloorNumber[0].Value.ToString()))
                                    {
                                        values.Assigned = Constant.BYUSER_CAMELCASE;
                                        values.State = Constant.SELECTED_CAMELCASE;
                                    }
                                }
                                else if (itemValues.FloorNumber == Convert.ToInt32(maxFloorNumber.FloorNumber))
                                {
                                    values.Assigned = Constant.BYUSER_CAMELCASE;
                                    values.State = Constant.SELECTED_CAMELCASE;
                                }
                                lstValues.Add(values);
                            }
                            if (lstValues.Count > 0)
                            {
                                itemVariables.Values = lstValues;
                            }
                        }
                    }
                }

            }
            else if (controlLocationValue.Count == 0)
            {
                var removedControlFloor = item.sections.Where(x => x.Id.Equals(Constant.CONTROLLOCATIONID)).FirstOrDefault().Variables.Where(x => x.Id.Contains(groupContantsDictionary[Constant.PARAMETERSSPCONTROLFLOOR])).FirstOrDefault();
                foreach (var sectionVariables in item.sections)
                {
                    sectionVariables.Variables.Remove(removedControlFloor);
                }
            }
        }

        private void GenerateElevatorSectionsForGroup(SectionsValues topItem, List<SectionsValues> mainValueData, Dictionary<string, string> unitMappingResponse, List<UnitMappingValues> unitMappingListValues)
        {
            var varMatchedVariables = topItem.Variables.Where(varValues => varValues.Id.Contains(Constant.PARAMETERSREAROPEN)).ToList();
            if (varMatchedVariables.Count > 0)
            {
                var varValuesWithNameValues = varMatchedVariables[0].Values.Where(valuesName => valuesName.Name != null).ToList();
                foreach (var varitems in varValuesWithNameValues)
                {
                    if (!string.IsNullOrEmpty(varitems.Name) && !string.IsNullOrEmpty(varitems.Assigned))
                    {
                        if (Utility.CheckEquals(varitems.Assigned, Constant.BYUSER_CAMELCASE) && Utility.CheckEquals(varitems.value.ToString().ToUpper(), Constant.FALSEVALUES))
                        {
                            //removing ReardoorType
                            var variableId = topItem.Id.ToString() + Constant.PARAMETERSREARDOOR;
                            var varVariableitems = (from varItems in topItem.Variables
                                                    where varItems.Id != variableId
                                                    select varItems).ToList();
                            topItem.Variables = varVariableitems;
                            //Removing Hallstation
                            variableId = topItem.Id.ToString() + Constant.PARAMETERSHALLRISER;
                            varVariableitems = (from varItems in topItem.Variables
                                                where varItems.Id != variableId
                                                select varItems).ToList();
                            topItem.Variables = varVariableitems;
                        }
                    }
                }
            }

            // getting names for that assigned carpos values
            var gettingMatchedVariableIds = topItem.Variables.Where(varibaleValues => varibaleValues.Id.Contains(Constant.PARAMETERSCARPOS)).ToList();
            if (gettingMatchedVariableIds.Count > 0)
            {
                var variableValuesWithNameValues = gettingMatchedVariableIds[0].Values != null ? gettingMatchedVariableIds[0].Values.Where(valuesName => valuesName != null).ToList() : new List<Values>();
                if (variableValuesWithNameValues.Any())
                {
                    //adding required elevators to the mainvaluedata attribute
                    mainValueData.Add(topItem);
                }
                // GETTING UNIT TABLE ALL VALUES
                foreach (var VariableNameValues in variableValuesWithNameValues)
                {
                    if (!string.IsNullOrEmpty(VariableNameValues.Name) && !string.IsNullOrEmpty(VariableNameValues.Assigned))
                    {
                        if (Utility.CheckEquals(VariableNameValues.Assigned, Constant.BYUSER_CAMELCASE) || Utility.CheckEquals(VariableNameValues.Assigned, Constant.BYRULE_CAMELCASE) || Utility.CheckEquals(VariableNameValues.Assigned, Constant.BYDEFAULT_CAMELCASE))
                        {
                            // get that dictionary values 
                            // get values for that 
                            var mappingUnitValues = unitMappingResponse.Where(unitValue => unitValue.Key.Equals(VariableNameValues.Name)).Select(unitValue => unitValue.Value).FirstOrDefault();
                            var newUnitMapValues = new UnitMappingValues()
                            {
                                ElevatorName = VariableNameValues.Name,
                                UnitId = mappingUnitValues
                            };
                            if (newUnitMapValues != null)
                            {
                                unitMappingListValues.Add(newUnitMapValues);
                            }
                        }
                    }
                }
            }
        }

        private void FormattingValuesForGroupResponse(Sections itemMainGroupValues, IDictionary<string, string> groupContantsDictionary, string selectedTab)
        {
            foreach (var sectionOfLayout in itemMainGroupValues.sections)
            {
                // looping the filter values to assign property values from stub
                foreach (var variableItems in sectionOfLayout.Variables)
                {
                    var toBeFormattedVariables = new List<string>()
                    {
                        groupContantsDictionary[Constant.PARAMETERSYDIMENSIONVALUES],
                        groupContantsDictionary[Constant.PARMETERSXDIMENSIONVALUES]
                    };

                    if (toBeFormattedVariables.Any(_ => variableItems.Id.EndsWith(_)))
                    {
                        foreach (var variableValue in variableItems.Values)
                        {
                            if (variableValue.Assigned != null)
                            {
                                variableValue.value = (Convert.ToDecimal(variableValue.value) / 12).ToString();
                            }
                        }
                    }
                    var minValues = string.Empty;
                    var maxValues = string.Empty;
                    if (variableItems.Id.EndsWith(groupContantsDictionary[Constant.ACROSSTHEHALLDISTANCEPARAMETER]))
                    {
                        foreach (var property in variableItems.Properties)
                        {
                            if (Utility.CheckEquals(property.Id, Constant.MINVALUE) & property.Value != null)
                            {
                                minValues = (Convert.ToDecimal(property.Value) / 12).ToString();
                                //property.Value = minValues;

                            }
                            if (Utility.CheckEquals(property.Id, Constant.MAXVALUE) & property.Value != null)
                            {
                                maxValues = (Convert.ToDecimal(property.Value) / 12).ToString();
                                //property.Value = maxValues;
                            }
                        }
                        var constanval = Constant.GENERALINFORMATIONMESSAGE;
                        variableItems.Properties.Add(new Properties { Id = Constant.RANGEVALIDATION, Value = string.Format(constanval, minValues.ToString().Split('.')[0], '0', maxValues.ToString().Split('.')[0], '0') });
                    }
                    //Condition for changing string to boolean
                    if (variableItems.Id.ToString().Contains(Constant.PARAMETERSVALUES, StringComparison.OrdinalIgnoreCase) && Utility.CheckEquals(selectedTab, Constant.GROUPLAYOUTCONFIGURATION))
                    {
                        foreach (var variableItemsValues in variableItems.Values)
                        {
                            if (variableItemsValues.value != null)
                            {
                                if (variableItemsValues.value.Equals(Constant.TRUEVALUES))
                                {
                                    variableItemsValues.value = true;
                                }
                                else if (variableItemsValues.value.Equals(Constant.FALSEVALUES))
                                {
                                    variableItemsValues.value = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void GenerateConfigurationRequestForDoors(ConfigurationRequest configureRequest)
        {
            var doorsMaterialVariant = JObject.Parse(JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES))[Constant.DOORSMATERIALVARIANT].ToString());
            configureRequest.Line.ProductId = "Group_Config_Doors_UI";
            configureRequest.Line.Product.Id = "Group_Config_Doors_UI";
            configureRequest.GlobalArguments["materialVariant"] = doorsMaterialVariant;
        }
        /// <summary>
        /// OtherEquipmentCall
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public ConfigurationResponse OtherEquipmentUnitConfigureBL(string productType, ConfigurationResponse baseConfigureResponse)
        {
            var stubUnitConfigurationResponseObj = new ConfigurationResponse();
            var stubHoistwayTractionResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.OTHEREQUIPMENTUIRESPONSETEMPLATE, Constant.EVOLUTION200)));
            if (productType.Equals(Constant.ENDURA_100))
            {
                stubHoistwayTractionResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.OTHEREQUIPMENTUIRESPONSETEMPLATE, Constant.END100)));
            }
            if (productType.Equals(Constant.MODEL_EVO100))
            {
                stubHoistwayTractionResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.OTHEREQUIPMENTUIRESPONSETEMPLATE, Constant.EVOLUTION__100)));
            }
            // setting stub data into an sectionsValues object
            var mainFilteredRespone = Utility.MapVariables(JObject.FromObject(baseConfigureResponse), stubHoistwayTractionResponse);
            var filteredSection = mainFilteredRespone.ToObject<ConfigurationResponse>();
            var filteredSections = filteredSection.Sections;
            stubUnitConfigurationResponseObj.Sections = filteredSections;
            return stubUnitConfigurationResponseObj;
        }


        /// <summary>
        /// GeneralInformationUnitConfigureBl
        /// </summary>
        /// <param name="baseConfigureRequest"></param>
        /// <param name="productType"></param>
        /// <param name="baseConfigureResponse"></param>
        /// <param name="rangeValuesList"></param>
        /// <returns></returns>
        public static ConfigurationResponse GeneralInformationUnitConfigureBl(ConfigureRequest baseConfigureRequest, string productType, ConfigurationResponse baseConfigureResponse, List<JToken> rangeValuesList)
        {
            var generalInformationTemplateObject = new ConfigurationResponse();
            var responseObj = new ConfigurationResponse();
            var generalInformationTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.GENERALINFORMATIONTEMPLATE, productType)));
            //Sub Section Stub Path
            //use currentproductType
            // setting stub data into an sectionsValues object
            //Sub Sections Stub Path
            //no need to serialize
            var mainFilteredRespone = Utility.MapVariables(JObject.FromObject(baseConfigureResponse), generalInformationTemplate);

            var filteredSection = mainFilteredRespone.ToObject<ConfigurationResponse>();
            var filteredSections = filteredSection.Sections;
            //enrichedTemplateObj.Sections = filteredSections;
            responseObj.Sections = filteredSections;

            return responseObj;
        }


        /// <summary>
        /// SetGroupCrossPackage
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="variableAssignments"></param>
        /// <param name="groupContantsDictionary"></param>
        /// <param name="selectedTabValue"></param>
        /// <returns></returns>
        private List<VariableAssignment> SetGroupCrossPackage(string sessionId, JObject variableAssignments, IDictionary<string, string> groupContantsDictionary, string selectedTabValue)
        {
            var crossPackagevariableAssignments = new List<VariableAssignment>();
            var getCrossPackageValues = GetCrosspackageVariableAssignments(sessionId, Constant.GROUPCONFIGURATION);
            var groupConstantsDictionary = Utility.GetVariableMapping(Constant.GROUPMAPPERVARIABLES, Constant.GROUPMAPPERCONFIGURATION);
            if (!string.IsNullOrEmpty(getCrossPackageValues))
            {
                crossPackagevariableAssignments = Utility.DeserializeObjectValue<List<VariableAssignment>>(getCrossPackageValues);
            }
            var configAssignments = Utility.DeserializeObjectValue<Line>(Utility.SerializeObjectValue(variableAssignments)).VariableAssignments;

            if (selectedTabValue == Constant.CONTROLROOMTAB)
            {
                configAssignments = configAssignments.Where(x => !x.VariableId.EndsWith(groupConstantsDictionary[Constant.ACROSSTHEHALLDISTANCEPARAMETER]) || !x.VariableId.EndsWith(groupConstantsDictionary[Constant.BANKOFFSETPARAMETER])).ToList();
                foreach (var varAssign in configAssignments)
                {
                    if (varAssign.VariableId.EndsWith(groupConstantsDictionary[Constant.PARMETERSXDIMENSIONVALUES]) || varAssign.VariableId.EndsWith(groupConstantsDictionary[Constant.PARAMETERSYDIMENSIONVALUES]))
                    {
                        varAssign.Value = (Convert.ToDecimal(varAssign.Value) * 12);
                    }
                }
            }
            else
            {
                configAssignments = configAssignments.Where(x => !x.VariableId.EndsWith(groupConstantsDictionary[Constant.ACROSSTHEHALLDISTANCEPARAMETER]) ||
                !x.VariableId.EndsWith(groupConstantsDictionary[Constant.BANKOFFSETPARAMETER]) ||
                !x.VariableId.EndsWith(groupConstantsDictionary[Constant.PARMETERSXDIMENSIONVALUES]) ||
                !x.VariableId.EndsWith(groupConstantsDictionary[Constant.PARAMETERSYDIMENSIONVALUES])).ToList();
            }
            var assignments = new Line() { VariableAssignments = configAssignments };

            //generate cross package variable assignments
            crossPackagevariableAssignments = GenerateVariableAssignmentsForUnitConfiguration(crossPackagevariableAssignments, assignments);

            //removing the hall station variables from cache which are deselected 
            if (selectedTabValue == Constant.RISERLOCATIONSTAB)
            {
                var hallStationVariables = JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES))[Constant.HALLSTATIONS].ToList();
                foreach (var varAssign in crossPackagevariableAssignments)
                {
                    if (hallStationVariables.Contains(varAssign.VariableId) && varAssign.Value.Equals(Constant.FALSEVALUES))
                    {
                        crossPackagevariableAssignments = crossPackagevariableAssignments.Where(x => !x.VariableId.Equals(varAssign.VariableId)).ToList();
                    }
                }
            }

            //removing the control location variables from cache which are deselected 
            if (selectedTabValue == Constant.CONTROLROOMTAB)
            {
                foreach (var varAssign in crossPackagevariableAssignments)
                {
                    if (varAssign.Value.Equals(Constant.FALSEVALUES))
                    {
                        crossPackagevariableAssignments = crossPackagevariableAssignments.Where(x => !x.VariableId.Equals(varAssign.VariableId)).ToList();
                    }
                }
            }

            SetCrosspackageVariableAssignments(crossPackagevariableAssignments, sessionId, Constant.GROUPCONFIGURATION);
            return crossPackagevariableAssignments;
        }

        /// <summary>
        /// CabInteriorChangeUnitConfigureBl
        /// </summary>
        /// <param name="productType"></param>
        /// <param name="baseConfigureResponse"></param>
        /// <returns></returns>
        public static ConfigurationResponse CabInteriorUnitConfigureBl(string productType, ConfigurationResponse baseConfigureResponse, List<VariableAssignment> crossPackageVariable)
        {
            var responseObj = new ConfigurationResponse();
            var unitMapperVariablesCabInterior = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.CABINTERIORMAPPER);
            var cabInteriorTemplate = JObject.Parse(File.ReadAllText(string.Format(Constant.CABINTERIORTEMPLATE, productType)));
            if (Utility.CheckEquals(productType, Constants.EVOLUTION200))
            {
                foreach (var value in crossPackageVariable)
                {
                    if (Utility.CheckEquals(value.VariableId, unitMapperVariablesCabInterior[Constants.HANDRAILTYPE]))
                    {
                        if (Utility.CheckEquals(value.Value.ToString(), Constants.None))
                        {
                            cabInteriorTemplate = JObject.Parse(File.ReadAllText(string.Format(Constants.CABINTERIORTEMPLATENEW, productType)));
                        }
                    }
                }
            }
            var mainFilteredRespone = Utility.MapVariables(JObject.FromObject(baseConfigureResponse), cabInteriorTemplate);
            var filteredSection = mainFilteredRespone.ToObject<ConfigurationResponse>();
            var filteredSections = filteredSection.Sections;
            responseObj.Sections = filteredSections;
            return responseObj;
        }

        /// <summary>
        /// GetConfigurationMatrialValues
        /// </summary>
        /// <param name="configurationRequest"></param>
        /// <param name="materialPath"></param>
        /// <returns></returns>
        private ConfigurationRequest GetConfigurationMatrialValues(ConfigurationRequest configurationRequest, string materialPath)
        {
            var sysValContantsDictionary = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.SYSTEMVALIDATIONCONSTANTMAPPER);
            configurationRequest.PackagePath = _configuration[Constant.VALIDATIONPATH];
            if (Utility.CheckEquals(materialPath, Constants.CABVALIDATIONSUBPATHEVO100))
            {
                configurationRequest.PackagePath = _configuration[Constants.CABVLAIDATIONPATHEVO100];
            }
            if (Utility.CheckEquals(materialPath, Constants.EMPTYVALIDATIONPATHEVO100SUBPATH))
            {
                configurationRequest.PackagePath = _configuration[Constants.EMPTYVALIDATIONPATHEVO100];
            }
            if (Utility.CheckEquals(materialPath, Constants.DUTYVALIDATIONEVO100SUBPATH))
            {
                configurationRequest.PackagePath = _configuration[Constants.DUTYVALIDATIONPATHEVO100];
            }
            if(Utility.CheckEquals(materialPath, Constants.SLINGVALIDATIONSUBPATHEVO100))
            {
                configurationRequest.PackagePath = _configuration[Constants.SLINGVALIDATIONPATHEVO100];
            }
            if (Utility.CheckEquals(materialPath, Constants.CABVALIDATIONSUBPATHEND100))
            {
                configurationRequest.PackagePath = _configuration[Constants.CABVLAIDATIONPATHEND100];
            }
            if (Utility.CheckEquals(materialPath, Constants.SLINGVALIDATIONSUBPATHEND100))
            {
                configurationRequest.PackagePath = _configuration[Constants.SLINGVALIDATIONPATHEND100];
            }
            if (Utility.CheckEquals(materialPath, Constants.DUTYVALIDATIONEND100SUBPATH))
            {
                configurationRequest.PackagePath = _configuration[Constants.DUTYVALIDATIONPATHEND100];
            }
            if (Utility.CheckEquals(materialPath, Constants.JACKDUTYVALIDATIONEND100SUBPATH))
            {
                configurationRequest.PackagePath = _configuration[Constants.JACKDUTYVALIDATIONPATHEND100];
            }
            configurationRequest.GlobalArguments[sysValContantsDictionary[Constant.MATERIALNAMEVALUES]] = _configuration[materialPath];
            configurationRequest.GlobalArguments[sysValContantsDictionary[Constant.MATERIALEXTERNALID]] = _configuration[materialPath];
            configurationRequest.Line.Product.Id = _configuration[materialPath];
            configurationRequest.Line.ProductId = _configuration[materialPath];
            return configurationRequest;
        }
        /// <summary>
        /// EntrancesUnitConfigureBL
        /// </summary>
        /// <param name="productType"></param>
        /// <param name="baseConfigureResponse"></param>
        /// <param name="listEntranceConfigurations"></param>
        /// <returns></returns>
        public ConfigurationResponse EntrancesUnitConfigureBL(string productType, ConfigurationResponse baseConfigureResponse, List<EntranceConfigurations> listEntranceConfigurations)
        {

            var stubUnitConfigurationResponseObj = new ConfigurationResponse();

            var stubHoistwayTractionResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.ENTRANCESUIRESPONSETEMPLATE, Constant.EVOLUTION200)));
            if (productType.Equals(Constant.ENDURA_100))
            {
                stubHoistwayTractionResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.ENTRANCESUIRESPONSETEMPLATE, Constant.END100)));
            }
            if (productType.Equals(Constant.MODEL_EVO100))
            {
                stubHoistwayTractionResponse = JObject.Parse(File.ReadAllText(string.Format(Constant.ENTRANCESUIRESPONSETEMPLATE, Constant.EVOLUTION__100)));
            }
            // setting stub data into an sectionsValues object
            var mainFilteredRespone = Utility.MapVariables(JObject.FromObject(baseConfigureResponse), stubHoistwayTractionResponse);
            var configurationSection = Utility.GetTokens(Constant.SECTIONS, mainFilteredRespone).Where(x => x[Constant.IDPARAM].Value<string>().Equals(Constant.ENTRANCECONSOLESECTION)).ToList().FirstOrDefault();
            var section = configurationSection.ToObject<SectionsValues>();
            section.sections = new List<SectionsGroupValues>();
            //Setting property for All opening assigned or not
            var lstSelectedFrontOpenings = (from entrance in listEntranceConfigurations
                                            from location in entrance.FixtureLocations
                                            where location.Front.Value.Equals(true)
                                            select location.FloorNumber).ToList();
            var lstSelectedRearOpenings = (from entrance in listEntranceConfigurations
                                           from location in entrance.FixtureLocations
                                           where location.Rear.Value.Equals(true)
                                           select location.FloorNumber).ToList();
            var FrontOpenings = listEntranceConfigurations.Count > 0 ? listEntranceConfigurations[0].FrontOpenings : 0;
            var RearOpenings = listEntranceConfigurations.Count > 0 ? listEntranceConfigurations[0].RearOpenings : 0;
            var isFront = listEntranceConfigurations.Count > 0 && listEntranceConfigurations[0].Openings.Front;
            var isRear = listEntranceConfigurations.Count > 0 && listEntranceConfigurations[0].Openings.Rear;
            section.AllOpeningsSelected = isFront ? isRear ? lstSelectedFrontOpenings.Count == FrontOpenings && lstSelectedRearOpenings.Count == RearOpenings
                                               : lstSelectedFrontOpenings.Count == FrontOpenings : !isRear || lstSelectedRearOpenings.Count == RearOpenings;
            //calling methods to create list of consoles
            section = GetListOfEntranceConsoles(listEntranceConfigurations, section);
            var filteredSection = mainFilteredRespone.ToObject<ConfigurationResponse>();
            var filteredSections = filteredSection.Sections;
            foreach (var filteredsection in filteredSections[0].sections.Where(x => x.Id.Equals(Constant.ENTRANCECONSOLESECTION)))
            {
                filteredsection.sections = section.sections;
                filteredsection.AllOpeningsSelected = section.AllOpeningsSelected;
                filteredsection.Variables = new List<Variables>();

            }
            stubUnitConfigurationResponseObj.Sections = filteredSections;
            return stubUnitConfigurationResponseObj;
        }

        /// <summary>
        /// Method to create the list of entrance consoles
        /// </summary>
        /// <param name="listEntranceConfigurations"></param>
        /// <param name="section"></param>
        public SectionsValues GetListOfEntranceConsoles(List<EntranceConfigurations> listEntranceConfigurations, SectionsValues section)
        {

            var iscontrollerCount = (from vconsole in listEntranceConfigurations
                                     where vconsole.IsController.Equals(true)
                                     select vconsole).ToList();
            foreach (var entranceconsole in listEntranceConfigurations)
            {
                if (entranceconsole.EntranceConsoleId != 0)
                {
                    var frontRearAssignment = Utility.GetLandingOpeningAssignmentSelected(entranceconsole.FixtureLocations);
                    var listOfVariables = new List<Variables>();
                    foreach (var variable in section.Variables)
                    {
                        var variableValue = entranceconsole.VariableAssignments.Where(x => x.VariableId.Equals(variable.Id)).ToList().FirstOrDefault();
                        var newVariable = new Variables()
                        {
                            Id = variable.Id,
                            Value = variableValue.Value
                        };
                        listOfVariables.Add(newVariable);
                    }
                    var varSection = new SectionsGroupValues()
                    {
                        Id = entranceconsole.EntranceConsoleId.ToString(),
                        Name = Constant.ENTRANCECONSOLESECTION + Constant.DOT + entranceconsole.ConsoleName,
                        OpeningRange = frontRearAssignment,
                        assignOpenings = entranceconsole.AssignOpenings,
                        isController = entranceconsole.IsController,
                        IsDelete = iscontrollerCount.Count > 0 ? entranceconsole.EntranceConsoleId != 1 && entranceconsole.EntranceConsoleId != 2 : entranceconsole.EntranceConsoleId != 1,
                        Variables = listOfVariables,
                        Properties = new List<PropertyDetailsValues>
                                    {
                                        new PropertyDetailsValues()
                                        {
                                            Id = Constant.SEQUENCE,
                                            Value = entranceconsole.EntranceConsoleId,
                                            Type=Constant.NUMBERLOWER
                                        },
                                        new PropertyDetailsValues()
                                        {
                                            Id = Constant.SECTIONNAMEPASCAL,
                                            Value = entranceconsole.ConsoleName,
                                            Type=Constant.STRING
                                        },
                                        new PropertyDetailsValues()
                                        {
                                            Id = Constant.SECTIONTYPE,
                                            Value = Constant.CONSOLE,
                                            Type=Constant.STRING
                                        }
                                    }
                    };
                    section.sections.Add(varSection);
                }
            }
            return section;
        }

        private dynamic GetPropertyValue(string propertyName, dynamic minData, dynamic maxData, dynamic customValue)
        {
            var customData = customValue.ToString();
            if (!customData.Equals(Constant.CUSTOM))
            {
                minData = -10000;
                maxData = 10000;
            }
            else if (customValue.Equals(Constant.CUSTOM) && (minData == null || maxData == null))
            {
                minData = -10000;
                maxData = 10000;
            }
            switch (propertyName)
            {
                case Constant.MINVALUE:
                    return Math.Round(Convert.ToDouble(minData), 3);

                case Constant.MAXVALUE:
                    return Math.Round(Convert.ToDouble(maxData), 3);

                case Constant.ISRANGEINPUTTYPE:
                    return true;

                case Constant.READONLY_LOWER:
                    var readVal = customValue.Equals(Constant.MAXIMUM) || customValue.Equals(Constant.MINIMUM);
                    return readVal;


                case Constant.RANGEVALIDATION:
                    var minValues = Convert.ToDouble(minData) / 12;
                    var maxValues = Convert.ToDouble(maxData) / 12;
                    var minInches = Math.Round((Convert.ToDouble(minData) % 12), 3).ToString();
                    var maxInches = Math.Round((Convert.ToDouble(maxData) % 12), 3).ToString();
                    var constanval = Constant.GENERALINFORMATIONMESSAGE;
                    return string.Format(constanval, minValues.ToString().Split('.')[0], minInches, maxValues.ToString().Split('.')[0], maxInches);


                default:
                    return string.Empty;

            }
        }
        private dynamic GetPropertyValueForHeight(string propertyName, dynamic minData, dynamic maxData, dynamic customValue)
        {
            switch (propertyName)
            {
                case Constant.MINVALUE:
                    return Math.Round(Convert.ToDouble(minData), 2);

                case Constant.MAXVALUE:
                    return Math.Round(Convert.ToDouble(maxData), 2);

                case Constant.ISRANGEINPUTTYPE:
                    return true;

                case Constant.READONLY_LOWER:
                    var readVal = customValue.Equals(Constant.MAXIMUM) || customValue.Equals(Constant.MINIMUM);
                    return readVal;


                case Constant.RANGEVALIDATION:
                    if (Utility.CheckEquals(Convert.ToString(customValue), Constants.OTHERWEIGHT))
                    {
                        var minValues = Convert.ToDouble(minData);
                        var maxValues = Convert.ToDouble(maxData);
                        var constanval = Constants.WEIGHTRANGE;
                        return string.Format(constanval, minValues, maxValues);
                    }
                    else
                    {
                        var minValues = Convert.ToDouble(minData) / 12;
                        var maxValues = Convert.ToDouble(maxData) / 12;
                        var minInches = Math.Round((Convert.ToDouble(minData) % 12), 2).ToString();
                        var maxInches = Math.Round((Convert.ToDouble(maxData) % 12), 2).ToString();
                        var constanval = Constant.GENERALINFORMATIONMESSAGE;
                        return string.Format(constanval, minValues.ToString().Split('.')[0], minInches, maxValues.ToString().Split('.')[0], maxInches);
                    }


                default:
                    return string.Empty;

            }
        }

        private Values CreateSingletonValue(dynamic value)
        {
            return new Values()
            {
                Type = Constant.SINGLETONVALUE,
                value = value,
                Assigned = Constant.BYRULE,
                InCompatible = false,
                State = Constant.AVAILABLE
            };
        }

        /// <summary>
        /// ReleaseInfoCLMCall
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="sectionTab"></param>
        /// <param name="fixtureStrategy"></param>
        /// <param name="setId"></param>
        /// <param name="productType"></param>
        /// <param name="variableAssignments"></param>
        /// <param name="lstunits"></param>
        /// <param name="entranceConsoles"></param>
        /// <param name="unitHallFixtureConsoles"></param>
        /// <param name="groupConfigurationId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> ReleaseInfoCLMCall(string sessionId, string sectionTab, string productType,
                      JObject variableAssignments = null, List<UnitNames> lstunits = null, List<EntranceConfigurations> entranceConsoles = null, List<UnitHallFixtures> unitHallFixtureConsoles = null, int groupConfigurationId = 0)
        {
            var methodBeginTime = Utility.LogBegin();
            var unithallfixtureContantsDictionary = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.UNITHALLFIXTURECONSTANTMAPPER);
            var configureRequest = CreateConfigurationRequestWithTemplate(variableAssignments, Constant.UNITNAME, null, productType);
            var mainGroupConfigurationResponse = new ConfigurationResponse
            {
                Sections = new List<Sections>()
            };
            List<Compartment> lstcompartments = new List<Compartment>();
            var packagePath = configureRequest?.PackagePath;
            //Gets the base configuration of the model
            var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest);
            //Adding include sections
            baseConfigureRequest = GenerateIncludeSections(baseConfigureRequest, Constant.UNITCONFIGURATION, sectionTab, productType);
            //Remove hoistwaydimension variable if the min or max is selected
            var stubRangeValues = JObject.Parse(File.ReadAllText(Constant.RANGEJOBJECT));
            var rangeValuesList = stubRangeValues[Constant.RANGEVALUES].ToList();
            var assignments = (List<VariableAssignment>)baseConfigureRequest.Line.VariableAssignments;
            var hoistway = assignments.Where(x => x.VariableId.Contains(Constant.HOISTWAYDIMENSIONS)).ToList();
            if (hoistway.Count > 0)
            {
                var valdimension = Convert.ToString(hoistway.FirstOrDefault().Value);
                if (Utility.CheckEquals(valdimension, Constant.MINIMUM) || Utility.CheckEquals(valdimension, Constant.MAXIMUM))
                {
                    assignments = assignments.Where(x => !rangeValuesList.Contains(x.VariableId)).ToList();
                }
            }
            baseConfigureRequest.Line.VariableAssignments = assignments;
            var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, packagePath, sessionId).ConfigureAwait(false);
            return baseConfigureResponseJObj;
        }

        public void UpdateCacheWithAutoResolvedConflicts(UIMappingBuildingConfigurationResponse configureResponse, JObject variableAssignments, string sessionId, string selectedTabValue, Dictionary<string, string> constants)
        {
            if (configureResponse.ConflictAssignments.ResolvedAssignments.Count > 0)
            {
                foreach (var assignment in configureResponse.ConflictAssignments.ResolvedAssignments)
                {
                    foreach (var item in variableAssignments[Constant.SYSVALVARIABLEASSIGNMENTS].Children())
                    {
                        var itemProperties = item.Children<JProperty>();
                        if (itemProperties.Any(x => x.Name == Constant.VARIABLEID && Utility.MapElevatorNameFromModelToUI((string)x.Value) == assignment.VariableId.ToString()))
                        {
                            itemProperties.First(x => x.Name == Constant.VALUE).Value = assignment.Value.ToString();
                        }
                        if (itemProperties.Any(x => x.Name == Constant.VARIABLEID && ((string)x.Value).Contains("Parameters_SP.HS") && ((string)x.Value).Equals(assignment.VariableId)))
                        {
                            itemProperties.First(x => x.Name == Constant.VALUE).Value = assignment.Value.ToString();
                        }
                    }
                }
                SetGroupCrossPackage(sessionId, variableAssignments, constants, selectedTabValue);
            }
        }

        /// <summary>
        /// GetCrossPackageVariableDefaultValues
        /// </summary>
        /// <param name="variableAssignmentsz"></param>
        /// <param name="groupId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<string> GetCrossPackageVariableDefaultValues(JObject variableAssignmentsz, int groupId, string sessionId)
        {
            var variableAssignmentz = new Line();
            var cachedConfigurations = _cpqCacheManager.GetCache(sessionId, _environment, groupId.ToString(), Constant.CURRENTGROUPCONFIGURATION);
            //fetching configuration from clm if cached configuration is null
            if (string.IsNullOrEmpty(cachedConfigurations))
            {
                var groupLayout = _groupConfiguration.GetGroupConfigurationDetailsByGroupId(groupId, Constant.GROUPLAYOUTCONFIGURATION, sessionId);
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
                var variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(variableAssignmentz));
                var configureRequest = CreateConfigurationRequestWithTemplate(variableAssignments, Constant.GROUPCONFIGURATIONNAME);
                var configureResponseJObj =
                    await ConfigurationBl(configureRequest, configureRequest.PackagePath, sessionId).ConfigureAwait(false);
                var configureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
                var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponse.Arguments));
                cachedConfigurations = Utility.SerializeObjectValue(configureResponseArgumentJObject[Constant.CONFIGURATION]);
                _cpqCacheManager.SetCache(sessionId, _environment, groupId.ToString(), Constant.CURRENTGROUPCONFIGURATION, cachedConfigurations);
            }
            return cachedConfigurations;
        }

        /// <summary>
        /// GetDefaultUnitValues
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> GetDefaultUnitValues(string sessionId)
        {
            var configureRequest = CreateConfigurationRequestWithTemplate(null, Constant.UNITNAME, null, Constant.EVO_200);
            var packagePath = configureRequest?.PackagePath;
            var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest);

            var varAssignments = (List<VariableAssignment>)baseConfigureRequest.Line.VariableAssignments;
            baseConfigureRequest.Line.VariableAssignments = varAssignments;
            var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, packagePath, sessionId).ConfigureAwait(false);
            // configuration object values for conflict mapping
            var configureResponse = baseConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = configureResponse.Arguments;
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            var configureRequestDictionary = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();
            return configureRequestDictionary;
        }

        /// <summary>
        /// AddDynamicLayout
        /// </summary>
        /// <param name="stubUnitConfigurationSubSectionResponseObj"></param>
        /// <param name="variablesItem"></param>
        /// <returns></returns>
        private IList<Properties> AddDynamicLayout(ConfigurationResponse stubUnitConfigurationSubSectionResponseObj, Variables variablesItem)
        {
            var unitMapperVariablesGeneralInformation = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constant.GENERALINFOMAPPER);
            var valueList = (from subSectionDetail in stubUnitConfigurationSubSectionResponseObj.Sections
                             from subSection in subSectionDetail.sections
                             from variableVal in subSection.Variables
                             select variableVal.Id).ToList();
            if (!valueList.Contains(unitMapperVariablesGeneralInformation[Constants.REARDOORHEIGHTMAP]) && !valueList.Contains(unitMapperVariablesGeneralInformation[Constants.REARDOORTYPEMAP]) && !valueList.Contains(unitMapperVariablesGeneralInformation[Constants.REARDOORWIDTHMAP]))
            {
                var layout = new Properties { Id = Constants.LAYOUT, Type = Constants.STRINGVAL, Value = Constants.LEFTLAYOUT };
                variablesItem.Properties.Add(layout);
            }
            else
            {
                var layout = new Properties { Id = Constants.LAYOUT, Type = Constants.STRINGVAL, Value = Constants.FULLLAYOUT };
                variablesItem.Properties.Add(layout);
            }
            return variablesItem.Properties;
        }

        /// <summary>
        /// GetMapperVariables
        /// </summary>
        /// <returns></returns>
        public List<ConfigVariable> GetMapperVariables()
        {
            var buildingContantsDictionary = Utility.GetVariableMapping(Constant.BUILDINGMAPPERVARIABLESMAPPERPATH, Constant.BUILDINGMAPPERCONFIGURATION);
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
            return mapperVariables;
        }

        public async Task<OpeningLocations> GetDataToCheckOccupiedSpace(int groupId, JObject variableAssignments, string sessionId)
        {
            var tractionVariableAssignments = JObject.FromObject(new Line());
            var tractionConfigureRequest = CreateConfigurationRequestWithTemplate(variableAssignments, Constant.GROUPCONFIGURATIONNAME);
            var configureResponseJObj = await ConfigurationBl(tractionConfigureRequest, tractionConfigureRequest.PackagePath, sessionId).ConfigureAwait(false);

            var tractionConfigureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var tractionConfigureResponseArgument = tractionConfigureResponse.Arguments;
            var tractionConfigureResponseArgumentJObject = JObject.FromObject(tractionConfigureResponseArgument);
            var TractionConfigureRequestDictionary = tractionConfigureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();

            var crossPackagevariableDictionary = new Dictionary<string, string>();

            var crossPackageVariableId = (JObject.Parse(File.ReadAllText(Constant.VARIABLEDICTIONARY)));
            JToken crossPackageVariables;
            crossPackageVariables = crossPackageVariableId[Constant.DEFAULTVALUES];
            crossPackagevariableDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(crossPackageVariables));

            var openingLocationVariableAssignments = (from val1 in TractionConfigureRequestDictionary
                                                      from val2 in crossPackagevariableDictionary
                                                      where Utility.CheckEquals(val1.Key.ToString(), val2.Key.ToString())
                                                      select new VariableAssignment
                                                      {
                                                          VariableId = val1.Key,
                                                          Value = val1.Value
                                                      }).Distinct().ToList();
            var mapperVariables = GetMapperVariables();
            var result = _openingLocationdl.GetOpeningLocationBygroupId(groupId, openingLocationVariableAssignments, mapperVariables, sessionId);
            return result;
        }

        private void SetIncompatibilityForCWSFTY(OpeningLocations result, Variables variablesItem, List<UnitNames> lstunits)
        {
            var listOfUnits = (from unit in result.Units
                               where unit.OcuppiedSpace == true
                               select unit).ToList();
            var commonUnits = new List<int>();
            foreach (var unit in listOfUnits)
            {
                foreach (var units in lstunits)
                {
                    if (units.Unitid == unit.UnitId)
                    {
                        commonUnits.Add(unit.UnitId);
                    }
                }
            }
            foreach (var value in variablesItem.Values)
            {
                if (commonUnits.Count.Equals(0))
                {
                    if (value.value.ToString() == Constants.TRUEVALUES)
                    {
                        value.InCompatible = true;
                        value.Assigned = null;
                    }
                    if (value.value.ToString() == Constants.FALSEVALUES)
                    {
                        value.InCompatible = false;
                        value.Assigned = "byUser";
                    }
                }
                else
                {
                    if (value.value.ToString() == Constants.FALSEVALUES)
                    {
                        value.InCompatible = true;
                        value.Assigned = null;
                    }
                    if (value.value.ToString() == Constants.TRUEVALUES)
                    {
                        value.InCompatible = false;
                        value.Assigned = "byUser";
                    }
                }
            }
        }

        public void SetGroupDefaultsCache(string sessionId, JObject configureResponseArgumentJObject)
        {
            // adding defaults to cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constants.DEFAULTGROUPCONFIGVALUES, Utility.SerializeObjectValue(configureResponseArgumentJObject));
        }
        /// <summary>
        /// GetDefaultValues
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="defaultType"></param>
        /// <param name="packageType"></param>
        /// <returns></returns>
        public async Task<List<VariableAssignment>> GetDefaultValues(string sessionId, string defaultType, string packageType)
        {

            var utilti = new Dictionary<string, object>();
            var variablesAssignments = new List<VariableAssignment>();
            //var cacheVaraibles = _cpqCacheManager.GetCache(sessionId, _environment, defaultType);
            //if (!string.IsNullOrEmpty(cacheVaraibles))
            //{
            //    var cacheJobjectResponse = JObject.Parse(cacheVaraibles);
            //    utilti = Utility.DeserializeObjectValue<Dictionary<string, object>>(Utility.SerializeObjectValue(cacheJobjectResponse[Constant.CONFIGURATION]));
            //    //}
            //    //else
            //    //{
            //    //var configureRequest = CreateConfigurationRequestWithTemplate(null, packageType, null, Constant.EVO_200);
            //    //var packagePath = configureRequest?.PackagePath;
            //    //var baseConfigureRequest = _configureService.GetBaseConfigureRequest(configureRequest);

            //    //var varAssignments = (List<VariableAssignment>)baseConfigureRequest.Line.VariableAssignments;
            //    //baseConfigureRequest.Line.VariableAssignments = varAssignments;
            //    //var baseConfigureResponseJObj = await ConfigurationBl(baseConfigureRequest, packagePath, sessionId).ConfigureAwait(false);
            //    //// configuration object values for conflict mapping
            //    //var configureResponse = baseConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            //    //var configureResponseArgument = configureResponse.Arguments;
            //    //var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponseArgument));
            //    //utilti = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();

            //    //}
            //    foreach (var item in utilti)
            //    {
            //        var variableId = string.Empty;
            //        if (Utility.CheckEquals(defaultType, Constants.DEFAULTBUILDINGCONFIGVALUES) && item.Key.Contains(Constants.BUILDINGCONFIGURATIONID + Constants.DOT))
            //        {
            //            variableId = item.Key.Replace(Constants.BUILDINGCONFIGURATIONID + Constants.DOT, string.Empty);
            //        }
            //        else if (Utility.CheckEquals(defaultType, Constants.DEFAULTGROUPCONFIGVALUES) && item.Key.Contains(Constants.BUILDINGCONFIGURATIONID))
            //        {
            //            variableId = item.Key.Replace(Constants.BUILDINGCONFIGURATIONID + Constants.DOT, Constants.ELEVATOR);
            //        }
            //        var variableAssginment = new VariableAssignment()
            //        {
            //            VariableId = !string.IsNullOrEmpty(variableId) ? variableId : item.Key,
            //            Value = item.Value
            //        };
            //        variablesAssignments.Add(variableAssginment);
            //    }
            //}
            return variablesAssignments;
        }
        /// <summary>
        /// SaveConflictsValues
        /// </summary>
        /// <param name="configurationId"></param>
        /// <param name="listOfChangedVariables"></param>
        /// <returns></returns>
        public List<Result> SaveConflictsValues(int configurationId, List<VariableAssignment> listOfChangedVariables, string entityType)
        {
            List<Result> conflictResponse = _groupConfiguration.SaveBuildingConflicts(configurationId, listOfChangedVariables, entityType);
            return conflictResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="configurationType"></param>
        /// <returns></returns>
        public List<ConflictMgmtList> GetCacheValuesForInternalConflicts(string sessionId, string conflictType)
        {
            var methodBeginTime = Utility.LogBegin();
            var filterConflictResponse = new List<ConflictMgmtList>();
            var getCacheConflictsResponse = string.Empty;
            if (!string.IsNullOrEmpty(conflictType))
            {
                switch (conflictType)
                {
                    case Constant.BUILDING:
                        getCacheConflictsResponse = _cpqCacheManager.GetCache(sessionId, _environment, Constant.INTERNALPREVIOUSCONFLICTSVALUES);
                        break;
                    case Constant.GROUP:
                        getCacheConflictsResponse = _cpqCacheManager.GetCache(sessionId, _environment, Constant.INTERNALPREVIOUSGROUPCONFLICTSVALUES);
                        break;
                    case Constant.UNIT:
                        getCacheConflictsResponse = _cpqCacheManager.GetCache(sessionId, _environment, Constant.INTERNALPREVIOUSUNITCONFLICTSVALUES);
                        break;
                    default:
                        getCacheConflictsResponse = _cpqCacheManager.GetCache(sessionId, _environment, Constant.INTERNALPREVIOUSCONFLICTSVALUES);
                        break;
                }
                if (!string.IsNullOrEmpty(getCacheConflictsResponse))
                {
                    filterConflictResponse = Utility.DeserializeObjectValue<List<ConflictMgmtList>>(getCacheConflictsResponse);
                }
            }
            Utility.LogEnd(methodBeginTime);
            return filterConflictResponse;
        }
        /// <summary>
        /// get 
        /// </summary>
        /// <param name="configurationRequest"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> OBOMConfigureBl(ConfigurationRequest configurationRequest, string sessionId, string packagePath)
        {
            if (configurationRequest == null)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = _localizer[Constant.REQUESTCANNOTBENULL + Constant.HYPHEN]
                });
            }
            //var configureResponseJobj = JObject.Parse(System.IO.File.ReadAllText(Constant.OBOMRESPONSESTUBPATH)).ToString();
            var configureResponseJObj =
            await ConfigurationBl(configurationRequest, configurationRequest.PackagePath, sessionId).ConfigureAwait(false);

            var response = configureResponseJObj.Response;

            //configuration object values
            //var configurationResponse = configureResponseJobj.Response.ToObject<StartConfigureResponse>();
            var configurationResponse = Utility.DeserializeObjectValue<OBOMResponse>(Utility.SerializeObjectValue(response));
            return new ResponseMessage { Response = Utilities.FilterNullValues(configurationResponse) };
        }
        private void GetRangeValidationForHoistway(Variables variablesItem, dynamic data)
        {
            var getPropertiesVal = JArray.Parse(File.ReadAllText(string.Format(Constant.PROPERTYTEMPLATE, Constant.EVOLUTION200)));
            var variableProperties = Utility.DeserializeObjectValue<IList<Properties>>(Utility.SerializeObjectValue(getPropertiesVal));
            var unitMapperVariablesOtherEquipment = Utility.GetVariableMapping(Constant.UNITSVARIABLESMAPPERPATH, Constants.OTHEREQUIPMENT);
            if (variablesItem.Id.Equals(unitMapperVariablesOtherEquipment[Constants.ADDITIONALWIRING]))
            {
                foreach (var item in variablesItem.Properties)
                {
                    if (data != null)
                    {
                        if (item.Id.Equals(Constant.MAXVALUESMALLCASE))
                        {
                            item.Value = Convert.ToInt32(item.Value) / 12 - Convert.ToInt32(data) / 12;
                        }
                    }
                    else
                    {
                        if (item.Id.Equals(Constant.MAXVALUESMALLCASE))
                        {
                            item.Value = Convert.ToInt32(item.Value) / 12;
                        }

                    }
                }
                foreach (var itemval in variableProperties)
                {
                    var minData = (from value in variablesItem.Properties where value.Id.Equals(Constant.MINVALUESMALLCASE) select value.Value).FirstOrDefault();
                    var maxData = (from value in variablesItem.Properties where value.Id.Equals(Constant.MAXVALUESMALLCASE) select value.Value).FirstOrDefault();
                    if (itemval.Id == Constant.RANGEVALIDATION)
                    {
                        var minValues = Convert.ToDouble(minData);
                        var maxValues = Convert.ToDouble(maxData);
                        var constanval = Constants.ADDITIONALWIRINGMESSAGE;
                        itemval.Value = string.Format(constanval, maxValues.ToString().Split('.')[0]);
                        variablesItem.Properties.Add(itemval);
                    }
                }
            }
            else
            {
                foreach (var item in variablesItem.Properties)
                {
                    if (item.Id.Equals(Constant.MINVALUESMALLCASE) || item.Id.Equals(Constant.MAXVALUESMALLCASE))
                    {
                        item.Value = Convert.ToInt32(item.Value) / 12;
                    }
                }
                foreach (var itemval in variableProperties)
                {
                    var minData = (from value in variablesItem.Properties where value.Id.Equals(Constant.MINVALUESMALLCASE) select value.Value).FirstOrDefault();
                    var maxData = (from value in variablesItem.Properties where value.Id.Equals(Constant.MAXVALUESMALLCASE) select value.Value).FirstOrDefault();
                    if (itemval.Id == Constant.RANGEVALIDATION)
                    {
                        var minValues = Convert.ToDouble(minData);
                        var maxValues = Convert.ToDouble(maxData);
                        var constanval = Constants.HOISTWAYVALIDATIONMESSAGE;
                        itemval.Value = string.Format(constanval, maxValues.ToString().Split('.')[0]);
                        variablesItem.Properties.Add(itemval);
                    }
                }
            }
        }

        private dynamic GetPropertyValueForWallThicknessEntrances(string propertyName, double minData, double maxData, dynamic customValue)
        {
            switch (propertyName)
            {
                case Constant.MINVALUE:
                    return Math.Round(minData, 2);

                case Constant.MAXVALUE:
                    return Math.Round(maxData, 2);

                case Constant.ISRANGEINPUTTYPE:
                    return true;

                case Constant.READONLY_LOWER:
                    var readVal = customValue.Equals(Constant.MAXIMUM) || customValue.Equals(Constant.MINIMUM);
                    return readVal;


                case Constant.RANGEVALIDATION:
                    var minInches = Math.Round(minData, 2).ToString();
                    var maxInches = Math.Round(maxData, 2).ToString();
                    var constanval = Constants.WALLTHICKNESSRANGEMESSAGE;
                    return string.Format(constanval, minInches, maxInches);


                default:
                    return string.Empty;

            }
        }

        /// <summary>
        /// Method for getting the value of allOpeningsSelected flag
        /// </summary>
        /// <param name="groupHallFixtureConsoles"></param>
        /// <param name="fixtureStrategy"></param>
        /// <returns></returns>
        private bool GetAllOpeningsFlagForGroupHallFixtures(List<GroupHallFixtures> groupHallFixtureConsoles, string fixtureStrategy)
        {
            var openingsFlag = false;
            if (fixtureStrategy.Equals(Constants.ETA))
            {

                var allFrontOpenings = new Dictionary<string, int>();
                var allSelectedFrontOpenings = new Dictionary<string, int>();

                foreach (var console in groupHallFixtureConsoles)
                {
                    if (console.GroupHallFixtureType == Constants.TRADITIONALHALLSTATION && console.ConsoleId == 1)
                    {
                        foreach (var hallstation in console.HallStations)
                        {
                            if (hallstation.HallStationId.Contains(Constants.FRONTHALLSTATION))
                            {
                                allFrontOpenings[hallstation.HallStationName] = ((from opening in hallstation.openingsAssigned
                                                                                  where opening.Front.NotAvailable.ToString().Equals(Constants.False, StringComparison.OrdinalIgnoreCase)
                                                                                  select opening.FloorDesignation).Distinct().ToList().Count());
                            }

                        }
                    }
                    if (console.GroupHallFixtureType == Constants.TRADITIONALHALLSTATION)
                    {
                        foreach (var hallstation in console.HallStations)
                        {
                            if (hallstation.HallStationId.Contains(Constants.FRONTHALLSTATION) && allSelectedFrontOpenings.Keys.Contains((hallstation.HallStationName)))
                            {
                                allSelectedFrontOpenings[hallstation.HallStationName] += ((from opening in hallstation.openingsAssigned
                                                                                           where opening.Front.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                                                                           select opening.FloorDesignation).Distinct().ToList().Count());
                            }
                            else if (hallstation.HallStationId.Contains(Constants.FRONTHALLSTATION))
                            {
                                allSelectedFrontOpenings[hallstation.HallStationName] = ((from opening in hallstation.openingsAssigned
                                                                                          where opening.Front.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                                                                          select opening.FloorDesignation).Distinct().ToList().Count());
                            }

                        }
                    }

                }
                var frontFlag = true;
                foreach (var hallstation in allFrontOpenings.Keys)
                {
                    if (allFrontOpenings[hallstation] != allSelectedFrontOpenings[hallstation])
                    {
                        frontFlag = false;
                        break;
                    }
                }

                var allRearOpenings = new Dictionary<string, int>();
                var allSelectedRearOpenings = new Dictionary<string, int>();
                foreach (var console in groupHallFixtureConsoles)
                {
                    if (console.GroupHallFixtureType == Constants.TRADITIONALHALLSTATION && console.ConsoleId == 1)
                    {
                        foreach (var hallstation in console.HallStations)
                        {
                            if (hallstation.HallStationId.Contains(Constants.REARHALLSTATION))
                            {
                                allRearOpenings[hallstation.HallStationName] = ((from opening in hallstation.openingsAssigned
                                                                                 where opening.Rear.NotAvailable.ToString().Equals(Constants.False, StringComparison.OrdinalIgnoreCase)
                                                                                 select opening.FloorDesignation).Distinct().ToList().Count());

                            }

                        }
                    }
                    if (console.GroupHallFixtureType == Constants.TRADITIONALHALLSTATION)
                    {
                        foreach (var hallstation in console.HallStations)
                        {
                            if (hallstation.HallStationId.Contains(Constants.REARHALLSTATION) && allSelectedRearOpenings.Keys.Contains((hallstation.HallStationName)))
                            {
                                allSelectedRearOpenings[hallstation.HallStationName] += ((from opening in hallstation.openingsAssigned
                                                                                          where opening.Rear.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                                                                          select opening.FloorDesignation).Distinct().ToList().Count());
                            }
                            else if (hallstation.HallStationId.Contains(Constants.REARHALLSTATION))
                            {
                                allSelectedRearOpenings[hallstation.HallStationName] = ((from opening in hallstation.openingsAssigned
                                                                                         where opening.Rear.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                                                                         select opening.FloorDesignation).Distinct().ToList().Count());
                            }

                        }
                    }

                }
                var rearFlag = true;
                foreach (var hallstation in allRearOpenings.Keys)
                {
                    if (allRearOpenings[hallstation] != allSelectedRearOpenings[hallstation])
                    {
                        rearFlag = false;
                        break;
                    }
                }

                var isRearDoorAvailable = (from console in groupHallFixtureConsoles
                                           where console.GroupHallFixtureType == Constants.TRADITIONALHALLSTATION && console.ConsoleId == 1
                                           from hallstation in console.HallStations
                                           where hallstation.HallStationId.Contains(Constants.REARHALLSTATION)
                                           select hallstation).Distinct().Count() > 0 ? true : false;
                openingsFlag = isRearDoorAvailable ? frontFlag && rearFlag : frontFlag;
            }
            else if (fixtureStrategy.Equals(Constants.ETD))
            {
                var allFrontOpenings = (from console in groupHallFixtureConsoles
                                        where console.GroupHallFixtureType == Constants.AGILEHALLSTATION && console.ConsoleId == 1
                                        from openings in console.GroupHallFixtureLocations
                                        where openings.Front.NotAvailable.ToString().Equals(Constants.False, StringComparison.OrdinalIgnoreCase)
                                        select openings.FloorNumber).Distinct();
                var allSelectedFrontOpenings = (from console in groupHallFixtureConsoles
                                                where console.GroupHallFixtureType == Constants.AGILEHALLSTATION
                                                from openings in console.GroupHallFixtureLocations
                                                where openings.Front.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                                select openings.FloorNumber).Distinct();
                var allRearOpenings = (from console in groupHallFixtureConsoles
                                       where console.GroupHallFixtureType == Constants.AGILEHALLSTATION && console.ConsoleId == 1
                                       from openings in console.GroupHallFixtureLocations
                                       where openings.Rear.NotAvailable.ToString().Equals(Constants.False, StringComparison.OrdinalIgnoreCase)
                                       select openings.FloorNumber).Distinct();
                var allSelectedRearOpenings = (from console in groupHallFixtureConsoles
                                               where console.GroupHallFixtureType == Constants.AGILEHALLSTATION
                                               from openings in console.GroupHallFixtureLocations
                                               where openings.Rear.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                               select openings.FloorNumber).Distinct();
                var isRearDoorAvailable = (from console in groupHallFixtureConsoles
                                           where console.GroupHallFixtureType == Constants.AGILEHALLSTATION && console.Openings.Rear && console.ConsoleId == 1
                                           select console.Openings.Rear).FirstOrDefault();
                openingsFlag = isRearDoorAvailable ? allSelectedFrontOpenings.Count() == allFrontOpenings.Count() && allSelectedRearOpenings.Count() == allRearOpenings.Count() : allSelectedFrontOpenings.Count() == allFrontOpenings.Count();
            }
            else
            {
                var allFrontOpenings = (from console in groupHallFixtureConsoles
                                        where console.GroupHallFixtureType == Constants.AGILEHALLSTATION && console.ConsoleId == 1
                                        from openings in console.GroupHallFixtureLocations
                                        where openings.Front.NotAvailable.ToString().Equals(Constants.False, StringComparison.OrdinalIgnoreCase)
                                        select openings.FloorNumber).Distinct().ToList();
                var allSelectedFrontOpenings = (from console in groupHallFixtureConsoles
                                                where console.GroupHallFixtureType == Constants.AGILEHALLSTATION
                                                from openings in console.GroupHallFixtureLocations
                                                where openings.Front.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                                select openings.FloorNumber).Distinct().ToList();

                var allRearOpenings = (from console in groupHallFixtureConsoles
                                       where console.GroupHallFixtureType == Constants.AGILEHALLSTATION && console.ConsoleId == 1
                                       from openings in console.GroupHallFixtureLocations
                                       where openings.Rear.NotAvailable.ToString().Equals(Constants.False, StringComparison.OrdinalIgnoreCase)
                                       select openings.FloorNumber).Distinct().ToList();

                var allSelectedRearOpenings = (from console in groupHallFixtureConsoles
                                               where console.GroupHallFixtureType == Constants.AGILEHALLSTATION
                                               from openings in console.GroupHallFixtureLocations
                                               where openings.Rear.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                               select openings.FloorNumber).Distinct().ToList();

                var allFrontOpeningsTraditional = new Dictionary<string, int>();
                var allSelectedFrontOpeningsTraditional = new Dictionary<string, int>();

                foreach (var console in groupHallFixtureConsoles)
                {
                    if (console.GroupHallFixtureType == Constants.TRADITIONALHALLSTATION && console.ConsoleId == 1)
                    {
                        foreach (var hallstation in console.HallStations)
                        {
                            if (hallstation.HallStationId.Contains(Constants.FRONTHALLSTATION))
                            {
                                allFrontOpeningsTraditional[hallstation.HallStationName] = ((from opening in hallstation.openingsAssigned
                                                                                             where opening.Front.NotAvailable.ToString().Equals(Constants.False, StringComparison.OrdinalIgnoreCase)
                                                                                             select opening.FloorDesignation).Distinct().ToList().Count());
                            }

                        }
                    }
                    if (console.GroupHallFixtureType == Constants.TRADITIONALHALLSTATION)
                    {
                        foreach (var hallstation in console.HallStations)
                        {
                            if (hallstation.HallStationId.Contains(Constants.FRONTHALLSTATION) && allSelectedFrontOpeningsTraditional.Keys.Contains((hallstation.HallStationName)))
                            {
                                allSelectedFrontOpeningsTraditional[hallstation.HallStationName] += ((from opening in hallstation.openingsAssigned
                                                                                                      where opening.Front.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                                                                                      select opening.FloorDesignation).Distinct().ToList().Count());
                            }
                            else if (hallstation.HallStationId.Contains(Constants.FRONTHALLSTATION))
                            {
                                allSelectedFrontOpeningsTraditional[hallstation.HallStationName] = ((from opening in hallstation.openingsAssigned
                                                                                                     where opening.Front.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                                                                                     select opening.FloorDesignation).Distinct().ToList().Count());
                            }

                        }
                    }

                }
                var frontFlag = true;
                foreach (var hallstation in allFrontOpeningsTraditional.Keys)
                {
                    if ((allFrontOpeningsTraditional[hallstation] - allSelectedFrontOpenings.Count()) != allSelectedFrontOpeningsTraditional[hallstation])
                    {
                        frontFlag = false;
                        break;
                    }
                }

                var allRearOpeningsTraditional = new Dictionary<string, int>();
                var allSelectedRearOpeningsTraditional = new Dictionary<string, int>();
                foreach (var console in groupHallFixtureConsoles)
                {
                    if (console.GroupHallFixtureType == Constants.TRADITIONALHALLSTATION && console.ConsoleId == 1)
                    {
                        foreach (var hallstation in console.HallStations)
                        {
                            if (hallstation.HallStationId.Contains(Constants.REARHALLSTATION))
                            {
                                allRearOpeningsTraditional[hallstation.HallStationName] = ((from opening in hallstation.openingsAssigned
                                                                                            where opening.Rear.NotAvailable.ToString().Equals(Constants.False, StringComparison.OrdinalIgnoreCase)
                                                                                            select opening.FloorDesignation).Distinct().ToList().Count());
                            }

                        }
                    }
                    if (console.GroupHallFixtureType == Constants.TRADITIONALHALLSTATION)
                    {
                        foreach (var hallstation in console.HallStations)
                        {
                            if (hallstation.HallStationId.Contains(Constants.REARHALLSTATION) && allSelectedRearOpeningsTraditional.Keys.Contains((hallstation.HallStationName)))
                            {
                                allSelectedRearOpeningsTraditional[hallstation.HallStationName] += ((from opening in hallstation.openingsAssigned
                                                                                                     where opening.Rear.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                                                                                     select opening.FloorDesignation).Distinct().ToList().Count());
                            }
                            else if (hallstation.HallStationId.Contains(Constants.REARHALLSTATION))
                            {
                                allSelectedRearOpeningsTraditional[hallstation.HallStationName] = ((from opening in hallstation.openingsAssigned
                                                                                                    where opening.Rear.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase)
                                                                                                    select opening.FloorDesignation).Distinct().ToList().Count());
                            }

                        }
                    }

                }
                var rearFlag = true;
                foreach (var hallstation in allRearOpeningsTraditional.Keys)
                {
                    if ((allRearOpeningsTraditional[hallstation] - allSelectedRearOpenings.Count()) != allSelectedRearOpeningsTraditional[hallstation])
                    {
                        rearFlag = false;
                        break;
                    }
                }



                var isRearDoorAvailable = (from console in groupHallFixtureConsoles
                                           where console.GroupHallFixtureType == Constants.AGILEHALLSTATION && console.Openings.Rear && console.ConsoleId == 1
                                           select console.Openings.Rear).FirstOrDefault();
                openingsFlag = isRearDoorAvailable ? frontFlag && rearFlag : frontFlag;
            }
            var controllerConsoles = (from console in groupHallFixtureConsoles
                                      where (console.GroupHallFixtureType == Constants.AGILEHALLSTATION || console.GroupHallFixtureType == Constants.TRADITIONALHALLSTATION) && console.ConsoleId == 1
                                      select console).ToList();
            var selectionFlag = false;
            foreach (var console in controllerConsoles)
            {

                var isRearDoorAvailable = (from console1 in groupHallFixtureConsoles
                                           where (console1.GroupHallFixtureType == Constants.AGILEHALLSTATION || console1.GroupHallFixtureType == Constants.TRADITIONALHALLSTATION) && console1.Openings.Rear && console1.ConsoleId == 1
                                           select console1.Openings.Rear).FirstOrDefault();
                var frontSelections = console.GroupHallFixtureLocations.Any(x => x.Front.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase));
                var rearSelections = false;
                if (isRearDoorAvailable)
                {
                    rearSelections = console.GroupHallFixtureLocations.Any(x => x.Rear.Value.ToString().Equals(Constants.True, StringComparison.OrdinalIgnoreCase));
                }
                selectionFlag = !(frontSelections || rearSelections);
                if (selectionFlag)
                {
                    break;
                }

            }
            return openingsFlag && (!selectionFlag);
        }

        public List<VariableAssignment> SetCacheControlLocationValues(List<VariableAssignment> variables, string sessionId, int setId)
        {
            var methodBeginTime = Utility.LogBegin();
            var controlocValue = variables;
            var username = GetUserId(sessionId);
            if (variables != null && variables.Any())
            {
                _cpqCacheManager.SetCache(username, _environment, Constants.CONTROLLOCATIONVALUE, setId.ToString(), Utility.SerializeObjectValue(controlocValue));
            }
            else
            {
                var controlocget = _cpqCacheManager.GetCache(username, _environment, Constants.CONTROLLOCATIONVALUE, setId.ToString());
                if (!string.IsNullOrEmpty(controlocget))
                {
                    controlocValue = Utility.DeserializeObjectValue<List<VariableAssignment>>(controlocget);
                }
            }
            Utility.LogEnd(methodBeginTime);
            return controlocValue;

        }

        /// <summary>
        /// GetSetDisplayVariableAssignmentsForGroup
        /// </summary>
        /// <param name="displayVariableAssignments"></param>
        /// <param name="groupId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<List<DisplayVariableAssignmentsValues>> GetSetDisplayVariableAssignmentsForGroup(List<DisplayVariableAssignmentsValues> displayVariableAssignments, int groupId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var resultDisplayVariableAssignments = new List<DisplayVariableAssignmentsValues>();
            if (displayVariableAssignments != null && displayVariableAssignments.Count > 0)
            {
                _cpqCacheManager.SetCache(sessionId, _environment, groupId.ToString(), Constants.DISPLAYVARIABLEASSIGNMENTSFORGROUP, Utility.SerializeObjectValue(displayVariableAssignments));
            }
            else
            {
                var cachedDisplayVariableAssignments = _cpqCacheManager.GetCache(sessionId, _environment, groupId.ToString(), Constants.DISPLAYVARIABLEASSIGNMENTSFORGROUP);
                if (!String.IsNullOrEmpty(cachedDisplayVariableAssignments))
                {
                    resultDisplayVariableAssignments = Utility.DeserializeObjectValue<List<DisplayVariableAssignmentsValues>>(cachedDisplayVariableAssignments);
                }
            }
            Utility.LogEnd(methodBeginTime);
            return resultDisplayVariableAssignments;
        }
        private IList<Properties> GetPropertyForCustomPitDepth()
        {
            var getProperties = JArray.Parse(File.ReadAllText(Constants.CUSTOMPROPERTYTEMPLATE));
            var variableProperties = Utility.DeserializeObjectValue<IList<Properties>>(Utility.SerializeObjectValue(getProperties));
            return variableProperties;
        }

        private void UnitLayoutVariablesMapping(ConfigurationResponse response)
        {
            var variablesDictionary = Utility.GetVariableMapping(Constant.FDAMAPPERVARIABLESMAPPERPATH, Constant.FDAVARIABLES);
            response.Sections.Where(sectionItem => (sectionItem!=null) && (sectionItem.Id.Equals(Constant.LAYOUTSECTIONKEY, StringComparison.OrdinalIgnoreCase))).ToList().FirstOrDefault().sections.ToList().ForEach(newSectionItem =>
            {
                newSectionItem.CounterWeightLocation = newSectionItem.Variables?.Where(x => x.Id.Contains(variablesDictionary[Constants.COUNTERWTLOCATION])).ToList().FirstOrDefault();
                newSectionItem.Layout.FirstOrDefault().layoutVariables.Add(newSectionItem.Variables?.Where(x => x.Id.Contains(variablesDictionary[Constants.GOVACCESS])).ToList().FirstOrDefault());
                newSectionItem.Layout.FirstOrDefault().layoutVariables.Add(newSectionItem.Variables?.Where(x => x.Id.Contains(variablesDictionary[Constants.SUMPQTY_SP])).ToList().FirstOrDefault());
                newSectionItem.Variables = new List<Variables>();
            });
        }
    }
}