using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ClassDetailDTO
{
    public class TraineeInClassDetail
    {
        public int TraineeId { get; set; }
        public string Fullname { get; set; }
        public string AvatarURL { get; set; }
        public string Email { get; set; }
        public DateTime DOB { get; set; }
    }
}