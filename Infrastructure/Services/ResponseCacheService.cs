using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase database;

        public ResponseCacheService(IConnectionMultiplexer redis)
        {
            this.database = redis.GetDatabase();
        }

        // Put things to redis database
        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response == null) return;

            // Serialize json response
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var serializedResponse = JsonSerializer.Serialize(response, options);

            await this.database.StringSetAsync(cacheKey, serializedResponse, timeToLive);
        }

        // Get things out of redis database
        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            var cachedResponse = await this.database.StringGetAsync(cacheKey);

            if (cachedResponse.IsNullOrEmpty) return null;

            return cachedResponse;
        }
    }
}