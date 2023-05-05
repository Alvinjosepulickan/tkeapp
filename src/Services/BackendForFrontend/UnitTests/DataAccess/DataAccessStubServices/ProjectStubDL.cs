/************************************************************************************************************
************************************************************************************************************
    File Name     :   ProjectStubDL
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
********************************************************************************************/
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.BFF.Test.Common;
using TKE.SC.Common.Model.ExceptionModel;

namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    public class ProjectStubDL : IProjectsDL
    {
        /// <summary>
        /// this method is for setting stub data for the method GetAllProjectsDetails
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseMessage> GetAllProjectsDetails()
        {
            //return null;
            var response = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.PROJECTDETAILS));
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = JObject.Parse(response.ToString())
            };
        }
        /// <summary>
        /// this method is for setting stub data for the method GetListOfProjectsForUser
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Projects> GetListOfProjectsForUser(int userId)
        {
            if (userId > 0)
            {
                List<Projects> lstProjects = new List<Projects>();
                return lstProjects;
            }
            else if (userId == 0)
            {
                throw new FileNotFoundException();
            }
            else
            {
                return null;
            }

        }

        public OpportunityEntity GetOpportunityData(string oppId)
        {
            return null;
        }

        /// <summary>
        /// this method is for setting stub data for the method GetProjectDetails
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetProjectDetails(string id)
        {
            try
            {

                ProjectsList pl = new ProjectsList();
                pl.projectlist = new List<Projects>();
                var response = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.PROJECTDETAILS));
                var resultObject = (from p in response["Projects"] where (string)p["id"] == id select (string)p["id"]).ToList();
                ResponseMessage res;
                if (resultObject.Count > 0)
                {
                    res = new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = JArray.FromObject(resultObject) };
                }
                else
                {
                    res = new ResponseMessage { StatusCode = Constant.NOTFOUND, Message = "not found" };
                }
                return res;

            }
            catch (Exception Ex)
            {
                return new ResponseMessage { StatusCode = Constant.BADREQUEST, Message = Ex.Message };
            }
        }

        /// <summary>
        /// this method is for setting stub data for the method SearchUser
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<User> SearchUser(string userName)
        {
            if (userName == null)
            {
                ResponseMessage errorMessage = new ResponseMessage();
                errorMessage.Message = "There are no users";
                throw new CustomException(errorMessage);               
            }

            User neewUser = new User();
            List<User> lstUsers = new List<User>();
            neewUser.Id = 1;
            lstUsers.Add(neewUser);
            return lstUsers;
        }

        async Task<ResponseMessage> IProjectsDL.GetProjectInfo(string opportunityId, string versionId)
        {
            var getResponse = JObject.Parse((File.ReadAllText(AppGatewayJsonFilePath.VIEWEXPORTVARIABLELIST)).ToString());
            if (opportunityId == string.Empty && versionId == string.Empty)
            {
                return new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Response = JObject.FromObject(getResponse)
                };
            }
            else if (opportunityId == "Val")
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR
                });
            }
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = JObject.Parse(getResponse.ToString())
            };

        }

        public ViewExportDetails GetVariablesAndValuesForView(string opportunityId)
        {
            throw new NotImplementedException();
        }

        public ViewExportDetails GetVariablesAndValuesForView1(string opportunityId, List<string> exportJsonBuildingVariables, List<string> exportJsoneqmntConsoleVariables, List<string> exportJsonEqmntConfgnVariables, List<string> exportJsonControlLocationVariables, List<string> exportJsonUnitConfigurationVariables)
        {
            if (opportunityId != string.Empty)
            {
                ViewExportDetails viewDetails = new ViewExportDetails();
                return viewDetails;
            }
            else if (opportunityId == string.Empty)
            {
                throw new FileNotFoundException();
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseMessage> SaveConfigurationToView(string viewPostUrl, string viewUserName, string viewPassword, ResponseMessage requestBody)
        {

            ViewExportDetails view = new ViewExportDetails();
            var getResponse = JObject.Parse((File.ReadAllText(AppGatewayJsonFilePath.VIEWEXPORTVARIABLELIST)).ToString());
            if (viewUserName != string.Empty)
            {
                return new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Response = JObject.Parse(getResponse.ToString())
                };
            }
            else if (viewUserName == "")
            {
                return new ResponseMessage
                {
                    StatusCode = Constant.NOTFOUND,
                    Response = JObject.Parse(getResponse.ToString())
                };
            }
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = JObject.Parse(getResponse.ToString())
            };

            //try
            //{

            //    ViewExportDetails view = new ViewExportDetails();
            //    var response = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.VIEWEXPORTVARIABLELIST));
            //    //var resultObject = (from p in response["Projects"] where (string)p["id"] ==  select (string)p["id"]).ToList();
            //    ResponseMessage res;
            //    if (viewUserName != string.Empty)
            //    {
            //        res = new ResponseMessage { StatusCode = Constant.SUCCESS, ResponseArray = JArray.FromObject(response) };
            //    }
            //    else
            //    {
            //        res = new ResponseMessage { StatusCode = Constant.NOTFOUND, Message = "not found" };
            //    }
            //    return res;

            //}
            //catch (Exception Ex)
            //{
            //    return new ResponseMessage { StatusCode = Constant.BADREQUEST, Message = Ex.Message };
            //}
        }

        //Task<ResponseMessage> IProjectsDL.GetAllProjectsDetails()
        //{
        //    throw new NotImplementedException();
        //}

        List<Projects> IProjectsDL.GetListOfProjectsForUser(int userId)
        {
            if (userId == 0)
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR
                });
            }
            if (userId != 1)
            {
                return new List<Projects> { new Projects { } };
            }
            else
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR
                });

            }
        }

        //Task<ResponseMessage> IProjectsDL.GetProjectDetails(string id)
        //{
        //    throw new NotImplementedException();
        //}

        OpportunityEntity IProjectsDL.GetOpportunityData(string oppId)
        {
            throw new NotImplementedException();
        }

        List<User> IProjectsDL.SearchUser(string userName)
        {
            if (userName != null)
            {
                return new List<User> { new User { UserId = "" } };
            }
            else
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR
                });
            }
        }

        //CreateProjectResponseObject IProjectsDL.GetMiniProjectValues(string sessionId, string userDetails, string projectId, CreateProjectResponseObject enrichedData)
        //{
        //    throw new NotImplementedException();
        //}

        List<ResultProjectSave> IProjectsDL.SaveAndUpdateMiniProjectValues(VariableDetails enrichedData, string userName, bool isAddQuote = false)
        {
            return new List<ResultProjectSave> { new ResultProjectSave { Result = 1,Message="The Quote is Saved and Updated Succesfully" } };
        }

        List<ResultProjectSave> IProjectsDL.GenerateQuoteId(ViewProjectDetails viewDetails, string userName, int parentVersionId = 0)
        {
            throw new NotImplementedException();
        }

        public List<ResultProjectSave> GenerateQuoteId(ViewProjectDetails viewDetails, string userName)
        {
            throw new NotImplementedException();
        }

        public List<ProjectResponseDetails> GetListOfProjectsDetailsDl(string userName, string countryCode)
        {
            throw new NotImplementedException();
        }


        public List<BuildingVariableAssignment> Getvariablevalues(string opportunityId)
        {
            List<BuildingVariableAssignment> buildingvariableAssingn = new List<BuildingVariableAssignment>();
            return buildingvariableAssingn;
        }

        public List<Permissions> GetPermissionByRole(string roleName)
        {
            throw new NotImplementedException();
        }

        //public List<string> GetPermissionForProjectScreen(string rolename, string projectid)
        //{
        //    throw new NotImplementedException();
        //}

        public ResultProjectSave SetQuoteToPrimaryDL(string userName, string quoteId)
        {
            ResultProjectSave result = new ResultProjectSave();
            if (string.IsNullOrEmpty(quoteId))
            {
                result.Result = 0;
                result.Message = "Quote Failed";
            }
            else
            {
                result.Result = 1;
                result.QuoteId = quoteId;
                result.Message = "Quote succeeded";
            }
            return result;
        }

        public async Task<ResponseMessage> SaveConfigurationToView(ResponseMessage requestBody)
        {
            Dictionary<string, string> resultcode = new Dictionary<string, string>();
           
            if (requestBody.Response != null)
            {
                resultcode.Add(Constant.CODEVALUES, "200");
                resultcode.Add(Constant.RETMSGVALUES, "The project is saved successfully");
                return new ResponseMessage
                {
                    StatusCode = 1,
                    Message = requestBody.Message,
                    Response =JObject.FromObject(resultcode)
                };
            }
            else
            {
                resultcode.Add(Constant.CODEVALUES, "");
                resultcode.Add(Constant.RETMSGVALUES, "");
                return new ResponseMessage
                {
                    StatusCode = 0,
                    Message = requestBody.Message,
                    Response = JObject.FromObject(resultcode)

                };
            }
        }

        public List<ResultProjectSave> GetDuplicateQuoteByProjectIdDL(string projectId, string quoteId, string userName, string country)
        {
            throw new NotImplementedException();
        }

        CreateProjectResponseObject IProjectsDL.GetMiniProjectValues(string sessionId, string userDetails, string projectId, CreateProjectResponseObject enrichedData, int versionId)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR
                });
            }
            return new CreateProjectResponseObject
            {
                Sections = new List<Sections> { new Sections { } }
            ,
                Permissions = new List<string> { "" },
                VariableDetails = new Dictionary<string, string> { { "projects.ProjectId", "" }

                    ,{ "projects.ProjectName", "" } ,{ "projects.Branch", "" } ,{ "projects.SalesStage", "" } ,{ "layoutDetails.Language", "" } ,{ "accountDetails.SiteAddress.AddressLine2", "" } ,
                        { "layoutDetails.MeasuringUnit", "" } ,{ "accountDetails.AccountName", "" } ,{ "accountDetails.SiteAddress.AddressLine1", "" } ,{ "accountDetails.SiteAddress.Country", "" } ,{ "accountDetails.SiteAddress.City", "" },
                        { "accountDetails.SiteAddress.State", "" } ,{ "accountDetails.SiteAddress.ZipCode", "" }
                    ,{ "accountDetails.Contact", "" } ,{ "projects.AwardCloseDate", "02/02/21" } }
            };
        }

        List<ProjectResponseDetails> IProjectsDL.GetListOfProjectsDetailsDl(User user)
        {
            return new List<ProjectResponseDetails> { new ProjectResponseDetails {SalesMan="Salesman"
            ,ViewUrl="",Quotes= new List<QuoteDisplayDetails>{ new QuoteDisplayDetails { VersionId="Val"} },
            SalesStage= new Status{StatusId=1,StatusKey="",StatusName="",Description="",DisplayName="" },
            Branch="",Name="",OpportunityId="1",Permissions= new List<string>{ ""},CscCoordinator="",QuoteCount=1,
            LastModifiedDate= new DateTime{ }
            } };
        }

        ViewExportDetails IProjectsDL.GetVariablesAndValuesForView1(string opportunityId, List<string> exportJsonBuildingVariables, List<string> exportJsoneqmntConsoleVariables, List<string> exportJsonEqmntConfgnVariables, List<string> exportJsonControlLocationVariables, List<string> exportJsonUnitConfigurationVariables, List<BuildingVariableAssignment> defaultVtPackageValues)
        {
            if (string.IsNullOrEmpty(opportunityId))
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR
                });
            }
            return new ViewExportDetails{Units= new List<Identifications> { new Identifications { Identification
            = new Unitsection{UEID="" } } } ,Quotation= new QuotationDetails {OpportunityInfo=new OpportunityValues { BaseBidCreator
            ="",OpportunityId="",OpportunityURL="",QuickQuote=true,FactoryQuoteCurrency=""},Quote=
            new Quote { QuoteStatus = "", BaseBid = true, QuoteNumber = "", VIEW_Version = "" },UnitMaterials=new List<JObject> { new JObject { } }
            }
            };
        }


        List<ResultProjectDelete> IProjectsDL.DeleteProjectById(string projectId, string versionId, string userId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR
                });
            }
            if (projectId == "1")
            {
                return new List<ResultProjectDelete> { new ResultProjectDelete { Result = 1 ,Message="Deleted Successfully"} };
            }
            else
            {
                return new List<ResultProjectDelete> { new ResultProjectDelete { Result = 1, Message = "Deleted Successfully" } };
            }
        }

        List<BuildingVariableAssignment> IProjectsDL.Getvariablevalues(string opportunityId)
        {
            return new List<BuildingVariableAssignment>
            { new BuildingVariableAssignment {BuildingVariableAssignments= new List<Configit.Configurator.Server.Common.VariableAssignment>
            { new Configit.Configurator.Server.Common.VariableAssignment{ Value="value",VariableId="Id"} },BuildingId=1,
            GroupVariableAssignment= new List<GroupVariableAssignment>{ new GroupVariableAssignment {
            GroupId=1,isNCP=false,UnitVariableAssignments = new List<UnitVariableAssignment>{ new UnitVariableAssignment {
            SetId=1,UnitId=1} }, SetVariableAssignment=new List<SetVariableAssignment>{ new SetVariableAssignment { SetId=1
            ,RearDoorSelected=true,ProductName="EVO_200",ProductVariableAssignments=new List<Configit.Configurator.Server.Common.VariableAssignment>
            {
                new Configit.Configurator.Server.Common.VariableAssignment{ Value="",VariableId=""}
            },UnitVariableAssignments=new List<Configit.Configurator.Server.Common.VariableAssignment>
            {
                new Configit.Configurator.Server.Common.VariableAssignment{ Value="",VariableId=""}
            },ProductTreeVariables= new Dictionary<string, object>{ { "",""} },SytemValidationVariableAssignments
            =new List<Configit.Configurator.Server.Common.VariableAssignment>{ new Configit.Configurator.Server.Common.VariableAssignment {Value="",VariableId="" } }
            
            }},GroupVariableAssignments=
            new List<Configit.Configurator.Server.Common.VariableAssignment>{new Configit.Configurator.Server.Common.VariableAssignment
            { Value="",VariableId=""} } } } } };
        }



        List<Permissions> IProjectsDL.GetPermissionByRole(string roleName)
        {
            return
                new List<Permissions> { new Permissions {BuildingStatus="",GroupStatus="",ProjectStage=""
                ,UnitStatus="",Entity="",PermissionKey=""} };
        }

        public List<string> GetPermissionForProjectScreen(string rolename, string projectid)
        {
            return new List<string> { "value" };
        }

        ResultProjectSave IProjectsDL.SetQuoteToPrimaryDL(string userName, string quoteId)
        {
            if (quoteId == "1")
            {
                return new ResultProjectSave { Result = 1,QuoteStatus="Quote Set To Primary"};
            }
            else
            {
                return new ResultProjectSave { Result = 0 };
            }
        }

        List<ResultProjectSave> IProjectsDL.GetDuplicateQuoteByProjectIdDL(string projectId, string quoteId, string userName, string country)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR
                });
            }
            if (quoteId == "1")
            {
                return new List<ResultProjectSave> { new ResultProjectSave { Result = 0, Message = "Duplicated Successfully" } };
            }
            else
            {
                return new List<ResultProjectSave> { new ResultProjectSave { Result = 0 ,Message="Duplicated Successfully"} };
            }
        }

        //ResultProjectSave IProjectsDL.SetQuoteToPrimaryDL(string userName, string quoteId)
        //{
        //    throw new NotImplementedException();
        //}


    }
}
