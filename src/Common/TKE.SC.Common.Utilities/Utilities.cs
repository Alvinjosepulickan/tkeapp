using Configit.Configurator.Server.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.HttpClientModel;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.Common
{
    public class Utilities : LogHelper
    {
        private static readonly HttpClient _client = new HttpClient(new HttpClientHandler() { Proxy=new WebProxy() { BypassProxyOnLocal=true} })
        {
            Timeout = TimeSpan.FromMinutes(60)
        };

        /// <summary>
        /// Checks equality of two strings
        /// </summary>
        /// <param Name="valueOne"></param>
        /// <param Name="valueTwo"></param>
        /// <returns></returns>
        public static bool CheckEquals(string valueOne, string valueTwo)
        {
            if (!string.IsNullOrEmpty(valueOne) && !string.IsNullOrEmpty(valueTwo))
            {
                return (valueOne.Trim().ToUpper(CultureInfo.InvariantCulture) ==
                        valueTwo.Trim().ToUpper(CultureInfo.InvariantCulture));
            }

            return false;
        }
        public static async Task<HttpResponseMessage> MakeHttpRequest(HttpClientRequestModel clientRequestModel)
        {
            var methodStartTime = LogBegin();
            //TODO: construct URI
            var url = clientRequestModel.BaseUrl + clientRequestModel.EndPoint;
            var serilogProperties = new Dictionary<string, object>()
            {
                { "API-EndPoint" , url },
                { "API-MethodType",  clientRequestModel.Method  },
                { "API-RequestHeaders", clientRequestModel.RequestHeaders },
                //{ "API-RequestBody", clientRequestModel.RequestBody?.ToString() },
                { "API-BodyToEncode", clientRequestModel.BodyToEncode },
                { "API-Proxy", clientRequestModel.Proxy }
            };

            LogTrace($"API request to {clientRequestModel.BaseUrl}{clientRequestModel.EndPoint}", serilogProperties);

            HttpResponseMessage response;
            UpdateHeaders(clientRequestModel.ContentType, clientRequestModel.RequestHeaders);
            switch (clientRequestModel.Method)
            {
                case HTTPMETHODTYPE.POST:
                    if (CheckEquals(clientRequestModel.ContentType, Constants.CONTENTTYPEFORMURI))
                    {
                        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                        {
                            Content = new FormUrlEncodedContent(clientRequestModel.BodyToEncode)
                        };
                        response = await _client.SendAsync(requestMessage).ConfigureAwait(false);
                        break;
                    }
                    if (CheckEquals(clientRequestModel.PostAs, clientRequestModel.ContentType))
                    {
                        var content = new StringContent(string.Empty);
                        if (CheckEquals(clientRequestModel.ContentType, "text/plain"))
                        {
                            content = new StringContent(clientRequestModel.RequestString, Encoding.UTF8, clientRequestModel.PostAs);
                        }

                        else
                        {
                            content = new StringContent(clientRequestModel.RequestBody.ToString(), Encoding.UTF8, clientRequestModel.PostAs);
                        }

                        response = await _client.PostAsync(url, content).ConfigureAwait(false);
                        break;
                    }
                    else if (CheckEquals(clientRequestModel.PostAs, "JSONString"))
                    {
                        response = await _client.PostAsJsonAsync(url, Newtonsoft.Json.JsonConvert.DeserializeObject(clientRequestModel.RequestString)).ConfigureAwait(false);
                        break;
                    }

                    response = await _client.PostAsJsonAsync(url, clientRequestModel.RequestBody).ConfigureAwait(false);
                    break;
                case HTTPMETHODTYPE.PUT:
                    response = await _client.PutAsJsonAsync(url, clientRequestModel.RequestBody).ConfigureAwait(false);
                    break;
                default:
                    response = await _client.GetAsync(url).ConfigureAwait(false);
                    break;
            }

            serilogProperties.Add("API-Response", response);
            LogTrace($"API response from {clientRequestModel.BaseUrl}{clientRequestModel.EndPoint}", serilogProperties);
            LogEnd(methodStartTime);
            return response;
        }

        private static HttpClientHandler InitializeProxy(HttpClientHandler clientHandler)
        {
            //var configuration = new ConfigurationBuilder().SetBasePath(System.IO.Directory.GetCurrentDirectory()).AddJsonFile(Constants.APPSETTINGS).Build();
            //var viewSettings = GetSection(GetSection(configuration, Constants.PARAMSETTINGS), Constants.PROJECTMMANAGEMENTSETTINGS);
            //var proxyURL = GetPropertyValue(viewSettings, Constants.PROXYURI);

            //IWebProxy proxy = WebRequest.GetSystemWebProxy();
            //if (!string.IsNullOrEmpty(proxyURL))
            //{
            //    proxy = new WebProxy
            //    {
            //        Address = new Uri(proxyURL)
            //    };
            //}
            //proxy.Credentials = CredentialCache.DefaultCredentials;

            //clientHandler.Proxy = proxy;
            return clientHandler;
        }

        private static void UpdateProxy()
        {
        }

        private static void UpdateHeaders(string contentType, IDictionary<string, string> requestHeaders)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType ?? Constants.CONTENTTYPE));

            if (requestHeaders is null)
            {
                return;
            }

            foreach (var item in requestHeaders)
            {
                _client.DefaultRequestHeaders.Add(item.Key, item.Value);
            }

        }

        public static string GetPropertyValue(IConfiguration section, string propertyName)
        {
            if (GetSection(section, propertyName) is null)
            {
                return string.Empty;
            }
            return section.GetSection(propertyName).Value;
        }

        public static IConfiguration GetSection(IConfiguration section, string propertyName)
        {
            var subSection = section.GetSection(propertyName);
            if (subSection is null)
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = 500,
                    Message = $"No settings found for key:{propertyName}",
                    Description = $"No settings found for key:{propertyName}"
                });
            }
            return subSection;
        }

        public static string GetLandingOpeningAssignmentSelectedForGroupHallFixture(List<UnitDetailsValues> groupUnitHallFixturesOpeningValues)
        {

            var FrontRearUnitAssignment = "";
            foreach (var groupHallFixturesOpeningValues in groupUnitHallFixturesOpeningValues)
            {
                var frontvariable = new LandingOpening()
                {
                    Value = false
                };
                if (groupHallFixturesOpeningValues.UnitGroupValues == null)
                {
                    groupHallFixturesOpeningValues.UnitGroupValues = new List<GroupHallFixtureLocations>();
                }
                groupHallFixturesOpeningValues.UnitGroupValues.Where(x => x.Front == null).ToList().ForEach(y => y.Front = frontvariable);
                groupHallFixturesOpeningValues.UnitGroupValues.Where(x => x.Rear == null).ToList().ForEach(y => y.Rear = frontvariable);
                var FrontRearAssignment = "";
                //logic to fetch assigned opening for Entrance Console Card
                var lstFrontLanding = (from location in groupHallFixturesOpeningValues.UnitGroupValues
                                       where location.Front.Value.Equals(true)
                                       orderby location.FloorNumber ascending
                                       select location.FloorNumber).ToList();
                var lstRearLanding = (from location in groupHallFixturesOpeningValues.UnitGroupValues
                                      where location.Rear.Value.Equals(true)
                                      orderby location.FloorNumber ascending
                                      select location.FloorNumber).ToList();
                var FloorNumber = lstFrontLanding.Count > 0 ? lstFrontLanding[0] : 1;
                var FrontAssignement = "";
                var RearAssignment = "";

                if (lstFrontLanding.Count == 1)
                {
                    FrontAssignement = groupHallFixturesOpeningValues.UniDesgination + Constants.COLON + Constants.EMPTYSPACE + "F - " + lstFrontLanding[0];
                }
                else
                {
                    for (var index = 1; index < lstFrontLanding.Count; index++)
                    {
                        if (lstFrontLanding[index] - lstFrontLanding[index - 1] != 1)
                        {
                            if (lstFrontLanding[index - 1] != FloorNumber)
                            {
                                FrontAssignement += FrontAssignement.Equals("") ? FloorNumber + "-" + lstFrontLanding[index - 1] : ", " + FloorNumber + "-" + lstFrontLanding[index - 1];

                            }
                            else
                            {
                                FrontAssignement += FrontAssignement.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                            }
                            FloorNumber = lstFrontLanding[index];

                        }
                        if (index == lstFrontLanding.Count - 1)
                        {
                            if (lstFrontLanding[index] != FloorNumber)
                            {
                                FrontAssignement += FrontAssignement.Equals("") ? FloorNumber + "-" + lstFrontLanding[index] : ", " + FloorNumber + "-" + lstFrontLanding[index];

                            }
                            else
                            {
                                FrontAssignement += FrontAssignement.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                            }
                            FrontAssignement = groupHallFixturesOpeningValues.UniDesgination + Constants.COLON + Constants.EMPTYSPACE + "F - " + FrontAssignement;

                        }
                    }
                }
                FloorNumber = lstRearLanding.Count > 0 ? lstRearLanding[0] : 1;
                if (lstRearLanding.Count == 1)
                {
                    RearAssignment = groupHallFixturesOpeningValues.UniDesgination + Constants.COLON + Constants.EMPTYSPACE + "R - " + lstRearLanding[0];
                }
                else
                {
                    for (var index = 1; index < lstRearLanding.Count; index++)
                    {

                        if (lstRearLanding[index] - lstRearLanding[index - 1] != 1)
                        {
                            if (lstRearLanding[index - 1] != FloorNumber)
                            {
                                RearAssignment += RearAssignment.Equals("") ? FloorNumber + "-" + lstRearLanding[index - 1] : ", " + FloorNumber + "-" + lstRearLanding[index - 1];

                            }
                            else
                            {
                                RearAssignment += RearAssignment.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                            }

                            FloorNumber = lstRearLanding[index];

                        }
                        if (index == lstRearLanding.Count - 1)
                        {
                            if (lstRearLanding[index] != FloorNumber)
                            {
                                RearAssignment += RearAssignment.Equals("") ? FloorNumber + "-" + lstRearLanding[index] : ", " + FloorNumber + "-" + lstRearLanding[index];

                            }
                            else
                            {
                                RearAssignment += RearAssignment.Equals("") ? FloorNumber.ToString() : ", " + FloorNumber;
                            }
                            RearAssignment = groupHallFixturesOpeningValues.UniDesgination + Constants.COLON + Constants.EMPTYSPACE + "R - " + RearAssignment;
                        }

                    }
                }


                FrontRearAssignment = lstFrontLanding.Count == 0 ? lstRearLanding.Count == 0 ? "" : RearAssignment : lstRearLanding.Count == 0 ? FrontAssignement :
                                     FrontAssignement + " | " + RearAssignment;
                if (!string.IsNullOrEmpty(FrontRearAssignment))
                {
                    FrontRearUnitAssignment = string.IsNullOrEmpty(FrontRearUnitAssignment) ? FrontRearAssignment : FrontRearUnitAssignment + ", " + FrontRearAssignment;

                }
            }


            return FrontRearUnitAssignment;
        }
        /// <summary>
        /// Deserialize string Value to the specified object
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="value"></param>
        /// <returns></returns>
        public static T DeserializeObjectValue<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static async Task<List<User>> GetUserDetailsAsync(List<string> emailIds, IConfiguration _configuration)
        {
            var methodBeginTime = LogBegin();
            var identityServerSetting = GetSection(GetSection(_configuration, Constants.PARAMSETTINGS), Constants.IDENTITYSERVERSETTINGS);
            var requestObject = new HttpClientRequestModel()
            {
                BaseUrl = GetPropertyValue(identityServerSetting, Constants.ISSUER),
                EndPoint = GetPropertyValue(identityServerSetting, Constants.USERINFO),
                PostAs = "JSONString",
                Method = HTTPMETHODTYPE.POST,
                RequestString = (Newtonsoft.Json.JsonConvert.SerializeObject(emailIds))
            };
            var apiResponse = await MakeHttpRequest(requestObject).ConfigureAwait(true);
            var listUser = DeserializeObjectValue<List<User>>(apiResponse.Content.ReadAsStringAsync().Result);
            //List<User> listUser = new List<User>();
            //foreach (var email in emailIds)
            //{
            //    User userInfo = new User();
            //    TextInfo nameInfo = CultureInfo.CurrentCulture.TextInfo;
            //    userInfo.Email = email;                
            //    userInfo.FirstName = nameInfo.ToTitleCase(email.Split('@')[0].Split('.')[0]);
            //    userInfo.LastName = nameInfo.ToTitleCase(email.Split('@')[0].Split('.')[1]);
            //    listUser.Add(userInfo);
            //}
            LogEnd(methodBeginTime);
            return listUser;
        }

        /// <summary>
        /// Constant Mapper for dl methods
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="variableType"></param>
        /// <returns></returns>
        public static Dictionary<string, string> VariableMapper(string filePath, string variableType)
        {
            // get the constant Values
            var mapperData = JObject.Parse(File.ReadAllText(filePath));
            var buildingContantsDictionary = mapperData[variableType].ToObject<Dictionary<string, string>>();
            return buildingContantsDictionary;
        }
        /// <summary>
        /// Removes null values from the object
        /// </summary>
        /// <param Name="obj"></param>
        /// <returns></returns>
        public static JObject FilterNullValues(object obj)
        {
            var filteredJsonString = JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            return JObject.Parse(filteredJsonString);
        }


        public static ObomVariableAssignment GenerateObomVariablesAssignments(DataSet summaryScreenDataSet)
        {
            var obomVariableAssignments = new ObomVariableAssignment()
            {
                BuildingData = new List<BuildingData>()
            };
            var specMemo = new List<Characteristics>();
            if (summaryScreenDataSet != null)
            {
                var buildingIdList = (from DataRow dRow in summaryScreenDataSet.Tables[0].Rows
                                      select new
                                      {
                                          BuildingId = Convert.ToInt32(dRow[Constants.BUILDINGIDCOLUMNNAME])
                                      }).Distinct().ToList();

                var buildingVariableList = (from DataRow dRow in summaryScreenDataSet.Tables[0].Rows
                                            select new
                                            {
                                                BuildingId = Convert.ToInt32(dRow[Constants.BUILDINGIDCOLUMNNAME]),
                                                variableId = Convert.ToString(dRow[Constants.BUINDINGTYPE]),
                                                variableValue = Convert.ToString(dRow[Constants.BUINDINGVALUE])
                                            }).Distinct().ToList();
                var buildingElevation = (from DataRow dRow in summaryScreenDataSet.Tables[1].Rows
                                         select new
                                         {
                                             BuildingId = Convert.ToInt32(dRow[Constants.BUILDINGIDCOLUMNNAME]),
                                             FloorNumber = Convert.ToString(dRow["floorNumber"]),
                                             ElevationFeet = Convert.ToDecimal(dRow["ElevationFeet"]),
                                             ElevationInch = Convert.ToDecimal(dRow["ElevationInch"]),
                                             FloorToFloorHeightFeet = Convert.ToDecimal(dRow["FloorToFloorHeightFeet"]),
                                             FloorToFloorHeighInch = Convert.ToDecimal(dRow["FloorToFloorHeightInch"]),
                                             FloorDesignation = Convert.ToString(dRow["FloorDesignation"])
                                         }).Distinct().ToList().OrderBy(x => x.FloorNumber).ToList();
                var groupIdList = (from DataRow dRow in summaryScreenDataSet.Tables[2].Rows
                                   select new
                                   {
                                       BuildingId = Convert.ToInt32(dRow[Constants.BUILDINGIDCOLUMNNAME]),
                                       GroupId = Convert.ToInt32(dRow["GroupId"])
                                   }).Distinct().ToList();

                var groupConfigList = (from DataRow dRow in summaryScreenDataSet.Tables[2].Rows
                                       select new
                                       {
                                           GroupId = Convert.ToInt32(dRow["GroupId"]),
                                           variableId = Convert.ToString(dRow[Constants.GROUPCONFIGURATIONTYPE]),
                                           variableValue = Convert.ToString(dRow[Constants.GROUPCONFIGURATIONVALUE])
                                       }).Distinct().ToList();
                var unitData = (from DataRow dRow in summaryScreenDataSet.Tables[3].Rows
                                select new
                                {
                                    GroupId = Convert.ToInt32(dRow["GroupConfigurationId"]),
                                    UnitId = Convert.ToInt32(dRow["UnitId"]),
                                    UnitJson = Convert.ToString(dRow["UnitJson"]),
                                    SetId = Convert.ToInt32(dRow["SetId"]),
                                    Designation = Convert.ToString(dRow["Designation"]),
                                    UEID = Convert.ToString(dRow["UEID"]),
                                    MappedLocation = Convert.ToString(dRow["MappedLocation"]),
                                    FactoryJobId = Convert.ToString(dRow["FactoryJobId"])
                                }).Distinct().ToList();
                var unitIdList = (from DataRow dRow in summaryScreenDataSet.Tables[3].Rows
                                  select new
                                  {
                                      GroupId = Convert.ToInt32(dRow["GroupConfigurationId"]),
                                      UnitId = Convert.ToInt32(dRow["UnitId"]),
                                  }).Distinct().ToList();
                var setIdList = (from DataRow dRow in summaryScreenDataSet.Tables[3].Rows
                                 select new
                                 {
                                     GroupId = Convert.ToInt32(dRow["GroupConfigurationId"]),
                                     SetId = Convert.ToInt32(dRow["SetId"])
                                 }).Distinct().ToList();
                var controlLocation = (from DataRow dRow in summaryScreenDataSet.Tables[4].Rows
                                       select new
                                       {
                                           GroupId = Convert.ToInt32(dRow["GroupConfigurationId"]),
                                           variableId = Convert.ToString(dRow["ControlLocationType"]),
                                           variableValue = Convert.ToString(dRow["ControlLocationValue"])
                                       }).Distinct().ToList();
                var hallRiser = (from DataRow dRow in summaryScreenDataSet.Tables[5].Rows
                                 select new
                                 {
                                     GroupId = Convert.ToInt32(dRow["GroupConfigurationId"]),
                                     variableId = Convert.ToString(dRow["HallRiserType"]),
                                     variableValue = Convert.ToString(dRow["HallRiserValue"])
                                 }).Distinct().ToList();
                var doors = (from DataRow dRow in summaryScreenDataSet.Tables[6].Rows
                             select new
                             {
                                 GroupId = Convert.ToInt32(dRow["GroupConfigurationId"]),
                                 UnitId = Convert.ToInt32(dRow["UnitId"]),
                                 variableId = Convert.ToString(dRow["DoorType"]),
                                 variableValue = Convert.ToString(dRow["DoorValue"])
                             }).Distinct().ToList();
                var openingLocation = (from DataRow dRow in summaryScreenDataSet.Tables[7].Rows
                                       select new
                                       {
                                           GroupId = Convert.ToInt32(dRow["GroupConfigurationId"]),
                                           UnitId = Convert.ToInt32(dRow["UnitId"]),
                                           TravelFeet = Convert.ToDecimal(dRow["TravelFeet"]),
                                           TravelInch = Convert.ToDecimal(dRow["TravelInch"]),
                                           FloorDesignation = Convert.ToString(dRow["Floordesignation"]),
                                           FloorNumber = Convert.ToString(dRow["FloorNumber"]),
                                           OccupiedSpaceBelow = Convert.ToString(dRow["OcuppiedSpaceBelow"]),
                                           Front = Convert.ToBoolean(dRow["Front"]),
                                           Rear = Convert.ToBoolean(dRow["Rear"]),
                                       }).Distinct().ToList();
                var ghfConsoleVariable = (from DataRow dRow in summaryScreenDataSet.Tables[8].Rows
                                          select new
                                          {
                                              ConsoleId = Convert.ToInt32(dRow["GroupHallFixtureConsoleId"]),
                                              VariableType = Convert.ToString(dRow["VariableType"]),
                                              VariableValue = Convert.ToString(dRow["VariableValue"])
                                          }).Distinct().ToList();
                var setVariables = (from DataRow dRow in summaryScreenDataSet.Tables[9].Rows
                                    select new
                                    {
                                        SetId = Convert.ToInt32(dRow["SetId"]),
                                        VariableType = Convert.ToString(dRow["ConfigureVariables"]),
                                        VariableValue = Convert.ToString(dRow["ConfigureValues"])
                                    }).Distinct().ToList();
                var entranceConsoleVariables = (from DataRow dRow in summaryScreenDataSet.Tables[10].Rows
                                                select new
                                                {
                                                    ConsoleId = Convert.ToInt32(dRow["EntranceConsoleId"]),
                                                    VariableType = Convert.ToString(dRow["VariableType"]),
                                                    VariableValue = Convert.ToString(dRow["VariableValue"])
                                                }).Distinct().ToList();
                var unitHallFixtureConsoleVariables = (from DataRow dRow in summaryScreenDataSet.Tables[11].Rows
                                                       select new
                                                       {
                                                           ConsoleId = Convert.ToInt32(dRow["UnitHallFixtureConsoleId"]),
                                                           VariableType = Convert.ToString(dRow["VariableType"]),
                                                           VariableValue = Convert.ToString(dRow["VariableValue"])
                                                       }).Distinct().ToList();
                var groupHallFixtureData = (from DataRow dRow in summaryScreenDataSet.Tables[12].Rows
                                            select new
                                            {
                                                ConsoleId = Convert.ToInt32(dRow["GroupHallFixtureConsoleId"]),
                                                VariableType = Convert.ToString(dRow["VariableType"]),
                                                VariableValue = Convert.ToString(dRow["VariableValue"]),
                                                FloorDesignation = Convert.ToString(dRow["FloorDesignation"]),
                                                Front = Convert.ToBoolean(dRow["Front"]),
                                                Rear = Convert.ToBoolean(dRow["Rear"])
                                            }).Distinct().ToList();
                var groupHallFixtureConsoleList = (from DataRow dRow in summaryScreenDataSet.Tables[24].Rows
                                                   select new
                                                   {
                                                       ConsoleId = Convert.ToInt32(dRow["GroupHallFixtureConsoleId"]),
                                                       UnitId = Convert.ToInt32(dRow["GroupId"])
                                                   }).Distinct().ToList();
                var entranceLocation = (from DataRow dRow in summaryScreenDataSet.Tables[13].Rows
                                        select new
                                        {
                                            ConsoleId = Convert.ToInt32(dRow["EntranceConsoleId"]),
                                            FloorDesignation = Convert.ToString(dRow["FloorNumber"]),
                                            Front = Convert.ToBoolean(dRow["Front"]),
                                            Rear = Convert.ToBoolean(dRow["Rear"])
                                        }).Distinct().ToList();
                var unitHallFixtureLocation = (from DataRow dRow in summaryScreenDataSet.Tables[14].Rows
                                               select new
                                               {
                                                   ConsoleId = Convert.ToInt32(dRow["UnitHallFixtureConsoleId"]),
                                                   FloorDesignation = Convert.ToString(dRow["FloorNumber"]),
                                                   Front = Convert.ToString(dRow["Front"]),
                                                   Rear = Convert.ToString(dRow["Rear"])
                                               }).Distinct().ToList();
                var entranceConsoleIdList = (from DataRow dRow in summaryScreenDataSet.Tables[15].Rows
                                             select new
                                             {
                                                 ConsoleId = Convert.ToInt32(dRow["EntranceConsoleId"]),
                                                 SetId = Convert.ToInt32(dRow["SetId"])
                                             }).Distinct().ToList();
                var unitHallFixtureConsoleIdList = (from DataRow dRow in summaryScreenDataSet.Tables[16].Rows
                                                    select new
                                                    {
                                                        ConsoleId = Convert.ToInt32(dRow["UnitHallFixtureConsoleId"]),
                                                        SetId = Convert.ToInt32(dRow["SetId"])
                                                    }).Distinct().ToList();
                var specMemoList = (from DataRow dRow in summaryScreenDataSet.Tables[17].Rows
                                    select new
                                    {
                                        SpecMemoVersion = Convert.ToInt32(dRow["SpecMemoVersion"]),
                                        GroupId = Convert.ToInt32(dRow["GroupId"])
                                    }).Distinct().ToList();
                var priceDetails = (from DataRow dRow in summaryScreenDataSet.Tables[19].Rows
                                    select new
                                    {
                                        UnitId = Convert.ToInt32(dRow[Constants.UNITIDCOLUMNID]),
                                        VariableId = Convert.ToString(dRow[Constants.VARIABLEID]),
                                        variableValue = Convert.ToString(dRow[Constants.VARIABLEVALUE])
                                    }).Distinct().ToList();
                var projectDetails = (from DataRow dRow in summaryScreenDataSet.Tables[20].Rows
                                      select new
                                      {
                                          Name = Convert.ToString(dRow["Name"]),
                                          OpportunityId = Convert.ToString(dRow["OpportunityId"]),
                                          VersionId = Convert.ToString(dRow["VersionId"]),
                                          AccountName = Convert.ToString(dRow["AccountName"]),
                                          AddressLine1 = Convert.ToString(dRow["AddressLine1"]),
                                          AddressLine2 = Convert.ToString(dRow["AddressLine2"]),
                                          City = Convert.ToString(dRow["City"]),
                                          State = Convert.ToString(dRow["State"]),
                                          Country = Convert.ToString(dRow["Country"]),
                                          ZipCode = Convert.ToString(dRow["ZipCode"]),
                                          CustomerNumber = Convert.ToString(dRow["CustomerNumber"]),
                                          ProjectStatus = Convert.ToString(dRow["ProjectStatus"]),
                                          Branch = Convert.ToString(dRow["Branch"])
                                      }).Distinct().ToList();
                var quoteDetails = (from DataRow dRow in summaryScreenDataSet.Tables[18].Rows
                                    select new
                                    {
                                        OpportunityId = Convert.ToString(dRow["OpportunityId"]),
                                        VersionId = Convert.ToString(dRow["VersionId"]),
                                        QuoteId = Convert.ToString(dRow["QuoteId"]),
                                        QuoteStatus = Convert.ToString(dRow["DisplayName"])
                                    }).Distinct().ToList();
                var SystemValidationVariables = (from DataRow dRow in summaryScreenDataSet.Tables[21].Rows
                                                 select new
                                                 {
                                                     SetId = Convert.ToInt32(dRow["SetId"]),
                                                     VariableId = Convert.ToString(dRow["SystemVariableKeys"]),
                                                     Value = Convert.ToString(dRow["SystemVariableValues"])
                                                 }).Distinct().ToList();
                var fdaVariables = (from DataRow dRow in summaryScreenDataSet.Tables[22].Rows
                                    select new
                                    {
                                        GroupId = Convert.ToInt32(dRow["GroupId"]),
                                        VariableId = Convert.ToString(dRow["ConfigureVariables"]),
                                        Value = Convert.ToString(dRow["ConfigureValues"])
                                    }).Distinct().ToList();
                var productNameList = (from DataRow dRow in summaryScreenDataSet.Tables[23].Rows
                                       select new
                                       {
                                           SetId = Convert.ToInt32(dRow["SetId"]),
                                           ProductName = Convert.ToString(dRow["ProductName"]),
                                       }).Distinct().ToList();
                if (quoteDetails.Any())
                {
                    obomVariableAssignments.QuoteId = quoteDetails[0].QuoteId;
                    obomVariableAssignments.OpportunityId = quoteDetails[0].OpportunityId;
                    obomVariableAssignments.VersionId = quoteDetails[0].VersionId;
                    obomVariableAssignments.QuoteStatus = quoteDetails[0].QuoteStatus;
                    if (quoteDetails[0].QuoteStatus.Contains("Release", StringComparison.OrdinalIgnoreCase))
                    {
                        obomVariableAssignments.IsQuoteReleased = true;
                    }
                }
                if (projectDetails.Any())
                {
                    obomVariableAssignments.Name = projectDetails[0].Name;
                    obomVariableAssignments.ProjectStatus = projectDetails[0].ProjectStatus;
                    if (projectDetails[0].OpportunityId.Contains("SC", StringComparison.OrdinalIgnoreCase))
                    {
                        obomVariableAssignments.AccountName = projectDetails[0].AccountName;
                        obomVariableAssignments.Address = projectDetails[0].AddressLine1 + "\n" + projectDetails[0].AddressLine2 + "\n" + projectDetails[0].City + "\n" + projectDetails[0].State + "\n" + projectDetails[0].Country;
                        obomVariableAssignments.CustomerNumber = projectDetails[0].CustomerNumber;
                        obomVariableAssignments.Branch = projectDetails[0].Branch;
                        obomVariableAssignments.Country= projectDetails[0].Country;

                    }
                }
                foreach (var buildingId in buildingIdList)
                {
                    var buildingData = new BuildingData()
                    {
                        BuildingId = buildingId.BuildingId,
                        ConfigurationVariables = new List<VariableAssignment>(),
                        GroupData = new List<GroupData>(),
                        BuildingConfigurationVariables = new Dictionary<string, object>(),
                        NumberOfLanding = buildingElevation.Count()
                    };
                    var buildingElevationForBuilding = buildingElevation.Where(x => x.BuildingId == buildingId.BuildingId).ToList();
                    var buildingVariableListForBuilding= buildingVariableList.Where(x=>x.BuildingId== buildingId.BuildingId).ToList();
                    foreach (var buildingVariable in buildingVariableListForBuilding)
                    {
                        buildingData.ConfigurationVariables
                            .Add(new VariableAssignment()
                            {
                                VariableId = buildingVariable.variableId,
                                Value = buildingVariable.variableValue
                            });
                    }
                    var groupIdListForBuilding = groupIdList.Where(x => x.BuildingId == buildingId.BuildingId).ToList();
                    foreach (var groupId in groupIdListForBuilding)
                    {
                        var groupData = new GroupData()
                        {
                            GroupId = groupId.GroupId,
                            IsNcp = true,
                            VariableAssignment = new List<VariableAssignment>(),
                            UnitDataForObom = new List<UnitDataForObom>(),
                            FDAVariableAssignments = new List<VariableAssignment>(),
                            SetData = new List<SetData>(),
                            GroupConfigurationVariables = new Dictionary<string, object>()
                        };
                        var groupDataList = groupConfigList.Where(x => x.GroupId == groupId.GroupId).ToList();
                        foreach (var configurationVariable in groupDataList)
                        {
                            groupData.VariableAssignment
                                .Add(new VariableAssignment()
                                {
                                    VariableId = configurationVariable.variableId,
                                    Value = configurationVariable.variableValue
                                });
                            if (CheckEquals(configurationVariable.variableValue, "Elevator"))
                            {
                                groupData.IsNcp = false;
                            }
                        }
                        var unitListForGroup = unitData.Where(x => x.GroupId == groupId.GroupId);
                        var specmemoListForGroup = specMemoList.Where(x => x.GroupId == groupId.GroupId).ToList();
                        int elevatorNum = 0;
                        foreach (var unit in unitListForGroup)
                        {
                            elevatorNum += 1;
                            var setIdListForUnit = productNameList.Where(x => x.SetId == unit.SetId).ToList();
                            var characteristics = new List<Characteristics>();

                            characteristics.Add(new Characteristics()
                            {
                                ClassName = "ORDER",
                                CharacName = "UEID",
                                Value = unit.UEID
                            });
                            characteristics.Add(new Characteristics()
                            {
                                ClassName = "ORDER",
                                CharacName = "Unit Model",
                                Value = setIdListForUnit.Any() ? setIdListForUnit[0].ProductName : string.Empty
                            });
                            characteristics.Add(new Characteristics()
                            {
                                ClassName = "ORDER",
                                CharacName = "MJOBNUM",
                                Value = unit.FactoryJobId
                            });
                            var priceDetailsForUnits = priceDetails.Where(x => x.UnitId == unit.UnitId).ToList();
                            foreach (var unitPrice in priceDetailsForUnits)
                            {
                                var specMemoItem = new Characteristics()
                                {
                                    ClassName = "ORDER",
                                    CharacName = CheckEquals("unitprice", unitPrice.VariableId) ? "LISTPRICE" :
                                                 CheckEquals("totalprice", unitPrice.VariableId) ? "NETPRICE" :
                                                 CheckEquals("corporateassistance", unitPrice.VariableId) ? "CORPORATEASSISTANCE" :
                                                 CheckEquals("CorporateSubsidies", unitPrice.VariableId) ? "PRODUCTSUBSIDIES" :
                                                 CheckEquals("strategicdiscount", unitPrice.VariableId) ? "STRATEGICDISCOUNT" : string.Empty,
                                    Value = unitPrice.variableValue
                                };
                                if (!string.IsNullOrEmpty(specMemoItem.CharacName))
                                {
                                    characteristics.Add(specMemoItem);
                                }
                            }
                            groupData.UnitDataForObom.Add(new UnitDataForObom()
                            {
                                UnitId = unit.UnitId,
                                UnitName = unit.Designation,
                                VariableType = unit.UnitJson,
                                UEID = unit.UEID,
                                SetId = unit.SetId,
                                ElevatorName = setIdListForUnit.Any() ? setIdListForUnit[0].ProductName : string.Empty,
                                Location = unit.MappedLocation,
                                SpecMemoVersion = specmemoListForGroup.Any() ? specmemoListForGroup[0].SpecMemoVersion : 1,
                                Characteristics = characteristics,
                                MjobNum = unit.FactoryJobId,
                                Name = "ELEVATOR00" + Convert.ToString(elevatorNum),
                                OpeningLocation=new List<OpeningsAssigned>()
                            });
                            if (!groupData.IsNcp && !string.IsNullOrEmpty(unit.UnitJson))
                            {
                                groupData.VariableAssignment.Add(DeserializeObjectValue<VariableAssignment>(unit.UnitJson));
                            }
                        }
                        if (!groupData.IsNcp)
                        {
                            var fdaVariablesForGroup = fdaVariables.Where(x => x.GroupId == groupId.GroupId).ToArray();
                            foreach (var fdaVariable in fdaVariablesForGroup)
                            {
                                groupData.FDAVariableAssignments.Add(new VariableAssignment() { VariableId = fdaVariable.VariableId, Value = fdaVariable.Value });
                            }
                            var controlLocationForGroup = controlLocation.Where(x => x.GroupId == groupId.GroupId).ToList();
                            foreach (var configurationVariable in controlLocationForGroup)
                            {
                                groupData.VariableAssignment
                                    .Add(new VariableAssignment()
                                    {
                                        VariableId = configurationVariable.variableId,
                                        Value = configurationVariable.variableValue
                                    });
                            }
                            var hallRiserForGroup = hallRiser.Where(x => x.GroupId == groupId.GroupId).ToList();
                            foreach (var configurationVariable in hallRiserForGroup)
                            {
                                groupData.VariableAssignment
                                    .Add(new VariableAssignment()
                                    {
                                        VariableId = configurationVariable.variableId,
                                        Value = configurationVariable.variableValue
                                    });
                            }

                            var doorsForGroup = doors.Where(x => x.GroupId == groupId.GroupId).ToList();
                            foreach (var configurationVariable in doorsForGroup)
                            {
                                var mappedUnit = groupData.UnitDataForObom.Where(x => configurationVariable.variableId.Contains(x.Location, StringComparison.OrdinalIgnoreCase)).ToList();
                                if (mappedUnit.Any())
                                {
                                    groupData.VariableAssignment
                                        .Add(new VariableAssignment()
                                        {
                                            VariableId = configurationVariable.variableId.Replace(mappedUnit[0].Location, mappedUnit[0].Name),
                                            Value = configurationVariable.variableValue
                                        });
                                }
                            }
                            int elevatorNumber = 0;
                            var unitIdListForGroup = unitIdList.Where(x => x.GroupId == groupId.GroupId).ToList().OrderBy(x => x.UnitId);
                            foreach (var unit in unitIdListForGroup)
                            {
                                var openingdata = new List<OpeningsAssigned>();
                                elevatorNumber++;
                                var openingLocationForUnit = openingLocation.Where(x => x.UnitId == unit.UnitId).ToList();
                                groupData.VariableAssignment.Add(new VariableAssignment()
                                {
                                    VariableId = "ELEVATOR00" + Convert.ToString(elevatorNumber) + ".Parameters_SP.totalNumLandings_SP",
                                    Value = openingLocationForUnit.Count()
                                });
                                groupData.VariableAssignment.Add(new VariableAssignment()
                                {
                                    VariableId = "ELEVATOR00" + Convert.ToString(elevatorNumber) + ".Parameters.OPNFRTP",
                                    Value = openingLocationForUnit.Where(x => x.Front).ToList().Count()
                                });
                                groupData.VariableAssignment.Add(new VariableAssignment()
                                {
                                    VariableId = "ELEVATOR00" + Convert.ToString(elevatorNumber) + ".Parameters.CWTSFTY",
                                    Value = openingLocationForUnit[0].OccupiedSpaceBelow
                                });
                                groupData.VariableAssignment.Add(new VariableAssignment()
                                {
                                    VariableId = "ELEVATOR00" + Convert.ToString(elevatorNumber) + ".Parameters.OPNREARP",
                                    Value = openingLocationForUnit.Where(x => x.Rear).ToList().Count()
                                });
                                groupData.VariableAssignment.Add(new VariableAssignment()
                                {
                                    VariableId = "ELEVATOR00" + Convert.ToString(elevatorNumber) + ".Parameters.TOPR",
                                    Value = openingLocationForUnit.Where(x => x.Rear).ToList().Count()
                                });
                                groupData.VariableAssignment.Add(new VariableAssignment()
                                {
                                    VariableId = "ELEVATOR00" + Convert.ToString(elevatorNumber) + ".Parameters.TOPF",
                                    Value = openingLocationForUnit.Where(x => x.Front).Select(x => x.FloorNumber).Max()
                                });
                                groupData.VariableAssignment.Add(new VariableAssignment()
                                {
                                    VariableId = "ELEVATOR00" + Convert.ToString(elevatorNumber) + ".Parameters.TOTOPN",
                                    Value = openingLocationForUnit.Where(x => x.Front || x.Rear).Select(x => x.FloorNumber).Max()
                                });
                                groupData.VariableAssignment.Add(new VariableAssignment()
                                {
                                    VariableId = "ELEVATOR00" + Convert.ToString(elevatorNumber) + ".Parameters.TRAVEL",
                                    Value = openingLocationForUnit[0].TravelFeet * 12 + openingLocationForUnit[0].TravelInch
                                });
                                var groupHallFixtureConsoleForUnit = groupHallFixtureConsoleList.Where(x => x.UnitId == groupId.GroupId).OrderBy(x => x.ConsoleId).ToList();
                                foreach (var opening in openingLocationForUnit)
                                {
                                    var buildingElevationForFloor = buildingElevationForBuilding.Where(x => x.FloorNumber == opening.FloorNumber).ToList();
                                    groupData.VariableAssignment.Add(new VariableAssignment()
                                    {
                                        VariableId = "ELEVATOR00" + Convert.ToString(elevatorNumber) + ".FloorMatrix.LANDING" + Convert.ToString(opening.FloorNumber).PadLeft(3, Convert.ToChar(Constants.ZERO)) + ".Parameters.ENTF",
                                        Value = opening.Front
                                    });
                                    groupData.VariableAssignment.Add(new VariableAssignment()
                                    {
                                        VariableId = "ELEVATOR00" + Convert.ToString(elevatorNumber) + ".FloorMatrix.LANDING" + Convert.ToString(opening.FloorNumber).PadLeft(3, Convert.ToChar(Constants.ZERO)) + ".Parameters.ENTR",
                                        Value = opening.Rear
                                    });
                                    groupData.VariableAssignment.Add(new VariableAssignment()
                                    {
                                        VariableId = "ELEVATOR00" + Convert.ToString(elevatorNumber) + ".FloorMatrix.LANDING" + Convert.ToString(opening.FloorNumber).PadLeft(3, Convert.ToChar(Constants.ZERO)) + ".Parameters.ELEVATION",
                                        Value = buildingElevationForFloor[0].ElevationFeet * 12 + buildingElevationForFloor[0].ElevationInch
                                    });
                                    groupData.VariableAssignment.Add(new VariableAssignment()
                                    {
                                        VariableId = "ELEVATOR00" + Convert.ToString(elevatorNumber) + ".FloorMatrix.LANDING" + Convert.ToString(opening.FloorNumber).PadLeft(3, Convert.ToChar(Constants.ZERO)) + ".Parameters.FLRHTF",
                                        Value = buildingElevationForFloor[0].FloorToFloorHeightFeet * 12 + buildingElevationForFloor[0].ElevationInch
                                    });
                                    groupData.VariableAssignment.Add(new VariableAssignment()
                                    {
                                        VariableId = "ELEVATOR00" + Convert.ToString(elevatorNumber) + ".FloorMatrix.LANDING" + Convert.ToString(opening.FloorNumber).PadLeft(3, Convert.ToChar(Constants.ZERO)) + ".Parameters.FLRHTR",
                                        Value = buildingElevationForFloor[0].FloorToFloorHeightFeet * 12 + buildingElevationForFloor[0].ElevationInch
                                    });
                                    openingdata.Add(new OpeningsAssigned()
                                        {
                                        FloorNumber= Convert.ToInt32(buildingElevationForFloor[0].FloorNumber),
                                        Front = opening.Front,
                                        Rear=opening.Rear
                                    });
                                    foreach (var ghfConsole in groupHallFixtureConsoleForUnit)
                                    {
                                        var ghfConsolevariable = ghfConsoleVariable.Where(x => x.ConsoleId == ghfConsole.ConsoleId).ToList();
                                        var ghfData = groupHallFixtureData.Where(x => x.ConsoleId == ghfConsole.ConsoleId && x.FloorDesignation == buildingElevationForFloor[0].FloorDesignation).ToList();
                                        foreach (var data in ghfData)
                                        {
                                            if (data.Front || data.Rear)
                                            {

                                                groupData.VariableAssignment.Add(new VariableAssignment()
                                                {
                                                    VariableId = data.VariableType,
                                                    Value = data.VariableValue
                                                });
                                                if (!string.IsNullOrEmpty(data.VariableType))
                                                {
                                                    var variableValue = data.VariableType.Split(".");
                                                    if (variableValue.Any())
                                                    {
                                                        groupData.VariableAssignment.Add(new VariableAssignment()
                                                        {
                                                            VariableId = "ELEVATOR00" + ".FloorMatrix.LANDING" + Convert.ToString(data.FloorDesignation).PadLeft(3, Convert.ToChar(Constants.ZERO)) + ".Parameters." + variableValue[variableValue.Length - 1],
                                                            Value = data.VariableValue
                                                        });
                                                    }
                                                }

                                            }
                                        }

                                    }
                                }
                                foreach (var unitId in groupData.UnitDataForObom)
                                {
                                    if (unitId.UnitId.Equals(unit.UnitId))
                                    {
                                        unitId.OpeningLocation = openingdata;
                                    }
                                }
                            }
                        }


                        var setIdListForGroup = setIdList.Where(x => x.GroupId == groupId.GroupId).ToList();
                        foreach (var set in setIdListForGroup)
                        {
                            var setIdListForUnit = productNameList.Where(x => x.SetId == set.SetId).ToList();
                            var selectedUnits = groupData.UnitDataForObom.Where(x => x.SetId == set.SetId).ToList();
                            var setData = new SetData()
                            {
                                SetId = set.SetId,
                                ProductSelected = setIdListForUnit.Any() ? setIdListForUnit[0].ProductName : string.Empty,
                                SystemValidationVariables = new List<VariableAssignment>(),
                                VariableAssignment = new List<VariableAssignment>(),
                                SelectedUnits = selectedUnits.Any() ? selectedUnits.Select(x => x.Name).ToList() : new List<string>(),
                                SetConfigurationVariables = new Dictionary<string, object>()
                            };
                            var systemValidationVariablesForSet = SystemValidationVariables.Where(x => x.SetId == set.SetId).ToList();
                            foreach (var systemValidationVariable in systemValidationVariablesForSet)
                            {
                                setData.SystemValidationVariables
                                    .Add(new VariableAssignment()
                                    {
                                        VariableId = systemValidationVariable.VariableId,
                                        Value = systemValidationVariable.Value
                                    });
                            }
                            var setConfigurationVariables = setVariables.Where(x => x.SetId == set.SetId).ToList();
                            foreach (var setConfigurationVariable in setConfigurationVariables)
                            {
                                setData.SystemValidationVariables
                                    .Add(new VariableAssignment()
                                    {
                                        VariableId = setConfigurationVariable.VariableType,
                                        Value = setConfigurationVariable.VariableValue
                                    });
                            }

                            var unitHallFixtureConsoleforUnit = unitHallFixtureConsoleIdList.Where(x => x.SetId == set.SetId).OrderBy(x => x.ConsoleId).ToList();
                            foreach (var uhfConsole in unitHallFixtureConsoleforUnit)
                            {
                                var uhfConsolevariable = unitHallFixtureConsoleVariables.Where(x => x.ConsoleId == uhfConsole.ConsoleId).ToList();
                                var uhfLocations = unitHallFixtureLocation.Where(x => x.ConsoleId == uhfConsole.ConsoleId).ToList();
                                foreach (var opening in uhfLocations)
                                {
                                    if (opening.Front.Equals(Constants.TRUEVALUES, StringComparison.OrdinalIgnoreCase) || opening.Rear.Equals(Constants.TRUEVALUES, StringComparison.OrdinalIgnoreCase))
                                    {
                                        foreach (var consoleVariable in uhfConsolevariable)
                                        {

                                            if (!string.IsNullOrEmpty(consoleVariable.VariableType))
                                            {
                                                var variableValue = consoleVariable.VariableType.Split(".");
                                                if (variableValue.Any())
                                                {
                                                    setData.VariableAssignment.Add(new VariableAssignment()
                                                    {
                                                        VariableId = "ELEVATOR.FloorMatrix.LANDING" + Convert.ToString(opening.FloorDesignation).PadLeft(3, Convert.ToChar(Constants.ZERO)) + ".Parameters." + variableValue[variableValue.Length - 1],
                                                        Value = consoleVariable.VariableValue
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            var entranceConsoleforUnit = entranceConsoleIdList.Where(x => x.SetId == set.SetId).OrderBy(x => x.ConsoleId).ToList();
                            foreach (var entranceConsole in entranceConsoleforUnit)
                            {
                                var entranceConsolevariable = entranceConsoleVariables.Where(x => x.ConsoleId == entranceConsole.ConsoleId).ToList();
                                var entranceLocations = entranceLocation.Where(x => x.ConsoleId == entranceConsole.ConsoleId).ToList();
                                foreach (var opening in entranceLocations)
                                {
                                    if (opening.Front.Equals(true) || opening.Rear.Equals(true))
                                    {
                                        foreach (var consoleVariable in entranceConsolevariable)
                                        {

                                            if (!string.IsNullOrEmpty(consoleVariable.VariableType))
                                            {
                                                var variableValue = consoleVariable.VariableType.Split(".");
                                                if (variableValue.Any())
                                                {
                                                    setData.VariableAssignment.Add(new VariableAssignment()
                                                    {
                                                        VariableId = "ELEVATOR.FloorMatrix.LANDING" + Convert.ToString(opening.FloorDesignation).PadLeft(3, Convert.ToChar(Constants.ZERO)) + ".Parameters." + variableValue[variableValue.Length - 1],
                                                        Value = consoleVariable.VariableValue
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            groupData.SetData.Add(setData);
                        }
                        buildingData.GroupData.Add(groupData);
                    }
                    obomVariableAssignments.BuildingData.Add(buildingData);
                }
            }
            return obomVariableAssignments;
        }

        /// <summary>
        /// adding data to sql parameter for DA
        /// </summary>
        /// <param Name="DAJobDetailsDataTable"></param>
        /// <returns></returns>
        public static DataTable DaJobDetails(List<DaJobDetails> DaJobDetailsList)
        {
            DataTable DaJobDetailsDataTable = new DataTable();
            {
                DaJobDetailsDataTable.Clear();
                DataColumn jobId = new DataColumn("JobId")
                {
                    DataType = typeof(string)
                };
                DaJobDetailsDataTable.Columns.Add(jobId);

                DataColumn packageName = new DataColumn("PackageName")
                {
                    DataType = typeof(string)
                };
                DaJobDetailsDataTable.Columns.Add(packageName);

                DataColumn daJobStatus = new DataColumn("DAJobStatus")
                {
                    DataType = typeof(string)
                };
                DaJobDetailsDataTable.Columns.Add(daJobStatus);

                DataColumn packageError = new DataColumn("packageError")
                {
                    DataType = typeof(string)
                };
                DaJobDetailsDataTable.Columns.Add(packageError);
                foreach (var details in DaJobDetailsList)
                {
                    DataRow drawingTableRow = DaJobDetailsDataTable.NewRow();
                    drawingTableRow[0] = details?.DaJobId;
                    drawingTableRow[1] = details?.PackageName;
                    drawingTableRow[2] = details?.DaJobStatus;
                    drawingTableRow[3] = details?.PackageError;
                    DaJobDetailsDataTable.Rows.Add(drawingTableRow);
                }
                return DaJobDetailsDataTable;
            }
        }
    }
}
