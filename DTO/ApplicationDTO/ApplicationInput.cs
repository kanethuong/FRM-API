using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace kroniiapi.DTO.ApplicationDTO
{
    public class ApplicationInput
    {
        public string Description { get; set; }
        public int TraineeId { get; set; }
        public int ApplicationCategoryId { get; set; }
    }
}