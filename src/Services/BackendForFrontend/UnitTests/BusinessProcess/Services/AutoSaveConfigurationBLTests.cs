/************************************************************************************************************
************************************************************************************************************
   File Name     :   GroupConfigurationBL.cs 
   Created By    :   Infosys LTD
   Created On    :   01-JAN-2020
   Modified By   :
   Modified On   :
   Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model;
using TKE.SC.BFF.DataAccess.Interfaces;
using System.Threading.Tasks;
using TKE.SC.Common.Model.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TKE.SC.BFF.BusinessProcess.Helpers;
using System.Linq;
using Configit.Configurator.Server.Common;
using System.IO;
using TKE.SC.BFF.BusinessProcess.Services;
using NUnit.Framework;
using TKE.SC.BFF.Test.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace TKE.SC.BFF.Test.BusinessProcess.Services
{
    class AutoSaveConfigurationBLTests
    {
        private AutoSaveConfigurationBL _save;
        private ILogger<AutoSaveConfigurationBL> _autoSaveConfigurationBlLogger;

        #region PrivateMethods
        [SetUp]
        public void InitialConfiguration()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _autoSaveConfigurationBlLogger = services.BuildServiceProvider().GetService<ILogger<AutoSaveConfigurationBL>>();

            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iAutoSave = (IAutoSaveConfigurationDL)servicesProvider.GetService(typeof(IAutoSaveConfigurationDL));
            var iConfigure = (IConfigure)servicesProvider.GetService(typeof(IConfigure));
            _save = new AutoSaveConfigurationBL(_autoSaveConfigurationBlLogger, iAutoSave, iConfigure);

        }
        #endregion

        #region Input Values
        #endregion

        [Test] //to be implemented
        public void AutoSaveConfiguration()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.AUTOSAVEBUILDINGCONFIGREQUESTBODY));
            var requestObject = jObject.ToObject<AutoSaveConfiguration>();
            var response = _save.AutoSaveConfiguration(requestObject).Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        [Test]
        public void AutoSaveConfigurationError()
        {
            var response = _save.AutoSaveConfiguration(null).Result;
            Assert.AreEqual(400, response.StatusCode);
        }

        [Test] //to be implemented
        public void DeleteAutoSaveConfigurationByUser()
        {
            var response = _save.DeleteAutoSaveConfigurationByUser("sessionId").Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        [Test]
        public void DeleteAutoSaveConfigurationByUserError()
        {
            var response = _save.DeleteAutoSaveConfigurationByUser(null).Result;
            Assert.AreEqual(400, response.StatusCode);
        }

        [Test] //to be implemented
        public void GetAutoSaveConfigurationByUser()
        {
            var response = _save.GetAutoSaveConfigurationByUser("sessionId").Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        [Test]
        public void GetAutoSaveConfigurationByUserError()
        {
            var response = _save.GetAutoSaveConfigurationByUser(null).Result;
            Assert.AreEqual(400, response.StatusCode);
        }

        [Test] //to be implemented
        public void GetAutoSaveConfigurationByUserCreatedError()
        {
            var response = _save.GetAutoSaveConfigurationByUser("sessionId").Result;
            Assert.AreEqual(200, response.StatusCode);
        }

        [Test]
        public void DeleteAutoSaveConfigurationByUserInternalError()
        {
            var response = _save.DeleteAutoSaveConfigurationByUser("test").Result;
            Assert.AreEqual(400, response.StatusCode);
        }

    }
}
