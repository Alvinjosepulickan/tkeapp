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
//using TKE.SC.Common.Model.UIModel;
//using TKE.SC.Common.Model.ViewModel;
//using TKE.SC.BFF.Test.Common;

//namespace TKE.SC.BFF.Test.Controller
//{
//    public class BuildingConfigurationControllerTests
//    {


//        #region variables

//        private BuildingConfigurationController _buildingConfigurationController;
//        private ILogger<BuildingConfigurationController> _buildingConfigurationControllerLogger;

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
//            _buildingConfigurationControllerLogger = services.BuildServiceProvider().GetService<ILogger<BuildingConfigurationController>>();

//            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();

//            var iBuilding = (IBuildingConfiguration)servicesProvider.GetService(typeof(IBuildingConfiguration));
//            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));



//            _buildingConfigurationController = new BuildingConfigurationController(_buildingConfigurationControllerLogger, iBuilding, iConfigure);
//            _buildingConfigurationController.ControllerContext = new ControllerContext();
//            _buildingConfigurationController.ControllerContext.HttpContext = new DefaultHttpContext();
//            _buildingConfigurationController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
//        }
//        #endregion

//        #region SetUp Input Values

//        public static IEnumerable<TestCaseData> InputBuildingValues()
//        {
//            yield return new TestCaseData(15);
//        }
//        public static IEnumerable<TestCaseData> InputDataForGetBuildingConfiguration()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY, 1);
//        }
//        public static IEnumerable<TestCaseData> InputDataForGetBuildingConfigurationError()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY, 0);
//        }

//        public static IEnumerable<TestCaseData> InputDataForSaveBuildingConfiguration()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGREQUESTBODY, "15", "building2");
//        }

//        public static IEnumerable<TestCaseData> InputDataForSaveBuildingConfigurationError()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGREQUESTBODY, "", "");
//        }

//        public static IEnumerable<TestCaseData> InputDataForUpdateBuildingConfiguration()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEBUILDINGREQUESTBODY, "15", 15, "building2");
//        }

//        public static IEnumerable<TestCaseData> InputDataForUpdateBuildingConfigurationError()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEBUILDINGREQUESTBODY, "1", 0, "");
//        }


//        public static IEnumerable<TestCaseData> InputDataForSavingBuildingElevation()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGELEVATIONREQUESTBODY2);
//        }

//        public static IEnumerable<TestCaseData> InputDataForSavingBuildingElevationInternalError()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGELEVATIONREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForSavingBuildingElevationError()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGELEVATIONREQUESTBODY1);
//        }

//        public static IEnumerable<TestCaseData> InputDataForUpdateBuildingElevation()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEBUILDINGELEVATIONREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForUpdateBuildingElevationInternalError()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEBUILDINGELEVATIONREQUESTBODY2);
//        }

//        public static IEnumerable<TestCaseData> InputDataForUpdateBuildingElevationError()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEBUILDINGELEVATIONREQUESTBODY1);
//        }

//        public static IEnumerable<TestCaseData> InputBuildingIdForBuildingElevation()
//        {
//            yield return new TestCaseData(9);
//        }

//        public static IEnumerable<TestCaseData> InputBuildingIdToDelete()
//        {
//            yield return new TestCaseData(1);
//        }

//        public static IEnumerable<TestCaseData> InputBuildingIdToDeleteError()
//        {
//            yield return new TestCaseData(0);
//        }

//        public static IEnumerable<TestCaseData> InputDataForStartBuildingConfigure()
//        {
//            yield return new TestCaseData("val", AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForBuildingConfigureError()
//        {
//            yield return new TestCaseData(0, AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY);

//        }
//        public static IEnumerable<TestCaseData> InputDataForChangeBuildingConfigure()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForStartBuildingConfigureError()
//        {
//            yield return new TestCaseData("bsa",AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY);
//        }
//        #endregion

//        /// GetListOfBuildingsForProjectController
//        /// </summary>
//        /// <param name="projectCode"></param>
//        //[TestCaseSource(nameof(InputBuildingValues))]
//        //public void GetListOfBuildingsForProjectController(string projectId)
//        //{
//        //    var response = _buildingConfigurationController.GetListOfConfigurationForProject(projectId);
//        //    Assert.IsNotNull(response);
//        //}

//        [Test]
//        public void GetListOfBuildingsForProjectInput()
//        {
//            var response = _buildingConfigurationController.GetListOfConfigurationForProject("1");
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        /// <summary>
//        /// GetListOfBuildingsForProject
//        /// </summary>
//        [Test]
//        public void GetListOfBuildingsForProject()
//        {
//            var response = _buildingConfigurationController.GetListOfConfigurationForProject("15");
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// GetListOfBuildingsForProjectError
//        /// </summary>
//        [Test]
//        public void GetListOfBuildingsForProjectError()
//        {
//            var response = _buildingConfigurationController.GetListOfConfigurationForProject("");
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void GetListOfBuildingsForProjectModelStateError()
//        {
//            _buildingConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _buildingConfigurationController.GetListOfConfigurationForProject("");
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        /// <summary>
//        /// GetBuildingConfigurationByIdController
//        /// </summary>
//        /// <param name="buildingId"></param>
//        //[TestCaseSource(nameof(InputDataForGetBuildingConfiguration))]
//        //public void GetBuildingConfigurationByIdController(string requestBody, int buildingId)
//        //{
//        //    var jObject = JObject.Parse(File.ReadAllText(requestBody));
//        //    //var requestObject = jObject.ToObject<ConfigurationRequest>();
//        //    var response = _buildingConfigurationController.GetBuildingConfigurationById(buildingId);
//        //    Assert.IsNotNull(response);
//        //}

//        //// <summary>
//        //// GetBuildingConfigurationById
//        ////</summary>
//        //[TestCaseSource(nameof(InputDataForGetBuildingConfiguration))]
//        //public void GetBuildingConfigurationByIdControllerInput(string requestBody, int buildingId)
//        //{
//        //    var jObject = JObject.Parse(File.ReadAllText(requestBody));
//        //    var requestObject = jObject.ToObject<ConfigurationRequest>();
//        //    var response = _buildingConfigurationController.GetBuildingConfigurationById(buildingId, requestObject);
//        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);

//        //    //Assert.IsNotNull(response);
//        //}


//        ///// <summary>
//        ///// GetBuildingConfigurationByIdError
//        ///// </summary>
//        //[TestCaseSource(nameof(InputDataForGetBuildingConfigurationError))]
//        //public void GetBuildingConfigurationByIdError(string requestBody, int buildingId)
//        //{
//        //    var jObject = JObject.Parse(File.ReadAllText(requestBody));
//        //    //var requestObject = jObject.ToObject<ConfigurationRequest>();
//        //    var response = _buildingConfigurationController.GetBuildingConfigurationById(buildingId);
//        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        //}

//        //[TestCaseSource(nameof(InputDataForGetBuildingConfigurationError))]
//        //public void GetBuildingConfigurationByIdModelStateError(string requestBody, int buildingId)
//        //{
//        //    var jObject = JObject.Parse(File.ReadAllText(requestBody));
//        //    //var requestObject = jObject.ToObject<ConfigurationRequest>();
//        //    _buildingConfigurationController.ModelState.AddModelError("test", "test");
//        //    var response = _buildingConfigurationController.GetBuildingConfigurationById(buildingId);
//        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        //}

//        /// <summary>
//        /// SaveBuildingConfigurationForProjectController
//        /// </summary>
//        /// <param name="projectId"></param>
//        /// <param name="UserId"></param>
//        /// <param name="buildingName"></param>
//        [TestCaseSource(nameof(InputDataForSaveBuildingConfiguration))]
//        public void SaveBuildingConfigurationForProjectController(string requestBody, string projectId, string buildingName)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var requestObject = jObject.ToObject<BuildingConfiguration>();
//            var response = _buildingConfigurationController.SaveBuildingConfigurationForProject(projectId, requestObject);
//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForSaveBuildingConfiguration))]
//        public void DuplicateBuildingConfigurationForProjectController(string requestBody, string projectId, string buildingName)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var requestObject = jObject.ToObject<BuildingConfiguration>();
//            requestObject.BuildingIDs =new List<int> { 214 };
//            requestObject.VariableAssignments = null;
//            requestObject.Operation = Operation.Duplicate;
//            var response = _buildingConfigurationController.SaveBuildingConfigurationForProject(projectId, requestObject);
//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForSaveBuildingConfiguration))]
//        public void DuplicateBuildingConfigurationForProjectControllerError(string requestBody, string projectId, string buildingName)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var requestObject = jObject.ToObject<BuildingConfiguration>();
//            requestObject.BuildingIDs = new List<int> { 0 };
//            requestObject.VariableAssignments = null;
//            requestObject.Operation = Operation.Duplicate;
//            var response = _buildingConfigurationController.SaveBuildingConfigurationForProject(projectId, requestObject);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// GetBuildingConfigurationByIdController
//        /// </summary>
//        /// <param name="projectId"></param>
//        /// <param name="UserId"></param>
//        /// <param name="buildingName"></param>
//        [TestCaseSource(nameof(InputDataForSaveBuildingConfigurationError))]
//        public void SaveBuildingConfigurationForProjectControllerError(string requestBody,  string projectId, string buildingName)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var requestObject = jObject.ToObject<BuildingConfiguration>();
//            var response = _buildingConfigurationController.SaveBuildingConfigurationForProject(projectId, requestObject); 
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);

            
//        }

//        [TestCaseSource(nameof(InputDataForSaveBuildingConfigurationError))]
//        public void SaveBuildingConfigurationForProjectControllerModelStateError(string requestBody, string projectId, string buildingName)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var requestObject = jObject.ToObject<BuildingConfiguration>();
//            _buildingConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _buildingConfigurationController.SaveBuildingConfigurationForProject(projectId, requestObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);


//        }

//        /// <summary>
//        /// UpdateBuildingConfigurationForProjectController
//        /// </summary>
//        /// <param name="projectId"></param>
//        /// <param name="buildingId"></param>
//        /// <param name="UserId"></param>
//        /// <param name="buildingName"></param>
//        [TestCaseSource(nameof(InputDataForUpdateBuildingConfiguration))]
//        public void UpdateBuildingConfigurationForProjectController(string requestBody, string projectId, int buildingId, string buildingName)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            //var requestObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _buildingConfigurationController.UpdateBuildingConfigurationForProject(projectId, buildingId, jObject);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// UpdateBuildingConfigurationForProjectControllerInput
//        /// </summary>
//        /// <param name="projectId"></param>
//        /// <param name="buildingId"></param>
//        /// <param name="UserId"></param>
//        /// <param name="buildingName"></param>
//        [TestCaseSource(nameof(InputDataForUpdateBuildingConfiguration))]
//        public void UpdateBuildingConfigurationForProjectControllerInput(string requestBody, string projectId, int buildingId  , string buildingName)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            //var requestObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _buildingConfigurationController.UpdateBuildingConfigurationForProject(projectId, buildingId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        /// <summary>
//        /// UpdateBuildingConfigurationForProjectControllerError
//        /// </summary>
//        /// <param name="projectId"></param>
//        /// <param name="buildingId"></param>
//        /// <param name="UserId"></param>
//        /// <param name="buildingName"></param>
//        //[TestCaseSource(nameof(InputDataForUpdateBuildingConfigurationError))]
//        //public void UpdateBuildingConfigurationForProjectControllerError(string requestBody, string projectId, int buildingId, string buildingName)
//        //{
//        //    var jObject = JObject.Parse(File.ReadAllText(requestBody));
//        //    //var requestObject = jObject.ToObject<ConfigurationRequest>();
//        //    var response = _buildingConfigurationController.UpdateBuildingConfigurationForProject(projectId, buildingId, jObject);
//        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);

           
//        //}

//        [TestCaseSource(nameof(InputDataForUpdateBuildingConfigurationError))]
//        public void UpdateBuildingConfigurationForProjectControllerModelStateError(string requestBody, string projectId, int buildingId, string buildingName)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            //var requestObject = jObject.ToObject<ConfigurationRequest>();
//            _buildingConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _buildingConfigurationController.UpdateBuildingConfigurationForProject(projectId, buildingId, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);


//        }

//        /// <summary>
//        /// Saving the Building ElevationController
//        /// </summary>
//        /// <param name="buildingElevation"></param>
//        [TestCaseSource(nameof(InputDataForSavingBuildingElevation))]
//        public void SaveBuildingElevationController(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
//            var response = _buildingConfigurationController.SaveBuildingElevation(buildingElevationObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        /// <summary>
//        /// Saving the Building ElevationController
//        /// </summary>
//        /// <param name="buildingElevation"></param>
//        [TestCaseSource(nameof(InputDataForSavingBuildingElevationInternalError))]
//        public void SaveBuildingElevationControllerInternalError(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
//            var response = _buildingConfigurationController.SaveBuildingElevation(buildingElevationObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.INTERNALSERVERERROR);

           
//        }

//        [TestCaseSource(nameof(InputDataForSavingBuildingElevation))]
//        public void SaveBuildingElevationControllerModelStateError(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
//            _buildingConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _buildingConfigurationController.SaveBuildingElevation(buildingElevationObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        /// <summary>
//        /// Update the Building ElevationController
//        /// </summary>
//        /// <param name="buildingElevation"></param>
//        [TestCaseSource(nameof(InputDataForUpdateBuildingElevation))]
//        public void UpdateBuildingElevationController(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
//            var response = _buildingConfigurationController.UpdateBuildingElevation(buildingElevationObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        /// <summary>
//        /// Update the Building ElevationControllerError
//        /// </summary>
//        /// <param name="buildingElevation"></param>
//        [TestCaseSource(nameof(InputDataForUpdateBuildingElevationError))]
//        public void UpdateBuildingElevationControllerError(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
//            var response = _buildingConfigurationController.UpdateBuildingElevation(buildingElevationObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputDataForUpdateBuildingElevation))]
//        public void UpdateBuildingElevationControllerModelStateError(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
//            _buildingConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _buildingConfigurationController.UpdateBuildingElevation(buildingElevationObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        /// <summary>
//        /// Get Building Elevation Details By BuildingIdController
//        /// </summary>
//        /// <param name="buildingId"></param>
//        [TestCaseSource(nameof(InputBuildingIdForBuildingElevation))]
//        public void GetBuildingElevationByIdController(int buildingId)
//        {
//            var response = _buildingConfigurationController.GetBuildingElevationById(buildingId);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// Get Building Elevation Details By BuildingId
//        /// </summary>
//        [Test]
//        public void GetBuildingElevationById()
//        {
//            var response = _buildingConfigurationController.GetBuildingElevationById(1);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);

//        }

//        /// <summary>
//        /// Get Building Elevation Details By BuildingIdError
//        /// </summary>
//        [Test]
//        public void GetBuildingElevationByIdError()
//        {
//            var response = _buildingConfigurationController.GetBuildingElevationById(0);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void GetBuildingElevationByIdModelStateError()
//        {
//            _buildingConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _buildingConfigurationController.GetBuildingElevationById(0);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }
//        /// <summary>
//        /// Delete Building configuration By BuildingIdController
//        /// </summary>
//        /// <param name="buildingId"></param>
//        [TestCaseSource(nameof(InputBuildingIdToDelete))]
//        public void DeleteBuildingConfigurationByIdController(int buildingConfigurationId)
//        {
//            var response = _buildingConfigurationController.DeleteBuildingConfigurationById(buildingConfigurationId);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// Delete Building configuration By BuildingIdControllerInput
//        /// </summary>
//        /// <param name="buildingId"></param>
//        [TestCaseSource(nameof(InputBuildingIdToDelete))]
//        public void DeleteBuildingConfigurationByIdControllerInput(int buildingConfigurationId)
//        {
//            var response = _buildingConfigurationController.DeleteBuildingConfigurationById(buildingConfigurationId);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);

           
//        }

//        /// <summary>
//        /// Delete Building configuration By BuildingIdController
//        /// </summary>
//        /// <param name="buildingId"></param>
//        [TestCaseSource(nameof(InputBuildingIdToDeleteError))]
//        public void DeleteBuildingConfigurationByIdControllerError(int buildingConfigurationId)
//        {
//            var response = _buildingConfigurationController.DeleteBuildingConfigurationById(buildingConfigurationId);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [TestCaseSource(nameof(InputBuildingIdToDeleteError))]
//        public void DeleteBuildingConfigurationByIdControllerModelStateError(int buildingConfigurationId)
//        {
//            _buildingConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _buildingConfigurationController.DeleteBuildingConfigurationById(buildingConfigurationId);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        // <summary>
//        /// DeleteBuildingElevationById
//        /// </summary>
//        [Test]
//        public void DeleteBuildingElevationById()
//        {
//            var response = _buildingConfigurationController.DeleteBuildingElevationById(15);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        // <summary>
//        /// DeleteBuildingElevationById
//        /// </summary>
//        [Test]
//        public void DeleteBuildingElevationByIdInput()
//        {
//            var response = _buildingConfigurationController.DeleteBuildingElevationById(5);
//            Assert.IsNotNull(response);

//        }


//        /// <summary>
//        /// DeleteBuildingElevationByIdError
//        /// </summary>
//        [Test]
//        public void DeleteBuildingElevationByIdError()
//        {
//            var response = _buildingConfigurationController.DeleteBuildingElevationById(0);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void DeleteBuildingElevationByIdModelStateError()
//        {
//            _buildingConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _buildingConfigurationController.DeleteBuildingElevationById(0);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }
//        /// <summary>
//        /// StartBuildingConfigure
//        /// </summary>
//        /// <param name="requestbody"></param>

//        [TestCaseSource(nameof(InputDataForStartBuildingConfigure))]
//        public void StartBuildingConfigure(string projectid, string requestbody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestbody));
//            //var requestObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _buildingConfigurationController.StartBuildingConfigure(projectid, 1, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForStartBuildingConfigure))]
//        public void StartBuildingConfigureErrorTest(string projectid, string requestbody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestbody));
//            //var requestObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _buildingConfigurationController.StartBuildingConfigure(projectid, 0, jObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForStartBuildingConfigure))]
//        public void StartBuildingConfigureModelStateError(string projectid, string requestbody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestbody));
//            _buildingConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _buildingConfigurationController.StartBuildingConfigure(projectid, 1, jObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        /// <summary>
//        /// ChangeBuildingConfigure
//        /// </summary>
//        /// <param name="requestbody"></param>

//        [TestCaseSource(nameof(InputDataForChangeBuildingConfigure))]
//        public void ChangeBuildingConfigure(string requestbody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestbody));
//            //var requestObject = jObject.ToObject<ConfigurationRequest>();
//            var response = _buildingConfigurationController.ChangeBuildingConfigure(jObject);
//            Assert.IsNotNull(response);
//        }


//        /// <summary>
//        /// AutoSaveBuildingElevation
//        /// </summary>
//        /// <param name="requestBody"></param>
//        [TestCaseSource(nameof(InputDataForSavingBuildingElevation))]
//        public void AutoSaveBuildingElevation(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
//            var response = _buildingConfigurationController.AutoSaveBuildingElevation(buildingElevationObject);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// AutoSaveBuildingElevationError
//        /// </summary>
//        /// <param name="requestBody"></param>
//        [Test]
//        public void AutoSaveBuildingElevationError()
//        {
//            var response = _buildingConfigurationController.AutoSaveBuildingElevation(null);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void AutoSaveBuildingElevationModelStateError()
//        {
//            _buildingConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _buildingConfigurationController.AutoSaveBuildingElevation(null);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// Delete Building configuration By BuildingIdError
//        /// </summary>
//        [Test]
//        public void DeleteBuildingConfigurationByIdControllerError()
//        {
//            var response = _buildingConfigurationController.DeleteBuildingConfigurationById(0);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// SaveBuildingConfigurationForProjectInternalError
//        /// </summary>
//        [Test]
//        public void SaveBuildingConfigurationForProjectInternalError()
//        {
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEBUILDINGREQUESTBODY));
//            var requestObject = jObject.ToObject<BuildingConfiguration>();
//            var response = _buildingConfigurationController.SaveBuildingConfigurationForProject("15", requestObject);
//            Assert.IsNotNull(response);
//        }

//        ///// <summary>
//        ///// Saving the Building ElevationControllerError
//        ///// </summary>
//        //[Test]
//        //public void SaveBuildingElevationControllerError()
//        //{
//        //    var response = _buildingConfigurationController.SaveBuildingElevation(null);
//        //    Assert.IsNotNull(response);
//        //}

//        //[Test]
//        //public void SaveBuildingElevationControllerModelStateError()
//        //{
//        //    _buildingConfigurationController.ModelState.AddModelError("test", "test");
//        //    var response = _buildingConfigurationController.SaveBuildingElevation(null);
//        //    Assert.IsNotNull(response);
//        //}
//        /// <summary>
//        /// UpdateBuildingConfigurationForProjectError
//        /// </summary>
//        [Test]
//        public void UpdateBuildingConfigurationForProjectError()
//        { 
//            var response = _buildingConfigurationController.UpdateBuildingConfigurationForProject("15", 1, null);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// UpdateBuildingElevationError
//        /// </summary>
//        [Test]
//        public void UpdateBuildingElevationError()
//        {
//            var response = _buildingConfigurationController.UpdateBuildingElevation(null);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void UpdateBuildingElevationModelStateError()
//        {
//            _buildingConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _buildingConfigurationController.UpdateBuildingElevation(null);
//            Assert.IsNotNull(response);
//        }


//        /// <summary>
//        /// Saving the Building ElevationController
//        /// </summary>
//        /// <param name="buildingElevation"></param>
//        [TestCaseSource(nameof(InputDataForSavingBuildingElevationError))]
//        public void SaveBuildingElevationControllerError(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
//            var response = _buildingConfigurationController.SaveBuildingElevation(buildingElevationObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);


//        }

//        /// <summary>
//        /// StartBuildingConfigureError
//        /// </summary>
//        [Test]
//        //public void StartBuildingConfigureError()
//        //{
//        //    var response = _buildingConfigurationController.StartBuildingConfigure("", "", "", null);
//        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        //}

//        /// <summary>
//        /// Update the Building ElevationControllerInternalError
//        /// </summary>
//        /// <param name="buildingElevation"></param>
//        [TestCaseSource(nameof(InputDataForUpdateBuildingElevationInternalError))]
//        public void UpdateBuildingElevationControllerInternalError(string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
//            var response = _buildingConfigurationController.UpdateBuildingElevation(buildingElevationObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.INTERNALSERVERERROR);
//        }

//        [TestCaseSource(nameof(InputDataForStartBuildingConfigure))]
//        public void ResetBuildingController(string projectId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _buildingConfigurationController.ResetBuildingConfigure(projectId, 1);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForStartBuildingConfigure))]
//        public void ResetBuildingControllerModelStateError(string projectId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            _buildingConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _buildingConfigurationController.ResetBuildingConfigure(projectId, 1);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void QuickConfigurationSummary()
//        {
//            var response = _buildingConfigurationController.QuickConfigurationSummary("building", "1");
//            // Assert.IsNotNull((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void QuickConfigurationSummaryProject()
//        {
//            var response = _buildingConfigurationController.QuickConfigurationSummary("opportunity", "Id");
//            // Assert.IsNotNull((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void QuickConfigurationSummaryGroup()
//        {
//            var response = _buildingConfigurationController.QuickConfigurationSummary("group", "1");
//            // Assert.IsNotNull((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void QuickConfigurationSummaryUnit()
//        {
//            var response = _buildingConfigurationController.QuickConfigurationSummary("set", "1");
//            // Assert.IsNotNull((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void GetBuildingConfigurationSectionTab()
//        {
//            var response = _buildingConfigurationController.GetBuildingConfigurationSectionTab(1);
//            Assert.AreEqual(200, (response.Result as ObjectResult).StatusCode);
//        }

//        [Test]
//        public void GetBuildingConfigurationSectionTabModelStateError()
//        {
//            _buildingConfigurationController.ModelState.AddModelError("model", "model");
//            var response = _buildingConfigurationController.GetBuildingConfigurationSectionTab(1);
//            Assert.AreEqual(400, (response.Result as ObjectResult).StatusCode);
//        }

//        [Test]
//        public void GetBuildingConfigurationSectionTabError()
//        {
//            var response = _buildingConfigurationController.GetBuildingConfigurationSectionTab(0);
//            Assert.AreEqual(400, (response.Result as ObjectResult).StatusCode);
//        }

//        [Test]
//        public void QuickConfigurationSummaryModelError()
//        {
//            _buildingConfigurationController.ModelState.AddModelError("model", "model");
//            var response = _buildingConfigurationController.QuickConfigurationSummary("building", "1");
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void ResetBuildingErrorTest()
//        {
//            var response = _buildingConfigurationController.ResetBuildingConfigure("projectId", 0);
//            Assert.AreEqual(400, (response.Result as ObjectResult).StatusCode);
//        }
//    }
//}
