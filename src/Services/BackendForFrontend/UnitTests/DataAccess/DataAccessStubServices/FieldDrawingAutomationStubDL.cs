using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Database;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.Test.DataAccess.DataAccessStubServices
{
    class FieldDrawingAutomationStubDL : IFieldDrawingAutomationDL
    {
        public List<RecurringJobData> CheckHangFireRecurringJob(int IntegratedProcessId)
        {
            throw new NotImplementedException();
        }

        public List<Reference> CheckRequestIdByFDAIntegrationId(int fieldDrawingIntegrationMasterId)
        {
            throw new NotImplementedException();
        }

        public object GenerateWrapperToken(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GenerateWrapperToken()
        {
            throw new NotImplementedException();
        }

        public object GetAccessToken(string v1, string userName, string v2, object password)
        {
            throw new System.NotImplementedException();
        }

        public List<ConfigVariable> GetBuildingGroupVariablesByGroupId(int groupid)
        {
            throw new System.NotImplementedException();
        }

        public List<ConfigVariable> GetFieldDrawingAutomationByGroupId(int groupid)
        {
            if (groupid > 0)
            {
                ConfigVariable cv = new ConfigVariable();
                cv.VariableId = "VariableId";
                cv.Value = "value";
                List<ConfigVariable> lstResult = new List<ConfigVariable>();
                lstResult.Add(cv);
                return lstResult;
            }

            throw new System.NotImplementedException();
        }

        public List<ConfigVariable> GetFieldDrawingAutomationByGroupId(int groupid, string projectId)
        {
            List<SqlParameter> sp = new List<SqlParameter>();
            List<ConfigVariable> listGroup = new List<ConfigVariable>();
            DataSet ds = new DataSet();

            List<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID, groupid),
                new SqlParameter(Constant.@OPPORTUNITYID, projectId),
                new SqlParameter(Constant.ACTION, Constant.ACTIONFIELDDRAWINGAUTOMATION),
            };

            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGGETFIELDDRAWINGAUTOMATIONBYGROUPID, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ConfigVariable us = new ConfigVariable();
                    us.VariableId = Convert.ToString(ds.Tables[0].Rows[i][Constant.FDATYPE]);
                    us.Value = Convert.ToString(ds.Tables[0].Rows[i][Constant.FDAVALUE]);
                    listGroup.Add(us);
                }
            }
            return listGroup;
        }

        public List<UnitLayOutDetails> GetFieldDrawingAutomationLayoutByGroupId(int groupid)
        {
            if (groupid > 0)
            {
                DoorOpenings x = new DoorOpenings();

                UnitLayOutDetails cv = new UnitLayOutDetails();
                cv.displayCarPosition = "displayCarPosition";
                cv.doorOpenings = x;
                cv.unitCurrentlyConfigured = true;
                cv.unitDesignation = "unitDesignation";
                List<UnitLayOutDetails> lstResult = new List<UnitLayOutDetails>();
                lstResult.Add(cv);
                return lstResult;
            }
            throw new System.NotImplementedException();
        }

        public List<UnitLayOutDetails> GetFieldDrawingAutomationLayoutByGroupId(int groupid, string projectId)
        {
            List<SqlParameter> sp = new List<SqlParameter>();
            List<UnitLayOutDetails> listGroup = new List<UnitLayOutDetails>();
            UnitsDetails units = new UnitsDetails();

            DataSet ds = new DataSet();

            List<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID, groupid),
                new SqlParameter(Constant.@OPPORTUNITYID, projectId),
                new SqlParameter(Constant.ACTION, Constant.ACTIONFIELDDRAWINGLAYOUTDETAILS),
            };

            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGGETFIELDDRAWINGAUTOMATIONBYGROUPID, param);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                units.unitLayoutDetails = (from DataRow row in ds.Tables[0].Rows
                                           where ds.Tables[0].Columns.Contains(Constant.UNITID)
                                           select new UnitLayOutDetails
                                           {
                                               unitDesignation = Convert.ToString(row[Constant.UNITSDESIGNATION]),
                                               displayCarPosition = Convert.ToString(row[Constant.DISPLAYCARPOSITION]),
                                               //unitCurrentlyConfigured = Convert.ToBoolean(row[Constant.UNITCURRENTLYCONFIGURED])
                                           }).ToList();

                foreach (var Unitdoordetails in units.unitLayoutDetails)
                {
                    DoorOpenings doorOpenings = new DoorOpenings();
                    doorOpenings.frontDoor = (from DataRow row in ds.Tables[0].Rows
                                              where ds.Tables[0].Columns.Contains(Constant.UNITID) && Utility.CheckEquals(Convert.ToString(row[Constant.UNITDESIGNATION]), Unitdoordetails.unitDesignation)
                                              select new Front
                                              {
                                                  isSelected = false,
                                                  doorTypeHand = (Convert.ToString(row[Constant.FRONTDOOR])) == String.Empty ? "Center" : Convert.ToString(row[Constant.FRONTDOOR])

                                              }).FirstOrDefault();


                    doorOpenings.rearDoor = (from DataRow row in ds.Tables[0].Rows
                                             where ds.Tables[0].Columns.Contains(Constant.UNITID) && Utility.CheckEquals(Convert.ToString(row[Constant.UNITDESIGNATION]), Unitdoordetails.unitDesignation)
                                             select new Front
                                             {
                                                 isSelected = false,
                                                 doorTypeHand = Convert.ToString(row[Constant.REARDOOR])

                                             }).FirstOrDefault();
                    doorOpenings.leftSideDoor = (from DataRow row in ds.Tables[0].Rows
                                                 where ds.Tables[0].Columns.Contains(Constant.UNITID) && Utility.CheckEquals(Convert.ToString(row[Constant.UNITDESIGNATION]), Unitdoordetails.unitDesignation)
                                                 select new Front
                                                 {
                                                     isSelected = false,
                                                     //doorTypeHand = Convert.ToString(row[Constant.LEFTSIDEDOOR])

                                                 }).FirstOrDefault();
                    doorOpenings.rightSideDoor = (from DataRow row in ds.Tables[0].Rows
                                                  where ds.Tables[0].Columns.Contains(Constant.UNITID) && Utility.CheckEquals(Convert.ToString(row[Constant.UNITDESIGNATION]), Unitdoordetails.unitDesignation)
                                                  select new Front
                                                  {
                                                      isSelected = false,
                                                      //doorTypeHand = Convert.ToString(row[Constant.RIGHTSIDEDOOR])

                                                  }).FirstOrDefault();



                    //doorOpenings.frontDoor = (from DataRow row in ds.Tables[0].Rows
                    //                          where ds.Tables[0].Columns.Contains(Constant.UNITID) && Utility.CheckEquals(Convert.ToString(row[Constant.UNITDESIGNATION]), Unitdoordetails.unitDesignation)
                    //                          select new Front
                    //                          {
                    //                              isSelected = false,
                    //                              doorTypeHand = Convert.ToString(row[Constant.FRONTDOOR])

                    //                          }).FirstOrDefault();

                    //doorOpenings.rearDoor = (from DataRow row in ds.Tables[0].Rows
                    //                         where ds.Tables[0].Columns.Contains(Constant.UNITID) && Utility.CheckEquals(Convert.ToString(row[Constant.UNITDESIGNATION]), Unitdoordetails.unitDesignation)
                    //                         select new Front
                    //                         {
                    //                             isSelected = false,
                    //                             doorTypeHand = Convert.ToString(row[Constant.REARDOOR])

                    //                         }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(doorOpenings.frontDoor.doorTypeHand))
                    {
                        doorOpenings.frontDoor.isSelected = true;
                    }
                    else if (!string.IsNullOrEmpty(doorOpenings.rearDoor.doorTypeHand))
                    {
                        doorOpenings.rearDoor.isSelected = true;
                    }
                    Unitdoordetails.doorOpenings = doorOpenings;
                }
            }
            return units.unitLayoutDetails;

        }

        public GroupDetailsForDrawingDetails GetFieldDrawingsByGroupId(int groupid, string quoteId)
        {
            throw new NotImplementedException();
        }

        public GroupDetailsForDrawingDetails GetFieldDrawingsByProjectId(string OpportunityId)
        {
            if (OpportunityId != null)
            {
                GroupDetailsForDrawingDetails req = new GroupDetailsForDrawingDetails();
                return req;
            }
            throw new System.NotImplementedException();
        }

        public string GetFieldDrawingStatusByGroupId(int groupId, string quoteId)
        {
            throw new NotImplementedException();
        }

        public string GetGroupStatusByGroupId(int groupid)
        {
            throw new NotImplementedException();
        }

        public FieldDrawingStatus GetLayoutRequestIdWithStatus(int groupid, string quoteId)
        {
            throw new NotImplementedException();
        }

        public List<ConfigVariable> GetLiftDesignerByGroupId(int groupid)
        {
            throw new NotImplementedException();
        }

        //public List<FDATypes> GetOutputTypesForXMGeneration(int groupid)
        //{
        //    throw new NotImplementedException();
        //}

        public string GetProjectStatusByGroupId(string quoteId)
        {
            throw new NotImplementedException();
        }

        public string GetQuoteIdByGroupId(int groupId)
        {
            throw new NotImplementedException();
        }

        public List<Reference> GetRequestIdsByGroupId(int groupid)
        {
            throw new NotImplementedException();
        }

        public RequestHistory GetRequestQueueByGroupId(int groupId)
        {
            if (groupId > 0)
            {
                RequestHistory req = new RequestHistory();
                return req;
            }
            throw new System.NotImplementedException();
        }

        public GroupDetailsForSendToCoordination GetSendToCoordinationByProjectId(string OpportunityId)
        {
            if (!OpportunityId.Equals(Constant.EMPTYSTRING))
            {
                GroupDetailsForSendToCoordination req = new GroupDetailsForSendToCoordination();
                return req;
            }
            throw new System.NotImplementedException();
        }

        public List<UnitVariables> GetUnitsVariablesByGroupId(int groupid)
        {
            throw new System.NotImplementedException();
        }

        public List<UnitVariables> GetUnitsVariablesWithUnitByGroupId(int groupid)
        {
            throw new System.NotImplementedException();
        }

        //public int LayoutStatus(int groupId, string projectId, string userName, string ReferenceId, string WrapperToken);
        public List<ConfigVariable> GetXMLGenerationDetailsByGroupId(int groupid)
        {
            throw new System.NotImplementedException();
        }

        public JObject LayoutStatus(string FormsLayoutReferenceId)
        {
            throw new System.NotImplementedException();
        }

        public int LayoutStatus(int groupId, string projectId, string userName, string ReferenceId, int IntegratedProcessId, string WrapperToken)
        {
            throw new System.NotImplementedException();
        }

        public string RequestLayouts(JObject json, string token)
        {
            throw new System.NotImplementedException();
        }

        public List<ResultGroupConfiguration> SaveFDAByGroupId(int groupid, string projectId, string createdBy, List<ConfigVariable> FieldDrawingAutomationDataTable, List<ConfigVariable> groupVariableAssignment)
        {
            throw new NotImplementedException();
        }

        public ResultGroupConfiguration SaveFdaByGroupId(int groupid, string quoteId, string createdBy, List<ConfigVariable> FieldDrawingAutomationDataTable, List<ConfigVariable> groupVariableAssignment)
        {
            throw new NotImplementedException();
        }

        public List<ResultGroupConfiguration> SaveFieldDrawingAutomationByGroupId(int groupid, string projectid, string createdBy, List<ConfigVariable> FieldDrawingAutomationDataTable, List<ConfigVariable> groupVariablesAssignment)
        {
            throw new System.NotImplementedException();
        }

        public int SaveReferenceId(int groupId, string projectId, string userName, int statusId, int intergratedSystemId, string referenceId)
        {
            throw new System.NotImplementedException();
        }

        public int SaveReferenceId(int groupId, string projectId, string userName, string statusId, int intergratedSystemId, string referenceId)
        {
            throw new NotImplementedException();
        }

        public List<ResultGroupConfiguration> SaveSendToCoordination(string projectId, string createdBy, List<SendToCoordinationData> coordinationData)
        {
            if (projectId.Equals(Constant.EMPTYSTRING))
            {
                throw new System.NotImplementedException();
            }
            ResultGroupConfiguration resultMessage = new ResultGroupConfiguration();
            resultMessage.Message = Constant.FDASAVEMESSAGE;
            List<ResultGroupConfiguration> result = new List<ResultGroupConfiguration>();
            result.Add(resultMessage);
            return result;
        }

        public List<ResultGroupConfiguration> UpdateFDADrawingMethodByGroupId(int groupid, int drawingMethod)
        {
            throw new NotImplementedException();
        }

        public List<ResultGroupConfiguration> UpdateFdaDrawingMethodByGroupId(int groupid, int drawingMethod)
        {
            throw new NotImplementedException();
        }

        public void UpdateFDARequestStatusByFieldDrawingId(int fieldDrawingId)
        {
            throw new NotImplementedException();
        }

        public int UpdateLockPropertyForGroups(string projectId, int groupId, int islock)
        {
            throw new NotImplementedException();
        }

        //Task IFieldDrawingAutomationDL.LayoutStatus(int groupId, string projectId, string userName, string ReferenceId, int IntegratedProcessId, string WrapperToken)
        //{
        //    throw new NotImplementedException();
        //}

        Task<string> IFieldDrawingAutomationDL.RequestLayouts(JObject json, string token)
        {
            throw new NotImplementedException();
        }

        List<ResultGroupConfiguration> IFieldDrawingAutomationDL.UpdateLockPropertyForGroups(string projectId, int groupId, string islock)
        {
            throw new NotImplementedException();
        }

        public   List<ConfigVariable> GetFieldDrawingAutomationByGroupId(int groupid, string projectId, DataTable ConfigVariables)
        {
            throw new NotImplementedException();
        }

        public List<UnitLayOutDetails> GetFieldDrawingAutomationLayoutByGroupId(int groupid, string projectId, DataTable ConfigVariables)
        {
            throw new NotImplementedException();
        }

        public GroupDetailsForDrawingDetails GetFieldDrawingsByProjectId(string OpportunityId, DataTable dtVariables)
        {
            if (OpportunityId != null)
            {
                GroupDetailsForDrawingDetails req = new GroupDetailsForDrawingDetails();
                return req;
            }
            throw new System.NotImplementedException();
        }


        public Task GetLayoutStatus(int groupId, string projectId, string userName, string ReferenceId, int IntegratedProcessId, string WrapperToken)
        {
            throw new NotImplementedException();
        }

        public List<UnitVariables> GetUnitsVariablesWithUnitByGroupId(int groupid, DataTable ConfigVariables)
        {
            throw new NotImplementedException();
        }



        public List<ConfigVariable> GetLiftDesignerByGroupId(int groupid, string storedProcedureName)
        {
            throw new NotImplementedException();
        }



        public List<ResultGroupConfiguration> UpdateLockPropertyForGroups(string projectId, int groupId, string islock)
        {
            throw new NotImplementedException();
        }





        public List<FDAOutputTypes> GetOutputTypesForXMGeneration(int groupid)
        {
            throw new NotImplementedException();
        }



        public GroupDetailsForDrawingDetails GetFieldDrawingsByGroupId(int groupid, string quoteId, DataTable dtVariables)
        {
            throw new NotImplementedException();
        }

        






        /// <summary>
        /// GetPermissionForFDA
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public List<Permissions> GetPermissionForFDA(string quoteId, string roleName, string entity)
         {
            List<Permissions> permission = new List<Permissions>();
            Permissions perm = new Permissions();
            perm.Entity = "First";
            perm.PermissionKey = "Keyvault";
            perm.ProjectStage = "Last";
            perm.BuildingStatus = "Built";
            perm.GroupStatus = "Checked";
            perm.UnitStatus = "Done";
            permission.Add(perm);
            return permission;
          }
    /// <summary>
    /// GetSendToCoordinationStatus
    /// </summary>
    /// <param name="qouteId"></param>
    /// <returns></returns>
        public SendToCoordinationStatus GetSendToCoordinationStatus(string qouteId)
        {
            if (qouteId != null)
            {
                SendToCoordinationStatus sendStatus = new SendToCoordinationStatus();
                Status status = new Status();
                status.StatusId = Convert.ToInt32(qouteId);
                status.DisplayName = "Elevation_Success";
                status.StatusName = "Elevator_Status";
                status.StatusKey = "Elevator_Key";
                status.Description = "Elevator_Display";
                sendStatus.CoordinationStatus = status;
                return sendStatus;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        List<UnitVariables> GetBuildingGroupVariablesWithUnitByGroupId(int groupid, DataTable dtConfigVariable)
        {
            throw new NotImplementedException();
        }

        List<UnitVariables> GetUnitsVariablesWithUnitDetailsByGroupId(int groupid, DataTable dtVariables)
        {
            throw new NotImplementedException();
        }

        List<UnitVariables> GetXMLVariablesWithUnitByGroupId(int groupid, DataTable dtVariables)
        {
            throw new NotImplementedException();
        }

        ProjectDet GetOpportunityAndVersionByQuoteId(string quoteId)
        {
            throw new NotImplementedException();
        }
        int GetBuildingId(int groupId)
        {
            throw new NotImplementedException();
        }
        string GetLDResponseJson(int groupId)
        {
            throw new NotImplementedException();
        }

        List<ConfigVariable> IFieldDrawingAutomationDL.GetFieldDrawingAutomationByGroupId(int groupid, string projectId, DataTable ConfigVariables)
        {
            return new List<ConfigVariable> { new ConfigVariable { Value = "", VariableId = "" } };
        }

        List<UnitLayOutDetails> IFieldDrawingAutomationDL.GetFieldDrawingAutomationLayoutByGroupId(int groupid, string projectId, DataTable ConfigVariables)
        {
            if (groupid > 0)
            {
                DoorOpenings x = new DoorOpenings();

                UnitLayOutDetails cv = new UnitLayOutDetails();
                cv.displayCarPosition = "displayCarPosition";
                cv.doorOpenings = x;
                cv.unitCurrentlyConfigured = true;
                cv.unitDesignation = "unitDesignation";
                cv.doorOpenings.frontDoor = new Front { isSelected=true,doorTypeHand=""};
                cv.doorOpenings.leftSideDoor = new Front { isSelected = true, doorTypeHand = "" };
                cv.doorOpenings.rightSideDoor = new Front { isSelected = true, doorTypeHand = "" };
                cv.doorOpenings.rearDoor = new Front { isSelected = true, doorTypeHand = "" };
                List<UnitLayOutDetails> lstResult = new List<UnitLayOutDetails>();
                lstResult.Add(cv);
                return lstResult;
            }
            throw new System.NotImplementedException();
        }

        GroupDetailsForDrawingDetails IFieldDrawingAutomationDL.GetFieldDrawingsByProjectId(string OpportunityId, DataTable dtVariables)
        {

                return new GroupDetailsForDrawingDetails { IsPrimaryQuote=true};

        }

        RequestHistory IFieldDrawingAutomationDL.GetRequestQueueByGroupId(int groupId)
        {
            if(groupId>0)
            {
                return new RequestHistory { requestHistory= new List<RequestQueue> { new RequestQueue { GroupId=1} } };
            }
            else
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR
                });
            }
        }

        Task<string> IFieldDrawingAutomationDL.GenerateWrapperToken()
        {
            throw new NotImplementedException();
        }

        Task IFieldDrawingAutomationDL.GetLayoutStatus(int groupId, string projectId, string userName, string ReferenceId, int IntegratedProcessId, string WrapperToken)
        {
            throw new NotImplementedException();
        }

        int IFieldDrawingAutomationDL.SaveReferenceId(int groupId, string projectId, string userName, string statusId, int intergratedSystemId, string referenceId)
        {
            throw new NotImplementedException();
        }

        List<UnitVariables> IFieldDrawingAutomationDL.GetUnitsVariablesWithUnitByGroupId(int groupid, DataTable ConfigVariables)
        {
            throw new NotImplementedException();
        }

        List<Reference> IFieldDrawingAutomationDL.CheckRequestIdByFDAIntegrationId(int fieldDrawingIntegrationMasterId)
        {
            throw new NotImplementedException();
        }

        List<ConfigVariable> IFieldDrawingAutomationDL.GetLiftDesignerByGroupId(int groupid, string storedProcedureName)
        {
            return new List<ConfigVariable> { new ConfigVariable { Value = "", VariableId = "" } };
        }

        ResultGroupConfiguration IFieldDrawingAutomationDL.SaveFdaByGroupId(int groupid, string quoteId, string createdBy, List<ConfigVariable> FieldDrawingAutomationDataTable, List<ConfigVariable> groupVariableAssignment)
        {
            if (groupid >0)
            {
                return new ResultGroupConfiguration { Result =1 } ;
            }
            else
            {
                throw new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR
                });
            }
        }

        List<ResultGroupConfiguration> IFieldDrawingAutomationDL.UpdateFdaDrawingMethodByGroupId(int groupid, int drawingMethod)
        {
            throw new NotImplementedException();
        }

        List<RecurringJobData> IFieldDrawingAutomationDL.CheckHangFireRecurringJob(int IntegratedProcessId)
        {
            throw new NotImplementedException();
        }

    

        List<FDAOutputTypes> IFieldDrawingAutomationDL.GetOutputTypesForXMGeneration(int groupid)
        {
            throw new NotImplementedException();
        }

        FieldDrawingStatus IFieldDrawingAutomationDL.GetLayoutRequestIdWithStatus(int groupid, string quoteId)
        {
            throw new NotImplementedException();
        }

        GroupDetailsForDrawingDetails IFieldDrawingAutomationDL.GetFieldDrawingsByGroupId(int groupid, string quoteId, DataTable dtVariables)
        {
            throw new NotImplementedException();
        }

        void IFieldDrawingAutomationDL.UpdateFDARequestStatusByFieldDrawingId(int fieldDrawingId)
        {
            throw new NotImplementedException();
        }

        string IFieldDrawingAutomationDL.GetGroupStatusByGroupId(int groupid)
        {
            return "val";
        }

        string IFieldDrawingAutomationDL.GetProjectStatusByGroupId(string quoteId)
        {
            return "val";
        }

        string IFieldDrawingAutomationDL.GetFieldDrawingStatusByGroupId(int groupId, string quoteId)
        {
            return "val";
        }

        string IFieldDrawingAutomationDL.GetQuoteIdByGroupId(int groupId)
        {
            throw new NotImplementedException();
        }

        List<Permissions> IFieldDrawingAutomationDL.GetPermissionForFDA(string quoteId, string roleName, string entity)
        {
            if (roleName != null)
            {
                return new List<Permissions> {new  Permissions{ BuildingStatus="",ProjectStage="",GroupStatus="",UnitStatus="",
            Entity="",PermissionKey=""} };
            }
            else
            {
                throw  new CustomException(new ResponseMessage()
                {
                    StatusCode = Constant.INTERNALSERVERERROR
                });
            }
        }

        SendToCoordinationStatus IFieldDrawingAutomationDL.GetSendToCoordinationStatus(string qouteId)
        {
            return new SendToCoordinationStatus { CoordinationStatus= new Status { StatusId= Convert.ToInt32(qouteId),
            StatusKey="",StatusName="",Description="",DisplayName=""} };
        }

        List<UnitVariables> IFieldDrawingAutomationDL.GetBuildingGroupVariablesWithUnitByGroupId(int groupid, DataTable dtConfigVariable)
        {
            throw new NotImplementedException();
        }

        List<UnitVariables> IFieldDrawingAutomationDL.GetUnitsVariablesWithUnitDetailsByGroupId(int groupid, DataTable dtVariables)
        {
            throw new NotImplementedException();
        }

        List<UnitVariables> IFieldDrawingAutomationDL.GetXMLVariablesWithUnitByGroupId(int groupid, DataTable dtVariables)
        {
            throw new NotImplementedException();
        }

        ProjectDet IFieldDrawingAutomationDL.GetOpportunityAndVersionByQuoteId(string quoteId)
        {
            throw new NotImplementedException();
        }

        int IFieldDrawingAutomationDL.GetBuildingId(int groupId)
        {
            throw new NotImplementedException();
        }

        string IFieldDrawingAutomationDL.GetLDResponseJson(int groupId)
        {
            throw new NotImplementedException();
        }

        public int SaveVariableArguments(int groupId, List<UnitVariables> listVariablesArgument, string userName)
        {
            throw new NotImplementedException();
        }
    }
}
