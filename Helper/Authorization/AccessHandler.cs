using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace kroniiapi.Requirements
{
    public class AccessHandler : AuthorizationHandler<AccessRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessRequirement requirement)
        {
            var user = context.User;

            // Get username to validate
            var username = context.User.FindFirst("username");

            // Check if user claim matching all requirements
            bool claimCheck = true;

            var claim = context.User.FindFirst(requirement.Role);
            if (!(claim != null && claim.Value.Equals(requirement.Role, StringComparison.OrdinalIgnoreCase)))
            {
                claimCheck = false;
            }

            // True 
            if (claimCheck == true)
            {
                context.Succeed(requirement);
            }

            // False
            return Task.CompletedTask;
        }
    }
}