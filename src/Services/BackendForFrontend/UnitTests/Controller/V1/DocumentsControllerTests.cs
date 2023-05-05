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
using System.Security.Principal;

namespace TKE.SC.BFF.UnitTests.Controller.V1
{
    public class DocumentsControllerTests
    {
        private DocumentsController _document;
        private ILogger<DocumentsController> _documentControllerLogger;
        [SetUp, Order(1)]
        public void InitialConfigurationSetup()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _documentControllerLogger = services.BuildServiceProvider().GetService<ILogger<DocumentsController>>();

            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
            var iLogHistory = (ILogHistory)servicesProvider.GetService(typeof(ILogHistory));
            var iDocument = (IDocument)servicesProvider.GetService(typeof(IDocument));
            var identity = new GenericIdentity("testuser", "jwt");
            identity.AddClaim(new System.Security.Claims.Claim("SessionId", "SessionId"));
            var principal = new GenericPrincipal(identity, new string[] { "SessionId" });
            _document = new DocumentsController(iConfigure, iLogHistory, _documentControllerLogger, iDocument);
            _document.ControllerContext = new ControllerContext();
            _document.ControllerContext.HttpContext = new DefaultHttpContext();
            _document.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
            _document.HttpContext.User = principal;
        }


        [Test]
        public void GetVaultLocation()
        {
            var response = _document.GetVaultLocation("1");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, 200);
        }

    }
}
