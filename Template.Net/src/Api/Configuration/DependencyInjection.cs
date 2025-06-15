using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Template.Core.Domain.Interfaces;
using Template.Core.Domain.Ports;
using Template.Core.Application.Services;
using Template.Shared.Infrastructure.Settings;
using Template.Infrastructure.Repositories;
using Template.Infrastructure.Database;
using Microsoft.Extensions.Options;

namespace Template.Api.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Get configuration from the service provider
        var sp = services.BuildServiceProvider();
        var configuration = sp.GetRequiredService<IConfiguration>();

        // Configure settings first
        var dbSettings = new DatabaseSettings();
        configuration.GetSection("DatabaseSettings").Bind(dbSettings);

        if (string.IsNullOrEmpty(dbSettings.SqlConnection))
        {
            throw new InvalidOperationException("Database connection string is not configured in appsettings.json");
        }

        services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));

        // Register application services
        services.AddScoped<IWeatherForecastService, WeatherForecastService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserService, UserService>();

        // Register repositories
        services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();
        services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Register database services
        services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure JWT settings
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // Add JWT authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings?.Issuer,
                ValidAudience = jwtSettings?.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(jwtSettings?.SecretKey ?? throw new InvalidOperationException("JWT SecretKey is not configured")))
            };
        });

        return services;
    }
}