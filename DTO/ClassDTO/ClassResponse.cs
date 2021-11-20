using System;
using System.Collections.Generic;

namespace kroniiapi.DTO.ClassDTO
{
    public class ClassResponse
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public IEnumerable<TrainerInClassResponse> Trainer { get; set; }
        public string Creator { get; set; }//admin fullname
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public int NoOfModule { get; set; }
    }
}