using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.DTO.ClassDTO
{
    public class DeleteClassResponse
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public DateTime DeactivatedAt { get; set; }
        public DeleteBy DeleteBy { get; set; }
    }
}