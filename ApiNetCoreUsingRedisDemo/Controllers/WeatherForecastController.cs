using ApiNetCoreUsingRedisDemo.Attributes;
using ApiNetCoreUsingRedisDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiNetCoreUsingRedisDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly IResponseCacheService _responseCacheService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IResponseCacheService responseCacheService)
        {
            _logger = logger;
            _responseCacheService = responseCacheService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        [Cache]
        public async Task<IActionResult> Get(string? textSearch = null, int page = 1, int pageSize = 20)
        {
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update()
        {
            Summaries[0] = "FreezingUpdate";

            //remove cached
            await _responseCacheService.RemoveCacheResponseAsync(HttpContext.Request.Path);

            return Ok();
        }
    }
}