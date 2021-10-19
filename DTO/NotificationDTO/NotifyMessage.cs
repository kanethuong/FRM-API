using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.NotificationDTO
{
    public class NotifyMessage
    {
        [Required]
        [EmailAddress]
        public string user { get; set; }
        
        [Required]
        public string sendTo { get; set; }
        [Required]
        public string message { get; set; }
    }
}