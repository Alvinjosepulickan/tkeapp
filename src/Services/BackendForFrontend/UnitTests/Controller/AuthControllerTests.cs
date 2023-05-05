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
//using TKE.SC.Common.Model.RequestModel;
//using TKE.SC.Common.Model.ViewModel;
//using TKE.SC.BFF.Test.Common;
//using System.Text;

//namespace TKE.SC.BFF.Test.Controller
//{
//    public class AuthControllerTests
//    {
//        #region 

//        private AuthController _authController;
//        private ILogger<AuthController> _authCoontrollerLogger;

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
//            _authCoontrollerLogger = services.BuildServiceProvider().GetService<ILogger<AuthController>>();

//            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();

//            var iAuth = (IAuth) servicesProvider.GetService(typeof(IAuth));
//            var iGenerateToken =(IGenerateToken) servicesProvider.GetService(typeof(IGenerateToken));
//            var http=(IHttpContextAccessor)servicesProvider.GetService(typeof(IHttpContextAccessor));
//            //_authController = new AuthController(_authCoontrollerLogger, iAuth, iGenerateToken)
//            //{
//            //    ControllerContext = new ControllerContext
//            //    {
//            //        HttpContext = new DefaultHttpContext { Items = {["SessionId"] = "SessionIdValue"}}
//            //    }
//            //};

//            _authController = new AuthController(_authCoontrollerLogger, iAuth, iGenerateToken,http);
//            _authController.ControllerContext = new ControllerContext();
//            _authController.ControllerContext.HttpContext = new DefaultHttpContext();
//            _authController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
//        }
//        #endregion

//        #region SetUp Input Values

//        public static IEnumerable<TestCaseData> InputUserInfoValues()
//        {
//            yield return new TestCaseData(1041845);
//        }

//        #endregion

//        /// <summary>
//        /// GetUserInfoController
//        /// </summary>
//        /// <param name="userCode"></param>
//        [TestCaseSource(nameof(InputUserInfoValues))]
//        public void GetUserInfoController(int userCode)
//        {
//            var response = _authController.GetUserInfo(userCode);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// GetUserInfo
//        /// </summary>
//        [Test]
//        public void GetUserInfo()
//        {
//            var response = _authController.GetUserInfo(1041845);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// Get UserInfoError
//        /// </summary>
//        [Test]
//        public void GetUserInfoError()
//        {
//            var response = _authController.GetUserInfo(0);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        } 

//        [Test]
//        public void GetUserInfoModelError()
//        {
//            _authController.ModelState.AddModelError("test", "test");
//            var response = _authController.GetUserInfo(10);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void LogOff()
//        {
//            var inputRequest = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.LOGOFFREQUESTBODY));
//            var inputRequestObj = inputRequest.ToObject<GigyaAttributes>();
//            var response = _authController.Logoff(inputRequestObj);
//            Assert.IsNotNull(response);
//        }
//        /// <summary>
//        /// ValidateToken
//        /// </summary>
//        [Test]
//        public void ValidateToken()
//        {
           
//            GigyaAttributes gigyaAttributes = new GigyaAttributes();
//            gigyaAttributes.Persona = "en";
//            gigyaAttributes.Locale = "en";
//            var response = _authController.ValidateToken(gigyaAttributes);
//            Assert.IsNotNull(response);
//        }


//        /// <summary>
//        /// ValidateToken Error
//        /// </summary>
//        [Test]
//        public void ValidateTokenError()
//        {

//            GigyaAttributes gigyaAttributes = new GigyaAttributes();
//            gigyaAttributes = null;
//            var response = _authController.ValidateToken(gigyaAttributes);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void ValidateTokenModelStateError()
//        {

//            GigyaAttributes gigyaAttributes = new GigyaAttributes();
//            gigyaAttributes = null;
//            _authController.ModelState.AddModelError("test", "test");
//            var response = _authController.ValidateToken(gigyaAttributes);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// Logoff
//        /// </summary>
//        [Test]
//        public void Logoff()
//        {
//            GigyaAttributes gigyaAttributes = new GigyaAttributes();
//            gigyaAttributes.Persona = "en";
//            gigyaAttributes.Locale = "en";
//            var response = _authController.Logoff(gigyaAttributes);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// LogoffError
//        /// </summary>
//        [Test]
//        public void LogoffError()
//        {

//            GigyaAttributes gigyaAttributes = new GigyaAttributes();
//            gigyaAttributes = null;
//            var response = _authController.Logoff(gigyaAttributes);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void LogoffModelStateError()
//        {

//            GigyaAttributes gigyaAttributes = new GigyaAttributes();
//            gigyaAttributes = null;
//            _authController.ModelState.AddModelError("test", "test");
//            var response = _authController.Logoff(gigyaAttributes);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void RequestConfiguration()
//        {
//            var inputRequest = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGEGROUPCONFIGREQUESTBODY));
//            var inputRequestObj = inputRequest.ToObject<SublineRequest>();
//            var response = _authController.RequestConfiguration(inputRequestObj);
//            Assert.IsNotNull(response);
//        }
//    }
//}
