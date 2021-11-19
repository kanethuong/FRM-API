using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.DTO.CompanyDTO
{
    public class TraineeSearchResponse
    {
        public int TraineeId { get; set; }
        public string Fullname { get; set; }
        public string AvatarURL { get; set; }
        public ICollection<string> ModuleNames { get; set; }
        public string RoleName { get; set; }
    }
}