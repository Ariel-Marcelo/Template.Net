using template_net7.Core.Domain.Entities;
using template_net7.Core.Domain.Ports;

namespace template_net7.Infrastructure.Repositories;

public class WeatherForecastRepository : IWeatherForecastRepository
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastRepository> _logger;

    public WeatherForecastRepository(ILogger<WeatherForecastRepository> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<WeatherForecast>> GetForecastsAsync()
    {
        _logger.LogInformation("Generating weather forecast");

        var forecast = Enumerable.Range(1, 5).Select(index =>
            {
                var temperature = Random.Shared.Next(-20, 55);
                var summary = Summaries[Random.Shared.Next(Summaries.Length)];

                _logger.LogDebug("Generated forecast: {Temperature}°C, {Summary}", temperature, summary);

                return new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = temperature,
                    Summary = summary
                };
            })
            .ToArray();

        _logger.LogInformation("Successfully generated {Count} weather forecasts", forecast.Length);
        return await Task.FromResult(forecast);
    }
}