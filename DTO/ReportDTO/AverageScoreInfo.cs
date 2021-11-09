using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ReportDTO
{
    public class AverageScoreInfo
    {
        public int TopicId { get; set; }
        public DateTime Month { get; set; }
        public float MaxScore { get; set; }
        public float PassingScore { get; set; }
        public float WeightNumber { get; set; }
    }
}