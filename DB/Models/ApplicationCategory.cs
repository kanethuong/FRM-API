using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DB.Models
{
    public class ApplicationCategory
    {
        public int ApplicationCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string SampleFileURL { get; set; }
        public ICollection<Application> Applications { get; set; }
    }
}