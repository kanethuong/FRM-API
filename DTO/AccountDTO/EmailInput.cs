using System.ComponentModel.DataAnnotations;

namespace kroniiapi.DTO.AccountDTO
{
    public class EmailInput
    {
        [Required]
        [EmailAddress]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Email must be from 6 to 100 characters")]
        public string Email { get; set; }
    }
}