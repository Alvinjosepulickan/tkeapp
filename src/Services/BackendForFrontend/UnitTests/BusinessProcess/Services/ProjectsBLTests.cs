/************************************************************************************************************
************************************************************************************************************
    File Name     :   ProjectsBLTests.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.BusinessProcess.Services;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.BFF.Test.Common;
using TKE.SC.BFF.Test.BusinessProcess.Helper;
using TKE.SC.Common.Model.ExceptionModel;

namespace TKE.SC.BFF.Test.BusinessProcess.Services
{
    public class ProjectsBLTests
    {
        #region Variables
        private ProjectsBL _projectbl;
        private ILogger<ProjectsBL> _projectblLogger;
        #endregion

        #region PrivateMethods
        [SetUp]
        public void InitialConfiguration()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _projectblLogger = services.BuildServiceProvider().GetService<ILogger<ProjectsBL>>();
            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iproduct = (IProjectsDL)servicesProvider.GetService(typeof(IProjectsDL));
            var ilogger = (ILogger)servicesProvider.GetService(typeof(ILogger));
            var iconfiguration = (IConfiguration)servicesProvider.GetService(typeof(IConfiguration));
            var cpqCacheManager = (ICacheManager)servicesProvider.GetService(typeof(ICacheManager));
            var iauth = (IAuth)servicesProvider.GetService(typeof(IAuth));
            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
            var iVault = (IVaultDL)servicesProvider.GetService(typeof(IVaultDL));
            _projectbl = new ProjectsBL(_projectblLogger, iproduct, iconfiguration, cpqCacheManager, iauth, iConfigure);
        }
        #endregion


        /// <summary>
        /// Input data for get list of projects for user test case
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestCaseData> InputDataForGetListOfProjectsForUser()
        {
            yield return new TestCaseData(79023);
        }

        /// <summary>
        /// input data for get list of projects for user not found test case
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestCaseData> InputDataForGetListOfProjectsForUserNotFound()
        {
            yield return new TestCaseData(0);
        }





        /// <summary>
        /// get list of projects for user test case
        /// </summary>
        /// <param name="userid"></param> //To be implemnted
        [TestCaseSource(nameof(InputDataForGetListOfProjectsForUser))]
        public void GetListOfProjectsForUser(int userid)
        {
            var res = _projectbl.GetListOfProjectsForUser(userid);
            Assert.AreEqual(res.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// Get list of projects for user error test case
        /// </summary>
        /// <param name="userid"></param>//To be implemnted
        [TestCaseSource(nameof(InputDataForGetListOfProjectsForUserNotFound))]
        public void GetListOfProjectsForUserError(int userid)
        {
            var res = _projectbl.GetListOfProjectsForUser(userid);
            Assert.AreEqual(res.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        /// <summary>
        /// Get project details test case
        /// </summary>
        [Test]
        public void GetAllProjectsDetails()
        {
            var res = _projectbl.GetProjectDetails("","","");
            Assert.AreEqual(res.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// Get project details error test case
        /// </summary>
        [Test]
        public void GetAllProjectsDetailsError()
        {
            var res = _projectbl.GetProjectDetails("11", "11", "11",0,true);
            Assert.ThrowsAsync<CustomException>(() => res);
        }

        /// <summary>
        /// SearchUser
        /// </summary>
        [Test] //To be implemnted
        public void SearchUser()
        {
            var res = _projectbl.SearchUser("bhadra.j");
            Assert.AreEqual(res.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// SearchUserError
        /// </summary>
        [Test] //To be implemnted
        public void SearchUserError()
        {
            var res = _projectbl.SearchUser(null);
            Assert.AreEqual(res.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        /// <summary>
        /// SearchUser
        /// </summary>
        //[Test]
        //public void SearchUser1()
        //{
        //    var res = _projectbl.SearchUser(null);
        //    Assert.IsNotNull(res);
        //}

        /// <summary>
        /// SaveConfigurationToView
        /// </summary>
        [Test]
        public void SaveConfigurationToView()  
        {
            var res = _projectbl.SaveConfigurationToView("60", "TEST", "SessionId");
            Assert.AreEqual(res.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// SaveConfigurationToView
        /// </summary>
        [Test] 
        public void Saveconfigurationtoview1()
        {
            var res = _projectbl.SaveConfigurationToView("60", "", "sessionid");
            Assert.AreEqual(res.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// SaveConfigurationToView
        /// </summary>
        [Test] //To be implemnted
        public void SaveConfigurationToView2()
        {
            var res = _projectbl.SaveConfigurationToView("50", "TEST", "SessionId");
            Assert.AreEqual(res.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// SaveProjectInfo1
        /// </summary>
        [Test]
        public void SaveProjectInfo1()
        {
            var res = _projectbl.SaveProjectInfo1("60", "TEST", "SessionId");
            Assert.AreEqual(res.Result.StatusCode,Constant.SUCCESS);
        }


        /// <summary>
        /// SaveProjectInfo1
        /// </summary>
        [Test] //To be implemnted
        public void SaveProjectInfo1View()
        {
            var res = _projectbl.SaveProjectInfo1("60", "", "SessionId");
            Assert.AreEqual(res.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// SaveProjectInfo1Error
        /// </summary>
        [Test] //To be implemnted
        public void SaveProjectInfo1ViewError()
        {
            var res = _projectbl.SaveProjectInfo1("", "TEST", "SessionId");
            Assert.AreEqual(res.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }


        /// <summary>
        /// SaveProjectInfo1
        /// </summary>
        [Test]
        public void GetProjectInfo()
        {
            var res = _projectbl.GetProjectInfo("SC-", "0", "SessionId");
            var check = res.Result.Response.Root.ToString();
            Assert.AreEqual(res.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// SaveProjectInfo1
        /// </summary>
        [Test]
        public void GetProjectInfoView()
        {
            var res = _projectbl.GetProjectInfo("SC-", "0", "SessionId");
            Assert.AreEqual(res.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// GetProjectInfoError
        /// </summary>
        [Test] 
        public void Getprojectinfoerror()
        {
            var res = _projectbl.GetProjectInfo("", "0", "sessionid");
            Assert.AreEqual(res.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }
    }
}
