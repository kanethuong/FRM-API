using System;
using System.Collections.Generic;

namespace kroniiapi.DB.Models
{
    public class Exam
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public string Description { get; set; }
        public DateTime ExamDay { get; set; }
        public int DurationInMinute { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsCancelled { get; set; } = false;
        public DateTime? CancalledAt { get; set; }

        // One-Many module
        public int ModuleId { get; set; }
        public Module Module { get; set; }

        // One-Many admin
        public int AdminId { get; set; }
        public Admin Admin { get; set; }

        // Many-Many trainee
        public ICollection<Trainee> Trainees { get; set; }
        public ICollection<TraineeExam> TraineeExams { get; set; }

        // One-Many room
        public int? RoomId { get; set; }
        public Room Room { get; set; }
    }

    public class TraineeExam
    {
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }
        public int ExamId { get; set; }
        public Exam Exam { get; set; }
    }
}