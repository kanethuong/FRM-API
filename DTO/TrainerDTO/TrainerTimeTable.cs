using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DTO.TrainerDTO;

namespace kroniiapi.DTO.TrainerDTO
{
    public class TrainerTimeTable
    {
        public int CalendarId { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public DateTime Date { get; set; }
        public int SlotDuration { get; set; }
        public string RoomName { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
    }
}