using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.CompanyDTO
{
    public class RequestTraineeInput
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public IEnumerable<TraineeInRequestDetail> Trainees { get; set; }

        public string Content { get; set; }
    }
}