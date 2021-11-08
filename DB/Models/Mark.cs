using System;
using System.ComponentModel.DataAnnotations;

namespace kroniiapi.DB.Models
{
    public class Mark
    {
        public int ModuleId { get; set; }
        public Module Module { get; set; }
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }
        [Range(0, 10, ErrorMessage = "Mark or Score can only from 0 to 10")]
        public float Score { get; set; }
        public DateTime PublishedAt { get; set; } = DateTime.Now;
    }
}