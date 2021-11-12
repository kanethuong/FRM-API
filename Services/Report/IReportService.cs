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
        TopicGrades GetTopicGrades(int classId);
        Task<ICollection<TraineeGPA>> GetTraineeGPAs(int classId, DateTime reportAt = default(DateTime));

        ICollection<RewardAndPenalty> GetRewardAndPenaltyCore(int classId, DateTime reportAt = default(DateTime));
        ICollection<TraineeFeedback> GetTraineeFeedbacks(int classId, DateTime reportAt = default(DateTime));
        Task<List<AttendanceReport>> GetFeedbackReport(int classId, DateTime reportAt = default(DateTime));
    }
}