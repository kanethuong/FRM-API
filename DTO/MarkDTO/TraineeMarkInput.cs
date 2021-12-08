using System;
using System.ComponentModel.DataAnnotations;

namespace kroniiapi.DTO.MarkDTO
{
    public class TraineeMarkInput
    {
        public int ModuleId { get; set; }
        public int TraineeId { get; set; }
        [Range(0, 10, ErrorMessage = "Score can only from 0 to 10")]
        public float Score { get; set; }

    }
}