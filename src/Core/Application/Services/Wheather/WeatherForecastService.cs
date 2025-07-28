using template_net7.Core.Domain.Adapters.Wheather;
using template_net7.Core.Domain.Entities;
using template_net7.Core.Domain.Ports;

namespace template_net7.Core.Application.Services.Wheather;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly IWeatherForecastRepository _weatherForecastRepository;

    public WeatherForecastService(IWeatherForecastRepository weatherForecastRepository)
    {
        _weatherForecastRepository = weatherForecastRepository;
    }

    public async Task<IEnumerable<WeatherForecast>> GetForecast()
    {
        return await  _weatherForecastRepository.GetForecastsAsync();
    }
}