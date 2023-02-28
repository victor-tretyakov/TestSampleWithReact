using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestSampleWithReact.Models;

namespace TestSampleWithReact.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private double[] UserData = new[]
        {
            -1.0, -3.0, -1.5, 4, 5.0, 16, 18, 19, 15, 6, 3, -0.5
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var firstMonth = new DateTime(DateTime.Now.Year, 1, 1);

            return Enumerable.Range(1, 12).Select(index => {
                var temperature = Random.Shared.Next(-20, 55);
                return new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(firstMonth.AddMonths(index)),
                    TemperatureC = temperature,
                    Summary = GetSummariesByTemperature(temperature)
                };
            })
            .ToArray();
        }


        [HttpGet("{userId}")]
        public IEnumerable<WeatherForecast> GetByUserId(string userId)
        {
            var firstMonth = new DateTime(DateTime.Now.Year, 1, 1);

            return Enumerable.Range(0, 12).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(firstMonth.AddMonths(index)),
                TemperatureC = UserData[index],
                Summary = GetSummariesByTemperature(UserData[index])
            });
        }

        private string GetSummariesByTemperature(double temperature)
        {
            return temperature switch
            {
                (<= -30) => "Freezing",
                (> -30) and (<= 0) => "Cold",
                (> 0) and (<= 10) => "Normal",
                (> 10) and (<= 18) => "Chill",
                (> 18) and (<= 25) => "Warm",
                (> 25) and (<= 30) => "Hot",
                (> 30) => "Scorching",
                _ => string.Empty
            };
        }
    }
}