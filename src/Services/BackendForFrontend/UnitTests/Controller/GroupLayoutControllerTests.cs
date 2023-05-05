//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json.Linq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using TKE.SC.BFF.BusinessProcess.Helpers;
//using TKE.SC.BFF.BusinessProcess.Interfaces;
//using TKE.SC.BFF.Controllers;
//using TKE.SC.Common.Model.UIModel;
//using TKE.SC.Common.Model.ViewModel;
//using TKE.SC.BFF.Test.Common;

//namespace TKE.SC.BFF.Test.Controller
//{
//    class GroupLayoutControllerTests
//    {
//        #region variables

//        private GroupLayoutController _groupLayoutController;
//        private ILogger<GroupConfigurationController> _groupConfigurationControllerLogger;

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
//            _groupConfigurationControllerLogger = services.BuildServiceProvider().GetService<ILogger<GroupConfigurationController>>();

//            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();

//            var iGroupLayout = (IGroupLayout)servicesProvider.GetService(typeof(IGroupLayout));
//            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));



//            _groupLayoutController = new GroupLayoutController(_groupConfigurationControllerLogger, iGroupLayout, iConfigure);
//            _groupLayoutController.ControllerContext = new ControllerContext();
//            _groupLayoutController.ControllerContext.HttpContext = new DefaultHttpContext();
//            _groupLayoutController.ControllerContext.HttpContext.Items["SessionId"] = "SessionIdValue";
//        }
//        #endregion

//        #region Input Values

//        public static IEnumerable<TestCaseData> InputDataForSaveGroupLayoutDetails()
//        {
//            yield return new TestCaseData(1, AppGatewayJsonFilePath.SAVEGROUPLAYOUTFLOORPLANREQUESTBODY);
//        }

//        public static IEnumerable<TestCaseData> InputDataForUpdateGroupLayoutDetails()
//        {
//            yield return new TestCaseData( 1, AppGatewayJsonFilePath.SAVEGROUPLAYOUTFLOORPLANERRORREQUESTBODY);
//        }

//        #endregion

//        [TestCaseSource(nameof(InputDataForSaveGroupLayoutDetails))]
//        public void UpdateGroupLayoutDetailsModelStateError( int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<GroupLayoutSave>();
//            _groupLayoutController.ModelState.AddModelError("test", "test");
//            var response = _groupLayoutController.UpdateGroupLayoutDetails(groupId, groupLayoutObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveGroupLayoutDetails))]
//        public void UpdateGroupLayoutDetails(int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<GroupLayoutSave>();
//            var response = _groupLayoutController.UpdateGroupLayoutDetails(groupId, groupLayoutObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveGroupLayoutDetails))]
//        public void UpdateGroupLayoutDetailsNew(int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<GroupLayoutSave>();
//            groupLayoutObject.Operation = Operation.OverWrite;
//            var response = _groupLayoutController.UpdateGroupLayoutDetails(groupId, groupLayoutObject);
//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForSaveGroupLayoutDetails))]
//        public void UpdateGroupLayoutDetailsInternalError( int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<GroupLayoutSave>();
//            var response = _groupLayoutController.UpdateGroupLayoutDetails(groupId, groupLayoutObject);
//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForSaveGroupLayoutDetails))]
//        public void SaveGroupLayoutDetailsModelStateError(int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<GroupLayoutSave>();
//            _groupLayoutController.ModelState.AddModelError("test", "test");
//            var response = _groupLayoutController.SaveGroupLayoutDetails(groupId, groupLayoutObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveGroupLayoutDetails))]
//        public void SaveGroupLayoutDetails(int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<GroupLayoutSave>();
//            var response = _groupLayoutController.SaveGroupLayoutDetails(groupId, groupLayoutObject);
//            Assert.IsNotNull(response);
//        }
//        [TestCaseSource(nameof(InputDataForSaveGroupLayoutDetails))]
//        public void SaveGroupLayoutDetailsInternalError(int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<GroupLayoutSave>();
//            var response = _groupLayoutController.SaveGroupLayoutDetails(groupId, groupLayoutObject);
//            Assert.IsNotNull(response);
//        }

//        [TestCaseSource(nameof(InputDataForSaveGroupLayoutDetails))]
//        public void DuplicateGroupLayoutDetails(int groupId, string requestBody)
//        {
//            var jObject = JObject.Parse(File.ReadAllText(requestBody));
//            var groupLayoutObject = jObject.ToObject<GroupLayoutSave>();
//            groupLayoutObject.Operation = Operation.Duplicate;
//            var response = _groupLayoutController.SaveGroupLayoutDetails(groupId, groupLayoutObject);
//            Assert.IsNotNull(response);
//        }
//    }
//}
