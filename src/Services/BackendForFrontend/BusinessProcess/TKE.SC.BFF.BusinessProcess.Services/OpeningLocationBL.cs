/************************************************************************************************************
************************************************************************************************************
    File Name     :   OpeningLocationBL.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Configit.Configurator.Server.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class OpeningLocationBL : IOpeningLocation
    {
        /// <summary>
        /// Variable Collection
        /// </summary>
        #region Variables
        /// <summary>
        /// object IOpeningLocationDL
        /// </summary>
        private readonly IOpeningLocationDL _openingLocationdl;
        /// <summary>
        /// object IConfigure
        /// </summary>
        private readonly IConfigure _configure;
        /// <summary>
        /// Object IGroupConfigurationDL
        /// </summary>
        private readonly IGroupConfigurationDL _group;

        private readonly IGroupConfiguration _groupconfiguration;
        private readonly string _environment;
        private readonly ICacheManager _cpqCacheManager;
        private readonly IConfiguration _configuration;
        #endregion

        /// <summary>
        /// Constructor for OpeningLocationBL
        /// </summary>
        /// <param Name="utility"></param>
        /// <param Name="openingLocationdl"></param>
        /// <param Name="configure"></param>
        /// <param Name="group"></param>
        /// <param Name="groupConfiguration"></param>
        public OpeningLocationBL(ILogger<OpeningLocationBL> logger, IOpeningLocationDL openingLocationdl, IConfigure configure, IGroupConfigurationDL group, IGroupConfiguration groupConfiguration, ICacheManager cpqCacheManager, IConfiguration iConfig)
        {
            _configure = configure;
            _group = group;
            _configuration = iConfig;
            _environment = Constant.DEV;
            _openingLocationdl = openingLocationdl;
            _groupconfiguration = groupConfiguration;
            _cpqCacheManager = cpqCacheManager;
            Utility.SetLogger(logger);
        }

        /// <summary>
        /// UpdateOpeningLocation
        /// </summary>
        /// <param Name="openingLocation"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> UpdateOpeningLocation(OpeningLocations openingLocation, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var cachedConsole = _configure.SetCacheOpeningLocation(null, sessionId, openingLocation.GroupConfigurationId);
            var changeLogForOpenings = GetOpeningLocationHistory(cachedConsole, openingLocation);
            var result = _openingLocationdl.UpdateOpeningLocation(openingLocation, changeLogForOpenings);
            if (result == 1)
            {
                Utility.LogEnd((methodBeginTime));
                var updateOpeningLocationresponseArray = new JArray();
                updateOpeningLocationresponseArray.Add(JsonConvert.DeserializeObject(Constant.OPENINGLOCATIONUPDATE));
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = updateOpeningLocationresponseArray };
            }
            else
            {
                Utility.LogEnd((methodBeginTime));
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.SOMETHINGWENTWRONGMSG,
                    Description = Constant.SOMETHINGWENTWRONGMSG
                });
            }


        }

        /// <summary>
        /// GetOpeningLocationByGroupId
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetOpeningLocationByGroupId(int GroupConfigurationId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            
            // Main Response Template Stub 
            var mainUiResponseTemplate = JObject.Parse(File.ReadAllText(Constant.MAINUIRESPONSETEMPLATE));
            var mainUiResponse = mainUiResponseTemplate.ToObject<ConfigurationResponse>();
            var variableAssignments = JObject.FromObject(new Line());
            var configureRequest = _configure.CreateConfigurationRequestWithTemplate(variableAssignments, Constant.GROUPCONFIGURATIONNAME);
            var configureResponseJObj =
                await _configure.ConfigurationBl(configureRequest, configureRequest.PackagePath, sessionId).ConfigureAwait(false);
           
            var configureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = configureResponse.Arguments;
            var configureResponseArgumentJObject = JObject.FromObject(configureResponseArgument);
            // adding defaults to cache
            _configure.SetGroupDefaultsCache(sessionId, configureResponseArgumentJObject);
            var configureRequestDictionary = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();

            var crossPackagevariableDictionary = new Dictionary<string, string>();

            var crossPackageVariableId = (JObject.Parse(File.ReadAllText(Constant.VARIABLEDICTIONARY)));
            JToken crossPackageVariables;
            crossPackageVariables = crossPackageVariableId[Constant.DEFAULTVALUES];
            crossPackagevariableDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(crossPackageVariables));

            var openingLocationVariableAssignments = (from val1 in configureRequestDictionary
                       from val2 in crossPackagevariableDictionary
                       where Utility.CheckEquals(val1.Key.ToString(), val2.Key.ToString())
                       select new VariableAssignment
                       {
                           VariableId = val1.Key,
                           Value = val1.Value
                       }).Distinct().ToList();
            var mapperVariables = _configure.GetMapperVariables();
            var result = _openingLocationdl.GetOpeningLocationBygroupId(GroupConfigurationId, openingLocationVariableAssignments, mapperVariables, sessionId);
            var cachedData = _configure.SetCacheOpeningLocation(result, sessionId, result.GroupConfigurationId);
            result.ReadOnly = _group.CheckUnitConfigured(GroupConfigurationId);
            var rolename = _configure.GetRoleName(sessionId);
            var permission = _openingLocationdl.GetPermissionByRole(GroupConfigurationId, rolename);
            result.Permissions = permission;
            result.ConflictAssignments = _configure.GetConflictAssignments(sessionId);
            //Adding internal conflicts
            if (result.VariableIds != null && result.VariableIds.Any())
            {
                var conflictVariablesData = new List<ConflictMgmtList>();
                foreach (var conflictItems in result.VariableIds)
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
                    var filteredPendingConflictsVariables = conflictVariablesData.GroupBy(x => x.VariableId).Select(s => s.FirstOrDefault()).ToList();
                    if (mainUiResponse.ConflictAssignments != null)
                    {
                        mainUiResponse.ConflictAssignments.PendingAssignments.AddRange(filteredPendingConflictsVariables);
                    }
                    else 
                    {
                        mainUiResponse.ConflictAssignments = new ConflictManagement()
                        {
                            PendingAssignments = filteredPendingConflictsVariables,
                            ResolvedAssignments = new List<ConflictMgmtList>()
                        };
                    }
                    
                }
            }
            foreach (var items in mainUiResponse.Sections)
            {
                if (Utility.CheckEquals(items.Id, Constant.OPENINGLOCATIONSTAB))
                {
                    items.OpeningLocationResponse = result;
                    var cachedFlag = _cpqCacheManager.GetCache(sessionId, _environment, GroupConfigurationId.ToString(), Constants.UHFEXISTSFLAG);
                    if (cachedFlag != null)
                    {
                        var uHFFlag =  cachedFlag.Equals("true") ? true:false;
                        items.OpeningLocationResponse.UHFExists = uHFFlag;
                    }
                    var flagInCache = _cpqCacheManager.GetCache(sessionId, _environment, GroupConfigurationId.ToString(), Constants.SAVEOPENINGLOCATIONSFLAG);
                    if(flagInCache != null)
                    {
                        var saveOpeningLocationFlag = flagInCache.Equals("true") ? true : false;
                        items.OpeningLocationResponse.SaveOpeningLocations = saveOpeningLocationFlag;
                    }
                    break;
                }
            }
            var conflictStatusFlag = false;
            if (mainUiResponse.ConflictAssignments != null)
            {
                conflictStatusFlag = true;
            }
            var resultConflictStatusUpdate = _openingLocationdl.UpdateGroupConflictStatus(GroupConfigurationId, conflictStatusFlag);

            var enrichedData = JObject.Parse(File.ReadAllText(Constant.ELEVATORENRICHMENTTEMPLATE));
            mainUiResponse.EnrichedData = enrichedData;
            if (result.GroupConfigurationId == 0)
            {
                Utility.LogEnd((methodBeginTime));
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.INAVLIDGROUPID,
                    Description = Constant.INAVLIDGROUPID
                });
            }
            else
            {
                Utility.LogEnd(methodBeginTime);
                var openingLocationString = Utility.SerializeObjectValue(mainUiResponse);
                var openingLocationResult = JObject.FromObject(JsonConvert.DeserializeObject(openingLocationString)); 
                return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = openingLocationResult };
            }
        }
        /// <summary>
        /// This method is used to fetch the history data for opening location logs
        /// </summary>
        /// <param Name="cachedLocationData"></param>
        /// <param Name="newLocationData"></param>
        /// <returns></returns>
        public List<LogHistoryTable> GetOpeningLocationHistory(OpeningLocations cachedLocationData, OpeningLocations newLocationData)
        {
            var methodBeginTime = Utility.LogBegin();
            List<LogHistoryTable> logHistoryTable = new List<LogHistoryTable>();
            List<ConsoleHistory> changedOpenings = new List<ConsoleHistory>();
            foreach (var units in newLocationData.Units)
            {
                var cachedUnit = (from unit in cachedLocationData.Units
                                  where unit.UnitName.Equals(units.UnitName)
                                  select unit).ToList().FirstOrDefault();
                if (cachedUnit != null)
                {
                    if (!units.OcuppiedSpace.Equals(cachedUnit?.OcuppiedSpace))
                    {
                        var history = new LogHistoryTable()
                        {
                            VariableId = units.UnitName + Constant.HYPHEN + Constant.OCCUPIEDSPACEBELOW,
                            UpdatedValue = Convert.ToString(units.OcuppiedSpace),
                            PreviuosValue = Convert.ToString(cachedUnit.OcuppiedSpace)
                        };
                        logHistoryTable.Add(history);
                    }
                    foreach (var opening in units.OpeningsAssigned)
                    {
                        if (cachedUnit?.OpeningsAssigned != null)
                        {
                            var cachedOpening = (from cacheOpening in cachedUnit.OpeningsAssigned
                                                 where cacheOpening.FloorDesignation.Equals(opening.FloorDesignation)
                                                 select cacheOpening).ToList().FirstOrDefault();
                            if (cachedOpening != null)
                            {
                                if (opening.Rear != cachedOpening?.Rear)
                                {
                                    changedOpenings.Add(new ConsoleHistory()
                                    {
                                        Console = Constant.OPENINGLOCATIONPASCALCASE,
                                        Parameter = string.Empty,
                                        UnitId = units.UnitName,
                                        FloorNumber = opening.FloorNumber,
                                        Opening = Constant.R,
                                        PresentValue = Convert.ToString(opening.Rear),
                                        PreviousValue = Convert.ToString(cachedOpening.Rear)
                                    });
                                }
                                if (opening.Front != cachedOpening?.Front)
                                {
                                    changedOpenings.Add(new ConsoleHistory()
                                    {
                                        Console = Constant.OPENINGLOCATIONPASCALCASE,
                                        Parameter = string.Empty,
                                        UnitId = units.UnitName,
                                        FloorNumber = opening.FloorNumber,
                                        Opening = Constant.F,
                                        PresentValue = Convert.ToString(opening.Front),
                                        PreviousValue = Convert.ToString(cachedOpening.Front)
                                    });
                                }
                            }
                        }
                    }
                }
            }
            logHistoryTable.AddRange(_groupconfiguration.GetLogHistoryTableForConsole(changedOpenings));
            Utility.LogEnd(methodBeginTime);
            return logHistoryTable;
        }
    }
}
