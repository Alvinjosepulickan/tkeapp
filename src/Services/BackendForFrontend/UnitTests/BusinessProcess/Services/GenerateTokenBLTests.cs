///************************************************************************************************************
//************************************************************************************************************
//    File Name     :   GenerateTokenBLTests.cs 
//    Created By    :   Infosys LTD
//    Created On    :   01-JAN-2020
//    Modified By   :
//    Modified On   :
//    Version       :   1.0  
//************************************************************************************************************
//************************************************************************************************************/
//using System;
//using System.Collections.Generic;
//using NUnit.Framework;
//using TKE.SC.BFF.BusinessProcess.Services;
//using TKE.SC.BFF.BusinessProcess.Helpers;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Caching.Distributed;
//using Microsoft.Extensions.Configuration;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System.IO;
//using UserInfoUI = TKE.SC.Common.Model.UIModel.UserInfo;
//using Microsoft.Extensions.Localization;
//using TKE.SC.BFF.Test.CPQCacheManager.CPQCacheMangerStubServices;
//using TKE.SC.BFF.Test.Common;
//using TKE.SC.Common.Model.ExceptionModel;

//namespace TKE.SC.BFF.Test.BusinessProcess.Services
//{
//    public class GenerateTokenBLTests
//    {
//        /// <summary>
//        /// Variable Collection
//        /// </summary>
//        #region Variables
//        private GenerateTokenBL _generateToken;
//        private static IConfigurationRoot _configurationRoot;
//        private static IHttpContextAccessor _contextAccessor;
//        private IStringLocalizer<GenerateTokenBL> _stringLocalizer;
//        #endregion

//        /// <summary>
//        /// Private Methods
//        /// </summary>
//        #region PrivateMethods
//        [SetUp]
//        public void InitialConfiguration()
//        {
//            _contextAccessor = new HttpContextAccessor();
//            _contextAccessor.HttpContext = new DefaultHttpContext();
//            _configurationRoot = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(Constant.APPSETTINGS).Build();
//            _generateToken = new GenerateTokenBL(_configurationRoot, new Newtonsoft.Json.JsonSerializer(), new CPQCacheMangerStubServicesTest(), _contextAccessor, _stringLocalizer);
//        }
//        #endregion

//        /// <summary>
//        /// Set Up Input Values
//        /// </summary>
//        /// <returns></returns>
//        #region SetUp Input Values
//        public static IEnumerable<TestCaseData> InputGetAuthToken()
//        {
//            yield return new TestCaseData("sessionId",null,"tkeUid");
//        }

//        public static IEnumerable<TestCaseData> InputExtendSession()
//        {
//            yield return new TestCaseData("sessionId");
//        }
//        #endregion

//        /// <summary>
//        /// Validity Token Test case
//        /// </summary>
//        [Test]
//        public void ValidityTokenTestCase()
//        {
//            var response = _generateToken.ValidityToken().Result;
//            Assert.AreEqual(200, response.StatusCode);
//        }

//        [Test]
//        public void ValidityTokenTestCaseError()
//        {
//            Assert.ThrowsAsync<CustomException>(async() => await _generateToken.ValidityToken("df", "sf", "tst", "f", true));           
            
//        }

//        /// <summary>
//        /// Get Auth Token Test Case
//        /// </summary>
//        /// <param name="sessionId"></param>
//        /// <param name="userDetails"></param>
//        /// <param name="tkeUid"></param>
//        [TestCaseSource(nameof(InputGetAuthToken))]
//        public void GetAuthTokenTestCase(String sessionId, UserInfoUI userDetails, string tkeUid)
//        {
//            string response = _generateToken.GetAuthToken(sessionId, userDetails, tkeUid);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// Extend Session Test Case
//        /// </summary>
//        /// <param name="sessionId"></param>
//        [TestCaseSource(nameof(InputExtendSession))]
//        public void ExtendSessionTestCase(String sessionId)
//        {
//            JObject response = _generateToken.ExtendSession(sessionId);
//            Assert.IsNotNull(response);
//        }

//        /// <summary>
//        /// UI App Settings Test Case
//        /// </summary>
//        [Test]
//        public void UIAppSettings()
//        {
//            List<IConfigurationSection> response = _generateToken.UIAppsettings();
//            Assert.IsNotNull(response);
//        }

        
         

//    }
//}
