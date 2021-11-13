using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.BonusAndPunishDTO
{
    public class TraineeInBP
    {
        public int TraineeId { get; set; }
        public string Fullname { get; set; }
        public string AvatarURL { get; set; }
        public int? ClassId { get; set; }
        public string ClassName { get; set; }
    }
}