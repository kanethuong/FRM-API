using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace kroniiapi.Services
{
    public class CacheProvider : ICacheProvider
    {
        private readonly IRedisCacheClient _cache;

        public CacheProvider(IRedisCacheClient cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Get a value (include class data) in cache using key
        /// </summary>
        /// <typeparam name="T">A class</typeparam>
        public async Task<T> GetFromCache<T>(string key) where T : class
        {
            var cacheRes = await _cache.Db0.GetAsync<T>(key);
            return cacheRes == null ? null : cacheRes;
        }

        /// <summary>
        /// Insert new data (include class data) to cache databse
        /// </summary>
        /// <typeparam name="T">A class</typeparam>
        public async Task SetCache<T>(string key, T value) where T : class
        {
            bool added = await _cache.Db0.AddAsync(key, value);
        }

        /// <summary>
        /// Add value to existing key
        /// </summary>
        /// <typeparam name="T">A class</typeparam>
        public async Task AddValueToKey<T>(string key, T value) 
        {
            LinkedList<T> valueList = new LinkedList<T>();
            try
            {
                valueList = await GetFromCache<LinkedList<T>>(key);
                valueList.AddLast(value);
            }catch(Exception e)
            {
                valueList = new LinkedList<T>();
                valueList.AddLast(value);
            }
            await SetCache<LinkedList<T>>(key,valueList);
        }

        /// <summary>
        /// Clear value in cache using key
        /// </summary>
        /// <param name="key">What to clear</param>
        public async Task ClearCache(string key)
        {
            await _cache.Db0.RemoveAsync(key);
        }
    }
}