using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DB.Models
{
    public class Trainer
    {
        public int TrainerId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string AvatarURL { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DOB { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Facebook { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeactivated { get; set; } = false;
        public DateTime? DeactivatedAt { get; set; }

        // One-Many role
        public int RoleId { get; set; }
        public Role Role { get; set; }

        // Many-One class
        public ICollection<ClassModule> ClassModules { get; set; }
    }
}