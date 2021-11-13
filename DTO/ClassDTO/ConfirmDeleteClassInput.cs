using System.ComponentModel.DataAnnotations;

namespace kroniiapi.DTO.ClassDTO
{
    public class ConfirmDeleteClassInput
    {
        [Required]
        public int ClassId { get; set; }
        [Required]
        public bool IsDeactivate { get; set; }
    }
}