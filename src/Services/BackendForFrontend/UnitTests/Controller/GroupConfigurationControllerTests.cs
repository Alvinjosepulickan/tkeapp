//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using TKE.SC.BFF.BusinessProcess.Helpers;
//using TKE.SC.BFF.BusinessProcess.Interfaces;
//using TKE.SC.BFF.Controllers;
//using TKE.SC.Common.Model.UIModel;
//using TKE.SC.Common.Model.ViewModel;
//using TKE.SC.BFF.Test.Common;

//namespace TKE.SC.BFF.Test.Controller
//{
//    public class GroupConfigurationControllerTests
//    {

//        #region 

//        private GroupConfigurationController _groupConfigurationController;
//        private ILogger<GroupConfigurationController> _groupConfigurationControllerLogger;
//        private IConfigure _configure;

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
//            _groupConfigurationControllerLogger = services.BuildServiceProvider().GetService<ILogger<GroupConfigurationController>>();

//            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();

//            var iGroup = (IGroupConfiguration)servicesProvider.GetService(typeof(IGroupConfiguration));
//            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));



//            _groupConfigurationController = new GroupConfigurationController(_groupConfigurationControllerLogger, iGroup, iConfigure);
//            _groupConfigurationController.ControllerContext = new ControllerContext();
//            _groupConfigurationController.ControllerContext.HttpContext = new DefaultHttpContext();
//            _groupConfigurationController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
//        }
//        #endregion

//        #region SetUp Input Values

//        public static IEnumerable<TestCaseData> InputDataForStartGroupConfiguration()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.STARTGROUPCONFIGREQUESTBODY, 2);
//        }

//        public static IEnumerable<TestCaseData> InputDataForChangeGroupConfiguration()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.CHANGEGROUPCONFIGREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForDeleteGroupConfiguration()
//        {
//            yield return new TestCaseData(272);
//        }


//        public static IEnumerable<TestCaseData> InputDataForDeleteGroupConfigurationError()
//        {
//            yield return new TestCaseData(0);
//        }

//        public static IEnumerable<TestCaseData> InputDataForUpdateOpeningLocation()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEOPENINGLOCATIONREQUESTBODY, 236);
//        }

//        public static IEnumerable<TestCaseData> InputGroupValues()
//        {
//            yield return new TestCaseData("15");
//        }
//        public static IEnumerable<TestCaseData> InputDataForGetGroupConfiguration()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.GETGROUPCONFIGREQUESTBODY, 55);
//        }

//        public static IEnumerable<TestCaseData> InputDataForGetGroupConfigurationInput()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.GETGROUPCONFIGREQUESTBODY, 99);
//        }
//        public static IEnumerable<TestCaseData> InputDataForGetGroupConfigurationError()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.GETGROUPCONFIGREQUESTBODY, 0);
//        }
//        public static IEnumerable<TestCaseData> InputDataForSaveGroupConfiguration()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEGROUPREQUESTBODY, 1);
//        }

//        public static IEnumerable<TestCaseData> InputDataForSaveGroupConfigurationError()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEGROUPREQUESTBODY, 0);
//        }

//        public static IEnumerable<TestCaseData> InputDataForUpdateGroupConfiguration()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEGROUPREQUESTBODY, 1, 54);
//        }

//        public static IEnumerable<TestCaseData> InputDataForUpdateGroupConfigurationError()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEGROUPREQUESTBODY, 0, 0);
//        }

//        public static IEnumerable<TestCaseData> InputDataForStartGroupConfigurationError()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.STARTGROUPCONFIGREQUESTBODY, 0);
//        }

//        public static IEnumerable<TestCaseData> InputDataForStartGroupConfigurationErrorDL()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.STARTGROUPCONFIGREQUESTBODY, 0);
//        }

//        public static IEnumerable<TestCaseData> InputDataForStartGroupConsole()
//        {
//            yield return new TestCaseData(1, 1, AppGatewayJsonFilePath.STARTGROUPCONFIGREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForStartGroupConsoleError()
//        {
//            yield return new TestCaseData(0, 0, AppGatewayJsonFilePath.STARTGROUPCONFIGREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForDeleteGroupHallFixtures()
//        {
//            yield return new TestCaseData(1, 1, AppGatewayJsonFilePath.DELETEGROUPHALLFIXTUREREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForDeleteGroupHallFixtureConsoleError()
//        {
//            yield return new TestCaseData(0, 0, AppGatewayJsonFilePath.DELETEGROUPHALLFIXTUREREQUESTBODY);
//        }

//        #endregion

//        /// <summary>
//        /// This is the initial method which will return current configuration object with enriched data and prices
//        /// </summary>
//        /// <param name="configureRequest"></param>
//        /// <param name="buildingId"></param>
//        [TestCaseSource(nameof(InputDataForStartGroupConfiguration))]
//        public void StartGroupConfigureController(string requestbody, int buildingId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestbody));
//            //var requestObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _groupConfigurationController.StartGroupConfigure("", "", buildingId, 0,"groupconfiguration", jObject);
//            //Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForStartGroupConfiguration))]
//        public void StartGroupConfigureControllerError(string requestbody, int buildingId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestbody));
//            var response = _groupConfigurationController.StartGroupConfigure("", "", -1, 0, "groupconfiguration", jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode,400);
//        }
//        ///// <summary>
//        ///// Start Group ConfigurationError
//        ///// </summary>
//        ///// <param name="groupId"></param>
//        ///// <param name="buildingId"></param>
//        //[TestCaseSource(nameof(InputDataForStartGroupConfigurationError))]
//        //public void StartGroupConfigureControllerError(string requestbody, int buildingId)
//        //{
//        //    var jObject = JObject.Parse(File.ReadAllText(requestbody));
//        //    var requestObject = jObject.ToObject<ConfigurationRequest>();
//        //    requestObject.PackagePath = null;
//        //    var response = _groupConfigurationController.StartGroupConfigure("", "", 0, "groupConfiguration", jObject);
//        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        //}

//        [TestCaseSource(nameof(InputDataForStartGroupConfiguration))]
//        public void StartGroupConfigureControllerModelStateError(string requestbody, int buildingId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestbody));
//            var requestObject = jObject.ToObject<ConfigurationRequest>();
//            requestObject.PackagePath = null;
//            _groupConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _groupConfigurationController.StartGroupConfigure("", "", 1, 0,"groupconfiguration", jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, 400);
//        }

//        /// </summary>
//        /// <param name="configureRequest"></param>
//        [TestCaseSource(nameof(InputDataForChangeGroupConfiguration))]
//        public void ChangeGroupConfigureController(string requestbody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestbody));
//            var requestObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _groupConfigurationController.ChangeGroupConfigure(jObject, 0,"groupConfiguration");
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// Delete Group Configuration
//        /// </summary>
//        /// <param name="groupId"></param>
//        [TestCaseSource(nameof(InputDataForDeleteGroupConfiguration))]
//        public void DeleteGroupConfigureController(int GroupId)
//        {
//            var response = _groupConfigurationController.DeleteGroupConfiguration(GroupId);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);


//        }

//        [TestCaseSource(nameof(InputDataForDeleteGroupConfiguration))]
//        public void DeleteGroupConfigureControllerModelStateError(int GroupId)
//        {
//            _groupConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _groupConfigurationController.DeleteGroupConfiguration(GroupId);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);


//        }
//        /// <summary>
//        /// Delete Group Configuration
//        /// </summary>
//        /// <param name="groupId"></param>
//        [TestCaseSource(nameof(InputDataForDeleteGroupConfigurationError))]
//        public void DeleteGroupConfigureControllerInput(int GroupId)
//        {
//            var response = _groupConfigurationController.DeleteGroupConfiguration(GroupId);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// Delete Group Configuration
//        /// </summary>
//        /// <param name="groupId"></param>
//        [TestCaseSource(nameof(InputDataForDeleteGroupConfigurationError))]

//        public void DeleteGroupConfigureControllerErrorInput(int GroupId)
//        {
//            var response = _groupConfigurationController.DeleteGroupConfiguration(GroupId);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.NOTFOUND);

//        }

//        /// <summary>
//        /// Delete Group ConfigurationError
//        /// </summary>
//        /// <param name="groupId"></param>
//        [Test]
//        public void DeleteGroupConfigureError()
//        {
//            var response = _groupConfigurationController.DeleteGroupConfiguration(-1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        /// <summary>
//        /// GetGroupConfigurationByBuildingIdController
//        /// </summary>
//        [TestCaseSource(nameof(InputGroupValues))]
//        public void GetGroupConfigurationByBuildingIdController(string buildingId)
//        {
//            var response = _groupConfigurationController.GetGroupConfigurationByBuildingId(buildingId);
//            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
//        }

//        /// <summary>
//        /// GetGroupConfigurationByBuildingIdController
//        /// </summary>
//        [TestCaseSource(nameof(InputGroupValues))]
//        public void GetGroupConfigurationByBuildingIdInput(string buildingId)
//        {
//            var response = _groupConfigurationController.GetGroupConfigurationByBuildingId(buildingId);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// GetListOfBuildingsForProject
//        /// </summary>
//        [Test]
//        public void GetGroupConfigurationByBuildingId()
//        {
//            var response = _groupConfigurationController.GetGroupConfigurationByBuildingId("NO");
//            Assert.AreEqual(response.Result.StatusCode, Constant.NOTFOUND);

//            //Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// GetListOfBuildingsForProjectError
//        /// </summary>
//        [Test]
//        public void GetGroupConfigurationByBuildingIdError()
//        {
//            var response = _groupConfigurationController.GetGroupConfigurationByBuildingId("TEST");
//            Assert.AreEqual(response.Result.StatusCode, Constant.BADREQUEST);

//            //Assert.IsNotNull(response);
//        }

//        [Test]
//        public void GetGroupConfigurationByBuildingIdModelStateError()
//        {
//            _groupConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _groupConfigurationController.GetGroupConfigurationByBuildingId("TEST");
//            Assert.AreEqual(response.Result.StatusCode, Constant.BADREQUEST);
//        }

//        /// <summary>
//        /// GetGroupConfigurationDetailsByGroupIdController
//        /// </summary>
//        /// <param name="groupConfigurationId"></param>
//        [TestCaseSource(nameof(InputDataForGetGroupConfiguration))]
//        public void GetGroupConfigurationDetailsByGroupId(string requestBody, int groupConfigurationId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            //var requestObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _groupConfigurationController.GetGroupConfigurationDetailsByGroupId(groupConfigurationId, "selectedTab", jObject);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// GetGroupConfigurationDetailsInputByGroupIdController
//        /// </summary>
//        /// <param name="groupConfigurationId"></param>
//        [TestCaseSource(nameof(InputDataForGetGroupConfigurationInput))]
//        public void GetGroupConfigurationDetailsInputByGroupId(string requestBody, int groupConfigurationId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var requestObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _groupConfigurationController.GetGroupConfigurationDetailsByGroupId(groupConfigurationId, "groupConfiguration", jObject);
//            Assert.IsNotNull(response);
//        }


//        /// <summary>
//        /// GetGroupConfigurationDetailsByGroupIdErrorController
//        /// </summary>
//        [TestCaseSource(nameof(InputDataForGetGroupConfigurationError))]
//        public void GetGroupConfigurationDetailsByGroupIdError(string requestBody, int groupConfigurationId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            //var requestObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _groupConfigurationController.GetGroupConfigurationDetailsByGroupId(groupConfigurationId, "selectedTab", jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForGetGroupConfigurationError))]
//        public void GetGroupConfigurationDetailsByGroupIdModelStateError(string requestBody, int groupConfigurationId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            //var requestObject = jObject.ToObject<ConfigurationRequest>();
//            _groupConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _groupConfigurationController.GetGroupConfigurationDetailsByGroupId(groupConfigurationId, "selectedTab", jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }
//        /// <summary>
//        /// SaveGroupConfigurationDetailsController
//        /// </summary>
//        /// <param name="buildingId"></param>
//        /// <param name="userName"></param>
//        [TestCaseSource(nameof(InputDataForSaveGroupConfiguration))]
//        public void SaveGroupConfigurationDetailsController(string requestBody, int buildingId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var requestObject = jObject.ToObject<GroupConfigurationSave>();
//            var response = _groupConfigurationController.SaveGroupConfigurationDetails(buildingId, requestObject);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// SaveGroupConfigurationDetailsController
//        /// </summary>
//        /// <param name="buildingId"></param>
//        /// <param name="userName"></param>
//        [TestCaseSource(nameof(InputDataForSaveGroupConfiguration))]
//        public void SaveGroupConfigurationDetailsControllerInput(string requestBody, int buildingId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var requestObject = jObject.ToObject<GroupConfigurationSave>();
//            var response = _groupConfigurationController.SaveGroupConfigurationDetails(buildingId, requestObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);

//        }

//        /// <summary>
//        /// SaveGroupConfigurationDetailsControllerError
//        /// </summary>
//        /// <param name="buildingId"></param>
//        /// <param name="userName"></param>
//        [TestCaseSource(nameof(InputDataForSaveGroupConfigurationError))]
//        public void SaveGroupConfigurationDetailsControllerError(string requestBody, int buildingId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var requestObject = jObject.ToObject<GroupConfigurationSave>();
//            var response = _groupConfigurationController.SaveGroupConfigurationDetails(buildingId, requestObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForSaveGroupConfigurationError))]
//        public void SaveGroupConfigurationDetailsControllerModelStateError(string requestBody, int buildingId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            _groupConfigurationController.ModelState.AddModelError("test", "test");
//            var requestObject = jObject.ToObject<GroupConfigurationSave>();
//            var response = _groupConfigurationController.SaveGroupConfigurationDetails(buildingId, requestObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        /// <summary>
//        /// UpdateGroupConfigurationDetailsController
//        /// </summary>
//        /// <param name="buildingId"></param>
//        /// <param name="groupConfigurationId"></param>
//        [TestCaseSource(nameof(InputDataForUpdateGroupConfiguration))]
//        public void UpdateGroupConfigurationDetailsController(string requestBody, int buildingId, int groupConfigurationId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            //var requestObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _groupConfigurationController.UpdateGroupConfigurationDetails(buildingId, groupConfigurationId, jObject);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// UpdateGroupConfigurationDetailsControllerError
//        /// </summary>
//        /// <param name="buildingId"></param>
//        /// <param name="groupConfigurationId"></param>
//        [TestCaseSource(nameof(InputDataForUpdateGroupConfigurationError))]
//        public void UpdateGroupConfigurationDetailsControllerError(string requestBody, int buildingId, int groupConfigurationId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            //var requestObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _groupConfigurationController.UpdateGroupConfigurationDetails(buildingId, groupConfigurationId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForUpdateGroupConfigurationError))]
//        public void UpdateGroupConfigurationDetailsControllerModelStateError(string requestBody, int buildingId, int groupConfigurationId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            //var requestObject = jObject.ToObject<ConfigurationRequest>();
//            _groupConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _groupConfigurationController.UpdateGroupConfigurationDetails(buildingId, groupConfigurationId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForStartGroupConsole))]
//        public void StartGroupConsoleGroupHallFixture(int groupConfigurationId, int consoleId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTGROUPCONSOLEREQUESTBODY));
//           // jObject[Constant.SECTIONTABS] = Constant.GROUPHALLFIXTURE;
//            var response = _groupConfigurationController.StartGroupConsole(groupConfigurationId, consoleId, jObject);
//            Assert.IsNotNull(response);
//        }

//        //[TestCaseSource(nameof(InputDataForStartGroupConsole))]
//        [Test]
//        public void StartGroupConsole()
//        {
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTGROUPCONSOLEREQUESTBODY));
//            jObject[Constant.SECTIONTABS] = Constant.GROUPHALLFIXTURE;
//            var resultObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _groupConfigurationController.StartGroupConsole(1, 356, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForStartGroupConsoleError))]
//        public void StartGroupConsoleError(int consoleId, int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTGROUPCONSOLEREQUESTBODY));
//            //jObject[Constant.SECTIONTABS] = Constant.GROUPHALLFIXTURE;
//            var response = _groupConfigurationController.StartGroupConsole(consoleId, groupId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForStartGroupConsoleError))]
//        public void StartGroupConsoleModelStateError(int consoleId, int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTGROUPCONSOLEREQUESTBODY));
//            //jObject[Constant.SECTIONTABS] = Constant.GROUPHALLFIXTURE;
//            _groupConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _groupConfigurationController.StartGroupConsole(consoleId, groupId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void ChangeGroupHallFixtureConfigure()
//        {
//            List<GroupHallFixtures> lst = new List<GroupHallFixtures>();
//            GroupHallFixtures console1 = new GroupHallFixtures();
//            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONSOLECACHEDATA));
//            console1 = jObject1.ToObject<GroupHallFixtures>();
//            lst.Add(console1);
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGECONSOLECACHEDDATA));
//            GroupHallFixturesData console = new GroupHallFixturesData();
//            console = jObject.ToObject<GroupHallFixturesData>();
//            var response = _groupConfigurationController.ChangeGroupHallFixtureConfigure(1, console);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void ChangeGroupHallFixtureConfigureError()
//        {
//            List<GroupHallFixtures> lst = new List<GroupHallFixtures>();
//            GroupHallFixtures console1 = new GroupHallFixtures();
//            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONSOLECACHEDATA));
//            console1 = jObject1.ToObject<GroupHallFixtures>();
//            lst.Add(console1);
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGECONSOLECACHEDDATA));
//            GroupHallFixturesData console = new GroupHallFixturesData();
//            console = jObject.ToObject<GroupHallFixturesData>();
//            var response = _groupConfigurationController.ChangeGroupHallFixtureConfigure(0, console);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void ChangeGroupHallFixtureConfigureModelStateError()
//        {
//            List<GroupHallFixtures> lst = new List<GroupHallFixtures>();
//            GroupHallFixtures console1 = new GroupHallFixtures();
//            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONSOLECACHEDATA));
//            console1 = jObject1.ToObject<GroupHallFixtures>();
//            lst.Add(console1);
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGECONSOLECACHEDDATA));
//            GroupHallFixturesData console = new GroupHallFixturesData();
//            console = jObject.ToObject<GroupHallFixturesData>();
//            _groupConfigurationController.ModelState.AddModelError("model", "model");
//            var response = _groupConfigurationController.ChangeGroupHallFixtureConfigure(0, console);
//            Assert.IsNotNull(response);
//        }
//        /// <summary>
//        /// Delete GroupHallFixture
//        /// </summary>
//        /// <param name="groupId"></param>
//        [TestCaseSource(nameof(InputDataForDeleteGroupHallFixtures))]
//        public void DeleteGroupHallFixtureConfigure(int groupId, int consoleId, string requestbody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestbody));
//            jObject[Constant.SECTIONTABS] = Constant.GROUPHALLFIXTURE;
//            var response = _groupConfigurationController.DeleteGroupHallFixtureConfigure(groupId, consoleId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);


//        }

//        [TestCaseSource(nameof(InputDataForDeleteGroupHallFixtures))]
//        public void DeleteGroupHallFixtureConfigureModelStateError(int groupId, int consoleId, string requestbody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestbody));
//            jObject[Constant.SECTIONTABS] = Constant.GROUPHALLFIXTURE;
//            _groupConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _groupConfigurationController.DeleteGroupHallFixtureConfigure(groupId, consoleId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);


//        }

//        /// <summary>
//        /// Delete GroupHallFixture
//        /// </summary>
//        /// <param name="groupId"></param>
//        [TestCaseSource(nameof(InputDataForDeleteGroupHallFixtureConsoleError))]
//        public void DeleteGroupHallFixtureConfigureError(int groupId, int consoleId, string requestbody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestbody));
//            jObject[Constant.SECTIONTABS] = Constant.GROUPHALLFIXTURE;
//            var response = _groupConfigurationController.DeleteGroupHallFixtureConfigure(groupId, consoleId, jObject);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// Delete GroupHallFixture
//        /// </summary>
//        /// <param name="groupId"></param>
//        [TestCaseSource(nameof(InputDataForDeleteGroupHallFixtureConsoleError))]

//        public void DeleteGroupHallFixtureConfigureErrorInput(int groupId, int consoleId, string requestbody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestbody));
//            jObject[Constant.SECTIONTABS] = Constant.GROUPHALLFIXTURE;
//            var response = _groupConfigurationController.DeleteGroupHallFixtureConfigure(groupId, consoleId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);

//        }

//        /// <summary>
//        /// Delete Group Hall Fixture Console
//        /// </summary>
//        /// <param name="groupId"></param>
//        [Test]
//        public void DeleteGroupHallFixtureConsoleError()
//        {
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.DELETEGROUPHALLFIXTUREREQUESTBODY));
//            jObject[Constant.SECTIONTABS] = Constant.GROUPHALLFIXTURE;
//            var response = _groupConfigurationController.DeleteGroupHallFixtureConfigure(-1,1, jObject);
//            Assert.IsNotNull(response);
//        }


//        [Test]
//        public void GetStartGroupHallFixtureErrorTest()
//        {
//            var response = _groupConfigurationController.GetStartGroupHallFixture(0);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void GetStartGroupHallFixtureModelStateErrorTest()
//        {
//            _groupConfigurationController.ModelState.AddModelError("model", "model");
//            var response = _groupConfigurationController.GetStartGroupHallFixture(1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }


//        [Test]
//        public void SaveGroupHallFixtureErrorTest()
//        {
//            var jObject = JsonConvert.DeserializeObject<GroupHallFixturesData>(JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEGROUPHALLFIXTURESREQUESTBODY)).ToString());
//            var response = _groupConfigurationController.SaveGroupHallFixture(1, 0, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void SaveGroupHallFixtureModelStateErrorTest()
//        {
//            _groupConfigurationController.ModelState.AddModelError("model", "model");
//            var jObject = JsonConvert.DeserializeObject<GroupHallFixturesData>(JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEGROUPHALLFIXTURESREQUESTBODY)).ToString());
//            var response = _groupConfigurationController.SaveGroupHallFixture(1, 0, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }
//    }
//}
