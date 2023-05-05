using System;
using System.Collections.Generic;
using System.Text;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using Configit.TKE.OrderBom.WebApi;
using Configit.TKE.DesignAutomation.Models;
using Configit.TKE.DesignAutomation.Services;
using Configit.TKE.DesignAutomation.WebApi;
using Configit.TKE.OrderBom.CLMPlatform;
using Configit.TKE.OrderBom.Models;
using Configit.TKE.OrderBom.Services;
using Microsoft.Extensions.Configuration;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.BFF.BusinessProcess.Helpers;
using System.Threading.Tasks;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common;
using Configit.TKE.OrderBom.WebApi.Models;
using Configit.TKE.DesignAutomation.Services.Models;
using Newtonsoft.Json.Linq;
using Constants = TKE.SC.Common.Constants;
using TKE.SC.BFF.DataAccess.Interfaces;
using Configit.TKE.DesignAutomation.WebApi.Models;
using System.Linq;
using System.Collections;
using Configit.Configurator.Server.Common;
using TKE.SC.Common.Model.ExceptionModel;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Hangfire;
using Configit.Grid;

namespace TKE.SC.BFF.BusinessProcess.Services
{
    public class DesignAutomationBL: IDesignAutomation
    {
        #region Variables
        private readonly ILogger _logger;
        private readonly IDesignAutomationDL _dadl;
        private readonly IObom _obombl;

        private IConfiguration _configuration;
        /// <summary>
        /// object ICacheManager
        /// </summary>
        private readonly ICacheManager _cpqCacheManager;
        /// <summary>
        /// string
        /// </summary>
        private readonly string _environment;
        #endregion

        public DesignAutomationBL(IDesignAutomationDL dadl,IObom obomBL, IConfiguration iConfig, ICacheManager cpqCacheManager, ILogger<DesignAutomationBL> logger)
        {
            _dadl = dadl;
            _obombl = obomBL;
            _configuration = iConfig;
            _cpqCacheManager = cpqCacheManager;
            _environment = Constant.DEV;
            Utilities.SetLogger(logger);
            _logger = logger;
        }

        /// <summary>
        /// Method to get DA Response
        /// </summary>
        /// <param name="configurationDetails"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetDAResponse(ConfigurationDetails configurationDetails, string sessionId, JObject variableAssignments=null)
        {
            var methodBeginTime = Utility.LogBegin();
            var jobId = string.Empty;
            if (variableAssignments != null)
            {
                var groupVariableListData = Utility.DeserializeObjectValue<Line>(variableAssignments.ToString()).VariableAssignments;
                List<string> drawingTypes = groupVariableListData.Where(oh => oh.VariableId.Contains(Constant.DRAWINGTYPES) &&  oh.Value.Equals(true)).Select(
                    variableAssignments=>variableAssignments.VariableId
                    ).ToList<string>();
                var outputTypes = groupVariableListData.Where(oh => oh.VariableId.Contains(Constant.OUTPUTTYPES) && oh.Value.Equals(true)).Select(
                    variableAssignments => variableAssignments.VariableId
                    ).ToList<string>();
                if (drawingTypes.Any(x=>x.Contains(Constants.ARCHITECTURALPACKAGE)))
                {
                    jobId = BackgroundJob.Enqueue(() => GetDAResponseAsync(configurationDetails, sessionId, drawingTypes, outputTypes));
                    _cpqCacheManager.SetCache(sessionId, _environment, Convert.ToString(configurationDetails.GroupId), jobId);
                    var result = _dadl.SaveUpdateHangFireJobDetailsForDA(configurationDetails.GroupId, configurationDetails.QuoteId, Convert.ToString(DaStatus.DA_INI_ST), jobId);
                }
            }
            else
            {
                var drawingTypes = new List<string> { Constants.EXTERIORPACKAGE, Constants.INTERIORPACKAGE };
                var outPutTypes = new List<string> { Constants.DWGOUTPUTTYPE, Constants.PDFOUTPUTTYPE };
                jobId = BackgroundJob.Enqueue(() => GetDAResponseAsync(configurationDetails, sessionId,drawingTypes,outPutTypes));
                _cpqCacheManager.SetCache(sessionId, _environment, Convert.ToString(configurationDetails.GroupId), jobId);
                var result = _dadl.SaveUpdateHangFireJobDetailsForDA(configurationDetails.GroupId, configurationDetails.QuoteId, Convert.ToString(DaStatus.DA_INI_ST), jobId);
            }

            Utility.LogEnd(methodBeginTime);
            return new ResponseMessage
            {
                
                Response=JObject.FromObject(new DAResponseModel { DaJobStatus= Convert.ToString(DaStatus.DA_INI_ST), JobId=jobId,Message=Constants.DASTATREDMESSAGE, StatusCode=200 })
            };

        }

        /// <summary>
        /// Method for Getting DA Status
        /// </summary>
        /// <param name="configurationDetails"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetDAStatus(ConfigurationDetails configurationDetails, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var hangFireJobId = string.Empty;
            var daStatus = string.Empty;
            var jobList =_dadl.GetJobIdForDA(configurationDetails.GroupId, sessionId, ref hangFireJobId,out daStatus);
            
            var jobStatus = new List<AutomationTaskDetailsReference>();
            var response = new ResponseMessage();
            foreach (var jobs in jobList)
            {
                var individualJobStatus= await _dadl.GetJobStatus(jobs?.JobId);
                var daJobDetailsList = new List<DaJobDetails> { new DaJobDetails { DaJobId = individualJobStatus.Id, DaJobStatus = Convert.ToString(individualJobStatus.Status) } };
                _dadl.SaveUpdateJobDetailsForDA(configurationDetails.GroupId, daJobDetailsList, hangFireJobId);
                var ueid=Convert.ToString(_cpqCacheManager.GetCache(sessionId, _environment, jobs?.JobId));
                if (individualJobStatus != null)
                {
                    individualJobStatus.StatusMessage = Convert.ToString(individualJobStatus.Status);
                    individualJobStatus.PackageName = jobs.PackageName;
                    if(!string.IsNullOrEmpty(ueid))
                        individualJobStatus.UEID = ueid;
                    jobStatus.Add(individualJobStatus);
                }
                else
                {
                    throw new ExternalCallException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Response = null,
                        Message = Constants.SOMETHINGWENTWRONGMSG,
                    });
                }
            }
            
            if (jobStatus.Count != 0)
            {
                var listStatus = jobStatus.Select(x => Convert.ToString(x.Status)).ToList();
                
                if (listStatus.Any(o => o.Equals(Convert.ToString(JobStatus.Failed))) || listStatus.Any(q => q.Equals(Convert.ToString(JobStatus.Failing))))
                {
                    daStatus = Convert.ToString(DaStatus.DA_FAIL);
                }
                else if(listStatus.Any(o => o.Equals(Convert.ToString(JobStatus.Aborted))) || listStatus.Any(q => q.Equals(Convert.ToString(JobStatus.Aborting))))
                {
                    daStatus = Convert.ToString(DaStatus.DA_ABT);
                }
                else if (listStatus.Any(o => o.Equals(Convert.ToString(JobStatus.Pending))))
                {
                    daStatus = Convert.ToString(DaStatus.DA_PEN);
                }
                else if (listStatus.Any(o => o.Equals(Convert.ToString(JobStatus.InProgress))) || listStatus.Any(q => q.Equals(Convert.ToString(JobStatus.Created))))
                {
                    daStatus = Convert.ToString(DaStatus.DA_INPRO);
                }
                else
                {
                    daStatus = Convert.ToString(DaStatus.DA_SCS);
                }
            }
            var result = _dadl.SaveUpdateHangFireJobDetailsForDA(configurationDetails.GroupId, configurationDetails.QuoteId, daStatus, hangFireJobId);

            response.Response = JObject.FromObject(new DaStatusResponseModel { DaJobStatus = daStatus, StatusCode = Constants.SUCCESS, IndividualJobStatus = jobStatus });
            response.StatusCode = Constants.SUCCESS;
            Utility.LogEnd(methodBeginTime);
            return response;
        }

        /// <summary>
        /// Method to Get Quantity variables
        /// </summary>
        /// <param name="obomVariables"></param>
        /// <returns></returns>
        private ArrayList GetQuanityVariables(List<ObomVariables> obomVariables)
        {
            var qtyArrayList = new ArrayList();
            foreach (var bomItem in obomVariables)
            {
                if (bomItem != null)
                {
                    if ((bomItem.XMLVariables!=null)&&(bomItem.XMLVariables.Where(x => (x.Key.Equals(Constants.BOMGROUP) && x.Value.Equals(true))).Select(x => x).ToList().Any()))
                    {
                        var qtyVariable = bomItem.XMLVariables.Where(x => (x.Key.Equals(Constants.FULLYQUALIFIEDNAME))).Select(x => x.Value).FirstOrDefault().ToString() + Constants.QTY;
                        if (!string.IsNullOrEmpty(qtyVariable))
                        {
                            qtyArrayList.Add(new OrderAssignment { Name = qtyVariable, Value = Constants.ONE });
                        }

                    }
                    if (bomItem.Child != null && bomItem.Child.Any())
                    {
                        qtyArrayList.AddRange(GetQuanityVariables(bomItem.Child));
                    }
                }
                
            }
            
            return qtyArrayList;
        }

        /// <summary>
        /// Method to Update OrderItems
        /// </summary>
        /// <param name="orderItems"></param>
        /// <param name="packagePath"></param>
        /// <param name="exportTypes"></param>
        /// <param name="guid"></param>
        /// <param name="outputLocation"></param>
        /// <param name="configurationDetails"></param>
        /// <returns></returns>
        private OrderItem[] UpdateOrderItems(List<OrderItem> orderItems,string packagePath,List<string> exportTypes,string guid,ref string outputLocation,ConfigurationDetails configurationDetails)
        {
            var daSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constants.DESIGNAUTOMATION);
            outputLocation = Utility.GetPropertyValue(daSettings, Constants.DAOUTPUTPATH) + Constants.BACKSLASH + configurationDetails.QuoteId + Constants.HYPHEN + configurationDetails.GroupId + Constants.HYPHEN +configurationDetails.UEID + Constants.HYPHEN + guid;
            OrderItem[] orderItemArray=new OrderItem[orderItems.Count];
            var itemNum = 0;
            foreach (var orderItem in orderItems)
            {
                if (orderItem.AssemblyFile != string.Empty)
                {
                    
                    orderItem.AssemblyFile.Replace(Constants.DOUBLE_QUOTES, Constants.EMPTY);

                    if (!orderItem.AssemblyFile.Contains(Constants.BACKSLASH))
                    {
                        if (orderItem.AssemblyFile.Contains(Constant.DOT))
                        {
                            orderItem.AssemblyFile = orderItem.AssemblyFile.Split(Constant.DOT).FirstOrDefault() + Constants.BACKSLASH + orderItem.AssemblyFile;
                        }
                        else
                        {
                            orderItem.AssemblyFile = orderItem.AssemblyFile + Constants.BACKSLASH + orderItem.AssemblyFile + Constant.DOT + Constants.SLDASM;
                        }
                    }
                    orderItem.OutputLocations.Add(outputLocation);
                    orderItem.ExportTypes.AddRange(exportTypes);
                    orderItem.Categories.Add(Constants.ASTERISK);
                    orderItemArray[itemNum] = orderItem;
                    itemNum++;
                    
                }
                if (orderItem.Children != null && orderItem.Children.Any())
                {
                    UpdateOrderItems(orderItem.Children, packagePath,exportTypes,guid,ref outputLocation, configurationDetails);
                }
            }
            return orderItemArray;
        }
        /// <summary>
        /// Background job for getting DA Response
        /// </summary>
        /// <param name="configurationDetails"></param>
        /// <param name="sessionId"></param>
        /// <param name="drawingTypes"></param>
        /// <param name="outputTypes"></param>
        /// <returns></returns>
        public async Task GetDAResponseAsync(ConfigurationDetails configurationDetails,string sessionId,List<string> drawingTypes=null, List<string> outputTypes=null)
        {
            var methodBeginTime = Utility.LogBegin();
            var productSelectionMapperVariables = Utilities.VariableMapper(Constants.OBOMVARIABLEMAPPERPATH, Constants.VARIABLESKEY);
            var requestBody = JObject.Parse(System.IO.File.ReadAllText(Constants.CONFIGITOBOMREQUESTBODYPATH)).ToString();
            var daPackagePath = JObject.Parse(System.IO.File.ReadAllText(Constants.DAPACKAGEPATH));
            var requestBodyObject = Utility.DeserializeObjectValue<CreateBomRequest>(requestBody);
            var variableAssignmentsForOBOM = new ObomVariableAssignment();
            var obomVariableAssignmentscache=_cpqCacheManager.GetCache(sessionId, _environment, Constants.OBOMVARIABLEASSIGNMENTS + Convert.ToString(configurationDetails.GroupId));
            if (obomVariableAssignmentscache != null)
            {
                variableAssignmentsForOBOM = Utilities.DeserializeObjectValue<ObomVariableAssignment>(obomVariableAssignmentscache);
            }
            else
            {
                variableAssignmentsForOBOM = await _obombl.GetBuildingVariableAssignmentsForOBOM(configurationDetails, sessionId).ConfigureAwait(false);
            }
            var orderAssignementsArrayList = new ArrayList();
            var listExportTypes = new List<string>();
            var outputLocation = string.Empty;
            //var packagePaths = Utility.DeserializeObjectValue<Dictionary<string, string>>(daPackagePath.SelectToken(Constants.PACKAGEPATH).ToString());
            var defaultExportTypes = await _dadl.GetDefaultExportTypes();
            //var availableExportTypes = await _dadl.GETAvailableExportTypes();
            //availableExportTypes.Select(x => x.Name).ToList();
            var submitBomResponseList = new List<SubmitBomResponse>();
            
            
            

            
            var timer1 = new Stopwatch();
            timer1.Start();
            foreach (var building in variableAssignmentsForOBOM?.BuildingData)
            {
                foreach (var group in building?.GroupData)
                {
                    foreach (var unit in group?.UnitDataForObom)
                    {
                        var variableAssignments = new List<VariableAssignment>();
                        var setData = group.SetData.Where(x => x.SetId.Equals(unit.SetId)).ToList();
                        if (setData != null && setData.Any()&&(Utility.CheckEquals(setData[0].ProductSelected,Constants.EVO_100)|| Utility.CheckEquals(setData[0].ProductSelected, Constants.EVO_200))) 
                        {
                            
                            foreach (var variables in setData[0]?.SetConfigurationVariables)
                            {
                                variableAssignments.Add(new VariableAssignment() { VariableId = variables.Key, Value = variables.Value });
                            }
                            for (int i = building.NumberOfLanding + 1; i <= 200; i++)
                            {
                                var outOfIndex = Convert.ToString(i);
                                variableAssignments.RemoveAll(unit => unit.VariableId.Contains(outOfIndex));
                            }
                            configurationDetails.UEID = unit.UEID;
                        }
                        else
                        {
                            continue;
                        }

                        var globalVariableAssigmnets = variableAssignments.Where(x => !x.VariableId.Contains(Constants.FLOORMATRIX)).ToList();
                        var variableAssignmentList = new List<VariableAssignment>();
                        var outputTypesStub = Utility.DeserializeObjectValue<Dictionary<string, string>>(daPackagePath.SelectToken(Constants.OUTPUTTYPE).ToString());
                        foreach (var type in outputTypes)
                        {
                            if (outputTypesStub.ContainsKey(type))
                            {
                                listExportTypes.Add(outputTypesStub.Where(x => x.Key.Contains(type)).Select(x => x.Value).FirstOrDefault());
                            }

                        }


                        foreach (var variable in globalVariableAssigmnets)
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
                        globalVariableAssigmnets = variableAssignments.Where(x => x.VariableId.Contains(Constants.FLOORMATRIX, StringComparison.OrdinalIgnoreCase)).ToList();
                        foreach (var variable in globalVariableAssigmnets)
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
                        //System.IO.File.WriteAllText(@"D:\FC\VariableAssignments.json", Utility.SerializeObjectValue(variableAssignmentList));
                        foreach (var variables in variableAssignmentList)
                        {
                            orderAssignementsArrayList.Add(new OrderAssignment { Name = variables.VariableId, Value = Convert.ToString(variables.Value) });
                        }
                        var packagePaths = new Dictionary<string, string>();

                        if (drawingTypes.Contains(Constants.EXTERIORPACKAGE))
                        {
                            var exteriorPackagePaths = daPackagePath.SelectToken(Constants.EXTERIORPACKAGE);
                            var exteriorPaths = new Dictionary<string, string>();
                            if (setData[0].ProductSelected.Equals(Constants.EVO_100))
                            {
                                exteriorPaths = Utilities.DeserializeObjectValue<Dictionary<string, string>>(exteriorPackagePaths.SelectToken(Constants.EVO_100).ToString());
                            }
                            else if (setData[0].ProductSelected.Equals(Constants.EVO_200))
                            {
                                exteriorPaths = Utilities.DeserializeObjectValue<Dictionary<string, string>>(exteriorPackagePaths.SelectToken(Constants.EVO_200).ToString());
                            }
                            foreach (KeyValuePair<string, string> keyValue in exteriorPaths)
                            {
                                packagePaths.Add(keyValue.Key, keyValue.Value);
                            }
                        }
                        if (drawingTypes.Contains(Constants.INTERIORPACKAGE))
                        {
                            var interiorPackagePaths = daPackagePath.SelectToken(Constants.INTERIORPACKAGE);
                            var interiorPaths = new Dictionary<string, string>();
                            if (setData[0].ProductSelected.Equals(Constants.EVO_100))
                            {
                                interiorPaths = Utilities.DeserializeObjectValue<Dictionary<string, string>>(interiorPackagePaths.SelectToken(Constants.EVO_100).ToString());
                            }
                            else if (setData[0].ProductSelected.Equals(Constants.EVO_200))
                            {
                                interiorPaths = Utilities.DeserializeObjectValue<Dictionary<string, string>>(interiorPackagePaths.SelectToken(Constants.EVO_200).ToString());
                            }
                            foreach (KeyValuePair<string, string> keyValue in interiorPaths)
                            {
                                packagePaths.Add(keyValue.Key, keyValue.Value);
                            }
                        }
                        var timer = new Stopwatch();
                        timer.Start();
                        var parallelTaskList = new List<Task<ResponseMessage>>();
                        foreach (var packagePath in packagePaths)
                        {

                            var configRequest = _obombl.CreateConfigurationRequestWithTemplate(packagePath.Key);
                            var configitObomAssignments = new List<ObomVariables>();
                            parallelTaskList.Add(_obombl.OBOMPAckageCall(packagePath.Key, new List<ObomVariables>(), variableAssignmentList, new Line(), sessionId, 0, false));
                        }
                        var daPackageResponseArray = await Task.WhenAll(parallelTaskList).ConfigureAwait(false);
                        timer.Stop();
                        TimeSpan timeTaken = timer.Elapsed;
                        _logger.LogInformation("DA VT Package Time taken: " + timeTaken.ToString(@"m\:ss\.fff"));
                        Guid guid = Guid.NewGuid();
                        foreach (var packagePath in packagePaths)
                        {
                            var packageOrderAssignmentArrayList = orderAssignementsArrayList;
                            var configitObomAssignments = new List<ObomVariables>();
                            foreach (var daResponse in daPackageResponseArray)
                            {
                                if (daResponse.Message.Equals(packagePath.Key))
                                {
                                    configitObomAssignments.AddRange(Utility.DeserializeObjectValue<List<ObomVariables>>(Utility.SerializeObjectValue(daResponse.ResponseArray)));
                                    break;
                                }
                            }
                            var qtyVariables = GetQuanityVariables(configitObomAssignments);
                            packageOrderAssignmentArrayList.AddRange(qtyVariables);

                            requestBodyObject.Configurations[0].ModelFileName = packagePath.Key;
                            requestBodyObject.Configurations[0].ClmPlatformPackagePath = packagePath.Value;
                            var orderAssignmentArray = (OrderAssignment[])packageOrderAssignmentArrayList.ToArray(typeof(OrderAssignment));
                            requestBodyObject.Configurations[0].Assignments = orderAssignmentArray;
                            //System.IO.File.WriteAllText(@"D:\FC\" + packagePath.Key + "_ConfigitRequest.json", Utility.SerializeObjectValue(requestBodyObject));
                            var configitObom = await _dadl.GetOBOMResponseForDA(requestBodyObject,configurationDetails,sessionId, packagePath.Value).ConfigureAwait(false);
                            //System.IO.File.WriteAllText(@"D:\DAResponseNew\" + packagePath.Key+"_ConfigitResponse.json", Utility.SerializeObjectValue(configitObom.Response));
                            var obomResponse = Utility.DeserializeObjectValue<CreateBomResponse>(Utility.SerializeObjectValue(configitObom?.Response));


                            listExportTypes.AddRange(defaultExportTypes);
                            var submitBomRequest = new SubmitBomRequest
                            {
                                Lines = new List<OrderBom>()
                            };
                            var orderItemList = new List<OrderItem>();
                            foreach (var orderItems in obomResponse?.Lines)
                            {
                                foreach (var orderItem in orderItems.Items)
                                {
                                    orderItemList.Add(orderItem);
                                }
                            }
                            var orderItemsArray = UpdateOrderItems(orderItemList, packagePath.Key, listExportTypes, guid.ToString(), ref outputLocation,configurationDetails);

                            if (orderItemsArray != null)
                            {
                                var Lines = new List<OrderBom>();
                                var orderBom = new OrderBom
                                {
                                    Items = orderItemsArray
                                };
                                Lines.Add(orderBom);
                                submitBomRequest.Lines = Lines;

                            }
                            //System.IO.File.WriteAllText(@"D:\DAResponseNew\" + packagePath.Key +unit.UEID+ "_SubmitBomRequest.json", Utility.SerializeObjectValue(submitBomRequest));
                            submitBomResponseList.Add(await _dadl.GetSubmitBOMResponse(submitBomRequest,configurationDetails,sessionId,packagePath.Value).ConfigureAwait(false));

                        }

                        

                    }
                }

            }
            //var allResponse = await Task.WhenAll(submitBomResponseTaskList);

            timer1.Stop();
            TimeSpan timeTaken2 = timer1.Elapsed;
            _logger.LogInformation("submit Bom Response Task Time taken: " + timeTaken2.ToString(@"m\:ss\.fff"));
            var listJobId = new List<string>();
            var JobDetailsList = new List<DaJobDetails>();
            if (submitBomResponseList != null)
            {
                for (int i = 0; i <= submitBomResponseList.Count-1; i++)
                {
                    listJobId= submitBomResponseList[i]?.Items?.Select(y => y?.TaskDetails?.Id).ToList();
                    foreach (var jobId in listJobId)
                    {
                        JobDetailsList.Add(new DaJobDetails { DaJobId = jobId, PackageName = submitBomResponseList[i]?.Items[0]?.MaterialName });
                        _cpqCacheManager.SetCache(sessionId, _environment,jobId,configurationDetails.UEID);
                    }       
                    
                }
                var hangFireJobId=_cpqCacheManager.GetCache(sessionId, _environment, Convert.ToString(configurationDetails.GroupId));
                _dadl.SaveUpdateHangFireJobDetailsForDA(configurationDetails.GroupId, configurationDetails.QuoteId, Convert.ToString(DaStatus.DA_INI_CMP), hangFireJobId);
                _dadl.SaveUpdateJobDetailsForDA(configurationDetails.GroupId, JobDetailsList,hangFireJobId, outputLocation);
                Utility.LogEnd(methodBeginTime);
            }
            else
            {
                throw new ExternalCallException(
                    new ResponseMessage
                    {
                        Message = Constants.SOMETHINGWENTWRONGMSG
                    });
            }
        }
    }
}
