using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TraineeDTO
{
    public class TraineeResponse
    {
        public int TraineeId { get; set; }
        public string Fullname { get; set; }
        public string AvatarURL { get; set; }
        public string Email { get; set; }
        public DateTime DOB { get; set; }
        public decimal Wage { get; set; }
        public int ClassId { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }

    }
}