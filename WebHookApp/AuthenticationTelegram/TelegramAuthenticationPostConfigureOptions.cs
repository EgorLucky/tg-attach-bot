using Microsoft.Extensions.Options;

namespace WebHookApp.AuthenticationTelegram
{
    public class TelegramAuthenticationPostConfigureOptions : IPostConfigureOptions<TelegramAuthenticationOptions>
    {
        public void PostConfigure(string name, TelegramAuthenticationOptions options)
        {
        }
    }
}
