using System;
using System.Collections.Generic;

namespace kroniiapi.DB.Models
{
    public class Class
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime StartDay { get; set; } = DateTime.Now;
        public DateTime EndDay { get; set; }
        public bool IsDeactivated { get; set; } = false;
        public DateTime? DeactivatedAt { get; set; }

        // Many-One trainee
        public ICollection<Trainee> Trainees { get; set; }

        // One-Many admin
        public int AdminId { get; set; }
        public Admin Admin { get; set; }

        // One-Many trainer
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }

        // One-Many room
        public int RoomId { get; set; }
        public Room Room { get; set; }

        // Many-Many module
        public ICollection<Module> Modules { get; set; }
        public ICollection<ClassModule> ClassModules { get; set; }

        // One-One delete class request
        public DeleteClassRequest DeleteClassRequest { get; set; }

        // Many-One class
        public ICollection<Calendar> Calendars { get; set; }
    }
}