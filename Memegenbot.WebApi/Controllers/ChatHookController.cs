using System;
using Memegenbot.WebApi.Integration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// ReSharper disable InconsistentNaming

namespace Memegenbot.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatHookController : ControllerBase
    {
        private const string ERROR_MESSAGE = "Failed to create a meme :(";
        private const string CFG_BOT_TOKEN = "Telegram:botToken";

        private readonly ILogger<MemeGeneratorController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ImgflipMemeGenerator _generator;

        public ChatHookController(ILogger<MemeGeneratorController> logger, IConfiguration configuration, ImgflipMemeGenerator generator)
        {
            _logger = logger;
            _configuration = configuration;
            _generator = generator;
        }

        [HttpPost]
        // ReSharper disable once UseDeconstructionOnParameter
        public void Post([FromBody] Telegram.Update update)
        {
            try
            {
                if (update.message.text is {Length: 0})
                {
                    _logger.LogDebug($"Empty message received: {update}");
                    return;
                }

                var (success, data, errorMessage) = _generator.CreateRandomMeme(update.message.text);

                if (!success)
                {
                    _logger.LogError(errorMessage);
                    return;
                }

                Telegram.SendMessage(_configuration[CFG_BOT_TOKEN], update.message.chat.id, data.url);
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e.ToString());
                Telegram.SendMessage(_configuration[CFG_BOT_TOKEN], update.message.chat.id, ERROR_MESSAGE);
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e.ToString());
                Telegram.SendMessage(_configuration[CFG_BOT_TOKEN], update.message.chat.id, ERROR_MESSAGE);
            }
        }
    }
}