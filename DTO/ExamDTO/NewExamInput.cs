using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.DTO.ExamDTO
{
    public class NewExamInput
    {
        public string ExamName { get; set; }
        public string Description { get; set; }
        public DateTime ExamDay { get; set; }
        public int DurationInMinute { get; set; }
        public int AdminId { get; set; }
        public IEnumerable<int> TraineeIdList { get; set; }
        public int ModuleId { get; set; }
        public int? classId {get;set;}
    }
}