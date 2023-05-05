/************************************************************************************************************
************************************************************************************************************
   File Name     :   ProjectsBL.cs
   Created By    :   Infosys LTD
   Created On    :   10-JAN-2020
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
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.Common;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class ProjectsBL : IProject
    {
        /// <summary>
        /// Variable Collection
        /// </summary>
        #region Variables
        /// <summary>
        /// object IProjectsBL
        /// </summary>
        private readonly IProjectsDL _projectsdl;
        /// <summary>
        /// object IProjectsDL
        /// </summary>
        private IConfiguration _configuration;
        /// <summary>
        /// object ICacheManager
        /// </summary>
        private readonly ICacheManager _cpqCacheManager;
        /// <summary>
        /// string
        /// </summary>
        private readonly string _environment;
        /// <summary>
        /// Auth Token
        /// </summary>
        private readonly IAuth _auth;
        /// <summary>
        /// Configure
        /// </summary>
        private readonly IConfigure _configure;
        private readonly IVaultDL _mFileDL;
        #endregion

        /// <summary>
        /// Constructor for ProjectsBL
        /// </summary>
        /// <param Name="utility"></param>
        /// <param Name="projectsdl"></param>
        public ProjectsBL(ILogger<ProjectsBL> logger, IProjectsDL projectsdl, IConfiguration iConfig, ICacheManager cpqCacheManager, IAuth auth, IConfigure configure)
        {
            _projectsdl = projectsdl;
            Utility.SetLogger(logger);
            _configuration = iConfig;
            _cpqCacheManager = cpqCacheManager;
            _environment = Constant.DEV;
            _auth = auth;
            _configure = configure;
        }

        /// <summary>
        /// Get list of Projects for user method
        /// </summary>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetListOfProjectsForUser(int userId)
        {
            var methodBeginTime = Utility.LogBegin();
            List<Projects> lstproject = _projectsdl.GetListOfProjectsForUser(userId);
            var response = JArray.FromObject(lstproject);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
        }

        /// <summary>
        /// GetOpportunityData
        /// </summary>
        /// <param Name="oppId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetOpportunityData(string oppId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            _configuration = new ConfigurationBuilder().SetBasePath(System.IO.Directory.GetCurrentDirectory()).AddJsonFile(Constant.APPSETTINGS).Build();
            bool isCRMOD = Convert.ToBoolean(_configuration.GetSection(Constant.PARAMSETTINGS).GetSection(Constant.CRMOD).Value);
            JObject response;

            if (isCRMOD)
            {
                var oppData = _cpqCacheManager.GetCache(oppId, _environment, Constant.USERADDRESS);
                if (oppData == null)
                {
                    OpportunityEntity opportunityEntity = _projectsdl.GetOpportunityData(oppId);
                    response = JObject.FromObject(opportunityEntity);
                    if (string.IsNullOrEmpty(opportunityEntity.OpportunityName))
                    {
                        throw new CustomException(new ResponseMessage()
                        {
                            StatusCode = Constant.NOTFOUND,
                            Message = Constant.INVALIDOPPORTUNITYID
                        });
                    }
                    else
                    {
                        _cpqCacheManager.SetCache(oppId, _environment, Constant.USERADDRESS, Utility.SerializeObjectValue(opportunityEntity));
                        _cpqCacheManager.SetCache(sessionId, _environment, Constant.USERADDRESS, Utility.SerializeObjectValue(opportunityEntity));
                    }
                }
                else
                {
                    response = JObject.FromObject(JsonConvert.DeserializeObject<OpportunityEntity>(oppData));
                    _cpqCacheManager.SetCache(sessionId, _environment, Constant.USERADDRESS, Utility.SerializeObjectValue(response));
                }
            }
            else
            {
                var OppFile = "Opportunity" + "_" + oppId + ".json";
                response = JObject.Parse(File.ReadAllText(@"SampleStubData\Projects\" + OppFile));
                _cpqCacheManager.SetCache(sessionId, _environment, Constant.USERADDRESS, Utility.SerializeObjectValue(response));
            }
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = response };
        }

        /// <summary>
        /// this method is used to search user
        /// </summary>
        /// <param Name="userName"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SearchUser(string userName)
        {
            var methodBeginTime = Utility.LogBegin();
            List<User> lstuser = _projectsdl.SearchUser(userName);
            var response = JArray.FromObject(lstuser);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
        }

        /// <summary>
        /// Method to get the project info from VIEW
        /// </summary>
        /// <param Name="OpportunityId"></param>
        /// <param Name="versionId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetProjectDetails(string opportunityId, string versionId, string sessionId, int parentVersionId = 0, bool refreshCache = false)
        {
            var methodBeginTime = Utility.LogBegin();
            //JObject response
            var viewProjectDetailsCache = _cpqCacheManager.GetCache(opportunityId + versionId, _environment, Constant.USERADDRESS);
            if (viewProjectDetailsCache == null || refreshCache)
            {
                Utility.LogEnd(methodBeginTime);
                return await GetProjectInfo(opportunityId, versionId, sessionId, parentVersionId).ConfigureAwait(false);
            }
            else
            {
                var projectInfo = JsonConvert.DeserializeObject<ViewProjectDetails>(viewProjectDetailsCache);
                _cpqCacheManager.SetCache(sessionId, _environment, Constant.USERADDRESS, Utility.SerializeObjectValue(projectInfo.ProjectInfoDetails));
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = JObject.Parse(JsonConvert.SerializeObject(projectInfo.ProjectInfoDetails)) };
            }
        }

        /// <summary>
        /// SaveProjectInfo1
        /// </summary>
        /// <param Name="OpportunityId"></param>
        /// <param Name="versionId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveProjectInfo1(string opportunityId, string versionId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var FilteredVariableList = Getvariablevalues(opportunityId);
            foreach (var building in FilteredVariableList)
            {
                var lineObject = new Line();
                var variables = new List<VariableAssignment>();
                foreach (var buildingVariableAssignments in building.BuildingVariableAssignments)
                {
                    variables.Add(buildingVariableAssignments);
                }
                lineObject.VariableAssignments = variables;
                var configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineObject), Constant.BUILDINGNAME);
                var configResponse = await _configure.GetByDefaultOrRulevaluesFromPackage(configRequest, sessionId);
                Dictionary<string, object> configVariables = JsonConvert.DeserializeObject<Dictionary<string, object>>(configResponse[Constant.CONFIGURATION].ToString());
                variables = new List<VariableAssignment>();
                foreach (var vtpckageVariable in configVariables)
                {
                    variables.Add(new VariableAssignment { VariableId = vtpckageVariable.Key, Value = vtpckageVariable.Value });
                }
                building.BuildingVariableAssignments = variables;
                foreach (var group in building.GroupVariableAssignment)
                {
                    group.GroupVariableAssignments = group.GroupVariableAssignments.Where(x => x != null).ToList();
                    if (!group.isNCP)
                    {
                        lineObject = new Line();
                        variables = new List<VariableAssignment>();
                        foreach (var unit in group.GroupVariableAssignments)
                        {
                            variables.Add(unit);
                        }
                        lineObject.VariableAssignments = variables;
                        configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineObject), Constant.GROUPCONFIGURATIONNAME);
                        configResponse = await _configure.GetByDefaultOrRulevaluesFromPackage(configRequest, sessionId);
                        configVariables = JsonConvert.DeserializeObject<Dictionary<string, object>>(configResponse[Constant.CONFIGURATION].ToString());
                        variables = new List<VariableAssignment>();
                        foreach (var vtpckageVariable in configVariables)
                        {
                            variables.Add(new VariableAssignment { VariableId = vtpckageVariable.Key, Value = vtpckageVariable.Value });
                        }
                        group.GroupVariableAssignments = variables;
                    }
                    foreach (var setVariableAssignment in group.SetVariableAssignment)
                    {
                        lineObject = new Line();
                        variables = new List<VariableAssignment>();
                        foreach (var set in setVariableAssignment.UnitVariableAssignments)
                        {
                            variables.Add(set);
                        }
                        lineObject.VariableAssignments = variables;
                        if (!group.isNCP)
                        {
                            configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineObject), Constant.UNITCONFIG, new List<VariableAssignment>(), setVariableAssignment.ProductName);
                        }
                        else
                        {
                            if (Utility.CheckEquals(setVariableAssignment.ProductName, Constant.TWIN))
                            {
                                configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineObject), Constant.TWINELEVATOR);
                            }
                            else if (setVariableAssignment.ProductName.ToUpper().Contains(Constant.THIRDPARTY))
                            {
                                configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineObject), Constant.OTHER);
                            }
                            else
                            {
                                configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineObject), Constant.ESCLATORMOVINGWALK);
                            }
                        }
                        configResponse = await _configure.GetByDefaultOrRulevaluesFromPackage(configRequest, sessionId);
                        configVariables = JsonConvert.DeserializeObject<Dictionary<string, object>>(configResponse[Constant.CONFIGURATION].ToString());
                        variables = new List<VariableAssignment>();
                        foreach (var vtpckageVariable in configVariables)
                        {
                            variables.Add(new VariableAssignment { VariableId = vtpckageVariable.Key, Value = vtpckageVariable.Value });
                        }
                        setVariableAssignment.UnitVariableAssignments = variables;
                        lineObject.VariableAssignments = GenerateVariableAssignmentsForProductTree(setVariableAssignment);
                        configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineObject), Constant.PRODUCTTREE);
                        configResponse = await _configure.GetByDefaultOrRulevaluesFromPackage(configRequest, sessionId).ConfigureAwait(false);
                        setVariableAssignment.ProductTreeVariables = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        setVariableAssignment.ProductTreeVariables = JsonConvert.DeserializeObject<Dictionary<string, object>>(configResponse[Constant.CONFIGURATION].ToString());
                    }
                }
            }
            var exportJson = JObject.Parse(File.ReadAllText(Constant.VIEWEXPORTVARIABLELIST));
            var exportJsonBuildingVariables = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(exportJson[Constant.BUILDINGCONFIGURATIONVARIABLELIST]));
            var exportJsoneqmntConsoleVariables = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(exportJson[Constant.BUILDINGCONSOLECONFIGURATIONVARIABLELIST]));
            var exportJsonEqmntConfgnVariables = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(exportJson[Constant.BUILDINGEQUIPMENTCONFIGURATIONVARIABLELIST]));
            var exportJsonControlLocationVariables = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(exportJson[Constant.CONTROLLOCATIONVARIABLELIST]));
            var exportJsonUnitConfigurationVariables = Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(exportJson[Constant.UNITCNFIGURATIONVARIABLELIST]));

            var viewExportDetails1 = _projectsdl.GetVariablesAndValuesForView1(opportunityId, exportJsonBuildingVariables, exportJsoneqmntConsoleVariables, exportJsonEqmntConfgnVariables, exportJsonControlLocationVariables, exportJsonUnitConfigurationVariables, FilteredVariableList);

            var viewProjectDetailsCache = _cpqCacheManager.GetCache(viewExportDetails1.Quotation.OpportunityInfo.OpportunityId + viewExportDetails1.Quotation.Quote.VIEW_Version, _environment, Constant.USERADDRESS);
            if (viewProjectDetailsCache == null)
            {
                var oppData = await GetProjectInfo(viewExportDetails1.Quotation.OpportunityInfo.OpportunityId, Convert.ToString(viewExportDetails1.Quotation.Quote.VIEW_Version), sessionId).ConfigureAwait(false);
                viewProjectDetailsCache = _cpqCacheManager.GetCache(viewExportDetails1.Quotation.OpportunityInfo.OpportunityId + viewExportDetails1.Quotation.Quote.VIEW_Version, _environment, Constant.USERADDRESS);
                var projectInfo = JsonConvert.DeserializeObject<ViewProjectDetails>(viewProjectDetailsCache);
                viewExportDetails1.Quotation.OpportunityInfo.OpportunityURL = projectInfo.Data.Quotation.OpportunityInfo.OpportunityURL;
                if (string.IsNullOrEmpty(projectInfo.Data.Quotation.Quote.BaseBid))
                {
                    projectInfo.Data.Quotation.Quote.BaseBid = "true";
                }
                viewExportDetails1.Quotation.Quote.BaseBid = Convert.ToBoolean(projectInfo.Data.Quotation.Quote.BaseBid);
            }
            else
            {
                var projectInfo = JsonConvert.DeserializeObject<ViewProjectDetails>(viewProjectDetailsCache);
                viewExportDetails1.Quotation.OpportunityInfo.OpportunityURL = projectInfo.Data.Quotation.OpportunityInfo.OpportunityURL;
                if (string.IsNullOrEmpty(projectInfo.Data.Quotation.Quote.BaseBid))
                {
                    projectInfo.Data.Quotation.Quote.BaseBid = "true";
                }
                viewExportDetails1.Quotation.Quote.BaseBid = Convert.ToBoolean(projectInfo.Data.Quotation.Quote.BaseBid);
            }
            var response = JObject.FromObject(viewExportDetails1, new Newtonsoft.Json.JsonSerializer());
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = response };
        }

        /// <summary>
        /// SaveConfigurationToView
        /// </summary>
        /// <param Name="OpportunityId"></param>
        /// <param Name="versionId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveConfigurationToView(string opportunityId, string versionId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var response = await SaveProjectInfo1(opportunityId, versionId, sessionId).ConfigureAwait(false);
            ViewExportDetails viewExportDetails = Utility.DeserializeObjectValue<ViewExportDetails>(Utility.SerializeObjectValue(response.Response));
            if (viewExportDetails.Quotation.UnitMaterials.Count == 0)
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.NOCONFIGURATIONDATAAVAILABLE,
                    Description = Constant.NOCONFIGURATIONDATAAVAILABLEDESCRIPTION
                });
            }
            var viewsaveResponse = await _projectsdl.SaveConfigurationToView(response).ConfigureAwait(false);
            if (Convert.ToString(viewsaveResponse.Response[Constant.CODEVALUES]) == "200")
            {
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage()
                {
                    StatusCode = Constant.SUCCESS,
                    Message = Convert.ToString(viewsaveResponse.Response[Constant.RETMSGVALUES])
                };
            }
            else if (Convert.ToString(viewsaveResponse.Response[Constant.CODEVALUES]) == "500")
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR,
                    Message = Convert.ToString(viewsaveResponse.Response[Constant.RETMSGVALUES])
                });
            }
            else if (Convert.ToString(viewsaveResponse.Response[Constant.CODEVALUES]) == "503")
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.SERVICEUNAVAILABLE,
                    Message = Convert.ToString(viewsaveResponse.Response[Constant.RETMSGVALUES])
                });
            }
            else if (Convert.ToString(viewsaveResponse.Response[Constant.CODEVALUES]) == "409")
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INVALIDCONFIG,
                    Message = Convert.ToString(viewsaveResponse.Response[Constant.RETMSGVALUES])
                });
            }
            Utility.LogEnd(methodBeginTime);
            throw new CustomException(new ResponseMessage()
            {
                StatusCode = Constant.BADREQUEST,
                Message = Convert.ToString(viewsaveResponse.Response[Constant.RETMSGVALUES])
            });
        }

        /// <summary>
        /// CreateProjectsBL
        /// </summary>
        /// <param Name="oppId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> CreateProjectsBL(string sessionId, string projectId, int versionId = 1)
        {
            var methodBeginTime = Utility.LogBegin();
            //0 marks, new project creation
            var enrichmentData = new CreateProjectResponseObject();
            //JObject response;
            var userName = GetUserId(sessionId);
            // to edit flow
            var validatedProjectId = projectId;
            if (projectId.Equals(Constant.NEWPROJECTIDFLAG, StringComparison.OrdinalIgnoreCase))
            {
                validatedProjectId = string.Empty;
            }
            var projectStubFile = JObject.Parse(File.ReadAllText(Constant.PROJECTSENRICHEDDATA));
            enrichmentData.Sections = Utility.DeserializeObjectValue<List<Sections>>(Utility.SerializeObjectValue(projectStubFile[Constant.SECTIONSVALUES]));
            // Getting from cache GETENRICHMENTVALUESDATA
            _cpqCacheManager.GetCache(sessionId, _environment, Constant.GETENRICHMENTVALUESDATA);
            var rolename = _configure.GetRoleName(sessionId);
            if (!projectId.Equals(Constant.NEWPROJECTIDFLAG, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var item in enrichmentData.Sections[0].Variables)
                {
                    if (Utility.CheckEquals(item.Id, "Projects.SalesStage"))
                    {
                        foreach (var items in item.Properties)
                        {
                            if (Utility.CheckEquals(items.Id, "isEditable"))
                            {
                                items.Value = true;
                            }
                        }
                    }
                }
                var enrichedDbResponse = _projectsdl.GetMiniProjectValues(sessionId, userName, validatedProjectId, enrichmentData, versionId);
                var permissions = _projectsdl.GetPermissionForProjectScreen(rolename, validatedProjectId);
                enrichedDbResponse.Permissions = permissions;
                enrichedDbResponse.VariableDetails = enrichedDbResponse.VariableDetails;
                enrichedDbResponse.ProjectDisplayDetails = enrichedDbResponse.ProjectDisplayDetails;
                enrichedDbResponse.VariableDetails[Constant.COUNTRYVALUE] = Constant.CANADA.ToUpper();
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = Utility.FilterNullValues(enrichedDbResponse) };
            }
            var permission = _projectsdl.GetPermissionForProjectScreen(rolename, validatedProjectId);
            enrichmentData.Permissions = permission;
            var variableVals = JObject.Parse(File.ReadAllText(Constant.VARIABLESDATAVALUES));
            enrichmentData.VariableDetails = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(variableVals[Constant.VARIABLEDETAILS]));
            enrichmentData.ProjectDisplayDetails = null;
            enrichmentData.VariableDetails[Constant.COUNTRYVALUE] = Constant.CANADA.ToUpper();
            var enrichmentResponseData = _projectsdl.GetMiniProjectValues(sessionId, userName, validatedProjectId, enrichmentData);

            // load the response in the cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.GETENRICHMENTVALUESDATA, Utility.SerializeObjectValue(enrichmentResponseData));
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = Utility.FilterNullValues(enrichmentResponseData) };
        }

        /// <summary>
        /// GetUserId
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public string GetUserId(string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var cachedUserDetail = _cpqCacheManager.GetCache(sessionId, _environment, Constant.USERDETAILSCPQ);
            User userDetail = new User();
            if (!string.IsNullOrEmpty(cachedUserDetail))
            {
                userDetail = Utility.DeserializeObjectValue<User>(cachedUserDetail);
            }
            Utility.LogEnd(methodBeginTime);
            return userDetail.UserId;
        }

        /// <summary>
        /// SaveAndUpdateMiniProjectsBL
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="variablesValues"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveAndUpdateMiniProjectsBL(string sessionId, JObject variablesValues)
        {
            var methodBeginTime = Utility.LogBegin();
            var enrichmentData = new CreateProjectResponseObject();
            VariableDetails requestVariableDetails = new VariableDetails();
            var userName = GetUserId(sessionId);
            // to edit flow
            var getCacheData = _cpqCacheManager.GetCache(sessionId, _environment, Constant.GETENRICHMENTVALUESDATA);

            if (!string.IsNullOrEmpty(getCacheData) && variablesValues != null)
            {
                var enrichedDataValues = Utility.DeserializeObjectValue<CreateProjectResponseObject>(getCacheData);
                var getVariableValues = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(variablesValues[Constant.VARIABLEDETAILS]));
                enrichedDataValues.VariableDetails = getVariableValues;
                // set cache as per updated request pay load if any new chanegs are included
                _cpqCacheManager.SetCache(sessionId, _environment, Constant.GETENRICHMENTVALUESDATA, Utility.SerializeObjectValue(enrichmentData));
            }
            if (variablesValues != null)
            {
                var calues = Utility.DeserializeObjectValue<VariableDetails>(Utility.SerializeObjectValue(variablesValues[Constant.VARIABLEDETAILS]));
                requestVariableDetails = calues;
            }
            bool containsSpecialCharacter = requestVariableDetails.Country.Any(c => !char.IsLetterOrDigit(c));

            if (containsSpecialCharacter)
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.COUNTRYNAMEERRORMESSAGE
                });
            }
            //Convert List to Json
            var result = _projectsdl.SaveAndUpdateMiniProjectValues(requestVariableDetails, userName, false);
            var response = JArray.FromObject(result);

            // load the response in the cache
            _cpqCacheManager.SetCache(sessionId, _environment, Constant.ISEDITFLOWFLAGCHECK, "true");

            if (result[0].Result == 1)
            {
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
            }
            else
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = result[0].Message,
                    Description = Constant.SOMEERROROCCURED,
                    ResponseArray = response
                });
            }
        }

        /// <summary>
        /// getListOfProjectsDetailsBl
        /// </summary>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetListOfProjectsDetailsBl(string sessionId)
        {
            //Call to DB
            var methodBeginTime = Utility.LogBegin();
            UIResponseProjectList projectResponse = new UIResponseProjectList();
            var viewProjectResponse = new GetProject()
            {
                createProject = true,
                Projects = new List<ProjectResponseDetails>()
            };
            //get User Response
            var cachedUserDetail = _cpqCacheManager.GetCache(sessionId, _environment, Constant.USERDETAILSCPQ);
            User userDetail = new User();
            if (!string.IsNullOrEmpty(cachedUserDetail))
            {
                userDetail = Utility.DeserializeObjectValue<User>(cachedUserDetail);
            }
            List<ProjectResponseDetails> lstproject = _projectsdl.GetListOfProjectsDetailsDl(userDetail);
            if (!Utility.CheckEquals(userDetail.Location.country, Constant.CANADA))
            {
                projectResponse.IsViewUser = true;
            }

            projectResponse.Projects = new List<ProjectResponseDetails>();
            if (lstproject.Count > 0)
            {
                projectResponse.Projects = lstproject;
            }
            foreach (var item in viewProjectResponse.Projects)
            {
                projectResponse.Projects.Add(item);
            }
            var roleName = _configure.GetRoleName(sessionId);
            var permission = _projectsdl.GetPermissionByRole(roleName);
            List<string> permissionAdd = (from objpermission in permission
                                          where objpermission.Entity.Equals(Constant.PROJECT)
                                          && objpermission.ProjectStage.Equals(string.Empty)
                                          select objpermission.PermissionKey).Distinct().ToList();
            projectResponse.Permissions = permissionAdd;
            if (projectResponse.Projects != null && projectResponse.Projects.Any())
            {
                foreach (var project in projectResponse.Projects)
                {
                    List<string> permissionProject = (from objpermission in permission
                                                      where objpermission.Entity.Equals(Constant.PROJECT)
                                                      && objpermission.ProjectStage.Equals(project.SalesStage.StatusName)
                                                      select objpermission.PermissionKey).Distinct().ToList();
                    project.Permissions = permissionProject;
                    if (project.Quotes != null && project.Quotes.Any())
                    {
                        foreach (var quote in project.Quotes)
                        {
                            List<string> permissionQuote = (from objpermission in permission
                                                            where objpermission.Entity.Equals(Constant.QUOTE)
                                                            && objpermission.ProjectStage.Equals(project.SalesStage.StatusName)
                                                            select objpermission.PermissionKey).Distinct().ToList();
                            quote.Permissions = permissionQuote;
                        }
                    }
                }
            }
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = Utility.FilterNullValues(projectResponse) };
        }

        /// <summary>
        /// GetProjectInfo
        /// </summary>
        /// <param Name="OpportunityId"></param>
        /// <param Name="versionId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetProjectInfo(string opportunityId, string versionId, string sessionId, int parentVersionId = 0)
        {
            var methodBeginTime = Utility.LogBegin();
            var viewDetails = new ViewProjectDetails();
            var variableMapperValues = Utility.GetVariableMapping(Constant.INTEGRATIONCONSTANTMAPPER, Constant.USERLOGINERRORTYPE);
            var userName = GetUserId(sessionId);
            // location check 
            var userLocation = GetViewUserCheck(sessionId);
            if (!userLocation && !opportunityId.Contains(Constant.SCUSER))
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = variableMapperValues[Constant.CANADATOVIEWUSERERRORMESSAGE],
                    Description = variableMapperValues[Constant.CANADATOVIEWUSERERRORMESSAGE]
                });
            }
            else if (userLocation && opportunityId.Contains(Constant.SCUSER))
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = variableMapperValues[Constant.VIEWTOCANADAUSERERRORMESSAGE],
                    Description = variableMapperValues[Constant.VIEWTOCANADAUSERERRORMESSAGE]
                });
            }

            if (opportunityId.Contains(Constant.SCUSER))
            {
                var val = await CreateProjectsBL(sessionId, opportunityId, Int32.Parse(versionId)).ConfigureAwait(false);
                var responseData = Utility.DeserializeObjectValue<CreateProjectResponseObject>(Utility.SerializeObjectValue(val.Response));
                var variableValuesData = Utility.DeserializeObjectValue<VariableDetails>(Utility.SerializeObjectValue(responseData.VariableDetails));
                var projectDisplayDetails = Utility.DeserializeObjectValue<ProjectDisplayDetails>(Utility.SerializeObjectValue(responseData.ProjectDisplayDetails));

                viewDetails.ProjectInfoDetails = new OpportunityEntity()
                {
                    Id = opportunityId,
                    OpportunityName = variableValuesData?.ProjectName,
                    LineOfBusiness = Constant.NEWINSTALLATIONVALUES,
                    SalesStage = variableValuesData?.SalesStage,
                    AccountName = variableValuesData?.AccountName,
                    Branch = variableValuesData?.Branch,
                    AccountAddress = new AccountEntity()
                    {
                        AccountName = variableValuesData?.AccountName,
                        AccountAddressCity = variableValuesData?.City,
                        AccountAddressState = variableValuesData?.State,
                        AccountAddressCountry = variableValuesData?.Country,
                        AccountAddressAddressZipCode = variableValuesData?.ZipCode,
                        AccountAddressStreetAddress = variableValuesData?.AddressLine1,
                        AccountAddressStreetAddress2 = variableValuesData?.AddressLine2
                    },
                    BookingDate = projectDisplayDetails?.ContractBookedDate,
                    ProposedDate = projectDisplayDetails?.ProposedDate,
                    CreatedDate = projectDisplayDetails?.CreatedDate,
                    QuoteId = projectDisplayDetails?.QuoteId,
                    QuoteStatus = projectDisplayDetails?.QuoteStatus,
                    SalesRepEmail = variableValuesData?.SalesRepEmail,
                    OperationContactEmail = variableValuesData?.OperationContactEmail
                };
                _cpqCacheManager.SetCache(sessionId, _environment, Constant.USERADDRESS, Utility.SerializeObjectValue(viewDetails.ProjectInfoDetails));
                _cpqCacheManager.SetCache(opportunityId + versionId, _environment, Constant.USERADDRESS, Utility.SerializeObjectValue(viewDetails));
                _cpqCacheManager.SetCache(viewDetails.ProjectInfoDetails.QuoteId, _environment, Constant.USERADDRESS, Utility.SerializeObjectValue(viewDetails));
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = JObject.Parse(JsonConvert.SerializeObject(viewDetails.ProjectInfoDetails)) };
            }



            // Getting project Values

            var viewProjectTask = _projectsdl.GetProjectInfo(opportunityId, versionId).ConfigureAwait(false);

            var viewProjectDetails = await viewProjectTask;
            var response = JObject.FromObject(viewProjectDetails.Response);
            _cpqCacheManager.SetCache(opportunityId + versionId, _environment, Constant.VIEWDATA, Utility.SerializeObjectValue(response));
            var unitIdentifaction = response.SelectToken(Constant.VIEWUNITS);
            List<FactoryDetails> unitIdetificationList = null;
            if (unitIdentifaction != null)
            {
                unitIdetificationList = new List<FactoryDetails>();
                foreach (var unit in unitIdentifaction)
                {
                    var unitvalue = new FactoryDetails()
                    {
                        FacotryJobId = (string)unit.SelectToken(Constant.FACTORYJOBNUMBER),
                        UEID = (string)unit.SelectToken(Constant.VIEWUEID)
                    };
                    unitIdetificationList.Add(unitvalue);
                }
            }
            // View Variables Values Mapping
            var jObjectDictionry = (JObject.Parse(File.ReadAllText(Constant.VIEWVARIABLEMAPPING)));
            var viewVariables = jObjectDictionry[Constant.VIEWVARIABLES];
            var variablesDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(viewVariables));
            // Contact Variables Data
            var contactVariables = jObjectDictionry[Constant.CONTACTVRBLES];
            var variablesDictionary_Contact = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(contactVariables));
            // gCVariables data
            var gCVariables = jObjectDictionry[Constant.GCVARIABLES];
            var variablesDictionary_GC = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(gCVariables));
            // ownerVariables data
            var ownerVariables = jObjectDictionry[Constant.OWNERVARIABLES];
            var variablesDictionary_Owner = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(ownerVariables));
            // architectVariables data
            var architectVariables = jObjectDictionry[Constant.ARCHITECHVRBLES];
            var variablesDictionary_Architect = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(architectVariables));
            // billingVariables data
            var billingVariables = jObjectDictionry[Constant.BILLINGVRBLES];
            var variablesDictionary_Billing = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(billingVariables));
            // BuildingVariables data
            var BuildingVariables = jObjectDictionry[Constant.BUILDINGVRBLES];
            var variablesDictionary_Building = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(BuildingVariables));
            // quoteVariables data
            var quoteVariables = jObjectDictionry[Constant.QUOTEVRBLES];
            var variablesDictionary_QuoteInfo = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(quoteVariables));
            // building Info Variables data
            var buildingInfoVariables = jObjectDictionry[Constant.BUILDINGINFOVRBLES];
            var variablesDictionary_BuildingInfo = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(buildingInfoVariables));
            // Getting Units List Data 
            var unitsDetailsData = new List<UnitsData>();
            var unitsData = new UnitsData()
            {
                BuildingInfo = new BuildingInfo()
                {
                    City = (string)response.SelectToken(variablesDictionary_BuildingInfo[Constant.CITY]),
                    County = (string)response.SelectToken(variablesDictionary_BuildingInfo[Constant.COUNTRY]),
                    State = (string)response.SelectToken(variablesDictionary_BuildingInfo[Constant.STATE]),
                    ZipCode_bldg = (string)response.SelectToken(variablesDictionary_BuildingInfo[Constant.ZIPCODE_BLDG]),
                    AddressLine1_bldg = (string)response.SelectToken(variablesDictionary_BuildingInfo[Constant.ADDRESSLINE1_BLDG]),
                    AddressLine2_bldg = (string)response.SelectToken(variablesDictionary_BuildingInfo[Constant.ADDRESSLINE2_BLDG])
                }
            };
            // Assign Unit data Values
            unitsDetailsData.Add(unitsData);
            // Default Null Date
            DateTime? defaultNullableDate = null;
            // View Details Mapping
            viewDetails = new ViewProjectDetails()
            {
                Code = (string)response.SelectToken(variablesDictionary[Constant.CODE]),
                Message = (string)response.SelectToken(variablesDictionary[Constant.RETMSG]),
                Data = new DataDetails()
                {
                    VersionId = (string)response.SelectToken(variablesDictionary[Constant.VERSIONID]),
                    Quotation = new Quotation()
                    {
                        OpportunityInfo = new OpportunityInfo()
                        {
                            OpportunityId = (string)response.SelectToken(variablesDictionary[Constant.OPPORTNTYID]),
                            OpportunityURL = (string)response.SelectToken(variablesDictionary[Constant.OPPURL]),
                            BusinessLine = (string)response.SelectToken(variablesDictionary[Constant.BUSINESSLINE]),
                            JobName = (string)response.SelectToken(variablesDictionary[Constant.JOBNAMEVIEW]),
                            Branch = (string)response.SelectToken(variablesDictionary[Constant.BRANCH]),
                            SalesmanActiveDirectoryID = (string)response.SelectToken(variablesDictionary[Constant.SLSMNACTVDRCTRYID]),
                            Salesman = (string)response.SelectToken(variablesDictionary[Constant.SALESMAN]),
                            Category = (string)response.SelectToken(variablesDictionary[Constant.CATGRY]),
                            SalesEmail = (string)response.SelectToken(variablesDictionary[Constant.SALESEMAIL]),

                            AwardClosedDate = new AwardClosedDate()
                            {
                                DateFormat = (string)response.SelectToken(variablesDictionary[Constant.DATEFORMAT]),
                                DateValue = (!string.IsNullOrEmpty((string)response.SelectToken(variablesDictionary[Constant.DATEVALUE]))) ? Convert.ToDateTime(response.SelectToken(variablesDictionary[Constant.DATEVALUE])) : defaultNullableDate
                            },

                            ProposedDate = new ProposedDate()
                            {
                                DateFormat = (string)response.SelectToken(variablesDictionary[Constant.PROPOSEDDATEFORMAT]),
                                DateValue = (!string.IsNullOrEmpty((string)response.SelectToken(variablesDictionary[Constant.PROPOSEDDATE]))) ? Convert.ToDateTime(response.SelectToken(variablesDictionary[Constant.PROPOSEDDATE])) : defaultNullableDate
                            },

                            ContractBookedDate = new ContractBookedDate()
                            {
                                DateFormat = (string)response.SelectToken(variablesDictionary[Constant.CONTRACTBOOKEDDATEFORMAT]),
                                DateValue = (!string.IsNullOrEmpty((string)response.SelectToken(variablesDictionary[Constant.CONTRACTBOOKEDDATE]))) ? Convert.ToDateTime(response.SelectToken(variablesDictionary[Constant.CONTRACTBOOKEDDATE])) : defaultNullableDate
                            },
                            OraclePSNumber = (string)response.SelectToken(variablesDictionary[Constant.ORACLEPSNMBR]),
                            SalesStage = (string)response.SelectToken(variablesDictionary[Constant.SALESSTG]),
                            Superintendent = (string)response.SelectToken(variablesDictionary[Constant.SUPERINTENDENT])
                        },
                        Contact = new Contact()
                        {
                            FirstName = (string)response.SelectToken(variablesDictionary_Contact[Constant.FIRSTNAME]),
                            LastName = (string)response.SelectToken(variablesDictionary_Contact[Constant.LASTNAME]),
                            Email = (string)response.SelectToken(variablesDictionary_Contact[Constant.EMAIL]),
                            MobilePhone = (string)response.SelectToken(variablesDictionary_Contact[Constant.MOBILEPHONE]),
                        },
                        GC = new UserAddressDetailsDataValues()
                        {
                            AccountName = (string)response.SelectToken(variablesDictionary_GC[Constant.ACCOUNTNME]),
                            AddressLine1 = (string)response.SelectToken(variablesDictionary_GC[Constant.ADDRESSLINE1]),
                            AddressLine2 = (string)response.SelectToken(variablesDictionary_GC[Constant.ADDRESSLINE2]),
                            City = (string)response.SelectToken(variablesDictionary_GC[Constant.CITY]),
                            Country = (string)response.SelectToken(variablesDictionary_GC[Constant.COUNTRY]),
                            County = (string)response.SelectToken(variablesDictionary_GC[Constant.COUNTY]),
                            State = (string)response.SelectToken(variablesDictionary_GC[Constant.STATE]),
                            ZipCode = (string)response.SelectToken(variablesDictionary_GC[Constant.ZIPCODE]),
                            CustomerNumber = (string)response.SelectToken(variablesDictionary_GC[Constant.CUSTOMERNMBR])
                        },
                        Owner = new UserAddressDetailsDataValues()
                        {
                            AccountName = (string)response.SelectToken(variablesDictionary_Owner[Constant.ACCOUNTNME]),
                            AddressLine1 = (string)response.SelectToken(variablesDictionary_Owner[Constant.ADDRESSLINE1]),
                            AddressLine2 = (string)response.SelectToken(variablesDictionary_Owner[Constant.ADDRESSLINE2]),
                            City = (string)response.SelectToken(variablesDictionary_Owner[Constant.CITY]),
                            Country = (string)response.SelectToken(variablesDictionary_Owner[Constant.COUNTRY]),
                            County = (string)response.SelectToken(variablesDictionary_Owner[Constant.COUNTY]),
                            State = (string)response.SelectToken(variablesDictionary_Owner[Constant.STATE]),
                            ZipCode = (string)response.SelectToken(variablesDictionary_Owner[Constant.ZIPCODE]),
                            CustomerNumber = (string)response.SelectToken(variablesDictionary_Owner[Constant.CUSTOMERNMBR])
                        },
                        Architect = new UserAddressDetailsDataValues()
                        {
                            AccountName = (string)response.SelectToken(variablesDictionary_Architect[Constant.ACCOUNTNME]),
                            AddressLine1 = (string)response.SelectToken(variablesDictionary_Architect[Constant.ADDRESSLINE1]),
                            AddressLine2 = (string)response.SelectToken(variablesDictionary_Architect[Constant.ADDRESSLINE2]),
                            City = (string)response.SelectToken(variablesDictionary_Architect[Constant.CITY]),
                            Country = (string)response.SelectToken(variablesDictionary_Architect[Constant.COUNTRY]),
                            County = (string)response.SelectToken(variablesDictionary_Architect[Constant.COUNTY]),
                            State = (string)response.SelectToken(variablesDictionary_Architect[Constant.STATE]),
                            ZipCode = (string)response.SelectToken(variablesDictionary_Architect[Constant.ZIPCODE]),
                            CustomerNumber = (string)response.SelectToken(variablesDictionary_Architect[Constant.CUSTOMERNMBR])
                        },
                        Billing = new UserAddressDetailsDataValues()
                        {
                            AccountName = (string)response.SelectToken(variablesDictionary_Billing[Constant.ACCOUNTNME]),
                            AddressLine1 = (string)response.SelectToken(variablesDictionary_Billing[Constant.ADDRESSLINE1]),
                            AddressLine2 = (string)response.SelectToken(variablesDictionary_Billing[Constant.ADDRESSLINE2]),
                            City = (string)response.SelectToken(variablesDictionary_Billing[Constant.CITY]),
                            Country = (string)response.SelectToken(variablesDictionary_Billing[Constant.COUNTRY]),
                            County = (string)response.SelectToken(variablesDictionary_Billing[Constant.COUNTY]),
                            State = (string)response.SelectToken(variablesDictionary_Billing[Constant.STATE]),
                            ZipCode = (string)response.SelectToken(variablesDictionary_Billing[Constant.ZIPCODE]),
                            CustomerNumber = (string)response.SelectToken(variablesDictionary_Billing[Constant.CUSTOMERNMBR])
                        },
                        Building = new UserAddressDetailsDataValues()
                        {

                            AccountName = (string)response.SelectToken(variablesDictionary_Building[Constant.ACCOUNTNME]),
                            AddressLine1 = (string)response.SelectToken(variablesDictionary_Building[Constant.ADDRESSLINE1]),
                            AddressLine2 = (string)response.SelectToken(variablesDictionary_Building[Constant.ADDRESSLINE2]),
                            City = (string)response.SelectToken(variablesDictionary_Building[Constant.CITY]),
                            Country = (string)response.SelectToken(variablesDictionary_Building[Constant.COUNTRY]),
                            County = (string)response.SelectToken(variablesDictionary_Building[Constant.COUNTY]),
                            State = (string)response.SelectToken(variablesDictionary_Building[Constant.STATE]),
                            ZipCode = (string)response.SelectToken(variablesDictionary_Building[Constant.ZIPCODE]),
                            CustomerNumber = (string)response.SelectToken(variablesDictionary_Building[Constant.CUSTOMERNMBR])
                        },
                        Quote = new QuoteInfo()
                        {
                            QuoteDescription = (string)response.SelectToken(variablesDictionary_QuoteInfo[Constant.QUOTEDESCRIPTION]),
                            BaseBid = (string)response.SelectToken(variablesDictionary_QuoteInfo[Constant.QUOTEBASEBID]),
                            LatestViewVersion = (Boolean)response.SelectToken(variablesDictionary_QuoteInfo[Constant.LATESTVIEWVERSION]),
                            IsPrimary = ((Boolean)response.SelectToken(variablesDictionary_QuoteInfo[Constant.QUOTEBASEBID]) && (Boolean)response.SelectToken(variablesDictionary_QuoteInfo[Constant.LATESTVIEWVERSION])),
                            UIVersionId = (string)response.SelectToken(variablesDictionary_QuoteInfo[Constant.UIVERSIONID]),
                        }
                    },
                    UnitsData = unitsDetailsData
                },
                ProjectInfoDetails = new OpportunityEntity()
                {
                    Id = (string)response.SelectToken(variablesDictionary[Constant.OPPORTNTYID]),
                    OpportunityName = (string)response.SelectToken(variablesDictionary[Constant.JOBNAMEVIEW]),
                    LineOfBusiness = (string)response.SelectToken(variablesDictionary[Constant.BUSINESSLINE]),
                    SalesStage = (string)response.SelectToken(variablesDictionary[Constant.SALESSTG]),
                    AccountName = (string)response.SelectToken(variablesDictionary_Building[Constant.ACCOUNTNME]),
                    Branch = (string)response.SelectToken(variablesDictionary[Constant.BRANCH]),
                    CloseDate = (!string.IsNullOrEmpty((string)response.SelectToken(variablesDictionary[Constant.DATEVALUE]))) ? Convert.ToDateTime(response.SelectToken(variablesDictionary[Constant.DATEVALUE])) : defaultNullableDate,
                    BookingDate = (!string.IsNullOrEmpty((string)response.SelectToken(variablesDictionary[Constant.CONTRACTBOOKEDDATE]))) ? Convert.ToDateTime(response.SelectToken(variablesDictionary[Constant.CONTRACTBOOKEDDATE])) : defaultNullableDate,
                    ProposedDate = (!string.IsNullOrEmpty((string)response.SelectToken(variablesDictionary[Constant.PROPOSEDDATE]))) ? Convert.ToDateTime(response.SelectToken(variablesDictionary[Constant.PROPOSEDDATE])) : defaultNullableDate,
                    AccountAddress = new AccountEntity()
                    {
                        AccountName = (string)response.SelectToken(variablesDictionary_Building[Constant.ACCOUNTNME]),
                        AccountAddressCity = (string)response.SelectToken(variablesDictionary_Building[Constant.CITY]),
                        AccountAddressState = (string)response.SelectToken(variablesDictionary_Building[Constant.STATE]),
                        AccountAddressCountry = (string)response.SelectToken(variablesDictionary_Building[Constant.COUNTY]),
                        AccountAddressAddressZipCode = (string)response.SelectToken(variablesDictionary_Building[Constant.ZIPCODE]),
                        AccountAddressStreetAddress = (string)response.SelectToken(variablesDictionary_Building[Constant.ADDRESSLINE1]),
                        AccountAddressStreetAddress2 = (string)response.SelectToken(variablesDictionary_Building[Constant.ADDRESSLINE2])
                    },
                },
                FactoryDetails = unitIdetificationList
            };
            // mapping the Quote Id from DB
            if (string.IsNullOrEmpty(viewDetails?.Data?.Quotation?.OpportunityInfo?.OpportunityId) || string.IsNullOrEmpty(viewDetails?.Data?.Quotation?.OpportunityInfo?.JobName) || string.IsNullOrEmpty(viewDetails?.Data?.Quotation?.OpportunityInfo?.Branch) || string.IsNullOrEmpty(viewDetails?.Data?.Quotation?.OpportunityInfo?.SalesStage) || string.IsNullOrEmpty(viewDetails?.Data?.Quotation?.OpportunityInfo?.BusinessLine) || string.IsNullOrEmpty(viewDetails?.Data?.Quotation?.OpportunityInfo?.Salesman) || string.IsNullOrEmpty(viewDetails?.Data?.Quotation?.OpportunityInfo?.SalesmanActiveDirectoryID) || string.IsNullOrEmpty(viewDetails?.Data?.VersionId))
            {
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage { StatusCode = Constant.BADREQUEST, Message = Constant.VIEWERROEMESSAGE, Description = Constant.VIEWERROEMESSAGE };
            }
            var quoteIdList = _projectsdl.GenerateQuoteId(viewDetails, userName, parentVersionId);
            foreach (var quoteId in quoteIdList)
            {
                if (quoteId.Result == 0)
                {
                    throw new CustomException(new ResponseMessage()
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = quoteId.Message,
                        Description = Constant.VIEWERROEMESSAGE
                    });
                }
                viewDetails.ProjectInfoDetails.QuoteId = quoteId.QuoteId;
                viewDetails.ProjectInfoDetails.QuoteStatus = quoteId.QuoteStatus;
            }

            _cpqCacheManager.SetCache(sessionId, _environment, Constant.USERADDRESS, Utility.SerializeObjectValue(viewDetails.ProjectInfoDetails));
            _cpqCacheManager.SetCache(opportunityId + versionId, _environment, Constant.USERADDRESS, Utility.SerializeObjectValue(viewDetails));
            _cpqCacheManager.SetCache(viewDetails.ProjectInfoDetails.QuoteId, _environment, Constant.USERADDRESS, Utility.SerializeObjectValue(viewDetails));
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = JObject.Parse(JsonConvert.SerializeObject(viewDetails.ProjectInfoDetails)) };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param Name="projectId"></param>
        /// <param Name="versionId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> DeleteProjectById(string projectId, string versionId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var userName = GetUserId(sessionId);
            if (string.IsNullOrEmpty(versionId))
            {
                versionId = string.Empty;
            }
            var reslist = _projectsdl.DeleteProjectById(projectId, versionId, userName);
            var response = JArray.FromObject(reslist);
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
        }

        public List<BuildingVariableAssignment> Getvariablevalues(string opportunityId)
        {
            return _projectsdl.Getvariablevalues(opportunityId);
        }



        public List<VariableAssignment> GenerateVariableAssignmentsForProductTree(SetVariableAssignment setVariableAssignment)
        {
            var productName = string.Empty;
            var subModel = string.Empty;
            var methodBeginTime = Utility.LogBegin();
            var productNameMapperJson = Utility.GetVariableMapping(Constant.PROJECTCONSTANTMAPPER, Constant.PROJECTCOMMONNAME);
            var subModelMapperJson = Utility.GetVariableMapping(Constant.PROJECTCONSTANTMAPPER, Constant.PRODUCTTREESUBMODEL);
            if (subModelMapperJson.ContainsKey(setVariableAssignment.ProductName))
            {
                subModel = subModelMapperJson[setVariableAssignment.ProductName];
            }
            if (productNameMapperJson.ContainsKey(setVariableAssignment.ProductName))
            {
                productName = productNameMapperJson[setVariableAssignment.ProductName];
            }
            var variables = new List<VariableAssignment>();
            var variableAssignmentForProductTree = new VariableAssignment
            {
                VariableId = Constant.COMMONNAME_SP,
                Value = productName
            };
            var commonNameVariable = new VariableAssignment
            {
                VariableId = Constant.COMMONNAME,
                Value = productName
            };
            if (!string.IsNullOrEmpty(Convert.ToString(variableAssignmentForProductTree.Value)))
            {
                variables.Add(variableAssignmentForProductTree);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(commonNameVariable.Value)))
            {
                variables.Add(commonNameVariable);
            }
            if (setVariableAssignment.RearDoorSelected)
            {
                variableAssignmentForProductTree = new VariableAssignment()
                {
                    VariableId = Constant.PRODUCTTREEREAROPENVARIABLE,
                    Value = Constant.TRUEVALUES
                };
                variables.Add(variableAssignmentForProductTree);
            }
            var variableNameMapperJson = Utility.GetVariableMapping(Constant.PROJECTCONSTANTMAPPER, Constant.PRODUCTTREEVARIABLEMAPPER);
            var capacityVariable = GetListOfVariablesForProductTree(Constant.CAPACITY, variableNameMapperJson[Constant.CAPACITY], setVariableAssignment);
            variables.AddRange(capacityVariable);
            var carSpeedVariable = GetListOfVariablesForProductTree(Constant.CARSPEEDVARIABLE, variableNameMapperJson[Constant.CARSPEEDVARIABLE], setVariableAssignment);
            variables.AddRange(carSpeedVariable);
            var typsvcVariable = GetListOfVariablesForProductTree(Constant.TYPSVC, variableNameMapperJson[Constant.TYPSVC], setVariableAssignment);
            variables.AddRange(typsvcVariable);
            var jackTypevariable = GetListOfVariablesForProductTree(Constant.JACKTYPE, variableNameMapperJson[Constant.JACKTYPE], setVariableAssignment);
            variables.AddRange(jackTypevariable);
            var productTechnologyvariable = setVariableAssignment.UnitVariableAssignments.Where(x => Utilities.CheckEquals(x.VariableId, variableNameMapperJson[Constants.INCLINED])).ToList();
            if(Utility.CheckEquals(setVariableAssignment.ProductName, Constant.ORINOCO) && productTechnologyvariable.Any())
            {
                variables.Add(new VariableAssignment()
                {
                    VariableId = variableNameMapperJson[Constants.PRODUCTTECHNOLOGY],
                    Value = (Utilities.CheckEquals(Convert.ToString(productTechnologyvariable[0].Value), Constants.TRUEVALUES) ? Constants.INCLINEDCAMALCASE : Constants.HORIZONTAL)
                });
            }

            if (Utility.CheckEquals(setVariableAssignment.ProductName, Constant.HYDRAULIC) && jackTypevariable.Any()
                && subModelMapperJson.ContainsKey(Convert.ToString(jackTypevariable[0].Value)))
            {
                subModel = subModelMapperJson[Convert.ToString(jackTypevariable[0].Value)];
            }
            if (Utility.CheckEquals(setVariableAssignment.ProductName, Constant.GEARLESS))
            {
                var machineApplication = setVariableAssignment.UnitVariableAssignments.Where(x => x.VariableId.Contains(Constant.MACHINEAPPLICATION)).ToList();
                if (machineApplication.Any())
                {
                    subModel = subModel + Constant.EMPTYSPACE + Convert.ToString(machineApplication[0].Value).ToUpper();
                }
            }
            var varSubModelType = new VariableAssignment
            {
                VariableId = variableNameMapperJson[Constant.SUBMODELTYPE],
                Value = subModel
            };
            variables.Add(varSubModelType);
            Utility.LogEnd(methodBeginTime);
            return variables;
        }



        public async Task<ResponseMessage> GetProjectInfoForViewuser(string projectId, string versionId, string sessionId)
        {
            try
            {
                var methodBeginTime = Utility.LogBegin();
                var viewDetails = new ViewProjectDetails();
                var userName = GetUserId(sessionId);

                // Getting project Values
                var viewProjectDetails = await _projectsdl.GetProjectInfo(projectId, versionId).ConfigureAwait(false);
                var response = JObject.FromObject(viewProjectDetails.Response);
                _cpqCacheManager.SetCache(projectId + versionId, _environment, Constant.VIEWDATA, Utility.SerializeObjectValue(response));
                // View Variables Values Mapping
                var jObjectDictionry = (JObject.Parse(File.ReadAllText(Constant.VIEWVARIABLEMAPPING)));
                var viewVariables = jObjectDictionry[Constant.VIEWVARIABLES];
                var variablesDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(viewVariables));
                // Contact Variables Data
                var contactVariables = jObjectDictionry[Constant.CONTACTVRBLES];
                var variablesDictionary_Contact = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(contactVariables));
                // gCVariables data
                var gCVariables = jObjectDictionry[Constant.GCVARIABLES];
                var variablesDictionary_GC = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(gCVariables));
                // ownerVariables data
                var ownerVariables = jObjectDictionry[Constant.OWNERVARIABLES];
                var variablesDictionary_Owner = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(ownerVariables));
                // architectVariables data
                var architectVariables = jObjectDictionry[Constant.ARCHITECHVRBLES];
                var variablesDictionary_Architect = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(architectVariables));
                // billingVariables data
                var billingVariables = jObjectDictionry[Constant.BILLINGVRBLES];
                var variablesDictionary_Billing = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(billingVariables));
                // BuildingVariables data
                var BuildingVariables = jObjectDictionry[Constant.BUILDINGVRBLES];
                var variablesDictionary_Building = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(BuildingVariables));
                // quoteVariables data
                var quoteVariables = jObjectDictionry[Constant.QUOTEVRBLES];
                var variablesDictionary_QuoteInfo = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(quoteVariables));
                // building Info Variables data
                var buildingInfoVariables = jObjectDictionry[Constant.BUILDINGINFOVRBLES];
                var variablesDictionary_BuildingInfo = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(buildingInfoVariables));
                // Getting Units List Data 
                var unitsDetailsData = new List<UnitsData>();
                var unitsData = new UnitsData()
                {
                    BuildingInfo = new BuildingInfo()
                    {
                        City = (string)response.SelectToken(variablesDictionary_BuildingInfo[Constant.CITY]),
                        County = (string)response.SelectToken(variablesDictionary_BuildingInfo[Constant.COUNTRY]),
                        State = (string)response.SelectToken(variablesDictionary_BuildingInfo[Constant.STATE]),
                        ZipCode_bldg = (string)response.SelectToken(variablesDictionary_BuildingInfo[Constant.ZIPCODE_BLDG]),
                        AddressLine1_bldg = (string)response.SelectToken(variablesDictionary_BuildingInfo[Constant.ADDRESSLINE1_BLDG]),
                        AddressLine2_bldg = (string)response.SelectToken(variablesDictionary_BuildingInfo[Constant.ADDRESSLINE2_BLDG])
                    }
                };
                // Assign unit data Values
                unitsDetailsData.Add(unitsData);
                // Default Null Date
                DateTime? defaultNullableDate = null;
                // View Details Mapping
                viewDetails = new ViewProjectDetails()
                {
                    Code = (string)response.SelectToken(variablesDictionary[Constant.CODE]),
                    Message = (string)response.SelectToken(variablesDictionary[Constant.RETMSG]),
                    Data = new DataDetails()
                    {
                        VersionId = (string)response.SelectToken(variablesDictionary[Constant.VERSIONID]),
                        Quotation = new Quotation()
                        {
                            OpportunityInfo = new OpportunityInfo()
                            {
                                OpportunityId = (string)response.SelectToken(variablesDictionary[Constant.OPPORTNTYID]),
                                OpportunityURL = (string)response.SelectToken(variablesDictionary[Constant.OPPURL]),
                                BusinessLine = (string)response.SelectToken(variablesDictionary[Constant.BUSINESSLINE]),
                                JobName = (string)response.SelectToken(variablesDictionary[Constant.JOBNAMEVIEW]),
                                Branch = (string)response.SelectToken(variablesDictionary[Constant.BRANCH]),
                                SalesmanActiveDirectoryID = (string)response.SelectToken(variablesDictionary[Constant.SLSMNACTVDRCTRYID]),
                                Salesman = (string)response.SelectToken(variablesDictionary[Constant.SALESMAN]),
                                Category = (string)response.SelectToken(variablesDictionary[Constant.CATGRY]),
                                SalesEmail = (string)response.SelectToken(variablesDictionary[Constant.SALESEMAIL]),

                                AwardClosedDate = new AwardClosedDate()
                                {
                                    DateFormat = (string)response.SelectToken(variablesDictionary[Constant.DATEFORMAT]),
                                    DateValue = (!string.IsNullOrEmpty((string)response.SelectToken(variablesDictionary[Constant.DATEVALUE]))) ? Convert.ToDateTime(response.SelectToken(variablesDictionary[Constant.DATEVALUE])) : defaultNullableDate
                                },

                                ProposedDate = new ProposedDate()
                                {
                                    DateFormat = (string)response.SelectToken(variablesDictionary[Constant.PROPOSEDDATEFORMAT]),
                                    DateValue = (!string.IsNullOrEmpty((string)response.SelectToken(variablesDictionary[Constant.PROPOSEDDATE]))) ? Convert.ToDateTime(response.SelectToken(variablesDictionary[Constant.PROPOSEDDATE])) : defaultNullableDate
                                },

                                ContractBookedDate = new ContractBookedDate()
                                {
                                    DateFormat = (string)response.SelectToken(variablesDictionary[Constant.CONTRACTBOOKEDDATEFORMAT]),
                                    DateValue = (!string.IsNullOrEmpty((string)response.SelectToken(variablesDictionary[Constant.CONTRACTBOOKEDDATE]))) ? Convert.ToDateTime(response.SelectToken(variablesDictionary[Constant.CONTRACTBOOKEDDATE])) : defaultNullableDate
                                },
                                OraclePSNumber = (string)response.SelectToken(variablesDictionary[Constant.ORACLEPSNMBR]),
                                SalesStage = (string)response.SelectToken(variablesDictionary[Constant.SALESSTG]),
                                Superintendent = (string)response.SelectToken(variablesDictionary[Constant.SUPERINTENDENT])
                            },
                            Contact = new Contact()
                            {
                                FirstName = (string)response.SelectToken(variablesDictionary_Contact[Constant.FIRSTNAME]),
                                LastName = (string)response.SelectToken(variablesDictionary_Contact[Constant.LASTNAME]),
                                Email = (string)response.SelectToken(variablesDictionary_Contact[Constant.EMAIL]),
                                MobilePhone = (string)response.SelectToken(variablesDictionary_Contact[Constant.MOBILEPHONE]),
                            },
                            GC = new UserAddressDetailsDataValues()
                            {
                                AccountName = (string)response.SelectToken(variablesDictionary_GC[Constant.ACCOUNTNME]),
                                AddressLine1 = (string)response.SelectToken(variablesDictionary_GC[Constant.ADDRESSLINE1]),
                                AddressLine2 = (string)response.SelectToken(variablesDictionary_GC[Constant.ADDRESSLINE2]),
                                City = (string)response.SelectToken(variablesDictionary_GC[Constant.CITY]),
                                Country = (string)response.SelectToken(variablesDictionary_GC[Constant.COUNTRY]),
                                County = (string)response.SelectToken(variablesDictionary_GC[Constant.COUNTY]),
                                State = (string)response.SelectToken(variablesDictionary_GC[Constant.STATE]),
                                ZipCode = (string)response.SelectToken(variablesDictionary_GC[Constant.ZIPCODE]),
                                CustomerNumber = (string)response.SelectToken(variablesDictionary_GC[Constant.CUSTOMERNMBR])
                            },
                            Owner = new UserAddressDetailsDataValues()
                            {
                                AccountName = (string)response.SelectToken(variablesDictionary_Owner[Constant.ACCOUNTNME]),
                                AddressLine1 = (string)response.SelectToken(variablesDictionary_Owner[Constant.ADDRESSLINE1]),
                                AddressLine2 = (string)response.SelectToken(variablesDictionary_Owner[Constant.ADDRESSLINE2]),
                                City = (string)response.SelectToken(variablesDictionary_Owner[Constant.CITY]),
                                Country = (string)response.SelectToken(variablesDictionary_Owner[Constant.COUNTRY]),
                                County = (string)response.SelectToken(variablesDictionary_Owner[Constant.COUNTY]),
                                State = (string)response.SelectToken(variablesDictionary_Owner[Constant.STATE]),
                                ZipCode = (string)response.SelectToken(variablesDictionary_Owner[Constant.ZIPCODE]),
                                CustomerNumber = (string)response.SelectToken(variablesDictionary_Owner[Constant.CUSTOMERNMBR])
                            },
                            Architect = new UserAddressDetailsDataValues()
                            {
                                AccountName = (string)response.SelectToken(variablesDictionary_Architect[Constant.ACCOUNTNME]),
                                AddressLine1 = (string)response.SelectToken(variablesDictionary_Architect[Constant.ADDRESSLINE1]),
                                AddressLine2 = (string)response.SelectToken(variablesDictionary_Architect[Constant.ADDRESSLINE2]),
                                City = (string)response.SelectToken(variablesDictionary_Architect[Constant.CITY]),
                                Country = (string)response.SelectToken(variablesDictionary_Architect[Constant.COUNTRY]),
                                County = (string)response.SelectToken(variablesDictionary_Architect[Constant.COUNTY]),
                                State = (string)response.SelectToken(variablesDictionary_Architect[Constant.STATE]),
                                ZipCode = (string)response.SelectToken(variablesDictionary_Architect[Constant.ZIPCODE]),
                                CustomerNumber = (string)response.SelectToken(variablesDictionary_Architect[Constant.CUSTOMERNMBR])
                            },
                            Billing = new UserAddressDetailsDataValues()
                            {
                                AccountName = (string)response.SelectToken(variablesDictionary_Billing[Constant.ACCOUNTNME]),
                                AddressLine1 = (string)response.SelectToken(variablesDictionary_Billing[Constant.ADDRESSLINE1]),
                                AddressLine2 = (string)response.SelectToken(variablesDictionary_Billing[Constant.ADDRESSLINE2]),
                                City = (string)response.SelectToken(variablesDictionary_Billing[Constant.CITY]),
                                Country = (string)response.SelectToken(variablesDictionary_Billing[Constant.COUNTRY]),
                                County = (string)response.SelectToken(variablesDictionary_Billing[Constant.COUNTY]),
                                State = (string)response.SelectToken(variablesDictionary_Billing[Constant.STATE]),
                                ZipCode = (string)response.SelectToken(variablesDictionary_Billing[Constant.ZIPCODE]),
                                CustomerNumber = (string)response.SelectToken(variablesDictionary_Billing[Constant.CUSTOMERNMBR])
                            },
                            Building = new UserAddressDetailsDataValues()
                            {

                                AccountName = (string)response.SelectToken(variablesDictionary_Building[Constant.ACCOUNTNME]),
                                AddressLine1 = (string)response.SelectToken(variablesDictionary_Building[Constant.ADDRESSLINE1]),
                                AddressLine2 = (string)response.SelectToken(variablesDictionary_Building[Constant.ADDRESSLINE2]),
                                City = (string)response.SelectToken(variablesDictionary_Building[Constant.CITY]),
                                Country = (string)response.SelectToken(variablesDictionary_Building[Constant.COUNTRY]),
                                County = (string)response.SelectToken(variablesDictionary_Building[Constant.COUNTY]),
                                State = (string)response.SelectToken(variablesDictionary_Building[Constant.STATE]),
                                ZipCode = (string)response.SelectToken(variablesDictionary_Building[Constant.ZIPCODE]),
                                CustomerNumber = (string)response.SelectToken(variablesDictionary_Building[Constant.CUSTOMERNMBR])
                            },
                            Quote = new QuoteInfo()
                            {
                                QuoteDescription = (string)response.SelectToken(variablesDictionary_QuoteInfo[Constant.QUOTEDESCRIPTION]),
                                BaseBid = (string)response.SelectToken(variablesDictionary_QuoteInfo[Constant.QUOTEBASEBID]),
                                LatestViewVersion = (Boolean)response.SelectToken(variablesDictionary_QuoteInfo[Constant.LATESTVIEWVERSION]),
                                IsPrimary = ((Boolean)response.SelectToken(variablesDictionary_QuoteInfo[Constant.QUOTEBASEBID]) && (Boolean)response.SelectToken(variablesDictionary_QuoteInfo[Constant.LATESTVIEWVERSION])),
                                UIVersionId = (string)response.SelectToken(variablesDictionary_QuoteInfo[Constant.UIVERSIONID])
                            }

                        },
                        UnitsData = unitsDetailsData
                    },
                    ProjectInfoDetails = new OpportunityEntity()
                    {
                        Id = (string)response.SelectToken(variablesDictionary[Constant.OPPORTNTYID]),
                        OpportunityName = (string)response.SelectToken(variablesDictionary[Constant.JOBNAMEVIEW]),
                        LineOfBusiness = (string)response.SelectToken(variablesDictionary[Constant.BUSINESSLINE]),
                        SalesStage = (string)response.SelectToken(variablesDictionary[Constant.SALESSTG]),
                        AccountName = (string)response.SelectToken(variablesDictionary_Building[Constant.ACCOUNTNME]),
                        Branch = (string)response.SelectToken(variablesDictionary[Constant.BRANCH]),
                        CloseDate = (!string.IsNullOrEmpty((string)response.SelectToken(variablesDictionary[Constant.DATEVALUE]))) ? Convert.ToDateTime(response.SelectToken(variablesDictionary[Constant.DATEVALUE])) : defaultNullableDate,
                        BookingDate = (!string.IsNullOrEmpty((string)response.SelectToken(variablesDictionary[Constant.CONTRACTBOOKEDDATE]))) ? Convert.ToDateTime(response.SelectToken(variablesDictionary[Constant.CONTRACTBOOKEDDATE])) : defaultNullableDate,
                        ProposedDate = (!string.IsNullOrEmpty((string)response.SelectToken(variablesDictionary[Constant.PROPOSEDDATE]))) ? Convert.ToDateTime(response.SelectToken(variablesDictionary[Constant.PROPOSEDDATE])) : defaultNullableDate,
                        AccountAddress = new AccountEntity()
                        {
                            AccountName = (string)response.SelectToken(variablesDictionary_Building[Constant.ACCOUNTNME]),
                            AccountAddressCity = (string)response.SelectToken(variablesDictionary_Building[Constant.CITY]),
                            AccountAddressState = (string)response.SelectToken(variablesDictionary_Building[Constant.STATE]),
                            AccountAddressCountry = (string)response.SelectToken(variablesDictionary_Building[Constant.COUNTY]),
                            AccountAddressAddressZipCode = (string)response.SelectToken(variablesDictionary_Building[Constant.ZIPCODE]),
                            AccountAddressStreetAddress = (string)response.SelectToken(variablesDictionary_Building[Constant.ADDRESSLINE1]),
                            AccountAddressStreetAddress2 = (string)response.SelectToken(variablesDictionary_Building[Constant.ADDRESSLINE2])
                        },
                    },
                };

                // mapping the Quote Id from DB
                if (string.IsNullOrEmpty(viewDetails?.Data?.Quotation?.OpportunityInfo?.OpportunityId) || string.IsNullOrEmpty(viewDetails?.Data?.Quotation?.OpportunityInfo?.JobName) || string.IsNullOrEmpty(viewDetails?.Data?.Quotation?.OpportunityInfo?.Branch) || string.IsNullOrEmpty(viewDetails?.Data?.Quotation?.OpportunityInfo?.SalesStage) || string.IsNullOrEmpty(viewDetails?.Data?.Quotation?.OpportunityInfo?.BusinessLine) || string.IsNullOrEmpty(viewDetails?.Data?.Quotation?.OpportunityInfo?.Salesman) || string.IsNullOrEmpty(viewDetails?.Data?.Quotation?.OpportunityInfo?.SalesmanActiveDirectoryID) || string.IsNullOrEmpty(viewDetails?.Data?.VersionId))
                {
                    return new ResponseMessage { StatusCode = Constant.BADREQUEST, Message = Constant.VIEWERROEMESSAGE, Description = Constant.VIEWERROEMESSAGE };
                }
                var quoteIdList = _projectsdl.GenerateQuoteId(viewDetails, userName);
                foreach (var quoteId in quoteIdList)
                {
                    if (quoteId.Result == 0)
                    {
                        throw new CustomException(new ResponseMessage()
                        {
                            StatusCode = Constant.BADREQUEST,
                            Message = quoteId.Message,
                            Description = Constant.VIEWERROEMESSAGE
                        });
                    }
                    viewDetails.ProjectInfoDetails.QuoteId = quoteId.QuoteId;
                    viewDetails.ProjectInfoDetails.QuoteStatus = quoteId.QuoteStatus;
                }

                _cpqCacheManager.SetCache(sessionId, _environment, Constant.USERADDRESS, Utility.SerializeObjectValue(viewDetails.ProjectInfoDetails));
                _cpqCacheManager.SetCache(projectId + versionId, _environment, Constant.USERADDRESS, Utility.SerializeObjectValue(viewDetails));
                _cpqCacheManager.SetCache(viewDetails.ProjectInfoDetails.QuoteId, _environment, Constant.USERADDRESS, Utility.SerializeObjectValue(viewDetails));
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, QuoteId = viewDetails.ProjectInfoDetails.QuoteId, IsSuccessStatusCode = true };
            }
            catch (Exception ex)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Response = JObject.FromObject(ex),
                    Message = Constant.VIEWERROEMESSAGE,
                    Description = Constant.VIEWERROEMESSAGE
                });
            }
        }




        public static List<VariableAssignment> GetListOfVariablesForProductTree(string variableName, string VariableId, SetVariableAssignment setVariableAssignment)
        {
            return setVariableAssignment.UnitVariableAssignments.Where(oh => oh.VariableId.EndsWith(variableName)).Select(
                variableAssignment => new VariableAssignment
                {
                    VariableId = VariableId,
                    Value = variableAssignment.Value
                }).ToList<VariableAssignment>();
        }

        /// <summary>
        /// AddQuoteForProjectId
        /// </summary>
        /// <param name="opportunityId"></param>
        /// <param name="versionId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> AddQuoteForProject(string opportunityId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            //JObject response
            var userName = GetUserId(sessionId);
            var enrichmentData = new CreateProjectResponseObject();
            var projectStubFile = JObject.Parse(File.ReadAllText(Constant.PROJECTSENRICHEDDATA));
            enrichmentData.Sections = Utility.DeserializeObjectValue<List<Sections>>(Utility.SerializeObjectValue(projectStubFile[Constant.SECTIONSVALUES]));
            var enrichedDbResponse = _projectsdl.GetMiniProjectValues(sessionId, userName, opportunityId, enrichmentData);
            var variablesDictionary = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(enrichedDbResponse.VariableDetails));
            var requestVariableDetails = new VariableDetails()
            {
                ProjectId = variablesDictionary[Constant.PROJECTS_PROJECTID],
                ProjectName = variablesDictionary[Constant.PROJECTS_PROJECTNAME],
                VersionId = 0,
                Branch = variablesDictionary[Constant.PROJECTS_BRANCH],
                SalesStage = variablesDictionary[Constant.PROJECTS_SALESSATGE],
                Language = variablesDictionary[Constant.LAYOUT_LANGUAGE],
                MeasuringUnit = variablesDictionary[Constant.LAYOUT_MEASURINGUNIT],
                AccountName = variablesDictionary[Constant.ACCOUNTDETAIL_ACCOUNTNAME],
                AddressLine1 = variablesDictionary[Constant.ACCOUNTDETAIL_ADDRESS1],
                AddressLine2 = variablesDictionary[Constant.ACCOUNTDETAIL_ADDRESS2],
                Country = variablesDictionary[Constant.ACCOUNTDETAIL_COUNTRY],
                City = variablesDictionary[Constant.ACCOUNTDETAIL_CITY],
                State = variablesDictionary[Constant.ACCOUNTDETAIL_STATE],
                ZipCode = variablesDictionary[Constant.ACCOUNTDETAIL_ZIPCODE],
                Contact = variablesDictionary[Constant.ACCOUNTDETAIL_CONTACT],
                AwardCloseDate = Convert.ToDateTime(variablesDictionary[Constant.ACCOUNTDETAIL_AWARDCLOSEDATE])
            };

            var savedProjectResponse = _projectsdl.SaveAndUpdateMiniProjectValues(requestVariableDetails, userName, true);

            if (savedProjectResponse != null && savedProjectResponse.Any())
            {
                Utility.LogEnd(methodBeginTime);
                return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = Utility.FilterNullValues(savedProjectResponse.FirstOrDefault()) };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.SUCCESS,
                    Response = Utility.FilterNullValues(savedProjectResponse.FirstOrDefault()),
                    Message = Constant.ADDQUOTEERRORMESSAGE,
                    Description = Constant.ADDQUOTEERRORMESSAGE
                });
            }
        }

        /// <summary>
        /// AddToPrimaryQuoteBL
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> AddToPrimaryQuote(string quoteId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            //JObject response
            var userName = GetUserId(sessionId);
            var result = _projectsdl.SetQuoteToPrimaryDL(userName, quoteId);
            if (result?.Result == 0)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    QuoteId = result.QuoteId,
                    Message = result.Message,
                    Description = result.Message
                });
            }
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = Utility.FilterNullValues(result) };
        }
        /// <summary>
        /// DuplicateQuotesByQuoteId
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> DuplicateQuotesByQuoteId(string projectId, string quoteId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            //JObject response
            var userName = GetUserId(sessionId);
            var opportunityData = _configure.GetUserAddress(sessionId);
            string country;
            if (opportunityData == null)
            {
                country = projectId.Contains(Constant.SCUSER) ? Constant.CANADA : Constant.US;
            }
            else
            {
                var OpportunityData = (JsonConvert.DeserializeObject<OpportunityEntity>(opportunityData));
                country = OpportunityData.AccountAddress.AccountAddressCountry;
            }
            if (Utility.CheckEquals(country, Constant.CANADA))
            {
                country = Constant.CA;
            }
            else
            {
                country = Constant.US;
            }
            var duplicateQuotesResponse = _projectsdl.GetDuplicateQuoteByProjectIdDL(projectId, quoteId, userName, country);
            var response = JArray.FromObject(duplicateQuotesResponse);
            if (duplicateQuotesResponse[0].Result.Equals(-1))
            {
                Utility.LogEnd(methodBeginTime);
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = duplicateQuotesResponse[0].Message,
                    Description = Constant.SOMEERROROCCURED,
                    ResponseArray = response
                });
            }
            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = response };
        }

        /// <summary>
        /// GetUserLocation
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public bool GetViewUserCheck(string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var cachedUserDetail = _cpqCacheManager.GetCache(sessionId, _environment, Constant.USERDETAILSCPQ);
            User userDetail = new User();
            if (!string.IsNullOrEmpty(cachedUserDetail))
            {
                userDetail = Utility.DeserializeObjectValue<User>(cachedUserDetail);
            }
            Utility.LogEnd(methodBeginTime);
            return userDetail.IsViewUser;
        }
    }
}
