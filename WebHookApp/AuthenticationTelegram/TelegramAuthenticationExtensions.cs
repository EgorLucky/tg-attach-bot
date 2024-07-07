using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using WebHookApp.AuthenticationTelegram;

namespace WebHookApp.AuthenticationTelegram
{
    public static class TelegramAuthenticationExtensions
    {
        public static AuthenticationBuilder AddTelegramWebAppScheme(this AuthenticationBuilder builder)
        {
            return AddTelegramWebAppScheme(builder, TelegramSchemeDefaults.AuthenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddTelegramWebAppScheme(this AuthenticationBuilder builder, string authenticationScheme)
        {
            return AddTelegramWebAppScheme(builder, authenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddTelegramWebAppScheme(this AuthenticationBuilder builder, Action<TelegramAuthenticationOptions> configureOptions)
        {
            return AddTelegramWebAppScheme(builder, TelegramSchemeDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddTelegramWebAppScheme(this AuthenticationBuilder builder, string authenticationScheme, Action<TelegramAuthenticationOptions> configureOptions)
        {
            builder.Services.AddSingleton<IPostConfigureOptions<TelegramAuthenticationOptions>,
                                TelegramAuthenticationPostConfigureOptions>();

            return builder.AddScheme<TelegramAuthenticationOptions, TelegramAuthenticationHandler>(
                authenticationScheme, configureOptions);
        }
    }
}
