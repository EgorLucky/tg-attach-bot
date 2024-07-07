using Microsoft.AspNetCore.Authorization;

namespace WebHookApp.AuthorizationPolicies.User
{
    public class RegistredUserRequirement : IAuthorizationRequirement
    {
        public bool RegistredUserRequired { get; }

        public RegistredUserRequirement(bool registredUserRequired)
        {
            RegistredUserRequired = registredUserRequired;
        }
    }
}
