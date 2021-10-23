using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TraineeDTO
{
    public class TraineeProfileDetail
    {
        [Required]
        public string Fullname { get; set; }
        [Required]
        public string AvatarURL { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public DateTime DOB { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Gender { get; set; }
    }
}