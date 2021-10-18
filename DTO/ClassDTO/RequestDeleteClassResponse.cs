using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.DTO.ClassDTO
{
    public class RequestDeleteClassResponse
    {
        public int DeleteClassRequestId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public CreatorDTO Admin { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Reason { get; set; }
    }
}