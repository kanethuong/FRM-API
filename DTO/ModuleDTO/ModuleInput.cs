using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.ModuleDTO
{
    public class ModuleInput
    {
        [Required]
        public string ModuleName { get; set; }
        [Required]
        public string IconURL { get; set; }
        [Required]
        public string SyllabusURL { get; set; }
        [Required]
        public int NoOfSlot { get; set; }
        [Required]
        public string Description { get; set; }
    }
}