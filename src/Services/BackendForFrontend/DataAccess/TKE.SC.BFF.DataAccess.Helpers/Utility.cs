using Configit.Configurator.Server.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.HttpClientModel;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common;
using static TKE.SC.Common.Model.UIModel.ListOfConfiguration;
using Product = TKE.SC.Common.Model.UIModel.Product;

namespace TKE.SC.BFF.DataAccess.Helpers
{
    public class Utility : Utilities
    {
        public static List<string> NonConfigurableProducts = new List<string>(3)
            {
                 Constant.ESCLATORMOVINGWALK
                ,Constant.TWINELEVATOR
                ,Constant.OTHER
            };
        /// <summary>
        /// Common method for initializing a Http request.
        /// </summary>
        /// <param Name="client"></param>
        /// <param Name="packagePath"></param>
        /// <param Name="methodType"></param>
        /// <param Name="requestUrl"></param>
        /// <param Name="apiRoute"></param>
        /// <param Name="requestBody"></param>
        /// <param Name="accessToken"></param>
        /// <param Name="requestFor"></param>
        /// <param Name="muleSoftClientId"></param>
        /// <param Name="muleSoftClientSecret"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> InitializeHttpRequest(HttpClient client, string packagePath,
            string methodType, Uri requestUrl, string apiRoute, JObject requestBody, string jwtToken, RequestingContext requestFor = 0, string muleSoftClientId = null, string muleSoftClientSecret = null)
        {
            HttpResponseMessage response;
            client.BaseAddress = requestUrl;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constant.CONTENTTYPE));
            client.Timeout = TimeSpan.FromMinutes(30);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                client.DefaultRequestHeaders.Add("Authorization", jwtToken);
            }

            if (CheckEquals(methodType, Constant.POST))
            {
                if (requestFor == RequestingContext.AccessToken)
                {
                    client.DefaultRequestHeaders.Add(Constant.GRANTTYPE, Constant.GRANTTYPEVALUE);
                    client.DefaultRequestHeaders.Add(Constant.CLIENTID, muleSoftClientId);
                    client.DefaultRequestHeaders.Add(Constant.CLIENTSECRET, muleSoftClientSecret);
                }

                response = await client.PostAsJsonAsync(apiRoute, requestBody).ConfigureAwait(false);

            }
            else if (CheckEquals(methodType, Constant.PUT))
            {
                response = await client.PutAsJsonAsync(apiRoute, requestBody).ConfigureAwait(false);
            }
            else
            {
                response = await client.GetAsync(apiRoute).ConfigureAwait(false);
            }

            return response;
        }

        /// <summary>
        /// Common method for initializing a HttpsRequest
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="client"></param>
        /// <param Name="methodType"></param>
        /// <param Name="requestUrl"></param>
        /// <param Name="apiRoute"></param>
        /// <param Name="requestBody"></param>
        /// <param Name="accessToken"></param>
        /// <param Name="requestFor"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> InitializeHttpsRequest<T>(HttpClient client, string methodType, Uri requestUrl,
            string apiRoute, T requestBody, string accessToken = null, RequestingContext requestFor = 0)
        {
            var response = new HttpResponseMessage();
            using (var handler = new HttpClientHandler())
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                client.BaseAddress = requestUrl;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constant.CONTENTTYPE));
                switch (methodType.ToUpper())
                {
                    case Constant.GET:

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(accessToken);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constant.CONTENTTYPE));
                            response = await client.GetAsync(apiRoute).ConfigureAwait(false);
                        }
                        else
                        {
                            response = await client.GetAsync(apiRoute).ConfigureAwait(false);
                        }
                        break;
                    case Constant.POST:
                        var type = requestBody?.GetType();
                        if (CheckEquals(type?.ToString(), Constant.TYPEMULTIFORMCONTENT) &&
                            !string.IsNullOrEmpty(accessToken))
                        {
                            client.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue(Constant.ACCESSTOKENTYPE, accessToken);
                            client.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue(Constant.CONTENTTYPEFORMDATA));
                            response = await client
                                .PostAsync(apiRoute, (MultipartFormDataContent)(object)requestBody)
                                .ConfigureAwait(false);
                            break;
                        }


                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(accessToken);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constant.CONTENTTYPE));
                            response = await client.PostAsJsonAsync(apiRoute, requestBody).ConfigureAwait(false);

                        }
                        else
                        {
                            response = await client.PostAsJsonAsync(apiRoute, requestBody).ConfigureAwait(false);
                        }
                        break;
                    case Constant.PUT:
                        response = await client.PutAsJsonAsync(apiRoute, requestBody).ConfigureAwait(false);
                        break;
                    case Constant.PATCH:

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(accessToken);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constant.CONTENTTYPE));
                            response = await PatchAsJsonAsync(client, apiRoute, requestBody).ConfigureAwait(false);
                            break;
                        }
                        response = await PatchAsJsonAsync(client, apiRoute, requestBody).ConfigureAwait(false);
                        break;
                    default:
                        break;
                }

            }

            return response;
        }

        /// <summary>
        /// Patch As Json Async Class
        /// </summary>
        /// <typeparam Name="JObject"></typeparam>
        /// <param Name="client"></param>
        /// <param Name="requestUri"></param>
        /// <param Name="requestBody"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PatchAsJsonAsync<JObject>(HttpClient client, string requestUri,
            JObject requestBody)
        {
            var requestUrl = string.Empty;
            if (client != null)
            {
                requestUrl = string.Concat(client.BaseAddress, requestUri);
                var content = new ObjectContent<JObject>(requestBody, new JsonMediaTypeFormatter());
                var request = new HttpRequestMessage(new HttpMethod(Constant.PATCH), requestUrl) { Content = content };
                return await client.SendAsync(request).ConfigureAwait(false);
            }

            return null;
        }


        /// <summary>
        /// Get Description based on Enum Value
        /// </summary>
        /// <param Name="val"></param>
        /// <returns></returns>
        public static string ShowEnumLabel(bldStatus val)
        {
            Type type = val.GetType();
            string name = Enum.GetName(type, val);
            if (name != null)
            {
                var field = type.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field,
                       typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    {
                        //Console.WriteLine(attr.Description);
                        return attr.Description;
                    }
                }
            }
            return "";
            // Console.WriteLine("No matching label!");
        }

        /// <summary>
        /// Get Description based on Enum Value
        /// </summary>
        /// <param Name="val"></param>
        /// <returns></returns>
        public static string ShowEnumLabelForFDA(drawingGenerationMethod val)
        {
            Type type = val.GetType();
            string name = Enum.GetName(type, val);
            if (name != null)
            {
                var field = type.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field,
                       typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    {
                        //Console.WriteLine(attr.Description);
                        return attr.Description;
                    }
                }
            }
            return "";
            // Console.WriteLine("No matching label!");
        }




        /// <summary>
        /// Maps the response 
        /// </summary>
        /// <param Name="response"></param>
        /// <param Name="json"></param>
        /// <returns></returns>
        public static async Task<ResponseMessage> MapResponse(HttpResponseMessage response, bool userInfo = false, bool isArrayObj = false)
        {
            var json = string.Empty;
            if (response.Content != null)
            {
                using (var content = response.Content)
                {
                    json = await content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }

            var responseObj = new ResponseMessage
            {
                StatusCode = (int)response?.StatusCode
            };
            if (response.IsSuccessStatusCode)
            {
                if (!userInfo)
                {
                    if (isArrayObj)
                    {
                        responseObj.ResponseArray = JArray.Parse(json);
                    }
                    else
                    {
                        responseObj.Response = JObject.Parse(json);
                    }
                }
                responseObj.Message = json;
                responseObj.IsSuccessStatusCode = true;
                return responseObj;
            }

            responseObj.RequestUri = response.RequestMessage?.RequestUri?.ToString();
            responseObj.HttpResponseMessage = response;
            responseObj.HttpRequestMessage = response.RequestMessage;
            responseObj.Message = json;
            return responseObj;
        }

        /// <summary>
        /// SerializeObject Value Class
        /// </summary>
        /// <param Name="value"></param>
        /// <returns></returns>
        public static string SerializeObjectValue(object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        //For testing - 
        /// <summary>
        /// Deserialize string value to the specified object
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="value"></param>
        /// <returns></returns>
        public static T DeserializeObjectValue<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static List<SqlParameter> SqlParameterForAddandUpdateBuilding(int buildingId, string userId, string projectId, string buildingName, string bldVariablejson, ConflictsStatus isEditFlow, bool hasConflictsFlag, DataTable variableMapperAssignment)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@quoteId",Value=projectId,Direction = ParameterDirection.Input }
                ,new SqlParameter() { ParameterName = "@OpportunityId",Value=projectId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = "@BldName",Value=buildingName,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = "@BldJson",Value=bldVariablejson,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = "@ModifiedBy",Value=userId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = "@Id",Value=buildingId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter(){ ParameterName ="@IsEditFlow",Value = isEditFlow ,Direction=ParameterDirection.Input,SqlDbType = SqlDbType.VarChar }
               ,new SqlParameter() { ParameterName = "@Result",Value=buildingId,Direction = ParameterDirection.Output }
        };
            if (buildingId > 0)
            {
                lstSqlParameter.Add(new SqlParameter() { ParameterName = Constant.VARIABLEMAPPERDATATABLE, Value = variableMapperAssignment, Direction = ParameterDirection.Input });
            }
            return lstSqlParameter;
        }



        public static List<SqlParameter> SqlParameterForLayoutStatus(int groupId, int fieldDrawingIntegrationMasterId, string obj, string userName)
        {
            List<SqlParameter> lstSqlParameter = new List<SqlParameter>
                        {
                            new SqlParameter() { ParameterName = Constant.FDAGROUPID,Value=groupId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
                           ,new SqlParameter() { ParameterName = Constant.INTEGRATEDPROCESSID,Value=fieldDrawingIntegrationMasterId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
                           , new SqlParameter() { ParameterName = Constant.FDAPROCESSJSON,Value=Convert.ToString(obj),Direction = ParameterDirection.Input, SqlDbType = SqlDbType.NVarChar}
                            ,new SqlParameter() { ParameterName = Constant.FDACREATEDDBY,Value=userName,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.NVarChar}
                            ,new SqlParameter() { ParameterName = Constant.FDAPARAMSTATUSID,Value="DWG_ERR",Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
                        };

            return lstSqlParameter;
        }



        public static List<SqlParameter> SqlParameterForUnitSet(DataTable unitDataTable)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@unitList",Value=unitDataTable}
               ,new SqlParameter() { ParameterName = "@Result",Value=0,Direction = ParameterDirection.Output }
            };
            return lstSqlParameter;
        }
        public static List<SqlParameter> SqlParameterForAutoSaveConfiguration(AutoSaveConfiguration autoSaveRequest)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@requestMessage",Value=JsonConvert.SerializeObject(autoSaveRequest.RequestMessage),Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = "@userId",Value=autoSaveRequest.UserName,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = "@Result",Value=0,Direction = ParameterDirection.Output }
            };
            return lstSqlParameter;
        }
        public static List<SqlParameter> SqlParameterForSavingProductSelection(int groupConfigurationId, ProductSelection productSelection, DataTable productSelectionDataTable, string businessLine, string country, int controlLanding,
            DataTable defaultUHFVariables, DataTable defaultEntranceConfigurations, string fixtureStrategy, string supplyingFactory)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@username",Value=productSelection.UserName,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = "@unitList",Value=productSelectionDataTable,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@BusinessLine",Value=businessLine,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@country",Value=country,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@supplyingFactory",Value=supplyingFactory,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@productSelected",Value=productSelection.productSelected,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = "@controlLanding",Value=controlLanding,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@defaultConfigurationUHF",Value=defaultUHFVariables,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@defaultConfigurationEntrances",Value=defaultEntranceConfigurations,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@FixtureStrategy",Value=fixtureStrategy,Direction = ParameterDirection.Input, SqlDbType = SqlDbType.VarChar }
               ,new SqlParameter() { ParameterName = "@Result",Value=0,Direction = ParameterDirection.Output }
            };
            return lstSqlParameter;
        }



        public static List<SqlParameter> SqlParameterListForEditUnitIndependently(int unitId, string userName)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@UserName",Value=userName,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = "@UnitId",Value=unitId,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@Result",Value=0,Direction = ParameterDirection.Output }
            };
            return lstSqlParameter;
        }

        public static List<SqlParameter> SqlParameterForUpdatingProductSelection(int groupConfigurationId, string productSelection)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@GroupConfigurationId",Value=groupConfigurationId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@productSelected",Value=productSelection,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = "@Result",Value=0,Direction = ParameterDirection.Output }
            };
            return lstSqlParameter;
        }

        /// <summary>
        /// Utility method to return list of sqlparameter for deleting building configuration
        /// </summary>
        /// <param Name="userName"></param>
        /// <returns></returns>
        public static List<SqlParameter> SqlParameterForDeletingAutoSaveConfiguration(string userName)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
               new SqlParameter() { ParameterName = "@userName",Value=userName,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = "@Result",Value=0,Direction = ParameterDirection.Output }
            };
            return lstSqlParameter;
        }


        /// <summary>
        /// Utility method to return list of sqlparameter for sqlParameterFor BuildingElevation
        /// </summary>
        /// <param Name="dtBuildingElevation"></param>
        /// <returns></returns>
        public static List<SqlParameter> sqlParameterForBuildingElevation(DataTable dtBuildingElevation)
        {
            List<SqlParameter> lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@elevationData",Value=dtBuildingElevation}
                ,new SqlParameter() { ParameterName = "@Result",Value=1,Direction = ParameterDirection.Output }
            };
            return lstSqlParameter;
        }

        /// <summary>
        /// adding data to sql parameter for saving group configuration 
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="GroupName"></param>
        /// <param Name="userName"></param>
        /// <param Name="grpVariablejson"></param>
        /// <returns></returns>
        public static List<SqlParameter> SqlParameterForSavingGroupConfiguration(int buildingId, string groupName, string userName, string grpVariablejson)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@BuildingId",Value=buildingId,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@GroupName",Value=groupName,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@GroupJson",Value=grpVariablejson,Direction = ParameterDirection.Input}
               //,new SqlParameter() { ParameterName = "@UnitVariables",Value=unitVariableAssignment,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userName,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@Result",Value=buildingId,Direction = ParameterDirection.Output }
            };
            return lstSqlParameter;
        }




        public static List<SqlParameter> SqlParameterForSavingUnitsForNCP(int groupId, int numberOfUnits, string userName)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.GROUPINGID_LOWERCASE,Value=groupId,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = Constant.NUMBEROFUNITS_LOWERCASE,Value=numberOfUnits,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName =  Constant.CREATEDBY_LOWERCASE,Value=userName,Direction = ParameterDirection.Input}
            };
            return lstSqlParameter;
        }


        /// <summary>
        /// adding data to sql parameter for updating group configuration
        /// </summary>
        /// <param Name="BuildingId"></param>
        /// <param Name="GroupName"></param>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="grpVariablejson"></param>
        /// <returns></returns>
        public static List<SqlParameter> SqlParameterForUpdatingGroupConfiguration(int buildingId, string groupName, int groupConfigurationId, string grpVariablejson)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@BuildingId",Value=buildingId,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@GroupName",Value=groupName,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@GroupJson",Value=grpVariablejson,Direction = ParameterDirection.Input}
               //,new SqlParameter() { ParameterName = "@UnitVariables",Value=unitVariableAssignment,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@GroupConfigurationId",Value=groupConfigurationId,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@Result",Value=buildingId,Direction = ParameterDirection.Output }
            };
            return lstSqlParameter;
        }

        /// <summary>
        /// adding data to sql parameter for FDA
        /// </summary>
        /// <param Name="FieldDrawingAutomationDataTable"></param>
        /// <returns></returns>
        public static DataTable FDADrawingAutomation(List<ConfigVariable> FieldDrawingAutomationDataTable)
        {
            DataTable DrawingDataTable = new DataTable();
            {
                DrawingDataTable.Clear();
                DataColumn FDAType = new DataColumn("FDAType")
                {
                    DataType = typeof(string)
                };
                DrawingDataTable.Columns.Add(FDAType);
                //value
                DataColumn FDAValue = new DataColumn("FDAValue")
                {
                    DataType = typeof(int)
                };
                DrawingDataTable.Columns.Add(FDAValue);
                foreach (var unit in FieldDrawingAutomationDataTable)
                {
                    DataRow DrawingTableRow = DrawingDataTable.NewRow();
                    DrawingTableRow[0] = unit.VariableId;
                    DrawingTableRow[1] = Convert.ToInt32(unit.Value);
                    DrawingDataTable.Rows.Add(DrawingTableRow);
                }
                return DrawingDataTable;
            }
        }

        /// <summary>
        /// adding data to sql parameter for FDA
        /// </summary>
        /// <param Name="FieldDrawingAutomationDataTable"></param>
        /// <returns></returns>
        public static DataTable GroupVariableForDrawingAutomation(List<ConfigVariable> GroupVariableAutomationDataTable)
        {
            DataTable DrawingDataTable = new DataTable();
            {
                DrawingDataTable.Clear();
                DataColumn FDAType = new DataColumn("GroupConfigurationType")
                {
                    DataType = typeof(string)
                };
                DrawingDataTable.Columns.Add(FDAType);
                //value
                DataColumn FDAValue = new DataColumn("GroupConfigurationValue")
                {
                    DataType = typeof(string)
                };
                DrawingDataTable.Columns.Add(FDAValue);
                foreach (var unit in GroupVariableAutomationDataTable)
                {
                    DataRow DrawingTableRow = DrawingDataTable.NewRow();
                    DrawingTableRow[0] = unit.VariableId;
                    DrawingTableRow[1] = unit.Value;
                    DrawingDataTable.Rows.Add(DrawingTableRow);
                }
                return DrawingDataTable;
            }
        }


        /// <summary>
        /// adding data to sql paramter for updating openinng location
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="OpeningLocationDataTable"></param>
        /// <returns></returns>
        public static List<SqlParameter> SqlParameterForUpdateOpeningLocation(OpeningLocations openingLocation, DataTable openingLocationDataTable, DataTable historyTable)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@GroupConfigurationId",Value=openingLocation.GroupConfigurationId,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@OpeningLocationDataTable",Value=openingLocationDataTable,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@historyTable",Value=historyTable,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int},
            };
            return lstSqlParameter;
        }

        /// <summary>
        /// generate data table for Unit table
        /// </summary>
        /// <param Name="unitVariableAssignment"></param>
        /// <returns></returns>
        public static DataTable GenerateDataTableForUnitTable(List<ConfigVariable> unitVariableAssignment, List<DisplayVariableAssignmentsValues> displayVariableAssignments)
        {
            DataTable unitDataTable = new DataTable();
            unitDataTable.Clear();
            DataColumn unitId = new DataColumn("UnitId")
            {
                DataType = typeof(int)
            };
            unitDataTable.Columns.Add(unitId);
            //variableId
            DataColumn unitJson = new DataColumn("UnitJson")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(unitJson);
            //value
            DataColumn value = new DataColumn("value")
            {
                DataType = typeof(Boolean)
            };
            unitDataTable.Columns.Add(value);
            //jsondata
            DataColumn unitJsonData = new DataColumn("unitJsonData")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(unitJsonData);

            DataColumn DisplayUnitJson = new DataColumn("DisplayUnitJson")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(DisplayUnitJson);

            DataColumn Location = new DataColumn("Location")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(Location);

            DataColumn MappedLocationJson = new DataColumn("MappedLocationJson")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(MappedLocationJson);
            DataColumn UnitName = new DataColumn("UnitName")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(UnitName);

            DataColumn UnitDesignation = new DataColumn("UnitDesignation")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(UnitDesignation);

            DataColumn IsFutureElevator = new DataColumn("IsFutureElevator")
            {
                DataType = typeof(Boolean)
            };
            unitDataTable.Columns.Add(IsFutureElevator);

            var unitNames = JObject.Parse(JObject.Parse(File.ReadAllText(Constant.GROUPMAPPERVARIABLES))[Constant.CARPOSITION].ToString());
            var unitNameDictionary = unitNames.ToObject<Dictionary<string, string>>();
            int count = 0;
            int numberOfUnits = 1;
            foreach (var unit in unitVariableAssignment)
            {
                if (Convert.ToBoolean(unit.Value))
                {
                    var variableAssignment = (from variables in displayVariableAssignments
                                              where Utility.CheckEquals(variables.MappedTo, unit.VariableId) && Convert.ToBoolean(variables.Value)
                                              select variables).FirstOrDefault();
                    if (variableAssignment is null)
                    {
                        continue;
                    }
                    DataRow unitTableRow = unitDataTable.NewRow();
                    unitTableRow[0] = numberOfUnits++;
                    unitTableRow[1] = unit.VariableId;
                    unitTableRow[2] = unit.Value;
                    unitTableRow[3] = JsonConvert.SerializeObject(unit);

                    var positionVariable = variableAssignment.VariableId.Split(Constant.DOT);
                    var actualvariable = variableAssignment.MappedTo.Split(Constant.DOT);
                    var unitNameValue = unitNameDictionary[Convert.ToString(positionVariable[positionVariable.Count() - 1])];
                    if (count == 0)
                    {
                        unitTableRow[4] = JsonConvert.SerializeObject(displayVariableAssignments);
                    }
                    unitTableRow[5] = actualvariable[actualvariable.Count() - 1];
                    unitTableRow[6] = positionVariable[positionVariable.Count() - 1];
                    unitTableRow[7] = unitNameDictionary[actualvariable[actualvariable.Count() - 1]];
                    count += 1;
                    unitDataTable.Rows.Add(unitTableRow);
                    if (string.IsNullOrEmpty(variableAssignment.UnitDesignation))
                    {
                        variableAssignment.UnitDesignation = string.Empty;
                    }
                    unitTableRow[8] = variableAssignment.UnitDesignation;
                    unitTableRow[9] = variableAssignment.IsFutureElevator;
                }
            }
            return unitDataTable;
        }

        public static DataTable GenerateDataTableForProductSelection(ProductSelection productSelection)
        {
            DataTable productSelectionDataTable = new DataTable();
            productSelectionDataTable.Clear();

            DataColumn unitId = new DataColumn("UnitId")
            {
                DataType = typeof(int)
            };
            productSelectionDataTable.Columns.Add(unitId);

            for (int i = 0; i < productSelection.UnitId.Count; i++)
            {
                DataRow unitIdRow = productSelectionDataTable.NewRow();
                unitIdRow[0] = Convert.ToInt32(productSelection.UnitId[i]);
                productSelectionDataTable.Rows.Add(unitIdRow);
            }
            return productSelectionDataTable;
        }

        public static DataTable GenerateDataTableForProductSelection(List<int> unitIdList)
        {
            DataTable unitDataTable = new DataTable();
            unitDataTable.Clear();

            DataColumn unitId = new DataColumn("UnitId")
            {
                DataType = typeof(int)
            };
            unitDataTable.Columns.Add(unitId);

            for (int i = 0; i < unitIdList.Count; i++)
            {
                DataRow unitIdRow = unitDataTable.NewRow();
                unitIdRow[0] = Convert.ToInt32(unitIdList[i]);
                unitDataTable.Rows.Add(unitIdRow);
            }
            return unitDataTable;
        }

        public static DataTable GenerateDataTableForSaveGroupLayout(List<ConfigVariable> unitVariableAssignment, List<UnitMappingValues> unitMappingValues)
        {
            DataTable unitDataTable = new DataTable();
            unitDataTable.Clear();


            DataColumn unitJson = new DataColumn("UnitJson")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(unitJson);

            DataColumn value = new DataColumn("value")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(value);

            DataColumn unitJsonData = new DataColumn("unitJsonData")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(unitJsonData);

            DataColumn unitName = new DataColumn("UnitName");
            unitJsonData.DataType = typeof(string);
            unitDataTable.Columns.Add(unitName);

            foreach (var unit in unitVariableAssignment)
            {


                DataRow unitTableRow = unitDataTable.NewRow();
                unitTableRow[0] = unit.VariableId;
                unitTableRow[1] = unit.Value;
                unitTableRow[2] = JsonConvert.SerializeObject(unit);

                foreach (var untiValueData in unitMappingValues)
                {
                    if (unit.VariableId.Contains(untiValueData.ElevatorName.ToString()))
                    {
                        unitTableRow[3] = untiValueData.UnitId;
                    }
                }

                unitDataTable.Rows.Add(unitTableRow);

            }
            return unitDataTable;
        }

        public static DataTable GenerateDataTableForUnitConfiguration(List<ConfigVariable> unitVariableAssignment)
        {
            DataTable unitDataTable = new DataTable();
            unitDataTable.Clear();


            DataColumn unitJson = new DataColumn("UnitJson")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(unitJson);

            DataColumn value = new DataColumn("value")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(value);

            DataColumn unitJsonData = new DataColumn("unitJsonData")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(unitJsonData);

            foreach (var unit in unitVariableAssignment)
            {

                DataRow unitTableRow = unitDataTable.NewRow();
                unitTableRow[0] = unit.VariableId;

                if (unit.VariableId.Equals(Constant.REGEN))
                {
                    if (unit.Value.Equals(true))
                    {
                        unit.Value = Constant.TRUE_LOWERCASE;
                    }
                    else if (unit.Value.Equals(false))
                    {
                        unit.Value = Constant.FALSE_LOWERCASE;
                    }
                }
                else if (unit.Value.Equals(true))
                {
                    unit.Value = Constant.TRUE_UPPERCASE;
                }
                else if (unit.Value.Equals(false))
                {
                    unit.Value = Constant.FALSE_UPPERCASE;
                }
                unitTableRow[1] = unit.Value;
                unitTableRow[2] = JsonConvert.SerializeObject(unit);

                unitDataTable.Rows.Add(unitTableRow);

            }
            return unitDataTable;
        }


        public static DataTable GenerateDataTableForWiring(ConfigVariable unitVariableAssignment, int unitId)
        {
            DataTable unitDataTable = new DataTable();
            unitDataTable.Clear();

            DataColumn unitIds = new DataColumn("unitIds")
            {
                DataType = typeof(int)
            };
            unitDataTable.Columns.Add(unitIds);
            DataColumn unitJson = new DataColumn("UnitJson")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(unitJson);

            DataColumn value = new DataColumn("value")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(value);

            DataColumn unitJsonData = new DataColumn("unitJsonData")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(unitJsonData);
            if (unitVariableAssignment != null)
            {
                DataRow unitTableRow = unitDataTable.NewRow();
                unitTableRow[0] = unitId;
                unitTableRow[1] = unitVariableAssignment.VariableId;
                unitTableRow[2] = unitVariableAssignment.Value;
                unitTableRow[3] = JsonConvert.SerializeObject(unitVariableAssignment);
                unitDataTable.Rows.Add(unitTableRow);
            }
            return unitDataTable;
        }

        public static DataTable GenerateEntranceConsoleDataTable(EntranceConfigurationData entranceConfiguration)
        {
            DataTable entranceConsoleDataTable = new DataTable();
            entranceConsoleDataTable.Clear();

            DataColumn consoleId = new DataColumn("ConsoleId") { DataType = typeof(int) };
            entranceConsoleDataTable.Columns.Add(consoleId);

            DataColumn consoleName = new DataColumn("ConsoleName") { DataType = typeof(string) };
            entranceConsoleDataTable.Columns.Add(consoleName);

            DataColumn IsController = new DataColumn("IsController") { DataType = typeof(bool) };
            entranceConsoleDataTable.Columns.Add(IsController);

            DataRow unitTableRow = entranceConsoleDataTable.NewRow();
            unitTableRow[0] = entranceConfiguration.EntranceConsoleId;
            unitTableRow[1] = entranceConfiguration.ConsoleName;
            unitTableRow[2] = entranceConfiguration.IsController;

            entranceConsoleDataTable.Rows.Add(unitTableRow);
            return entranceConsoleDataTable;
        }

        public static DataTable GenerateUnitHallFixtureConsoleDataTable(UnitHallFixtureData UnitHallFixtureConfiguration)
        {
            DataTable entranceConsoleDataTable = new DataTable();
            entranceConsoleDataTable.Clear();

            DataColumn consoleId = new DataColumn("ConsoleId") { DataType = typeof(int) };
            entranceConsoleDataTable.Columns.Add(consoleId);

            DataColumn consoleName = new DataColumn("ConsoleName") { DataType = typeof(string) };
            entranceConsoleDataTable.Columns.Add(consoleName);

            DataColumn IsController = new DataColumn("IsController") { DataType = typeof(bool) };
            entranceConsoleDataTable.Columns.Add(IsController);

            DataColumn fixtureType = new DataColumn("FixtureType") { DataType = typeof(string) };
            entranceConsoleDataTable.Columns.Add(fixtureType);

            DataRow unitTableRow = entranceConsoleDataTable.NewRow();
            unitTableRow[0] = UnitHallFixtureConfiguration.ConsoleId;
            unitTableRow[1] = UnitHallFixtureConfiguration.ConsoleName;
            unitTableRow[2] = UnitHallFixtureConfiguration.IsController;
            unitTableRow[3] = UnitHallFixtureConfiguration.FixtureType;

            entranceConsoleDataTable.Rows.Add(unitTableRow);
            return entranceConsoleDataTable;
        }

        public static DataTable GenerateEntranceConfigurationDataTable(List<ConfigVariable> entranceConfiguration, int ConsoleId)
        {
            DataTable entranceConfigurationDataTable = new DataTable();
            entranceConfigurationDataTable.Clear();


            DataColumn entranceConsoleId = new DataColumn("EntranceConsoleId")
            {
                DataType = typeof(int)
            };
            entranceConfigurationDataTable.Columns.Add(entranceConsoleId);

            DataColumn variableType = new DataColumn("VariableType")
            {
                DataType = typeof(string)
            };
            entranceConfigurationDataTable.Columns.Add(variableType);

            DataColumn variableValue = new DataColumn("VariableValue")
            {
                DataType = typeof(string)
            };
            entranceConfigurationDataTable.Columns.Add(variableValue);


            foreach (var entrance in entranceConfiguration)
            {
                DataRow unitTableRow = entranceConfigurationDataTable.NewRow();
                //var variableId = entrance.VariableId.Split(Constant.ENTRANCECONFIGURATIONVARIABLEIDSPLIT);
                unitTableRow[0] = ConsoleId;
                unitTableRow[1] = entrance.VariableId;//variableId[1];//entrance.VariableId.Replace("ELEVATOR.Parameters.Doors.Landing_Doors_Assembly.", "");
                if (entrance.Value.Equals(true))
                {
                    entrance.Value = "TRUE";
                }
                else if (entrance.Value.Equals(false))
                {
                    entrance.Value = "FALSE";
                }
                unitTableRow[2] = entrance.Value;
                entranceConfigurationDataTable.Rows.Add(unitTableRow);

            }
            return entranceConfigurationDataTable;
        }

        public static DataTable GenerateEntranceLocationDataTable(List<EntranceLocation> entranceConfiguration, int ConsoleId)
        {
            DataTable entranceLocationDataTable = new DataTable();
            entranceLocationDataTable.Clear();

            DataColumn entranceConsoleId = new DataColumn("EntranceConsoleId")
            {
                DataType = typeof(int)
            };
            entranceLocationDataTable.Columns.Add(entranceConsoleId);

            DataColumn floorNumber = new DataColumn("FloorNumber")
            {
                DataType = typeof(int)
            };
            entranceLocationDataTable.Columns.Add(floorNumber);

            DataColumn front = new DataColumn("Front") { DataType = typeof(bool) };
            entranceLocationDataTable.Columns.Add(front);

            DataColumn rear = new DataColumn("Rear") { DataType = typeof(bool) };
            entranceLocationDataTable.Columns.Add(rear);

            foreach (var entrance in entranceConfiguration)
            {
                DataRow unitTableRow = entranceLocationDataTable.NewRow();
                unitTableRow[0] = ConsoleId;
                unitTableRow[1] = entrance.FloorNumber;
                unitTableRow[2] = entrance.Front;
                unitTableRow[3] = entrance.Rear;
                entranceLocationDataTable.Rows.Add(unitTableRow);
            }
            return entranceLocationDataTable;
        }

        public static DataTable GenerateUnitHallFixtureLocationLocationDataTable(List<UnitHallFixtureLocation> entranceConfiguration, int ConsoleId)
        {
            DataTable entranceLocationDataTable = new DataTable();
            entranceLocationDataTable.Clear();

            DataColumn entranceConsoleId = new DataColumn("EntranceConsoleId")
            {
                DataType = typeof(int)
            };
            entranceLocationDataTable.Columns.Add(entranceConsoleId);


            DataColumn floorNumber = new DataColumn("FloorNumber")
            {
                DataType = typeof(int)
            };
            entranceLocationDataTable.Columns.Add(floorNumber);

            DataColumn front = new DataColumn("Front") { DataType = typeof(bool) };
            entranceLocationDataTable.Columns.Add(front);


            DataColumn rear = new DataColumn("Rear") { DataType = typeof(bool) };
            entranceLocationDataTable.Columns.Add(rear);


            foreach (var entrance in entranceConfiguration)
            {
                DataRow unitTableRow = entranceLocationDataTable.NewRow();
                unitTableRow[0] = ConsoleId;
                unitTableRow[1] = entrance.FloorNumber;
                unitTableRow[2] = entrance.Front;
                unitTableRow[3] = entrance.Rear;
                entranceLocationDataTable.Rows.Add(unitTableRow);
            }
            return entranceLocationDataTable;
        }
        public static DataTable GenerateGroupHallFixtureConsoleDataTable(GroupHallFixturesData groupHallFixtures)
        {
            DataTable groupHallFixtureConsoleInfoDataTable = new DataTable();
            groupHallFixtureConsoleInfoDataTable.Clear();
            DataColumn consoleId = new DataColumn("ConsoleId") { DataType = typeof(int) };
            groupHallFixtureConsoleInfoDataTable.Columns.Add(consoleId);
            DataColumn consoleName = new DataColumn("ConsoleName") { DataType = typeof(string) };
            groupHallFixtureConsoleInfoDataTable.Columns.Add(consoleName);
            DataColumn IsController = new DataColumn("IsController") { DataType = typeof(bool) };
            groupHallFixtureConsoleInfoDataTable.Columns.Add(IsController);
            DataColumn FixtureType = new DataColumn("FixtureType") { DataType = typeof(string) };
            groupHallFixtureConsoleInfoDataTable.Columns.Add(FixtureType);

            DataRow unitTableRow = groupHallFixtureConsoleInfoDataTable.NewRow();
            unitTableRow[0] = groupHallFixtures.GroupHallFixtureConsoleId;
            unitTableRow[1] = groupHallFixtures.ConsoleName;
            unitTableRow[2] = groupHallFixtures.IsController;
            unitTableRow[3] = groupHallFixtures.FixtureType;

            groupHallFixtureConsoleInfoDataTable.Rows.Add(unitTableRow);
            return groupHallFixtureConsoleInfoDataTable;
        }

        public static DataTable GenerateHallLanternConfigurationDataTable(List<ConfigVariable> entranceConfiguration, int ConsoleId)
        {
            DataTable entranceConfigurationDataTable = new DataTable();
            entranceConfigurationDataTable.Clear();


            DataColumn entranceConsoleId = new DataColumn("EntranceConsoleId")
            {
                DataType = typeof(int)
            };
            entranceConfigurationDataTable.Columns.Add(entranceConsoleId);

            DataColumn variableType = new DataColumn("VariableType")
            {
                DataType = typeof(string)
            };
            entranceConfigurationDataTable.Columns.Add(variableType);

            DataColumn variableValue = new DataColumn("VariableValue")
            {
                DataType = typeof(string)
            };
            entranceConfigurationDataTable.Columns.Add(variableValue);


            foreach (var entrance in entranceConfiguration)
            {
                DataRow unitTableRow = entranceConfigurationDataTable.NewRow();
                //var variableId = entrance.VariableId.Contains(Constant.HALLLANTERNVARIABLEIDSPLIT) ? entrance.VariableId.Split(Constant.HALLLANTERNVARIABLEIDSPLIT):
                //    entrance.VariableId.Split(Constant.HALLLANTERNVARIABLESIDSPLITLOP);
                var variableId = entrance.VariableId;
                unitTableRow[0] = ConsoleId;
                unitTableRow[1] = variableId;//entrance.VariableId.Replace("ELEVATOR.Parameters.Doors.Landing_Doors_Assembly.", "");
                unitTableRow[2] = entrance.Value.ToString();
                entranceConfigurationDataTable.Rows.Add(unitTableRow);



            }
            return entranceConfigurationDataTable;
        }


        public static DataTable GenerateGroupHallFixtureLocationDataTable(List<GroupHallFixtureLocation> groupHallFixtureLocation, int ConsoleId, bool is_HallStation)
        {
            DataTable groupHallFixtureLocationDataTable = new DataTable();
            groupHallFixtureLocationDataTable.Clear();

            DataColumn grouphallFixtureConsoleId = new DataColumn("GroupHallFixtureConsoleId")
            {
                DataType = typeof(string)
            };
            groupHallFixtureLocationDataTable.Columns.Add(grouphallFixtureConsoleId);

            DataColumn unitId = new DataColumn("UnitId")
            {
                DataType = typeof(int)
            };
            groupHallFixtureLocationDataTable.Columns.Add(unitId);
            DataColumn floorDesignation = new DataColumn("FloorDesignation")
            {
                DataType = typeof(string)
            };
            groupHallFixtureLocationDataTable.Columns.Add(floorDesignation);

            DataColumn front = new DataColumn("Front") { DataType = typeof(bool) };
            groupHallFixtureLocationDataTable.Columns.Add(front);

            DataColumn rear = new DataColumn("Rear") { DataType = typeof(bool) };
            groupHallFixtureLocationDataTable.Columns.Add(rear);

            DataColumn hallStationName = new DataColumn("HallStationName") { DataType = typeof(string) };
            groupHallFixtureLocationDataTable.Columns.Add(hallStationName);


            foreach (var fixtureLocation in groupHallFixtureLocation)
            {
                foreach (var opening in fixtureLocation.Assignments)
                {
                    DataRow unitTableRow = groupHallFixtureLocationDataTable.NewRow();
                    unitTableRow[0] = ConsoleId;
                    unitTableRow[1] = 0;
                    unitTableRow[2] = opening.FloorDesignation;
                    unitTableRow[3] = opening.Front;
                    unitTableRow[4] = opening.Rear;
                    unitTableRow[5] = "Parameters_SP." + fixtureLocation.HallStationName + "_SP";
                    groupHallFixtureLocationDataTable.Rows.Add(unitTableRow);
                }

            }


            return groupHallFixtureLocationDataTable;
        }

        /// <summary>
        /// Generate DataTable for update Opening location
        /// </summary>
        /// <param Name="openingLocation"></param>
        /// <returns></returns>



        /// <summary>
        /// Utility method to return list of sqlparameter for deleting group details
        /// </summary>
        /// <param Name="GroupId"></param>
        /// <returns></returns>
        public static List<SqlParameter> SqlParameterForDeleteGroup(int GroupId)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@GroupConfigId",Value=GroupId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int},
                new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
            };
            return lstSqlParameter;
        }

        public static List<SqlParameter> SqlParameterForSavingGroupLayout(int groupId, DataTable unitVariableAssignment,
            DataTable unitDataTableForHallRiser, DataTable unitDataTableForDoor, string userName, DataTable unitDataTableForControlLocation
            , ConflictsStatus isEditFlow, string fdaJsonData, string sectionTab)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@GroupConfigurationId",Value=groupId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@UnitVariables",Value=unitVariableAssignment,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@UnitVariablesHallRiser",Value=unitDataTableForHallRiser,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@UnitVariablesDoor",Value=unitDataTableForDoor,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@UnitVariableControlLocation",Value=unitDataTableForControlLocation,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userName,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@IsEditFlows",Value=isEditFlow,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@fdaJsonData",Value=fdaJsonData,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@sectionTab", Value = sectionTab, Direction = ParameterDirection.Input, SqlDbType = SqlDbType.NVarChar }
               ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
            };
            return lstSqlParameter;
        }


        public static DataTable CreateDataTableForUpdateOpeningLocation(OpeningLocations openingLocation)
        {
            DataTable openingLocationDataTable = new DataTable();
            openingLocationDataTable.Clear();


            DataColumn groupConfigurationId = new DataColumn("GroupConfigurationId")
            {
                DataType = typeof(int),
                DefaultValue = openingLocation.GroupConfigurationId
            };
            openingLocationDataTable.Columns.Add(groupConfigurationId);

            DataColumn unitId = new DataColumn("UnitId")
            {
                DataType = typeof(int)
            };
            openingLocationDataTable.Columns.Add(unitId);

            DataColumn travelFeet = new DataColumn("Travelfeet")
            {
                DataType = typeof(int)
            };
            openingLocationDataTable.Columns.Add(travelFeet);

            DataColumn travelInch = new DataColumn("TravelInch")
            {
                DataType = typeof(decimal)
            };
            openingLocationDataTable.Columns.Add(travelInch);

            DataColumn ocuppiedSpaceBelow = new DataColumn("OcuppiedSpaceBelow")
            {
                DataType = typeof(Boolean)
            };
            openingLocationDataTable.Columns.Add(ocuppiedSpaceBelow);

            DataColumn noOfFloors = new DataColumn("NoOfFloors")
            {
                DataType = typeof(int)
            };
            openingLocationDataTable.Columns.Add(noOfFloors);

            DataColumn frontOpening = new DataColumn("FrontOpening")
            {
                DataType = typeof(int)
            };
            openingLocationDataTable.Columns.Add(frontOpening);

            DataColumn rearOpening = new DataColumn("RearOpening")
            {
                DataType = typeof(int)
            };
            openingLocationDataTable.Columns.Add(rearOpening);

            DataColumn SideOpening = new DataColumn("SideOpening")
            {
                DataType = typeof(int)
            };
            openingLocationDataTable.Columns.Add(SideOpening);

            DataColumn floorDesignation = new DataColumn("FloorDesignation")
            {
                DataType = typeof(string)
            };
            openingLocationDataTable.Columns.Add(floorDesignation);

            DataColumn floorNumber = new DataColumn("FloorNumber")
            {
                DataType = typeof(int)
            };
            openingLocationDataTable.Columns.Add(floorNumber);

            DataColumn front = new DataColumn("Front")
            {
                DataType = typeof(Boolean)
            };
            openingLocationDataTable.Columns.Add(front);

            DataColumn side = new DataColumn("Side")
            {
                DataType = typeof(Boolean)
            };
            openingLocationDataTable.Columns.Add(side);

            DataColumn rear = new DataColumn("Rear")
            {
                DataType = typeof(Boolean)
            };
            openingLocationDataTable.Columns.Add(rear);

            DataColumn userName = new DataColumn("UserName")
            {
                DataType = typeof(string),
                DefaultValue = openingLocation.UserName
            };
            openingLocationDataTable.Columns.Add(userName);

            foreach (var unit in openingLocation.Units)
            {
                foreach (var openingAssigned in unit.OpeningsAssigned)
                {
                    DataRow openingLocationUnit = openingLocationDataTable.NewRow();
                    openingLocationUnit[1] = unit.UnitId;
                    openingLocationUnit[2] = unit.Travel.feet;
                    openingLocationUnit[3] = unit.Travel.inch;
                    openingLocationUnit[4] = unit.OcuppiedSpace;
                    openingLocationUnit[5] = unit.NoOfFloors;
                    openingLocationUnit[6] = unit.FrontOpening;
                    openingLocationUnit[7] = unit.RearOpening;
                    openingLocationUnit[8] = unit.SideOpening;
                    openingLocationUnit[9] = openingAssigned.FloorDesignation;
                    openingLocationUnit[10] = openingAssigned.FloorNumber;
                    openingLocationUnit[11] = openingAssigned.Front;
                    openingLocationUnit[12] = openingAssigned.Side;
                    openingLocationUnit[13] = openingAssigned.Rear;
                    openingLocationDataTable.Rows.Add(openingLocationUnit);
                }

            }

            return openingLocationDataTable;
        }


        public static DataTable GenerateDataTableForSaveGeneralInformation(List<ConfigVariable> unitVariableAssignment)
        {
            DataTable unitDataTable = new DataTable();
            unitDataTable.Clear();



            DataColumn unitJson = new DataColumn("UnitJson")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(unitJson);



            DataColumn value = new DataColumn("value")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(value);



            DataColumn unitJsonData = new DataColumn("unitJsonData")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(unitJsonData);



            DataColumn unitName = new DataColumn("UnitName");
            unitJsonData.DataType = typeof(string);
            unitDataTable.Columns.Add(unitName);



            foreach (var unit in unitVariableAssignment)
            {
                DataRow unitTableRow = unitDataTable.NewRow();
                unitTableRow[0] = unit.VariableId;
                unitTableRow[1] = unit.Value;
                unitTableRow[2] = JsonConvert.SerializeObject(unit);
                unitDataTable.Rows.Add(unitTableRow);



            }
            return unitDataTable;
        }


        public static List<SqlParameter> SqlParameterForSavingGeneralInformation(int groupId, string productName, DataTable GeneralInformationDataTable, string userId)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@GroupConfigurationId",Value=groupId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@ProductName",Value=productName,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@UnitVariablesGeneralInformation",Value=GeneralInformationDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
            };
            return lstSqlParameter;
        }

        public static List<SqlParameter> SqlParameterForSaveUnitConfiguration(int setId, DataTable unitVariableAssignment, string userId, ConflictsStatus isEditFlow, DataTable historyTable, DataTable dataTableWiring)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@SetId",Value=setId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@UnitVariables",Value=unitVariableAssignment,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@historyTable",Value=historyTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@IsEditFlows",Value=Convert.ToString(isEditFlow),Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@DataTableWiring",Value=dataTableWiring,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
            };
            return lstSqlParameter;
        }


        public static List<SqlParameter> SqlParameterForNonConfigurableSaveUnitConfiguration(int setId, DataTable unitVariableAssignment, string userId, DataTable configVariables)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@SetId",Value=setId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@UnitVariables",Value=unitVariableAssignment,Direction = ParameterDirection.Input}
                ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
                ,new SqlParameter(){ ParameterName = Constant.CONSTANTMAPPERLIST,Value=configVariables }
                ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
            };
            return lstSqlParameter;
        }

        public static List<SqlParameter> SqlParameterForSaveEntranceConfiguration(int setId, int ConsoleNumber, DataTable entranceConsoleDataTable, DataTable entranceConfigurationDataTable, DataTable entranceLocationDataTable, string userId, bool isReset, DataTable historyTable)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@SetId",Value=setId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@ConsoleNumber",Value=ConsoleNumber,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@EntranceConsoleVariables",Value=entranceConsoleDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@EntranceConfigurationVariables",Value=entranceConfigurationDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@EntranceLocationVariables",Value=entranceLocationDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@isReset",Value=isReset,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@historyTable",Value=historyTable,Direction = ParameterDirection.Input}
            };
            return lstSqlParameter;
        }

        public static List<SqlParameter> SqlParameterForSaveHallLanternConfiguration(int setId, int ConsoleNumber, DataTable entranceConsoleDataTable, DataTable entranceConfigurationDataTable, DataTable entranceLocationDataTable, string userId, DataTable historyTable)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@SetId",Value=setId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@ConsoleNumber",Value=ConsoleNumber,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@UnitHallFixtureConsoleVariables",Value=entranceConsoleDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@UnitHallFixtureConfigurationVariables",Value=entranceConfigurationDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@UnitHallFixtureLocationVariables",Value=entranceLocationDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@historyTable",Value=historyTable,Direction = ParameterDirection.Input}
            };
            return lstSqlParameter;
        }

        public static List<SqlParameter> SqlParameterForSaveCabInteriorDetails(int groupId, string productName, DataTable unitVariableAssignment, string userId)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@GroupConfigurationId",Value=groupId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@ProductName",Value=productName,Direction = ParameterDirection.Input, SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@UnitVariables",Value=unitVariableAssignment,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
            };
            return lstSqlParameter;
        }

        public static List<SqlParameter> SqlParameterForEditUnitdesignation(int groupId, int unitId, string userId, UnitDesignation unit)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@GroupConfigurationId",Value=groupId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@UnitId",Value=unitId,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@UserName",Value=userId,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@Designation",Value=unit.Designation,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@Description",Value=unit.Description,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
            };
            return lstSqlParameter;
        }

        public static List<SqlParameter> SqlParameterForSavingGeneralInformation(int groupId, int unitId, DataTable GeneralInformationDataTable, string userId)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@GroupConfigurationId",Value=groupId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@UnitId",Value=unitId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@UnitVariablesGeneralInformation",Value=GeneralInformationDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
            };
            return lstSqlParameter;
        }

        public static List<SqlParameter> SqlParameterForSaveGroupHallFixture(int groupId, int ConsoleNumber, DataTable groupHallFixtureConsoleInfoDataTable, DataTable entranceConfigurationDataTable, DataTable groupHallFixtureLocationDataTable, string userId, DataTable historyTable)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@GroupId",Value=groupId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@ConsoleNumber",Value=ConsoleNumber,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@GroupHallFixtureConsoleVariables",Value=groupHallFixtureConsoleInfoDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@GroupHallFixtureConfigurationVariables",Value=entranceConfigurationDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@GroupHallFixtureLocationVariables",Value=groupHallFixtureLocationDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@historyTable",Value=historyTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
            };
            return lstSqlParameter;
        }

        public static DataTable GenerateDataTableForAssignGroupsConfiguration(List<ConfigVariable> entranceConfiguration, int consoleId)
        {
            DataTable entranceConfigurationDataTable = new DataTable();
            entranceConfigurationDataTable.Clear();


            DataColumn entranceConsoleId = new DataColumn("EntranceConsoleId")
            {
                DataType = typeof(int)
            };
            entranceConfigurationDataTable.Columns.Add(entranceConsoleId);

            DataColumn variableType = new DataColumn("VariableType")
            {
                DataType = typeof(string)
            };
            entranceConfigurationDataTable.Columns.Add(variableType);

            DataColumn variableValue = new DataColumn("VariableValue")
            {
                DataType = typeof(string)
            };
            entranceConfigurationDataTable.Columns.Add(variableValue);


            foreach (var entrance in entranceConfiguration)
            {
                DataRow unitTableRow = entranceConfigurationDataTable.NewRow();
                //var variableId = entrance.VariableId.Split(Constant.ENTRANCECONFIGURATIONVARIABLEIDSPLIT);
                unitTableRow[0] = consoleId;
                unitTableRow[1] = entrance.VariableId;//variableId[1];//entrance.VariableId.Replace("ELEVATOR.Parameters.Doors.Landing_Doors_Assembly.", "");
                if (entrance.Value.Equals(true))
                {
                    entrance.Value = "True";
                }
                else if (entrance.Value.Equals(false))
                {
                    entrance.Value = "False";
                }
                unitTableRow[2] = entrance.Value;
                entranceConfigurationDataTable.Rows.Add(unitTableRow);

            }
            return entranceConfigurationDataTable;
        }

        public static DataTable GenerateDataTableForAssignGroups(BuildingEquipmentData buildingEquipmentConfiguration)
        {
            DataTable entranceConsoleDataTable = new DataTable();
            entranceConsoleDataTable.Clear();

            DataColumn consoleId = new DataColumn("ConsoleId") { DataType = typeof(int) };
            entranceConsoleDataTable.Columns.Add(consoleId);

            DataColumn consoleName = new DataColumn("ConsoleName") { DataType = typeof(string) };
            entranceConsoleDataTable.Columns.Add(consoleName);

            DataColumn IsController = new DataColumn("IsController") { DataType = typeof(bool) };
            entranceConsoleDataTable.Columns.Add(IsController);

            DataColumn IsLobbyPanel = new DataColumn("IsLobbyPanel") { DataType = typeof(bool) };
            entranceConsoleDataTable.Columns.Add(IsLobbyPanel);

            DataRow unitTableRow = entranceConsoleDataTable.NewRow();
            unitTableRow[0] = buildingEquipmentConfiguration.ConsoleId;
            unitTableRow[1] = buildingEquipmentConfiguration.ConsoleName;
            unitTableRow[2] = buildingEquipmentConfiguration.IsController;
            unitTableRow[3] = buildingEquipmentConfiguration.IsLobbyPanel;

            entranceConsoleDataTable.Rows.Add(unitTableRow);
            return entranceConsoleDataTable;
        }

        public static DataTable GenerateDataTableForAssignedGroups(List<ConfiguredGroups> ConfiguredGroups, int consoleId)
        {
            DataTable assignedGroupsTable = new DataTable();
            assignedGroupsTable.Clear();


            DataColumn ConsoleId = new DataColumn("ConsoleId")
            {
                DataType = typeof(int)
            };
            assignedGroupsTable.Columns.Add(ConsoleId);

            DataColumn groupId = new DataColumn("GroupId")
            {
                DataType = typeof(string)
            };
            assignedGroupsTable.Columns.Add(groupId);

            DataColumn groupName = new DataColumn("GroupName")
            {
                DataType = typeof(string)
            };
            assignedGroupsTable.Columns.Add(groupName);

            DataColumn isChecked = new DataColumn("IsChecked")
            {
                DataType = typeof(Boolean)
            };
            assignedGroupsTable.Columns.Add(isChecked);

            if (ConfiguredGroups != null)
            {
                foreach (var group in ConfiguredGroups)
                {
                    DataRow unitTableRow = assignedGroupsTable.NewRow();
                    //var variableId = entrance.VariableId.Split(Constant.ENTRANCECONFIGURATIONVARIABLEIDSPLIT);
                    unitTableRow[0] = consoleId;

                    unitTableRow[1] = group.groupId;
                    unitTableRow[2] = group.groupName;
                    if (group.isChecked.Equals(true))
                    {
                        unitTableRow[3] = 1;
                    }
                    else if (group.isChecked.Equals(false))
                    {
                        unitTableRow[3] = 0;
                    }


                    assignedGroupsTable.Rows.Add(unitTableRow);

                }
            }
            return assignedGroupsTable;
        }

        public static DataTable GenerateDataTableForExistingGroups(List<BuildingEquipmentGroupDetails> ConfiguredGroups, int consoleId)
        {
            DataTable assignedGroupsTable = new DataTable();
            assignedGroupsTable.Clear();


            DataColumn ConsoleId = new DataColumn("ConsoleId")
            {
                DataType = typeof(int)
            };
            assignedGroupsTable.Columns.Add(ConsoleId);

            DataColumn groupCategoryId = new DataColumn("GroupCategoryId")
            {
                DataType = typeof(int)
            };
            assignedGroupsTable.Columns.Add(groupCategoryId);

            DataColumn groupName = new DataColumn("GroupName")
            {
                DataType = typeof(string)
            };
            assignedGroupsTable.Columns.Add(groupName);

            DataColumn noOfUnits = new DataColumn("NoOfUnits")
            {
                DataType = typeof(string)
            };
            assignedGroupsTable.Columns.Add(noOfUnits);

            DataColumn factoryId = new DataColumn("GroupFactoryId")
            {
                DataType = typeof(string)
            };
            assignedGroupsTable.Columns.Add(factoryId);

            if (ConfiguredGroups != null)
            {
                foreach (var group in ConfiguredGroups)
                {


                    DataRow unitTableRow = assignedGroupsTable.NewRow();
                    unitTableRow[0] = consoleId;
                    unitTableRow[1] = 1;
                    unitTableRow[2] = group.groupName;
                    unitTableRow[3] = group.noOfUnits;
                    unitTableRow[4] = group.groupFactoryId;

                    assignedGroupsTable.Rows.Add(unitTableRow);


                }
            }

            return assignedGroupsTable;
        }

        public static DataTable GenerateDataTableForFutureGroups(List<BuildingEquipmentGroupDetails> ConfiguredGroups, int consoleId)
        {
            DataTable assignedGroupsTable = new DataTable();
            assignedGroupsTable.Clear();



            DataColumn ConsoleId = new DataColumn("ConsoleId")
            {
                DataType = typeof(int)
            };
            assignedGroupsTable.Columns.Add(ConsoleId);

            DataColumn groupCategoryId = new DataColumn("GroupCategoryId")
            {
                DataType = typeof(int)
            };
            assignedGroupsTable.Columns.Add(groupCategoryId);

            DataColumn groupName = new DataColumn("GroupName")
            {
                DataType = typeof(string)
            };
            assignedGroupsTable.Columns.Add(groupName);

            DataColumn noOfUnits = new DataColumn("NoOfUnits")
            {
                DataType = typeof(string)
            };
            assignedGroupsTable.Columns.Add(noOfUnits);

            if (ConfiguredGroups != null)
            {
                foreach (var group in ConfiguredGroups)
                {
                    DataRow unitTableRow = assignedGroupsTable.NewRow();
                    //var variableId = entrance.VariableId.Split(Constant.ENTRANCECONFIGURATIONVARIABLEIDSPLIT);

                    unitTableRow[0] = consoleId;
                    unitTableRow[1] = 2;
                    unitTableRow[2] = group.groupName;
                    unitTableRow[3] = group.noOfUnits;


                    assignedGroupsTable.Rows.Add(unitTableRow);

                }
            }

            return assignedGroupsTable;
        }

        public static List<SqlParameter> SqlParameterForSaveAssignedGroupsConfiguration(int buildingId, int consoleId, DataTable consoleDataTable,
            DataTable consoleConfigurationDataTable, DataTable assignedGroupsDataTable, DataTable existingGroupsDataTable, DataTable futureGroupsDataTable, string userId,
            DataTable logHistoryTable)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@BuildingId",Value=buildingId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@ConsoleNumber",Value=consoleId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@ConsoleVariables",Value=consoleDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@ConsoleConfigurationVariables",Value=consoleConfigurationDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@AssignedGroupsDataTable",Value=assignedGroupsDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@ExistingGroupsDataTable",Value=existingGroupsDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@FutureGroupsDataTable",Value=futureGroupsDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@LogHistoryTable",Value=logHistoryTable,Direction=ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
            };
            return lstSqlParameter;
        }

        public static List<SqlParameter> SqlParameterForSaveBuildingEquipmentConfiguration(int buildingId, DataTable configurationDataTable, string userId)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                 new SqlParameter() {ParameterName = "@BuildngId", Value=buildingId, Direction = ParameterDirection.Input, SqlDbType = SqlDbType.Int}
                ,new SqlParameter() {ParameterName = "@buildingVariables", Value = configurationDataTable, Direction = ParameterDirection.Input}
                ,new SqlParameter() {ParameterName = "@CreatedBy", Value = userId, Direction = ParameterDirection.Input, SqlDbType = SqlDbType.NVarChar}
                ,new SqlParameter() {ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int }
            };
            return lstSqlParameter;
        }

        public static DataTable GenerateDataTableForFutureElevatorUnitTable(List<FutureElevatorrVariable> futureElevatorVariableAssignment)
        {
            DataTable unitDataTableForFutureElevator = new DataTable();
            unitDataTableForFutureElevator.Clear();


            DataColumn unitJson = new DataColumn("UnitJson")
            {
                DataType = typeof(string)
            };
            unitDataTableForFutureElevator.Columns.Add(unitJson);

            DataColumn IsFutureElevator = new DataColumn("IsFutureElevator")
            {
                DataType = typeof(string)
            };
            unitDataTableForFutureElevator.Columns.Add(IsFutureElevator);

            DataColumn unitJsonData = new DataColumn("unitJsonData")
            {
                DataType = typeof(string)
            };
            unitDataTableForFutureElevator.Columns.Add(unitJsonData);

            foreach (var unit in futureElevatorVariableAssignment)
            {

                DataRow unitTableRow = unitDataTableForFutureElevator.NewRow();
                unitTableRow[0] = unit.VariableId;
                if (unit.IsFutureElevator.Equals(true))
                {
                    unit.IsFutureElevator = "True";
                }
                else if (unit.IsFutureElevator.Equals(false))
                {
                    unit.IsFutureElevator = "False";
                }
                unitTableRow[1] = unit.IsFutureElevator;
                unitTableRow[2] = JsonConvert.SerializeObject(unit);
                unitDataTableForFutureElevator.Rows.Add(unitTableRow);
            }
            return unitDataTableForFutureElevator;
        }

        public static List<SqlParameter> SqlParameterForSaveCarCallCutoutOpenings(int setId, int ConsoleNumber, DataTable entranceLocationDataTable, string userId, DataTable historyTable)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@SetId",Value=setId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@ConsoleNumber",Value=ConsoleNumber,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@EntranceLocationVariables",Value=entranceLocationDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@historyTable",Value=historyTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
            };
            return lstSqlParameter;
        }
        public static DataTable GenerateDataTableForHistoryTable(List<LogHistoryTable> historyTable)
        {
            DataTable historyDataTable = new DataTable();
            historyDataTable.Clear();


            DataColumn variableId = new DataColumn("VariableId")
            {
                DataType = typeof(string)
            };
            historyDataTable.Columns.Add(variableId);

            DataColumn updatedValue = new DataColumn("UpdatedValue")
            {
                DataType = typeof(string)
            };
            historyDataTable.Columns.Add(updatedValue);

            DataColumn previousValue = new DataColumn("PreviousValue")
            {
                DataType = typeof(string)
            };
            historyDataTable.Columns.Add(previousValue);

            foreach (var history in historyTable)
            {

                DataRow unitTableRow = historyDataTable.NewRow();
                unitTableRow[0] = history.VariableId;
                unitTableRow[1] = history.UpdatedValue;
                unitTableRow[2] = history.PreviuosValue;
                historyDataTable.Rows.Add(unitTableRow);
            }
            return historyDataTable;
        }
        /// <summary>
        /// To get Basic auth tocken 
        /// </summary>
        /// <param Name="viewUserName"></param>
        /// <param Name="viewPassword"></param>
        /// <returns></returns>
        public static string GenerateBasicAuthTocken(string viewUserName, string viewPassword)
        {
            var authTocken = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding(Constant.ISOCODE).GetBytes(viewUserName + Constant.COLON + viewPassword));
            return authTocken;
        }

        /// <summary>
        /// generate data table for export view
        /// </summary>
        /// <param Name="ExportVariables"></param>
        /// <returns></returns>
        public static DataTable GenerateDataTableForExportView(List<string> ExportVariables)
        {

            DataTable buildingDataTable = new DataTable();
            buildingDataTable.Clear();
            DataColumn VariableId = new DataColumn("VariableId")
            {
                DataType = typeof(string)
            };
            buildingDataTable.Columns.Add(VariableId);

            foreach (var var in ExportVariables)
            {

                DataRow TableRow = buildingDataTable.NewRow();
                TableRow[0] = var;
                buildingDataTable.Rows.Add(TableRow);
            }

            return buildingDataTable;
        }
        public static List<SqlParameter> SqlParameterForExportView(string opportunityId, DataTable buildingDataTable,
           DataTable buildinEqmntConsoleDataTable, DataTable buildinEqmntConfigureDataTable, DataTable ControlLocationDataTable
           , DataTable UnitConfigurationDataTable)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@QuoteId",Value=opportunityId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@buildingVariables",Value=buildingDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@buildingEqmntVariables",Value=buildinEqmntConsoleDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@buildingEqmntConsoleVariables",Value=buildinEqmntConfigureDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@controlLocationVariables",Value=ControlLocationDataTable,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@unitConfigurationVariables",Value=UnitConfigurationDataTable,Direction = ParameterDirection.Input}

            };
            return lstSqlParameter;
        }
        /// <summary>
        /// GetSqlParametersForMiniProject
        /// </summary>
        /// <param Name="userName"></param>
        /// <param Name="isEditFlow"></param>
        /// <returns></returns>
        public static List<SqlParameter> GetSqlParametersForMiniProject(string userName, string projectId, int versionId = 1)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
               new SqlParameter() { ParameterName = "@CreatedBy",Value=userName,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = Constant.PROJECTIDVALUES,Value=projectId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter(){ ParameterName = Constant.VERSIONIDVALUE,Value = versionId,Direction =ParameterDirection.Input,SqlDbType=SqlDbType.Int}
            };
            return lstSqlParameter;
        }

        /// <summary>
        /// SqlParameterForSavinAndUpdateMiniProject
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="unitVariableAssignment"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        public static List<SqlParameter> SqlParameterForSavinAndUpdateMiniProject(DataTable unitVariableAssignment, string userName, DataTable accountVariableAssignment)
        {
            var quoteCreationFlag = 1;
            if (unitVariableAssignment.Rows[0][0] != "")
            {
                quoteCreationFlag = 0;
            }
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@ProjectDetailList",Value=unitVariableAssignment,Direction = ParameterDirection.Input}
                ,new SqlParameter() { ParameterName = "@quoteCreationFlag",Value=quoteCreationFlag,Direction = ParameterDirection.Input}
                ,new SqlParameter() { ParameterName = "@ProjectsInfoTemp",Value=accountVariableAssignment,Direction = ParameterDirection.Input }
                ,new SqlParameter() { ParameterName = "@Result",Value=0 ,SqlDbType = SqlDbType.Int}
            };
            return lstSqlParameter;
        }

        public static List<SqlParameter> SqlParameterForGeneratingQuoteId(DataTable unitVariableAssignment, DataTable unitIdentificationTable, string userName)
        {
            DataTable projectDataTable = new DataTable();
            projectDataTable.Clear();
            DataColumn opportunityId = new DataColumn("OpportunityId")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(opportunityId);
            //variableId
            DataColumn Type = new DataColumn("Type")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(Type);
            //value
            DataColumn AddressLine1 = new DataColumn("AddressLine1")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(AddressLine1);
            //jsondata
            DataColumn AddressLine2 = new DataColumn("AddressLine2")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(AddressLine2);

            DataColumn City = new DataColumn("City")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(City);

            DataColumn State = new DataColumn("State")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(State);

            DataColumn Country = new DataColumn("Country")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(Country);

            DataColumn ZipCode = new DataColumn("ZipCode")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(ZipCode);

            DataColumn CustomerNumber = new DataColumn("CustomerNumber")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(CustomerNumber);
            DataColumn accountName = new DataColumn("accountName")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(accountName);
            DataColumn AwardCloseDate = new DataColumn("AwardCloseDate")
            {
                DataType = typeof(DateTime)
            };
            projectDataTable.Columns.Add(AwardCloseDate);
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@ProjectDetailList",Value=unitVariableAssignment,Direction = ParameterDirection.Input}
                ,new SqlParameter() { ParameterName = "@quoteCreationFlag",Value=1,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@ProjectsInfoTemp",Value=projectDataTable,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@factoryJobIdList",Value=unitIdentificationTable,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@Result",Value=-1}
            };
            return lstSqlParameter;
        }

        public static DataTable CoordinationData(List<SendToCoordinationData> coordinationDataTable)
        {
            DataTable DrawingDataTable = new DataTable();
            {
                DrawingDataTable.Clear();
                DataColumn GroupId = new DataColumn("GroupId")
                {
                    DataType = typeof(int)
                };
                DrawingDataTable.Columns.Add(GroupId);
                //questionsJson
                DataColumn QuestionsJson = new DataColumn("QuestionsJson")
                {
                    DataType = typeof(string)
                };
                DrawingDataTable.Columns.Add(QuestionsJson);
                foreach (var data in coordinationDataTable)
                {
                    var jsondata = JsonConvert.SerializeObject(data.questions);
                    DataRow DrawingTableRow = DrawingDataTable.NewRow();
                    DrawingTableRow[0] = data.groupId;
                    DrawingTableRow[1] = jsondata;
                    DrawingDataTable.Rows.Add(DrawingTableRow);
                }
                return DrawingDataTable;
            }
        }
        /// <summary>
        /// generate data table for Unit table
        /// </summary>
        /// <param Name="projectVariableAssignment"></param>
        /// <returns></returns>
        public static DataTable GenerateDataTableForProjectsTable(VariableDetails projectVariableAssignment, string userName)
        {
            DataTable projectDataTable = new DataTable();
            projectDataTable.Clear();
            DataColumn opportunityId = new DataColumn("OpportunityId")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(opportunityId);
            //variableId
            DataColumn Name = new DataColumn("Name")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(Name);
            //value
            DataColumn BranchValue = new DataColumn("BranchValue")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(BranchValue);
            //jsondata
            DataColumn SalesId = new DataColumn("SalesId")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(SalesId);

            DataColumn QuoteStatus = new DataColumn("QuoteStatus")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(QuoteStatus);

            DataColumn description = new DataColumn("Description")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(description);

            DataColumn BusinessLine = new DataColumn("BusinessLine")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(BusinessLine);

            DataColumn MeasuringUnit = new DataColumn("MeasuringUnit")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(MeasuringUnit);


            DataColumn Salesman = new DataColumn("Salesman")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(Salesman);

            DataColumn SalesmanActiveDirectoryID = new DataColumn("SalesmanActiveDirectoryID")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(SalesmanActiveDirectoryID);

            DataColumn country = new DataColumn("country")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(country);

            DataColumn CreatedBy = new DataColumn("CreatedBy")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(CreatedBy);

            DataColumn ProjectJson = new DataColumn("ProjectJson")
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(ProjectJson);

            DataColumn VersionId = new DataColumn("VersionId")
            {
                DataType = typeof(int)
            };
            projectDataTable.Columns.Add(VersionId);

            DataColumn isPrimaryQuote = new DataColumn(Constant.PROJECTCOLUMNPRIMARYQUOTE)
            {
                DataType = typeof(bool)
            };
            projectDataTable.Columns.Add(isPrimaryQuote);

            DataRow projectTableRow = projectDataTable.NewRow();

            projectTableRow[0] = string.Empty;
            if (!string.IsNullOrEmpty(projectVariableAssignment.ProjectId))
            {
                projectTableRow[0] = projectVariableAssignment.ProjectId;
            }
            projectTableRow[1] = projectVariableAssignment.ProjectName;
            projectTableRow[2] = projectVariableAssignment.Branch;
            projectTableRow[3] = projectVariableAssignment.SalesStage;
            // need to add the quote Id
            projectTableRow[4] = Constant.INCOMPLETEVALUES;
            projectTableRow[5] = projectVariableAssignment.Description != null ? projectVariableAssignment.Description : string.Empty;
            projectTableRow[6] = Constant.NEWINSTALLATION;
            projectTableRow[7] = projectVariableAssignment.MeasuringUnit;
            projectTableRow[8] = Constant.LEADVALUES;
            projectTableRow[9] = 10001;
            projectTableRow[10] = Constant.CANADA.ToUpper();
            projectTableRow[11] = userName;
            projectTableRow[12] = Utility.SerializeObjectValue(projectVariableAssignment);
            projectTableRow[13] = projectVariableAssignment.VersionId;
            projectTableRow[14] = false;
            projectDataTable.Rows.Add(projectTableRow);


            return projectDataTable;
        }

        /// <summary>
        /// data table for the create/update of project(datatable will store the account details against the project)
        /// </summary>
        /// <param Name="viewDetails"></param>
        /// <param Name="requestVariableDetails"></param>
        /// <returns></returns>
        public static DataTable GenerateDataTableForProjectsInfoTable(ViewProjectDetails viewDetails, VariableDetails requestVariableDetails)
        {
            VariableDetails projectVariableAssignment = new VariableDetails();
            DataTable projectDataTable = new DataTable();
            projectDataTable.Clear();
            DataColumn opportunityId = new DataColumn(Constant.CREATEPROJECTCOLUMNOPPORTUNITYID)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(opportunityId);
            //variableId
            DataColumn Type = new DataColumn(Constant.CREATEPROJECTCOLUMNTYPE)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(Type);
            //value
            DataColumn AddressLine1 = new DataColumn(Constant.CREATEPROJECTCOLUMNADDRESSLINE1)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(AddressLine1);
            //jsondata
            DataColumn AddressLine2 = new DataColumn(Constant.CREATEPROJECTCOLUMNADDRESSLINE2)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(AddressLine2);

            DataColumn City = new DataColumn(Constant.CREATEPROJECTCOLUMNCITY)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(City);

            DataColumn State = new DataColumn(Constant.CREATEPROJECTCOLUMNSTATE)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(State);

            DataColumn Country = new DataColumn(Constant.CREATEPROJECTCOLUMNCOUNTRY)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(Country);

            DataColumn ZipCode = new DataColumn(Constant.CREATEPROJECTCOLUMNZIPCODE)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(ZipCode);

            DataColumn CustomerNumber = new DataColumn(Constant.CREATEPROJECTCOLUMNCUSTOMERNUMBER)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(CustomerNumber);
            DataColumn AccountName = new DataColumn(Constant.CREATEPROJECTCOLUMNACCOUNTNAME)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(AccountName);
            DataColumn AwardCloseDate = new DataColumn(Constant.CREATEPROJECTCOLUMNAWARDCLOSEDATE)
            {
                DataType = typeof(DateTime)
            };
            projectDataTable.Columns.Add(AwardCloseDate);
            var detailsData = new List<UserAddressDetailsDataValues>();
            var data = new List<string>();
            var oppId = string.Empty;
            if (viewDetails != null)
            {
                oppId = viewDetails.Data.Quotation.OpportunityInfo.OpportunityId;

                var architectValue = viewDetails.Data.Quotation.Architect;
                var billingValue = viewDetails.Data.Quotation.Billing;
                var buildingValue = viewDetails.Data.Quotation.Building;
                //var contactValue = viewDetails.Data.Quotation.Contact;
                var ownerValue = viewDetails.Data.Quotation.Owner;
                var genContractorValue = viewDetails.Data.Quotation.GC;


                detailsData.Add(architectValue);
                detailsData.Add(billingValue);
                detailsData.Add(buildingValue);
                //detailsData.Add(contactValue);
                detailsData.Add(ownerValue);
                detailsData.Add(genContractorValue);


                string[] val = { "Architect", "Billing", "Building", "Owner", "GC" };
                data.AddRange(val);
            }
            else
            {
                if (requestVariableDetails != null)
                {
                    oppId = requestVariableDetails.ProjectId;
                    var val1 = new UserAddressDetailsDataValues()
                    {
                        AddressLine1 = requestVariableDetails.AddressLine1,
                        AddressLine2 = requestVariableDetails.AddressLine2,
                        City = requestVariableDetails.City,
                        State = requestVariableDetails.State,
                        Country = requestVariableDetails.Country,
                        ZipCode = requestVariableDetails.ZipCode,
                        CustomerNumber = requestVariableDetails.Contact,
                        AccountName = requestVariableDetails.AccountName,
                        AwardCloseDate = Convert.ToDateTime(requestVariableDetails.AwardCloseDate != null ? requestVariableDetails.AwardCloseDate : DateTime.Now)
                    };


                    detailsData.Add(val1);


                    string[] val = { "Building" };
                    data.AddRange(val);
                }
            }

            if (detailsData != null && detailsData.Any())
            {
                int i = 0;
                foreach (var item in detailsData)
                {
                    DataRow projectTableRow = projectDataTable.NewRow();

                    projectTableRow[0] = oppId;
                    projectTableRow[1] = data[i];

                    projectTableRow[2] = item.AddressLine1;
                    projectTableRow[3] = item.AddressLine2;
                    projectTableRow[4] = item.City;
                    projectTableRow[5] = item.State;
                    // need to add the quote Id
                    projectTableRow[6] = item.Country;
                    projectTableRow[7] = item.ZipCode;
                    projectTableRow[8] = item.CustomerNumber;
                    projectTableRow[9] = item.AccountName;
                    projectTableRow[10] = item.AwardCloseDate;
                    projectDataTable.Rows.Add(projectTableRow);
                    i++;
                }
            }
            return projectDataTable;
        }

        public static DataTable GenerateDataTableForProjectsTableforViewUsers(ViewProjectDetails viewDetails, string userName)
        {
            VariableDetails projectVariableAssignment = new VariableDetails();
            DataTable projectDataTable = new DataTable();
            projectDataTable.Clear();
            DataColumn opportunityId = new DataColumn(Constant.PROJECTCOLUMNOPPORTUNITYID)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(opportunityId);
            //variableId
            DataColumn Name = new DataColumn(Constant.PROJECTCOLUMNNAME)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(Name);
            //value
            DataColumn BranchValue = new DataColumn(Constant.PROJECTCOLUMNBRANCHVALUE)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(BranchValue);
            //jsondata
            DataColumn SalesId = new DataColumn(Constant.PROJECTCOLUMNSALESID)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(SalesId);

            DataColumn QuoteStatus = new DataColumn(Constant.PROJECTCOLUMNQUOTESTATUS)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(QuoteStatus);

            DataColumn description = new DataColumn(Constant.PROJECTCOLUMNDESCRIPTION)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(description);

            DataColumn BusinessLine = new DataColumn(Constant.PROJECTCOLUMNBUSINESSLINE)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(BusinessLine);

            DataColumn MeasuringUnit = new DataColumn(Constant.PROJECTCOLUMNMEASURINGUNIT)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(MeasuringUnit);


            DataColumn Salesman = new DataColumn(Constant.PROJECTCOLUMNSALESMAN)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(Salesman);

            DataColumn SalesmanActiveDirectoryID = new DataColumn(Constant.PROJECTCOLUMNSALESMANACTIVEDIRECTORYID)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(SalesmanActiveDirectoryID);

            DataColumn country = new DataColumn(Constant.PROJECTCOLUMNCOUNTRY)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(country);

            DataColumn CreatedBy = new DataColumn(Constant.PROJECTCOLUMNCREATEDBY)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(CreatedBy);

            DataColumn ProjectJson = new DataColumn(Constant.PROJECTCOLUMNPROJECTJSON)
            {
                DataType = typeof(string)
            };
            projectDataTable.Columns.Add(ProjectJson);

            DataColumn VersionId = new DataColumn(Constant.PROJECTCOLUMNVERSIONID)
            {
                DataType = typeof(int)
            };
            projectDataTable.Columns.Add(VersionId);

            DataColumn isPrimaryQuote = new DataColumn(Constant.PROJECTCOLUMNPRIMARYQUOTE)
            {
                DataType = typeof(bool)
            };
            projectDataTable.Columns.Add(isPrimaryQuote);

            DataRow projectTableRow = projectDataTable.NewRow();

            projectTableRow[0] = viewDetails.Data.Quotation.OpportunityInfo.OpportunityId;

            projectTableRow[1] = viewDetails.Data.Quotation.OpportunityInfo.JobName;

            projectTableRow[2] = viewDetails.Data.Quotation.OpportunityInfo.Branch;

            projectTableRow[3] = viewDetails.Data.Quotation.OpportunityInfo.SalesStage;
            // need to add the quote Id
            projectTableRow[4] = Constant.PROJECTCOLUMNVALUEINCOMPLETE;
            projectTableRow[5] = viewDetails.Data.Quotation.Quote.QuoteDescription;
            projectTableRow[6] = viewDetails.Data.Quotation.OpportunityInfo.BusinessLine;
            projectTableRow[7] = Constant.PROJECTCOLUMNVALUEMETRIC;
            projectTableRow[8] = viewDetails.Data.Quotation.OpportunityInfo.Salesman;
            projectTableRow[9] = viewDetails.Data.Quotation.OpportunityInfo.SalesmanActiveDirectoryID;
            projectTableRow[10] = Constant.PROJECTCOLUMNVALUEUS;
            projectTableRow[11] = userName;
            projectTableRow[12] = "";
            projectTableRow[13] = viewDetails.Data.VersionId;
            projectTableRow[14] = viewDetails.Data.Quotation.Quote.IsPrimary;

            projectDataTable.Rows.Add(projectTableRow);


            return projectDataTable;
        }

        public static List<SqlParameter> SqlParameterForOzView(string opportunityId)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@quoteId",Value=opportunityId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.NVarChar}


            };
            return lstSqlParameter;
        }

        public static List<SqlParameter> SqlParameterForSaveGetSystemValues(int setId, string statusKey, string userId, DataTable systemDataTable, DataTable systemVariableDataTable)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@setId",Value=setId,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@systemsMetaData",Value=systemDataTable,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = "@StatusName",Value=statusKey,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userId,Direction = ParameterDirection.Input}
            };
            if (systemVariableDataTable != null)
            {
                lstSqlParameter.Add(new SqlParameter() { ParameterName = "@systemsVariables", Value = systemVariableDataTable, Direction = ParameterDirection.Input });
            }
            return lstSqlParameter;
        }

        public static DataTable GenerateDataTableForSaveSystemValidation(List<SystemValidationKeyValues> systemKeyValues)
        {
            DataTable systemDataTable = new DataTable();
            systemDataTable.Clear();


            DataColumn Id = new DataColumn("Id")
            {
                DataType = typeof(int)
            };
            systemDataTable.Columns.Add(Id);

            DataColumn SystemValidKeys = new DataColumn("SystemValidKeys")
            {
                DataType = typeof(string)
            };
            systemDataTable.Columns.Add(SystemValidKeys);

            DataColumn SystemValidValues = new DataColumn("SystemValidValues")
            {
                DataType = typeof(string)
            };
            systemDataTable.Columns.Add(SystemValidValues);


            DataColumn StatusKey = new DataColumn("StatusKey")
            {
                DataType = typeof(string)
            };
            systemDataTable.Columns.Add(StatusKey);

            DataColumn UnitId = new DataColumn("UnitId")
            {
                DataType = typeof(int)
            };
            systemDataTable.Columns.Add(UnitId);

            foreach (var sysValues in systemKeyValues)
            {
                DataRow systemTableRow = systemDataTable.NewRow();
                systemTableRow[0] = sysValues.Id;
                systemTableRow[1] = sysValues.SystemValidKeys;
                systemTableRow[2] = sysValues.SystemValidValues;
                systemTableRow[3] = sysValues.StatusKey;
                systemTableRow[4] = sysValues.UnitId;

                systemDataTable.Rows.Add(systemTableRow);
            }
            return systemDataTable;
        }

        public static DataTable GenerateDataTableForSaveSystemVariables(List<VariableAssignment> systemKeyVariables, int setId)
        {
            DataTable systemDataTable = new DataTable();
            systemDataTable.Clear();

            DataColumn SystemValidKeys = new DataColumn("SystemValidKeys")
            {
                DataType = typeof(string)
            };
            systemDataTable.Columns.Add(SystemValidKeys);

            DataColumn SystemValidValues = new DataColumn("SystemValidValues")
            {
                DataType = typeof(string)
            };
            systemDataTable.Columns.Add(SystemValidValues);

            DataColumn SetId = new DataColumn("SetId")
            {
                DataType = typeof(int)
            };
            systemDataTable.Columns.Add(SetId);

            if (systemKeyVariables != null && systemKeyVariables.Count > 0)
            {
                foreach (var sysVariables in systemKeyVariables)
                {
                    DataRow systemTableRow = systemDataTable.NewRow();
                    systemTableRow[0] = sysVariables.VariableId;
                    systemTableRow[1] = sysVariables.Value;
                    systemTableRow[2] = setId;

                    systemDataTable.Rows.Add(systemTableRow);
                }
            }
            return systemDataTable;
        }

        public static List<Equipment> GenerateDoDDetails(List<UnitDetailsOz> unitIDList)
        {
            var equipment = new List<Equipment>();
            foreach (var unitDetails in unitIDList)
            {


                //if (unitDetails.productName != string.Empty)
                //{

                Equipment equip = new Equipment
                {
                    DesignOnDemand = new DesignOnDemand()
                };
                if (!string.IsNullOrEmpty(unitDetails.DoDQuestions))
                {
                    var DoDQuestions = Utility.DeserializeObjectValue<JArray>(unitDetails.DoDQuestions);
                    foreach (var DOD in DoDQuestions)
                    {
                        if (Utility.CheckEquals(DOD[Constant.DODID].ToString(), Constant.Q1))
                        {
                            var value1 = DOD[Constant.VALUE].ToString();
                            if (value1 != string.Empty && value1 == Constant.NO)
                            {
                                equip.DesignOnDemand.IsDoD = false;
                            }
                            else if (value1 != string.Empty && value1 == Constant.YES)
                            {
                                equip.DesignOnDemand.IsDoD = true;
                            }
                        }
                        if (Utility.CheckEquals(DOD[Constant.DODID].ToString(), Constant.Q2))
                        {
                            var value2 = DOD[Constant.VALUE].ToString();
                            if (value2 != string.Empty && value2 == Constant.SUBMITTALSOUTFORAPPROVAL)
                            {
                                equip.DesignOnDemand.OutForApproval = true;
                                equip.DesignOnDemand.ForFinal = false;
                                equip.DesignOnDemand.ForReviseResubmit = false;
                            }
                            else if (value2 != string.Empty && value2 == Constant.SUBMITTALSBEINGRETURNEDFORFINALS)
                            {
                                equip.DesignOnDemand.OutForApproval = false;
                                equip.DesignOnDemand.ForFinal = true;
                                equip.DesignOnDemand.ForReviseResubmit = false;
                            }
                            else if (value2 != string.Empty && value2 == Constant.SUBMITTALSBEINGRETURNEDFORREVISIONS)
                            {
                                equip.DesignOnDemand.OutForApproval = false;
                                equip.DesignOnDemand.ForFinal = false;
                                equip.DesignOnDemand.ForReviseResubmit = true;
                            }
                        }
                        if (Utility.CheckEquals(DOD[Constant.DODID].ToString(), Constant.Q3))
                        {
                            var value1 = DOD[Constant.VALUE].ToString();
                            if (value1 != string.Empty)
                            {
                                equip.DesignOnDemand.SentDate = value1 ?? string.Empty;
                            }
                        }

                        if (Utility.CheckEquals(DOD[Constant.DODID].ToString(), Constant.Q4))
                        {
                            var value1 = DOD[Constant.VALUE].ToString();
                            if (value1 != string.Empty)
                            {
                                equip.DesignOnDemand.ReceivedDate = value1 ?? string.Empty;
                            }
                        }
                    }
                }
                equip.EstimateIdentifier = new EstimateIdentifier();
                equip.General = new General
                {
                    Model = new Common.Model.UIModel.Model(),
                    Product = new Product()
                };
                equip.EstimateIdentifier.LineId = unitDetails.UEID;
                equip.General.Units = 1;
                equip.General.Designation = unitDetails.unitDesignation;
                //equip.General.Model.ProductModel = unitDetails.ProductName;
                equip.General.Model.ProductModel = "evolution 200 25/35";
                equip.General.Product.ProductLineIdName = "Traction - evolution 200";
                equipment.Add(equip);
                //}

            }
            return equipment;
        }
        public static RequestedDrawing GetRequestedDrawingDetails(List<FdaOz> fdaList)
        {
            RequestedDrawing requestedDrawing = new RequestedDrawing();
            foreach (var fdatype in fdaList)
            {
                if (fdatype.FDAType.Equals("DrawingTypes_InteriorArchitecturalPackage"))
                {
                    requestedDrawing.Cab = true;
                    requestedDrawing.CarStation = true;
                }
                if (fdatype.FDAType.Equals("DrawingTypes_ExteriorArchitecturalPackage"))
                {
                    requestedDrawing.Entrance = true;
                    requestedDrawing.HallStation = true;
                }
                if (fdatype.FDAType.Equals("DrawingTypes_BasePackage"))
                {
                    requestedDrawing.Layout = true;
                }
            }
            requestedDrawing.Freight = false;
            requestedDrawing.LobbyPanel = false;
            return requestedDrawing;
        }


        public static List<SqlParameter> SqlParameterForSaveReleaseInfo(int groupid, DataTable unitVariableAssignment, string userId, ConflictsStatus isEditFlow, string actionFlag)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@GroupId",Value=groupid,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@UnitVariables",Value=unitVariableAssignment,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@IsEditFlows",Value=Convert.ToString(isEditFlow),Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@ActionFlag",Value=Convert.ToString(actionFlag),Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
            };
            return lstSqlParameter;
        }


        public static DataTable GenerateDataTableForSaveReleaseInfo(List<ReleaseInfoSetUnitDetails> unitVariableAssignment, List<ReleaseInfoQuestions> queryAssignment)
        {
            DataTable unitDataTable = new DataTable();
            unitDataTable.Clear();

            DataColumn unitJson = new DataColumn("unitJsonVariables")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(unitJson);

            DataColumn value = new DataColumn("SetId")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(value);

            DataColumn unitJsonData = new DataColumn("SaveRelFlag")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(unitJsonData);

            DataColumn isAcknowledge = new DataColumn("isAcknowledge")
            {
                DataType = typeof(bool)
            };
            unitDataTable.Columns.Add(isAcknowledge);

            DataColumn relComments = new DataColumn("ReleaseComments")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(relComments);

            foreach (var que in queryAssignment)
            {
                DataRow unitTableRow = unitDataTable.NewRow();

                unitTableRow[0] = que.ReleaseQueId;
                unitTableRow[1] = string.Empty;
                unitTableRow[2] = string.Empty;
                unitTableRow[3] = que.ReleaseQueCheck;
                unitTableRow[4] = string.Empty;

                unitDataTable.Rows.Add(unitTableRow);
            }

            foreach (var unit in unitVariableAssignment)
            {
                foreach (var key in unit.DataPointDetails)
                {
                    DataRow unitTableRow = unitDataTable.NewRow();

                    unitTableRow[0] = key.Id;
                    unitTableRow[1] = unit.SetId;
                    unitTableRow[2] = string.Empty;
                    unitTableRow[3] = key.IsAcknowledged == null ? false : key.IsAcknowledged;
                    unitTableRow[4] = unit.ReleaseComments;

                    unitDataTable.Rows.Add(unitTableRow);
                }
            }
            return unitDataTable;
        }

        public static List<SqlParameter> SqlParameterForSavePriceValues(int setId, DataTable unitVariableAssignment, DataTable leadTimeAssignment, string userId, DataTable unitPrices)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@SetId",Value=setId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@UnitVariables",Value=unitVariableAssignment,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@LeadTimeVariables",Value=leadTimeAssignment,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@CreatedBy",Value=userId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
               ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = "@UnitPrices",Value=unitPrices,Direction = ParameterDirection.Input}
            };
            return lstSqlParameter;
        }


        /// <summary>
        /// Utility function for http client call
        /// </summary>
        /// <param name="methodType"></param>
        /// <param name="tokenType"></param>
        /// <param name="requestUrl"></param>
        /// <param name="apiRoute"></param>
        /// <param name="requestBody"></param>
        /// <param name="accessToken"></param>
        /// <param name="requestFor"></param>
        /// <param name="encodedBody"></param>
        /// <returns></returns>



        public static dynamic ConvertDataTypeForView(string type, object value)
        {
            switch (type.ToUpper())
            {
                case Constant.INT:
                    return Convert.ToInt32(value);
                case Constant.BOOLEAN:
                    return CheckEquals(Convert.ToString(value), "NR") ? false : Convert.ToBoolean(value);
                case Constant.DECIMAL:
                    return Convert.ToDecimal(value);
                default:
                    return Convert.ToString(value);
            }
        }

        /// <summary>
        /// SqlParameterForSavinAndUpdateMiniProject
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="unitVariableAssignment"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        public static List<SqlParameter> SqlParameterForSaveAddNewQuoteValues(DataTable unitVariableAssignment, string userName, DataTable accountVariableAssignment, int quoteCreationFlag)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@ProjectDetailList",Value=unitVariableAssignment,Direction = ParameterDirection.Input}
                ,new SqlParameter() { ParameterName = "@quoteCreationFlag",Value=quoteCreationFlag,Direction = ParameterDirection.Input}
                ,new SqlParameter() { ParameterName = "@ProjectsInfoTemp",Value=accountVariableAssignment,Direction = ParameterDirection.Input }
                ,new SqlParameter() { ParameterName = "@Result",Value=0 ,SqlDbType = SqlDbType.Int}
            };
            return lstSqlParameter;
        }


        /// <summary>
        /// DataTableForGuids
        /// </summary>
        /// <param name="unitVariableAssignment"></param>
        /// <param name="queryAssignment"></param>
        /// <returns></returns>
        public static DataTable DataTableForGuids(IList<string> listOfGroups)
        {
            DataTable unitDataTable = new DataTable();
            unitDataTable.Clear();

            DataColumn lstGuids = new DataColumn("Guid")
            {
                DataType = typeof(string)
            };
            unitDataTable.Columns.Add(lstGuids);

            foreach (var groupGuid in listOfGroups)
            {
                DataRow unitTableRow = unitDataTable.NewRow();

                unitTableRow[0] = groupGuid.ToString();
                unitDataTable.Rows.Add(unitTableRow);
            }
            return unitDataTable;
        }

        /// <summary>
        /// SqlParameterForGetUserDetails
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupGuidDataTable"></param>
        /// <returns></returns>
        public static List<SqlParameter> SqlParameterForGetUserDetails(DataTable groupGuidDataTable)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@guid",Value=groupGuidDataTable,Direction = ParameterDirection.Input}
            };
            return lstSqlParameter;
        }

        public static string MultiFormUploadFileApi(string url, IDictionary<string, string> headers, string boundary, IDictionary<string, string> postData, dynamic fileInfo, byte[] file)
        {
            var methodStartTime = Utility.LogBegin();
            Utility.LogTrace($"API request to {url}", new Dictionary<string, object>() { { "request", postData.Values.First() } });
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            foreach (var item in headers)
            {
                request.Headers.Add(item.Key, item.Value);
            }

            request.KeepAlive = true;

            request.ContentType = "multipart/form-data; boundary=" + boundary;

            Stream requestStream = request.GetRequestStream();

            WriteMultipartFormData(postData, requestStream, boundary);

            var fileName = fileInfo.GetType().GetProperty("Name").GetValue(fileInfo);
            var fileMime = fileInfo.GetType().GetProperty("Mime").GetValue(fileInfo);
            var fileStream = (byte[])fileInfo.GetType().GetProperty("Stream").GetValue(fileInfo);

            WriteMultipartFormDataStream(file, requestStream, boundary, fileMime, "file", fileName);

            byte[] endBytes = Encoding.UTF8.GetBytes("--" + boundary + "--");
            requestStream.Write(endBytes, 0, endBytes.Length);
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var responseString = reader.ReadToEnd();
                        Utility.LogTrace($"API response from {url}", new Dictionary<string, object>() { { "response", responseString } });
                        Utility.LogEnd(methodStartTime);
                        return responseString;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Exception while upoading to MFile- {ex.Message}");
                return ex.Message;
            }
        }



        public const string FormDataTemplate = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n";

        public const string HeaderTemplate = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n";


        /// <summary>
        /// Writes a dictionary to a stream as a multipart/form-data set.
        /// </summary>
        /// <param name="dictionary">The dictionary of form values to write to the stream.</param>
        /// <param name="stream">The stream to which the form data should be written.</param>
        /// <param name="mimeBoundary">The MIME multipart form boundary string.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="stream" /> or <paramref name="mimeBoundary" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown if <paramref name="mimeBoundary" /> is empty.
        /// </exception>
        /// <remarks>
        /// If <paramref name="dictionary" /> is <see langword="null" /> or empty,
        /// nothing wil be written to the stream.
        /// </remarks>
        private static void WriteMultipartFormData(IDictionary<string, string> dictionary, Stream stream, string mimeBoundary)
        {
            /// <summary>
            /// Template for a multipart/form-data item.
            /// </summary>
            if (dictionary == null || dictionary.Count == 0)
            {
                return;
            }
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (mimeBoundary == null)
            {
                throw new ArgumentNullException("mimeBoundary");
            }
            if (mimeBoundary.Length == 0)
            {
                throw new ArgumentException("MIME boundary may not be empty.", "mimeBoundary");
            }
            foreach (string key in dictionary.Keys)
            {
                string item = String.Format(FormDataTemplate, mimeBoundary, key, dictionary[key]);
                byte[] itemBytes = System.Text.Encoding.UTF8.GetBytes(item);

                stream.Write(itemBytes, 0, itemBytes.Length);
            }
        }

        private static void WriteMultipartFormDataStream(byte[] file, Stream stream, string mimeBoundary, string mimeType, string formKey, string fileName)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (mimeBoundary == null)
            {
                throw new ArgumentNullException("mimeBoundary");
            }
            if (mimeBoundary.Length == 0)
            {
                throw new ArgumentException("MIME boundary may not be empty.", "mimeBoundary");
            }
            if (mimeType == null)
            {
                throw new ArgumentNullException("mimeType");
            }
            if (mimeType.Length == 0)
            {
                throw new ArgumentException("MIME type may not be empty.", "mimeType");
            }
            if (formKey == null)
            {
                throw new ArgumentNullException("formKey");
            }
            if (formKey.Length == 0)
            {
                throw new ArgumentException("Form key may not be empty.", "formKey");
            }
            string header = String.Format(HeaderTemplate, mimeBoundary, formKey, fileName, mimeType);
            byte[] headerbytes = Encoding.UTF8.GetBytes(header);
            stream.Write(headerbytes, 0, headerbytes.Length);


            //convert to base64
            var base64Content = System.Convert.ToBase64String(file);

            //encoding to ASCII
            var encoded = Encoding.ASCII.GetBytes(base64Content);

            stream.Write(encoded);

            byte[] newlineBytes = Encoding.UTF8.GetBytes("\r\n");
            stream.Write(newlineBytes, 0, newlineBytes.Length);
        }

        /// <summary>
        /// Map Variables
        /// </summary>
        /// <param name="clmResponseString"></param>
        /// <param name="appResponseString"></param>
        /// <returns></returns>
        public static string MapVariables(string clmResponseString, string appResponseString)
        {
            //transform to JObject
            var clmResponse = JObject.Parse(clmResponseString);
            var appResponse = JObject.Parse(appResponseString);

            return MapVariables(clmResponse, appResponse).ToString();
        }

        /// <summary>
        /// MapVariables
        /// </summary>
        /// <param name="clmResponse"></param>
        /// <param name="appResponse"></param>
        /// <returns></returns>
        public static JObject MapVariables(JObject clmResponse, JObject appResponse)
        {
            //extract the variables
            var collection = GetVariables(clmResponse);
            var appSections = GetTokens("sections", appResponse);

            foreach (var item in appSections)
            {
                var currentVariables = new JArray(GetTokens("variables", item));
                var replacable = (collection.Where(x => currentVariables.Any(y => (string)x["id"] == (string)y["id"]))).ToArray<JToken>();
                currentVariables.Merge(replacable, new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Replace });
                item["variables"] = currentVariables;
            }

            return appResponse;
        }
        public static IEnumerable<JToken> GetVariables(JToken parent)
        {
            var tokenCollection = new List<JToken>();
            var children = GetTokens("sections", parent);

            foreach (var item in children)
            {
                tokenCollection.AddRange(GetTokens("variables", item));
            }

            return tokenCollection;
        }


        public static IEnumerable<JToken> GetTokens(string propertyName, JToken parent, bool includeEmptyTokens = false)
        {
            var collection = new List<JToken>();

            if (parent is JArray jArrayParent)
            {
                collection.AddRange(GetTokens(propertyName, jArrayParent));
            }

            if (parent is JObject jObjectParent)
            {
                foreach (var item in jObjectParent.Children<JProperty>())
                {
                    if (item.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (includeEmptyTokens || item.Value.Count() > 0)
                        {
                            collection.AddRange(GetTokens(propertyName, item.Value));
                            collection.AddRange(item.Value.Children());

                        }
                    }
                }
            }

            return collection;
        }

        private static IEnumerable<JToken> GetTokens(string propertyName, JArray parent)
        {
            return parent.Children().SelectMany(x => GetTokens(propertyName, x));
        }

        /// <summary>
        /// To generate variable Mapper Data Table for Stored Procedures
        /// </summary>
        /// <param name="variableAssignments"></param>
        /// <returns></returns>
        public static DataTable GenerateVariableMapperDataTable(List<ConfigVariable> variableAssignments)
        {
            DataTable VariableMapper = new DataTable();
            {
                VariableMapper.Clear();
                DataColumn VariableKey = new DataColumn(Constant.VARIABLEKEY)
                {
                    DataType = typeof(string)
                };
                VariableMapper.Columns.Add(VariableKey);
                //value
                DataColumn VariableValue = new DataColumn(Constant.VARIABLETYPE)
                {
                    DataType = typeof(string)
                };
                VariableMapper.Columns.Add(VariableValue);
                foreach (var variable in variableAssignments)
                {
                    DataRow DrawingTableRow = VariableMapper.NewRow();
                    DrawingTableRow[0] = variable.VariableId;
                    DrawingTableRow[1] = variable.Value;
                    VariableMapper.Rows.Add(DrawingTableRow);
                }
                return VariableMapper;
            }
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

        public static Dictionary<string, string> GetConfigurationKeyValues(JObject clmResponse)
        {
            var configurationResponse = clmResponse.ToObject<StartConfigureResponse>();
            var configureResponseArgumentJObject = DeserializeObjectValue<JObject>(SerializeObjectValue(configurationResponse.Arguments));
            var configureRequestDictionary = configureResponseArgumentJObject[Constant.CONFIGURATION].ToObject<Dictionary<string, string>>();
            return configureRequestDictionary;
        }

        public static DataTable GenerateDataTableForDefaultConsoleVariables(Dictionary<string, Dictionary<string, string>> ConsoleDefaultVariables)
        {
            DataTable systemDataTable = new DataTable();
            systemDataTable.Clear();

            DataColumn ConsoleType = new DataColumn("ConsoleType")
            {
                DataType = typeof(string)
            };
            systemDataTable.Columns.Add(ConsoleType);

            DataColumn VariableId = new DataColumn("VariableType")
            {
                DataType = typeof(string)
            };
            systemDataTable.Columns.Add(VariableId);

            DataColumn Value = new DataColumn("VariableValue")
            {
                DataType = typeof(string)
            };
            systemDataTable.Columns.Add(Value);

            if (ConsoleDefaultVariables != null && ConsoleDefaultVariables.Count > 0)
            {
                foreach (var consoleType in ConsoleDefaultVariables.Keys)
                {
                    foreach (var variableAssignment in ConsoleDefaultVariables[consoleType])
                    {
                        DataRow systemTableRow = systemDataTable.NewRow();
                        systemTableRow[0] = consoleType;
                        systemTableRow[1] = variableAssignment.Key;
                        systemTableRow[2] = variableAssignment.Value;

                        systemDataTable.Rows.Add(systemTableRow);
                    }

                }
            }
            return systemDataTable;
        }

        public static DataTable GenerateDataTableForUnitPrices(List<UnitNames> unitPrices)
        {
            DataTable systemDataTable = new DataTable();
            systemDataTable.Clear();

            DataColumn UnitId = new DataColumn("UnitId")
            {
                DataType = typeof(string)
            };
            systemDataTable.Columns.Add(UnitId);

            DataColumn VariableId = new DataColumn("VariableId")
            {
                DataType = typeof(string)
            };
            systemDataTable.Columns.Add(VariableId);

            DataColumn Value = new DataColumn("VariableValue")
            {
                DataType = typeof(string)
            };
            systemDataTable.Columns.Add(Value);

            if (unitPrices != null && unitPrices.Count > 0)
            {

                foreach (var unitDetails in unitPrices)
                {
                    DataRow systemTableRow = systemDataTable.NewRow();
                    systemTableRow[0] = unitDetails.Unitid;
                    systemTableRow[1] = "unitPrice";
                    systemTableRow[2] = unitDetails.Price;

                    systemDataTable.Rows.Add(systemTableRow);
                }


            }
            return systemDataTable;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="variableAssignments"></param>
        /// <returns></returns>
        public static DataTable GenerateVariableMapperDataTableForConflict(List<VariableAssignment> variableAssignments)
        {
            DataTable VariableMapper = new DataTable();
            {
                VariableMapper.Clear();
                DataColumn VariableId = new DataColumn(Constant.VARIABLEIDVALUES)
                {
                    DataType = typeof(string)
                };
                VariableMapper.Columns.Add(VariableId);
                DataColumn Value = new DataColumn(Constant.VALUE)
                {
                    DataType = typeof(string)
                };
                VariableMapper.Columns.Add(Value);
                foreach (var variable in variableAssignments)
                {
                    DataRow DrawingTableRow = VariableMapper.NewRow();
                    var vai = variable.VariableId.Replace("\n", string.Empty);
                    vai = variable.VariableId.Replace("\r", string.Empty);
                    vai = variable.VariableId.Replace("\t", string.Empty);
                    DrawingTableRow[0] = vai;
                    DrawingTableRow[1] = variable.Value;
                    VariableMapper.Rows.Add(DrawingTableRow);
                }
                return VariableMapper;
            }
        }
        public static DataTable GenerateDataTableForUnitIdentification(ViewProjectDetails viewDetails)
        {
            DataTable systemDataTable = new DataTable();
            systemDataTable.Clear();

            DataColumn FactoryJobId = new DataColumn(Constants.FACTORYJOBID)
            {
                DataType = typeof(string)
            };
            systemDataTable.Columns.Add(FactoryJobId);

            DataColumn UEID = new DataColumn(Constants.UEID)
            {
                DataType = typeof(string)
            };
            systemDataTable.Columns.Add(UEID);

            if (viewDetails?.FactoryDetails != null && viewDetails?.FactoryDetails.Count > 0)
            {
                foreach (var units in viewDetails.FactoryDetails)
                {
                    DataRow systemTableRow = systemDataTable.NewRow();
                    systemTableRow[0] = string.IsNullOrEmpty(units.FacotryJobId) ? string.Empty : units.FacotryJobId;
                    systemTableRow[1] = units.UEID;

                    systemDataTable.Rows.Add(systemTableRow);
                }
            }
            return systemDataTable;
        }
        public static List<SqlParameter> SqlParameterForCreateUpdateFactoryJobId(int unitId, string userId, string factoryJobId)
        {
            var lstSqlParameter = new List<SqlParameter>()
            {

               new SqlParameter() { ParameterName = "@UnitId",Value=unitId,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@UserName",Value=userId,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@FactoryJobID",Value=factoryJobId,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = "@Result",Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
            };
            return lstSqlParameter;
        }
    }
}
