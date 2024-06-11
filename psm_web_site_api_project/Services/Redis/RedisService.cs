using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace psm_web_site_api_project.Services.Redis;

public class RedisService(IDistributedCache cache, IConfiguration configuration) : IRedisService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IDistributedCache _cache = cache;
    private string _dataSerializable = null!;

    public async Task<List<T>?> GetData<T>(string key)
    {
        var redisList = await _cache.GetAsync(key);

        if (redisList != null)
        {
            _dataSerializable = Encoding.UTF8.GetString(redisList);
            return JsonConvert.DeserializeObject<List<T>>(_dataSerializable);
        }
        return default;
    }

    public async Task<bool> SetData<T>(string key, List<T> value)
    {
        var absoluteExpiration = _configuration.GetSection("Clients:Redis:absoluteExpiration").Value;
        var slidingExpiration = _configuration.GetSection("Clients:Redis:slidingExpiration").Value;
        var options = new DistributedCacheEntryOptions()
               .SetAbsoluteExpiration(DateTime.Now.AddMinutes(double.Parse(absoluteExpiration)))
               .SetSlidingExpiration(TimeSpan.FromMinutes(double.Parse(slidingExpiration)));
        _dataSerializable = JsonConvert.SerializeObject(value);
        byte[] newData = Encoding.ASCII.GetBytes(_dataSerializable);
        await _cache.SetAsync(key, newData, options);
        return true;
    }

    public async Task RemoveData(string key)
    {
        var _isKeyExist = await _cache.GetAsync(key);
        if (_isKeyExist != null)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
