using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace kroniiapi.DTO.ApplicationDTO
{
    public class ApplicationInput
    {
        [StringLength(300, ErrorMessage = "Description must be less than 300 characters")]
        public string Description { get; set; }
        public int TraineeId { get; set; }
        public int ApplicationCategoryId { get; set; }
    }
}