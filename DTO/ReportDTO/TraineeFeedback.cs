using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ReportDTO
{
    public class TraineeFeedback
    {
        public float TraineeId { get; set; }
        public string Email { get; set; }
        public float TopicContent { get; set; }
        public float TopicObjective { get; set; }
        public float ApproriateTopicLevel { get; set; }
        public float TopicUsefulness { get; set; }
        public float TrainingMaterial { get; set; }
        public float TrainerKnowledge { get; set; }
        public float SubjectCoverage { get; set; }
        public float InstructionAndCommunicate { get; set; }
        public float TrainerSupport { get; set; }
        public float Logistics { get; set; }
        public float InformationToTrainees { get; set; }
        public float AdminSupport { get; set; }
        public string OtherComment { get; set; }
    }
}