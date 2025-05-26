using Newtonsoft.Json;
using StackExchange.Redis;

namespace psm_web_site_api_project.Services.Redis;

public class RedisService : IRedisService
{
    private readonly IDatabase _cache;
    private readonly IConfiguration _configuration;

    public RedisService(IConfiguration configuration)
    {
        _configuration = configuration;
        var redis = ConnectionMultiplexer.Connect(_configuration.GetSection("Clients:Redis:host").Value ?? string.Empty);
        _cache = redis.GetDatabase();
    }

    public async Task<List<T>> GetData<T>(string key)
    {
        var redisList = await _cache.StringGetAsync(key);

        if (!string.IsNullOrEmpty(redisList))
        {
            return JsonConvert.DeserializeObject<List<T>>(redisList) ?? [];
        }
        return [];
    }

    public async Task<T?> GetDataSingle<T>(string key)
    {
        var redisList = await _cache.StringGetAsync(key);

        if (!string.IsNullOrEmpty(redisList))
        {
            return JsonConvert.DeserializeObject<T>(redisList);
        }
        return default;
    }

    public async Task<bool> SetData<T>(string key, List<T> value)
    {
        var absoluteExpiration = _configuration.GetSection("Clients:Redis:absoluteExpiration").Value;
        return await _cache.StringSetAsync(key, JsonConvert.SerializeObject(value), TimeSpan.FromMinutes(Convert.ToDouble(absoluteExpiration)));
    }

    public async Task<bool> SetDataSingle<T>(string key, T value)
    {
        var shortExpiration = _configuration.GetSection("Clients:Redis:shortExpiration").Value;
        return await _cache.StringSetAsync(key, JsonConvert.SerializeObject(value), TimeSpan.FromMinutes(Convert.ToDouble(shortExpiration)));
    }

    public async Task<bool> RemoveData(string key)
    {
        var _isKeyExist = await _cache.KeyExistsAsync(key);
        if (_isKeyExist)
        {
            await _cache.KeyDeleteAsync(key);
        }
        return _isKeyExist;
    }
}