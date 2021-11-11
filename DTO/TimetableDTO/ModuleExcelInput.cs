using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TimetableDTO
{
    public class ModuleExcelInput
    {
        public int Day { set; get; }
        public IEnumerable<DayContent> DayContents { set; get; }
    }
    public class DayContent
    {
        public string DeliveryType { set; get; }
        public int Duration_mins { set; get; }
    }
}