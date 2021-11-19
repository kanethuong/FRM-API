using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.CompanyDTO
{
    public class RequestTraineeResponse
    {
        public int Id { get; set; } //Already mapped from CompanyRequestId
        public int NumberOfTrainee { get; set; } //Already mapped from count of CompanyRequestDetail,no need to count again
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool? IsAccepted { get; set; }
    }
}