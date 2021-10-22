using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.FeedbackDTO
{
    public class AdminFeedbackInput
    {
        [Required]
        public int Rate { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public int TraineeId { get; set; }
        [Required]
        public int AdminId { get; set; }
    }
}