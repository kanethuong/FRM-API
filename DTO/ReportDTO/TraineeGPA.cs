using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ReportDTO
{
    public class TraineeGPA
    {
        public int TraineeId { get; set; }
        public float AcademicMark { get; set; }
        public float DisciplinaryPoint { get; set; }
        public float Bonus { get; set; }
        public float Penalty { get; set; }
        public float GPA { get; set; }
        public string Level { get; set; }
    }
}