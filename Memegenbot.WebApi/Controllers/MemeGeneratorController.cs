using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Memegenbot.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemeGeneratorController : ControllerBase
    {
        private const string REQUEST_ERROR = "Request error occured";
        private readonly ILogger<MemeGeneratorController> _logger;
        private readonly ImgflipMemeGenerator _generator;

        public MemeGeneratorController(ILogger<MemeGeneratorController> logger, ImgflipMemeGenerator generator)
        {
            _logger = logger;
            _generator = generator;
        }

        [HttpGet]
        public string Get([FromQuery] string text)
        {
            var (success, data, errorMessage) = _generator.CreateRandomMeme(text);
            
            if (success)
                return data.url;
            
            _logger.LogError(errorMessage);
            return REQUEST_ERROR;
        }
    }
}