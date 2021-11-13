using System.ComponentModel.DataAnnotations;

namespace kroniiapi.DTO.ClassDTO
{
    public class RemoveModuleInput
    {
        [Required]
        public int ClassId { get; set; }
        public int ModuleId { get; set; }
        public int TrainerId { get; set; }
    }
}