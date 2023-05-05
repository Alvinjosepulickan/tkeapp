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
//using TKE.SC.BFF.DataAccess.Interfaces;
//using TKE.SC.Common.Model.UIModel;
//using TKE.SC.Common.Model.ViewModel;
//using TKE.SC.BFF.Test.Common;

//namespace TKE.SC.BFF.Test.Controller
//{
//    public class ProductSelectionControllerTests
//    {
//        #region 

//        private ProductSelectionController _productSelectionController;
//        private ILogger<ProductSelectionController> _productSelectionControllerLogger;

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
//            _productSelectionControllerLogger = services.BuildServiceProvider().GetService<ILogger<ProductSelectionController>>();

//            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();

//            var iGroup = (IProductSelection)servicesProvider.GetService(typeof(IProductSelection));
//            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));



//            _productSelectionController = new ProductSelectionController(_productSelectionControllerLogger, iGroup, iConfigure);
//            _productSelectionController.ControllerContext = new ControllerContext();
//            _productSelectionController.ControllerContext.HttpContext = new DefaultHttpContext();
//            _productSelectionController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
//        }
//        #endregion

//        /// <summary>
//        /// ProductSelection
//        /// </summary>
//        [Test]
//        public void ProductSelection()
//        {
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.PRODUCTSELECTIONCONTROLLERREQUESTBODY));
//            List<int> input = new List<int>() { 2 };
//            var response = _productSelectionController.ProductSelection("", "", input, "");
//            Assert.IsNotNull(response);
//        }

//        ///// <summary>
//        ///// SaveProductSelection
//        ///// </summary>
//        //[Test]
//        //public void SaveProductSelection()
//        //{
//        //    var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEPRODUCTSELECTIONREQUESTBODY));
//        //    var requestObject = jObject.ToObject<ProductSelection>();
//        //    var response = _productSelectionController.SaveProductSelection(55, requestObject);
//        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        //}

//        /// <summary>
//        /// SaveProductSelectionError
//        /// </summary>
//        [Test]
//        public void SaveProductSelectionError()
//        {
//            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEPRODUCTSELECTIONREQUESTBODY));
//            var requestObject = jObject.ToObject<ProductSelection>();
//            _productSelectionController.ModelState.AddModelError("test", "test");
//            var response = _productSelectionController.SaveProductSelection(0, requestObject);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }


//        ///// <summary>
//        ///// GetUnitDetailsForProductSelection
//        ///// </summary>
//        //[Test]
//        //public void GetUnitDetailsForProductSelection()
//        //{
//        //    var response = _productSelectionController.GetUnitDetails(55);
//        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
//        //}

//        ///// <summary>
//        ///// GetUnitDetailsForProductSelectionError
//        ///// </summary>
//        //[Test]
//        //public void GetUnitDetailsForProductSelectionError()
//        //{
//        //    var response = _productSelectionController.GetUnitDetails(0);
//        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        //}

//        ///// <summary>
//        ///// GetUnitDetailsForProductSelectionModelError
//        ///// </summary>
//        //[Test]
//        //public void GetUnitDetailsForProductSelectionModelError()
//        //{
//        //    _productSelectionController.ModelState.AddModelError("test", "test");
//        //    var response = _productSelectionController.GetUnitDetails(0);
//        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        //}



//    }
//}
