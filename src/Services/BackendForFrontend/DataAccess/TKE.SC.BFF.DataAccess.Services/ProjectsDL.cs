/************************************************************************************************************
    File Name     :   ProjectsDL
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
********************************************************************************************/
using Configit.Configurator.Server.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OracleCRMOD_Account;
using OracleCRMOD_Opportunity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Database;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.HttpClientModel;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class ProjectsDL : IProjectsDL
    {
        private readonly ILogger _logger;
        private static readonly string loginUrlString = @"https://secure-ausomxdia.crmondemand.com/Services/Integration?command=login&quot;";
        private static readonly string logoffUrlString = @"https://secure-ausomxdia.crmondemand.com/Services/Integration?command=logoff&quot;";
        private static string userName;
        private static string password;
        private static string proxyserver;        //Proxy server for Infosys Network
        private readonly StringBuilder sb = new StringBuilder();
        private readonly string _environment;
        private IConfiguration _configuration;
        private ICacheManager _cacheManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param Name="logger"></param>
        /// <param Name="iConfig"></param>
        public ProjectsDL(ILogger<ProjectsDL> logger, IConfiguration iConfig, ICacheManager cpqCacheManager)
        {
            Utility.SetLogger(logger);
            _configuration = iConfig;
            _logger = logger;
            _cacheManager = cpqCacheManager;
            _environment = Constant.DEV;
        }

        /// <summary>
        /// this method is to get the List of project for the user
        /// </summary>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public List<Projects> GetListOfProjectsForUser(int userId)
        {
            var methodStartTime = Utility.LogBegin();
            List<Projects> projectlist = new List<Projects>();
            Projects project;
            List<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@_ID, Value = userId}
            };
            DataSet ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETLISTOFPROJECTSFORUSER, sp);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    project = new Projects()
                    {
                        id = row["Id"].ToString(),
                        name = row["Name"].ToString(),
                        completedDate = Convert.ToDateTime(row["completedDate"]),
                        bidDate = Convert.ToDateTime(row["bidDate"]),
                        bookDate = Convert.ToDateTime(row["bookDate"]),
                        noOfUnits = 12,
                        location = new Location()
                        {
                            id = row["locationId"].ToString(),
                            city = row["city"].ToString(),
                            state = row["state"].ToString(),
                            country = row["country"].ToString(),
                        },
                        //Client
                        client = new Client()
                        {
                            id = row["clientId"].ToString(),
                            name = row["Name"].ToString(),
                            //ContactPerson
                            contactPerson = new User()
                            {
                                Id = Convert.ToInt32(row["contactPersonId"]),
                                FirstName = row["firstName"].ToString(),
                                LastName = row["lastName"].ToString(),
                            }
                        },
                        //TotalPrice
                        totalPrice = new TotalPrice()
                        {
                            currency = row["currency"].ToString(),
                            price = Convert.ToDecimal(row["price"]),
                        },
                        //CreatedBy
                        createdOn = Convert.ToDateTime(row["CreatedOn"]),
                        createdBy = new User()
                        {
                            Id = Convert.ToInt32(row["createdBy"]),
                            FirstName = row["createdByFN"].ToString(),
                            LastName = row["createdByLN"].ToString(),
                        },
                        //ModifiedBy
                        modifiedOn = Convert.ToDateTime(row["ModifiedOn"]),
                        modifiedBy = new User()
                        {
                            Id = Convert.ToInt32(row["modifiedBy"]),
                            FirstName = row["modifiedByFN"].ToString(),
                            LastName = row["modifiedByLN"].ToString(),
                        }
                    };
                    projectlist.Add(project);
                }
            }
            Utility.LogEnd(methodStartTime);
            return projectlist;
        }

        /// <summary>
        /// To Get Account Address
        /// </summary>
        /// <param Name="accountId"></param>
        /// <returns></returns>
        public AccountEntity GetAccountAddress(string accountId)
        {
            var methodStartTime = Utility.LogBegin();
            AccountEntity accountAddress = new AccountEntity();
            _configuration = new ConfigurationBuilder().SetBasePath(System.IO.Directory.GetCurrentDirectory()).AddJsonFile(Constant.APPSETTINGS).Build();
            string loggedInPath = Convert.ToString(_configuration.GetSection(Constant.PARAMSETTINGS).GetSection(Constant.ENVIRONMENTPATH).Value);
            userName = Convert.ToString(_configuration.GetSection(Constant.PARAMSETTINGS).GetSection(Constant.CRMODUSERNAME).Value);
            password = Convert.ToString(_configuration.GetSection(Constant.PARAMSETTINGS).GetSection(Constant.CRMODPASSWORD).Value);
            var endpointAddress = new System.ServiceModel.EndpointAddress(new System.Uri(loggedInPath));
            AccountClient cli = new AccountClient(AccountClient.EndpointConfiguration.Account, endpointAddress);
            var logger = (ILogger<WsSecurityEndpointBehavior>)_logger;
            cli.Endpoint.EndpointBehaviors.Add(new WsSecurityEndpointBehavior(userName, password, logger));
            var httpbinding = cli.Endpoint.Binding as System.ServiceModel.BasicHttpBinding;
            httpbinding.UseDefaultWebProxy = false;
            //To check if proxy server has to be used
            CheckNetworkToUseProxy(cli.Endpoint.Binding as System.ServiceModel.BasicHttpBinding);
            Account myAcc = cli;
            OracleCRMOD_Account.queryType myId = new OracleCRMOD_Account.queryType
            {
                Value = "='" + @accountId + "'"
            };
            // Create accountquery instance and set the appropriate parameters
            OracleCRMOD_Account.AccountQuery myAccQuery = new OracleCRMOD_Account.AccountQuery()
            {
                Id = myId,
                AccountName = new OracleCRMOD_Account.queryType(),
                PrimaryBillToCity = new OracleCRMOD_Account.queryType(),
                PrimaryBillToState = new OracleCRMOD_Account.queryType(),
                PrimaryBillToPostalCode = new OracleCRMOD_Account.queryType(),
                PrimaryBillToCountry = new OracleCRMOD_Account.queryType(),
                PrimaryBillToStreetAddress = new OracleCRMOD_Account.queryType(),
                PrimaryBillToStreetAddress2 = new OracleCRMOD_Account.queryType(),
            };
            // Set ListOfAccountQuery
            OracleCRMOD_Account.ListOfAccountQuery lstOfAccQuery = new OracleCRMOD_Account.ListOfAccountQuery
            {
                Account = myAccQuery,
                // Number of records to fetch
                pagesize = "100"
            };
            // set AccountQueryPage_Input
            AccountQueryPage_Input myAccInput = new AccountQueryPage_Input
            {
                ListOfAccount = lstOfAccQuery
            };
            AccountQueryPageRequest myAccQPR = new AccountQueryPageRequest
            {
                AccountQueryPage_Input = myAccInput
            };
            // Get the output
            AccountQueryPage_Output myOutput = new AccountQueryPage_Output();
            AccountQueryPageResponse res = new AccountQueryPageResponse
            {
                AccountQueryPage_Output = myOutput
            };
            res = myAcc.AccountQueryPageAsync(myAccQPR).Result;
            // Get ListOfAccountData 
            OracleCRMOD_Account.ListOfAccountData myAccData = res.AccountQueryPage_Output.ListOfAccount;
            if (myAccData != null && myAccData.Account != null)
            {
                OracleCRMOD_Account.AccountData[] accData = myAccData.Account;
                // Total number of records returned
                foreach (OracleCRMOD_Account.AccountData oData in accData)
                {
                    accountAddress.AccountId = oData.Id;
                    accountAddress.AccountName = oData.AccountName;
                    accountAddress.AccountAddressCity = oData.PrimaryBillToCity;
                    accountAddress.AccountAddressAddressZipCode = oData.PrimaryBillToPostalCode;
                    accountAddress.AccountAddressCountry = oData.PrimaryBillToCountry;
                    accountAddress.AccountAddressState = oData.PrimaryBillToState;
                    accountAddress.AccountAddressStreetAddress = oData.PrimaryBillToStreetAddress;
                    accountAddress.AccountAddressStreetAddress2 = oData.PrimaryBillToStreetAddress2;
                }
            }
            Utility.LogEnd(methodStartTime);
            return accountAddress;
        }

        /// <summary>
        /// To Get Opportunity Data
        /// </summary>
        /// <param Name="oppId"></param>
        /// <returns></returns>
        public OpportunityEntity GetOpportunityData(string oppId)
        {
            var methodStartTime = Utility.LogBegin();
            OpportunityEntity oppty = new OpportunityEntity();
            Utility.LogTrace("Process Started" + "-" + DateTime.Now);
            Utility.LogTrace("CRMOD login Started" + "-" + DateTime.Now);
            _configuration = new ConfigurationBuilder().SetBasePath(System.IO.Directory.GetCurrentDirectory()).AddJsonFile(Constant.APPSETTINGS).Build();
            var crmodCredentials = _configuration.GetSection(Constant.PARAMSETTINGS).GetSection(Constant.ENVIRONMENTPATH).Value;
            string loggedInPath = "";
            if (crmodCredentials != null)
            {
                loggedInPath = Convert.ToString(crmodCredentials);
            }
            crmodCredentials = Convert.ToString(_configuration.GetSection(Constant.PARAMSETTINGS).GetSection(Constant.CRMODUSERNAME).Value);
            if (crmodCredentials != null)
            {
                userName = Convert.ToString(crmodCredentials);
            }
            crmodCredentials = Convert.ToString(_configuration.GetSection(Constant.PARAMSETTINGS).GetSection(Constant.CRMODPASSWORD).Value);
            if (crmodCredentials != null)
            {
                password = Convert.ToString(crmodCredentials);
            }
            var endpointAddress = new System.ServiceModel.EndpointAddress(new System.Uri(loggedInPath));
            OpportunityClient cli = new OpportunityClient(OpportunityClient.EndpointConfiguration.Opportunity, endpointAddress);
            var logger = (ILogger<WsSecurityEndpointBehavior>)_logger;
            cli.Endpoint.EndpointBehaviors.Add(new WsSecurityEndpointBehavior(userName, password, logger));
            var httpbinding = cli.Endpoint.Binding as System.ServiceModel.BasicHttpBinding;
            httpbinding.UseDefaultWebProxy = false;
            Utility.LogTrace("authentication completed" + "-" + DateTime.Now);
            //To check if proxy server has to be used
            CheckNetworkToUseProxy(cli.Endpoint.Binding as System.ServiceModel.BasicHttpBinding);
            OracleCRMOD_Opportunity.Opportunity myOpp = cli;
            OracleCRMOD_Opportunity.queryType myId = new OracleCRMOD_Opportunity.queryType
            {
                Value = "='" + @oppId + "'"
            };
            Utility.LogTrace("opportunity ID" + @oppId + "-" + DateTime.Now);
            // Create opportunityquery instance and set the appropriate parameters
            OracleCRMOD_Opportunity.OpportunityQuery myOppQuery = new OracleCRMOD_Opportunity.OpportunityQuery()
            {
                Id = myId,
                OpportunityName = new OracleCRMOD_Opportunity.queryType(),
                OpportunityType = new OracleCRMOD_Opportunity.queryType(),
                SalesStage = new OracleCRMOD_Opportunity.queryType(),
                AccountId = new OracleCRMOD_Opportunity.queryType(),
                AccountName = new OracleCRMOD_Opportunity.queryType(),
                plAccount_Type = new OracleCRMOD_Opportunity.queryType(),
                AccountAddressCity = new OracleCRMOD_Opportunity.queryType(),
                AccountAddressState = new OracleCRMOD_Opportunity.queryType(),
                AccountAddressAddressZipCode = new OracleCRMOD_Opportunity.queryType(),
                AccountAddressCountry = new OracleCRMOD_Opportunity.queryType(),
                AccountAddressStreetAddress = new OracleCRMOD_Opportunity.queryType(),
                AccountAddressStreetAddress2 = new OracleCRMOD_Opportunity.queryType(),
                IndexedDate0 = new OracleCRMOD_Opportunity.queryType(),  //Proposed Date
                CreatedDate = new OracleCRMOD_Opportunity.queryType(),
                ModifiedDate = new OracleCRMOD_Opportunity.queryType(),
                IndexedPick2 = new OracleCRMOD_Opportunity.queryType(), //LineOfBusiness
                IndexedPick4 = new OracleCRMOD_Opportunity.queryType(), //Region
                IndexedPick0 = new OracleCRMOD_Opportunity.queryType(), //Branch
                IndexedPick1 = new OracleCRMOD_Opportunity.queryType(), //MarketSegment;
                plEagle_Sales_Stage = new OracleCRMOD_Opportunity.queryType(),
                cExpected_Eagle_Revenue = new OracleCRMOD_Opportunity.queryType(),
                Probability = new OracleCRMOD_Opportunity.queryType(),
                LeadSource = new OracleCRMOD_Opportunity.queryType(),
                dDate_Booking_Package_Received = new OracleCRMOD_Opportunity.queryType(),
                CloseDate = new OracleCRMOD_Opportunity.queryType(),
                CreatedBy = new OracleCRMOD_Opportunity.queryType(),
                CreatedByFirstName = new OracleCRMOD_Opportunity.queryType(),
                //new params added
                Owner = new OracleCRMOD_Opportunity.queryType(),
                OwnerFullName = new OracleCRMOD_Opportunity.queryType(),
                OwnerId = new OracleCRMOD_Opportunity.queryType(),
                CreatedById = new OracleCRMOD_Opportunity.queryType(),
                CreatedByFullName = new OracleCRMOD_Opportunity.queryType(),
                CreatedByAlias = new OracleCRMOD_Opportunity.queryType(),
            };

            // Set ListOfOpportunityQuery
            OracleCRMOD_Opportunity.ListOfOpportunityQuery lstOfOppQuery = new OracleCRMOD_Opportunity.ListOfOpportunityQuery
            {
                Opportunity = myOppQuery
            };
            // Number of records to fetch
            Utility.LogTrace("opportunity Query initialization Completed" + "-" + DateTime.Now);
            lstOfOppQuery.pagesize = "100";
            // set OpportunityQueryPage_Input
            OpportunityQueryPage_Input myOppInput = new OpportunityQueryPage_Input
            {
                ListOfOpportunity = lstOfOppQuery
            };
            OpportunityQueryPageRequest myOppQPR = new OpportunityQueryPageRequest
            {
                OpportunityQueryPage_Input = myOppInput
            };
            // Get the output
            OpportunityQueryPage_Output myOutput = new OpportunityQueryPage_Output();
            OpportunityQueryPageResponse res = new OpportunityQueryPageResponse
            {
                OpportunityQueryPage_Output = myOutput
            };
            res = myOpp.OpportunityQueryPageAsync(myOppQPR).Result;
            Utility.LogTrace("res Completed" + "-" + DateTime.Now);
            // Get ListOfOpportunityData 
            OracleCRMOD_Opportunity.ListOfOpportunityData myOppData = res.OpportunityQueryPage_Output.ListOfOpportunity;
            if (myOppData != null && myOppData.Opportunity != null)
            {
                Utility.LogTrace("if condition started" + "-" + DateTime.Now);
                OracleCRMOD_Opportunity.OpportunityData[] oppData = myOppData.Opportunity;
                // Total number of records returned
                Utility.LogTrace("Data assinging started" + "-" + DateTime.Now);
                foreach (OracleCRMOD_Opportunity.OpportunityData oData in oppData)
                {
                    oppty.Id = oData.Id;
                    oppty.OpportunityName = oData.OpportunityName;
                    oppty.OpportunityType = oData.OpportunityType;
                    oppty.SalesStage = oData.SalesStage;
                    oppty.AccountId = oData.AccountId;
                    oppty.AccountName = oData.AccountName;
                    oppty.AccountType = oData.plAccount_Type;
                    oppty.ProposedDate = oData.IndexedDate0;  //Proposed Date
                    oppty.CreatedDate = oData.CreatedDate;
                    oppty.ModifiedDate = oData.ModifiedDate;
                    oppty.LineOfBusiness = oData.IndexedPick2; //LineOfBusiness
                    oppty.Region = oData.IndexedPick4; //Region
                    oppty.Branch = oData.IndexedPick0; //Branch
                    oppty.MarketSegment = oData.IndexedPick1; //MarketSegment;
                    oppty.EagleSalesStage = oData.plEagle_Sales_Stage;
                    oppty.ExpectedEagleRevenue = (double)oData.cExpected_Eagle_Revenue;
                    oppty.Probability = (double)oData.Probability;
                    oppty.LeadSource = oData.LeadSource;
                    oppty.BookingDate = oData.dDate_Booking_Package_Received;
                    oppty.CloseDate = oData.CloseDate;
                    oppty.CreatedBy = oData.CreatedBy;
                    oppty.CreatedByFirstName = oData.CreatedByFirstName;
                    oppty.Owner = oData.Owner;
                    oppty.OwnerFullName = oData.OwnerFullName;
                    oppty.OwnerId = oData.OwnerId;
                    oppty.CreatedById = oData.CreatedById;
                    oppty.CreatedByFullName = oData.CreatedByFullName;
                    oppty.CreatedByAlias = oData.CreatedByAlias;
                    oppty.AccountAddress = GetAccountAddress(oppty.AccountId);
                }
                Utility.LogTrace("Data assinging forloop completed" + "-" + DateTime.Now);
                Utility.LogTrace("Opportunuty data binding completed" + "-" + DateTime.Now);
            }
            else
            {
                Utility.LogTrace("Opprunity values are null" + "-" + DateTime.Now);
            }
            Utility.LogTrace("Process completed" + "-" + DateTime.Now);
            Utility.LogTrace("Opporunity Data" + oppty.CreatedBy + "-" + DateTime.Now);
            Utility.LogEnd(methodStartTime);
            return oppty;
        }

        /// <summary>
        /// this method is to search a user
        /// </summary>
        /// <param Name="userName"></param>
        /// <returns></returns>
        public List<User> SearchUser(string userName)
        {
            var methodStartTime = Utility.LogBegin();
            List<User> userlist = new List<User>();
            User user;
            List<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@USERNAME, Value = userName}
            };
            DataSet ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPSEARCHUSER, sqlParameters);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    user = new User()
                    {
                        Location = new Location(),
                        Role = new Role(),
                        Id = Convert.ToInt32(row["id"]),
                        UserId = row["userId"].ToString(),
                        FirstName = row["firstName"].ToString(),
                        LastName = row["lastName"].ToString(),
                        Email = row["email"].ToString(),
                        ProfilePic = row["profilePic"].ToString(),
                    };
                    userlist.Add(user);
                }
            }
            Utility.LogEnd(methodStartTime);
            return userlist;
        }

        /// <summary>
        /// This function checks for the network interfaces and detects if corporate network is connected,
        /// to check if CRMOD request has to be sent through proxy server
        /// </summary>
        /// <param Name="crmReqBinding"></param>
        public void CheckNetworkToUseProxy(System.ServiceModel.BasicHttpBinding crmReqBinding)
        {
            var methodStartTime = Utility.LogBegin();
            bool onCorpNetwork = false;
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                if (adapter.OperationalStatus == OperationalStatus.Up && properties.DnsSuffix == Constant.ADINFOSYS)
                {
                    onCorpNetwork = true;
                    break;
                }
            }
            if (onCorpNetwork)
            {
                proxyserver = Convert.ToString(_configuration.GetSection(Constant.PARAMSETTINGS).GetSection(Constant.CRMODPROXYSERVER).Value);

                crmReqBinding.ProxyAddress = new Uri(proxyserver);
                crmReqBinding.BypassProxyOnLocal = false;
                crmReqBinding.UseDefaultWebProxy = false;
            }
            Utility.LogEnd(methodStartTime);
        }

        /// <summary>
        /// Method to Get Project Info
        /// </summary>
        /// <param Name="OpportunityId"></param>
        /// <param Name="versionId"></param>
        /// <param Name="defineSymbol"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetProjectInfo(string opportunityId, string versionId)
        {
            var methodStartTime = Utility.LogBegin();
            var viewSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constant.PROJECTMMANAGEMENTSETTINGS);
            var viewUserName = Utility.GetPropertyValue(viewSettings, Constant.APIUSERNAME);
            var viewPassword = Utility.GetPropertyValue(viewSettings, Constant.APIPASSWORD);
            var authToken = Utility.GenerateBasicAuthTocken(viewUserName, viewPassword);

            var requestObject = new HttpClientRequestModel()
            {
                Method = HTTPMETHODTYPE.GET,
                BaseUrl = Utility.GetPropertyValue(viewSettings, Constant.BASEURL),
                EndPoint = string.Format(Utility.GetPropertyValue(viewSettings, Constant.PROJECTINFOAPI), opportunityId, versionId),
                RequestHeaders = new Dictionary<string, string>
                {
                    { Constant.AUTHORIZATION, Constant.BASIC + authToken }
                }
            };

            var viewResponse = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(false);
            var responseContent = String.IsNullOrEmpty(viewResponse.Content.ReadAsStringAsync().Result) ? Constant.VIEWERROEMESSAGE : viewResponse.Content.ReadAsStringAsync().Result;
            if (!viewResponse.IsSuccessStatusCode)
            {
                Utility.LogError(Constant.VIEWERROEMESSAGE);
                throw new ExternalCallException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.VIEWERROEMESSAGE,
                    Description = Constant.VIEWERROEMESSAGE
                });
            }

            JObject projectDetails = JObject.Parse(responseContent.ToString());
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = projectDetails };
        }

        /// <summary>
        ///  Method to Get Project Info
        /// </summary>
        /// <param Name="OpportunityId"></param>
        /// <param Name="exportJsonBuildingVariables"></param>
        /// <param Name="exportJsoneqmntConsoleVariables"></param>
        /// <param Name="exportJsonEqmntConfgnVariables"></param>
        /// <param Name="exportJsonControlLocationVariables"></param>
        /// <param Name="exportJsonUnitConfigurationVariables"></param>
        /// <param Name="defaultVtPackageValues"></param>
        /// <returns></returns>
        public ViewExportDetails GetVariablesAndValuesForView1(string opportunityId, List<string> exportJsonBuildingVariables, List<string> exportJsoneqmntConsoleVariables, List<string> exportJsonEqmntConfgnVariables, List<string> exportJsonControlLocationVariables, List<string> exportJsonUnitConfigurationVariables, List<BuildingVariableAssignment> defaultVtPackageValues)
        {
            var methodStartTime = Utility.LogBegin();
            Utility.LogTrace(Constant.GENARETAREQUESTBODYFORVIEWEXPORTSTARTED);
            DataTable buildingDataTable = Utility.GenerateDataTableForExportView(exportJsonBuildingVariables);
            DataTable buildinEqmntConsoleDataTable = Utility.GenerateDataTableForExportView(exportJsoneqmntConsoleVariables);
            DataTable buildinEqmntConfigureDataTable = Utility.GenerateDataTableForExportView(exportJsonEqmntConfgnVariables);
            DataTable ControlLocationDataTable = Utility.GenerateDataTableForExportView(exportJsonControlLocationVariables);
            DataTable UnitConfigurationDataTable = Utility.GenerateDataTableForExportView(exportJsonUnitConfigurationVariables);
            List<SqlParameter> lstSqlParameter = Utility.SqlParameterForExportView(opportunityId, buildingDataTable, buildinEqmntConsoleDataTable, buildinEqmntConfigureDataTable, ControlLocationDataTable, UnitConfigurationDataTable);
            DataSet ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETVALUESFORVIEWEXPORT, lstSqlParameter);
            ViewExportDetails viewExportDetails = new ViewExportDetails()
            {
                Units = new List<Identifications>(),
                Quotation = new QuotationDetails()
                {
                    OpportunityInfo = new OpportunityValues(),
                    Quote = new Quote(),
                    UnitMaterials = new List<JObject>(),
                }
            };
            Quote quote = new Quote();
            OpportunityValues opportunityValues = new OpportunityValues()
            {
                OpportunityId = opportunityId
            };
            QuotationDetails quotationDetails = new QuotationDetails()
            {
                OpportunityInfo = opportunityValues,
                Quote = quote,
            };
            List<UnitMaterials> ListUnitMaterials = new List<UnitMaterials>();
            var results = opportunityId.Split(Constant.HYPHEN);
            var equipmentPerElevatore = new List<Equipment>();

            viewExportDetails.Quotation.Quote.QuoteCreatedDate = new DateFormatClass()
            {
                DateValue = DateTime.Now.ToString(Constant.DATETIMESTRINGFORVIEWEXPORT),
                DateFormat = Constant.DATETIMESTRINGFORVIEWEXPORT
            };
            viewExportDetails.Quotation.Quote.QuoteLastModifiedDate = new DateFormatClass()
            {
                DateValue = DateTime.Now.ToString(Constant.DATETIMESTRINGFORVIEWEXPORT),
                DateFormat = Constant.DATETIMESTRINGFORVIEWEXPORT
            };
            viewExportDetails.Quotation.OpportunityInfo.FactoryQuoteCurrency = Constant.USD;

            if (ds != null && ds.Tables.Count > 0)
            {
                var oppDetails = (from DataRow dRow in ds.Tables[7].Rows
                                  select new
                                  {
                                      oppIdid = Convert.ToString(dRow[Constant.OPPORTUNITY]),
                                      versionId = Convert.ToString(dRow[Constant.VERSIONIDFORCRM]),
                                      createdBy = Convert.ToString(dRow[Constant.CREATEDBYCRM]),
                                      createdOn = Convert.ToString(dRow[Constant.CREATEDONCRM]),
                                      modifiedOn = Convert.ToString(dRow[Constant.MODIFIEDONCRM]),
                                      StatusName = Convert.ToString(dRow[Constant.STATUSNAME])
                                  }).Distinct().ToList();
                foreach (var item in oppDetails)
                {
                    viewExportDetails.Quotation.OpportunityInfo.OpportunityId = item.oppIdid;
                    viewExportDetails.Quotation.Quote.VIEW_Version = item.versionId;
                    viewExportDetails.Quotation.Quote.QuoteNumber = opportunityId;
                    viewExportDetails.Quotation.OpportunityInfo.BaseBidCreator = item.createdBy;
                    viewExportDetails.Quotation.Quote.QuoteCreatedDate.DateValue = Convert.ToDateTime(item.createdOn).ToString(Constant.DATETIMESTRINGFORVIEWEXPORT);
                    viewExportDetails.Quotation.Quote.QuoteCreatedDate.DateFormat = Constant.DATETIMESTRINGFORVIEWEXPORT;
                    viewExportDetails.Quotation.Quote.QuoteLastModifiedDate = new DateFormatClass();
                    viewExportDetails.Quotation.Quote.QuoteStatus = item.StatusName;
                    viewExportDetails.Quotation.OpportunityInfo.ValidityDate = new DateFormatClass()
                    {
                        DateFormat = Constant.DATETIMESTRINGFORVIEWEXPORT,
                        DateValue = DateTime.Now.AddDays(30).ToString(Constant.DATETIMESTRINGFORVIEWEXPORT)
                    };
                    if (Utility.CheckEquals(item.StatusName, Constant.INVALID))
                    {
                        viewExportDetails.Quotation.OpportunityInfo.ValidityDate.DateValue = null;
                    }
                    if (!string.IsNullOrEmpty(item.modifiedOn))
                    {
                        viewExportDetails.Quotation.Quote.QuoteLastModifiedDate.DateValue = Convert.ToDateTime(item.modifiedOn).ToString(Constant.DATETIMESTRINGFORVIEWEXPORT);
                    }
                    else
                    {
                        viewExportDetails.Quotation.Quote.QuoteLastModifiedDate.DateValue = viewExportDetails.Quotation.Quote.QuoteCreatedDate.DateValue;
                    }
                    viewExportDetails.Quotation.Quote.QuoteLastModifiedDate.DateFormat = Constant.DATETIMESTRINGFORVIEWEXPORT;
                }
                var buildingidList = (from DataRow dRow in ds.Tables[0].Rows
                                      select new
                                      {
                                          id = Convert.ToInt32(dRow[Constant.BUILDINGIDCOLUMNNAME])
                                      }).Distinct().ToList();
                var buildingConfigList = (from DataRow dRow in ds.Tables[0].Rows
                                          select new
                                          {
                                              id = Convert.ToInt32(dRow[Constant.BUILDINGIDCOLUMNNAME]),
                                              variableId = Convert.ToString(dRow[Constant.BUINDINGTYPE]),
                                              variableValue = Convert.ToString(dRow[Constant.BUINDINGVALUE])
                                          }).Distinct().ToList();
                var buildingEqmntConfigList = (from DataRow dRow in ds.Tables[1].Rows
                                               select new
                                               {
                                                   id = Convert.ToInt32(dRow[Constant.BUILDINGIDCOLUMNNAME]),
                                                   variableId = Convert.ToString(dRow[Constant.VARIABLETYPE]),
                                                   variableValue = Convert.ToString(dRow[Constant.VALUE])
                                               }).Distinct().ToList();
                var buildingEqmntConsoleList = (from DataRow dRow in ds.Tables[2].Rows
                                                select new
                                                {
                                                    id = Convert.ToInt32(dRow[Constant.BUILDINGIDCOLUMNNAME]),
                                                    variableId = Convert.ToString(dRow[Constant.VARIABLETYPE]),
                                                    variableValue = Convert.ToString(dRow[Constant.VALUE])
                                                }).Distinct().ToList();
                var groupIdList = (from DataRow dRow in ds.Tables[3].Rows
                                   select new
                                   {
                                       buildingId = Convert.ToInt32(dRow[Constant.BUILDINGIDCOLUMNNAME]),
                                       groupId = Convert.ToInt32(dRow[Constant.GROUPIDCOLUMNNAME]),
                                       groupName = Convert.ToString(dRow[Constant.GROUPNAMECOLUMNNAME])
                                   }).Distinct().ToList();
                var controlLocationList = (from DataRow dRow in ds.Tables[4].Rows
                                           select new
                                           {
                                               groupId = Convert.ToInt32(dRow[Constant.GROUPCONFIGID]),
                                               controlLocationType = Convert.ToString(dRow[Constant.CONTROLOCATIONTYPE]),
                                               controlLocationValue = Convert.ToString(dRow[Constant.CONTROLOCATIONVALUE])
                                           }).Distinct().ToList();
                var unitIDList = (from DataRow dRow in ds.Tables[5].Rows
                                  select new
                                  {
                                      groupId = Convert.ToInt32(dRow[Constant.GROUPCONFIGID]),
                                      unitId = Convert.ToInt32(dRow[Constant.UNITID]),
                                      UEID = Convert.ToString(dRow[Constant.UEID]),
                                      setId = Convert.ToInt32(dRow[Constant.SETCONFIGURATIONID]),
                                      setDescription = Convert.ToString(dRow[Constant.SETDESCRIPTION]),
                                      productName = Convert.ToString(dRow[Constant.PRODUCTNAMECOLUMN]),
                                      unitDesignation = Convert.ToString(dRow[Constant.DESIGNATION]),
                                      travelFeet = Convert.ToInt32(dRow[Constant.TRAVELFEET]),
                                      travelInch = Convert.ToDecimal(dRow[Constant.TRAVELINCH]),
                                      frontOpening = Convert.ToInt32(dRow[Constant.FRONTOPENING]),
                                      rearOpening = Convert.ToInt32(dRow[Constant.REAROPENING]),
                                      createdon = Convert.ToDateTime(dRow[Constant.CREATEDON]),
                                      status = Convert.ToString(dRow[Constant.STATUSNAME])
                                  }).Distinct().ToList();
                var unitList = (from DataRow dRow in ds.Tables[6].Rows
                                select new
                                {
                                    unitId = Convert.ToInt32(dRow[Constant.UNITID]),
                                    variableId = Convert.ToString(dRow[Constant.CONFIGVARIABLES]),
                                    value = Convert.ToString(dRow[Constant.CONFIGUREVALUES])
                                }).Distinct().ToList();
                var priceList = (from DataRow dRow in ds.Tables[10].Rows
                                 select new
                                 {
                                     unitId = Convert.ToInt32(dRow[Constant.UNITID]),
                                     variableId = Convert.ToString(dRow[Constant.VARIABLEID]),
                                     value = Convert.ToString(dRow[Constant.VARIABLEVALUE])
                                 }).Distinct().ToList();
                var leadTimeList = (from DataRow dRow in ds.Tables[11].Rows
                                    select new
                                    {
                                        setId = Convert.ToInt32(dRow[Constant.SETIDLOWERCASE]),
                                        variableId = Convert.ToString(dRow[Constant.VARIABLEID]),
                                        value = Convert.ToString(dRow[Constant.VARIABLEVALUE])
                                    }).Distinct().ToList();
                var systemValidationVariables = (from DataRow dRow in ds.Tables[12].Rows
                                                 select new
                                                 {
                                                     SetId = Convert.ToInt32(dRow[Constant.SETIDLOWERCASE]),
                                                     VariableId = Convert.ToString(dRow[Constant.VARIABLEID]),
                                                     Value = Convert.ToString(dRow[Constant.VARIABLEVALUE])
                                                 }).Distinct().ToList();
                var viewVariables = Utility.DeserializeObjectValue<Dictionary<string, ViewVariableAndDataType>>(System.IO.File.ReadAllText(Constant.JSONFIELDS));
                var viewHardCodedValues = Utility.VariableMapper(Constant.PROJECTCONSTANTMAPPER, Constant.PRODUCTREE);
                var enrichedData = JObject.Parse(File.ReadAllText(Constant.PROJECTCONSTANTMAPPER));
                var viewEnrichedValues = enrichedData[Constant.VIEWENRICHMENTS].ToObject<Dictionary<string, List<Enrichment>>>();
                string type = string.Empty;
                List<SqlParameter> lstSqlParameterforOz = Utility.SqlParameterForOzView(opportunityId);
                DataSet dsOZ = CpqDatabaseManager.ExecuteDataSet(Constant.GETVARIABLEVALUESFOROZ, lstSqlParameterforOz);
                List<Equipment> equipment = new List<Equipment>();
                RequestedDrawing requestedDrawing = new RequestedDrawing();
                if (dsOZ != null && dsOZ.Tables.Count > 1 && dsOZ.Tables[0].Rows.Count > 0)
                {
                    var unitIDListOZ = (from DataRow dRow in dsOZ.Tables[0].Rows
                                        select new
                                        {
                                            GroupId = Convert.ToInt32(dRow[Constant.GROUPID]),
                                            unitId = Convert.ToInt32(dRow[Constant.UNITID]),
                                            UEID = (dRow[Constant.UEID].ToString()),
                                            productName = (dRow[Constant.PRODUCTNAMECOLUMN].ToString()),
                                            unitDesignation = (dRow[Constant.DESIGNATION].ToString()),
                                            DoDQuestions = dRow[Constant.QUESTIONS].ToString(),

                                        }).Distinct().ToList();
                    var fdaDrawingList = (from DataRow dRow in dsOZ.Tables[1].Rows
                                          select new
                                          {
                                              FDAType = (dRow[Constant.FDATYPE]).ToString()

                                          }).Distinct().ToList();
                    var fdaList = Utility.SerializeObjectValue(fdaDrawingList);
                    var fdaDrawing = Utility.DeserializeObjectValue<List<FdaOz>>(fdaList);
                    requestedDrawing = Utility.GetRequestedDrawingDetails(fdaDrawing);
                    var unitlistOZ = Utility.SerializeObjectValue(unitIDListOZ);
                    var unitidlistOZ = Utility.DeserializeObjectValue<List<UnitDetailsOz>>(unitlistOZ);
                    equipment = Utility.GenerateDoDDetails(unitidlistOZ);
                }
                foreach (var buildingId in buildingidList)
                {
                    var buildingConfigListForbuildingId = buildingConfigList.Where(x => x.id.Equals(buildingId.id)).Distinct().ToList();
                    var buildingEqmntConfigListForbuildingId = buildingEqmntConfigList.Where(x => x.id.Equals(buildingId.id)).Distinct().ToList();
                    var buildingEqmntConsoleListForbuildingId = buildingEqmntConsoleList.Where(x => x.id.Equals(buildingId.id)).Distinct().ToList();
                    var groupListForBuilding = groupIdList.Where(x => x.buildingId.Equals(buildingId.id)).Distinct().ToList();
                    foreach (var group in groupListForBuilding)
                    {
                        var unitIDListForGroup = unitIDList.Where(x => x.groupId.Equals(group.groupId)).Distinct().ToList();
                        foreach (var unit in unitIDListForGroup)
                        {
                            if (unit.UEID != string.Empty)
                            {
                                var unitJson = new JObject();
                                dynamic json = Utility.DeserializeObjectValue<dynamic>(Utility.SerializeObjectValue(unitJson));
                                foreach (var jsonField in viewVariables.ToList())
                                {
                                    switch (jsonField.Value.type.ToUpper())
                                    {
                                        case Constant.INT:
                                            json[jsonField.Key] = 0;
                                            break;
                                        case Constant.BOOLEAN:
                                            json[jsonField.Key] = false;
                                            break;
                                        case Constant.DECIMAL:
                                            json[jsonField.Key] = 0.0;
                                            break;
                                        default:
                                            json[jsonField.Key] = null;
                                            break;
                                    }
                                }

                                json[Constant.GROUPIDUPPER] = Convert.ToString(group.groupId);
                                json[Constant.BUILDINGIDUPPER] = Convert.ToString(buildingId.id);
                                json[Constant.GROUPNAME] = group.groupName;
                                json[Constant.LINEID] = unit.UEID;
                                json[Constant.UNITNICKNAME] = unit.unitDesignation;
                                json[Constant.UNITDESIGNATION] = unit.unitDesignation;
                                json[Constant.FRONTOPENINGS] = unit.frontOpening;
                                json[Constant.REAROPENINGS] = unit.rearOpening;
                                json[Constant.SETDESCRIPTION] = unit.setDescription;
                                json[Constant.ESTIMATESATUS] = unit.status;

                                json[Constant.NUMBEROFSTOPS] = CalculateNumberofStops(ds.Tables[8], unit.unitId);
                                json[Constant.NUMBEROFGROUPSINBUILDING] = groupListForBuilding.Count();
                                json[Constant.NUMUNITSINGROUP] = unitIDListForGroup.Count();

                                var buildingDefaultvalues = defaultVtPackageValues.Where(x => x.BuildingId == buildingId.id).Distinct().ToList();
                                foreach (var building in buildingDefaultvalues)
                                {
                                    var groupvariables = building.GroupVariableAssignment.Where(x => x.GroupId == group.groupId).Distinct().ToList();
                                    foreach (var groupVariableAssignment in groupvariables)
                                    {
                                        var leadTimeListForSet = leadTimeList.Where(x => x.setId == unit.setId).Distinct().ToList();
                                        foreach (var item in leadTimeListForSet)
                                        {
                                            switch (item.variableId.ToUpper())
                                            {
                                                case Constant.BATCH1LEADTIMECAPS:
                                                    json[Constant.BATCH1LEADTIME] = Convert.ToInt32(item.value);
                                                    break;
                                                case Constant.BATCH2LEADTIMECAPS:
                                                    json[Constant.BATCH2LEADTIME] = Convert.ToInt32(item.value);
                                                    break;
                                                case Constant.BATCH3LEADTIMECAPS:
                                                    json[Constant.BATCH3LEADTIME] = Convert.ToInt32(item.value);
                                                    break;
                                                case Constant.BATCH4LEADTIMECAPS:
                                                    json[Constant.BATCH4LEADTIME] = Convert.ToInt32(item.value);
                                                    break;
                                                case Constant.BATCH5LEADTIMECAPS:
                                                    json[Constant.BATCH5LEADTIME] = Convert.ToInt32(item.value);
                                                    break;
                                                case Constant.BATCH6LEADTIMECAPS:
                                                    json[Constant.BATCH6LEADTIME] = Convert.ToInt32(item.value);
                                                    break;
                                                case Constant.MANUFACTURINGLEADTIMECAPS:
                                                    json[Constant.MANUFACTURINGLEADTIME] = Convert.ToInt32(item.value);
                                                    break;
                                            }
                                        }

                                        var sets = groupVariableAssignment.SetVariableAssignment.Where(x => x.SetId == unit.setId).Distinct().ToList();
                                        foreach (var set in sets)
                                        {

                                            foreach (var variableAssignmentz in set.UnitVariableAssignments)
                                            {
                                                foreach (var variable in viewVariables)
                                                {
                                                    if (variableAssignmentz.VariableId.ToUpper().EndsWith(variable.Value.value.ToUpper()))
                                                    {
                                                        if (viewEnrichedValues.ContainsKey(variable.Value.value) && viewEnrichedValues[variable.Value.value].Where(x => x.Value.Equals(Convert.ToString(variableAssignmentz.Value))).Any())
                                                        {
                                                            json[variable.Key] = viewEnrichedValues[variable.Value.value].Where(x => x.Value.Equals(Convert.ToString(variableAssignmentz.Value))).FirstOrDefault().ViewValue;
                                                        }
                                                        else
                                                        {
                                                            json[variable.Key] = Utility.ConvertDataTypeForView(variable.Value.type, variableAssignmentz.Value);
                                                        }
                                                    }
                                                }
                                            }
                                            foreach (var variableAssignmentz in set.ProductTreeVariables.ToList())
                                            {
                                                foreach (var variable in viewVariables)
                                                {
                                                    if (Utility.CheckEquals(variableAssignmentz.Key, variable.Value.value))
                                                    {
                                                        json[variable.Key] = Utility.ConvertDataTypeForView(variable.Value.type, variableAssignmentz.Value);
                                                        if (Utility.CheckEquals(variable.Value.type, Constant.STRINGVARIABLENAME))
                                                        {
                                                            var productTreeVariable = Convert.ToString(variableAssignmentz.Value).Replace(Constant.UNDERSCORE, Constant.EMPTYSPACE);
                                                            productTreeVariable = productTreeVariable.Replace(Constant.BY, Constant.SLASH);
                                                            json[variable.Key] = productTreeVariable;
                                                        }
                                                        else
                                                        {
                                                            json[variable.Key] = Utility.ConvertDataTypeForView(variable.Value.type, variableAssignmentz.Value);
                                                        }
                                                    }
                                                }
                                            }
                                            var productTechnology = Utility.VariableMapper(Constants.PROJECTCONSTANTMAPPER, Constant.VARIABLES);
                                            var productTechnologyA = set.ProductTreeVariables.ContainsKey(productTechnology[Constants.PRODUCTTECHNOLOGYA]) ? Convert.ToString(set.ProductTreeVariables[productTechnology[Constants.PRODUCTTECHNOLOGYA]]):string.Empty;
                                            var leadTimeData= JObject.Parse(File.ReadAllText(Constants.PROJECTCONSTANTMAPPER)).ToObject<Dictionary<string, Object>>();
                                            if(leadTimeData.ContainsKey(productTechnologyA.ToUpper()))
                                            {
                                                var leadTimeDefaultValues = Utility.DeserializeObjectValue<Dictionary<string, Int32>>(Utility.SerializeObjectValue(leadTimeData[productTechnologyA.ToUpper()]));
                                                foreach (var leadTime in leadTimeDefaultValues)
                                                {
                                                    if(json[leadTime.Key]<leadTime.Value)
                                                    {
                                                        json[leadTime.Key] = leadTime.Value;
                                                    }
                                                }
                                            }

                                        }
                                        foreach (var variableAsgmt in groupVariableAssignment.GroupVariableAssignments)
                                        {

                                            foreach (var variable in viewVariables)
                                            {
                                                if (variableAsgmt.VariableId.ToUpper().EndsWith(variable.Value.value.ToUpper()))
                                                {
                                                    if (viewEnrichedValues.ContainsKey(variable.Value.value) && viewEnrichedValues[variable.Value.value].Where(x => x.Value.Equals(Convert.ToString(variableAsgmt.Value))).Any())
                                                    {
                                                        json[variable.Key] = viewEnrichedValues[variable.Value.value].Where(x => x.Value.Equals(Convert.ToString(variableAsgmt.Value))).FirstOrDefault().ViewValue;
                                                    }
                                                    else
                                                    {
                                                        json[variable.Key] = Utility.ConvertDataTypeForView(variable.Value.type, variableAsgmt.Value);
                                                    }
                                                }
                                            }
                                        }

                                        if (!groupVariableAssignment.isNCP)
                                        {
                                            json[Constant.TRAVEL] = Convert.ToString(Convert.ToDecimal(unit.travelFeet + decimal.Divide(unit.travelInch, 12)));
                                            var travel = Convert.ToString(json[Constant.TRAVEL]).Split(Constant.DOT);
                                            if (travel.Length > 1)
                                            {
                                                if (Convert.ToString(travel[1]).Length > 1)
                                                {
                                                    json[Constant.TRAVEL] = Convert.ToString(travel[0]) + Constant.DOT + Convert.ToString(travel[1]).Substring(0, 2);
                                                }
                                                else
                                                {
                                                    json[Constant.TRAVEL] = Convert.ToString(travel[0]) + Constant.DOT + Convert.ToString(travel[1]) + Constant.ZERO;
                                                }
                                            }
                                            else
                                            {
                                                json[Constant.TRAVEL] = Convert.ToString(travel[0]) + Constant.DOT + Constant.ZEROZERO;
                                            }
                                        }
                                    }
                                    foreach (var variableAssignment in building.BuildingVariableAssignments)
                                    {
                                        foreach (var variable in viewVariables)
                                        {
                                            if (variableAssignment.VariableId.ToUpper().EndsWith(variable.Value.value.ToUpper()))
                                            {
                                                if (viewEnrichedValues.ContainsKey(variable.Value.value) && viewEnrichedValues[variable.Value.value].Where(x => x.Value.Equals(Convert.ToString(variableAssignment.Value))).Any())
                                                {
                                                    json[variable.Key] = viewEnrichedValues[variable.Value.value].Where(x => x.Value.Equals(Convert.ToString(variableAssignment.Value))).FirstOrDefault().ViewValue;
                                                }
                                                else
                                                {
                                                    json[variable.Key] = Utility.ConvertDataTypeForView(variable.Value.type, variableAssignment.Value);
                                                }
                                            }
                                        }
                                    }

                                }
                                foreach (var building in buildingConfigListForbuildingId)
                                {
                                    if (viewVariables.ContainsKey(building.variableId) && building.variableValue != null)
                                    {
                                        json[viewVariables[building.variableId].value] = Utility.ConvertDataTypeForView(viewVariables[building.variableId].type, building.variableValue);

                                    }
                                }
                                foreach (var buildingEquipmentVariable in buildingEqmntConfigListForbuildingId)
                                {
                                    if (viewVariables.ContainsKey(buildingEquipmentVariable.variableId) && buildingEquipmentVariable.variableValue != null)
                                    {
                                        json[viewVariables[buildingEquipmentVariable.variableId].value] = Utility.ConvertDataTypeForView(viewVariables[buildingEquipmentVariable.variableId].type, buildingEquipmentVariable.variableValue);

                                    }
                                }
                                foreach (var buildingEquipmentVariable in buildingEqmntConsoleListForbuildingId)
                                {
                                    if (viewVariables.ContainsKey(buildingEquipmentVariable.variableId) && buildingEquipmentVariable.variableValue != null)
                                    {
                                        json[viewVariables[buildingEquipmentVariable.variableId].value] = Utility.ConvertDataTypeForView(viewVariables[buildingEquipmentVariable.variableId].type, buildingEquipmentVariable.variableValue);

                                    }
                                }
                                var priceListForUnit = priceList.Where(x => x.unitId.Equals(unit.unitId)).Distinct().ToList();
                                foreach (var priceItem in priceListForUnit)
                                {
                                    var priceValue = string.IsNullOrEmpty(priceItem.value) ? Convert.ToString(0) : priceItem.value;
                                    if (Utility.CheckEquals(priceItem.variableId, Constants.UNITPRICE))
                                    {
                                        json[Constant.FACTORYMATERIALCOST] = priceValue;
                                        json[Constant.MATERIALCOST] = priceValue;
                                    }
                                    else if (Utility.CheckEquals(priceItem.variableId, Constants.CORPORATEASSISTANCECAMALCASE))
                                    {
                                        json[Constants.CORPORATEASSISTANCE] = priceValue;
                                    }
                                    else if (Utility.CheckEquals(priceItem.variableId, Constants.CORPORATESUBSIDIES))
                                    {
                                        json[Constants.PRODUCTSUBSIDIES] = priceValue;
                                    }
                                    else if (Utility.CheckEquals(priceItem.variableId, Constants.STRATEGICDISCOUNT))
                                    {
                                        json[Constants.MANUFACTURINGDISCOUNTS] = priceValue;
                                    }
                                }
                                var unitIDListForUnit = unitList.Where(x => x.unitId.Equals(unit.unitId)).Distinct().ToList();
                                var controlLocationGroupList = controlLocationList.Where(x => x.groupId.Equals(group.groupId)).Distinct().ToList();
                                foreach (var controlLocationdata in controlLocationGroupList)
                                {
                                    if (viewVariables.ContainsKey(controlLocationdata.controlLocationType) && controlLocationdata.controlLocationValue != null)
                                    {
                                        json[viewVariables[controlLocationdata.controlLocationType].value] = Utility.ConvertDataTypeForView(viewVariables[controlLocationdata.controlLocationType].type, controlLocationdata.controlLocationValue);


                                    }
                                }
                                foreach (var unitVariable in unitIDListForUnit)
                                {
                                    if (viewVariables.ContainsKey(unitVariable.variableId))
                                    {
                                        if (unitVariable.value != null)
                                        {
                                            type = viewVariables[unitVariable.variableId].type;
                                            switch (type.ToUpper())
                                            {
                                                case Constant.INT:
                                                    json[viewVariables[unitVariable.variableId].value] = Convert.ToInt32(unitVariable.value);
                                                    break;
                                                case Constant.BOOLEAN:
                                                    json[viewVariables[unitVariable.variableId].value] = Convert.ToBoolean(unitVariable.value);
                                                    break;
                                                case Constant.DECIMAL:
                                                    json[viewVariables[unitVariable.variableId].value] = Convert.ToDecimal(unitVariable.value);
                                                    break;
                                                default:
                                                    json[viewVariables[unitVariable.variableId].value] = unitVariable.value;
                                                    break;
                                            }
                                        }
                                    }
                                }
                                equipmentPerElevatore = equipment.Where(x => x.EstimateIdentifier.LineId.Equals(unit.UEID)).Distinct().ToList();
                                var unitIdentification = new Identifications();
                                var ueid = new Unitsection()
                                {
                                    UEID = unit.UEID
                                };
                                unitIdentification.Identification = ueid;
                                viewExportDetails.Units.Add(unitIdentification);
                                foreach (var unitOzData in equipmentPerElevatore)
                                {
                                    if (Utility.CheckEquals(unitOzData.EstimateIdentifier.LineId, unit.UEID))
                                    {
                                        json[Constant.ISDOD] = unitOzData.DesignOnDemand.IsDoD;
                                        json[Constant.OUTFORAPPROVAL] = unitOzData.DesignOnDemand.OutForApproval;
                                        json[Constant.FORFINAL] = unitOzData.DesignOnDemand.ForFinal;
                                        json[Constant.FORREVISERESUBMIT] = unitOzData.DesignOnDemand.ForReviseResubmit;
                                        if (!string.IsNullOrEmpty(unitOzData.DesignOnDemand.ReceivedDate))
                                        {
                                            json[Constant.RECEIVEDDATE] = Convert.ToDateTime(unitOzData.DesignOnDemand.ReceivedDate).ToString((Constant.DATETIMESTRINGFORVIEWEXPORT));
                                        }
                                        if (!string.IsNullOrEmpty(unitOzData.DesignOnDemand.SentDate))
                                        {
                                            json[Constant.SENTDATE] = Convert.ToDateTime(unitOzData.DesignOnDemand.SentDate).ToString((Constant.DATETIMESTRINGFORVIEWEXPORT));
                                        }
                                        json[Constant.LAYOUTSUBMITTAL] = requestedDrawing.Layout;
                                        json[Constant.ENTRANCESUBMITTAL] = requestedDrawing.Entrance;
                                        json[Constant.CABSUBMITTAL] = requestedDrawing.Cab;
                                        json[Constant.CARFIXTURESUBMITTAL] = requestedDrawing.CarStation;
                                        json[Constant.HALLFIXTURESUBMITTAL] = requestedDrawing.HallStation;
                                        json[Constant.LOBBYPANELSUBMITTAL] = requestedDrawing.LobbyPanel;
                                        json[Constant.FREIGHTSUBMITTAL] = requestedDrawing.Freight;
                                    }
                                }
                                foreach (var defaultValue in viewHardCodedValues)
                                {
                                    json[defaultValue.Key] = defaultValue.Value;
                                }

                                var systemValidationVariablesForSet = systemValidationVariables.Where(x => x.SetId == unit.setId).Distinct().ToList();
                                foreach (var systemVariable in systemValidationVariablesForSet)
                                {
                                    foreach (var variable in viewVariables)
                                    {
                                        if (systemVariable.VariableId.EndsWith(variable.Value.value))
                                        {
                                            json[variable.Key] = Utility.ConvertDataTypeForView(variable.Value.type, systemVariable.Value);
                                        }
                                    }
                                }


                                viewExportDetails.Quotation.UnitMaterials.Add(json);

                            }
                        }
                    }
                }
            }
            Utility.LogTrace(Constant.GENARETAREQUESTBODYFORVIEWEXPORTCOMPLETED);
            Utility.LogEnd(methodStartTime);
            return viewExportDetails;
        }

        /// <summary>
        /// To Save Configuration To View
        /// </summary>
        /// <param Name="viewPostUrl"></param>
        /// <param Name="viewUserName"></param>
        /// <param Name="viewPassword"></param>
        /// <param Name="requestBody"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> SaveConfigurationToView(ResponseMessage requestBody)
        {
            var methodStartTime = Utility.LogBegin();
            var viewSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constant.PROJECTMMANAGEMENTSETTINGS);
            var viewUserName = Utility.GetPropertyValue(viewSettings, Constant.APIUSERNAME);
            var viewPassword = Utility.GetPropertyValue(viewSettings, Constant.APIPASSWORD);
            var viewExportDetails = Utility.DeserializeObjectValue<ViewExportDetails>(Utility.SerializeObjectValue(requestBody.Response));


            var requestObject = new HttpClientRequestModel()
            {
                Method = HTTPMETHODTYPE.POST,
                RequestHeaders = new Dictionary<string, string>()
                {
                    { "Authorization", Constant.BASIC + Utility.GenerateBasicAuthTocken(viewUserName, viewPassword) }
                },
                BaseUrl = Utility.GetPropertyValue(viewSettings, Constant.BASEURL),
                EndPoint = Utility.GetPropertyValue(viewSettings, Constant.SAVEQUOTEAPI),
                RequestBody = JObject.FromObject(viewExportDetails, new Newtonsoft.Json.JsonSerializer())
            };

            var response = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(false);
            var responseContent = String.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result) ? Constant.VIEWERROEMESSAGE : response.Content.ReadAsStringAsync().Result;
            JObject projectDetails = JObject.Parse(responseContent.ToString());
            Utility.LogEnd(methodStartTime);
            return new ResponseMessage { StatusCode = Constant.SUCCESS, Response = projectDetails };
        }

        /// <summary>
        /// To Get Mini Project Values
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="userDetails"></param>
        /// <param Name="projectId"></param>
        /// <param Name="enrichedData"></param>
        /// <returns></returns>
        public CreateProjectResponseObject GetMiniProjectValues(string sessionId, string userDetails, string projectId, CreateProjectResponseObject enrichedData, int versionId = 1)
        {
            var methodStartTime = Utility.LogBegin();
            List<SqlParameter> sp = new List<SqlParameter>();
            DataSet basicInfoDataSet = new DataSet();
            //get User Response
            var cachedUserDetail = _cacheManager.GetCache(sessionId, _environment, Constants.USERDETAILSCPQ);
            User currentUser = new User();
            if (!string.IsNullOrEmpty(cachedUserDetail))
            {
                currentUser = Utility.DeserializeObjectValue<User>(cachedUserDetail);
            }

            IList<SqlParameter> lstSqlParameter = Utility.GetSqlParametersForMiniProject(userDetails, projectId, versionId);
            basicInfoDataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETMINIPROJECTVALUES, lstSqlParameter);
            if (basicInfoDataSet != null && basicInfoDataSet.Tables.Count > 0)
            {
                var branchSpValues = basicInfoDataSet.Tables[0];
                if (branchSpValues.Rows.Count > 0)
                {
                    foreach (var item in enrichedData.Sections[0].Variables)
                    {
                        if (Utility.CheckEquals(item.Id, "Projects.Branch"))
                        {
                            item.Values = new List<Values>();
                            foreach (DataRow item1 in branchSpValues.Rows)
                            {
                                var listData = new Values();
                                var branchGUID = item1[Constants.BRANCH_GUID].ToString();
                                if(string.IsNullOrEmpty(branchGUID) || !currentUser.Groups.Contains(branchGUID))
                                {
                                    continue;
                                }
                                var branchData = item1[Constant.BRANCHDETAILS].ToString();
                                if (branchData.Contains('\r'))
                                {
                                    listData.value = branchData.Split('\r')[0].ToString();
                                }
                                else
                                {
                                    listData.value = branchData;
                                }
                                item.Values.Add(listData);
                            }
                        }
                    }
                }
                var measuringUnits = basicInfoDataSet.Tables[1];
                if (measuringUnits.Rows.Count > 0)
                {
                    foreach (var item in enrichedData.Sections[0].Variables)
                    {
                        if (Utility.CheckEquals(item.Id, "LayoutDetails.MeasuringUnit"))
                        {
                            item.Values = new List<Values>();
                            foreach (DataRow item1 in measuringUnits.Rows)
                            {
                                var listData = new Values();
                                var measureUnitData = item1[Constant.MEASURINGUNITDETAILS].ToString();
                                if (measureUnitData.Contains('\r'))
                                {
                                    listData.value = measureUnitData.Split('\r')[0].ToString();
                                }
                                else
                                {
                                    listData.value = measureUnitData;
                                }
                                item.Values.Add(listData);
                            }
                        }
                    }
                }
                var salesDetails = basicInfoDataSet.Tables[2];
                if (salesDetails.Rows.Count > 0)
                {
                    foreach (var item in enrichedData.Sections[0].Variables)
                    {
                        if (Utility.CheckEquals(item.Id, "Projects.SalesStage"))
                        {
                            item.Values = new List<Values>();
                            foreach (DataRow item1 in salesDetails.Rows)
                            {
                                var listData = new Values();
                                var salesData = item1[Constant.SALESDETAILS].ToString();
                                if (salesData.Contains('\r'))
                                {
                                    listData.value = salesData.Split('\r')[0].ToString();
                                }
                                else
                                {
                                    listData.value = salesData;
                                }
                                item.Values.Add(listData);
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(projectId))
                {
                    ProjectDisplayDetails displayDetails = new ProjectDisplayDetails();
                    Dictionary<string, string> variableDetails = new Dictionary<string, string>();
                    VariableDetails addressVariableValues = new VariableDetails();
                    var porjectTableValues = basicInfoDataSet.Tables[3];
                    if (porjectTableValues.Rows.Count > 0)
                    {
                        foreach (DataRow row in porjectTableValues.Rows)
                        {
                            displayDetails.ProjectId = row["OpportunityId"].ToString();
                            displayDetails.ProposedDate = Convert.ToDateTime(row["CreatedOn"]);
                            displayDetails.CreatedDate = Convert.ToDateTime(row["CreatedOn"]);
                            displayDetails.ModifiedDate = Convert.ToDateTime(row["CreatedOn"]);
                            displayDetails.ContractBookedDate = Convert.ToDateTime(row["CreatedOn"]);
                            displayDetails.QuoteId = row["QuoteId"].ToString();
                            displayDetails.QuoteStatus = row["QuoteStatusDisplayName"].ToString();
                            addressVariableValues.AccountName = row["AccountName"].ToString();
                            addressVariableValues.AddressLine1 = row["AddressLine1"].ToString();
                            addressVariableValues.AddressLine2 = row["AddressLine2"].ToString();
                            addressVariableValues.City = row["City"].ToString();
                            addressVariableValues.Country = Constant.CANADA.ToUpper();
                            addressVariableValues.State = row["State"].ToString();
                            addressVariableValues.ZipCode = row["Zipcode"].ToString();
                            addressVariableValues.Language = "EN-US";
                            addressVariableValues.MeasuringUnit = row["MeasuringUnit"].ToString();
                            addressVariableValues.Branch = row["Branch"].ToString();
                            addressVariableValues.ProjectId = row["OpportunityId"].ToString();
                            addressVariableValues.ProjectName = row["Name"].ToString();
                            addressVariableValues.SalesStage = row["Salesman"].ToString();
                            addressVariableValues.VersionId = (int)row["VersionId"];
                            addressVariableValues.Contact = row["CustomerNumber"].ToString();
                            addressVariableValues.Description = row["Description"].ToString();
                            addressVariableValues.AwardCloseDate = (row["AwardCloseDate"] != DBNull.Value) ? Convert.ToDateTime(row["AwardCloseDate"]) : DateTime.Now;
                            addressVariableValues.SalesRepEmail = row["SalesRepEmail"].ToString();
                            addressVariableValues.OperationContactEmail = row["OperationContactEmail"].ToString();
                        }

                        if (Utility.CheckEquals(addressVariableValues.SalesStage, Constant.BIDAWARDED))
                        {
                            foreach (var item in enrichedData.Sections[0].Variables)
                            {
                                if (Utility.CheckEquals(item.Id, Constant.AWARDCLOSEDATEJSON))
                                {
                                    foreach (var property in item.Properties)
                                    {
                                        if (Utility.CheckEquals(property.Id, Constant.ISEDITABLE))
                                        {
                                            property.Value = true;
                                        }
                                    }
                                }
                            }
                        }
                        var variableJsonValues = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(addressVariableValues));
                        variableDetails = variableJsonValues;
                        enrichedData.ProjectDisplayDetails = Utility.DeserializeObjectValue<Dictionary<string, string>>(Utility.SerializeObjectValue(displayDetails));
                        enrichedData.VariableDetails = variableDetails;
                    }
                }
            }
            Utility.LogEnd(methodStartTime);
            return enrichedData;
        }

        /// <summary>
        /// To Save And Update Mini Project Values
        /// </summary>
        /// <param Name="requestVariableDetails"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        public List<ResultProjectSave> SaveAndUpdateMiniProjectValues(VariableDetails requestVariableDetails, string userName, bool isAddQuote = false)
        {
            var methodStartTime = Utility.LogBegin();
            List<ConfigVariable> unitVariableAssignment = new List<ConfigVariable>();
            List<ResultProjectSave> lstResult = new List<ResultProjectSave>();
            List<SqlParameter> lstSqlParameter = new List<SqlParameter>();
            DataTable projectDataTable = Utility.GenerateDataTableForProjectsTable(requestVariableDetails, userName);
            DataTable accountDataTable = Utility.GenerateDataTableForProjectsInfoTable(null, requestVariableDetails);
            if (isAddQuote)
            {
                lstSqlParameter = Utility.SqlParameterForSaveAddNewQuoteValues(projectDataTable, userName, accountDataTable, 1);
            }
            else
            {
                lstSqlParameter = Utility.SqlParameterForSavinAndUpdateMiniProject(projectDataTable, userName, accountDataTable);
            }
            var resultForSaveGroupLayout = CpqDatabaseManager.ExecuteDataSet(Constant.USPSAVEPROJECTVALUES, lstSqlParameter);
            if (resultForSaveGroupLayout.Tables.Count > 0)
            {
                var quoteidList = (from DataRow dRow in resultForSaveGroupLayout.Tables[0].Rows
                                   select new { oppId = Convert.ToString(dRow[Constant.OPPORTUNITYIDVARIABLE]), quoteid = Convert.ToString(dRow[Constant.QUOTEID]), versionId = Convert.ToInt32(dRow[Constant.VERSIONID]) }).Distinct();
                foreach (var quote in quoteidList)
                {
                    ResultProjectSave result = new ResultProjectSave
                    {
                        Result = 1,
                        OpportunityId = quote.oppId,
                        QuoteId = quote.quoteid,
                        VersionId = quote.versionId,
                        Message = Constant.PROJECTUPDATEMESSAGE
                    };
                    lstResult.Add(result);
                }
            }
            else
            {
                ResultProjectSave result = new ResultProjectSave
                {
                    Result = -1,
                    Message = Constant.PROJECTERRORSAVEMESSAGE,
                    QuoteId = "",
                    OpportunityId = ""
                };
                lstResult.Add(result);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.SOMETHINGWENTWRONG,
                    Description = Constant.SOMETHINGWENTWRONG
                });
            }
            Utility.LogEnd(methodStartTime);
            return lstResult;
        }

        /// <summary>
        /// To Get List Of Projects Details
        /// </summary>
        /// <param Name="userName"></param>
        /// <param Name="countryCode"></param>
        /// <returns></returns>
        public List<ProjectResponseDetails> GetListOfProjectsDetailsDl(User currentUser)
        {
            var methodStartTime = Utility.LogBegin();
            List<ProjectResponseDetails> projectlist = new List<ProjectResponseDetails>();
            List<ProjectResponseDetails> projectlistFilteredValues = new List<ProjectResponseDetails>();
            ProjectResponseDetails project;
            List<SqlParameter> sp = new List<SqlParameter>();
            SqlParameter param = new SqlParameter(Constant.USERID, currentUser.UserId);
            DataTable groupGuidDataTable = Utility.DataTableForGuids(currentUser.Groups);
            sp = Utility.SqlParameterForGetUserDetails(groupGuidDataTable);
            SqlParameter countryCodeParam = new SqlParameter(Constant.COUNTRYFORPROJECTS, currentUser.Location.country);
            SqlParameter Role = new SqlParameter(Constant.ROLE, currentUser.Role.id);
            sp.Add(param);
            sp.Add(countryCodeParam);
            sp.Add(Role);
            DataSet ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETLISTOFPROJECTS, sp);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        project = new ProjectResponseDetails
                        {
                            OpportunityId = row[Constant.PROJECTOPPORTUNITYID].ToString(),
                            Name = row[Constant.PROJECTNAME].ToString()
                        };
                        var branchData = row[Constant.PROJECTBRANCHNAME].ToString();
                        if (branchData.Contains(Constant.PROJECTSPACECHARATER))
                        {
                            project.Branch = branchData.Split(Constant.PROJECTSPACECHARATER)[0].ToString();
                        }
                        else
                        {
                            project.Branch = branchData;
                        }
                        project.SalesMan = string.Empty;
                        project.SalesStage = new Status()
                        {
                            StatusKey = row[Constant.PROJECTSTATUSKEY].ToString(),
                            StatusName = row[Constant.PROJECTPROJSTATUSNAME].ToString(),
                            Description = row[Constant.PROJECTPROJSTATUSDESCRIPTION].ToString(),
                            DisplayName = row[Constant.PROJECTPROJSTATUSDISPLAYNAME].ToString()
                        };
                        project.CreatedDate = Convert.ToDateTime(row[Constant.PROJECTCREATEDON]);
                        project.LastModifiedDate = Convert.ToDateTime(row[Constant.PROJECTMODIFIEDON]);
                        if (ds.Tables[2].Rows.Count > 0)
                        {
                            foreach (DataRow rowValues in ds.Tables[2].Rows)
                            {
                                var opportunityIdAtCountValues = rowValues[Constant.PROJECTOPPORTUNITYID].ToString();

                                if (Utility.CheckEquals(opportunityIdAtCountValues, project.OpportunityId))
                                {
                                    project.QuoteCount = Convert.ToInt32(rowValues[Constant.QUOTECOUNT]);
                                }
                            }
                        }
                        if (Convert.ToInt32(row[Constant.PROJECTSOURCE]) == 1)
                        {
                            project.ProjectSource = new ProjectSource()
                            {
                                Id = 1,
                                Source = Constant.SC
                            };
                        }
                        else
                        {
                            project.ProjectSource = new ProjectSource()
                            {
                                Id = 2,
                                Source = Constant.VIEW
                            };

                        }
                        projectlistFilteredValues.Add(project);
                    }
                    projectlist = projectlistFilteredValues.GroupBy(projects => projects.OpportunityId).Select(projectsValues => projectsValues.FirstOrDefault()).ToList();
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        var opportunityIdAtTable = row[Constant.PROJECTOPPORTUNITYID].ToString();
                        if (projectlist != null && projectlist.Any())
                        {
                            var QuoteDetailsData = new List<QuoteDisplayDetails>();
                            foreach (var projectlistValues in projectlist)
                            {
                                if (Utility.CheckEquals(opportunityIdAtTable, projectlistValues.OpportunityId))
                                {
                                    var isPrimaryValue = false;
                                    if (ds.Tables[1].Columns.Contains(Constant.PRIMARYQUOTEID) && !string.IsNullOrEmpty(row[Constant.PRIMARYQUOTEID]?.ToString()))
                                    {
                                        isPrimaryValue = true;
                                    }
                                    var quoteValues = new QuoteDisplayDetails
                                    {
                                        QuoteId = row[Constant.PROJECTQUOTEID].ToString(),
                                        VersionId = row[Constant.PROJECTVERSIONID].ToString(),
                                        Description = row[Constant.PROJECTDESCRIPTION].ToString(),
                                        QuoteStatus = new Status()
                                        {
                                            StatusKey = row[Constant.PROJECTQUOTESTATUSKEY].ToString(),
                                            StatusName = row[Constant.PROJECTQUOTESTATUSNAME].ToString(),
                                            Description = row[Constant.PROJECTQUOTESTATUSDESCRIPTION].ToString(),
                                            DisplayName = row[Constant.PROJECTQUOTESTATUSDISPLAYNAME].ToString()
                                        },
                                        CreatedDate = Convert.ToDateTime(row[Constant.PROJECTCREATEDON]),
                                        ModifiedDate = Convert.ToDateTime(row[Constant.PROJECTMODIFIEDON]),
                                        IsPrimary = isPrimaryValue
                                    };
                                    var priceValues = new QuotePricingValues
                                    {
                                        Total = 0,
                                        Unit = Constant.PROJECTUSD
                                    };
                                    quoteValues.Pricing = priceValues;
                                    if (ds.Tables[2].Rows.Count > 0)
                                    {
                                        foreach (DataRow rowValues in ds.Tables[3].Rows)
                                        {
                                            var opportunityIdAtCountValues = rowValues[Constant.PROJECTQUOTEID].ToString();

                                            if (Utility.CheckEquals(opportunityIdAtCountValues, quoteValues.QuoteId))
                                            {
                                                quoteValues.UnitCount = Convert.ToInt32(rowValues[Constant.PROJECTUNITSCOUNTINQUOTES]);
                                            }

                                        }
                                        if (ds.Tables[4].Rows.Count > 0)
                                        {
                                            foreach (DataRow rowValues in ds.Tables[4].Rows)
                                            {
                                                var opportunityIdAtCountValues = rowValues[Constant.PROJECTQUOTEID].ToString();

                                                if (Utility.CheckEquals(opportunityIdAtCountValues, quoteValues.QuoteId))
                                                {
                                                    priceValues.Total = Convert.ToInt32(rowValues[Constant.PRICEFORQUOTE]);
                                                }

                                            }
                                        }
                                    }
                                    if (projectlistValues.Quotes == null)
                                    {
                                        QuoteDetailsData.Add(quoteValues);
                                        projectlistValues.Quotes = QuoteDetailsData;
                                    }
                                    else
                                    {
                                        projectlistValues.Quotes.Add(quoteValues);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Utility.LogEnd(methodStartTime);
            return projectlist;
        }

        /// <summary>
        /// Method to GenerateQuoteId
        /// </summary>
        /// <param Name="viewDetails"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        public List<ResultProjectSave> GenerateQuoteId(ViewProjectDetails viewDetails, string userName, int parentVersionId = 0)
        {
            var methodStartTime = Utility.LogBegin();
            using (var connection = CpqDatabaseManager.CreateSqlConnection())
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                SqlTransaction sqlTransaction = connection.BeginTransaction();
                cmd.Transaction = sqlTransaction;
                cmd.Connection = connection;
                try
                {
                    //sqlTransaction = connection.BeginTransaction();
                    var isNewQuote = false;
                    ResultProjectSave result = new ResultProjectSave();
                    List<ConfigVariable> unitVariableAssignment = new List<ConfigVariable>();
                    List<ResultProjectSave> lstResult = new List<ResultProjectSave>();
                    DataTable projectDataTable = Utility.GenerateDataTableForProjectsTableforViewUsers(viewDetails, userName);
                    DataTable unitIdentificationDataTable = Utility.GenerateDataTableForUnitIdentification(viewDetails);
                    IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForGeneratingQuoteId(projectDataTable,unitIdentificationDataTable, userName);

                    
                    cmd.CommandText = Constant.USPSAVEPROJECTVALUES;
                    cmd.CommandType = CommandType.StoredProcedure;

                    var resultForSaveGroupLayout = CpqDatabaseManager.ExcecuteSP(cmd, lstSqlParameter);
                    if (resultForSaveGroupLayout.Tables.Count > 0)
                    {
                        if (resultForSaveGroupLayout.Tables[0].Columns.Contains("InvalidBranchID"))
                        {
                            result.Result = 0;
                            result.Message = "Invalid Branch";
                            lstResult.Add(result);
                            return lstResult;
                        }

                        var quoteidList = (from DataRow dRow in resultForSaveGroupLayout.Tables[0].Rows
                                           select new
                                           {
                                               oppId = Convert.ToString(dRow[Constant.OPPORTUNITYIDVARIABLE]),
                                               quoteid = Convert.ToString(dRow[Constant.QUOTEID]),
                                               versionId = Convert.ToInt32(dRow[Constant.VERSIONID]),
                                               quoteStatus = Convert.ToString(dRow[Constant.PROJECTCOLUMNQUOTESTATUS]),
                                               IsNewQuote = Convert.ToBoolean(dRow[Constant.ISNEWQUOTE])
                                           }).Distinct();
                        foreach (var quote in quoteidList)
                        {
                            result.Result = 1;
                            result.OpportunityId = quote.oppId;
                            result.QuoteId = quote.quoteid;
                            result.VersionId = quote.versionId;
                            result.Message = Constant.PROJECTUPDATEMESSAGE;
                            isNewQuote = quote.IsNewQuote;
                            result.QuoteStatus = quote.quoteStatus;
                            lstResult.Add(result);
                        }
                    }
                    else
                    {
                        result.Result = -1;
                        result.Message = Constant.PROJECTERRORSAVEMESSAGE;
                        result.QuoteId = "";
                        result.OpportunityId = "";
                        result.QuoteStatus = string.Empty;
                        lstResult.Add(result);
                        throw new CustomException(new ResponseMessage
                        {
                            StatusCode = Constant.BADREQUEST,
                            Message = Constant.SOMETHINGWENTWRONG,
                            Description = Constant.SOMETHINGWENTWRONG
                        });
                    }
                    // Duploicate Flow started
                    if (isNewQuote && parentVersionId != 0)
                    {
                        var duplicateContantsDictionary = Utility.VariableMapper(Constant.DUPLICATECONSTANTMAPPERPATH, Constant.CONSTANTMAPPER);
                        //creating variableMapper to send to StoredProcedures
                        List<ConfigVariable> mapperVariables = new List<ConfigVariable>();
                        foreach (var variable in duplicateContantsDictionary)
                        {
                            mapperVariables.Add(new ConfigVariable()
                            {
                                VariableId = variable.Key,
                                Value = variable.Value
                            });
                        }
                        var variableMapperAssignment = Utility.GenerateVariableMapperDataTable(mapperVariables);
                        List<ResultProjectSave> lstResultDuplicateValues = new List<ResultProjectSave>();
                        if (lstResult != null && (lstResult.Any() && lstResult[0]?.Result == 1))
                        {
                            foreach (var resultProjectSave in lstResult)
                            {

                                IList<SqlParameter> sqlParameters = new List<SqlParameter>()
                                {
                                    new SqlParameter() { ParameterName = Constant.DESTINATIONPROJECTID, Value = resultProjectSave.OpportunityId},
                                    new SqlParameter() { ParameterName = Constant.@USERNAME, Value = userName},
                                    new SqlParameter() { ParameterName = Constant.@SOURCEQUOTEID, Value = string.Empty},
                                    new SqlParameter() { ParameterName = Constant.SOURCEVERSION, Value = resultProjectSave.VersionId},
                                    new SqlParameter() { ParameterName = Constant.PARENTVERSION, Value = parentVersionId},
                                    new SqlParameter() {ParameterName = Constant.COUNTRYFORPROJECTS,Value= Constant.PROJECTCOLUMNVALUEUS},
                                    new SqlParameter() { ParameterName =  Constant.VARIABLEMAPPERDATATABLE,Value=variableMapperAssignment,Direction = ParameterDirection.Input }
                                };
                                cmd.CommandText = Constant.DUPLICATEQUOTEBYQUOTEID;
                                cmd.Parameters.Clear();
                                var resultForDuplicateQuote = CpqDatabaseManager.ExcecuteSP(cmd, sqlParameters);
                                if (resultForDuplicateQuote.Tables.Count > 0)
                                {
                                    var quoteidList = (from DataRow dRow in resultForDuplicateQuote.Tables[0].Rows
                                                       select new
                                                       {
                                                           oppId = Convert.ToString(dRow[Constant.OPPORTUNITYIDVARIABLE]),
                                                           quoteid = Convert.ToString(dRow[Constant.QUOTEID]),
                                                           versionId = Convert.ToInt32(dRow[Constant.VERSIONID])
                                                       }).Distinct();
                                    foreach (var quote in quoteidList)
                                    {
                                        ResultProjectSave duplicatesRresult = new ResultProjectSave
                                        {
                                            Result = 1,
                                            OpportunityId = quote.oppId,
                                            QuoteId = quote.quoteid,
                                            VersionId = quote.versionId,
                                            Message = Constant.QUOTEDUPLICATESUCESSMESSAGE
                                        };
                                        lstResultDuplicateValues.Add(duplicatesRresult);
                                    }
                                }
                                else
                                {
                                    ResultProjectSave duplicatesRresult = new ResultProjectSave
                                    {
                                        Result = -1,
                                        Message = Constant.QUOTEDUPLICATEERRORSAVEMESSAGE,
                                        QuoteId = string.Empty,
                                        OpportunityId = string.Empty
                                    };
                                    lstResultDuplicateValues.Add(duplicatesRresult);
                                    throw new CustomException(new ResponseMessage
                                    {
                                        StatusCode = Constant.BADREQUEST,
                                        Message = Constant.SOMETHINGWENTWRONG,
                                        Description = Constant.QUOTEDUPLICATEERRORSAVEMESSAGE
                                    });
                                }

                            }
                            lstResult = lstResultDuplicateValues;
                        }
                        //return listQuotesresult;
                    }
                    sqlTransaction.Commit();
                    Utility.LogEnd(methodStartTime);
                    return lstResult;
                }
                catch (Exception ex)
                {

                    sqlTransaction.Rollback();
                    throw new CustomException(new ResponseMessage()
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = Constant.SOMETHINGWENTWRONG,
                        Description = Constant.SOMETHINGWENTWRONG + string.Empty + ex
                    });
                }
                finally
                {
                    connection.Close();
                }
            }

        }

        /// <summary>
        /// Method to DeleteProjectById
        /// </summary>
        /// <param Name="projectId"></param>
        /// <param Name="versionId"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public List<ResultProjectDelete> DeleteProjectById(string projectId, string versionId, string userId)
        {
            var methodStartTime = Utility.LogBegin();
            ResultProjectDelete resultDeleted = new ResultProjectDelete();
            List<ResultProjectDelete> lstResult = new List<ResultProjectDelete>();


            IList<SqlParameter> sp1 = new List<SqlParameter>()
            {
              new SqlParameter() { ParameterName = Constant.@PROJECTID,Value=projectId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar,Size = 50},
              new SqlParameter() { ParameterName = Constant.VERSIONIDVALUE,Value=versionId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar,Size = 50},
              new SqlParameter() { ParameterName = Constant.@USERNAME,Value=userId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar,Size=50},
              new SqlParameter() { ParameterName = Constant.@RESULT,Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int},
            };
            int result = CpqDatabaseManager.ExecuteNonquery(Constant.DELETEPROJECTIDINITIATE, sp1);

            if (result == 1)
            {
                resultDeleted.Result = result;
                resultDeleted.Message = Constant.DELETEPROJECTSUCCESSMSG;
                lstResult.Add(resultDeleted);
            }
            else
            {
                resultDeleted.Result = result;
                resultDeleted.Message = Constant.PROJECTERRORSAVEMESSAGE;
                lstResult.Add(resultDeleted);

                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.SOMETHINGWENTWRONG,
                    Description = Constant.SOMETHINGWENTWRONG
                });
            }
            Utility.LogEnd(methodStartTime);
            return lstResult;
        }

        /// <summary>
        /// Method to Getvariablevalues
        /// </summary>
        /// <param Name="OpportunityId"></param>
        /// <returns></returns>
        public List<BuildingVariableAssignment> Getvariablevalues(string opportunityId)
        {
            var methodStartTime = Utility.LogBegin();
            var unitMaterials = new List<BuildingVariableAssignment>();
            IList<SqlParameter> sqlParameterlist = new List<SqlParameter>()
            {
              new SqlParameter() { ParameterName = Constant.@QUOTEIDSPPARAMETER,Value=opportunityId,Direction = ParameterDirection.Input }
            };
            var variablesDataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETVARIABLESBYQUOTEID, sqlParameterlist);
            //var viewVariableMappingJSON = Utility.DeserializeObjectValue<JObject>(System.IO.File.ReadAllText(Constant.APPVARIABLETOVIEWMAPPINGJSON));
            if (variablesDataSet != null && variablesDataSet.Tables.Count > 0)
            {
                var buildingidList = (from DataRow dRow in variablesDataSet.Tables[0].Rows
                                      select new
                                      {
                                          id = Convert.ToInt32(dRow[Constant.BUILDINGIDCOLUMNNAME])
                                      }).Distinct().ToList();
                var buildingConfigList = (from DataRow dRow in variablesDataSet.Tables[0].Rows
                                          select new
                                          {
                                              id = Convert.ToInt32(dRow[Constant.BUILDINGIDCOLUMNNAME]),
                                              variableId = Convert.ToString(dRow[Constant.BUINDINGTYPE]),
                                              variableValue = Convert.ToString(dRow[Constant.BUINDINGVALUE])
                                          }).Distinct().ToList();
                var buildingEqmntConfigList = (from DataRow dRow in variablesDataSet.Tables[1].Rows
                                               select new
                                               {
                                                   id = Convert.ToInt32(dRow[Constant.BUILDINGIDCOLUMNNAME]),
                                                   variableId = Convert.ToString(dRow[Constant.VARIABLETYPE]),
                                                   variableValue = Convert.ToString(dRow[Constant.VALUE])
                                               }).Distinct().ToList();
                var buildingEqmntConsoleList = (from DataRow dRow in variablesDataSet.Tables[2].Rows
                                                select new
                                                {
                                                    id = Convert.ToInt32(dRow[Constant.BUILDINGIDCOLUMNNAME]),
                                                    variableId = Convert.ToString(dRow[Constant.VARIABLETYPE]),
                                                    variableValue = Convert.ToString(dRow[Constant.VALUE])
                                                }).Distinct().ToList();
                var groupIdList = (from DataRow dRow in variablesDataSet.Tables[3].Rows
                                   select new
                                   {
                                       buildingId = Convert.ToInt32(dRow[Constant.BUILDINGIDCOLUMNNAME]),
                                       groupId = Convert.ToInt32(dRow[Constant.GROUPIDCOLUMNNAME]),
                                       groupName = Convert.ToString(dRow[Constant.GROUPNAMECOLUMNNAME])
                                   }).Distinct().ToList();
                var controlLocationList = (from DataRow dRow in variablesDataSet.Tables[4].Rows
                                           select new
                                           {
                                               groupId = Convert.ToInt32(dRow[Constant.GROUPCONFIGID]),
                                               controlLocationType = Convert.ToString(dRow[Constant.CONTROLOCATIONTYPE]),
                                               controlLocationValue = Convert.ToString(dRow[Constant.CONTROLOCATIONVALUE])
                                           }).Distinct().ToList();
                var unitIDList = (from DataRow dRow in variablesDataSet.Tables[5].Rows
                                  select new
                                  {
                                      groupId = Convert.ToInt32(dRow[Constant.GROUPCONFIGID]),
                                      unitId = Convert.ToInt32(dRow[Constant.UNITID]),
                                      UEID = Convert.ToString(dRow[Constant.UEID]),
                                      setId = Convert.ToInt32(dRow[Constant.SETCONFIGURATIONID]),
                                      productName = Convert.ToString(dRow[Constant.PRODUCTNAMECOLUMN]),
                                      unitDesignation = Convert.ToString(dRow[Constant.DESIGNATION]),
                                      travelFeet = Convert.ToInt32(dRow[Constant.TRAVELFEET]),
                                      travelInch = Convert.ToDecimal(dRow[Constant.TRAVELINCH]),
                                      frontOpening = Convert.ToInt32(dRow[Constant.FRONTOPENING]),
                                      rearOpening = Convert.ToInt32(dRow[Constant.REAROPENING]),
                                      createdon = Convert.ToDateTime(dRow[Constant.CREATEDON]),
                                      UnitJson = Convert.ToString(dRow[Constant.UNITJSON])
                                  }).Distinct().ToList();
                var unitList = (from DataRow dRow in variablesDataSet.Tables[6].Rows
                                select new
                                {
                                    unitId = Convert.ToInt32(dRow[Constant.UNITID]),
                                    variableId = Convert.ToString(dRow[Constant.CONFIGVARIABLES]),
                                    value = Convert.ToString(dRow[Constant.CONFIGUREVALUES])
                                }).Distinct().ToList();
                var groupConfigurationData = (from DataRow dRow in variablesDataSet.Tables[8].Rows
                                              select new
                                              {
                                                  groupId = Convert.ToInt32(dRow[Constant.GROUPIDCOLUMNNAME]),
                                                  variableId = Convert.ToString(dRow[Constant.GROUPCONFIGURATIONTYPE]),
                                                  value = Convert.ToString(dRow[Constant.GROUPCONFIGURATIONVALUE])
                                              }).Distinct().ToList();
                foreach (var buildingId in buildingidList)
                {
                    var buildingConfigListForbuildingId = buildingConfigList.Where(x => x.id.Equals(buildingId.id)).Distinct().ToList();
                    var buildingEqmntConfigListForbuildingId = buildingEqmntConfigList.Where(x => x.id.Equals(buildingId.id)).Distinct().ToList();
                    var buildingEqmntConsoleListForbuildingId = buildingEqmntConsoleList.Where(x => x.id.Equals(buildingId.id)).Distinct().ToList();
                    var groupListForBuilding = groupIdList.Where(x => x.buildingId.Equals(buildingId.id)).Distinct().ToList();
                    var buildingVariableAssignment = new BuildingVariableAssignment
                    {
                        BuildingId = buildingId.id,
                        BuildingVariableAssignments = new List<VariableAssignment>()
                    };
                    foreach (var variable in buildingConfigListForbuildingId)
                    {
                        var variableAssignments = new VariableAssignment
                        {
                            VariableId = variable.variableId,
                            Value = variable.variableValue
                        };
                        buildingVariableAssignment.BuildingVariableAssignments.Add(variableAssignments);
                    }
                    foreach (var variable in buildingEqmntConfigListForbuildingId)
                    {
                        var variableAssignments = new VariableAssignment
                        {
                            VariableId = variable.variableId,
                            Value = variable.variableValue
                        };
                        buildingVariableAssignment.BuildingVariableAssignments.Add(variableAssignments);
                    }
                    foreach (var variable in buildingEqmntConsoleListForbuildingId)
                    {
                        var variableAssignments = new VariableAssignment
                        {
                            VariableId = variable.variableId,
                            Value = variable.variableValue
                        };
                        buildingVariableAssignment.BuildingVariableAssignments.Add(variableAssignments);
                    }
                    buildingVariableAssignment.GroupVariableAssignment = new List<GroupVariableAssignment>();
                    foreach (var group in groupListForBuilding)
                    {
                        var groupVariableAssignment = new GroupVariableAssignment
                        {
                            GroupId = group.groupId,
                            isNCP = true
                        };
                        var controlLocationGroupList = controlLocationList.Where(x => x.groupId.Equals(group.groupId)).Distinct().ToList();
                        var groupConfigurationDataForGroup = groupConfigurationData.Where(x => x.groupId.Equals(group.groupId)).Distinct().ToList();
                        if (groupConfigurationDataForGroup.Any())
                        {
                            foreach (var variable in groupConfigurationDataForGroup)
                            {
                                if (Utility.CheckEquals(variable.value, Constant.PRODUCTELEVATOR))
                                {
                                    groupVariableAssignment.isNCP = false;
                                }
                            }
                        }
                        groupVariableAssignment.GroupVariableAssignments = new List<VariableAssignment>();
                        foreach (var controlLocation in controlLocationGroupList)
                        {
                            var variableAssignments = new VariableAssignment
                            {
                                VariableId = controlLocation.controlLocationType,
                                Value = controlLocation.controlLocationValue
                            };
                            groupVariableAssignment.GroupVariableAssignments.Add(variableAssignments);
                        }
                        var unitsIngroup = unitIDList.Where(x => x.groupId.Equals(group.groupId)).Distinct().ToList();
                        groupVariableAssignment.UnitVariableAssignments = new List<UnitVariableAssignment>();
                        groupVariableAssignment.SetVariableAssignment = new List<SetVariableAssignment>();
                        foreach (var unit in unitsIngroup)
                        {
                            if (!string.IsNullOrEmpty(unit.UEID))
                            {
                                UnitVariableAssignment unitVariableAssignments = new UnitVariableAssignment
                                {
                                    UnitId = unit.unitId,
                                    SetId = unit.setId
                                };
                                groupVariableAssignment.GroupVariableAssignments.Add(Utility.DeserializeObjectValue<VariableAssignment>(unit.UnitJson));
                                groupVariableAssignment.UnitVariableAssignments.Add(unitVariableAssignments);
                                var unitSetList = unitList.Where(x => x.unitId.Equals(unit.unitId)).Distinct().ToList();
                                var setExists = false;
                                foreach (var set in groupVariableAssignment.SetVariableAssignment)
                                {
                                    if (set.SetId == unit.setId)
                                    {
                                        setExists = true;
                                    }
                                }
                                if (!setExists)
                                {
                                    var setVariableAssignments = new SetVariableAssignment
                                    {
                                        SetId = unit.setId,
                                        UnitVariableAssignments = new List<VariableAssignment>()
                                    };
                                    foreach (var variables in unitSetList)
                                    {
                                        var variableAssignmnets = new VariableAssignment
                                        {
                                            VariableId = variables.variableId,
                                            Value = variables.value
                                        };
                                        setVariableAssignments.UnitVariableAssignments.Add(variableAssignmnets);
                                    }
                                    setVariableAssignments.ProductName = unit.productName;
                                    setVariableAssignments.RearDoorSelected = Convert.ToBoolean(unit.rearOpening);
                                    groupVariableAssignment.SetVariableAssignment.Add(setVariableAssignments);
                                }
                            }
                        }
                        if (groupVariableAssignment.GroupVariableAssignments != null && groupVariableAssignment.GroupVariableAssignments.Count > 0 && groupVariableAssignment.SetVariableAssignment != null && groupVariableAssignment.SetVariableAssignment.Count > 0)
                        {
                            buildingVariableAssignment.GroupVariableAssignment.Add(groupVariableAssignment);
                        }
                    }
                    if (buildingVariableAssignment.BuildingId > 0 && buildingVariableAssignment.GroupVariableAssignment != null && buildingVariableAssignment.GroupVariableAssignment.Count > 0)
                    {
                        unitMaterials.Add(buildingVariableAssignment);
                    }
                }
            }
            Utility.LogEnd(methodStartTime);
            return unitMaterials;
        }

        /// <summary>
        /// Method to CalculateNumberofStops
        /// </summary>
        /// <param Name="table"></param>
        /// <param Name="UnitId"></param>
        /// <returns></returns>
        public int CalculateNumberofStops(DataTable table, int unitId)
        {
            var methodStartTime = Utility.LogBegin();
            var openingslist = (from DataRow dRow in table.Rows
                                select new
                                {
                                    unitId = Convert.ToInt32(dRow[Constant.UNITID]),
                                    FloorNumber = Convert.ToString(dRow[Constant.FLOORNUMBER]),
                                    Front = Convert.ToBoolean(dRow[Constant.FRONT]),
                                    Rear = Convert.ToBoolean(dRow[Constant.REAR])
                                }).Distinct().ToList();
            var unitopeningData = openingslist.Where(x => x.unitId == unitId).ToList();
            var unitData = unitopeningData.Where(x => (x.Front == true || x.Rear == true)).ToList();
            Utility.LogEnd(methodStartTime);
            return unitData.Count();
        }

        /// <summary>
        /// Method to GetPermissionByRole
        /// </summary>
        /// <param Name="roleName"></param>
        /// <returns></returns>
        public List<Permissions> GetPermissionByRole(string roleName)
        {
            var methodStartTime = Utility.LogBegin();
            List<Permissions> lstPermission = new List<Permissions>();
            IList<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant._ID, Value = string.Empty},
                new SqlParameter() { ParameterName = Constant.@ROLENAME, Value = roleName},
                new SqlParameter() { ParameterName = Constant.@ENTITY, Value = "ListofProjects"},
            };
            DataSet ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETPERMISSIONBYROLENAME, sp);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dRow in ds.Tables[0].Rows)
                {
                    var permission = new Permissions()
                    {
                        Entity = Convert.ToString(dRow[Constant.ENTITYNAME]),
                        PermissionKey = Convert.ToString(dRow[Constant.PERMISSIONKEY]),
                        ProjectStage = Convert.ToString(dRow[Constant.PROJECTSTAGE])
                    };
                    lstPermission.Add(permission);
                }
            }
            Utility.LogEnd(methodStartTime);
            return lstPermission;
        }

        /// <summary>
        /// Method to GetPermissionForProjectScreen
        /// </summary>
        /// <param Name="rolename"></param>
        /// <param Name="projectid"></param>
        /// <returns></returns>
        public List<string> GetPermissionForProjectScreen(string rolename, string projectid)
        {
            var methodStartTime = Utility.LogBegin();
            IList<SqlParameter> sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@_ID, Value = projectid},
                new SqlParameter() { ParameterName = Constant.@ROLENAME, Value = rolename},
                new SqlParameter() { ParameterName = Constant.@ENTITY, Value = "Project"},
            };
            List<string> permission = new List<string>();
            DataSet ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETPERMISSIONBYROLENAME, sp);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dRow in ds.Tables[0].Rows)
                {
                    permission.Add(Convert.ToString(dRow[Constant.PERMISSIONKEY]));
                }
            }
            Utility.LogEnd(methodStartTime);
            return permission;
        }

        /// <summary>
        /// SetQuoteToPrimaryDL
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public ResultProjectSave SetQuoteToPrimaryDL(string userName, string quoteId)
        {
            var methodStartTime = Utility.LogBegin();
            ResultProjectSave result = new ResultProjectSave();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.@USERNAME, Value = userName},
                new SqlParameter() { ParameterName = Constant.QUOTEID_UPPERCASE, Value = quoteId},
                new SqlParameter() { ParameterName = Constant.@RESULT,Direction = ParameterDirection.Output ,SqlDbType = SqlDbType.Int}
            };
            int resultForSetPrimary = CpqDatabaseManager.ExecuteNonquery(Constant.USPSETPRIMARYQUOTES, sqlParameters);
            if (resultForSetPrimary == 1)
            {
                result.Result = 1;
                result.QuoteId = quoteId;
                result.Message = Constant.SETPRIMARYMESSAGE;
            }
            else
            {
                result.Result = 0;
                result.QuoteId = quoteId;
                result.Message = Constant.SETPRIMARYERRORMESSAGE;
            }
            Utility.LogEnd(methodStartTime);
            return result;
        }

        /// <summary>
        /// SetQuoteToPrimaryDL
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public List<ResultProjectSave> GetDuplicateQuoteByProjectIdDL(string projectId, string quoteId, string userName, string country)
        {
            var methodStartTime = Utility.LogBegin();
            List<ResultProjectSave> listQuotesresult = new List<ResultProjectSave>();
            var duplicateContantsDictionary = Utility.VariableMapper(Constant.DUPLICATECONSTANTMAPPERPATH, Constant.CONSTANTMAPPER);
            //creating variableMapper to send to StoredProcedures
            List<ConfigVariable> mapperVariables = new List<ConfigVariable>();
            foreach (var variable in duplicateContantsDictionary)
            {
                mapperVariables.Add(new ConfigVariable()
                {
                    VariableId = variable.Key,
                    Value = variable.Value
                });
            }
            var variableMapperAssignment = Utility.GenerateVariableMapperDataTable(mapperVariables);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constant.DESTINATIONPROJECTID, Value = projectId},
                new SqlParameter() { ParameterName = Constant.@USERNAME, Value = userName},
                new SqlParameter() { ParameterName = Constant.@SOURCEQUOTEID, Value = quoteId},
                new SqlParameter() { ParameterName = Constant.SOURCEVERSION, Value = 0},
                new SqlParameter() { ParameterName = Constant.PARENTVERSION, Value = 0},
                new SqlParameter() {ParameterName = Constant.COUNTRYFORPROJECTS,Value= country},
                new SqlParameter() { ParameterName =  Constant.VARIABLEMAPPERDATATABLE,Value=variableMapperAssignment,Direction = ParameterDirection.Input }
            };
            var resultForDuplicateQuote = CpqDatabaseManager.ExecuteDataSet(Constant.DUPLICATEQUOTEBYQUOTEID, sqlParameters);
            if (resultForDuplicateQuote.Tables.Count > 0)
            {
                var quoteidList = (from DataRow dRow in resultForDuplicateQuote.Tables[0].Rows
                                   select new { oppId = Convert.ToString(dRow[Constant.OPPORTUNITYIDVARIABLE]), quoteid = Convert.ToString(dRow[Constant.QUOTEID]), versionId = Convert.ToInt32(dRow[Constant.VERSIONID]) }).Distinct();
                foreach (var quote in quoteidList)
                {
                    ResultProjectSave result = new ResultProjectSave
                    {
                        Result = 1,
                        OpportunityId = quote.oppId,
                        QuoteId = quote.quoteid,
                        VersionId = quote.versionId,
                        Message = Constant.QUOTEDUPLICATESUCESSMESSAGE
                    };
                    listQuotesresult.Add(result);
                }
            }
            else
            {
                ResultProjectSave result = new ResultProjectSave
                {
                    Result = -1,
                    Message = Constant.QUOTEDUPLICATEERRORSAVEMESSAGE,
                    QuoteId = string.Empty,
                    OpportunityId = string.Empty
                };
                listQuotesresult.Add(result);
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.SOMETHINGWENTWRONG,
                    Description = Constant.QUOTEDUPLICATEERRORSAVEMESSAGE
                });
            }
            Utility.LogEnd(methodStartTime);
            return listQuotesresult;
        }
    }
}