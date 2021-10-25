using Microsoft.AspNetCore.Authorization;

namespace kroniiapi.Requirements
{
    public class AccessRequirement : IAuthorizationRequirement
    {
        public string[] Roles { get; set; }
        public AccessRequirement(string[] roles)
        {
            Roles = roles;
        }
    }
}