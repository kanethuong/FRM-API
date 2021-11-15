using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.BonusAndPunishDTO
{
    public class BonusAndPunishInput
    {
        [Required]
        public float Score { get; set; }
        [Required]
        public string Reason { get; set; }       
        [Required]
        public int TraineeId { get; set; }
    }
}