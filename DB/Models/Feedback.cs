using System;

namespace kroniiapi.DB.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
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

        // One-Many trainee
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }
    }
}