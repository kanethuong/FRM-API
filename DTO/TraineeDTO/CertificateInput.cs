using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TraineeDTO
{
    public class CertificateInput
    {
        [Required]
        public int ModuleId { get; set; }
        [Required]
        public int TraineeId { get; set; }
        [Required]
        public string CertificateURL { get; set; }
    }
}