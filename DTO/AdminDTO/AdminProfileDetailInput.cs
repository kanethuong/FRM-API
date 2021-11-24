using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.AdminDTO
{
    public class AdminProfileDetailInput
    {
        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Fullname must be from 6 to 100 characters")]
        public string Fullname { get; set; }
        [Required]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Phone must be from 10 to 11 numbers")]
        public string Phone { get; set; }
        [Required]
        public DateTime DOB { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Facebook { get; set; }
    }
}