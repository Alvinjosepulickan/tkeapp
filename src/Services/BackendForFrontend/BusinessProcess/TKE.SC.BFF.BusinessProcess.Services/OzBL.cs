using Configit.Configurator.Server.Common;
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
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.Common;
using TKE.SC.PIPO;
using Constants = TKE.SC.Common.Constants;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class OzBL : IOzBL
    {
        #region Variables
        private readonly string _environment = Constant.DEV;
        private readonly ICacheManager _cpqCacheManager;
        private readonly IOzDL _ozdl;
        private readonly IProject _projectsbl;
        private readonly IPipoConnector _pipoConnector;
        /// <summary>
        /// Configure
        /// </summary>
        private readonly IConfigure _configure;
        private readonly IObom _obomBL;
        private readonly IDesignAutomation _daBL;
        #endregion

        /// <summary>
        /// Constructor for OzBL
        /// </summary>
        /// <param Name="ozDL"></param>
        /// <param Name="configure"></param>
        /// <param Name="cpqCacheManager"></param>
        /// <param Name="projectsBL"></param>
        /// <param Name="logger"></param>
        public OzBL(IOzDL ozDL, IConfigure configure, ICacheManager cpqCacheManager, IProject projectsBL, ILogger<OzBL> logger, IObom obomBL, IDesignAutomation daBL, IPipoConnector pipoConnector)
        {
            _ozdl = ozDL;
            _projectsbl = projectsBL;
            _cpqCacheManager = cpqCacheManager;
            _configure = configure;
            _obomBL = obomBL;
            _daBL = daBL;
            _pipoConnector = pipoConnector;
            Utility.SetLogger(logger);
        }



        /// <summary>
        /// Booking request.
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> BookingRequest(string quoteId, string sessionId)
        {
            var bookingRequest = await GenerateBookingRequestPayload(quoteId, sessionId).ConfigureAwait(false);
            var listOfConfiguration= _cpqCacheManager.GetCache(sessionId, _environment, Constants.LISTOFCONFIGURATION);
            var bookCoordinationResponse= await _ozdl.BookCoOrdination(quoteId, sessionId, bookingRequest).ConfigureAwait(false);
            var obomResponseList = false;
            if (listOfConfiguration != null)
            {
                foreach (var building in Utility.DeserializeObjectValue<List<ListOfConfiguration>>(listOfConfiguration))
                {
                    if (building?.Groups != null)
                    {
                        foreach (var group in building.Groups)
                        {
                            ConfigurationDetails configurationDetails = new ConfigurationDetails()
                            {
                                GroupId = group.groupId,
                                QuoteId=quoteId
                            };
                            var obomResponse = await _obomBL.GETOBOMResponse(configurationDetails, sessionId).ConfigureAwait(false);
                            if (obomResponse.XmlDocument.Any())
                            {
                                obomResponseList=true;
                                foreach (var obomXml in obomResponse.XmlDocument)
                                {
                                    var output = _pipoConnector.CreateOrUpdateSpecMemoAsync(Convert.ToString(obomXml), TKE.SC.PIPO.Constants.ContentType.XmlString, TKE.SC.PIPO.Constants.PiPoSettings).ConfigureAwait(false);
                                    if (output.GetAwaiter().GetResult())
                                        Utility.LogInfo("Xmls succesfully send to MFiles");
                                }
                            }
                            await _daBL.GetDAResponse(configurationDetails, sessionId).ConfigureAwait(false);
                        }
                    }
                }
            }
            //if(obomResponseList)
            //{
            //    await _obomBL.GenerateExcelForStatusReport(new ConfigurationDetails() { QuoteId = quoteId }).ConfigureAwait(false);
            //}
            return bookCoordinationResponse;
        }

        /// <summary>
        /// Form the Oz payload for a booking request.
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<OzBookingRequest> GenerateBookingRequestPayload(string quoteId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            OzBookingRequest oZBookingRequest = new OzBookingRequest
            {
                ProjectInformation = new ProjectInformation(),
                RequestedDrawing = new RequestedDrawing(),
                Equipment = new List<Equipment>()
            };
            Guid transcationId = Guid.NewGuid();

            var projectDictionary = _ozdl.GetProjectIdVersionId(quoteId);
            string projectId = projectDictionary[Constant.OPPORTUNITYID];
            string versionId = projectDictionary[Constant.VERSIONID];

            JObject OzRequest = JObject.Parse(File.ReadAllText(Constant.OZREQUESTBODY));
            var value = Utility.DeserializeObjectValue<JObject>(OzRequest.ToString());
            var projInfoTemplate = Utility.SerializeObjectValue(value[Constant.PROJECTINFORMATION]);
            //TODO:Find better to pick the data
            var projectInfoData = string.Empty;
            var proj = new ProjectInformation();
            if (projectId.Contains(Constant.SCUSER))
            {
                projectInfoData = _cpqCacheManager.GetCache(sessionId, _environment, Constant.USERADDRESS);
                if (string.IsNullOrEmpty(projectInfoData))
                {
                    await _projectsbl.GetProjectInfo(projectId, versionId, sessionId).ConfigureAwait(false);
                    projectInfoData = _cpqCacheManager.GetCache(quoteId, _environment, Constant.USERADDRESS);
                }
                var miniProjectDetails = Utility.DeserializeObjectValue<OpportunityEntity>(projectInfoData);
                var boundTemplate = BindDataTojsonTemplate(projInfoTemplate, miniProjectDetails);
                proj = Utility.DeserializeObjectValue<ProjectInformation>(boundTemplate);
                proj.ProjectIdentifier.ProjectVersionId = versionId;

            }
            else
            {
                projectInfoData = _cpqCacheManager.GetCache(quoteId, _environment, Constant.USERADDRESS);
                if (string.IsNullOrEmpty(projectInfoData))
                {
                    await _projectsbl.GetProjectInfo(projectId, versionId, sessionId).ConfigureAwait(false);
                    projectInfoData = _cpqCacheManager.GetCache(quoteId, _environment, Constant.USERADDRESS);
                }
                var viewDetails = Utility.DeserializeObjectValue<ViewProjectDetails>(projectInfoData);
                var boundTemplate = BindDataTojsonTemplate(projInfoTemplate, viewDetails);
                proj = Utility.DeserializeObjectValue<ProjectInformation>(boundTemplate);
            }

            proj.Branch.identifier.OracleId = _ozdl.GetBranchId(proj.Branch.Name);
            proj.ProjectIdentifier.ProjectId = projectId;
            proj.ProjectIdentifier.TransactionId = transcationId.ToString();
            proj.ProjectIdentifier.QuoteId = quoteId;


            var equipmentAndDrawing = _ozdl.GetEquipmentAndDrawingForOZ(quoteId);
            var productTreeVariables = new List<SetVariableAssignment>();
            var ozVariables = Utility.GetVariableMapping(Constant.INTEGRATIONCONSTANTMAPPER, Constant.OZ);
            foreach (var set in equipmentAndDrawing.SetConfigurationDetails)
            {
                var setVariableAssignment = new SetVariableAssignment
                {
                    SetId = set.SetId,
                    UnitVariableAssignments = set.VariableAssignments,
                    RearDoorSelected = set.RearDoorSelected,
                    ProductName = set.ProductName
                };
                var lineObject = new Line() { VariableAssignments = _projectsbl.GenerateVariableAssignmentsForProductTree(setVariableAssignment) };
                var configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineObject), Constant.PRODUCTTREE);
                var configResponse = await _configure.GetByDefaultOrRulevaluesFromPackage(configRequest, sessionId).ConfigureAwait(false);
                setVariableAssignment.ProductTreeVariables = new Dictionary<string, object>();
                setVariableAssignment.ProductTreeVariables = JsonConvert.DeserializeObject<Dictionary<string, object>>(configResponse[Constant.CONFIGURATION].ToString());
                productTreeVariables.Add(setVariableAssignment);

            }
            foreach (var unit in equipmentAndDrawing.equipment)
            {
                var setid = (from a in equipmentAndDrawing.UnitDictionary
                             where a.Value.Contains(unit.EstimateIdentifier.LineId)
                             select a.Key).ToList().FirstOrDefault();
                unit.General.Product.ProductLineIdName = (from produtreevariable in productTreeVariables
                                                          where produtreevariable.SetId.Equals(setid) && produtreevariable.ProductTreeVariables.ContainsKey(ozVariables[Constant.PRODUCTMODELSP])
                                                          select Convert.ToString(produtreevariable.ProductTreeVariables[ozVariables[Constant.PRODUCTMODELSP]])).ToList().FirstOrDefault();
                if (!string.IsNullOrEmpty(unit?.General?.Product?.ProductLineIdName))
                {
                    unit.General.Product.ProductLineIdName = unit.General.Product.ProductLineIdName.Replace(Constant.UNDERSCORECHAR, Constant.SPACECHAR);
                }
                else
                {
                    unit.General.Product.ProductLineIdName = string.Empty;
                }
                unit.General.Model.ProductModel = (from produtreevariable in productTreeVariables
                                                   where produtreevariable.SetId.Equals(setid) && produtreevariable.ProductTreeVariables.ContainsKey(ozVariables[Constant.TKEFACTORYMODEL])
                                                   select Convert.ToString(produtreevariable.ProductTreeVariables[ozVariables[Constant.TKEFACTORYMODEL]])).ToList().FirstOrDefault();
                if (!string.IsNullOrEmpty(unit?.General?.Model?.ProductModel))
                {
                    unit.General.Model.ProductModel = unit.General.Model.ProductModel.Replace(Constant.UNDERSCORECHAR, Constant.SPACECHAR);
                }
                else
                {
                    unit.General.Model.ProductModel = string.Empty;
                }
            }

            oZBookingRequest.ProjectInformation = proj;
            oZBookingRequest.Equipment = equipmentAndDrawing.equipment;
            oZBookingRequest.RequestedDrawing = equipmentAndDrawing.requestedDrawing;

            Utility.LogEnd(methodBeginTime);
            return oZBookingRequest;
        }
        private string BindDataTojsonTemplate(string template, object dataToBind)
        {
            var builder = new Stubble.Core.Builders.StubbleBuilder();
            var boundTemplate = builder.Build().Render(template, dataToBind);
            return boundTemplate;
        }
    }
}