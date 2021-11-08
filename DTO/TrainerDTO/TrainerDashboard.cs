using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TrainerDTO
{
    public class TrainerDashboard
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public DateTime Date { get; set; }
        public int SlotDuration { get; set; }
        public ClassRoom Class { get; set; }

    }
}