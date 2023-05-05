using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.Controllers;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.BFF.Test.Common;
using TKE.SC.BFF.APIController;
using System.Security.Principal;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.BFF.Test.BusinessProcess.Helper;

namespace TKE.SC.BFF.UnitTests.Controller.V1
{
    public class GroupsControllerTests
    {
        #region 

        private GroupsController _groupController;
        private ILogger<GroupsController> _groupControllerLogger;
        private IConfigure _configure;

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
            _groupControllerLogger = services.BuildServiceProvider().GetService<ILogger<GroupsController>>();

            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iGroupLayout = (IGroupLayout)servicesProvider.GetService(typeof(IGroupLayout));
            var iGroupConfiguration = (IGroupConfiguration)servicesProvider.GetService(typeof(IGroupConfiguration));
            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
            var iOpeningLocation = (IOpeningLocation)servicesProvider.GetService(typeof(IOpeningLocation));
            var identity = new GenericIdentity("testuser", "jwt");
            identity.AddClaim(new System.Security.Claims.Claim("SessionId", "SessionId"));
            var principal = new GenericPrincipal(identity, new string[] { "SessionId" });
            var iFieldDrawingautomation = (IFieldDrawingAutomation)servicesProvider.GetService(typeof(IFieldDrawingAutomation));


            _groupController = new GroupsController(iGroupLayout, iFieldDrawingautomation, iGroupConfiguration, iConfigure, iOpeningLocation, _groupControllerLogger);
            _groupController.ControllerContext = new ControllerContext();
            _groupController.ControllerContext.HttpContext = new DefaultHttpContext();
            _groupController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
            _groupController.HttpContext.User = principal;

        }

        #endregion

        #region SetUp Input Values

        public static IEnumerable<TestCaseData> InputDataForStartGroupConfiguration()
        {
            yield return new TestCaseData(2,2,AppGatewayJsonFilePath.STARTGROUPCONFIGREQUESTBODY);
        }
        public static IEnumerable<TestCaseData> InputDataForChangeFieldDrawingAutomation()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.FIELDDRAWINGAUTOMATIONSTUB);
        }
        public static IEnumerable<TestCaseData> InputDataForChangeGroupConfiguration()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.CHANGEGROUPCONFIGREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForDeleteGroupConfiguration()
        {
            yield return new TestCaseData(272);
        }


        public static IEnumerable<TestCaseData> InputDataForDeleteGroupConfigurationError()
        {
            yield return new TestCaseData(0);
        }

        public static IEnumerable<TestCaseData> InputDataForUpdateOpeningLocation()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEOPENINGLOCATIONREQUESTBODY, 236);
        }

        public static IEnumerable<TestCaseData> InputGroupValues()
        {
            yield return new TestCaseData("B");
        }
        public static IEnumerable<TestCaseData> InputDataForGetGroupConfiguration()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.GETGROUPCONFIGREQUESTBODY, 55);
        }

        public static IEnumerable<TestCaseData> InputDataForGetGroupConfigurationInput()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.GETGROUPCONFIGREQUESTBODY, 99);
        }
        public static IEnumerable<TestCaseData> InputDataForGetGroupConfigurationError()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.GETGROUPCONFIGREQUESTBODY, 0);
        }
        public static IEnumerable<TestCaseData> InputDataForSaveGroupConfiguration()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEGROUPREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForSaveGroupConfigurationError()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEGROUPREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForUpdateGroupConfiguration()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEGROUPREQUESTBODY, 1, 54);
        }

        public static IEnumerable<TestCaseData> InputDataForUpdateGroupConfigurationError()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEGROUPREQUESTBODY, 0, 0);
        }

        public static IEnumerable<TestCaseData> InputDataForStartGroupConfigurationError()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.STARTGROUPCONFIGREQUESTBODY, 0);
        }

        public static IEnumerable<TestCaseData> InputDataForStartGroupConfigurationErrorDL()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.STARTGROUPCONFIGREQUESTBODY, 0);
        }

        public static IEnumerable<TestCaseData> InputDataForStartGroupConsole()
        {
            yield return new TestCaseData();
        }

        public static IEnumerable<TestCaseData> InputDataForStartGroupConsoleError()
        {
            yield return new TestCaseData(0, 0, 0, "");
        }

        public static IEnumerable<TestCaseData> InputDataForDeleteGroupHallFixtures()
        {
            yield return new TestCaseData(1, 1, "Traditional_Hall_Stations");
        }

        public static IEnumerable<TestCaseData> InputDataForDeleteGroupHallFixtureConsoleError()
        {
            yield return new TestCaseData(0, 0, "Traditional_Hall_Stations");

        }

        public static IEnumerable<TestCaseData> InputDataForSaveGroupLayoutDetails()
        {
            yield return new TestCaseData(1, AppGatewayJsonFilePath.SAVEGROUPLAYOUTFLOORPLANREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForUpdateGroupLayoutDetails()
        {
            yield return new TestCaseData(1, AppGatewayJsonFilePath.SAVEGROUPLAYOUTFLOORPLANERRORREQUESTBODY);
        }

        #endregion

        /// <summary>
        /// This is the initial method which will return current configuration object with enriched data and prices
        /// </summary>
        /// <param name="configureRequest"></param>
        /// <param name="buildingId"></param>
        [TestCaseSource(nameof(InputDataForStartGroupConfiguration))]
        public void StartGroupConfigureController(int buildingId, int groupId, string selectedTab)
        {
            var response = _groupController.StartGroupConfigure(1, 2, "GROUPCONFIGURATION");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, 200);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string keyValue = check.Root["sections"].ToString().Trim();
            var value = JArray.Parse(keyValue);
            string key = value.Root[0]["id"].ToString().Trim();
            Assert.AreEqual(key, "Group");
        }

         [TestCaseSource(nameof(InputDataForStartGroupConfiguration))]
        public void StartGroupConfigureControllerError(int buildingId, int groupId, string selectedTab)
        {
            var response = _groupController.StartGroupConfigure(-1, -2, "GROUPCONFIGURATION");
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        /// <summary>
        /// Start Group ConfigurationError
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="buildingId"></param>
        [TestCaseSource(nameof(InputDataForStartGroupConfigurationError))]
        public void StartGroupConfigureControllerError(string requestbody, int buildingId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestbody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            requestObject.PackagePath = null;
            var response = _groupController.StartGroupConfigure(-1, -1, "GROUPCONFIGURATION");
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [TestCaseSource(nameof(InputDataForChangeGroupConfiguration))]
        public void ChangeGroupConfigureController(string requestbody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestbody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _groupController.ChangeGroupConfigure(jObject, 0, "groupConfiguration");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, 200);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string keyValue = check.Root["sections"].ToString().Trim();
            var value = JArray.Parse(keyValue);
            string key = value.Root[0]["id"].ToString().Trim();
            Assert.AreEqual(key, "Group");
        }


        [TestCaseSource(nameof(InputDataForDeleteGroupConfiguration))]
        public void DeleteGroupConfigureController(int GroupId)
        {
            var response = _groupController.DeleteGroupConfiguration(GroupId);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["Message"].ToString().Trim();
            Assert.AreEqual(key, "Group Deleted Successfully");
        }


        [TestCaseSource(nameof(InputDataForDeleteGroupConfigurationError))] //To be implemented
        public void DeleteGroupConfigureControllerInput(int GroupId)
        {
            var response = _groupController.DeleteGroupConfiguration(1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["Message"].ToString().Trim();
            Assert.AreEqual(key, "Group Deleted Successfully");
        }

        [TestCaseSource(nameof(InputDataForDeleteGroupConfigurationError))]
        //To be implemented
        public void DeleteGroupConfigureControllerErrorInput(int GroupId)
        {
            var response = _groupController.DeleteGroupConfiguration(GroupId);
            Assert.ThrowsAsync<CustomException>(() => response);

        }

        [Test] //To be implemented
        public void DeleteGroupConfigureError()
        {
            var response = _groupController.DeleteGroupConfiguration(-1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(InputGroupValues))] //To be implemented
        public void GetGroupConfigurationByBuildingIdController(string buildingId)
        {
            var response = _groupController.GetGroupConfigurationByBuildingId(buildingId);
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        [TestCaseSource(nameof(InputGroupValues))] //To be implemented
        public void GetGroupConfigurationByBuildingIdInput(string buildingId)
        {
            var response = _groupController.GetGroupConfigurationByBuildingId(buildingId);
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }


        [Test] //To be implemented
        public void GetGroupConfigurationByBuildingId()
        {
            var response = _groupController.GetGroupConfigurationByBuildingId("NO");
            Assert.AreEqual(response.Result.StatusCode, Constant.NOTFOUND);

            //Assert.IsNotNull(response);
        }

        [Test] //To be implemented
        public void GetGroupConfigurationByBuildingIdError()
        {
            var response = _groupController.GetGroupConfigurationByBuildingId("TEST");
            Assert.AreEqual(response.Result.StatusCode, Constant.BADREQUEST);

        }


        [TestCaseSource(nameof(InputDataForGetGroupConfiguration))] //To be implemented
        public void GetGroupConfigurationDetailsByGroupId(string requestBody, int groupConfigurationId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _groupController.GetGroupConfigurationDetailsByGroupId(groupConfigurationId, "selectedTab", jObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root.ToString().Trim();
            Assert.AreEqual(key, "{}");
        }


        [TestCaseSource(nameof(InputDataForGetGroupConfigurationInput))]
        public void GetGroupConfigurationDetailsInputByGroupId(string requestBody, int groupConfigurationId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _groupController.GetGroupConfigurationDetailsByGroupId(groupConfigurationId, "groupConfiguration", jObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root.ToString().Trim();
            Assert.AreEqual(key, "{}");
        }


        [TestCaseSource(nameof(InputDataForGetGroupConfigurationError))] //To be implemented
        public void GetGroupConfigurationDetailsByGroupIdError(string requestBody, int groupConfigurationId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _groupController.GetGroupConfigurationDetailsByGroupId(groupConfigurationId, "selectedTab", jObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [TestCaseSource(nameof(InputDataForSaveGroupConfiguration))] //To be implemented
        public void SaveGroupConfigurationDetailsController(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<GroupConfigurationSave>();
            requestObject.buildingID = 1;
            var response = _groupController.SaveGroupConfigurationDetails(requestObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["Result"].ToString().Trim();
            Assert.AreEqual(key, "1");
        }

        [TestCaseSource(nameof(InputDataForSaveGroupConfiguration))] //To be implemented
        public void SaveGroupConfigurationDetailsControllerInput(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<GroupConfigurationSave>();
            requestObject.buildingID = 1;
            var response = _groupController.SaveGroupConfigurationDetails(requestObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["Result"].ToString().Trim();
            Assert.AreEqual(key, "1");
        }


        [TestCaseSource(nameof(InputDataForSaveGroupConfigurationError))] //To be implemented
        public void SaveGroupConfigurationDetailsControllerError(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<GroupConfigurationSave>();
            requestObject.buildingID = 0;
            var response = _groupController.SaveGroupConfigurationDetails(requestObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [TestCaseSource(nameof(InputDataForUpdateGroupConfiguration))]
        public void UpdateGroupConfigurationDetailsController(string requestBody, int buildingId, int groupConfigurationId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _groupController.UpdateGroupConfigurationDetails(buildingId, groupConfigurationId, jObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["Message"].ToString().Trim();
            Assert.AreEqual(key, "Group Updated Successfully");
        }


        [TestCaseSource(nameof(InputDataForUpdateGroupConfigurationError))]
        public void UpdateGroupConfigurationDetailsControllerError(string requestBody, int buildingId, int groupConfigurationId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _groupController.UpdateGroupConfigurationDetails(buildingId, groupConfigurationId, jObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [TestCaseSource(nameof(InputDataForStartGroupConsoleError))]
        public void StartGroupConsoleGroupHallFixture(int consoleId, int sectionId, int groupId, string fixtureSelected)
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTGROUPCONSOLEREQUESTBODY));
            // jObject[Constant.SECTIONTABS] = Constant.GROUPHALLFIXTURE;
            var response = _groupController.StartGroupConsole(consoleId, sectionId, 1, fixtureSelected);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["id"].ToString().Trim();
            Assert.AreEqual(key, "1");
        }

        [TestCaseSource(nameof(InputDataForStartGroupConsoleError))]
        [Test] //to be implemented
        public void StartGroupConsole(int consoleId, int sectionId, int groupId, string fixtureSelected)
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTGROUPCONSOLEREQUESTBODY));
            jObject[Constant.SECTIONTABS] = Constant.GROUPHALLFIXTURE;
            var resultObject = jObject.ToObject<ConfigurationRequest>();
            var response = _groupController.StartGroupConsole(consoleId, sectionId, 1, fixtureSelected);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["id"].ToString().Trim();
            Assert.AreEqual(key, "1");
        }


        [TestCaseSource(nameof(InputDataForStartGroupConsoleError))]
        public void StartGroupConsole2(int consoleId, int sectionId, int groupId, string fixtureSelected)
        {
            var response = _groupController.StartGroupConsole(consoleId, sectionId, 1, fixtureSelected);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["id"].ToString().Trim();
            Assert.AreEqual(key, "1");
        }


        [Test] //To be implemented
        public void ChangeGroupHallFixtureConfigure()
        {
            List<GroupHallFixtures> lst = new List<GroupHallFixtures>();
            GroupHallFixtures console1 = new GroupHallFixtures();
            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONSOLECACHEDATA));
            console1 = jObject1.ToObject<GroupHallFixtures>();
            lst.Add(console1);
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGECONSOLECACHEDDATA));
            GroupHallFixturesData console = new GroupHallFixturesData();
            console = jObject.ToObject<GroupHallFixturesData>();
            var response = _groupController.ChangeGroupHallFixtureConfigure(1, console);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["id"].ToString().Trim();
            Assert.AreEqual(key, "1");
        }

        [Test] //To be implemented
        public void ChangeGroupHallFixtureConfigureError()
        {
            List<GroupHallFixtures> lst = new List<GroupHallFixtures>();
            GroupHallFixtures console1 = new GroupHallFixtures();
            var jObject1 = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONSOLECACHEDATA));
            console1 = jObject1.ToObject<GroupHallFixtures>();
            lst.Add(console1);
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGECONSOLECACHEDDATA));
            GroupHallFixturesData console = new GroupHallFixturesData();
            console = jObject.ToObject<GroupHallFixturesData>();
            var response = _groupController.ChangeGroupHallFixtureConfigure(1, console);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["id"].ToString().Trim();
            Assert.AreEqual(key, "1");
        }





        [TestCaseSource(nameof(InputDataForDeleteGroupHallFixtures))] //To be implemented
        public void DeleteGroupHallFixtureConfigure(int groupId, int consoleId, string fixtureTypes)
        {
            var response = _groupController.DeleteGroupHallFixtureConfigure(groupId, fixtureTypes, consoleId);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Group Hall Deleted Successfully");
        }




        [TestCaseSource(nameof(InputDataForDeleteGroupHallFixtureConsoleError))] //To be implemented
        public void DeleteGroupHallFixtureConfigureError(int groupId, int consoleId, string fixtureTypes)
        {
            //var jObject = JObject.Parse(File.ReadAllText(requestbody));
            //jObject[Constant.SECTIONTABS] = Constant.GROUPHALLFIXTURE;
            var response = _groupController.DeleteGroupHallFixtureConfigure(groupId, fixtureTypes, consoleId);
            Assert.ThrowsAsync<CustomException>(() => response);
        }




        [TestCaseSource(nameof(InputDataForDeleteGroupHallFixtureConsoleError))]

        public void DeleteGroupHallFixtureConfigureErrorInput(int groupId, int consoleId, string fixtureTypes)
        {
            //var jObject = JObject.Parse(File.ReadAllText(requestbody));
            //jObject[Constant.SECTIONTABS] = Constant.GROUPHALLFIXTURE;
            var response = _groupController.DeleteGroupHallFixtureConfigure(groupId, fixtureTypes, consoleId);
            Assert.ThrowsAsync<CustomException>(() => response);

        }


        [Test] //To be implemented
        public void DeleteGroupHallFixtureConsoleError()
        {
            //var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.DELETEGROUPHALLFIXTUREREQUESTBODY));
            //jObject[Constant.SECTIONTABS] = Constant.GROUPHALLFIXTURE;
            var response = _groupController.DeleteGroupHallFixtureConfigure(0, "Traditional_Hall_Stations", 2);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [Test] //To be implemented
        public void GetStartGroupHallFixtureErrorTest()
        {
            var response = _groupController.GetStartGroupHallFixture(0);
            Assert.ThrowsAsync<CustomException>(() => response);
        }



        [Test] //To be implemented
        public void SaveGroupHallFixtureErrorTest()
        {
            var jObject = JsonConvert.DeserializeObject<GroupHallFixturesData>(JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEGROUPHALLFIXTURESREQUESTBODY)).ToString());
            var response = _groupController.SaveGroupHallFixture(1, 0, 1, jObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [TestCaseSource(nameof(InputDataForSaveGroupLayoutDetails))]
        public void UpdateGroupLayoutDetails(int groupId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var groupLayoutObject = jObject.ToObject<GroupLayoutSave>();
            groupLayoutObject.UnitID = new List<int> { 1, 2, 3 };
            groupLayoutObject.CarPosition = new List<CarPosition> { new CarPosition { Position = "B1P1", UnitDesignation = "Front" } };
            var response = _groupController.UpdateGroupLayoutDetails(groupId, groupLayoutObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["Message"].ToString().Trim();
            Assert.AreEqual(key, "Building Configuration Updated Successfully");
        }

        [TestCaseSource(nameof(InputDataForSaveGroupLayoutDetails))]
        public void UpdateGroupLayoutDetailsNew(int groupId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var groupLayoutObject = jObject.ToObject<GroupLayoutSave>();
            groupLayoutObject.Operation = Operation.OverWrite;
            groupLayoutObject.UnitID = new List<int> { 1, 2, 3 };
            groupLayoutObject.CarPosition = new List<CarPosition> { new CarPosition { Position = "B1P1", UnitDesignation = "Front" } };
            var response = _groupController.UpdateGroupLayoutDetails(groupId, groupLayoutObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root.ToString().Trim();
            Assert.AreEqual(key, "[]");
           
        }

        [TestCaseSource(nameof(InputDataForSaveGroupLayoutDetails))]
        public void UpdateGroupLayoutDetailsInternalError(int groupId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var groupLayoutObject = jObject.ToObject<GroupLayoutSave>();
            var response = _groupController.UpdateGroupLayoutDetails(0, groupLayoutObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [TestCaseSource(nameof(InputDataForSaveGroupLayoutDetails))]
        public void SaveGroupLayoutDetailser(int groupId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var groupLayoutObject = jObject.ToObject<GroupLayoutSave>();
            groupLayoutObject.UnitID = new List<int> { 1, 2, 3 };
            groupLayoutObject.CarPosition = new List<CarPosition> { new CarPosition { Position = "B1P1", UnitDesignation = "Front" } };
            var response = _groupController.SaveGroupLayoutDetails(groupId, groupLayoutObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["Message"].ToString().Trim();
            Assert.AreEqual(key, "Building Configuration Saved Successfully");
        }

        [TestCaseSource(nameof(InputDataForSaveGroupLayoutDetails))]
        public void SaveGroupLayoutDetailsInternalError(int groupId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var groupLayoutObject = jObject.ToObject<GroupLayoutSave>();
            groupLayoutObject.UnitID = new List<int> { 1, 2, 3 };
            groupLayoutObject.CarPosition = new List<CarPosition> { new CarPosition { Position = "B1P1", UnitDesignation = "Front" } };
            var response = _groupController.SaveGroupLayoutDetails(0, groupLayoutObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(InputDataForSaveGroupLayoutDetails))] //To be implemented
        public void DuplicateGroupLayoutDetails(int groupId, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var groupLayoutObject = jObject.ToObject<GroupLayoutSave>();
            groupLayoutObject.Operation = Operation.Duplicate;
            groupLayoutObject.UnitID = new List<int> { 1, 2, 3 };
            groupLayoutObject.CarPosition = new List<CarPosition> { new CarPosition { Position="B1P1",UnitDesignation="Front"} };
            var response = _groupController.SaveGroupLayoutDetails(groupId, groupLayoutObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root.ToString().Trim();
            Assert.AreEqual(key, "[]");
        }

        [Test]
        public void GetRequestQueueByGroupId()
        {
            var response = _groupController.GetRequestQueueByGroupId(1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
        }

        [Test]
        public void GetRequestQueueByGroupIdErrorTest()
        {
            var response = _groupController.GetRequestQueueByGroupId(-1);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [TestCaseSource(nameof(InputDataForChangeFieldDrawingAutomation))]
        public void SaveFieldDrawingConfigure(string requestBody)
        {
            var Jobject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _groupController.SaveFieldDrawingConfigure(Jobject, 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
        }
        [TestCaseSource(nameof(InputDataForChangeFieldDrawingAutomation))]
        public void SaveFieldDrawingConfigureErrorTest(string requestBody)
        {
            var Jobject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _groupController.SaveFieldDrawingConfigure(Jobject, -1);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }
        [TestCaseSource(nameof(InputDataForChangeFieldDrawingAutomation))]
        public void SaveFieldDrawingConfigureModelStateErrorTest(string requestBody)
        {
            var Jobject = JObject.Parse(File.ReadAllText(requestBody));
            _groupController.ModelState.AddModelError("model", "model");
            var response = _groupController.SaveFieldDrawingConfigure(Jobject, -1);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        /// <param name = "configureRequest" ></ param >
        /// < param name="buildingId"></param>
        /// 
        [Test]
        public void StartFieldDrawingConfigure()
        {
            var response = _groupController.StartFieldDrawingConfigure(1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
        }
        [Test]
        public void StartFieldDrawingConfigureErrorTest()
        {
            var response = _groupController.StartFieldDrawingConfigure(-1);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [Test]
        public void StartFieldDrawingConfigureModelStateErrorTest()
        {
            _groupController.ModelState.AddModelError("test", "test");
            var response = _groupController.StartFieldDrawingConfigure(-1);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }


        [TestCaseSource(nameof(InputDataForChangeFieldDrawingAutomation))]
        public void ChangeFieldDrawingConfigureErrorTest(string requestBody)
        {
            var Jobject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _groupController.ChangeFieldDrawingConfigure(Jobject, -1);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [TestCaseSource(nameof(InputDataForChangeFieldDrawingAutomation))]
        public void ChangeFieldDrawingConfigureModelStateErrorTest(string requestBody)
        {
            var Jobject = JObject.Parse(File.ReadAllText(requestBody));
            _groupController.ModelState.AddModelError("test", "test");
            var response = _groupController.ChangeFieldDrawingConfigure(Jobject, -1);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }
    }
}
