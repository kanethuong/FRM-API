using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace kroniiapi.DTO.NotificationDTO
{
    public class NotifyMessageToTraineeList
    {
        [Required]
        [EmailAddress]
        public string User { get; set; }
        
        [Required]
        public IEnumerable<string> SendTo { get; set; }
        [Required]
        public string Message { get; set; }
    }
}