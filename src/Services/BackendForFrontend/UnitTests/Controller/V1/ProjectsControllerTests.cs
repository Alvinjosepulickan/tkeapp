using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.Controllers;
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.BFF.Test.Common;
using TKE.SC.BFF.APIController;
using TKE.SC.BFF.Test.BusinessProcess.Helper;
using System.Security.Claims;
using System.Security.Principal;

namespace TKE.SC.BFF.Controller.Controller.V1
{
    public class ProjectsControllerTests
    {
        #region 

        private ProjectsController _projectsController;
        private ILogger<ProjectsController> _projectsControllerLogger;

        #endregion

        #region privatemethods
        /// <summary>
        /// initialconfigurationsetup
        /// </summary>
        [SetUp, Order(1)]
        public void Initialconfigurationsetup()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _projectsControllerLogger = services.BuildServiceProvider().GetService<ILogger<ProjectsController>>();

            var servicesprovider = services.BuildServiceProvider().GetService<IServiceProvider>();

            var iprojects = (IProject)servicesprovider.GetService(typeof(IProject));


            var identity = new GenericIdentity("testuser", "jwt");
            identity.AddClaim(new System.Security.Claims.Claim("SessionId", "SessionId"));
            var principal = new GenericPrincipal(identity, new string[] { "SessionId" });
            _projectsController = new ProjectsController(iprojects, _projectsControllerLogger);
            _projectsController.ControllerContext = new ControllerContext();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("SessionId","SessionId")
                                   }, "TestAuthentication"));
            _projectsController.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
            _projectsController.ControllerContext.HttpContext.Items["sessionid"] = "sessionidvalue";
            _projectsController.HttpContext.User = principal;
        }
        #endregion

        #region SetUp Input Values

        public static IEnumerable<TestCaseData> InputProjectVariableValues()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEINPUTVARIABLEVALUES);
        }


        public static IEnumerable<TestCaseData> InputProjectVariableValuesError()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEINPUTVARIABLEVALUEDSERROR);
        }

        #endregion


        /// <summary>
        /// GetProjectDetails
        /// </summary>
        [Test]
        public void GetProjectDetails()
        {
            var response = _projectsController.GetProjectDetails(string.Empty, "145", 5);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["projects"][0].ToString().Trim();
            JObject json = JObject.Parse(key);
            var keyValue = json.Root["opportunityId"].ToString().Trim();
            Assert.AreEqual(keyValue, "1");
        }

        /// <summary>
        /// GetProjectDetailsError
        /// </summary>
        [Test]
        public void GetProjectDetailsError()
        {
            var response = _projectsController.GetProjectDetails("Val", string.Empty, 0);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        [Test]
        public void GetProjectDetailsModelStateError()
        {
            _projectsController.ModelState.AddModelError("test", "test");
            var response = _projectsController.GetProjectDetails("Val", string.Empty, 0);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        /// <summary>
        /// CreateProjects
        /// </summary>
        [Test]
        public void CreateProjects()
        {
            var response = _projectsController.CreateProjects("60");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["permissions"][0].ToString().Trim();
            Assert.AreEqual(key, "value");
        }

        /// <summary>
        /// CreateProjectsError
        /// </summary>

        [Test]
        public void CreateProjectsError()
        {
            var response = _projectsController.CreateProjects(string.Empty);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        /// <summary>
        /// CreateProjectsModelStateError
        /// </summary>
        [Test]
        public void CreateProjectsModelStateError()
        {
            _projectsController.ModelState.AddModelError("test", "test");
            var response = _projectsController.CreateProjects(string.Empty);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }



        /// <summary>
        /// SaveAndUpdateMiniProjects
        /// </summary>
        [TestCaseSource(nameof(InputProjectVariableValues))]
        public void SaveAndUpdateMiniProjects(string inputpath)
        {
            var jobject = JObject.Parse(File.ReadAllText(inputpath));
            var response = _projectsController.SaveAndUpdateMiniProjects(jobject);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["Message"].ToString().Trim();
            Assert.AreEqual(key, "The Quote is Saved and Updated Succesfully");
        }

        ///// <summary>
        ///// SaveAndUpdateMiniProjectsError
        ///// </summary>
        [TestCaseSource(nameof(InputProjectVariableValuesError))]
        public void SaveAndUpdateMiniProjectsError(string inputpath)
        {
            var jobject = JObject.Parse(File.ReadAllText(inputpath));
            var response = _projectsController.SaveAndUpdateMiniProjects(jobject);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        ///// <summary>
        ///// SaveAndUpdateMiniProjects
        ///// </summary>
        [Test]
        public void SaveAndUpdateMiniProjectsModelStateError()
        {
            var response = _projectsController.SaveAndUpdateMiniProjects(null);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }


        /// <summary>
        /// DeleteProjectById
        /// </summary>
        [Test]
        public void DeleteProjectById()
        {
            var response = _projectsController.DeleteProjectById("12", "23");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["Message"].ToString().Trim();
            Assert.AreEqual(key, "Deleted Successfully");
        }

        /// <summary>
        /// DeleteProjectByIdError
        /// </summary>
        [Test]
        public void DeleteProjectByIdError()
        {
            var response = _projectsController.DeleteProjectById("", "");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        /// <summary>
        /// DeleteProjectByIdModelStateError
        /// </summary>
        [Test]
        public void DeleteProjectByIdModelStateError()
        {
            _projectsController.ModelState.AddModelError("test", "test");
            var response = _projectsController.DeleteProjectById("", "");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }


        /// <summary>
        /// DuplicateQuoteByQuoteId
        /// </summary>
        [Test]
        public void DuplicateQuoteByQuoteId()
        {
            var response = _projectsController.DuplicateQuoteByQuoteId("2", "5");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JArray check = JArray.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root[0]["Message"].ToString().Trim();
            Assert.AreEqual(key, "Duplicated Successfully");
        }
        /// <summary>
        /// DuplicateQuoteByQuoteId
        /// </summary>
        [Test]
        public void DuplicateQuoteByQuoteIdError()
        {
            var response = _projectsController.DuplicateQuoteByQuoteId("", "");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        /// <summary>
        /// DuplicateQuoteByQuoteId
        /// </summary>
        [Test]
        public void DuplicateQuoteByQuoteIdModelStateError()
        {
            _projectsController.ModelState.AddModelError("test", "test");
            var response = _projectsController.DuplicateQuoteByQuoteId("", "");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        /// <summary>
        /// AddToPrimaryQuotes
        /// </summary>
        [Test]
        public void AddToPrimaryQuotes()
        {
            var response = _projectsController.AddToPrimaryQuotes("1");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["quoteStatus"].ToString().Trim();
            Assert.AreEqual(key, "Quote Set To Primary");
        }

        /// <summary>
        /// AddToPrimaryQuotesError
        /// </summary>
        [Test]
        public void AddToPrimaryQuotesError()
        {
            var response = _projectsController.AddToPrimaryQuotes("");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        /// <summary>
        /// AddToPrimaryQuotesModelStateError
        /// </summary>
        [Test]
        public void AddToPrimaryQuotesModelStateError()
        {
            _projectsController.ModelState.AddModelError("test", "test");
            var response = _projectsController.AddToPrimaryQuotes("");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        /// <summary>
        /// AddQuoteForProject
        /// </summary>
        [Test]
        public void AddQuoteForProject()
        {
            var response = _projectsController.AddQuoteForProject("5");
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
            JObject check = JObject.FromObject((response.Result as ObjectResult).Value);
            string key = check.Root["message"].ToString().Trim();
            Assert.AreEqual(key, "The Quote is Saved and Updated Succesfully");
        }

        /// <summary>
        /// AddQuoteForProjectError
        /// </summary>
        [Test]
        public void AddQuoteForProjectError()
        {
            var response = _projectsController.AddQuoteForProject("");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }
        /// <summary>
        /// AddQuoteForProject
        /// </summary>
        [Test]
        public void AddQuoteForProjectModelStateError()
        {
            _projectsController.ModelState.AddModelError("test", "test");
            var response = _projectsController.AddQuoteForProject("");
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }
        [Test]
        public void GetProjectDetailsError1()
        {
            var response = _projectsController.GetProjectDetails("60", "119", 1);
            Assert.AreEqual(response.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }
        /// <summary>
        /// GetProjectDetails
        /// </summary>
        [Test]
        public void GetProjectDetailsNonError()
        {
            var response = _projectsController.GetProjectDetails("", "", 1);
            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.SUCCESS);
        }
    }
}
