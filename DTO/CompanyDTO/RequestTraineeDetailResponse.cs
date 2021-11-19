using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.CompanyDTO
{
    public class RequestTraineeDetailResponse
    {
        public ICollection<TraineeInResponse> Trainees { get; set; }
        public string Content { get; set; }
    }
}