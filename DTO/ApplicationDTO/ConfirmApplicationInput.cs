using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ApplicationDTO
{
    public class ConfirmApplicationInput
    {
        [Required]
        public int AdminId { get; set; }
        [Required]
        public string Response { get; set; }
        [Required]
        public bool IsAccepted { get; set; }
    }
}