using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.AccountDTO
{
    public class EmailInput
    {
        [Required]
        public string Email { get; set; }
    }
}