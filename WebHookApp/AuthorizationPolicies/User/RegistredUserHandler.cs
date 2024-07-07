using DomainLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebHookApp.AuthorizationPolicies.User
{

    public class TelegramValidUserHandler : AuthorizationHandler<ValidTelegramUserRequirement>
    {
        private readonly AppDbContext _context;

        public TelegramValidUserHandler(AppDbContext context)
        {
            _context = context;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       ValidTelegramUserRequirement requirement)
        {
            var authenticationMethod = context
                            .User
                            .Claims
                            .Where(c => c.Type == ClaimTypes.AuthenticationMethod)
                            .Select(c => c.Value)
                            .FirstOrDefault();

            if (string.IsNullOrEmpty(authenticationMethod) || (authenticationMethod.ToLower() != "telegram" && authenticationMethod.ToLower() != "telegramwebappdata"))
            {
                return;
            }

            if (
                requirement.RegistredTelegramUserRequired)
            {
                context.Succeed(requirement);
            }
        }
    }
}
