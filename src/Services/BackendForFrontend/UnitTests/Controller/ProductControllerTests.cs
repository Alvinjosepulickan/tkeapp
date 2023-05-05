//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Reflection.Metadata;
//using System.Text;
//using TKE.SC.BFF.BusinessProcess.Interfaces;
//using TKE.SC.BFF.Controllers;
//using TKE.SC.BFF.Test.Common;

//namespace TKE.SC.BFF.Test.Controller
//{
//    public class ProductsControllerTests
//    {
//        private ProductController _product;
//        private ILogger<ProductController> _productsControllerLogger;
//        [SetUp, Order(1)]
//        public void InitialConfigurationSetup()
//        {
//            CommonFunctions.InitialConfiguration();
//            var services = CommonFunctions.ServiceCollection();
//            _productsControllerLogger = services.BuildServiceProvider().GetService<ILogger<ProductController>>();

//            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();

//            var iproduct = (IProduct)servicesProvider.GetService(typeof(IProduct));

//            _product = new ProductController(_productsControllerLogger, iproduct);
//            _product.ControllerContext = new ControllerContext();
//            _product.ControllerContext.HttpContext = new DefaultHttpContext();
//            _product.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
//        }

//        [Test]
//        public void GetListOfProductLine()
//        {
//            var response = _product.GetListOfProductLine();
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, 200);
//        }

//        [Test]
//        public void GetListOfProductLineModelStateError()
//        {
//            _product.ModelState.AddModelError("test", "test");
//            var response = _product.GetListOfProductLine();
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, 400);
//        }
//    }
//}
