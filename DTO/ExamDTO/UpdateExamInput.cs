using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ExamDTO
{
    public class UpdateExamInput
    {
        public int Duration { get; set; }
        public DateTime ExamDay { get; set; }
    }
}