/************************************************************************************************************
************************************************************************************************************
    File Name     :   GroupConfigurationBLTests.cs 
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
using System.IO;
using System.Text;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.BusinessProcess.Services;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.BFF.Test.Common;
using TKE.SC.BFF.Test.CPQCacheManager.CPQCacheMangerStubServices;
using TKE.SC.BFF.Test.DataAccess.DataAccessStubServices;
using Newtonsoft.Json.Linq;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.Common.Model.ExceptionModel;

namespace TKE.SC.BFF.Test.BusinessProcess.Services
{
    public class GroupConfigurationBLTests
    {
        #region Variables
        private GroupConfigurationBL _group;
        private ILogger<GroupConfigurationBL> _groupConfigurationBlLogger;

        private readonly ICacheManager _cpqCacheManager;
        #endregion

        #region PrivateMethods
        [SetUp]
        public void InitialConfiguration()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _groupConfigurationBlLogger = services.BuildServiceProvider().GetService<ILogger<GroupConfigurationBL>>();
            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iGroup = (IGroupConfigurationDL)servicesProvider.GetService(typeof(IGroupConfigurationDL));
            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
            var _cpqCacheManager = (ICacheManager)servicesProvider.GetService(typeof(ICacheManager));
            var iOpeningLocation = (IOpeningLocationDL)servicesProvider.GetService(typeof(IOpeningLocationDL));
            _group = new GroupConfigurationBL(iGroup, iConfigure, _cpqCacheManager, _groupConfigurationBlLogger,iOpeningLocation);
        }

        #endregion

        #region

        public static IEnumerable<TestCaseData> InputDataForStartGroupConfigure()
        {
            yield return new TestCaseData(2, "sessionId", AppGatewayJsonFilePath.STARTGROUPCONFIGREQUESTBODY);
        }

        public static IEnumerable<TestCaseData> InputDataForStartGroupConfigureError()
        {
            yield return new TestCaseData(2, "sessionId", AppGatewayJsonFilePath.STARTGROUPCONFIGREQUESTBODY);
        }

        #endregion

        /// <summary>
        /// Start group configure test case
        /// </summary>
        /// <param name="requestbody"></param>
        /// <param name="buildingId"></param>
        /// <param name="sessionId"></param> //to be implemented
        [TestCaseSource(nameof(InputDataForStartGroupConfigure))]
        public void StartGroupConfigure(int buildingId, string sessionId, string requestbody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestbody));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _group.StartGroupConfigure(jObject, buildingId, 1, sessionId, "groupconfiguration").Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        /// <summary>
        /// Start group configure error test case
        /// </summary>
        /// <param name="requestbody"></param>
        /// <param name="buildingId"></param>
        /// <param name="sessionId"></param>
        [TestCaseSource(nameof(InputDataForStartGroupConfigureError))]
        public void StartGroupConfigureForGroupLayout(int buildingId, string sessionId, string requestbody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestbody));
            var response = _group.StartGroupConfigure(jObject, buildingId,1, sessionId, "groupconfiguration").Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestCaseSource(nameof(InputDataForStartGroupConfigureError))]
        public void StartGroupConfigureError(int buildingId, string sessionId, string requestbody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestbody));
            var response = _group.StartGroupConfigure(jObject, 0, 0, "sessionId", "groupconfiguration");
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        /// <summary>
        /// Get Group Configuration Details By GroupId
        /// </summary>
        [Test] //to be implemented
        public void GetGroupConfigurationDetailsByGroupId()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.VARIABLEASSIGNMENTREQUESTPAYLOAD));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            requestObject.PackagePath = null;
            var response = _group.GetGroupConfigurationDetailsByGroupId(55, jObject, "sessionId", "groupconfiguration").Result;
            Assert.AreEqual(200, response.StatusCode);
        }


        /// <summary>
        /// Get Group Configuration Details By GroupId Error
        /// </summary>
        [Test] //to be implemented
        public void GetGroupConfigurationDetailsByGroupIdError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.VARIABLEASSIGNMENTREQUESTPAYLOAD));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _group.GetGroupConfigurationDetailsByGroupId(0, jObject, "sessionid", "groupconfiguration");
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// Get Group Configuration Details By GroupId
        /// </summary>
        [Test]
        public void GetGroupConfigurationDetailsByGroupIdValue()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.GETGROUPCONFIGREQUESTBODY));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _group.GetGroupConfigurationDetailsByGroupId(55, jObject, "sessionId", "Traditional_Hall_Stations").Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        /// <summary>
        /// Get Group Configuration Details By GroupId Error
        /// </summary>
        [Test]
        public void GetGroupConfigurationDetailsByGroupIdErrorLog()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.GETGROUPCONFIGREQUESTBODY));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _group.GetGroupConfigurationDetailsByGroupId(0, jObject, "sessionId", "Traditional_Hall_Stations");
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// Save Group Configuration
        /// </summary>
        [Test] //To be implemented
        public void SaveGroupConfiguration()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEGROUPREQUESTBODY));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _group.SaveGroupConfiguration(1, "sessionId", jObject).Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        /// <summary>
        /// Save Group Configuration Error
        /// </summary>
        [Test]  //To be implemnted
        public void SaveGroupConfigurationError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEGROUPREQUESTBODY));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _group.SaveGroupConfiguration(0, "sessionId", jObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// Save Group Configuration Error1
        /// </summary>
        [Test]
        public void SaveGroupConfigurationError1()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEGROUPREQUESTBODY));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _group.SaveGroupConfiguration(0, "Test", jObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// Update Group Configuration
        /// </summary>
        [Test] //To be implemnted
        public void UpdateGroupConfiguration()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.UPDATEGROUPREQUESTBODY));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _group.UpdateGroupConfiguration(1, 54, jObject).Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        /// <summary>
        /// Update Group Configuration Error
        /// </summary>
        [Test] //To be implemnted
        public void UpdateGroupConfigurationError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.UPDATEGROUPREQUESTBODY));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _group.UpdateGroupConfiguration(0, 0, jObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// Update Group Configuration Error1
        /// </summary>
        [Test] //To be implemnted
        public void UpdateGroupConfigurationError1()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.UPDATEGROUPREQUESTBODY));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _group.UpdateGroupConfiguration(0, -1, jObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// Update Group Configuration Error2
        /// </summary>
        [Test] //To be implemnted
        public void UpdateGroupConfigurationError2()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.UPDATEGROUPREQUESTBODY));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _group.UpdateGroupConfiguration(0, -2, jObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// Delete Group Configuration
        /// </summary>
        [Test] //to be implemented
        public void DeleteGroupConfiguration()
        {
            var response = _group.DeleteGroupConfiguration(1).Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        /// <summary>
        /// Delete Group Configuration Error
        /// </summary>
        [Test] //to be implemented
        public void DeleteGroupConfigurationError()
        {
            var response = _group.DeleteGroupConfiguration(0);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// Delete Group Configuration Error1
        /// </summary>
        [Test] //to be implemented
        public void DeleteGroupConfigurationError1()
        {
            var response = _group.DeleteGroupConfiguration(-1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test] //To be implemented
        public void StartGroupConsole()
        {
            //var variableassignment = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEGROUPHALLFIXTURESREQUESTBODY));
            var response = _group.AddorChangeGroupConsole(1, 0, 1, "SessionId", "Emergency_Power", true).Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        /// <summary>
        /// Delete GroupHallFixture
        /// </summary>
        [Test] //to be implemented
        public void DeleteGroupHallFixture()
        {

            var response = _group.DeleteGroupHallFixtureConsole(1, 1, "Traditional_Hall_Stations", "sessionId").Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        /// <summary>
        /// Delete GroupHallFixture Error
        /// </summary>
        [Test] //to be implemented
        public void DeleteGroupHallFixtureError()
        {
            var response = _group.DeleteGroupHallFixtureConsole(0, 1, "AGILE_Hall_Stations", "sessionId");
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        /// <summary>
        /// Delete GroupHallFixture Error
        /// </summary>
        [Test] //to be implemented
        public void DeleteGroupHallFixtureInputError()
        {
            var response = _group.DeleteGroupHallFixtureConsole(0, 0, "AGILE_Hall_Stations", "sessioId");
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test] //to be implemented
        public void DuplicateGroupTest()
        {
            List<int> groupList = new List<int>();
            groupList.Add(1);
            var response = _group.DuplicateGroupConfigurationById(groupList, 1).Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        [Test] //to be implemented
        public void DuplicateGroupTestError()
        {
            List<int> groupList = new List<int>();
            groupList.Add(1);
            var response = _group.DuplicateGroupConfigurationById(groupList, 0);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
    }
}
