using System;
using System.Collections.Generic;
using kroniiapi.DTO.AdminDTO;
using kroniiapi.DTO.ModuleDTO;
using kroniiapi.DTO.TrainerDTO;

namespace kroniiapi.DTO.ClassDTO
{
    public class ClassDetailResponse
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public ICollection<ModuleResponse> Modules { get; set; }
        public ICollection<TrainerResponse> Trainer { get; set; }
        public AdminResponse Admin { get; set; }
        public ICollection<String> RoomName { get; set; }

    }
}