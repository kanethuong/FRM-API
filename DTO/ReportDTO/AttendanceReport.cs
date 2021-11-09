using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ReportDTO
{
    public class AttendanceReport
    {
        public int TrainerId { get; set; }
        public int NumberOfAbsent { get; set; }
        public int NumberOfLateInAndEarlyOut { get; set; }
        public float NoPermissionRate { get; set; }
        public float DisciplinaryPoint { get; set; }
    }
}