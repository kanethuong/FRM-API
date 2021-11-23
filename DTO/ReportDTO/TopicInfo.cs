using System;

namespace kroniiapi.DTO.ReportDTO
{
    public class TopicInfo
    {
        public int TopicId { get; set; }
        public DateTime Month { get; set; }
        public string Name { get; set; }
        public float MaxScore { get; set; }
        public float PassingScore { get; set; }
        public float WeightNumber { get; set; }
    }
}