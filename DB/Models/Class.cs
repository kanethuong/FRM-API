using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace kroniiapi.DB.Models
{
    public class Class
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        // Many-Many module
        public ICollection<Module> Modules { get; set; }
        public ICollection<ClassModule> ClassModules { get; set; }

        // Many-One delete class request
        public ICollection<DeleteClassRequest> DeleteClassRequests { get; set; }

        // Many-One class
        public ICollection<Calendar> Calendars { get; set; }
    }
}