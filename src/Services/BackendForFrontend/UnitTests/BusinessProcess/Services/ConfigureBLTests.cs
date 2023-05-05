/************************************************************************************************************
************************************************************************************************************
    File Name     :   ConfigureBLTests.cs 
    Created By    :   Infosys LTD
    Created On    :   01-JAN-2020
    Modified By   :
    Modified On   :
    Version       :   1.0  
************************************************************************************************************
************************************************************************************************************/

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.IO;
using TKE.SC.BFF.BusinessProcess.Helpers;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.BusinessProcess.Services;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model.UIModel;
using TKE.SC.Common.Model.ViewModel;
using TKE.SC.BFF.Test.Common;
using Microsoft.Extensions.Logging;
using TKE.SC.BFF.Test.BusinessProcess.Helper;

namespace TKE.SC.BFF.Test.BusinessProcess.Services
{
    public class ConfigureBLTests
    {
        #region Variables
        private ConfigureBL _configurebl;
        private Utility _utility;
        #endregion

        #region PrivateMethods
        [SetUp]
        public void InitialConfiguration()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();

            var iConfigure = (IConfiguration)servicesProvider.GetService(typeof(IConfiguration));
            var iConfigureServices = (IConfigureServices)servicesProvider.GetService(typeof(IConfigureServices));
            var iConfiguratorServices = (IConfiguratorService)servicesProvider.GetService(typeof(IConfiguratorService));
            var caching = (ICacheManager)servicesProvider.GetService(typeof(ICacheManager));
            var http = (IHttpContextAccessor)servicesProvider.GetService(typeof(IHttpContextAccessor));
            var genToken = (IStringLocalizer<GenerateTokenBL>)servicesProvider.GetService(typeof(IStringLocalizer<GenerateTokenBL>));
            var iGroup = (IGroupConfigurationDL)servicesProvider.GetService(typeof(IGroupConfigurationDL));
            var iUnit = (IUnitConfigurationDL)servicesProvider.GetService(typeof(IUnitConfigurationDL));
            var iauth = (IAuth)servicesProvider.GetService(typeof(IAuth));
            var opening = (IOpeningLocationDL)servicesProvider.GetService(typeof(IOpeningLocationDL));
            var iLogger = (ILogger<ConfigureBL>)servicesProvider.GetService(typeof(ILogger<ConfigureBL>));
            _configurebl = new ConfigureBL(iConfigure, iConfigureServices, iConfiguratorServices, iauth, new Newtonsoft.Json.JsonSerializer(), caching, opening,http, genToken, iGroup, iUnit, iLogger);


        }
        #endregion


        /// <summary>
        /// ChangeConfigureBl
        /// </summary>
        [Test]
        public void ChangeConfigureBl()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEBUILDINGREQUESTBODY));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var res = _configurebl.ChangeBuildingConfigure(jObject, "");
            Assert.AreEqual(res.Status.ToString(),"RanToCompletion" );
        }

        /// <summary>
        /// RequestConfigurationBl
        /// </summary>
        [Test]
        public void RequestConfigurationBl()
        {
            SublineRequest objSublineRequest = new SublineRequest();
            objSublineRequest = null;
            var res = _configurebl.RequestConfigurationBl(objSublineRequest, "", "");
            Assert.AreEqual(res.Result.StatusCode, Constant.SUCCESS);
        }




        /// <summary>
        /// StartConfigureBlError
        /// </summary>
        [Test]
        public void StartConfigureBlError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY));
            var res = _configurebl.StartBuildingConfigure("", null);
            Assert.AreEqual(res.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        /// <summary>
        /// ChangeGroupConfigureBl
        /// </summary>
        [Test]
        public void ChangeGroupConfigureBl()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGEGROUPCONFIGBLREQUESTBODY));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var res = _configurebl.ChangeBuildingConfigure(jObject, "");
            Assert.AreEqual(res.Status.ToString(), "RanToCompletion");
        }

        /// <summary>
        /// ChangeGroupConfigureBl
        /// </summary>
        [Test]
        public void ChangeGroupConfigureBllayout()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGEGROUPCONFIGBLREQUESTBODY));
            //var requestObject = jObject.ToObject<ConfigurationRequest>();
            var res = _configurebl.ChangeGroupConfigure(jObject, 0, "", "GROUPLAYOUTCONFIGURATION", null);
            Assert.NotNull(res);
        }

        /// <summary>
        /// ChangeGroupConfigureBlError
        /// </summary>
        [Test]
        public void ChangeGroupConfigureBlError()
        {
            var res = _configurebl.ChangeGroupConfigure(null, 0, "", "", null);
            Assert.AreEqual(res.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }


        /// <summary>
        /// StartGroupConfigureBl
        /// </summary>
        [Test]
        public void StartGroupConfigureBl()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONFIGBLREQUESTBODY));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var res = _configurebl.StartGroupConfigureBl("", "", "", "", requestObject);
            Assert.AreEqual(res.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// StartGroupConfigureBlError
        /// </summary>
        [Test]
        public void StartGroupConfigureBlError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONFIGBLREQUESTBODY));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            requestObject.PackagePath = "grouplayout";
            var res = _configurebl.StartGroupConfigureBl("", "", "", "", null);
            Assert.AreEqual(res.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }


        /// <summary>
        /// TestStartConfigureBl
        /// </summary>
        [Test]
        public void TestStartConfigureBl()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONFIGBLREQUESTBODY));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var res = _configurebl.TestStartConfigureBl("", "", "", requestObject);
            Assert.AreEqual(res.Result.StatusCode, Constant.SUCCESS);
        }

        /// <summary>
        /// TestStartConfigureBlError
        /// </summary>
        [Test]
        public void TestStartConfigureBlError()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.STARTCONFIGBLREQUESTBODY));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            requestObject.PackagePath = null;
            var res = _configurebl.TestStartConfigureBl("", "", "", null);
            Assert.AreEqual(res.Status.ToString(), ConstantTest.FAULTEDSTATUS);
        }

        /// <summary>
        /// GenerateIncludeSections
        /// </summary>
        [Test]
        public void GenerateIncludeSections()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGEGROUPLAYOUTBLREQUESTBODY));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var res = _configurebl.GenerateIncludeSections(requestObject, "GROUPLAYOUTCONFIGURATION");
            Assert.AreEqual(res.PackagePath, "groupconfiguration");
        }

        /// <summary>
        /// GenerateIncludeSectionsopeninglocation
        /// </summary>
        [Test]
        public void GenerateIncludeSectionsopeninglocation()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGEGROUPLAYOUTBLREQUESTBODY));
            var requestObject = jObject.ToObject<ConfigurationRequest>();
            var res = _configurebl.GenerateIncludeSections(requestObject, "OPENINGLOCATION");
            Assert.AreEqual(res.PackagePath, "groupconfiguration");
        }


        /// <summary>
        /// ViewModelResponseMapper
        /// </summary>
        [Test]
        public void ViewModelResponseMapper()
        {
            var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CURRENTMACHINECONFIGURATION));
            var requestObject = jObject.ToObject<StartConfigureResponse>();
            var res = _configurebl.ViewModelResponseMapper(requestObject);
            Assert.AreEqual(res.Sections.Count, 1);
        }

        //[Test]
        //public void UnitHallFixtureConsoleConfigureBLTest()
        //{
        //    var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.SAVEUNITHALLFIXTUREREQUESTBODY));
        //    var requestObject = jObject.ToObject<UnitHallFixtures>();
        //    var res = _configurebl.UnitHallFixtureConsoleConfigureBl(requestObject, "sessionId", "fixtureType", 1, false);
        //    Assert.AreEqual(res.Result, Constant.SUCCESS);
        //}

        ///// <summary>
        ///// ChangeGroupConfigureLayoutBl
        ///// </summary>
        //[Test]
        //public void ChangeGroupConfigureLayoutBl()
        //{
        //    var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGEGROUPCONFIGLAYOUTREQUESTBODY));
        //    //var requestObject = jObject.ToObject<ConfigurationRequest>();
        //    var res = _configurebl.ChangeGroupConfigure(jObject, 0, "sessionId", "GROUPLAYOUTCONFIGURATION", null);
        //    Assert.AreEqual(res.Result, Constant.SUCCESS);
        //}

        //[Test]
        //public void ChangeGroupConfigureOpeningLocationBl()
        //{
        //    var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.CHANGEGROUPCONFIGLAYOUTREQUESTBODY));
        //    var res = _configurebl.ChangeGroupConfigure(jObject, 0, "sessionId", "OPENINGLOCATION", null);
        //    Assert.AreEqual(res.Result, Constant.SUCCESS);
        //}
        ///// <summary>
        ///// StartConfigureBl
        ///// </summary>
        //[Test]
        //public void StartConfigureBl()
        //{
        //    var jObject = JObject.Parse(File.ReadAllText(AppGatewayJsonFilePath.GETBUILDINGCONFIGREQUESTBODY));
        //    var requestObject = jObject.ToObject<ConfigurationRequest>();
        //    var res = _configurebl.StartBuildingConfigure("", requestObject);
        //    Assert.IsNotNull(res.Result.StatusCode);
        //}

    }
}
