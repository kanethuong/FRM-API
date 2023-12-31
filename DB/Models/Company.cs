using System;
using System.Collections.Generic;

namespace kroniiapi.DB.Models
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string AvatarURL { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public string Facebook { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeactivated { get; set; } = false;
        public DateTime? DeactivatedAt { get; set; }

        // One-Many role
        public int RoleId { get; set; }
        public Role Role { get; set; }

        // Many-One company request
        public ICollection<CompanyRequest> CompanyRequests { get; set; }
    }
}