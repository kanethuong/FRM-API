using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DB.Models
{
    public class Cost
    {
        public int CostId { get; set; }
        public string Content { get; set; }
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // One-Many cost type
        public int CostTypeId { get; set; }
        public CostType CostType { get; set; }

        // One-Many admin
        public int AdminId { get; set; }
        public Admin Admin { get; set; }
    }
}