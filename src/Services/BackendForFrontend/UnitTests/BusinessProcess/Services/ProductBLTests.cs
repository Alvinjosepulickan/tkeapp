/************************************************************************************************************
************************************************************************************************************
    File Name     :   ProductBLTests.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Services;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.BFF.Test.Common;

namespace TKE.SC.BFF.Test.BusinessProcess.Services
{
    public class ProductBLTests
    {
        #region Variables
        private ProductBL _product;
        private ILogger _logger;
        #endregion

        #region PrivateMethods
        [SetUp]
        public void InitialConfiguration()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iproduct = (IProductDL)servicesProvider.GetService(typeof(IProductDL));
            var ilogger = (ILogger<ProductBL>)servicesProvider.GetService(typeof(ILogger<ProductBL>));
            _product = new ProductBL(iproduct, ilogger);
        }
        #endregion

        /// <summary>
        /// Get list of product line test case
        /// </summary>
        [Test]
        public void GetListOfProductLine()
        {
            var response = _product.GetListOfProductLine();
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        
    }
}
