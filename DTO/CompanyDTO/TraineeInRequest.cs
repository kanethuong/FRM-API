using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.CompanyDTO
{
    public class TraineeInRequest
    {
        public int TraineeId { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }

    }
}