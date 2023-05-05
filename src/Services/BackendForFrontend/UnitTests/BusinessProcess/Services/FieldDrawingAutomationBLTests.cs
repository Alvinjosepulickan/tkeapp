using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.BFF.Test.Common;
using TKE.SC.Common.Model.ExceptionModel;

namespace TKE.SC.BFF.Test.BusinessProcess.Services
{
    public class FieldDrawingAutomationBLTests
    {

        private FieldDrawingAutomationBL _fieldDrawingAutomationBL;
        private ILogger<FieldDrawingAutomationBL> _fieldDrawingAutomationBLlogger;


        #region PrivateMethods
        [SetUp]
        [Obsolete]
        public void InitialConfiguration()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _fieldDrawingAutomationBLlogger = services.BuildServiceProvider().GetService<ILogger<FieldDrawingAutomationBL>>();
            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var fieldDrawingAutomationDL = (IFieldDrawingAutomationDL)servicesProvider.GetService(typeof(IFieldDrawingAutomationDL));
            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
            var iCache = (ICacheManager)servicesProvider.GetService(typeof(ICacheManager));
            var iConfiguration = (IConfiguration)servicesProvider.GetService(typeof(IConfiguration));
            var iHostingEnvironment = (IHostingEnvironment)servicesProvider.GetService(typeof(IHostingEnvironment));
            var iGroupDL = (IGroupConfigurationDL)servicesProvider.GetService(typeof(IGroupConfigurationDL));
            var iBuilding = (IBuildingConfiguration)servicesProvider.GetService(typeof(IBuildingConfiguration));
            var iProduct = (IProductSelection)servicesProvider.GetService(typeof(IProductSelection));
            var iBuildingDl = (IBuildingConfigurationDL)servicesProvider.GetService(typeof(IBuildingConfigurationDL));

            _fieldDrawingAutomationBL = new FieldDrawingAutomationBL(fieldDrawingAutomationDL, iConfigure, iCache, iConfiguration, _fieldDrawingAutomationBLlogger
                , iHostingEnvironment, iGroupDL, iBuilding, iBuildingDl, iProduct);


        }

        #endregion

        #region Setup Input Values
        public static IEnumerable<TestCaseData> InputDataForChangeFieldDrawingAutomation()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.FIELDDRAWINGAUTOMATIONSTUB);
        }
        #endregion

        [TestCaseSource(nameof(InputDataForChangeFieldDrawingAutomation))]
        public void StartFieldDrawingConfigure(string requestBody)
        {
            var Jobject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _fieldDrawingAutomationBL.StartFieldDrawingConfigure(Jobject, 1, "sessionId");
            Assert.IsNotNull(response);
        }
        //non implementation of interface
        [Test]
        public void StartFieldDrawingConfigureErrorTest()
        {
            var response = _fieldDrawingAutomationBL.StartFieldDrawingConfigure(null, 0, "sessionIdTest");
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        [Test]
        public void GetListOfConfigurationForProject()
        {
            var response = _fieldDrawingAutomationBL.GetFieldDrawingsByProjectId("15", "sessionId");
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        [Test]
        public void GetFieldDrawingsByProjectId()
        {
            var response = _fieldDrawingAutomationBL.GetFieldDrawingsByProjectId("1", "sessionId");
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }
        [Test]
        public void GetFieldDrawingsByProjectIdResponseErrorTest()
        {
            var response = _fieldDrawingAutomationBL.GetFieldDrawingsByProjectId("", "sessionId");
            Assert.AreNotEqual(response.Result.StatusCode, Constant.BADREQUEST);
        }

        [Test]
        public void GetRequestQueueByGroupId()
        {
            var response = _fieldDrawingAutomationBL.GetRequestQueueByGroupId(1);
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }
        [Test]
        public void GetRequestQueueByGroupIdResponseErrorTest()
        {
            var response = _fieldDrawingAutomationBL.GetRequestQueueByGroupId(-1);
            Assert.ThrowsAsync<CustomException>(() => response);
        }

        //need to put session id
        [TestCaseSource(nameof(InputDataForChangeFieldDrawingAutomation))]
        public void SaveFieldDrawingConfigure(string requestBody)
        {
            var Jobject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _fieldDrawingAutomationBL.SaveFieldDrawingConfigure(1, Jobject, "sessionId");
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }
        [Test]
        [TestCaseSource(nameof(InputDataForChangeFieldDrawingAutomation))]
        public void SaveFieldDrawingConfigureResponseErrorTest(string requestBody)
        {
            var Jobject = JObject.Parse(File.ReadAllText(requestBody));
            var response = _fieldDrawingAutomationBL.SaveFieldDrawingConfigure(-1, Jobject, "sessionId");
            Assert.ThrowsAsync<CustomException>(() => response);
        }

    }
}


