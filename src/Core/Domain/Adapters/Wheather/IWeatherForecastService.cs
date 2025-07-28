using template_net7.Core.Domain.Entities;

namespace template_net7.Core.Domain.Adapters.Wheather;

public interface IWeatherForecastService
{
    Task<IEnumerable<WeatherForecast>> GetForecast();
} 