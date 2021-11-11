using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TimetableDTO
{
    public class ModuleExcelInput
    {
        public int Day { set; get; }
        public string ModuleName { set; get; }
        public int Duration_mins { set; get; }
    }
}