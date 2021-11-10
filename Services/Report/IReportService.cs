using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DTO.ReportDTO;

namespace kroniiapi.Services.Report
{
    public interface IReportService
    {
        ICollection<TraineeGeneralInfo> GetTraineesInfo(int classId);
        ClassStatusReport GetClassStatusReport(int classId);
    }
}