using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using template_net7.Core.Domain.Adapters.Wheather;
using template_net7.Core.Domain.Entities;

namespace template_net7.Api.Controllers.Weather;

[Authorize]
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecastService _weatherForecastService;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(
        IWeatherForecastService weatherForecastService,
        ILogger<WeatherForecastController> logger)
    {
        _weatherForecastService = weatherForecastService;
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        _logger.LogInformation("Weather forecast requested at {Time}", DateTime.UtcNow);
        return await _weatherForecastService.GetForecast();
    }
}