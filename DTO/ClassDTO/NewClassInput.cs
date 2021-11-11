using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.DTO.ClassDTO
{
    public class NewClassInput
    {
        [Required]
        public string ClassName { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int AdminId { get; set; }
        [Required]
        public DateTime StartDay { get; set; } 
        [Required]
        public DateTime EndDay { get; set; }
        [Required]
        public ICollection<int> TraineeIdList { get; set; }
        public ICollection<TrainerModule> TrainerModuleList { get; set; }
    }
}