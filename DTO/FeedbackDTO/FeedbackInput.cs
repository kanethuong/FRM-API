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
        [Range(1, 5, ErrorMessage = "Feedback score can only from 1 to 5")]
        public int TopicContent { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Feedback score can only from 1 to 5")]
        public int TopicObjective { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Feedback score can only from 1 to 5")]
        public int ApproriateTopicLevel { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Feedback score can only from 1 to 5")]
        public int TopicUsefulness { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Feedback score can only from 1 to 5")]
        public int TrainingMaterial { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Feedback score can only from 1 to 5")]
        public int TrainerKnowledge { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Feedback score can only from 1 to 5")]
        public int SubjectCoverage { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Feedback score can only from 1 to 5")]
        public int InstructionAndCommunicate { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Feedback score can only from 1 to 5")]
        public int TrainerSupport { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Feedback score can only from 1 to 5")]
        public int Logistics { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Feedback score can only from 1 to 5")]
        public int InformationToTrainees { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Feedback score can only from 1 to 5")]
        public int AdminSupport { get; set; }
        [Required]
        [StringLength(300, ErrorMessage = "Comment must be less than 300 characters")]
        public string OtherComment { get; set; }
        [Required]
        public int TraineeId { get; set; }
    }
}