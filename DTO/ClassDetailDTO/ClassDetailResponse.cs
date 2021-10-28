using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DTO.PaginationDTO;

namespace kroniiapi.DTO.ClassDetailDTO
{
    public class ClassDetailResponse
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public ICollection<ModuleResponse> Modules { get; set; }
        public TrainerResponse Trainer { get; set; }
        public AdminResponse Admin { get; set; }
    }
}