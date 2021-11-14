using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.BonusAndPunishDTO
{
    public class BonusAndPunishResponse
    {
        public TraineeInBP trainee { get; set; }
        public float Score { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}