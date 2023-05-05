using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.BFF.BusinessProcess.Interfaces;
using TKE.SC.BFF.Helper;
using TKE.SC.Common.Model;
using TKE.SC.Common.Model.HttpClientModel;
using TKE.SC.Common;

namespace TKE.SC.BFF.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UtilitiesController : ControllerBase
    {
        private IConfiguration _configure;
        private ILogHistory _loghistory;
        private IFieldDrawingAutomation _fieldDrawingAutomation;
        /// <summary>
        /// UtilityController
        /// </summary>
        /// <param Name="logger"></param>
        public UtilitiesController(ILogger<UtilitiesController> logger, IConfiguration configure, ILogHistory loghistory, IFieldDrawingAutomation fieldDrawing)
        {
            Helper.Utility.SetLogger(logger);
            _configure = configure;
            _loghistory = loghistory;
            _fieldDrawingAutomation = fieldDrawing;
        }

        /// <summary>
        /// API responsible for reading the log file and formatting it to the JSON format before returning
        /// </summary>
        /// <param Name="yyyyMMdd"></param>
        /// <returns></returns>
        [Route("logs"), HttpGet]
        public IActionResult GetLogContent(int date, int pageIndex = 1, int recordCount = 1000 )
        {
            var logFileNames = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), Constant.LOGDIRECTORY))
                                          .GetFiles(Constant.JSONFILEFILTER)
                                          .OrderBy(f => f.LastWriteTime)
                                          .ToList();

            var response = GetLogStringAsJson(date.ToString());
            return Ok(new ResponseMessage { StatusCode = SC.BFF.BusinessProcess.Helpers.Constant.SUCCESS, Response = response });

            JObject GetLogStringAsJson(string dateString)
            {
                var fileName = string.IsNullOrEmpty(dateString) ? $"{Constant.CURRENTLOGFILENAME}.{Constant.JSONEXTENTION}" : $"{Constant.CURRENTLOGFILENAME}{dateString}";

                var currentScopeFiles = logFileNames.Where(x => x.FullName.Contains(fileName, System.StringComparison.InvariantCultureIgnoreCase))
                                                    .Select(x => x.FullName)
                                                    .Reverse();

                if (currentScopeFiles.Count() == 0)
                {
                    return null;
                }
                var fileContent = new JArray();
                var counter = 0;
                foreach (var currentFile in currentScopeFiles)
                {
                    using (var fileStream = new FileStream(currentFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var streamReader = new StreamReader(fileStream))
                        {
                            string line;
                            while (!string.IsNullOrEmpty(line = streamReader.ReadLine()) && counter <= ( pageIndex * recordCount ))
                            {
                                counter++;
                                if (((pageIndex -1)* recordCount) <= counter && counter <= ( pageIndex * recordCount))
                                {
                                    foreach (var item in Constant.SERILOGPROPERTIES)
                                    {
                                        line = line.Replace(item.Key, item.Value);
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                               
                                fileContent.Add(JsonConvert.DeserializeObject(line));
                            }
                        }
                    }
                }
                return new JObject(new JProperty("data", fileContent));
            }
        }

        /// <summary>
        /// This method fetches data from the Graph API
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Route("graphdata"), HttpGet]
        public async Task<string> FetchGraphAPIDataAsync(string query)
        {
            query = Encoding.UTF8.GetString(System.Convert.FromBase64String(query));

            var clientId = _configure["ParamSettings:GraphAPI:ClientId"];
            var clientSecret = _configure["ParamSettings:GraphAPI:ClientSecret"];
            var authority = _configure["ParamSettings:GraphAPI:Authority"];

            var app = ConfidentialClientApplicationBuilder.Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority(new Uri(authority))
                    .Build();

            string[] scopes = new string[] { $"{_configure["ParamSettings:GraphAPI:Scope"]}" };

            AuthenticationResult result = app.AcquireTokenForClient(scopes).ExecuteAsync().Result;

            var request = new HttpClientRequestModel()
            {
                BaseUrl=query,
                Method=HTTPMETHODTYPE.GET,
                RequestHeaders = new Dictionary<string,string>()
                {
                    {"Authorization",  $"Bearer {result.AccessToken}"}
                }
            };


            var response = await Utilities.MakeHttpRequest(request).ConfigureAwait(false);
            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// This method is used for getting log history details 
        /// </summary>
        /// <param Name="requestBody"></param>
        /// <returns></returns>
        [Route("history"), HttpPost]
        public async Task<IActionResult> GetLogHistory([FromBody] LogHistoryRequestBody requestBody)
        {
            var methodBegin = Utility.LogBegin();
            var sessionId = Utility.GetSessionId(User);
            var startGrpResponseObj = await _loghistory.GetLogHistory(requestBody).ConfigureAwait(false);
            Utility.LogEnd(methodBegin);
            return Ok(startGrpResponseObj.Response);
        }

        [Route("layoutresponse"), HttpGet]
        public IActionResult GetDrawingLink([FromQuery]int groupId)
        {
            return Ok(_fieldDrawingAutomation.GetLDResponseJson(groupId));
        }
    }

}
