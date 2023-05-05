using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TKE.SC.Common.Caching.CPQCacheManger.Interface
{
    public interface ICacheManager
    {

        /// <summary>
        /// Get Cache
        /// </summary>
        /// <param name="distributedCache"></param>
        /// <param name="sessionGuid"></param>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        string GetCache(string sessionGuid, string key1, string key2);
        /// <summary>
        /// Get Cache
        /// </summary>
        /// <param name="distributedCache"></param>
        /// <param name="sessionGuid"></param>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="key3"></param>
        /// <returns></returns>
        string GetCache(string sessionGuid, string key1, string key2, string key3);

        void Remove(string key);
        void RemoveCache(string sessionGuid, string key1, string key2, string key3);
        /// <summary>
        /// Get Cache Async
        /// </summary>
        /// <param name="distributedCache"></param>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        string GetCacheAsync(string cacheKey);
        /// <summary>
        /// Set Cache
        /// </summary>
        /// <param name="distributedCache"></param>
        /// <param name="cacheOptions"></param>
        /// <param name="sessionGuid"></param>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value"></param>
        void SetCache(string sessionGuid, string key1,
            string key2, string value);
        /// <summary>
        /// Set Cache
        /// </summary>
        /// <param name="distributedCache"></param>
        /// <param name="cacheOptions"></param>
        /// <param name="sessionGuid"></param>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="key3"></param>
        /// <param name="value"></param>
        void SetCache(string sessionGuid,
            string key1, string key2, string key3, string value);

        void SetCache(string sessionGuid,
            string key1, string key2, string key3, string value, ILogger logger);
        void ExtendUserSession(string environment, string sessionGuid);
    }
}
