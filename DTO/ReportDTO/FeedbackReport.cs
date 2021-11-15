namespace kroniiapi.DTO.ReportDTO
{
    public class FeedbackReport
    {
        // Every selection report
        public float TopicContent { get; set; }
        public float TopicObjective { get; set; }
        public float ApproriateTopicLevel { get; set; }
        public float TopicUsefulness { get; set; }
        public float TrainingMaterial { get; set; }
        //
        public float TrainerKnowledge { get; set; }
        public float SubjectCoverage { get; set; }
        public float InstructionAndCommunicate { get; set; }
        public float TrainerSupport { get; set; }
        //
        public float Logistics { get; set; }
        public float InformationToTrainees { get; set; }
        public float AdminSupport { get; set; }

        // Group report

        public float TrainingPrograms { get; set; }
        public float Trainer { get; set; }
        public float Organization { get; set; }

        //  Eval
        public float ContantEval { get; set; }
        public float TrainerEval { get; set; }
        public float OrganizeEval { get; set; }
        public float OJTEval { get; set; }

        // Total report
        public float AverageScore { get; set; }
    }
}