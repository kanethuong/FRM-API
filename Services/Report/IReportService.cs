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
        Task<Dictionary<DateTime, List<TraineeAttendance>>> GetAttendanceInfo(int classId, DateTime reportAt = default(DateTime));
        ICollection<RewardAndPenalty> GetRewardAndPenaltyScore(int classId, DateTime reportAt = default(DateTime));
        ICollection<TraineeFeedback> GetTraineeFeedbacks(int classId, DateTime reportAt = default(DateTime));
        List<FeedbackReport> GetFeedbackReport(int classId, DateTime reportAt = default(DateTime));
        Dictionary<int, List<AttendanceReport>> GetAttendanceReportEachMonth(int classId, int monthReport);
        List<AttendanceReport> GetTotalAttendanceReports(int classId);
        Task<CheckpointReport> GetCheckpointReport(int classId);
    }
}