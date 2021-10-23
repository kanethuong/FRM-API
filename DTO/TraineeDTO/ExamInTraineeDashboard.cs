using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TraineeDTO
{
    public class ExamInTraineeDashboard
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public DateTime ExamDay { get; set; }
        public int DurationInMinute { get; set; }
    }
}