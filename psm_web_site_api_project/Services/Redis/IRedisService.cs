namespace psm_web_site_api_project.Services.Redis;

public interface IRedisService
{
    Task<List<T>> GetData<T>(string key);
    Task<T>? GetDataSingle<T>(string key);
    Task<bool> SetData<T>(string key, List<T> value);
    Task<bool> SetDataSingle<T>(string key, T value);
    Task RemoveData(string key);
}