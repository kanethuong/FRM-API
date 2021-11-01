using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace kroniiapi.DTO.ModuleDTO
{
    public class ModuleInput
    {
        [Required]
        public string ModuleName { get; set; }
        [Required]
        public int NoOfSlot { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public TimeSpan SlotDuration { get; set; }
        [Required]
        public IFormFile Syllabus { get; set; }
        [Required]
        public IFormFile Icon { get; set; }
    }

    public class ModuleUpdateInput
    {
        public string ModuleName { get; set; }
        public int NoOfSlot { get; set; }
        public string Description { get; set; }
        public TimeSpan SlotDuration { get; set; }
        public IFormFile Syllabus { get; set; }
        public IFormFile Icon { get; set; }
    }
}