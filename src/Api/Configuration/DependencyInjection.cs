using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using template_net7.Core.Application.Services.Auth;
using template_net7.Core.Application.Services.Users;
using template_net7.Core.Application.Services.Wheather;
using template_net7.Core.Domain.Adapters.Auth;
using template_net7.Core.Domain.Adapters.Users;
using template_net7.Core.Domain.Adapters.Wheather;
using template_net7.Core.Domain.Ports;
using template_net7.Infrastructure.Database;
using template_net7.Infrastructure.Repositories;
using template_net7.Shared.Infrastructure.EnvironmentVariables;

namespace template_net7.Api.Configuration;

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
        services.AddScoped<IAuthService, AuthenticationService>();
        services.AddScoped<IUserService, UserService>();

        // Register repositories
        services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();
        services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Register database services
        services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();

        return services;
    }
}