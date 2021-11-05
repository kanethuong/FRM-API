using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ClassDTO
{
    public class AssignModuleInput
    {
        [Required]
        public int ClassId { get; set; }
        public int ModuleId { get; set; }
        public int TrainerId { get; set; }

    }
}