using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace ApiNetCoreUsingRedisDemo.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public ResponseCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
        }

        /// <summary>
        /// Gets the cached response asynchronous.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns></returns>
        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey)) return null;

            return await _distributedCache.GetStringAsync(cacheKey);
        }

        /// <summary>
        /// Removes the cache response asynchronous.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <exception cref="System.ArgumentNullException">Value cannot be null or whitespace.</exception>
        public async Task RemoveCacheResponseAsync(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException("Value cannot be null or whitespace.");

            //remove all "*"
            await foreach(var key in GetKeyAsync(pattern + "*"))
            {
                await _distributedCache.RemoveAsync(key);
            }
        }

        /// <summary>
        /// Gets the key asynchronous.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        private async IAsyncEnumerable<string> GetKeyAsync(string pattern)
        {
            foreach (var endPoint in _connectionMultiplexer.GetEndPoints())
            {
                var server = _connectionMultiplexer.GetServer(endPoint);

                foreach (var key in server.Keys(pattern: pattern))
                {
                    yield return key.ToString();
                }
            }
        }

        /// <summary>
        /// Sets the cache response asynchronous.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="response">The response.</param>
        /// <param name="timeOut">The time out.</param>
        public async Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeOut)
        {
            if (string.IsNullOrWhiteSpace(cacheKey) || response == null) return;

            var serializerResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            await _distributedCache.SetStringAsync(cacheKey, serializerResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeOut
            });
        }
    }
}
