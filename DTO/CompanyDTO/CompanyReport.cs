using System;

namespace kroniiapi.DTO.CompanyDTO
{
    public class CompanyReport
    {
        public int CompanyRequestId { get; set; }
        public string CompanyName { get; set; }
        public int NumberOfTrainee { get; set; }
        public DateTime AcceptedAt { get; set; }
        public string ReportURL { get; set; }
    }
}