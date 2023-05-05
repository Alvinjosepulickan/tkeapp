using TKE.SC.Common.Caching.CPQCacheManger.Interface;
using TKE.SC.Common.Caching.Helper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace TKE.SC.Common.Caching
{
    public class CpqCacheManager : ICacheManager
    {
        #region Variables
        private static IConfigurationRoot _configurationRoot;
        private static string _environment;
        private static DistributedCacheEntryOptions _cacheOptions;
        #endregion

        public ILogger Logger { get; set; }
        private readonly IDistributedCache _distributedCache;

        public CpqCacheManager(IDistributedCache distributedCache)


        {
            _distributedCache = distributedCache;
            if (_configurationRoot == null)
            {
                _configurationRoot = new ConfigurationBuilder().
                  SetBasePath(System.IO.Directory.GetCurrentDirectory()).AddJsonFile(Constant.APPSETTINGS).Build();
            }
            _environment = _configurationRoot.GetSection(Constant.PARAMSETTINGS)[Constant.ENVIRONMENT];
            _cacheOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(Double.Parse(_configurationRoot.GetSection(Constant.PARAMSETTINGS)
                    [Constant.CACHESLIDINGTIMEOUT], CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// GetCache
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="distributedCache"></param>
        /// <returns></returns>
        public string GetCache(string sessionGuid, string key1, string key2)
        {
            return GetCache(sessionGuid, key1, key2, string.Empty);
        }

        /// <summary>
        /// GetCache Method
        /// </summary>
        /// <param name="distributedCache"></param>
        /// <param name="sessionGuid"></param>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="key3"></param>
        /// <returns></returns>
        public string GetCache(string sessionGuid, string key1, string key2, string key3)
        {
            if (_distributedCache == null)
            {
                throw new CacheException(new ResponseMessage
                {
                    StatusCode = Constant.NOTFOUND,
                    Message = Constant.DISTRIBUTEDCACHENULLREDISNOTINITIALIZED + " distributedCache is null "

                });
            }

            var userKey = string.Empty;
            userKey = (string.IsNullOrEmpty(key3)) ? BuildUserKey(key1, sessionGuid)
                : BuildUserKeyWithParentCode(key1, sessionGuid, key3);
            var objectInCache = GetCacheAsync(userKey);
            if (objectInCache != null)
            {
                var userCache = DeserializeObjectValue<UserCache>(objectInCache);
                var cacheKey = BuildCacheKey(key2, key3);
                if (userCache != null)
                {
                    var keyValue = userCache?.CachedObjects?.Where(pair => string.Equals(pair.Key, cacheKey, StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault().Value;
                    return keyValue;
                }
            }
            return null;
        }

        /// <summary>
        /// GetCacheAsync method
        /// </summary>
        /// <param name="distributedCache"></param>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public string GetCacheAsync(string cacheKey)
        {
            if (_distributedCache == null)
            {
                throw new CacheException(new ResponseMessage
                {
                    StatusCode = Constant.NOTFOUND,
                    Message = Constant.DISTRIBUTEDCACHENULLREDISNOTINITIALIZED + " distributedCache is null "

                });
            }
            return _distributedCache.GetString(cacheKey);
        }

        /// <summary>
        /// Sets the cache
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <param name="cacheOptions"></param>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value"></param>
        /// <param name="distributedCache"></param>
        public void SetCache(string sessionGuid, string key1, string key2, string value)
        {
            SetCache(sessionGuid, key1, key2, string.Empty, value);
        }

        public void Remove(string key)
        {
            _distributedCache.Remove(key);
        }
        public void RemoveCache(string sessionGuid, string key1, string key2, string key3)
        {
            //Removing the previous quote Id data in cache
            var userKey = string.Empty;
            userKey = (string.IsNullOrEmpty(key3)) ? BuildUserKey(key1, sessionGuid)
                : BuildUserKeyWithParentCode(key1, sessionGuid, key3);
            var objectInCache = GetCacheAsync(userKey);
            if (!string.IsNullOrEmpty(objectInCache))
            {
                var userCache = DeserializeObjectValue<UserCache>(objectInCache);
                var cacheKey = BuildCacheKey(key2.ToLower(), key3);
                if (userCache.CachedObjects.ContainsKey(cacheKey))
                {
                    userCache.CachedObjects.Remove(cacheKey);
                }
                _distributedCache.Remove(cacheKey);
                _distributedCache.SetString(userKey, SerializeObjectValue(userCache), _cacheOptions);
            }
        }

        /// <summary>
        /// Set Cache
        /// </summary>
        /// <param name="distributedCache"></param>
        /// <param name="cacheOptions"></param>
        /// <param name="sessionGuid"></param>
        /// <param name="Cachekeys">key</param>
        /// <param name="value">CPQ session model object</param>
        public void SetCache(string sessionGuid, string key1, string key2, string key3, string value, ILogger logger)
        {
            Logger = logger;
            SetCache(sessionGuid, key1, key2, key3, value);
        }
        public void SetCache(string sessionGuid, string key1, string key2, string key3, string value)
        {
            DateTime startTime = DateTime.Now;
            //if (cacheOptions != null)
            //{
            //    _cacheOptions = cacheOptions;
            //}
            if (_distributedCache == null)
            {
                throw new CacheException(new ResponseMessage
                {
                    StatusCode = Constant.NOTFOUND,
                    Message = Constant.DISTRIBUTEDCACHENULLREDISNOTINITIALIZED + " distributedCache is null "

                });
            }
            var userKey = string.Empty;
            userKey = (string.IsNullOrEmpty(key3)) ? BuildUserKey(key1, sessionGuid)
                : BuildUserKeyWithParentCode(key1, sessionGuid, key3, true);
            var objectInCache = GetCacheAsync(userKey);
            //Logger?.LogInformation("----> User Key :" + userKey);
            if (objectInCache == null)
            {
                var userCache = new UserCache();

                var cacheKey = BuildCacheKey(key2, key3);
                if (userCache.CachedObjects == null)
                {
                    userCache.CachedObjects = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
                }
                userCache.CachedObjects.Add(cacheKey, value);
                //var endTime2 = DateTime.Now;
                //Logger?.LogInformation("Inside Set Cache) Adding key to Cache call time: " + (endTime2 - startTime2).TotalSeconds.ToString());


                //needs to be removed after logging
                //var startTime3 = DateTime.Now;
                //var test = SerializeObjectValue(userCache);
                //var endTime3 = DateTime.Now;

                //Logger?.LogInformation("----> Cache size :" + test.Length);
                //Logger?.LogInformation("----> Cache count :" + userCache.CachedObjects.Keys.Count);

                //Logger?.LogInformation("(Inside Set Cache) Serialize Cache object call time: " + (endTime3 - startTime3).TotalSeconds.ToString());

                //var startTime1 = DateTime.Now;
                _distributedCache.SetString(userKey, SerializeObjectValue(userCache), _cacheOptions);
                //var endTime1 = DateTime.Now;
                //Logger?.LogInformation("(Inside Set Cache) Setting Value to Cache call time: " + (endTime1 - startTime1).TotalSeconds.ToString());
                //NEED TO BE REMOVED
                //try
                //{
                //    //List<string> keys = userCache.CachedObjects.Keys.ToList<string>();
                //    //Logger?.LogInformation("----> Cache List fo Keys :" + string.Join(",", keys));
                //    var s = "";
                //    userCache.CachedObjects.Keys.ToList().ForEach(key => { s += key + ":" + userCache.CachedObjects[key].Length + ","; });
                //    Logger?.LogInformation("----> Cache List fo [Keys:Size,] : " + s);
                //}
                //catch (Exception)
                //{
                //}
            }
            else
            {
                var userCache = DeserializeObjectValue<UserCache>(objectInCache);
                var cacheKey = BuildCacheKey(key2, key3);
                if (userCache.CachedObjects.ContainsKey(cacheKey))
                {
                    userCache.CachedObjects.Remove(cacheKey);
                }
                userCache.CachedObjects.Add(cacheKey, value);
                //var endTime2 = DateTime.Now;
                //Logger?.LogInformation("(Inside Set Cache) Adding key to Cache call time: " + (endTime2 - startTime2).TotalSeconds.ToString());


                //needs to be removed after logging
                //var startTime3 = DateTime.Now;
                //var test = SerializeObjectValue(userCache);
                //var endTime3 = DateTime.Now;
                //Logger?.LogInformation("----> Cache Key :" + cacheKey);
                //Logger?.LogInformation("----> Cache size :" + test.Length);
                //Logger?.LogInformation("----> Cache count :" + userCache.CachedObjects.Keys.Count);

                //Logger?.LogInformation("(Inside Set Cache) Serialize Cache object call time: " + (endTime3 - startTime3).TotalSeconds.ToString());

                //DATETIME
                //var startTime1 = DateTime.Now;
                _distributedCache.Remove(cacheKey);
                _distributedCache.SetString(userKey, SerializeObjectValue(userCache), _cacheOptions);
                //var endTime1 = DateTime.Now;
                //Logger?.LogInformation("(Inside Set Cache) Setting Value to Cache call time: " + (endTime1 - startTime1).TotalSeconds.ToString());
                //NEED TO BE REMOVED
                //try
                //{
                //    //List<string> keys = userCache.CachedObjects.Keys.ToList<string>();
                //    //Logger?.LogInformation("----> Cache List fo Keys :" + string.Join(",", keys));
                //    var s = "";
                //    userCache.CachedObjects.Keys.ToList().ForEach(key => { s += key + ":" + userCache.CachedObjects[key].Length + ","; });
                //    Logger?.LogInformation("----> Cache List fo [Keys:Size,] : " + s);
                //}
                //catch (Exception)
                //{
                //}
            }
        }

        /// <summary>
        /// BuildUserKey
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="sessionGuid"></param>
        /// <returns></returns>
        private static string BuildUserKey(string environment, string sessionGuid)
        {
            string environmentValue = environment;
            if (string.IsNullOrEmpty(environmentValue))
                environmentValue = _environment.Trim();
            if (!string.IsNullOrEmpty(sessionGuid))
            {
                string userKeyFormat = Constant.CPQ + Constant.UNDERSCORE + "{0}" + Constant.UNDERSCORE + "{1}";
                return string.Format(userKeyFormat, environmentValue, sessionGuid);
            }
            return Constant.CPQ + Constant.UNDERSCORE + environmentValue;
        }

        /// <summary>
        /// BuildUserKey
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="sessionGuid"></param>
        /// <returns></returns>
        public string BuildUserKeyWithParentCode(
            string environment, string sessionGuid, string parentCode, bool setFlag = false)
        {
            string environmentValue = environment;
            if (string.IsNullOrEmpty(environmentValue))
            {
                environmentValue = _environment.Trim();

            }
            if (!string.IsNullOrEmpty(sessionGuid) && string.IsNullOrEmpty(parentCode))
            {
                string userKeyFormat = Constant.CPQ + Constant.UNDERSCORE + "{0}" + Constant.UNDERSCORE + "{1}";
                return string.Format(userKeyFormat, environmentValue, sessionGuid);
            }
            else if (!string.IsNullOrEmpty(sessionGuid) && !string.IsNullOrEmpty(parentCode))
            {
                string userKeyWithIdFormat = Constant.CPQ + Constant.UNDERSCORE + "{0}" + Constant.UNDERSCORE + "{1}" + Constant.UNDERSCORE + "{2}";
                string key = string.Format(userKeyWithIdFormat, environmentValue, sessionGuid, parentCode);
                if (setFlag)
                {
                    AddToKeyBunch(environment, sessionGuid, key);
                }
                return key;
            }
            return Constant.CPQ + Constant.UNDERSCORE + environmentValue;
        }

        /// <summary>
        /// BuildCacheKey
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyExtension"></param>
        /// <returns></returns>
        private static string BuildCacheKey(string key, string keyExtension)
        {
            string cacheKeyFormat = "{0} " + Constant.UNDERSCORE + "{1}";
            return (string.IsNullOrEmpty(keyExtension)) ? key.Trim() : string.Format(cacheKeyFormat, key.Trim(), keyExtension.Trim());
        }

        public void AddToKeyBunch(string environment, string sessionGuid, string newkey)
        {
            string key = BuildUserKey(environment, sessionGuid);
            // get dict
            var objectInCache = GetCacheAsync(key);
            if (objectInCache != null)
            {
                var userCache = DeserializeObjectValue<UserCache>(objectInCache);
                var cacheKey = BuildCacheKey(Constant.KEYBUNCH, string.Empty);
                if (userCache != null)
                {
                    // get keybuc
                    var keyValue = userCache?.CachedObjects?.Where(pair => string.Equals(pair.Key, cacheKey, StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault().Value;
                    // chkc for dup on key bunch
                    if (!string.IsNullOrEmpty(keyValue))
                    {
                        keyValue = keyValue.Contains(newkey) ? keyValue : keyValue + "," + newkey;
                    }
                    // add to keybu
                    if (userCache.CachedObjects.ContainsKey(cacheKey.ToLower()))
                    {
                        userCache.CachedObjects.Remove(cacheKey.ToLower());
                    }
                    userCache.CachedObjects.Add(cacheKey.ToLower(), keyValue);
                    _distributedCache.SetString(key, SerializeObjectValue(userCache), _cacheOptions);
                }
            }
        }

        /// <summary>
        /// DeserializeObjectValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DeserializeObjectValue<T>(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(string.Empty);
            }
        }

        /// <summary>
        /// SerializeObjectValue
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeObjectValue(object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        public void ExtendUserSession(string environment, string sessionGuid)
        {
            string key = BuildUserKey(environment, sessionGuid);
            // get dict
            var objectInCache = GetCacheAsync(key);
            if (!string.IsNullOrEmpty(objectInCache))
            {
                var userCache = DeserializeObjectValue<UserCache>(objectInCache);
                var cacheKey = BuildCacheKey(Constant.KEYBUNCH, string.Empty);
                if (userCache != null)
                {
                    // get keybuc
                    var keyValue = userCache?.CachedObjects?.Where(pair => string.Equals(pair.Key, cacheKey, StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault().Value;
                    if (!string.IsNullOrEmpty(keyValue) && keyValue.Contains(","))
                    {
                        var keyValueList = keyValue.Split(',');
                        foreach (var keyName in keyValueList)
                        {
                            GetCacheAsync(keyName);
                        }
                    }
                }
            }
        }
    }
}
