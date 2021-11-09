using System;

namespace kroniiapi.DTO.ReportDTO
{
    public class TraineeGeneralInfo
    {
        public int EmpId { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Facebook { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool SalaryPaid { get; set; }
        public bool OJT { get; set; }
    }
}