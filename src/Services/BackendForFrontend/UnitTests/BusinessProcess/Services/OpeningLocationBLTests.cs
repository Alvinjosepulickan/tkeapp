/************************************************************************************************************
************************************************************************************************************
    File Name     :   OpeningLocationBLTests.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using TKE.SC.BFF.BusinessProcess.Helpers;
using Microsoft.Extensions.Logging;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.BusinessProcess.Services;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.BFF.Test.Common;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using Microsoft.Extensions.Configuration;

namespace TKE.SC.BFF.Test.BusinessProcess.Services
{
    public class OpeningLocationBLTests
    {
        #region Variables
        private OpeningLocationBL _openingbl;
        private ILogger<OpeningLocationBL> _openingLocationBLLogger;
        #endregion

        #region PrivateMethods
        [SetUp]
        public void InitialConfiguration()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _openingLocationBLLogger = services.BuildServiceProvider().GetService<ILogger<OpeningLocationBL>>();
            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iopening = (IOpeningLocationDL)servicesProvider.GetService(typeof(IOpeningLocationDL));
            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
            var ilogger = (ILogger)servicesProvider.GetService(typeof(ILogger));
            var iGroup = (IGroupConfigurationDL)servicesProvider.GetService(typeof(IGroupConfigurationDL));
            var iGroupconfig = (IGroupConfiguration)servicesProvider.GetService(typeof(IGroupConfiguration));
            var _cpqCacheManager = (ICacheManager)servicesProvider.GetService(typeof(ICacheManager));
            var iConfiguration = (IConfiguration)servicesProvider.GetService(typeof(IConfiguration));

            _openingbl = new OpeningLocationBL(_openingLocationBLLogger, iopening, iConfigure, iGroup, iGroupconfig,_cpqCacheManager,iConfiguration);
        }

        #endregion

        #region Input Values
        public static IEnumerable<TestCaseData> InputDataForUpdateOpeningLocationTestCase()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEOPENINGLOCATIONREQUESTBODY);

        }

        public static IEnumerable<TestCaseData> InputDataForUpdateOpeningLocationTestCaseError()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.UPDATEOPENINGLOCATIONREQUESTBODY);

        }
        #endregion

        /// <summary>
        /// Get opening location by group Id  Test Case
        /// </summary>
        [Test]//To be implemnted
        public void GetOpeningLocationBygroupId()
        {
            var res = _openingbl.GetOpeningLocationByGroupId(2, "sessionId");
            Assert.AreEqual(res.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// Get opening location by group Id error test case
        /// </summary>
        [Test]//To be implemnted
        public void GetOpeningLocationBygroupIdError()
        {
            var res = _openingbl.GetOpeningLocationByGroupId(0, "sessionId");
            Assert.ThrowsAsync<CustomException>(() => res);
        }

        /// <summary>
        /// Update Opening Location Test Case
        /// </summary>
        /// <param name = "requestBody" ></ param >
        /// < param name="groupConfigurationId"></param>//To be implemnted
        [TestCaseSource(nameof(InputDataForUpdateOpeningLocationTestCase))]
        public void UpdateOpeningLocationTestCase(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var OpeningLocationObject = jObject.ToObject<OpeningLocations>();
            var response = _openingbl.UpdateOpeningLocation(OpeningLocationObject, "sessionId");
            Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// Update Opening Location Error Test Case
        /// </summary>
        /// <param name = "requestBody" ></ param >
        /// < param name="groupConfigurationId"></param> //To be implemnted
        [TestCaseSource(nameof(InputDataForUpdateOpeningLocationTestCase))]
        public void UpdateOpeningLocationTestCaseError(string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var OpeningLocationObject = jObject.ToObject<OpeningLocations>();
            OpeningLocationObject.BuildingRise = 0;
            var response = _openingbl.UpdateOpeningLocation(OpeningLocationObject, "sessionId");
            Assert.ThrowsAsync<CustomException>(() => response);
        }
    }
}
