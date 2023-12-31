using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TraineeDTO
{
    public class ExamInTraineeDashboard
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int DurationInMinute { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public string AdminAvatarURL { get; set; }
        public string AdminEmail { get; set; }
        
    }
}