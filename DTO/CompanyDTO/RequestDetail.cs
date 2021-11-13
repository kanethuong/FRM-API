using System;

namespace kroniiapi.DTO.CompanyDTO
{
    public class RequestDetail
    {
        public int CompanyRequestId { get; set; }
        public string CompanyName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}