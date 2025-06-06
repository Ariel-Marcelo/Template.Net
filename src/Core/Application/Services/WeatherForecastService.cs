using Template.Core.Domain.Interfaces;
using Template.Core.Domain.Entities;
using Template.Core.Domain.Ports;

namespace Template.Core.Application.Services;

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