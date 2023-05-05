/************************************************************************************************************
************************************************************************************************************
    File Name     :   UnitConfigurationDL.cs  
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Configit.Configurator.Server.Common;
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
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Database;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.HttpClientModel;
using TKE.SC.Common.Model.UIModel;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common;
using TKE.SC.Common.Model.CommonModel;

namespace TKE.SC.BFF.DataAccess.Services
{
    public class UnitConfigurationDL : IUnitConfigurationDL
    {
        /// <summary>
        /// configuration object
        /// </summary>
        private IConfiguration _configuration;
        /// <summary>
        /// object ICacheManager
        /// </summary>
        private readonly ICacheManager _cpqCacheManager;
        /// <summary>
        /// string environment
        /// </summary>
        private readonly string _environment;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param Name="logger"></param>
        public UnitConfigurationDL(ILogger<UnitConfigurationDL> logger, IConfiguration iConfig, ICacheManager cpqCacheManager)
        {
            Utility.SetLogger(logger);
            _cpqCacheManager = cpqCacheManager;
            _configuration = iConfig;
            _environment = Constant.DEV;
        }
        /// <summary>
        /// method to get the general information
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="selectedTab"></param>
        /// <returns></returns>
        public List<ConfigVariable> GetGeneralInformationByGroupId(int groupConfigurationId, string productName, string selectedTab)
        {
            var methodBegin = Utility.LogBegin();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            List<ConfigVariable> listGroup = new List<ConfigVariable>();
            List<UnitNames> lstUnits = new List<UnitNames>();
            DataSet dataSet = new DataSet();
            if (selectedTab.ToUpper() == Constant.GENERALINFORMATION)
            {
                SqlParameter param = new SqlParameter(Constant.@GRPCONFIGID, groupConfigurationId);
                sqlParameters.Add(param);
                param = new SqlParameter(Constant.@PRODUCTNAME, productName);
                sqlParameters.Add(param);
                dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETGENERALINFORMATIONBYGROUPID, sqlParameters);
                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    string Json = string.Empty;
                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        if (!(Convert.ToString(dataSet.Tables[0].Rows[i][Constant.GENERALINFORMATIONJSON])).Equals(Constant.EMPTYSTRING))
                        {
                            Json += Convert.ToString(dataSet.Tables[0].Rows[i][Constant.GENERALINFORMATIONJSON]) + ",";
                        }
                    }
                    string jsonData = "[" + Json + "]";

                    listGroup = JsonConvert.DeserializeObject<List<ConfigVariable>>(jsonData.Replace(",]", "]").ToString());
                    listGroup = listGroup.GroupBy(group => group.VariableId).Select(g => g.First()).ToList();
                }
            }
            Utility.LogEnd(methodBegin);
            return listGroup;
        }

        /// <summary>
        /// This method is used for saving Unit Configuration Details
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="listOfDetails"></param>
        /// <param Name="userId"></param>
        /// <param Name="loghistory"></param>
        /// <returns></returns>
        public List<ResultSetConfiguration> SaveUnitConfigurationDL(int setId, List<ConfigVariable> listOfDetails, string userId, List<LogHistoryTable> loghistory, int unitId)
        {
            var methodBegin = Utility.LogBegin();
            ResultSetConfiguration result = new ResultSetConfiguration();
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
            DataTable dataTableWiring = new DataTable();
            if (unitId > 0 && listOfDetails.Where(x => x.VariableId.Contains(Constants.ADDITIONALWIRINGVALUE)).ToList().Count > 0)
            {
                var wiring = listOfDetails.Where(x => x.VariableId.Contains(Constants.ADDITIONALWIRINGVALUE)).ToList().FirstOrDefault();
                listOfDetails = listOfDetails.Where(x => !x.VariableId.Contains(Constants.ADDITIONALWIRINGVALUE)).ToList();
                dataTableWiring = Utility.GenerateDataTableForWiring(wiring, unitId);
            }
            else
            {
                dataTableWiring = Utility.GenerateDataTableForWiring(null, unitId);
            }
            DataTable unitDataTable = Utility.GenerateDataTableForUnitConfiguration(listOfDetails);
            DataTable historyTable = Utility.GenerateDataTableForHistoryTable(loghistory);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSaveUnitConfiguration(setId, unitDataTable, userId, ConflictsStatus.Valid, historyTable, dataTableWiring);
            int resultForSaveUnitConfiguration = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVEUNITCONFIGURATION, lstSqlParameter);
            if (resultForSaveUnitConfiguration > 0)
            {
                result.result = 1;
                result.setId = resultForSaveUnitConfiguration;
                result.message = Constant.UNITCONFIGURATIONSAVEMESSAGE;
            }
            else if (resultForSaveUnitConfiguration == 0)
            {
                result.result = 0;
                result.setId = resultForSaveUnitConfiguration;
                result.message = Constant.UNITCONFIGURATIONSAVEERRORMESSAGE;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBegin);
            return lstResult;
        }

        /// <summary>
        /// This method is used for updating Unit configuration details
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="listOfDetails"></param>
        /// <param Name="userId"></param>
        /// <param Name="isEditFlow"></param>
        /// <param Name="loghistory"></param>
        /// <returns></returns>
        public List<ResultSetConfiguration> UpdateUnitConfigurationDL(int setId, List<ConfigVariable> listOfDetails, string userId, ConflictsStatus isEditFlow, List<LogHistoryTable> loghistory, int unitId)
        {
            var methodBegin = Utility.LogBegin();
            ResultSetConfiguration result = new ResultSetConfiguration();
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
            DataTable unitDataTable = Utility.GenerateDataTableForUnitConfiguration(listOfDetails);
            DataTable historyTable = Utility.GenerateDataTableForHistoryTable(loghistory);
            DataTable dataTableWiring = new DataTable();
            if (unitId > 0 && listOfDetails.Where(x => x.VariableId.Contains(Constants.ADDITIONALWIRINGVALUE)).ToList().Count > 0)
            {
                var wiring = listOfDetails.Where(x => x.VariableId.Contains(Constants.ADDITIONALWIRINGVALUE)).ToList().FirstOrDefault();
                listOfDetails = listOfDetails.Where(x => !x.VariableId.Contains(Constants.ADDITIONALWIRINGVALUE)).ToList();
                dataTableWiring = Utility.GenerateDataTableForWiring(wiring, unitId);

            }
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSaveUnitConfiguration(setId, unitDataTable, userId, isEditFlow, historyTable, dataTableWiring);
            int resultForUpdateUnitConfiguration = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVEUNITCONFIGURATION, lstSqlParameter);
            if (resultForUpdateUnitConfiguration > 0)
            {
                result.result = 1;
                result.setId = resultForUpdateUnitConfiguration;
                result.message = Constant.UNITCONFIGURATIONUPDATEMESSAGE;
            }
            else if (resultForUpdateUnitConfiguration == 0)
            {
                result.result = 0;
                result.setId = resultForUpdateUnitConfiguration;
                result.message = Constant.UNITCONFIGURATIONUPDATEERRORMESSAGE;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBegin);
            return lstResult;
        }

        /// <summary>
        /// Method for save cab interior details
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="listOfDetails"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public List<ResultUnitConfiguration> SaveCabInteriorDetails(int groupConfigurationId, string productName, List<ConfigVariable> listOfDetails, string userId)
        {
            var methodBegin = Utility.LogBegin();
            ResultUnitConfiguration result = new ResultUnitConfiguration();
            List<ResultUnitConfiguration> lstResult = new List<ResultUnitConfiguration>();
            DataTable unitDataTable = Utility.GenerateDataTableForUnitConfiguration(listOfDetails);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSaveCabInteriorDetails(groupConfigurationId, productName, unitDataTable, userId);
            int resultForSaveCabInterior = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVECABINTERIORDETAILS, lstSqlParameter);
            if (resultForSaveCabInterior > 0)
            {
                result.result = 1;
                result.unitConfigurationId = resultForSaveCabInterior;
                result.message = Constant.CABINTERIORSAVEMESSAGE;
            }
            else if (resultForSaveCabInterior == 0)
            {
                result.result = 0;
                result.unitConfigurationId = resultForSaveCabInterior;
                result.message = Constant.CABINTERIORERRORMESSAGE;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBegin);
            return lstResult;
        }

        /// <summary>
        /// Method for updating cab interior details
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="listOfDetails"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public List<ResultUnitConfiguration> UpdateCabInteriorDetails(int groupConfigurationId, string productName, List<ConfigVariable> listOfDetails, string userId)
        {
            var methodBegin = Utility.LogBegin();
            ResultUnitConfiguration result = new ResultUnitConfiguration();
            List<ResultUnitConfiguration> lstResult = new List<ResultUnitConfiguration>();
            DataTable unitDataTable = Utility.GenerateDataTableForUnitConfiguration(listOfDetails);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSaveCabInteriorDetails(groupConfigurationId, productName, unitDataTable, userId);
            int resultForSaveCabInterior = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVECABINTERIORDETAILS, lstSqlParameter);
            if (resultForSaveCabInterior > 0)
            {
                result.result = 1;
                result.unitConfigurationId = resultForSaveCabInterior;
                result.message = Constant.CABINTERIORUPDATEMESSAGE;
            }
            else if (resultForSaveCabInterior == 0)
            {
                result.result = 0;
                result.unitConfigurationId = resultForSaveCabInterior;
                result.message = Constant.CABINTERIORUPDATEERRORMESSAGE;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBegin);
            return lstResult;
        }

        /// <summary>
        /// Method for save cab interior details
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="listOfDetails"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public List<ResultUnitConfiguration> SaveHoistwayTractionEquipment(int groupConfigurationId, string productName, List<ConfigVariable> listOfDetails, string userId)
        {
            var methodBegin = Utility.LogBegin();
            ResultUnitConfiguration result = new ResultUnitConfiguration();
            List<ResultUnitConfiguration> lstResult = new List<ResultUnitConfiguration>();
            DataTable unitDataTable = Utility.GenerateDataTableForUnitConfiguration(listOfDetails);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSaveCabInteriorDetails(groupConfigurationId, productName, unitDataTable, userId);
            int resultForSaveHoistwayTractionEquipment = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVEHOISTWAYTRACTIONEQUIPMENT, lstSqlParameter);
            if (resultForSaveHoistwayTractionEquipment > 0)
            {
                result.result = 1;
                result.unitConfigurationId = resultForSaveHoistwayTractionEquipment;
                result.message = Constant.HOISTWAYTRACTIONEQUIPMENTSAVEMESSAGE;
            }
            else if (resultForSaveHoistwayTractionEquipment == 0)
            {
                result.result = 0;
                result.unitConfigurationId = resultForSaveHoistwayTractionEquipment;
                result.message = Constant.HOISTWAYTRACTIONERRORMESSAGE;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBegin);
            return lstResult;
        }

        /// <summary>
        /// Method for update hoistway traction equipment
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="unitVariableAssignment"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        public List<ResultUnitConfiguration> UpdateHoistwayTractionEquipment(int groupConfigurationId, string productName, List<ConfigVariable> listOfDetails, string userId)
        {
            var methodBegin = Utility.LogBegin();
            ResultUnitConfiguration result = new ResultUnitConfiguration();
            List<ResultUnitConfiguration> lstResult = new List<ResultUnitConfiguration>();
            DataTable unitDataTable = Utility.GenerateDataTableForUnitConfiguration(listOfDetails);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSaveCabInteriorDetails(groupConfigurationId, productName, unitDataTable, userId);
            int resultForUpdateHoistwayTractionEquipment = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVEHOISTWAYTRACTIONEQUIPMENT, lstSqlParameter);
            if (resultForUpdateHoistwayTractionEquipment > 0)
            {
                result.result = 1;
                result.unitConfigurationId = resultForUpdateHoistwayTractionEquipment;
                result.message = Constant.HOISTWAYTRACTIONEQUIPMENTUPDATEMESSAGE;
            }
            else if (resultForUpdateHoistwayTractionEquipment == 0)
            {
                result.result = 0;
                result.unitConfigurationId = resultForUpdateHoistwayTractionEquipment;
                result.message = Constant.HOISTWAYTRACTIONUPDATEERRORMESSAGE;
            }

            lstResult.Add(result);
            Utility.LogEnd(methodBegin);
            return lstResult;
        }

        /// <summary>
        /// method to get the Unit configuration 
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="setId"></param>
        /// <param Name="selectedTab"></param>
        /// <returns></returns>
        public UnitVariableDetails GetUnitConfigurationByGroupId(int groupConfigurationId, int setId, string selectedTab)
        {
            var methodBegin = Utility.LogBegin();
            var unitMapperVariables = Utility.VariableMapper(Constant.UNITSVARIABLESMAPPERPATH, Constant.UNITMAPPER);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            UnitVariableDetails listGroup = new UnitVariableDetails();
            listGroup.listOfConfigVariables = new List<ConfigVariable>();
            List<UnitNames> lstUnits = new List<UnitNames>();
            DataSet dataSet = new DataSet();
            SqlParameter param = new SqlParameter(Constant.@GRPCONFIGID, groupConfigurationId);
            sqlParameters.Add(param);
            param = new SqlParameter(Constant.@SETID, setId);
            sqlParameters.Add(param);
            param = new SqlParameter(Constants.SELECTEDTAB, selectedTab);
            sqlParameters.Add(param);
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETUNITCONFIGURATIONBYGROUPID, sqlParameters);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    ConfigVariable us = new ConfigVariable();
                    us.VariableId = Convert.ToString(dataSet.Tables[0].Rows[i][Constant.CONFIGVARIABLES]);
                    us.Value = Convert.ToString(dataSet.Tables[0].Rows[i][Constant.CONFIGUREVALUES]);
                    listGroup.listOfConfigVariables.Add(us);
                }
                if (dataSet.Tables[4].Rows.Count > 0)
                {
                    if ((Int32)dataSet.Tables[4].Rows[0][Constant.TOTALNUMBEROFFLOORS] > 0)
                    {
                        ConfigVariable us = new ConfigVariable();
                        us.VariableId = unitMapperVariables[Constant.NUMBEROFFLOORSUNITPACKAGEVARIABLE];
                        us.Value = (Int32)dataSet.Tables[4].Rows[0][Constant.TOTALNUMBEROFFLOORS];
                        listGroup.listOfConfigVariables.Add(us);
                    }
                }
                //To get Total available Front and Rear openings 
                if (dataSet.Tables[5].Rows.Count > 0)
                {
                    if ((Int32)dataSet.Tables[5].Rows[0][Constant.FRONTOPENING] > 0)
                    {
                        ConfigVariable openingFront = new ConfigVariable();
                        openingFront.VariableId = unitMapperVariables[Constant.OPENFRONT];
                        openingFront.Value = ((Int32)dataSet.Tables[5].Rows[0][Constant.FRONTOPENING]);
                        listGroup.listOfConfigVariables.Add(openingFront);
                    }
                    if ((Int32)dataSet.Tables[5].Rows[0][Constant.REAROPENING] > 0)
                    {
                        ConfigVariable openingRear = new ConfigVariable();
                        openingRear.VariableId = unitMapperVariables[Constant.OPENREAR];
                        openingRear.Value = (Int32)dataSet.Tables[5].Rows[0][Constant.REAROPENING];
                        listGroup.listOfConfigVariables.Add(openingRear);
                    }
                }
                listGroup.conflictListOfStrings = new List<string>();
                if (dataSet.Tables[8] != null && dataSet.Tables[8].Rows.Count > 0)
                {
                    var conflictVaribales = new List<string>();
                    foreach (DataRow item in dataSet.Tables[8].Rows)
                    {
                        conflictVaribales.Add(item[Constants.CONFLICTVARIABLEIDDATA].ToString());
                    }
                    listGroup.conflictListOfStrings = conflictVaribales;
                }
            }
            Utility.LogEnd(methodBegin);
            return listGroup;
        }

        /// <summary>
        /// method to get the units
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="setId"></param>
        /// <returns></returns>
        public List<UnitNames> GetUnitsByGroupId(int groupConfigurationId, int setId)
        {
            var methodBegin = Utility.LogBegin();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            List<UnitNames> lstUnits = new List<UnitNames>();
            DataSet dataSet = new DataSet();
            SqlParameter param = new SqlParameter(Constant.@GRPCONFIGID, groupConfigurationId);
            sqlParameters.Add(param);
            param = new SqlParameter(Constant.SETID, setId);
            sqlParameters.Add(param);
            param = new SqlParameter(Constants.SELECTEDTAB, Constants.UNITS);
            sqlParameters.Add(param);
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETUNITCONFIGURATIONBYGROUPID, sqlParameters);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[2].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[2].Rows.Count; i++)
                {
                    UnitNames us = new UnitNames();
                    us.Unitid = Convert.ToInt32(dataSet.Tables[2].Rows[i][Constant.UNITID_LOWERCASE]);
                    us.Unitname = Convert.ToString(dataSet.Tables[2].Rows[i][Constant.UNITNAME_LOWERCASE]);
                    us.Ueid = Convert.ToString(dataSet.Tables[2].Rows[i][Constant.UEID_LOWERCASE]);
                    lstUnits.Add(us);
                }
            }
            Utility.LogEnd(methodBegin);
            return lstUnits;
        }

        /// <summary>
        /// SaveGeneralInformation
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="listOfGeneralInformationVariables"></param>
        /// <returns></returns>
        public List<ResultUnitConfiguration> SaveGeneralInformation(int groupConfigurationId, string productName, List<ConfigVariable> listOfGeneralInformationVariables, string userId)
        {
            var methodBegin = Utility.LogBegin();
            ResultUnitConfiguration result = new ResultUnitConfiguration();
            List<ResultUnitConfiguration> lstResult = new List<ResultUnitConfiguration>();
            DataTable GeneralInformationDataTable = Utility.GenerateDataTableForSaveGeneralInformation(listOfGeneralInformationVariables);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSavingGeneralInformation(groupConfigurationId, productName, GeneralInformationDataTable, userId);
            int resultForSaveGeneralInformation = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVEGENERALINFORMATION, lstSqlParameter);
            if (resultForSaveGeneralInformation > 0)
            {
                result.result = 1;
                result.unitConfigurationId = resultForSaveGeneralInformation;
                result.message = Constant.GENERALINFORMATIONSAVEMESSAGE;
            }
            else if (resultForSaveGeneralInformation == 0)
            {
                result.result = 0;
                result.unitConfigurationId = resultForSaveGeneralInformation;
                result.message = Constant.GENERALINFORMATIONERRORMESSAGE;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBegin);
            return lstResult;
        }

        /// <summary>
        /// UpdateGeneralInformation
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="listOfGeneralInformationVariables"></param>
        /// <returns></returns>
        public List<ResultUnitConfiguration> UpdateGeneralInformation(int groupConfigurationId, string productName, List<ConfigVariable> listOfGeneralInformationVariables, string userId)
        {
            var methodBegin = Utility.LogBegin();
            ResultUnitConfiguration result = new ResultUnitConfiguration();
            List<ResultUnitConfiguration> lstResult = new List<ResultUnitConfiguration>();
            DataTable GeneralInformationDataTable = Utility.GenerateDataTableForSaveGeneralInformation(listOfGeneralInformationVariables);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSavingGeneralInformation(groupConfigurationId, productName, GeneralInformationDataTable, userId);
            int resultForSaveGeneralInformation = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVEGENERALINFORMATION, lstSqlParameter);
            if (resultForSaveGeneralInformation > 0)
            {
                result.result = 1;
                result.unitConfigurationId = resultForSaveGeneralInformation;
                result.message = Constant.GENERALINFORMATIONUPDATEMESSAGE;
            }
            else if (resultForSaveGeneralInformation == 0)
            {
                result.result = 0;
                result.unitConfigurationId = resultForSaveGeneralInformation;
                result.message = Constant.GENERALINFORMATIONUPDATEERRORMESSAGE;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBegin);
            return lstResult;
        }

        /// <summary>
        /// GetCabInteriorByGroupId
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="selectedTab"></param>
        /// <returns></returns>
        public List<ConfigVariable> GetCabInteriorByGroupId(int groupConfigurationId, string productName, string selectedTab)
        {
            var methodBegin = Utility.LogBegin();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            List<ConfigVariable> listGroup = new List<ConfigVariable>();
            DataSet dataSet = new DataSet();
            if (selectedTab.ToUpper() == Constant.CABINTERIOR)
            {
                SqlParameter param = new SqlParameter(Constant.@GRPCONFIGID, groupConfigurationId);
                sqlParameters.Add(param);
                param = new SqlParameter(Constant.@PRODUCTNAME, productName);
                sqlParameters.Add(param);
                dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETCABINTERIORGROUPID, sqlParameters);
                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    string Json = string.Empty;
                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        if (!(Convert.ToString(dataSet.Tables[0].Rows[i][Constant.CABINTERIORJSON])).Equals(Constant.EMPTYSTRING))
                        {
                            Json += Convert.ToString(dataSet.Tables[0].Rows[i][Constant.CABINTERIORJSON]) + ",";
                        }
                    }
                    string jsonData = "[" + Json + "]";
                    listGroup = JsonConvert.DeserializeObject<List<ConfigVariable>>(jsonData.Replace(",]", "]").ToString());
                    listGroup = listGroup.GroupBy(group => group.VariableId).Select(g => g.First()).ToList();
                }
            }
            Utility.LogEnd(methodBegin);
            return listGroup;
        }

        /// <summary>
        /// GetHoistwayTractionByGroupId
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="selectTab"></param>
        /// <returns></returns>
        public List<ConfigVariable> GetHoistwayTractionByGroupId(int groupConfigurationId, string productName, string selectTab)
        {
            var methodBegin = Utility.LogBegin();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            List<ConfigVariable> listGroup = new List<ConfigVariable>();
            List<UnitNames> lstUnits = new List<UnitNames>();
            DataSet dataSet = new DataSet();
            if (selectTab.ToUpper() == Constant.HOISTWAYTRACTIONEQUIPMENT)
            {
                SqlParameter param = new SqlParameter(Constant.@GRPCONFIGID, groupConfigurationId);
                sqlParameters.Add(param);
                param = new SqlParameter(Constant.@PRODUCTNAME, productName);
                sqlParameters.Add(param);
                dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETHOSITWAYTRACTIONEQUIPMENTBYGROUPID, sqlParameters);
                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    string Json = string.Empty;
                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        if (!(Convert.ToString(dataSet.Tables[0].Rows[i][Constant.HOSITWAYTRACTIONEQUIPMENTJSON])).Equals(Constant.EMPTYSTRING))
                        {
                            Json += Convert.ToString(dataSet.Tables[0].Rows[i][Constant.HOSITWAYTRACTIONEQUIPMENTJSON]) + ",";
                        }
                    }
                    string jsonData = "[" + Json + "]";
                    listGroup = JsonConvert.DeserializeObject<List<ConfigVariable>>(jsonData.Replace(",]", "]").ToString());
                    listGroup = listGroup.GroupBy(group => group.VariableId).Select(g => g.First()).ToList();
                }
            }
            Utility.LogEnd(methodBegin);
            return listGroup;
        }

        /// <summary>
        /// GetHoistwayTractionUnitsByGroupId
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="selectTab"></param>
        /// <returns></returns>
        public List<UnitNames> GetHoistwayTractionUnitsByGroupId(int groupConfigurationId, string productName, string selectTab)
        {
            var methodBegin = Utility.LogBegin();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            List<UnitNames> lstUnits = new List<UnitNames>();
            DataSet dataSet = new DataSet();
            if (selectTab.ToUpper() == Constant.HOISTWAYTRACTIONEQUIPMENT)
            {
                SqlParameter param = new SqlParameter(Constant.@GRPCONFIGID, groupConfigurationId);
                sqlParameters.Add(param);
                param = new SqlParameter(Constant.@PRODUCTNAME, productName);
                sqlParameters.Add(param);
                dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETHOSITWAYTRACTIONEQUIPMENTBYGROUPID, sqlParameters);
                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[1].Rows.Count > 0)
                {
                    for (int i = 0; i < dataSet.Tables[1].Rows.Count; i++)
                    {
                        UnitNames us = new UnitNames();
                        us.Unitid = Convert.ToInt32(dataSet.Tables[1].Rows[i][Constant.UNITID]);
                        us.Unitname = Convert.ToString(dataSet.Tables[1].Rows[i][Constant.UNITNAME_LOWERCASE]);
                        lstUnits.Add(us);
                    }
                }

            }
            Utility.LogEnd(methodBegin);
            return lstUnits;
        }

        /// <summary>
        /// GetEntranceByGroupId
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="selectedTab"></param>
        /// <returns></returns>
        public List<ConfigVariable> GetEntranceByGroupId(int groupConfigurationId, string productName, string selectedTab)
        {
            var methodBegin = Utility.LogBegin();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            List<ConfigVariable> listGroup = new List<ConfigVariable>();
            DataSet dataSet = new DataSet();
            if (selectedTab.ToUpper() == Constant.ENTRANCES)
            {
                SqlParameter param = new SqlParameter(Constant.@GRPCONFIGID, groupConfigurationId);
                sqlParameters.Add(param);
                param = new SqlParameter(Constant.@PRODUCTNAME, productName);
                sqlParameters.Add(param);
                dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETENTRANCEBYGROUPID, sqlParameters);
                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    string Json = string.Empty;
                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        if (!(Convert.ToString(dataSet.Tables[0].Rows[i][Constant.ENTRANCEJSON])).Equals(Constant.EMPTYSTRING))
                        {
                            Json += Convert.ToString(dataSet.Tables[0].Rows[i][Constant.ENTRANCEJSON]) + ",";
                        }
                    }

                    string jsonData = "[" + Json + "]";
                    listGroup = JsonConvert.DeserializeObject<List<ConfigVariable>>(jsonData.Replace(",]", "]").ToString());
                    listGroup = listGroup.GroupBy(group => group.VariableId).Select(g => g.First()).ToList();
                }
            }
            Utility.LogEnd(methodBegin);
            return listGroup;
        }

        /// <summary>
        /// EditUnitDesignation
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="UnitId"></param>
        /// <param Name="userid"></param>
        /// <returns></returns>
        public int EditUnitDesignation(int groupId, int unitId, string userid, UnitDesignation unit)
        {
            var methodBegin = Utility.LogBegin();
            if (unit.Designation == null)
            {
                unit.Designation = "";
            }
            if (unit.Description == null)
            {
                unit.Description = "";
            }
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForEditUnitdesignation(groupId, unitId, userid, unit);
            Utility.LogEnd(methodBegin);
            return CpqDatabaseManager.ExecuteNonquery(Constant.SPEDITUNITDESIGNATION, lstSqlParameter); ;
        }

        /// <summary>
        /// SaveEntranceConfiguration
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="entranceConfigurationData"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public List<ResultSetConfiguration> SaveEntranceConfiguration(int setId, EntranceConfigurationData entranceConfigurationData, string userId, int is_Saved, bool isReset, List<LogHistoryTable> historyTable)
        {
            var methodBegin = Utility.LogBegin();
            ResultSetConfiguration result = new ResultSetConfiguration();
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
            DataTable entranceConsoleDataTable = Utility.GenerateEntranceConsoleDataTable(entranceConfigurationData);
            DataTable entranceConfigurationDataTable = Utility.GenerateEntranceConfigurationDataTable(entranceConfigurationData.VariableAssignments, Convert.ToInt32(entranceConfigurationData.EntranceConsoleId));
            DataTable entranceLocationDataTable = Utility.GenerateEntranceLocationDataTable(entranceConfigurationData.EntranceLocations, Convert.ToInt32(entranceConfigurationData.EntranceConsoleId));
            DataTable historyDataTable = Utility.GenerateDataTableForHistoryTable(historyTable);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSaveEntranceConfiguration(setId, Convert.ToInt32(entranceConfigurationData.EntranceConsoleId), entranceConsoleDataTable, entranceConfigurationDataTable, entranceLocationDataTable, userId, isReset, historyDataTable);
            int resultForSaveUnitConfiguration = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVEENTRANCECONFIGURATION, lstSqlParameter);
            if (resultForSaveUnitConfiguration > 0 && is_Saved.Equals(0))
            {
                result.result = 1;
                result.setId = resultForSaveUnitConfiguration;
                result.message = Constant.UNITCONFIGURATIONSAVEMESSAGE;
            }
            else if (resultForSaveUnitConfiguration > 0 && is_Saved.Equals(1))
            {
                result.result = 1;
                result.setId = resultForSaveUnitConfiguration;
                result.message = Constant.UNITCONFIGURATIONUPDATEMESSAGE;
            }
            else if (resultForSaveUnitConfiguration == 0)
            {
                result.result = 0;
                result.setId = resultForSaveUnitConfiguration;
                result.message = Constant.UNITCONFIGURATIONSAVEERRORMESSAGE;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBegin);
            return lstResult;
        }

        /// <summary>
        /// GetEntranceConfiguration
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="groupid"></param>
        /// <param Name="selectTab"></param>
        /// <returns></returns>
        public List<EntranceConfigurations> GetEntranceConfiguration(int setId, int groupid, string controlLanding, string username, bool isJambMounted = false)
        {
            var methodBegin = Utility.LogBegin();
            List<EntranceConfigurations> objEntranceConfigurationData = new List<EntranceConfigurations>();
            var entranceDefaults = String.Empty;
            var productType = GetProductType(setId);
            if (productType.Equals(Constant.EVO200))
            {
                entranceDefaults = JObject.Parse(File.ReadAllText(string.Format(Constants.ENTRANCESCONSOLEDEFAULTVALUES, Constant.EVOLUTION200))).ToString();
            }
            else if (productType.Equals(Constant.ENDURA100))
            {
                entranceDefaults = JObject.Parse(File.ReadAllText(string.Format(Constants.ENTRANCESCONSOLEDEFAULTVALUES, Constants.END100))).ToString();
            }
            else if (productType.Equals(Constant.EVO100))
            {
                entranceDefaults = JObject.Parse(File.ReadAllText(string.Format(Constants.ENTRANCESCONSOLEDEFAULTVALUES, Constants.EVOLUTION100))).ToString();
            }
            var defaultEntranceConfigurationValues = Utility.DeserializeObjectValue<Dictionary<string, Dictionary<string, string>>>(entranceDefaults);
            DataTable defaultVariablesEntranceConfiguration = Utility.GenerateDataTableForDefaultConsoleVariables(defaultEntranceConfigurationValues);
            int controlLdg = controlLanding == null || String.IsNullOrEmpty(controlLanding) ? 0 : Convert.ToInt32(controlLanding);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                 new SqlParameter() { ParameterName = Constant.SETID, Value = setId},
                 new SqlParameter() { ParameterName = Constant.@GROUPCONFIGRATIONID, Value = groupid},
                 new SqlParameter() { ParameterName = Constant.ISJAMBMOUNTED, Value = isJambMounted},
                 new SqlParameter() { ParameterName = Constant.@CONTROLLANDING, Value = controlLdg},
                 new SqlParameter() { ParameterName = Constant.USERNAME, Value = username},
                 new SqlParameter() { ParameterName = Constant.@DEFAULTUHFCONFIGURATION, Value = defaultVariablesEntranceConfiguration}
            };
            DataSet dataSet = new DataSet();
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETENTRANCEBYSETID, sqlParameters);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables.Count > 1)
            {
                int noOfFloor = 0;
                string productName = "";
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    noOfFloor = Convert.ToInt32(dataSet.Tables[0].Rows[0][Constant.NOOFFLOOR]);
                    productName = dataSet.Tables[0].Rows[0][Constant.PRODUCTNAME1].ToString();
                }
                if (dataSet.Tables[1].Rows.Count > 0)
                {
                    List<EntranceConfigurations> entranceConsoles = new List<EntranceConfigurations>();
                    var entranceConsoleList = (from DataRow row in dataSet.Tables[1].Rows
                                               select new
                                               {
                                                   entranceConsoleId = Convert.ToInt32(row[Constant.ENTRANCECONSOLEID]),
                                                   ConsoleName = row[Constant.ENTRANCECONSOLENAME].ToString(),
                                                   isController = Convert.ToBoolean(row[Constant.ISCONTROLLER]),
                                                   FrontOpening = Convert.ToInt32(row[Constant.FRONTOPENING]),
                                                   RearOpening = Convert.ToInt32(row[Constant.REAROPENING])
                                               }).Distinct();
                    foreach (var console in entranceConsoleList)
                    {
                        var opening = new Openings()
                        {
                            Front = console.FrontOpening > 0,
                            Rear = console.RearOpening > 0
                        };
                        EntranceConfigurations entranceConsole = new EntranceConfigurations()
                        {
                            EntranceConsoleId = console.entranceConsoleId,
                            ConsoleName = console.ConsoleName,
                            AssignOpenings = !console.isController,
                            IsController = console.isController,
                            FrontOpenings = console.FrontOpening,
                            RearOpenings = console.RearOpening,
                            ProductName = productName,
                            NoOfFloor = noOfFloor,
                            Openings = opening

                        };
                        var variableList = (from DataRow row in dataSet.Tables[1].Rows
                                            select new
                                            {
                                                entranceConsoleId = Convert.ToInt32(row[Constant.ENTRANCECONSOLEID]),
                                                variableType = row[Constant.VARIABLETYPE].ToString(),
                                                variablevalue = row[Constant.VARIABLEVALUE].ToString()
                                            }).Distinct();
                        variableList = variableList.Where(x => x.entranceConsoleId.Equals(entranceConsole.EntranceConsoleId)).Distinct().ToList();
                        List<ConfigVariable> variableAssignments = new List<ConfigVariable>();
                        foreach (var assignments in variableList)
                        {
                            ConfigVariable variableAssignment = new ConfigVariable()
                            {
                                VariableId = assignments.variableType,
                                Value = assignments.variablevalue
                            };
                            variableAssignments.Add(variableAssignment);

                        }
                        entranceConsole.VariableAssignments = variableAssignments;
                        var entranceList = (from DataRow row in dataSet.Tables[1].Rows
                                            select new
                                            {
                                                entranceConsoleId = Convert.ToInt32(row[Constant.ENTRANCECONSOLEID]),
                                                floorNumber = Convert.ToInt32(row[Constant.FLOORNUMBER]),
                                                floorDesignation = row[Constant.FLOORNUMBER].ToString(),
                                                front = Convert.ToBoolean(row[Constant.FRONT]),
                                                rear = Convert.ToBoolean(row[Constant.REAR]),
                                                openingFront = Convert.ToBoolean(row[Constant.OPENINGFRONT]),
                                                openingRear = Convert.ToBoolean(row[Constant.OPENINGREAR])
                                            }).Distinct();
                        entranceList = entranceList.Where(x => x.entranceConsoleId.Equals(entranceConsole.EntranceConsoleId)).Distinct().ToList();
                        List<EntranceLocations> entranceLocations = new List<EntranceLocations>();
                        foreach (var entrancelocation in entranceList)
                        {
                            EntranceLocations entranceLocation = new EntranceLocations()
                            {
                                FloorNumber = entrancelocation.floorNumber,
                                FloorDesignation = entrancelocation.floorDesignation
                            };
                            LandingOpening landingOpening = new LandingOpening()
                            {
                                InCompatible = false,
                                NotAvailable = !entrancelocation.openingFront,
                                Value = entrancelocation.front
                            };
                            entranceLocation.Front = landingOpening;
                            landingOpening = new LandingOpening()
                            {
                                InCompatible = false,
                                NotAvailable = !entrancelocation.openingRear,
                                Value = entrancelocation.rear
                            };
                            entranceLocation.Rear = landingOpening;
                            entranceLocations.Add(entranceLocation);

                        }
                        entranceConsole.FixtureLocations = entranceLocations;
                        entranceConsoles.Add(entranceConsole);
                    }
                    if (entranceConsoles.Count > 0)
                    {
                        var opening = entranceConsoles[0].Openings;
                        var lstlocation = new List<EntranceLocations>();
                        var varEntranceLocation = entranceConsoles[0].FixtureLocations;
                        foreach (var location in varEntranceLocation)
                        {
                            var varlocation = new EntranceLocations()
                            {
                                FloorNumber = location.FloorNumber,
                                FloorDesignation = location.FloorDesignation,
                                Front = new LandingOpening()
                                {
                                    Value = false,
                                    InCompatible = false,
                                    NotAvailable = location.Front.NotAvailable
                                },
                                Rear = new LandingOpening()
                                {
                                    Value = false,
                                    InCompatible = false,
                                    NotAvailable = location.Rear.NotAvailable
                                }
                            };
                            lstlocation.Add(varlocation);
                        }
                        EntranceConfigurations objEntranceConsole = new EntranceConfigurations()
                        {
                            EntranceConsoleId = 0,
                            VariableAssignments = new List<ConfigVariable>(),
                            ConsoleName = Constant.ENTRANCECONSOLEONE,
                            IsController = false,
                            ProductName = productName,
                            FixtureLocations = lstlocation,
                            Openings = opening,
                            AssignOpenings = true
                        };
                        entranceConsoles.Add(objEntranceConsole);
                    }
                    objEntranceConfigurationData = entranceConsoles;
                }
            }
            else
            {
                List<EntranceConfigurations> EntranceConfiguration = new List<EntranceConfigurations>();
                List<EntranceLocations> varLocation = new List<EntranceLocations>();
                bool isController = false;
                string productName = "";
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    isController = Convert.ToBoolean(dataSet.Tables[0].Rows[0][Constant.ISCONTROLLER]);
                    productName = dataSet.Tables[0].Rows[0][Constant.PRODUCTNAME1].ToString();
                    int NoofFloor = Convert.ToInt32(dataSet.Tables[0].Rows[0][Constant.NOOFFLOOR]);
                    var lstEntranceLocation = (from DataRow row in dataSet.Tables[0].Rows
                                               select new
                                               {
                                                   FloorNumber = Convert.ToInt32(row[Constant.FLOORNUMBER]),
                                                   FloorDesignation = row[Constant.FLOORDESIGNATION].ToString(),
                                                   Front = Convert.ToInt32(row[Constant.FRONT]),
                                                   Rear = Convert.ToInt32(row[Constant.REAR]),
                                                   frontOpening = Convert.ToInt32(row[Constant.FRONTOPENING]),
                                                   rearOpening = Convert.ToInt32(row[Constant.REAROPENING])
                                               }).Distinct().ToList();

                    var opening = new Openings()
                    {
                        Front = lstEntranceLocation[0].frontOpening > 0,
                        Rear = lstEntranceLocation[0].rearOpening > 0
                    };
                    varLocation = new List<EntranceLocations>();
                    foreach (var rowEntranceLocation in lstEntranceLocation)
                    {
                        EntranceLocations varEntranceLocations = new EntranceLocations()
                        {
                            FloorNumber = rowEntranceLocation.FloorNumber,
                            FloorDesignation = rowEntranceLocation.FloorDesignation
                        };
                        LandingOpening landingOpening = new LandingOpening()
                        {
                            InCompatible = false,
                            NotAvailable = !opening.Front,
                            Value = false
                        };
                        varEntranceLocations.Front = landingOpening;
                        landingOpening = new LandingOpening()
                        {
                            InCompatible = false,
                            NotAvailable = !opening.Rear,
                            Value = false
                        };
                        varEntranceLocations.Rear = landingOpening;
                        varLocation.Add(varEntranceLocations);
                    }

                    EntranceConfigurations objEntranceConsole = new EntranceConfigurations()
                    {
                        EntranceConsoleId = 0,
                        VariableAssignments = new List<ConfigVariable>(),
                        ConsoleName = Constant.ENTRANCECONSOLEONE,
                        IsController = false,
                        ProductName = productName,
                        FixtureLocations = varLocation,
                        Openings = opening,
                        AssignOpenings = true
                    };
                    EntranceConfiguration.Add(objEntranceConsole);
                }
                objEntranceConfigurationData = EntranceConfiguration;
            }
            Utility.LogEnd(methodBegin);
            return objEntranceConfigurationData;
        }

        /// <summary>
        /// GetFixtureStrategy
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <returns></returns>
        public string GetFixtureStrategy(int groupConfigurationId)
        {
            var methodBegin = Utility.LogBegin();


            IList<SqlParameter> sqlParam = new List<SqlParameter>();
            SqlParameter param = new SqlParameter(Constant.GROUPID, groupConfigurationId);
            sqlParam.Add(param);
            var fixtureStrategy = CpqDatabaseManager.ExecuteScalarForReturnString(Constant.SPGETFIXTURESTRATEGY, sqlParam);

            Utility.LogEnd(methodBegin);
            return fixtureStrategy;
        }

        /// <summary>
        /// GetUnitHallFixturesData
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="groupid"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        public List<UnitHallFixtures> GetUnitHallFixturesData(int setId, int groupid, string userName, string fixtureStrategy, string sessionId)
        {
            var methodBegin = Utility.LogBegin();
            List<UnitHallFixtures> objUnitHallFixtureConfigurationData = new List<UnitHallFixtures>();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            var uHFDefaults = String.Empty;
            var entranceDefaults = String.Empty;
            var productType = GetProductType(setId);
            if (productType.Equals(Constant.EVO200))
            {
                uHFDefaults = JObject.Parse(File.ReadAllText(string.Format(Constants.UNITHALLFIXTURECONSOLEDEFAULTVALUES, Constant.EVOLUTION200))).ToString();
                entranceDefaults = JObject.Parse(File.ReadAllText(string.Format(Constants.ENTRANCESCONSOLEDEFAULTVALUES, Constant.EVOLUTION200))).ToString();
            }
            else if (productType.Equals(Constant.ENDURA100))
            {
                uHFDefaults = JObject.Parse(File.ReadAllText(string.Format(Constants.UNITHALLFIXTURECONSOLEDEFAULTVALUES, Constants.END100))).ToString();
                entranceDefaults = JObject.Parse(File.ReadAllText(string.Format(Constants.ENTRANCESCONSOLEDEFAULTVALUES, Constants.END100))).ToString();
            }
            else if (productType.Equals(Constant.EVO100))
            {
                uHFDefaults = JObject.Parse(File.ReadAllText(string.Format(Constants.UNITHALLFIXTURECONSOLEDEFAULTVALUES, Constants.EVOLUTION100))).ToString();
                entranceDefaults = JObject.Parse(File.ReadAllText(string.Format(Constants.ENTRANCESCONSOLEDEFAULTVALUES, Constants.EVOLUTION100))).ToString();
            }
            var defaultUHFVariables = Utility.DeserializeObjectValue<Dictionary<string, Dictionary<string, string>>>(uHFDefaults);
            DataTable defaultVariablesUHF = Utility.GenerateDataTableForDefaultConsoleVariables(defaultUHFVariables);
            SqlParameter param = new SqlParameter(Constant.SETID, setId);
            sqlParameters.Add(param);
            param = new SqlParameter(Constant.@GROUPCONFIGRATIONID, groupid);
            sqlParameters.Add(param);
            param = new SqlParameter(Constant.@USERNAME, userName);
            sqlParameters.Add(param);
            param = new SqlParameter(Constant.@FIXTURESTRATEGY, fixtureStrategy);
            sqlParameters.Add(param);
            param = new SqlParameter(Constant.@UHFDefaults, defaultVariablesUHF);
            sqlParameters.Add(param);
            DataSet dataSet = new DataSet();
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETHALLLANTERNCONFIGURATION, sqlParameters);
            if (dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    List<UnitHallFixtures> unitHallFixtureConsoles = new List<UnitHallFixtures>();
                    var unitHallFixtureConsoleList = (from DataRow row in dataSet.Tables[0].Rows
                                                      select new
                                                      {
                                                          entranceConsoleId = Convert.ToInt32(row[Constant.UNITHALLFIXTURECONSOLEID]),
                                                          ConsoleName = row["UnitHallFixturenConsoleName"].ToString(),
                                                          isController = Convert.ToBoolean(row[Constant.ISCONTROLLER]),
                                                          FrontOpening = Convert.ToBoolean(row[Constant.FRONTOPENING]),
                                                          RearOpening = Convert.ToBoolean(row[Constant.REAROPENING]),
                                                          FixtureType = row[Constant.FIXTURETYPEVARIABLE].ToString()

                                                      }).Distinct();
                    foreach (var console in unitHallFixtureConsoleList)
                    {
                        var opening = new Openings()
                        {
                            Front = console.FrontOpening,
                            Rear = console.RearOpening
                        };
                        UnitHallFixtures unitHallFixtureConsole = new UnitHallFixtures()
                        {
                            ConsoleId = console.entranceConsoleId,
                            ConsoleName = console.ConsoleName,
                            AssignOpenings = !console.isController,
                            IsController = console.isController,
                            Openings = opening,
                            UnitHallFixtureType = console.FixtureType,
                        };

                        var variableList = (from DataRow rows in dataSet.Tables[0].Rows
                                            select new
                                            {
                                                entranceConsoleId = Convert.ToInt32(rows[Constant.UNITHALLFIXTURECONSOLEID]),
                                                variableType = "",
                                                variablevalue = "",
                                                unitFixtureType = rows["FixtureType"].ToString(),
                                            }).Distinct();

                        DataColumnCollection columns = dataSet.Tables[0].Columns;
                        if (columns.Contains(Constant.VARIABLETYPE) && columns.Contains(Constant.VARIABLEVALUE))
                        {
                            var variableListValues = (from DataRow rows in dataSet.Tables[0].Rows
                                                      select new
                                                      {
                                                          entranceConsoleId = Convert.ToInt32(rows[Constant.UNITHALLFIXTURECONSOLEID]),
                                                          variableType = rows[Constant.VARIABLETYPE] != DBNull.Value ? rows[Constant.VARIABLETYPE].ToString() : "",
                                                          variablevalue = rows[Constant.VARIABLEVALUE] != DBNull.Value ? rows[Constant.VARIABLEVALUE]?.ToString() : "",
                                                          unitFixtureType = rows["FixtureType"].ToString(),
                                                      }).Distinct();
                            variableList = variableListValues;
                        }

                        variableList = variableList?.Where(x => x.unitFixtureType.Equals(unitHallFixtureConsole.UnitHallFixtureType)
                        && x.entranceConsoleId.Equals(unitHallFixtureConsole.ConsoleId)).Distinct().ToList();
                        List<ConfigVariable> variableAssignments = new List<ConfigVariable>();
                        foreach (var assignments in variableList)
                        {
                            ConfigVariable variableAssignment = new ConfigVariable()
                            {
                                VariableId = assignments.variableType,
                                Value = assignments.variablevalue
                            };
                            variableAssignments.Add(variableAssignment);
                        }
                        unitHallFixtureConsole.VariableAssignments = variableAssignments;
                        var entranceList = (from DataRow row in dataSet.Tables[0].Rows
                                            select new
                                            {
                                                entranceConsoleId = Convert.ToInt32(row[Constant.UNITHALLFIXTURECONSOLEID]),
                                                floorNumber = Convert.ToInt32(row[Constant.FLOORNUMBER]),
                                                floorDesignation = row[Constant.FLOORNUMBER].ToString(),
                                                front = Convert.ToBoolean(row[Constant.FRONT]),
                                                rear = Convert.ToBoolean(row[Constant.REAR]),
                                                openingFront = Convert.ToBoolean(row[Constant.OPENINGFRONT]),
                                                openingRear = Convert.ToBoolean(row[Constant.OPENINGREAR]),
                                                unitHallFixtureType = row["FixtureType"].ToString()
                                            }).Distinct();
                        entranceList = entranceList.Where(x => x.entranceConsoleId.Equals(unitHallFixtureConsole.ConsoleId) && x.unitHallFixtureType.Equals(unitHallFixtureConsole.UnitHallFixtureType)).Distinct().ToList();
                        List<EntranceLocations> entranceLocations = new List<EntranceLocations>();
                        foreach (var entrancelocation in entranceList)
                        {
                            EntranceLocations entranceLocation = new EntranceLocations()
                            {
                                FloorNumber = entrancelocation.floorNumber,
                                FloorDesignation = entrancelocation.floorDesignation
                            };
                            LandingOpening landingOpening = new LandingOpening()
                            {
                                InCompatible = false,
                                NotAvailable = !entrancelocation.openingFront,
                                Value = entrancelocation.front
                            };
                            entranceLocation.Front = landingOpening;
                            landingOpening = new LandingOpening()
                            {
                                InCompatible = false,
                                NotAvailable = !entrancelocation.openingRear,
                                Value = entrancelocation.rear
                            };
                            entranceLocation.Rear = landingOpening;
                            entranceLocations.Add(entranceLocation);
                        }
                        unitHallFixtureConsole.UnitHallFixtureLocations = entranceLocations;
                        unitHallFixtureConsoles.Add(unitHallFixtureConsole);
                    }
                    if (unitHallFixtureConsoles.Count > 0)
                    {
                        var opening = unitHallFixtureConsoles[0].Openings;
                        var lstlocation = new List<EntranceLocations>();
                        var varUnitHallFixtureLocation = unitHallFixtureConsoles[0].UnitHallFixtureLocations;
                        foreach (var location in varUnitHallFixtureLocation)
                        {
                            var varlocation = new EntranceLocations()
                            {
                                FloorNumber = location.FloorNumber,
                                FloorDesignation = location.FloorDesignation,
                                Front = new LandingOpening()
                                {
                                    Value = false,
                                    NotAvailable = location.Front.NotAvailable,
                                    InCompatible = false
                                },
                                Rear = new LandingOpening()
                                {
                                    Value = false,
                                    NotAvailable = location.Rear.NotAvailable,
                                    InCompatible = false
                                }
                            };
                            lstlocation.Add(varlocation);
                        }
                        UnitHallFixtures objEntranceConsole = new UnitHallFixtures()
                        {
                            ConsoleId = 0,
                            VariableAssignments = new List<ConfigVariable>(),
                            ConsoleName = "HallLanternConsole1",
                            IsController = false,
                            UnitHallFixtureLocations = lstlocation,
                            Openings = opening,
                            AssignOpenings = true
                        };
                        unitHallFixtureConsoles.Add(objEntranceConsole);
                    }
                    objUnitHallFixtureConfigurationData = unitHallFixtureConsoles;
                }
                if (dataSet.Tables[1].Rows.Count > 0)
                {
                    var dataPoint = (from DataRow row in dataSet.Tables[1].Rows
                                     select new
                                     {
                                         variableId = row[Constant.CONFIGVARIABLES].ToString(),
                                         value = row[Constant.CONFIGUREVALUES].ToString(),
                                         setId = row["setid"]
                                     }).Distinct().FirstOrDefault();
                    _cpqCacheManager.SetCache(sessionId, _environment, "HALLLANTERNVARIABLE", Utility.SerializeObjectValue(dataPoint));
                }
                if (dataSet.Tables[2].Rows.Count > 0)
                {
                    var lobbyRecallSwitchFlag = (from DataRow row in dataSet.Tables[2].Rows
                                                 select new
                                                 {
                                                     value = row["Value"].ToString()
                                                 }).Distinct().FirstOrDefault();
                    _cpqCacheManager.SetCache(sessionId, _environment, setId.ToString(), Constant.LOBBYRECALLSWITCHFLAG, Utility.SerializeObjectValue(lobbyRecallSwitchFlag));
                }
                if (dataSet.Tables[3].Rows.Count > 0)
                {
                    var CarRidingLanternQuantity = (from DataRow row in dataSet.Tables[3].Rows
                                                    select new
                                                    {
                                                        CarRidingLanternQuantity = row[Constants.CARRIDINGQUANTITYVALUE].ToString()
                                                    }).Distinct().ToList();
                    var Quantity = string.Empty;
                    foreach (var value in CarRidingLanternQuantity)
                    {
                        Quantity = value.CarRidingLanternQuantity;
                    }
                    if (Quantity.Equals(String.Empty))
                    {
                        Quantity = "1";
                    }
                    _cpqCacheManager.SetCache(sessionId, _environment, setId.ToString(), Constants.CARRIDINGLANTERNQUANT, Quantity);
                }
            }
            Utility.LogEnd(methodBegin);
            return objUnitHallFixtureConfigurationData;
        }

        /// <summary>
        /// GetDetailsForTP2SummaryScreen 
        /// </summary>
        /// <param Name="SetId"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public TP2Summary GetDetailsForTP2SummaryScreen(int setId)
        {
            var methodBegin = Utility.LogBegin();
            var unitMapperVariables = Utility.VariableMapper(Constant.UNITSVARIABLESMAPPERPATH, Constant.UNITMAPPER);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            TP2Summary variableAssignments = new TP2Summary()
            {
                ChangedData = new List<ChangeLog>(),
                QuoteSummary = new List<QuoteSummary>(),
                FloorMatrixTable = new List<PriceSectionDetails>(),
                CustomPriceLine = new List<CustomPriceLine>()
            };
            DataSet summaryScreenDataSet = new DataSet();
            var val = JArray.Parse(File.ReadAllText(Constant.LISTVARIABLEASSIGNMENTS));
            var mainVariableResObj = Utility.DeserializeObjectValue<List<VariablesList>>(Utility.SerializeObjectValue(val));
            SqlParameter param = new SqlParameter(Constant.SETID, setId);
            sqlParameters.Add(param);
            summaryScreenDataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETDETAILSFORTP2SUMMARY, sqlParameters);
            List<ConfigVariable> buildngConfig = new List<ConfigVariable>();
            List<ConfigVariable> groupVariables = new List<ConfigVariable>();
            List<ConfigVariable> unitVariable = new List<ConfigVariable>();
            List<UnitDetailsForTP2> unitDetailsForTP2 = new List<UnitDetailsForTP2>();
            List<OpeningVariables> openingsValues = new List<OpeningVariables>();
            if (summaryScreenDataSet != null && summaryScreenDataSet.Tables.Count > 0)
            {
                var buildingConfiguration = summaryScreenDataSet.Tables[0];
                if (buildingConfiguration.Rows.Count > 0)
                {
                    buildngConfig = JsonConvert.DeserializeObject<List<ConfigVariable>>(buildingConfiguration.Rows[0][Constant.BUILDINGJSON].ToString());
                }
                var groupConfiguration = summaryScreenDataSet.Tables[1];
                if (groupConfiguration.Rows.Count > 0)
                {
                    if (summaryScreenDataSet.Tables[1].Rows.Count > 0)
                    {
                        string variableAssignmentJsonString = string.Empty;
                        foreach (DataRow groupVariableAssignment in groupConfiguration.Rows)
                        {
                            if (!(Convert.ToString(groupVariableAssignment[Constant.UNITJSON])).Equals(Constant.EMPTYSTRING))
                            {
                                variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.UNITJSON]) + Constant.COMA;
                            }
                            if (!(Convert.ToString(groupVariableAssignment[Constant.HALLRISERJSON])).Equals(Constant.EMPTYSTRING))
                            {
                                variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.HALLRISERJSON]) + Constant.COMA;
                            }
                            if (!(Convert.ToString(groupVariableAssignment[Constant.DOORJSON])).Equals(Constant.EMPTYSTRING))
                            {
                                variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.DOORJSON]) + Constant.COMA;
                            }
                            if (!(Convert.ToString(groupVariableAssignment[Constant.CONTROLLOCATIONJSON])).Equals(Constant.EMPTYSTRING))
                            {
                                variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.CONTROLLOCATIONJSON]) + Constant.COMA;
                            }
                        }
                        string jsonData = Constant.OPENINGSQUAREBRACKET + variableAssignmentJsonString + Constant.CLOSINGSQUAREBRACKET;
                        groupVariables = JsonConvert.DeserializeObject<List<ConfigVariable>>(jsonData.Replace(Constant.COMA + Constant.CLOSINGSQUAREBRACKET, Constant.CLOSINGSQUAREBRACKET).ToString());
                        groupVariables = groupVariables.GroupBy(group => group.VariableId).Select(g => g.First()).ToList();
                    }
                }
                var unitConfiguration = summaryScreenDataSet.Tables[2];
                if (unitConfiguration.Rows.Count > 0)
                {
                    //create Unit configuration list
                    foreach (DataRow unitConfigurationData in unitConfiguration.Rows)
                    {
                        string unitConfigurationJson = Convert.ToString(unitConfigurationData[Constant.CONFIGUREJSON]);
                        var unitConfig = Utility.DeserializeObjectValue<ConfigVariable>(unitConfigurationJson);
                        unitVariable.Add(unitConfig);
                        openingsValues.Add(new OpeningVariables
                        {
                            VariableAssigned = unitConfig,
                            TotalOpenings = Convert.ToInt32(unitConfigurationData[Constant.TOTALOPENINGS])
                        });
                    }
                }
                var unitDetails = summaryScreenDataSet.Tables[3];
                if (unitDetails.Rows.Count > 0)
                {
                    //create Unit configuration list
                    foreach (DataRow unit in unitDetails.Rows)
                    {
                        unitDetailsForTP2.Add(new UnitDetailsForTP2()
                        {
                            UnitId = Convert.ToInt32((unit[Constant.UNITID]).ToString()),
                            UnitName = (unit[Constant.DESIGNATION]).ToString(),
                            Ueid = (unit[Constant.UEID_LOWERCASE]).ToString(),
                            ProductName = (unit[Constant.PRODUCTNAM]).ToString(),
                            Status = (unit[Constant.STATUS]).ToString(),
                            FactoryJobID = Convert.ToString(unit[Constants.FACTORYJOBID])
                        });
                    }
                }
                unitVariable.Add(new ConfigVariable { VariableId = unitMapperVariables[Constant.TOTALUNITSINPANEL], Value = unitDetails.Rows.Count });
                var groupId = summaryScreenDataSet.Tables[4];
                if (groupId.Rows.Count > 0)
                {
                    foreach (DataRow groupid in groupId.Rows)
                    {
                        variableAssignments.GroupId = Convert.ToInt32((groupid[Constant.GROUPCONFIGIDVALUE]).ToString());
                    }
                }

                var travel = summaryScreenDataSet.Tables[5];
                if (travel.Rows.Count > 0)
                {
                    var travelVariableAssignments = new VariableAssignment();
                    travelVariableAssignments.VariableId = unitMapperVariables[Constant.TRAVELVARIABLEIDVALUE];
                    int travelFeet = 0;
                    decimal travelinch = 0;
                    foreach (DataRow travelFeetAndInch in travel.Rows)
                    {
                        if (!(Convert.ToString(travelFeetAndInch[Constant.TRAVELFEET])).Equals(Constant.EMPTYSTRING))
                        {
                            travelFeet = Convert.ToInt32(travelFeetAndInch[Constant.TRAVELFEET]);
                        }
                        if (!(Convert.ToString(travelFeetAndInch[Constant.TRAVELINCH])).Equals(Constant.EMPTYSTRING))
                        {
                            travelinch = Convert.ToInt32(travelFeetAndInch[Constant.TRAVELINCH]);
                        }

                    }
                    travelVariableAssignments.Value = travelFeet * 12 + travelinch;
                    variableAssignments.TravelVariableAssignments = travelVariableAssignments;
                    unitVariable.Add(new ConfigVariable { VariableId = travelVariableAssignments.VariableId, Value = travelVariableAssignments.Value });
                }

                var groupHallFixtureData = summaryScreenDataSet.Tables[6];
                List<OpeningVariables> OpeningVariableAssginments = new List<OpeningVariables>();
                if (groupHallFixtureData.Rows.Count > 0)
                {
                    List<ConfigVariable> lstVariables = new List<ConfigVariable>();
                    foreach (DataRow hallFixtureData in groupHallFixtureData.Rows)
                    {
                        ConfigVariable variable = new ConfigVariable();
                        variable.VariableId = Convert.ToString(hallFixtureData[Constant.VARIABLETYPE]);
                        variable.Value = Convert.ToString(hallFixtureData[Constant.VARIABLEVALUE]);
                        lstVariables.Add(variable);
                    }
                    var lstUniqueVariables = lstVariables.GroupBy(x => x.Value).Select(y => y.First()).ToList();
                    foreach (var var2 in lstUniqueVariables)
                    {
                        List<UnitDetailsValues> unitValues = new List<UnitDetailsValues>();
                        unitVariable.Add(var2);
                        var getunitGroupConsoleList = (from DataRow row in summaryScreenDataSet.Tables[6].Rows
                                                       select new
                                                       {
                                                           VariableId = Convert.ToString(row[Constant.VARIABLETYPE]),
                                                           Value = Convert.ToString(row[Constant.VARIABLEVALUE]),
                                                           unitId = Convert.ToInt32(row[Constant.UNITID]),
                                                           UnitDesignation = Convert.ToString(row[Constant.NAMECOLUMN]),
                                                           floorDesignation = row[Constant.FLOORDESIGNATION].ToString(),
                                                           front = Convert.ToBoolean(row[Constant.FRONT]),
                                                           rear = Convert.ToBoolean(row[Constant.REAR])
                                                       });
                        var openings = (from opening in getunitGroupConsoleList
                                        where opening.VariableId.Equals(var2.VariableId) && opening.Value.Equals(var2.Value)
                                        select opening).ToList();
                        var frontSelectedOpenings = (from opening in openings
                                                     where opening.front.Equals(true)
                                                     select opening).ToList().Count();
                        var rearSelectedOpenings = (from opening in openings
                                                    where opening.rear.Equals(true)
                                                    select opening).ToList().Count();
                        var unitsList = (from units in openings
                                         select units.unitId).Distinct().ToList();
                        foreach (var unit in unitsList)
                        {
                            List<GroupHallFixtureLocations> ghfLocations = new List<GroupHallFixtureLocations>();
                            var openingsPerUnit = (from opening in openings
                                                   where opening.unitId.Equals(unit)
                                                   select opening).Distinct().ToList();
                            UnitDetailsValues udv = new UnitDetailsValues();

                            udv.UnitId = Convert.ToInt32(unit);
                            udv.UniDesgination = openingsPerUnit[0].UnitDesignation.ToString();
                            foreach (var opening in openingsPerUnit)
                            {
                                GroupHallFixtureLocations groupHallLocation = new GroupHallFixtureLocations()
                                {
                                    FloorNumber = Convert.ToInt32(Regex.Match(opening.floorDesignation, @"\d+").Value),
                                    FloorDesignation = opening.floorDesignation
                                };
                                LandingOpening landingOpening = new LandingOpening()
                                {
                                    InCompatible = false,
                                    NotAvailable = !opening.front,
                                    Value = opening.front
                                };
                                groupHallLocation.Front = landingOpening;
                                landingOpening = new LandingOpening()
                                {
                                    InCompatible = false,
                                    NotAvailable = !opening.front,
                                    Value = opening.rear
                                };
                                groupHallLocation.Rear = landingOpening;
                                ghfLocations.Add(groupHallLocation);
                            }
                            udv.UnitGroupValues = ghfLocations;
                            unitValues.Add(udv);
                        }
                        var openingString = TKE.SC.BFF.BusinessProcess.Helpers.Utility.GetLandingOpeningAssignmentSelectedForGroupHallFixture(unitValues);
                        openingsValues.Add(new OpeningVariables
                        {
                            VariableAssigned = var2,
                            OpeningsAssigned = openingString,
                            TotalOpenings = var2.VariableId.Contains(Constant.EMPIVARIABLE) ? unitValues.Count() : frontSelectedOpenings + rearSelectedOpenings
                        });
                    }
                }

                var unitHallFixtureData = summaryScreenDataSet.Tables[7];
                if (unitHallFixtureData.Rows.Count > 0)
                {
                    List<ConfigVariable> lstVariables = new List<ConfigVariable>();
                    foreach (DataRow hallFixtureData in unitHallFixtureData.Rows)
                    {
                        ConfigVariable variable = new ConfigVariable();
                        variable.VariableId = Convert.ToString(hallFixtureData[Constant.VARIABLETYPE]);
                        variable.Value = Convert.ToString(hallFixtureData[Constant.VARIABLEVALUE]);
                        lstVariables.Add(variable);
                    }
                    var lstUniqueVariables = lstVariables.GroupBy(x => new { x.VariableId, x.Value }).Select(y => y.First()).ToList();
                    foreach (var var2 in lstUniqueVariables)
                    {
                        if (var2.Value.Equals(Constant.TRUE_LOWERCASE))
                        {
                            var2.Value = Constant.TRUE_UPPERCASE;
                        }
                        unitVariable.Add(var2);
                        var getunitGroupConsoleList = (from DataRow row in summaryScreenDataSet.Tables[7].Rows
                                                       select new
                                                       {
                                                           VariableId = Convert.ToString(row[Constant.VARIABLETYPE]),
                                                           Value = Convert.ToString(row[Constant.VARIABLEVALUE]),
                                                           floorNumber = row[Constant.FLOORNUMBER].ToString(),
                                                           front = Convert.ToBoolean(row[Constant.FRONT]),
                                                           rear = Convert.ToBoolean(row[Constant.REAR])
                                                       });
                        var Openings = (from openings in getunitGroupConsoleList
                                        where openings.VariableId.Equals(var2.VariableId) && openings.Value.Equals(var2.Value)
                                        select openings).ToList();
                        var frontSelectedOpenings = (from opening in Openings
                                                     where opening.front.Equals(true)
                                                     select opening).ToList().Count();
                        var rearSelectedOpenings = (from opening in Openings
                                                    where opening.rear.Equals(true)
                                                    select opening).ToList().Count();
                        List<EntranceLocations> listOfLocation = new List<EntranceLocations>();
                        foreach (var opening in Openings)
                        {
                            EntranceLocations entranceLocation = new EntranceLocations()
                            {
                                FloorNumber = Convert.ToInt32(opening.floorNumber),
                                FloorDesignation = opening.floorNumber
                            };
                            LandingOpening landingOpening = new LandingOpening()
                            {
                                InCompatible = false,
                                NotAvailable = !opening.front,
                                Value = opening.front
                            };
                            entranceLocation.Front = landingOpening;
                            landingOpening = new LandingOpening()
                            {
                                InCompatible = false,
                                NotAvailable = !opening.rear,
                                Value = opening.rear
                            };
                            entranceLocation.Rear = landingOpening;
                            listOfLocation.Add(entranceLocation);
                        }
                        var openingString = TKE.SC.BFF.BusinessProcess.Helpers.Utility.GetLandingOpeningAssignmentSelectedForUnitHallFixture(listOfLocation);

                        foreach (var opening in listOfLocation)
                        {
                            if (opening.Front.Value.Equals(true) || opening.Rear.Value.Equals(true))
                            {
                                var id = var2.VariableId.Split(Constant.DOT).ToList().Skip(3);
                                ConfigVariable newVar = new ConfigVariable();
                                newVar.VariableId = "ELEVATOR.FloorMatrix.LANDING#" + Constant.DOT + String.Join(Constant.DOT, id);
                                newVar.Value = var2.Value;
                                var strFloornumber = opening.FloorNumber.ToString().PadLeft(3, '0');
                                newVar.VariableId = newVar.VariableId.Replace("#", strFloornumber);
                                openingsValues.Add(new OpeningVariables
                                {
                                    VariableAssigned = newVar,
                                    TotalOpenings = newVar.VariableId.Contains(Constant.RETURNVARIABLE) ? unitDetails.Rows.Count : frontSelectedOpenings + rearSelectedOpenings
                                });
                            }
                        }

                        openingsValues.Add(new OpeningVariables
                        {
                            VariableAssigned = var2,
                            OpeningsAssigned = openingString,
                            TotalOpenings = var2.VariableId.Contains(Constant.RETURNVARIABLE) ? unitDetails.Rows.Count : frontSelectedOpenings + rearSelectedOpenings
                        });
                    }
                }

                var entranceData = summaryScreenDataSet.Tables[8];
                if (entranceData.Rows.Count > 0)
                {
                    List<ConfigVariable> lstVariables = new List<ConfigVariable>();
                    foreach (DataRow hallFixtureData in entranceData.Rows)
                    {
                        ConfigVariable variable = new ConfigVariable();
                        variable.VariableId = Convert.ToString(hallFixtureData[Constant.VARIABLETYPE]);
                        variable.Value = Convert.ToString(hallFixtureData[Constant.VARIABLEVALUE]);
                        lstVariables.Add(variable);
                    }
                    var lstUniqueVariables = lstVariables.GroupBy(x => x.VariableId).Select(y => y.First()).ToList();
                    foreach (var var2 in lstUniqueVariables)
                    {
                        if (var2.Value.Equals(Constant.TRUE_LOWERCASE))
                        {
                            var2.Value = Constant.TRUE_UPPERCASE;
                        }
                        unitVariable.Add(var2);
                        var getunitGroupConsoleList = (from DataRow row in summaryScreenDataSet.Tables[8].Rows
                                                       select new
                                                       {
                                                           VariableId = Convert.ToString(row[Constant.VARIABLETYPE]),
                                                           Value = Convert.ToString(row[Constant.VARIABLEVALUE]),
                                                           floorNumber = row[Constant.FLOORNUMBER].ToString(),
                                                           front = Convert.ToBoolean(row[Constant.FRONT]),
                                                           rear = Convert.ToBoolean(row[Constant.REAR])
                                                       });
                        var Openings = (from openings in getunitGroupConsoleList
                                        where openings.VariableId.Equals(var2.VariableId) && openings.Value.Equals(var2.Value)
                                        select openings).ToList();
                        var frontSelectedOpenings = (from opening in Openings
                                                     where opening.front.Equals(true)
                                                     select opening).ToList().Count();
                        var rearSelectedOpenings = (from opening in Openings
                                                    where opening.rear.Equals(true)
                                                    select opening).ToList().Count();
                        List<EntranceLocations> listOfLocation = new List<EntranceLocations>();
                        foreach (var opening in Openings)
                        {
                            EntranceLocations entranceLocation = new EntranceLocations()
                            {
                                FloorNumber = Convert.ToInt32(opening.floorNumber),
                                FloorDesignation = opening.floorNumber
                            };
                            LandingOpening landingOpening = new LandingOpening()
                            {
                                InCompatible = false,
                                NotAvailable = !opening.front,
                                Value = opening.front
                            };
                            entranceLocation.Front = landingOpening;
                            landingOpening = new LandingOpening()
                            {
                                InCompatible = false,
                                NotAvailable = !opening.rear,
                                Value = opening.rear
                            };
                            entranceLocation.Rear = landingOpening;
                            listOfLocation.Add(entranceLocation);
                        }
                        var openingString = TKE.SC.BFF.BusinessProcess.Helpers.Utility.GetLandingOpeningAssignmentSelected(listOfLocation);
                        openingsValues.Add(new OpeningVariables { VariableAssigned = var2, OpeningsAssigned = openingString, TotalOpenings = frontSelectedOpenings + rearSelectedOpenings });
                    }
                }

                var projectData = summaryScreenDataSet.Tables[9];
                if (projectData.Rows.Count > 0)
                {
                    var projectsData = (from DataRow row in projectData.Rows
                                        select new
                                        {
                                            ProjectName = Convert.ToString(row[Constant.NAMECOLUMN]),
                                            ProjectId = Convert.ToString(row[Constant.PROJECTOPPORTUNITYID]),
                                            Source = Convert.ToString(row[Constant.SOURCE]),
                                            Status = Convert.ToString(row[Constant.STATUS]),
                                            Branch = Convert.ToString(row[Constant.BRANCH]),
                                            BldName = Convert.ToString(row[Constant.BUILDINGNAME]),
                                            VersionId = Convert.ToString(row[Constant.VERSIONID]),
                                            QuoteId = Convert.ToString(row[Constant.PROJECTQUOTEID]),
                                            GroupName = Convert.ToString(row[Constant.GRPNAME]),
                                            Designation = Convert.ToString(row[Constant.DESIGNATION]),
                                            FrontOpening = Convert.ToString(row[Constant.FRONTOPENING]),
                                            RearOpening = Convert.ToString(row[Constant.REAROPENING]),
                                            Travel = Convert.ToString(row[Constant.TRAVEL]),
                                            ProjectStatus = Convert.ToString(row[Constant.PROJECTSTATUSCOLUMNNAME])
                                        }).ToList();
                    variableAssignments.projectInfo = new ProjectInfo()
                    {
                        ProjectName = projectsData[0].ProjectName,
                        ProjectId = projectsData[0].ProjectId,
                        Branch = projectsData[0].Branch,
                        BuildingName = projectsData[0].BldName,
                        QuoteVersion = projectsData[0].VersionId,
                        PrimarySalesRep = string.Empty,
                        GroupName = projectsData[0].GroupName,
                        OracleProjectId = string.Empty,
                        PrimaryCoordinator = string.Empty,
                        UnitName = new List<string>(),
                        UnitMFGJobNo = string.Empty,
                        Status = projectsData[0].Status,
                        Source = projectsData[0].Source,
                        FrontOpenings = projectsData[0].FrontOpening,
                        RearOpenings = projectsData[0].RearOpening,
                        Travel = projectsData[0].Travel,
                        QuoteId = projectsData[0].QuoteId
                    };
                    foreach (var unit in projectsData)
                    {
                        variableAssignments.projectInfo.UnitName.Add(unit.Designation);
                    }
                    //foreach (DataRow projectDataRow in projectData.Rows)
                    //{

                    //    ConfigVariable variable = new ConfigVariable();
                    //    variable.VariableId = Convert.ToString(hallFixtureData["VariableType"]);
                    //    variable.Value = Convert.ToString(hallFixtureData["VariableValue"]);
                    //    lstVariables.Add(variable);
                    //}
                }

                var buildingEquipmentData = summaryScreenDataSet.Tables[10];
                if (buildingEquipmentData.Rows.Count > 0)
                {
                    foreach (DataRow equipmentVar in buildingEquipmentData.Rows)
                    {
                        ConfigVariable variable = new ConfigVariable();
                        variable.VariableId = Convert.ToString(equipmentVar[Constant.VARIABLETYPE]).Replace(Constant.BUILDING_CONFIGURATION, Constant.ELEVATOR_CONFIGURATION);
                        variable.Value = Convert.ToString(equipmentVar[Constant.FDAXMLVALUE]);
                        if (int.TryParse(variable.Value.ToString(), out int result))
                        {
                            openingsValues.Add(new OpeningVariables { VariableAssigned = variable, TotalOpenings = Convert.ToInt32(variable.Value) });
                        }

                    }
                }

                var carCallCutoutSwitches = summaryScreenDataSet.Tables[11];
                if (carCallCutoutSwitches.Rows.Count > 0)
                {
                    foreach (DataRow equipmentVar in carCallCutoutSwitches.Rows)
                    {
                        var parameter = (from variables in unitVariable
                                         where variables.VariableId.Contains(Constant.CARCALLLOCKOUTVARIABLEID)
                                         select variables).ToList();
                        if (!String.IsNullOrEmpty(Convert.ToString(equipmentVar[Constant.CARCALLCUTOUTCOLUMN])) && parameter.Count > 0)
                        {
                            ConfigVariable variable = new ConfigVariable();
                            variable = parameter.FirstOrDefault();
                            var setFlag = 0;
                            foreach (var openingAssignment in openingsValues)
                            {
                                if (openingAssignment.VariableAssigned.VariableId.Contains(Constant.CARCALLLOCKOUTVARIABLEID))
                                {
                                    openingAssignment.TotalOpenings = Convert.ToInt32(equipmentVar[Constant.CARCALLCUTOUTCOLUMN]);
                                    setFlag = 1;
                                }
                            }
                            if (setFlag == 0)
                            {
                                openingsValues.Add(new OpeningVariables { VariableAssigned = variable, TotalOpenings = Convert.ToInt32(equipmentVar[Constant.CARCALLCUTOUTCOLUMN]) });
                            }

                        }


                    }
                }

                var changeLog = summaryScreenDataSet.Tables[12];
                variableAssignments.ChangedData.AddRange(GetChangeLogData(changeLog));
                changeLog = summaryScreenDataSet.Tables[13];
                variableAssignments.ChangedData.AddRange(GetChangeLogData(changeLog));
                changeLog = summaryScreenDataSet.Tables[14];
                variableAssignments.ChangedData.AddRange(GetChangeLogData(changeLog));
                var quoteSummary = summaryScreenDataSet.Tables[15];
                variableAssignments.QuoteSummary.AddRange(GetQuoteSummary(quoteSummary));
                var projectInfo = summaryScreenDataSet.Tables[16];
                variableAssignments.ProjectData = GetProjectData(projectInfo);

                var unitInfoOfGroup = summaryScreenDataSet.Tables[17];
                if (unitInfoOfGroup.Rows.Count > 0)
                {
                    variableAssignments.GroupUnitInfo = new List<UnitNames>();
                    foreach (DataRow unitInfo in unitInfoOfGroup.Rows)
                    {
                        UnitNames unitInfoDetails = new UnitNames
                        {
                            Unitname = unitInfo[Constant.NAME].ToString(),
                            Unitid = Convert.ToInt32(unitInfo[Constant.UNITID]),
                            Ueid = unitInfo[Constant.UEID].ToString(),
                            SetId = Convert.ToInt32(unitInfo[Constant.SETCONFIGURATIONID])
                        };
                        variableAssignments.GroupUnitInfo.Add(unitInfoDetails);
                    }
                }

                var elevationData = summaryScreenDataSet.Tables[19];
                if (elevationData.Rows.Count > 0)
                {
                    foreach (DataRow elevationInfo in elevationData.Rows)
                    {
                        if (Convert.ToBoolean(elevationInfo[Constant.MAINEGRESS]))
                        {
                            variableAssignments.MainEgress = Convert.ToString(elevationInfo[Constant.FLOORNUMBER]) + Constant.OPENINGBRACKET
                                + Convert.ToString(elevationInfo[Constant.FLOORDESIGNATION]) + Constant.CLOSINGINGBRACKET;
                        }
                        else
                        {
                            variableAssignments.AlternateEgress = Convert.ToString(elevationInfo[Constant.FLOORNUMBER]) + Constant.OPENINGBRACKET
                                + Convert.ToString(elevationInfo[Constant.FLOORDESIGNATION]) + Constant.CLOSINGINGBRACKET;
                        }
                    }
                }
                elevationData = summaryScreenDataSet.Tables[20];
                if (elevationData.Rows.Count > 0)
                {
                    var floorDetails = (from DataRow dRow in elevationData.Rows
                                        select new
                                        {
                                            FloorNumber = Convert.ToInt32(dRow[Constant.FLOORNUMBER]),
                                            FloorDesignation = Convert.ToString(dRow[Constant.FLOORDESIGNATION]),
                                            FloorToFloorHeightFeet = Convert.ToString(dRow[Constant.FLOORTOFLOORHEIGHTFEET]),
                                            FloorToFloorHeightInch = Convert.ToString(dRow[Constant.FLOORTOFLOORHEIGHTINCH])
                                        }).Distinct();
                    variableAssignments.TopFloor = floorDetails.Max(x => x.FloorNumber);
                    var entranceConsoleData = summaryScreenDataSet.Tables[21];
                    var floorMatrixTableVariables = Utility.VariableMapper(Constant.UNITSVARIABLESMAPPERPATH, Constant.DOCUMENTGENERATION);
                    if (entranceConsoleData.Rows.Count > 0)
                    {
                        var entranceConsoleList = (from DataRow dRow in entranceConsoleData.Rows
                                                   select new
                                                   {
                                                       EntranceConsoleId = Convert.ToInt32(dRow[Constant.ENTRANCECONSOLEID]),
                                                       VariableType = Convert.ToString(dRow[Constant.VARIABLETYPE]),
                                                       VariableValue = Convert.ToString(dRow[Constant.VARIABLEVALUE]),
                                                       FloorNumber = Convert.ToInt32(dRow[Constant.FLOORNUMBER]),
                                                       Front = Convert.ToBoolean(dRow[Constant.FRONT]),
                                                       Rear = Convert.ToBoolean(dRow[Constant.REAR])
                                                   }).Distinct();
                        var generalFloorProperties = new PriceSectionDetails()
                        {
                            Name = Constant.FLOORMATRIXGENERALFLOORPROPERTIES,
                            Section = Convert.ToString(1),
                            Id = Constant.FLOORMATRIX,
                            PriceKeyInfo = new List<PriceValuesDetails>()
                        };
                        foreach (var floor in floorDetails)
                        {
                            var entranceConsoleListForFloor = entranceConsoleList.Where(x => x.FloorNumber == floor.FloorNumber).ToList().Distinct();
                            var wallThicknessFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.WALLTHICKNESSFRONT])).ToList();
                            var wallThicknessRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.WALLTHICKNESSREAR])).ToList();
                            var frameConstructionFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.FRAMECONSTRUCTIONFRONT])).ToList();
                            var frameConstructionRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.FRAMECONSTRUCTIONREAR])).ToList();
                            var faceOfFrameFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.FACEOFFRAMEFRONT])).ToList();
                            var faceOfFrameRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.FACEOFFRAMEREAR])).ToList();
                            var floorData = new PriceValuesDetails()
                            {
                                Section = Convert.ToString(floor.FloorNumber),
                                ItemNumber = floor.FloorDesignation,
                                PartDescription = floor.FloorDesignation,
                                ComponentName = floor.FloorToFloorHeightFeet + Constant.APOSTROPHE + Constant.HYPHEN.ToString() + floor.FloorToFloorHeightInch + Constant.APOSTROPHE + Constant.APOSTROPHE,
                                qty = 0,
                                Parameter3 = wallThicknessFront.Any() ? wallThicknessFront[0].VariableValue : Constant.HYPHEN.ToString(),
                                Parameter3Value = wallThicknessRear.Any() ? wallThicknessRear[0].VariableValue : Constant.HYPHEN.ToString(),
                                LeadTime = frameConstructionFront.Any() ? frameConstructionFront[0].VariableValue : Constant.HYPHEN.ToString(),//FrameConstructionFront
                                BatchNo = frameConstructionRear.Any() ? frameConstructionRear[0].VariableValue : Constant.HYPHEN.ToString(),//FrameConstructionRear
                                Parameter4 = faceOfFrameFront.Any() ? faceOfFrameFront[0].VariableValue : Constant.HYPHEN.ToString(),//FaceOfFrameFront
                                Parameter4Value = faceOfFrameRear.Any() ? faceOfFrameRear[0].VariableValue : Constant.HYPHEN.ToString()//FaceOfFrameRear
                            };
                            generalFloorProperties.PriceKeyInfo.Add(floorData);
                        }
                        variableAssignments.FloorMatrixTable.Add(generalFloorProperties);
                        var entranceFrames = new PriceSectionDetails()
                        {
                            Name = Constant.FLOORMATRIXENTRANCEFRAME,
                            Section = Convert.ToString(1),
                            Id = Constant.FLOORMATRIX,
                            PriceKeyInfo = new List<PriceValuesDetails>()
                        };
                        foreach (var floor in floorDetails)
                        {
                            var entranceConsoleListForFloor = entranceConsoleList.Where(x => x.FloorNumber == floor.FloorNumber).ToList().Distinct();
                            var frameFinishFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.FRAMEFINISHFRONT])).ToList();
                            var frameFinishRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.FRAMEFINISHREAR])).ToList();
                            var floorData = new PriceValuesDetails()
                            {
                                Section = Convert.ToString(floor.FloorNumber),
                                ItemNumber = frameFinishFront.Any() ? frameFinishFront[0].VariableValue : "-",//FrameFinishFront
                                PartDescription = frameFinishRear.Any() ? frameFinishRear[0].VariableValue : "-",//FrameFinishRear
                                ComponentName = frameFinishRear.Any() ? frameFinishRear[0].VariableValue : "-",//FrameFinishRear
                                qty = 0,
                                Parameter3 = Constant.HYPHEN.ToString(),//FrameFinishColorFront
                                Parameter3Value = Constant.HYPHEN.ToString(),//FrameFinishColorRear
                            };
                            entranceFrames.PriceKeyInfo.Add(floorData);
                        }
                        variableAssignments.FloorMatrixTable.Add(entranceFrames);



                        var hoistwaySills = new PriceSectionDetails()
                        {
                            Name = Constant.FLOORMATRIXHOISTWAYSILLS,
                            Section = Convert.ToString(1),
                            Id = Constant.FLOORMATRIX,
                            PriceKeyInfo = new List<PriceValuesDetails>()
                        };
                        foreach (var floor in floorDetails)
                        {
                            var entranceConsoleListForFloor = entranceConsoleList.Where(x => x.FloorNumber == floor.FloorNumber).ToList().Distinct();
                            var hoistwaySillsFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HOISTWAYSILLSFRONT])).ToList();
                            var hoistwaySillsRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HOISTWAYSILLSREAR])).ToList();
                            var floorData = new PriceValuesDetails()
                            {
                                Section = Convert.ToString(floor.FloorNumber),
                                ItemNumber = Constant.HYPHEN.ToString() + Constant.HYPHEN.ToString(),
                                PartDescription = hoistwaySillsRear.Any() ? hoistwaySillsRear[0].VariableValue : Constant.HYPHEN.ToString(),
                                ComponentName = hoistwaySillsFront.Any() ? hoistwaySillsFront[0].VariableValue : Constant.HYPHEN.ToString(),
                                qty = 0,
                                SectionName = Constant.HYPHEN.ToString(),
                                Component = Constant.HYPHEN.ToString(),
                            };
                            hoistwaySills.PriceKeyInfo.Add(floorData);
                        }
                        variableAssignments.FloorMatrixTable.Add(hoistwaySills);
                    }
                    var hallStationsData = summaryScreenDataSet.Tables[22];
                    if (hallStationsData.Rows.Count > 0)
                    {
                        var entranceConsoleList = (from DataRow dRow in hallStationsData.Rows
                                                   select new
                                                   {
                                                       EntranceConsoleId = Convert.ToInt32(dRow[Constant.ENTRANCECONSOLEID]),
                                                       VariableType = Convert.ToString(dRow[Constant.VARIABLETYPE]),
                                                       VariableValue = Convert.ToString(dRow[Constant.VARIABLEVALUE]),
                                                       FloorNumber = Convert.ToInt32(dRow[Constant.FLOORNUMBER]),
                                                       Front = Convert.ToBoolean(dRow[Constant.FRONT]),
                                                       Rear = Convert.ToBoolean(dRow[Constant.REAR])
                                                   }).Distinct();
                        variableAssignments.NoOfInconRisersFront = entranceConsoleList.Where(x => x.Front && x.VariableType.EndsWith(Constant.INCONFFRONT)).ToList().Count();
                        variableAssignments.NoOfInconRisersRear = entranceConsoleList.Where(x => x.Front && x.VariableType.EndsWith(Constant.INCONFREAR)).ToList().Count();

                        var hoistwayAccess = new PriceSectionDetails()
                        {
                            Name = Constant.FLOORMATRIXHALLSTATIONS,
                            Section = Convert.ToString(1),
                            Id = Constant.FLOORMATRIX,
                            PriceKeyInfo = new List<PriceValuesDetails>()
                        };
                        foreach (var floor in floorDetails)
                        {
                            var entranceConsoleListForFloor = entranceConsoleList.Where(x => x.FloorNumber == floor.FloorNumber).ToList().Distinct();
                            var hallLanterTypeFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALOCTOPF])).ToList();
                            var hallLanterTypeRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALOCTOPF])).ToList();
                            var floorData = new PriceValuesDetails()
                            {
                                Section = Convert.ToString(floor.FloorNumber),
                                ItemNumber = Constant.HYPHEN.ToString() + Constant.HYPHEN.ToString(),
                                PartDescription = hallLanterTypeFront.Any() ? hallLanterTypeFront[0].VariableValue : Constant.HYPHEN.ToString(),
                                ComponentName = hallLanterTypeRear.Any() ? hallLanterTypeRear[0].VariableValue : Constant.HYPHEN.ToString(),
                                qty = 0
                            };
                            hoistwayAccess.PriceKeyInfo.Add(floorData);
                        }
                        variableAssignments.FloorMatrixTable.Add(hoistwayAccess);

                        var hallLanterns = new PriceSectionDetails()
                        {
                            Name = Constant.FLOORMATRIXHALLLANTERN,
                            Section = Convert.ToString(1),
                            Id = Constant.FLOORMATRIX,
                            PriceKeyInfo = new List<PriceValuesDetails>()
                        };
                        foreach (var floor in floorDetails)
                        {
                            var entranceConsoleListForFloor = entranceConsoleList.Where(x => x.FloorNumber == floor.FloorNumber).ToList().Distinct();
                            var hallLanterTypeFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALLLANTERNTYPEFRONT])).ToList();
                            var hallLanterTypeRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALLLANTERNTYPEREAR])).ToList();
                            var floorData = new PriceValuesDetails()
                            {
                                Section = Convert.ToString(floor.FloorNumber),
                                ItemNumber = Constant.HYPHEN.ToString() + Constant.HYPHEN.ToString(),
                                PartDescription = hallLanterTypeFront.Any() ? hallLanterTypeFront[0].VariableValue : Constant.HYPHEN.ToString(),
                                ComponentName = hallLanterTypeRear.Any() ? hallLanterTypeRear[0].VariableValue : Constant.HYPHEN.ToString(),
                                qty = 0,
                                SectionName = Constant.HYPHEN.ToString(),
                                Component = Constant.HYPHEN.ToString(),
                            };
                            hallLanterns.PriceKeyInfo.Add(floorData);
                        }
                        variableAssignments.FloorMatrixTable.Add(hallLanterns);



                        var hallCombo = new PriceSectionDetails()
                        {
                            Name = Constant.FLOORMATRIXCOMBO,
                            Section = Convert.ToString(1),
                            Id = Constant.FLOORMATRIX,
                            PriceKeyInfo = new List<PriceValuesDetails>()
                        };
                        foreach (var floor in floorDetails)
                        {
                            var entranceConsoleListForFloor = entranceConsoleList.Where(x => x.FloorNumber == floor.FloorNumber).ToList().Distinct();
                            var comboFinishFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALLLANTERNTYPEFRONT])).ToList();
                            var comboFinishRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALLLANTERNTYPEREAR])).ToList();
                            var piColorFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALLLANTERNTYPEFRONT])).ToList();
                            var piColorRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALLLANTERNTYPEREAR])).ToList();
                            var lanternShapeFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALLLANTERNTYPEFRONT])).ToList();
                            var lanternShapeRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALLLANTERNTYPEREAR])).ToList();

                            var floorData = new PriceValuesDetails()
                            {
                                Section = Convert.ToString(floor.FloorNumber),
                                PartDescription = comboFinishFront.Any() ? comboFinishFront[0].VariableValue : Constant.HYPHEN.ToString(),
                                ComponentName = comboFinishRear.Any() ? comboFinishRear[0].VariableValue : Constant.HYPHEN.ToString(),
                                qty = 0,
                                LeadTime = piColorFront.Any() ? piColorFront[0].VariableValue : Constant.HYPHEN.ToString(),
                                ItemNumber = piColorRear.Any() ? piColorRear[0].VariableValue : Constant.HYPHEN.ToString(),
                                Parameter3 = lanternShapeFront.Any() ? lanternShapeFront[0].VariableValue : Constant.HYPHEN.ToString(),
                                Parameter3Value = lanternShapeRear.Any() ? lanternShapeRear[0].VariableValue : Constant.HYPHEN.ToString()
                            };
                            hallCombo.PriceKeyInfo.Add(floorData);
                        }
                        variableAssignments.FloorMatrixTable.Add(hallCombo);



                        var hallPIFinish = new PriceSectionDetails()
                        {
                            Name = Constant.FLOORMATRIXPI,
                            Section = Convert.ToString(1),
                            Id = Constant.FLOORMATRIX,
                            PriceKeyInfo = new List<PriceValuesDetails>()
                        };
                        foreach (var floor in floorDetails)
                        {
                            var entranceConsoleListForFloor = entranceConsoleList.Where(x => x.FloorNumber == floor.FloorNumber).ToList().Distinct();
                            var hallLanterTypeFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALLLANTERNTYPEFRONT])).ToList();
                            var hallLanterTypeRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALLLANTERNTYPEREAR])).ToList();
                            var hallPIcolorFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.PICOLOR])).ToList();
                            var hallPIcolorRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.PICOLOR])).ToList();

                            var floorData = new PriceValuesDetails()
                            {
                                Section = Convert.ToString(floor.FloorNumber),
                                PartDescription = hallLanterTypeFront.Any() ? hallLanterTypeFront[0].VariableValue : Constant.HYPHEN.ToString(),
                                ComponentName = hallLanterTypeRear.Any() ? hallLanterTypeRear[0].VariableValue : Constant.HYPHEN.ToString(),
                                qty = 0,
                                LeadTime = hallPIcolorFront.Any() ? hallPIcolorFront[0].VariableValue : Constant.HYPHEN.ToString(),
                                ItemNumber = hallPIcolorRear.Any() ? hallPIcolorRear[0].VariableValue : Constant.HYPHEN.ToString(),
                            };
                            hallPIFinish.PriceKeyInfo.Add(floorData);
                        }
                        variableAssignments.FloorMatrixTable.Add(hallPIFinish);

                        var hallBraille = new PriceSectionDetails()
                        {
                            Name = Constant.FLOORMATRIXBRAILLE,
                            Section = Convert.ToString(1),
                            Id = Constant.FLOORMATRIX,
                            PriceKeyInfo = new List<PriceValuesDetails>()
                        };
                        foreach (var floor in floorDetails)
                        {
                            var entranceConsoleListForFloor = entranceConsoleList.Where(x => x.FloorNumber == floor.FloorNumber).ToList().Distinct();
                            var hallLanterTypeFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.BRAILLETYPE])).ToList();
                            var hallLanterTypeRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.BRAILLETYPE])).ToList();

                            var floorData = new PriceValuesDetails()
                            {
                                Section = Convert.ToString(floor.FloorNumber),
                                PartDescription = hallLanterTypeFront.Any() ? hallLanterTypeFront[0].VariableValue : Constant.HYPHEN.ToString(),
                                ComponentName = hallLanterTypeRear.Any() ? hallLanterTypeRear[0].VariableValue : Constant.HYPHEN.ToString(),
                                qty = 0,
                            };
                            hallBraille.PriceKeyInfo.Add(floorData);
                        }
                        variableAssignments.FloorMatrixTable.Add(hallBraille);




                        var floorMatrixDesignationPlate = new PriceSectionDetails()
                        {
                            Name = Constant.FLOORMATRIXDESIGNATIONPLATE,
                            Section = Convert.ToString(1),
                            Id = Constant.FLOORMATRIX,
                            PriceKeyInfo = new List<PriceValuesDetails>()
                        };
                        foreach (var floor in floorDetails)
                        {
                            var entranceConsoleListForFloor = entranceConsoleList.Where(x => x.FloorNumber == floor.FloorNumber).ToList().Distinct();
                            var designtionPlateFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.DESIGNATIONPLATETYPE])).ToList();
                            var designtionPlateRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.DESIGNATIONPLATETYPE])).ToList();
                            var illuminationFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.ILLUMINATIONTYPE])).ToList();
                            var illuminationRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.ILLUMINATIONTYPE])).ToList();
                            var finishFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALLPIFINISH])).ToList();
                            var finishRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALLPIFINISH])).ToList();

                            var floorData = new PriceValuesDetails()
                            {
                                Section = Convert.ToString(floor.FloorNumber),
                                PartDescription = designtionPlateFront.Any() ? designtionPlateFront[0].VariableValue : Constant.HYPHEN.ToString(),
                                ComponentName = designtionPlateRear.Any() ? designtionPlateRear[0].VariableValue : Constant.HYPHEN.ToString(),
                                qty = 0,
                                LeadTime = illuminationFront.Any() ? illuminationFront[0].VariableValue : Constant.HYPHEN.ToString(),
                                ItemNumber = illuminationRear.Any() ? illuminationRear[0].VariableValue : Constant.HYPHEN.ToString(),
                                Parameter3 = finishFront.Any() ? finishFront[0].VariableValue : Constant.HYPHEN.ToString(),
                                Parameter3Value = finishRear.Any() ? finishRear[0].VariableValue : Constant.HYPHEN.ToString()
                            };
                            floorMatrixDesignationPlate.PriceKeyInfo.Add(floorData);
                        }
                        variableAssignments.FloorMatrixTable.Add(floorMatrixDesignationPlate);



                        var hallTargetIndicator = new PriceSectionDetails()
                        {
                            Name = Constant.FLOORMATRIXHALLTARGETINDICATOR,
                            Section = Convert.ToString(1),
                            Id = Constant.FLOORMATRIX,
                            PriceKeyInfo = new List<PriceValuesDetails>()
                        };
                        foreach (var floor in floorDetails)
                        {
                            var entranceConsoleListForFloor = entranceConsoleList.Where(x => x.FloorNumber == floor.FloorNumber).ToList().Distinct();
                            var hallLanterTypeFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALLTARGETINDICATOR])).ToList();
                            var hallLanterTypeRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.HALLTARGETINDICATOR])).ToList();

                            var floorData = new PriceValuesDetails()
                            {
                                Section = Convert.ToString(floor.FloorNumber),
                                PartDescription = hallLanterTypeFront.Any() ? hallLanterTypeFront[0].VariableValue : Constant.HYPHEN.ToString(),
                                ComponentName = hallLanterTypeRear.Any() ? hallLanterTypeRear[0].VariableValue : Constant.HYPHEN.ToString(),
                                qty = 0,
                            };
                            hallTargetIndicator.PriceKeyInfo.Add(floorData);
                        }
                        variableAssignments.FloorMatrixTable.Add(hallTargetIndicator);




                        var hallBrailleETAETD = new PriceSectionDetails()
                        {
                            Name = Constant.FLOORMATRIXBRAILLEETAETD,
                            Section = Convert.ToString(1),
                            Id = Constant.FLOORMATRIX,
                            PriceKeyInfo = new List<PriceValuesDetails>()
                        };
                        foreach (var floor in floorDetails)
                        {
                            var entranceConsoleListForFloor = entranceConsoleList.Where(x => x.FloorNumber == floor.FloorNumber).ToList().Distinct();
                            var hallLanterTypeFront = entranceConsoleListForFloor.Where(x => x.Front && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.BRAILLETYPE])).ToList();
                            var hallLanterTypeRear = entranceConsoleListForFloor.Where(x => x.Rear && x.VariableType.EndsWith(floorMatrixTableVariables[Constant.BRAILLETYPE])).ToList();

                            var floorData = new PriceValuesDetails()
                            {
                                Section = Convert.ToString(floor.FloorNumber),
                                PartDescription = hallLanterTypeFront.Any() ? hallLanterTypeFront[0].VariableValue : Constant.HYPHEN.ToString(),
                                ComponentName = hallLanterTypeRear.Any() ? hallLanterTypeRear[0].VariableValue : Constant.HYPHEN.ToString(),
                                qty = 0,
                            };
                            hallBrailleETAETD.PriceKeyInfo.Add(floorData);
                        }
                        variableAssignments.FloorMatrixTable.Add(hallBrailleETAETD);


                    }

                    var priceAndDiscountDetails = summaryScreenDataSet.Tables[23];
                    if (priceAndDiscountDetails.Rows.Count > 0)
                    {
                        variableAssignments.PriceAndDiscountData = new List<DiscountDataPerUnit>();
                        var discountData = (from DataRow dRow in priceAndDiscountDetails.Rows
                                            select new
                                            {
                                                VariableType = Convert.ToString(dRow[Constant.VARIABLEID]),
                                                VariableValue = Convert.ToString(dRow[Constant.VARIABLEVALUE]),
                                                UnitId = Convert.ToInt32(dRow[Constant.UNITID]),
                                                CreatedBy = Convert.ToString(dRow[Constant.CREATEDBY]),
                                                CreatedOn = Convert.ToString(dRow[Constant.CREATEDON])
                                            }).Distinct();
                        foreach (var discount in discountData)
                        {
                            variableAssignments.PriceAndDiscountData.Add(new DiscountDataPerUnit
                            {
                                VariableForUnit = new VariableAssignment { VariableId = discount.VariableType, Value = discount.VariableValue },
                                Unitid = discount.UnitId
                            });
                        }

                        var manuFacturingComments = discountData.Where(x => Utility.CheckEquals(x.VariableType, Constant.MANUFACTORINGCOMMENTS)).Distinct().ToList();
                        manuFacturingComments = manuFacturingComments.GroupBy(x => x.VariableValue)
                            .Select(g => g.First())
                            .ToList();
                        if (manuFacturingComments.Any())
                        {
                            var manufacturingCommentsTable = new PriceSectionDetails()
                            {
                                Name = Constant.MANUFACTURINGCOMMENTSTABLENAME,
                                Section = Convert.ToString(1),
                                Id = Constant.MANUFACTURINGCOMMENTSTABLEID,
                                PriceKeyInfo = new List<PriceValuesDetails>()
                            };
                            foreach (var comment in manuFacturingComments)
                            {
                                if (!string.IsNullOrEmpty(comment.VariableValue))
                                {
                                    var commentData = new PriceValuesDetails()
                                    {
                                        ItemNumber = (Convert.ToString(Constant.HYPHEN) + Convert.ToString(Constant.HYPHEN)).ToString(),
                                        Section = comment.CreatedOn,
                                        PartDescription = comment.VariableValue,
                                        ComponentName = comment.CreatedBy,
                                        qty = 0
                                    };
                                    manufacturingCommentsTable.PriceKeyInfo.Add(commentData);
                                }
                            }
                            if (manufacturingCommentsTable.PriceKeyInfo.Any())
                            {
                                variableAssignments.FloorMatrixTable.Add(manufacturingCommentsTable);
                            }
                        }
                    }

                    var totalFloorsServicedDetails = summaryScreenDataSet.Tables[29];
                    if (totalFloorsServicedDetails.Rows.Count > 0)
                    {
                        var totalFloorsServiced = (from DataRow dRow in totalFloorsServicedDetails.Rows
                                                   select new
                                                   {
                                                       totalFloors = Convert.ToInt32(dRow[Constant.FLOORSSERVICED])
                                                   }).Distinct();
                        unitVariable.Add(new ConfigVariable { VariableId = unitMapperVariables[Constant.ELELANDPPARAMETER], Value = totalFloorsServiced.Select(x => x.totalFloors).FirstOrDefault() });
                        unitVariable.Add(new ConfigVariable { VariableId = unitMapperVariables[Constant.TOTALOPENINGSPARAMETER], Value = totalFloorsServiced.Select(x => x.totalFloors).FirstOrDefault() });
                    }
                }

                var customPriceKeys = summaryScreenDataSet.Tables[28];
                if (customPriceKeys.Rows.Count > 0)
                {
                    var customPriceLines = (from DataRow row in customPriceKeys.Rows
                                            select new
                                            {
                                                PriceKeyId = Convert.ToInt32(row[Constants.ID]),
                                                SectionId = Convert.ToString(row[Constants.SECTIONID]),
                                                ItemNumber = Convert.ToString(row[Constants.ITEMNUMBER]),
                                                ComponentName = Convert.ToString(row[Constants.COMPONENTNAME]),
                                                Description = Convert.ToString(row[Constants.DESCRIPTION]),
                                                Quantity = Convert.ToInt32(row[Constants.QUANTITY]),
                                                Unit = Convert.ToString(row[Constant.UNIT]),
                                                UnitPrice = Convert.ToDecimal(row[Constants.UNITPRICE]),
                                                LeadTime = Convert.ToString(row[Constants.LEADTIME])
                                            }).ToList();
                    if (customPriceLines.Count > 0)
                    {
                        foreach (var priceLIneItem in customPriceLines)
                        {
                            var priceLine = new UnitPriceValues()
                            {
                                quantity = priceLIneItem.Quantity,
                                totalPrice = priceLIneItem.Quantity * priceLIneItem.UnitPrice,
                                Unit = priceLIneItem.Unit,
                                unitPrice = priceLIneItem.UnitPrice
                            };
                            var customPriceLine = (new CustomPriceLine()
                            {
                                priceKeyInfo = new PriceValuesDetails()
                                {
                                    PriceKeyId = priceLIneItem.PriceKeyId,
                                    Section = priceLIneItem.SectionId,
                                    ItemNumber = priceLIneItem.ItemNumber,
                                    ComponentName = priceLIneItem.ComponentName,
                                    PartDescription = priceLIneItem.Description,
                                    IsCustomPriceLine = true,
                                    LeadTime = Utility.CheckEquals(priceLIneItem.LeadTime, "0") ? null : priceLIneItem.LeadTime
                                },
                                PriceValue = new Dictionary<string, UnitPriceValues>()
                            });
                            customPriceLine.PriceValue.Add(priceLIneItem.ItemNumber, priceLine);
                            variableAssignments.CustomPriceLine.Add(customPriceLine);
                        }
                    }
                }

                var machpnVariable = summaryScreenDataSet.Tables[32];
                if (machpnVariable.Rows.Count > 0)
                {
                    var requiredVariable = (from DataRow row in machpnVariable.Rows
                                            select new
                                            {
                                                Id = Convert.ToString(row[Constants.SYSTEMVARIABLEKEYS]),
                                                Value = Convert.ToString(row[Constants.SYSTEMVARIABLEVALUES])
                                            }).ToList();
                    foreach (var variable in requiredVariable)
                    {
                        unitVariable.Add(new ConfigVariable { VariableId = Constants.ELEVATOR + Constants.DOT + variable.Id, Value = variable.Value });
                    }
                }
            }
            variableAssignments.UnitDetails = unitDetailsForTP2;

            foreach (var item in mainVariableResObj)
            {
                switch (item.Id)
                {
                    case Constant.BUILDINGVARIABLESLIST:
                        item.VariableAssignments = buildngConfig;
                        break;
                    case Constant.GROUPVARIABLESLIST:
                        item.VariableAssignments = groupVariables;
                        break;
                    case Constant.UNITVARIABLESLIST:
                        item.VariableAssignments = unitVariable;
                        break;
                    default:
                        item.VariableAssignments = unitVariable;
                        break;
                }
            }

            variableAssignments.VariableAssignments = mainVariableResObj;
            variableAssignments.OpeningVariableAssginments = openingsValues;
            Utility.LogEnd(methodBegin);
            return variableAssignments;
        }

        /// <summary>
        /// SaveUnitHallFixtureConfiguration
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="unitHallFixtureConfigurationData"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public List<ResultSetConfiguration> SaveUnitHallFixtureConfiguration(int setId, UnitHallFixtureData unitHallFixtureConfigurationData, string userId, int is_Saved, List<LogHistoryTable> historyTable)
        {
            var methodBegin = Utility.LogBegin();
            ResultSetConfiguration result = new ResultSetConfiguration();
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
            DataTable hallLanternConsoleDataTable = Utility.GenerateUnitHallFixtureConsoleDataTable(unitHallFixtureConfigurationData);
            DataTable hallLanternConfigurationDataTable = Utility.GenerateHallLanternConfigurationDataTable(unitHallFixtureConfigurationData.VariableAssignments, Convert.ToInt32(unitHallFixtureConfigurationData.ConsoleId));
            DataTable hallLanternLocationDataTable = Utility.GenerateUnitHallFixtureLocationLocationDataTable(unitHallFixtureConfigurationData.FixtureLocations, Convert.ToInt32(unitHallFixtureConfigurationData.ConsoleId));
            DataTable historyDataTable = Utility.GenerateDataTableForHistoryTable(historyTable);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSaveHallLanternConfiguration(setId, Convert.ToInt32(unitHallFixtureConfigurationData.ConsoleId), hallLanternConsoleDataTable, hallLanternConfigurationDataTable, hallLanternLocationDataTable, userId, historyDataTable);
            int resultForSaveHallLanternConfiguration = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVEUNITHALLFIXTURECONFIGURATION, lstSqlParameter);

            if (resultForSaveHallLanternConfiguration > 0 && is_Saved.Equals(0))
            {
                result.result = 1;
                result.setId = resultForSaveHallLanternConfiguration;
                result.message = Constant.UNITCONFIGURATIONSAVEMESSAGE;
            }
            else if (resultForSaveHallLanternConfiguration > 0 && is_Saved.Equals(1))
            {
                result.result = 1;
                result.setId = resultForSaveHallLanternConfiguration;
                result.message = Constant.UNITCONFIGURATIONUPDATEMESSAGE;
            }
            else if (resultForSaveHallLanternConfiguration == 0)
            {
                result.result = 0;
                result.setId = resultForSaveHallLanternConfiguration;
                result.message = Constant.UNITCONFIGURATIONSAVEERRORMESSAGE;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBegin);
            return lstResult;
        }

        /// <summary>
        /// GenerateUnitHallFixturesList
        /// </summary>
        /// <param Name="FixtureStrategy"></param>
        /// <returns></returns>
        public List<string> GenerateUnitHallFixturesList(string fixtureStrategy)
        {
            var methodBegin = Utility.LogBegin();
            List<string> unitHallFixtureTypes = new List<string>();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            SqlParameter param = new SqlParameter(Constant.@FIXTURESTRATEGY, fixtureStrategy);
            sqlParameters.Add(param);
            DataSet dataSet = new DataSet();
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETFIXTURESLIST, sqlParameters);
            if ((dataSet.Tables.Count > 0) & (dataSet.Tables[0].Rows.Count > 0))
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                    unitHallFixtureTypes.Add(row[Constant.UNITHALLFIXTURETYPELIST].ToString());
            }
            Utility.LogEnd(methodBegin);
            return unitHallFixtureTypes;
        }

        /// <summary>
        /// GetGroupHallFixturesTypesList
        /// </summary>
        /// <param Name="FixtureStrategy"></param>
        /// <returns></returns>
        public List<string> GetGroupHallFixturesTypesList(string fixtureStrategy)
        {
            var methodBegin = Utility.LogBegin();
            List<string> unitHallFixtureTypes = new List<string>();

            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            SqlParameter param = new SqlParameter(Constant.@FIXTURESTRATEGY, fixtureStrategy);
            sqlParameters.Add(param);
            DataSet dataSet = new DataSet();
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETGROUPFIXTURESLIST, sqlParameters);
            if ((dataSet.Tables.Count > 0) & (dataSet.Tables[0].Rows.Count > 0))
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                    unitHallFixtureTypes.Add(row[Constant.GROUPHALLFIXTURETYPELIST].ToString());
            }
            Utility.LogEnd(methodBegin);
            return unitHallFixtureTypes;
        }

        /// <summary>
        /// DeleteEntranceConsole
        /// </summary>
        /// <param Name="consoleId"></param>
        /// <param Name="setId"></param>
        /// <param Name="logHistoryTable"></param>
        /// <returns></returns>
        public List<ResultSetConfiguration> DeleteEntranceConsole(int consoleId, int setId, List<LogHistoryTable> logHistoryTable, string userId)
        {
            var methodBegin = Utility.LogBegin();
            ResultSetConfiguration result = new ResultSetConfiguration();
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
            DataTable historyTable = Utility.GenerateDataTableForHistoryTable(logHistoryTable);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            SqlParameter param = new SqlParameter(Constant.SETID, setId);
            sqlParameters.Add(param);
            param = new SqlParameter(Constant.CONSOLEID, consoleId);
            sqlParameters.Add(param);
            param = new SqlParameter() { ParameterName = @"historyTable", Direction = ParameterDirection.Input, Value = historyTable };
            sqlParameters.Add(param);
            param = new SqlParameter() { ParameterName = @"CreatedBy", Direction = ParameterDirection.Input, Value = userId };
            sqlParameters.Add(param);
            param = new SqlParameter() { ParameterName = Constant.RESULT, Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int };
            sqlParameters.Add(param);
            int Result = CpqDatabaseManager.ExecuteNonquery(Constant.SPDELETEENTRANCECONSOLE, sqlParameters);
            if (Result > 0)
            {
                result.result = 1;
                result.setId = Result;
                result.message = Constant.DELETEENTRANCECONSOLESUCCESSMSG;
            }
            else if (Result == 0)
            {
                result.result = 0;
                result.setId = Result;
                result.message = Constant.DELETEENTRANCECONSOLEERRORMSG;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBegin);
            return lstResult;
        }

        /// <summary>
        /// DeleteUnitHallFixtureConsole
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="consoleId"></param>
        /// <returns></returns>
        public List<ResultSetConfiguration> DeleteUnitHallFixtureConsole(int setId, int consoleId, string fixtureType, List<LogHistoryTable> logHistoryTable, string userId)
        {
            var methodBegin = Utility.LogBegin();
            ResultSetConfiguration result = new ResultSetConfiguration();
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
            DataTable historyTable = Utility.GenerateDataTableForHistoryTable(logHistoryTable);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            SqlParameter param = new SqlParameter(Constant.SETID, setId);
            sqlParameters.Add(param);
            param = new SqlParameter(Constant.CONSOLEID, consoleId);
            sqlParameters.Add(param);
            param = new SqlParameter(Constant.FIXTURETYPE, fixtureType);
            sqlParameters.Add(param);
            param = new SqlParameter() { ParameterName = @"historyTable", Direction = ParameterDirection.Input, Value = historyTable };
            sqlParameters.Add(param);
            param = new SqlParameter() { ParameterName = @"CreatedBy", Direction = ParameterDirection.Input, Value = userId };
            sqlParameters.Add(param);
            param = new SqlParameter() { ParameterName = Constant.RESULT, Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int };
            sqlParameters.Add(param);
            int Result = CpqDatabaseManager.ExecuteNonquery(Constant.SPDELETEUNITHALLFIXTURECONFIGBYID, sqlParameters);
            if (Result > 0)
            {
                result.result = 1;
                result.setId = setId;
                result.message = Constant.DELETEUNITHALLFIXTURECONSOLESUCCESSMSG;
            }
            else if (Result == 0)
            {
                result.result = 0;
                result.setId = setId;
                result.message = Constant.DELETEUNITHALLFIXTURECONSOLEERRORMSG;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBegin);
            return lstResult;

        }

        /// <summary>
        /// ResetUnitHallFixtureConsole
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="consoleId"></param>
        /// <returns></returns>
        public List<UnitHallFixtures> ResetUnitHallFixtureConsole(int setId, int consoleId, string fixtureType, string userName)
        {
            var methodBegin = Utility.LogBegin();
            ResultSetConfiguration result = new ResultSetConfiguration();
            List<UnitHallFixtures> objUnitHallFixtureConfigurationData = new List<UnitHallFixtures>();
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            SqlParameter param = new SqlParameter(Constant.SETID1, setId);
            sqlParameters.Add(param);
            param = new SqlParameter(Constant.ATFIXTURETYPE, fixtureType);
            sqlParameters.Add(param);
            param = new SqlParameter(Constant.USERNAME, fixtureType);
            sqlParameters.Add(param);
            param = new SqlParameter(Constant.ATCONSOLENUMBER, consoleId);
            sqlParameters.Add(param);
            DataSet ds = new DataSet();
            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPRESETUNITHALLFIXTURE, sqlParameters);

            if (ds.Tables[0].Rows.Count > 0)
            {
                List<UnitHallFixtures> unitHallFixtureConsoles = new List<UnitHallFixtures>();
                var unitHallFixtureConsoleList = (from DataRow row in ds.Tables[0].Rows
                                                  select new
                                                  {
                                                      entranceConsoleId = Convert.ToInt32(row[Constant.UNITHALLFIXTURECONSOLEID]),
                                                      ConsoleName = row["UnitHallFixturenConsoleName"].ToString(),
                                                      isController = Convert.ToBoolean(row[Constant.ISCONTROLLER]),
                                                      FrontOpening = Convert.ToBoolean(row[Constant.FRONTOPENING]),
                                                      RearOpening = Convert.ToBoolean(row[Constant.REAROPENING]),
                                                      FixtureType = row["FixtureType"].ToString(),

                                                  }).Distinct();
                foreach (var console in unitHallFixtureConsoleList)
                {
                    var opening = new Openings()
                    {
                        Front = console.FrontOpening,
                        Rear = console.RearOpening
                    };
                    UnitHallFixtures unitHallFixtureConsole = new UnitHallFixtures()
                    {
                        ConsoleId = console.entranceConsoleId,
                        ConsoleName = console.ConsoleName,
                        AssignOpenings = !console.isController,
                        IsController = console.isController,
                        Openings = opening,
                        UnitHallFixtureType = console.FixtureType,
                    };

                    var variableList = (from DataRow rows in ds.Tables[0].Rows
                                        select new
                                        {
                                            entranceConsoleId = Convert.ToInt32(rows[Constant.UNITHALLFIXTURECONSOLEID]),
                                            variableType = "",
                                            variablevalue = "",
                                            unitFixtureType = rows["FixtureType"].ToString(),
                                        }).Distinct();

                    DataColumnCollection columns = ds.Tables[0].Columns;
                    if (columns.Contains(Constant.VARIABLETYPE) && columns.Contains(Constant.VARIABLEVALUE))
                    {
                        var variableListValues = (from DataRow rows in ds.Tables[0].Rows
                                                  select new
                                                  {
                                                      entranceConsoleId = Convert.ToInt32(rows[Constant.UNITHALLFIXTURECONSOLEID]),
                                                      variableType = rows[Constant.VARIABLETYPE] != DBNull.Value ? rows[Constant.VARIABLETYPE].ToString() : "",
                                                      variablevalue = rows[Constant.VARIABLEVALUE] != DBNull.Value ? rows[Constant.VARIABLEVALUE]?.ToString() : "",
                                                      unitFixtureType = rows["FixtureType"].ToString(),
                                                  }).Distinct();
                        variableList = variableListValues;
                    }


                    variableList = variableList?.Where(x => x.unitFixtureType.Equals(unitHallFixtureConsole.UnitHallFixtureType)
                    && x.entranceConsoleId.Equals(unitHallFixtureConsole.ConsoleId)).Distinct().ToList();


                    List<ConfigVariable> variableAssignments = new List<ConfigVariable>();
                    foreach (var assignments in variableList)
                    {
                        ConfigVariable variableAssignment = new ConfigVariable()
                        {
                            VariableId = assignments.variableType,
                            Value = assignments.variablevalue
                        };
                        variableAssignments.Add(variableAssignment);
                    }
                    unitHallFixtureConsole.VariableAssignments = variableAssignments;
                    var entranceList = (from DataRow row in ds.Tables[0].Rows
                                        select new
                                        {
                                            entranceConsoleId = Convert.ToInt32(row[Constant.UNITHALLFIXTURECONSOLEID]),
                                            floorNumber = Convert.ToInt32(row[Constant.FLOORNUMBER]),
                                            floorDesignation = row[Constant.FLOORNUMBER].ToString(),
                                            front = Convert.ToBoolean(row[Constant.FRONT]),
                                            rear = Convert.ToBoolean(row[Constant.REAR]),
                                            openingFront = Convert.ToBoolean(row[Constant.OPENINGFRONT]),
                                            openingRear = Convert.ToBoolean(row[Constant.OPENINGREAR]),
                                            unitHallFixtureType = row["FixtureType"].ToString()
                                        }).Distinct();
                    entranceList = entranceList.Where(x => x.entranceConsoleId.Equals(unitHallFixtureConsole.ConsoleId) && x.unitHallFixtureType.Equals(unitHallFixtureConsole.UnitHallFixtureType)).Distinct().ToList();
                    List<EntranceLocations> entranceLocations = new List<EntranceLocations>();
                    foreach (var entrancelocation in entranceList)
                    {
                        EntranceLocations entranceLocation = new EntranceLocations()
                        {
                            FloorNumber = entrancelocation.floorNumber,
                            FloorDesignation = entrancelocation.floorDesignation
                        };
                        LandingOpening landingOpening = new LandingOpening()
                        {
                            InCompatible = !unitHallFixtureConsole.Openings.Front,
                            Value = entrancelocation.front
                        };
                        entranceLocation.Front = landingOpening;
                        landingOpening = new LandingOpening()
                        {
                            InCompatible = !unitHallFixtureConsole.Openings.Rear,
                            Value = entrancelocation.rear
                        };
                        entranceLocation.Rear = landingOpening;
                        entranceLocations.Add(entranceLocation);
                    }
                    unitHallFixtureConsole.UnitHallFixtureLocations = entranceLocations;
                    unitHallFixtureConsoles.Add(unitHallFixtureConsole);
                }

                objUnitHallFixtureConfigurationData = unitHallFixtureConsoles;

            }
            Utility.LogEnd(methodBegin);
            return objUnitHallFixtureConfigurationData;
        }

        /// <summary>
        /// SaveCarCallCutoutKeyswitchOpenings
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="carcallCutoutData"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        public List<ResultSetConfiguration> SaveCarCallCutoutKeyswitchOpenings(int setId, CarcallCutoutData carcallCutoutData, string userId, List<LogHistoryTable> loghistoryTable)
        {
            var methodBegin = Utility.LogBegin();
            ResultSetConfiguration result = new ResultSetConfiguration();
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();

            DataTable entranceLocationDataTable = Utility.GenerateEntranceLocationDataTable(carcallCutoutData.EntranceLocations, Convert.ToInt32(carcallCutoutData.EntranceConsoleId));
            DataTable historyTable = Utility.GenerateDataTableForHistoryTable(loghistoryTable);

            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSaveCarCallCutoutOpenings(setId, Convert.ToInt32(carcallCutoutData.EntranceConsoleId), entranceLocationDataTable, userId, historyTable);
            int resultForSaveUnitConfiguration = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVECARCALLCUTOUTKEYSWITCHOPENING, lstSqlParameter);

            if (resultForSaveUnitConfiguration > 0)
            {
                result.result = 1;
                result.setId = resultForSaveUnitConfiguration;
                result.message = Constant.UNITCONFIGURATIONSAVEMESSAGE;
            }
            else
            {
                result.result = 0;
                result.setId = resultForSaveUnitConfiguration;
                result.message = Constant.UNITCONFIGURATIONSAVEERRORMESSAGE;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBegin);
            return lstResult;
        }

        /// <summary>
        /// GetCarCallCutoutOpenings
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        public EntranceAssignment GetCarCallCutoutOpenings(int setId)
        {
            var methodBegin = Utility.LogBegin();
            EntranceAssignment objOpeningsData = new EntranceAssignment();

            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            SqlParameter param = new SqlParameter(Constant.SETID, setId);
            sqlParameters.Add(param);
            DataSet dataSet = new DataSet();
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETCARCALLCUTOUTKEYSWITCHOPENINGS, sqlParameters);

            int mainEgress = 0;
            bool isSaved = false;
            if (dataSet != null && dataSet.Tables.Count > 1)
            {
                if (dataSet.Tables[1].Rows.Count > 0)
                {
                    mainEgress = Convert.ToInt32(dataSet.Tables[1].Rows[0][Constant.FLOORNUMBER]);
                }
            }
            objOpeningsData.IsSaved = isSaved;
            if (dataSet != null && dataSet.Tables.Count > 2)
            {
                if (dataSet.Tables[2].Rows.Count > 0)
                {
                    isSaved = Convert.ToBoolean(dataSet.Tables[2].Rows[0][Constant.ISSAVED]);
                }
            }
            objOpeningsData.IsSaved = isSaved;
            if (dataSet != null && dataSet.Tables.Count > 0)
            {

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    List<EntranceConfigurations> entranceConsoles = new List<EntranceConfigurations>();
                    var entranceConsoleList = (from DataRow row in dataSet.Tables[0].Rows
                                               select new
                                               {
                                                   FrontOpening = Convert.ToInt32(row[Constant.FRONTOPENING]),
                                                   RearOpening = Convert.ToInt32(row[Constant.REAROPENING])
                                               }).Distinct();
                    foreach (var console in entranceConsoleList)
                    {
                        var opening = new Openings()
                        {
                            Front = console.FrontOpening > 0,
                            Rear = console.RearOpening > 0
                        };
                        var entranceList = (from DataRow row in dataSet.Tables[0].Rows
                                            select new
                                            {
                                                floorNumber = Convert.ToInt32(row[Constant.FLOORNUMBER]),
                                                floorDesignation = row[Constant.FLOORNUMBER].ToString(),
                                                front = Convert.ToBoolean(row[Constant.FRONT]),
                                                rear = Convert.ToBoolean(row[Constant.REAR]),
                                                FrontOpening = Convert.ToInt32(row[Constant.FRONTOPENING]),
                                                RearOpening = Convert.ToInt32(row[Constant.REAROPENING]),
                                                openingFront = Convert.ToBoolean(row[Constant.OPENINGFRONT]),
                                                openingRear = Convert.ToBoolean(row[Constant.OPENINGREARS])
                                            }).Distinct();
                        List<EntranceLocations> entranceLocations = new List<EntranceLocations>();
                        foreach (var entrancelocation in entranceList)
                        {
                            EntranceLocations entranceLocation = new EntranceLocations()
                            {
                                FloorNumber = entrancelocation.floorNumber,
                                FloorDesignation = entrancelocation.floorDesignation
                            };
                            LandingOpening landingOpening = new LandingOpening()
                            {
                                InCompatible = false,
                                NotAvailable = !entrancelocation.openingFront,
                                Value = entrancelocation.front
                            };
                            entranceLocation.Front = landingOpening;
                            landingOpening = new LandingOpening()
                            {
                                InCompatible = false,
                                NotAvailable = !entrancelocation.openingRear,
                                Value = entrancelocation.rear
                            };
                            entranceLocation.Rear = landingOpening;
                            entranceLocations.Add(entranceLocation);


                        }
                        foreach (var location in entranceLocations)
                        {
                            if (location.FloorNumber.Equals(mainEgress))
                            {
                                location.Front.NotAvailable = true;
                                location.Rear.NotAvailable = true;
                            }
                        }
                        objOpeningsData.Openings = opening;
                        objOpeningsData.FixtureAssignments = entranceLocations;
                    }

                }
            }
            Utility.LogEnd(methodBegin);
            return objOpeningsData;
        }

        /// <summary>
        /// GetCarCallcutoutSavedOpenings
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        public int GetCarCallcutoutSavedOpenings(int setId)
        {
            var methodBegin = Utility.LogBegin();
            int Quantity = 0;
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            DataSet dataSet = new DataSet();

            SqlParameter param = new SqlParameter(Constant.@SETID, setId);
            sqlParameters.Add(param);
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETCARCUTOUTSAVEDOPENINGS, sqlParameters);
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                Quantity = Convert.ToInt32(dataSet.Tables[0].Rows[0][Constant.QUANTITY]);
            }
            Utility.LogEnd(methodBegin);
            return Quantity;
        }

        /// <summary>
        /// GetLogHistoryUnit
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="lastDate"></param>
        /// <returns></returns>
        public LogHistoryResponse GetLogHistoryUnit(int setId, int unitId, string lastDate)
        {
            var methodBegin = Utility.LogBegin();
            LogHistoryResponse response = new LogHistoryResponse();
            response.Data = new List<Data>();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            DataSet dataSet = new DataSet();
            SqlParameter param = new SqlParameter(Constant.@SETID, setId);
            sqlParameters.Add(param);
            param = new SqlParameter(Constant.@UNITID, unitId);
            sqlParameters.Add(param);
            if (lastDate != null && !lastDate.Equals(""))
            {
                var culture = System.Globalization.CultureInfo.CurrentCulture;
                param = new SqlParameter(Constant.DATE, DateTime.ParseExact(lastDate, "MM/dd/yyyy", culture));
            }
            else
            {
                param = new SqlParameter(Constant.DATE, DBNull.Value);
            }
            sqlParameters.Add(param);
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETLOGHISTORYUNIT, sqlParameters);
            if (dataSet != null)
            {
                if (dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        var lstDate = (from DataRow row in dataSet.Tables[0].Rows
                                       select new
                                       {
                                           setId = Convert.ToInt32(row[Constant.SETCONFIGURATIONID]),
                                           date = Convert.ToDateTime(row[Constant.CREATEDON]).ToString(Constant.MMDDYYYYFORMAT)
                                       }).Distinct();
                        var LogHistory = (from DataRow row in dataSet.Tables[0].Rows
                                          select new
                                          {
                                              setId = Convert.ToInt32(row[Constant.SETCONFIGURATIONID]),
                                              date = Convert.ToDateTime(row[Constant.CREATEDON]).ToString(Constant.MMDDYYYYFORMAT),
                                              variableId = Convert.ToString(row[Constant.VARIABLEID]),
                                              currentValue = Convert.ToString(row[Constant.CURRENTVALUE]),
                                              previousValue = Convert.ToString(row[Constant.PREVIOUSVALUE]),
                                              user = Convert.ToString(row[Constant.CREATEDBY]),
                                              time = Convert.ToDateTime(row[Constant.CREATEDON]).ToString("hh:mm tt")
                                          }).Distinct();
                        List<Data> lstData = new List<Data>();
                        if (lstDate.Any())
                        {
                            foreach (var varDate in lstDate)
                            {
                                Data data = new Data();
                                data.Date = varDate.date;
                                List<LogParameters> lstLogparameters = new List<LogParameters>();
                                var filteresHistory = LogHistory.Where(x => x.date.Equals(varDate.date));
                                if (filteresHistory.Any())
                                {
                                    foreach (var loghistory in filteresHistory)
                                    {
                                        LogParameters logparameter = new LogParameters()
                                        {
                                            VariableId = loghistory.variableId,
                                            Name = loghistory.variableId,
                                            UpdatedValue = loghistory.currentValue,
                                            PreviousValue = loghistory.previousValue,
                                            User = loghistory.user,
                                            Role = string.Empty,
                                            Time = loghistory.time
                                        };

                                        lstLogparameters.Add(logparameter);
                                    }

                                }
                                data.LogParameters = lstLogparameters;
                                lstData.Add(data);
                            }
                        }
                        response.Data = lstData;
                    }
                }
                if (dataSet.Tables.Count > 1)
                {
                    if (dataSet.Tables[1].Rows.Count > 0)
                    {
                        var designation = Convert.ToString(dataSet.Tables[1].Rows[0][Constant.DESIGNATION]);
                        response.Description = designation;
                        response.Section = Constant.UNIT;
                    }
                }
                if (dataSet.Tables.Count > 2)
                {
                    if (dataSet.Tables[2].Rows.Count > 0)
                    {
                        var showLoadMore = Convert.ToBoolean(dataSet.Tables[2].Rows[0][Constant.SHOWLOADMORE]);
                        response.ShowLoadMore = showLoadMore;
                    }
                }
            }
            Utility.LogEnd(methodBegin);
            return response;
        }

        /// <summary>
        /// SaveEntrances
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="productName"></param>
        /// <param Name="listOfEntranceVariables"></param>
        /// <returns></returns>
        public List<ResultUnitConfiguration> SaveEntrances(int groupid, string productName, List<ConfigVariable> listOfEntranceVariables, string userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// UpdateEntrances
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="productName"></param>
        /// <param Name="listOfEntranceVariables"></param>
        /// <returns></returns>
        public List<ResultUnitConfiguration> UpdateEntrances(int groupid, string productName, List<ConfigVariable> listOfEntranceVariables, string userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get Travel Value
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        public TP2Summary GetTravelValue(int setId)
        {
            var methodBegin = Utility.LogBegin();
            var unitMapperVariables = Utility.VariableMapper(Constant.UNITSVARIABLESMAPPERPATH, Constant.UNITMAPPER);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            TP2Summary variableAssignments = new TP2Summary();
            DataSet travelValueSummarySet = new DataSet();

            SqlParameter param = new SqlParameter(Constant.SETID, setId);
            sqlParameters.Add(param);
            travelValueSummarySet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETTRAVELVALUEFORUNITCONFIGURATION, sqlParameters);

            if (travelValueSummarySet != null && travelValueSummarySet.Tables.Count > 0)
            {
                //int derivedGroupId = 0;
                var travel = travelValueSummarySet.Tables[0];
                var dataSetGroupId = travelValueSummarySet.Tables[1];
                if (dataSetGroupId.Rows.Count > 0)
                {
                    var derivedGroupId = (from DataRow dRow in travelValueSummarySet.Tables[1].Rows
                                          select new
                                          { groupId = Convert.ToInt32(dRow[Constants.GROUPCONFIGURATIONID]) }).FirstOrDefault();
                    variableAssignments.GroupId = derivedGroupId.groupId;
                }
                if (travel.Rows.Count > 0)
                {
                    var travelVariableAssignments = new VariableAssignment();
                    travelVariableAssignments.VariableId = unitMapperVariables[Constant.TRAVELVARIABLEIDVALUE];
                    int travelFeet = 0;
                    decimal travelinch = 0;
                    foreach (DataRow travelFeetAndInch in travel.Rows)
                    {
                        if (!(Convert.ToString(travelFeetAndInch[Constant.TRAVELFEET])).Equals(Constant.EMPTYSTRING))
                        {
                            travelFeet = Convert.ToInt32(travelFeetAndInch[Constant.TRAVELFEET]);
                        }
                        if (!(Convert.ToString(travelFeetAndInch[Constant.TRAVELINCH])).Equals(Constant.EMPTYSTRING))
                        {
                            travelinch = Convert.ToInt32(travelFeetAndInch[Constant.TRAVELINCH]);
                        }

                    }
                    travelVariableAssignments.Value = travelFeet * 12 + travelinch;

                    variableAssignments.TravelVariableAssignments = travelVariableAssignments;
                }
            }
            Utility.LogEnd(methodBegin);
            return variableAssignments;
        }

        /// <summary>
        /// GetSystemsValValues
        /// </summary>
        /// <param Name="SetId"></param>
        /// <param Name="userid"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        public Status GetSystemsValValues(int setId, string userid)
        {
            var methodBegin = Utility.LogBegin();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            var systemStatusFlag = new Status();
            DataSet summaryScreenDataSet = new DataSet();

            SqlParameter param = new SqlParameter(Constant.SETID, setId);
            SqlParameter paramsData = new SqlParameter(Constant.FDACREATEDDBY, userid);
            sqlParameters.Add(param);
            sqlParameters.Add(paramsData);
            summaryScreenDataSet = CpqDatabaseManager.ExecuteDataSet(Constant.USP_GETINPROGRESSSYSTEMSVALUES, sqlParameters);

            if (summaryScreenDataSet != null && summaryScreenDataSet.Tables.Count > 0)
            {
                var systemStstausValues = summaryScreenDataSet.Tables[0];
                if (systemStstausValues.Rows.Count > 0)
                {
                    systemStatusFlag = new Status()
                    {
                        StatusName = systemStstausValues.Rows[0][Constant.SYSTEMSTATUS].ToString(),
                        DisplayName = systemStstausValues.Rows[0][Constant.DISPLAYNAMESTATUS].ToString(),
                        Description = systemStstausValues.Rows[0][Constant.DESCRIPTIONSTATUS].ToString()
                    };
                }
            }
            Utility.LogEnd(methodBegin);
            return systemStatusFlag;
        }

        /// <summary>
        /// GetLatestSystemsValValues
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="statusKey"></param>
        /// <param Name="userId"></param>
        /// <param Name="UnitDetails"></param>
        /// <returns></returns>
        public ConfigurationResponse GetLatestSystemsValValues(int setId, string statusKey, string userId, List<SystemValidationKeyValues> systemKeyValues, string type, List<UnitDetailsForTP2> UnitDetails, string sessionId, List<VariableAssignment> Response = null)
        {
            var methodBegin = Utility.LogBegin();
            ConfigurationResponse mainRespone = new ConfigurationResponse();
            ConflictManagement conflicts = new ConflictManagement();
            List<UnitConflictValues> unitConflictValues = new List<UnitConflictValues>();
            List<SystemValidationConflictsValues> sysVal = new List<SystemValidationConflictsValues>();
            var systemStatusFlag = new Status();
            DataSet summaryScreenDataSet = new DataSet();
            DataTable systemDataTable = new DataTable();
            DataTable systemVariablesDataTable = new DataTable();
            if (systemKeyValues == null)
            {
                systemKeyValues = new List<SystemValidationKeyValues>();
                systemDataTable = Utility.GenerateDataTableForSaveSystemValidation(systemKeyValues);
            }
            else
            {
                systemDataTable = Utility.GenerateDataTableForSaveSystemValidation(systemKeyValues);
            }

            systemVariablesDataTable = Utility.GenerateDataTableForSaveSystemVariables(Response, setId);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForSaveGetSystemValues(setId, statusKey, userId, systemDataTable, systemVariablesDataTable);
            IList<SqlParameter> lstSqlParameterForValidationStatus = Utility.SqlParameterForSaveGetSystemValues(setId, statusKey, userId, systemDataTable, null);
            if (!string.IsNullOrEmpty(type))
            {
                summaryScreenDataSet = CpqDatabaseManager.ExecuteDataSet(Constant.USP_GETVALIDATIONSTATUS, lstSqlParameterForValidationStatus);
            }
            else
            {
                summaryScreenDataSet = CpqDatabaseManager.ExecuteDataSet(Constant.USP_SAVEANDUPDATESYSTEMVALIDATION, lstSqlParameter);
            }

            if (summaryScreenDataSet != null && summaryScreenDataSet.Tables.Count > 0)
            {
                var systemStstausValues = summaryScreenDataSet.Tables[0];
                if (systemStstausValues.Rows.Count > 0)
                {
                    systemStatusFlag = new Status()
                    {
                        StatusKey = systemStstausValues.Rows[0]["StatusKey"]?.ToString(),
                        StatusName = systemStstausValues.Rows[0]["StatusName"]?.ToString(),
                        DisplayName = systemStstausValues.Rows[0]["DisplayName"]?.ToString(),
                        Description = systemStstausValues.Rows[0]["Description"]?.ToString()
                    };

                    if (systemStstausValues.Columns.Contains("SystemValidKeys"))
                    {
                        foreach (DataRow systemTableData in systemStstausValues.Rows)
                        {
                            // change names here
                            int unitId = 0;
                            if (systemStstausValues.Columns.Contains("UnitId") && !string.IsNullOrEmpty(systemTableData["UnitId"]?.ToString()))
                            {
                                unitId = Convert.ToInt32(systemTableData["UnitId"]?.ToString());
                            }
                            var sysMappingData = new SystemValidationConflictsValues()
                            {
                                FlagId = systemTableData["SystemValidKeys"]?.ToString(),
                                Description = systemTableData["SystemsDescriptionKeys"]?.ToString(),
                                Message = systemTableData["SystemsDescriptionValues"]?.ToString(),
                                UnitId = unitId,
                                SectionId = new List<string>(),
                                SystemConflictVariables = new List<string>()

                            };
                            var flagNames = systemTableData["FlagName"]?.ToString();
                            if (!string.IsNullOrEmpty(flagNames))
                            {
                                var variablesStub = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATERESPONSESTUB, Constant.EVOLUTION200)));
                                if (UnitDetails.FirstOrDefault().ProductName.Equals(Constants.EVO_200))
                                {
                                    variablesStub = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATERESPONSESTUB, Constant.EVOLUTION200)));
                                }
                                else if (UnitDetails.FirstOrDefault().ProductName.Equals(Constants.EVO_100))
                                {
                                    variablesStub = JObject.Parse(File.ReadAllText(string.Format(Constant.SYSTEMVALIDATERESPONSESTUB, Constants.EVOLUTION100)));
                                }
                                var dbToFlagMappingVariables = Utility.DeserializeObjectValue<Dictionary<string, List<string>>>(Utility.SerializeObjectValue(variablesStub["mappingListVariables"]));
                                var dbToFlagMappingSections = Utility.DeserializeObjectValue<Dictionary<string, List<string>>>(Utility.SerializeObjectValue(variablesStub["mappingListSections"]));
                                var sectionId = new List<string>();
                                foreach (var item in dbToFlagMappingVariables)
                                {
                                    if (Utility.CheckEquals(item.Key, flagNames))
                                    {
                                        sectionId = item.Value;
                                    }
                                }
                                var flagVariablesValues = dbToFlagMappingSections.Where(s => s.Key.ToLower().Equals(flagNames.ToLower())).Select(s => s.Value).FirstOrDefault();
                                if (flagVariablesValues != null && flagVariablesValues.Any())
                                {
                                    sysMappingData.SectionId = flagVariablesValues;
                                }
                                var flagSectionsValues = dbToFlagMappingVariables.Where(s => s.Key.ToLower().Equals(flagNames.ToLower())).Select(s => s.Value).FirstOrDefault();
                                if (flagSectionsValues != null && flagSectionsValues.Any())
                                {
                                    sysMappingData.SystemConflictVariables = flagSectionsValues;
                                }
                            }
                            sysVal.Add(sysMappingData);

                        }
                    }
                }

                if (summaryScreenDataSet.Tables.Count >= 2 && summaryScreenDataSet.Tables[1].Rows.Count > 0)
                {
                    var buildingEquipmentStatus = (from DataRow dRow in summaryScreenDataSet.Tables[1].Rows
                                                   select new
                                                   {
                                                       buildingStatus = dRow[Constants.BUILDINGEQUIPMENTSTATUS]
                                                   }).Distinct();
                    var configurationConflictExist = (from DataRow dRow in summaryScreenDataSet.Tables[2].Rows
                                                      select new
                                                      {
                                                          buildingStatus = dRow[Constants.CONFIGURATIONCONFLICTEXIST]
                                                      }).Distinct();
                    _cpqCacheManager.SetCache(sessionId, _environment, setId.ToString(), Constants.BUILDINGEQUIPMENTSTATUSKEY, Utility.SerializeObjectValue(buildingEquipmentStatus.FirstOrDefault()));
                    _cpqCacheManager.SetCache(sessionId, _environment, setId.ToString(), Constants.CONFIGURATIONCONFLICTS, Utility.SerializeObjectValue(configurationConflictExist.FirstOrDefault()));
                }

                foreach (var unitValuesItems in UnitDetails)
                {
                    var unitConflictData = new UnitConflictValues()
                    {
                        UnitId = unitValuesItems.UnitId,
                        UnitName = unitValuesItems.UnitName,
                        ConflictVaribales = new List<SystemValidationConflictsValues>()
                    };
                    foreach (var sysValItems in sysVal)
                    {
                        if (sysValItems.UnitId == unitValuesItems.UnitId)
                        {

                            unitConflictData.ConflictVaribales.Add(sysValItems);

                        }
                    }
                    unitConflictValues.Add(unitConflictData);
                }

                conflicts.ValidationAssignments = unitConflictValues.Distinct().ToList();
                mainRespone.ConflictAssignments = conflicts;
                if (systemStatusFlag.StatusId == 0 && string.IsNullOrEmpty(systemStatusFlag.StatusName))
                {
                    systemStatusFlag.StatusName = string.Empty;
                }
                mainRespone.SystemValidateStatus = systemStatusFlag;
            }
            Utility.LogEnd(methodBegin);
            return mainRespone;
        }

        /// <summary>
        /// GetPermissionByRole
        /// </summary>
        /// <param Name="Id"></param>
        /// <param Name="roleName"></param>
        /// <returns></returns>
        public List<string> GetPermissionByRole(int id, string roleName, string entity = Constants.UNIT_ENTITY)
        {
            var methodBegin = Utility.LogBegin();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            List<string> permission = new List<string>();
            SqlParameter param = new SqlParameter(Constant.@_ID, id);
            sqlParameters.Add(param);
            param = new SqlParameter(Constant.@ROLENAME, roleName);
            sqlParameters.Add(param);
            param = new SqlParameter(Constant.@ENTITY, entity);
            sqlParameters.Add(param);
            DataSet ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETPERMISSIONBYROLENAME, sqlParameters);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var buildingConfigList = (from DataRow dRow in ds.Tables[0].Rows
                                          select new
                                          {
                                              permissions = Convert.ToString(dRow[Constant.PERMISSIONKEY]),
                                          }).Distinct().ToList();
                if (buildingConfigList != null && buildingConfigList.Any())
                {
                    permission = buildingConfigList.Select(x => x.permissions).ToList();
                }



            }
            Utility.LogEnd(methodBegin);
            return permission;
        }

        /// <summary>
        /// GetDetailsForUnits
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        public TP2Summary GetDetailsForUnits(int setId)
        {
            var methodBegin = Utility.LogBegin();
            var unitMapperVariables = Utility.VariableMapper(Constant.UNITSVARIABLESMAPPERPATH, Constant.UNITMAPPER);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            TP2Summary variableAssignments = new TP2Summary();
            DataSet summaryScreenDataSet = new DataSet();
            var val = JArray.Parse(File.ReadAllText(Constant.LISTVARIABLEASSIGNMENTS));

            var mainVariableResObj = Utility.DeserializeObjectValue<List<VariablesList>>(Utility.SerializeObjectValue(val));

            SqlParameter param = new SqlParameter(Constant.SETID, setId);
            sqlParameters.Add(param);
            summaryScreenDataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETDETAILSFORTP2SUMMARY, sqlParameters);
            List<ConfigVariable> buildngConfig = new List<ConfigVariable>();
            List<ConfigVariable> groupVariables = new List<ConfigVariable>();
            List<ConfigVariable> unitVariable = new List<ConfigVariable>();
            List<UnitDetailsForTP2> unitDetailsForTP2 = new List<UnitDetailsForTP2>();
            List<UnitVariablesDetailsForTP2> unitVariablesDetailsList = new List<UnitVariablesDetailsForTP2>();
            List<ConfigVariable> groupUnitVariables = new List<ConfigVariable>();
            if (summaryScreenDataSet != null && summaryScreenDataSet.Tables.Count > 0)
            {

                var buildingConfiguration = summaryScreenDataSet.Tables[0];
                if (buildingConfiguration.Rows.Count > 0)
                {

                    buildngConfig = JsonConvert.DeserializeObject<List<ConfigVariable>>(buildingConfiguration.Rows[0][Constant.BUILDINGJSON].ToString());
                }
                var groupConfiguration = summaryScreenDataSet.Tables[1];
                if (groupConfiguration.Rows.Count > 0)
                {

                    if (summaryScreenDataSet.Tables[1].Rows.Count > 0)
                    {
                        string variableAssignmentJsonString = string.Empty;
                        foreach (DataRow groupVariableAssignment in groupConfiguration.Rows)
                        {
                            if (!(Convert.ToString(groupVariableAssignment[Constant.UNITJSON])).Equals(Constant.EMPTYSTRING))
                            {
                                variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.UNITJSON]) + Constant.COMA;
                            }
                            if (!(Convert.ToString(groupVariableAssignment[Constant.HALLRISERJSON])).Equals(Constant.EMPTYSTRING))
                            {
                                variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.HALLRISERJSON]) + Constant.COMA;
                            }
                            if (!(Convert.ToString(groupVariableAssignment[Constant.DOORJSON])).Equals(Constant.EMPTYSTRING))
                            {
                                variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.DOORJSON]) + Constant.COMA;
                            }
                            if (!(Convert.ToString(groupVariableAssignment[Constant.CONTROLLOCATIONJSON])).Equals(Constant.EMPTYSTRING))
                            {
                                variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.CONTROLLOCATIONJSON]) + Constant.COMA;
                            }
                        }
                        string jsonData = Constant.OPENINGSQUAREBRACKET + variableAssignmentJsonString + Constant.CLOSINGSQUAREBRACKET;
                        groupVariables = JsonConvert.DeserializeObject<List<ConfigVariable>>(jsonData.Replace(Constant.COMA + Constant.CLOSINGSQUAREBRACKET, Constant.CLOSINGSQUAREBRACKET).ToString());
                        groupVariables = groupVariables.GroupBy(group => group.VariableId).Select(g => g.First()).ToList();
                    }
                }

                var unitConfiguration = summaryScreenDataSet.Tables[2];
                if (unitConfiguration.Rows.Count > 0)
                {
                    //create Unit configuration list
                    foreach (DataRow unitConfigurationData in unitConfiguration.Rows)
                    {
                        string unitConfigurationJson = Convert.ToString(unitConfigurationData[Constant.CONFIGUREJSON]);
                        var unitConfig = Utility.DeserializeObjectValue<ConfigVariable>(unitConfigurationJson);
                        unitVariable.Add(unitConfig);
                    }
                }
                var unitDetails = summaryScreenDataSet.Tables[3];
                if (unitDetails.Rows.Count > 0)
                {
                    //create Unit configuration list
                    foreach (DataRow unit in unitDetails.Rows)
                    {
                        UnitDetailsForTP2 unitDetail = new UnitDetailsForTP2();
                        unitDetail.UnitId = Convert.ToInt32((unit[Constant.UNITID]).ToString());
                        unitDetail.UnitName = (unit[Constant.DESIGNATION]).ToString();
                        unitDetail.Ueid = (unit[Constant.UEID_LOWERCASE]).ToString();
                        unitDetail.ProductName = (unit[Constant.PRODUCTNAM]).ToString();
                        unitDetailsForTP2.Add(unitDetail);

                        UnitVariablesDetailsForTP2 unitVariableData = new UnitVariablesDetailsForTP2();
                        unitVariableData.UnitId = Convert.ToInt32((unit[Constant.UNITID]).ToString());
                        unitVariableData.UnitName = (unit[Constant.DESIGNATION]).ToString();
                        unitVariableData.Ueid = (unit[Constant.UEID_LOWERCASE]).ToString();
                        unitVariableData.ProductName = (unit[Constant.PRODUCTNAM]).ToString();
                        unitVariableData.VariablesDetails = buildngConfig;
                        unitVariableData.VariablesDetails.AddRange(unitVariable);

                        if (groupConfiguration.Rows.Count > 0)
                        {

                            if (summaryScreenDataSet.Tables[1].Rows.Count > 0)
                            {
                                string variableAssignmentJsonString = string.Empty;
                                foreach (DataRow groupVariableAssignment in groupConfiguration.Rows)
                                {
                                    if (unitDetail.UnitId == Convert.ToInt32(groupVariableAssignment[Constant.UNITID].ToString()))
                                    {
                                        if (!(Convert.ToString(groupVariableAssignment[Constant.UNITJSON])).Equals(Constant.EMPTYSTRING))
                                        {
                                            variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.UNITJSON]) + Constant.COMA;
                                        }
                                        if (!(Convert.ToString(groupVariableAssignment[Constant.HALLRISERJSON])).Equals(Constant.EMPTYSTRING))
                                        {
                                            variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.HALLRISERJSON]) + Constant.COMA;
                                        }
                                        if (!(Convert.ToString(groupVariableAssignment[Constant.DOORJSON])).Equals(Constant.EMPTYSTRING))
                                        {
                                            variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.DOORJSON]) + Constant.COMA;
                                        }
                                        if (!(Convert.ToString(groupVariableAssignment[Constant.CONTROLLOCATIONJSON])).Equals(Constant.EMPTYSTRING))
                                        {
                                            variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.CONTROLLOCATIONJSON]) + Constant.COMA;
                                        }
                                    }
                                }
                                string jsonData = Constant.OPENINGSQUAREBRACKET + variableAssignmentJsonString + Constant.CLOSINGSQUAREBRACKET;
                                groupUnitVariables = JsonConvert.DeserializeObject<List<ConfigVariable>>(jsonData.Replace(Constant.COMA + Constant.CLOSINGSQUAREBRACKET, Constant.CLOSINGSQUAREBRACKET).ToString());
                                groupUnitVariables = groupUnitVariables.GroupBy(group => group.VariableId).Select(g => g.First()).ToList();
                                unitVariableData.VariablesDetails.AddRange(groupUnitVariables);
                            }
                        }

                        unitVariablesDetailsList.Add(unitVariableData);
                    }
                }
                var groupId = summaryScreenDataSet.Tables[4];
                if (groupId.Rows.Count > 0)
                {
                    foreach (DataRow groupid in groupId.Rows)
                    {
                        variableAssignments.GroupId = Convert.ToInt32((groupid[Constant.GROUPCONFIGIDVALUE]).ToString());
                    }
                }

                var travel = summaryScreenDataSet.Tables[5];
                if (travel.Rows.Count > 0)
                {
                    var travelVariableAssignments = new VariableAssignment();
                    travelVariableAssignments.VariableId = unitMapperVariables[Constant.TRAVELVARIABLEIDVALUE];
                    int travelFeet = 0;
                    decimal travelinch = 0;
                    foreach (DataRow travelFeetAndInch in travel.Rows)
                    {
                        if (!(Convert.ToString(travelFeetAndInch[Constant.TRAVELFEET])).Equals(Constant.EMPTYSTRING))
                        {
                            travelFeet = Convert.ToInt32(travelFeetAndInch[Constant.TRAVELFEET]);
                        }
                        if (!(Convert.ToString(travelFeetAndInch[Constant.TRAVELINCH])).Equals(Constant.EMPTYSTRING))
                        {
                            travelinch = Convert.ToInt32(travelFeetAndInch[Constant.TRAVELINCH]);
                        }

                    }
                    travelVariableAssignments.Value = travelFeet * 12 + travelinch;

                    variableAssignments.TravelVariableAssignments = travelVariableAssignments;
                }
            }
            variableAssignments.UnitDetails = unitDetailsForTP2;

            foreach (var item in mainVariableResObj)
            {
                switch (item.Id)
                {
                    case Constant.BUILDINGVARIABLESLIST:
                        item.VariableAssignments = buildngConfig;
                        break;
                    case Constant.GROUPVARIABLESLIST:
                        item.VariableAssignments = groupVariables;
                        break;
                    case Constant.UNITVARIABLESLIST:
                        item.VariableAssignments = unitVariable;
                        break;
                    default:
                        item.VariableAssignments = unitVariable;
                        break;
                }
            }

            variableAssignments.VariableAssignments = mainVariableResObj;
            variableAssignments.UnitLevelVariables = unitVariablesDetailsList;
            Utility.LogEnd(methodBegin);
            return variableAssignments;
        }



        /// <summary>
        /// method to save price details
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="listOfDetails"></param>
        /// <param Name="userId"></param>
        /// <param Name="loghistory"></param>
        /// <returns></returns>
        public List<ResultSetConfiguration> SavePriceValuesDL(int setId, List<ConfigVariable> listOfDetails, List<ConfigVariable> listOfLeadTimes, string userId, List<LogHistoryTable> loghistory, List<UnitNames> unitPrices)
        {
            var methodBegin = Utility.LogBegin();
            ResultSetConfiguration result = new ResultSetConfiguration();
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();

            DataTable unitDataTable = Utility.GenerateDataTableForUnitConfiguration(listOfDetails);
            DataTable unitDataTableForLeadTime = Utility.GenerateDataTableForUnitConfiguration(listOfLeadTimes);
            DataTable unitPricesTable = Utility.GenerateDataTableForUnitPrices(unitPrices);
            List<SqlParameter> lstSqlParameter = Utility.SqlParameterForSavePriceValues(setId, unitDataTable, unitDataTableForLeadTime, userId, unitPricesTable);
            int resultForSaveUnitConfiguration = CpqDatabaseManager.ExecuteNonquery(Constant.SPSAVEPRICEDETAILS, lstSqlParameter);
            if (resultForSaveUnitConfiguration > 0)
            {
                result.result = 1;
                result.setId = resultForSaveUnitConfiguration;
                result.message = Constant.PRICEDETAILSSAVEMESSAGE;
            }
            else if (resultForSaveUnitConfiguration == 0)
            {
                result.result = 0;
                result.setId = resultForSaveUnitConfiguration;
                result.message = Constant.PRICEDETAILSSAVEERROR;
            }
            lstResult.Add(result);
            return lstResult;
        }


        /// <summary>
        /// method for Get Product Category By GroupId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetProductCategoryByGroupId(int id, string type, DataTable configVariables)
        {
            var methodBeginTime = Utility.LogBegin();
            string productCategory = string.Empty;
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.@_ID, id),
                new SqlParameter(Constant.@TYPE, type),
                new SqlParameter(Constant.CONSTANTMAPPERLIST, configVariables)
            };

            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETPRODUCTCATEGORYBYGROUPID, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                productCategory = Convert.ToString(ds.Tables[0].Rows[0][Constant.GROUPCONFIGURATIONVALUE]);
            }
            Utility.LogEnd(methodBeginTime);
            return productCategory;
        }

        /// <summary>
        /// Method for Get Variable Assignments SetId
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        public List<ConfigVariable> GetVariableAssignmentsBySetId(int setId, DataTable configAssignments)
        {
            var methodBeginTime = Utility.LogBegin();
            List<ConfigVariable> configVariables = new List<ConfigVariable>();
            DataSet ds = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.@UNITSETID, setId),
                new SqlParameter(Constant.CONSTANTMAPPERLIST, configAssignments)
            };

            ds = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETUNITVARIABLEASSIGNMENTSBYSETID, param);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                configVariables = (from DataRow row in ds.Tables[0].Rows
                                   select new ConfigVariable
                                   {
                                       VariableId = Convert.ToString(row[Constant.VARIABLEID]),
                                       Value = Convert.ToString(row[Constant.VALUE]),
                                   }).ToList().GroupBy(x => x.VariableId).Select(x => x.FirstOrDefault()).ToList();
            }
            Utility.LogEnd(methodBeginTime);
            return configVariables;
        }

        /// <summary>
        /// Method for Save Non Configurable Products
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="listOfDetails"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<ResultSetConfiguration> SaveNonConfigurableUnitConfigurationDL(int setId, List<ConfigVariable> listOfDetails, string userId, DataTable configVariables)
        {
            var methodBegin = Utility.LogBegin();
            ResultSetConfiguration result = new ResultSetConfiguration();
            List<ResultSetConfiguration> lstResult = new List<ResultSetConfiguration>();
            DataTable unitDataTable = Utility.GenerateDataTableForUnitConfiguration(listOfDetails);
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForNonConfigurableSaveUnitConfiguration(setId, unitDataTable, userId, configVariables);
            int resultForSaveUnitConfiguration = CpqDatabaseManager.ExecuteNonquery(Constant.SAVEUNITCONFIGURATIONFORNCP, lstSqlParameter);
            if (resultForSaveUnitConfiguration > 0)
            {
                result.result = 1;
                result.setId = resultForSaveUnitConfiguration;
                result.message = Constant.UNITCONFIGURATIONSAVEMESSAGE;
            }
            else if (resultForSaveUnitConfiguration == 0)
            {
                result.result = 0;
                result.setId = resultForSaveUnitConfiguration;
                result.message = Constant.UNITCONFIGURATIONSAVEERRORMESSAGE;
            }
            lstResult.Add(result);
            Utility.LogEnd(methodBegin);
            return lstResult;
        }



        public ConfigurationResponse GetLatestSystemsValValues(int setId, string statusKey, string userId, List<SystemValidationKeyValues> systemKeyValues, string type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method for Get Product type
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        public string GetProductType(int setId)
        {
            var methodBegin = Utility.LogBegin();
            List<SqlParameter> sqlParam = new List<SqlParameter>();
            SqlParameter param = new SqlParameter(Constant.SETID, setId);
            sqlParam.Add(param);
            var productType = CpqDatabaseManager.ExecuteScalarForReturnString(Constant.SPGETPRODUCTTYPE, sqlParam);
            Utility.LogEnd(methodBegin);
            return productType;
        }

        private List<ChangeLog> GetChangeLogData(DataTable changeLogDataTable)
        {
            List<ChangeLog> changeLog = new List<ChangeLog>();
            var changedData = (from DataRow row in changeLogDataTable.Rows
                               select new
                               {
                                   VariableId = Convert.ToString(row[Constant.VARIABLEID]),
                                   CurrentValue = Convert.ToString(row[Constant.CURRENTVALUE]),
                                   PreviousValue = Convert.ToString(row[Constant.PREVIOUSVALUE]),
                                   CreatedBy = Convert.ToString(row[Constant.PROJECTCOLUMNCREATEDBY]),
                                   CreatedOn = Convert.ToDateTime(row[Constant.PROJECTCREATEDON])
                               }).ToList();
            foreach (var logData in changedData)
            {
                ChangeLog changedValue = new ChangeLog()
                {
                    User = logData.CreatedBy,
                    ChangedDate = logData.CreatedOn
                };
                if (string.IsNullOrEmpty(logData.CurrentValue))
                {
                    changedValue.ChangeType = Constant.REMOVED;
                    changedValue.Data = logData.VariableId + Constant.COLON + logData.PreviousValue;
                }
                else if (string.IsNullOrEmpty(logData.PreviousValue))
                {
                    changedValue.ChangeType = Constant.ADDED;
                    changedValue.Data = logData.VariableId + Constant.COLON + logData.CurrentValue;
                }
                else
                {
                    changedValue.ChangeType = Constant.MODIFIED;
                    changedValue.Data = logData.VariableId + Constant.COLON + logData.CurrentValue;
                }
                changeLog.Add(changedValue);
            }
            return changeLog;
        }
        private List<QuoteSummary> GetQuoteSummary(DataTable quoteSummary)
        {
            var quoteSummaryList = new List<QuoteSummary>();

            var productNameMapperJson = Utility.VariableMapper(Constant.PROJECTCONSTANTMAPPER, Constant.PROJECTCOMMONNAME);
            var buildingDataList = (from DataRow row in quoteSummary.Rows
                                    select new
                                    {
                                        BuildingId = Convert.ToString(row[Constant.BUILDINGIDCOLUMNNAME]),
                                        BuildingName = Convert.ToString(row[Constant.BUILDINGNAMECOLUMNNAME])
                                    }).Distinct();
            var buildingIDList = (from DataRow row in quoteSummary.Rows
                                  select new
                                  {
                                      BuildingId = Convert.ToString(row[Constant.BUILDINGIDCOLUMNNAME])
                                  }).Distinct();
            var groupDataList = (from DataRow row in quoteSummary.Rows
                                 select new
                                 {
                                     BuildingId = Convert.ToString(row[Constant.BUILDINGIDCOLUMNNAME]),
                                     GroupId = Convert.ToString(row[Constant.GROUPCONFIGIDVALUE]),
                                     SetId = Convert.ToString(row[Constant.SETCONFIGURATIONID]),
                                     GroupName = Convert.ToString(row[Constant.GROUPNAMERLSINFO_CAMELCASE])
                                 }).Distinct();
            var groupIdList = (from DataRow row in quoteSummary.Rows
                               select new
                               {
                                   BuildingId = Convert.ToString(row[Constant.BUILDINGIDCOLUMNNAME]),
                                   GroupId = Convert.ToString(row[Constant.GROUPCONFIGIDVALUE])
                               }).Distinct();
            var setIdList = (from DataRow row in quoteSummary.Rows
                             select new
                             {
                                 GroupId = Convert.ToString(row[Constant.GROUPCONFIGIDVALUE]),
                                 SetId = Convert.ToString(row[Constant.SETCONFIGURATIONID]),
                                 ProductName = Convert.ToString(row[Constant.PRODUCTNAM])
                             }).Distinct();
            var unitDataList = (from DataRow row in quoteSummary.Rows
                                select new
                                {
                                    SetId = Convert.ToString(row[Constant.SETCONFIGURATIONID]),
                                    GroupId = Convert.ToString(row[Constant.GROUPCONFIGIDVALUE]),
                                    UnitName = Convert.ToString(row[Constant.DESIGNATION]),
                                }).Distinct();
            var productList = (from DataRow row in quoteSummary.Rows
                               select new
                               {
                                   SetId = Convert.ToString(row[Constant.SETCONFIGURATIONID]),
                                   GroupId = Convert.ToString(row[Constant.GROUPCONFIGIDVALUE]),
                                   ProductName = Convert.ToString(row[Constant.PRODUCTNAM])
                               }).Distinct();
            foreach (var buildingId in buildingIDList)
            {
                var buildingDataListForBuilding = buildingDataList.Where(x => x.BuildingId == buildingId.BuildingId).ToList();
                foreach (var building in buildingDataListForBuilding)
                {
                    var groupIdListForBuilding = groupIdList.Where(x => x.BuildingId == building.BuildingId).ToList();
                    foreach (var groupId in groupIdListForBuilding)
                    {
                        var groupListForbuilding = groupDataList.Where(x => x.GroupId == groupId.GroupId).ToList().GroupBy(x => x.GroupId).Select(y => y.First()).ToList();

                        foreach (var group in groupListForbuilding)
                        {
                            var setIdListForGroup = setIdList.Where(x => x.GroupId.Equals(group.GroupId)).ToList().GroupBy(x => x.SetId).Select(y => y.First()).ToList();
                            foreach (var set in setIdListForGroup)
                            {
                                var unitListForSet = unitDataList.Where(x => x.SetId == set.SetId && x.GroupId == set.GroupId).ToList();
                                StringBuilder unitName = new StringBuilder();
                                foreach (var unit in unitListForSet)
                                {
                                    unitName.Append(unit.UnitName + Constant.EMPTYSPACE + Constant.COMA);
                                }
                                unitName.Append(Constant.CLOSINGSQUAREBRACKET);
                                var buildingData = new QuoteSummary()
                                {
                                    ProductLine = string.IsNullOrEmpty(set.ProductName) ? "  " : productNameMapperJson.ContainsKey(set.ProductName) ? productNameMapperJson[set.ProductName] : set.ProductName,
                                    BuildingName = string.IsNullOrEmpty(building.BuildingName) ? "  " : building.BuildingName,
                                    BuildingId = string.IsNullOrEmpty(building.BuildingId) ? "  " : building.BuildingId,
                                    GroupName = string.IsNullOrEmpty(group.GroupName) ? "  " : group.GroupName,
                                    GroupId = string.IsNullOrEmpty(group.GroupId) ? "  " : group.GroupId,
                                    UnitName = unitName.Replace(Constant.CLOSINGSQUAREBRACKETWITHCOMA, string.Empty).ToString(),
                                    NumberOfUnits = string.IsNullOrEmpty(Convert.ToString(unitListForSet.Count)) ? "  " : Convert.ToString(unitListForSet.Count),
                                    OrderType = "CTO",
                                    SetId = string.IsNullOrEmpty(set.SetId) ? Convert.ToString(0) : set.SetId
                                };
                                quoteSummaryList.Add(buildingData);
                            }

                        }
                    }
                }
            }
            return quoteSummaryList;
        }

        private ProjectData GetProjectData(DataTable projectInfo)
        {
            var projectData = new ProjectData();
            var projectDataList = (from DataRow row in projectInfo.Rows
                                   select new
                                   {
                                       OpportunityId = Convert.ToString(row[Constant.OPPORTUNITY]),
                                       VersionId = Convert.ToString(row[Constant.VERSIONIDFORCRM]),
                                       CreatedOn = Convert.ToString(row[Constant.CREATEDONCRM]),
                                       AccountName = Convert.ToString(row[Constant.CREATEPROJECTCOLUMNACCOUNTNAME]),
                                       AddressLine1 = Convert.ToString(row[Constant.CREATEPROJECTCOLUMNADDRESSLINE1]),
                                       AddressLine2 = Convert.ToString(row[Constant.CREATEPROJECTCOLUMNADDRESSLINE2]),
                                       City = Convert.ToString(row[Constant.CREATEPROJECTCOLUMNCITY]),
                                       State = Convert.ToString(row[Constant.CREATEPROJECTCOLUMNSTATE]),
                                       Country = Convert.ToString(row[Constant.CREATEPROJECTCOLUMNCOUNTRY]),
                                       ZipCode = Convert.ToString(row[Constant.CREATEPROJECTCOLUMNZIPCODE]),
                                       CustomerNumber = Convert.ToString(row[Constant.CREATEPROJECTCOLUMNCUSTOMERNUMBER]),
                                       StatusName = Convert.ToString(row[Constant.STATUSNAME]),
                                       PROJECTQUOTESTATUSKEY = Convert.ToString(Constant.PROJECTQUOTESTATUSKEY),
                                       QuoteStatus = Convert.ToString(row[Constant.PROJECTCOLUMNQUOTESTATUS])
                                   }).ToList().Distinct();
            foreach (var projectDataVariable in projectDataList)
            {
                string address = projectDataVariable.AddressLine1 + System.Environment.NewLine + projectDataVariable.AddressLine2 + System.Environment.NewLine + projectDataVariable.City + System.Environment.NewLine + projectDataVariable.Country + System.Environment.NewLine + projectDataVariable.ZipCode;
                projectData = new ProjectData()
                {
                    OpportunityId = projectDataVariable.OpportunityId,
                    VersionId = projectDataVariable.VersionId,
                    QuoteDate = projectDataVariable.CreatedOn,
                    Contact = projectDataVariable.CustomerNumber,
                    ProjectStatus = projectDataVariable.StatusName,
                    SalesStage = projectDataVariable.PROJECTQUOTESTATUSKEY,
                    BookeDate = projectDataVariable.CreatedOn,
                    CustomerAccount = projectDataVariable.AccountName,
                    Address = Convert.ToString(address),
                    QuoteStatus = projectDataVariable.QuoteStatus
                };
            }
            return projectData;
        }
        public List<CustomPriceLine> SaveCustomPriceLine(int setId, string userId, List<CustomPriceLine> customPriceLine)
        {
            var methodBegin = Utility.LogBegin();
            var customPriceDataTable = GenerateCustomPriceKeyDataTableForSave(customPriceLine, userId);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@setId",Value=setId,Direction = ParameterDirection.Input },
                new SqlParameter() { ParameterName = "@customPriceLine",Value=customPriceDataTable,Direction = ParameterDirection.Input }
                ,new SqlParameter() { ParameterName = "@Result",Value=0,Direction = ParameterDirection.Output }
            };
            var priceLineId = CpqDatabaseManager.ExecuteDataSet(Constants.SPUPSERTPRICELINE, sqlParameters);
            if (priceLineId != null && priceLineId.Tables.Count > 0 && priceLineId.Tables[0].Rows.Count > 0)
            {
                var priceKeyList = (from DataRow row in priceLineId.Tables[0].Rows
                                    select new
                                    {
                                        Id = Convert.ToInt32(row[Constants.ID]),
                                        SectionId = Convert.ToString(row[Constants.SECTIONID]),
                                        ComponentName = Convert.ToString(row[Constants.COMPONENTNAME]),
                                        Description = Convert.ToString(row[Constants.DESCRIPTION]),
                                        ItemNumber = Convert.ToString(row[Constants.ITEMNUMBER])
                                    }).ToList();
                foreach (var priceKey in priceKeyList)
                {
                    foreach (var priceLine in customPriceLine)
                    {
                        priceLine.priceKeyInfo.IsCustomPriceLine = true;
                        if (Utility.CheckEquals(priceLine.priceKeyInfo.Section, priceKey.SectionId)
                            && Utility.CheckEquals(priceLine.priceKeyInfo.ComponentName, priceKey.ComponentName)
                            && Utility.CheckEquals(priceLine.priceKeyInfo.PartDescription, priceKey.Description))
                        {
                            var unitPriceValue = priceLine.PriceValue.ToList()[0].Value;
                            priceLine.priceKeyInfo.ItemNumber = priceKey.ItemNumber;
                            priceLine.priceKeyInfo.PriceKeyId = priceKey.Id;
                            priceLine.priceKeyInfo.qty = unitPriceValue.quantity;
                            Dictionary<string, UnitPriceValues> priveValeDictionary = new Dictionary<string, UnitPriceValues>();
                            unitPriceValue.totalPrice = unitPriceValue.unitPrice * unitPriceValue.quantity;
                            priveValeDictionary.Add(priceKey.ItemNumber, unitPriceValue);
                            priceLine.PriceValue = priveValeDictionary;
                        }
                    }
                }
            }
            Utility.LogEnd(methodBegin);
            return customPriceLine;
        }
        public CustomPriceLine EditCustomPriceLine(int setId, string sessionId, CustomPriceLine customPriceLine)
        {
            var methodBegin = Utility.LogBegin();
            var customPriceDataTable = GenerateCustomPriceKeyDataTable(customPriceLine);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@setId",Value=setId,Direction = ParameterDirection.Input },
                new SqlParameter() { ParameterName = "@customPriceLine",Value=customPriceDataTable,Direction = ParameterDirection.Input }
                ,new SqlParameter() { ParameterName = "@Result",Value=0,Direction = ParameterDirection.Output }
            };
            var priceLineId = CpqDatabaseManager.ExecuteNonquery(Constants.SPUPSERTPRICELINE, sqlParameters);
            customPriceLine.priceKeyInfo.PriceKeyId = priceLineId;
            customPriceLine.priceKeyInfo.IsCustomPriceLine = true;
            Dictionary<string, UnitPriceValues> priveValeDictionary = new Dictionary<string, UnitPriceValues>();
            var unitPriceValue = customPriceLine.PriceValue.ToList()[0].Value;
            unitPriceValue.totalPrice = unitPriceValue.unitPrice * unitPriceValue.quantity;
            priveValeDictionary.Add(customPriceLine.PriceValue.ToList()[0].Key, unitPriceValue);
            Utility.LogEnd(methodBegin);
            return customPriceLine;
        }
        public int DeleteCustomPriceLine(int setId, int priceLineId)
        {
            var methodBegin = Utility.LogBegin();
            IList<SqlParameter> sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = "@setId",Value=setId,Direction = ParameterDirection.Input },
                new SqlParameter() { ParameterName = "@priceLineId",Value=priceLineId,Direction = ParameterDirection.Input }
                ,new SqlParameter() { ParameterName = "@Result",Value=0,Direction = ParameterDirection.Output }
            };
            var priceLine = CpqDatabaseManager.ExecuteNonquery(Constants.SPDELETEPRICELINE, sqlParameters);
            Utility.LogEnd(methodBegin);
            return 1;
        }
        private DataTable GenerateCustomPriceKeyDataTable(CustomPriceLine customPriceLine)
        {
            DataTable priceKeyDataTable = new DataTable();
            DataColumn priceKeyId = new DataColumn("Id")
            {
                DataType = typeof(int),
                DefaultValue = customPriceLine.priceKeyInfo.PriceKeyId
            };
            priceKeyDataTable.Columns.Add(priceKeyId);
            DataColumn SectionId = new DataColumn("SectionId")
            {
                DataType = typeof(int),
                DefaultValue = customPriceLine.priceKeyInfo.Section
            };
            priceKeyDataTable.Columns.Add(SectionId);
            DataColumn ItemNumber = new DataColumn("ItemNumber")
            {
                DataType = typeof(string),
                DefaultValue = customPriceLine.priceKeyInfo.ItemNumber
            };
            priceKeyDataTable.Columns.Add(ItemNumber);
            DataColumn ComponentName = new DataColumn("ComponentName")
            {
                DataType = typeof(string),
                DefaultValue = customPriceLine.priceKeyInfo.ComponentName
            };
            priceKeyDataTable.Columns.Add(ComponentName);
            DataColumn Description = new DataColumn("Description")
            {
                DataType = typeof(string),
                DefaultValue = customPriceLine.priceKeyInfo.PartDescription
            };
            priceKeyDataTable.Columns.Add(Description);
            DataColumn Quantity = new DataColumn("Quantity")
            {
                DataType = typeof(int),
                DefaultValue = customPriceLine.PriceValue.ToList()[0].Value.quantity
            };
            priceKeyDataTable.Columns.Add(Quantity);

            DataColumn Unit = new DataColumn("Unit")
            {
                DataType = typeof(string),
                DefaultValue = customPriceLine.PriceValue.ToList()[0].Value.Unit
            };
            priceKeyDataTable.Columns.Add(Unit);

            DataColumn UnitPrice = new DataColumn("UnitPrice")
            {
                DataType = typeof(decimal),
                DefaultValue = customPriceLine.PriceValue.ToList()[0].Value.unitPrice
            };
            priceKeyDataTable.Columns.Add(UnitPrice);
            DataColumn LeadTime = new DataColumn("LeadTime")
            {
                DataType = typeof(string),
                DefaultValue = string.IsNullOrEmpty(customPriceLine.priceKeyInfo.LeadTime) ? 0 : Convert.ToInt32(customPriceLine.priceKeyInfo.LeadTime)
            };
            priceKeyDataTable.Columns.Add(LeadTime);
            DataColumn UserId = new DataColumn("UserId")
            {
                DataType = typeof(string),
                DefaultValue = customPriceLine.UserId
            };
            priceKeyDataTable.Columns.Add(UserId);
            DataRow dataRow = priceKeyDataTable.NewRow();
            dataRow[0] = customPriceLine.priceKeyInfo.PriceKeyId;
            dataRow[1] = customPriceLine.priceKeyInfo.Section;
            dataRow[2] = customPriceLine.priceKeyInfo.ItemNumber;
            dataRow[3] = customPriceLine.priceKeyInfo.ComponentName;
            dataRow[4] = customPriceLine.priceKeyInfo.PartDescription;
            dataRow[5] = customPriceLine.PriceValue != null && customPriceLine.PriceValue.ToList().Any() ?
                customPriceLine.PriceValue.ToList()[0].Value.quantity : 0;
            dataRow[6] = customPriceLine.PriceValue != null && customPriceLine.PriceValue.ToList().Any()
                && !string.IsNullOrEmpty(customPriceLine.PriceValue.ToList()[0].Value.Unit) ?
                customPriceLine.PriceValue.ToList()[0].Value.Unit : Constants.CURRENCYCODE;
            dataRow[7] = customPriceLine.PriceValue != null && customPriceLine.PriceValue.ToList().Any() ?
                customPriceLine.PriceValue.ToList()[0].Value.unitPrice : 0;
            dataRow[8] = string.IsNullOrEmpty(customPriceLine.priceKeyInfo.LeadTime) ? 0 : Convert.ToInt32(customPriceLine.priceKeyInfo.LeadTime);
            dataRow[9] = customPriceLine.UserId;
            priceKeyDataTable.Rows.Add(dataRow);
            return priceKeyDataTable;
        }


        private DataTable GenerateCustomPriceKeyDataTableForSave(List<CustomPriceLine> customPriceLine, string userId)
        {
            DataTable priceKeyDataTable = new DataTable();
            DataColumn priceKeyId = new DataColumn("Id")
            {
                DataType = typeof(int)
            };
            priceKeyDataTable.Columns.Add(priceKeyId);
            DataColumn SectionId = new DataColumn("SectionId")
            {
                DataType = typeof(int)
            };
            priceKeyDataTable.Columns.Add(SectionId);
            DataColumn ItemNumber = new DataColumn("ItemNumber")
            {
                DataType = typeof(string)
            };
            priceKeyDataTable.Columns.Add(ItemNumber);
            DataColumn ComponentName = new DataColumn("ComponentName")
            {
                DataType = typeof(string)
            };
            priceKeyDataTable.Columns.Add(ComponentName);
            DataColumn Description = new DataColumn("Description")
            {
                DataType = typeof(string)
            };
            priceKeyDataTable.Columns.Add(Description);
            DataColumn Quantity = new DataColumn("Quantity")
            {
                DataType = typeof(int)
            };
            priceKeyDataTable.Columns.Add(Quantity);

            DataColumn Unit = new DataColumn("Unit")
            {
                DataType = typeof(string)
            };
            priceKeyDataTable.Columns.Add(Unit);

            DataColumn UnitPrice = new DataColumn("UnitPrice")
            {
                DataType = typeof(decimal)
            };
            priceKeyDataTable.Columns.Add(UnitPrice);
            DataColumn LeadTime = new DataColumn("LeadTime")
            {
                DataType = typeof(string)
            };
            priceKeyDataTable.Columns.Add(LeadTime);
            DataColumn UserId = new DataColumn("UserId")
            {
                DataType = typeof(string)
            };
            priceKeyDataTable.Columns.Add(UserId);
            foreach (var priceLine in customPriceLine)
            {
                DataRow dataRow = priceKeyDataTable.NewRow();
                dataRow[0] = priceLine.priceKeyInfo.PriceKeyId;
                dataRow[1] = priceLine.priceKeyInfo.Section;
                dataRow[2] = priceLine.priceKeyInfo.ItemNumber;
                dataRow[3] = priceLine.priceKeyInfo.ComponentName;
                dataRow[4] = priceLine.priceKeyInfo.PartDescription;
                dataRow[5] = priceLine.PriceValue != null && priceLine.PriceValue.ToList().Any() ? priceLine.PriceValue.ToList()[0].Value.quantity : 0;
                dataRow[6] = priceLine.PriceValue != null && priceLine.PriceValue.ToList().Any() && !string.IsNullOrEmpty(priceLine.PriceValue.ToList()[0].Value.Unit) ?
                    priceLine.PriceValue.ToList()[0].Value.Unit : Constants.CURRENCYCODE;
                dataRow[7] = priceLine.PriceValue != null && priceLine.PriceValue.ToList().Any() ? priceLine.PriceValue.ToList()[0].Value.unitPrice : 0;
                dataRow[8] = string.IsNullOrEmpty(priceLine.priceKeyInfo.LeadTime) ? 0 : Convert.ToInt32(priceLine.priceKeyInfo.LeadTime);
                dataRow[9] = string.IsNullOrEmpty(userId) ? string.Empty : userId;
                priceKeyDataTable.Rows.Add(dataRow);
            }
            return priceKeyDataTable;
        }
        /// <summary>
        /// Method for Get Variables for Hoistway Wiring
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        public List<ConfigVariable> GetDetailsForHoistwayWiring(string valData, int setId, string sessionId, int unitId)
        {
            var methodBegin = Utility.LogBegin();
            var unitMapperVariables = Utility.VariableMapper(Constant.UNITSVARIABLESMAPPERPATH, Constant.UNITMAPPER);
            IList<SqlParameter> sqlParameters = new List<SqlParameter>();
            TP2Summary variableAssignments = new TP2Summary()
            {
                ChangedData = new List<ChangeLog>(),
                QuoteSummary = new List<QuoteSummary>(),
                FloorMatrixTable = new List<PriceSectionDetails>()
            };
            var VariableId = unitMapperVariables[Constants.NUMBEROFFLOORSUNITPACKAGEVARIABLES];
            DataSet summaryScreenDataSet = new DataSet();
            var val = JArray.Parse(File.ReadAllText(Constant.LISTVARIABLEASSIGNMENTS));
            var mainVariableResObj = Utility.DeserializeObjectValue<List<VariablesList>>(Utility.SerializeObjectValue(val));
            SqlParameter param = new SqlParameter(Constant.SETID, setId);
            sqlParameters.Add(param);
            summaryScreenDataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETDETAILSFORTP2SUMMARY, sqlParameters);
            List<ConfigVariable> variableAssignmentValues = new List<ConfigVariable>();
            if (summaryScreenDataSet != null && summaryScreenDataSet.Tables.Count > 0)
            {
                if (valData == null)
                {
                    var carposVariable = summaryScreenDataSet.Tables[26];
                    if (carposVariable != null)
                    {
                        if (carposVariable.Rows.Count > 0)
                        {

                            var getunitGroupConsoleList = (from DataRow row in summaryScreenDataSet.Tables[26].Rows
                                                           select new
                                                           {
                                                               unitId = row["UnitId"].ToString(),
                                                               name = row["name"].ToString(),
                                                               MappedLocation = row["location"].ToString()
                                                           }).ToList();
                            var lowestUnitId = 0;
                            if (unitId != null && unitId > 0)
                            {
                                lowestUnitId = unitId;
                            }
                            else
                            {
                                lowestUnitId = Convert.ToInt32((from openingData in getunitGroupConsoleList select openingData?.unitId)?.ToList()?.Min());
                            }
                            _cpqCacheManager.SetCache(sessionId, _environment, setId.ToString(), Constants.SETEXCEPTIONCARPOSITION, Utility.SerializeObjectValue(getunitGroupConsoleList));
                            List<ConfigVariable> valD = new List<ConfigVariable>();
                            foreach (var opening in getunitGroupConsoleList)
                            {
                                if (opening.unitId.Equals(lowestUnitId.ToString()))
                                {
                                    ConfigVariable newVar = new ConfigVariable();
                                    newVar.VariableId = unitMapperVariables[Constants.CARPOSITIONVAL];
                                    newVar.Value = opening.MappedLocation;
                                    valD.Add(newVar);
                                }
                            }
                            variableAssignmentValues.AddRange(valD);
                        }
                    }
                    var controlLanding = summaryScreenDataSet.Tables[1];
                    if (controlLanding.Rows.Count > 0)
                    {
                        ConfigVariable newVar = new ConfigVariable();
                        newVar.VariableId = unitMapperVariables[Constants.CONTROLLERLANDINGVAL];
                        string variableAssignmentJsonString = string.Empty;
                        foreach (DataRow groupVariableAssignment in controlLanding.Rows)
                        {
                            if (!(Convert.ToString(groupVariableAssignment[Constant.CONTROLLOCATIONJSON])).Equals(Constant.EMPTYSTRING))
                            {
                                variableAssignmentJsonString += Convert.ToString(groupVariableAssignment[Constant.CONTROLLOCATIONJSON]) + Constant.COMA;
                            }
                        }
                        string jsonData = Constant.OPENINGSQUAREBRACKET + variableAssignmentJsonString + Constant.CLOSINGSQUAREBRACKET;
                        var groupVariables = JsonConvert.DeserializeObject<List<ConfigVariable>>(jsonData.Replace(Constant.COMA + Constant.CLOSINGSQUAREBRACKET, Constant.CLOSINGSQUAREBRACKET).ToString());
                        groupVariables = groupVariables.GroupBy(group => group.VariableId).Select(g => g.First()).ToList();
                        newVar.VariableId = unitMapperVariables[Constants.CONTROLLERLANDINGVAL];
                        newVar.Value = 0;
                        foreach (var controls in groupVariables)
                        {
                            if (controls.VariableId.Equals(unitMapperVariables[Constants.CONTROLROOMFLOOR]))
                            {
                                newVar.Value = controls.Value;
                            }
                        }
                        variableAssignmentValues.Add(newVar);
                    }
                    else
                    {
                        ConfigVariable newVar = new ConfigVariable();
                        newVar.VariableId = unitMapperVariables[Constants.CONTROLLERLANDINGVAL];
                        newVar.Value = 0;
                        variableAssignmentValues.Add(newVar);
                    }
                    var blandingVariable = summaryScreenDataSet.Tables[30];
                    if (blandingVariable != null)
                    {
                        if (blandingVariable.Rows.Count > 0)
                        {

                            var blandingdata = (from DataRow row in summaryScreenDataSet.Tables[30].Rows
                                                select new
                                                {
                                                    buildingType = Convert.ToString(row[Constants.BuildingType]),
                                                    buildingValue = Convert.ToString(row[Constants.BuildingValue])
                                                }).ToList();
                            if (blandingdata != null)
                            {
                                var blanding = (from dat in blandingdata where dat.buildingType.Equals(unitMapperVariables[Constants.NUMBEROFFLOORS]) select dat).FirstOrDefault();
                                List<ConfigVariable> valD = new List<ConfigVariable>();

                                ConfigVariable newVar = new ConfigVariable();
                                if (blanding != null)
                                {
                                    newVar.VariableId = unitMapperVariables[Constants.NUMBEROFFLOORSUNITPACKAGEVARIABLES];
                                    newVar.Value = blanding.buildingValue;
                                }
                                else
                                {
                                    newVar.VariableId = unitMapperVariables[Constants.NUMBEROFFLOORSUNITPACKAGEVARIABLES];
                                    newVar.Value = 0;
                                }
                                variableAssignmentValues.Add(newVar);
                            }
                            else
                            {
                                ConfigVariable newVar = new ConfigVariable();
                                newVar.VariableId = unitMapperVariables[Constants.NUMBEROFFLOORSUNITPACKAGEVARIABLES];
                                newVar.Value = 0;
                                variableAssignmentValues.Add(newVar);
                            }
                        }
                        else
                        {
                            ConfigVariable newVar = new ConfigVariable();
                            newVar.VariableId = unitMapperVariables[Constants.NUMBEROFFLOORSUNITPACKAGEVARIABLES];
                            newVar.Value = 0;
                            variableAssignmentValues.Add(newVar);
                        }
                    }

                    var additionalWiring = summaryScreenDataSet.Tables[31];
                    if (additionalWiring.Rows.Count > 0)
                    {
                        ConfigVariable newVar = new ConfigVariable();

                        var blandingdata = (from DataRow row in summaryScreenDataSet.Tables[31].Rows
                                            select new
                                            {
                                                unitId = Convert.ToInt32(row[Constants.UNITIDVALUE]),
                                                VariableId = Convert.ToString(row[Constants.CONFIGUREVARIABLES]),
                                                Value = Convert.ToString(row[Constants.CONFIGUREVALUES])

                                            }).ToList();

                        var lowestUnitId = 0;
                        if (unitId > 0)
                        {
                            lowestUnitId = unitId;
                        }
                        else
                        {
                            lowestUnitId = Convert.ToInt32((from openingData in blandingdata select openingData?.unitId)?.ToList()?.Min());
                        }
                        _cpqCacheManager.SetCache(sessionId, _environment, setId.ToString(), Constants.SETEXCEPTIONWIRINGDATA, Utility.SerializeObjectValue(blandingdata));
                        foreach (var controls in blandingdata)
                        {
                            if (controls.unitId.Equals(lowestUnitId))
                            {
                                newVar.VariableId = "ELEVATOR.Parameters_SP.additionalWiring_SP";
                                newVar.Value = controls.Value;
                                variableAssignmentValues.Add(newVar);
                            }
                        }

                    }

                }
                else
                {
                    var accrossDistance = summaryScreenDataSet.Tables[13];
                    if (accrossDistance != null)
                    {
                        if (accrossDistance.Rows.Count > 0)
                        {
                            var getunitGroupConsoleList = (from DataRow row in summaryScreenDataSet.Tables[13].Rows
                                                           select new
                                                           {
                                                               variable = Convert.ToString(row[Constants.VARIABLEIDS]),
                                                               value = Convert.ToString(row[Constants.CURRENTVALUE])
                                                           }).ToList();
                            //create Unit configuration list
                            var listOfFloorLayout = new List<ConfigVariable>() {
                        new ConfigVariable {VariableId="B1P1",Value=Constants.FALSEVALUES },
                        new ConfigVariable {VariableId="B1P2",Value=Constants.FALSEVALUES },
                        new ConfigVariable {VariableId="B1P3",Value=Constants.FALSEVALUES},
                        new ConfigVariable {VariableId="B1P4",Value=Constants.FALSEVALUES },
                        new ConfigVariable {VariableId="B2P1",Value=Constants.FALSEVALUES},
                        new ConfigVariable {VariableId="B2P2",Value=Constants.FALSEVALUES },
                        new ConfigVariable {VariableId="B2P3",Value=Constants.FALSEVALUES },
                        new ConfigVariable {VariableId="B2P4",Value=Constants.FALSEVALUES },
                    };
                            foreach (var valda in listOfFloorLayout)
                            {
                                if (!valda.VariableId.StartsWith(unitMapperVariables[Constants.GENERICVARIABLE]))
                                {
                                    valda.VariableId = unitMapperVariables[Constants.GENERICVARIABLE] + valda.VariableId;
                                }
                            }
                            variableAssignmentValues.AddRange(listOfFloorLayout);
                            foreach (var unitConfigurationData in getunitGroupConsoleList)
                            {
                                ConfigVariable newVar = new ConfigVariable();
                                if (unitConfigurationData.variable.Equals(unitMapperVariables[Constants.BNKSEMIDISTANCE]))
                                {
                                    if (unitConfigurationData.value != null)
                                    {
                                        newVar.VariableId = unitMapperVariables[Constants.BNKDISTANCE];
                                        newVar.Value = Convert.ToDecimal(unitConfigurationData.value);
                                        variableAssignmentValues.Add(newVar);
                                    }

                                }
                            }

                        }
                    }
                    var elavationData = summaryScreenDataSet.Tables[24];
                    if (elavationData != null)
                    {
                        if (elavationData.Rows.Count > 0)
                        {

                            var getunitGroupConsoleList = (from DataRow row in summaryScreenDataSet.Tables[24].Rows
                                                           select new
                                                           {

                                                               floorNumber = row[Constants.FLOORNUMBERS].ToString(),
                                                               front = Convert.ToString(row[Constants.FRONTVAL]),
                                                               rear = Convert.ToString(row[Constants.REARVAL]),
                                                               unitId = row["UnitId"]
                                                           }).ToList();


                            List<ConfigVariable> valD = new List<ConfigVariable>();
                            var lowestUnitId = (from openingData in getunitGroupConsoleList select openingData?.unitId)?.ToList()?.Min();
                            foreach (var opening in getunitGroupConsoleList)
                            {
                                if (opening.front.Equals(Constants.True) && opening.unitId.Equals(lowestUnitId))
                                {
                                    ConfigVariable newVar = new ConfigVariable();
                                    newVar.VariableId = unitMapperVariables[Constants.FLOORMATRIXVALUE] + Constant.DOT + String.Join(Constant.DOT, unitMapperVariables[Constants.FRONTVALUE]);
                                    newVar.Value = opening.front.ToUpper();
                                    var strFloornumber = opening.floorNumber.ToString().PadLeft(3, '0');
                                    newVar.VariableId = newVar.VariableId.Replace("#", strFloornumber);
                                    valD.Add(newVar);
                                }
                                if (opening.rear.Equals(Constants.True) && opening.unitId.Equals(lowestUnitId))
                                {
                                    ConfigVariable newVar = new ConfigVariable();
                                    newVar.VariableId = unitMapperVariables[Constants.FLOORMATRIXVALUE] + Constant.DOT + String.Join(Constant.DOT, unitMapperVariables[Constants.REARVALUE]);
                                    newVar.Value = opening.rear.ToUpper();
                                    var strFloornumber = opening.floorNumber.ToString().PadLeft(3, '0');
                                    newVar.VariableId = newVar.VariableId.Replace("#", strFloornumber);
                                    valD.Add(newVar);
                                }
                                if (opening.front.Equals(Constants.False) && opening.unitId.Equals(lowestUnitId))
                                {
                                    ConfigVariable newVar = new ConfigVariable();
                                    newVar.VariableId = unitMapperVariables[Constants.FLOORMATRIXVALUE] + Constant.DOT + String.Join(Constant.DOT, unitMapperVariables[Constants.FRONTVALUE]);
                                    newVar.Value = opening.front.ToUpper();
                                    var strFloornumber = opening.floorNumber.ToString().PadLeft(3, '0');
                                    newVar.VariableId = newVar.VariableId.Replace("#", strFloornumber);
                                    valD.Add(newVar);
                                }
                                if (opening.rear.Equals(Constants.False) && opening.unitId.Equals(lowestUnitId))
                                {
                                    ConfigVariable newVar = new ConfigVariable();
                                    newVar.VariableId = unitMapperVariables[Constants.FLOORMATRIXVALUE] + Constant.DOT + String.Join(Constant.DOT, unitMapperVariables[Constants.REARVALUE]);
                                    newVar.Value = opening.rear.ToUpper();
                                    var strFloornumber = opening.floorNumber.ToString().PadLeft(3, '0');
                                    newVar.VariableId = newVar.VariableId.Replace("#", strFloornumber);
                                    valD.Add(newVar);
                                }

                            }
                            variableAssignmentValues.AddRange(valD);
                        }
                    }
                    var elevationValue = summaryScreenDataSet.Tables[25];
                    if (elevationValue.Rows.Count > 0)
                    {

                        var getunitGroupConsoleList = (from DataRow row in summaryScreenDataSet.Tables[25].Rows
                                                       select new
                                                       {
                                                           floorNumber = row[Constants.FLOORNUMBERS].ToString(),
                                                           elevation = Convert.ToInt32(row[Constants.ELEVATIONFEET])
                                                       }).ToList();
                        List<ConfigVariable> valD = new List<ConfigVariable>();
                        foreach (var opening in getunitGroupConsoleList)
                        {
                            ConfigVariable newVar = new ConfigVariable();
                            newVar.VariableId = unitMapperVariables[Constants.FLOORMATRIXVALUE] + Constant.DOT + String.Join(Constant.DOT, unitMapperVariables[Constants.ELEVATIONPARAMETER]);
                            newVar.Value = opening.elevation * 12;
                            var strFloornumber = opening.floorNumber.ToString().PadLeft(3, '0');
                            newVar.VariableId = newVar.VariableId.Replace("#", strFloornumber);
                            valD.Add(newVar);
                        }
                        variableAssignmentValues.AddRange(valD);
                    }
                }

            }

            return variableAssignmentValues;
        }
        /// <summary>
        ///  Create Update FactoryJob Id
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="userid"></param>
        /// <param name="factoryJobId"></param>
        /// <returns></returns>
        public int CreateUpdateFactoryJobId(int unitId, string userid, string factoryJobId)
        {
            var methodBegin = Utility.LogBegin();
            if (factoryJobId == null)
            {
                factoryJobId = string.Empty;
            }
            IList<SqlParameter> lstSqlParameter = Utility.SqlParameterForCreateUpdateFactoryJobId(unitId, userid, factoryJobId);
            Utility.LogEnd(methodBegin);
            return CpqDatabaseManager.ExecuteNonquery(Constants.SPCREATEUPDATEFACTORYJOBID, lstSqlParameter); ;
        }

        /// <summary>
        /// Method for Getting conflicts
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        public List<string> GetConflictsData(int setId)
        {
            var methodBegin = Utility.LogBegin();
            var conflictVariables = new List<string>();
            List<SqlParameter> sqlParam = new List<SqlParameter>();
            SqlParameter param = new SqlParameter(Constant.SETID, setId);
            sqlParam.Add(param);
            var conflictsData = CpqDatabaseManager.ExecuteDataSet("usp_GetConflictData", sqlParam);
            if (conflictsData != null && conflictsData.Tables.Count > 0 && conflictsData.Tables[0].Rows.Count > 0)
            {
                conflictVariables = (from DataRow row in conflictsData.Tables[0].Rows
                                     select row["ConflictParameter"].ToString()).Distinct().ToList();
            }
            Utility.LogEnd(methodBegin);
            return conflictVariables;
        }

        public List<UnitVariables> GetUnitsVariables(int groupid, DataTable configVariables)
        {
            var methodBeginTime = Utility.LogBegin();
            IList<SqlParameter> parameterList = new List<SqlParameter>();
            List<UnitVariables> lstUnitVariables = new List<UnitVariables>();
            DataSet dataSet = new DataSet();
            IList<SqlParameter> param = new List<SqlParameter>
            {
                new SqlParameter(Constant.GROUPINGID_LOWERCASE, groupid),
                new SqlParameter(Constant.CONSTANTMAPPERLIST ,configVariables),
                new SqlParameter(Constant.TYPE, Constants.UNIT)
            };
            dataSet = CpqDatabaseManager.ExecuteDataSet(Constant.SPGETBUILDINGGROUPUNITVARIABLEASSIGNMENTS, param);
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
            return lstUnitVariables.Distinct().ToList();
        }
    }
}
