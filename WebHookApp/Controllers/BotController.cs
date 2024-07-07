using DomainLogic;
using DomainLogic.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Telegram.Bot.Types;
using WebHookApp.AuthenticationTelegram;

namespace WebHookApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        [HttpPost("{token}")]
        public async Task<IActionResult> Post([FromServices] HandleUpdateService handleUpdateService,
                                          [FromBody] Update update,
                                          [FromRoute] string token)
        {
            await handleUpdateService.EchoAsync(update, token);
            return Ok();
        }

        [HttpPost("getToken")]
        public async Task<IActionResult> GetToken([FromServices] BotConfiguration _botConfiguration, [FromForm] Dictionary<string, string> data)
        {
            if(TelegramHashValidator.IsValidWebAppData(data, _botConfiguration.BotToken))
            {
                return Ok(new 
                { 
                    access_token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data))) 
                });
            }

            return BadRequest();
        }
    }
}