using System.Collections.Generic;

namespace kroniiapi.DB.Models
{
    public class CostType
    {
        public int CostTypeId { get; set; }
        public string CostTypeName { get; set; }
        public ICollection<Cost> Costs { get; set; }
    }
}