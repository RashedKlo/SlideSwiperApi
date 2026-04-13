using Application.Common.Interfaces;
using Infrastructure.Caching;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── PostgreSQL ─────────────────────────────────────────

        var dbUrl = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Database connection string is missing.");

     
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(dbUrl));

        services.AddScoped<ISlideRepository, SlideRepository>();

        // ── Redis ──────────────────────────────────────────────
        // Converts Railway's redis:// URL to StackExchange format
        var redisUrl = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Redis connection string is missing.");

        var redisConfig = ConvertRedisUrl(redisUrl);

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConfig));

        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }


    /// <summary>
    /// Converts redis://user:password@host:port
    /// to host:port,password=password,ssl=true,abortConnect=false
    /// </summary>
    private static string ConvertRedisUrl(string url)
    {
        // If it's already in host:port format, return as is
        if (!url.StartsWith("redis://") && !url.StartsWith("rediss://"))
            return url + ",abortConnect=false";

        var uri = new Uri(url);
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 6379;
        var ssl = url.StartsWith("rediss://");

        var password = string.Empty;
        if (!string.IsNullOrEmpty(uri.UserInfo))
        {
            var parts = uri.UserInfo.Split(':');
            password = parts.Length > 1 ? parts[1] : parts[0];
        }

        var config = $"{host}:{port},abortConnect=false";
        if (!string.IsNullOrEmpty(password))
            config += $",password={password}";
        if (ssl)
            config += ",ssl=true";

        return config;
    }
}
