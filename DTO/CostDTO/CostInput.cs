using System;
using System.ComponentModel.DataAnnotations;

namespace kroniiapi.DTO.CostDTO
{
    public class CostInput
    {
        [StringLength(300, ErrorMessage = "Content must be less than 300 characters")]
        public string Content { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal Amount { get; set; }
        public int CostTypeId { get; set; }
        public int AdminId { get; set; }
    }
}