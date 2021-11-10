using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ReportDTO;
using Microsoft.AspNetCore.Mvc;

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
        public Dictionary<DateTime, List<TraineeAttendance>> GetAttendanceInfo(int classId, DateTime reportAt = default(DateTime))
        {
            return null;
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
        public ICollection<TraineeGPA> GetTraineeGPAs(int classId, DateTime reportAt = default(DateTime))
        {
            return null;
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
            var classModules = _dataContext.ClassModules.Where(f => f.ClassId == classId)
                                                        .Select(c => new ClassModule
                                                        {
                                                            ModuleId = c.ModuleId,
                                                            Module = c.Module,
                                                            WeightNumber = c.WeightNumber
                                                        })
                                                        .ToList();
            List<TopicInfo> topicInfo = new List<TopicInfo>();
            foreach (var item in classModules)
            {
                var itemToResponse = new TopicInfo
                {
                    TopicId = item.ModuleId,
                    Name = item.Module.ModuleName,
                    MaxScore = item.Module.MaxScore,
                    PassingScore = item.Module.PassingScore,
                    WeightNumber = item.WeightNumber
                };
                topicInfo.Add(itemToResponse);
            }

            List<AverageScoreInfo> averageScoreInfo = new List<AverageScoreInfo>();
            DateTime startDate = _dataContext.Classes.Where(c => c.ClassId == classId).Select(d => d.StartDay).FirstOrDefault();
            DateTime endDate = _dataContext.Classes.Where(c => c.ClassId == classId).Select(d => d.EndDay).FirstOrDefault();
            var duration = endDate.Month - startDate.Month + 1;
            foreach (var item in classModules)
            {
                var itemToResponse = new AverageScoreInfo
                {
                    TopicId = item.ModuleId,
                    Month = startDate,
                    MaxScore = item.Module.MaxScore * item.WeightNumber,
                    PassingScore = item.Module.PassingScore * item.WeightNumber,
                    WeightNumber = item.WeightNumber
                };
                startDate = startDate.AddMonths(1);
                averageScoreInfo.Add(itemToResponse);
            }
            var key = averageScoreInfo.Where(c => classModules.Select(m => m.ModuleId).Contains(c.TopicId)).Select(m => m.Month.Month).FirstOrDefault();

            var topicIds = topicInfo.Select(i => i.TopicId).ToList();
            var trainees = _dataContext.Trainees.Where(f => f.ClassId == classId).Select(t => t.TraineeId).ToList();
            var marks = _dataContext.Marks.Where(f => topicIds.Contains(f.ModuleId) && trainees.Contains(f.TraineeId)).OrderBy(c => c.TraineeId).ToList();
            Dictionary<int, List<TraineeGrades>> traineeGrades = new Dictionary<int, List<TraineeGrades>>();
            List<TraineeGrades> traineeGrade = new List<TraineeGrades>();
            foreach (var item in marks)
            {
                var itemToResponse = new TraineeGrades
                {
                    TopicId = item.ModuleId,
                    TraineeId = item.TraineeId,
                    Score = item.Score
                };
                if (itemToResponse == null)
                {
                    var newMark = new Mark
                    {
                        ModuleId = itemToResponse.TopicId,
                        TraineeId = itemToResponse.TraineeId,
                        Score = 0,
                    };
                    _dataContext.Marks.Add(newMark);
                }
                traineeGrade.Add(itemToResponse);
                traineeGrades.Add(key, traineeGrade);
                if (key < 12)
                {
                    key++;
                }
                if (key == 12)
                {
                    key = key - duration;
                }

            }

            Dictionary<int, List<TraineeGrades>> traineeAvarageGrades = new Dictionary<int, List<TraineeGrades>>();
            List<TraineeGrades> traineeAvarageGrade = new List<TraineeGrades>();
            foreach (var item in marks)
            {
                var itemToResponse = new TraineeGrades
                {
                    TopicId = item.ModuleId,
                    TraineeId = item.TraineeId,
                    Score = (item.Score * averageScoreInfo.Where(m => m.TopicId == item.ModuleId)
                                                            .Select(w => w.WeightNumber)
                                                            .FirstOrDefault()) / (averageScoreInfo.Where(m => m.TopicId == item.ModuleId)
                                                                        .Select(m => m.MaxScore)
                                                                        .FirstOrDefault())

                };
                traineeAvarageGrade.Add(itemToResponse);
                traineeAvarageGrades.Add(key, traineeAvarageGrade);
            }


            List<TraineeGrades> finalMarks = new List<TraineeGrades>();
            foreach (var item in averageScoreInfo)
            {
                var itemToResponse = new TraineeGrades
                {
                    TopicId = item.TopicId,
                    TraineeId = marks.Where(m => m.ModuleId == item.TopicId).Select(t => t.TraineeId).FirstOrDefault(),
                    Score = 7
                };
                finalMarks.Add(itemToResponse);
            }

            TopicGrades topicGrades = new TopicGrades()
            {
                TopicInfos = topicInfo,
                TraineeTopicGrades = traineeGrades,
                AverageScoreInfos = averageScoreInfo,
                TraineeAverageGrades = traineeAvarageGrades,
                FinalMarks = finalMarks
            };
            return topicGrades;
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