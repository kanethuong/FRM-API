using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DB.Models
{
    public class Calendar
    {
        public int CalendarId { get; set; }
        public int SyllabusSlot { get; set; }
        public DateTime Date { get; set; }
        public int SlotInDay { get; set; }

        // One-Many module
        public int ModuleId { get; set; }
        public Module Module { get; set; }

        // One-Many class
        public int ClassId { get; set; }
        public Class Class { get; set; }

        // Many-Many trainee by attendance
        public ICollection<Attendance> Attendances { get; set; }

    }
}