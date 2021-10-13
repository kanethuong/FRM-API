using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DB.Models
{
    public class CompanyRequest
    {
        public int CompanyRequestId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string ReportURL { get; set; }
        public bool? IsAccepted { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        // Many-Many trainee by request detail
        public ICollection<Trainee> Trainees { get; set; }
        public ICollection<CompanyRequestDetail> CompanyRequestDetails { get; set; }
    }
}