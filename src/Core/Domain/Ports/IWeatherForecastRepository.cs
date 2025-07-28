using template_net7.Core.Domain.Entities;

namespace template_net7.Core.Domain.Ports;

public interface IWeatherForecastRepository
{
    Task<IEnumerable<WeatherForecast>> GetForecastsAsync();
}