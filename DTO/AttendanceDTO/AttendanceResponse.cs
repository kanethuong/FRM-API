using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.AttendanceDTO
{
    public class AttendanceResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
    }
}