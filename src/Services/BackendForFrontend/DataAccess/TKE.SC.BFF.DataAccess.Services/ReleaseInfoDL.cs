using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common;
using TKE.SC.Common.Database;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class ReleaseInfoDL : IReleaseInfoDL
    {
        private readonly IConfigure _configure;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param Name="logger"></param>
        public ReleaseInfoDL(ILogger<ReleaseInfoDL> logger, IConfigure configure)
        {
            Utility.SetLogger(logger);
            _configure = configure;
        }
        /// <summary>
        /// GetPermissionForReleaseInfo
        /// </summary>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public List<Permissions> GetPermissionForReleaseInfo(string quoteId,string roleName,string Entity)
        {
            var methodBegin = Utility.LogBegin();
            List<Permissions> lstPermission = new List<Permissions>();
            List<SqlParameter> sp = new List<SqlParameter>();
            SqlParameter param = new SqlParameter(Constant.@_ID, quoteId);
            sp.Add(param);
            param = new SqlParameter(Constant.@ROLENAME, roleName);
            sp.Add(param);
            param = new SqlParameter(Constant.@ENTITY, Entity);
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
            Utility.LogEnd(methodBegin);
            return lstPermission;
        }

        /// <summary>
        /// Method to get building and group details for release info tab
        /// </summary>
        /// <param Name="projectId"></param>
        /// <returns></returns>
        public GroupDetailsForRelease GetProjectReleaseInfo(string projectId)
        {
            var methodBeginTime = Utility.LogBegin();
            GroupDetailsForRelease releaseInfoDetails = new GroupDetailsForRelease();
            DataSet dsReleaseInfo = new DataSet();
            List<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.@OPPORTUNITYID, projectId)
            };
            dsReleaseInfo = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETRELEASEINFO, param);
            if (dsReleaseInfo != null && dsReleaseInfo.Tables.Count > 0 && dsReleaseInfo.Tables[0].Rows.Count > 0)
            {
                releaseInfoDetails.GroupDetailsForReleaseInfo = new List<ReleaseInfoBuildingDetails>();
                var buildingConfigList = (from DataRow dRow in dsReleaseInfo.Tables[0].Rows
                                          select new { buildingId = Convert.ToInt32(dRow[Constant.BUILDINGIDFDA_CAMELCASE]), buildingName = Convert.ToString(dRow[Constant.BUILDINGNAME_CAMELCASE]) }).Distinct().ToList();
                foreach (var building in buildingConfigList)
                {
                    ReleaseInfoBuildingDetails configurationList = new ReleaseInfoBuildingDetails
                    {
                        BuildingId = building.buildingId,
                        BuildingName = building.buildingName
                    };
                    var groupList = (from DataRow dRow in dsReleaseInfo.Tables[0].Rows
                                     where(Convert.ToInt32(dRow[Constant.BUILDINGIDFDA_CAMELCASE]).Equals(building.buildingId))
                                     select new {
                                         buildingId = Convert.ToInt32(dRow[Constant.BUILDINGIDFDA_CAMELCASE]),
                                         groupId = Convert.ToInt32(dRow[Constant.GROUPIDLOWERCASE]),
                                         groupName = Convert.ToString(dRow[Constant.GROUPNAMELOWERCASE]),
                                         unitCount = Convert.ToInt32(dRow[Constant.UNITCOUNTLOWERCASE]),
                                         releaseToManufacturing = Convert.ToBoolean(Convert.ToInt32(dRow[Constant.RELEASETOMANUFACTURING])),
                                         productCategory = Convert.ToString(dRow[Constant.PRODUCTCATEGORY_CAMELCASE]) }).Distinct().ToList();

                    configurationList.GroupDetails = new List<ReleaseInfoGroupDetails>();
                    foreach (var group in groupList)
                    {
                        if (group.groupId > 0)
                        {
                            ReleaseInfoGroupDetails groupConfiguration = new ReleaseInfoGroupDetails()
                            {
                                GroupId = group.groupId,
                                GroupName = group.groupName,
                                UnitLength = group.unitCount,
                                ProductCategory = group.productCategory,
                                ReleaseToManufacturing = group.releaseToManufacturing
                            };
                            var groupValidityList = (from DataRow dRow in dsReleaseInfo.Tables[0].Rows
                                                     select new {   groupId = Convert.ToInt32(dRow[Constant.GROUPIDLOWERCASE]),
                                                         grpStatusId = Convert.ToInt32(dRow["grpStatusId"]),
                                                         grpStatusKey = Convert.ToString(dRow["grpStatusKey"]),
                                                         grpStatusName = Convert.ToString(dRow["grpStatusName"]),
                                                         grpDescription = Convert.ToString(dRow["grpDescription"]),
                                                         grpDisplayName = Convert.ToString(dRow["grpDisplayName"])}).ToList();
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
                                            StatusName = grpValidity.grpStatusName,
                                            DisplayName = grpValidity.grpDisplayName,
                                            Description = grpValidity.grpDescription,
                                        };
                                        groupConfiguration.GroupStatus = grpStatusConfig;
                                    }
                                }
                            }
                            configurationList.GroupDetails.Add(groupConfiguration);
                        }
                    }
                    releaseInfoDetails.GroupDetailsForReleaseInfo.Add(configurationList);
                }
            }
            Utility.LogEnd(methodBeginTime);
            return releaseInfoDetails;
        }

        /// <summary>
        /// GetGroupConfigurationDataSet
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        private DataSet GetGroupConfigurationDataSet(int groupId)
        {
            List<SqlParameter> sp = new List<SqlParameter>();
            var param = new SqlParameter(Constant.@GRPCONFIGURATIONID, groupId);
            sp.Add(param);
            return CpqDatabaseManager.ExecuteDataSet(Constant.SPGETRELEASETOMANUFACTURE, sp);
        }

        /// <summary>
        /// GetGroupConfigurations
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        private dynamic GetGroupConfigurations(DataSet dataSet)
        {
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                var groupConfigList = (from DataRow dRow in dataSet.Tables[0].Rows
                                       select new
                                       {
                                           groupId = Convert.ToInt32(dRow[Constant.GROUPIDRLSINFO_CAMELCASE]),
                                           groupName = Convert.ToString(dRow[Constant.GROUPNAMERLSINFO_CAMELCASE]),
                                       }).Distinct().ToList();
                return groupConfigList;
            }
            return new List<string>();
        }

        /// <summary>
        /// GetReleaseInfoQueries
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        private dynamic GetReleaseInfoQueries(DataSet dataSet)
        {
            var questionsList = (from DataRow dRow in dataSet.Tables[5].Rows
                                 select new
                                 {
                                     queryId = Convert.ToString(dRow[Constant.QUERYIDLOWERCASE]),
                                     queryName = Convert.ToString(dRow[Constant.QUERYNAMELOWERCASE]),
                                     isAcknowledge = Convert.ToBoolean(dRow[Constant.ISACKNOWLEDGELOWERCASE])
                                 }).ToList();
            return questionsList;
        }

        /// <summary>
        /// GetUnitAndSetList
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        private dynamic GetUnitAndSetList(DataSet dataSet)
        {
            var unitList = (from DataRow dRow in dataSet.Tables[3].Rows
                            select new
                            {
                                unitId = Convert.ToInt32(dRow[Constant.UNITIDLOWERCASE]),
                                unitName = Convert.ToString(dRow[Constant.UNITNAMELOWERCASE]),
                                setId = Convert.ToString(dRow[Constant.SETIDLOWERCASE]),
                                relComments = Convert.ToString(dRow[Constant.RELEASECOMMENTSLOWERCASE]),
                                factoryJobId= Convert.ToString(dRow[Constants.FACTORYJOBID]),
                            }).ToList();
            return unitList;
        }

        /// <summary>
        /// GetDataPoints
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        private List<DataPoint> GetDataPoints(DataSet dataSet)
        {
            var dataPointsList = (from DataRow dRow in dataSet.Tables[1].Rows
                                  select new DataPoint
                                  {
                                      Id = Convert.ToString(dRow[Constant.DATAPOINTVAR_CAMELCASE]),
                                      Value = Convert.ToString(dRow[Constant.DATAPOINTVALUE_CAMELCASE]),
                                      IsAcknowledged = Convert.ToBoolean(dRow[Constant.ISACKNOWLEDGE_CAMELCASE]),
                                      SetId = Convert.ToString(dRow[Constant.SETIDLOWERCASE])
                                  }).ToList();
            return dataPointsList;
        }

        /// <summary>
        /// GetDistinctDataPoints
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        private (IEnumerable<string>, IEnumerable<DataPoint>) GetDistinctDataPoints(DataSet dataSet, string sessionId)
        {
            var distinctDataPoints = new List<string>();
            var dataPoints = new List<DataPoint>();
            if (dataSet.Tables[2].Rows.Count > 0)
            {
                distinctDataPoints = (from DataRow dRow in dataSet.Tables[2].Rows
                                          select Convert.ToString(dRow[Constant.DISTINCTDATAPOINT_CAMELCASE])
                                          ).ToList();
                dataPoints = GetDataPoints(dataSet);
            }
            else
            {
                var baseConfigurationRequest = _configure.ReleaseInfoCLMCall(sessionId, null, Constant.EVO200).Result;
                var variableTemplate = JObject.Parse(File.ReadAllText(Constant.UNITSVARIABLESMAPPERPATH));
                var releaseInfoDataPoints = variableTemplate[Constant.RELEASEINFODATAPOINTS].ToObject<List<string>>();
                var relaseInfoDictionary = Utility.GetConfigurationKeyValues(baseConfigurationRequest.Response);

                distinctDataPoints = relaseInfoDictionary.Keys.Where(x => releaseInfoDataPoints.Any(y => Utility.CheckEquals(x,y))).ToList();
                dataPoints = relaseInfoDictionary.Where(x => distinctDataPoints.Contains(x.Key)).Select(x => new DataPoint
                    {
                        Id = x.Key,
                        Value = x.Value,
                        IsAcknowledged = false,
                        SetId = string.Empty
                    }).ToList();

                if (dataSet.Tables[1].Rows.Count > 0)
                {
                    var dataPointList = GetDataPoints(dataSet);

                    foreach (var parameter in dataPoints)
                    {
                        parameter.IsAcknowledged = (from dataPoint in dataPointList
                                                    where dataPoint.Id.Equals(parameter.Id)
                                                    select dataPoint.IsAcknowledged).FirstOrDefault();
                    }
                }
            }
            return (distinctDataPoints, dataPoints);
        }

        /// <summary>
        /// Method to get the Unit configuration datapoint details for release to manufacture popup
        /// </summary>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        public DetailsForReleaseToManufacture GetGroupReleaseInfo(int groupId, string sessionId)
        {
            var methodBeginTime = Utility.LogBegin();
            DetailsForReleaseToManufacture releaseToManufactureDetails = new DetailsForReleaseToManufacture();
            GroupDetailsReleaseToManufacture groupConfiguration = new GroupDetailsReleaseToManufacture();
            var dataSet = GetGroupConfigurationDataSet(groupId);
            

            foreach (var group in GetGroupConfigurations(dataSet))
            {
                groupConfiguration.GroupId = group.groupId;
                groupConfiguration.GroupName = group.groupName;
                var readyToReleaseCheck = dataSet.Tables[6].Rows[0].Field<bool>(Constant.ALLSELECTEDCAMELCASE);

                groupConfiguration.ReadyToReleaseCheck = readyToReleaseCheck || false;

                groupConfiguration.ReleaseQueries = new List<ReleaseInfoQuestions>();
                foreach (var query in GetReleaseInfoQueries(dataSet))
                {
                    if (query.queryId != null)
                    {
                        ReleaseInfoQuestions releaseQuestionChecks = new ReleaseInfoQuestions();
                        releaseQuestionChecks.ReleaseQueId = query.queryId;
                        releaseQuestionChecks.ReleaseQueDesc = query.queryName;
                        releaseQuestionChecks.ReleaseQueCheck = query.isAcknowledge;

                        groupConfiguration.ReleaseQueries.Add(releaseQuestionChecks);
                    }
                }
                groupConfiguration.UnitDetails = new List<ReleaseInfoSetUnitDetails>();
                var response = GetDistinctDataPoints(dataSet, sessionId);
                var dataPoints = response.Item2;
                var distinctDataPoints = response.Item1;

                foreach (var unit in GetUnitAndSetList(dataSet))
                {
                    if (unit.unitId > 0)
                    {
                        ReleaseInfoSetUnitDetails unitSetDetails = new ReleaseInfoSetUnitDetails()
                        {
                            Id = unit.unitId,
                            UnitName = unit.unitName,
                            SetId = Convert.ToInt32(unit.setId),
                            ReleaseComments = unit.relComments,
                            FactoryJobId=unit.factoryJobId,
                            DataPointDetails = new List<ReleaseInfoDataPoints>()
                        };
                       
                        var currentDataPoints = dataPoints.Any( x => string.IsNullOrEmpty( x.SetId )) ? dataPoints : dataPoints.Where(x => Utility.CheckEquals(x.SetId, unit.setId));
                        var currentDataPointIds = currentDataPoints.Select(x => x.Id);
                        foreach (var dataId in distinctDataPoints)
                        {
                            ReleaseInfoDataPoints dataPointDetails = new ReleaseInfoDataPoints()
                            {
                                Id = dataId,
                                Value = string.Empty,
                                IsAcknowledged = false
                            };
                            if (currentDataPointIds.Contains(dataId))
                            {
                                dataPointDetails.IsAcknowledged = currentDataPoints.First(x => Utility.CheckEquals(x.Id, dataId)).IsAcknowledged ;
                                dataPointDetails.Value = currentDataPoints.First(x => Utility.CheckEquals(x.Id, dataId)).Value;
                            }
                            unitSetDetails.DataPointDetails.Add(dataPointDetails);
                        }
                        groupConfiguration.UnitDetails.Add(unitSetDetails);
                    }
                }
                releaseToManufactureDetails.GroupReleaseToManufacture = groupConfiguration;
            }
            Utility.LogEnd(methodBeginTime);
            return releaseToManufactureDetails;
        }

        /// <summary>
        /// Method to save the datapoints and release the corresponding group  
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="listOfDetails"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public List<ResultGroupReleaseResponse> SaveUpdatReleaseInfoDetailsDL(int groupid, List<ReleaseInfoSetUnitDetails> listOfDetails, List<ReleaseInfoQuestions> listOfQueries, string userId, string actionFlag)
        {
            var methodBeginTime = Utility.LogBegin();
            ResultGroupReleaseResponse result = new ResultGroupReleaseResponse();
            List<ResultGroupReleaseResponse> lstResult = new List<ResultGroupReleaseResponse>();
            int resultForSaveReleasInfo = 0;
            using (var connection = CpqDatabaseManager.CreateSqlConnection())
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                SqlTransaction sqlTransaction = connection.BeginTransaction();
                cmd.Transaction = sqlTransaction;
                cmd.Connection = connection;

                cmd.CommandText = Constant.SPSAVERELEASETOMANUFACTURE;
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var unitDetails in listOfDetails)
                {
                    cmd.Parameters.Clear();
                    DataTable unitDataTable = Utility.GenerateDataTableForSaveReleaseInfo(new List<ReleaseInfoSetUnitDetails> { unitDetails }, listOfQueries);
                    List<SqlParameter> lstSqlParameter = Utility.SqlParameterForSaveReleaseInfo(groupid, unitDataTable, userId, ConflictsStatus.Valid, actionFlag);
                    resultForSaveReleasInfo = CpqDatabaseManager.ExecuteNonquery(cmd, lstSqlParameter);
                    if (resultForSaveReleasInfo == 0)
                    {
                        sqlTransaction.Rollback();
                        break;
                    }
                }
                sqlTransaction.Commit();
            }
            if (resultForSaveReleasInfo < 0)
            {
                result.Result = 1;
                result.GroupId = resultForSaveReleasInfo;
                result.Message = Constant.RELEASETOMANUFACTURINGMESSAGE;
            }
            if (resultForSaveReleasInfo > 0)
            {
                result.Result = 2;
                result.GroupId = resultForSaveReleasInfo;
                result.Message = Constant.RELEASEINFOSAVEUPDATEMESSAGE;
            }
            else if (resultForSaveReleasInfo == 0)
            {
                result.Result = 0;
                result.GroupId = resultForSaveReleasInfo;
                result.Message = Constant.RELEASEINFOSAVEUPDATEERRORMESSAGE;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBeginTime);
            return lstResult;
        }

    }
}
