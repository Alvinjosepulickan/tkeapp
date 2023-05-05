//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using NUnit.Framework;
//using System;
//using TKE.SC.BFF.BusinessProcess.Helpers;
//using TKE.SC.BFF.BusinessProcess.Interfaces;
//using TKE.SC.BFF.Controllers;
//using TKE.SC.BFF.Test.Common;

//namespace TKE.SC.BFF.Tets.Controller
//{
//    public class OzControllerTests
//    {
//        #region Variables
//        private OZController _ozController;
//        private IConfigure _configure;
//        private Utility _utility;
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

//            var iOz = (IOz)services.BuildServiceProvider().GetService<IServiceProvider>().GetService(typeof(IOz));

//            _ozController = new OZController(iOz, _configure, _utility);
//            _ozController.ControllerContext = new ControllerContext();
//            _ozController.ControllerContext.HttpContext = new DefaultHttpContext();
//            _ozController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
//            _configure = (IConfigure)services.BuildServiceProvider().GetService<IServiceProvider>().GetService(typeof(IConfigure));
//        }
//        #endregion
//        #region SetUp Input Values

//        #endregion


//        [Test]
//        public void BookingRequest()
//        {
//            var response = _ozController.BookingRequest("1");
//            Assert.IsNotNull(response);
//        }
//        [Test]
//        public void BookingRequestError()
//        {
//            var response = _ozController.BookingRequest("0");
//            Assert.IsNotNull(response);
//        }
//    }
//}
