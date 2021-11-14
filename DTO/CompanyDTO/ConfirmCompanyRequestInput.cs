using System.ComponentModel.DataAnnotations;

namespace kroniiapi.DTO.CompanyDTO
{
    public class ConfirmCompanyRequestInput
    {
        [Required]
        public bool isAccepted { get; set; }
    }
}