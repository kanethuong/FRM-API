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
        public ICollection<ModuleInClassDetail> Modules { get; set; }
        public TrainerInClassDetail Trainer { get; set; }
        public AdminInClassDetail Admin { get; set; }
    }
}