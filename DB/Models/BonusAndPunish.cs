using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DB.Models
{
    public class BonusAndPunish
    {
        public int BonusAndPunishId { get; set; }
        public float Score { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }
    }
}