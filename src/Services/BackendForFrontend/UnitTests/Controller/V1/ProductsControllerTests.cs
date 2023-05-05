using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.Controllers;
using TKE.SC.BFF.Test.Common;
using TKE.SC.BFF.APIController;
using Newtonsoft.Json.Linq;
using System.IO;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ExceptionModel;
using System.Security.Principal;

namespace TKE.SC.BFF.UnitTests.Controller.V1
{
    public class ProductsControllerTests
    {
        private ProductsController _product;
        private ILogger<ProductsController> _productsControllerLogger;
        [SetUp, Order(1)]
        public void InitialConfigurationSetup()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _productsControllerLogger = services.BuildServiceProvider().GetService<ILogger<ProductsController>>();

            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
            var iproductSelection = (IProductSelection)servicesProvider.GetService(typeof(IProductSelection));
            var identity = new GenericIdentity("testuser", "jwt");
            identity.AddClaim(new System.Security.Claims.Claim("SessionId", "SessionId"));
            var principal = new GenericPrincipal(identity, new string[] { "SessionId" });
            _product = new ProductsController(iproductSelection, iConfigure, _productsControllerLogger);
            _product.ControllerContext = new ControllerContext();
            _product.ControllerContext.HttpContext = new DefaultHttpContext();
            _product.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
            _product.HttpContext.User = principal;
        }


        /// <summary>
        /// ProductSelection
        /// </summary>
        [Test]
        public void ProductSelection()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.PRODUCTSELECTIONCONTROLLERREQUESTBODY));
            List<int> input = new List<int>() { 2 };
            var response = _product.ProductSelection("", "", input, "");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, TKE.SC.BFF.BusinessProcess.Helpers.Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["sections"].ToString().Trim();
            JArray json = JArray.Parse(key);
            var keyValue = json.Root[0]["id"].ToString().Trim();
            Assert.AreEqual(keyValue, "productSelectionMaster");
        }

        /// <summary>
        /// SaveProductSelection
        /// </summary>
        [Test]
        public void SaveProductSelection()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEPRODUCTSELECTIONREQUESTBODY));
            var requestObject = jObject.ToObject<ProductSelection>();
            var response = _product.SaveProductSelection(55, requestObject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, TKE.SC.BFF.BusinessProcess.Helpers.Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["message"].ToString().Trim();
            Assert.AreEqual(key, "Product selection saved successfully");
        }

        /// <summary>
        /// SaveProductSelectionError
        /// </summary>
        [Test]
        public void SaveProductSelectionError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEPRODUCTSELECTIONREQUESTBODY));
            var requestObject = jObject.ToObject<ProductSelection>();
            var response = _product.SaveProductSelection(0, requestObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

    }
}
