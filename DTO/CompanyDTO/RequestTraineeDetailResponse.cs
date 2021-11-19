using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.CompanyDTO
{
    public class RequestTraineeDetailResponse
    {
        public IEnumerable<TraineeInResponse> Trainees { get; set; }
        public string Content { get; set; }
    }
}