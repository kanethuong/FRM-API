using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.CompanyDTO
{
    public class CompanyRequestResponse
    {
        public int CompanyRequestId { get; set; }
        public string CompanyName { get; set; }
        public int NumberOfTrainee { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}