/************************************************************************************************************
************************************************************************************************************
    File Name     :   GroupConfigurationStubDL class 
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
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.BFF.DataAccess.Helpers;
using Constant = TKE.SC.BFF.DataAccess.Helpers.Constant;
using System.Data;
using Configit.Configurator.Server.Common;
using System.Linq;
using TKE.SC.Common.Model.ExceptionModel;


namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    public class GroupConfigurationStubDL : IGroupConfigurationDL
    {
        /// <summary>
        ///  This method used to get group configuration details by groupConfigurationId
        /// </summary>
        /// <param name="groupConfigurationId"></param>
        /// <returns></returns>
        public List<ConfigVariable> GetGroupConfigurationDetailsByGroupId(int groupConfigurationId , string selectedTab)
        {
            if (groupConfigurationId == 0)
            {
                return null;
            }
            ConfigVariable resultGroupConfiguration = new ConfigVariable();
            List<ConfigVariable> lstResult = new List<ConfigVariable>();
            resultGroupConfiguration.VariableId = "id";
            lstResult.Add(resultGroupConfiguration);
            return lstResult;
        }


        /// <summary>
        /// This method used to save group configuration and floorPlan
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="groupName"></param>
        /// <param name="userName"></param>
        /// <param name="grpVariablejson"></param>
        /// <returns></returns>
        public List<ResultGroupConfiguration> SaveGroupConfiguration(int buildingId, string groupName, string userName, string grpVariablejson )
        {
            if (buildingId == 0 || groupName == null|| userName == null || grpVariablejson == null)
            {
                throw new NotImplementedException();
            }
            else if (userName == "Test")
            {
                ResultGroupConfiguration resGroupConfiguration1 = new ResultGroupConfiguration();
                resGroupConfiguration1.Result = -1;
                List<ResultGroupConfiguration> lstResult1 = new List<ResultGroupConfiguration>();
                resGroupConfiguration1.Message = Constant.BUILDINGSAVEMESSAGE;
                lstResult1.Add(resGroupConfiguration1);
                return lstResult1;
            }
            ResultGroupConfiguration resGroupConfiguration = new ResultGroupConfiguration();
            resGroupConfiguration.Result = 1;
            List<ResultGroupConfiguration> lstResult = new List<ResultGroupConfiguration>();
            resGroupConfiguration.Message = Constant.BUILDINGSAVEMESSAGE;
            lstResult.Add(resGroupConfiguration);
            return lstResult;
        }

        /// <summary>
        /// This method is used to update Group configuration details
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="groupName"></param>
        /// <param name="groupConfigurationId"></param>
        /// <param name="grpVariablejson"></param>
        /// <param name="unitVariableAssignment"></param>
        /// <returns></returns>
        public List<ResultGroupConfiguration> UpdateGroupConfiguration(int buildingId, string groupName, int groupConfigurationId, string grpVariablejson)
        {
            if (buildingId == 0 || groupName == null || groupConfigurationId == 0 || grpVariablejson == null )
            {
                return null;
            }
            else if(groupConfigurationId.Equals(-1))
            {
                ResultGroupConfiguration resGroupConfiguration1 = new ResultGroupConfiguration();
                resGroupConfiguration1.Result = -1;
                List<ResultGroupConfiguration> lstResult1 = new List<ResultGroupConfiguration>();
                resGroupConfiguration1.Message = Constant.BUILDINGSAVEMESSAGE;
                lstResult1.Add(resGroupConfiguration1);
                return lstResult1;
            }
            else if (groupConfigurationId == -2)
            {
                ResultGroupConfiguration resGroupConfiguration1 = new ResultGroupConfiguration();
                List<ResultGroupConfiguration> lstResult1 = new List<ResultGroupConfiguration>();
                resGroupConfiguration1.Message = Constant.BUILDINGSAVEMESSAGE;
                lstResult1.Add(resGroupConfiguration1);
                return lstResult1;
            }
            ResultGroupConfiguration resGroupConfiguration = new ResultGroupConfiguration();
            resGroupConfiguration.Result = 1;
            List<ResultGroupConfiguration> lstResult = new List<ResultGroupConfiguration>();
            resGroupConfiguration.Message = Constant.BUILDINGSAVEMESSAGE;
            lstResult.Add(resGroupConfiguration);
            return lstResult;


        }



        /// <summary>
        /// Calling Sp & Get response from store procedure of Groupconfiguration by ProjectId
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> GetGroupConfigurationByBuildingId(string buildingId)
        {
            var getResponse = File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.GETGROUPCONFIGRESPONSE));
            if (buildingId == "TEST")
            {
                return new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Response = JObject.Parse(getResponse)
                };
            }
            else if(buildingId == "NO")
            {
                return new ResponseMessage
                {
                    StatusCode = Constant.NOTFOUND,
                    Response = JObject.Parse(getResponse)
                };
            }
            return new ResponseMessage
            {
                StatusCode = Constant.SUCCESS,
                Response = JObject.Parse(getResponse)
            };
        }



        /// <summary>
        /// To delete group configuration by groupId
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public List<GroupResult> DeleteGroupConfiguration(int GroupId)
        {
            if (GroupId == 2)
            {
                return null;
            }
            GroupResult resultDeleteGroupConfiguration = new GroupResult();
            List<GroupResult> lstResult = new List<GroupResult>();
            resultDeleteGroupConfiguration.Message = Constant.GROUPSAVEMESSAGE;
            lstResult.Add(resultDeleteGroupConfiguration);
            return lstResult;
            

        }



        /// <summary>
        /// To generate group name by buildingId
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public int GenerateGroupName(int buildingId)
        {
            if(buildingId>0)
            {
                return 0;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// To get Product Category
        /// </summary>
        /// <param name="productCategoryId"></param>
        /// <returns></returns>
        public string GenerateProductCategory(int productCategoryId)
        {
            return null;
        }

        public List<BuildingElevationData> GetFloorDesignationFloorNumberByGroupId(int groupId)
        {
            List<BuildingElevationData> res = new List<BuildingElevationData>();
            return res;
        }

        public string GetGroupValues(string VariableName)
        {
            return null;
        }

        public List<ResultGroupConfiguration> SaveGroupHallFixture(int groupId, GroupHallFixturesData groupHallFixturesData, string userId, int is_Saved,List<LogHistoryTable> logHistoryTable)
        {
            throw new NotImplementedException();
        }

        public List<GroupHallFixtures> GetGroupHallFixturesData(int groupid, string userName)
        {
            throw new NotImplementedException();
        }

        public string GetGroupFixtureStrategy(int groupConfigurationId)
        {
            return null;
        }

        public List<UnitDetail> GetUnitDetails(int groupConfigurationId)
        {
            throw new NotImplementedException();
        }


        public List<ResultSetConfiguration> DeleteGroupHallFixtureConsole(int groupId, int consoleId, string fixtureType,List<LogHistoryTable> historyTable,string userId)
        {
            if (groupId == 0 || consoleId == 0 || fixtureType == null )
            {
                return null;
            }
            else if (groupId == -1)
            {
                ResultSetConfiguration resGroupHallFixture1 = new ResultSetConfiguration();
                resGroupHallFixture1.result = -1;
                List<ResultSetConfiguration> lstResult1 = new List<ResultSetConfiguration>();
                resGroupHallFixture1.message = Constant.GROUPIDNOTFOUND;
                lstResult1.Add(resGroupHallFixture1);
                return lstResult1;

            }
            ResultSetConfiguration resGroupHallFixture2 = new ResultSetConfiguration();
            resGroupHallFixture2.result = 1;
            List<ResultSetConfiguration> lstResult2 = new List<ResultSetConfiguration>();
            resGroupHallFixture2.message = Constant.GROUPIDDELETED;
            lstResult2.Add(resGroupHallFixture2);
            return lstResult2;

        }

        public bool CheckUnitConfigured(int groupId)
        {
            if (groupId == 0)
            {
                return false;
            }
            return true;
        }

        public bool CheckProductSelected(int groupConfigurationId)
        {
            throw new NotImplementedException();
        }

        public DataSet DuplicateGroupConfigurationById(DataTable groupIDDataTable, int buildingID)
        {
            if(groupIDDataTable.Rows.Count==0 || buildingID==0)
            {
                throw new NotImplementedException();
            }
            return new DataSet();
        }

        public List<ConfigVariable> GetBuildingVariableAssignments(int groupConfigurationId)
        {
            throw new NotImplementedException();
        }

        public List<GroupHallFixtures> GetGroupHallFixturesData(int groupid, string userName, string fixtureStrategy)
        {
            throw new NotImplementedException();
        }

        public List<UnitDetailsValues> GetUnitDetails(int groupConfigurationId, List<VariableAssignment> doorDetails)
        {
            throw new NotImplementedException();
        }

        public LogHistoryResponse GetLogHistoryGroup(int groupId, string lastDate)
        {
            if (groupId != 0)
            {
                var loghistoryresponse = new LogHistoryResponse();
                loghistoryresponse.Data = new List<Data>();
                Data data = new Data();
                string date;
                date = Constant.RANDOMDATE;
                List<LogParameters> logparamters = new List<LogParameters>();
                {

                    foreach (var num in Enumerable.Range(1, 5))
                    {

                        LogParameters log = new LogParameters();
                        log.VariableId = TKE.SC.Common.Constants.GROUPDESIGNATIONNAME;
                        log.Name = Constant.GROUP;
                        log.UpdatedValue = Constant.G1;
                        log.PreviousValue = Constant.EMPTYSPACE;
                        log.User = Constant.USER;
                        log.Role = Constant.EMPTYSPACE;
                        log.Time = Constant.RANDOMTIME;
                        logparamters.Add(log);

                    }
                    data = new Data { Date = date, LogParameters = logparamters };
                }

                loghistoryresponse.Section = Constant.GROUP;
                loghistoryresponse.Description = Constant.G1;
                loghistoryresponse.Data.Add(data);
                loghistoryresponse.ShowLoadMore = false;
                return loghistoryresponse;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public bool GetGroupConfigurationSectionTab(int groupId)
        {
            throw new NotImplementedException();
        }

        //Dictionary<string, bool> IGroupConfigurationDL.GetGroupConfigurationSectionTab(int groupId)
        //{
        //    throw new NotImplementedException();
        //}

        public List<string> GetPermissionByRole(int id, string roleName)
        {
            throw new NotImplementedException();
        }

        public List<ResultGroupConfiguration> SaveGroupConfiguration(int buildingId, string groupName, string userName, string grpVariablejson, string productCategory, int numberOfUnits)
        {
            throw new NotImplementedException();
        }

        public string GetProductCategoryByGroupId(int id, string type)
        {
            throw new NotImplementedException();
        }

        public List<ConfigVariable> GetGroupInformationByGroupId(int groupId)
        {
            throw new NotImplementedException();
        }

        List<ResultGroupConfiguration> IGroupConfigurationDL.SaveGroupConfiguration(int buildingId, string groupName, string userName, string grpVariablejson, string productCategory, int numberOfUnits)
        {
           if(buildingId>0)
            {
                return new List<ResultGroupConfiguration> { new ResultGroupConfiguration { Result = 1, Message="Group Saved Successfully" } };
            }
           else
            {
                return new List<ResultGroupConfiguration> { new ResultGroupConfiguration { Result = 2 } };
            }
        }

        List<ResultGroupConfiguration> IGroupConfigurationDL.UpdateGroupConfiguration(int buildingId, string groupName, int groupConfigurationId, string grpVariablejson)
        {
            if (buildingId > 0)
            {
                return new List<ResultGroupConfiguration> { new ResultGroupConfiguration { Result = 1, Message = "Group Updated Successfully" } };
            }
            else
            {
                return new List<ResultGroupConfiguration> { new ResultGroupConfiguration { Result = 2 } };
            }
        }

        async Task<ResponseMessage> IGroupConfigurationDL.GetGroupConfigurationByBuildingId(string buildingId)
        {
            if(buildingId.Contains("B"))
            {
                return new ResponseMessage { StatusCode=200};
            }
            if (buildingId.Contains("NO"))
            {
                return new ResponseMessage { StatusCode = 404 };
            }
            else
            {
                return new ResponseMessage { StatusCode=400};
            }
        }

        List<GroupResult> IGroupConfigurationDL.DeleteGroupConfiguration(int GroupId)
        {
            if(GroupId>0)
            {
                return new List<GroupResult> { new GroupResult {Message= "Group Deleted Successfully" } };
            }
            else
            {
                return null;
            }
        }

        int IGroupConfigurationDL.GenerateGroupName(int buildingId)
        {
            return 1;
        }

        string IGroupConfigurationDL.GenerateProductCategory(int productCategoryId)
        {
            throw new NotImplementedException();
        }

        List<BuildingElevationData> IGroupConfigurationDL.GetFloorDesignationFloorNumberByGroupId(int groupId)
        {
            throw new NotImplementedException();
        }

        string IGroupConfigurationDL.GetGroupValues(string VariableName)
        {
            throw new NotImplementedException();
        }

        List<ResultGroupConfiguration> IGroupConfigurationDL.SaveGroupHallFixture(int groupId, GroupHallFixturesData groupHallFixturesData, string userId, int is_Saved, List<LogHistoryTable> historyTable)
        {
            if(groupId==1)
            {
                return new List<ResultGroupConfiguration> { new ResultGroupConfiguration {Result=0 } };
            }
            else
            {
                return new List<ResultGroupConfiguration> { new ResultGroupConfiguration { Result = 1, Message = "Group Hall Saved Successfully" } };
            }
        }

        List<GroupHallFixtures> IGroupConfigurationDL.GetGroupHallFixturesData(int groupid, string userName, string fixtureStrategy, List<ConfigVariable> hallStations)
        {
            throw new NotImplementedException();
        }

        List<UnitDetailsValues> IGroupConfigurationDL.GetUnitDetails(int groupConfigurationId, List<ConfigVariable> doorDetails)
        {
            throw new NotImplementedException();
        }

        string IGroupConfigurationDL.GetGroupFixtureStrategy(int groupConfigurationId)
        {
            if(groupConfigurationId>0)
            {
                return "string";
            }
            else
            {
                return "failstring";
            }
        }

        List<ResultSetConfiguration> IGroupConfigurationDL.DeleteGroupHallFixtureConsole(int groupId, int consoleId, string fixtureType, List<LogHistoryTable> historyTable, string userId)
        {
            if(groupId>0)
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result=1, message = "Group Hall Deleted Successfully" } };
            }
            else
            {
                return new List<ResultSetConfiguration> { new ResultSetConfiguration { result =0} };
            }
        }

        bool IGroupConfigurationDL.CheckUnitConfigured(int groupId)
        {
            return true;
        }

        bool IGroupConfigurationDL.CheckProductSelected(int groupConfigurationId)
        {
            throw new NotImplementedException();
        }

        DataSet IGroupConfigurationDL.DuplicateGroupConfigurationById(DataTable groupIDDataTable, int buildingID)
        {
            if (buildingID > 0)
            {
                return new DataSet { };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST
                });
            }
        }

        List<ConfigVariable> IGroupConfigurationDL.GetBuildingVariableAssignments(int groupConfigurationId)
        {
            throw new NotImplementedException();
        }

        LogHistoryResponse IGroupConfigurationDL.GetLogHistoryGroup(int groupId, string lastDate)
        {
            throw new NotImplementedException();
        }

        List<string> IGroupConfigurationDL.GetPermissionByRole(int id, string roleName)
        {
            if (id > 0)
            {
                return new List<string> { "READ" };
            }
            else

            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                });
            }
        }

        Dictionary<string, bool> IGroupConfigurationDL.GetGroupConfigurationSectionTab(int groupId, List<ConfigVariable> hallStations)
        {
            if(groupId>0)
            {
                return new Dictionary<string, bool> { { "OpeningLocations", true } };
            }
            else
            {
                return new Dictionary<string, bool> { { "OpeningLocations", true } };
            }
        }

        string IGroupConfigurationDL.GetProductCategoryByGroupId(int id, string type, DataTable configVariables)
        {
           if(id>0)
            {
                return "Elevator";
            }
           else
            {
                return "Escalator/Moving-Walk";
            }
        }

        List<ConfigVariable> IGroupConfigurationDL.GetGroupInformationByGroupId(int groupId)
        {
            if (groupId > 0)
            {
                return new List<ConfigVariable> { new ConfigVariable {Value="",VariableId="" } };
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                });
            }
        }

        public GroupLayout GetGroupConfigurationDetailsByGroupId(int groupConfigurationId, string selectTab, string sessionId)
        {
            return new GroupLayout { UpdatedTotalNumberOfFloors=1,ConfigVariable=new List<ConfigVariable>
            { new ConfigVariable{ Value="",VariableId=""} },DisplayVariableAssignmentsValues= new List<DisplayVariableAssignmentsValues>
            {new DisplayVariableAssignmentsValues{IsFutureElevator=true,Key="",MappedTo="",UnitDesignation="",Value="",VariableId="" }
            }
            };
        }

        public List<Result> SaveBuildingConflicts(int buildingId, List<VariableAssignment> conflictVariables, string entityType)
        {
            return new List<Result> { new Result {buildingId=1,message="Saved",result=1 } };
        }
    }
}
