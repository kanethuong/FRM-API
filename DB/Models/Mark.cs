using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DB.Models
{
    public class Mark
    {
        public int ModuleId { get; set; }
        public Module Module { get; set; }
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }
        public float Score { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}