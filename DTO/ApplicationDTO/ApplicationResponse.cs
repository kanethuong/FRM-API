using System;

namespace kroniiapi.DTO.ApplicationDTO
{
    public class ApplicationResponse
    {
        public int ApplicationId { get; set; }
        public string TraineeName { get; set; }
        public string Category { get; set; }
        public bool? IsAccepted { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}