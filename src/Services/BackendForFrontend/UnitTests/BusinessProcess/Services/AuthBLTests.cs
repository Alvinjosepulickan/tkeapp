/************************************************************************************************************
************************************************************************************************************
    File Name     :   AuthBLTests.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Services;
using TKE.SC.BFF.Test.CPQCacheManager.CPQCacheMangerStubServices;
using TKE.SC.BFF.Test.DataAccess.DataAccessStubServices;

namespace TKE.SC.BFF.Test.BusinessProcess.Services
{
    public class AuthBLTests
    {
        #region Variables
        private AuthBL _auth;
        private static IConfigurationRoot _configurationRoot;
        #endregion

        #region PrivateMethods
        [SetUp]
        public void InitialConfiguration()
        {
            _configurationRoot = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(Constant.APPSETTINGS).Build();

           // _auth = new AuthBL(_configurationRoot, new CPQCacheMangerStubServicesTest(), new AuthStubDL());
        }

        #endregion

        # region SetUp Input Values
        public static IEnumerable<TestCaseData> InputValidateUserValues()
        {
            yield return new TestCaseData(1041845);
        }
        #endregion
        /// <summary>
        /// ValidateUserTestCase
        /// </summary>
        /// <param name="userId"></param>
        //[TestCaseSource(nameof(InputValidateUserValues))]
        //public void ValidateUseTestCase(int userId)
        //{
        //    var response = _auth.ValidateUser(userId).Result;
        //    Assert.AreEqual(200, response.StatusCode);
        //}

        /// <summary>
        /// validateUserValueForFailedCode
        /// </summary>
        //[Test]
        //public void ValidateUserValueForFailedCase()
        //{
        //    var response = _auth.ValidateUser(000).Result;
        //    Assert.AreEqual(401, response.StatusCode);
        //}

        /// <summary>
        /// GetUSerData
        /// </summary>
        /// <param name="userCode"></param>
        //[TestCaseSource(nameof(InputValidateUserValues))]
        //public void GetUSerData(int userCode)
        //{
        //    var response = _auth.GetUserInfo(userCode,"sessionId");
        //    Assert.IsNotNull(response);
        //}

        /// <summary>
        /// logoff test case
        /// </summary>
        //[Test]
        //public void LogoffTestCase()
        //{
        //    var response = _auth.Logoff("sa", "sb", "test", "\"test\"");
        //    Assert.AreEqual(response.Result.StatusCode, Constant.SUCCESS);
        //}

        /// <summary>
        /// logoff test case error
        /// </summary>
        //[Test]
        //public void LogoffTestCaseError()
        //{
        //    var response = _auth.Logoff();
        //    Assert.AreEqual(response.Result.StatusCode, Constant.UNAUTHORIZED);
        //}

        
    }
}
