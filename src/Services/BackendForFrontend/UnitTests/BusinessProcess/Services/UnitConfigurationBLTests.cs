/************************************************************************************************************
************************************************************************************************************
    File Name     :   UnitConfigurationBLTests.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.BusinessProcess.Services;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.BFF.Test.Common;
using TKE.SC.Common.Model.ExceptionModel;

namespace TKE.SC.BFF.Test.BusinessProcess.Services
{
    class UnitConfigurationBLTests
    {
        #region Variables
        private UnitConfigurationBL _unitConfiguration;
        private ILogger<UnitConfigurationBL> _unitConfigurationBLLogger;
        private ConfigureBL _configurebl;
        private Utility _utility;
        #endregion

        #region PrivateMethods
        [SetUp]
        public void InitialConfiguration()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _unitConfigurationBLLogger = services.BuildServiceProvider().GetService<ILogger<UnitConfigurationBL>>();
            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iUnitConfiguration = (IUnitConfigurationDL)servicesProvider.GetService(typeof(IUnitConfigurationDL));
            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
            var ilogger = (ILogger<ConfigureBL>)servicesProvider.GetService(typeof(ILogger<ConfigureBL>));
            var caching = (ICacheManager)servicesProvider.GetService(typeof(ICacheManager));
            var iProject = (IProject)servicesProvider.GetService(typeof(IProject));
            var iProductSelection = (IProductSelection)servicesProvider.GetService(typeof(IProductSelection));
            _unitConfiguration = new UnitConfigurationBL(iConfigure, iUnitConfiguration, _unitConfigurationBLLogger, iProject, caching,iProductSelection);
            var iConfigureServices = (IConfigureServices)servicesProvider.GetService(typeof(IConfigureServices));
            var iConfiguratorServices = (IConfiguratorService)servicesProvider.GetService(typeof(IConfiguratorService));
            var http = (IHttpContextAccessor)servicesProvider.GetService(typeof(IHttpContextAccessor));
            var genToken = (IStringLocalizer<GenerateTokenBL>)servicesProvider.GetService(typeof(IStringLocalizer<GenerateTokenBL>));
            var iGroup = (IGroupConfigurationDL)servicesProvider.GetService(typeof(IGroupConfigurationDL));
            var iUnit = (IUnitConfigurationDL)servicesProvider.GetService(typeof(IUnitConfigurationDL));
            var iauth = (IAuth)servicesProvider.GetService(typeof(IAuth));
            var iOpeningLocation = (IOpeningLocationDL)servicesProvider.GetService(typeof(IOpeningLocationDL));
            var iConfiguration = (IConfiguration)servicesProvider.GetService(typeof(IConfiguration));
            _configurebl = new ConfigureBL(iConfiguration, iConfigureServices, iConfiguratorServices, iauth, new Newtonsoft.Json.JsonSerializer(), caching, iOpeningLocation, http, genToken, iGroup, iUnitConfiguration, ilogger);

        }
        #endregion

        #region Input Values

        public static IEnumerable<TestCaseData> InputDataForStartUnitConfigure()
        {
            yield return new TestCaseData(1, "EVO_100", "GENERALINFORMATION", "", AppGatewayJsonFilePath.STARTUNITCONFIGREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForStartUnitConfigureError()
        {
            yield return new TestCaseData(1, "EVO_100", "GENERALINFORMATION", "", AppGatewayJsonFilePath.STARTUNITCONFIGREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForChangeUnitConfigure()
        {
            yield return new TestCaseData(1, "EVO_100", "GENERALINFORMATION", AppGatewayJsonFilePath.CHANGEUNITCONFIGREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForChangeUnitConfigureError()
        {
            yield return new TestCaseData(1, "EVO_100", "GENERALINFORMATION", AppGatewayJsonFilePath.CHANGEUNITCONFIGREQUESTBODY);
        }


        public static IEnumerable<TestCaseData> InputValuesForSaveCabInterior()
        {
            yield return new TestCaseData(55, "productName", AppGatewayJsonFilePath.SAVECABINTERIORREQUESTBODY, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputValuesForSaveCabInteriorError()
        {
            yield return new TestCaseData(55, null, AppGatewayJsonFilePath.SAVECABINTERIORREQUESTBODY, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputValuesForUpdateCabInterior()
        {
            yield return new TestCaseData(55, "productName", AppGatewayJsonFilePath.SAVECABINTERIORREQUESTBODY, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputValuesForUpdateCabInteriorError()
        {
            yield return new TestCaseData(55, null, AppGatewayJsonFilePath.SAVECABINTERIORREQUESTBODY, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputValuesForSaveHoistwayTractionEquipment()
        {
            yield return new TestCaseData(55, "productName", AppGatewayJsonFilePath.SAVEHOISTWAYTRACTIONEQUIPMENTREQUESTBODY, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputValuesForSaveHoistwayTractionEquipmentError()
        {
            yield return new TestCaseData(0, "productName", AppGatewayJsonFilePath.SAVEHOISTWAYTRACTIONEQUIPMENTREQUESTBODY, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputValuesForUpdateHoistwayTractionEquipment()
        {
            yield return new TestCaseData(55, "productName", AppGatewayJsonFilePath.SAVEHOISTWAYTRACTIONEQUIPMENTREQUESTBODY, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputValuesForUpdateHoistwayTractionEquipmentError()
        {
            yield return new TestCaseData(0, "productName", AppGatewayJsonFilePath.SAVEHOISTWAYTRACTIONEQUIPMENTREQUESTBODY, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputValuesForSaveEntrance()
        {
            yield return new TestCaseData(5, "productName", AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputValuesForSaveEntranceError()
        {
            yield return new TestCaseData(0, "productName", AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputValuesForUpdateEntrance()
        {
            yield return new TestCaseData(1, "productName", AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputValuesForUpdateEntranceError()
        {
            yield return new TestCaseData(0, "productName", AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputValuesForSaveGeneralInformation()
        {
            yield return new TestCaseData(1, "productName", AppGatewayJsonFilePath.SAVEGENERALINFORMATIONREQUESTBODY, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputValuesForSaveGeneralInformationError()
        {
            yield return new TestCaseData(0, "productName", AppGatewayJsonFilePath.SAVEGENERALINFORMATIONREQUESTBODY, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputValuesForSaveUnitConfiguration()
        {
            yield return new TestCaseData(1, AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputGetTP2SummaryDetails()
        {
            yield return new TestCaseData(1, "sessionId");
        }

        public static IEnumerable<TestCaseData> InputGetTP2SummaryDetailsError()
        {
            yield return new TestCaseData(-1, "sessionId");
        }


        #endregion

        //To be implemnted
        [TestCaseSource(nameof(InputValuesForSaveUnitConfiguration))]
        public void SaveUnitConfiguration(int setId, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.SaveUnitConfiguration(setId, jObject, sessionId, 1).Result;
            Assert.AreEqual(200, response.StatusCode);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForSaveUnitConfiguration))]
        public void SaveUnitConfigurationError(int setId, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.SaveUnitConfiguration(0, jObject, sessionId, 1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForSaveUnitConfiguration))]
        public void SaveUnit(int setId, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.UpdateUnitConfiguration(setId, jObject, sessionId, 1).Result;
            Assert.AreEqual(200, response.StatusCode);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForSaveUnitConfiguration))]
        public void UpdateUnitConfigurationError(int setId, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.UpdateUnitConfiguration(0, jObject, sessionId, 1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        ////To be implemnted
        [TestCaseSource(nameof(InputValuesForSaveCabInterior))]
        public void SaveCabInteriorDetails(int groupId, string productName, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.SaveCabInteriorDetails(groupId, productName, jObject, sessionId).Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        //To be implemnted
        [TestCaseSource(nameof(InputValuesForSaveCabInteriorError))]
        public void SaveCabInteriorDetailsError(int groupId, string productName, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.SaveCabInteriorDetails(0, productName, jObject, sessionId);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForUpdateCabInterior))]
        public void UpdateCabInteriorDetails(int groupId, string productName, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.UpdateCabInteriorDetails(groupId, productName, jObject, sessionId).Result;
            Assert.AreEqual(200, response.StatusCode);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForUpdateCabInteriorError))]
        public void UpdateCabInteriorDetailsError(int groupId, string productName, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.UpdateCabInteriorDetails(0, productName, jObject, sessionId);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForSaveHoistwayTractionEquipment))]
        public void SaveHoistwayTractionEquipment(int groupId, string productName, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.SaveHoistwayTractionEquipment(groupId, productName, jObject, sessionId).Result;
            Assert.AreEqual(200, response.StatusCode);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForSaveHoistwayTractionEquipmentError))]
        public void SaveHoistwayTractionEquipmentError(int groupId, string productName, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.SaveHoistwayTractionEquipment(groupId, productName, jObject, sessionId);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForUpdateHoistwayTractionEquipment))]
        public void UpdateHoistwayTractionEquipment(int groupId, string productName, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.UpdateHoistwayTractionEquipment(groupId, productName, jObject, sessionId).Result;
            Assert.AreEqual(200, response.StatusCode);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForUpdateHoistwayTractionEquipmentError))]
        public void UpdateHoistwayTractionEquipmentError(int groupId, string productName, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.UpdateHoistwayTractionEquipment(groupId, productName, jObject, sessionId);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForSaveEntrance))]
        public void SaveEntrance(int groupId, string productName, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.SaveEntrances(groupId, productName, jObject, sessionId).Result;
            Assert.AreEqual(200, response.StatusCode);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForSaveEntranceError))]
        public void SaveEntranceError(int groupId, string productName, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.SaveEntrances(0, productName, jObject, sessionId);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForUpdateEntrance))]
        public void UpdateEntrance(int groupId, string productName, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.UpdateEntrances(groupId, productName, jObject, sessionId).Result;
            Assert.AreEqual(200, response.StatusCode);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForSaveEntranceError))]
        public void UpdateEntranceError(int groupId, string productName, string requestBody, string sessionId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _unitConfiguration.UpdateEntrances(groupId, productName, jObject, sessionId);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        //To be implemnted
        [TestCaseSource(nameof(InputGetTP2SummaryDetails))] //To be implemnt
        public void GetTP2SummaryDetails(int unitId, string sessionId)
        {
            var response = _unitConfiguration.GetDetailsForTP2SummaryScreen(unitId, sessionId).Result;
            Assert.IsNotNull(response);
            //Assert.AreEqual(200, response.StatusCode);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputGetTP2SummaryDetailsError))]
        public void GetTP2SummaryDetailsError(int unitId, string sessionId)
        {
            var response = _unitConfiguration.GetDetailsForTP2SummaryScreen(unitId, sessionId);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        [Test] //To be implemnted
        public void startUnitConfigureEntrances()
        {
            var variableassignment = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY));
            var response = _unitConfiguration.StartUnitConfigure(variableassignment, 1, 1, "sessionId", "entrances", 1).Result;
            Assert.AreEqual(200, response.StatusCode);
        }
        [Test] //To be implemnted
        public void startUnitConfigureEntrancesError()
        {
            var variableassignment = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY));
            var response = _unitConfiguration.StartUnitConfigure(variableassignment, 0, 0, "SessionIdTest", "entrances", 1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        [Test] //To be implemnted
        public void SaveUnitHallFixtureConfigure()
        {
            var variableassignment = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEUNITHALLFIXTUREREQUESTBODY));
            var unitHallFixtureData = variableassignment.ToObject<UnitHallFixtureData>();
            unitHallFixtureData.ConsoleId = 1;
            var response = _unitConfiguration.SaveUnitHallFixtureConfiguration(1, unitHallFixtureData, "sessionId", 0).Result;
            Assert.AreEqual(200, response.StatusCode);
        }
        [Test] //To be implemnted
        public void SaveUnitHallFixtureConfigureError()
        {
            var variableassignment = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEUNITHALLFIXTUREREQUESTBODY));
            var unitHallFixtureData = variableassignment.ToObject<UnitHallFixtureData>();
            var response = _unitConfiguration.SaveUnitHallFixtureConfiguration(1, unitHallFixtureData, "sessionId", 0);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test] //To be implemnted
        public void StartUnitHallFixtureConsole()
        {
            List<UnitHallFixtures> lst = new List<UnitHallFixtures>();
            UnitHallFixtures console1 = new UnitHallFixtures();
            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.EDITUNITHLLFIXTURECONSOLEDATA));
            console1 = jObject1.ToObject<UnitHallFixtures>();
            lst.Add(console1);
            var response = _unitConfiguration.StartUnitHallFixtureConfigure(0, 0, 1, "sessionId", "fixtureSelected", false).Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        [Test] //To be implemnted
        public void StartUnitHallFixtureConsoleError()
        {
            var response = _unitConfiguration.StartUnitHallFixtureConfigure(0, 0, 5, "sessionId", "fixtureSelected", false);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test] //To be implemnted
        public void DeleteUnitHallFixture()
        {
            var response = _unitConfiguration.DeleteUnitHallFixtureConsole(1, 1, "fixtureSelected", "sessionId").Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        [Test] //To be implemnted
        public void DeleteUnitHallFixtureError()
        {
            var response = _unitConfiguration.DeleteUnitHallFixtureConsole(0, 1, "fixtureType", "sessioId");
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test]
        public void EditUnitDesignation()
        {
            UnitDesignation unit = new UnitDesignation();
            unit.Designation = "Unit1";
            unit.Description = "Description";
            var response = _unitConfiguration.EditUnitDesignation(1, 1, "sessionId", unit).Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        [Test] //To be implemnted
        public void EditUnitDesignationError()
        {
            UnitDesignation unit = new UnitDesignation();
            unit.Designation = "";
            unit.Description = "Description";
            var response = _unitConfiguration.EditUnitDesignation(0, 1, "sessionId", unit);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
    }
}

