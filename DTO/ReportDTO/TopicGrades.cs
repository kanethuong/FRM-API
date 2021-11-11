using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ReportDTO
{
    public class TopicGrades
    {
        public ICollection<TopicInfo> TopicInfos { get; set; }
        public Dictionary<int, List<TraineeGrades>> TraineeTopicGrades { get; set; }
        public ICollection<AverageScoreInfo> AverageScoreInfos { get; set; }
        public Dictionary<int, List<TraineeGrades>> TraineeAverageGrades { get; set; }
        public ICollection<TraineeGrades> FinalMarks { get; set; }
    }
}