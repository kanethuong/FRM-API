using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ApplicationDTO
{
    public class ApplicationDetail
    {
        public int ApplicationId { get; set; }
        public string TraineeName { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public string ApplicationURL { get; set; }
        public string Response { get; set; }
        public bool? IsAccepted { get; set; }
    }
}