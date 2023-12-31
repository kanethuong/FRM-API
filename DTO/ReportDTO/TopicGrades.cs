using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ReportDTO
{
    public class TopicGrades
    {
        public ICollection<TopicInfo> TopicInfos { get; set; }
        public Dictionary<DateTime, List<TraineeGrades>> TraineeTopicGrades { get; set; }
        public ICollection<AverageScoreInfo> AverageScoreInfos { get; set; }
        public Dictionary<DateTime, List<TraineeGrades>> TraineeAverageGrades { get; set; }
        public FinalMarksInfo FinalMarksInfo{get; set;}
        public ICollection<TraineeGrades> FinalMarks { get; set; }
    }
}