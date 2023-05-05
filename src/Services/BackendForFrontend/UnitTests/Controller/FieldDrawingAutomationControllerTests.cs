//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
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
//    public class FieldDrawingAutomationControllerTests
//    {

//        #region 

//        private FieldDrawingAutomationController _fieldDrawingAutomation;
//        private ILogger<FieldDrawingAutomationController> _fieldDrawingAutomationControllerLogger;
//        private IConfigure _configure;


//        #endregion

//        #region PrivateMethods
//        /// <summary>
//        /// InitialConfigurationSetup
//        /// </summary>
//        [SetUp, Order(1)]
//        [Obsolete]
//        public void InitialConfigurationSetup()
//        {
//            CommonFunctions.InitialConfiguration();
//            var services = CommonFunctions.ServiceCollection();
//            _fieldDrawingAutomationControllerLogger = services.BuildServiceProvider().GetService<ILogger<FieldDrawingAutomationController>>();

//            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();


//            var iGroup = (IFieldDrawingAutomation)servicesProvider.GetService(typeof(IFieldDrawingAutomation));
//            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
//            var iConfiguration = (IConfiguration)servicesProvider.GetService(typeof(IConfiguration));
 


//            _fieldDrawingAutomation = new FieldDrawingAutomationController(_fieldDrawingAutomationControllerLogger, iGroup, iConfigure, iConfiguration);
//            _fieldDrawingAutomation.ControllerContext = new ControllerContext();
//            _fieldDrawingAutomation.ControllerContext.HttpContext = new DefaultHttpContext();
//            _fieldDrawingAutomation.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
//        }
//        #endregion

//        #region Setup Input Values
//        public static IEnumerable<TestCaseData> InputDataForChangeFieldDrawingAutomation()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.FIELDDRAWINGAUTOMATIONSTUB);
//        }
//        public static IEnumerable<TestCaseData> InputDataForSaveSendToCoordination()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.SAVESENTTOCOORDINATIONREQUESTLAYOUTJSON);
//        }
//        #endregion

//        /// <param name="configureRequest"></param>
//        /// <param name="buildingId"></param>
//        /// 
//        [Test]
//        public void StartFieldDrawingConfigure()
//        {
            
//            var response = _fieldDrawingAutomation.StartFieldDrawingConfigure(1);
         
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void StartFieldDrawingConfigureErrorTest()
//        {

//            var response = _fieldDrawingAutomation.StartFieldDrawingConfigure(-1);

//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void StartFieldDrawingConfigureModelStateErrorTest()
//        {
//            _fieldDrawingAutomation.ModelState.AddModelError("test", "test");
//            var response = _fieldDrawingAutomation.StartFieldDrawingConfigure(-1);

//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForChangeFieldDrawingAutomation))]
//        public void ChangeFieldDrawingConfigure(string requestBody)
//        {
//            var Jobject = JObject.Parse(File.ReadAllText(requestBody));


//            var response = _fieldDrawingAutomation.ChangeFieldDrawingConfigure(Jobject, 1);

//            Assert.IsNotNull(response);
//        }
        
//        [TestCaseSource(nameof(InputDataForChangeFieldDrawingAutomation))]
//        public void ChangeFieldDrawingConfigureErrorTest(string requestBody)
//        {
//            var Jobject = JObject.Parse(File.ReadAllText(requestBody));

//            var response = _fieldDrawingAutomation.ChangeFieldDrawingConfigure(Jobject, -1);

//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForChangeFieldDrawingAutomation))]
//        public void ChangeFieldDrawingConfigureModelStateErrorTest(string requestBody)
//        {
//            var Jobject = JObject.Parse(File.ReadAllText(requestBody));
//            _fieldDrawingAutomation.ModelState.AddModelError("test", "test");
//            var response = _fieldDrawingAutomation.ChangeFieldDrawingConfigure(Jobject, -1);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void GetFieldDrawingsByProjectId()
//        {
            
//            var response = _fieldDrawingAutomation.GetFieldDrawingsByProjectId("xyz");
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void GetFieldDrawingsByProjectIdErrorTest()
//        {

//            var response = _fieldDrawingAutomation.GetFieldDrawingsByProjectId("");
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void GetFieldDrawingsByProjectIdModelStateErrorTest()
//        {
//            _fieldDrawingAutomation.ModelState.AddModelError("test", "test");
//            var response = _fieldDrawingAutomation.GetFieldDrawingsByProjectId("");
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void GetRequestQueueByGroupId()
//        {
           
//            var response = _fieldDrawingAutomation.GetRequestQueueByGroupId(1);
           
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void GetRequestQueueByGroupIdErrorTest()
//        {

//            var response = _fieldDrawingAutomation.GetRequestQueueByGroupId(-1);

//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void GetRequestQueueByGroupIdModelStateErrorTest()
//        {
//            _fieldDrawingAutomation.ModelState.AddModelError("test", "test");
//            var response = _fieldDrawingAutomation.GetRequestQueueByGroupId(1);

//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForChangeFieldDrawingAutomation))]
//        public void SaveFieldDrawingConfigure(string requestBody)
//        {
//            var Jobject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _fieldDrawingAutomation.SaveFieldDrawingConfigure(null,1,null);

//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForChangeFieldDrawingAutomation))]
//        public void SaveFieldDrawingConfigureErrorTest(string requestBody)
//        {
//            var Jobject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _fieldDrawingAutomation.SaveFieldDrawingConfigure(Jobject,-1, "projectId");

//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForChangeFieldDrawingAutomation))]
//        public void SaveFieldDrawingConfigureModelStateErrorTest(string requestBody)
//        {
//            var Jobject = JObject.Parse(File.ReadAllText(requestBody));
//            _fieldDrawingAutomation.ModelState.AddModelError("model", "model");
//            var response = _fieldDrawingAutomation.SaveFieldDrawingConfigure(Jobject, -1, "");

//            Assert.IsNotNull(response);
//        }


//        [Test]
//        public void GetSendToCoordinationByProjectId()
//        {

//            var response = _fieldDrawingAutomation.GetSendToCoordinationByProjectId(Constant.PROJECTID);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }
//        [Test]
//        public void GetSendToCoordinationByProjectIdErrorTest()
//        {

//            var response = _fieldDrawingAutomation.GetSendToCoordinationByProjectId(Constant.SPACE);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void GetSendToCoordinationByProjectIdModelStateErrorTest()
//        {
//            _fieldDrawingAutomation.ModelState.AddModelError(Constant.TEST, Constant.TEST);
//            var response = _fieldDrawingAutomation.GetSendToCoordinationByProjectId(Constant.SPACE);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }


//        [TestCaseSource(nameof(InputDataForSaveSendToCoordination))]
//        public void SaveSendToCoordination(string requestBody)
//        {
//            var requestObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _fieldDrawingAutomation.SaveSendToCoordination(requestObject, Constant.PROJECTID);
//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForSaveSendToCoordination))]
//        public void SaveSendToCoordinationErrorTest(string requestBody)
//        {
//            var requestObject = JObject.Parse(File.ReadAllText(requestBody));
//            var response = _fieldDrawingAutomation.SaveSendToCoordination(requestObject, Constant.SPACE);

//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }
//        [TestCaseSource(nameof(InputDataForSaveSendToCoordination))]
//        public void SaveSendToCoordinationModelStateErrorTest(string requestBody)
//        {
//            var requestObject = JObject.Parse(File.ReadAllText(requestBody));
//            _fieldDrawingAutomation.ModelState.AddModelError(Constant.MODEL, Constant.MODEL);
//            var response = _fieldDrawingAutomation.SaveSendToCoordination(requestObject, Constant.SPACE);

//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }
//    }
//}


 