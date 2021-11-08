using System.Collections.Generic;

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