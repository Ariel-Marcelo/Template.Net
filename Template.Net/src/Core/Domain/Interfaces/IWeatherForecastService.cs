using Template.Core.Domain.Entities;

namespace Template.Core.Domain.Interfaces;

public interface IWeatherForecastService
{
    Task<IEnumerable<WeatherForecast>> GetForecast();
} 