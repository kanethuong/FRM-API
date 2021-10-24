using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.FeedbackDTO
{
    public class AdminInFeedbackResponse
    {
        public int AdminId { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string AvatarURL { get; set; }
    }
}