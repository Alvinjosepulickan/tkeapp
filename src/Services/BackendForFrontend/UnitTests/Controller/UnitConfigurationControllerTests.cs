//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json.Linq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using TKE.SC.BFF.BusinessProcess.Helpers;
//using TKE.SC.BFF.BusinessProcess.Interfaces;
//using TKE.SC.BFF.Controllers;
//using TKE.SC.Common.Model;
//using TKE.SC.Common.Model.UIModel;
//using TKE.SC.Common.Model.ViewModel;
//using TKE.SC.BFF.Test.Common;

//namespace TKE.SC.BFF.Test.Controller
//{
//    class UnitConfigurationControllerTests
//    {
//        #region variables

//        private UnitConfigurationController _unitConfigurationController;
//        private ILogger<UnitConfigurationController> _unitConfigurationControllerLogger;
//        private  IConfigure _configure;
//        private ILogHistory _logHistory;

//        #endregion

//        #region PrivateMethods
//        /// <summary>
//        /// InitialConfigurationSetup
//        /// </summary>
//        [SetUp, Order(1)]
//        public void InitialConfigurationSetup()
//        {
//            CommonFunctions.InitialConfiguration();
//            var services = CommonFunctions.ServiceCollection();
//            _unitConfigurationControllerLogger = services.BuildServiceProvider().GetService<ILogger<UnitConfigurationController>>();
//            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
//            var iUnitConfiguration = (IUnitConfiguration)servicesProvider.GetService(typeof(IUnitConfiguration));
//            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
//            var iLogHistory = (ILogHistory)servicesProvider.GetService(typeof(ILogHistory));
//            _unitConfigurationController = new UnitConfigurationController(_unitConfigurationControllerLogger, iUnitConfiguration, iConfigure, iLogHistory);
//            _unitConfigurationController.ControllerContext = new ControllerContext();
//            _unitConfigurationController.ControllerContext.HttpContext = new DefaultHttpContext();
//            _unitConfigurationController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
//            _configure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
//            _logHistory = (ILogHistory)servicesProvider.GetService(typeof(ILogHistory));

//        }
//        #endregion

//        #region Input Values

//        public static IEnumerable<TestCaseData> InputDataForStartUnitConfigure()
//        {
//            yield return new TestCaseData(1, 1, AppGatewayJsonFilePath.STARTUNITCONFIGREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForStartUnitConfigureCarFixture()
//        {
//            yield return new TestCaseData(0, 0, AppGatewayJsonFilePath.STARTUNITCONFIGCARFIXTUREREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForStartUnitConfigureError()
//        {
//            yield return new TestCaseData(1, 0, AppGatewayJsonFilePath.STARTUNITCONFIGREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForChangeUnitConfigure()
//        {
//            yield return new TestCaseData(1, "EVO_100", "GENERALINFORMATION", AppGatewayJsonFilePath.CHANGEUNITCONFIGREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForChangeUnitConfigureError()
//        {
//            yield return new TestCaseData(1, "EVO_100", "GENERALINFORMATION", AppGatewayJsonFilePath.CHANGEUNITCONFIGREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForSaveCabInteriorDetails()
//        {
//            yield return new TestCaseData( 1, "userId", "productName", AppGatewayJsonFilePath.SAVECABINTERIORREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForSaveCabInteriorDetailsError()
//        {
//            yield return new TestCaseData(0, "userId", "productName", AppGatewayJsonFilePath.SAVECABINTERIORREQUESTBODY);
//        }
//        public static IEnumerable<TestCaseData> InputDataForUpdateCabInteriorDetails()
//        {
//            yield return new TestCaseData(1, "userId", "productName", AppGatewayJsonFilePath.SAVECABINTERIORREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForUpdateCabInteriorDetailsError()
//        {
//            yield return new TestCaseData(0, "userId", "productName", AppGatewayJsonFilePath.SAVECABINTERIORREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForSaveHoistwayTractionEquipment()
//        {
//            yield return new TestCaseData(1, "productName", AppGatewayJsonFilePath.SAVEHOISTWAYTRACTIONEQUIPMENTREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForSaveHoistwayTractionEquipmentError()
//        {
//            yield return new TestCaseData(0, "productName", AppGatewayJsonFilePath.SAVEHOISTWAYTRACTIONEQUIPMENTREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForUpdateHoistwayTractionEquipment()
//        {
//            yield return new TestCaseData(1, "productName", AppGatewayJsonFilePath.SAVEHOISTWAYTRACTIONEQUIPMENTREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForUpdateHoistwayTractionEquipmentError()
//        {
//            yield return new TestCaseData(0,  "productName", AppGatewayJsonFilePath.SAVEHOISTWAYTRACTIONEQUIPMENTREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForSaveGeneralInformation()
//        {
//            yield return new TestCaseData(1, AppGatewayJsonFilePath.SAVEGENERALINFORMATIONREQUESTBODY);
//        }
//        public static IEnumerable<TestCaseData> InputDataForSaveGeneralInformationError()
//        {
//            yield return new TestCaseData(0, AppGatewayJsonFilePath.SAVEGENERALINFORMATIONREQUESTBODY);
//        }


//        public static IEnumerable<TestCaseData> InputDataForSaveEntrance()
//        {
//            yield return new TestCaseData(1, "productName", AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForUpdateEntrance()
//        {
//            yield return new TestCaseData(1, "productName", AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY);
//        }
//        public static IEnumerable<TestCaseData> InputDataForSaveUnitConfiguration()
//        {
//            yield return new TestCaseData(1, AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForSaveUnitConfigurationError()
//        {
//            yield return new TestCaseData(0, AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForUpdateUnitConfigurationError()
//        {
//            yield return new TestCaseData(0, AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputGetTP2SummaryDetails()
//        {
//            yield return new TestCaseData(1);
//        }

//        public static IEnumerable<TestCaseData> InputGetTP2SummaryDetailsError()
//        {
//            yield return new TestCaseData(-1);
//        }
//        public static IEnumerable<TestCaseData> InputDataForChangeUnitConfigureSaveEntranceConfigure()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEENTRANCEREQUESTBODY);
//        }
//        public static IEnumerable<TestCaseData> InputDataForSaveUnitHallFixture()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEUNITHALLFIXTUREREQUESTBODY);
//        }
       
//        #endregion

//        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
//        public void StartUnitConfigureEntrance(int groupConfigurationId, int setId,  string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            jObject["sectionTab"] = "entrances";
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigureCarFixture))]
//        public void StartUnitConfigureCarFixture(int groupConfigurationId, int setId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            jObject["sectionTab"] = "carfixture";
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
//        public void StartUnitConfigureErrorGeneralInformation(int groupConfigurationId, int setId,  string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
//        public void StartUnitConfigureGeneralInformation(int groupConfigurationId, int setId,  string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _unitConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
//        public void StartUnitConfigureErrorGeneralInformationError(int groupConfigurationId, int setId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
//        public void StartUnitConfigureErrorEntrance(int groupConfigurationId, int setId,  string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            jObject["sectionTab"] = "entrances";
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
//        public void StartUnitConfigureErrorCabinterior(int groupConfigurationId, int setId,  string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            jObject["sectionTab"] = "CABINTERIOR";
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }
//        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
//        public void StartUnitConfigureCabinterior(int groupConfigurationId, int setId,  string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            jObject["sectionTab"] = "CABINTERIOR";
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
//        public void StartUnitConfigureErrorhoistway(int groupConfigurationId, int setId,  string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            jObject["sectionTab"] = "TRACTIONHOISTWAYEQUIPMENT";
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }
//        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
//        public void StartUnitConfigurehoistway(int groupConfigurationId, int setId,  string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            jObject["sectionTab"] = "TRACTIONHOISTWAYEQUIPMENT";
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
//        public void StartUnitConfigurehoistwayNotFound(int groupConfigurationId, int setId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            jObject["sectionTab"] = "TRACTIONHOISTWAYEQUIPMENT";
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
//        public void StartUnitConfigureMethod(int groupConfigurationId, int setId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _unitConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
//        public void StartUnitConfigureModelStateError(int groupConfigurationId, int setId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var unitObject = jObject.ToObject<ConfigurationRequest>();
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForChangeUnitConfigure))]
//        public void ChangeUnitConfigure(int groupConfigurationId, string productName, string selectedTab, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.ChangeUnitConfigure(jObject, selectedTab,0);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [TestCaseSource(nameof(InputDataForChangeUnitConfigureError))]
//        public void ChangeUnitConfigureError(int groupConfigurationId, string productName, string selectedTab, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.ChangeUnitConfigure(jObject, selectedTab,0);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForChangeUnitConfigure))]
//        public void ChangeUnitConfigureMethod(int groupConfigurationId, string productName, string selectedTab, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.ChangeUnitConfigure(jObject, selectedTab,0);
//            Assert.IsNotNull(response);
//        }


//        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
//        public void ChangeUnitConfigureCabInterior(int groupConfigurationId, int setId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.ChangeUnitConfigure(jObject, "CABINTERIOR", 0);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
//        public void ChangeUnitConfigureCabInteriorError(int groupConfigurationId, int setId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.ChangeUnitConfigure(jObject, "CABINTERIOR",0);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
//        public void ChangeUnitConfigureHoistwayTractionEquipmentError(int groupConfigurationId, int setId,  string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _unitConfigurationController.ChangeUnitConfigure(jObject, "HOISTWAYTRACTIONEQUIPMENT",0);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
//        public void ChangeUnitConfigureHoistwayTractionEquipment(int groupConfigurationId, int setId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _unitConfigurationController.ChangeUnitConfigure(jObject, "HOISTWAYTRACTIONEQUIPMENT", 0);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
//        public void ChangeUnitConfigureEntrance(int groupConfigurationId, int setId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _unitConfigurationController.ChangeUnitConfigure(jObject, "ENTRANCES", 0);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
//        public void ChangeUnitConfigureEntranceError(int groupConfigurationId, int setId,  string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _unitConfigurationController.ChangeUnitConfigure(jObject, "ENTRANCES",0);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
//        public void ChangeUnitConfigureCarFixture(int groupConfigurationId, int setId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _unitConfigurationController.ChangeUnitConfigure(jObject, "CARFIXTURE");
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
//        public void ChangeUnitConfigureCarFixtureError(int groupConfigurationId, int setId,  string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _unitConfigurationController.ChangeUnitConfigure(jObject, "CARFIXTURE");
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveUnitConfiguration))]
//        public void SaveUnitConfiguration(int unitId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.SaveUnitConfiguration(unitId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [TestCaseSource(nameof(InputDataForSaveUnitConfigurationError))]
//        public void SaveUnitErrorConfiguration(int unitId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.SaveUnitConfiguration(unitId, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveUnitConfiguration))]
//        public void SaveUnitConfigurationError(int unitId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.SaveUnitConfiguration(0, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveUnitConfiguration))]
//        public void SaveUnitConfigurationModelStateError(int unitId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.SaveUnitConfiguration(unitId, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForUpdateUnitConfigurationError))]
//        public void UpdateUnitErrorConfiguration(int unitId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.UpdateUnitConfiguration(unitId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForSaveUnitConfiguration))]
//        public void UpdateUnitConfiguration(int unitId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.UpdateUnitConfiguration(unitId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [TestCaseSource(nameof(InputDataForSaveUnitConfiguration))]
//        public void UpdateUnitConfigurationError(int unitId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.UpdateUnitConfiguration(0, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveUnitConfiguration))]
//        public void UpdateUnitConfigurationModelStateError(int unitId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.UpdateUnitConfiguration(unitId, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveCabInteriorDetails))]
//        public void SaveCabInteriorDetails(int groupid, string userId, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response =_unitConfigurationController.SaveCabInteriorDetails(groupid, productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveCabInteriorDetailsError))]
//        public void SaveCabInteriorDetailsError(int groupid, string userId, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.SaveCabInteriorDetails(groupid,  productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveCabInteriorDetails))]
//        public void SaveCabInteriorDetailsModelStateError(int groupid, string userId, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.SaveCabInteriorDetails(groupid, productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForUpdateCabInteriorDetails))]
//        public void UpdateCabInteriorDetails(int groupid, string userId, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.UpdateCabInteriorDetails(groupid,  productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForUpdateCabInteriorDetailsError))]
//        public void UpdateCabInteriorDetailsError(int groupid, string userId, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.SaveCabInteriorDetails(-1,  productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForUpdateCabInteriorDetails))]
//        public void UpdateCabInteriorDetailsModelStateError(int groupid, string userId, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.UpdateCabInteriorDetails(groupid,  productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveHoistwayTractionEquipment))]
//        public void SaveHoistwayTractionEquipment(int groupid, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.SaveHoistwayTractionEquipment(groupid,  productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveHoistwayTractionEquipmentError))]
//        public void SaveHoistwayTractionEquipmentError(int groupid, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.SaveHoistwayTractionEquipment(groupid, productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveHoistwayTractionEquipment))]
//        public void SaveHoistwayTractionEquipmentModelStateError(int groupid, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.SaveHoistwayTractionEquipment(groupid, productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveHoistwayTractionEquipment))]
//        public void UpdateHoistwayTractionEquipment(int groupid, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.UpdateHoistwayTractionEquipment(groupid,  productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveHoistwayTractionEquipmentError))]
//        public void UpdateHoistwayTractionEquipmentError(int groupid,  string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.UpdateHoistwayTractionEquipment(groupid,  productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveHoistwayTractionEquipment))]
//        public void UpdateHoistwayTractionEquipmentModelStateError(int groupid, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.UpdateHoistwayTractionEquipment(groupid,  productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveGeneralInformation))]
//        public void SaveGroupLayoutDetails(int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _unitConfigurationController.SaveGeneralInformation(groupId,"EVO_100" ,jObject);
//            Assert.IsNotNull(response);
//        }
//        /// <summary>
//        /// SaveGeneralInformationError
//        /// </summary>
//        /// <param name="groupId"></param>
//        /// <param name="requestBody"></param>
//        [TestCaseSource(nameof(InputDataForSaveGeneralInformationError))]
//        public void SaveGeneralInformationError(int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _unitConfigurationController.SaveGeneralInformation(groupId,"EVO_100" ,jObject);
//            Assert.IsNotNull(response);
//        }
//        /// <summary>
//        /// SaveGeneralInformationModelStateError
//        /// </summary>
//        /// <param name="groupId"></param>
//        /// <param name="requestBody"></param>
//        [TestCaseSource(nameof(InputDataForSaveGeneralInformationError))]
//        public void SaveGeneralInformationModelStateError(int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.SaveGeneralInformation(groupId,"" ,jObject);
//            Assert.IsNotNull(response);
//        }
//        /// <summary>
//        /// SaveGeneralInformationExceptionError
//        /// </summary>
//        /// <param name="groupId"></param>
//        /// <param name="requestBody"></param>
//        [TestCaseSource(nameof(InputDataForSaveGeneralInformation))]
//        public void SaveGeneralInformationExceptionError(int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _unitConfigurationController.SaveGeneralInformation(groupId, "",jObject);
//            Assert.IsNotNull(response);
//        }
//        /// <summary>
//        /// UpdateGeneralInformation
//        /// </summary>
//        /// <param name="groupId"></param>
//        /// <param name="requestBody"></param>
//        [TestCaseSource(nameof(InputDataForSaveGeneralInformation))]
//        public void UpdateGeneralInformation(int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _unitConfigurationController.UpdateGeneralInformation(groupId, "EVO_100",jObject);
//            Assert.IsNotNull(response);
//        }
//        /// <summary>
//        ///UpdateGeneralInformationError
//        /// </summary>
//        /// <param name="groupId"></param>
//        /// <param name="requestBody"></param>
//        [TestCaseSource(nameof(InputDataForSaveGeneralInformationError))]
//        public void UpdateGeneralInformationError(int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _unitConfigurationController.UpdateGeneralInformation(groupId,"EV0_100", jObject);
//            Assert.IsNotNull(response);
//        }
//        /// <summary>
//        /// UpdateGeneralInformationModelStateError
//        /// </summary>
//        /// <param name="groupId"></param>
//        /// <param name="requestBody"></param>
//        [TestCaseSource(nameof(InputDataForSaveGeneralInformationError))]
//        public void UpdateGeneralInformationModelStateError(int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.UpdateGeneralInformation(groupId,"" ,jObject);
//            Assert.IsNotNull(response);
//        }
//        /// <summary>
//        /// UpdateGeneralInformationExceptionError
//        /// </summary>
//        /// <param name="groupId"></param>
//        /// <param name="requestBody"></param>
//        [TestCaseSource(nameof(InputDataForSaveGeneralInformation))]
//        public void UpdateGeneralInformationExceptionError(int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _unitConfigurationController.UpdateGeneralInformation(groupId,"" ,jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveEntrance))]
//        public void SaveEntrance(int groupid, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.SaveEntrances(groupid, productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveEntrance))]
//        public void SaveEntranceError(int groupid, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.SaveEntrances(groupid, productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveEntrance))]
//        public void SaveEntranceModelStateError(int groupid, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.SaveEntrances(groupid, productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForUpdateEntrance))]
//        public void UpdateEntrance(int groupid, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.UpdateEntrances(groupid, productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForUpdateEntrance))]
//        public void UpdateEntranceError(int groupid, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.UpdateEntrances(groupid, productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForUpdateEntrance))]
//        public void UpdateEntranceModelStateError(int groupid, string productName, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<ConfigurationRequest>();
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.UpdateEntrances(groupid, productName, jObject);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// GetDetailsForTP2SummaryScreenController
//        /// </summary>
//        [TestCaseSource(nameof(InputGetTP2SummaryDetails))]
//        public void GetDetailsForTP2SummaryScreen(int setId)
//        {
//            var response = _unitConfigurationController.GetDetailsForTP2SummaryScreen(setId);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        /// <summary>
//        /// GetListOfBuildingsForProject
//        /// </summary>
//        [TestCaseSource(nameof(InputGetTP2SummaryDetailsError))]
//        public void GetDetailsForTP2SummaryScreenError(int setId)
//        {
//            var response = _unitConfigurationController.GetDetailsForTP2SummaryScreen(setId);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);

//        }

//        [TestCaseSource(nameof(InputGetTP2SummaryDetailsError))]
//        public void GetDetailsForTP2SummaryScreenModelStateError(int setId)
//        {
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.GetDetailsForTP2SummaryScreen(setId);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);

//        }
//        [Test]
//        public void AddEntranceConfiguration()
//        {
//            var response = _unitConfigurationController.StartEntranceConfigure(0, 1);
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void AddEntranceConfigurationError()
//        {
//            var response = _unitConfigurationController.StartEntranceConfigure(0, 0);
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void EditEntranceConfiguration()
//        {
//            var response = _unitConfigurationController.StartEntranceConfigure(1, 1);
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void EditEntranceConfigurationError()
//        {
//            var response = _unitConfigurationController.StartEntranceConfigure(1, 0);
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void EditEntranceConfigurationModelStateError()
//        {
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.StartEntranceConfigure(1, 0);
//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForChangeUnitConfigureSaveEntranceConfigure))]
//        public void ChangeEntranceConfigure(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var entrancedata = jObject.ToObject<EntranceConfigurationData>();
//            var response = _unitConfigurationController.ChangeEntranceConfigure(1, entrancedata);
//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForChangeUnitConfigureSaveEntranceConfigure))]
//        public void ChangeEntranceConfigureError(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var entrancedata = jObject.ToObject<EntranceConfigurationData>();
//            var response = _unitConfigurationController.ChangeEntranceConfigure(0, entrancedata);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForChangeUnitConfigureSaveEntranceConfigure))]
//        public void ChangeEntranceConfigureModelStateError(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var entrancedata = jObject.ToObject<EntranceConfigurationData>();
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.ChangeEntranceConfigure(0, entrancedata);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForChangeUnitConfigureSaveEntranceConfigure))]
//        public void SaveEntranceConfigure(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var entrancedata = jObject.ToObject<EntranceConfigurationData>();
//            var response = _unitConfigurationController.SaveEntranceConfiguration(1, entrancedata);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForChangeUnitConfigureSaveEntranceConfigure))]
//        public void SaveEntranceConfigureModelStateError(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var entrancedata = jObject.ToObject<EntranceConfigurationData>();
//            _unitConfigurationController.ModelState.AddModelError("model", "model");
//            var response = _unitConfigurationController.SaveEntranceConfiguration(1, entrancedata);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode,Constant.BADREQUEST);
//        }
//        [TestCaseSource(nameof(InputDataForChangeUnitConfigureSaveEntranceConfigure))]
//        public void SaveEntranceConfigureError(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var entrancedata = jObject.ToObject<EntranceConfigurationData>();
//            var response = _unitConfigurationController.SaveEntranceConfiguration(0, entrancedata);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForChangeUnitConfigureSaveEntranceConfigure))]
//        public void UpdateEntranceConfigure(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var entrancedata = jObject.ToObject<EntranceConfigurationData>();
//            var response = _unitConfigurationController.UpdateEntranceConfiguration(1, entrancedata);
//            Assert.IsNotNull(response);
//        }


//        [TestCaseSource(nameof(InputDataForChangeUnitConfigureSaveEntranceConfigure))]
//        public void UpdateEntranceConfigureError(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var entrancedata = jObject.ToObject<EntranceConfigurationData>();
//            var response = _unitConfigurationController.UpdateEntranceConfiguration(0, entrancedata);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForChangeUnitConfigureSaveEntranceConfigure))]
//        public void UpdateEntranceConfigureModelStateError(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var entrancedata = jObject.ToObject<EntranceConfigurationData>();
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.UpdateEntranceConfiguration(1, entrancedata);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
//        public void StartUnitConfigureUnitHallFixture(int groupConfigurationId, int setId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            jObject[Constant.SECTIONTABS] = Constant.UNITHALLFIXTURE;
//            var resultObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [TestCaseSource(nameof(InputDataForSaveUnitHallFixture))]
//        public void SaveUnitHallFixtureConfigure(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var unitHallFixtureData = jObject.ToObject<UnitHallFixtureData>();
//            var response = _unitConfigurationController.SaveUnitHallFixtureConfiguration(1, unitHallFixtureData);
//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForSaveUnitHallFixture))]
//        public void SaveUnitHallFixtureConfigureError(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var unitHallFixtureData = jObject.ToObject<UnitHallFixtureData>();
//            var response = _unitConfigurationController.SaveUnitHallFixtureConfiguration(0, unitHallFixtureData);
//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForSaveUnitHallFixture))]
//        public void UpdateUnitHallFixtureConfigure(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var unitHallFixtureData = jObject.ToObject<UnitHallFixtureData>();
//            var response = _unitConfigurationController.SaveUnitHallFixtureConfiguration(1, unitHallFixtureData);
//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForSaveUnitHallFixture))]
//        public void UpdateUnitHallFixtureConfigureMpdelStatError(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var unitHallFixtureData = jObject.ToObject<UnitHallFixtureData>();
//            _unitConfigurationController.ModelState.AddModelError("model", "model");
//            var response = _unitConfigurationController.SaveUnitHallFixtureConfiguration(1, unitHallFixtureData);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode,Constant.BADREQUEST);
//        }
//        [TestCaseSource(nameof(InputDataForSaveUnitHallFixture))]
//        public void UpdateUnitHallFixtureConfigureError(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var unitHallFixtureData = jObject.ToObject<UnitHallFixtureData>();
//            var response = _unitConfigurationController.SaveUnitHallFixtureConfiguration(0, unitHallFixtureData);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void StartUnitHallFixtureConsole()
//        {
//            List<UnitHallFixtures> lst = new List<UnitHallFixtures>();
//            UnitHallFixtures console = new UnitHallFixtures();
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONSOLECACHEDATA));
//            console = jObject.ToObject<UnitHallFixtures>();
//            lst.Add(console);
//            var cache =  _configure.SetCacheUnitHallFixtureConsoles(lst, "", 1);
//            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTUNITHALLFIXTURECONSOLEREQUESTBOSY));
//            var response = _unitConfigurationController.StartUnitHallFixtureConfigure(0, 1, jObject1);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void StartUnitHallFixtureModelStateErrorConsole()
//        {
//            List<UnitHallFixtures> lst = new List<UnitHallFixtures>();
//            UnitHallFixtures console = new UnitHallFixtures();
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONSOLECACHEDATA));
//            console = jObject.ToObject<UnitHallFixtures>();
//            lst.Add(console);
//            var cache = _configure.SetCacheUnitHallFixtureConsoles(lst, "", 1);
//            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTUNITHALLFIXTURECONSOLEREQUESTBOSY));
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.StartUnitHallFixtureConfigure(0, 1, jObject1);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void StartUnitHallFixtureConsoleError()
//        {
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTUNITHALLFIXTURECONSOLEREQUESTBOSY));
//            var response = _unitConfigurationController.StartUnitHallFixtureConfigure(0, 0, jObject);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void EditUnitHallFixtureConsole()
//        {
//            List<UnitHallFixtures> lst = new List<UnitHallFixtures>();
//            UnitHallFixtures console = new UnitHallFixtures();
//            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTUNITHALLFIXTURECONSOLEREQUESTBOSY));
//            var response = _unitConfigurationController.StartUnitHallFixtureConfigure(0, 1, jObject1);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void EditUnitHallFixtureConsoleError()
//        {
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTUNITHALLFIXTURECONSOLEREQUESTBOSY));
//            var response = _unitConfigurationController.StartUnitHallFixtureConfigure(0, 0, jObject);
//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForStartUnitConfigure))]
//        public void StartUnitConfigureUnitHallFixtureOther(int groupConfigurationId, int setId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            jObject[Constant.SECTIONTABS] = Constant.UNITHALLFIXTURE;
//            var resultObject = jObject.ToObject<ConfigurationRequest>();
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.StartUnitConfigure(111, 111, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForStartUnitConfigureError))]
//        public void StartUnitConfigureUnitHallFixtureError(int groupConfigurationId, int setId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            jObject[Constant.SECTIONTABS] = Constant.UNITHALLFIXTURE;
//            var resultObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _unitConfigurationController.StartUnitConfigure(groupConfigurationId, setId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void ChangeUnitConfigureUnitHallFixture()
//        {
//            List<UnitHallFixtures> lst = new List<UnitHallFixtures>();
//            UnitHallFixtures console1 = new UnitHallFixtures();
//            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONSOLECACHEDATA));
//            console1 = jObject1.ToObject<UnitHallFixtures>();
//            lst.Add(console1);
//            var cache = _configure.SetCacheUnitHallFixtureConsoles(lst, "", 1);

//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGECONSOLECACHEDDATA));
//            UnitHallFixtureData console = new UnitHallFixtureData();
//            console = jObject.ToObject<UnitHallFixtureData>();
//            var response = _unitConfigurationController.ChangeUnitHallFixtureConfigure(1, console);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void ChangeUnitConfigureUnitHallFixtureError()
//        {
//            List<UnitHallFixtures> lst = new List<UnitHallFixtures>();
//            UnitHallFixtures console1 = new UnitHallFixtures();
//            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONSOLECACHEDATA));
//            console1 = jObject1.ToObject<UnitHallFixtures>();
//            lst.Add(console1);
//            var cache = _configure.SetCacheUnitHallFixtureConsoles(lst, "", 1);

//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGECONSOLECACHEDDATA));
//            UnitHallFixtureData console = new UnitHallFixtureData();
//            console = jObject.ToObject<UnitHallFixtureData>();
//            var response = _unitConfigurationController.ChangeUnitHallFixtureConfigure(0, console);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void ChangeUnitConfigureUnitHallFixturModelStateError()
//        {
//            List<UnitHallFixtures> lst = new List<UnitHallFixtures>();
//            UnitHallFixtures console1 = new UnitHallFixtures();
//            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONSOLECACHEDATA));
//            console1 = jObject1.ToObject<UnitHallFixtures>();
//            lst.Add(console1);
//            var cache = _configure.SetCacheUnitHallFixtureConsoles(lst, "", 1);

//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGECONSOLECACHEDDATA));
//            UnitHallFixtureData console = new UnitHallFixtureData();
//            console = jObject.ToObject<UnitHallFixtureData>();
//            _unitConfigurationController.ModelState.AddModelError("Model", "Model");
//            var response = _unitConfigurationController.ChangeUnitHallFixtureConfigure(0, console);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void DeleteUnitHallFixture()
//        {
//            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTUNITHALLFIXTURECONSOLEREQUESTBOSY));
//            var response = _unitConfigurationController.DeleteUnitHallFixtureConfigure(1, 1, jObject1);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void DeleteUnitHallFixtureError()
//        {
//            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTUNITHALLFIXTURECONSOLEREQUESTBOSY));
//            var response = _unitConfigurationController.DeleteUnitHallFixtureConfigure(0, 1, jObject1);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void DeleteUnitHallFixtureModelStateError()
//        {
//            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTUNITHALLFIXTURECONSOLEREQUESTBOSY));
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.DeleteUnitHallFixtureConfigure(0, 1, jObject1);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void SaveCarCallCutoutKeyswitchOpenings()
//        {
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVECARCALLCUTOUTKEYSWITCHOPENINGS));
//            var CarCallCutoutKeyswitchOpenings = jObject.ToObject<CarcallCutoutData>();
//            var response = _unitConfigurationController.SaveCarCallCutoutKeyswitchOpenings(1, CarCallCutoutKeyswitchOpenings);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [Test]
//        public void SaveCarCallCutoutKeyswitchOpeningsError()
//        {
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVECARCALLCUTOUTKEYSWITCHOPENINGS));
//            var CarCallCutoutKeyswitchOpenings = jObject.ToObject<CarcallCutoutData>();
//            var response = _unitConfigurationController.SaveCarCallCutoutKeyswitchOpenings(0, CarCallCutoutKeyswitchOpenings);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void SaveCarCallCutoutKeyswitchOpeningsModelStateError()
//        {         
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVECARCALLCUTOUTKEYSWITCHOPENINGS));
//            var CarCallCutoutKeyswitchOpenings = jObject.ToObject<CarcallCutoutData>();
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.SaveCarCallCutoutKeyswitchOpenings(1, CarCallCutoutKeyswitchOpenings);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void StartCarCallCutoutAssignOpenings()
//        { 
//            var response = _unitConfigurationController.StartCarCallCutoutAssignOpenings(1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [Test]
//        public void StartCarCallCutoutAssignOpeningsError()
//        {
//            var response = _unitConfigurationController.StartCarCallCutoutAssignOpenings(0);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void StartCarCallCutoutAssignOpeningsModelStateError()
//        {
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.StartCarCallCutoutAssignOpenings(0);
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void GetLogHistory()
//        {

//            var loghistory = new LogHistoryRequestBody();
//            loghistory.Section = "unit";
//            loghistory.SetID = 3;
//            loghistory.UnitID = 1;
//            var response = _unitConfigurationController.GetLogHistory(loghistory);
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void GetLogHistoryModelStateError()
//        {

//            var loghistory = new LogHistoryRequestBody();
//            loghistory.Section = "unit";
//            loghistory.SetID = 3;
//            loghistory.UnitID = 1;
//            _unitConfigurationController.ModelState.AddModelError("model", "model");
//            var response = _unitConfigurationController.GetLogHistory(loghistory);
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void GetLogHistoryError()
//        {

//            var loghistory = new LogHistoryRequestBody();
//            loghistory.Section = "unit";
//            loghistory.SetID = 0;
//            loghistory.UnitID = 0;
//            var response = _unitConfigurationController.GetLogHistory(loghistory);
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void GetLogHistoryGroup()
//        {

//            var loghistory = new LogHistoryRequestBody();
//            loghistory.Section = "Group";
//            loghistory.GroupId = 76;
//            loghistory.SetID = 3;
//            var response = _unitConfigurationController.GetLogHistory(loghistory);
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void GetLogHistoryGroupError()
//        {

//            var loghistory = new LogHistoryRequestBody();
//            loghistory.Section = "Group";
//            loghistory.GroupId = 0;
//            var response = _unitConfigurationController.GetLogHistory(loghistory);
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void GetLogHistoryBuilding()
//        {

//            var loghistory = new LogHistoryRequestBody();
//            loghistory.Section = "Building";
//            loghistory.BuildingId = 76;
//            var response = _unitConfigurationController.GetLogHistory(loghistory);
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void GetLogHistoryBuildingError()
//        {

//            var loghistory = new LogHistoryRequestBody();
//            loghistory.Section = "Building";
//            loghistory.BuildingId = 0;
//            var response = _unitConfigurationController.GetLogHistory(loghistory);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void EditUnitDesignationTest()
//        {
//            UnitDesignation unitDesignation = new UnitDesignation();
//            unitDesignation.Description = "unitDescription";
//            unitDesignation.Designation = "unitDesignation";
//            var response = _unitConfigurationController.EditUnitDesignation(1, 1, unitDesignation);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [Test]
//        public void EditUnitDesignationErrorTest()
//        {
//            UnitDesignation unitDesignation = new UnitDesignation();
//            unitDesignation.Description = "unitDescription";
//            unitDesignation.Designation = "unitDesignation";
//            var response = _unitConfigurationController.EditUnitDesignation(0, 1, unitDesignation);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void EditUnitDesignationModelStateErrorTest()
//        {
//            UnitDesignation unitDesignation = new UnitDesignation();
//            unitDesignation.Description = "unitDescription";
//            unitDesignation.Designation = "unitDesignation";
//            _unitConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _unitConfigurationController.EditUnitDesignation(1, 1, unitDesignation);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void DeleteEntranceConfigurationTest()
//        {
//            var response = _unitConfigurationController.DeleteEntranceConfigure(1, 1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [Test]
//        public void DeleteEntranceConfigurationErrorTest()
//        {
//            var response = _unitConfigurationController.DeleteEntranceConfigure(0, 1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void DeleteEntranceConfigurationModelStateErrorTest()
//        {
//            _unitConfigurationController.ModelState.AddModelError("Test", "Test");
//            var response = _unitConfigurationController.DeleteEntranceConfigure(1, 1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }
//    }


//}
