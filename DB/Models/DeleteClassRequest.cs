using System;

namespace kroniiapi.DB.Models
{
    public class DeleteClassRequest
    {
        public int DeleteClassRequestId { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? IsAccepted { get; set; }
        public DateTime? AcceptedAt { get; set; }

        // One-Many class
        public int ClassId { get; set; }
        public Class Class { get; set; }

        // One-Many admin
        public int AdminId { get; set; }
        public Admin Admin { get; set; }
    }
}