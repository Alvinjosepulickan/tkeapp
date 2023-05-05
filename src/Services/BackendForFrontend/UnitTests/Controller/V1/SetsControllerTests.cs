using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.Controllers;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.BFF.Test.Common;
using TKE.SC.BFF.APIController;
using System.Security.Principal;
using TKE.SC.Common.Model.ExceptionModel;
using System.Security.Claims;

namespace TKE.SC.BFF.UnitTests.Controller.V1
{
    public class SetsControllerTests
    {
        #region variables

        private SetsController _setConfigurationController;
        private ILogger<SetsController> _unitConfigurationControllerLogger;
        private IConfigure _configure;
        private ILogHistory _logHistory;

        #endregion

        #region PrivateMethods
        /// <summary>
        /// InitialConfigurationSetup
        /// </summary>
        [SetUp, Order(1)]
        public void InitialConfigurationSetup()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _unitConfigurationControllerLogger = services.BuildServiceProvider().GetService<ILogger<SetsController>>();
            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iUnitConfiguration = (IUnitConfiguration)servicesProvider.GetService(typeof(IUnitConfiguration));
            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
            var iLogHistory = (ILogHistory)servicesProvider.GetService(typeof(ILogHistory));
            _setConfigurationController = new SetsController(iUnitConfiguration, iConfigure, iLogHistory, _unitConfigurationControllerLogger);
            _setConfigurationController.ControllerContext = new ControllerContext();
            //setting claims for fetching session id
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("SessionId","SessionId")
                                   }, "TestAuthentication"));
            _setConfigurationController.ControllerContext.HttpContext = new DefaultHttpContext() {User = user};
            _setConfigurationController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
            _configure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
            _logHistory = (ILogHistory)servicesProvider.GetService(typeof(ILogHistory));
            var identity = new GenericIdentity("testuser", "jwt");
            identity.AddClaim(new System.Security.Claims.Claim("SessionId", "SessionId"));
            var principal = new GenericPrincipal(identity, new string[] { "SessionId" });
            _setConfigurationController.HttpContext.User = principal;

        }
        #endregion

        #region Input Values

        public static IEnumerable<TestCaseData> InputDataForStartUnitConfigure()
        {
            yield return new TestCaseData(1, 1, AppGatewayJsonFilePath.STARTUNITCONFIGREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForStartUnitConfigureCarFixture()
        {
            yield return new TestCaseData(0, 0, AppGatewayJsonFilePath.STARTUNITCONFIGCARFIXTUREREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForStartUnitConfigureError()
        {
            yield return new TestCaseData(1, 0, AppGatewayJsonFilePath.STARTUNITCONFIGREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForChangeUnitConfigure()
        {
            yield return new TestCaseData(1, "EVO_100", "GENERALINFORMATION", AppGatewayJsonFilePath.CHANGEUNITCONFIGREQUESTBODY);
        }
        public static IEnumerable<TestCaseData> InputDataForChangeUnitConfigureValue()
        {
            yield return new TestCaseData(1, 1,  AppGatewayJsonFilePath.CHANGEUNITCONFIGREQUESTBODY);
        }
        public static IEnumerable<TestCaseData> InputDataForChangeUnitConfigureError()
        {
            yield return new TestCaseData(1, "EVO_100", "GENERALINFORMATION", AppGatewayJsonFilePath.CHANGEUNITCONFIGREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForSaveCabInteriorDetails()
        {
            yield return new TestCaseData(1, "userId", "productName", AppGatewayJsonFilePath.SAVECABINTERIORREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForSaveCabInteriorDetailsError()
        {
            yield return new TestCaseData(0, "userId", "productName", AppGatewayJsonFilePath.SAVECABINTERIORREQUESTBODY);
        }
        public static IEnumerable<TestCaseData> InputDataForUpdateCabInteriorDetails()
        {
            yield return new TestCaseData(1, "userId", "productName", AppGatewayJsonFilePath.SAVECABINTERIORREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForUpdateCabInteriorDetailsError()
        {
            yield return new TestCaseData(0, "userId", "productName", AppGatewayJsonFilePath.SAVECABINTERIORREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForSaveHoistwayTractionEquipment()
        {
            yield return new TestCaseData(1, "productName", AppGatewayJsonFilePath.SAVEHOISTWAYTRACTIONEQUIPMENTREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForSaveHoistwayTractionEquipmentError()
        {
            yield return new TestCaseData(0, "productName", AppGatewayJsonFilePath.SAVEHOISTWAYTRACTIONEQUIPMENTREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForUpdateHoistwayTractionEquipment()
        {
            yield return new TestCaseData(1, "productName", AppGatewayJsonFilePath.SAVEHOISTWAYTRACTIONEQUIPMENTREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForUpdateHoistwayTractionEquipmentError()
        {
            yield return new TestCaseData(0, "productName", AppGatewayJsonFilePath.SAVEHOISTWAYTRACTIONEQUIPMENTREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForSaveGeneralInformation()
        {
            yield return new TestCaseData(1, AppGatewayJsonFilePath.SAVEGENERALINFORMATIONREQUESTBODY);
        }
        public static IEnumerable<TestCaseData> InputDataForSaveGeneralInformationError()
        {
            yield return new TestCaseData(0, AppGatewayJsonFilePath.SAVEGENERALINFORMATIONREQUESTBODY);
        }


        public static IEnumerable<TestCaseData> InputDataForSaveEntrance()
        {
            yield return new TestCaseData(1, "productName", AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForUpdateEntrance()
        {
            yield return new TestCaseData(1, "productName", AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY);
        }
        public static IEnumerable<TestCaseData> InputDataForSaveUnitConfiguration()
        {
            yield return new TestCaseData(1, 1, AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForSaveUnitConfigurationError()
        {
            yield return new TestCaseData(0, 0, AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForUpdateUnitConfigurationError()
        {
            yield return new TestCaseData(0, AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputGetTP2SummaryDetails()
        {
            yield return new TestCaseData(1);
        }

        public static IEnumerable<TestCaseData> InputGetTP2SummaryDetailsError()
        {
            yield return new TestCaseData(-1);
        }
        public static IEnumerable<TestCaseData> InputDataForChangeUnitConfigureSaveEntranceConfigure()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY);
        }
        public static IEnumerable<TestCaseData> InputDataForSaveUnitHallFixture()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEUNITHALLFIXTUREREQUESTBODY);
        }

        #endregion

        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
        public void StartUnitConfigureEntrance(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            jObject["sectionTab"] = "entrances";
            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject, 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["sections"].ToString().Trim();
            var value=JArray.Parse(key);
            string keyValue = value.Root[0]["id"].ToString().Trim();
            Assert.AreEqual(keyValue, "generalInformation");
        }

        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
        public void StartUnitConfigureErrorGeneralInformation(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject, 1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test] //to be implemented
        public void StartCarCallCutoutAssignOpeningsError()
        {
            var response = _setConfigurationController.StartCarCallCutoutAssignOpenings(0);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
        public void StartUnitConfigureGeneralInformation(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _setConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject, 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["sections"].ToString().Trim();
            var value = JArray.Parse(key);
            string keyValue = value.Root[0]["id"].ToString().Trim();
            Assert.AreEqual(keyValue, "generalInformation");
        }

        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
        public void StartUnitConfigureErrorEntrance(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            jObject["sectionTab"] = "entrances";
            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.StartUnitConfigure(groupConfigurationId, 0, jObject, 1);
            Assert.ThrowsAsync<CustomException>(() => response);

        }

        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
        public void StartUnitConfigureCabinterior(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            jObject["sectionTab"] = "CABINTERIOR";
            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject, 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["sections"].ToString().Trim();
            var value = JArray.Parse(key);
            string keyValue = value.Root[1]["id"].ToString().Trim();
            Assert.AreEqual(keyValue, "cabInterior");
        }
        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))] //TotalPrice// be implemented
        public void StartUnitConfigureErrorhoistway(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            jObject["sectionTab"] = "TRACTIONHOISTWAYEQUIPMENT";
            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject, 1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]// to be implemented
        public void StartUnitConfigurehoistway(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            jObject["sectionTab"] = "TRACTIONHOISTWAYEQUIPMENT";
            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject, 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["sections"].ToString().Trim();
            var value = JArray.Parse(key);
            string keyValue = value.Root[5]["id"].ToString().Trim();
            Assert.AreEqual(keyValue, "tractionhoistwayequipment");
        }

        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
        public void StartUnitConfigurehoistwayNotFound(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            jObject["sectionTab"] = "TRACTIONHOISTWAYEQUIPMENT";
            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject, 1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
        public void StartUnitConfigureMethod(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _setConfigurationController.StartUnitConfigure(groupConfigurationId, 1, jObject, 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["sections"].ToString().Trim();
            var value = JArray.Parse(key);
            string keyValue = value.Root[0]["id"].ToString().Trim();
            Assert.AreEqual(keyValue, "generalInformation");
        }

        [TestCaseSource(nameof(InputDataForChangeUnitConfigureValue))]
        public void ChangeUnitConfigure(int groupConfigurationId, int productName, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.ChangeUnitConfigure(1, jObject, "GENERALINFORMATION", 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["sections"].ToString().Trim();
            var value = JArray.Parse(key);
            string keyValue = value.Root[0]["id"].ToString().Trim();
            Assert.AreEqual(keyValue, "generalInformation");
        }

        [TestCaseSource(nameof(InputDataForChangeUnitConfigureError))]
        public void ChangeUnitConfigureError(int groupConfigurationId, string productName, string selectedTab, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.ChangeUnitConfigure(0, jObject, selectedTab, 1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(InputDataForChangeUnitConfigure))]
        public void ChangeUnitConfigureMethod(int groupConfigurationId, string productName, string selectedTab, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.ChangeUnitConfigure(1, jObject, selectedTab, 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["sections"].ToString().Trim();
            var value = JArray.Parse(key);
            string keyValue = value.Root[0]["id"].ToString().Trim();
            Assert.AreEqual(keyValue, "generalInformation");
        }


        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
        public void ChangeUnitConfigureCabInterior(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.ChangeUnitConfigure(setId, jObject, "CABINTERIOR", 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["sections"].ToString().Trim();
            var value = JArray.Parse(key);
            string keyValue = value.Root[1]["id"].ToString().Trim();
            Assert.AreEqual(keyValue, "cabInterior");
        }

        [TestCaseSource(nameof(InputDataForChangeUnitConfigureValue))]
        public void ChangeUnitConfigureCabInteriorError(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.ChangeUnitConfigure(0, jObject, "CABINTERIOR", 1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
        public void ChangeUnitConfigureHoistwayTractionEquipmentError(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _setConfigurationController.ChangeUnitConfigure(0, jObject, "TRACTIONHOISTWAYEQUIPMENT", 1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
        public void ChangeUnitConfigureHoistwayTractionEquipment(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _setConfigurationController.ChangeUnitConfigure(setId, jObject, "TRACTIONHOISTWAYEQUIPMENT", 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["sections"].ToString().Trim();
            var value = JArray.Parse(key);
            string keyValue = value.Root[5]["id"].ToString().Trim();
            Assert.AreEqual(keyValue, "tractionhoistwayequipment");
        }

        [TestCaseSource(nameof(InputDataForChangeUnitConfigureValue))]
        public void ChangeUnitConfigureEntrance(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _setConfigurationController.ChangeUnitConfigure(setId, jObject, "ENTRANCES",1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["sections"].ToString().Trim();
            var value = JArray.Parse(key);
            string keyValue = value.Root[4]["id"].ToString().Trim();
            Assert.AreEqual(keyValue, "entrances");
        }

        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
        public void ChangeUnitConfigureEntranceError(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _setConfigurationController.ChangeUnitConfigure(setId, jObject, "ENTRANCES",1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
        public void ChangeUnitConfigureCarFixtureError(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _setConfigurationController.ChangeUnitConfigure(setId, jObject, "CARFIXTURE",1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(InputDataForSaveUnitConfiguration))] //to be implemented
        public void SaveUnitConfiguration(int sectionId, int unitId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.SaveUnitConfiguration(sectionId, unitId, jObject,1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Units Saved Successfully");
        }

        [TestCaseSource(nameof(InputDataForSaveUnitConfigurationError))]
        public void SaveUnitConfiguration2(int sectionId, int unitId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.SaveUnitConfiguration(1, 1, jObject,1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Units Saved Successfully");
        }


        [TestCaseSource(nameof(InputDataForSaveUnitConfiguration))]
        public void SaveUnitConfigurationError(int sectionId, int unitId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.SaveUnitConfiguration(1, 2, jObject,1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }



        /// <summary>
        /// GetDetailsForTP2SummaryScreenController
        /// </summary>
        [TestCaseSource(nameof(InputGetTP2SummaryDetails))] //to be implemented
        public void GetDetailsForTP2SummaryScreen(int setId)
        {
            var response = _setConfigurationController.GetDetailsForTP2SummaryScreen(setId);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// GetListOfBuildingsForProject
        /// </summary>
        [TestCaseSource(nameof(InputGetTP2SummaryDetailsError))] //to be implemented
        public void GetDetailsForTP2SummaryScreenError(int setId)
        {
            var response = _setConfigurationController.GetDetailsForTP2SummaryScreen(0);
            Assert.ThrowsAsync<CustomException>(() => response);

        }

        [Test] //to be implemnted
        public void AddEntranceConfiguration()
        {
            var response = _setConfigurationController.StartEntranceConfigure(0, 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["name"].ToString().Trim();
            Assert.AreEqual(key, "EntranceConsole3");
        }
        [Test] //To be implemnted
        public void AddEntranceConfigurationError()
        {
            var response = _setConfigurationController.StartEntranceConfigure(0, 0);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        [Test] //To be implemneted
        public void EditEntranceConfiguration()
        {
            var response = _setConfigurationController.StartEntranceConfigure(1, 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["name"].ToString().Trim();
            Assert.AreEqual(key, "EntranceConsoleWithController");
        }
        [Test]//To be implemneted
        public void EditEntranceConfigurationError()
        {
            var response = _setConfigurationController.StartEntranceConfigure(1, 0);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        //To be implemneted
        [TestCaseSource(nameof(InputDataForChangeUnitConfigureSaveEntranceConfigure))]
        public void ChangeEntranceConfigure(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var entrancedata = jObject.ToObject<EntranceConfigurationData>();
            var response = _setConfigurationController.ChangeEntranceConfigure(1, entrancedata);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["name"].ToString().Trim();
            Assert.AreEqual(key, "EntranceConsoleWithController");
        }
        //To be implemneted
        [TestCaseSource(nameof(InputDataForChangeUnitConfigureSaveEntranceConfigure))]
        public void ChangeEntranceConfigureError(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var entrancedata = jObject.ToObject<EntranceConfigurationData>();
            var response = _setConfigurationController.ChangeEntranceConfigure(0, entrancedata);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        
        //To be implemnted
        [TestCaseSource(nameof(InputDataForChangeUnitConfigureSaveEntranceConfigure))]
        public void SaveEntranceConfigure(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var entrancedata = jObject.ToObject<EntranceConfigurationData>();
            var response = _setConfigurationController.SaveEntranceConfiguration(1, entrancedata);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Entrance Configuration Saved Successfully");
        }


        [TestCaseSource(nameof(InputDataForChangeUnitConfigureSaveEntranceConfigure))]
        public void SaveEntranceConfigureError(string requestBody) //To be implemented
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var entrancedata = jObject.ToObject<EntranceConfigurationData>();
            entrancedata.ConsoleName = "2";
            var response = _setConfigurationController.SaveEntranceConfiguration(1, entrancedata);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(InputDataForChangeUnitConfigureSaveEntranceConfigure))]
        public void UpdateEntranceConfigure(string requestBody) // to be implemented
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var entrancedata = jObject.ToObject<EntranceConfigurationData>();
            var response = _setConfigurationController.UpdateEntranceConfiguration(1, entrancedata);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Entrance Configuration Saved Successfully");
        }


        [TestCaseSource(nameof(InputDataForChangeUnitConfigureSaveEntranceConfigure))]
        public void UpdateEntranceConfigureError(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var entrancedata = jObject.ToObject<EntranceConfigurationData>();
            entrancedata.ConsoleName = "2";
            var response = _setConfigurationController.UpdateEntranceConfiguration(1, entrancedata);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputDataForStartUnitConfigure))] //to be implemented
        public void StartUnitConfigureUnitHallFixture(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            jObject[Constant.SECTIONTABS] = Constant.UNITHALLFIXTURE;
            var resultObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject, 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["sections"].ToString().Trim();
            var value = JArray.Parse(key);
            string keyValue = value.Root[3]["id"].ToString().Trim();
            Assert.AreEqual(keyValue, "unithallfixture");
        }

        [TestCaseSource(nameof(InputDataForSaveUnitHallFixture))]
        public void SaveUnitHallFixtureConfigure(string requestBody)//To be implemented
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var unitHallFixtureData = jObject.ToObject<UnitHallFixtureData>();
            unitHallFixtureData.ConsoleId = "1";
            var response = _setConfigurationController.SaveUnitHallFixtureConfiguration(1, unitHallFixtureData);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Unit Hall Fixture Saved Successfully");
        }
        [TestCaseSource(nameof(InputDataForSaveUnitHallFixture))]
        public void SaveUnitHallFixtureConfigureError(string requestBody)
        {
            //To be implemented
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var unitHallFixtureData = jObject.ToObject<UnitHallFixtureData>();
            var response = _setConfigurationController.SaveUnitHallFixtureConfiguration(1, unitHallFixtureData);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        [TestCaseSource(nameof(InputDataForSaveUnitHallFixture))]
        public void UpdateUnitHallFixtureConfigure(string requestBody)
        {
            //To be implemented
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var unitHallFixtureData = jObject.ToObject<UnitHallFixtureData>();
            unitHallFixtureData.ConsoleId = "1";
            var response = _setConfigurationController.SaveUnitHallFixtureConfiguration(1, unitHallFixtureData);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Unit Hall Fixture Saved Successfully");
        }

        [TestCaseSource(nameof(InputDataForSaveUnitHallFixture))]  //To be implemented
        public void UpdateUnitHallFixtureConfigureError(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var unitHallFixtureData = jObject.ToObject<UnitHallFixtureData>();
            unitHallFixtureData.FixtureType = "Inconspicuous_Riser";
            var response = _setConfigurationController.SaveUnitHallFixtureConfiguration(1, unitHallFixtureData);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test]
        public void StartUnitHallFixtureConsole()
        {
            List<UnitHallFixtures> lst = new List<UnitHallFixtures>();
            UnitHallFixtures console = new UnitHallFixtures();
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONSOLECACHEDATA));
            console = jObject.ToObject<UnitHallFixtures>();
            lst.Add(console);
            var cache = _configure.SetCacheUnitHallFixtureConsoles(lst, "", 1);
            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTUNITHALLFIXTURECONSOLEREQUESTBOSY));
            var response = _setConfigurationController.StartUnitHallFixtureConfigure(1, 1, "fixtureSelected");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["id"].ToString().Trim();
            Assert.AreEqual(key, "1");
        }

        [Test]
        public void StartUnitHallFixtureConsoleError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTUNITHALLFIXTURECONSOLEREQUESTBOSY));
            var response = _setConfigurationController.StartUnitHallFixtureConfigure(0, 0, "fixtureSelected");
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test]
        public void EditUnitHallFixtureConsole()
        {
            List<UnitHallFixtures> lst = new List<UnitHallFixtures>();
            UnitHallFixtures console = new UnitHallFixtures();
            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTUNITHALLFIXTURECONSOLEREQUESTBOSY));
            var response = _setConfigurationController.StartUnitHallFixtureConfigure(0, 1, "fixtureSelected");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["name"].ToString().Trim();
            Assert.AreEqual(key, "fixtureSelected 2");
        }

        [Test]
        public void EditUnitHallFixtureConsoleError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTUNITHALLFIXTURECONSOLEREQUESTBOSY));
            var response = _setConfigurationController.StartUnitHallFixtureConfigure(0, 0, "fixtureselected");
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))] //to be implemented
        public void StartUnitConfigureUnitHallFixtureError(int groupConfigurationId, int setId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            jObject[Constant.SECTIONTABS] = Constant.UNITHALLFIXTURE;
            var resultObject = jObject.ToObject<ConfigurationRequest>();
            var response = _setConfigurationController.StartUnitConfigure(groupConfigurationId, 0, jObject, 1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test]
        public void ChangeUnitConfigureUnitHallFixture()
        {
            List<UnitHallFixtures> lst = new List<UnitHallFixtures>();
            UnitHallFixtures console1 = new UnitHallFixtures();
            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONSOLECACHEDATA));
            console1 = jObject1.ToObject<UnitHallFixtures>();
            lst.Add(console1);
            var cache = _configure.SetCacheUnitHallFixtureConsoles(lst, "", 1);
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGECONSOLECACHEDDATA));
            UnitHallFixtureData console = new UnitHallFixtureData();
            console = jObject.ToObject<UnitHallFixtureData>();
            var response = _setConfigurationController.ChangeUnitHallFixtureConfigure(1, console);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["id"].ToString().Trim();
            Assert.AreEqual(key, "1");
        }

        [Test]
        public void ChangeUnitConfigureUnitHallFixtureError()
        {
            List<UnitHallFixtures> lst = new List<UnitHallFixtures>();
            UnitHallFixtures console1 = new UnitHallFixtures();
            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONSOLECACHEDATA));
            console1 = jObject1.ToObject<UnitHallFixtures>();
            lst.Add(console1);
            var cache = _configure.SetCacheUnitHallFixtureConsoles(lst, "", 1);

            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGECONSOLECACHEDDATA));
            UnitHallFixtureData console = new UnitHallFixtureData();
            console = jObject.ToObject<UnitHallFixtureData>();
            var response = _setConfigurationController.ChangeUnitHallFixtureConfigure(0, console);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [Test]
        public void DeleteUnitHallFixture()
        {
            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTUNITHALLFIXTURECONSOLEREQUESTBOSY));
            var response = _setConfigurationController.DeleteUnitHallFixtureConfigure(1, 1, "fixtureSelected");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Unit Hall Fixture Deleted Successfully");
        }

        [Test]
        public void DeleteUnitHallFixtureError()
        {
            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTUNITHALLFIXTURECONSOLEREQUESTBOSY));
            var response = _setConfigurationController.DeleteUnitHallFixtureConfigure(0, 1, "fixture seleted");
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test] //to be implemented
        public void SaveCarCallCutoutKeyswitchOpenings()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVECARCALLCUTOUTKEYSWITCHOPENINGS));
            var CarCallCutoutKeyswitchOpenings = jObject.ToObject<CarcallCutoutData>();
            var response = _setConfigurationController.SaveCarCallCutoutKeyswitchOpenings(1, CarCallCutoutKeyswitchOpenings);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Car Call Saved");
        }

        [Test]// to be implemented
        public void SaveCarCallCutoutKeyswitchOpeningsError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVECARCALLCUTOUTKEYSWITCHOPENINGS));
            var CarCallCutoutKeyswitchOpenings = jObject.ToObject<CarcallCutoutData>();
            var response = _setConfigurationController.SaveCarCallCutoutKeyswitchOpenings(0, CarCallCutoutKeyswitchOpenings);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [Test] //to be implemented
        public void StartCarCallCutoutAssignOpenings()
        {
            var response = _setConfigurationController.StartCarCallCutoutAssignOpenings(1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["name"].ToString().Trim();
            Assert.AreEqual(key, "CarCallCutoutKeyswitchesConsole");
        }

        [Test] //to be implemented
        public void StartCarCallCutoutAssignOpeningsErrors()
        {
            var response = _setConfigurationController.StartCarCallCutoutAssignOpenings(0);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test] //to be implemented
        public void EditUnitDesignationTest()
        {
            UnitDesignation unitDesignation = new UnitDesignation();
            unitDesignation.Description = "unitDescription";
            unitDesignation.Designation = "unitDesignation";
            var response = _setConfigurationController.EditUnitDesignation(1, 1, unitDesignation);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["message"].ToString().Trim();
            Assert.AreEqual(key, "UnitDetails updated successfully");
        }

        [Test] //to be implemented
        public void EditUnitDesignationErrorTest()
        {
            UnitDesignation unitDesignation = new UnitDesignation();
            unitDesignation.Description = "unitDescription";
            unitDesignation.Designation = "unitDesignation";
            var response = _setConfigurationController.EditUnitDesignation(0, 1, unitDesignation);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test] //to be implemented
        public void DeleteEntranceConfigurationTest()
        {
            var response = _setConfigurationController.DeleteEntranceConfigure(1, 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Entrance Configuration Deleted");
        }

        [Test] //to be implemented
        public void DeleteEntranceConfigurationErrorTest()
        {
            var response = _setConfigurationController.DeleteEntranceConfigure(0, 1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        //[TestCaseSource(nameof(InputDataForUpdateUnitConfigurationError))]
        //public void UpdateUnitErrorConfiguration(int unitId, string requestBody)
        //{
        //    var jObject = JObject.Parse(File.ReadAllText(requestBody));
        //    var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
        //    var response = _setConfigurationController.UpdateUnitConfiguration(unitId, jObject);
        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
        //}

        //[TestCaseSource(nameof(InputDataForSaveUnitConfiguration))]
        //public void UpdateUnitConfiguration(int unitId, string requestBody)
        //{
        //    var jObject = JObject.Parse(File.ReadAllText(requestBody));
        //    var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
        //    var response = _setConfigurationController.UpdateUnitConfiguration(unitId, jObject);
        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
        //}

        //[TestCaseSource(nameof(InputDataForSaveUnitConfiguration))]
        //public void UpdateUnitConfigurationError(int unitId, string requestBody)
        //{
        //    var jObject = JObject.Parse(File.ReadAllText(requestBody));
        //    var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
        //    var response = _setConfigurationController.UpdateUnitConfiguration(0, jObject);
        //    Assert.IsNotNull(response);
        //}

        //[TestCaseSource(nameof(InputDataForSaveEntrance))]
        //public void SaveEntrance(int groupid, string productName, string requestBody)
        //{
        //    var jObject = JObject.Parse(File.ReadAllText(requestBody));
        //    var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
        //    var response = _setConfigurationController.SaveEntrances(groupid, productName, jObject);
        //    Assert.IsNotNull(response);
        //}

        //[TestCaseSource(nameof(InputDataForSaveEntrance))]
        //public void SaveEntranceError(int groupid, string productName, string requestBody)
        //{
        //    var jObject = JObject.Parse(File.ReadAllText(requestBody));
        //    var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
        //    var response = _setConfigurationController.SaveEntrances(groupid, productName, jObject);
        //    Assert.IsNotNull(response);
        //}

        //[TestCaseSource(nameof(InputDataForUpdateEntrance))]
        //public void UpdateEntrance(int groupid, string productName, string requestBody)
        //{
        //    var jObject = JObject.Parse(File.ReadAllText(requestBody));
        //    var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
        //    var response = _setConfigurationController.UpdateEntrances(groupid, productName, jObject);
        //    Assert.IsNotNull(response);
        //}

        //[TestCaseSource(nameof(InputDataForUpdateEntrance))]
        //public void UpdateEntranceError(int groupid, string productName, string requestBody)
        //{
        //    var jObject = JObject.Parse(File.ReadAllText(requestBody));
        //    var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
        //    var response = _setConfigurationController.UpdateEntrances(groupid, productName, jObject);
        //    Assert.IsNotNull(response);
        //}

        ////To be implemented
        //[TestCaseSource(nameof(InputDataForStartUnitConfigureError))] /*[Test] to be implemented*/
        //public void StartCarCallCutoutAssignOpeningsError()
        //{
        //    var response = _setConfigurationController.StartCarCallCutoutAssignOpenings(0);
        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
        //}

        //public void StartUnitConfigureErrorCabinterior(int groupConfigurationId, int setId, string requestBody)
        //{
        //    var jObject = JObject.Parse(File.ReadAllText(requestBody));
        //    jObject["sectionTab"] = "CABINTERIOR";
        //    var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
        //    setId = 0;
        //    var response = _setConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject, 1);
        //    Assert.ThrowsAsync<CustomException>(() => response);
        //}
        //To be implemnted
        //[TestCaseSource(nameof(InputDataForStartUnitConfigureError))] 
        //public void StartUnitConfigureErrorGeneralInformationError(int groupConfigurationId, int setId, string requestBody)
        //{
        //    var jObject = JObject.Parse(File.ReadAllText(requestBody));
        //    var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
        //    var response = _setConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
        //}

        //[TestCaseSource(nameof(InputDataForStartUnitConfigureError))] to be implemented
        //public void StartCarCallCutoutAssignOpeningsError()
        //{
        //    var response = _setConfigurationController.StartCarCallCutoutAssignOpenings(0);
        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
        //}
    }
}
