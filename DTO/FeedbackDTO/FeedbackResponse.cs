using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.FeedbackDTO
{
    public class FeedbackResponse
    {
        public TrainerFeedbackResponse TrainerFeedback { get; set; }
        public AdminFeedbackResponse AdminFeedback { get; set; }
    }
}