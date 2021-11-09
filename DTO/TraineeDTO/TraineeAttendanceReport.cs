using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TraineeDTO
{
    public class TraineeAttendanceReport
    {
        //A (Absent), P (Present), L (Late), Ln (Late no Reason), E (Early), En (Early no Reason) ,An (Absent no reason), Ob (OnBoard)
        public int NumberOfSlotAbsent { get; set; } 
        public int NumberOfSlotPresent { get; set; } 
        public int NumberOfSlotLate { get; set; } 
        public int NumberOfSlotLateNoReason { get; set; } 
        public int NumberOfSlotEarly { get; set; } 
        public int NumberOfSlotEarlyNoReason { get; set; } 
        public int NumberOfSlotAbsentNoReason { get; set; } 
    }
}