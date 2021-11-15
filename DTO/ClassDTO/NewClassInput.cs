using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace kroniiapi.DTO.ClassDTO
{
    public class NewClassInput
    {
        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Class name must be from 6 to 100 characters")]
        public string ClassName { get; set; }
        [Required]
        [StringLength(300, ErrorMessage = "Description must be less than 300 characters")]
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