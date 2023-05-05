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
//    public class OpeningLocationControllerTests
//    {

//        #region 

//        private OpeningLocationController _openingLocationController;
//        private ILogger<OpeningLocationController> _openingLocationControllerLogger;

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
//            _openingLocationControllerLogger = services.BuildServiceProvider().GetService<ILogger<OpeningLocationController>>();

//            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();

//            var iOpeningLocation = (IOpeningLocation)servicesProvider.GetService(typeof(IOpeningLocation));
//            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));



//            _openingLocationController = new OpeningLocationController(_openingLocationControllerLogger, iOpeningLocation);
//            _openingLocationController.ControllerContext = new ControllerContext();
//            _openingLocationController.ControllerContext.HttpContext = new DefaultHttpContext();
//            _openingLocationController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
//        }
//        #endregion


//        #region SetUp Input Values

//        public static IEnumerable<TestCaseData> InputDataForUpdateOpeningLocation()
//        {
//            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEOPENINGLOCATIONREQUESTBODY, 236);
//        }
//        #endregion

//        /// <summary>
//        /// Update Opening Location
//        /// </summary>
//        /// <param name="groupConfigurationId"></param>
//        /// <param name="openingLocation"></param>
//        [TestCaseSource(nameof(InputDataForUpdateOpeningLocation))]
//        public void UpdateOpeningLocationController(string requestbody, int groupConfigurationId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestbody));
//            var OpeningLocationObject = jObject.ToObject<OpeningLocations>();
//            var response = _openingLocationController.UpdateOpeningLocation(OpeningLocationObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForUpdateOpeningLocation))]
//        public void UpdateOpeningLocationControllerModelStateError(string requestbody, int groupConfigurationId)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestbody));
//            var OpeningLocationObject = jObject.ToObject<OpeningLocations>();
//            _openingLocationController.ModelState.AddModelError("test", "test");
//            var response = _openingLocationController.UpdateOpeningLocation( OpeningLocationObject);
//            Assert.IsNotNull(response);
//        }


//        [Test]
//        public void GetOpeningLocationBygroupId()
//        {
//            var res = _openingLocationController.GetOpeningLocationByGoupId(2);
//            Assert.AreEqual((res.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void GetOpeningLocationBygroupIdServerError()
//        {
//            var res = _openingLocationController.GetOpeningLocationByGoupId(001);
//            Assert.AreEqual((res.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }
//        [Test]
//        public void GetOpeningLocationBygroupIdError()
//        {
//            var res = _openingLocationController.GetOpeningLocationByGoupId(0);
//            Assert.AreEqual((res.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void GetOpeningLocationBygroupIdModelStateError()
//        {
//            _openingLocationController.ModelState.AddModelError("test", "test");
//            var res = _openingLocationController.GetOpeningLocationByGoupId(0);
//            Assert.AreEqual((res.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        /// <summary>
//        /// Update Opening Location Error
//        /// </summary>
//        /// <param name="groupConfigurationId"></param>
//        /// <param name="openingLocation"></param>
//        [Test]
//        public void UpdateOpeningLocationError()
//        {
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.UPDATEOPENINGLOCATIONREQUESTBODY));
//            var OpeningLocationObject = jObject.ToObject<OpeningLocations>();
//            var response = _openingLocationController.UpdateOpeningLocation( OpeningLocationObject);
//            Assert.IsNotNull(response);
//        }
//    }
//}
