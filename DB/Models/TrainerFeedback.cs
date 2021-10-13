using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DB.Models
{
    public class TrainerFeedback
    {
        public int TrainerFeedbackId { get; set; }
        public int Rate { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }
    }
}