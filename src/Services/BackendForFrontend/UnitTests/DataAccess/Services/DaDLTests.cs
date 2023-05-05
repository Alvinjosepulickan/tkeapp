
using Configit.TKE.DesignAutomation.Services.Models;
using Configit.TKE.DesignAutomation.WebApi.Models;
using Configit.TKE.OrderBom.WebApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TKE.SC.BFF.BusinessProcess.Services;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.BFF.DataAccess.Services;
using TKE.SC.BFF.Test.Common;
using TKE.SC.BFF.UnitTests.DataAccess.Models;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.Common.Model.ExceptionModel;

namespace TKE.SC.BFF.UnitTests.DataAccess.Services
{
    class DaDLTests
    {
        private DesignAutomationDL _daDL;
        private ILogger<DesignAutomationDL> _daDLlogger;


        #region PrivateMethods
        [SetUp]
        [Obsolete]
        public void InitialConfiguration()
        {
            CommonFunctions.InitialConfiguration();
            var services = CommonFunctions.ServiceCollection();
            _daDLlogger = services.BuildServiceProvider().GetService<ILogger<DesignAutomationDL>>();
            var servicesProvider = services.BuildServiceProvider().GetService<IServiceProvider>();
            var iCache = (ICacheManager)servicesProvider.GetService(typeof(ICacheManager));
            var iConfiguration = (IConfiguration)servicesProvider.GetService(typeof(IConfiguration));
            var iDaDL = (IDesignAutomationDL)servicesProvider.GetService(typeof(IDesignAutomationDL));

            _daDL = new DaDL(iConfiguration, _daDLlogger);


        }

        #endregion

        #region Setup Input Values
        public static IEnumerable<TestCaseData> CreateBomCSC1003AAEXTF()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.CSC1003AAEXTF);
        }
        public static IEnumerable<TestCaseData> CreateBomCSC1003AAEXTE()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.CSC1003AAEXTE);
        }
        public static IEnumerable<TestCaseData> CreateBomCSC1003AAINT()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.CSC1003AAINT);
        }
        public static IEnumerable<TestCaseData> CreateBomCSC1003AAINTF()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.CSC1003AAINTF);
        }
        public static IEnumerable<TestCaseData> SubmitBomCSC1003AAEXTF()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SUBMITBOMREQUESTCSC1003AAEXTF);
        }
        public static IEnumerable<TestCaseData> SubmitBomCSC1003AAEXTE()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SUBMITBOMREQUESTCSC1003AAEXTE);
        }
        public static IEnumerable<TestCaseData> SubmitBomCSC1003AAINT()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SUBMITBOMREQUESTCSC1003AAINT);
        }
        public static IEnumerable<TestCaseData> SubmitBomCSC1003AAINTF()
        {
            yield return new TestCaseData(AppGatewayJsonFilePath.SUBMITBOMREQUESTCSC1003AAINTF);
        }
        #endregion

        [TestCaseSource(nameof(CreateBomCSC1003AAEXTF))]
        public void GETOBOMResponseTestCSC1003AAEXTF(string requestBody)
        {
            var requestBodyObject = Utility.DeserializeObjectValue<CreateBomRequest>(File.ReadAllText(requestBody));
            var response = _daDL.GETOBOMResponse(requestBodyObject);
            var outputResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<CreateBomResponseTest>(Utility.SerializeObjectValue(response.Result.Response)));
            var expectedResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<CreateBomResponseTest>(File.ReadAllText(AppGatewayJsonFilePath.CSC1003AAEXTFRESPONSE)));
            
            Assert.AreEqual(expectedResponse,outputResponse);
        }
        [TestCaseSource(nameof(CreateBomCSC1003AAEXTE))]
        public void GETOBOMResponseTestCSC1003AAEXTE(string requestBody)
        {
            var requestBodyObject = Utility.DeserializeObjectValue<CreateBomRequest>(File.ReadAllText(requestBody));
            var response = _daDL.GETOBOMResponse(requestBodyObject);
            var outputResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<CreateBomResponseTest>(Utility.SerializeObjectValue(response.Result.Response)));
            var expectedResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<CreateBomResponseTest>(File.ReadAllText(AppGatewayJsonFilePath.CSC1003AAEXTERESPONSE)));

            Assert.AreEqual(expectedResponse, outputResponse);
        }
        [TestCaseSource(nameof(CreateBomCSC1003AAINT))]
        public void GETOBOMResponseTestCSC1003AAINT(string requestBody)
        {
            var requestBodyObject = Utility.DeserializeObjectValue<CreateBomRequest>(File.ReadAllText(requestBody));
            var response = _daDL.GETOBOMResponse(requestBodyObject);
            var outputResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<CreateBomResponseTest>(Utility.SerializeObjectValue(response.Result.Response)));
            var expectedResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<CreateBomResponseTest>(File.ReadAllText(AppGatewayJsonFilePath.CSC1003AAINTRESPONSE)));

            Assert.AreEqual(expectedResponse, outputResponse);
        }
        [TestCaseSource(nameof(CreateBomCSC1003AAINTF))]
        public void GETOBOMResponseTestCSC1003AAINTF(string requestBody)
        {
            var requestBodyObject = Utility.DeserializeObjectValue<CreateBomRequest>(File.ReadAllText(requestBody));
            var response = _daDL.GETOBOMResponse(requestBodyObject);
            var outputResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<CreateBomResponseTest>(Utility.SerializeObjectValue(response.Result.Response)));
            var expectedResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<CreateBomResponseTest>(File.ReadAllText(AppGatewayJsonFilePath.CSC1003AAINTFRESPONSE)));

            Assert.AreEqual(expectedResponse, outputResponse);
        }
        [TestCaseSource(nameof(CreateBomCSC1003AAEXTF))]
        public void GETOBOMResponseErrorTest(string requestBody)
        {
            var requestBodyObject = Utility.DeserializeObjectValue<CreateBomRequest>(File.ReadAllText(requestBody));
            requestBodyObject = new CreateBomRequest();
            var response = _daDL.GETOBOMResponse(requestBodyObject);
            Assert.ThrowsAsync<ExternalCallException>(() => response);
        }
        [TestCaseSource(nameof(SubmitBomCSC1003AAEXTF))]
        public void GetSubmitBomResponseTestCSC1003AAEXTF(string requestBody)
        {
            var requestBodyObject = Utility.DeserializeObjectValue<SubmitBomRequest>(File.ReadAllText(requestBody));
            var response = _daDL.GETSubmitBOMResponse(requestBodyObject);
            var outputResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<SubmitBomResponseTest>(Utility.SerializeObjectValue(response.Result)));
            var expectedResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<SubmitBomResponseTest>(File.ReadAllText(AppGatewayJsonFilePath.SUBMITBOMRESPONSECSC1003AAEXTF)));
           
            Assert.AreEqual(expectedResponse, outputResponse);
        }
        [TestCaseSource(nameof(SubmitBomCSC1003AAEXTE))]
        public void GetSubmitBomResponseTestCSC1003AAEXTE(string requestBody)
        {
            var requestBodyObject = Utility.DeserializeObjectValue<SubmitBomRequest>(File.ReadAllText(requestBody));
            var response = _daDL.GETSubmitBOMResponse(requestBodyObject);
            var outputResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<SubmitBomResponseTest>(Utility.SerializeObjectValue(response.Result)));
            var expectedResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<SubmitBomResponseTest>(File.ReadAllText(AppGatewayJsonFilePath.SUBMITBOMRESPONSECSC1003AAEXTE)));

            Assert.AreEqual(expectedResponse, outputResponse);
        }
        [TestCaseSource(nameof(SubmitBomCSC1003AAINT))]
        public void GetSubmitBomResponseTestCSC1003AAINT(string requestBody)
        {
            var requestBodyObject = Utility.DeserializeObjectValue<SubmitBomRequest>(File.ReadAllText(requestBody));
            var response = _daDL.GETSubmitBOMResponse(requestBodyObject);
            var outputResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<SubmitBomResponseTest>(Utility.SerializeObjectValue(response.Result)));
            var expectedResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<SubmitBomResponseTest>(File.ReadAllText(AppGatewayJsonFilePath.SUBMITBOMRESPONSECSC1003AAINT)));

            Assert.AreEqual(expectedResponse, outputResponse);
        }
        [TestCaseSource(nameof(SubmitBomCSC1003AAINTF))]
        public void GetSubmitBomResponseTestCSC1003AAINTF(string requestBody)
        {
            var requestBodyObject = Utility.DeserializeObjectValue<SubmitBomRequest>(File.ReadAllText(requestBody));
            var response = _daDL.GETSubmitBOMResponse(requestBodyObject);
            var outputResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<SubmitBomResponseTest>(Utility.SerializeObjectValue(response.Result)));
            var expectedResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<SubmitBomResponseTest>(File.ReadAllText(AppGatewayJsonFilePath.SUBMITBOMRESPONSECSC1003AAINTF)));

            Assert.AreEqual(expectedResponse, outputResponse);
        }
        [TestCaseSource(nameof(SubmitBomCSC1003AAEXTF))]
        public void GetSubmitBomResponseErrorTest(string requestBody)
        {
            var requestBodyObject = Utility.DeserializeObjectValue<SubmitBomRequest>(File.ReadAllText(requestBody));
            requestBodyObject = new SubmitBomRequest();
            var response = _daDL.GETSubmitBOMResponse(requestBodyObject);
            Assert.ThrowsAsync<ExternalCallException>(() => response);
        }
        [Test]
        public void GetDefaultExportTypesTest()
        {

            var response = _daDL.GetDefaultExportTypes();
            var outputResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<List<string>>(Utility.SerializeObjectValue(response.Result)));
            var expectedList = new List<string> { "PdfExport", "FeatureAttributesTask", "SheetMetalAttributesTask", "CadBomTask", "CenterOfGravityTask", "BillOfFeaturesTask" };
            var expectedResponse = Utility.SerializeObjectValue(expectedList);

            Assert.AreEqual(expectedResponse, outputResponse);
        }

        [Test]
        public void GetAvaliableExportTypesTest()
        {

            var response = _daDL.GetAvailableExportTypes();
            var outputResponse = Utility.DeserializeObjectValue<ExportTypeResponse[]>(Utility.SerializeObjectValue(response.Result));
            var expectedResponse = Utility.DeserializeObjectValue<ExportTypeResponse[]>(File.ReadAllText(AppGatewayJsonFilePath.AVAILABLEEXPORTTYPESRESPONSE)); 
            Assert.AreSame(expectedResponse, outputResponse);
        }

        [Test]
        public void GetJobStatusTest()
        {

            var response = _daDL.GetJobStatus("f034cc39-49cd-4227-9b04-8c585c62bfa7");
            var outputResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<AutomationTaskDetailsTest>(Utility.SerializeObjectValue(response.Result)));
            var expectedResponse = Utility.SerializeObjectValue(Utility.DeserializeObjectValue<AutomationTaskDetailsTest>(File.ReadAllText(AppGatewayJsonFilePath.GETJOBSTATUSESRESPONSE)));

            Assert.AreEqual(expectedResponse, outputResponse);
        }
        
    }
}
