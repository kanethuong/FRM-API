using System.ComponentModel.DataAnnotations;

namespace kroniiapi.DTO.ApplicationDTO
{
    public class ConfirmApplicationInput
    {
        [Required]
        public int AdminId { get; set; }
        [Required]
        [StringLength(300, ErrorMessage = "Response must be less than 300 characters")]
        public string Response { get; set; }
        [Required]
        public bool IsAccepted { get; set; }
    }
}