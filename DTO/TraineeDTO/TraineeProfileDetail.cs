using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TraineeDTO
{
    public class TraineeProfileDetail
    {
        public string Fullname { get; set; }
        public string AvatarURL { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DOB { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public decimal Wage { get; set; }
        public string Facebook { get; set; }
        public string Status { get; set; }
    }
}