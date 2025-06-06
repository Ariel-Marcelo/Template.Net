using Template.Core.Domain.Entities;

namespace Template.Core.Domain.Ports;

public interface IWeatherForecastRepository
{
    Task<IEnumerable<WeatherForecast>> GetForecastsAsync();
}