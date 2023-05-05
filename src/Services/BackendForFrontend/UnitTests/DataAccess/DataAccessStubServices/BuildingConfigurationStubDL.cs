/************************************************************************************************************
************************************************************************************************************
    File Name     :   BuildingConfigurationStubDL class 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.UIModel;
using Constant = TKE.SC.BFF.DataAccess.Helpers.Constant;

namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    public class BuildingConfigurationStubDL : IBuildingConfigurationDL
    {

        /// <summary>
        /// To get all building details for project by projectId
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<ListOfConfiguration> GetListOfConfigurationForProject(string projectId)
        {
            if (projectId == "")
            {
                return null;
            }
            ListOfConfiguration resultListOfConfiguration = new ListOfConfiguration();
            List<ListOfConfiguration> lstResult = new List<ListOfConfiguration>();
            resultListOfConfiguration.Id = 1;
            lstResult.Add(resultListOfConfiguration);
            return lstResult;
        }

        /// <summary>
        /// To get building details by buildingId
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public List<ConfigVariable> GetBuildingConfigurationById(int buildingId)
        {
            if (buildingId == 23)
            {
                var getResponse = File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.GETBUILDINGCONFIGBYIDSTUBRESPONSEBODY1));
                var stubResponse = JArray.Parse(getResponse);
                var response = stubResponse.ToObject<List<ConfigVariable>>();
                return response;
            }
            else if (buildingId == 0)
            {
                return null;
            }
            else
            {
                var getResponse = File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.GETBUILDINGCONFIGBYIDSTUBRESPONSEBODY));
                var stubResponse = JArray.Parse(getResponse);
                var response = stubResponse.ToObject<List<ConfigVariable>>();
                return response;
            }



            //ConfigVariable resultBuildingConfiguration = new ConfigVariable();
            //List<ConfigVariable> lstResult = new List<ConfigVariable>();
            //resultBuildingConfiguration.VariableId = "id";
            //lstResult.Add(resultBuildingConfiguration);

        }

        /// <summary>
        /// To save building details
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        /// <param name="buildingName"></param>
        /// <param name="bldVariablejson"></param>
        /// <returns></returns>
        public List<Result> SaveBuildingConfigurationForProject(int buildingId, string userId, string projectId, string buildingName, string bldVariablejson, ConflictsStatus isEditFlow, bool hasConflictsFlag)
        {
            if (userId == null || projectId == "" || buildingName == null)
            {
                return null;
            }
            else if (userId == "Test")
            {
                Result resBuildingConfiguration1 = new Result();
                List<Result> lstResult1 = new List<Result>();
                resBuildingConfiguration1.message = "Building Configuration Not Saved Successfully";
                lstResult1.Add(resBuildingConfiguration1);
                return lstResult1;
            }
            Result resBuildingConfiguration = new Result();
            resBuildingConfiguration.result = 1;
            List<Result> lstResult = new List<Result>();
            resBuildingConfiguration.message = "Building Configuration Saved Successfully";
            lstResult.Add(resBuildingConfiguration);
            return lstResult;
        }

        /// <summary>
        /// To save building elevation details
        /// </summary>
        /// <param name="dtBuildingElevation"></param>
        /// <returns></returns>
        public List<Result> SaveBuildingElevation(DataTable dtBuildingElevation)
        {
            if (dtBuildingElevation == null)
            {
                return null;
            }
            Result resBuildingElevation = new Result();
            List<Result> lstResult = new List<Result>();
            resBuildingElevation.message = "Building Elevation Saved Successfully";
            lstResult.Add(resBuildingElevation);
            return lstResult;
        }

        /// <summary>
        /// To update building elevation details
        /// </summary>
        /// <param name="dtBuildingElevation"></param>
        /// <returns></returns>
        public List<Result> UpdateBuildingElevation(DataTable dtBuildingElevation)
        {
            if (dtBuildingElevation == null)
            {
                return null;
            }
            Result resBuildingElevation = new Result();
            List<Result> lstResult = new List<Result>();
            resBuildingElevation.message = "Building Elevation Updated Successfully";
            lstResult.Add(resBuildingElevation);
            return lstResult;
        }

        /// <summary>
        /// To get building elevation details by buildingId
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public List<BuildingElevation> GetBuildingElevationById(int buildingId)
        {
            if (buildingId == 0)
            {
                return null;
            }
            BuildingElevation resultBuildingElevation = new BuildingElevation();
            List<BuildingElevation> lstResult = new List<BuildingElevation>();
            resultBuildingElevation.buildingConfigurationId = 1;
            lstResult.Add(resultBuildingElevation);
            return lstResult;
        }

        /// <summary>
        /// TO delete building details by buildingConfigurationId
        /// </summary>
        /// <param name="buildingConfigurationId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Result> DeleteBuildingConfigurationById(int buildingConfigurationId, string userId)
        {
            if (userId == "test")
            {
                return null;
            }
            Result resBuildingConfiguration = new Result();
            List<Result> lstResult = new List<Result>();
            resBuildingConfiguration.message = "Building Configuration Deleted Successfully";
            lstResult.Add(resBuildingConfiguration);
            return lstResult;
        }

        /// <summary>
        /// To delete building elevation by buildingConfigurationId
        /// </summary>
        /// <param name="buildingConfigurationId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Result> DeleteBuildingElevationById(int buildingConfigurationId, string userId)
        {
            if (userId == "test")
            {
                List<Result> lstResult1 = new List<Result>();
                return lstResult1;
            }
            Result resultBuildingConfiguration = new Result();
            List<Result> lstResult = new List<Result>();
            resultBuildingConfiguration.message = "Building Elevation Deleted Successfully";
            lstResult.Add(resultBuildingConfiguration);
            return lstResult;
        }



        /// <summary>
        /// To generate building name by projectId
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        //public int GenerateBuildingName(int projectId)
        //{
        //    if (userId == null || projectId == 0 || buildingName == null || bldVariablejson == null)
        //    {
        //        return null;
        //    }
        //    else if (buildingName == "Test")
        //    {
        //        AutoSaveBuildingResult resBuildingConfiguration1 = new AutoSaveBuildingResult();
        //        List<AutoSaveBuildingResult> lstResult1 = new List<AutoSaveBuildingResult>();
        //        resBuildingConfiguration1.message = "Building Configuration Not AutoSaved Successfully";
        //        lstResult1.Add(resBuildingConfiguration1);
        //        return lstResult1;
        //    }
        //    AutoSaveBuildingResult resBuildingConfiguration = new AutoSaveBuildingResult();
        //    resBuildingConfiguration.result = 1;
        //    List<AutoSaveBuildingResult> lstResult = new List<AutoSaveBuildingResult>();
        //    resBuildingConfiguration.message = "Building Configuration AutoSaved Successfully";
        //    lstResult.Add(resBuildingConfiguration);
        //    return lstResult;
        //}



        public List<Result> AutoSaveBuildingElevation(DataTable dtBuildingElevation)
        {
            if (dtBuildingElevation == null)
            {
                return null;
            }

            Result resultBuildingElevation = new Result();
            List<Result> lstResult = new List<Result>();
            resultBuildingElevation.message = "Building Elevation AutoSaved Successfully";
            lstResult.Add(resultBuildingElevation);
            return lstResult;
        }

        /// <summary>
        /// To generate building name by projectId
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public int GenerateBuildingName(string projectId)
        {
            return 0;
        }


        public QuickSummary QuickConfigurationSummary(string opportunityId, int buildingId, int groupId, int setId, string sessionId)
        {
            if (buildingId > 0 || groupId > 0 || setId > 0)
            {
                QuickSummary result = new QuickSummary();
                BuildingDetails bld = new BuildingDetails();
                bld.id = 1;
                bld.name = "building1";
                result.building = bld;
                GroupDetails grp = new GroupDetails();
                grp.id = 1;
                grp.name = "group1";
                result.group = grp;
                OpportunityDetails proj = new OpportunityDetails();
                proj.AccountId = "AccountId";
                proj.AccountName = "AccountName";
                proj.AccountType = "AccountType";
                proj.Id = null;
                proj.OpportunityName = "OpportunityName";
                ProjectDetails pd = new ProjectDetails();
                pd.numberOfBuildings = 1;
                proj.projectDetails = pd;
                //proj.projectDetails.numberOfBuildings = 1;
                result.project = proj;
                SelectedUnits ud = new SelectedUnits();
                ud.ueid = "ueid";
                ud.unitid = 1;
                ud.unitname = "U1";
                UnitsTable lst = new UnitsTable();
                //lst.Add(ud);
                result.units = lst;

                return result;
            }
            else if (!opportunityId.Equals(null) || buildingId > 0 || groupId > 0 || setId > 0 || !sessionId.Equals(null))
            {
                QuickSummary qs = new QuickSummary();
                return qs;

            }
            else
            {
                throw new NotImplementedException();
            }



        }

        public bool CheckGroupExists(int buildingId)
        {
            if (buildingId > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public DataSet DuplicateBuildingConfigurationById(DataTable buildingIDDataTable, string projectId)
        {
            throw new NotImplementedException();
        }

        public bool GetBuildingConfigurationSectionTab(int buildingId)
        {
            if (buildingId > 0)
            {
                return true;
            }
            throw new NotImplementedException();
        }

        public LogHistoryResponse GetLogHistoryBuilding(int BuildingId, string lastDate)
        {
            if (BuildingId != 0)
            {
                var loghistoryresponse = new LogHistoryResponse();
                loghistoryresponse.Data = new List<Data>();
                Data data = new Data();
                string date;
                date = "09 / 03 / 2021";
                List<LogParameters> logparamters = new List<LogParameters>();
                {

                    foreach (var num in Enumerable.Range(1, 5))
                    {

                        LogParameters log = new LogParameters();
                        log.VariableId = "Building_Configuration.Parameters.Basic_Info.BLDGNAME";
                        log.Name = "Building Name";
                        log.UpdatedValue = "B86";
                        log.PreviousValue = " ";
                        log.User = "c2duser";
                        log.Role = " ";
                        log.Time = "08:08 am";
                        logparamters.Add(log);

                    }
                    data = new Data { Date = date, LogParameters = logparamters };
                }

                loghistoryresponse.Section = "Building";
                loghistoryresponse.Description = "B86";
                loghistoryresponse.Data.Add(data);
                loghistoryresponse.ShowLoadMore = false;
                return loghistoryresponse;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public DataSet GetQuoteDetails(string quoteId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetPermissionByRole(int id, string roleName)
        {
            throw new NotImplementedException();
        }




        public Task<ResponseMessage> GetListOfConfigurationForQuote(string quoteId, string sessionId)
        {
            throw new NotImplementedException();
        }

        public string GetProductCategoryBySetId(int id, string type)
        {
            if (id != 0 && !string.IsNullOrEmpty(type))
            {
                return string.Empty;
            }
            else
            {
                throw new NotImplementedException();
            }

        }

        public List<ListOfConfiguration> GetListofConfigurationForProject(string quoteId, DataTable configVariables)
        {
            throw new NotImplementedException();
        }


        List<ConfigVariable> IBuildingConfigurationDL.GetBuildingConfigurationById(int buildingId)
        {
            if (buildingId > 0)
            {
                return new List<ConfigVariable> { new ConfigVariable { VariableId = "Building_Configuration.Parameters.BLDGNAME", Value = "val"} };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST
                });
            }
        }

        List<Result> IBuildingConfigurationDL.SaveBuildingConfigurationForProject(int buildingId, string userId, string projectId, string buildingName, string bldVariablejson, ConflictsStatus isEditFlow, bool hasConflictsFlag, List<ConfigVariable> mapperVariablesForSP)
        {
            if (buildingId > 0)
            {
                return new List<Result> { new Result { buildingId = 1, message = "Building Saved Successfully", result = 1 } };
            }
            else
            {
                return new List<Result> { new Result { buildingId = 1, message = "", result = -2 } };
            }
        }

        List<Result> IBuildingConfigurationDL.SaveBuildingElevation(DataTable dtBuildingElevation)
        {
            List<Result> newRes = new List<Result>();
            var newRRs = new Result();
            newRRs.buildingId = 1;
            newRRs.message = "Building Saved Successfully";
            newRRs.result = 1;
            newRes.Add(newRRs);
            return newRes;
        }

        List<Result> IBuildingConfigurationDL.AutoSaveBuildingElevation(DataTable dtBuildingElevation)
        {
            if (dtBuildingElevation.Rows.Count > 0)
            {
                return new List<Result> { new Result { buildingId = 1, message = "", result = 1 } };
            }
            else
            {
                return new List<Result> { new Result { buildingId = 1, message = "", result = -2 } };
            }
        }

        List<Result> IBuildingConfigurationDL.UpdateBuildingElevation(DataTable dtBuildingElevation, List<ConfigVariable> mapperVariables)
        {
            if (mapperVariables.Count > 0)
            {
                return new List<Result> { new Result { buildingId = 1, message = "Building Updated Successfully", result = 1 } };
            }
            else
            {
                return new List<Result> { new Result { buildingId = 1, message = "", result = -2 } };
            }
        }


        List<Result> IBuildingConfigurationDL.DeleteBuildingConfigurationById(int buildingConfigurationId, string userId)
        {
            if (buildingConfigurationId > 0)
            {
                return new List<Result> { new Result {
                buildingId=1,result=1,message="Building Deleted Successfully"} };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST
                });
            }
        }

        List<Result> IBuildingConfigurationDL.DeleteBuildingElevationById(int buildingConfigurationId, string userId)
        {
            if (buildingConfigurationId > 0)
            {
                return new List<Result> { new Result {
                buildingId=1,result=1} };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST
                });
            }
        }

        int IBuildingConfigurationDL.GenerateBuildingName(string quoteId)
        {
            if (quoteId is not null)
            {
                return 1;
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST
                });
            }
        }

        QuickSummary IBuildingConfigurationDL.QuickConfigurationSummary(string opportunityId, int buildingId, int groupId, int setId, string sessionId)
        {
            if (buildingId == 0|| buildingId == 1)
            {
                return new QuickSummary { building = new BuildingDetails {id=1,name="",numberOfGroups=1 },units
                =new UnitsTable { selectedUnits= new List<SelectedUnits> { new SelectedUnits { ueid="",unitid=1,unitname=""}
                },model="",unitConfigurationDetails=new UnitConfigurationDetails {variables= new List<VariablesDetails> { 
                new VariablesDetails{ id="",name=""} } },unitDetails= new UnitConfigurationDetails1 { 
                speed="",status="",capacity="",machineType="",depth="",pitDepth="",motorTypeSize="",dimensionSelection="",
                availableFinishWeight="",overHead="",width=""} }
                };
            }

            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST
                });
            }
        }

        bool IBuildingConfigurationDL.CheckGroupExists(int buildingId)
        {
            if (buildingId > 0)
            {
                return true;
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST
                });
            }
        }

        DataSet IBuildingConfigurationDL.DuplicateBuildingConfigurationById(DataTable buildingIDDataTable, string quoteId)
        {
            DataSet ds = new DataSet();
            
            if (string.Equals(quoteId, "1"))
            {
                return ds;
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST
                });
            }
        }

        bool IBuildingConfigurationDL.GetBuildingConfigurationSectionTab(int buildingId)
        {
            if(buildingId>0)
            {
                return true;
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST
                });
            }
        }

        LogHistoryResponse IBuildingConfigurationDL.GetLogHistoryBuilding(int BuildingId, string lastDate)
        {
            throw new NotImplementedException();
        }

        DataSet IBuildingConfigurationDL.GetQuoteDetails(string quoteId)
        {
            throw new NotImplementedException();
        }

        List<string> IBuildingConfigurationDL.GetPermissionByRole(int id, string roleName)
        {
            if (id > 0)
            {
                return new List<string> { ""};
            }
            else
            {
                return null;
            }
        }

        List<Permissions> IBuildingConfigurationDL.GetPermissionForConfiguration(string quoteId, string roleName)
        {
            if (quoteId.Length > 0)
            {
                return new List<Permissions> { new Permissions { BuildingStatus="",GroupStatus="",ProjectStage="",UnitStatus="",Entity= "Building", PermissionKey= "Key_derived" } };
            }
            else
            {
                return null;
            }
        }

        Task<ResponseMessage> IBuildingConfigurationDL.GetListOfConfigurationForQuote(string quoteId, string sessionId)
        {
            throw new NotImplementedException();
        }


        public List<ListOfConfiguration> GetListOfConfigurationForProject(string quoteId, DataTable configVariables, string sessionId)
        {
            if(quoteId.Length>0)
            {
                return new List<ListOfConfiguration> { new ListOfConfiguration {Id=1,Permissions= new List<string> { "value"},
                BuildingName="BuildingName",BuildingStatus= new Status{StatusId=1,StatusKey="",StatusName="",Description="",DisplayName="" },
                Groups= new List<GrupConfiguration>{ new GrupConfiguration { groupStatus=new Status { StatusId=1,StatusKey="",StatusName="",
                Description="",DisplayName=""},groupId=1,groupName="",NeedsValidation=true,productCategory="",
                ConflictsStatus=new ConflictsStatus{},Permissions=new List<string>{"" },Units=new List<Unit>{ new Unit {
                speed="",SetId=1,Description="",SetName="",capacity="",RearOpening=1,FrontOpenings=1,price=212,ProductId="",Product=""
                ,UnitPosition="",unitName="",unitId=1,UEID="",Permissions= new List<string>{ ""},Landings=1,Factory
                =new Factory{FactoryJobId="",IsReadOnly=false },Status=new Status{ StatusId=1,StatusKey="",StatusName="",Description=""
                ,DisplayName=""},ConflictsStatus= new ConflictsStatus{},CreatedOn=new DateTime{ }
                } }
                }
                } } };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST
                });
            }
        }

        public List<BuildingElevation> GetBuildingElevationById(int buildingId, List<ConfigVariable> mapperVariables, string sessionId)
        {
            if(buildingId==0)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST
                });
            }
            return new List<BuildingElevation> { new BuildingElevation { buildingConfigurationId = 1 } };
        }

        public List<string> GetBuildingConflicts(int buildingId)
        {
            return new List<string> { "Conflict"};
        }
        //public QuickSummary QuickConfigurationSummary(string opportunityId, int buildingId, int groupId, int setId, string sessionId)
        //{
        //    if(!opportunityId.Equals(null) || buildingId>0 || groupId>0 || setId>0 || !sessionId.Equals(null))
        //    {
        //        QuickSummary qs = new QuickSummary();
        //        return qs;

        //    }

        //    throw new NotImplementedException();


        //}
        //public List<ListOfConfiguration> GetListOfConfigurationForProject(string projectId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
