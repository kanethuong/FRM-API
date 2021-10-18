using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DB.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string AvatarURL { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DOB { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        [Column(TypeName = "money")]
        public decimal Wage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeactivated { get; set; } = false;
        public DateTime? DeactivatedAt { get; set; }

        // One-Many role
        public int RoleId { get; set; }
        public Role Role { get; set; }

        // Many-One class
        public ICollection<Class> Classes { get; set; }

        // Many-One exam
        public ICollection<Exam> Exams { get; set; }

        // One-Many application
        public ICollection<Application> Applications { get; set; }

        // Many-One admin feedback
        public ICollection<AdminFeedback> AdminFeedbacks { get; set; }

        // Many-One cost
        public ICollection<Cost> Costs { get; set; }

        // Many-One delete class request
        public ICollection<DeleteClassRequest> DeleteClassRequests { get; set; }
    }
}