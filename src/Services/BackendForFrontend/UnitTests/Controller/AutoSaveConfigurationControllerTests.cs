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
//using TKE.SC.Common.Model.UIModel;
//using TKE.SC.Common.Model.ViewModel;
//using TKE.SC.BFF.Test.Common;

//namespace TKE.SC.BFF.Test.Controller
//{
//    public class AutoSaveConfigurationControllerTests
//    {
//        #region 

//        private AutoSaveConfigurationController _autoSaveConfigurationController;
//        private ILogger<AutoSaveConfigurationController> _autoSaveConfigurationControllerLogger;

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
//            _autoSaveConfigurationControllerLogger = services.BuildServiceProvider().GetService<ILogger<AutoSaveConfigurationController>>();

//            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();

//            var iAutoSaveConfiguration = (IAutoSaveConfiguration) servicesProvider.GetService(typeof(IAutoSaveConfiguration));
//            var iConfigure = (IConfigure) servicesProvider.GetService(typeof(IConfigure));



//            _autoSaveConfigurationController = new AutoSaveConfigurationController(_autoSaveConfigurationControllerLogger, iAutoSaveConfiguration, iConfigure);
//            _autoSaveConfigurationController.ControllerContext = new ControllerContext();
//            _autoSaveConfigurationController.ControllerContext.HttpContext = new DefaultHttpContext();
//            _autoSaveConfigurationController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
//        }
//        #endregion


//        /// <summary>
//        /// AutoSaveConfiguration
//        /// </summary>
//        [Test]
//        public void AutoSaveConfiguration()
//        {
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.AUTOSAVEBUILDINGCONFIGREQUESTBODY));
//            var requestObject = jObject.ToObject<AutoSaveConfiguration>();
//            var response = _autoSaveConfigurationController.AutoSaveConfiguration(requestObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }

//        /// <summary>Error
//        /// </summary>
//        [Test]
//        public void AutoSaveConfigurationError()
//        {
//            var response = _autoSaveConfigurationController.AutoSaveConfiguration(null);
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void AutoSaveConfigurationModelStateError()
//        {
//            _autoSaveConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _autoSaveConfigurationController.AutoSaveConfiguration(null);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// DeleteAutoSaveConfigurationByUser
//        /// </summary>
//        [Test]
//        public void DeleteAutoSaveConfigurationByUser()
//        {
//            var response = _autoSaveConfigurationController.DeleteAutoSaveConfigurationByUser();
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        }


//        /// <summary>
//        /// DeleteAutoSaveConfigurationByUserError
//        /// </summary>
//        //[Test]
//        //public void DeleteAutoSaveConfigurationByUserError()
//        //{
//        //    var response = _autoSaveConfigurationController.DeleteAutoSaveConfigurationByUser();
//        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        //}

//        [Test]
//        public void DeleteAutoSaveConfigurationByUserModelStateError()
//        {
//            _autoSaveConfigurationController.ModelState.AddModelError("test", "test");
//            var response = _autoSaveConfigurationController.DeleteAutoSaveConfigurationByUser();
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// GetAutoSaveConfigurationByUser
//        /// </summary>
//        [Test]
//        public void GetAutoSaveConfigurationByUser()
//        {
//            var response = _autoSaveConfigurationController.GetAutoSaveConfigurationByUser();
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// GetAutoSaveConfigurationByUserError
//        /// </summary>
//        //[Test]
//        //public void GetAutoSaveConfigurationByUserError()
//        //{
//        //    var response = _autoSaveConfigurationController.GetAutoSaveConfigurationByUser();
//        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        //}

//        /// <summary>
//        /// GetAutoSaveConfigurationByUserError
//        /// </summary>
//        [Test]
//        public void GetAutoSaveConfigurationByUserModelStateError()
//        {
//            _autoSaveConfigurationController.ModelState.AddModelError("test","test");
//            var response = _autoSaveConfigurationController.GetAutoSaveConfigurationByUser();
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }
//    }
//}