using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DB.Models
{
    public class Attendance
    {
        public int CalendarId { get; set; }
        public Calendar Calendar { get; set; }
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }
        public bool? IsAbsent { get; set; }
        public string Reason { get; set; }
    }
}