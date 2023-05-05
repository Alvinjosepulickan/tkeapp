//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using NUnit.Framework;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using TKE.SC.BFF.BusinessProcess.Helpers;
//using TKE.SC.BFF.BusinessProcess.Interfaces;
//using TKE.SC.BFF.Controllers;
//using TKE.SC.Common.Model.ViewModel;
//using TKE.SC.BFF.Test.Common;
//using TKE.SC.BFF.APIController;

//namespace TKE.SC.BFF.Test.Controller
//{
//    public class ProjectsControllerTests
//    {
//        #region 

//        private ProjectsController _projectsController;
//        private ILogger<ProjectsController> _projectsControllerLogger;

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
//            _projectsControllerLogger = services.BuildServiceProvider().GetService<ILogger<ProjectsController>>();

//            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();

//            var iProjects = (IProject)servicesProvider.GetService(typeof(IProject));



//            _projectsController = new ProjectsController(_projectsControllerLogger, iProjects);
//            _projectsController.ControllerContext = new ControllerContext();
//            _projectsController.ControllerContext.HttpContext = new DefaultHttpContext();
//            _projectsController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
//        }
//        #endregion

//        #region SetUp Input Values

//        public static IEnumerable<TestCaseData> InputSearchUserValues()
//        {
//            yield return new TestCaseData("user1");
//        }

//        #endregion

//        [TestCaseSource(nameof(InputSearchUserValues))]
//        public void SerachUserTestCase(string userName)
//        {
//            var response = _projectsController.SearchUser(userName);
//            Assert.IsNotNull(response);

//        }

//        /// <summary>
//        /// SearchUserError
//        /// </summary>
//        [Test]
//        public void SearchUserError()
//        {
//            var response = _projectsController.SearchUser(null);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        [Test]
//        public void SearchUserModelStateError()
//        {
//            _projectsController.ModelState.AddModelError("test", "test");
//            var response = _projectsController.SearchUser(null);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }
//        /// <summary>
//        /// GetProjectDetails
//        /// </summary>
//        [Test]
//        public void GetProjectDetails()
//        {
//            var response = _projectsController.GetProjectDetails("62","145");
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// GetProjectDetailsError
//        /// </summary>
//        [Test]
//        public void GetProjectDetailsError()
//        {
//            var response = _projectsController.GetProjectDetails("","");
//            Assert.IsNotNull(response); 
//        }

//        [Test]
//        public void GetProjectDetailsModelStateError()
//        {
//            _projectsController.ModelState.AddModelError("test", "test");
//            var response = _projectsController.GetProjectDetails(null,null);
//            Assert.IsNotNull(response);
//        }
//        /// <summary>
//        /// GetProjectDetails
//        /// </summary>
//        [Test]
//        public void GetProjectDetailsError1()
//        {
//            var response = _projectsController.GetProjectDetails("60","119");
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }
//        /// <summary>
//        /// GetProjectDetails
//        /// </summary>
//        [Test]
//        public void GetProjectDetailsError2()
//        {
//            var response = _projectsController.GetProjectDetails("","");
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        /// <summary>
//        /// GetListOfProjectsForUser
//        /// </summary>
//        [Test]
//        public void getListOfProjectsForUser()
//        {
//            var response = _projectsController.getListOfProjectsForUser(1);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// GetListOfProjectsForUserError
//        /// </summary>
//        [Test]
//        public void getListOfProjectsForUserError()
//        {
//            var response = _projectsController.getListOfProjectsForUser(0);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST); ;
//        }

//        [Test]
//        public void getListOfProjectsForUserModelStateError()
//        {
//            _projectsController.ModelState.AddModelError("test", "test");
//            var response = _projectsController.getListOfProjectsForUser(0);
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST); ;
//        }

//        /// <summary>
//        /// GetProjectDetails
//        /// </summary>
//        [Test]
//        public void GetProjectInfo()
//        {
//            var response = _projectsController.GetProjectInfo("ADIA-1N16VED","119");
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// GetProjectDetailsError
//        /// </summary>
//        [Test]
//        public void GetProjectInfoError()
//        {
//            var response = _projectsController.GetProjectInfo("","");
//            Assert.IsNotNull(response);
//        }

//        [Test]
//        public void GetProjectInfoModelError()
//        {
//            _projectsController.ModelState.AddModelError("test", "test");
//            var response = _projectsController.GetProjectInfo("","");
//            Assert.IsNotNull(response);
//        }
//        /// <summary>
//        /// GetProjectDetails
//        /// </summary>
//        [Test]
//        public void GetProjectInfoError2()
//        {
//            var response = _projectsController.GetProjectInfo("60","119");
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST);
//        }

//        /// <summary>
//        /// SaveConfigurationToView
//        /// </summary>
//        [Test]
//        public void SaveConfigurationToView()
//        {
//            var response = _projectsController.SaveConfigurationToExternalSystem("");
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// SaveConfigurationToViewError
//        /// </summary>
//        //[Test]
//        //public void SaveConfigurationToViewError()
//        //{
//        //    var response = _projectsController.SaveConfigurationToView("");
//        //    Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST); ;
//        //}

//        [Test]
//        public void SaveConfigurationToViewModelError()
//        {
//            _projectsController.ModelState.AddModelError("test", "test");
//            var response = _projectsController.SaveConfigurationToExternalSystem("");
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST); ;
//        }

//        /// <summary>
//        /// SaveConfigurationToView
//        /// </summary>
//        [Test]
//        public void GenerateViewConfigurationRequest()
//        {
//            var response = _projectsController.GenerateViewConfigurationRequest("");
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// SaveConfigurationToViewError
//        /// </summary>
//        [Test]
//        public void GenerateViewConfigurationRequestError()
//        {
//            var response = _projectsController.GenerateViewConfigurationRequest("");
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST); ;
//        }

//        [Test]
//        public void GenerateViewConfigurationRequestModelError()
//        {
//            _projectsController.ModelState.AddModelError("test", "test");
//            var response = _projectsController.GenerateViewConfigurationRequest("");
//            Assert.AreEqual((response.Result as ObjectResult).StatusCode, Constant.BADREQUEST); ;
//        }

//    }
//}
