using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ClassDTO
{
    public class RequestDeleteClassInput
    {
        public string Reason { get; set; }
        public int ClassId { get; set; }
        public int AdminId { get; set; }
    }
}