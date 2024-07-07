using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;

namespace WebHookApp.AuthenticationTelegram
{
    public class TelegramAuthenticationHandler: AuthenticationHandler<TelegramAuthenticationOptions>
    {
        private readonly BotConfiguration _botConfiguration;

        public TelegramAuthenticationHandler(
            IOptionsMonitor<TelegramAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            BotConfiguration botConfiguration)
        : base(options, logger, encoder)
        {
            _botConfiguration = botConfiguration;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                //Authorization header not in request
                return AuthenticateResult.NoResult();
            }

            if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out AuthenticationHeaderValue headerValue))
            {
                //Invalid Authorization header
                return AuthenticateResult.NoResult();
            }

            try
            {
                if ("Telegram".Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    var telegramDataJsonString = HttpUtility.UrlDecode(headerValue.Parameter);
                    var dictionaryData = JsonConvert.DeserializeObject<Dictionary<string, string>>(telegramDataJsonString);

                    var isValid = TelegramHashValidator.IsValid(dictionaryData, _botConfiguration.BotToken);

                    if (!isValid)
                    {
                        return AuthenticateResult.NoResult();
                    }

                    var claims = new Claim[] {
                        new Claim("userId", dictionaryData["id"] ?? ""),
                        new Claim("last_name", dictionaryData.GetValueOrDefault("last_name") ?? ""),
                        new Claim("first_name", dictionaryData.GetValueOrDefault("first_name") ?? ""),
                        new Claim(ClaimTypes.Surname, dictionaryData.GetValueOrDefault("last_name") ?? ""),
                        new Claim(ClaimTypes.Name, dictionaryData.GetValueOrDefault("first_name") ?? ""),
                        new Claim("username", dictionaryData.GetValueOrDefault("username") ?? ""),
                        new Claim(ClaimTypes.AuthenticationMethod, "Telegram")
                    };
                    var identity = new ClaimsIdentity(claims, "Telegram");
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, "Telegram");
                    return AuthenticateResult.Success(ticket);
                }
                else if ("TelegramWebAppData".Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    var telegramWebAppDataJsonString = Encoding.UTF8.GetString(Convert.FromBase64String(headerValue.Parameter));
                    var telegramWebAppDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(telegramWebAppDataJsonString);

                    var isValid = TelegramHashValidator.IsValidWebAppData(telegramWebAppDataDictionary, _botConfiguration.BotToken);

                    if (!isValid)
                    {
                        return AuthenticateResult.NoResult();
                    }

                    var user = JsonConvert.DeserializeObject<Dictionary<string, string>>(telegramWebAppDataDictionary["user"]);

                    var claims = new Claim[] {
                        new Claim("userId", user["id"] ?? ""),
                        new Claim("last_name", user["last_name"] ?? ""),
                        new Claim("first_name", user["first_name"] ?? ""),
                        new Claim(ClaimTypes.Surname, user["last_name"] ?? ""),
                        new Claim(ClaimTypes.Name, user["first_name"] ?? ""),
                        new Claim("username", user["username"] ?? ""),
                        new Claim(ClaimTypes.AuthenticationMethod, "Telegram")
                    };
                    var identity = new ClaimsIdentity(claims, "Telegram");
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, "Telegram");
                    return AuthenticateResult.Success(ticket);
                }
                else 
                { 
                    //Not Basic authentication header
                    return AuthenticateResult.NoResult();
                }
            }
            catch (Exception e)
            {
                return AuthenticateResult.Fail("Invalid acceessTOKEN");
            }

        }
    }
}
