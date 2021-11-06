using System.ComponentModel.DataAnnotations.Schema;

namespace kroniiapi.DB.Models
{
    public class CompanyRequestDetail
    {
        public int CompanyRequestId { get; set; }
        public CompanyRequest CompanyRequest { get; set; }
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; }
        [Column(TypeName = "money")]
        public decimal Wage { get; set; }
    }
}