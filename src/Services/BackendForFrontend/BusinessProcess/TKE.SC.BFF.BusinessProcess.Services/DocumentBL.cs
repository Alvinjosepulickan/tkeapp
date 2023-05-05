using Configit.Configurator.Server.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.VaultModel;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using System.Data;
using TKE.SC.Common;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class DocumentBL: IDocument
    {
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
        /// string
        /// </summary>
        private readonly string _environment;
        private readonly IDocumentDL _document;
        private readonly IVaultDL _vault;
        private IConfiguration _configuration;
        private IEnumerable<PriceValuesDetails> _staticPriceKeys;
        public DocumentBL(IConfigure configure, IUnitConfigurationDL unitdl, ILogger<UnitConfigurationBL> logger, IProject _project,
            IDocumentDL document, IVaultDL vault, ICacheManager cpqCacheManager, IConfiguration iConfig)
		{
            _configure = configure;
            _document = document;
            _vault = vault;
            _projectDl = _project;
            _unitConfigurationdl = unitdl;
            Utility.SetLogger(logger);
            _cpqCacheManager = cpqCacheManager;
            _environment = Constant.DEV;
            _configuration = iConfig;
            _staticPriceKeys = ReadPriceValues();
        }

        /// <summary>
        /// GetTP2SummaryDocumentGenerationDetails
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="UnitId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetDataForDocumentGeneration(int setId, int unitId, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            var result = _unitConfigurationdl.GetDetailsForTP2SummaryScreen(setId);
            var productDetails = result.UnitDetails.Where(x => x.UnitId == unitId).ToList()[0].ProductName;

            // to get the unitConfiguration response
            //for building
            //for group
            var unitVaribaleAssignmentValues = result.VariableAssignments.Where(s => s.Id.Equals(Constant.UNITVARIABLESLIST)).FirstOrDefault();
            var buildingVaribaleAssignmentValues = result.VariableAssignments.Where(s => s.Id.Equals(Constant.BUILDINGVARIABLESLIST)).FirstOrDefault();
            var lineObject = new Line() { VariableAssignments = Utility.DeserializeObjectValue<List<VariableAssignment>>(Utility.SerializeObjectValue(buildingVaribaleAssignmentValues.VariableAssignments)) };
            var configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineObject), Constant.BUILDINGNAME);
            var configResponse = await _configure.GetByDefaultOrRulevaluesFromPackage(configRequest, sessionId);
            Dictionary<string, object> configVariables = JsonConvert.DeserializeObject<Dictionary<string, object>>(configResponse[Constant.CONFIGURATION].ToString());

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

            var unitValues = result.UnitDetails;
            // Get the Unit Arguments
            var unitConfigurationArgumentsResponse = await _configure.UnitsConfigurationArgumentsResponse(lstvariableassignment, sessionId, productDetails);
            // Get the Response for Prices
            var unitResponseObj = _cpqCacheManager.GetCache(sessionId, _environment, Constant.PRICESECTION);
            if (unitResponseObj == null)
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR,
                    Message = Constant.TP2SUMMARYERROR
                });
            }
            var unitsTp2Response = Utility.DeserializeObjectValue<UnitSummaryUIModel>(unitResponseObj);


            configVariables.ToList().ForEach(x => { if (!unitConfigurationArgumentsResponse.ContainsKey(x.Key)) { unitConfigurationArgumentsResponse.Add(x.Key, x.Value); } });
            buildingVaribaleAssignmentValues = result.VariableAssignments.Where(s => s.Id.Equals(Constant.GROUPVARIABLESLIST)).FirstOrDefault();
            var groupVariableAssignments = Utility.DeserializeObjectValue<List<VariableAssignment>>(Utility.SerializeObjectValue(buildingVaribaleAssignmentValues.VariableAssignments));
            lineObject = new Line() { VariableAssignments = groupVariableAssignments };
            configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineObject), Constant.GROUPCONFIGURATIONNAME);
            configResponse = await _configure.GetByDefaultOrRulevaluesFromPackage(configRequest, sessionId);
            configVariables = JsonConvert.DeserializeObject<Dictionary<string, object>>(configResponse[Constant.CONFIGURATION].ToString());
            configVariables.ToList().ForEach(x => { if (!unitConfigurationArgumentsResponse.ContainsKey(x.Key)) { unitConfigurationArgumentsResponse.Add(x.Key, x.Value); } });
            groupVariableAssignments.ForEach(x => { if (!unitConfigurationArgumentsResponse.ContainsKey(x.VariableId)) { unitConfigurationArgumentsResponse.Add(x.VariableId, x.Value); } });
            var variableAssignmentList = new List<VariableAssignment>();
            var rearDoorSelected = false;

            var constantValues = JObject.Parse(File.ReadAllText(Constant.DOCUMENTGENERATIONMAPPERPATH));
            var frontRisers = constantValues[Constant.FRONTRISERS].ToObject<List<string>>();
            var rearRisers = constantValues[Constant.REARRISERS].ToObject<List<string>>();
            var noOfFrontRisers = (from riser in frontRisers
                                   from variableAssignment in buildingVaribaleAssignmentValues.VariableAssignments
                                   where (Utility.CheckEquals(variableAssignment.VariableId, riser))
                                   select riser).ToList().Count();
            var noOfRearRisers = (from riser in rearRisers
                                  from variableAssignment in buildingVaribaleAssignmentValues.VariableAssignments
                                  where (Utility.CheckEquals(variableAssignment.VariableId, riser))
                                  select riser).ToList().Count();
            foreach (var variableAssignment in unitConfigurationArgumentsResponse.ToList())
            {
                if (variableAssignment.Key.Contains("rearDoorTypeAndHand_SP") && !Utility.CheckEquals(Convert.ToString(variableAssignment.Value), "NR"))
                {
                    rearDoorSelected = true;
                }
                variableAssignmentList.Add(new VariableAssignment()
                {
                    VariableId = variableAssignment.Key,
                    Value = variableAssignment.Value
                });
            }
            var setVariableAssignment = new SetVariableAssignment()
            {
                ProductName = productDetails,
                UnitVariableAssignments = variableAssignmentList,
                RearDoorSelected = rearDoorSelected
            };

            var VariableAssignmentsForProductTree = _projectDl.GenerateVariableAssignmentsForProductTree(setVariableAssignment);
            lineObject.VariableAssignments = VariableAssignmentsForProductTree;
            configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineObject), Constant.PRODUCTTREE);
            configResponse = await _configure.GetByDefaultOrRulevaluesFromPackage(configRequest, sessionId).ConfigureAwait(false);
            if (configResponse != null)
            {
                var productTreeVariables = JsonConvert.DeserializeObject<Dictionary<string, object>>(configResponse[Constant.CONFIGURATION].ToString());
                productTreeVariables.ToList().ForEach(x => { if (!unitConfigurationArgumentsResponse.ContainsKey(x.Key)) { unitConfigurationArgumentsResponse.Add(x.Key, x.Value); } });

            }
            var mainDocumentResponse = new Tp2DocumentGeneration()
            {
                Culture = "en-US",
                Identifier = result?.projectInfo?.Source,
                PriceSections = FilterPriceSections(unitsTp2Response.PriceSections, unitId,unitValues),
                PriceValue = unitsTp2Response.PriceValue,
                Variables = unitConfigurationArgumentsResponse
            };
            var docGenUrl = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constant.DOCUMENTGENERATOR);
            var baseUrl = Utility.GetPropertyValue(docGenUrl, Constant.BASEURL);
            var configurationPath = string.Format(Utility.GetPropertyValue(docGenUrl, Constant.CONFIGURATIONPATH), result.ProjectData.OpportunityId, result.ProjectData.VersionId, result.projectInfo.QuoteId);
            var buildingConfigurationPath = Utility.GetPropertyValue(docGenUrl, Constant.BUILDINGCONFIGURATIONURL);
            var groupConfigurationPath = Utility.GetPropertyValue(docGenUrl, Constant.GROUPCONFIGURATIONURL);
            var unitConfigurationPath = Utility.GetPropertyValue(docGenUrl, Constant.UNITCONFIGURATIONURL);
            var buildingDataForDocGen = new PriceSectionDetails()
            {
                Id = "Project Summary",
                Name = "Project Summary",
                Section = "1",
                PriceKeyInfo = new List<PriceValuesDetails>()
            };
            foreach (var item in result.QuoteSummary)
            {
                var buildingData = new PriceValuesDetails()
                {
                    Section = item.BuildingName,
                    ItemNumber = item.GroupName,
                    PartDescription = item.UnitName,
                    ComponentName = Convert.ToString(item.NumberOfUnits),
                    SectionName = item.OrderType,
                    Parameter3 = baseUrl + configurationPath + string.Format(buildingConfigurationPath, item.BuildingId),
                    Parameter3Value = baseUrl + configurationPath + string.Format(groupConfigurationPath, item.BuildingId, item.GroupId),
                    LeadTime = item.ProductLine.Equals(Constant.CLOSESQUAREBRACKET) ? string.Empty : item.ProductLine,
                    BatchNo = item.OrderType,
                    Parameter4 = baseUrl + configurationPath + string.Format(unitConfigurationPath, item.BuildingId, item.GroupId, item.SetId)
                };
                if(string.IsNullOrEmpty(item.SetId) || Convert.ToInt32(item.SetId)==0)
                {
                    buildingData.Parameter4 = string.Empty;
                }
                buildingDataForDocGen.PriceKeyInfo.Add(buildingData);
            }
            //mainDocumentResponse.Variables.Add(configVariables);
            var unitDetails = unitValues.Where(x => x.UnitId == unitId).ToList()[0];
            var projectInfo = await _projectDl.GetProjectDetails(result?.projectInfo?.ProjectId, result?.projectInfo?.QuoteVersion, sessionId).ConfigureAwait(false);
            if (projectInfo != null)
            {
                var viewData = Utility.DeserializeObjectValue<OpportunityEntity>(Utility.SerializeObjectValue(projectInfo.Response));
                mainDocumentResponse.Variables.Add(Constant.CUSTOMERACCOUNT, viewData?.AccountName);
                mainDocumentResponse.Variables.Add(Constant.PROJECTSALESTAGE, viewData?.SalesStage);
            }
            var leadTime = 0;
            if (unitsTp2Response?.PriceSections != null)
            {
                decimal totalPrice = 0;
                foreach (var priceSection in unitsTp2Response.PriceSections)
                {
                    if (priceSection != null && priceSection.PriceKeyInfo!=null)
                    {
                        foreach (var priceKeyInfo in priceSection.PriceKeyInfo)
                        {
                            if (priceKeyInfo != null && !string.IsNullOrEmpty(priceKeyInfo.ItemNumber))
                            {
                                foreach (var priceKey in unitsTp2Response.PriceValue)
                                {
                                    if(Utility.CheckEquals(Convert.ToString(priceKey.Key),(Convert.ToString(priceKeyInfo.ItemNumber))) && priceKey.Value!=null )
                                    {
                                        totalPrice += priceKey.Value.totalPrice;
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var priceSection in unitsTp2Response.PriceSections)
                {
                    if (priceSection?.PriceKeyInfo != null && !string.IsNullOrEmpty(priceSection.Section))
                    {
                        foreach (var priceKeyInfo in priceSection.PriceKeyInfo)
                        {
                            if (priceKeyInfo?.LeadTime != null && Convert.ToInt32(priceKeyInfo.LeadTime) > leadTime)
                            {
                                leadTime = Convert.ToInt32(priceKeyInfo.LeadTime);
                            }
                        }
                        if (unitsTp2Response?.Variables != null)
                        {
                            foreach (var userVariable in unitsTp2Response.Variables)
                            {
                                if (userVariable.Value != null && !String.IsNullOrEmpty(userVariable.Value.ToString()) && userVariable.Id != "ManufacturingComments")
                                {
                                    var newPriceKeyInfo = new PriceValuesDetails
                                    {
                                        ComponentName = userVariable.Id,
                                        ItemNumber = userVariable.Id + priceSection.Section,
                                        PartDescription = userVariable.Id,
                                        Section = priceSection.Section
                                    };
                                    priceSection.PriceKeyInfo.Add(newPriceKeyInfo);
                                    CreateNewPriceKeyBasedOnUserVariables(newPriceKeyInfo, unitsTp2Response, unitId, totalPrice);
                                }

                            }
                        }
                    }
                }
            }
            var documentGenerationMapper = Utility.GetVariableMapping(Constant.DOCUMENTGENERATIONMAPPERPATH, Constant.ORDERFORM);
            mainDocumentResponse.Variables.Add(Constant.UNITIDCOLUMNID, unitDetails?.UnitId);
            mainDocumentResponse.Variables.Add(Constant.UNITNAMECOLUMNNAME, unitDetails?.UnitName);
            mainDocumentResponse.Variables.Add(Constant.UEID.ToLower(), unitDetails?.Ueid);
            mainDocumentResponse.Variables.Add(Constant.PRODUCTNAMECOLUMN, unitDetails?.ProductName);
            mainDocumentResponse.Variables.Add(Constant.PROJECTNAME, result?.projectInfo?.ProjectName);
            mainDocumentResponse.Variables.Add(Constant.PROJECTIDCOLUMNNAME, result?.projectInfo?.ProjectId);
            mainDocumentResponse.Variables.Add(Constant.BRANCH, result?.projectInfo?.Branch);
            mainDocumentResponse.Variables.Add(Constant.BUILDINGNAMECOLUMNNAME, result?.projectInfo?.BuildingName);
            mainDocumentResponse.Variables.Add(Constant.QUOTEVERSION, result?.projectInfo?.QuoteVersion);
            mainDocumentResponse.Variables.Add(Constant.GROUPNAMECOLUMNNAME, result?.projectInfo?.GroupName);
            mainDocumentResponse.Variables.Add(Constant.ORACLEPROJECTID, result?.projectInfo?.OracleProjectId);
            mainDocumentResponse.Variables.Add(Constant.PRIMARYCOORDINATOR, result?.projectInfo?.PrimaryCoordinator);
            mainDocumentResponse.Variables.Add(Constant.FRONTOPENING, result?.projectInfo?.FrontOpenings);
            mainDocumentResponse.Variables.Add(Constant.REAROPENING, result?.projectInfo?.RearOpenings);
            mainDocumentResponse.Variables.Add(Constant.PROJECTSTATUS, result?.projectInfo?.ProjectStatus);
            mainDocumentResponse.Variables.Add(Constant.UNITMFGJOBNO, result?.projectInfo?.UnitMFGJobNo);
            mainDocumentResponse.Variables.Add(Constant.UNITSTATUS, result?.projectInfo?.Status);
            mainDocumentResponse.Variables.Add(Constant.TRAVEL.ToLower(), Utility.ConvertInchtoFeet(result.projectInfo.Travel));
            mainDocumentResponse.Variables = ConvertTOFeetInch(mainDocumentResponse.Variables, documentGenerationMapper[Constant.ELEVATIONATBUILDINGBASE]);
            mainDocumentResponse.Variables = ConvertTOFeetInch(mainDocumentResponse.Variables, documentGenerationMapper[Constant.AVEREAGEROOFHEIGHT]);
            mainDocumentResponse.Variables = ConvertTOFeetInch(mainDocumentResponse.Variables, documentGenerationMapper[Constant.FRONTDOORWIDTH]);
            mainDocumentResponse.Variables = ConvertTOFeetInch(mainDocumentResponse.Variables, documentGenerationMapper[Constant.REARDOORWIDTH]);
            mainDocumentResponse.Variables = ConvertTOFeetInch(mainDocumentResponse.Variables, documentGenerationMapper[Constant.FRONTDOORHEIGHT]);
            mainDocumentResponse.Variables = ConvertTOFeetInch(mainDocumentResponse.Variables, documentGenerationMapper[Constant.REARDOORHEIGHT]);
            mainDocumentResponse.Variables.ToList().ForEach(x => { if (x.Key.Contains(documentGenerationMapper[Constant.REEVING]) && Convert.ToInt32(x.Value) == 1) { mainDocumentResponse.Variables[x.Key] = "1:1"; } });
            mainDocumentResponse.Variables.ToList().ForEach(x => { if (x.Key.Contains(documentGenerationMapper[Constant.REEVING]) && Convert.ToInt32(x.Value) == 2) { mainDocumentResponse.Variables[x.Key] = "2:1"; } });
            mainDocumentResponse.Variables.Add(Constant.LEADTIME, leadTime);
            var unitDetailsList=result.UnitDetails.Where(x => x.UnitId == unitId).ToList();
            if(unitDetailsList.Any())
            {
                mainDocumentResponse.Variables[Constants.FACTORYJOBID] = unitDetailsList[0].FactoryJobID;
            }
            mainDocumentResponse.Variables.ToList().ForEach
                (x => 
                { if ((x.Key.Equals(documentGenerationMapper[Constant.HOISTWAYDEPTH]) ||
                x.Key.Equals(documentGenerationMapper[Constant.HOISTWAYWIDTH]) || 
                x.Key.Equals(documentGenerationMapper[Constant.PITDEPTH])) &&
                Convert.ToInt32(x.Value) < 12) 
                    { mainDocumentResponse.Variables[x.Key] = 12 * Convert.ToInt32(x.Value); 
                    } 
                }
                );

            if (mainDocumentResponse.Variables.ContainsKey(documentGenerationMapper[Constant.REARDOOR.ToUpper()]) &&
                Utility.CheckEquals(mainDocumentResponse.Variables[documentGenerationMapper[Constant.REARDOOR.ToUpper()]].ToString(), Constant.NR))
            {
                if (mainDocumentResponse.Variables.ContainsKey(documentGenerationMapper[Constant.REARDOORWIDTHVTPACKAGEVARIABLE]))
                {
                    mainDocumentResponse.Variables.Remove(documentGenerationMapper[Constant.REARDOORWIDTHVTPACKAGEVARIABLE]);
                }
                if (mainDocumentResponse.Variables.ContainsKey(documentGenerationMapper[Constant.REARDOORHEIGHTVTPACKAGEVARIABLE]))
                {
                    mainDocumentResponse.Variables.Remove(documentGenerationMapper[Constant.REARDOORHEIGHTVTPACKAGEVARIABLE]);
                }
            }
            if (!result.ProjectData.OpportunityId.StartsWith(Constant.SCUSER))
            {
                var viewData = _cpqCacheManager.GetCache(result.ProjectData.OpportunityId + result.ProjectData.VersionId, _environment, Constant.VIEWDATA);
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
                    var viewDataJObject = Utility.DeserializeObjectValue<JObject>(viewData);
                    DateTime? defaultNullableDate = null;
                    var j = (string)viewDataJObject.SelectToken(variablesDictionary_Contact[Constant.MOBILEPHONE]);
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
                    mainDocumentResponse.Variables.Add("Account", (string)viewDataJObject.SelectToken(variablesDictionary_Building[Constant.ACCOUNTNME]));
                    mainDocumentResponse.Variables.Add("Contact", (string)viewDataJObject.SelectToken(variablesDictionary_Contact[Constant.MOBILEPHONE]));
                    mainDocumentResponse.Variables.Add("Address", address);
                    mainDocumentResponse.Variables.Add("SalesStage", (string)viewDataJObject.SelectToken(variablesDictionary[Constant.SALESSTG]));
                    mainDocumentResponse.Variables.Add("ProjectStatusValue", result.ProjectData.ProjectStatus);
                    mainDocumentResponse.Variables.Add("QuotedDate", result.ProjectData.QuoteDate);
                    mainDocumentResponse.Variables.Add("BookedDate", (!string.IsNullOrEmpty((string)viewDataJObject.SelectToken(variablesDictionary[Constant.CONTRACTBOOKEDDATE]))) ? 
                        Convert.ToDateTime(viewDataJObject.SelectToken(variablesDictionary[Constant.CONTRACTBOOKEDDATE])) 
                        : defaultNullableDate);
                    mainDocumentResponse.Variables.Add("OraclePSNumber", (string)viewDataJObject.SelectToken(variablesDictionary[Constant.ORACLEPSNMBR]));
                    mainDocumentResponse.Variables.Add("SalesMan", (string)viewDataJObject.SelectToken(variablesDictionary[Constant.SALESMAN]));
                    mainDocumentResponse.Variables[Constant.BRANCH] = (string)viewDataJObject.SelectToken(variablesDictionary[Constant.BRANCH]);
                }
            }
            else
            {
                mainDocumentResponse.Variables.Add("Account", result.ProjectData.CustomerAccount);
                mainDocumentResponse.Variables.Add("Contact", result.ProjectData.Contact);
                mainDocumentResponse.Variables.Add("Address", result.ProjectData.Address);
                mainDocumentResponse.Variables.Add("SalesStage", result.ProjectData.SalesStage);
                mainDocumentResponse.Variables.Add("ProjectStatusValue", result.ProjectData.ProjectStatus);
                mainDocumentResponse.Variables.Add("QuotedDate", result.ProjectData.QuoteDate);
                mainDocumentResponse.Variables.Add("BookedDate", result.ProjectData.BookeDate);

            }
            mainDocumentResponse.Variables.Add(Constants.PROJECTQUOTESTATUSKEY, result.ProjectData.QuoteStatus);
            mainDocumentResponse.Variables.Add(Constant.MAINEGRESS, result.MainEgress);
            mainDocumentResponse.Variables.Add(Constant.ALTERNATEEGRESS, result.AlternateEgress);
            mainDocumentResponse.Variables.Add(Constant.NOOFFRONTRISERS, noOfFrontRisers);
            mainDocumentResponse.Variables.Add(Constant.NOOFREARRISERS, noOfRearRisers);
            mainDocumentResponse.Variables.Add(Constant.NOOFFRONTINCONFRISERS, result.NoOfInconRisersFront);
            mainDocumentResponse.Variables.Add(Constant.NOOFREARINCONFRISERS, result.NoOfInconRisersRear);
            //need to remove
            mainDocumentResponse.Variables.Add("MACHPN", "PMB285 L3.0");
            mainDocumentResponse.Variables.Add("ProductConfiguration", "Lorem ipsum");

            var manufacturingComments = result.FloorMatrixTable.Where(x => Utilities.CheckEquals(x.Name ,Constants.MANUFACTURINGCOMMENTSTABLENAME)).ToList();
            if (manufacturingComments.Any())
            {
                var listofEmails = manufacturingComments[0].PriceKeyInfo.Select(x => Convert.ToString(x.ComponentName)).Distinct().ToList();
                var listOfUserDetails = await Utilities.GetUserDetailsAsync(listofEmails, _configuration).ConfigureAwait(false);
                if (listOfUserDetails != null)
                {
                    foreach (var user in manufacturingComments[0].PriceKeyInfo)
                    {
                        foreach (var userInfo in listOfUserDetails)
                        {
                            if (Utility.CheckEquals(Convert.ToString(user.ComponentName), Convert.ToString(userInfo.Email)))
                            {
                                user.ComponentName = Convert.ToString(userInfo.FirstName) + Constants.EMPTYSPACE + Convert.ToString(userInfo.LastName);
                            }
                        }
                    }
                }
            }
            mainDocumentResponse.PriceSections.AddRange(result.FloorMatrixTable);
            if (mainDocumentResponse.Variables.ContainsKey(documentGenerationMapper[Constant.FIXTURESTRATEGY]) && 
                constantValues.ContainsKey(mainDocumentResponse.Variables[documentGenerationMapper[Constant.FIXTURESTRATEGY]].ToString()))
            {
                var tablesTobeRemoved = constantValues[mainDocumentResponse.Variables[documentGenerationMapper[Constant.FIXTURESTRATEGY]].
                    ToString()].ToObject<List<string>>();
                foreach (var tableName in tablesTobeRemoved)
                {
                    mainDocumentResponse.PriceSections.RemoveAll(x => Utility.CheckEquals(x.Name, tableName));
                }
            } 
            mainDocumentResponse.PriceSections.Add(buildingDataForDocGen);
            mainDocumentResponse.PriceSections.Add(GenerateTableForCOPandLockedCompartments(mainDocumentResponse, unitId,result));



            if (mainDocumentResponse.Variables.ContainsKey(documentGenerationMapper[Constants.CONTROLLERLOCATION]) && mainDocumentResponse.Variables[documentGenerationMapper[Constants.CONTROLLERLOCATION]].ToString().Contains(Constant.JAMB))
            {
                mainDocumentResponse.Variables[documentGenerationMapper[Constants.CONTROLLERLOCATION]] = documentGenerationMapper[Constants.JAMB];
                if (mainDocumentResponse.Variables.ContainsKey(documentGenerationMapper[Constant.CONTROLLERFLOOR]))
                {
                    mainDocumentResponse.Variables[documentGenerationMapper[Constant.CONTROLLERFLOOR]] = result.TopFloor;
                }
                else
                {
                    mainDocumentResponse.Variables.Add(documentGenerationMapper[Constant.CONTROLLERFLOOR], result.TopFloor);
                }
            }
            DocumentrequestBody documentGenerationRequestBody = new DocumentrequestBody()
            {
                DocumentModel = mainDocumentResponse
            };
            if (documentGenerationRequestBody.DocumentModel?.Variables != null)
            {
                var buildingEnrichments = JObject.Parse(File.ReadAllText(string.Format(Constant.BUILDINGENRICHEDDATA))).ToObject<Dictionary<string, object>>();
                var groupEnrichments = JObject.Parse(File.ReadAllText(string.Format(Constant.ELEVATORENRICHMENTTEMPLATE))).ToObject<Dictionary<string, object>>();
                var unitEnrichments = documentGenerationRequestBody.DocumentModel.Variables.ContainsKey("ProductName")? documentGenerationRequestBody.DocumentModel.Variables["ProductName"] switch
                {
                    Constants.EVO_200 => JObject.Parse(File.ReadAllText(string.Format(Constants.UNITENRICHMENTSTEMPLATE, Constants.EVOLUTION200))),
                    Constants.EVO_100 => JObject.Parse(File.ReadAllText(string.Format(Constants.UNITENRICHMENTSTEMPLATE, Constants.EVOLUTION100))),
                    Constants.ENDURA_100 => JObject.Parse(File.ReadAllText(string.Format(Constants.UNITENRICHMENTSTEMPLATE, Constants.END100))),
                    Constants.ESCLATORMOVINGWALK => JObject.Parse(File.ReadAllText(string.Format(Constants.NCPENRICHEDDATA, Constants.ESCALATORTYPE))),
                    Constants.TWINELEVATOR => JObject.Parse(File.ReadAllText(string.Format(Constants.NCPENRICHEDDATA, Constants.TWINELEVATOR))),
                    Constants.OTHER => JObject.Parse(File.ReadAllText(string.Format(Constants.NCPENRICHEDDATA, Constants.OTHER))),
                    _ => JObject.Parse(File.ReadAllText(string.Format(Constants.CUSTOMENGINEEREDENRICHEDDATA, Convert.ToString(documentGenerationRequestBody.DocumentModel.Variables["ProductName"]).Replace("CE_", "")))),
                }:
                JObject.Parse(File.ReadAllText(string.Format(Constants.UNITENRICHMENTSTEMPLATE, Constants.EVOLUTION100)))
                ;
                var unitEnrichment = unitEnrichments.ToObject<Dictionary<string, object>>();
                var buildingVariables = Utility.DeserializeObjectValue<Dictionary<string, object>>(Utility.SerializeObjectValue(buildingEnrichments["variables"]));
                var groupVariables = Utility.DeserializeObjectValue<Dictionary<string, object>>(Utility.SerializeObjectValue(groupEnrichments["variables"]));
                var unitVariables = Utility.DeserializeObjectValue<Dictionary<string, object>>(Utility.SerializeObjectValue(unitEnrichments["variables"]));
                var buildingValues = Utility.DeserializeObjectValue<Dictionary<string, object>>(Utility.SerializeObjectValue(buildingEnrichments["options"]));
                var groupValues = Utility.DeserializeObjectValue<Dictionary<string, object>>(Utility.SerializeObjectValue(groupEnrichments["options"]));
                var unitEnrichmentValues = Utility.DeserializeObjectValue<Dictionary<string, object>>(Utility.SerializeObjectValue(unitEnrichments["options"]));
                foreach (var configVariable in documentGenerationRequestBody.DocumentModel.Variables.ToList())
                {
                    if (buildingVariables != null && buildingVariables.ContainsKey(configVariable.Key))
                    {
                        if (buildingValues != null && buildingValues.ContainsKey(Convert.ToString(configVariable.Value)))
                        {
                            var variablePropeties = Utility.DeserializeObjectValue<Dictionary<string, Object>>(Utility.SerializeObjectValue(buildingValues[Convert.ToString(configVariable.Value)]));
                            var properties = Utility.DeserializeObjectValue<List<Properties>>(Utility.SerializeObjectValue(variablePropeties["properties"]));
                            if (properties != null)
                            {
                                foreach (var property in properties)
                                {
                                    if (property!=null && !string.IsNullOrEmpty(property.Id) && Utilities.CheckEquals("displayname", property.Id))
                                    {
                                        documentGenerationRequestBody.DocumentModel.Variables[configVariable.Key] = (object)property.Value;
                                    }
                                }
                            }
                        }
                    }
                    if (groupVariables != null && groupVariables.ContainsKey(configVariable.Key))
                    {
                        if (groupValues != null && groupValues.ContainsKey(Convert.ToString(configVariable.Value)))
                        {
                            var variablePropeties = Utility.DeserializeObjectValue<Dictionary<string, Object>>(Utility.SerializeObjectValue(groupValues[Convert.ToString(configVariable.Value)]));
                            var properties = Utility.DeserializeObjectValue<List<Properties>>(Utility.SerializeObjectValue(variablePropeties["properties"]));
                            if (properties != null)
                            {
                                foreach (var property in properties)
                                {
                                    if (property != null && !string.IsNullOrEmpty(property.Id) && Utilities.CheckEquals("displayname", property.Id))
                                    {
                                        documentGenerationRequestBody.DocumentModel.Variables[configVariable.Key] = (object)property.Value;
                                    }
                                }
                            }
                        }
                    }
                    if (unitVariables != null && unitVariables.ContainsKey(configVariable.Key))
                    {
                        if (unitEnrichmentValues != null && unitEnrichmentValues.ContainsKey(Convert.ToString(configVariable.Value)))
                        {
                            var variablePropeties = Utility.DeserializeObjectValue<Dictionary<string, Object>>(Utility.SerializeObjectValue(unitEnrichmentValues[Convert.ToString(configVariable.Value)]));
                            var properties = Utility.DeserializeObjectValue<List<Properties>>(Utility.SerializeObjectValue(variablePropeties["properties"]));
                            if (properties!=null)
                            {
                                foreach (var property in properties)
                                {
                                    if (property != null && !string.IsNullOrEmpty(property.Id) && Utilities.CheckEquals("displayname", property.Id))
                                    {
                                        documentGenerationRequestBody.DocumentModel.Variables[configVariable.Key] = (object)property.Value;
                                    }
                                }
                            }
                        }

                    }
                }
            }
        var documentGenerationResponse = await _document.SendRequestToDocumentGenerator(JObject.FromObject(documentGenerationRequestBody)).ConfigureAwait(false);
            var cachedUserDetail = JObject.Parse(_cpqCacheManager.GetCache(sessionId, _environment, Constant.USERDETAILSCPQ)).ToObject<User>();
            if (cachedUserDetail.IsViewUser)
            {
                var vaultUploadInfo = GetUploadInfo(result, unitDetails);
                await _vault.UploadDocument(documentGenerationResponse.Content.ReadAsByteArrayAsync().Result, vaultUploadInfo);
            }
            Utility.LogEnd(methodBegin);
            return documentGenerationResponse;


        }

        public async Task<string> GetVaultLocation(string projectId)
        {
            var beginTime = Utility.LogBegin();
            var location = await _vault.GetFolderPath(projectId).ConfigureAwait(false);
            Utility.LogEnd(beginTime);
            return location;
        }

        public Dictionary<string, object> ConvertTOFeetInch(Dictionary<string, object> variables, string variableId)
        {
            if (variables.ContainsKey(variableId))
            {
                var value = variables[variableId];
                variables[variableId]= Convert.ToString(Convert.ToInt32(Convert.ToDecimal(value) / 12))+ "'-"+ Convert.ToDecimal(Convert.ToDouble(value) % 12)+"''";
            }
            return variables;
        }

        private VaultUploadModel GetUploadInfo(TP2Summary tp2Summary, UnitDetailsForTP2 unitDetails )
        {
            var viewDataForUpload = _cpqCacheManager.GetCache(tp2Summary.ProjectData.OpportunityId + tp2Summary.ProjectData.VersionId, _environment, Constant.USERADDRESS);
            var viewDetails = Utility.DeserializeObjectValue<ViewProjectDetails>(viewDataForUpload);
            return new VaultUploadModel()
            {
                Payload = new VaultFileUploadInfo[1]
             {
                    new VaultFileUploadInfo()
                    {
                        ProjectViewID = tp2Summary?.projectInfo?.ProjectId,
                        QuoteID= $"{tp2Summary?.projectInfo?.ProjectId}.{viewDetails.Data.Quotation.Quote.UIVersionId}",
                        LayoutTypes = null,
                        MFClassName= "order form",
                        FileName =unitDetails?.Ueid,
                        FileExtension="pdf",
                        HasErrors=string.Empty,
                        ErrorMessage=string.Empty,
                        Estimates = new Estimate[1]
                        {
                            new Estimate(){
                            ExternalEstimateId =unitDetails?.Ueid
                            }
                        }
                    }
             }
            };
        }

        private void CreateNewPriceKeyBasedOnUserVariables(PriceValuesDetails newPriceKeyInfo, UnitSummaryUIModel unitsTp2Response, int unitId,decimal totalPrice)
        {
            var sectionNumber = newPriceKeyInfo.Section;
            var priceKeysOfSection = new List<String>();
            decimal subtotalOfSection = 0;
            var totalValueAddedByUser = (from userVariable in unitsTp2Response.Variables
                                         where newPriceKeyInfo.ItemNumber.Contains(userVariable.Id)
                                         select userVariable).FirstOrDefault();
            foreach (var priceDetails in unitsTp2Response.PriceSections.Where(x => x.Section.Equals(sectionNumber)).FirstOrDefault().PriceKeyInfo)
            {
                priceKeysOfSection.Add(priceDetails.ItemNumber);
            }
            foreach (var pricekey in unitsTp2Response.PriceValue)
            {
                if (priceKeysOfSection.Contains(pricekey.Key.ToUpper()))
                {
                    subtotalOfSection += pricekey.Value.totalPrice;
                }
            }
            var newPricePerSection = decimal.Negate((subtotalOfSection / totalPrice) * Convert.ToDecimal(totalValueAddedByUser.Value));
            unitsTp2Response.PriceValue.Add(newPriceKeyInfo.ItemNumber, new UnitPriceValues { Unit = Constant.CURRENCYCODE, quantity = 1, totalPrice = newPricePerSection });
        }

        private List<PriceSectionDetails> FilterPriceSections(List<PriceSectionDetails> priceSections, int unitId, List<UnitDetailsForTP2> unitDetails)
        {
            var includeGroupPriceKey = IsGroupValueApplicable(unitId, unitDetails);
            if (!includeGroupPriceKey)
            {
                return priceSections.Select(section =>
                {
                    section.PriceKeyInfo = section.PriceKeyInfo.Where(info => !IsGroupMaterial(info)).ToList();
                    return section;
                }).ToList();
            }
            return priceSections;
        }

        private bool IsGroupValueApplicable(int unitId, List<UnitDetailsForTP2> unitDetails)
        {
            var sortedUnit = (from unitDetail in unitDetails
                              where !String.IsNullOrEmpty(unitDetail.Ueid)
                              orderby unitDetail.Ueid
                              select unitDetail).First();
            if(sortedUnit.UnitId.Equals(unitId))
            {
                return true;
            }
            return false;
        }

        private bool IsGroupMaterial(PriceValuesDetails priveValueDetails)
        {
          
            return _staticPriceKeys.Any(
                x => x.ItemNumber.Replace("-","_") == priveValueDetails.ItemNumber && 
                x.Section == priveValueDetails.Section && 
                Convert.ToBoolean(x.GroupMaterial));
        }
        private IEnumerable<PriceValuesDetails> ReadPriceValues()
        {
            var unitMappingObjValues = JObject.Parse(File.ReadAllText(Constant.PRICEDETAILS));
            var priceValues = Utility.DeserializeObjectValue<List<PriceValuesDetails>>(Utility.SerializeObjectValue(unitMappingObjValues[Constant.PRICEKEYDETAILS]));
            return priceValues.Select(x =>
           {
               x.ItemNumber = x.ItemNumber.Replace("-", "_");
               return x;
           });
        }

        public PriceSectionDetails GenerateTableForCOPandLockedCompartments(Tp2DocumentGeneration mainDocumentResponse,int unitId, TP2Summary setSummary)
        {
            var copandLockedCompartmentsTable = new PriceSectionDetails()
            {
                Id = Constants.COPANDLOCKEDCOMPARTMENTSTABLENAME,
                Name = Constants.COPANDLOCKEDCOMPARTMENTSTABLENAME,
                Section = Convert.ToString(1),
                PriceKeyInfo = new List<PriceValuesDetails>()
            };
            var productDetails = setSummary.UnitDetails.Where(x => x.UnitId == unitId).ToList()[0].ProductName;
            var enrichedData = JObject.Parse(File.ReadAllText(string.Format(Constant.UNITENRICHMENTSTEMPLATE, Constant.EVOLUTION200)));
            if (Utility.CheckEquals(productDetails,Constant.ENDURA_100))
            {
                enrichedData = JObject.Parse(File.ReadAllText(Constant.UNITENRICHEDDATAENDURA100));
            }
            //list of variable for name
            var sectionList = Utility.DeserializeObjectValue<Dictionary<string,object>>(Utility.SerializeObjectValue(enrichedData[Constants.VARIABLEVALUES]));
            // for value
            var constantValues = JObject.Parse(File.ReadAllText(Constant.DOCUMENTGENERATIONMAPPERPATH));
            var copValue = constantValues[Constants.COPVALUES].ToObject<List<string>>();
            //value to table mapping
            var tableName = Utility.GetVariableMapping(Constant.DOCUMENTGENERATIONMAPPERPATH, Constants.VALUETOCOMPARTMENT).ToList();
            foreach (var variable in mainDocumentResponse.Variables.ToList())
            {
                if(copValue.Where(x=>Utility.CheckEquals(x,Convert.ToString(variable.Value))).Any())
                {
                    var variableDetails = sectionList.ToList().Where(x=>Utility.CheckEquals(x.Key,variable.Key)).ToList();
                    if(variableDetails.Any())
                    {
                        var displayName= Utility.DeserializeObjectValue<List<Properties>>(Utility.SerializeObjectValue(JObject.Parse(Utility.SerializeObjectValue(variableDetails[0].Value))[Constant.PROPERTIES]));
                        var name = displayName.Where(x => Utility.CheckEquals( x.Id,Constants.DISPLAYNAME)).ToList();
                        var rowData = new PriceValuesDetails()
                        {
                            Section = name.Any()? name[0].Value.ToString() : variable.Key,
                            ItemNumber =Utility.CheckEquals(variable.Value.ToString().ToUpper(), Constants.FACEPLT) ?Constants.YES : Constants.NO,
                            PartDescription = Utility.CheckEquals(variable.Value.ToString().ToUpper(), Constants.SVCCBT) ? Constants.YES : Constants.NO,
                            ComponentName = Utility.CheckEquals(variable.Value.ToString().ToUpper(), Constants.FIRECBT) ? Constants.YES : Constants.NO,
                        };
                        copandLockedCompartmentsTable.PriceKeyInfo.Add(rowData);
                    }
                }
            }
            
            return copandLockedCompartmentsTable;
        }
    }
}

