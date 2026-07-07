using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace TurnosApi.Infrastructure;

/// <summary>
/// Extension methods para configurar Redis como caché distribuido.
/// </summary>
public static class RedisConfig
{
    /// <summary>
    /// Registra IDistributedCache con StackExchange.Redis usando configuración de appsettings.
    /// Sección esperada: Redis:Configuration y Redis:InstanceName.
    /// </summary>
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:Configuration"];
            options.InstanceName = configuration["Redis:InstanceName"];
        });

        return services;
    }
}
