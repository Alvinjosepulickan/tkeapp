using Configit.Configurator.Server.Common;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stubble.Core.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.Common;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class FieldDrawingAutomationBL : IFieldDrawingAutomation
    {
        #region Variables
        private readonly ICacheManager _cpqCacheManager;
        private readonly IFieldDrawingAutomationDL _fieldDrawingAutomationdl;
        private readonly IConfigure _configure;
        private readonly string _environment;
        private IConfiguration _configuration;
        private readonly IHostingEnvironment _environmentEnv;
        private readonly IGroupConfigurationDL _groupConfiguration;
        private readonly IBuildingConfiguration _buildingConfiguration;
        private readonly IBuildingConfigurationDL _buildingConfiguraionDl;
        private readonly List<LaytouchDetails> lstlaytouch = new List<LaytouchDetails>();
        private readonly IProductSelection _productSelection;
        private readonly IUnitConfigurationDL _unitConfigurationDL;
        #endregion

        /// <summary>
        /// Constructor for GroupConfiguationBL
        /// </summary>
        /// <param Name="fieldDrawingAutomationDL"></param>
        /// <param Name="configure"></param>
        /// <param Name="cpqCacheManager"></param>
        /// <param Name="configuration"></param>
        /// <param Name="logger"></param>
        public FieldDrawingAutomationBL(IFieldDrawingAutomationDL fieldDrawingAutomationDL,
            IConfigure configure,
            ICacheManager cpqCacheManager,
            IConfiguration configuration,
            ILogger<FieldDrawingAutomationBL> logger,
            IHostingEnvironment hostingEnvironment,
            IGroupConfigurationDL groupConfiguration,
            IBuildingConfiguration buildingConfiguration,
            IBuildingConfigurationDL buildingConfiguraionDl,
            IProductSelection productSelection,
            IUnitConfigurationDL unitConfigurationDL)
        {
            _fieldDrawingAutomationdl = fieldDrawingAutomationDL;
            _configure = configure;
            _environment = Constant.DEV;
            _cpqCacheManager = cpqCacheManager;
            _configuration = configuration;
            _environmentEnv = hostingEnvironment;
            _groupConfiguration = groupConfiguration;
            _buildingConfiguration = buildingConfiguration;
            _buildingConfiguraionDl = buildingConfiguraionDl;
            _productSelection = productSelection;
            _unitConfigurationDL = unitConfigurationDL;
            Utility.SetLogger(logger);
        }

        /// <summary>
        /// /Get QuoteId From Cache
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public string GetQuoteIdFromCache(string sessionId)
        {
            string quoteId = string.Empty;
            //Get QuoteId from Cache
            var opportunityDetailsCache = _cpqCacheManager.GetCache(sessionId, _environment, Constant.USERADDRESS);
            if (opportunityDetailsCache != null)
            {
                var opportunityDetails = Utility.DeserializeObjectValue<OpportunityEntity>(opportunityDetailsCache);
                if (opportunityDetails != null)
                {
                    quoteId = opportunityDetails?.QuoteId;
                }
            }
            return quoteId;
        }

        /// <summary>
        /// Method to call the configuration details
        /// </summary>
        /// <param Name="configurationRequest"></param>
        /// <param Name="projectId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> StartFieldDrawingConfigure(JObject variableAssignments, int groupId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            //Get QuoteId from Cache
            var quoteId = GetQuoteIdFromCache(sessionId);
            variableAssignments = new JObject();
            var configurationRequest = CreateGroupConfigurationRequest(variableAssignments);
            List<ConfigVariable> lstConfigureVariable = new List<ConfigVariable>();
            List<ConfigVariable> lstSumpPitQuantity = new List<ConfigVariable>();
            List<UnitLayOutDetails> lstUnitLayoutDetails = new List<UnitLayOutDetails>();
            if (Convert.ToString(groupId) != null && !string.IsNullOrEmpty(quoteId))
            {
                if (groupId != 0)
                {
                    var fieldDrawingContantsDictionary = Utility.GetVariableMapping(Constant.FDAMAPPERVARIABLESMAPPERPATH, Constant.FDAVARIABLES);
                    List<ConfigVariable> lstConfigVariable = new List<ConfigVariable>();
                    foreach (var variable in fieldDrawingContantsDictionary)
                    {
                        ConfigVariable configVariable = new ConfigVariable() { VariableId = variable.Key, Value = variable.Value };
                        lstConfigVariable.Add(configVariable);
                    }

                    var dtVariables = Utility.DeserializeObjectValue<DataTable>(Utility.SerializeObjectValue(lstConfigVariable));

                    string userName = _configure.GetUserId(sessionId);
                    //Get FieldDrawingAutomation Variables
                    lstConfigureVariable = _fieldDrawingAutomationdl.GetFieldDrawingAutomationByGroupId(groupId, quoteId, dtVariables);

                    //Get FieldDrawing Automation layout Data
                    lstUnitLayoutDetails = _fieldDrawingAutomationdl.GetFieldDrawingAutomationLayoutByGroupId(groupId, quoteId, dtVariables);
                    var doorsVariableSelection = new List<VariableAssignment>();
                    if (lstUnitLayoutDetails != null)
                    {
                        //Setting the Layout Details in Cache
                        lstUnitLayoutDetails = _configure.SetCacheFieldDrawingAutomationLayoutDetails(lstUnitLayoutDetails, sessionId, groupId);
                        //Generating doors variables to pass to model
                        doorsVariableSelection.AddRange(GenerateDoorsVariableSelectionForCounterWeight(lstUnitLayoutDetails));
                    }
                    //Converting ConfigureVariable to VariableAssignments
                    List<VariableAssignment> lstvariableassignment = lstConfigureVariable.Select(
                     variableAssignment => new VariableAssignment
                     {
                         VariableId = variableAssignment.VariableId,
                         Value = variableAssignment.Value
                     }).ToList<VariableAssignment>();
                    lstvariableassignment.AddRange(doorsVariableSelection);
                    configurationRequest.Line.VariableAssignments = lstvariableassignment;
                }
            }
            else
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.FDANOTNULLERRORMESSAGE,
                    Description = Constant.FDANOTNULLERRORMESSAGE
                });
            }
            variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configurationRequest.Line));
            var response = await ChangeFieldDrawingConfigure(variableAssignments, groupId, false,
                    sessionId, lstUnitLayoutDetails , quoteId).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = JObject.FromObject(response.Response) };
        }

        /// <summary>
        /// Method to Change Configuration
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="groupId"></param>
        /// <param Name="isChange"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="lstUnitLayoutDetails"></param>
        /// <param Name="lstConfigureVariable"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> ChangeFieldDrawingConfigure(JObject variableAssignments, int groupId, bool isChange, string sessionId, List<UnitLayOutDetails> lstUnitLayoutDetails, string quoteId = "")
        {
            var methodBeginTime = Utility.LogBegin();
            //Get QuoteId from Cache
            quoteId = GetQuoteIdFromCache(sessionId);

            if (isChange)
            {
                var carPositionAssignments = _fieldDrawingAutomationdl.GetLiftDesignerByGroupId(groupId, Constant.SPGGETCARPOSITIONBYGROUPID);
                List<VariableAssignment> lstvariableassignment = carPositionAssignments.Select(
                       variableAssignment => new VariableAssignment
                       {
                           VariableId = variableAssignment.VariableId,
                           Value = variableAssignment.Value
                       }).ToList<VariableAssignment>();
                var dbAssignments = Utility.DeserializeObjectValue<Line>(Utility.SerializeObjectValue(variableAssignments));
                lstvariableassignment.AddRange(dbAssignments.VariableAssignments);
                //For adding doors selection variable in variable assignments to send to model
                var cachedListUnitLayoutDetails = _configure.SetCacheFieldDrawingAutomationLayoutDetails(lstUnitLayoutDetails, sessionId, groupId);
                if (cachedListUnitLayoutDetails != null)
                {
                    lstvariableassignment.AddRange(GenerateDoorsVariableSelectionForCounterWeight(cachedListUnitLayoutDetails));
                }
                var variableAssignmentz = new Line
                {
                    VariableAssignments = lstvariableassignment
                };
                variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(variableAssignmentz));
            }

            var drawingStatusKey = _fieldDrawingAutomationdl.GetFieldDrawingStatusByGroupId(groupId, quoteId);
            var statusKey = _fieldDrawingAutomationdl.GetGroupStatusByGroupId(groupId);
            var projectStatusKey = _fieldDrawingAutomationdl.GetProjectStatusByGroupId(quoteId);
            var roleName = _configure.GetRoleName(sessionId);
            var permissions = _fieldDrawingAutomationdl.GetPermissionForFDA(groupId.ToString(), roleName, Constant.FDA);
            var permissionKey = permissions.Select(x => x.PermissionKey).Distinct().ToList();
            var response = await _configure.StartFieldDrawingConfigure(variableAssignments, groupId, isChange,
            sessionId, lstUnitLayoutDetails, statusKey, projectStatusKey, drawingStatusKey, permissionKey).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return response;
        }

        /// <summary>
        /// Method to Get the Drawing Details
        /// </summary>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetFieldDrawingsByProjectId(string quoteId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var buildingEquipmentContantsDictionary = Utility.GetVariableMapping(Constant.FDAMAPPERVARIABLESMAPPERPATH, Constant.FDAVARIABLES);
            var opportunityDetailsCache = _cpqCacheManager.GetCache(sessionId, _environment, Constant.USERADDRESS);
            if (opportunityDetailsCache != null)
            {
                var opportunityDetails = Utility.DeserializeObjectValue<OpportunityEntity>(opportunityDetailsCache);
                if (opportunityDetails != null)
                {
                    opportunityDetails.QuoteId = quoteId;
                }
                var opportunityDetailsCacheNew = Utility.SerializeObjectValue(opportunityDetails);
                _cpqCacheManager.SetCache(sessionId, _environment, Constant.USERADDRESS, opportunityDetailsCacheNew);
            }
            List<ConfigVariable> lstConfigVariable = new List<ConfigVariable>();
            foreach (var variable in buildingEquipmentContantsDictionary)
            {
                ConfigVariable configVariable = new ConfigVariable() { VariableId = variable.Key, Value = variable.Value };
                lstConfigVariable.Add(configVariable);
            }

            var dtVariables = Utility.DeserializeObjectValue<DataTable>(Utility.SerializeObjectValue(lstConfigVariable));

            var lstDrawingDetails = _fieldDrawingAutomationdl.GetFieldDrawingsByProjectId(quoteId, dtVariables);
            var rolename = _configure.GetRoleName(sessionId);
            var permissions = _fieldDrawingAutomationdl.GetPermissionForFDA(quoteId, rolename, Constant.FDALIST);
            if (lstDrawingDetails.GroupDetailsForDrawings != null && lstDrawingDetails.GroupDetailsForDrawings.Any())
            {
                foreach (var building in lstDrawingDetails.GroupDetailsForDrawings)
                {
                    foreach (var groups in building.GroupDetails)
                    {
                        var permissionGroup = (from permission in permissions
                                               where permission.GroupStatus.Equals(groups.GroupStatus.StatusName)
                                               select permission.PermissionKey).Distinct().ToList();
                        groups.Permissions = permissionGroup;
                    }
                }
            }
            var response = JObject.FromObject(lstDrawingDetails);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = response };
        }

        /// <summary>
        /// Method to Get the Drawing Status By GroupId
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetRequestQueueByGroupId(int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            var lstDrawingDetails = _fieldDrawingAutomationdl.GetRequestQueueByGroupId(groupId);
            var response = JObject.FromObject(lstDrawingDetails);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = response };
        }

        /// <summary>
        /// Method to Calling the Lift Designer
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="projectId"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public Task<ResponseMessage> RequestLDDrawings(int groupId, JObject variableAssignments, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            //Get QuoteId from Cache
            var userName = _configure.GetUserId(sessionId);
            //Get complete information using groupId
            var groupInformation = _fieldDrawingAutomationdl.GetGroupInformation(groupId);
            var quoteId = groupInformation?.FirstOrDefault().QuoteId;
            //Saving the FDA details
            var savedResponse = SaveFda(groupId, variableAssignments, sessionId, quoteId, userName);
            //Updating the Status
            _fieldDrawingAutomationdl.UpdateFDARequestStatusByFieldDrawingId(savedResponse.FieldDrawingId);
            //Calling Drawing Generation and Request Layout Background Job
            var jobId = BackgroundJob.Enqueue(() => RequestDrawings(sessionId, groupId, userName, quoteId, groupInformation));
            ResultGroupConfiguration result = new ResultGroupConfiguration();
            List<ResultGroupConfiguration> lstResult = new List<ResultGroupConfiguration>();
            result.Result = 1;
            result.GroupConfigurationId = groupId;
            result.Message = Constant.FDASAVEPROCESSINGMESSAGE;
            lstResult.Add(result);
            Utility.LogEnd(methodBeginTime);
            return Task.FromResult(new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = JArray.FromObject(lstResult) });
        }

        /// <summary>
        /// Method to Get the Status
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="quoteId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> RefreshFieldDrawingConfigure(int groupId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            //Get QuoteId from Cache
            var quoteId = GetQuoteIdFromCache(sessionId);
            var buildingEquipmentContantsDictionary = Utility.GetVariableMapping(Constant.FDAMAPPERVARIABLESMAPPERPATH, Constant.FDAVARIABLES);
            List<ConfigVariable> lstConfigVariable = new List<ConfigVariable>();
            foreach (var variable in buildingEquipmentContantsDictionary)
            {
                ConfigVariable configVariable = new ConfigVariable() { VariableId = variable.Key, Value = variable.Value };
                lstConfigVariable.Add(configVariable);
            }

            var dtVariables = Utility.DeserializeObjectValue<DataTable>(Utility.SerializeObjectValue(lstConfigVariable));
            var lstStatus = _fieldDrawingAutomationdl.GetLayoutRequestIdWithStatus(groupId, quoteId);
            if (!string.IsNullOrEmpty(lstStatus.statusId))
            {
                if (lstStatus.statusId.Equals(Constant.DWGPENDING) || lstStatus.statusId.Equals(Constant.DWGSUBMITTED))
                {
                    //Pending
                    var userName = _configure.GetUserId(sessionId);
                    //Generate Token
                    var wrapperToken = await _fieldDrawingAutomationdl.GenerateWrapperToken().ConfigureAwait(false);
                    //Calling the Get Layout Status API
                    await _fieldDrawingAutomationdl.GetLayoutStatus(groupId, quoteId, userName, lstStatus.referenceId, lstStatus.fieldDrawingIntegrationMasterId, wrapperToken).ConfigureAwait(false);
                }
            }
            var lstDrawings = _fieldDrawingAutomationdl.GetFieldDrawingsByGroupId(groupId, quoteId, dtVariables);
            var rolename = _configure.GetRoleName(sessionId);
            var permissions = _fieldDrawingAutomationdl.GetPermissionForFDA(quoteId, rolename, Constant.FDALIST);
            if (lstDrawings.GroupDetailsForDrawings != null && lstDrawings.GroupDetailsForDrawings.Any())
            {
                foreach (var building in lstDrawings.GroupDetailsForDrawings)
                {
                    foreach (var groups in building.GroupDetails)
                    {
                        var permissionGroup = (from permission in permissions
                                               where permission.GroupStatus.Equals(groups.GroupStatus.StatusName)
                                               select permission.PermissionKey).Distinct().ToList();
                        groups.Permissions = permissionGroup;
                    }
                }
            }

            var response = JObject.FromObject(lstDrawings);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = response };
        }

        /// <summary>
        /// Method for Calling the Lift Designer and Unit VT package
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="groupId"></param>
        /// <param Name="userName"></param>
        /// <param Name="projectId"></param>
        /// <param Name="groupVariableListData"></param>
        /// <param Name="FieldDrawingId"></param>
        public async Task RequestDrawings(string sessionId, int groupId, string userName, string quoteId, List<GroupInfo> groupInformation)
        {
            var methodBeginTime = Utility.LogBegin();
            //Getting Building,Group and Unit Variables
            var listOfBuildingGroupUnitVariables = await GetBuildingGroupUnitVariableAssignmentsAsync(groupId, sessionId).ConfigureAwait(false);
            //Calling the Lift Designer VT Package
            var drawingMethod = GetLiftDesignerDrawingMethod(sessionId, groupId, listOfBuildingGroupUnitVariables, groupInformation);
            if (drawingMethod.Equals(Constant.AUTOMATED))
            {
                //HangFire Job For XMLGenerationandRequestLayout
                RequestLayoutInfo requestLayoutInfo = await XMLGenerationandRequestLayout(groupId, sessionId, userName, quoteId, listOfBuildingGroupUnitVariables, groupInformation).ConfigureAwait(false);
                RequestLayoutStatus(groupId, quoteId, userName, requestLayoutInfo.ReferenceId, requestLayoutInfo.FieldDrawingIntegrationMasterId);
            }
            Utility.LogEnd(methodBeginTime);
        }

        /// <summary>
        /// Method to Get the Lift Designer Laytouch Status
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="groupId"></param>
        /// <param Name="lstBuildingGroupUnitVariables"></param>
        /// <returns></returns>
        private string GetLiftDesignerDrawingMethod(string sessionId, int groupId, List<UnitVariables> allUnitsVariables, List<GroupInfo> groupInformation)
        {
            var methodBeginTime = Utility.LogBegin();
            int drawingMethod = 0;
            string drawingMethodName = string.Empty;
            //Calling the Unit VT
            var sortedUnitVariables = allUnitsVariables.GroupBy(lst => lst.unitId).Select(grp => grp.ToList()).Distinct().ToList();
            foreach (var unitVariablesCollection in sortedUnitVariables)
            {
                var productName = (from units in groupInformation
                            where units.Unitid.Equals(unitVariablesCollection.FirstOrDefault().unitId)
                            select units.ProductName).ToList().FirstOrDefault();
                var resultunitPackageVariables = GetVariablesFromVTPackage(sessionId, unitVariablesCollection, Constant.UNITDEFAULTSCLMCALL, productName).ConfigureAwait(false).GetAwaiter().GetResult();
                GetLaytouchVariableDetails(sessionId, resultunitPackageVariables, productName).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            //Checking the Laytouch condition
            var laytouchCount = lstlaytouch.Where(x => x.Laytouch.Equals(true)).Select(y => y.Laytouch).ToList();
            if (laytouchCount.Count() > 0)
            {
                drawingMethod = 1;
                drawingMethodName = Constant.MANUALCSC;
            }
            else
            {
                drawingMethod = 2;
                drawingMethodName = Constant.AUTOMATED;
            }
            var response = _fieldDrawingAutomationdl.UpdateFdaDrawingMethodByGroupId(groupId, drawingMethod);
            Utility.LogEnd(methodBeginTime);
            return drawingMethodName;
        }

        /// <summary>
        /// Method to Get the Request Layout Status
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="quoteId"></param>
        /// <param name="userName"></param>
        /// <param name="integratedSystemRef"></param>
        /// <param name="integratedProcessId"></param>
        public void RequestLayoutStatus(int groupId, string quoteId, string userName, string integratedSystemRef, int integratedProcessId)
        {
            var methodBeginTime = Utility.LogBegin();
            var wrapperConfiguration = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constant.DRAWINGSAPISETTINGS);
            var interval = 0;
            if (!int.TryParse(Utility.GetPropertyValue(wrapperConfiguration, Constant.GETREQUESTLAYOUTINTERVAL), out interval))
            {
                interval = 2;
            }
            //HangFire Calling the GetLayoutStatus Based on the Interval
            var jobId = BackgroundJob.Schedule(() => GetLDLayoutStatus(groupId, quoteId, userName, integratedSystemRef, integratedProcessId), TimeSpan.FromMinutes(interval));
            Utility.LogEnd(methodBeginTime);
        }

        /// <summary>
        /// Method to Get the Wrapper Layout Status
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="quoteId"></param>
        /// <param name="userName"></param>
        /// <param name="referenceId"></param>
        /// <param name="fieldDrawingIntegrationMasterId"></param>
        /// <returns></returns>
        public async Task GetLDLayoutStatus(int groupId, string quoteId, string userName, string referenceId, int fieldDrawingIntegrationMasterId)
        {
            var methodBeginTime = Utility.LogBegin();
            //Generate Token
            var WrapperToken = await _fieldDrawingAutomationdl.GenerateWrapperToken().ConfigureAwait(false);
            //Calling the Get Layout Status API
            await _fieldDrawingAutomationdl.GetLayoutStatus(groupId, quoteId, userName, referenceId, fieldDrawingIntegrationMasterId, WrapperToken);
            var lstRequestDetails = _fieldDrawingAutomationdl.CheckRequestIdByFDAIntegrationId(fieldDrawingIntegrationMasterId);
            if (lstRequestDetails.Count() > 0)
            {
                RequestLayoutStatus(groupId, quoteId, userName, lstRequestDetails[0].IntegratedSystemRef, lstRequestDetails[0].IntegratedProcessId);
            }
            Utility.LogEnd(methodBeginTime);
        }

        /// <summary>
        /// Method to Calling the Xml Generation and RequestLayoutInfo 
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="userName"></param>
        /// <param Name="projectId"></param>
        /// <param Name="groupVariableListData"></param>
        /// <param Name="lstBuildingGroupUnitVariables"></param>
        /// <returns></returns>
        public async Task<RequestLayoutInfo> XMLGenerationandRequestLayout(int groupId, string sessionId, string userName, string quoteId, List<UnitVariables> lstBuildingGroupUnitVariables, List<GroupInfo> groupInformation)
        {
            var methodBeginTime = Utility.LogBegin();
            string ldXml = await LDXMLGenerationAsync(groupId, sessionId, lstBuildingGroupUnitVariables, quoteId, groupInformation).ConfigureAwait(false);
            JObject requestLayoutRequestBody = CreateRequestPayloadForRequestLayout(ldXml, groupId, quoteId);
            //Calling the Request Layout API
            RequestLayoutInfo layoutInfo = await GetRequestLayout(requestLayoutRequestBody, groupId, quoteId, userName).ConfigureAwait(false);
            Utility.LogEnd(methodBeginTime);
            return layoutInfo;
        }

        /// <summary>
        /// Method to Save the Field Drawing Details
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="projectId"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveFieldDrawingConfigure(int groupId, JObject variableAssignments, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            //Get QuoteId from Cache
            var quoteId = GetQuoteIdFromCache(sessionId);
            var groupVariableListData = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            //Group variables
            List<ConfigVariable> GroupVariableAssignment = groupVariableListData.Where(oh => !oh.VariableId.Contains(Constant.OUTPUTTYPES) && (!oh.VariableId.Contains(Constant.DRAWINGTYPES))).Select(
           variableAssignment => new ConfigVariable
           {
               VariableId = variableAssignment.VariableId,
               Value = variableAssignment.Value
           }).ToList<ConfigVariable>();
            //Layout Generation Settings
            List<ConfigVariable> fdaVariableAssignment = groupVariableListData.Where(oh => oh.VariableId.Contains(Constant.OUTPUTTYPES) || oh.VariableId.Contains(Constant.DRAWINGTYPES)).Select(
           variableAssignment => new ConfigVariable
           {
               VariableId = variableAssignment.VariableId,
               Value = variableAssignment.Value
           }).ToList<ConfigVariable>();
            //Saving the FDA
            var userName = _configure.GetUserId(sessionId);
            var response = _fieldDrawingAutomationdl.SaveFdaByGroupId(groupId, quoteId, userName, fdaVariableAssignment, GroupVariableAssignment);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = JObject.FromObject(response) };
        }

        /// <summary>
        /// Method to Get RequestId from Layout API
        /// </summary>
        /// <param Name="requestLayoutRequestBody"></param>
        /// <param Name="groupId"></param>
        /// <param Name="projectId"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        private async Task<RequestLayoutInfo> GetRequestLayout(JObject requestLayoutRequestBody, int groupId, string quoteId, string userName)
        {
            var methodBeginTime = Utility.LogBegin();
            var ReferenceId = string.Empty;
            var fieldDrawingIntegrationMasterId = 0;
            //Generate Token
            var wrapperToken = await _fieldDrawingAutomationdl.GenerateWrapperToken().ConfigureAwait(false);
            if (!string.IsNullOrEmpty(wrapperToken))
            {
                //Request Layouts
                ReferenceId = await _fieldDrawingAutomationdl.RequestLayouts(requestLayoutRequestBody, wrapperToken.ToString()).ConfigureAwait(false);
                fieldDrawingIntegrationMasterId = _fieldDrawingAutomationdl.SaveReferenceId(groupId, quoteId, userName, Constant.REFRERENCEIDGENERATED, Constant.DOCGEN, ReferenceId);
            }
            else
            {
                ReferenceId = string.Empty;
                fieldDrawingIntegrationMasterId = _fieldDrawingAutomationdl.SaveReferenceId(groupId, quoteId, userName, Constant.ERROR, Constant.DOCGEN, ReferenceId);
            }
            RequestLayoutInfo layoutInfo = new RequestLayoutInfo
            {
                ReferenceId = ReferenceId,
                FieldDrawingIntegrationMasterId = fieldDrawingIntegrationMasterId
            };
            Utility.LogEnd(methodBeginTime);
            return layoutInfo;
        }

        /// <summary>
        /// Method to Create the RequestBody for Layout API
        /// </summary>
        /// <param Name="groupVariableListData"></param>
        /// <param Name="doc"></param>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        private JObject CreateRequestPayloadForRequestLayout(string ldXml, int groupId, string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            var outputTypes = _fieldDrawingAutomationdl.GetOutputTypesForXMGeneration(groupId);
            // UI Template
            var mainResponseTemplate = JObject.Parse(File.ReadAllText(Constant.STARTFIELDDRAWINGAUTOMATIONMAINTEMPLATEPATH));
            var appSections = Utility.GetTokens(Constant.TOKENSECTIONS, mainResponseTemplate, false);
            var layoutGenerationSection = appSections.AsEnumerable().Where(s => Convert.ToString(s[Constant.TOKENID]).Equals(Constant.OUTPUTSECTIONKEY, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            var layoutSection = layoutGenerationSection.ToObject<Sections>();
            List<string> outputTypeList = new List<string>();
            if (outputTypes.Count() > 0)
            {
                foreach (var variable in layoutSection.Variables)
                {
                    foreach (var outputType in outputTypes)
                    {
                        if (Utility.CheckEquals(variable.Id, outputType.fdaType))
                        {
                            outputTypeList.Add(variable.Name);
                        }
                    }
                }
            }

            RequestBody reqBody = new RequestBody()
            {
                TargetLanguage = Constant.ENUS,
                OutputTypes = outputTypeList,
                PlotStyle = Constant.DEFAULTPLOTSTYLE,
                ExternalSystemIdentifier = quoteId,
                LDLayoutXML = ldXml
            };

            var requestLayoutRequestBody = JObject.FromObject(reqBody, new Newtonsoft.Json.JsonSerializer());
            Utility.LogEnd(methodBeginTime);
            return requestLayoutRequestBody;
        }

        /// <summary>
        /// Method to Get the Cross Package Variables
        /// </summary>
        /// <param name="lstBuildingGroupVariablesWithUnits"></param>
        /// <param name="crossPackageKey"></param>
        /// <returns></returns>
        public List<UnitVariables> GetCrossPackageVariables(List<UnitVariables> lstVariables, string path, string crossPackageKey, bool isHeatandLD, int unitId = 0, int groupId = 0)
        {
            var methodBeginTime = Utility.LogBegin();
            List<UnitVariables> CrossPackageAssignments = new List<UnitVariables>();
            var crossPackagevariableDictionaryObj = new Dictionary<string, string>();
            var allcrossPackageVariables = (JObject.Parse(File.ReadAllText(path)));
            JToken crossPackageVariableItems;
            crossPackageVariableItems = allcrossPackageVariables[crossPackageKey];
            crossPackagevariableDictionaryObj = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(crossPackageVariableItems));

            if (!isHeatandLD)
            {
                CrossPackageAssignments = (from val1 in lstVariables
                                           from val2 in crossPackagevariableDictionaryObj
                                           where Utility.CheckEquals(val1.VariableId.ToString(), val2.Key.ToString())
                                           select new UnitVariables
                                           {
                                               VariableId = val2.Value,
                                               Value = val1.Value,
                                               groupConfigurationId = val1.groupConfigurationId,
                                               unitId = val1.unitId,
                                               name = val1.name,
                                               MappedLocation = val1.MappedLocation
                                           }).Distinct().ToList();
            }
            else
            {
                CrossPackageAssignments = (from val1 in lstVariables
                                           from val2 in crossPackagevariableDictionaryObj
                                           where Utility.CheckEquals(val1.VariableId.ToString(), val2.Key.ToString())
                                           select new UnitVariables
                                           {
                                               VariableId = val2.Value,
                                               Value = val1.Value,
                                               unitId = unitId,
                                               groupConfigurationId = groupId
                                           }).Distinct().ToList();
            }

            Utility.LogEnd(methodBeginTime);
            return CrossPackageAssignments;
        }


        /// <summary>
        /// Method to Getting the Building Group and Unit Variable Assignments
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<List<UnitVariables>> GetBuildingGroupUnitVariableAssignmentsAsync(int groupId, string sessionId )
        {
            var methodBeginTime = Utility.LogBegin();
            var ldContantsDictionary = Utility.GetVariableMapping(Constant.LIFTDESIGNERXMLGENERATIONMAPPERSTUBPATH, Constant.BUILDINGANDUNITVARIABLESCONSTANTS);
            List<ConfigVariable> lstConfigVariable = new List<ConfigVariable>();
            foreach (var variable in ldContantsDictionary)
            {
                ConfigVariable configVariable = new ConfigVariable() { VariableId = variable.Key, Value = variable.Value };
                lstConfigVariable.Add(configVariable);
            }
            var configVariables = Utility.DeserializeObjectValue<DataTable>(Utility.SerializeObjectValue(lstConfigVariable));
            var allVariables = new List<UnitVariables>();
            var buildingVariables = _buildingConfiguraionDl.GetBuildingVariables(groupId, configVariables);
            var groupVariables = _groupConfiguration.GetGroupVariables(groupId, configVariables);
            var unitVariables = _unitConfigurationDL.GetUnitsVariables(groupId, configVariables);
            foreach (var variable in groupVariables.Where(x => x.VariableId.Contains(Constant.ELEVATOR, StringComparison.OrdinalIgnoreCase)).ToList())
            {
                var variableId = variable?.VariableId.Split(Constant.DOT);
                variable.VariableId = variable?.VariableId.Replace(variableId.FirstOrDefault(), Constant.ELEVATOR);
            }
            allVariables.AddRange(GetCrossPackageVariables(buildingVariables, Constant.CROSSPACKAGEVARIABLEMAPPING, Constant.BUILDINGTOUNITCROSSPACKAGEVARIABLE, false));
            allVariables.AddRange(GetCrossPackageVariables(groupVariables, Constant.CROSSPACKAGEVARIABLEMAPPING, Constant.GROUPTOUNITCROSSPACKAGEVARIABLE, false));
            allVariables.AddRange(unitVariables);
            Utility.LogEnd(methodBeginTime);
            return allVariables;
        }

        /// <summary>
        /// Method to Get the Dictuionary object By Path and Key
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetCrossPackageVariableDictionary(string path, string key)
        {
            var methodBeginTime = Utility.LogBegin();
            var mainSectionVariables = (JObject.Parse(File.ReadAllText(path)));
            var crossPackageMainVariables = mainSectionVariables[key];
            var crossPackagevariableDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(crossPackageMainVariables));
            Utility.LogEnd(methodBeginTime);
            return crossPackagevariableDictionary;
        }

        /// <summary>
        /// Method to Getting the Input Variable Assignments
        /// </summary>
        /// <param name="resultUnitPackageVariables"></param>
        /// <returns></returns>
        public List<VariableAssignment> GetInputVariableAssignments(string packageType, List<UnitVariables> resultUnitPackageVariables, Dictionary<string, string> buildingAndGroupValues)
        {
            var lstPackageVariableAssignment = GetUnitVariablestoDictionary(packageType);
            foreach (var variable in lstPackageVariableAssignment)
            {
                foreach (var bldGrpVariable in buildingAndGroupValues)
                {
                    if (variable.VariableId.Split(Constant.DOT).LastOrDefault().Equals(bldGrpVariable.Key.Split(Constant.DOT).LastOrDefault()))
                    {
                        variable.Value = bldGrpVariable.Value;
                    }
                }
            }
            foreach (var section in lstPackageVariableAssignment)
            {
                foreach (var unitvariable in resultUnitPackageVariables)
                {
                    string key = string.Concat(Constant.ELEVATOR_DOT + section.VariableId);
                    if (key.Contains(unitvariable.VariableId))
                    {
                        section.Value = unitvariable.Value;
                    }
                }
            }
            return lstPackageVariableAssignment;
        }

        public async Task<IDictionary<string, object>> GetGroupVariablesAsync(int groupId, string sessionId)
        {
            var groupLayout = _groupConfiguration.GetGroupConfigurationDetailsByGroupId(groupId, "", sessionId);
            var groupAssignments = groupLayout.ConfigVariable.Select(x => new VariableAssignment() { VariableId = x.VariableId, Value = x.Value });

            var variableAssignments = JObject.FromObject(new Line());
            var configureRequest = _configure.CreateConfigurationRequestWithTemplate(variableAssignments, Constant.GROUPCONFIGURATIONNAME);
            configureRequest.Line.VariableAssignments = groupAssignments.ToList();
            var configureResponseJObj =
                await _configure.ConfigurationBl(configureRequest, configureRequest.PackagePath, sessionId).ConfigureAwait(false);

            var configureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = configureResponse.Arguments;
            var configureResponseArgumentJObject = JObject.FromObject(configureResponseArgument);
            var groupResponse = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();

            return groupResponse;

        }


        public IList<UnitVariables> GetHallRiserQuadrants( IDictionary<string, object> groupVariableAssignments, string carPosition)
        {
            var riserQuadrants = new List<UnitVariables>();
            var riserLocations = groupVariableAssignments.Where(x => x.Key.Contains(".HS") && Convert.ToBoolean(x.Value)).Select( x => x.Key);

            //B1
            if (carPosition.StartsWith("B1"))
            {
                GetQuandrant(int.Parse(carPosition.Split("B1P")[1]));
            }

            //B2
            if (carPosition.StartsWith("B2"))
            {
                GetQuandrant(int.Parse(carPosition.Split("B2P")[1])+4);
            }

            return riserQuadrants;

            void GetQuandrant(int carPositionNo)
            {
                foreach (var item in riserLocations)
                {
                    var riserNo = int.Parse(Regex.Match(item, "\\d").Value);
                    var isFront = Regex.IsMatch(item, "HS\\dF");
                    var quadrant = isFront ? "Q3":"Q2" ;
                    if ( riserNo <= carPositionNo)
                    {
                        quadrant = isFront ? "Q4" : "Q1";
                    }
                    var defaultId = isFront ? $"HS1QUAD" : $"HS3QUAD";
                    riserQuadrants.Add(new UnitVariables()
                    {
                        VariableId = riserQuadrants.Any(x => x.VariableId.Equals(defaultId)) ? isFront ? $"HS2QUAD" : $"HS4QUAD" : defaultId ,
                        Value = quadrant
                    });

                }
            }


        }


        /// <summary>
        /// Method to Calling the Unit VT Package for XML Generation
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="lstBuildingGroupUnitVariables"></param>
        /// <returns></returns>
        public async Task<string> LDXMLGenerationAsync(int groupId, string sessionId, List<UnitVariables> lstBuildingGroupUnitVariables, string quoteId, List<GroupInfo> groupInformation)
        {
            var methodBeginTime = Utility.LogBegin();
            var userName = _configure.GetUserId(sessionId);
            var requestPayLoad = File.ReadAllText(Constant.REQUEST_TEMPLATE);
            StringBuilder builder = new StringBuilder();
            var ldVariableDictionary = Utility.GetVariableMapping(Constant.LIFTDESIGNERXMLGENERATIONMAPPERSTUBPATH, Constant.LDVARIABLEMAPPER);
            List<ConfigVariable> configureVariables = new List<ConfigVariable>();
            foreach (var variable in ldVariableDictionary)
            {
                ConfigVariable configVariable = new ConfigVariable() { VariableId = variable.Key, Value = variable.Value };
                configureVariables.Add(configVariable);
            }
            var constantMapperVariables = Utility.DeserializeObjectValue<DataTable>(Utility.SerializeObjectValue(configureVariables));
            //Getting the Units and its details
            var unitVariables = lstBuildingGroupUnitVariables.GroupBy(lst => lst.unitId).Select(grp => grp.ToList()).Distinct().ToList();
            //Getting quote details 
            var quoteDetails = GetQuoteInfoForLD(quoteId,sessionId);
            var groupAssignments = await GetGroupVariablesAsync(groupId, sessionId).ConfigureAwait(false);
            var lstDBValues = _fieldDrawingAutomationdl.GetXMLVariablesWithUnitByGroupId(groupId, constantMapperVariables);
            var unitDBValues = lstDBValues.GroupBy(lst => lst.unitId).Select(grp => grp.ToList()).Distinct().ToList();
            var buildingId = groupInformation.FirstOrDefault().BuildingId;
            var buildingValues = await GetBuildingConfigurationByGroupIdAsync(buildingId, sessionId).ConfigureAwait(false);
            var crossPackageValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(await _configure.GetCrossPackageVariableDefaultValues(null, groupId, sessionId).ConfigureAwait(false));
            var groupInfo = _groupConfiguration.GetGroupInformationByGroupId(groupId);

            foreach (var unitVariablesCollection in unitVariables)
            {
                var productName = (from units in groupInformation
                                   where units.Unitid.Equals(unitVariablesCollection.FirstOrDefault().unitId)
                                   select units.ProductName).ToList().FirstOrDefault();
                var unitInfo = await _productSelection.GetUnitVariableAssignments(new List<int> { unitVariablesCollection.Select(x =>x.unitId).First() }, sessionId, Constants.FDAVALUE).ConfigureAwait(false);
                var groupInfo2 = await GetGroupInfoAsync(unitInfo.Response, sessionId).ConfigureAwait(false);
                var unitDetails = _groupConfiguration.GetUnitDetails(groupId, null);

                //combining building group variables with default values included
                Dictionary<string, string> buildingAndGroupValues = buildingValues.ToDictionary(k => k.Key, k => (k.Value).Equals(null) ? string.Empty : k.Value.ToString());
                foreach (var data in crossPackageValues)buildingAndGroupValues.Add(data.Key, data.Value);
                //Getting all default and saved values by calling Unit,Heat, Bracket and LD VT Packages
                var allRequiredVariables = GetAllVariablesForPayloadGeneration(sessionId, productName, unitVariablesCollection, buildingAndGroupValues);
                //Saving default Arguments of LD 
                _fieldDrawingAutomationdl.SaveVariableArguments(groupId, allRequiredVariables, userName);

                var consolidatedList = new Dictionary<string, string>();
                var dummyCollection = new List<UnitVariables>();
                dummyCollection.AddRange(unitDBValues.Where(_ => _.Any(x => x.unitId == unitVariablesCollection.First().unitId)).FirstOrDefault());
                dummyCollection.AddRange(unitVariablesCollection);
                dummyCollection.AddRange(buildingValues.Select( x => new UnitVariables() { VariableId = x.Key, Value = x.Value }));
                dummyCollection.AddRange(crossPackageValues.Select(x => new UnitVariables() { VariableId = x.Key, Value = x.Value }));
                dummyCollection.AddRange(groupInfo.Select( x=> new UnitVariables() {VariableId = x.VariableId, Value = x.Value }));
                dummyCollection.AddRange(allRequiredVariables);
                foreach (var item in dummyCollection)
                {
                    var name = item.VariableId.Split(".").TakeLast(1).FirstOrDefault();
                    var value = item.Value.ToString();
                    if (consolidatedList.ContainsKey(name))
                    {
                        if (string.IsNullOrEmpty(consolidatedList[name]))
                        {
                            consolidatedList[name] = value;
                        }
                        continue;
                    }
                    consolidatedList.Add(name, value);

                }
                if( consolidatedList.TryGetValue("CARPOS", out string carPosition))
                {
                    foreach (var item in GetHallRiserQuadrants(groupAssignments, carPosition))
                    {
                        consolidatedList.TryAdd(item.VariableId, item.Value.ToString());
                    } 
                }
                //Adding all enrichments
                GetAllEnrichmentFunctionForLD(consolidatedList, quoteDetails);
                if (builder.Length == 0)
                {
                    var commonTag = BindData(Constant.COMMON_TAG_TEMPLATE, consolidatedList);
                    requestPayLoad = requestPayLoad.Replace("##commonTag", commonTag);

                }
                var unitTag = BindData(Constant.UNIT_TAG_TEMPLATE, consolidatedList);
                builder.AppendLine(unitTag);
            }
            string BindData(string templatePath, IDictionary<string, string> data)
            {
                var stubble = new StubbleBuilder().Build();
                var template = File.ReadAllText(templatePath);
                return stubble.Render(template, data);
            }
            requestPayLoad = requestPayLoad.Replace("##unitsTag", builder.ToString());
            string fileName = string.Concat(Constant.XMLPREFIXNAME + groupId + Constant.UNDERSCORE + DateTime.Now.ToString(Constant.DDMMYY) + Constant.XMLEXTENSION);
            string xmlDirectory = _environmentEnv.ContentRootPath + Constant.XMLFILEPATH;
            Utility.CreateAndWriteFile(xmlDirectory, fileName, requestPayLoad);
            Utility.LogEnd(methodBeginTime);
            return requestPayLoad;
        }

        

        /// <summary>
        /// Method for Getting the Mappers
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<VariableAssignment> GetUnitVariablestoDictionary(string type)
        {
            var methodBeginTime = Utility.LogBegin();
            string mapperName = string.Empty;

            if (Utility.CheckEquals(type, Constant.HEAT))
            {
                mapperName = Constant.HEATINPUTVARIABLEMAPPER;
            }
            else if (Utility.CheckEquals(type, Constant.BRACKET))
            {
                mapperName = Constant.BRACKETINPUTVARIABLEMAPPER;
            }

            var contantsDictionary = Utility.GetVariableMapping(Constant.LIFTDESIGNERXMLGENERATIONMAPPERSTUBPATH, mapperName);
            var packageVariables = JObject.FromObject(contantsDictionary, new Newtonsoft.Json.JsonSerializer());

            var crossPackagevariableDictionary = new Dictionary<string, string>();
            crossPackagevariableDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(packageVariables));

            List<VariableAssignment> lstcrosspackagevariableassignment = crossPackagevariableDictionary.Select(
                 variableAssignment => new VariableAssignment
                 {
                     VariableId = variableAssignment.Key,
                     Value = variableAssignment.Value
                 }).ToList<VariableAssignment>();

            Utility.LogEnd(methodBeginTime);
            return lstcrosspackagevariableassignment;
        }

        public static string MapPath(string path)
        {
            return Path.Combine(
                (string)AppDomain.CurrentDomain.GetData("ContentRootPath"),
                path);
        }


        /// <summary>
        /// Method for Getting the defaultVariables For LiftDesiger
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="lstBuildingGroupUnitVariableAssignments"></param>
        /// <returns></returns>
        public async Task<List<UnitVariables>> GetVariablesFromVTPackage(string sessionId, List<UnitVariables> lstBuildingGroupUnitVariableAssignments = null, string configurationName = null, string productName = null, bool isVariableAssigments = false, List<VariableAssignment> crosspackageVariableAssignments = null)
        {
            var methodBeginTime = Utility.LogBegin();
            List<VariableAssignment> lstcrosspackagevariableassignment = new List<VariableAssignment>();
            if (!isVariableAssigments)
            {
                if (lstBuildingGroupUnitVariableAssignments != null)
                {
                    lstcrosspackagevariableassignment = lstBuildingGroupUnitVariableAssignments.Select(
                            variableAssignment => new VariableAssignment
                            {
                                VariableId = variableAssignment.VariableId,
                                Value = variableAssignment.Value
                            }).ToList<VariableAssignment>();
                }
            }
            else
            {
                if (crosspackageVariableAssignments != null)
                {
                    lstcrosspackagevariableassignment = crosspackageVariableAssignments;
                }
            }
            var variableAssignmentz = new Line
            {
                VariableAssignments = lstcrosspackagevariableassignment
            };
            var variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(variableAssignmentz));
            productName = (productName!=null)?productName: Convert.ToString(lstBuildingGroupUnitVariableAssignments.Where(x => x.VariableId.Equals("ProductName", StringComparison.OrdinalIgnoreCase))?.Select(x=>x.Value).FirstOrDefault());
            var configureRequest = _configure.CreateConfigurationRequestWithTemplate(variableAssignments, configurationName,null, productName);
            var configureResponseJObj = await _configure.ConfigurationBl(configureRequest, configureRequest.PackagePath, sessionId).ConfigureAwait(false);
            // configuration object values 
            var configureResponse = configureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var configureResponseArgument = configureResponse.Arguments;
            var response = FilterArgumentVariables(configureResponseArgument);
            return response;
        }

        /// <summary>
        /// Method for Getting the Lift Designer Package Variables
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="lstVariableAssignments"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<List<UnitVariables>> GetLiftDesignerHeatBracketPackageVariables(string sessionId, List<VariableAssignment> lstVariableAssignments, string type, string productName = null)
        {
            var methodBeginTime = Utility.LogBegin();
            string configuration = string.Empty;
            if (Utility.CheckEquals(type, Constant.HEAT))
            {
                configuration = Constant.LIFTDESIGNERHEATDEFAULTSCLMCALL;
            }
            else if (Utility.CheckEquals(type, Constant.BRACKET))
            {
                configuration = Constant.LIFTDESIGNERBRACKETDEFAULTSCLMCALL;
            }

            var lstArguments = GetVariablesFromVTPackage(sessionId, null, configuration, productName, true, lstVariableAssignments).ConfigureAwait(false).GetAwaiter().GetResult();
            Utility.LogEnd(methodBeginTime);
            return lstArguments;
        }

        /// <summary>
        /// Method for Getting the Variables For LiftDesigner
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="lstBuildingGroupUnitVariableAssignments"></param>
        /// <returns></returns>
        public async Task<List<LaytouchDetails>> GetLaytouchVariableDetails(string sessionId, List<UnitVariables> lstVariableAssignments, string productName)
        {
            var methodBeginTime = Utility.LogBegin();
            //Compare and Get Cross package Variables
            var crossPackageAssignments = GetCrossPackageVariables(lstVariableAssignments, Constant.XMLGENERATIONVARIABLEMAPPING, Constant.CrossPackageVariablesUnittoLD, true);
            var lstArguments = GetVariablesFromVTPackage(sessionId, crossPackageAssignments, Constant.LIFTDESIGNERDEFAULTSCLMCALL, productName).ConfigureAwait(false).GetAwaiter().GetResult();
            var configureResponseArgumentJObject = lstArguments.GroupBy(c => c.VariableId).ToDictionary(k => k.Key, v => v.Select(f => f.Value).ToList());
            var ldContantsDictionary = Utility.GetVariableMapping(Constant.LIFTDESIGNERXMLGENERATIONMAPPERSTUBPATH, Constant.LAYTOUCH);
            string layTouch = ldContantsDictionary.Select(x => x.Value).FirstOrDefault();
            var laytouch = configureResponseArgumentJObject.Where(x => x.Key.Equals(layTouch)).Select(x => x.Value.First()).FirstOrDefault();
            LaytouchDetails laytouchdetails = new LaytouchDetails
            {
                Laytouch = Convert.ToBoolean(laytouch)
            };
            lstlaytouch.Add(laytouchdetails);
            Utility.LogEnd(methodBeginTime);
            return lstlaytouch;
        }


        /// <summary>
        /// Method for Getting the VariableAssignments for XML Generation
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="lstVariableAssignments"></param>
        /// <returns></returns>
        public async Task<List<UnitVariables>> GettingVariablesForLiftDesignerXmlGeneration(string sessionId, List<UnitVariables> lstVariableAssignments, string productName)
        {
            var methodBeginTime = Utility.LogBegin();
            //Compare and Get Cross package Variables
            var crossPackageAssignments = GetCrossPackageVariables(lstVariableAssignments, Constant.XMLGENERATIONVARIABLEMAPPING, Constant.CrossPackageVariablesUnittoLD, true);
            var lstArguments = GetVariablesFromVTPackage(sessionId, crossPackageAssignments, Constant.LIFTDESIGNERDEFAULTSCLMCALL, productName).ConfigureAwait(false).GetAwaiter().GetResult();
            Utility.LogEnd(methodBeginTime);
            return lstArguments;
        }

        /// <summary>
        /// this method to create group configuration request body
        /// </summary>
        /// <param Name="varibleAssignments"></param>
        /// <returns></returns>
        private ConfigurationRequest CreateGroupConfigurationRequest(JObject varibleAssignments)
        {
            var methodBeginTime = Utility.LogBegin();
            //creating req body
            var stubReqbody = JObject.Parse(System.IO.File.ReadAllText(Constant.GROUPCONFIGURATIONREQESTBODYSTUBPATH)).ToString();
            var configurationRequest = Utility.DeserializeObjectValue<ConfigurationRequest>(stubReqbody);
            configurationRequest.Date = DateTime.Now;
            var objLine = Utility.DeserializeObjectValue<Line>(varibleAssignments.ToString());
            configurationRequest.Line.VariableAssignments = objLine.VariableAssignments;
            Utility.LogEnd(methodBeginTime);
            return configurationRequest;
        }

        /// <summary>
        /// Method to Get the locked details of the group
        /// </summary>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        public Task<ResponseMessage> UpdateLockedGroupsByProjectId(string sessionId, int groupid, string islock)
        {
            var methodBeginTime = Utility.LogBegin();
            //Get QuoteId from Cache
            var quoteId = GetQuoteIdFromCache(sessionId);
            var result = _fieldDrawingAutomationdl.UpdateLockPropertyForGroups(quoteId, groupid, islock);
            Utility.LogEnd(methodBeginTime);
            return Task.FromResult(new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = JArray.FromObject(result) });
        }

        /// <summary>
        ///  Method to Save the Coordination Details
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <param Name="sendToCoordinationObj"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveSendToCoordination(string quoteId, JObject sendToCoordinationObj, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var coordinationData = Utility.DeserializeObjectValue<List<SendToCoordinationData>>(Utility.SerializeObjectValue(sendToCoordinationObj[Constant.SENDTOCOORDINATIONDATA]));
            var userName = _configure.GetUserId(sessionId);
            var response = _fieldDrawingAutomationdl.SaveSendToCoordination(quoteId, userName, coordinationData);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = JArray.FromObject(response) };
        }

        /// <summary>
        /// To get coordination Questions for the groups
        /// </summary>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetSendToCoordinationByProjectId(string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            var lstDrawingDetails = _fieldDrawingAutomationdl.GetSendToCoordinationByProjectId(quoteId);
            var response = JObject.FromObject(lstDrawingDetails);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = response };
        }
        /// <summary>
        /// method to get send to coordination status
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetSendToCoordinationStatus(string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            var coordinationStatus = _fieldDrawingAutomationdl.GetSendToCoordinationStatus(quoteId);
            var response = JObject.FromObject(coordinationStatus);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = Utility.FilterNullValues(response) };
        }
        
        private async Task<Dictionary<string, object>> GetBuildingConfigurationByGroupIdAsync(int buildingId, string sessionId)
        {
            var methodStartTime = Utility.LogBegin();
            JObject variableAssignments = null;
            // get the constant Values 
            var buildingContantsDictionary = Utility.GetVariableMapping(Constant.BUILDINGMAPPERVARIABLESMAPPERPATH, Constant.BUILDINGMAPPERCONFIGURATION);
            var configurationRequest = _buildingConfiguration.CreateBuildingConfigurationRequest(variableAssignments);
            var lstConfigureVariable = new List<ConfigVariable>();
            
               lstConfigureVariable = _buildingConfiguraionDl.GetBuildingConfigurationById(buildingId);
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
                variableAssignments = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configurationRequest.Line));
            //Sending the Variable Assignment Value into VTPKG
            var (response, configureresponse) = await _configure.ChangeBuildingConfigure(variableAssignments, sessionId).ConfigureAwait(false);
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureresponse.Arguments));
            var configureRequestDictionary = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();
            Utility.LogEnd(methodStartTime);
            return configureRequestDictionary.Where(x => !x.Key.Contains("Landing", StringComparison.OrdinalIgnoreCase)).ToDictionary( x => x.Key, x=> x.Value);
        }

        private async Task<Dictionary<string, string>> GetGroupInfoAsync(JObject variableAssignments, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var groupVariablesForFlags = new List<string>();
            var groupVariablesWithValues = new List<VariableAssignment>();
            var groupVariableValues = new List<VariableAssignment>();

            ConfigurationRequest configureRequest;

            var GroupConfigurationRequest = _configure.CreateProductConfigurationRequest(variableAssignments);

            var cachedUserDetail = _cpqCacheManager.GetCache(sessionId, _environment, Constant.USERDETAILSCPQ);
            User userDetail = new User();
            if (!string.IsNullOrEmpty(cachedUserDetail))
            {
                userDetail = Utility.DeserializeObjectValue<User>(cachedUserDetail);
            }
            var variableAssignmentList = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString());

            string product = Constant.PRODUCTSELECTION;
            configureRequest = _configure.CreateConfigurationRequestWithTemplate(variableAssignments, product);

            var mainGroupConfigurationResponse = new ConfigurationResponse
            {
                Sections = new List<Sections>()
            };
            var packagePath = configureRequest?.PackagePath;
            var groupPackagePath = GroupConfigurationRequest?.PackagePath;

            var groupConfigureResponseJObj = await _configure.ConfigurationBl(GroupConfigurationRequest, groupPackagePath, sessionId).ConfigureAwait(false);
            var groupConfigureResponse = groupConfigureResponseJObj.Response.ToObject<StartConfigureResponse>();
            var groupConfigureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(groupConfigureResponse.Arguments));
            var groupConfigureDictionary = groupConfigureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, string>>();
            return groupConfigureDictionary;
           
        }

        public string GetLDResponseJson(int groupId)
        {
            return _fieldDrawingAutomationdl.GetLDResponseJson(groupId);
        }

        private List<VariableAssignment> GenerateDoorsVariableSelectionForCounterWeight(List<UnitLayOutDetails> listUnitLayoutDetails)
        {
            var groupContantsDictionary = Utility.GetVariableMapping(Constant.GROUPMAPPERVARIABLES, Constant.GROUPMAPPERCONFIGURATION);
            var doorsVariables = new List<VariableAssignment>();
            listUnitLayoutDetails.ForEach(variablesItem =>
            {
                var variablesId = groupContantsDictionary[Constants.FRONTDOORTYPEANDHAND];
                var elevatorName = Utility.MapElevatorNameFromModelToUI(variablesItem.displayCarPosition);
                variablesId = String.Concat(elevatorName, Constants.DOT, variablesId);
                if (variablesItem != null)
                {
                    doorsVariables.Add(new VariableAssignment()
                    {
                        VariableId = variablesId,
                        Value = variablesItem.doorOpenings.frontDoor.doorTypeHand
                    });
                }
            });
            var sortedDoorsVariables = doorsVariables.OrderBy(x=> x.VariableId).ToList();
            var count = 1;
            foreach (var variable in sortedDoorsVariables)
            {
               variable.VariableId = variable.VariableId.Replace(variable.VariableId.Split(Constant.DOT).FirstOrDefault(), String.Concat(Constant.ELEVATORSVALUE, count));
                count = count + 1;
            }
            return sortedDoorsVariables;
        }

        public async Task<ResponseMessage> SaveUpdateFieldDrawingStatus(JObject variableAssignments)
        {
            var methodBeginTime = Utility.LogBegin();
            var groupVariableListData = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            var response = _fieldDrawingAutomationdl.UpdateStatusForFDA(groupVariableListData.Where(x=>x.VariableId.Contains(Constants.GUID)).FirstOrDefault().Value.ToString(),
                groupVariableListData.Where(x => x.VariableId.Contains(Constants.STATUS)).FirstOrDefault().Value.ToString());
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = JObject.Parse(response.ToString()) };
        }
        private ResultGroupConfiguration SaveFda(int groupId, JObject variableAssignments, string sessionId, string quoteId, string userName)
        {
            var groupVariableListData = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
            //Group variables
            List<ConfigVariable> groupVariableAssignment = groupVariableListData.Where(variable => !variable.VariableId.Contains(Constant.OUTPUTTYPES) && (!variable.VariableId.Contains(Constant.DRAWINGTYPES))).Select(
           variableAssignment => new ConfigVariable
           {
               VariableId = variableAssignment.VariableId,
               Value = variableAssignment.Value
           }).ToList<ConfigVariable>();
            //Layout Generation Settings
            List<ConfigVariable> fdaVariableAssignment = groupVariableListData.Where(variable => variable.VariableId.Contains(Constant.OUTPUTTYPES) || variable.VariableId.Contains(Constant.DRAWINGTYPES)).Select(
           variableAssignment => new ConfigVariable
           {
               VariableId = variableAssignment.VariableId,
               Value = variableAssignment.Value
           }).ToList<ConfigVariable>();
            //Saving the FDA
            Utility.LogDebug(Constant.SAVEFIELDDRAWINGAUTOMATIONBYGROUPIDINITIATEDDL);

            var response = _fieldDrawingAutomationdl.SaveFdaByGroupId(groupId, quoteId, userName, fdaVariableAssignment, groupVariableAssignment);
            Utility.LogDebug(Constant.SAVEFIELDDRAWINGAUTOMATIONBYGROUPIDCOMPLETEDL);
            return response;
        }

        public List<UnitVariables> FilterArgumentVariables(object configureResponse)
        {
            var methodBeginTime = Utility.LogBegin();
            var configureResponseArgumentJObject = Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(configureResponse));
            var configureArgumentDictionary = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, object>>();
            List<UnitVariables> listOfArguments = configureArgumentDictionary.Select(
                   variableAssignment => new UnitVariables
                   {
                       VariableId = variableAssignment.Key,
                       Value = variableAssignment.Value
                   }).ToList<UnitVariables>();
            Utility.LogEnd(methodBeginTime);
            listOfArguments.RemoveAll(x => x.VariableId.Contains(Constant.ERRORVALUE));
            return listOfArguments;
        }

        public QuoteDetailsForLD GetQuoteInfoForLD(string quoteId, string sessionId)
        {
            QuoteDetailsForLD quoteInfo = new QuoteDetailsForLD();
            //Getting the View Values
            var opportunityDet = _fieldDrawingAutomationdl.GetOpportunityAndVersionByQuoteId(quoteId);
            if (opportunityDet != null)
            {
                if (opportunityDet.OpportunityId != null && opportunityDet.VersionId != 0)
                {
                    if (opportunityDet.OpportunityId.Contains(Constant.SCUSER))
                    {
                        var userDetails = _cpqCacheManager.GetCache(sessionId, _environment, Constant.USERADDRESS);
                        if (userDetails != null)
                        {
                            var viewuserDetails = Utility.DeserializeObjectValue<OpportunityEntity>(userDetails);
                            quoteInfo.Branch = viewuserDetails?.Branch;
                        }
                    }
                    else
                    {
                        var viewProjectDetailsCache = _cpqCacheManager.GetCache(opportunityDet.OpportunityId + opportunityDet.VersionId, _environment, Constant.VIEWDATA);
                        if (viewProjectDetailsCache != null)
                        {
                            var viewDetails = Utility.DeserializeObjectValue<ViewProjectDetails>(viewProjectDetailsCache);
                            if (viewDetails != null)
                            {
                                quoteInfo.Architecture = viewDetails.Data?.Quotation?.Architect?.AccountName;
                                quoteInfo.GC = viewDetails.Data?.Quotation?.GC?.AccountName;
                                quoteInfo.City = viewDetails.Data?.Quotation?.Building?.City;
                                quoteInfo.Branch = viewDetails.Data?.Quotation?.OpportunityInfo?.Branch;
                            }
                        }
                    }
                }
            }
            return quoteInfo;
        }

        public List<UnitVariables> GetAllVariablesForPayloadGeneration(string sessionId,string productName, List<UnitVariables> unitVariablesCollection, Dictionary<string, string> buildingAndGroupValues)
        {
            //Unit Package Call
            var resultUnitPackageVariables = GetVariablesFromVTPackage(sessionId, unitVariablesCollection, Constant.UNITDEFAULTSCLMCALL, productName).ConfigureAwait(false).GetAwaiter().GetResult();
            //Lift Designer Package Call
            var resultLiftDesignerVariables = GettingVariablesForLiftDesignerXmlGeneration(sessionId, resultUnitPackageVariables, productName).ConfigureAwait(false).GetAwaiter().GetResult();
            ////Heat Input
            var lstHeatPackageVariableAssignment = GetInputVariableAssignments(Constant.HEAT, resultUnitPackageVariables, buildingAndGroupValues);
            //Lift Designer Heat Package Call
            var resultLiftDesignerHeatVariables = GetLiftDesignerHeatBracketPackageVariables(sessionId, lstHeatPackageVariableAssignment, Constant.HEAT, productName).ConfigureAwait(false).GetAwaiter().GetResult();
            //Bracket Input
            var lstBracketPackageVariableAssignment = GetInputVariableAssignments(Constant.BRACKET, resultUnitPackageVariables, buildingAndGroupValues);
            //Lift Designer Bracket Package Call
            var resultLiftDesignerBracketVariables = GetLiftDesignerHeatBracketPackageVariables(sessionId, lstBracketPackageVariableAssignment, Constant.BRACKET, productName).ConfigureAwait(false).GetAwaiter().GetResult();
            resultLiftDesignerBracketVariables.AddRange(resultLiftDesignerHeatVariables);
            resultLiftDesignerBracketVariables.AddRange(resultLiftDesignerVariables);
            resultLiftDesignerBracketVariables.AddRange(resultUnitPackageVariables);
            return resultLiftDesignerBracketVariables;
        }

        public void GetAllEnrichmentFunctionForLD(IDictionary<string, string> data, QuoteDetailsForLD quoteDetails)
        {
            Enrich_QuoteInfo(data);
            Enrich_GroupingInfo(data);
            Enrich_FloorMatrixInfo(data);
            Enrich_BasicInputs(data);
            Enrich_UnitConversion(data); 
            void Enrich_BasicInputs(IDictionary<string, string> data)
            {
                Enrich_ROOFHEIGHT(data);
            }
            void Enrich_ROOFHEIGHT(IDictionary<string, string> data)
            {
                if (data.TryGetValue("OVHEAD", out string ovHead) && data.ContainsKey("ROOFHEIGHT"))
                {
                    data["ROOFHEIGHT"] = (Convert.ToDouble(ovHead) + 1).ToString();
                }
            }
            void Enrich_UnitConversion(IDictionary<string, string> data)
            {
                Enrich_ConvertToFeet(Constant.TRAVEL, data);
            }
            //assuming to be in Inches
            void Enrich_ConvertToFeet(string propertyName, IDictionary<string, string> data)
            {
                if (data.TryGetValue(propertyName, out string propertyValue))
                {
                    data[propertyName] = Enrich_ValueMultiplier(0.08334f, propertyValue);
                }
            }


            string Enrich_ValueMultiplier(float multiplier, string value)
            {
                if (double.TryParse(value, out var doubleValue))
                {
                    return (multiplier * doubleValue).ToString();
                }
                return value;
            }
            void Enrich_QuoteInfo(IDictionary<string, string> data)
            {
                data.Add("ARCH", quoteDetails.Architecture);
                data.Add("GC", quoteDetails.GC);
                data.Add(Constant.BLDGCITY, quoteDetails.City);
                data.Add(Constant.BRANCH.ToUpper(), quoteDetails.Branch);
            }

            void Enrich_GroupingInfo(IDictionary<string, string> data)
            {
                Enrich_Shaft_IX(data);
            }

            void Enrich_FloorMatrixInfo(IDictionary<string, string> data)
            {
                Enrich_FloorMatrix_Property(data, Constant.TRANSFACEF);
                Enrich_FloorMatrix_Property(data, Constant.COLFACEF);
                Enrich_FloorMatrix_Property(data, Constant.TRANSFACER);
                Enrich_FloorMatrix_Property(data, Constant.COLFACER);
                Enrich_FLL_DISTANCES(data);


            }

            void Enrich_FLL_DISTANCES(IDictionary<string, string> data)
            {
                if (data.TryGetValue(Constant.BLANDINGS, out string blandings))
                {
                    var noOfLandings = Convert.ToInt32(blandings.Trim());
                    data.TryGetValue(Constant.AVGRFHT, out string avgRoofHeight);
                    data.TryGetValue(Constant.ROOFHEIGHT, out string roofHeight);
                    data[Constant.FLL_DISTANCES] = string.Join(',', Enumerable.Repeat(avgRoofHeight, noOfLandings - 2)) + "," + roofHeight;
                }
            }

            void Enrich_FloorMatrix_Property(IDictionary<string, string> data, string propertyName)
            {
                //we need to add comma separated values 
                // no of values equals, no of landings
                if (data.TryGetValue(Constant.BLANDINGS, out string blandings))
                {
                    var noOfLandings = Convert.ToInt32(blandings.Trim());
                    if (data.TryGetValue(propertyName, out string propertyValue))
                    {
                        data[propertyName] = string.Join(',', Enumerable.Repeat(propertyValue, noOfLandings));
                    }
                    if (data.TryGetValue("REAROPEN", out string rearOpen) && Convert.ToBoolean(rearOpen))
                    {
                        data["ENTR"] = string.Join(',', Enumerable.Repeat("F", noOfLandings));
                    }
                }
            }

            void Enrich_Shaft_IX(IDictionary<string, string> data)
            {
                //SHAFT_IX : we need to assign the value as CARID-1 ( as per the PDF)
                if (data.TryGetValue(Constant.CARID, out string value))
                {
                    if (string.IsNullOrEmpty(value) || !int.TryParse(value, out int cartId))
                    {
                        cartId = 1;
                    }
                    data.TryAdd(Constant.SHAFT_IX, (cartId - 1).ToString());
                }
            }
        }
    }
}