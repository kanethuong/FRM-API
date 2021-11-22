using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.CompanyDTO
{
    public class CompanyProfileDetailInput
    {
        [Required]
        public string Fullname { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Facebook { get; set; }
    }
}