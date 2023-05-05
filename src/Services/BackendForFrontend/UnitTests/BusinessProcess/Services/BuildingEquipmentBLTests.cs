using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.BusinessProcess.Services;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.BFF.Test.Common;
using TKE.SC.Common.Model.ExceptionModel;

namespace TKE.SC.BFF.Test.BusinessProcess.Services
{
    class BuildingEquipmentBLTests
    {
        private BuildingEquipmentBL _buildingEquipmentBL;
        private ILogger<BuildingEquipmentBL> _buildingEquipmentBlLogger;
        private ConfigureBL _configurebl;
        private Utility _utility;

        #region PrivateMethods
        [SetUp]
        public void InitialConfiguration()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _buildingEquipmentBlLogger = services.BuildServiceProvider().GetService<ILogger<BuildingEquipmentBL>>();
            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iBuilding = (IBuildingEquipmentDL)servicesProvider.GetService(typeof(IBuildingEquipmentDL));
            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
            var IProject = (IProject)servicesProvider.GetService(typeof(IProject));
            var iBuildingDL = (IBuildingConfigurationDL)servicesProvider.GetService(typeof(IBuildingConfigurationDL));
            _buildingEquipmentBL = new BuildingEquipmentBL(iBuilding,iConfigure,iBuildingDL, _buildingEquipmentBlLogger);

            var iConfigureServices = (IConfigureServices)servicesProvider.GetService(typeof(IConfigureServices));
            var iConfiguratorServices = (IConfiguratorService)servicesProvider.GetService(typeof(IConfiguratorService));
            var caching = (ICacheManager)servicesProvider.GetService(typeof(ICacheManager));
            var iOpeningLocation = (IOpeningLocationDL)servicesProvider.GetService(typeof(IOpeningLocationDL));
            var http = (IHttpContextAccessor)servicesProvider.GetService(typeof(IHttpContextAccessor));
            var genToken = (IStringLocalizer<GenerateTokenBL>)servicesProvider.GetService(typeof(IStringLocalizer<GenerateTokenBL>));
            var iGroup = (IGroupConfigurationDL)servicesProvider.GetService(typeof(IGroupConfigurationDL));
            var iUnit = (IUnitConfigurationDL)servicesProvider.GetService(typeof(IUnitConfigurationDL));
            var iauth = (IAuth)servicesProvider.GetService(typeof(IAuth));
            var iConfiguration = (IConfiguration)servicesProvider.GetService(typeof(IConfiguration));
            var iLogger = (ILogger<ConfigureBL>)servicesProvider.GetService(typeof(ILogger<ConfigureBL>));
            _configurebl = new ConfigureBL(iConfiguration, iConfigureServices, iConfiguratorServices, iauth, new Newtonsoft.Json.JsonSerializer(),
                             caching, iOpeningLocation, http, genToken, iGroup, iUnit, iLogger);


        }

        #endregion

        #region Input Values
        public static IEnumerable<TestCaseData> InputDataForChangeBuildingEquipment()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.CHANGEBUILDINGEQUIPMENTREQUESTBODY, 1,true);
        }

        public static IEnumerable<TestCaseData> InputDataForSaveBuildingEquipmentConsole()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SAVEBUILDINGEQUIPMENTCONSOLEREQUESTBODY, 1, 1);
        }

        public static IEnumerable<TestCaseData> InputDataForChangeBuldingEquipmentConsole()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.CHANGEBULDINGEQUIPMENTCONSOLEREQUESTBODY);
        }
        #endregion

        [Test] //to be implemented
        public void StartBuildingEquipmentConfigureBLTest()
        {
            var response = _buildingEquipmentBL.StartBuildingEquipmentConfigure(null, 1, "sessionId");
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        [Test] //to be implemented
        public void StartBuildingEquipmentConfigureBLErrorTest()
        {
            var response = _buildingEquipmentBL.StartBuildingEquipmentConfigure(null, -1, "sessionId");
            Assert.ThrowsAsync<CustomException>(() => response);
        }
       //to be implemented
        [TestCaseSource(nameof(InputDataForSaveBuildingEquipmentConsole))]
        public void SaveAssignedGroupsBLTest(string requestBody, int buildingId, int consoleId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var buildingData = jObject.ToObject<BuildingEquipmentData>();
            var list = new List<BuildingEquipmentData>();
            list.Add(buildingData);
            var response = _buildingEquipmentBL.SaveAssignGroups(buildingId, list, "sessionId", 1);
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);

        }
        //to be implemented
        [TestCaseSource(nameof(InputDataForSaveBuildingEquipmentConsole))]
        public void SaveAssignedGroupsBLErrorTest(string requestBody, int buildingId, int consoleId)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var buildingData = jObject.ToObject<BuildingEquipmentData>();
            var list = new List<BuildingEquipmentData>();
            buildingData.ConsoleId = 0;
            list.Add(buildingData);
            var response = _buildingEquipmentBL.SaveAssignGroups(1, list, "sessionId", 1);
            Assert.ThrowsAsync<CustomException>(() => response);

        }

        [Test] //to be implemented
        public void DuplicateBuildingEquipmentConsoleBLTest()
        {
            var response = _buildingEquipmentBL.DuplicateBuildingEquipmentConsole(1, 1, "sessionId");
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        [Test] //to be implemented
        public void DuplicateBuildingEquipmentConsoleBLErrorTest()
        {
            var response = _buildingEquipmentBL.DuplicateBuildingEquipmentConsole(0, 1, "sessionId");
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [Test] //to be implemented
        public void DeleteBuildingEquipmentConsoleBLTest()
        {
            var response = _buildingEquipmentBL.DeleteBuildingEquipmentConsole(1, 1, "sessionId");
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        [Test] //to be implemented
        public void DeleteBuildingEquipmentConsoleBLErrorTest()
        {
            var response = _buildingEquipmentBL.DeleteBuildingEquipmentConsole(1, 0, "sessionId");
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        //to be implemented
        [TestCaseSource(nameof(InputDataForChangeBuildingEquipment))]
        public void SaveBuildingEquipmentConfigurationBLTest(string requestBody, int buildingId, bool saveDraft)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _buildingEquipmentBL.SaveBuildingEquipmentConfiguration(buildingId, jObject, "sessionId", 1, saveDraft);
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }
        //to be implemented
        [TestCaseSource(nameof(InputDataForChangeBuildingEquipment))]
        public void SaveBuildingEquipmentConfigurationBLErrorTest(string requestBody, int buildingId, bool saveDraft)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _buildingEquipmentBL.SaveBuildingEquipmentConfiguration(0, jObject, "sessionId", 1, saveDraft);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(InputDataForChangeBuldingEquipmentConsole))]
        public void StartBuildingEquipmentConsole(string requestBody)
        {

            var jObject1 = JObject.Parse(File.ReadAllText(requestBody));
            BuildingEquipmentData console1 = jObject1.ToObject<BuildingEquipmentData>();
            var response = _buildingEquipmentBL.StartBuildingEquipmentConsole(0, 1, "sessionId", console1);
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        [Test] //to be implemented
        public void StartBuildingEquipmentConsoleError()
        {
            var response = _buildingEquipmentBL.StartBuildingEquipmentConsole(0, 0, "sessionId");
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        [TestCaseSource(nameof(InputDataForChangeBuldingEquipmentConsole))]
        public void ChangeBuildingEquipmentConsoleBLTest(string requestBody)
        {
            var jObject1 = JObject.Parse(File.ReadAllText(requestBody));
            BuildingEquipmentData console1 = jObject1.ToObject<BuildingEquipmentData>();
            var response = _buildingEquipmentBL.StartBuildingEquipmentConsole(0,1,"sessionId", console1);
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }
    }
}
