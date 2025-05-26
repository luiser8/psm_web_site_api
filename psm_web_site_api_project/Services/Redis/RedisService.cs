using System.Runtime.InteropServices;
using Newtonsoft.Json;
using psm_web_site_api_project.Services.Config;
using StackExchange.Redis;

namespace psm_web_site_api_project.Services.Redis;

public class RedisService : IRedisService, IDisposable
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _cache;
    private readonly RedisConfiguration _config;

    public RedisService(IConfiguration configuration)
    {
        _config = new RedisConfiguration(configuration);

        try
        {
            var configOptions = new ConfigurationOptions
            {
                EndPoints = { configuration["Clients:Redis:host"] ?? "localhost:6379" },
                AbortOnConnectFail = false,
                ConnectTimeout = 10000,
                SyncTimeout = 10000,
                ResponseTimeout = 10000,
                ReconnectRetryPolicy = new LinearRetry(2000),
                KeepAlive = 180,
                ConnectRetry = 5,
                AllowAdmin = true,
                SocketManager = new SocketManager("CustomManager",
                    useHighPrioritySocketThreads: true)
            };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                configOptions.SocketManager = SocketManager.ThreadPool;
                configOptions.CommandMap = CommandMap.Create(
                [
                    "INFO", "CONFIG", "CLUSTER", "PING", "ECHO", "CLIENT"
                ], false);
            }

            _redis = ConnectionMultiplexer.Connect(configOptions);
            _cache = _redis.GetDatabase();

            _redis.ConnectionFailed += (sender, args) =>
                Console.WriteLine($"Fallo de conexión Redis: {args.Exception}");

            _redis.ConnectionRestored += (sender, args) =>
                Console.WriteLine("Conexión Redis restaurada");

            _redis.ErrorMessage += (sender, args) =>
                Console.WriteLine($"Mensaje de error Redis: {args.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al conectar con Redis: {ex.Message}");
            throw;
        }
    }

    public async Task<List<T>> GetData<T>(string key)
    {
        try
        {
            var redisValue = await _cache.StringGetAsync(key);
            return redisValue.HasValue
                ? JsonConvert.DeserializeObject<List<T>>(redisValue) ?? []
                : [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener datos de Redis para la clave {key}: {ex.Message}");
            return [];
        }
    }

    public async Task<T?> GetDataSingle<T>(string key)
    {
        try
        {
            var redisValue = await _cache.StringGetAsync(key);
            return redisValue.HasValue
                ? JsonConvert.DeserializeObject<T>(redisValue)
                : default;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener dato único de Redis para la clave {key}: {ex.Message}");
            return default;
        }
    }

    public async Task<bool> SetData<T>(string key, List<T> value)
    {
        try
        {
            return await _cache.StringSetAsync(
                key,
                JsonConvert.SerializeObject(value),
                _config.AbsoluteExpiration);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al establecer datos en Redis para la clave {key}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SetDataSingle<T>(string key, T value)
    {
        try
        {
            return await _cache.StringSetAsync(
                key,
                JsonConvert.SerializeObject(value),
                _config.ShortExpiration);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al establecer dato único en Redis para la clave {key}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RemoveData(string key)
    {
        try
        {
            return await _cache.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar dato de Redis para la clave {key}: {ex.Message}");
            return false;
        }
    }

    public void Dispose()
    {
        _redis?.Close();
        _redis?.Dispose();
        GC.SuppressFinalize(this);
    }
}