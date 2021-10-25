using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TraineeDTO
{
    public class ModuleInTraineeDashboard
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public DateTime Date { get; set; }
        public TraineeClassInfo Class { get; set; }
    }
}