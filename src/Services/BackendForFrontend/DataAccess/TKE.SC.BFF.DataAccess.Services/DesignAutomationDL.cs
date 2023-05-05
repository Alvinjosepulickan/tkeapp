using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using Configit.TKE.OrderBom.WebApi;
using Configit.TKE.DesignAutomation.Models;
using Configit.TKE.DesignAutomation.Services;
using Configit.TKE.DesignAutomation.WebApi;
using Configit.TKE.OrderBom.CLMPlatform;
using Configit.TKE.OrderBom.Models;
using Configit.TKE.OrderBom.Services;
using Configit.TKE.OrderBom.WebApi.Models;
using System.Threading.Tasks;
using Constants = TKE.SC.Common.Constants;
using TKE.SC.Common.Model.HttpClientModel;
using Newtonsoft.Json.Linq;
using Configit.TKE.DesignAutomation.WebApi.Models;
using Configit.TKE.DesignAutomation.Services.Models;
using System.Data.SqlClient;
using System.Data;
using TKE.SC.Common.Database;
using System.Linq;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class DesignAutomationDL : IDesignAutomationDL
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger _logger;
        /// <summary>
        /// object ICacheManager
        /// </summary>
        private readonly ICacheManager _cpqCacheManager;

        private readonly string _environment;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param Name="logger"></param>
        public DesignAutomationDL(IConfiguration configuration, ICacheManager cpqCacheManager, ILogger<DesignAutomationDL> logger)
        {
            Utility.SetLogger(logger);
            _logger = logger;
            _cpqCacheManager = cpqCacheManager;
            _environment = Constant.DEV;
            _configuration = configuration;
        }

        /// <summary>
        /// External API Call For ConfigitBom Service
        /// </summary>
        /// <param name="requestBody"></param>
        /// <param name="packagePath"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetOBOMResponseForDA(CreateBomRequest requestBody, ConfigurationDetails configurationDetails, string sessionId, string packagePath="")
        {
            _logger.LogTrace("CreatBom Request of " + packagePath + "is Started");
            var daSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constants.DESIGNAUTOMATION);
            var requestObject = new HttpClientRequestModel()
            {
                BaseUrl = Utility.GetPropertyValue(daSettings, Constants.OBOMSERVICEURL),
                EndPoint = Utility.GetPropertyValue(daSettings, Constants.GETOBOM),
                RequestBody = JObject.FromObject(requestBody, new Newtonsoft.Json.JsonSerializer()),
                Method = HTTPMETHODTYPE.POST,
                PostAs = Constant.CONTENTTYPE,
                ContentType = Constant.CONTENTTYPE,
            };
            var apiResponse = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(false);
            var responseContent = apiResponse.Content.ReadAsStringAsync().Result;
            var response = Utility.DeserializeObjectValue<CreateBomResponse>(responseContent);
            if (response != null)
            {
                _logger.LogTrace("CreatBom Response of " + packagePath + " is Complete");
                return new ResponseMessage() { Response = JObject.FromObject(response), Message = requestBody.Configurations[0].ModelFileName};
            }
            else
            {
                _logger.LogTrace("CreatBom Response of " + packagePath + " is Null");
                if (!(apiResponse.StatusCode.Equals(Constants.SUCCESS)))
                {
                    var serilogProperties = new Dictionary<string, object>()
                    {
                        { "StatusCode" ,apiResponse.StatusCode  },
                        { "Headers",  apiResponse.Headers },
                        { "RequestMessage", apiResponse.RequestMessage },
                        { "ReasonPhrase", apiResponse.ReasonPhrase }
                    };

                    _logger.LogError(packagePath +"Error Message "+ apiResponse.ReasonPhrase, serilogProperties);
                    var hangFireJobId = _cpqCacheManager.GetCache(sessionId, _environment, Convert.ToString(configurationDetails.GroupId));
                    var daJobDetails = new List<DaJobDetails> { new DaJobDetails { PackageName = packagePath, PackageError = apiResponse.ReasonPhrase,DaJobStatus = string.Empty, DaJobId = string.Empty } };
                    SaveUpdateJobDetailsForDA(configurationDetails.GroupId, daJobDetails, hangFireJobId);
                }
                return new ResponseMessage() { Response = null, Message = requestBody.Configurations[0].ModelFileName };
            }
        }

        /// <summary>
        /// External API Call for Getting Default Export Types
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetDefaultExportTypes()
        {
            var daSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constants.DESIGNAUTOMATION);
            var requestObject = new HttpClientRequestModel()
            {
                BaseUrl = Utility.GetPropertyValue(daSettings, Constants.DASERVICEURL),
                EndPoint = Utility.GetPropertyValue(daSettings, Constants.DEFAULTEXPORTTYPES),
                Method = HTTPMETHODTYPE.GET,
                PostAs = Constant.CONTENTTYPE,
                ContentType = Constant.CONTENTTYPE,
            };
            var apiResponse = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(false);
            var responseContent = apiResponse.Content.ReadAsStringAsync().Result;
            var response = Utility.DeserializeObjectValue<List<string>>(responseContent);
            return response;
        }

        /// <summary>
        /// External API Call for Getting Available Export Types
        /// </summary>
        /// <returns></returns>
        public async Task<ExportTypeResponse[]> GetAvailableExportTypes()
        {
            var daSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constants.DESIGNAUTOMATION);
            var requestObject = new HttpClientRequestModel()
            {
                BaseUrl = Utility.GetPropertyValue(daSettings, Constants.DASERVICEURL),
                EndPoint = Utility.GetPropertyValue(daSettings, Constants.AVAILABLEEXPORTTYPES),
                Method = HTTPMETHODTYPE.GET,
                PostAs = Constant.CONTENTTYPE,
                ContentType = Constant.CONTENTTYPE,
            };
            var apiResponse = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(false);
            var responseContent = apiResponse.Content.ReadAsStringAsync().Result.ToString();
            var response = Utility.DeserializeObjectValue<ExportTypeResponse[]>(responseContent);
            return response;
        }

        /// <summary>
        /// External API Call for Getting SubmitBom Response
        /// </summary>
        /// <param name="requestBody"></param>
        /// <param name="packagePath"></param>
        /// <returns></returns>
        public async Task<SubmitBomResponse> GetSubmitBOMResponse(SubmitBomRequest requestBody,ConfigurationDetails configurationDetails,string sessionId,string packagePath="")
        {
            _logger.LogTrace("SubmitBom Request of " + packagePath + " is Started");
            var daSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constants.DESIGNAUTOMATION);
            var requestObject = new HttpClientRequestModel()
            {
                BaseUrl = Utility.GetPropertyValue(daSettings, Constants.DASERVICEURL),
                EndPoint = Utility.GetPropertyValue(daSettings, Constants.SUBMITBOM),
                RequestBody = JObject.FromObject(requestBody, new Newtonsoft.Json.JsonSerializer()),
                Method = HTTPMETHODTYPE.POST,
                PostAs = Constant.CONTENTTYPE,
                ContentType = Constant.CONTENTTYPE,
            };
            var apiResponse = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(false);
            var responseContent = apiResponse.Content.ReadAsStringAsync().Result;
            var response = new SubmitBomResponse();
            if (!string.IsNullOrEmpty(responseContent))
            {
                _logger.LogTrace("SubmitBom Response of " + packagePath + " is Complete");
                response = Utility.DeserializeObjectValue<SubmitBomResponse>(responseContent);
            }
            else
            {
                _logger.LogTrace("SubmitBom Response of " + packagePath +" is Null");
                if (!(apiResponse.StatusCode.Equals(Constants.SUCCESS)))
                {
                    var serilogProperties = new Dictionary<string, object>()
                    {
                        { "StatusCode" ,apiResponse.StatusCode  },
                        { "Headers",  apiResponse.Headers },
                        { "RequestMessage", apiResponse.RequestMessage },
                        { "ReasonPhrase", apiResponse.ReasonPhrase }
                    };

                    _logger.LogError(packagePath + "Error Message "+apiResponse.ReasonPhrase, serilogProperties);
                    var hangFireJobId = _cpqCacheManager.GetCache(sessionId, _environment, Convert.ToString(configurationDetails.GroupId));
                    var daJobDetails = new List<DaJobDetails> { new DaJobDetails { PackageName = packagePath ,PackageError=apiResponse.ReasonPhrase,DaJobStatus=string.Empty,DaJobId=string.Empty} };
                    SaveUpdateJobDetailsForDA(configurationDetails.GroupId, daJobDetails, hangFireJobId);
                }
            }

            return response;
        }

        /// <summary>
        /// External API Call for Getting DA Job Status
        /// </summary>
        /// <param name="jodId"></param>
        /// <returns></returns>
        public async Task<AutomationTaskDetailsReference> GetJobStatus(string jodId)
        {

            var daSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constants.DESIGNAUTOMATION);
            var requestObject = new HttpClientRequestModel()
            {
                BaseUrl = Utility.GetPropertyValue(daSettings, Constants.DASERVICEURL),
                EndPoint = Utility.GetPropertyValue(daSettings, Constants.AUTOMATIONSTATUS) +Constants.QUESTIONMARK+ Constants.JOBID +Constants.EQUALTO +jodId,
                Method = HTTPMETHODTYPE.GET,
                PostAs = Constant.CONTENTTYPE,
                ContentType = Constant.CONTENTTYPE,
            };
            var apiResponse = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(false);
            var responseContent = apiResponse.Content.ReadAsStringAsync().Result;
            var response = Utility.DeserializeObjectValue<AutomationTaskDetailsReference>(responseContent);
            return response;
        }

        /// <summary>
        /// Method to Save JobId to DB
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="daJobDetailsList"></param>
        /// <param name="hangFireJobId"></param>
        /// <param name="outputLocation"></param>
        /// <returns></returns>
        public int SaveUpdateJobDetailsForDA(int groupId,List<DaJobDetails>daJobDetailsList,string hangFireJobId, string outputLocation = "")
        {
            var methodBeginTime = Utility.LogBegin();
            DataTable daDetailsDataTable = Utilities.DaJobDetails(daJobDetailsList);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.GROUPID_CAMELCASE, Value = groupId},
                new SqlParameter() { ParameterName = Constants.OUTPUTLOCATION, Value = outputLocation},
                new SqlParameter() { ParameterName = Constants.HANGFIREJOBID, Value = hangFireJobId},
                new SqlParameter() { ParameterName =  Constants.DADETAILS,Value=daDetailsDataTable,Direction = ParameterDirection.Input },
                new SqlParameter() { ParameterName =Constant.@RESULT,Value= 0,Direction = ParameterDirection.Output }
            };
            var result = CpqDatabaseManager.ExecuteNonquery(Constants.USPSAVEUPDATEJOBDETAILSFORDA, sqlParameters);
            Utility.LogEnd(methodBeginTime);
            return result;
        }

        /// <summary>
        /// Method to get JobId form DB
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="sessionId"></param>
        /// <param name="hangFireJobId"></param>
        /// <param name="daStatus"></param>
        /// <returns></returns>
        public List<DaJobIdDetails> GetJobIdForDA(int groupId, string sessionId,ref string hangFireJobId, out string daStatus)
        {
            var methodBeginTime = Utility.LogBegin();
            daStatus = string.Empty;
            var jobIdList = new List<DaJobIdDetails>();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.GROUPID_CAMELCASE, Value = groupId}
            };
            DataSet ds = CpqDatabaseManager.ExecuteDataSet(Constants.USPGETJOBIDFORDA, sqlParameters);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var jobIdListDB = (from DataRow row in ds.Tables[0].Rows
                                select new
                                {
                                    JobId = Convert.ToString(row[Constants.JOBID_PASCALCASE]),
                                    HangFireJobId = Convert.ToString(row[Constants.HANGFIREJOBID_PASCALCASE]),
                                    PackageName = Convert.ToString(row[Constants.PACKAGENAME_PASCALCASE]),
                                    DaJobStatus = Convert.ToString(row[Constants.DAJOBSTATUS_PASCALCASE]),
                                }).Distinct().ToList();
                jobIdList= Utilities.DeserializeObjectValue<List<DaJobIdDetails>>(Utility.SerializeObjectValue(jobIdListDB));
                
            }
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
            {
                var hnagFireDetails = (from DataRow row in ds.Tables[1].Rows
                                   select new
                                   {
                                       HangFireJobId = Convert.ToString(row[Constants.HANGFIREJOBID_PASCALCASE]),
                                       DaJobStatus = Convert.ToString(row[Constants.JOBSTATUS_PASCALCASE]),
                                   }).Distinct().ToList();
                foreach (var job in hnagFireDetails)
                {
                    if (!string.IsNullOrEmpty(job?.HangFireJobId))
                    {
                        hangFireJobId = job?.HangFireJobId;
                        daStatus = job?.DaJobStatus;
                    }
                }
            }
            Utility.LogEnd(methodBeginTime);
            return jobIdList;
        }
        /// <summary>
        /// method to save Hangfire job details to db
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="qouteId"></param>
        /// <param name="hangfireJobStatus"></param>
        /// <param name="hangfireJobId"></param>
        /// <returns></returns>
        public int SaveUpdateHangFireJobDetailsForDA(int groupId, string qouteId, string hangfireJobStatus, string hangfireJobId)
        {
            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.GROUPID_CAMELCASE, Value = groupId},
                new SqlParameter() { ParameterName = Constants.HANGFIREJOBID, Value = hangfireJobId},
                new SqlParameter() { ParameterName = Constants.HANGFIREJOBSTATUS, Value = hangfireJobStatus},
                new SqlParameter() { ParameterName = Constant.QUOTEID_LOWERCASE, Value = qouteId},
                new SqlParameter() { ParameterName =Constant.@RESULT,Value= 0,Direction = ParameterDirection.Output }
            };
            var result = CpqDatabaseManager.ExecuteNonquery(Constants.USPSAVEUPDATEHANGFIREJOBDETAILSFORDA, sqlParameters);
            Utility.LogEnd(methodBeginTime);
            return result;
        }
    }
}
