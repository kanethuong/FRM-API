using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}