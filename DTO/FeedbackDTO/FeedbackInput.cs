using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.FeedbackDTO
{
    public class FeedbackInput
    {
        [Required]
        public int TraineeId { get; set; }
        public int TopicContent { get; set; }
        public int TopicObjective { get; set; }
        public int ApproriateTopicLevel { get; set; }
        public int TopicUsefulness { get; set; }
        public int TrainingMaterial { get; set; }
        public int TrainerKnowledge { get; set; }
        public int SubjectCoverage { get; set; }
        public int InstructionAndCommunicate { get; set; }
        public int TrainerSupport { get; set; }
        public int Logistics { get; set; }
        public int InformationToTrainees { get; set; }
        public int AdminSupport { get; set; }
        public string OtherComment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}