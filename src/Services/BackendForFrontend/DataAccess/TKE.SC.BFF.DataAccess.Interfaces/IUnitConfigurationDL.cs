/************************************************************************************************************
************************************************************************************************************
    File Name     :   IUnitConfigurationDL.cs  
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model;
using Configit.Configurator.Server.Common;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Data;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.CommonModel;

namespace TKE.SC.BFF.DataAccess.Interfaces

{
    public interface IUnitConfigurationDL
    {
        /// <summary>
        /// Interface to update Unit configuration details
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        List<ResultSetConfiguration> UpdateUnitConfigurationDL(int setId, List<ConfigVariable> listOfDetails, string userId, ConflictsStatus isEditFlow, List<LogHistoryTable> historyTable,int unitId);

        /// <summary>
        /// Interface to save Unit configuration details
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        List<ResultSetConfiguration> SaveUnitConfigurationDL(int setId, List<ConfigVariable> listOfDetails, string userId, List<LogHistoryTable> historyTable, int unitId);

        /// <summary>
        /// Interface to save cab interior details
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="unitVariableAssignment"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        List<ResultUnitConfiguration> SaveCabInteriorDetails(int groupid, string productName, List<ConfigVariable> listOfDetails, string userId);

        /// <summary>
        /// Interface to update cab interior details
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="unitVariableAssignment"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        List<ResultUnitConfiguration> UpdateCabInteriorDetails(int groupid, string productName, List<ConfigVariable> listOfDetails, string userId);

        /// <summary>
        /// Interface to save hoistway traction equipment
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="unitVariableAssignment"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        List<ResultUnitConfiguration> SaveHoistwayTractionEquipment(int groupid, string productName, List<ConfigVariable> listOfDetails, string userId);



        /// <summary>
        /// Interface to GetEntranceByGroupId
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="selectTab"></param>
        /// <returns></returns>
        UnitVariableDetails GetUnitConfigurationByGroupId(int groupConfigurationId, int setId, string selectTab);

        /// <summary>
        /// Interface to update hoistway traction equipment
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="unitVariableAssignment"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        List<ResultUnitConfiguration> UpdateHoistwayTractionEquipment(int groupid, string productName, List<ConfigVariable> listOfDetails, string userId);

        /// <summary>
        /// Interface to GetGeneralInformationByGroupId
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="selectTab"></param>
        /// <returns></returns>
        List<ConfigVariable> GetGeneralInformationByGroupId(int groupConfigurationId, string productName, string selectTab);

        /// <summary>
        /// Interface to GetUnitsByGroupId
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="selectTab"></param>
        /// <returns></returns>
        List<UnitNames> GetUnitsByGroupId(int groupConfigurationId, int setId);

        /// <summary>
        /// Interface to GetCabInteriorByGroupId
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="selectTab"></param>
        /// <returns></returns>
        List<ConfigVariable> GetCabInteriorByGroupId(int groupConfigurationId, string productName, string selectTab);


        /// <summary>
        ///  Interface to GetHoistwayTractionByGroupId
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="selectTab"></param>
        /// <returns></returns>
        List<ConfigVariable> GetHoistwayTractionByGroupId(int groupConfigurationId, string productName, string selectTab);

        /// <summary>
        ///  Interface to GetHoistwayTractionUnitsByGroupId
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="selectTab"></param>
        /// <returns></returns>
        List<UnitNames> GetHoistwayTractionUnitsByGroupId(int groupConfigurationId, string productName, string selectTab);

        /// <summary>
        ///  Interface to UpdateGeneralInformation
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="productName"></param>
        /// <param Name="listOfGeneralInformationVariables"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        List<ResultUnitConfiguration> UpdateGeneralInformation(int groupid, string productName, List<ConfigVariable> listOfGeneralInformationVariables, string userId);

        /// <summary>
        ///  Interface to SaveGeneralInformation
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="productName"></param>
        /// <param Name="listOfGeneralInformationVariables"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        List<ResultUnitConfiguration> SaveGeneralInformation(int groupid, string productName, List<ConfigVariable> listOfGeneralInformationVariables, string userId);

        /// <summary>
        ///  Interface to SaveGeneralInformation
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="productName"></param>
        /// <param Name="listOfGeneralInformationVariables"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        List<ResultUnitConfiguration> SaveEntrances(int groupid, string productName, List<ConfigVariable> listOfEntranceVariables, string userId);
        /// <summary>
        /// Interface to GetEntranceByGroupId
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="productName"></param>
        /// <param Name="selectTab"></param>
        /// <returns></returns>
        List<ConfigVariable> GetEntranceByGroupId(int groupConfigurationId, string productName, string selectTab);

        /// <summary>
        ///  Interface to SaveGeneralInformation
        /// </summary>
        /// <param Name="groupid"></param>
        /// <param Name="productName"></param>
        /// <param Name="listOfGeneralInformationVariables"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        List<ResultUnitConfiguration> UpdateEntrances(int groupid, string productName, List<ConfigVariable> listOfEntranceVariables, string userId);

        /// <summary>
        /// Interface for EditUnitDesignation
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="UnitId"></param>
        /// <param Name="userid"></param>
        /// <param Name="unit"></param>
        /// <returns></returns>
        int EditUnitDesignation(int groupId, int unitId, string userid, UnitDesignation unit);

        /// <summary>
        /// Interface to SaveEntranceConfiguration
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="entranceConfigurationData"></param>
        /// <param Name="userId"></param>
        /// <param Name="is_Saved"></param>
        /// <returns></returns>
        List<ResultSetConfiguration> SaveEntranceConfiguration(int setId, EntranceConfigurationData entranceConfigurationData, string userId, int is_Saved, bool isReset, List<LogHistoryTable> historyTable);
        /// <summary>
        /// Interface to GetEntranceConfigurationBtSetId
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        List<EntranceConfigurations> GetEntranceConfiguration(int setId, int groupId, string controlLanding, string username, bool isJambMounted = false);

        /// <summary>
        /// GetDetailsForTP2SummaryScreen
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        TP2Summary GetDetailsForTP2SummaryScreen(int setId);

        /// <summary>
        /// Interface to GetFixtureStrategy
        /// </summary>
        /// <param Name="GroupConfigurationId"></param>
        /// <returns></returns>
        string GetFixtureStrategy(int groupConfigurationId);

        /// <summary>
        /// GetUnitHallFixturesData
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="groupId"></param>
        /// <param Name="userName"></param>
        /// <param Name="FixtureStrategy"></param>
        /// <returns></returns>
        List<UnitHallFixtures> GetUnitHallFixturesData(int setId, int groupId, string userName, string fixtureStrategy, string sessionId);

        /// <summary>
        /// GenerateUnitHallFixturesFixturesList
        /// </summary>
        /// <param Name="FixtureStrategy"></param>
        /// <returns></returns>
        List<string> GenerateUnitHallFixturesList(string fixtureStrategy);

        /// <summary>
        /// Interface to HallLanternEntranceConfiguration
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="entranceConfigurationData"></param>
        /// <param Name="userId"></param>
        /// <param Name="is_Saved"></param>
        /// <returns></returns>
        List<ResultSetConfiguration> SaveUnitHallFixtureConfiguration(int setId, UnitHallFixtureData unitHallFixtureConfigurationData, string userId, int is_Saved, List<LogHistoryTable> logHistorutable);

        /// <summary>
        ///  GetGroupHallFixturesTypesList
        /// </summary>
        /// <param Name="FixtureStrategy"></param>
        /// <returns></returns>
        List<string> GetGroupHallFixturesTypesList(string fixtureStrategy);
        /// <summary>
        /// DeleteEntranceConsole
        /// </summary>
        /// <param Name="consoleId"></param>
        /// <param Name="setId"></param>
        /// <returns></returns>
        List<ResultSetConfiguration> DeleteEntranceConsole(int consoleId, int setId, List<LogHistoryTable> historyTable, string userId);

        /// <summary>
        /// DeleteUnitHallFixtureConfigurationById
        /// </summary>
        /// <param Name=""></param>
        /// <param Name=""></param>
        /// <returns></returns>
        List<ResultSetConfiguration> DeleteUnitHallFixtureConsole(int setId, int consoleId, string fixtureType, List<LogHistoryTable> historyTable, string userId);

        List<UnitHallFixtures> ResetUnitHallFixtureConsole(int setId, int consoleId, string fixtureType, string userName);

        /// <summary>
        /// SaveCarCallCutoutKeyswitchOpenings
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="carcallCutoutData"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        List<ResultSetConfiguration> SaveCarCallCutoutKeyswitchOpenings(int setId, CarcallCutoutData carcallCutoutData, string userId, List<LogHistoryTable> loghistoryTable);

        /// <summary>
        /// GetCarCallCutoutOpenings
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="groupId"></param>
        /// <returns></returns>
        EntranceAssignment GetCarCallCutoutOpenings(int setId);

        /// <summary>
        /// GetCarCallcutoutSavedOpenings
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        int GetCarCallcutoutSavedOpenings(int setId);
        /// <summary>
        /// GetLogHistoryunit
        /// </summary>
        /// <param Name="SetId"></param>
        /// <param Name="UnitId"></param>
        /// <param Name="lastDate"></param>
        /// <returns></returns>
        LogHistoryResponse GetLogHistoryUnit(int SetId, int UnitId, string lastDate);
        /// <summary>
        /// GetSystemsValValues
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        Status GetSystemsValValues(int setId, string userId);

        /// <summary>
        /// GetLogHistoryUnit
        /// </summary>
        /// <param Name="SetId"></param>
        /// <param Name="UnitId"></param>
        /// <param Name="lastDate"></param>
        /// <returns></returns>
       // LogHistoryResponse GetLogHistoryUnit(int SetId,int UnitId, string lastDate);

        /// <summary>
        /// GetTravelValue
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        TP2Summary GetTravelValue(int setId);

        /// <summary>
        /// GetPermissionByRole
        /// </summary>
        /// <param Name="Id"></param>
        /// <param Name="roleName"></param>
        /// <returns></returns>
        List<string> GetPermissionByRole(int id, string roleName, string entity = "Unit");

        /// <summary>
        /// GetLatestSystemsValValues
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="statusKey"></param>
        /// <param Name="userId"></param>
        /// <param Name="systemKeyValues"></param>
        /// <param Name="type"></param>
        /// <param Name="UnitDetails"></param>
        /// <returns></returns>
        ConfigurationResponse GetLatestSystemsValValues(int setId, string statusKey, string userId, List<SystemValidationKeyValues> systemKeyValues, string type, List<UnitDetailsForTP2> UnitDetails, string sessionId, List<VariableAssignment> Response = null);

        /// <summary>
        /// GetDetailsForUnits
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        TP2Summary GetDetailsForUnits(int setId);

        /// <summary>
        /// Interface to save Unit configuration details
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="userName"></param>
        /// <returns></returns>
        List<ResultSetConfiguration> SavePriceValuesDL(int setId, List<ConfigVariable> listOfDetails, List<ConfigVariable> listOfLeadTimes, string userId, List<LogHistoryTable> historyTable, List<UnitNames> unitPrices);

        /// <summary>
        /// Interface to Getting the Group
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetProductCategoryByGroupId(int id, string type, DataTable dtVariables);

        /// <summary>
        /// Interface to Getting the Variable Assigmnments for NCP
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        List<ConfigVariable> GetVariableAssignmentsBySetId(int setId, DataTable dtVariables);

        /// <summary>
        /// Interface to Saving the Variable Assigmnments for NCP
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="listOfDetails"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<ResultSetConfiguration> SaveNonConfigurableUnitConfigurationDL(int setId, List<ConfigVariable> listOfDetails, string userId,DataTable configVariables);

        /// <summary>
        /// Get Product Type
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        string GetProductType(int setId);

        /// <summary>
        /// save custom price key
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="sessionId"></param>
        /// <param name="customPriceLine"></param>
        /// <returns></returns>
        List<CustomPriceLine> SaveCustomPriceLine(int setId, string sessionId, List<CustomPriceLine> customPriceLine);
        /// <summary>
        /// Edit Custom PriceKey
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="sessionId"></param>
        /// <param name="customPriceLine"></param>
        /// <returns></returns>
        CustomPriceLine EditCustomPriceLine(int setId, string sessionId, CustomPriceLine customPriceLine);
        /// <summary>
        /// Delete Custom PriceKey
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="priceLineId"></param>
        /// <returns></returns>
        int DeleteCustomPriceLine(int setId, int priceLineId);
        /// <summary>
        /// Method for Get Variables for Hoistway Wiring
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        List<ConfigVariable> GetDetailsForHoistwayWiring(string val, int setId, string sessionId,int unitId);
        /// <summary>
        /// edit Factory Job ID
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="userid"></param>
        /// <param name="factoryJobId"></param>
        /// <returns></returns>
        int CreateUpdateFactoryJobId( int unitId, string userid, string  factoryJobId);
        /// <summary>
        /// GetConflictsData
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        public List<string> GetConflictsData(int setId);
        /// <summary>
        /// GetUnitsVariablesByGroupId
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="configVariables"></param>
        /// <returns></returns>
        List<UnitVariables> GetUnitsVariables(int groupid, DataTable configVariables);
    }

}



