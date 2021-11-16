using System;

namespace kroniiapi.DTO.CostDTO
{
    public class CostResponse
    {
        public int CostId { get; set; }
        public string Content { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CostType { get; set; }
        public string Creator { get; set; }
    }
}