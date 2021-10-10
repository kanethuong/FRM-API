using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ClassDTO
{
    public class ConfirmDeleteClassInput
    {
        [Required]
        public int ClassId { get; set; }
        [Required]
        public int DeleteClassRequestId { get; set; }
        [Required]
        public bool IsDeactivate { get; set; }
    }
}