using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DTO.ReportDTO;

namespace kroniiapi.DTO.AdminDTO
{
    public class AdminDashboard
    {
        public ClassStatusReport ClassStatus { get; set; }
        public CheckpointReport Checkpoint { get; set; }
        public FeedbackReport Feedback { get; set; }
    }
}