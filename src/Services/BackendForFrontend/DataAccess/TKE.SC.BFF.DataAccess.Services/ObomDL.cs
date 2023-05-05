using Configit.Configurator.Server.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.Common.Database;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.DataAccess.Services
{
	public class ObomDL: IObomDL
	{
        public DataSet GetvariableAssignmentsForObom(int setId)
        {
            var sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constants.SETID ,Value=setId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar }
            };
            return CpqDatabaseManager.ExecuteDataSet(Constants.SPGETDETAILSFOROBOMXMLGENERATION, sp);
        }
        public QuoteStatusReport GetConfigurationDetailsForStatusReport(string quoteId)
        {
            var sp = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = Constants.QUOTEIDSPVARIABLE ,Value=quoteId,Direction = ParameterDirection.Input ,SqlDbType = SqlDbType.VarChar }
            };
            var quoteStatusReport = new QuoteStatusReport() {QuoteId= quoteId, BuildingStatusReport = new List<BuildingStatusReport>() };
            var dataSet= CpqDatabaseManager.ExecuteDataSet(Constants.SPGETCONFIGURATIONDETAILSFORSTATUSREPORT, sp);
            if(dataSet?.Tables?.Count>0)
            {
                var quoteSummary = (from DataRow row in dataSet.Tables[0].Rows
                                           select new
                                           {
                                               QuoteStatus = Convert.ToString(row[Constants.DISPLAYNAMESTATUS])
                                           ,
                                               QuoteIsDeleted = Convert.ToBoolean(row[Constants.ISDELETED])
                                           }).Distinct().ToList();
                var buildingSummary = (from DataRow row in dataSet.Tables[1].Rows
                                       select new
                                       {
                                           BuildingName = Convert.ToString(row[Constant.BUILDINGNAMECOLUMNNAME])
                                       ,
                                           BuildingIsDeleted = Convert.ToBoolean(row[Constants.ISDELETED])
                                       ,
                                           BuildingId= Convert.ToInt32(row[Constants.BUILDINGIDCOLUMNNAME])
                                       ,
                                           BuildingStatus = Convert.ToString(row[Constants.DISPLAYNAMESTATUS])
                                       }).Distinct().ToList(); 
            var groupSummary = (from DataRow row in dataSet.Tables[2].Rows
                                   select new
                                   {
                                       GroupName = Convert.ToString(row[Constants.GROUPNAMECOLUMNNAME])
                                   ,
                                   GroupId= Convert.ToInt32(row[Constants.GROUPIDCOLUMNNAME])
                                   ,
                                       GroupIsDeleted = Convert.ToBoolean(row[Constants.ISDELETED])
                                   ,
                                       BuildingId = Convert.ToInt32(row[Constants.BUILDINGIDCOLUMNNAME])
                                   ,
                                       GroupStatus = Convert.ToString(row[Constants.DISPLAYNAMESTATUS])
                                   }).Distinct().ToList();
            var unitSummary = (from DataRow row in dataSet.Tables[3].Rows
                               select new
                               {
                                 
                                   GroupId = Convert.ToInt32(row[Constants.GROUPCONFIGURATIONID])
                               ,
                                    UnitId= Convert.ToInt32(row[Constants.UNITIDCOLUMN])
                               ,
                                    Designation=Convert.ToString(row[Constants.UNITDESIGNATIONCOLUMN])
                               ,
                                    Location= Convert.ToString(row[Constants.MAPPEDLOCATION])
                               ,
                                   FactoryJobID=Convert.ToString(row[Constants.FACTORYJOBID])
                               ,
                                   unitIsDeleted = Convert.ToBoolean(row[Constants.ISDELETED])
                               ,
                                   UEID = Convert.ToString(row[Constants.UEID])
                               ,
                                   SetId =Convert.ToInt32(row[Constants.SETIDCOLUMNNAME])
                               ,
                                   Status = Convert.ToString(row[Constants.DISPLAYNAMESTATUS])
                               }).Distinct().ToList();
                var setSummary = (from DataRow row in dataSet.Tables[4].Rows
                                  select new
                                  {
                                      SetId = Convert.ToInt32(row[Constants.SETIDCOLUMNNAME])
                                  ,
                                      ProductName = Convert.ToString(row[Constants.PRODUCTNAMECOLUMN])
                                  }).Distinct().ToList();

                if(quoteSummary.Any())
                {
                    quoteStatusReport.QuoteStatus = quoteSummary[0].QuoteStatus;
                    quoteStatusReport.QuoteIsDeleted = quoteSummary[0].QuoteIsDeleted;
                    foreach (var building in buildingSummary)
                    {
                        var buildingData = new BuildingStatusReport()
                        {
                            BuildingId = building.BuildingId,
                            BuildingIsDeleted = building.BuildingIsDeleted,
                            BuildingName = building.BuildingName,
                            BuildingStatus = building.BuildingStatus,
                            GroupStatusReport = new List<GroupStatusReport>()
                        };
                        var groupSummaryForBuilding = groupSummary.Where(x => x.BuildingId == building.BuildingId).ToList();
                        foreach (var group in groupSummaryForBuilding)
                        {
                            var groupData = new GroupStatusReport()
                            {
                                GroupId = group.BuildingId,
                                GroupIsDeleted = group.GroupIsDeleted,
                                GroupName = group.GroupName,
                                GroupStatus = group.GroupStatus,
                                UnitStatusReport = new List<UnitStatusReport>()
                            };
                            var unitSummaryForGroup = unitSummary.Where(x => x.GroupId == group.GroupId).ToList();
                            foreach (var unit in unitSummaryForGroup)
                            {
                                var setData = setSummary.Where(x => x.SetId == unit.SetId).ToList();
                                var unitData = new UnitStatusReport()
                                {
                                    FactoryJobId=unit.FactoryJobID,
                                    UEID=unit.UEID,
                                    UnitId=unit.UnitId,
                                    UnitIsDeleted=unit.unitIsDeleted,
                                    UnitLocation=unit.Location,
                                    UnitName=unit.Designation,
                                    UnitStatus=unit.Status,
                                    SetId=unit.SetId>0? Convert.ToString(unit.SetId):string.Empty,
                                    ProductName=setData.Any()? setData[0].ProductName:string.Empty
                                };
                                groupData.UnitStatusReport.Add(unitData);
                            }
                            buildingData.GroupStatusReport.Add(groupData);
                        }
                        quoteStatusReport.BuildingStatusReport.Add(buildingData);
                    }
                }
            }
            return quoteStatusReport;
        }
    }
}
