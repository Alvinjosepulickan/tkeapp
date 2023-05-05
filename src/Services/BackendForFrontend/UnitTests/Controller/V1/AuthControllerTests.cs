using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.Controllers;
using TKE.SC.Common.Model.RequestModel;
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.BFF.Test.Common;
using TKE.SC.BFF.Api.V1;
using System.Security.Principal;
using TKE.SC.BFF.Test.BusinessProcess.Helper;

namespace TKE.SC.BFF.UnitTests.Controller.V1
{
    public class AuthControllerTests
    {
        #region 

        private AuthController _authController;
        private ILogger<AuthController> _authCoontrollerLogger;

        #endregion

        #region PrivateMethods
        /// <summary>
        /// InitialConfigurationSetup
        /// </summary>
        [SetUp, Order(1)]
        public void InitialConfigurationSetup()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _authCoontrollerLogger = services.BuildServiceProvider().GetService<ILogger<AuthController>>();

            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();

            var iAuth = (IAuth)servicesProvider.GetService(typeof(IAuth));
            var iGenerateToken = (IGenerateToken)servicesProvider.GetService(typeof(IGenerateToken));
            var http = (IHttpContextAccessor)servicesProvider.GetService(typeof(IHttpContextAccessor));
            _authController = new AuthController(iAuth, iGenerateToken, http, _authCoontrollerLogger);
            _authController.ControllerContext = new ControllerContext();
            _authController.ControllerContext.HttpContext = new DefaultHttpContext();
            _authController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
            var identity = new GenericIdentity("testuser", "jwt");
            identity.AddClaim(new System.Security.Claims.Claim("SessionId", "SessionId"));
            var principal = new GenericPrincipal(identity, new string[] { "SessionId" });
            _authController.HttpContext.User = principal;
        }
        #endregion

        #region SetUp Input Values

        public static IEnumerable<TestCaseData> InputUserInfoValues()
        {
            yield return new TestCaseData(1041845);
        }

        #endregion

        /// <summary>
        /// GetUserInfoController
        /// </summary>
        [TestCaseSource(nameof(InputUserInfoValues))]
        public void GetUserInfoControllerError(int userCode)
        {
            var response = _authController.GetUserDetails();
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

    }
}
