using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ClassDTO
{
    public class TrainerModule
    {
        [Required]
        public int TrainerId { get; set; }
        [Required]
        public int ModuleId { get; set; }
        [Required]
        public float WeightNumber { get; set; }
    }
}