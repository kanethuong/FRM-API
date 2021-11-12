using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ReportDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services.Report
{
    public class ReportService : IReportService
    {
        private DataContext _dataContext;
        private readonly IMapper _mapper;

        public ReportService(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Return a list of trainee using classId
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <returns>A list of trainee in a class</returns>
        public ICollection<TraineeGeneralInfo> GetTraineesInfo(int classId)
        {
            // Finish GetClassStatusReport as a part of your task
            // Remember to get Class of trainee when get trainee data from DB
            List<Trainee> traineeList = _dataContext.Trainees.Where(t => t.ClassId == classId)
                                .Select(a => new Trainee
                                {
                                    TraineeId = a.TraineeId,
                                    Fullname = a.Fullname,
                                    Username = a.Username,
                                    DOB = a.DOB,
                                    Gender = a.Gender,
                                    Email = a.Email,
                                    Phone = a.Phone,
                                    Facebook = a.Facebook,
                                    Status = a.Status,
                                    Class = new Class
                                    {
                                        StartDay = a.Class.StartDay,
                                        EndDay = a.Class.EndDay,
                                        ClassId = (int)a.ClassId,
                                        ClassName = a.Class.ClassName
                                    },
                                    OnBoard = a.OnBoard
                                })
                                .ToList();
            List<TraineeGeneralInfo> traineeGeneralInfos = _mapper.Map<List<TraineeGeneralInfo>>(traineeList);
            return traineeGeneralInfos;
        }

        /// <summary>
        /// Call GetTraineesInfo then summary trainee status in a class
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <returns>Return number of trainees per status</returns>
        public ClassStatusReport GetClassStatusReport(int classId)
        {
            var listTraineeStatus = this.GetTraineesInfo(classId).Select(s => s.Status).ToList();
            ClassStatusReport statusReport = new ClassStatusReport();
            int passed = 0, failed = 0, deferred = 0, dropout = 0, cancel = 0;
            foreach (var item in listTraineeStatus)
            {
                if (item.ToLower().Contains("passed"))
                {
                    passed++;
                }
                if (item.ToLower().Contains("failed"))
                {
                    failed++;
                }
                if (item.ToLower().Contains("deferred"))
                {
                    deferred++;
                }
                if (item.ToLower().Contains("dropOut"))
                {
                    dropout++;
                }
                if (item.ToLower().Contains("cancel"))
                {
                    cancel++;
                }
                var itemToResponse = new ClassStatusReport
                {

                    Passed = passed,
                    Failed = failed,
                    Deferred = deferred,
                    DropOut = dropout,
                    Cancel = cancel,
                };
                statusReport = itemToResponse;
            }
            return statusReport;
        }

        /// <summary>
        /// Return a dictionary of attendance with Key is Attendance day and Value of Key is
        /// a list store all of trainee's attendance in that day
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <param name="reportAt">Choose the time to report</param>
        /// <returns>A dictionary store attendance date and list of trainee status in that day</returns>
        public async Task<Dictionary<DateTime, List<TraineeAttendance>>> GetAttendanceInfo(int classId, DateTime reportAt = default(DateTime))
        {
            IEnumerable<int> traineeIdList = await _dataContext.Trainees.Where(t => t.ClassId == classId && t.IsDeactivated == false).Select(t => t.TraineeId).ToListAsync();
            if (traineeIdList.Count() == 0)
            {
                return new Dictionary<DateTime, List<TraineeAttendance>>();
            }
            DateTime startDate = new DateTime(reportAt.Year, reportAt.Month, 1);
            DateTime endDate = new DateTime(reportAt.Year, reportAt.Month, DateTime.DaysInMonth(reportAt.Year, reportAt.Month));
            Dictionary<DateTime, List<TraineeAttendance>> attendanceInfo = new Dictionary<DateTime, List<TraineeAttendance>>();
            if (reportAt == default(DateTime))
            {
                startDate = DateTime.MinValue;
                endDate = DateTime.MaxValue;
            }

            List<TraineeAttendance> traineeAttendances;
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                traineeAttendances = new List<TraineeAttendance>();
                foreach (int traineeId in traineeIdList)
                {
                traineeAttendances.Add(await _dataContext.Attendances.Where(a => a.Date.Date.CompareTo(date.Date) == 0 && a.TraineeId == traineeId)
                                                       .Select(a => new TraineeAttendance
                                                       {
                                                           TraineeId = a.TraineeId,
                                                           Status = a.Status
                                                       })
                                                       .FirstOrDefaultAsync());
                }
                attendanceInfo.Add(date, traineeAttendances);
            }
            return attendanceInfo;
        }

        /// <summary>
        /// Calculate and return attendance report of a class in a selected time
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <param name="reportAt">Choose the time to report</param>
        /// <returns>A dictionary of store report month and list of tranee report</returns>
        public Dictionary<DateTime, List<AttendanceReport>> GetAttendanceReport(int classId, DateTime reportAt = default(DateTime))
        {
            return null;
        }

        /// <summary>
        /// Get all reward and penalty of class then return it as a collection
        /// </summary>
        /// <param name="classId">If of class</param>
        /// <param name="reportAt">Choose the time to report</param>
        /// <returns>List of reward and penalty of a class</returns>
        public ICollection<RewardAndPenalty> GetRewardAndPenaltyCore(int classId, DateTime reportAt = default(DateTime))
        {
            return null;
        }

        /// <summary>
        /// Calculate GPA of every trainee then return a list of GPA per trainee
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <param name="reportAt">Choose the time to report</param>
        /// <returns>A list of trainee GPA</returns>
        public async Task<ICollection<TraineeGPA>> GetTraineeGPAs(int classId, DateTime reportAt = default(DateTime))
        {
            Dictionary<int, TraineeGPA> traineeGPAById = new Dictionary<int, TraineeGPA>();

            IEnumerable<int> listTraineeIdInClass = await _dataContext.Trainees.Where(
                t => t.ClassId == classId && t.IsDeactivated == false).Select(t => t.TraineeId).ToListAsync();

            if (listTraineeIdInClass.Count() == 0)
                return new List<TraineeGPA>();

            foreach (var traineeId in listTraineeIdInClass)
            {
                traineeGPAById.Add(traineeId, new TraineeGPA { TraineeId = traineeId });
            }

            TopicGrades topicGrades = GetTopicGrades(classId);

            if (topicGrades == null)
                return null;

            foreach (var traineeMarkInfor in topicGrades.FinalMarks) //add academic mark 
            {
                traineeGPAById[traineeMarkInfor.TraineeId].AcademicMark = traineeMarkInfor.Score;
            }

            foreach (var row in GetRewardAndPenaltyCore(classId, reportAt)) // add bonus and penalty mark
            {
                if (row.BonusAndPenaltyPoint > 0)
                {
                    traineeGPAById[row.TraineeId].Bonus += row.BonusAndPenaltyPoint;
                }
                else
                {
                    traineeGPAById[row.TraineeId].Penalty += row.BonusAndPenaltyPoint;
                }
            }

            foreach (var traineeId in listTraineeIdInClass)
            {
                traineeGPAById[traineeId].GPA =
                    traineeGPAById[traineeId].AcademicMark * (float)0.7 +
                    traineeGPAById[traineeId].DisciplinaryPoint * (float)0.3 +
                    traineeGPAById[traineeId].Bonus * (float)0.1 +
                    traineeGPAById[traineeId].Penalty * (float)0.2;
                switch (traineeGPAById[traineeId].GPA)
                {
                    case >= (float)9.3:
                        traineeGPAById[traineeId].Level = "A+";
                        break;
                    case >= (float)8.6:
                        traineeGPAById[traineeId].Level = "A";
                        break;
                    case >= (float)7.2:
                        traineeGPAById[traineeId].Level = "B";
                        break;
                    case >= (float)6.0:
                        traineeGPAById[traineeId].Level = "C";
                        break;
                    default:
                        traineeGPAById[traineeId].Level = "D";
                        break;
                }

            }

            return traineeGPAById.Values;
        }

        /// <summary>
        /// Get all feedback of trainee in a class
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <param name="reportAt">Choose the time to report</param>
        /// <returns>List of feedbacks</returns>
        public ICollection<TraineeFeedback> GetTraineeFeedbacks(int classId, DateTime reportAt = default(DateTime))
        {
            // Finish GetFeedbackReport as a part of your task
            // Remeber to get traineeId when getting feedback from DB
            return null;
        }

        /// <summary>
        /// Calculate and return the feedback report
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <param name="reportAt">Choose the time to report</param>
        /// <returns>Feedback report data</returns>
        public FeedbackReport GetFeedbackReport(int classId, DateTime reportAt = default(DateTime))
        {
            return null;
        }

        /// <summary>
        /// Calculate and return trainee marks and relates
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <returns>Object with all fields in topic grades</returns>
        public TopicGrades GetTopicGrades(int classId)
        {
            return null;
        }

        /// <summary>
        /// Call GetTopicGrades then summary number of trainee per classifications
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <returns>Report object with number of trainees per classifications</returns>
        public CheckpointReport GetCheckpointReport(int classId)
        {
            return null;
        }

        /// <summary>
        /// Collect all pieces of data in every sheet of report then send it to export excel helper
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <returns>An excel report file</returns>
        public FileContentResult GenerateClassReport(int classId)
        {
            return null;
        }
    }
}