using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace kroniiapi.DB.Models
{
    public class Module
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public int NoOfSlot { get; set; }
        public TimeSpan SlotDuration { get; set; }
        public string IconURL { get; set; }
        public string SyllabusURL { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Range(0, 10, ErrorMessage = "Mark or Score can only from 0 to 10")]
        public float MaxScore { get; set; } = 10;
        [Range(0, 10, ErrorMessage = "Mark or Score can only from 0 to 10")]
        public float PassingScore { get; set; } = 6;

        // Many-Many class
        public ICollection<Class> Classes { get; set; }
        public ICollection<ClassModule> ClassModules { get; set; }

        // Many-Many trainee by mark
        public ICollection<Trainee> TraineesMark { get; set; }
        public ICollection<Mark> Marks { get; set; }

        // Many-Many trainee by certificate
        public ICollection<Trainee> TraineesCertificate { get; set; }
        public ICollection<Certificate> Certificates { get; set; }

        // Many-One Exam
        public ICollection<Exam> Exams { get; set; }

        // Many-One calendar
        public ICollection<Calendar> Calendars { get; set; }
    }

    public class ClassModule
    {
        public int ClassId { get; set; }
        public Class Class { get; set; }
        public int ModuleId { get; set; }
        public Module Module { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.Now;
        public float WeightNumber { get; set; }
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }
        public int? RoomId { get; set; }
        public Room Room { get; set; }
    }
}