using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.CompanyDTO
{
    public class TraineeInResponse
    {
        public string AvatarURL { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal Wage { get; set; }
    }
}