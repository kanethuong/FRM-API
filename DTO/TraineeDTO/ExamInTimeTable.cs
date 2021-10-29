using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TraineeDTO
{
    public class ExamInTimeTable
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public DateTime Date { get; set; }
        public int DurationInMinute { get; set; }
    }
}