using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
using TKE.SC.Common.Model.HttpClientModel;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class FieldDrawingAutomationDL : IFieldDrawingAutomationDL
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for Initializing the DataAccess Logic object
        /// </summary>
        /// <param Name="configuration"></param>
        /// <param Name="utility"></param>
        public FieldDrawingAutomationDL(IConfiguration configuration, ILogger<FieldDrawingAutomationDL> logger)
        {
            _configuration = configuration;
            Utility.SetLogger(logger);
        }

        /// <summary>
        /// Method to Get the Field Drawing details By GroupId
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        public List<ConfigVariable> GetFieldDrawingAutomationByGroupId(int groupid, string quoteId, DataTable configVariables)
        {
            var methodBeginTime = Utility.LogBegin();
            List<ConfigVariable> listGroup = new List<ConfigVariable>();
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID, groupid),
                new SqlParameter(Constant.@OPPORTUNITYID, quoteId),
                new SqlParameter(Constant.ACTION, Constant.ACTIONFIELDDRAWINGAUTOMATION),
                new SqlParameter(Constant.CONSTANTMAPPERLIST ,configVariables)
            };
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGGETFIELDDRAWINGAUTOMATIONBYGROUPID, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                listGroup = (from DataRow row in ds.Tables[0].Rows
                             select new ConfigVariable
                             {
                                 VariableId = Convert.ToString(row[Constant.FDATYPE]),
                                 Value = Convert.ToString(row[Constant.FDAVALUE]),
                             }).ToList();
            }
            Utility.LogEnd(methodBeginTime);
            return listGroup;
        }

        /// <summary>
        /// Method to Get the Variable Assignments By GroupId
        /// </summary>
        /// <param Name="groupid"></param>
        /// <returns></returns>
        public List<ConfigVariable> GetLiftDesignerByGroupId(int groupid, string storedProcedureName = null)
        {
            var methodBeginTime = Utility.LogBegin();
            List<ConfigVariable> variableAssignments = new List<ConfigVariable>();
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID_LOWERCASE, groupid),
            };

            if (storedProcedureName == null)
            {
                storedProcedureName = Constant.SPGETVARIABLESFORLIFTDESIGNER;
            }

            ds = CpqDatabaseManager.ExecuteDataSet(storedProcedureName, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                variableAssignments = (from DataRow row in ds.Tables[0].Rows
                                       select new ConfigVariable
                                       {
                                           VariableId = Convert.ToString(row[Constant.VARIABLEID]),
                                           Value = Convert.ToString(row[Constant.VALUE]),
                                       }).ToList();
            }
            Utility.LogEnd(methodBeginTime);
            return variableAssignments;
        }

        /// <summary>
        /// Method to Get the Group Status By GroupId
        /// </summary>
        /// <param Name="groupid"></param>
        /// <returns></returns>
        public string GetGroupStatusByGroupId(int groupid)
        {
            var methodBeginTime = Utility.LogBegin();
            DataSet ds = new DataSet();
            String statusKey = string.Empty;
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID_LOWERCASE, groupid),
            };
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETGROUPSTATUSFORFDA, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                statusKey = Convert.ToString(ds.Tables[0].Rows[0][Constant.STATUSKEY]);
            }
            Utility.LogEnd(methodBeginTime);
            return statusKey;
        }

        /// <summary>
        /// Method to Get the Project Status By GroupId
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <returns></returns>
        public string GetProjectStatusByGroupId(string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            DataSet ds = new DataSet();
            String statusKey = string.Empty;
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.QUOTEID_UPPERCASE, quoteId),
            };

            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETPROJECTSTATUSFORFDA, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                statusKey = Convert.ToString(ds.Tables[0].Rows[0][Constant.STATUSKEY]);
            }
            Utility.LogEnd(methodBeginTime);
            return statusKey;
        }


        /// <summary>
        /// Method to Get the Field Drawings Status By GroupId
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public string GetFieldDrawingStatusByGroupId(int groupId, string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            DataSet ds = new DataSet();
            string drawingStatusKey = string.Empty;
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.QUOTEID_UPPERCASE, quoteId),
                new SqlParameter(Constant.GROUPINGID_LOWERCASE, groupId)
            };

            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETDRAWINGSTATUSFORFDA, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                drawingStatusKey = Convert.ToString(ds.Tables[0].Rows[0][Constant.STATUSKEY]);
            }
            Utility.LogEnd(methodBeginTime);
            return drawingStatusKey;
        }

        /// <summary>
        /// Get QuoteId By GroupId
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public string GetQuoteIdByGroupId(int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            DataSet ds = new DataSet();
            string quoteId = string.Empty;
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID_LOWERCASE, groupId)
            };

            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETFDAQUOTEIDBYGROUPID, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                quoteId = Convert.ToString(ds.Tables[0].Rows[0][Constant.QUOTEID_CAMELCASE]);
            }
            Utility.LogEnd(methodBeginTime);
            return quoteId;
        }

        /// <summary>
        /// Method to Get the Field Drawing Automation By GroupId
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        public List<UnitLayOutDetails> GetFieldDrawingAutomationLayoutByGroupId(int groupid, string quoteId, DataTable configVariables)
        {
            var methodBeginTime = Utility.LogBegin();
            List<UnitLayOutDetails> listGroup = new List<UnitLayOutDetails>();
            UnitsDetails units = new UnitsDetails();
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID, groupid),
                new SqlParameter(Constant.@OPPORTUNITYID, quoteId),
                new SqlParameter(Constant.ACTION, Constant.ACTIONFIELDDRAWINGLAYOUTDETAILS),
                new SqlParameter(Constant.CONSTANTMAPPERLIST ,configVariables)
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
                                           }).ToList();

                var i = 0;
                foreach (var Unitdoordetails in units.unitLayoutDetails)
                {
                    DoorOpenings doorOpenings = new DoorOpenings();
                    doorOpenings.frontDoor = new Front();
                    doorOpenings.rearDoor = new Front();
                    doorOpenings.leftSideDoor = new Front();
                    doorOpenings.rightSideDoor = new Front();
                    if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[i][Constant.FRONTDOOR])))
                    {
                        doorOpenings.frontDoor.doorTypeHand = Convert.ToString(ds.Tables[0].Rows[i][Constant.FRONTDOOR]);
                        doorOpenings.frontDoor.isSelected = true;
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[i][Constant.REARDOOR])))
                    {
                        if (!Utility.CheckEquals(Convert.ToString(ds.Tables[0].Rows[i][Constant.REARDOOR]), Constant.NR))
                        {
                            doorOpenings.rearDoor.doorTypeHand = Convert.ToString(ds.Tables[0].Rows[i][Constant.REARDOOR]);
                            doorOpenings.rearDoor.isSelected = true;
                        }
                    }
                    Unitdoordetails.doorOpenings = doorOpenings;
                    i++;
                }
            }
            Utility.LogEnd(methodBeginTime);
            return units.unitLayoutDetails;
        }

        /// <summary>
        /// Method to Get the Get Drawing Details By ProjectId
        /// </summary>
        /// <param Name="OpportunityId"></param>
        /// <returns></returns>
        public GroupDetailsForDrawingDetails GetFieldDrawingsByProjectId(string quoteId, DataTable configVariables)
        {
            var methodBeginTime = Utility.LogBegin();
            GroupDetailsForDrawingDetails fieldDrawingDetails = new GroupDetailsForDrawingDetails();
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.@OPPORTUNITYID, quoteId),
                new SqlParameter(Constant.CONSTANTMAPPERLIST ,configVariables)
            };
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETDRAWINGS, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                fieldDrawingDetails.GroupDetailsForDrawings = new List<FieldDrawingBuildingDetails>();
                var sentToCordination = (from DataRow dRow in ds.Tables[0].Rows
                              select new
                              {
                                  sendToCoordination = Convert.ToBoolean(dRow[Constant.SENDTOCOORDINATION]),
                                  isPrimaryQuote = Convert.ToBoolean(dRow[Constant.ISPRIMARYQUOTE])
                              }).FirstOrDefault();
                fieldDrawingDetails.SendToCoordination = sentToCordination.sendToCoordination;
                fieldDrawingDetails.IsPrimaryQuote = sentToCordination.isPrimaryQuote;
                var buildingConfigList = (from DataRow dRow in ds.Tables[0].Rows
                                          select new { buildingId = Convert.ToInt32(dRow[Constant.BUILDINGIDFDA_CAMELCASE]), buildingName = Convert.ToString(dRow[Constant.BUILDINGNAME_CAMELCASE]) }).Distinct();
                foreach (var building in buildingConfigList)
                {
                    FieldDrawingBuildingDetails configurationList = new FieldDrawingBuildingDetails()
                    {
                        BuildingId = building.buildingId,
                        BuildingName = building.buildingName,
                    };
                    var groupList = (from DataRow dRow in ds.Tables[0].Rows
                                     select new { buildingId = Convert.ToInt32(dRow[Constant.BUILDINGIDFDA_CAMELCASE]), groupId = Convert.ToInt32(dRow[Constant.GROUPIDLOWERCASE]), groupName = Convert.ToString(dRow[Constant.GROUPNAMELOWERCASE]), manualInfoMessage = Convert.ToString(dRow[Constant.MANUALINFOMESSAGE]), productKey = Convert.ToString(dRow[Constant.PRODUCTKEY]) }).ToList();
                    groupList = groupList.Where(x => x.buildingId.Equals(building.buildingId)).Distinct().ToList();
                    configurationList.GroupDetails = new List<FieldDrawingGroupDetails>();
                    foreach (var group in groupList)
                    {
                        if (group.groupId > 0)
                        {
                            FieldDrawingGroupDetails groupConfiguration = new FieldDrawingGroupDetails()
                            {
                                GroupId = group.groupId,
                                GroupName = group.groupName,
                                ManualInfoMessage = group.manualInfoMessage,
                                ProductKey = group.productKey
                            };
                            var unitList = (from DataRow dRow in ds.Tables[0].Rows
                                            select new { groupId = Convert.ToInt32(dRow[Constant.GROUPIDLOWERCASE]), unitId = Convert.ToInt32(dRow[Constant.UNITID_CAMELCASE]), unitName = Convert.ToString(dRow[Constant.UNITNAMEFDA_CAMECASE]) }).ToList();
                            {
                                unitList = unitList.Where(x => x.groupId.Equals(groupConfiguration.GroupId)).Distinct().ToList();
                                groupConfiguration.Units = new List<FieldDrawingUnitData>();
                                foreach (var unit in unitList)
                                {
                                    if (unit.unitId > 0)
                                    {
                                        FieldDrawingUnitData unitConfig = new FieldDrawingUnitData()
                                        {
                                            UnitId = unit.unitId,
                                            UnitName = unit.unitName,
                                        };
                                        groupConfiguration.Units.Add(unitConfig);
                                    }
                                }
                            }
                            var groupValidityList = (from DataRow dRow in ds.Tables[0].Rows
                                                     select new
                                                     {
                                                         groupId = Convert.ToInt32(dRow[Constant.GROUPIDLOWERCASE])
                                                                 ,
                                                         grpStatusId = Convert.ToInt32(dRow[Constant.GROUPSTATUSID])
                                                                 ,
                                                         grpStatusKey = Convert.ToString(dRow[Constant.GROUPSTATUSKEY])
                                                                 ,
                                                         grpDisplayName = Convert.ToString(dRow[Constant.GROUPDISPLAYNAME])
                                                                 ,
                                                         grpStatusName = Convert.ToString(dRow[Constant.GROUPSTATUSNAME])
                                                                 ,
                                                         grpDescription = Convert.ToString(dRow[Constant.GROUPDESCRIPTION])
                                                     }).ToList();
                            {
                                groupValidityList = groupValidityList.Where(x => x.groupId.Equals(groupConfiguration.GroupId)).Distinct().ToList();
                                groupConfiguration.GroupStatus = new Status();
                                foreach (var grpValidity in groupValidityList)
                                {
                                    if (grpValidity.groupId > 0)
                                    {
                                        Status grpStatusConfig = new Status()
                                        {
                                            StatusId = grpValidity.grpStatusId,
                                            StatusKey = grpValidity.grpStatusKey,
                                            DisplayName = grpValidity.grpDisplayName,
                                            StatusName = grpValidity.grpStatusName,
                                            Description = grpValidity.grpDescription,
                                        };
                                        groupConfiguration.GroupStatus = grpStatusConfig;
                                    }
                                }
                            }
                            var drawingGenerationList = (from DataRow dRow in ds.Tables[0].Rows
                                                         select new
                                                         {
                                                             groupId = Convert.ToInt32(dRow[Constant.GROUPIDLOWERCASE])
                                                                     ,
                                                             drawingStatusId = Convert.ToInt32(dRow[Constant.DRAWINGSTATUSID])
                                                                     ,
                                                             drawingStatusKey = Convert.ToString(dRow[Constant.DRAWINGSTATUSKEY])
                                                                     ,
                                                             drawingDisplayName = Convert.ToString(dRow[Constant.DRAWINGDISPLAYNAME])
                                                                     ,
                                                             drawingStatusName = Convert.ToString(dRow[Constant.DRAWINGSTATUSNAME])
                                                                     ,
                                                             drawingDescription = Convert.ToString(dRow[Constant.DRAWINGDESCRIPTION])
                                                         }).ToList();
                            {
                                drawingGenerationList = drawingGenerationList.Where(x => x.groupId.Equals(groupConfiguration.GroupId)).Distinct().ToList();
                                groupConfiguration.DrawingStatus = new Status();
                                foreach (var drawingGeneration in drawingGenerationList)
                                {
                                    if (drawingGeneration.groupId > 0)
                                    {
                                        Status drawingGenerationConfig = new Status()
                                        {
                                            StatusId = drawingGeneration.drawingStatusId,
                                            StatusName = drawingGeneration.drawingStatusName,
                                            DisplayName = drawingGeneration.drawingDisplayName,
                                            StatusKey = drawingGeneration.drawingStatusKey,
                                            Description = drawingGeneration.drawingDescription,
                                        };
                                        groupConfiguration.DrawingStatus = drawingGenerationConfig;
                                    }
                                }
                            }
                            configurationList.GroupDetails.Add(groupConfiguration);
                        }
                    }
                    fieldDrawingDetails.GroupDetailsForDrawings.Add(configurationList);
                }
            }
            Utility.LogEnd(methodBeginTime);
            return fieldDrawingDetails;
        }

        /// <summary>
        /// Method to Get the Get the Request Queue Details
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        public RequestHistory GetRequestQueueByGroupId(int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            RequestHistory reqHistory = new RequestHistory();
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.FDAGROUPID, groupId)
            };
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.GETREQUESTQUEUEDETAILS, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                reqHistory.requestHistory = new List<RequestQueue>();

                var ConfigList = (from DataRow dRow in ds.Tables[0].Rows
                                  select new { buildingId = Convert.ToInt32(dRow[Constant.BUILDINGIDFDA_CAMELCASE]), buildingName = Convert.ToString(dRow[Constant.BUILDINGNAME_CAMELCASE]), groupId = Convert.ToInt32(dRow[Constant.GROUPIDLOWERCASE]), groupName = Convert.ToString(dRow[Constant.GROUPNAMELOWERCASE]), id = Convert.ToInt32(dRow[Constant.ID]), statusKey = Convert.ToString(dRow[Constant.STATUS_LOWERCASE]), statusName = Convert.ToString(dRow[Constant.STATUSNAME_LOWERCASE]), lastModified = Convert.ToString(dRow[Constant.LASTMODIFIED]), version = Convert.ToInt32(dRow[Constant.VERSION]), modifiedBy = Convert.ToString(dRow[Constant.MODIFIEDBY_CAMELCASE]) }).Distinct();
                foreach (var config in ConfigList)
                {
                    RequestQueue configurationList = new RequestQueue()
                    {
                        Id = config.id,
                        StatusKey = config.statusKey,
                        StatusName = config.statusName,
                        BuildingId = config.buildingId,
                        BuildingName = config.buildingName,
                        GroupId = config.groupId,
                        GroupName = config.groupName,
                        LastModified = config.lastModified,
                        Version = config.version,
                        ModifiedBy = config.modifiedBy,
                    };
                    var unitList = (from DataRow dRow in ds.Tables[0].Rows
                                    select new { groupId = Convert.ToInt32(dRow[Constant.GROUPIDLOWERCASE]), unitId = Convert.ToInt32(dRow[Constant.UNITID_CAMELCASE]), unitName = Convert.ToString(dRow[Constant.UNITNAMEFDA_CAMECASE]) }).ToList();
                    {
                        unitList = unitList.Where(x => x.groupId.Equals(configurationList.GroupId)).Distinct().ToList();
                        configurationList.Units = new List<FieldDrawingUnitData>();
                        foreach (var unit in unitList)
                        {
                            if (unit.unitId > 0)
                            {
                                FieldDrawingUnitData unitConfig = new FieldDrawingUnitData()
                                {
                                    UnitId = unit.unitId,
                                    UnitName = unit.unitName,
                                };
                                configurationList.Units.Add(unitConfig);
                            }
                        }
                    }
                    var stubGenerationSettingsMainResponseObj = new ConfigurationResponse();


                    var layoutGenerationMainResponseTemplate = JObject.Parse(File.ReadAllText(Constant.STARTFIELDDRAWINGAUTOMATIONMAINTEMPLATEPATH));
                    stubGenerationSettingsMainResponseObj = layoutGenerationMainResponseTemplate.ToObject<ConfigurationResponse>();
                    var appSections = Utility.GetTokens(Constant.TOKENSECTIONS, layoutGenerationMainResponseTemplate, false);

                    var unitLayoutSections = appSections.AsEnumerable().Where(s => Convert.ToString(s[Constant.TOKENID]).Equals(Constant.LAYOUTGENERATIONSETTINGSSECTIONKEY, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    var layoutSections = unitLayoutSections.ToObject<Sections>();



                    var fdaList = (from DataRow dRow in ds.Tables[0].Rows
                                   select new { Id = Convert.ToInt32(dRow[Constant.ID]), FDAType = Convert.ToString(dRow[Constant.FDATYPE]) }).ToList();
                    {
                        fdaList = fdaList.Where(x => x.Id.Equals(configurationList.Id)).Distinct().ToList();
                        configurationList.SelectedOutputTypes = new List<string>();
                        configurationList.SelectedDrawingTypes = new List<string>();
                        foreach (var main in layoutSections.sections)
                        {
                            foreach (var variable in main.Variables)
                            {
                                foreach (var layout in fdaList)
                                {
                                    if (variable.Id.ToString().ToUpper() == layout.FDAType.ToUpper())
                                    {
                                        if (variable.Id.Contains(Constant.OUTPUTTYPES))
                                        {
                                            configurationList.SelectedOutputTypes.Add(variable.Name);
                                        }
                                        else if (variable.Id.Contains(Constant.DRAWINGTYPES))
                                        {
                                            configurationList.SelectedDrawingTypes.Add(variable.Name);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    reqHistory.requestHistory.Add(configurationList);
                }
            }
            Utility.LogEnd(methodBeginTime);
            return reqHistory;
        }

        /// <summary>
        /// Method to Get the Save FDA Details By GroupId
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="projectId"></param>
        /// <param Name="CreatedBy"></param>
        /// <param Name="FieldDrawingAutomationDataTable"></param>
        /// <param Name="groupVariableAssignment"></param>
        /// <returns></returns>
        public ResultGroupConfiguration SaveFdaByGroupId(int groupid, string quoteId, string createdBy, List<ConfigVariable> fieldDrawingAutomationDataTable, List<ConfigVariable> groupVariableAssignment)
        {
            var methodBeginTime = Utility.LogBegin();
            ResultGroupConfiguration result = new ResultGroupConfiguration();
            List<ResultGroupConfiguration> lstResult = new List<ResultGroupConfiguration>();
            List<ConfigVariable> listGroup = new List<ConfigVariable>();
            DataTable FDAVariables = Utility.FDADrawingAutomation(fieldDrawingAutomationDataTable);
            DataTable GroupVariables = Utility.GroupVariableForDrawingAutomation(groupVariableAssignment);
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {

                new SqlParameter() { ParameterName = Constant.FDAGROUPID,Value=groupid,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
                ,new SqlParameter() { ParameterName = Constant.FDACREATEDDBY,Value=createdBy,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
                 ,new SqlParameter() { ParameterName =  Constant.FDAVARIABLES,Value=FDAVariables,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = Constant.FDAGROUPVARIABLES,Value=GroupVariables,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = Constant.OPPORTUNITYID,Value=quoteId,Direction = ParameterDirection.Input }
               ,new SqlParameter() { ParameterName = Constant.QUOTEID_UPPERCASE,Value=string.Empty,Direction = ParameterDirection.Input}
               ,new SqlParameter() { ParameterName = Constant.RESULT,Direction = ParameterDirection.ReturnValue}

            };
            var resultForSaveGroupLayout = CpqDatabaseManager.ExecuteNonquery(Constant.SPGSAVEFDABYGROUPID, param);
            if (resultForSaveGroupLayout > 0)
            {
                //TODO Replace List By Object
                result.Result = 1;
                result.FieldDrawingId = resultForSaveGroupLayout;
                result.GroupConfigurationId = groupid;
                result.Message = Constant.FDASAVEMESSAGE;
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.SOMETHINGWENTWRONG,
                    Description = Constant.SOMETHINGWENTWRONG
                });
            }
            Utility.LogEnd(methodBeginTime);
            return result;
        }

        /// <summary>
        /// Method to Get the Update the Request Status By FieldDrawingId
        /// </summary>
        /// <param Name="FieldDrawingId"></param>
        public void UpdateFDARequestStatusByFieldDrawingId(int fieldDrawingId)
        {
            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> param = new List<SqlParameter>
            {
            new SqlParameter() { ParameterName = Constant.@FIELDDRAWINGID,Value=fieldDrawingId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
            };
            var result = CpqDatabaseManager.ExecuteNonquery(Constant.USPUPDATEFDAREQUESTSTATUS, param, string.Empty);
            if (result < 0)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.SOMETHINGWENTWRONG,
                    Description = Constant.SOMETHINGWENTWRONG
                });
            }
            Utility.LogEnd(methodBeginTime);
        }

        /// <summary>
        /// Method toUpdate the FDA Method By GroupId
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="drawingMethod"></param>
        /// <returns></returns>
        public List<ResultGroupConfiguration> UpdateFdaDrawingMethodByGroupId(int groupid, int drawingMethod)
        {
            var methodBeginTime = Utility.LogBegin();
            ResultGroupConfiguration result = new ResultGroupConfiguration();
            List<ResultGroupConfiguration> lstResult = new List<ResultGroupConfiguration>();
            List<ConfigVariable> listGroup = new List<ConfigVariable>();
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = Constant.FDADRAWINGMETHOD,Value=drawingMethod,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
                ,new SqlParameter() { ParameterName = Constant.FDAGROUPID,Value=groupid,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
            };
            var resultForSaveGroupLayout = CpqDatabaseManager.ExecuteNonquery(Constant.USPUPDATEFDADRAWINGMETHODBYGROUPID, param, string.Empty);
            if (resultForSaveGroupLayout > 0)
            {
                result.Result = 1;
                result.GroupConfigurationId = groupid;
                result.Message = Constant.FDASAVEMESSAGE;
                lstResult.Add(result);
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.SOMETHINGWENTWRONG,
                    Description = Constant.SOMETHINGWENTWRONG
                });
            }
            Utility.LogEnd(methodBeginTime);
            return lstResult;
        }

        /// <summary>
        /// Method to Get the Output Types
        /// </summary>
        /// <param Name="groupid"></param>
        /// <returns></returns>
        public List<FDAOutputTypes> GetOutputTypesForXMGeneration(int groupid)
        {
            var methodBeginTime = Utility.LogBegin();
            List<FDAOutputTypes> lstOutputTypes = new List<FDAOutputTypes>();
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID_LOWERCASE, groupid),
            };
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETOUTPUTTYPESFORXMLGENERATION, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lstOutputTypes = (from DataRow row in ds.Tables[0].Rows
                                  select new FDAOutputTypes
                                  {
                                      fdaType = Convert.ToString(row[Constant.FDATYPE]),
                                  }).ToList();
            }
            Utility.LogEnd(methodBeginTime);
            return lstOutputTypes;
        }

        /// <summary>
        /// Method to Get the Request Status
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="quoteId"></param>
        /// <returns></returns>
        public FieldDrawingStatus GetLayoutRequestIdWithStatus(int groupid, string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            FieldDrawingStatus fds = new FieldDrawingStatus();
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID_LOWERCASE, groupid),
                new SqlParameter(Constant.QUOTEID_LOWERCASE, quoteId),
            };
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETFDASTATUSFORREFRESH, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                fds.statusId = Convert.ToString(ds.Tables[0].Rows[0][Constant.FDASTATUSIDLOWERCASE]);
                fds.referenceId = Convert.ToString(ds.Tables[0].Rows[0][Constant.FDAREFERENCEID]);
                fds.fieldDrawingIntegrationMasterId = Convert.ToInt32(ds.Tables[0].Rows[0][Constant.FDAFIELDDRAWINGINTEGRATIONID]);
            }
            Utility.LogEnd(methodBeginTime);
            return fds;
        }

        /// <summary>
        /// Method to Get the Field Drawing Details
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="quoteId"></param>
        /// <returns></returns>
        public GroupDetailsForDrawingDetails GetFieldDrawingsByGroupId(int groupid, string quoteId, DataTable configVariables)
        {
            var methodBeginTime = Utility.LogBegin();
            GroupDetailsForDrawingDetails fieldDrawingDetails = new GroupDetailsForDrawingDetails();
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID_LOWERCASE, groupid),
                new SqlParameter(Constant.QUOTEID_LOWERCASE, quoteId),
                new SqlParameter(Constant.CONSTANTMAPPERLIST ,configVariables)

            };
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETDRAWINGSBYGROUPIDFORREFRESH, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                fieldDrawingDetails.GroupDetailsForDrawings = new List<FieldDrawingBuildingDetails>();

                var buildingConfigList = (from DataRow dRow in ds.Tables[0].Rows
                                          select new { buildingId = Convert.ToInt32(dRow[Constant.BUILDINGIDFDA_CAMELCASE]), buildingName = Convert.ToString(dRow[Constant.BUILDINGNAME_CAMELCASE]), sendToCoordination = Convert.ToBoolean(Convert.ToInt32(dRow[Constant.SENDTOCOORDINATION])) }).Distinct();
                foreach (var building in buildingConfigList)
                {
                    fieldDrawingDetails.SendToCoordination = building.sendToCoordination;
                    FieldDrawingBuildingDetails configurationList = new FieldDrawingBuildingDetails()
                    {
                        BuildingId = building.buildingId,
                        BuildingName = building.buildingName,
                    };
                    var groupList = (from DataRow dRow in ds.Tables[0].Rows
                                     select new
                                     {
                                         buildingId = Convert.ToInt32(dRow[Constant.BUILDINGIDFDA_CAMELCASE])
                                                 ,
                                         groupId = Convert.ToInt32(dRow[Constant.GROUPIDLOWERCASE])
                                                 ,
                                         groupName = Convert.ToString(dRow[Constant.GROUPNAMELOWERCASE])
                                                 ,
                                         manualInfoMessage = Convert.ToString(dRow[Constant.MANUALINFOMESSAGE])
                                                 ,
                                         productKey = Convert.ToString(dRow[Constant.PRODUCTKEY])
                                     }).ToList();

                    groupList = groupList.Where(x => x.buildingId.Equals(building.buildingId)).Distinct().ToList();
                    configurationList.GroupDetails = new List<FieldDrawingGroupDetails>();
                    foreach (var group in groupList)
                    {
                        if (group.groupId > 0)
                        {
                            FieldDrawingGroupDetails groupConfiguration = new FieldDrawingGroupDetails()
                            {
                                GroupId = group.groupId,
                                GroupName = group.groupName,
                                ManualInfoMessage = group.manualInfoMessage,
                                ProductKey = group.productKey
                            };
                            var unitList = (from DataRow dRow in ds.Tables[0].Rows
                                            select new { groupId = Convert.ToInt32(dRow[Constant.GROUPIDLOWERCASE]), unitId = Convert.ToInt32(dRow[Constant.UNITID_CAMELCASE]), unitName = Convert.ToString(dRow[Constant.UNITNAMEFDA_CAMECASE]) }).ToList();
                            {
                                unitList = unitList.Where(x => x.groupId.Equals(groupConfiguration.GroupId)).Distinct().ToList();
                                groupConfiguration.Units = new List<FieldDrawingUnitData>();
                                foreach (var unit in unitList)
                                {
                                    if (unit.unitId > 0)
                                    {
                                        FieldDrawingUnitData unitConfig = new FieldDrawingUnitData()
                                        {
                                            UnitId = unit.unitId,
                                            UnitName = unit.unitName,
                                        };
                                        groupConfiguration.Units.Add(unitConfig);
                                    }
                                }
                            }
                            var groupValidityList = (from DataRow dRow in ds.Tables[0].Rows
                                                     select new
                                                     {
                                                         groupId = Convert.ToInt32(dRow[Constant.GROUPIDLOWERCASE])
                                                                 ,
                                                         grpStatusId = Convert.ToInt32(dRow[Constant.GROUPSTATUSID])
                                                                 ,
                                                         grpStatusKey = Convert.ToString(dRow[Constant.GROUPSTATUSKEY])
                                                                 ,
                                                         grpDisplayName = Convert.ToString(dRow[Constant.GROUPDISPLAYNAME])
                                                                 ,
                                                         grpStatusName = Convert.ToString(dRow[Constant.GROUPSTATUSNAME])
                                                                 ,
                                                         grpDescription = Convert.ToString(dRow[Constant.GROUPDESCRIPTION])
                                                     }).ToList();
                            {
                                groupValidityList = groupValidityList.Where(x => x.groupId.Equals(groupConfiguration.GroupId)).Distinct().ToList();
                                groupConfiguration.GroupStatus = new Status();
                                foreach (var grpValidity in groupValidityList)
                                {
                                    if (grpValidity.groupId > 0)
                                    {
                                        Status grpStatusConfig = new Status()
                                        {
                                            StatusId = grpValidity.grpStatusId,
                                            StatusKey = grpValidity.grpStatusKey,
                                            DisplayName = grpValidity.grpDisplayName,
                                            StatusName = grpValidity.grpStatusName,
                                            Description = grpValidity.grpDescription,
                                        };
                                        groupConfiguration.GroupStatus = grpStatusConfig;
                                    }
                                }
                            }
                            var drawingGenerationList = (from DataRow dRow in ds.Tables[0].Rows
                                                         select new
                                                         {
                                                             groupId = Convert.ToInt32(dRow[Constant.GROUPIDLOWERCASE])
                                                                     ,
                                                             drawingStatusId = Convert.ToInt32(dRow[Constant.DRAWINGSTATUSID])
                                                                     ,
                                                             drawingStatusKey = Convert.ToString(dRow[Constant.DRAWINGSTATUSKEY])
                                                                     ,
                                                             drawingDisplayName = Convert.ToString(dRow[Constant.DRAWINGDISPLAYNAME])
                                                                     ,
                                                             drawingStatusName = Convert.ToString(dRow[Constant.DRAWINGSTATUSNAME])
                                                                     ,
                                                             drawingDescription = Convert.ToString(dRow[Constant.DRAWINGDESCRIPTION])
                                                         }).ToList();
                            {
                                drawingGenerationList = drawingGenerationList.Where(x => x.groupId.Equals(groupConfiguration.GroupId)).Distinct().ToList();
                                groupConfiguration.DrawingStatus = new Status();
                                foreach (var drawingGeneration in drawingGenerationList)
                                {
                                    if (drawingGeneration.groupId > 0)
                                    {
                                        Status drawingGenerationConfig = new Status()
                                        {
                                            StatusId = drawingGeneration.drawingStatusId,
                                            StatusName = drawingGeneration.drawingStatusName,
                                            DisplayName = drawingGeneration.drawingDisplayName,
                                            StatusKey = drawingGeneration.drawingStatusKey,
                                            Description = drawingGeneration.drawingDescription,
                                        };
                                        groupConfiguration.DrawingStatus = drawingGenerationConfig;
                                    }
                                }
                            }
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
                            {
                                var designAutomationList = (from DataRow dRow in ds.Tables[1].Rows
                                                             select new
                                                             {
                                                                 groupId = Convert.ToInt32(dRow[Constant.GROUPIDLOWERCASE])
                                                                         ,
                                                                 designAutomationStatusId = Convert.ToInt32(dRow[Constants.DESIGNAUTOMATIONSTATUSID])
                                                                         ,
                                                                 designAutomationStatusKey = Convert.ToString(dRow[Constants.DESIGNAUTOMATIONSTATUSKEY])
                                                                         ,
                                                                 designAutomationDisplayName = Convert.ToString(dRow[Constants.DESIGNAUTOMATIONDISPLAYNAME])
                                                                         ,
                                                                 designAutomationStatusName = Convert.ToString(dRow[Constants.DESIGNAUTOMATIONSTATUSNAME])
                                                                         ,
                                                                 designAutomationDescription = Convert.ToString(dRow[Constants.DESIGNAUTOMATIONDESCRIPTION])
                                                             }).ToList();
                                {
                                    designAutomationList = designAutomationList.Where(x => x.groupId.Equals(groupConfiguration.GroupId)).Distinct().ToList();
                                    groupConfiguration.DesignAutomationStatus = new Status();
                                    foreach (var designAutomation in designAutomationList)
                                    {
                                        if (designAutomation.groupId > 0)
                                        {
                                            Status designAutomationConfig = new Status()
                                            {
                                                StatusId = designAutomation.designAutomationStatusId,
                                                StatusName = designAutomation.designAutomationStatusName,
                                                DisplayName = designAutomation.designAutomationDisplayName,
                                                StatusKey = designAutomation.designAutomationStatusKey,
                                                Description = designAutomation.designAutomationDescription,
                                            };
                                            groupConfiguration.DesignAutomationStatus = designAutomationConfig;
                                        }
                                    }
                                }
                            }
                            configurationList.GroupDetails.Add(groupConfiguration);
                        }
                    }
                    fieldDrawingDetails.GroupDetailsForDrawings.Add(configurationList);
                }
            }
            
                Utility.LogEnd(methodBeginTime);
            return fieldDrawingDetails;
        }

        /// <summary>
        /// Method to Check the Request By FieldDrawingId
        /// </summary>
        /// <param Name="fieldDrawingIntegrationMasterId"></param>
        /// <returns></returns>
        public List<Reference> CheckRequestIdByFDAIntegrationId(int fieldDrawingIntegrationMasterId)
        {
            var methodBeginTime = Utility.LogBegin();
            List<Reference> lstRequestIds = new List<Reference>();
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.FIELDDRAWINGINTEGRATIONMASTERID, fieldDrawingIntegrationMasterId),
            };
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPCHECKREQUESTIDBYFDAINTEGRATIONID, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lstRequestIds = (from DataRow row in ds.Tables[0].Rows
                                 select new Reference
                                 {
                                     IntegratedProcessId = Convert.ToInt32(row[Constant.FDAINTEGRATEDPROCESSID]),
                                     IntegratedSystemRef = Convert.ToString(row[Constant.FDAINTEGRATEDSYSTEMREF]),
                                 }).ToList();
            }
            Utility.LogEnd(methodBeginTime);
            return lstRequestIds;
        }

        /// <summary>
        /// Method to check the Hangfire Recurring Jon
        /// </summary>
        /// <param Name="IntegratedProcessId"></param>
        /// <returns></returns>
        public List<RecurringJobData> CheckHangFireRecurringJob(int IntegratedProcessId)
        {
            var methodBeginTime = Utility.LogBegin();
            List<RecurringJobData> lstRecurringJob = new List<RecurringJobData>();
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.INTEGRATEDPROCESSID, IntegratedProcessId),
            };
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPCHECKHANGFIRERECURRINGJOB, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lstRecurringJob = (from DataRow row in ds.Tables[0].Rows
                                   select new RecurringJobData
                                   {
                                       Id = Convert.ToInt32(row[Constant.FDAID]),
                                       StatusId = Convert.ToInt32(row[Constant.FDASTATUSID]),
                                   }).ToList();
            }
            Utility.LogEnd(methodBeginTime);
            return lstRecurringJob;
        }

        /// <summary>
        /// Method to Get the Unit Variables By groupId
        /// </summary>
        /// <param Name="groupid"></param>
        /// <returns></returns>
        public List<UnitVariables> GetUnitsVariablesWithUnitDetailsByGroupId(int groupid, DataTable configVariables)
        {
            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> parameterList = new List<SqlParameter>();
            List<UnitVariables> lstUnitVariables = new List<UnitVariables>();
            DataSet dataSet = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID_LOWERCASE, groupid),
                new SqlParameter(Constant.CONSTANTMAPPERLIST ,configVariables)

            };
            try
            {
                dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPWRAPPERAPIXMLGENERATIONBYGROUPID, param);
                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[1].Rows.Count > 0)
                {
                    lstUnitVariables = (from DataRow row in dataSet.Tables[1].Rows
                                        select new UnitVariables
                                        {
                                            groupConfigurationId = Convert.ToInt32(row[Constant.FDAGROUPCONFIGURATIONID]),
                                            name = Convert.ToString(row[Constant.FDAUNITNAME]),
                                            unitId = Convert.ToInt32(row[Constant.FDAUNITID]),
                                            MappedLocation = Convert.ToString(row[Constant.MAPPEDLOCATION]),
                                        }).ToList();
                }
            }
            catch (Exception ex)
            {
            }
            Utility.LogEnd(methodBeginTime);
            return lstUnitVariables;
        }


        /// <summary>
        /// Method to Get the Building and Group Variables By groupId
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        
        /// <summary>
        /// Method to Get the XML Variables By groupId
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public List<UnitVariables> GetXMLVariablesWithUnitByGroupId(int groupid, DataTable configVariables)
        {
            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> parameterList = new List<SqlParameter>();
            List<UnitVariables> lstUnitVariables = new List<UnitVariables>();
            DataSet dataSet = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID_LOWERCASE, groupid),
                new SqlParameter(Constant.CONSTANTMAPPERLIST ,configVariables)

            };
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPWRAPPERAPIXMLGENERATIONBYGROUPID, param);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                lstUnitVariables = (from DataRow row in dataSet.Tables[0].Rows
                                    select new UnitVariables
                                    {
                                        groupConfigurationId = Convert.ToInt32(row[Constant.FDAGROUPCONFIGURATIONID]),
                                        name = Convert.ToString(row[Constant.FDAUNITNAME]),
                                        unitId = Convert.ToInt32(row[Constant.FDAUNITID]),
                                        MappedLocation = Convert.ToString(row[Constant.MAPPEDLOCATION]),
                                        VariableId = Convert.ToString(row[Constant.VARIABLEID]),
                                        Value = Convert.ToString(row[Constant.VALUE]),
                                    }).ToList();
            }
            Utility.LogEnd(methodBeginTime);
            return lstUnitVariables;
        }

        /// <summary>
        /// Method to Get the Version and OpportunityId By QuoteId
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public ProjectDet GetOpportunityAndVersionByQuoteId(string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            ProjectDet projectDet = new ProjectDet();
            IList<SqlParameter> parameterList = new List<SqlParameter>();
            DataSet dataSet = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.@QUOTEIDSPPARAMETER, quoteId),
            };
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETOPPORTUNITYANDVERSIONIDBYQUOTEID, param);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                projectDet.VersionId = Convert.ToInt32(dataSet.Tables[0].Rows[0][Constant.VERSIONID]);
                projectDet.OpportunityId = Convert.ToString(dataSet.Tables[0].Rows[0][Constant.OPPORTUNITYIDVARIABLE]);
            }
            Utility.LogEnd(methodBeginTime);
            return projectDet;
        }


        /// <summary>
        /// Generating Wrapper Token
        /// </summary>
        /// <param Name="username"></param>
        /// <param Name="password"></param>
        /// <returns></returns>
        public async Task<string> GenerateWrapperToken()
        {
            var methodBeginTime = Utility.LogBegin();

            var accessToken = string.Empty;
            var wrapperSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constant.DRAWINGSAPISETTINGS);
            string proxy = Utility.GetPropertyValue(wrapperSettings, Constant.PROXYURI);

            var encodedBody = new Dictionary<string, string>
            {
                {Constant.APIUSERNAME.ToLower(), Utility.GetPropertyValue(wrapperSettings, Constant.APIUSERNAME) },
                {Constant.APIPASSWORD.ToLower(), Utility.GetPropertyValue(wrapperSettings, Constant.APIPASSWORD) },
                {Constant.GRANTTYPE.ToLower(), Utility.GetPropertyValue(wrapperSettings, Constant.GRANTTYPESETTING) }
            };

            var requestObject = new HttpClientRequestModel()
            {
                BaseUrl = Utility.GetPropertyValue(wrapperSettings, Constant.BASEURL),
                EndPoint = Utility.GetPropertyValue(wrapperSettings, Constant.TOKENAPI),
                Method = HTTPMETHODTYPE.POST,
                BodyToEncode = encodedBody,
                ContentType = Constants.CONTENTTYPEFORMURI,
                Proxy = proxy
            };

            var apiResponse = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(true);

            if (!apiResponse.IsSuccessStatusCode)
            {
                Utility.LogEnd(methodBeginTime);
                return string.Empty;
            }

            JObject respObj = JObject.Parse(apiResponse.Content.ReadAsStringAsync().Result);
            if (respObj[Constant.ACCESSTOKENOBJ] != null)
            {
                accessToken = Convert.ToString(respObj[Constant.ACCESSTOKENOBJ]);
            }
            Utility.LogEnd(methodBeginTime);
            return accessToken;
        }



        /// <summary>
        /// RequestLayouts Wrapper API
        /// </summary>
        /// <param Name="json"></param>
        /// <param Name="Token"></param>
        /// <returns></returns>
        public async Task<string> RequestLayouts(JObject json, string token)
        {
            var methodBeginTime = Utility.LogBegin();
            var FormsLayoutReferenceId = string.Empty;
            if (!string.IsNullOrEmpty(token))
            {
                var wrapperSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constant.DRAWINGSAPISETTINGS);
                var requestObject = new HttpClientRequestModel()
                {
                    BaseUrl = Utility.GetPropertyValue(wrapperSettings, Constant.BASEURL),
                    EndPoint = Utility.GetPropertyValue(wrapperSettings, Constant.REQUESTLAYOUTAPI),
                    Method = HTTPMETHODTYPE.POST,
                    RequestHeaders = new Dictionary<string, string>
                        {
                            { Constant.AUTHORIZATION, Constant.BEARER + token }
                        },
                    RequestBody = json
                };
                var response = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var obj = response.Content.ReadAsStringAsync().Result;
                JObject wrapperToken = JsonConvert.DeserializeObject<dynamic>(obj);
                FormsLayoutReferenceId = wrapperToken.Value<string>(Constant.FORMSLAYOUTREFERENCEID);
            }
            Utility.LogEnd(methodBeginTime);
            return FormsLayoutReferenceId;
        }


        /// <summary>
        /// Layout Status Wrapper API
        /// </summary>
        /// <param Name="FormsLayoutReferenceId"></param>
        /// <returns></returns>
        public async Task GetLayoutStatus(int groupId, string quoteId, string userName, string referenceId, int fieldDrawingIntegrationMasterId, string wrapperToken)
        {
            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(wrapperToken))
            {
                var wrapperSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constant.DRAWINGSAPISETTINGS);

                var requestObject = new HttpClientRequestModel()
                {
                    BaseUrl = Utility.GetPropertyValue(wrapperSettings, Constant.BASEURL),
                    EndPoint = Utility.GetPropertyValue(wrapperSettings, Constant.LAYOUTSTATUSAPI),
                    Method = HTTPMETHODTYPE.POST,
                    ContentType = Constants.CONTENTTYPEFORMURI,
                    RequestHeaders = new Dictionary<string, string>
                    {
                        { Constant.ACCEPT, "*/*" },
                        { Constant.AUTHORIZATION, Constant.BEARER + wrapperToken }
                    },
                    BodyToEncode = new Dictionary<string, string>
                    {
                        { Constant.FORMSLAYOUTREFERENCEID, referenceId }
                    }
                };

                var layoutStatusResponse = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(false);
                var layoutStatusResult = layoutStatusResponse.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(layoutStatusResult))
                {
                    if (layoutStatusResult.Contains(Constant.ERRORMESSAGE) || layoutStatusResult.Contains(Constant.REPORTERRORMESSAGE))
                    {
                        sqlParameters = Utility.SqlParameterForLayoutStatus(groupId, fieldDrawingIntegrationMasterId, layoutStatusResult, userName);
                    }
                    else
                    {
                        JObject data = JObject.Parse(layoutStatusResult);
                        string layoutStatus = Convert.ToString(data[Constant.QUEUEPOSITION]);
                        string statusId = string.Empty;
                        Utility.LogTrace(layoutStatus);
                        switch (layoutStatus)
                        {
                            case Constant.FDACURRENT:
                                statusId = Constant.DWGPENDING;
                                break;
                            case Constant.FDACOMPLETED:
                                statusId = Constant.DWGCOMPLETED;
                                break;
                        }
                        sqlParameters = new List<SqlParameter>
                        {
                            new SqlParameter() { ParameterName = Constant.FDAGROUPID,Value=groupId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
                           ,new SqlParameter() { ParameterName = Constant.INTEGRATEDPROCESSID,Value=fieldDrawingIntegrationMasterId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
                           , new SqlParameter() { ParameterName = Constant.FDAPROCESSJSON,Value=Convert.ToString(layoutStatusResult),Direction = ParameterDirection.Input, SqlDbType = SqlDbType.NVarChar}
                            ,new SqlParameter() { ParameterName = Constant.FDACREATEDDBY,Value=userName,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.NVarChar}
                            ,new SqlParameter() { ParameterName = Constant.FDAPARAMSTATUSID,Value=statusId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.NVarChar}
                        };
                    }
                }
                var queryResult = CpqDatabaseManager.ExecuteNonquery(Constant.USPSAVEWRAPPERFIELDLAYOUT, sqlParameters, string.Empty);
                if (queryResult < 0)
                {
                    throw new CustomException(new ResponseMessage
                    {
                        StatusCode = Constant.BADREQUEST,
                        Message = Constant.SOMETHINGWENTWRONG,
                        Description = Constant.SOMETHINGWENTWRONG
                    });
                }
            }
            else
            {
                sqlParameters = Utility.SqlParameterForLayoutStatus(groupId, fieldDrawingIntegrationMasterId, Constant.PROXYISSUE, userName);
                var result = CpqDatabaseManager.ExecuteNonquery(Constant.USPSAVEWRAPPERFIELDLAYOUT, sqlParameters, string.Empty);
            }
            Utility.LogEnd(methodBeginTime);
        }

        /// <summary>
        /// saver referenceId in FieldDrawingIntegrationMaster
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="projectId"></param>
        /// <param Name="userName"></param>
        /// <param Name="statusId"></param>
        /// <param Name="intergratedSystemId"></param>
        /// <param Name="referenceId"></param>
        /// <returns></returns>
        public int SaveReferenceId(int groupId, string quoteId, string userName, string statusId, int intergratedSystemId, string referenceId)
        {

            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = Constant.FDAGROUPID,Value=groupId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = Constant.@QUOTEIDSPPARAMETER,Value=quoteId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = Constant.FDACREATEDDBY,Value=userName,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = Constant.STATUSID,Value=statusId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = Constant.INTEGRATEDSYSTEMID,Value=intergratedSystemId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = Constant.INTEGRATEDSYSTEMREF,Value=referenceId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = Constant.RESULT,Direction = ParameterDirection.ReturnValue}

             };
            var fieldDrawingIntegrationMasterId = CpqDatabaseManager.ExecuteNonquery(Constant.USPSAVEWRAPPERFIELDDRAWINGAUTOMATION, param);
            if (fieldDrawingIntegrationMasterId < 0)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.SOMETHINGWENTWRONG,
                    Description = Constant.SOMETHINGWENTWRONG
                });

            }

            Utility.LogEnd(methodBeginTime);
            return fieldDrawingIntegrationMasterId;
        }

        /// <summary>
        /// Method to Update the Lock Property
        /// </summary>
        /// <param Name="projectId"></param>
        /// <param Name="groupId"></param>
        /// <param Name="islock"></param>
        /// <returns></returns>
        public List<ResultGroupConfiguration> UpdateLockPropertyForGroups(string quoteId, int groupId, string islock)
        {
            var methodBeginTime = Utility.LogBegin();
            ResultGroupConfiguration result = new ResultGroupConfiguration();
            List<ResultGroupConfiguration> lstResult = new List<ResultGroupConfiguration>();
            int islocked = 0;
            if ((Utility.CheckEquals(islock, Constant.TRUE_FULL_LOWERCASE)))
            {
                islocked = 1;
            }
            else if ((Utility.CheckEquals(islock, Constant.FALSE_FULL_LOWERCASE)))
            {
                islocked = 0;
            }

            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = Constant.ISLOCK,Value=islocked,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = Constant.FDAGROUPID,Value=groupId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.Int}
               ,new SqlParameter() { ParameterName = Constant.OPPORTUNITYID,Value=quoteId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.VarChar}
            };
            var resultConfiguration = CpqDatabaseManager.ExecuteNonquery(Constant.USPUPDATELOCKGROUPPROPERTYFORFDA, param, string.Empty);
            result.Result = resultConfiguration;
            result.GroupConfigurationId = groupId;
            result.Message = Constant.FDAUPDATELOCKSUCCESS;
            if (resultConfiguration > 0)
            {
                if (Utility.CheckEquals(islock, Constant.TRUE_FULL_LOWERCASE))
                {
                    result.Message = Constant.FDAUPDATELOCKSUCCESS;
                }
                else if (Utility.CheckEquals(islock, Constant.FALSE_FULL_LOWERCASE))
                {
                    result.Message = Constant.FDAUPDATEUNLOCKSUCCESS;
                }
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.SOMETHINGWENTWRONG,
                    Description = Constant.SOMETHINGWENTWRONG
                });
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBeginTime);
            return lstResult;
        }

        /// <summary>
        /// Method to Save the Coordination Details
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <param Name="CreatedBy"></param>
        /// <param Name="coordinationData"></param>
        /// <returns></returns>
        public List<ResultGroupConfiguration> SaveSendToCoordination(string quoteId, string createdBy, List<SendToCoordinationData> coordinationData)
        {
            var methodBeginTime = Utility.LogBegin();
            ResultGroupConfiguration result = new ResultGroupConfiguration();
            List<ResultGroupConfiguration> lstResult = new List<ResultGroupConfiguration>();
            DataTable coordinationTable = Utility.CoordinationData(coordinationData);
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = Constant.@QUOTEID,Value=quoteId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.NVarChar}
                ,new SqlParameter() { ParameterName = Constant.OPPORTUNITYID,Value="",Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.NVarChar }
                ,new SqlParameter() { ParameterName = Constant.FDACREATEDDBY,Value=createdBy,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.NVarChar}
                ,new SqlParameter() { ParameterName =  Constant.COORDINATIONDATA,Value=coordinationTable,Direction = ParameterDirection.Input }
            };
            var resultForSaveGroupLayout = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVESENDTOCOORDINATION, param, string.Empty);
            if (resultForSaveGroupLayout > 0)
            {
                result.Result = 1;
                result.Message = Constant.SENDTOCOORDINATIONSAVEMESSAGE;
                lstResult.Add(result);
            }
            else
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.SOMETHINGWENTWRONG,
                    Description = Constant.SOMETHINGWENTWRONG
                });
            }
            Utility.LogEnd(methodBeginTime);
            return lstResult;
        }

        /// <summary>
        /// Method to Get the Cordination Details
        /// </summary>
        /// <param Name="quoteId"></param>
        /// <returns></returns>
        public GroupDetailsForSendToCoordination GetSendToCoordinationByProjectId(string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> sp = new List<SqlParameter>();
            GroupDetailsForSendToCoordination fieldDrawingDetails = new GroupDetailsForSendToCoordination();
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.QUOTEID, quoteId)
            };
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETSENDTOCOORDINATION, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                fieldDrawingDetails.groupDetailsForSendToCoordination = new List<FieldDrawingBuildingDetail>();

                var buildingConfigList = (from DataRow dRow in ds.Tables[0].Rows
                                          select new { buildingId = Convert.ToInt32(dRow[Constant.BUILDINGIDFDA_CAMELCASE]), buildingName = Convert.ToString(dRow[Constant.BUILDINGNAME_CAMELCASE]), enableSendToCoordination = Convert.ToBoolean(dRow[Constant.ENABLESENDTOCOORDINATION]) }).Distinct();
                foreach (var building in buildingConfigList)
                {
                    fieldDrawingDetails.enableSendToCoordination = building.enableSendToCoordination;
                    FieldDrawingBuildingDetail configurationList = new FieldDrawingBuildingDetail()
                    {
                        buildingId = building.buildingId,
                        buildingName = building.buildingName,
                    };
                    var groupList = (from DataRow dRow in ds.Tables[0].Rows
                                     select new { buildingId = Convert.ToInt32(dRow[Constant.BUILDINGIDFDA_CAMELCASE]), groupId = Convert.ToInt32(dRow[Constant.GROUPIDLOWERCASE]), groupName = Convert.ToString(dRow[Constant.GROUPNAMELOWERCASE]), isSaved = Convert.ToBoolean(dRow[Constant.ISSAVED]), questions = Convert.ToString(dRow[Constant.QUESTIONS]) }).ToList();
                    groupList = groupList.Where(x => x.buildingId.Equals(building.buildingId)).Distinct().ToList();
                    configurationList.groupDetails = new List<FieldDrawingGroupDetail>();
                    foreach (var group in groupList)
                    {
                        if (group.groupId > 0)
                        {
                            FieldDrawingGroupDetail groupConfiguration = new FieldDrawingGroupDetail()
                            {
                                groupId = group.groupId,
                                groupName = group.groupName,
                                isGroupSaved = group.isSaved,
                            };
                            var coordinationStubData = JArray.Parse(File.ReadAllText(Constant.COORDINATIONQUESTIONSSTUBPATH));
                            var coordinationData1 = Utility.DeserializeObjectValue<List<CoordinationQuestions>>(Utility.SerializeObjectValue(coordinationStubData));
                            if (!group.questions.Equals(Constant.EMPTYSTRING))
                            {
                                coordinationData1 = JsonConvert.DeserializeObject<List<CoordinationQuestions>>(group.questions);
                            }
                            groupConfiguration.questions = coordinationData1;
                            configurationList.groupDetails.Add(groupConfiguration);
                        }
                    }
                    fieldDrawingDetails.groupDetailsForSendToCoordination.Add(configurationList);
                }
            }
            Utility.LogEnd(methodBeginTime);
            return fieldDrawingDetails;
        }

        /// <summary>
        /// GetPermissionForFDA
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public List<Permissions> GetPermissionForFDA(string quoteId, string roleName, string entity)
        {
            List<Permissions> lstPermission = new List<Permissions>();
            List<SqlParameter> sp = new List<SqlParameter>();
            SqlParameter param = new SqlParameter(Constant.@_ID, quoteId);
            sp.Add(param);
            param = new SqlParameter(Constant.@ROLENAME, roleName);
            sp.Add(param);
            param = new SqlParameter(Constant.@ENTITY, entity);
            sp.Add(param);
            DataSet ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETPERMISSIONBYROLENAME, sp);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dRow in ds.Tables[0].Rows)
                {
                    var permission = new Permissions()
                    {
                        Entity = Convert.ToString(dRow[Constant.ENTITYNAME]),
                        PermissionKey = Convert.ToString(dRow[Constant.PERMISSIONKEY]),
                        GroupStatus = Convert.ToString(dRow[Constant.GROUPSTATUS_CAMEL])
                    };
                    lstPermission.Add(permission);
                }

            }
            return lstPermission;
        }
        /// <summary>
        /// method to get send to coordination status
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public SendToCoordinationStatus GetSendToCoordinationStatus(string quoteId)
        {
            var methodBeginTime = Utility.LogBegin();     
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.@QUOTEIDSPPARAMETER, quoteId)
            };
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETSENDTOCORDINATIONSTATUS, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var result = (from DataRow dRow in ds.Tables[0].Rows
                              select new { statusId = Convert.ToInt32(dRow[Constant.FDASTATUSID]),
                                           statusKey= Convert.ToString(dRow[Constant.STATUSKEY]),
                                           displayName = Convert.ToString(dRow[Constant.DISPLAYNAMESTATUS]),
                                           description= Convert.ToString(dRow[Constant.DESCRIPTION])
                              }).FirstOrDefault();
                Utility.LogEnd(methodBeginTime);
                return new SendToCoordinationStatus()
                {
                    CoordinationStatus = new Status()
                    {
                        StatusId = result.statusId,
                        StatusKey=result.statusKey,
                        DisplayName=result.displayName,
                        Description=result.description
                    }
                };

            }
            return new SendToCoordinationStatus();
        }

        public int GetBuildingId(int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            DataSet ds = new DataSet();
            int buildingId = 0;
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter("groupConfigurationId", groupId),
            };

            ds = CpqDatabaseManager.ExecuteDataSet("usp_GetBuildingIdByGroupId", param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                buildingId = Convert.ToInt32(ds.Tables[0].Rows[0]["BuildingId"]);
            }
            Utility.LogEnd(methodBeginTime);
            return buildingId;
        }

        public string GetLDResponseJson(int groupId)
        {
            var methodBeginTime = Utility.LogBegin();
            DataSet ds = new DataSet();
            string responseString ="";
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter("groupId", groupId),
            };

            ds = CpqDatabaseManager.ExecuteDataSet("usp_GetLDResponseJson", param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                responseString = Convert.ToString(ds.Tables[0].Rows[0]["response"]);
            }
            Utility.LogEnd(methodBeginTime);
            return responseString;
        }
        public int SaveVariableArguments(int groupId, List<UnitVariables> listVariablesArgument, string userName)
        {
            var methodBeginTime = Utility.LogBegin();
            var listArguments=listVariablesArgument.Select(
                unitvariables => new ConfigVariable
                {
                    VariableId = unitvariables.VariableId,
                    Value = unitvariables.Value
                }).ToList();
            DataTable varibaleTable= Utility.GenerateDataTableForUnitConfiguration(listArguments);
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter{ParameterName ="@GroupId",Value= groupId,Direction = ParameterDirection.Input },
                new SqlParameter{ ParameterName =  "@Variables",Value=varibaleTable,Direction = ParameterDirection.Input },
                new SqlParameter{ParameterName ="@CreatedBy",Value= userName,Direction = ParameterDirection.Input },
                new SqlParameter{ParameterName ="@Result",Value= 0,Direction = ParameterDirection.Output }

            };

            var result = CpqDatabaseManager.ExecuteNonquery("usp_SaveDefaultArguments", param);
            Utility.LogEnd(methodBeginTime);
            return result;
        }

        public int UpdateStatusForFDA(string guid, string statusId)
        {

            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = Constant.FDAGUID,Value=guid,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = Constant.STATUSID,Value=statusId,Direction = ParameterDirection.Input,SqlDbType = SqlDbType.VarChar}
               ,new SqlParameter() { ParameterName = Constant.RESULT,Direction = ParameterDirection.ReturnValue}

             };
            var fieldDrawingIntegrationMasterId = CpqDatabaseManager.ExecuteNonquery(Constant.USPUPDATESTATUSFORFDA, param);
            if (fieldDrawingIntegrationMasterId < 0)
            {
                throw new CustomException(new ResponseMessage
                {
                    StatusCode = Constant.BADREQUEST,
                    Message = Constant.SOMETHINGWENTWRONG,
                    Description = Constant.SOMETHINGWENTWRONG
                });

            }
            Utility.LogEnd(methodBeginTime);
            return fieldDrawingIntegrationMasterId;
        }

        /// <summary>
        /// To get all of Group Information by group id
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public List<GroupInfo> GetGroupInformation(int groupid)
        {
            var methodBeginTime = Utility.LogBegin();
            List<GroupInfo> groupInfo = new List<GroupInfo>();
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID, groupid)
            };
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGGETGROUPINFORMATION, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                groupInfo = (from DataRow row in ds.Tables[0].Rows
                             select new GroupInfo
                             {
                                 QuoteId = Convert.ToString(row[Constants.QUOTEID]),
                                 BuildingId = Convert.ToInt32(row[Constants.BUILDINGID]),
                                 GroupId = Convert.ToInt32(row[Constants.GROUPID]),
                                 Unitid = Convert.ToInt32(row[Constants.UNITID]),
                                 SetId = Convert.ToInt32(row[Constants.SETIDCAMELCASE]),
                                 ProductName = Convert.ToString(row[Constants.PRODUCTNAME])
                             }).ToList();
            }
            Utility.LogEnd(methodBeginTime);
            return groupInfo;
        }
    }
}