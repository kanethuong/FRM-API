using System;

namespace kroniiapi.DB.Models
{
    public class Certificate
    {
        public int ModuleId { get; set; }
        public Module Module { get; set; }
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }
        public string CertificateURL { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}