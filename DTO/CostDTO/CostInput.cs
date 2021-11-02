using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.CostDTO
{
    public class CostInput
    {
        public string Content { get; set; }
        public decimal Amount { get; set; }
        public int CostTypeId { get; set; }
        public int AdminId { get; set; }
    }
}