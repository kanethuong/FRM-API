using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.MarkDTO
{
    public class TraineeMarkInput
    {
        public int ModuleId { get; set; }
        public int TraineeId { get; set; }
        public float Score { get; set; }

    }
}