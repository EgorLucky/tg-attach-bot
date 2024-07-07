using Microsoft.AspNetCore.Authorization;

namespace WebHookApp.AuthorizationPolicies.User
{
    public class ValidTelegramUserRequirement : IAuthorizationRequirement
    {
        public bool RegistredTelegramUserRequired { get; }

        public ValidTelegramUserRequirement(bool validTelegramUserRequired)
        {
            RegistredTelegramUserRequired = validTelegramUserRequired;
        }
    }
}
