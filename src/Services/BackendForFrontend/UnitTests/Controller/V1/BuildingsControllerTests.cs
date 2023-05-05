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
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.BFF.Test.Common;
using TKE.SC.BFF.Api.V1;
using Newtonsoft.Json;
using TKE.SC.Common.Model.ExceptionModel;
using System.Security.Principal;

namespace TKE.SC.BFF.UnitTests.Controller.V1
{
    public class BuildingsControllerTests
    {


        #region variables

        private BuildingsController _buildingConfigurationController;
        private ILogger<BuildingsController> _buildingConfigurationControllerLogger;

        #endregion

        [SetUp, Order(1)]
        public void InitialConfigurationSetup()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _buildingConfigurationControllerLogger = services.BuildServiceProvider().GetService<ILogger<BuildingsController>>();

            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();

            var iBuilding = (IBuildingConfiguration)servicesProvider.GetService(typeof(IBuildingConfiguration));
            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));

            var iBuildingEquipment = (IBuildingEquipment)servicesProvider.GetService(typeof(IBuildingEquipment));
            var identity = new GenericIdentity("testuser", "jwt");
            identity.AddClaim(new System.Security.Claims.Claim("SessionId", "SessionId"));
            var principal = new GenericPrincipal(identity, new string[] { "SessionId" });
            _buildingConfigurationController = new BuildingsController(iBuilding, iBuildingEquipment, iConfigure, _buildingConfigurationControllerLogger);
            _buildingConfigurationController.ControllerContext = new ControllerContext();
            _buildingConfigurationController.ControllerContext.HttpContext = new DefaultHttpContext();
            _buildingConfigurationController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
            _buildingConfigurationController.HttpContext.User = principal;
        }


        #region SetUp Input Values

        public static IEnumerable<TestCaseData> InputBuildingValues()
        {
            yield return new TestCaseData(15);
        }
        public static IEnumerable<TestCaseData> InputDataForGetBuildingConfiguration()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY, 1);
        }
        public static IEnumerable<TestCaseData> InputDataForGetBuildingConfigurationError()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY, 0);
        }

        public static IEnumerable<TestCaseData> InputDataForSaveBuildingConfiguration()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGREQUESTBODY, "15", "building2");
        }

        public static IEnumerable<TestCaseData> InputDataForSaveBuildingConfigurationError()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGREQUESTBODY, "", "");
        }

        public static IEnumerable<TestCaseData> InputDataForUpdateBuildingConfiguration()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEBUILDINGREQUESTBODY, "15", 15, "building2");
        }

        public static IEnumerable<TestCaseData> InputDataForUpdateBuildingConfigurationError()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEBUILDINGREQUESTBODY, "1", 0, "");
        }


        public static IEnumerable<TestCaseData> InputDataForSavingBuildingElevation()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGELEVATIONREQUESTBODY2);
        }

        public static IEnumerable<TestCaseData> InputDataForSavingBuildingElevationInternalError()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGELEVATIONREQUESTBODY);
        }
        
        public static IEnumerable<TestCaseData> InputDataForSavingBuildingElevationError()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGELEVATIONREQUESTBODY1);
        }
        public static IEnumerable<TestCaseData> InputDataForSavingBuildingElevationNonError()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGEQUIPMENTCONSOLEREQUESTBODY);
        }
        public static IEnumerable<TestCaseData> InputDataForUpdateBuildingElevation()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEBUILDINGELEVATIONREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForUpdateBuildingElevationInternalError()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEBUILDINGELEVATIONREQUESTBODY2);
        }

        public static IEnumerable<TestCaseData> InputDataForUpdateBuildingElevationError()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEBUILDINGELEVATIONREQUESTBODY1);
        }

        public static IEnumerable<TestCaseData> InputBuildingIdForBuildingElevation()
        {
            yield return new TestCaseData(9);
        }

        public static IEnumerable<TestCaseData> InputBuildingIdToDelete()
        {
            yield return new TestCaseData(1);
        }

        public static IEnumerable<TestCaseData> InputBuildingIdToDeleteError()
        {
            yield return new TestCaseData(0);
        }

        public static IEnumerable<TestCaseData> InputDataForStartBuildingConfigure()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY);
        }
        public static IEnumerable<TestCaseData> InputDataForStartBuildingConfigureValue()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY);
        }
        public static IEnumerable<TestCaseData> InputDataForStartBuildingConfigureVal()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGELEVATIONREQUESTBODYVALUE);
        }
        public static IEnumerable<TestCaseData> VariableAssignmentsObject()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGELEVATIONREQUESTBODYVALUE);
        }

        public static IEnumerable<TestCaseData> InputDataForBuildingConfigureError()
        {
            yield return new TestCaseData(0, AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY);

        }
        public static IEnumerable<TestCaseData> InputDataForChangeBuildingConfigure()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForStartBuildingConfigureError()
        {
            yield return new TestCaseData("bsa", AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY);
        }
        #endregion


        [TestCaseSource(nameof(InputDataForSaveBuildingConfiguration))]
        public void SaveBuildingConfigurationForProjectController(string requestBody, string projectId, string buildingName)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<BuildingConfiguration>();
            var response = _buildingConfigurationController.SaveBuildingConfigurationForProject(requestObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Building Saved Successfully");
        }
        [TestCaseSource(nameof(InputDataForSaveBuildingConfiguration))]
        public void DuplicateBuildingConfigurationForProjectController(string requestBody, string projectId, string buildingName)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<BuildingConfiguration>();
            requestObject.BuildingIDs = new List<int> { 214 };
            requestObject.VariableAssignments = null;
            requestObject.Operation = Operation.Duplicate;
            var response = _buildingConfigurationController.SaveBuildingConfigurationForProject(requestObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
        }
        [TestCaseSource(nameof(InputDataForSaveBuildingConfiguration))]
        public void DuplicateBuildingConfigurationForProjectControllerError(string requestBody, string projectId, string buildingName)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<BuildingConfiguration>();
            requestObject.BuildingIDs = new List<int> { 0 };
            requestObject.QuoteId = "-1";
            requestObject.VariableAssignments = null;
            requestObject.Operation = Operation.Duplicate;
            var response = _buildingConfigurationController.SaveBuildingConfigurationForProject(requestObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(InputDataForSaveBuildingConfigurationError))] //to be implemented
        public void SaveBuildingConfigurationForProjectControllerError(string requestBody, string projectId, string buildingName)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<BuildingConfiguration>();
            requestObject.BuildingIDs = new List<int> { 0 };
            var response = _buildingConfigurationController.SaveBuildingConfigurationForProject(requestObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [TestCaseSource(nameof(InputDataForUpdateBuildingConfiguration))]
        public void UpdateBuildingConfigurationForProjectController(string requestBody, string projectId, int buildingId, string buildingName)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<BuildingConfiguration>();
            requestObject.BuildingIDs = new List<int> { 1 };
            requestObject.QuoteId = "1";
            var response = _buildingConfigurationController.UpdateBuildingConfigurationForProject(requestObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Building Saved Successfully");
        }

        [TestCaseSource(nameof(InputDataForUpdateBuildingConfiguration))] //to be implemented
        public void UpdateBuildingConfigurationForProjectControllerInput(string requestBody, string projectId, int buildingId, string buildingName)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<BuildingConfiguration>();
            requestObject.BuildingIDs = new List<int> { 1 };
            requestObject.QuoteId = "1";
            var response = _buildingConfigurationController.UpdateBuildingConfigurationForProject(requestObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Building Saved Successfully");
        }

        [TestCaseSource(nameof(InputDataForUpdateBuildingConfigurationError))]
        public void UpdateBuildingConfigurationForProjectControllerError(string requestBody, string projectId, int buildingId, string buildingName)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<BuildingConfiguration>();
            requestObject.BuildingIDs = new List<int> {0 };
            requestObject.QuoteId = "1";
            var response = _buildingConfigurationController.UpdateBuildingConfigurationForProject(requestObject);
            Assert.ThrowsAsync<CustomException>(() => response);

        }

        [TestCaseSource(nameof(InputDataForSavingBuildingElevationError))] //to be implemented
        public void SaveBuildingElevationController(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            buildingElevationObject.createdBy.UserId = "User";
            var response = _buildingConfigurationController.SaveBuildingElevation(buildingElevationObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Building Saved Successfully");
        }

        [TestCaseSource(nameof(InputDataForSavingBuildingElevationInternalError))] //to be implemented
        public void SaveBuildingElevationControllerInternalError(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            var response = _buildingConfigurationController.SaveBuildingElevation(buildingElevationObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// Update the Building ElevationController
        /// </summary>
        /// <param name="buildingElevation"></param>
        [TestCaseSource(nameof(InputDataForUpdateBuildingElevation))] //to be implemented
        public void UpdateBuildingElevationController(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            var response = _buildingConfigurationController.UpdateBuildingElevation(buildingElevationObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Building Updated Successfully");
        }

        [TestCaseSource(nameof(InputDataForUpdateBuildingElevationError))] //to be implemented
        public void UpdateBuildingElevationControllerError(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            buildingElevationObject.buildingConfigurationId = 0;
            var response = _buildingConfigurationController.UpdateBuildingElevation(buildingElevationObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [TestCaseSource(nameof(InputBuildingIdForBuildingElevation))]
        public void GetBuildingElevationByIdController(int buildingId)
        {
            var response = _buildingConfigurationController.GetBuildingElevationById(buildingId);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["buildingConfigurationId"].ToString().Trim();
            Assert.AreEqual(key, "1");
        }

        [Test] //to be implemented
        public void GetBuildingElevationById()
        {
            var response = _buildingConfigurationController.GetBuildingElevationById(1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["buildingConfigurationId"].ToString().Trim();
            Assert.AreEqual(key, "1");

        }

        [Test] //to be implemented
        public void GetBuildingElevationByIdError()
        {
            var response = _buildingConfigurationController.GetBuildingElevationById(0);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(InputBuildingIdToDelete))]
        public void DeleteBuildingConfigurationByIdController(int buildingConfigurationId)
        {
            var response = _buildingConfigurationController.DeleteBuildingConfigurationById(buildingConfigurationId);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Building Deleted Successfully");
        }

        [TestCaseSource(nameof(InputBuildingIdToDelete))] //to be implemented
        public void DeleteBuildingConfigurationByIdControllerInput(int buildingConfigurationId)
        {
            var response = _buildingConfigurationController.DeleteBuildingConfigurationById(buildingConfigurationId);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Building Deleted Successfully");
        }

        [TestCaseSource(nameof(InputBuildingIdToDeleteError))] //to be implemented
        public void DeleteBuildingConfigurationByIdControllerError(int buildingConfigurationId)
        {
            var response = _buildingConfigurationController.DeleteBuildingConfigurationById(0);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [TestCaseSource(nameof(InputBuildingIdToDelete))]
        public void StartBuildingConfigure(int val)
        {
            var response = _buildingConfigurationController.StartBuildingConfigure("1", val);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["isEditFlow"].ToString().Trim();
            Assert.AreEqual(key, "False");
        }

        [Test] //to be implemented
        public void StartBuildingConfigureTest()
        {
            var response = _buildingConfigurationController.StartBuildingConfigure("", 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["isEditFlow"].ToString().Trim();
            Assert.AreEqual(key, "False");
        }



        [TestCaseSource(nameof(InputDataForChangeBuildingConfigure))]
        public void ChangeBuildingConfigure(string requestbody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestbody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _buildingConfigurationController.ChangeBuildingConfigure(jObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["isDelete"].ToString().Trim();
            Assert.AreEqual(key, "False");
        }


        [Test] //to be implemented
        public void DeleteBuildingConfigurationByIdControllerError()
        {
            var response = _buildingConfigurationController.DeleteBuildingConfigurationById(0);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test]
        public void SaveBuildingConfigurationForProjectInternalError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEBUILDINGREQUESTBODY));
            var requestObject = jObject.ToObject<BuildingConfiguration>();
            requestObject.BuildingIDs = new List<int> {0 };
            var response = _buildingConfigurationController.SaveBuildingConfigurationForProject(requestObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// Saving the Building ElevationControllerError
        /// </summary>
        [TestCaseSource(nameof(InputDataForSavingBuildingElevationError))] //to be implemented
        public void SaveBuildingElevationControllerError(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            var response = _buildingConfigurationController.SaveBuildingElevation(buildingElevationObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test]
        public void UpdateBuildingConfigurationForProjectError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEBUILDINGREQUESTBODY));
            var requestObject = jObject.ToObject<BuildingConfiguration>();
            requestObject.BuildingIDs = new List<int> { };
            var response = _buildingConfigurationController.UpdateBuildingConfigurationForProject(requestObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(InputDataForUpdateBuildingElevationError))] //to be implemented
        public void UpdateBuildingElevationError(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            buildingElevationObject.buildingConfigurationId = 0;
            var response = _buildingConfigurationController.UpdateBuildingElevation(buildingElevationObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [TestCaseSource(nameof(InputDataForSavingBuildingElevationError))] //to be implemented
        public void SaveBuildingElevationControllerError1(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            buildingElevationObject.createdBy.UserId = "";
            var response = _buildingConfigurationController.SaveBuildingElevation(buildingElevationObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test]
        public void StartBuildingConfigureError()
        {
            var response = _buildingConfigurationController.StartBuildingConfigure("",-1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(InputDataForUpdateBuildingElevationInternalError))] //to be implemented
        public void UpdateBuildingElevationControllerInternalError(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            var response = _buildingConfigurationController.UpdateBuildingElevation(buildingElevationObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test]
        public void ResetBuildingController()
        {
            var response = _buildingConfigurationController.ResetBuildingConfigure( 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["isEditFlow"].ToString().Trim();
            Assert.AreEqual(key, "False");
        }

        [Test] //to be implemented
        public void GetBuildingConfigurationSectionTab()
        {
            var response = _buildingConfigurationController.GetBuildingConfigurationSectionTab(1);
            Assert.AreEqual(200, (response.Result as ObjectResult).StatusCode);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["sections"].ToString().Trim();
            JArray json = JArray.Parse(key);
            var keyValue = json.Root[0]["id"].ToString().Trim();
            Assert.AreEqual(keyValue, "buildingconfigure");
        }

        [Test] //to be implemented
        public void GetBuildingConfigurationSectionTabError()
        {
            var response = _buildingConfigurationController.GetBuildingConfigurationSectionTab(0);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test] //to be implemented
        public void StartBuildingEquipmentTest()
        {
            var response = _buildingConfigurationController.StartBuildingEquipmentConfigure(1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["sections"].ToString().Trim();
            JArray json = JArray.Parse(key);
            var keyValue = json.Root[0]["id"].ToString().Trim();
            Assert.AreEqual(keyValue, "buildingEquipment");
        }

        [Test] //to be implemented
        public void StartBuildingEquipmentErrorTest()
        {
            var response = _buildingConfigurationController.StartBuildingEquipmentConfigure(-1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [TestCaseSource(nameof(InputDataForStartBuildingConfigureVal))] //to be implemented
        public void ChangeBuildingEquipmentTest(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _buildingConfigurationController.ChangeBuildingEquipmentConfigure(jObject, 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["sections"].ToString().Trim();
            JArray json = JArray.Parse(key);
            var keyValue = json.Root[0]["id"].ToString().Trim();
            Assert.AreEqual(keyValue, "buildingEquipment");
        }

        [TestCaseSource(nameof(InputDataForSavingBuildingElevationNonError))] //to be implemented
        public void SaveAssignedGroupsTest(string requestBody)
        {
            int buildingId = 1;
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var buildingDataVal= jObject.ToObject<BuildingEquipmentData>();
            var buildingData = new List<BuildingEquipmentData> { buildingDataVal};
            var response = _buildingConfigurationController.SaveAssignGroups(buildingId, buildingData);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Saved Assigned Groups");
        }

        [TestCaseSource(nameof(InputDataForSavingBuildingElevationNonError))] //to be implemented
        public void InputDataForSavingBuildingElevation(string requestBody)
        {
            int buildingId = 1;
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var buildingDataVal = jObject.ToObject<BuildingEquipmentData>();
            var buildingData = new List<BuildingEquipmentData> { buildingDataVal };
            var response = _buildingConfigurationController.SaveAssignGroups(buildingId, buildingData);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Saved Assigned Groups");
        }



        [TestCaseSource(nameof(InputDataForSavingBuildingElevationNonError))] //to be implemented
        public void UpdateAssignedGroupsTest(string requestBody)
        {
            int buildingId = 1; int consoleId = 1;
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var buildingDataVal = jObject.ToObject<BuildingEquipmentData>();
            var buildingData = new List<BuildingEquipmentData> { buildingDataVal };
            var response = _buildingConfigurationController.UpdateAssignGroups(consoleId, buildingId, buildingData);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Saved Assigned Groups");
        }

        [TestCaseSource(nameof(InputDataForSavingBuildingElevationNonError))]//to be implemented
        public void UpdateAssignedGroupsErrorTest(string requestBody )
        {
            int consoleId=5;
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var buildingDataVal = jObject.ToObject<BuildingEquipmentData>();
            buildingDataVal.ConsoleId = 5;
            var buildingData = new List<BuildingEquipmentData> { buildingDataVal };
            var response = _buildingConfigurationController.UpdateAssignGroups(consoleId,1, buildingData);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [Test] //to be implemented
        public void DuplicateBuildingEquipmentConsoleTest()
        {
            var response = _buildingConfigurationController.DuplicateBuildingEquipmentConsole(1, 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Building Equipment selections saved successfully");
        }

        [Test] //to be implemented
        public void DuplicateBuildingEquipmentConsoleErrorTest()
        {
            var response = _buildingConfigurationController.DuplicateBuildingEquipmentConsole(0, 0);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [Test] //to be implemented
        public void DeleteBuildingEquipmentConsoleTest()
        {
            var response = _buildingConfigurationController.DeleteBuildingEquipmentConsole(1, 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Building Equipment selections saved successfully");
        }

        [Test] //to be implemented
        public void DeleteBuildingEquipmentConsoleErrorTest()
        {
            var response = _buildingConfigurationController.DeleteBuildingEquipmentConsole(0, 1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(VariableAssignmentsObject))] //to be implemented
        public void SaveBuildingEquipmentConfigurationTest(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _buildingConfigurationController.SaveBuildingEquipmentConfiguration(1, jObject, true);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Building Configuration selections saved successfully");
        }

        [TestCaseSource(nameof(VariableAssignmentsObject))] //to be implemented
        public void SaveBuildingEquipmentConfigurationErrorTest(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _buildingConfigurationController.SaveBuildingEquipmentConfiguration(0, jObject, true);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [TestCaseSource(nameof(InputDataForStartBuildingConfigureVal))] //to be implemented
        public void UpdateBuildingEquipmentConfigurationTest(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _buildingConfigurationController.UpdateBuildingEquipmentConfiguration(1, jObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Building Configuration selections saved successfully");
        }

        [TestCaseSource(nameof(InputDataForStartBuildingConfigureVal))] //to be implemented
        public void UpdateBuildingEquipmentConfigurationErrorTest(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _buildingConfigurationController.UpdateBuildingEquipmentConfiguration(0, jObject);
            Assert.ThrowsAsync<CustomException>(() => response);

        }

        [Test] //to be implemented
        public void StartBuildingEquipmentConsoleErrorTest()
        {
            var response = _buildingConfigurationController.StartBuildingEquipmentConsole(0, 1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test]
        public void StartBuildingEquipmentConsoleTest()
        {
            var response = _buildingConfigurationController.StartBuildingEquipmentConsole(1, 1);
            Assert.ThrowsAsync<CustomException>(() => response);

        }

        [TestCaseSource(nameof(InputDataForStartBuildingConfigure))] //to be implemented
        public void ChangeBuildingEquipmentConsoleTest(string requestBody)
        {
            var jObject = JsonConvert.DeserializeObject<BuildingEquipmentData>(JObject.Parse(File.ReadAllText(requestBody)).ToString());
            jObject.VariableAssignments = new List<ConfigVariable> { new ConfigVariable { Value=1,VariableId=""} };
            var response = _buildingConfigurationController.ChangeBuildingEquipmentConsoleConfigure(1, jObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        [Test] //to be implemented//to be implemented
        public void ResetBuildingErrorTest()
        {
            var response = _buildingConfigurationController.ResetBuildingConfigure(0);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test]
        public void ResetBuildingErrorTest2()
        {
            var response = _buildingConfigurationController.ResetBuildingConfigure( 0);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

    }
}
