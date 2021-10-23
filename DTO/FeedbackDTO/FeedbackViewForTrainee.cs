using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.FeedbackDTO
{
    public class FeedbackViewForTrainee
    {
        public TrainerInFeedbackResponse trainer { get; set; }
        public AdminInFeedbackResponse admin { get; set; }
    }
}