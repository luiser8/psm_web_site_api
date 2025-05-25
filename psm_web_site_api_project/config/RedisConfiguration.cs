namespace psm_web_site_api_project.Services.Config;

public class RedisConfiguration
{
    public string Host { get; }
    public bool UseSsl { get; }
    public TimeSpan AbsoluteExpiration { get; }
    public TimeSpan ShortExpiration { get; }

    public RedisConfiguration(IConfiguration configuration)
    {
        Host = configuration.GetValue<string>("Clients:Redis:host") ?? "localhost:6379";
        UseSsl = configuration.GetValue<bool>("Clients:Redis:useSsl");

        var absExpMinutes = configuration.GetValue<double>("Clients:Redis:absoluteExpiration", 30);
        var shortExpMinutes = configuration.GetValue<double>("Clients:Redis:shortExpiration", 5);

        AbsoluteExpiration = TimeSpan.FromMinutes(absExpMinutes);
        ShortExpiration = TimeSpan.FromMinutes(shortExpMinutes);
    }
}
