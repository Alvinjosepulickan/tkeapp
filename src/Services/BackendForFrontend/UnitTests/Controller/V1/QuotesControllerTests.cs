using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.Controllers;
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.BFF.Test.Common;
using TKE.SC.BFF.APIController;
using TTKE.SC.BFF.Api.V1;
using System.Security.Claims;
using TKE.SC.BFF.Test.BusinessProcess.Helper;

namespace TKE.SC.BFF.UnitTests.Controller.V1
{
    public class QuotesControllerTests
    {
        #region 

        private QuotesController _quotesController;
        private ILogger<QuotesController> __quotesControllerLogger;

        #endregion

        #region privatemethods
        /// <summary>
        /// initialconfigurationsetup
        /// </summary>
        [SetUp, Order(1)]
        public void Initialconfigurationsetup()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            __quotesControllerLogger = services.BuildServiceProvider().GetService<ILogger<QuotesController>>();

            var servicesprovider = services.BuildServiceProvider().GetService<IServiceProvider>();

            var iproject = (IProject)servicesprovider.GetService(typeof(IProject));
            var iBuildingConfiguration = (IBuildingConfiguration)servicesprovider.GetService(typeof(IBuildingConfiguration));
            var iFieldDrawingAutomation = (IFieldDrawingAutomation)servicesprovider.GetService(typeof(IFieldDrawingAutomation));
            var iOzBl = (IOzBL)servicesprovider.GetService(typeof(IOzBL));
            var iReleaseInfo = (IReleaseInfo)servicesprovider.GetService(typeof(IReleaseInfo));

            _quotesController = new QuotesController(iBuildingConfiguration,iproject, iOzBl, iFieldDrawingAutomation, __quotesControllerLogger, iReleaseInfo);
            _quotesController.ControllerContext = new ControllerContext();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("SessionId","SessionId")
                                   }, "TestAuthentication"));
            _quotesController.ControllerContext.HttpContext = new DefaultHttpContext() {User=user};
            _quotesController.ControllerContext.HttpContext.Items["sessionid"] = "sessionidvalue";
        }
        #endregion
        public static IEnumerable<TestCaseData> InputBuildingValues()
        {
            yield return new TestCaseData(15);
        }
        public static IEnumerable<TestCaseData> InputDataForChangeFieldDrawingAutomation()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.FIELDDRAWINGAUTOMATIONSTUB);
        }
        public static IEnumerable<TestCaseData> InputDataForSaveSendToCoordination()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVESENTTOCOORDINATIONREQUESTLAYOUTJSON);
        }
        


        //GetListOfBuildingsForProjectController
        //</summary>
        //<param name = "projectCode" ></ param >
        [Test]
        public void GetListOfBuildingsForQuotesController()
        {
            var response = _quotesController.GetListOfConfigurationForProject("15");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["permissions"][0].ToString().Trim();
            Assert.AreEqual(key, "Key_derived");
        }
         
        [Test]
        public void GetListOfBuildingsForQuotesInput()
        {
            var response = _quotesController.GetListOfConfigurationForProject("1");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["permissions"][0].ToString().Trim();
            Assert.AreEqual(key, "Key_derived");
        }

        /// <summary>
        /// GetListOfBuildingsForProject
        /// </summary>
        [Test]
        public void GetListOfBuildingsForQuotes()
        {
            var response = _quotesController.GetListOfConfigurationForProject("15");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["permissions"][0].ToString().Trim();
            Assert.AreEqual(key, "Key_derived");
        }

        /// <summary>
        /// GetListOfBuildingsForProjectError
        /// </summary>
        [Test]
        public void GetListOfBuildingsForQuotesError()
        {
            var response = _quotesController.GetListOfConfigurationForProject("");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [Test]
        public void GetListOfBuildingsForQuotesModelStateError()
        {
            _quotesController.ModelState.AddModelError("test", "test");
            var response = _quotesController.GetListOfConfigurationForProject("");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        //
        //QuickConfigurationSummary
        [Test]
        public void QuickConfigurationSummary()
        {
            var response = _quotesController.QuickConfigurationSummary("building", "1");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["building"]["id"].ToString().Trim();
            Assert.AreEqual(key, "1");
        }

        [Test]
        public void QuickConfigurationSummaryProject()
        {
            var response = _quotesController.QuickConfigurationSummary("opportunity", "Id");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS); 
        }

        [Test]
        public void QuickConfigurationSummaryGroup()
        {
            var response = _quotesController.QuickConfigurationSummary("group", "1");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);   
        }

        [Test]
        public void QuickConfigurationSummaryUnit()
        {
            var response = _quotesController.QuickConfigurationSummary("set", "1");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
        }
        //QuickConfigurationSummary End




        [Test] //To be implemented
        public void GetFieldDrawingsByProjectId()
        {

            var response = _quotesController.GetFieldDrawingsByProjectId("xyz");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode,Constant.SUCCESS);
        }

        [Test]
        public void GetFieldDrawingsByProjectIdErrorTest()
        {

            var response = _quotesController.GetFieldDrawingsByProjectId("");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
        }

        [Test] //To be implemented
        public void GetFieldDrawingsByProjectIdModelStateErrorTest()
        {
            _quotesController.ModelState.AddModelError("test", "test");
            var response = _quotesController.GetFieldDrawingsByProjectId("");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
        }


        [Test] //To be implemented
        public void GetSendToCoordinationByProjectId()
        {

            var response = _quotesController.GetSendToCoordinationByProjectId(Constant.PROJECTID);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
        }

        [Test] //To be implemented
        public void GetSendToCoordinationByProjectIdErrorTest()
        {

            var response = _quotesController.GetSendToCoordinationByProjectId(Constant.SPACE);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [Test] //To be implemented
        public void GetSendToCoordinationByProjectIdModelStateErrorTest()
        {
            _quotesController.ModelState.AddModelError(Constant.TEST, Constant.TEST);
            var response = _quotesController.GetSendToCoordinationByProjectId(Constant.SPACE);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }


        [TestCaseSource(nameof(InputDataForSaveSendToCoordination))] //To be implemented
        public void SaveSendToCoordination(string requestBody)
        {
            var requestObject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _quotesController.SaveSendToCoordination(requestObject, Constant.PROJECTID);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode,Constant.SUCCESS);
        }

        [TestCaseSource(nameof(InputDataForSaveSendToCoordination))] //to be implemented
        public void SaveSendtoCoordinationErrorTest(string requestbody)
        {
            var requestobject = JObject.Parse(File.ReadAllText(requestbody));
            var response = _quotesController.SaveSendToCoordination(requestobject, Constant.SPACE);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [TestCaseSource(nameof(InputDataForSaveSendToCoordination))] //to be implemented
        public void SaveSendtoCoordinationModelStateErrorTest(string requestbody)
        {
            var requestobject = JObject.Parse(File.ReadAllText(requestbody));
            _quotesController.ModelState.AddModelError(Constant.MODEL,Constant.MODEL);
            var response = _quotesController.SaveSendToCoordination(requestobject, Constant.SPACE);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [Test] 
        public void BookingRequest()
        {
            var response = _quotesController.BookingRequest("SC-1");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode,Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["Description"].ToString().Trim();
            Assert.AreEqual(key, "The Book coordination saved");
        }

        [Test]
        public void BookingRequestError()
        {
            var response = _quotesController.BookingRequest("0");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [Test]
        public void BookingRequestModelError()
        {
            _quotesController.ModelState.AddModelError(Constant.TEST, Constant.TEST);
            var response = _quotesController.BookingRequest("0");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }



        //
        //GetSendToCoordinationStatus
        //
        [Test]
        public void GetSendToCoordinationStatus()
        {
            var response = _quotesController.GetSendToCoordinationStatus("15");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode,Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["CoordinationStatus"]["StatusId"].ToString().Trim();
            Assert.AreEqual(key, "15");
        }

        [Test]
        public void GetSendToCoordinationStatusError()
        {
            var response = _quotesController.GetSendToCoordinationStatus(string.Empty);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [Test]
        public void GetSendToCoordinationStatusModelError()
        {
            var response = _quotesController.GetSendToCoordinationStatus(string.Empty);
            _quotesController.ModelState.AddModelError(Constant.MODEL, Constant.MODEL);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        /// <summary>
        /// SaveConfigurationToView
        /// </summary>
        [Test]
        public void SaveConfigurationToExternalSystem()
        {
            var response = _quotesController.SaveConfigurationToExternalSystem("20");
            Assert.IsNotNull(response);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["Message"].ToString().Trim(); 
            Assert.AreEqual(key, "The project is saved successfully");
        }

        /// <summary>
        /// SaveConfigurationToViewError
        /// </summary>
        [Test]
        public void SaveConfigurationToExternalSystemError()
        {
            var response = _quotesController.SaveConfigurationToExternalSystem("");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        /// <summary>
        /// SaveConfigurationToExternalSystemModelError
        /// </summary>
        [Test]
        public void SaveConfigurationToExternalSystemModelError()
        {
            _quotesController.ModelState.AddModelError("test", "test");
            var response = _quotesController.SaveConfigurationToExternalSystem("");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }


        /// <summary>
        /// SaveConfigurationToView
        /// </summary>
        [Test] //To be implemented all three
        public void GenerateViewConfigurationRequest()
        {
            var response = _quotesController.GenerateViewConfigurationRequest("25");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode,Constant.SUCCESS);
        }

        /// <summary>
        /// SaveConfigurationToViewError
        /// </summary>
        [Test]
        public void GenerateViewConfigurationRequestError()
        {
            var response = _quotesController.GenerateViewConfigurationRequest("");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS); ;
        }

        [Test]
        public void GenerateViewConfigurationRequestModelError()
        {
            _quotesController.ModelState.AddModelError("test", "test");
            var response = _quotesController.GenerateViewConfigurationRequest("");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS); ;
        }

        [Test]
        public void GenerateOzRequestBodyTest()
        {
            var response = _quotesController.GenerateOzRequestBody("SC-36");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode,Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["Equipment"][0]["EstimateIdentifier"]["LineId"].ToString().Trim();
            Assert.AreEqual(key, "50");
        }

        [Test]
        public void GenerateOzRequestBodyErrorTest()
        {
            var response = _quotesController.GenerateOzRequestBody("36");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [Test]
        public void GenerateOzRequestBodyModelErrorTest()
        {
            _quotesController.ModelState.AddModelError("test", "test");
            var response = _quotesController.GenerateOzRequestBody("36");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [Test]
        public void GetReleaseInfoTest() 
        {
            var response = _quotesController.GetReleaseInfo("30");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["GroupDetailsForReleaseInfo"][0]["BuildingId"].ToString().Trim();
            Assert.AreEqual(key, "30");
        }

        [Test]
        public void GetReleaseInfoTestError()
        {
            var response = _quotesController.GetReleaseInfo("");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [Test]
        public void GetReleaseInfoTestModelError()
        {
            _quotesController.ModelState.AddModelError("test", "test");
            var response = _quotesController.GetReleaseInfo("");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [Test]
        public void GetReleaseToManufactureByGroupId()
        {
            var response = _quotesController.GetReleaseToManufactureByGroupId(25);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["Permissions"][0].ToString().Trim();
            Assert.AreEqual(key, "Elevator_Key");
        }

        [Test]
        public void GetReleaseToManufactureByGroupIdError()
        {
            var response = _quotesController.GetReleaseToManufactureByGroupId(45);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [Test]
        public void GetReleaseToManufactureByGroupIdModelError()
        {
            _quotesController.ModelState.AddModelError("test", "test");
            var response = _quotesController.GetReleaseToManufactureByGroupId(45);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [Test]
        public void SaveUpdatReleaseInfoDetails()
        {
            var text = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.VARIABLEASSIGNMENTREQUESTPAYLOAD));
            var response = _quotesController.SaveUpdatReleaseInfoDetails(15, text);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["Message"].ToString().Trim();
            Assert.AreEqual(key, "Release Info Updated");
        }
    }
}
