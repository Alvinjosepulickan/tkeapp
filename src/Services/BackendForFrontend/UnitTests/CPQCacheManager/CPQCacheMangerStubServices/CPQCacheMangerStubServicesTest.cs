using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.BFF.DataAccess.Helpers;

namespace TKE.SC.BFF.Test.CPQCacheManager.CPQCacheMangerStubServices
{
    public class CPQCacheMangerStubServicesTest : ICacheManager
    {

        public string GetCache(string sessionGuid = null, string key1 = null, string key2 = null)
        {
            return GetCache(sessionGuid, key1, key2, string.Empty);
            //return string.Empty;
        }

        public string GetCache(string sessionGuid = "", string key1 = "", string key2 = "", string key3 = "")
        {
            if (sessionGuid == null)
            {
                return null;
            }
            var cacheValue = string.Empty;
            if (string.IsNullOrEmpty(key2))
                key2 = string.Empty;
            if (string.IsNullOrEmpty(key3))
                key3 = string.Empty;
            //if (File.Exists(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.INPUTJSONPATH, "session" + key2 + key3 + ".json")))
            if (File.Exists(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.INPUTJSONPATH, sessionGuid + key2 + key3 + ".json")))
                cacheValue = File.ReadAllText(Path.Combine(Constant.APPGATEWAY_INPUTJSON_PATH, Constant.INPUTJSONPATH, sessionGuid + key2 + key3 + ".json"));
            return cacheValue;
        }

        public string GetCache(string cacheKey = null)
        {
            return GetCache(null, null, null, string.Empty);
            //return string.Empty;
        }

        public Task<string> GetCacheAsync(string cacheKey = null)
        {
            return null;
        }

        public void RefreshCache(string cacheKey = null)
        {
        }
        public void RemoveCache(string sessionGuid, string key1, string key2, string key3)
        {
        }
        public void SetCache(string sessionGuid = null, string key1 = null, string key2 = null, string value = null)
        {
        }

        public void SetCache(string sessionGuid = null, string key1 = null, string key2 = null, string key3 = null, string value = null)
        {
        }
        public void SetCache(string sessionGuid = null, string key1 = null, string key2 = null, string key3 = null, string value = null, ILogger logger = null)
        {
        }
        public void ExtendUserSession(string environment, string sessionGuid)
        {
        }

        public void Remove(string key)
        {
            throw new System.NotImplementedException();
        }

        string ICacheManager.GetCacheAsync(string cacheKey)
        {
            throw new NotImplementedException();
        }
    }
}
