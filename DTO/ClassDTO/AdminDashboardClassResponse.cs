using System;

namespace kroniiapi.DTO.ClassDTO
{
    public class AdminDashboardClassResponse
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
    }
}