using System;
using System.ComponentModel.DataAnnotations;

namespace kroniiapi.DTO.AccountDTO
{
    public class AccountInput
    {
        [Required]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Username must be from 6 to 32 characters")]
        public string Username { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Fullname must be from 6 to 100 characters")]
        public string Fullname { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Email must be from 6 to 100 characters")]
        public string Email { get; set; }
        [Required]
        public string Role { get; set; }
        public string Phone { get; set; } = "";
        public DateTime DOB { get; set; } = default(DateTime);
        public string Address { get; set; } = "";
        public string Gender { get; set; } = "";
        public string Facebook { get; set; } = "";
    }
}