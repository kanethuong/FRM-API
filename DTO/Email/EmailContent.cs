using System.ComponentModel.DataAnnotations;

namespace kroniiapi.DTO.Email
{
    public class EmailContent
    {
        [Required]
        public string ToEmail { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public bool IsBodyHtml { get; set; }
        [Required]
        public string Body { get; set; }
    }
}