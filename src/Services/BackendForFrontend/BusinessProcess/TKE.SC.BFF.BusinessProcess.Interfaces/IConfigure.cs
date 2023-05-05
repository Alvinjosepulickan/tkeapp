/************************************************************************************************************
************************************************************************************************************
    File Name     :   IConfigure.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

using Configit.Configurator.Server.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;

namespace TKE.SC.BFF.BusinessProcess.Interfaces
{
    public interface IConfigure
    {
        /// <summary>
        /// Interface for Sublines Business Layer
        /// </summary>
        /// <param Name="request"></param>
        /// <returns></returns>
        Task<ResponseMessage> RequestConfigurationBl(SublineRequest request, string packagePath, string sessionId);




        /// <summary>
        /// Interface for Configure Business Layer
        /// </summary>
        /// <param Name="request"></param>
        /// <returns></returns>
        Task<ResponseMessage> ConfigurationBl(ConfigurationRequest request, string packagePath,
            string sessionId);

        /// <summary>
        /// Interface for StartConfigure Business Layer
        /// </summary>
        /// <param Name="configureRequest"></param>
        /// <param Name="modelNumber"></param>
        /// <param Name="locale"></param>
        /// <returns></returns>
        Task<ResponseMessage> StartBuildingConfigure(string sessionId,
            ConfigurationRequest configureRequest = null);


        /// <summary>
        /// Interface for StartBuildingEquipmentConfigureBl
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="entranceConsoles"></param>
        /// <returns></returns>
        Task<ResponseMessage> StartBuildingEquipmentConfigureBl(string sessionId, List<string> permission, int buildingId,
                     JObject variableAssignments = null, List<BuildingEquipmentData> entranceConsoles = null);


        /// <summary>
        /// Interface for ChangeBuildingEquipmentConfigureBl
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        Task<JObject> ChangeBuildingEquipmentConfigureBl(JObject variableAssignments, string sessionId, int buildingId);

        /// <summary>
        /// Interface for TestStartConfigureBl
        /// </summary>
        /// <param Name="parentCode"></param>
        /// <param Name="modelNumber"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="configureRequest"></param>
        /// <param Name="locale"></param>
        /// <returns></returns>
        Task<ResponseMessage> TestStartConfigureBl(string parentCode, string modelNumber, string sessionId,
         ConfigurationRequest configureRequest = null, string locale = null);

        /// <summary>
        /// Interface for ChangeConfigure Business Layer
        /// </summary>
        /// <param Name="configureRequest"></param>
        /// <returns></returns>
        Task<Tuple<JObject, StartConfigureResponse>> ChangeBuildingConfigure(JObject variableAssignments, string sessionId);
        /// <summary>
        /// Interface for StartGroupConfigureBl Business Layer
        /// </summary>
        /// <param Name="parentCode"></param>
        /// <param Name="modelNumber"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="configureRequest"></param>
        /// <param Name="locale"></param>
        /// <returns></returns>
        Task<ResponseMessage> StartGroupConfigureBl(string parentCode, string modelNumber, string sessionId, string selectedTab,
            ConfigurationRequest configureRequest = null, string locale = null);

        /// <summary>
        /// Interface for ChangeGroupConfigureBl Business Layer
        /// </summary>
        /// <param Name="configureRequest"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="parentCode"></param>
        /// <param Name="locale"></param>
        /// <param Name="modelNumber"></param>
        /// <returns></returns>
        Task<JObject> ChangeGroupConfigure(JObject variableAssignments, int groupId, string sessionId
             , string selectedTab, List<DisplayVariableAssignmentsValues> displayVariablesValuesResponse, bool getConflictCache = false);



        /// <summary>
        /// Interface for StartFieldDrawingConfigure Business Layer
        /// </summary>
        /// <param name="variableAssignments"></param>
        /// <param name="groupId"></param>
        /// <param name="isChange"></param>
        /// <param name="sessionId"></param>
        /// <param name="lstUnitLayoutDetails"></param>
        /// <param name="lstConfigureVariable"></param>
        /// <param name="statusKey"></param>
        /// <param name="projectStatusKey"></param>
        /// <param name="drawingStausKey"></param>
        /// <returns></returns>
        Task<ResponseMessage> StartFieldDrawingConfigure(JObject variableAssignments, int groupId, bool isChange, string sessionId, List<UnitLayOutDetails> lstUnitLayoutDetails, string statusKey, string projectStatusKey, string drawingStausKey, List<string> permissions);

        /// <summary>
        /// Interface for StartUnitConfigureBl Business Layer
        /// </summary>
        /// <param Name="parentCode"></param>
        /// <param Name="modelNumber"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="selectedTab"></param>
        /// <param Name="configureRequest"></param>
        /// <param Name="locale"></param>
        /// <returns></returns>
        Task<ResponseMessage> StartUnitConfigure(string sessionId, string selectedTab, string fixtureStrategy, int setId, string productType, string controlLanding,
           JObject configureRequest = null, List<UnitNames> lstunits = null, List<EntranceConfigurations> entranceConsoles = null, List<UnitHallFixtures> unitHallFixtureConsoles = null, int groupConfigurationId = 0,int unitId=0);



        /// <summary>
        /// Interface for ChangeUnitConfigureBl Business Layer
        /// </summary>
        /// <param Name="variableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="parentCode"></param>
        /// <param Name="locale"></param>
        /// <param Name="selectedTab"></param>
        /// <param Name="modelNumber"></param>
        /// <returns></returns>
        Task<JObject> ChangeUnitConfigureBl(JObject variableAssignments, string sessionId, string selectedTab, int setId,int unitId=0);


        Task<JObject> GetAvailableProducts(JObject variableAssignments, string sessionId
           , string locale, List<int> unitId,
             string modelNumber = null);

        /// <summary>
        /// Method to get userid from cache
        /// </summary>
        /// <returns></returns>
        string GetUserId(string sessionId);

        string GetUnitDetails(string sessionId);

        string GetUnitfixtureDetails(string sessionId);
        string GetUserAddress(string sessionId);

        string GetOpportunityData(string OppAndVersion, string sessionId);
        /// <summary>
        /// GetUnitName
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<List<UnitMappingValues>> GetUnitName(JObject variableAssignments, int groupId, string sessionId, string selectedTab, List<DisplayVariableAssignmentsValues> displayVariablesValuesResponse);

        /// <summary>
        /// GetCacheUnitsList
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="setId"></param>
        /// <returns></returns>
        Task<List<UnitMappingValues>> GetCacheUnitsList(string sessionId, int setId);

        /// <summary>
        /// SetCacheMappingValues
        /// </summary>
        /// <param Name="unitMappingValues"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        List<UnitMappingValues> SetCacheMappingValues(List<UnitMappingValues> unitMappingValues, string sessionId);

        /// <summary>
        /// SetCacheEntranceConsoles
        /// </summary>
        /// <param Name="objEntranceConfigurationData"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="setId"></param>
        /// <param Name="entranceConsoleId"></param>
        /// <returns></returns>
        List<EntranceConfigurations> SetCacheEntranceConsoles(List<EntranceConfigurations> objEntranceConfigurationData, string sessionId, int setId);
        /// <summary>
        /// EntranceConsoleConfigureBl
        /// </summary>
        /// <param Name="entranceConsole"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        Task<JObject> EntranceConsoleConfigureBl(EntranceConfigurations entranceConsole, string sessionId, bool isSave, int setId);

        /// <summary>
        /// SetCacheUnitHallFixtureConsoles
        /// </summary>
        /// <param Name="objEntranceConfigurationData"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="setId"></param>
        /// <param Name="entranceConsoleId"></param>
        /// <returns></returns>
        List<UnitHallFixtures> SetCacheUnitHallFixtureConsoles(List<UnitHallFixtures> objEntranceConfigurationData, string sessionId, int setId);

        /// <summary>
        /// UnitHallFixtureConsoleConfigureBl
        /// </summary>
        /// <param name="entranceConsole"></param>
        /// <param name="sessionId"></param>
        /// <param name="fixtureType"></param>
        /// <param name="setId"></param>
        /// <param name="isSave"></param>
        /// <returns></returns>
        Task<JObject> UnitHallFixtureConsoleConfigureBl(UnitHallFixtures entranceConsole, string sessionId, string fixtureType, int setId, bool isSave);


        /// <summary>
        /// SummaryUnitConfigureBl
        /// </summary>
        /// <param name="lineVariableAssignment"></param>
        /// <param name="unitValues"></param>
        /// <param name="sessionId"></param>
        /// <param name="sectionTab"></param>
        /// <param name="openingVariables"></param>
        /// <param name="setId"></param>
        /// <param name="groupUnitInfo"></param>
        /// <param name="priceAndDiscountData"></param>
        /// <param name="manufacturingCommentsTable"></param>
        /// <returns></returns>
        Task<JObject> SummaryUnitConfigureBl(JObject lineVariableAssignment, List<UnitDetailsForTP2> unitValues, string sessionId, string sectionTab, 
            List<OpeningVariables> openingVariables, int setId, List<UnitNames> groupUnitInfo, List<DiscountDataPerUnit> priceAndDiscountData, 
            List<PriceSectionDetails> manufacturingCommentsTable);

        /// <summary>
        /// SetCacheUnitsList
        /// </summary>
        /// <param name="listUnitsData"></param>
        /// <param name="sessionId"></param>
        /// <param name="setId"></param>
        /// <returns></returns>
        List<UnitNames> SetCacheUnitsList(List<UnitNames> listUnitsData, string sessionId, int setId);

        /// <summary>
        /// SetCacheRearOpenForUnitSet
        /// </summary>
        /// <param name="lstConfigVariable"></param>
        /// <param name="sessionId"></param>
        /// <param name="setId"></param>
        /// <returns></returns>
        List<ConfigVariable> SetCacheRearOpenForUnitSet(List<ConfigVariable> lstConfigVariable, string sessionId, int setId);


        /// <summary>
        /// SetCacheUnitHallFixtureConsoles
        /// </summary>
        /// <param Name="objGroupHallFixturesData"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="groupId"></param>
        /// <param Name="ConsoleId"></param>
        /// <returns></returns>
        List<GroupHallFixtures> SetCacheGroupHallFixtureConsoles(List<GroupHallFixtures> objGroupHallFixturesData, string sessionId, int groupId);

        /// <summary>
        /// UnitHallFixtureConsole
        /// </summary>
        /// <param Name="groupConsole"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="isSave"></param>
        /// <returns></returns>
        Task<JObject> GroupHallFixtureConsoleConfigureBl(GroupHallFixtures groupConsole, string sessionId, string fixtureType, bool isSave, string fixtureStrategy,int groupId);


        /// <summary>
        /// StartGroupFixtureBL
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="sectionTab"></param>
        /// <param Name="FixtureStrategy"></param>
        /// <param Name="variableAssignments"></param>
        /// <param Name="lstunits"></param>
        /// <param Name="entranceConsoles"></param>
        /// <param Name="unitHallFixtureConsoles"></param>
        /// <returns></returns>
        Task<ResponseMessage> StartGroupHallFixtures(string sessionId, string sectionTab, string fixtureStrategy,
              JObject variableAssignments = null, List<GroupHallFixtures> unitHallFixtureConsoles = null);


        /// <summary>
        /// StartBuildinGEquipmentBL
        /// </summary>
        /// <param Name="objBuildingEquipmentData"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        List<BuildingEquipmentData> SetCacheBuildingEquipmentConsoles(List<BuildingEquipmentData> objBuildingEquipmentData, string sessionId, int buildingId, bool changeFlag);


        /// <summary>
        /// SetCacheFieldDrawingAutomationLayoutDetails
        /// </summary>
        /// <param Name="objBuildingEquipmentData"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="groupid"></param>
        /// <returns></returns>
        List<UnitLayOutDetails> SetCacheFieldDrawingAutomationLayoutDetails(List<UnitLayOutDetails> objBuildingEquipmentData, string sessionId, int groupid);




        /// <summary>
        /// BuildingEquipmentConsoleConfigureBl
        /// </summary>
        /// <param Name="entranceConsole"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="isSave"></param>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        Task<JObject> BuildingEquipmentConsoleConfigureBl(BuildingEquipmentData entranceConsole, string sessionId, bool isSave, int buildingId, bool editFlag, bool islobby, bool isChange);

        /// <summary>
        /// Set Cache Fixture Strategy
        /// </summary>
        /// <param Name="variables"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="BuildingId"></param>
        /// <returns></returns>
        public List<VariableAssignment> SetCacheFixtureStrategy(List<VariableAssignment> variables, string sessionId, int buildingId);




        /// <summary>
        /// generate variable assignments for cross 
        /// </summary>
        /// <param Name="currentConfiguration"></param>
        /// <param Name="configVariables"></param>
        /// <returns></returns>
        List<ConfigVariable> GeneratevariableAssignmentsForCrosspackageDependecy(string currentConfiguration, List<ConfigVariable> configVariables);
        /// <summary>
        /// Get Caeched variable assignments
        /// </summary>
        /// <param Name="crosspackagevariableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="setId"></param>
        /// <returns></returns>
        string GetCrosspackageVariableAssignments(string sessionId, string configurationType);
        /// <summary>
        /// update caeched variable assignments
        /// </summary>
        /// <param Name="crosspackagevariableAssignments"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        int SetCrosspackageVariableAssignments(List<VariableAssignment> crosspackagevariableAssignments, string sessionId, string configurationType);
        /// <summary>
        /// Generate variable assignments for Unit configuration
        /// </summary>
        /// <param Name="crossPackagevariableAssignments"></param>
        /// <param Name="assignments"></param>
        /// <returns></returns>
        List<VariableAssignment> GenerateVariableAssignmentsForUnitConfiguration(List<VariableAssignment> crossPackagevariableAssignments, Line assignments);
        /// <summary>
        /// SetOrGetCacheForEditConflictFlow
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <param Name="isEditFlow"></param>
        /// <returns></returns>
        string SetOrGetCacheForEditConflictFlow(string sessionId, bool isEditFlow);
        /// <summary>
        /// GetCacheValuesForConflictManagement
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        bool GetCacheValuesForConflictManagement(string sessionId, string conflictType);
        /// <summary>
        /// GetCacheVariablesForConflictChanges
        /// </summary>
        /// <param Name="getVariables"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        List<VariableAssignment> GetCacheVariablesForConflictChanges(List<VariableAssignment> getVariables, string sessionId);

        /// <summary>
        /// check the save request body to give conflict flag
        /// </summary>
        /// <param Name="buildingData"></param>
        /// <param Name="getVariables"></param>
        /// <returns></returns>
        List<VariableAssignment> CheckConflict(List<VariableAssignment> buildingData, List<VariableAssignment> getVariables);

        /// <summary>
        /// CreateConfigurationRequest
        /// </summary>
        /// <param Name="varibleAssignments"></param>
        /// <param Name="typeOfConfiguration"></param>
        /// <param Name="varRearOpen"></param>
        /// <returns></returns>
        ConfigurationRequest CreateConfigurationRequestWithTemplate(JObject varibleAssignments, string typeOfConfiguration, List<VariableAssignment> varRearOpen = null, string productType = null);
        /// <summary>
        /// GetConflictAssignments
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        ConflictManagement GetConflictAssignments(string sessionId);
        /// <summary>
        /// SetCacheCarCallCutout
        /// </summary>
        /// <param Name="setId"></param>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        List<EntranceLocations> SetCacheCarCallCutout(int setId, string sessionId);

        /// <summary>
        /// SetCacheOpeningLocation
        /// </summary>
        /// <param Name="openingLocationData"></param>
        /// <param Name="sessionId"></param>
        /// <param Name="setId"></param>
        /// <returns></returns>
        OpeningLocations SetCacheOpeningLocation(OpeningLocations openingLocationData, string sessionId, int setId);


        Task<JObject> GetByDefaultOrRulevaluesFromPackage(ConfigurationRequest configurationRequest, string sessionId);

        /// <summary>
        /// SystemValidationForUnitBl
        /// </summary>
        /// <param Name="lineVariableAssignment"></param>
        /// <param Name="unitValues"></param
        /// <param Name="sessionId"></param>
        /// <param Name="sectionTab"></param>
        /// <returns></returns>
        Task<Tuple<ResponseMessage, List<VariableAssignment>>> SystemValidationForUnitBl(List<VariableAssignment> lineVariableAssignment, List<UnitDetailsForTP2> unitValues, string sessionId, string sectionTab, List<UnitVariablesDetailsForTP2> unitVariablesDetailsForTP2s);
        /// <summary>
        /// getConflictCacheValues
        /// </summary>
        /// <param Name="sessionId"></param>
        /// <returns></returns>
        ConfigurationResponse GetConflictCacheValues(string sessionId, ConfigurationResponse cacheSetForConflicts);
        /// <summary>
        /// Get RoleName
        /// </summary>
        /// <returns></returns>
        string GetRoleName(string sessionId);

        /// <summary>
        /// SetCacheHoistwayDimentions
        /// </summary>
        /// <returns></returns>
        List<VariableAssignment> SetCacheHoistwayDimensions(List<VariableAssignment> variables, string sessionId, int setId);

        /// <summary>
        /// StartNonConfigurableProductBl
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="productCategory"></param>
        /// <param name="variableAssignments"></param>
        /// <param name="setId"></param>
        /// <returns></returns>
        Task<ConfigurationResponse> StartNonConfigurableProductConfigure(string sessionId, string productCategory,
                     JObject variableAssignments = null, int setId = 0, List<string> permissions = null);

        /// <summary>
        /// ChangeCustomEngineeredNonConfigurableProductBl
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="productType"></param>
        /// <param name="variableAssignments"></param>
        /// <param name="permissions"></param>
        /// <param name="setId"></param>
        /// <returns></returns>
        //Task<JObject> ChangeCustomEngineeredNonConfigurableProductBl(string sessionId, string productType,
        //          JObject variableAssignments = null, List<string> permissions = null, int setId = 0);

        /// <summary>
        /// GroupInfoConfigureBl
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="variableAssignments"></param>
        /// <returns></returns>
        Task<JObject> GroupInfoConfigure(string sessionId, List<ConfigVariable> variableAssignments = null);

        /// <summary>
        /// SetCacheProdcutType
        /// </summary>
        /// <param name="variables"></param>
        /// <param name="sessionId"></param>
        /// <param name="setId"></param>
        /// <returns></returns>
        List<VariableAssignment> SetCacheProductType(List<VariableAssignment> variables, string sessionId, int setId);

        /// <summary>
        /// UnitsConfigurationArgumentsResponse
        /// </summary>
        /// <param name="lstVariableAssignment"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task<Dictionary<string, object>> UnitsConfigurationArgumentsResponse(List<VariableAssignment> lstVariableAssignment, string sessionId,string productName);

        /// <summary>
        /// CarFixtureStartUnitConfigure
        /// </summary>
        /// <param name="lstVariableAssignment"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task<ConfigurationResponse> CarFixtureStartUnitConfigureBL(string sessionId, string fixtureStrategy, string productType, string currentProductType,
            ConfigurationResponse baseConfigureResponse, ConfigurationRequest configureRequest, int setId, string sectionTab);


        /// <summary>
        /// CarFixtureChangeUnitConfigure
        /// </summary>
        /// <param name="lstVariableAssignment"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task<ConfigurationResponse> CarFixtureChangeUnitConfigureBL(string sessionId, List<object> fixtureStrategy, string productType, string currentProductType,
            ConfigurationResponse baseConfigureResponse, ConfigurationRequest configureRequest, int setId, string sectionTab,
            ResponseMessage configureResponseJObj, ConfigurationResponse stubUnitConfigurationMainResponseObj);
        /// <summary>
        /// StartUnitHallFixtures
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="sectionTab"></param>
        /// <param name="fixtureStrategy"></param>
        /// <param name="productType"></param>
        /// <param name="variableAssignments"></param>
        /// <param name="lstunits"></param>
        /// <param name="unitHallFixtureConsoles"></param>
        /// <returns></returns>
        Task<ResponseMessage> StartUnitHallFixtures(string sessionId, string sectionTab, string fixtureStrategy, string productType, int setId,
              JObject variableAssignments = null, List<UnitNames> lstunits = null, List<UnitHallFixtures> unitHallFixtureConsoles = null);

        /// <summary>
        /// StartCustomEngineeredProductConfigure 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="productType"></param>
        /// <param name="variableAssignments"></param>
        /// <param name="permissions"></param>
        /// <param name="setId"></param>
        /// <returns></returns>
        Task<ConfigurationResponse> StartCustomEngineeredProductConfigure(string sessionId, string productType,
                   JObject variableAssignments = null, int setId = 0, List<string> permissions = null);


        /// <summary>
        /// ReleaseInfoCLMCall
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="sectionTab"></param>
        /// <param name="productType"></param>
        /// <param name="variableAssignments"></param>
        /// <param name="lstunits"></param>
        /// <param name="entranceConsoles"></param>
        /// <param name="unitHallFixtureConsoles"></param>
        /// <param name="groupConfigurationId"></param>
        /// <returns></returns>
        Task<ResponseMessage> ReleaseInfoCLMCall(string sessionId, string sectionTab, string productType,
                      JObject variableAssignments = null, List<UnitNames> lstunits = null, List<EntranceConfigurations> entranceConsoles = null, List<UnitHallFixtures> unitHallFixtureConsoles = null, int groupConfigurationId = 0);
        /// <summary>
        /// GetCrossPackageDefaultValues
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="variableAssignments"></param>
        /// <param name="groupConfigurationId"></param>
        /// <returns></returns>
        Task<string> GetCrossPackageVariableDefaultValues(JObject variableAssignmentsz, int groupId, string sessionId);
        /// <summary>
        /// GetDefaultUnitValues
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>

        Task<Dictionary<string, object>> GetDefaultUnitValues(string sessionId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="varibleAssignments"></param>
        /// <returns></returns>
        ConfigurationRequest CreateProductConfigurationRequest(JObject varibleAssignments);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configureRequest"></param>
        /// <param name="selectedTab"></param>
        /// <param name="sectionTab"></param>
        /// <param name="productType"></param>
        /// <returns></returns>
        ConfigurationRequest GenerateIncludeSections(ConfigurationRequest configureRequest, string selectedTab, string sectionTab = null, string productType = null);

        /// <summary>
        /// GetMapperVariables
        /// </summary>
        /// <returns></returns>
        List<ConfigVariable> GetMapperVariables();

        public void SetGroupDefaultsCache(string sessionId, JObject configureResponseArgumentJObject);

        /// <summary>
        /// GetDefaultValues
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="defaultType"></param>
        /// <param name="packageType"></param>
        /// <returns></returns>
        Task<List<VariableAssignment>> GetDefaultValues(string sessionId, string defaultType, string packageType);
        /// <summary>
        /// SaveConflictsValues
        /// </summary>
        /// <param name="configurationId"></param>
        /// <param name="listOfChangedVariables"></param>
        /// <returns></returns>
        List<Result> SaveConflictsValues(int configurationId, List<VariableAssignment> listOfChangedVariables,string entityType);

        /// <summary>
        /// GetSetDisplayVariableAssignmentsForGroup
        /// </summary>
        /// <param name="displayVariableAssignments"></param>
        /// <param name="groupId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task<List<DisplayVariableAssignmentsValues>> GetSetDisplayVariableAssignmentsForGroup(List<DisplayVariableAssignmentsValues> displayVariableAssignments, int groupId, string sessionId);
        Task<ResponseMessage> OBOMConfigureBl(ConfigurationRequest configurationRequest, string sessionId, string packagePath);
    }
}
