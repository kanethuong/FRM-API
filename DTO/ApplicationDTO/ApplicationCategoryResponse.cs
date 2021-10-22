using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ApplicationDTO
{
    public class ApplicationCategoryResponse
    {
        public int ApplicationCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string SampleFileURL { get; set; }
    }
}