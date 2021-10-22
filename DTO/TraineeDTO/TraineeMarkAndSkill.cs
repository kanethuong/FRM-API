using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TraineeDTO
{
    public class TraineeMarkAndSkill
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public string IconURL { get; set; }
        public float Score { get; set; }
        public string CertificateURL { get; set; }
    }
}