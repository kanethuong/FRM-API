using System;
using System.ComponentModel.DataAnnotations;

namespace kroniiapi.DTO.ClassDTO
{
    public class AssignModuleInput
    {
        [Required]
        public int ClassId { get; set; }
        [Required]
        public int ModuleId { get; set; }
        [Required]
        [Range(0.0, Double.MaxValue, ErrorMessage = "Weight number must greater than 0")]
        public double WeightNumber { get; set; }
        [Required]
        public int TrainerId { get; set; }


    }
}