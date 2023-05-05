using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.Controllers;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.BFF.Test.Common;
using TKE.SC.BFF.APIController;

namespace TKE.SC.BFF.UnitTests.Controller.V1
{
    public class UtilitiesControllersTests
    {
        private UtilitiesController _utilitiesConfigurationController;
        private ILogger<UtilitiesController> _utilitiesConfigurationControllerLogger;
        private IConfigure _configure;
        private ILogHistory _logHistory;

        #region PrivateMethods
        /// <summary>
        /// InitialConfigurationSetup
        /// </summary>
        [SetUp, Order(1)]
        public void InitialConfigurationSetup()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _utilitiesConfigurationControllerLogger = services.BuildServiceProvider().GetService<ILogger<UtilitiesController>>();
            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iConfiguration = (IConfiguration)servicesProvider.GetService(typeof(IConfiguration));
            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
            var iLogHistory = (ILogHistory)servicesProvider.GetService(typeof(ILogHistory));
            var iFieldDrawingAutomation = (IFieldDrawingAutomation)servicesProvider.GetService(typeof(IFieldDrawingAutomation));
            _utilitiesConfigurationController = new UtilitiesController(_utilitiesConfigurationControllerLogger, iConfiguration, iLogHistory,iFieldDrawingAutomation );
            _utilitiesConfigurationController.ControllerContext = new ControllerContext();
            _utilitiesConfigurationController.ControllerContext.HttpContext = new DefaultHttpContext();
            _utilitiesConfigurationController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
        }
        #endregion

        [Test]
        public void GetLogContent()
        {
            var response = _utilitiesConfigurationController.GetLogContent(1, 1, 1000);
            Assert.AreEqual((response as ObjectResult).StatusCode, Constant.SUCCESS);
        }

        //[Test]
        //public void FetchGraphAPIDataAsync(string query)
        //{
        //    var response = _utilitiesConfigurationController.FetchGraphAPIDataAsync(query);
        //    Assert.IsNotNull(response);
        //}

        //[Test]
        //public void FetchGraphAPIDataAsyncAsError(string query)
        //{
        //    var response = _utilitiesConfigurationController.FetchGraphAPIDataAsync(query);
        //    Assert.AreEqual((respons., Constant.BADREQUEST);
        //}

        //[Test]  //to be implemented
        //public void DrawingLink(int groupId)
        //{

        //    var response = _utilitiesConfigurationController.GetDrawingLink(groupId);
        //    Assert.AreEqual((response as ObjectResult).StatusCode, Constant.SUCCESS);
        //}
    }
}