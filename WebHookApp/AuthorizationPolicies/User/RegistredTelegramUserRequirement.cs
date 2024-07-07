using Microsoft.AspNetCore.Authorization;

namespace WebHookApp.AuthorizationPolicies.User
{
    public class RegistredTelegramUserRequirement : IAuthorizationRequirement
    {
        public bool RegistredTelegramUserRequired { get; }

        public RegistredTelegramUserRequirement(bool registredTelegramUserRequired)
        {
            RegistredTelegramUserRequired = registredTelegramUserRequired;
        }
    }
}
