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
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.BFF.Test.BusinessProcess.Services
{
    public class DaBLTests
    {

        private DesignAutomationBL _daBL;
        private ILogger<DesignAutomationBL> _daBLlogger;


        #region PrivateMethods
        [SetUp]
        [Obsolete]
        public void InitialConfiguration()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _daBLlogger = services.BuildServiceProvider().GetService<ILogger<DesignAutomationBL>>();
            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iCache = (ICacheManager)servicesProvider.GetService(typeof(ICacheManager));
            var iConfiguration = (IConfiguration)servicesProvider.GetService(typeof(IConfiguration));
            var iDaDL = (IDesignAutomationDL)servicesProvider.GetService(typeof(IDesignAutomationDL));
            var iObom = (IObom)servicesProvider.GetService(typeof(IObom));

            _daBL = new DesignAutomationBL(iDaDL, iObom, iConfiguration, iCache, _daBLlogger);


        }

        #endregion

        #region Setup Input Values
        public static IEnumerable<TestCaseData> InputDataForDA()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.DESIGNAUTOMATIONSTUB);
        }
        #endregion

        [TestCaseSource(nameof(InputDataForDA))]
        public void GetDaResponseTest(string requestBody)
        {
            var configurationDetails = Utility.DeserializeObjectValue<ConfigurationDetails>(File.ReadAllText(requestBody));
            var response = _daBL.GetDAResponse(configurationDetails, "sessionId");
            Assert.IsNotNull(response);
        }
        [TestCaseSource(nameof(InputDataForDA))]
        public void GetDaResponseErrorTest(string requestBody)
        {
            var configurationDetails = Utility.DeserializeObjectValue<ConfigurationDetails>(File.ReadAllText(requestBody));
            configurationDetails.GroupId = 0;
            var response = _daBL.GetDAResponse(configurationDetails, "sessionId");
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        [TestCaseSource(nameof(InputDataForDA))]
        public void GetDAStatusTest(string requestBody)
        {
            var configurationDetails = Utility.DeserializeObjectValue<ConfigurationDetails>(File.ReadAllText(requestBody));
            var response = _daBL.GetDAStatus(configurationDetails, "sessionId");
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        [TestCaseSource(nameof(InputDataForDA))]
        public void GetDAStatusErrorTest(string requestBody)
        {
            var configurationDetails = Utility.DeserializeObjectValue<ConfigurationDetails>(File.ReadAllText(requestBody));
            configurationDetails.GroupId = 0;
            var response = _daBL.GetDAStatus(configurationDetails, "sessionId");
            Assert.AreNotEqual(response.Result.StatusCode, Constant.BADREQUEST);
        }

        
    }
}


