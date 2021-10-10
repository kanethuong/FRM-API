using System;
using System.Collections.Generic;
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
        public DateTime CreatedAt { get; set; }
        public bool? IsDeactivated { get; set; }
        public DateTime DeactivatedAt { get; set; }

        // One-Many role
        public int RoleId { get; set; }
        public Role Role { get; set; }

        // Many-One class
        public ICollection<Class> Classes { get; set; }

        // Many-One feedback
        public ICollection<TrainerFeedback> TrainerFeedbacks { get; set; }
    }
}