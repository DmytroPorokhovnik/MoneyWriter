using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoneyConverterController : ControllerBase
    {
        private readonly ILogger<MoneyConverterController> _logger;

        public MoneyConverterController(ILogger<MoneyConverterController> logger)
        {
            _logger = logger;
        }

        //[HttpGet(Name = "")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //}
    }
}