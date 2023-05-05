using Configit.Configurator.Server.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Database;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.HttpClientModel;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common;
using Constant = TKE.SC.BFF.DataAccess.Helpers.Constant;
using TKE.SC.Common.Model;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class OzDL : IOzDL
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param Name="logger"></param>
        public OzDL(IConfiguration configuration, ILogger<OzDL> logger)
        {
            Utility.SetLogger(logger);
            _configuration = configuration;
        }
        /// <summary>
        /// Get Equipment Values for Oz API
        /// </summary>
        /// <param Name="qouteId"></param>
        /// <returns></returns>
        public EquipmentandDrawing GetEquipmentAndDrawingForOZ(string qouteId)
        {
            var methodBeginTime = Utility.LogBegin();
            var ozVariables=Utility.VariableMapper(Constant.INTEGRATIONCONSTANTMAPPER, Constant.OZ);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForOzView(qouteId);
            DataSet dataSetOz = CpqDatabaseManager.ExecuteDataSet(Constant.GETVARIABLEVALUESFOROZ, lstSqlParameter);
            List<Equipment> lstEquipment = new List<Equipment>();
            RequestedDrawing requestedDrawing = new RequestedDrawing();
            var setConfigurationDetails = new List<UnitVariablesAssignmentValue>();
            Dictionary<int, List<string>> unitDict = new Dictionary<int, List<string>>();
            var equipmentanddrawing = new EquipmentandDrawing();
            if (dataSetOz != null && dataSetOz.Tables.Count > 1 && dataSetOz.Tables[0].Rows.Count > 0)
            {
                var unitIDList = (from DataRow dRow in dataSetOz.Tables[0].Rows
                                  select new
                                  {
                                      GroupId = Convert.ToInt32(dRow[Constant.GROUPID]),
                                      unitId = Convert.ToInt32(dRow[Constant.UNITID]),
                                      setId=Convert.ToInt32(dRow[Constant.SETIDPASCALCASE]),
                                      UEID = (dRow[Constant.UEID].ToString()),
                                      productName = (dRow[Constant.PRODUCTNAM].ToString()),
                                      unitDesignation = (dRow[Constant.DESIGNATION].ToString()),
                                      DoDQuestions = dRow[Constant.QUESTIONS].ToString(),
                                  }).Distinct().ToList();
                var unitlist = Utility.SerializeObjectValue(unitIDList);
                var unitidlist = Utility.DeserializeObjectValue<List<UnitDetailsOz>>(unitlist);
                lstEquipment = Utility.GenerateDoDDetails(unitidlist);
                foreach(var unit in unitidlist)
                {
                    if(unitDict.ContainsKey(unit.SetId))
                    {
                        unitDict[unit.SetId].Add(unit.UEID);
                    }
                    else
                    {

                        unitDict[unit.SetId] = new List<string>();
                        unitDict[unit.SetId].Add(unit.UEID);
                    }
                }
                var fdaDrawingList = (from DataRow dRow in dataSetOz.Tables[1].Rows
                                      select new
                                      {
                                          FDAType = (dRow[Constant.FDATYPE]).ToString()

                                      }).Distinct().ToList();
                var fdaList = Utility.SerializeObjectValue(fdaDrawingList);
                var fdaDrawing = Utility.DeserializeObjectValue<List<FdaOz>>(fdaList);
                requestedDrawing = Utility.GetRequestedDrawingDetails(fdaDrawing);

                var unitDetails = (from DataRow dRow in dataSetOz.Tables[2].Rows
                                   select new
                                   {
                                       SetId = Convert.ToInt32(dRow[Constant.SETIDPASCALCASE]),
                                       VariableId = Convert.ToString(dRow[Constant.SYSTEMVARIABLESKEYS]),
                                       VariableValue = Convert.ToString(dRow[Constant.SYSTEMVARIABLESVALUES]),
                                   }).Distinct().ToList();
                var unitConfigDetails = Utility.SerializeObjectValue(unitDetails);
                var unitVariableValues = Utility.DeserializeObjectValue<List<UnitVariablesOz>>(unitConfigDetails);

                var unitDetailsSet = (from DataRow dRow in dataSetOz.Tables[3].Rows
                                      select new
                                      {
                                          SetId = Convert.ToInt32(dRow[Constant.SETIDPASCALCASE]),
                                          VariableId = Convert.ToString(dRow[Constant.SYSTEMVARIABLESKEYS]),
                                          VariableValue = Convert.ToString(dRow[Constant.SYSTEMVARIABLESVALUES]),
                                      }).Distinct().ToList();
                unitConfigDetails = Utility.SerializeObjectValue(unitDetailsSet);
                unitVariableValues.AddRange(Utility.DeserializeObjectValue<List<UnitVariablesOz>>(unitConfigDetails));

                var setUnitVariables = unitVariableValues.GroupBy(u => u.SetId).ToList();
                foreach (var key in setUnitVariables)
                {
                    var unitVariableAssignments = new UnitVariablesAssignmentValue();
                    unitVariableAssignments.SetId = key.Key;
                    unitVariableAssignments.VariableAssignments = new List<VariableAssignment>();
                    unitVariableAssignments.ProductName=(from units in unitidlist where units.SetId == unitVariableAssignments.SetId select units.ProductName).ToList().FirstOrDefault();
                    foreach (var elements in key)
                    {
                        var variableAssignments = new VariableAssignment
                        {
                            VariableId = elements.VariableId,
                            Value = elements.VariableValue
                        };
                        if((Utility.CheckEquals(elements.VariableId, ozVariables[Constant.PARAMETERBASICREAROPEN]))&&(Utility.CheckEquals(elements.VariableValue, Constant.TRUE_LOWERCASE)))
                        {
                            unitVariableAssignments.RearDoorSelected = true;
                        }
                        unitVariableAssignments.VariableAssignments.Add(variableAssignments);
                    }
                    setConfigurationDetails.Add(unitVariableAssignments);
                }
                
            }
            equipmentanddrawing.equipment = lstEquipment;
            equipmentanddrawing.requestedDrawing = requestedDrawing;
            equipmentanddrawing.SetConfigurationDetails = setConfigurationDetails;
            equipmentanddrawing.UnitDictionary = unitDict;
            Utility.LogEnd(methodBeginTime);
            return equipmentanddrawing;
        }

        /// <summary>
        /// Get Branch ID Oz API
        /// </summary>
        /// <param Name="branchName"></param>
        /// <returns></returns>
        public int GetBranchId(string branchName)
        {
            var methodBeginTime = Utility.LogBegin();
            int BranchNumber = 0;
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.BRANCHNAME, Value = branchName}
            };
            DataSet dsBranch = new DataSet();
            dsBranch = CpqDatabaseManager.ExecuteDataSet(Constant.USPGETBRANCHID, sqlParameters);
            if (dsBranch != null && dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
            {
                BranchNumber = Convert.ToInt32(dsBranch.Tables[0].Rows[0][Constant.BRANCHNUMBER]);
            }
            Utility.LogEnd(methodBeginTime);
            return BranchNumber;
        }

        /// <summary>
        /// Get Project ID and Version ID Values for Oz API
        /// </summary>
        /// <param Name="qouteId"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetProjectIdVersionId(string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            Dictionary<string, string> dictProject = new Dictionary<string, string>();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.QUOTEID, Value = quoteId }
            };
            DataSet dsProjVer = new DataSet();
            dsProjVer = CpqDatabaseManager.ExecuteDataSet(Constant.USPGETPROJECTIDVERSIONID, sqlParameters);
            if (dsProjVer != null && dsProjVer.Tables.Count > 0 && dsProjVer.Tables[0].Rows.Count > 0)
            {
                string opportunityId = (dsProjVer.Tables[0].Rows[0][Constant.OPPORTUNITY_ID]).ToString();
                string versionId = (dsProjVer.Tables[0].Rows[0][Constant.VERSIONID]).ToString();
                dictProject[Constant.OPPORTUNITY_ID] = opportunityId;
                dictProject[Constant.VERSIONID] = versionId;
            }
            Utility.LogEnd(methodBeginTime);
            return dictProject;
        }

        ///// <summary>
        /// Method sets the status for qoute and underlying entities as 'Coordination' when 
        /// send to coordination operation is successfull
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <returns></returns>
        public int SaveSentToCoordinationWorkflowstatusforQuote(string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            List<SqlParameter> lstParams = new List<SqlParameter>
            {
                new SqlParameter()
                {
                    ParameterName=Constant._ID,Value=quoteId
                },
                new SqlParameter()
                {
                    ParameterName=Constant.SECTION,Value="senttocordination"
                }
            };
            int resultForUpdateWorkflowstatus = CpqDatabaseManager.ExecuteNonquery(Constant.SPUPDATEWORKFLOWSTATUS, lstParams, string.Empty);
            Utility.LogEnd(methodBeginTime);
            return resultForUpdateWorkflowstatus;
        }

        public async Task<string> GetOzToken(string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            var ozSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constant.COORDINATIONSETTINGS);

            var requestObject = new HttpClientRequestModel()
            {
                BaseUrl = Utility.GetPropertyValue(ozSettings, Constant.BASEURL),
                EndPoint = Utility.GetPropertyValue(ozSettings, Constant.TOKENAPI),
                BodyToEncode = new Dictionary<string, string>
                {
                    {Constant.APIUSERNAME.ToLower(),Utility.GetPropertyValue(ozSettings, Constant.APIUSERNAME) },
                     {Constant.APIPASSWORD.ToLower(), Utility.GetPropertyValue(ozSettings, Constant.APIPASSWORD) },
                     {Constant.GRANTTYPE.ToLower(), Utility.GetPropertyValue(ozSettings, Constant.GRANTTYPESETTING) }
                },
                Method = HTTPMETHODTYPE.POST,
                ContentType = Constants.CONTENTTYPEFORMURI,
                Proxy = Utility.GetPropertyValue(ozSettings, Constant.PROXYURI)
            };

            var apiResponse = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(false);
            var accessToken = string.Empty;
            JObject respObj = JObject.Parse(apiResponse.Content.ReadAsStringAsync().Result);

            if (respObj[Constant.ACCESSTOKENOBJ] != null)
            {
                accessToken = Convert.ToString(respObj[Constant.ACCESSTOKENOBJ]);
            }
            Utility.LogEnd(methodBeginTime);
            return accessToken;
        }

        public async Task<ResponseMessage> BookCoOrdination(string quoteId, string sessionId, OzBookingRequest ozBookingRequest)
        {
            var methodBeginTime = Utility.LogBegin();

            var ozSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constant.COORDINATIONSETTINGS);
            var accessToken = await GetOzToken(sessionId).ConfigureAwait(false);
          
            var requestObject = new HttpClientRequestModel()
            {
                BaseUrl = Utility.GetPropertyValue(ozSettings, Constant.BASEURL),
                EndPoint = Utility.GetPropertyValue(ozSettings, Constant.BOOKINGAPI),
                RequestBody = JObject.FromObject(ozBookingRequest, new Newtonsoft.Json.JsonSerializer()),
                Method = HTTPMETHODTYPE.POST,
                Proxy = Utility.GetPropertyValue(ozSettings, Constant.PROXYURI),
                PostAs = Constant.CONTENTTYPE,
                ContentType=Constant.CONTENTTYPE,
                RequestHeaders = new Dictionary<string, string>
                        {
                            { Constant.AUTHORIZATION, Constant.BEARER + accessToken }
                        }
            };

            var apiResponse = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(false);
            var responseContent = apiResponse.Content.ReadAsStringAsync().Result;

            switch (Convert.ToString(apiResponse.StatusCode).ToUpper())
            {
                case Constant.BADREQUESTTEXT:
                    Utility.LogEnd(methodBeginTime);
                    throw new ExternalCallException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Response = null,
                        Message = Constant.ERRORINBOOKINGREQUEST,
                        Description = OzErrorValidation(responseContent,ozBookingRequest),
                        HttpResponseMessage = new HttpResponseMessage(apiResponse.StatusCode)
                    });
                case Constant.OK:
                case Constant.SUCCESSTEXT:
                    int result = SaveSentToCoordinationWorkflowstatusforQuote(quoteId);
                    if (result == -1)
                    {
                        Utility.LogEnd(methodBeginTime);
                        throw new CustomException(new ResponseMessage
                        {
                            StatusCode = Constant.BADREQUEST,
                            Message = Constant.ERRORWHILEUPDATINGWORKFLOWSTATUS,
                            Description = Constant.ERRORWHILEUPDATINGWORKFLOWSTATUS
                        });
                    }
                    Utility.LogEnd(methodBeginTime);
                    return new ResponseMessage { StatusCode = Constant.SUCCESS, Message = Constant.SENDTOCOORDINATIONSUCCESSMSG };
                default:

                    Utility.LogEnd(methodBeginTime);
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = Convert.ToInt32(apiResponse.StatusCode),
                        Message = Constant.ERRORINBOOKINGREQUEST,
                        Description = apiResponse.ReasonPhrase
                    });

            }


        }
        private string OzErrorValidation(string responseContent,OzBookingRequest ozBookingRequest)
        {
            var content = responseContent;
            if (responseContent.Contains(Constant.OZQUOTETABLE) && responseContent.Contains(Constant.DUPLICATE))
            {
                content = Constant.QUOTEIDERROR1 + ozBookingRequest.ProjectInformation.ProjectIdentifier.QuoteId + Constant.QUOTEIDERROR2;
            }
            else if (responseContent.Contains(Constant.OZSUBMITTALTABLE) && responseContent.Contains(Constant.DUPLICATE))
            {
                content = Constant.OZUEIDERROR;
            }
            return content;
        }
    }
}
