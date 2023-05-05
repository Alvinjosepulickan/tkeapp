/************************************************************************************************************
************************************************************************************************************
    File Name     :   BuildingConfigurationBLTests.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.BusinessProcess.Services;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.BFF.Test.Common;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
namespace TKE.SC.BFF.Test.BusinessProcess.Services
{
    public class BuildingConfigurationBLTests
    {

        private BuildingConfigurationBL _build;
        private ILogger<BuildingConfigurationBL> _buildingConfigurationBlLogger;


        #region PrivateMethods
        [SetUp]
        public void InitialConfiguration()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _buildingConfigurationBlLogger = services.BuildServiceProvider().GetService<ILogger<BuildingConfigurationBL>>();
            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iBuilding = (IBuildingConfigurationDL)servicesProvider.GetService(typeof(IBuildingConfigurationDL));
            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
            var IProject = (IProject)servicesProvider.GetService(typeof(IProject));
            var iCache = (ICacheManager)servicesProvider.GetService(typeof(ICacheManager));
            _build = new BuildingConfigurationBL(iBuilding,iConfigure, IProject,_buildingConfigurationBlLogger, iCache);


        }

        #endregion

        #region Input Values

        public static IEnumerable<TestCaseData> InputDataForStartBuildingConfigure()
        {
            yield return new TestCaseData("test1", "sessionId", AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForStartBuildingConfigureError()
        {
            yield return new TestCaseData("test", "sessionId", AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY);
        }

        #endregion

        /// <summary>
        /// This is the initial method which will return current configuration object with enriched data and prices
        /// </summary>
        /// <param name="configureRequest"></param>
        /// <param name="projectId"></param>
        /// <param name="sessionId"></param>
        [TestCaseSource(nameof(InputDataForStartBuildingConfigure))]
        public void StartBuildingConfigure(string projectId, string sessionId, string requestbody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestbody));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _build.StartBuildingConfigure(null, projectId,1, sessionId,true).Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        /// <summary>
        /// This is the initial method which will return current configuration object with enriched data and prices
        /// </summary>
        /// <param name="configureRequest"></param>
        /// <param name="buildingId"></param>
        /// <param name="sessionId"></param>
        [TestCaseSource(nameof(InputDataForStartBuildingConfigureError))]
        public void StartBuildingConfigureError(string projectId, string sessionId, string requestbody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestbody));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            //requestObject.PackagePath = null;
            var response = _build.StartBuildingConfigure(null,null, 0, sessionId, false);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// GetListOfConfigurationForProject
        /// </summary>
        [Test]
        public void GetListOfConfigurationForProject()
        {
            var response = _build.GetListOfConfigurationForProject("15","");
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// GetListOfConfigurationForProjectError
        /// </summary>
        [Test]
        public void GetListOfConfigurationForProjectError()
        {
            var res = _build.GetListOfConfigurationForProject("","");
            Assert.ThrowsAsync<CustomException>(() => res);
        }

        /// <summary>
        /// GetBuildingConfigurationById
        /// </summary>
        [Test]
        public void GetBuildingConfigurationById()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _build.GetBuildingConfigurationById(9, jObject, "");
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// GetBuildingConfigurationByIdError
        /// </summary>
        [Test] //to be implemented
        public void GetBuildingConfigurationByIdError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var res = _build.GetBuildingConfigurationById(0, jObject, "");
            Assert.ThrowsAsync<CustomException>(() => res);
        }



        /// <summary>
        /// SaveBuildingConfigurationForProjectError
        /// </summary>
        [Test] //to be implemented
        public void SaveBuildingConfigurationForProjectError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVENEWBUILDINGELEVATIONREQUESTBODY));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _build.SaveBuildingConfigurationForProject(0, "Test", "15", jObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }





        /// <summary>
        /// SaveBuildingElevation
        /// </summary>
        [Test] //to be implemented
        public void SaveBuildingElevationError1()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEBUILDINGELEVATIONREQUESTBODY));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            var response = _build.SaveBuildingElevation(buildingElevationObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// SaveBuildingElevation
        /// </summary>
        [Test] //to be implemented
        public void SaveBuildingElevationError2()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEBUILDINGELEVATIONREQUESTBODY1));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            buildingElevationObject.buildingConfigurationId = 0;
            var response = _build.SaveBuildingElevation(buildingElevationObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// SaveBuildingElevation
        /// </summary>
        [Test] //to be implemented
        public void SaveBuildingElevation()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEBUILDINGELEVATIONREQUESTBODY2));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            var response = _build.SaveBuildingElevation(buildingElevationObject);
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// SaveBuildingElevationError
        /// </summary>
        [Test]
        public void SaveBuildingElevationtError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.UPDATEBUILDINGELEVATIONREQUESTBODY));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            buildingElevationObject.buildingConfigurationId = 0;
            var response = _build.SaveBuildingElevation(buildingElevationObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// UpdateBuildingElevation
        /// </summary>
        [Test] //to be implemented
        public void UpdateBuildingElevation()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.UPDATEBUILDINGELEVATIONREQUESTBODY));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            var response = _build.UpdateBuildingElevation(buildingElevationObject);
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// UpdateBuildingElevationError
        /// </summary>
        [Test]
        public void UpdateBuildingElevationError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.UPDATEBUILDINGELEVATIONREQUESTBODY));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            buildingElevationObject.buildingConfigurationId = 0;
            var response = _build.UpdateBuildingElevation(buildingElevationObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// UpdateBuildingElevationError1
        /// </summary>
        [Test] //to be implemented
        public void UpdateBuildingElevationError1()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.UPDATEBUILDINGELEVATIONREQUESTBODY1));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            buildingElevationObject.buildingConfigurationId = 0;
            var response = _build.UpdateBuildingElevation(buildingElevationObject); 
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        /// <summary>
        /// UpdateBuildingElevationError2
        /// </summary>
        [Test] //to be implemented
        public void UpdateBuildingElevationError2()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.UPDATEBUILDINGELEVATIONREQUESTBODY2));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            buildingElevationObject.buildingConfigurationId = 0;
            var response = _build.UpdateBuildingElevation(buildingElevationObject); 
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// AutoSaveBuildingElevation
        /// </summary>
        [Test] //tobe implemented
        public void AutoSaveBuildingElevation()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEBUILDINGELEVATIONREQUESTBODY));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            var response = _build.AutoSaveBuildingElevation(buildingElevationObject);
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }


        /// <summary>
        /// GetBuildingConfigurationById1
        /// </summary>
        [Test]
        public void GetBuildingConfigurationById1()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _build.GetBuildingConfigurationById(9, jObject, "");
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }





        /// <summary>
        /// SaveBuildingConfigurationForProjectError1
        /// </summary>
        [Test]
        public void SaveBuildingConfigurationForProjectError1()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVENEWBUILDINGELEVATIONREQUESTBODY));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _build.SaveBuildingConfigurationForProject(0, "SessionId", "15", jObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// AutoSaveBuildingElevationError1
        /// </summary>
        [Test] //To be implemnted
        public void AutoSaveBuildingElevationError1()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEBUILDINGELEVATIONREQUESTBODY1));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            var response = _build.AutoSaveBuildingElevation(buildingElevationObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// AutoSaveBuildingElevationError
        /// </summary>
        [Test] //to be implemented
        public void AutoSaveBuildingElevationError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEBUILDINGELEVATIONREQUESTBODY1));
            var buildingElevationObject = jObject.ToObject<BuildingElevation>();
            var response = _build.AutoSaveBuildingElevation(buildingElevationObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// GetBuildingElevationById
        /// </summary>
        [Test]
        public void GetBuildingElevationById()
        {
            var response = _build.GetBuildingElevationById(9,"");
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// GetBuildingElevationByIdError
        /// </summary>
        [Test]
        public void GetBuildingElevationByIdError()
        {
            var response = _build.GetBuildingElevationById(0,"");
            Assert.ThrowsAsync<CustomException>(() => response);
        }


        /// <summary>
        /// DeleteBuildingConfigurationById
        /// </summary>
        [Test] //to be implemented
        public void DeleteBuildingConfigurationById()
        {
            var response = _build.DeleteBuildingConfigurationById(1, "");
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// DeleteBuildingConfigurationByIdError
        /// </summary>
        [Test] //to be implemented
        public void DeleteBuildingConfigurationByIdError()
        {
            var response = _build.DeleteBuildingConfigurationById(0, null);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// DeleteBuildingConfigurationByIdError1
        /// </summary>
        [Test]
        public void DeleteBuildingConfigurationByIdError1()
        {
            var response = _build.DeleteBuildingConfigurationById(9, "test");
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }


        /// <summary>
        /// StartBuildingConfigure
        /// </summary>
        [Test]
        public void StartBuildingConfigure()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY));
            var response = _build.StartBuildingConfigure(jObject, "buildingId" ,5, "", true);
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// StartBuildingConfigure
        /// </summary>
        [Test]
        public void StartBuildingConfigure1()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY));
            var response = _build.StartBuildingConfigure(jObject, "buildingId", 0, "", true);
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }
        /// <summary>
        /// StartBuildingConfigureError
        /// </summary>
        [Test]
        public void StartBuildingConfigureError()
        {
            var response = _build.StartBuildingConfigure(null, null,0, "",true);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test]
        public void StartBuildingConfigureReset()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY));
            var response = _build.StartBuildingConfigure(jObject, "projectId", 5, "sessionId", true);
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        [Test]
        public void StartBuildingConfigureResetError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY));
            var response = _build.StartBuildingConfigure(null, "projectId", -1, "sessionId", true);
            Assert.ThrowsAsync<CustomException>(() => response);
        }




        [Test] //to be implemented
        public void DeleteBuildingElevationById()
        {
            var res = _build.DeleteBuildingElevationById(5, "").Result;
            Assert.AreEqual(200, res.StatusCode);
        }
        [Test] //to be implemented
        public void DeleteBuildingElevationByIdError()
        {
            var res = _build.DeleteBuildingElevationById(0, null);
            Assert.ThrowsAsync<CustomException>(() => res);
        }


        /// <summary
        /// SaveBuildingConfigurationForProject
        /// </summary>
        [Test] //to be implemented
        public void SaveBuildingConfigurationForProject()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEBUILDINGREQUESTBODY));
            // var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _build.SaveBuildingConfigurationForProject(1, "sessionIdTest", "15", jObject);
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        [Test] //to be implemented
        public void QuickConfigurationSummaryBuilding()
        {
            var res = _build.QuickConfigurationSummary("building", "0", "sessionId").Result;
            Assert.AreEqual(200, res.StatusCode);
        }
        [Test]
        public void QuickConfigurationSummaryProject()
        {
            var res = _build.QuickConfigurationSummary("project", "Id", "sessionId").Result;
            Assert.AreEqual(200, res.StatusCode);
        }
        [Test] //to be implemented
        public void QuickConfigurationSummaryGroup()
        {
            var res = _build.QuickConfigurationSummary("group", "1", "sessionId").Result;
            Assert.AreEqual(200, res.StatusCode);
        }
        [Test]
        public void QuickConfigurationSummaryUnit()
        {
            var res = _build.QuickConfigurationSummary("unit", "1", "sessionId").Result;
            Assert.AreEqual(200, res.StatusCode);
        }
        [Test]
        public void QuickConfigurationSummarybuildingError()
        {
            var res = _build.QuickConfigurationSummary("building", "0", "sessionId").Result;
            Assert.AreEqual(200, res.StatusCode);
        }
        [Test]
        public void QuickConfigurationSummaryGroupError()
        {
            var res = _build.QuickConfigurationSummary("group", "0", "sessionId").Result;
            Assert.AreEqual(200, res.StatusCode);
        }
        [Test]
        public void QuickConfigurationSummaryUnitError()
        {
            var res = _build.QuickConfigurationSummary("set", "0", "sessionId").Result;
            Assert.AreEqual(200, res.StatusCode);
        }
    }  
}
