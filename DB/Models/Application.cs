using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DB.Models
{
    public class Application
    {
        public int ApplicationId { get; set; }
        public string Description { get; set; }
        public string ApplicationURL { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? IsAccepted { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public string Response { get; set; }

        // One-Many trainee
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }

        // One-Many admin 
        public int? AdminId { get; set; }
        public Admin Admin { get; set; }

        // One-Many category
        public int ApplicationCategoryId { get; set; }
        public ApplicationCategory ApplicationCategory { get; set; }
    }
}