using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ReportDTO
{
    public class TraineeGrades
    {
        public int TopicId { get; set; }
        public int TraineeId { get; set; }
        public float Score { get; set; }
    }
}