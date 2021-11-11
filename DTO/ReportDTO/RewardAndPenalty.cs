using System;

namespace kroniiapi.DTO.ReportDTO
{
    public class RewardAndPenalty
    {
        public int TraineeId { get; set; }
        public DateTime Date { get; set; }
        public float BonusAndPenaltyPoint { get; set; }
        public string Reason { get; set; }
    }
}