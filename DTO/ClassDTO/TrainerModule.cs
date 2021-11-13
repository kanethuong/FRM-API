using System.ComponentModel.DataAnnotations;

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