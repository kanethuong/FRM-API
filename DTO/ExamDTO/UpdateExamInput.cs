using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ExamDTO
{
    public class UpdateExamInput
    {
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int Duration { get; set; }
        public DateTime ExamDay { get; set; }
    }
}