/************************************************************************************************************
************************************************************************************************************
   File Name     :   ProductSelectionBL.cs 
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
using System.IO;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class ProductSelectionBL : IProductSelection
    {
        /// <summary>
        /// Variable Collection
        /// </summary>
        #region Variables
        private readonly IProductSelectionDL _productSelectiondl;
        private readonly IConfigure _configure;
        private readonly ICacheManager _cpqCacheManager;
        private readonly string _environment;
        #endregion
        ///<summary>
        ///constructor for product selection
        ///</summary>
        public ProductSelectionBL(ILogger<ProductSelectionBL> logger , IProductSelectionDL productSelectionDL, IConfigure configure, ICacheManager cpqCacheManager)
        {
            _productSelectiondl = productSelectionDL;
            _configure = configure;
            _cpqCacheManager = cpqCacheManager;
            _environment = Constant.DEV;
            Utility.SetLogger(logger);
        }
        /// <summary>
        /// for saving the product and creating the unitset
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productSelection"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveProductSelection(int groupConfigurationId, ProductSelection productSelection, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            productSelection.UserName = _configure.GetUserId(sessionId);
            var opportunityData = _configure.GetUserAddress(sessionId);
            var variableId = Convert.ToString(JObject.Parse(File.ReadAllText(Constants.PRODUCTSELECTIONCONSTANTTEMPLATEPATH))[Constants.PRODUCTSELECTED]);
            var supplyingFactory = Convert.ToString(JObject.Parse(File.ReadAllText(Constants.PRODUCTSELECTIONCONSTANTTEMPLATEPATH))[Constants.SUPPLYINGFACTORY]);
            var VariableAssignment = new List<VariableAssignment>();
            VariableAssignment.Add( new VariableAssignment()
            {
                VariableId = variableId,
                Value = productSelection.productSelected
            });
            var lineObject = new Line()
            {
                VariableAssignments = new List<VariableAssignment>()
            };
            var configureRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineObject), Constants.PRODUCTSELECTION);
            configureRequest.Line.VariableAssignments= VariableAssignment;
            configureRequest.Settings.Debug = true;
            var configResponse =await _configure.GetByDefaultOrRulevaluesFromPackage(configureRequest, sessionId).ConfigureAwait(false);
            Dictionary<string, object> configVariables = JsonConvert.DeserializeObject<Dictionary<string, object>>(configResponse[Constant.CONFIGURATION].ToString());
            supplyingFactory = configVariables.ContainsKey(supplyingFactory) ? Convert.ToString(configVariables[supplyingFactory]) : Constant.US;
            string businessLine;
            string country;
            if (opportunityData == null)
            {
                businessLine = Constant.NI;
                country = Constant.US;
            }
            else
            {
                var OpportunityData = (JsonConvert.DeserializeObject<OpportunityEntity>(opportunityData));
                businessLine = OpportunityData.LineOfBusiness;
                country = OpportunityData.AccountAddress.AccountAddressCountry;
            } var controlLanding = _cpqCacheManager.GetCache(productSelection.UserName, string.Join("_", productSelection.UnitId), "CONTROLLDG");
            if (Utility.CheckEquals(businessLine, Constant.NEWINSTALLATION))
            {
                businessLine = Constant.NI;
            }
            else
            {
                businessLine = Constant.MD;
            }
            if (Utility.CheckEquals(country, Constant.CANADA))
            {
                country = Constant.CA;
            }
            else
            {
                country = Constant.US;
            }
            var cachedConfigurations = _cpqCacheManager.GetCache(sessionId, string.Join("_", productSelection.UnitId), "PRODUCTVARIABLES");
            var fixtureStrategy = string.Empty;
            if(!String.IsNullOrEmpty(cachedConfigurations))
            {
                var cachedData = JObject.Parse(cachedConfigurations)[Constant.CONFIGURATION].ToObject<Dictionary<string, string>>();
                fixtureStrategy = (from varAssign in cachedData
                                       where varAssign.Key.Contains("Parameters_SP.fixtureStrategy_SP")
                                       select varAssign.Value).FirstOrDefault();
            }
            int result = _productSelectiondl.SaveProductSelection(groupConfigurationId, productSelection, businessLine, country, controlLanding, fixtureStrategy, supplyingFactory);

            var responseValues = new SaveProductSelectionResponse()
            {
                SetId = result,
                Message = Constant.SAVEPRODUCTSELECTIONMESSAGE
            };
            if (string.IsNullOrEmpty(productSelection.productSelected))
            {
                responseValues.Message = Constant.EDITINDEPENDENTMESSAGE;
            }

            var SaveProductSelectionResponseArray = new JArray();
            SaveProductSelectionResponseArray.Add(Utility.DeserializeObjectValue<JObject>(Utility.SerializeObjectValue(responseValues)));
            Utility.LogEnd(methodBegin);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = SaveProductSelectionResponseArray };
        }

        /// <summary>
        /// To check the selected units can be configured as a set
        /// </summary>
        /// <param Name="UnitId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> UnitSetValidation(List<int> unitId)
        {
            var methodBegin = Utility.LogBegin();
            var validationResult = _productSelectiondl.UnitSetValidation(unitId);
            var response = new ResponseMessage();
            switch (validationResult)
            {
                case 1:
                    Utility.LogEnd(methodBegin);
                    response.StatusCode = Constant.SUCCESS;
                    return response;

                case 2:
                    Utility.LogEnd(methodBegin);
                    throw new ExternalCallException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message=Constant.UNITERRORMESSAGE,
                        Description = Constant.UNITSETERRORMESSAGEOPENING1
                    });

                case 3:
                    Utility.LogEnd(methodBegin);
                    throw new ExternalCallException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = Constant.UNITSETERRORMESSAGE,
                        Description = Constant.UNITSETERRORMESSAGE1
                    });

                case 4:
                    Utility.LogEnd(methodBegin);
                    throw new ExternalCallException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = Constant.UNITSETERRORMESSAGE,
                        Description = Constant.UNITSETERRORMESSAGE1
                    });

                case 5:
                    Utility.LogEnd(methodBegin);
                    throw new ExternalCallException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = Constant.UNITSETERRORMESSAGE,
                        Description = Constant.UNITSETERRORMESSAGEFORDOORS
                    });

                case 6:
                    Utility.LogEnd(methodBegin);
                    throw new ExternalCallException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = Constant.UNITERRORMESSAGE,
                        Description = Constant.UNITSETERRORMESSAGEOPENING2
                    });

                case 7:
                    Utility.LogEnd(methodBegin);
                    throw new ExternalCallException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = Constant.UNITSETERRORMESSAGE,
                        Description = Constant.UNITSETERRORFORCONFLICTINGOPENINGS
                    });

                default:
                    response.StatusCode = Constant.BADREQUEST;
                    response.Message = Constant.SOMETHINGWENTWRONG;
                    Utility.LogEnd(methodBegin);
                    throw new CustomException(response);


            }

        }
        /// <summary>
        /// For getting the variable valuse for prdouct selection flag
        /// </summary>
        /// <param Name="UnitId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetUnitVariableAssignments(List<int> unitId,string sessionId,string identifier)
        {
            var methodBegin = Utility.LogBegin();
            var result = _productSelectiondl.GetUnitVariableAssignments(unitId, identifier);
            if (result == null)
            {
                Utility.LogEnd(methodBegin);
                throw new ExternalCallException(new ResponseMessage
                {
                    StatusCode = Constant.INTERNALSERVERERROR,
                    Message = Constant.UNITSETERRORMESSAGE1,
                    Description = Constant.UNITSETERRORMESSAGE1
                });
            }
            var lineObj = new Line();
            var configRequest = _configure.CreateConfigurationRequestWithTemplate(JObject.FromObject(lineObj), Constant.BUILDINGNAME);
            configRequest.Line.VariableAssignments = Utility.DeserializeObjectValue<List<VariableAssignment>>(Utility.SerializeObjectValue(result));
            var buildingVariables =await _configure.GetByDefaultOrRulevaluesFromPackage(configRequest, sessionId).ConfigureAwait(false);
            
            var crossPackageVariable = Utility.GetVariableMapping(Constant.VARIABLEDICTIONARY, Constant.GROUPTOBUILDINGCROSSPACKAGEVARIABLE);
            Dictionary<string, object> configVariables = JsonConvert.DeserializeObject<Dictionary<string, object>>(buildingVariables[Constant.CONFIGURATION].ToString());
            foreach (var groupVariable in crossPackageVariable.ToList())
            {
                foreach (var buildingVariable in configVariables.ToList())
                {
                    if(Utility.CheckEquals(groupVariable.Value,buildingVariable.Key))
                    {
                        result.Add(new ConfigVariable()
                        {
                            VariableId = groupVariable.Key,
                            Value = buildingVariable.Value
                        });
                    }
                }
            }
            var elevatorLevelVariables = new List<ConfigVariable>();
            var groupVariables = result.Where(x => !x.VariableId.Contains(Constants.ELEVATORSVALUE, StringComparison.OrdinalIgnoreCase)).ToList();
            if (groupVariables != null && groupVariables.Any())
            {
                foreach (var variable in groupVariables)
                {
                    result.Add(new ConfigVariable()
                    {
                        VariableId = Constants.ELEVATOR001 + variable.VariableId,
                        Value = variable.Value
                    });
                }
            }
            var variableAssignments = JObject.FromObject(new { variableAssignments = result });
            Utility.LogEnd(methodBegin);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = JObject.FromObject(variableAssignments) };
        }
    }
}
