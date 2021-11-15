using System.ComponentModel.DataAnnotations;

namespace kroniiapi.DTO.ClassDTO
{
    public class RequestDeleteClassInput
    {
        [StringLength(300, ErrorMessage = "Reason must be less than 300 characters")]
        public string Reason { get; set; }
        public int ClassId { get; set; }
        public int AdminId { get; set; }
    }
}