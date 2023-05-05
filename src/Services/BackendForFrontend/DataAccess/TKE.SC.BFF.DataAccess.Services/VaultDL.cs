using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TKE.SC.BFF.DataAccess.Helpers;
using TKE.SC.BFF.DataAccess.Interfaces;
using TKE.SC.Common.Model.ExceptionModel;
using TKE.SC.Common.Model.HttpClientModel;
using TKE.SC.Common.Model.VaultModel;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.Common.Model;
using Constant = TKE.SC.BFF.DataAccess.Helpers.Constant;
namespace TKE.SC.BFF.DataAccess.Services
{
    public class VaultDL : IVaultDL
    {
        private readonly IConfiguration _configuration;
        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param Name="logger"></param>
        public VaultDL(IConfiguration configuration, ILogger<VaultDL> logger, ICacheManager cacheManager)
        {
            Utility.SetLogger(logger);
            _configuration = configuration;
            _cacheManager = cacheManager;
        }

        public async Task<string> GetToken()
        {
            var methodBeginTime = Utility.LogBegin();
            var mFileSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constant.VAULTSETTINGS);

            var requestObject = new HttpClientRequestModel()
            {
                BaseUrl = Utility.GetPropertyValue(mFileSettings, Constant.BASEURL),
                EndPoint = Utility.GetPropertyValue(mFileSettings, Constant.TOKENAPI),
                RequestBody = JObject.FromObject(new Dictionary<string, string>
                {
                    {Constant.APIUSERNAME.ToLower(),Utility.GetPropertyValue(mFileSettings, Constant.APIUSERNAME) },
                     {Constant.APIPASSWORD.ToLower(), Utility.GetPropertyValue(mFileSettings, Constant.APIPASSWORD) },
                     {Constant.VAULTGUID.ToLower(), Utility.GetPropertyValue(mFileSettings, Constant.VAULTGUID) }
                }),
                Method = HTTPMETHODTYPE.POST,

            };

            var apiResponse = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(false);
            ValidateApiSuccessStatus(apiResponse);

            var accessToken = string.Empty;
            JObject respObj = JObject.Parse(apiResponse.Content.ReadAsStringAsync().Result);

            if (respObj["Value"] != null)
            {
                accessToken = Convert.ToString(respObj["Value"]);
            }
            Utility.LogEnd(methodBeginTime);
            return accessToken;
        }

        public async Task<string> GetFolderPath(string projectId)
        {
            var methodBeginTime = Utility.LogBegin();
            var location = string.Empty;
            var mFileSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constant.VAULTSETTINGS);
            var accessToken = await GetToken().ConfigureAwait(false);

            var requestObject = new HttpClientRequestModel()
            {
                BaseUrl = Utility.GetPropertyValue(mFileSettings, Constant.BASEURL),
                EndPoint = Utility.GetPropertyValue(mFileSettings, Constant.FOLDERAPI),
                RequestString = (new JArray() { projectId }).ToString(),
                Method = HTTPMETHODTYPE.POST,
                PostAs = "text/plain",
                ContentType = "text/plain",
                RequestHeaders = new Dictionary<string, string>
                        {
                            { Constant.XAUTHENTICATION, accessToken }
                        }
            };

            var apiResponse = await Utility.MakeHttpRequest(requestObject).ConfigureAwait(false);
            ValidateApiSuccessStatus(apiResponse);

            location = apiResponse.Content.ReadAsStringAsync().Result;
            _cacheManager.SetCache(Constant.VAULTSETTINGS, Constant.FOLDERAPI, projectId, location);
            Utility.LogEnd(methodBeginTime);
            return location;

        }

        private void ValidateApiSuccessStatus(HttpResponseMessage apiResponse)
        {
            if (apiResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new ExternalCallException(new ResponseMessage()
                {
                    StatusCode = (int)apiResponse.StatusCode,
                    Description = apiResponse.ReasonPhrase,
                    Message = Constant.VAULTEXCEPTION
                });
            }
        }

        public async Task<string> UploadDocument(byte[] fileStream, VaultUploadModel uploadInfo)
        {
            var methodBegin = Utility.LogBegin();
            var mFileSettings = Utility.GetSection(Utility.GetSection(_configuration, Constant.PARAMSETTINGS), Constant.VAULTSETTINGS);

            var token = await GetToken().ConfigureAwait(false);
            var baseUrl = Utility.GetPropertyValue(mFileSettings, Constant.BASEURL);
            var endPoint = Utility.GetPropertyValue(mFileSettings, Constant.UPLOADAPI);
            var url = $"{baseUrl}{endPoint}"; 

            var jsonPayload = new Dictionary<string, string>()
            {
                { "json", JsonConvert.SerializeObject(uploadInfo.Payload)  }
            };

            var headers = new Dictionary<string, string>()
            {
                { Constant.XAUTHENTICATION, token }
            };
            string boundary = "----------4aed9369a67e4e41ad5d607b00c639f1";

            var fileUploadInfo = new
            {
                Name = uploadInfo.Payload[0].FileName,
                Mime = $"application/{uploadInfo.Payload[0].FileExtension}",
                Stream = fileStream
            };
            var result = Utility.MultiFormUploadFileApi(url, headers, boundary, jsonPayload, fileUploadInfo, fileStream );
            Utility.LogEnd(methodBegin);
            return result;
        }

    }
}
