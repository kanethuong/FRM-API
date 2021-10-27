using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ClassDetailDTO
{
    public class ModuleResponse
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string IconURL { get; set; }
        public string SyllabusURL { get; set; }
        public int NoOfSlot { get; set; }
        public string Description { get; set; }
    }
}