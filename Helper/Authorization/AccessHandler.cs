using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace kroniiapi.Requirements
{
    public class AccessHandler : AuthorizationHandler<AccessRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessRequirement requirement)
        {
            var principal = context.User;

            // Get email and role from access token
            var claims = principal.Identities.First().Claims.ToList();
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var role = claims?.FirstOrDefault(c => c.Type.EndsWith("role", StringComparison.CurrentCultureIgnoreCase))?.Value;

            if (role != null && role.Equals(requirement.Role, StringComparison.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
            }

            // False
            return Task.CompletedTask;
        }
    }
}