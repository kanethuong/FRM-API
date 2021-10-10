using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DB.Models
{
    public class CompanyRequestDetail
    {
        public int CompanyRequestId { get; set; }
        public CompanyRequest CompanyRequest { get; set; }
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }
    }
}