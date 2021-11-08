using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DB.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }

        // One-Many to Trainee
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }
    }
}