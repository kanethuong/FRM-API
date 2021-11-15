using System.Collections.Generic;
using kroniiapi.DTO.AdminDTO;
using kroniiapi.DTO.ModuleDTO;

namespace kroniiapi.DTO.ClassDTO
{
    public class TrainerClassDetailResponse
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public ICollection<ModuleResponse> Modules { get; set; }
        public AdminResponse Admin { get; set; }
        public IEnumerable<TraineeInclassResponse> Trainees { get; set; }
    }
}