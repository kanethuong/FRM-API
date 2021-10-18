using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.FeedbackDTO
{
    public class AdminFeedbackResponse
    {
        public AdminInFeedbackResponse Admin { get; set; }
        public ICollection<FeedbackContent> Feedbacks { get; set; }
    }
}