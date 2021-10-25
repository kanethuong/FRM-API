using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.NotificationDTO
{
    public class NotifyMessage
    {
        [Required]
        [EmailAddress]
        public string User { get; set; }
        
        [Required]
        public string SendTo { get; set; }
        [Required]
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsSeen { get; set; }
    }
}