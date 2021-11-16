using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace kroniiapi.DTO.ModuleDTO
{
    public class ModuleInput
    {
        [Required]
        [StringLength(100, ErrorMessage = "Module name must be less than 100 characters")]
        public string ModuleName { get; set; }
        [Required]
        [StringLength(300, ErrorMessage = "Description must be less than 300 characters")]
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
        [Required]
        [StringLength(100, ErrorMessage = "Module name must be less than 100 characters")]
        public string ModuleName { get; set; }
        [Required]
        [StringLength(300, ErrorMessage = "Description must be less than 300 characters")]
        public string Description { get; set; }
        [Required]
        public TimeSpan SlotDuration { get; set; }
        public IFormFile Syllabus { get; set; }
        public IFormFile Icon { get; set; }
    }
}