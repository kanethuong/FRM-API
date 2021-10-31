using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.CompanyRequestDTO
{
    public class RequestDetail
    {
        public int CompanyRequestId { get; set; }
        public string CompanyName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<TraineeInRequest> TraineeList { get; set; }
    }
}