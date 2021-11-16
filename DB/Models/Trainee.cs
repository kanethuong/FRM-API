using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace kroniiapi.DB.Models
{
    public class Trainee
    {
        public int TraineeId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string AvatarURL { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; } = "";
        public DateTime DOB { get; set; } = default(DateTime);
        public string Address { get; set; } = "";
        public string Gender { get; set; } = "";
        public string Facebook { get; set; } = "";
        [Column(TypeName = "money")]
        public decimal Wage { get; set; }
        public string Status { get; set; }
        public bool OnBoard { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeactivated { get; set; } = false;
        public DateTime? DeactivatedAt { get; set; }

        // One-Many role
        public int RoleId { get; set; }
        public Role Role { get; set; }

        // One-Many class
        public int? ClassId { get; set; }
        public Class Class { get; set; }

        // Many-Many module by mark
        public ICollection<Module> ModulesMarks { get; set; }
        public ICollection<Mark> Marks { get; set; }

        // Many-many module by certificate
        public ICollection<Module> ModulesCertificate { get; set; }
        public ICollection<Certificate> Certificates { get; set; }

        // Many-Many exam
        public ICollection<Exam> Exams { get; set; }
        public ICollection<TraineeExam> TraineeExams { get; set; }

        // Many-One application
        public ICollection<Application> Applications { get; set; }

        // Many-One feedback
        public ICollection<Feedback> Feedbacks { get; set; }

        // Many-One Bonus and Punish
        public ICollection<BonusAndPunish> BonusAndPunishes { get; set; }

        // Many-Many calendar by attendance
        public ICollection<Attendance> Attendances { get; set; }

        // Many-Many company request
        public ICollection<CompanyRequest> CompanyRequests { get; set; }
        public ICollection<CompanyRequestDetail> CompanyRequestDetails { get; set; }
    }
}