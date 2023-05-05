/************************************************************************************************************
************************************************************************************************************
    File Name     :   IUnitConfiguration.cs 
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
using System.Threading.Tasks;
using TKE.SC.Common.Model.ViewModel;
using Newtonsoft.Json.Linq;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IUnitConfiguration
    {
        /// <summary>
        /// Interface for Save Unit Configuration method in the Business Layer
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="userName"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        Task<ResponseMessage> SaveUnitConfiguration(int setId, JObject variableAssignments, string sessionId, int unitId);

        /// <summary>
        /// Interface for Update Unit Configuration method in the Business Layer
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="userName"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        Task<ResponseMessage> UpdateUnitConfiguration(int setId, JObject variableAssignments, string sessionId, int unitId);

        /// <summary>
        /// Interface for Save Cab Interior Details method in the Business Layer
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="userName"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        Task<ResponseMessage> SaveCabInteriorDetails(int groupId, string productName, JObject variableAssignments, string sessionId);



        /// <summary>
        /// Interface for Save Cab Interior Details method in the Business Layer
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="userName"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        Task<ResponseMessage> UpdateCabInteriorDetails(int groupId, string productName, JObject variableAssignments, string sessionId);

        /// <summary>
        /// Interface for Save Hoistway Traction Equipment method in the Business Layer
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="userName"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        Task<ResponseMessage> SaveHoistwayTractionEquipment(int groupId, string productName, JObject variableAssignments, string sessionId);

        /// <summary>
        /// Interface for Update Hoistway Traction Equipment method in the Business Layer
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="userName"></param>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        Task<ResponseMessage> UpdateHoistwayTractionEquipment(int groupId, string productName, JObject variableAssignments, string sessionId);


        /// <summary>
        /// StartGeneralInformation
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="GroupConfigurationId"></param>
        /// <param Name="SessionId"></param>
        /// <param Name="selectedTab"></param>
        /// <returns></returns>
        Task<ResponseMessage> StartUnitConfigure(JObject variableAssignments, int groupConfigurationId, int setId, string SessionId, string sectionTab,int unitId);




        /// <summary>
        /// SaveGeneralInformation
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        Task<ResponseMessage> SaveGeneralInformation(int groupId, string productName, JObject variableAssignments, string sessionId);
        /// <summary>
        /// UpdateGeneralInformation
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="userId"></param>
        /// <returns></returns>
        Task<ResponseMessage> UpdateGeneralInformation(int groupId, string productName, JObject variableAssignments, string sessionId);

        /// <summary>
        /// Save entrances
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="productName"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> SaveEntrances(int groupId, string productName, JObject variableAssignments, string sessionId);

        /// <summary>
        /// Update entrances
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="productName"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> UpdateEntrances(int groupId, string productName, JObject variableAssignments, string sessionId);

        Task<ResponseMessage> EditUnitDesignation(int groupId,int unitId,string sessionId, UnitDesignation unit);

        /// <summary>
        /// StartEntranceConfigure
        /// </summary>
        /// <param Name="consoleId"></param>
        /// <param Name="setId"></param>
        /// <param Name="SessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> StartEntranceConfigure( int consoleId, int setId, string SessionId, EntranceConfigurationData entranceConsole =null,bool isSave=false, bool isReset=false);

        /// <summary>
        /// Interface for Save Entrance Configuration method in the Business Layer
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="entranceConfigurationData"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> SaveEntranceConfiguration(int setId, EntranceConfigurationData entranceConfigurationData, string sessionId, int is_Saved);

        /// <summary>
        /// Interface for Save HallLantern Configuration method in the Business Layer
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="entranceConfigurationData"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> SaveUnitHallFixtureConfiguration(int setId, UnitHallFixtureData unitHallFixtureConfigurationData, string sessionId, int is_Saved);

        /// <summary>
        /// StartUnitHallFixtureConfigure
        /// </summary>
        /// <param Name="consoleId"></param>
        /// <param Name="setId"></param>
        /// <param Name="SessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> StartUnitHallFixtureConfigure(int consoleId, int isChange, int setId, string SessionId,string fixtureSelected, bool isReset, UnitHallFixtureData entranceConsole = null, bool isSave = false);

        /// <summary>
        /// GetDetailsForTP2SummaryScreen
        /// </summary>
        /// <param Name="UnitId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetDetailsForTP2SummaryScreen(int unitId, string sessionId);
        /// <summary>
        /// DeleteEntranceConsole
        /// </summary>
        /// <param Name="consoleId"></param>
        /// <param Name="setId"></param>
        /// <returns></returns>
        Task<ResponseMessage> DeleteEntranceConsole(int consoleId, int setId, string sessionId);

        /// <summary>
        /// DeleteUnitHallFixtureConfigure
        /// </summary>
        /// <param Name="setId"></param>
        /// <returns></returns>
        Task<ResponseMessage> DeleteUnitHallFixtureConsole(int setId, int consoleId, string fixtureType,string sessionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="carcallCutoutData"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> SaveCarCallCutoutKeyswitchOpenings(int setId, CarcallCutoutData carcallCutoutData, string sessionId);

        /// <summary>
        /// StartCarCallCutoutAssignOpenings
        /// </summary>
        /// <param Name="groupId"></param>
        /// <param Name="setId"></param>
        /// <returns></returns>
        Task<ResponseMessage> StartCarCallCutoutAssignOpenings(int setId);

        /// <summary>
        /// Get System Validation For Unit
        /// </summary>
        /// <param Name="UnitId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetSystemValidationForUnit(int setId, string sessionId);


        /// <summary>
        /// GetLatestSystemValidationForUnit
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> GetLatestSystemValidationForUnit(int setId, string sessionId);

        /// <summary>
        /// SavePriceForTP2SummaryScreen
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="priceDetails"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> SavePriceForTP2SummaryScreen(int setId, UnitSummaryUIModel priceDetails, string sessionId, List<UnitNames> unitPrices);

        /// <summary>
        /// ChangeUnitConfigure
        /// </summary>
        /// <param name="variableAssignments"></param>
        /// <param name="sessionId"></param>
        /// <param name="sectionTab"></param>
        /// <param name="setId"></param>
        /// <returns></returns>
        Task<ResponseMessage> ChangeUnitConfigure(JObject variableAssignments, string sessionId, string sectionTab, int setId,int unitId=0);
        /// <summary>
        /// ValidateUnitHallFixtureConsoles
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task<ResponseMessage> ValidateUnitHallFixtureConsoles(int setId, string sessionId);
        /// <summary>
        /// ChangeTP2SummaryDetails
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="sessionId"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        Task<ResponseMessage> ChangeTP2SummaryDetails(int unitId, string sessionId, UnitSummaryUIModel requestBody);
        /// <summary>
        /// Save Custom PriceLine
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="sessionId"></param>
        /// <param name="customPriceLine"></param>
        /// <returns></returns>
        Task<ResponseMessage> SaveCustomPriceLine(int setId,string sessionId, List<CustomPriceLine> customPriceLine);
        /// <summary>
        /// Update Custom PriceLine
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="sessionId"></param>
        /// <param name="customPriceLine"></param>
        /// <returns></returns>
        Task<ResponseMessage> EditCustomPriceLine(int setId, string sessionId, CustomPriceLine customPriceLine);
        /// <summary>
        /// Delete Custom Price Line
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="sessionId"></param>
        /// <param name="priceLineId"></param>
        /// <returns></returns>
        Task<ResponseMessage> DeleteCustomPriceLine(int setId,string sessionId,int priceLineId);
        /// <summary>
        /// Edit Factory Job Id
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="unitId"></param>
        /// <param name="sessionId"></param>
        /// <param name="factoryJobId"></param>
        /// <returns></returns>
        Task<ResponseMessage> CreateUpdateFactoryJobId(int unitId, string sessionId, string factoryJobId);
    }
}
