using Microsoft.AspNetCore.Authorization;

namespace kroniiapi.Requirements
{
    public class AccessRequirement : IAuthorizationRequirement
    {
        public string Role { get; set; }
        public AccessRequirement(string role)
        {
            Role = role;
        }
    }
}