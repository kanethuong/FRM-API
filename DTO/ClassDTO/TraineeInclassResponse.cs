using System;

namespace kroniiapi.DTO.ClassDTO
{
    public class TraineeInclassResponse
    {
        public int TraineeId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DOB { get; set; }
    }
}