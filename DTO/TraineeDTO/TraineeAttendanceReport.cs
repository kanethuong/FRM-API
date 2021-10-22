using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TraineeDTO
{
    public class TraineeAttendanceReport
    {
        public string ModuleName { get; set; }
        public int NoOfSlot { get; set; }
        public int  NumberSlotAbsent { get; set; }   
    }
}