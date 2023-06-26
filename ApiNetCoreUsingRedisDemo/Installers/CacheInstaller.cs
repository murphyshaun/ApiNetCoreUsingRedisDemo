using ApiNetCoreUsingRedisDemo.Configurations;
using ApiNetCoreUsingRedisDemo.Services;
using StackExchange.Redis;

namespace ApiNetCoreUsingRedisDemo.Installers
{
    public class CacheInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            var redisConfiguration = new RedisConfiguration();

            //binding data from appsetting.json to variable object RedisConfiguration
            configuration.GetSection("RedisConfiguration").Bind(redisConfiguration); 

            services.AddSingleton(redisConfiguration);

            if (!redisConfiguration.Enabled) return;

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConfiguration.ConnectionString));
            services.AddStackExchangeRedisCache(option => option.Configuration = redisConfiguration.ConnectionString);
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();

        }
    }
}
