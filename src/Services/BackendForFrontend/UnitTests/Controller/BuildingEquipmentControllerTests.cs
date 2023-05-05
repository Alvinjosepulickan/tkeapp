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
//    class BuildingEquipmentControllerTests
//    {
//        #region variables

//        private BuildingEquipmentController _buildingEquipmentController;
//        private ILogger<BuildingEquipmentController> _buildingEquipmentControllerLogger;

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
//            _buildingEquipmentControllerLogger = services.BuildServiceProvider().GetService<ILogger<BuildingEquipmentController>>();

//            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();

//            var iBuilding = (IBuildingEquipment)servicesProvider.GetService(typeof(IBuildingEquipment));
//            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));

//            _buildingEquipmentController = new BuildingEquipmentController(_buildingEquipmentControllerLogger, iBuilding, iConfigure);
//            _buildingEquipmentController.ControllerContext = new ControllerContext();
//            _buildingEquipmentController.ControllerContext.HttpContext = new DefaultHttpContext();
//            _buildingEquipmentController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
//        }
//        #endregion

//        #region SetUp Input Values

//        public static IEnumerable<TestCaseData> InputDataForChangeBuildingEquipment()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.CHANGEBUILDINGEQUIPMENTREQUESTBODY, 1);
//        }

//        public static IEnumerable<TestCaseData> InputDataForSaveBuildingEquipmentConsole()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGEQUIPMENTCONSOLEREQUESTBODY, 1, 1);
//        }

//        public static IEnumerable<TestCaseData> InputDataForChangeBuldingEquipmentConsole()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.CHANGEBULDINGEQUIPMENTCONSOLEREQUESTBODY);
//        }
//        #endregion

//        [Test]
//        public void StartBuildingEquipmentTest()
//        {
//            var response = _buildingEquipmentController.StartBuildingEquipmentConfigure(null, 1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [Test]
//        public void StartBuildingEquipmentErrorTest()
//        {
//            var response = _buildingEquipmentController.StartBuildingEquipmentConfigure(null, -1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForChangeBuildingEquipment))]
//        public void ChangeBuildingEquipmentTest(string requestBody, int buildingId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _buildingEquipmentController.ChangeBuildingEquipmentConfigure(jObject, buildingId);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [TestCaseSource(nameof(InputDataForSaveBuildingEquipmentConsole))]
//        public void SaveAssignedGroupsTest(string requestBody, int buildingId, int consoleId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var buildingData = jObject.ToObject<BuildingEquipmentData>();
//            var response = _buildingEquipmentController.SaveAssignGroups(consoleId, buildingId, buildingData);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [TestCaseSource(nameof(InputDataForSaveBuildingEquipmentConsole))]
//        public void SaveAssignedGroupsErrorTest(string requestBody, int buildingId, int consoleId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var buildingData = jObject.ToObject<BuildingEquipmentData>();
//            var response = _buildingEquipmentController.SaveAssignGroups(consoleId, 0, buildingData);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForSaveBuildingEquipmentConsole))]
//        public void SaveAssignedGroupsModelStateErrorTest(string requestBody, int buildingId, int consoleId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var buildingData = jObject.ToObject<BuildingEquipmentData>();
//            _buildingEquipmentController.ModelState.AddModelError("test", "test");
//            var response = _buildingEquipmentController.SaveAssignGroups(consoleId, buildingId, buildingData);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForSaveBuildingEquipmentConsole))]
//        public void UpdateAssignedGroupsTest(string requestBody, int buildingId, int consoleId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var buildingData = jObject.ToObject<BuildingEquipmentData>();
//            var response = _buildingEquipmentController.UpdateAssignGroups(consoleId, buildingId, buildingData);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [TestCaseSource(nameof(InputDataForSaveBuildingEquipmentConsole))]
//        public void UpdateAssignedGroupsErrorTest(string requestBody, int buildingId, int consoleId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var buildingData = jObject.ToObject<BuildingEquipmentData>();
//            var response = _buildingEquipmentController.UpdateAssignGroups(consoleId, 0, buildingData);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForSaveBuildingEquipmentConsole))]
//        public void UpdateAssignedGroupsModelStateErrorTest(string requestBody, int buildingId, int consoleId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var buildingData = jObject.ToObject<BuildingEquipmentData>();
//            _buildingEquipmentController.ModelState.AddModelError("test", "test");
//            var response = _buildingEquipmentController.UpdateAssignGroups(consoleId, 0, buildingData);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void DuplicateBuildingEquipmentConsoleTest()
//        {
//            var response = _buildingEquipmentController.DuplicateBuildingEquipmentConsole(1, 1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [Test]
//        public void DuplicateBuildingEquipmentConsoleErrorTest()
//        {
//            var response = _buildingEquipmentController.DuplicateBuildingEquipmentConsole(0, 1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void DuplicateBuildingEquipmentConsoleModelStateErrorTest()
//        {
//            _buildingEquipmentController.ModelState.AddModelError("test", "test");
//            var response = _buildingEquipmentController.DuplicateBuildingEquipmentConsole(0, 1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void DeleteBuildingEquipmentConsoleTest()
//        {
//            var response = _buildingEquipmentController.DeleteBuildingEquipmentConsole(1, 1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [Test]
//        public void DeleteBuildingEquipmentConsoleModelErrorTest()
//        {
//            _buildingEquipmentController.ModelState.AddModelError("model", "model");
//            var response = _buildingEquipmentController.DeleteBuildingEquipmentConsole(1, 1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.NOTFOUND);
//        }

//        [Test]
//        public void DeleteBuildingEquipmentConsoleErrorTest()
//        {
//            var response = _buildingEquipmentController.DeleteBuildingEquipmentConsole(0, 1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForChangeBuildingEquipment))]
//        public void SaveBuildingEquipmentConfigurationTest(string requestBody, int buildingId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _buildingEquipmentController.SaveBuildingEquipmentConfiguration(buildingId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [TestCaseSource(nameof(InputDataForChangeBuildingEquipment))]
//        public void SaveBuildingEquipmentConfigurationErrorTest(string requestBody, int buildingId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _buildingEquipmentController.SaveBuildingEquipmentConfiguration(0, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForChangeBuildingEquipment))]
//        public void SaveBuildingEquipmentConfigurationModelStateErrorTest(string requestBody, int buildingId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            _buildingEquipmentController.ModelState.AddModelError("test", "test");
//            var response = _buildingEquipmentController.SaveBuildingEquipmentConfiguration(0, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void StartBuildingEquipmentConsoleErrorTest()
//        {
//            var response = _buildingEquipmentController.StartBuildingEquipmentConsole(0, null, 1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void StartBuildingEquipmentConsoleTest()
//        {
//            var response = _buildingEquipmentController.StartBuildingEquipmentConsole(0, null, 1);
//            Assert.IsNotNull(response);

//        }

//        [TestCaseSource(nameof(InputDataForChangeBuldingEquipmentConsole))]
//        public void ChangeBuildingEquipmentTest(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _buildingEquipmentController.ChangeBuildingEquipmentConfigure(jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForChangeBuldingEquipmentConsole))]
//        public void ChangeBuildingEquipmentConsoleTest(string requestBody)
//        {
//            var jObject = JsonConvert.DeserializeObject<BuildingEquipmentData>(JObject.Parse(File.ReadAllText(requestBody)).ToString());
//            var response = _buildingEquipmentController.ChangeBuildingEquipmentConsoleConfigure(1,jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForChangeBuldingEquipmentConsole))]
//        public void ChangeBuildingEquipmentConsoleModelErrorTest(string requestBody)
//        {
//            var jObject = JsonConvert.DeserializeObject<BuildingEquipmentData>(JObject.Parse(File.ReadAllText(requestBody)).ToString());
//            _buildingEquipmentController.ModelState.AddModelError("model", "model");
//            var response = _buildingEquipmentController.ChangeBuildingEquipmentConsoleConfigure(1, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForChangeBuildingEquipment))]
//        public void UpdateBuildingEquipmentConfigurationTest(string requestBody, int buildingId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _buildingEquipmentController.UpdateBuildingEquipmentConfiguration(buildingId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        [TestCaseSource(nameof(InputDataForChangeBuildingEquipment))]
//        public void UpdateBuildingEquipmentConfigurationErrorTest(string requestBody, int buildingId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _buildingEquipmentController.UpdateBuildingEquipmentConfiguration(0, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForChangeBuildingEquipment))]
//        public void UpdateBuildingEquipmentConfigurationModelStateErrorTest(string requestBody, int buildingId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            _buildingEquipmentController.ModelState.AddModelError("test", "test");
//            var response = _buildingEquipmentController.UpdateBuildingEquipmentConfiguration(0, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }
//    }
    
//}
