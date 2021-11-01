using System;

namespace kroniiapi.DTO.ExamDTO
{
    public class ExamResponse
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public DateTime ExamDay { get; set; }
        public int DurationInMinute { get; set; }
        public string AdminName { get; set; }
        public bool IsCancelled { get; set; }
    }
}