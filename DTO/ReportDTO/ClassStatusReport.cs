using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ReportDTO
{
    public class ClassStatusReport
    {
        public int Learning { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Deferred { get; set; }
        public int DropOut { get; set; }
        public int Cancel { get; set; }
    }
}