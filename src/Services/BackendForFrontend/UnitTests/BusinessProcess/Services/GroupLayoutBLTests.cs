/************************************************************************************************************
************************************************************************************************************
    File Name     :   GroupLayoutBLTests.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.IO;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.BusinessProcess.Services;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.BFF.Test.Common;
using TKE.SC.BFF.Test.CPQCacheManager.CPQCacheMangerStubServices;
using TKE.SC.BFF.Test.DataAccess.DataAccessStubServices;
using Newtonsoft.Json.Linq;
using TKE.SC.Common.Model.ViewModel;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using TKE.SC.Common.Model.ExceptionModel;

namespace TKE.SC.BFF.Test.BusinessProcess.Services
{
    class GroupLayoutBLTests
    {
        #region Variables
        private GroupLayoutBL _groupLayout;
        private ILogger<GroupLayoutBL> _groupLayoutBlLogger;
        #endregion

        #region PrivateMethods
        [SetUp]
        public void InitialConfiguration()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _groupLayoutBlLogger = services.BuildServiceProvider().GetService<ILogger<GroupLayoutBL>>();
            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iGroupLayout = (IGroupLayoutDL)servicesProvider.GetService(typeof(IGroupLayoutDL));
            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
            _groupLayout = new GroupLayoutBL(_groupLayoutBlLogger, iGroupLayout, iConfigure);
        }
        #endregion

        #region Input Values

        public static IEnumerable<TestCaseData> InputValuesForSaveGroupLayout()
        {
            yield return new TestCaseData(55,"",AppGatewayJsonFilePath.VARIABLEASSIGNMENTREQUESTPAYLOAD);
        }

        public static IEnumerable<TestCaseData> InputValuesForSaveGroupLayoutError()
        {
            yield return new TestCaseData(0, "", AppGatewayJsonFilePath.VARIABLEASSIGNMENTREQUESTPAYLOAD);
        }

        public static IEnumerable<TestCaseData> InputValuesForUpdateGroupLayout()
        {
            yield return new TestCaseData(55, "sessionId", AppGatewayJsonFilePath.VARIABLEASSIGNMENTREQUESTPAYLOAD);
        }

        public static IEnumerable<TestCaseData> InputValuesForUpdateGroupLayoutError()
        {
            yield return new TestCaseData(0,"sessionId", AppGatewayJsonFilePath.VARIABLEASSIGNMENTREQUESTPAYLOAD);
        }
        #endregion
        ////To be implemnted
        [TestCaseSource(nameof(InputValuesForSaveGroupLayout))]
        public void SaveGroupLayout(int groupId, string username, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _groupLayout.SaveGroupLayout(groupId, username, jObject).Result;
            Assert.AreEqual(200, response.StatusCode);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForSaveGroupLayoutError))]
        public void SaveGroupLayoutError(int groupId, string username, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _groupLayout.SaveGroupLayout(groupId, username, jObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForUpdateGroupLayout))]
        public void UpdateGroupLayout(int groupId, string username, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _groupLayout.UpdateGroupLayout(groupId, username,"sessionId", jObject).Result;
            Assert.AreEqual(200, response.StatusCode);
        }
        //To be implemnted
        [TestCaseSource(nameof(InputValuesForUpdateGroupLayoutError))]
        public void UpdateGroupLayoutError(int groupId, string username, string requestBody)
        {
            var jObject = JObject.Parse(File.ReadAllText(requestBody));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var response = _groupLayout.UpdateGroupLayout(groupId, username,"sessionId", jObject);
            Assert.ThrowsAsync<CustomException>(() => response);
        }
    }
}
