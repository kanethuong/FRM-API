using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DB.Models
{
    public class AdminFeedback
    {
        public int AdminFeedbackId { get; set; }
        public int Rate { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // One-Many trainee
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }

        // One-Many admin
        public int AdminId { get; set; }
        public Admin Admin { get; set; }
    }
}