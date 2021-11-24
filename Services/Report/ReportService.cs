using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.ReportDTO;
using kroniiapi.Helper;
using kroniiapi.Helper.Upload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using static kroniiapi.Services.Attendance.AttendanceStatus;

namespace kroniiapi.Services.Report
{
    public class ReportService : IReportService
    {
        private DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly IMegaHelper _megaHelper;

        public ReportService(DataContext dataContext, IMapper mapper, IMegaHelper megaHelper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _megaHelper = megaHelper;
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
            int passed = 0, failed = 0, deferred = 0, dropout = 0, cancel = 0, learning = 0;
            foreach (var item in listTraineeStatus)
            {
                if (item == null || item.ToLower().Contains("learning"))
                {
                    learning++;
                }
                else if (item.ToLower().Contains("passed"))
                {
                    passed++;
                }
                else if (item.ToLower().Contains("failed"))
                {
                    failed++;
                }
                else if (item.ToLower().Contains("deferred"))
                {
                    deferred++;
                }
                else if (item.ToLower().Contains("dropout"))
                {
                    dropout++;
                }
                else if (item.ToLower().Contains("cancel"))
                {
                    cancel++;
                }
            }
            ClassStatusReport statusReport = new ClassStatusReport()
            {
                Learning = learning,
                Passed = passed,
                Failed = failed,
                Deferred = deferred,
                DropOut = dropout,
                Cancel = cancel,
            };

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
                startDate = _dataContext.Classes.Where(c => c.ClassId == classId).Select(c => c.StartDay).FirstOrDefault();
                endDate = _dataContext.Classes.Where(c => c.ClassId == classId).Select(c => c.EndDay).FirstOrDefault();
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
                if (traineeAttendances == null || traineeAttendances.Any(t => t == null))
                {
                    continue;
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
        public Dictionary<DateTime, List<AttendanceReport>> GetAttendanceReportEachMonth(int classId, DateTime monthReport)
        {
            List<AttendanceReport> attendanceReports = new();
            var traineeList = _dataContext.Trainees.Where(t => t.ClassId == classId && t.IsDeactivated == false).ToList();
            foreach (var trainee in traineeList)
            {
                AttendanceReport ap = this.GetAttendanceReportByTraineeAndMonth(trainee, monthReport);
                attendanceReports.Add(ap);
            }
            return new Dictionary<DateTime, List<AttendanceReport>> {
                    { monthReport, attendanceReports }
                };
        }
        private AttendanceReport GetAttendanceReportByTraineeAndMonth(Trainee trainee, DateTime monthReport)
        {
            AttendanceReport ap = new AttendanceReport()
            {
                TraineeId = trainee.TraineeId,
                NumberOfAbsent = 0,
                NumberOfLateInAndEarlyOut = 0,
                NoPermissionRate = 0,
                DisciplinaryPoint = 1
            };
            //get Number of trainee absent with A or An Status with month in report
            ap.NumberOfAbsent = _dataContext.Attendances.Where(t => t.TraineeId == trainee.TraineeId
                                                                      && t.Date.Month == monthReport.Month
                                                                      && t.Date.Year == monthReport.Year
                                                                      && (t.Status == nameof(_attendanceStatus.An)
                                                                         || t.Status == nameof(_attendanceStatus.A))).Count();
            //get Number of trainee Late in and early out with Ln/L/En/E Status with month in report
            ap.NumberOfLateInAndEarlyOut = _dataContext.Attendances.Where(t => t.TraineeId == trainee.TraineeId
                                                                      && t.Date.Month == monthReport.Month
                                                                      && t.Date.Year == monthReport.Year
                                                                      && (t.Status == nameof(_attendanceStatus.Ln)
                                                                         || t.Status == nameof(_attendanceStatus.L)
                                                                         || t.Status == nameof(_attendanceStatus.En)
                                                                         || t.Status == nameof(_attendanceStatus.E))).Count();
            //Calculate No permission rate = Total of (Ln,An,En) / Total of (L,Ln,A,An,E,En)
            if (ap.NumberOfAbsent + ap.NumberOfLateInAndEarlyOut != 0)  //Total of (L,Ln,A,An,E,En) != 0
            {
                var numberOfNoPermission = _dataContext.Attendances.Where(t => t.TraineeId == trainee.TraineeId
                                                                     && t.Date.Month == monthReport.Month
                                                                      && t.Date.Year == monthReport.Year
                                                                     && (t.Status == nameof(_attendanceStatus.Ln)
                                                                        || t.Status == nameof(_attendanceStatus.An)
                                                                        || t.Status == nameof(_attendanceStatus.En))).Count();
                ap.NoPermissionRate = (float)numberOfNoPermission / (ap.NumberOfAbsent + ap.NumberOfLateInAndEarlyOut);
            }
            var attCount = _dataContext.Attendances.Where(t => t.TraineeId == trainee.TraineeId
                                                                       && t.Date.Month == monthReport.Month
                                                                      && t.Date.Year == monthReport.Year).Count();
            float violationRate = 0;
            if (attCount != 0)
            {
                violationRate = (float)(ap.NumberOfLateInAndEarlyOut / 2 + ap.NumberOfAbsent) / attCount;
            }
            //Calculate disciplinary point by using formula from excel
            ap.DisciplinaryPoint = CalculateDisciplinaryPoint(violationRate, ap.NoPermissionRate);
            return ap;
        }

        /// <summary>
        /// Apply formula from excel to calculate disciplinary point
        /// </summary>
        /// <param name="violationRate"></param>
        /// <param name="NoPermissionRate"></param>
        /// <returns>return disciplinary point</returns>
        private float CalculateDisciplinaryPoint(float violationRate, float NoPermissionRate)
        {
            float DisciplinaryPoint = 0;
            if (violationRate <= 0.05f)
            {
                DisciplinaryPoint = 10;
            }
            else if (violationRate <= 0.2f)
            {
                DisciplinaryPoint = 8;
            }
            else if (violationRate <= 0.3f)
            {
                DisciplinaryPoint = 6;
            }
            else if (violationRate < 0.5f)
            {
                DisciplinaryPoint = 5;
            }
            else if (violationRate >= 0.5f && NoPermissionRate >= 0.2f)
            {
                DisciplinaryPoint = 0;
            }
            else
            {
                DisciplinaryPoint = 0.2f;
            }
            return DisciplinaryPoint;
        }
        /// <summary>
        /// Get the months of class between start day and end day
        /// </summary>
        /// <param name="classId"></param>
        /// <returns>list of month</returns>
        public IEnumerable<DateTime> GetMonthListOfClass(int classId)
        {
            var classGet = _dataContext.Classes.FirstOrDefault(c => c.ClassId == classId);
            var start = classGet.StartDay;
            var end = classGet.EndDay;
            // set end-date to end of month
            end = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month));
            // get the list of month of class duration
            var listMonth = Enumerable.Range(0, Int32.MaxValue)
                                .Select(e => start.AddMonths(e))
                                .TakeWhile(e => e <= end)
                                .Select(e => e);
            return listMonth;
        }
        /// <summary>
        /// Get total attendance report in all month
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public List<AttendanceReport> GetTotalAttendanceReports(int classId)
        {
            var attendanceReports = new List<AttendanceReport>();
            var listMonth = this.GetMonthListOfClass(classId);

            var traineeList = _dataContext.Trainees.Where(t => t.ClassId == classId && t.IsDeactivated == false).ToList();
            foreach (var trainee in traineeList)
            {
                var traineeAttRpAll = new AttendanceReport()
                {
                    TraineeId = trainee.TraineeId,
                    NumberOfAbsent = 0,
                    NumberOfLateInAndEarlyOut = 0,
                    NoPermissionRate = 0,
                    DisciplinaryPoint = 0
                };
                // calculate the sum of absent, late in and early out 
                // sum of no permission rate and disciplinary point to calculate average later
                foreach (var month in listMonth)
                {
                    var att = this.GetAttendanceReportByTraineeAndMonth(trainee, month);
                    traineeAttRpAll.NumberOfAbsent += att.NumberOfAbsent;
                    traineeAttRpAll.NumberOfLateInAndEarlyOut += att.NumberOfLateInAndEarlyOut;
                    traineeAttRpAll.NoPermissionRate += att.NoPermissionRate;
                    traineeAttRpAll.DisciplinaryPoint += att.DisciplinaryPoint;
                }
                // divide no permission rate and disciplinary point by number of months to calculate average 
                traineeAttRpAll.NoPermissionRate = traineeAttRpAll.NoPermissionRate / listMonth.Count();
                traineeAttRpAll.DisciplinaryPoint = traineeAttRpAll.DisciplinaryPoint / listMonth.Count();
                attendanceReports.Add(traineeAttRpAll);
            }
            return attendanceReports;
        }

        /// <summary>
        /// Get all reward and penalty of class then return it as a collection
        /// </summary>
        /// <param name="classId">If of class</param>
        /// /// <param name="reportAt">Choose the time to report</param>
        /// <returns>List of reward and penalty of a class</returns>
        public ICollection<RewardAndPenalty> GetRewardAndPenaltyScore(int classId, DateTime reportAt = default(DateTime))
        {
            var startDate = new DateTime();
            var endDate = new DateTime();
            if (reportAt != default(DateTime))
            {
                TimeSpan oneday = new TimeSpan(23, 59, 59);
                startDate = new DateTime(reportAt.Year, reportAt.Month, 1);
                endDate = new DateTime(reportAt.Year, reportAt.Month, DateTime.DaysInMonth(reportAt.Year, reportAt.Month));
                endDate = endDate.Add(oneday);

            }
            else
            {
                startDate = DateTime.MinValue;
                endDate = DateTime.MaxValue;
            }
            var trainees = _dataContext.Trainees.Where(t => t.ClassId == classId && t.IsDeactivated == false).ToList();
            List<BonusAndPunish> rp = new List<BonusAndPunish>();
            foreach (var item in trainees)
            {
                rp.AddRange(_dataContext.BonusAndPunishes.Where(b => b.TraineeId == item.TraineeId && b.CreatedAt >= startDate && b.CreatedAt <= endDate).ToList());
            }
            List<RewardAndPenalty> rpDto = _mapper.Map<List<RewardAndPenalty>>(rp);
            return rpDto;
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

            if (topicGrades != null)
            {
                foreach (var traineeMarkInfor in topicGrades.FinalMarks) //add academic mark 
                {
                    traineeGPAById[traineeMarkInfor.TraineeId].AcademicMark = traineeMarkInfor.Score;
                }
            }

            var totalAttendanceReports = GetTotalAttendanceReports(classId);
            if (totalAttendanceReports != null)
            {
                foreach (var attendanceInfor in totalAttendanceReports)
                {
                    traineeGPAById[attendanceInfor.TraineeId].DisciplinaryPoint = attendanceInfor.DisciplinaryPoint;
                }
            }

            var RewardAndPenalty = GetRewardAndPenaltyScore(classId, reportAt);
            if (RewardAndPenalty != null)
            {
                foreach (var row in RewardAndPenalty) // add bonus and penalty mark
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
                    case >= (float)6:
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
            var trainees = _dataContext.Trainees.Where(t => t.ClassId == classId && t.IsDeactivated == false).ToList();
            List<Feedback> traineeFeedbacks = new();
            if (reportAt == new DateTime(1, 1, 1))
            {
                foreach (var item in trainees)
                {
                    traineeFeedbacks.AddRange(_dataContext.Feedbacks.Where(t => t.TraineeId == item.TraineeId).ToList());
                }
            }
            else
            {
                foreach (var item in trainees)
                {
                    traineeFeedbacks.Add(_dataContext.Feedbacks.Where(t => t.TraineeId == item.TraineeId && t.CreatedAt.Year == reportAt.Year && t.CreatedAt.Month == reportAt.Month).FirstOrDefault());
                }
            }
            ICollection<TraineeFeedback> traineeFeedbacksMapped = _mapper.Map<ICollection<TraineeFeedback>>(traineeFeedbacks);
            return traineeFeedbacksMapped;
        }
        public Dictionary<DateTime, ICollection<TraineeFeedback>> GetAllTraineeFeedbacks(int classId)
        {
            var class1 = _dataContext.Classes.Where(c => c.ClassId == classId && c.IsDeactivated == false).FirstOrDefault();
            var trainees = _dataContext.Trainees.Where(t => t.ClassId == classId && t.IsDeactivated == false).ToList();
            List<DateTime> datelist = new();
            Dictionary<DateTime, ICollection<TraineeFeedback>> dic = new();
            var tempDate = class1.StartDay;
            while (true)
            {
                datelist.Add(tempDate);
                if ((tempDate.Month == class1.EndDay.Month) && (tempDate.Year == class1.EndDay.Year))
                {
                    break;
                }
                tempDate = tempDate.AddMonths(1);
            }
            foreach (var item in datelist)
            {
                List<Feedback> traineeFeedbacks = new();
                if (item == datelist[datelist.Count() - 1])
                {
                    foreach (var id in trainees)
                    {
                        traineeFeedbacks.AddRange(_dataContext.Feedbacks.Where(t => t.TraineeId == id.TraineeId && t.CreatedAt >= item && t.CreatedAt <= class1.EndDay).ToList());
                    }
                }
                else
                {
                    foreach (var id in trainees)
                    {
                        traineeFeedbacks.AddRange(_dataContext.Feedbacks.Where(t => t.TraineeId == id.TraineeId && t.CreatedAt >= item && t.CreatedAt <= item.AddMonths(1)).ToList());
                    }
                }
                ICollection<TraineeFeedback> traineeFeedbacksMapped = _mapper.Map<ICollection<TraineeFeedback>>(traineeFeedbacks);
                dic.Add(item, traineeFeedbacksMapped);

            }
            return dic;
        }

        /// <summary>
        /// Calculate and return the feedback report
        /// Rate is int not %
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <param name="reportAt">Choose the time to report</param>
        /// <returns>Feedback report data</returns>
        public List<FeedbackReport> GetFeedbackReport(int classId, DateTime reportAt = default(DateTime))
        {
            List<FeedbackReport> feedbackReports = new List<FeedbackReport>();
            if (reportAt == new DateTime(1, 1, 1))
            {
                int startMonth = _dataContext.Classes.Where(cl => cl.ClassId == classId).Select(cl => cl.StartDay.Month).FirstOrDefault();
                int endMonth = _dataContext.Classes.Where(cl => cl.ClassId == classId).Select(cl => cl.EndDay.Month).FirstOrDefault();
                int startYear = _dataContext.Classes.Where(cl => cl.ClassId == classId).Select(cl => cl.StartDay.Year).FirstOrDefault();
                int endYear = _dataContext.Classes.Where(cl => cl.ClassId == classId).Select(cl => cl.EndDay.Year).FirstOrDefault();
                var start = _dataContext.Classes.Where(cl => cl.ClassId == classId).Select(cl => cl.StartDay).FirstOrDefault();
                var end = _dataContext.Classes.Where(cl => cl.ClassId == classId).Select(cl => cl.EndDay).FirstOrDefault();
                var listMonth = Enumerable.Range(0, Int32.MaxValue)
                                .Select(e => start.AddMonths(e))
                                .TakeWhile(e => e <= end)
                                .Select(e => e);
                foreach (var item in listMonth)
                {
                    // Content
                    var topiccontentL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == item.Year && fb.CreatedAt.Month == item.Month).Select(fb => fb.TopicContent);
                    var TopicContentAVG = (topiccontentL.Count() != 0) ? Convert.ToSingle(topiccontentL.Average()) : 0;
                    var topicobjectL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == item.Year && fb.CreatedAt.Month == item.Month).Select(fb => fb.TopicObjective);
                    var TopicObjectiveAVG = (topiccontentL.Count() != 0) ? Convert.ToSingle(topicobjectL.Average()) : 0;
                    var approL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == item.Year && fb.CreatedAt.Month == item.Month).Select(fb => fb.ApproriateTopicLevel);
                    var ApproriateTopicLevelAVG = (approL.Count() != 0) ? Convert.ToSingle(approL.Average()) : 0;
                    var topicUseL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == item.Year && fb.CreatedAt.Month == item.Month).Select(fb => fb.TopicUsefulness);
                    var TopicUsefulnessAVG = (topicUseL.Count() != 0) ? Convert.ToSingle(topicUseL.Average()) : 0;
                    var trainingL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == item.Year && fb.CreatedAt.Month == item.Month).Select(fb => fb.TrainingMaterial);
                    var TrainingMaterialAVG = (trainingL.Count() != 0) ? Convert.ToSingle(trainingL.Average()) : 0;
                    // Trainer
                    var trainerL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == item.Year && fb.CreatedAt.Month == item.Month).Select(fb => fb.TrainerKnowledge);
                    var TrainerKnowledgeAVG = (trainerL.Count() != 0) ? Convert.ToSingle(trainerL.Average()) : 0;
                    var subjectL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == item.Year && fb.CreatedAt.Month == item.Month).Select(fb => fb.SubjectCoverage);
                    var SubjectCoverageAVG = (subjectL.Count() != 0) ? Convert.ToSingle(subjectL.Average()) : 0;
                    var instrucL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == item.Year && fb.CreatedAt.Month == item.Month).Select(fb => fb.InstructionAndCommunicate);
                    var InstructionAndCommunicateAVG = (instrucL.Count() != 0) ? Convert.ToSingle(instrucL.Average()) : 0;
                    var trainerSL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == item.Year && fb.CreatedAt.Month == item.Month).Select(fb => fb.TrainerSupport);
                    var TrainerSupportAVG = (trainerL.Count() != 0) ? Convert.ToSingle(trainerL.Average()) : 0;
                    // Organization
                    var logicL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == item.Year && fb.CreatedAt.Month == item.Month).Select(fb => fb.Logistics);
                    var LogisticsAVG = (logicL.Count() != 0) ? Convert.ToSingle(logicL.Average()) : 0;
                    var inforL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == item.Year && fb.CreatedAt.Month == item.Month).Select(fb => fb.InformationToTrainees);
                    var InformationToTraineesAVG = (inforL.Count() != 0) ? Convert.ToSingle(inforL.Average()) : 0;
                    var adminSL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == item.Year && fb.CreatedAt.Month == item.Month).Select(fb => fb.AdminSupport);
                    var AdminSupportAVG = (adminSL.Count() != 0) ? Convert.ToSingle(adminSL.Average()) : 0;
                    // OJT Eval
                    var OJTEval = (TopicContentAVG + TopicObjectiveAVG + ApproriateTopicLevelAVG + TopicUsefulnessAVG + TrainingMaterialAVG
                                    + TrainerKnowledgeAVG + SubjectCoverageAVG + InstructionAndCommunicateAVG + TrainerSupportAVG
                                    + LogisticsAVG + InformationToTraineesAVG + AdminSupportAVG) / 12;
                    // Trainning Program
                    var TrainningProgram = (TopicContentAVG + TopicObjectiveAVG + ApproriateTopicLevelAVG + TopicUsefulnessAVG + TrainingMaterialAVG) / 5;
                    var Trainer = (TrainerKnowledgeAVG + SubjectCoverageAVG + InstructionAndCommunicateAVG + TrainerSupportAVG);
                    var Organization = (LogisticsAVG + InformationToTraineesAVG + AdminSupportAVG) / 3;
                    var AverageScore = (TrainningProgram + Trainer + Organization) / 3;
                    var fbReportAdd = new FeedbackReport
                    {
                        TopicContent = TopicContentAVG,
                        TopicObjective = TopicObjectiveAVG,
                        ApproriateTopicLevel = ApproriateTopicLevelAVG,
                        TopicUsefulness = TopicContentAVG,
                        TrainingMaterial = TrainingMaterialAVG,
                        TrainerKnowledge = TrainerKnowledgeAVG,
                        SubjectCoverage = SubjectCoverageAVG,
                        InstructionAndCommunicate = InstructionAndCommunicateAVG,
                        TrainerSupport = TrainerSupportAVG,
                        Logistics = LogisticsAVG,
                        InformationToTrainees = InformationToTraineesAVG,
                        AdminSupport = AdminSupportAVG,
                        TrainingPrograms = TrainningProgram,
                        Trainer = Trainer,
                        Organization = Organization,
                        ContantEval = TrainningProgram,
                        TrainerEval = Trainer,
                        OrganizeEval = Organization,
                        OJTEval = OJTEval,
                        AverageScore = AverageScore,
                        ReportAt = new DateTime(item.Year, item.Month, 1),
                        IsSumary = false,
                    };
                    feedbackReports.Add(fbReportAdd);
                }
                if (feedbackReports.Count() == 0)
                {
                    return null;
                }
                var sumaryReport = new FeedbackReport
                {
                    TopicContent = feedbackReports.Average(fb => fb.TopicContent),
                    TopicObjective = feedbackReports.Average(fb => fb.TopicObjective),
                    ApproriateTopicLevel = feedbackReports.Average(fb => fb.ApproriateTopicLevel),
                    TopicUsefulness = feedbackReports.Average(fb => fb.TopicUsefulness),
                    TrainingMaterial = feedbackReports.Average(fb => fb.TrainingMaterial),
                    TrainerKnowledge = feedbackReports.Average(fb => fb.TrainerKnowledge),
                    SubjectCoverage = feedbackReports.Average(fb => fb.SubjectCoverage),
                    InstructionAndCommunicate = feedbackReports.Average(fb => fb.InstructionAndCommunicate),
                    TrainerSupport = feedbackReports.Average(fb => fb.TrainerSupport),
                    Logistics = feedbackReports.Average(fb => fb.Logistics),
                    InformationToTrainees = feedbackReports.Average(fb => fb.InformationToTrainees),
                    AdminSupport = feedbackReports.Average(fb => fb.AdminSupport),

                    TrainingPrograms = (feedbackReports.Average(fb => fb.TopicContent) + feedbackReports.Average(fb => fb.TopicObjective) + feedbackReports.Average(fb => fb.ApproriateTopicLevel) + feedbackReports.Average(fb => fb.TopicUsefulness) + feedbackReports.Average(fb => fb.TrainingMaterial)) / 5,
                    Trainer = (feedbackReports.Average(fb => fb.TrainerKnowledge) + feedbackReports.Average(fb => fb.SubjectCoverage) + feedbackReports.Average(fb => fb.InstructionAndCommunicate) + feedbackReports.Average(fb => fb.TrainerSupport)) / 4,
                    Organization = (feedbackReports.Average(fb => fb.Logistics) + feedbackReports.Average(fb => fb.InformationToTrainees) + feedbackReports.Average(fb => fb.AdminSupport)) / 3,

                    ContantEval = (feedbackReports.Average(fb => fb.TopicContent) + feedbackReports.Average(fb => fb.TopicObjective) + feedbackReports.Average(fb => fb.ApproriateTopicLevel) + feedbackReports.Average(fb => fb.TopicUsefulness) + feedbackReports.Average(fb => fb.TrainingMaterial)) / 5,
                    TrainerEval = (feedbackReports.Average(fb => fb.TrainerKnowledge) + feedbackReports.Average(fb => fb.SubjectCoverage) + feedbackReports.Average(fb => fb.InstructionAndCommunicate) + feedbackReports.Average(fb => fb.TrainerSupport)) / 4,
                    OrganizeEval = (feedbackReports.Average(fb => fb.Logistics) + feedbackReports.Average(fb => fb.InformationToTrainees) + feedbackReports.Average(fb => fb.AdminSupport)) / 3,
                    OJTEval = (feedbackReports.Average(fb => fb.TopicContent) + feedbackReports.Average(fb => fb.TopicObjective) + feedbackReports.Average(fb => fb.ApproriateTopicLevel) + feedbackReports.Average(fb => fb.TopicUsefulness) + feedbackReports.Average(fb => fb.TrainingMaterial)
                                + feedbackReports.Average(fb => fb.TrainerKnowledge) + feedbackReports.Average(fb => fb.SubjectCoverage) + feedbackReports.Average(fb => fb.InstructionAndCommunicate) + feedbackReports.Average(fb => fb.TrainerSupport)
                                + feedbackReports.Average(fb => fb.Logistics) + feedbackReports.Average(fb => fb.InformationToTrainees) + feedbackReports.Average(fb => fb.AdminSupport)) / 12,
                    AverageScore = (((feedbackReports.Average(fb => fb.TopicContent) + feedbackReports.Average(fb => fb.TopicObjective) + feedbackReports.Average(fb => fb.ApproriateTopicLevel) + feedbackReports.Average(fb => fb.TopicUsefulness) + feedbackReports.Average(fb => fb.TrainingMaterial)) / 5)
                                        + ((feedbackReports.Average(fb => fb.TrainerKnowledge) + feedbackReports.Average(fb => fb.SubjectCoverage) + feedbackReports.Average(fb => fb.InstructionAndCommunicate) + feedbackReports.Average(fb => fb.TrainerSupport)) / 4)
                                        + ((feedbackReports.Average(fb => fb.Logistics) + feedbackReports.Average(fb => fb.InformationToTrainees) + feedbackReports.Average(fb => fb.AdminSupport)) / 3)) / 3,
                    ReportAt = new DateTime(1, 1, 1),
                    IsSumary = true,
                };
                feedbackReports.Add(sumaryReport);
            }
            else
            {
                // Content
                var topiccontentL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == reportAt.Year && fb.CreatedAt.Month == reportAt.Month).Select(fb => fb.TopicContent);
                var TopicContentAVG = (topiccontentL.Count() != 0) ? Convert.ToSingle(topiccontentL.Average()) : 0;
                var topicobjectL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == reportAt.Year && fb.CreatedAt.Month == reportAt.Month).Select(fb => fb.TopicObjective);
                var TopicObjectiveAVG = (topiccontentL.Count() != 0) ? Convert.ToSingle(topicobjectL.Average()) : 0;
                var approL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == reportAt.Year && fb.CreatedAt.Month == reportAt.Month).Select(fb => fb.ApproriateTopicLevel);
                var ApproriateTopicLevelAVG = (approL.Count() != 0) ? Convert.ToSingle(approL.Average()) : 0;
                var topicUseL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == reportAt.Year && fb.CreatedAt.Month == reportAt.Month).Select(fb => fb.TopicUsefulness);
                var TopicUsefulnessAVG = (topicUseL.Count() != 0) ? Convert.ToSingle(topicUseL.Average()) : 0;
                var trainingL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == reportAt.Year && fb.CreatedAt.Month == reportAt.Month).Select(fb => fb.TrainingMaterial);
                var TrainingMaterialAVG = (trainingL.Count() != 0) ? Convert.ToSingle(trainingL.Average()) : 0;
                // Trainer
                var trainerL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == reportAt.Year && fb.CreatedAt.Month == reportAt.Month).Select(fb => fb.TrainerKnowledge);
                var TrainerKnowledgeAVG = (trainerL.Count() != 0) ? Convert.ToSingle(trainerL.Average()) : 0;
                var subjectL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == reportAt.Year && fb.CreatedAt.Month == reportAt.Month).Select(fb => fb.SubjectCoverage);
                var SubjectCoverageAVG = (subjectL.Count() != 0) ? Convert.ToSingle(subjectL.Average()) : 0;
                var instrucL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == reportAt.Year && fb.CreatedAt.Month == reportAt.Month).Select(fb => fb.InstructionAndCommunicate);
                var InstructionAndCommunicateAVG = (instrucL.Count() != 0) ? Convert.ToSingle(instrucL.Average()) : 0;
                var trainerSL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == reportAt.Year && fb.CreatedAt.Month == reportAt.Month).Select(fb => fb.TrainerSupport);
                var TrainerSupportAVG = (trainerL.Count() != 0) ? Convert.ToSingle(trainerL.Average()) : 0;
                // Organization
                var logicL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == reportAt.Year && fb.CreatedAt.Month == reportAt.Month).Select(fb => fb.Logistics);
                var LogisticsAVG = (logicL.Count() != 0) ? Convert.ToSingle(logicL.Average()) : 0;
                var inforL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == reportAt.Year && fb.CreatedAt.Month == reportAt.Month).Select(fb => fb.InformationToTrainees);
                var InformationToTraineesAVG = (inforL.Count() != 0) ? Convert.ToSingle(inforL.Average()) : 0;
                var adminSL = _dataContext.Feedbacks.Where(fb => fb.Trainee.ClassId == classId && fb.CreatedAt.Year == reportAt.Year && fb.CreatedAt.Month == reportAt.Month).Select(fb => fb.AdminSupport);
                var AdminSupportAVG = (adminSL.Count() != 0) ? Convert.ToSingle(adminSL.Average()) : 0;
                // OJT Eval
                var OJTEval = (TopicContentAVG + TopicObjectiveAVG + ApproriateTopicLevelAVG + TopicUsefulnessAVG + TrainingMaterialAVG
                    + TrainerKnowledgeAVG + SubjectCoverageAVG + InstructionAndCommunicateAVG + TrainerSupportAVG
                    + LogisticsAVG + InformationToTraineesAVG + AdminSupportAVG) / 12;
                // Trainning Program
                var TrainningProgram = (TopicContentAVG + TopicObjectiveAVG + ApproriateTopicLevelAVG + TopicUsefulnessAVG + TrainingMaterialAVG) / 5;
                var Trainer = (TrainerKnowledgeAVG + SubjectCoverageAVG + InstructionAndCommunicateAVG + TrainerSupportAVG) / 4;
                var Organization = (LogisticsAVG + InformationToTraineesAVG + AdminSupportAVG) / 3;
                var AverageScore = (TrainningProgram + Trainer + Organization) / 3;
                var fbReportAdd = new FeedbackReport
                {
                    TopicContent = TopicContentAVG,
                    TopicObjective = TopicObjectiveAVG,
                    ApproriateTopicLevel = ApproriateTopicLevelAVG,
                    TopicUsefulness = TopicContentAVG,
                    TrainingMaterial = TrainingMaterialAVG,
                    TrainerKnowledge = TrainerKnowledgeAVG,
                    SubjectCoverage = SubjectCoverageAVG,
                    InstructionAndCommunicate = InstructionAndCommunicateAVG,
                    TrainerSupport = TrainerSupportAVG,
                    Logistics = LogisticsAVG,
                    InformationToTrainees = InformationToTraineesAVG,
                    AdminSupport = AdminSupportAVG,
                    TrainingPrograms = TrainningProgram,
                    Trainer = Trainer,
                    Organization = Organization,
                    ContantEval = TrainningProgram,
                    TrainerEval = Trainer,
                    OrganizeEval = Organization,
                    OJTEval = OJTEval,
                    AverageScore = AverageScore,
                    ReportAt = new DateTime(reportAt.Year, reportAt.Month, 1),
                    IsSumary = false,
                };
                feedbackReports.Add(fbReportAdd);
                if (feedbackReports.Count() == 0)
                {
                    return null;
                }
            }
            return feedbackReports;
        }

        /// <summary>
        /// Calculate and return trainee marks and relates
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <returns>Object with all fields in topic grades</returns>
        public TopicGrades GetTopicGrades(int classId)
        {
            //Get all modules with class id
            var classModules = _dataContext.ClassModules.Where(f => f.ClassId == classId)
                                                        .Select(c => new ClassModule
                                                        {
                                                            ModuleId = c.ModuleId,
                                                            Module = c.Module,
                                                            WeightNumber = c.WeightNumber
                                                        })
                                                        .ToList();
            //Set startDate and endDate to check and set key for Dictionary
            DateTime startDate = _dataContext.Classes.Where(c => c.ClassId == classId).Select(d => d.StartDay).FirstOrDefault();
            DateTime endDate = _dataContext.Classes.Where(c => c.ClassId == classId).Select(d => d.EndDay).FirstOrDefault();
            //Add item to Topic Info
            List<TopicInfo> topicInfo = new List<TopicInfo>();
            foreach (var item in classModules)
            {
                var itemToResponse = new TopicInfo
                {
                    TopicId = item.ModuleId,
                    Name = item.Module.ModuleName,
                    Month = startDate,
                    MaxScore = item.Module.MaxScore,
                    PassingScore = item.Module.PassingScore,
                    WeightNumber = item.WeightNumber
                };
                //Move to the next month
                startDate = startDate.AddMonths(1);
                topicInfo.Add(itemToResponse);
            }

            List<AverageScoreInfo> averageScoreInfo = new List<AverageScoreInfo>();
            //Set startDate again
            startDate = _dataContext.Classes.Where(c => c.ClassId == classId).Select(d => d.StartDay).FirstOrDefault();
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
                //Move to the next month
                startDate = startDate.AddMonths(1);
                averageScoreInfo.Add(itemToResponse);
            }
            //Set startDate again
            startDate = _dataContext.Classes.Where(c => c.ClassId == classId).Select(d => d.StartDay).FirstOrDefault();
            //Set Dictionary's key = first month of class duration
            var key = startDate;
            //Variable to check if key exceed over the number of months
            var nextKeyMonth = startDate;
            var topicIds = topicInfo.Select(i => i.TopicId).ToList();
            //List of traineeId in class
            var trainees = _dataContext.Trainees.Where(f => f.ClassId == classId).Select(t => t.TraineeId).ToList();
            //Get mark by moduleId and traineeId
            var marks = _dataContext.Marks.Where(f => topicIds.Contains(f.ModuleId) && trainees.Contains(f.TraineeId)).OrderBy(c => c.TraineeId).ToList();
            Dictionary<DateTime, List<TraineeGrades>> traineeGrades = new Dictionary<DateTime, List<TraineeGrades>>();
            List<TraineeGrades> traineeGradeList = new List<TraineeGrades>();

            foreach (var i in classModules)
            {
                foreach (var item in marks)
                {
                    var itemToResponse = new TraineeGrades
                    {
                        TopicId = item.ModuleId,
                        TraineeId = item.TraineeId,
                        Score = item.Score
                    };
                    //Add by distinct ModuleId
                    if (itemToResponse.TopicId == i.ModuleId)
                    {
                        traineeGradeList.Add(itemToResponse);
                    }
                }
                //Add to dictionary
                traineeGrades.Add(key, traineeGradeList);
                traineeGradeList = new List<TraineeGrades>();
                if (nextKeyMonth.Year == endDate.Year && nextKeyMonth.Month == endDate.Month)
                {
                    key = startDate.AddMonths(-1);
                    nextKeyMonth = startDate.AddMonths(-1);
                }
                if (nextKeyMonth.Year <= endDate.Year || nextKeyMonth.Month < endDate.Month)
                {
                    nextKeyMonth = nextKeyMonth.AddMonths(1);
                    key = nextKeyMonth;
                }

            }
            Dictionary<DateTime, List<TraineeGrades>> traineeAvarageGrades = new Dictionary<DateTime, List<TraineeGrades>>();
            List<TraineeGrades> traineeAvarageGradeList = new List<TraineeGrades>();
            //Set back variable
            nextKeyMonth = startDate;
            key = startDate;
            foreach (var i in classModules)
            {
                foreach (var item in marks)
                {
                    var itemToResponse = new TraineeGrades
                    {
                        TopicId = item.ModuleId,
                        TraineeId = item.TraineeId,
                        Score = (item.Score * averageScoreInfo.Where(m => m.TopicId == item.ModuleId)
                                                                .Select(w => w.WeightNumber)
                                                                .FirstOrDefault())

                    };
                    //Add to list, distinct by ModuleId
                    if (itemToResponse.TopicId == i.ModuleId)
                    {
                        traineeAvarageGradeList.Add(itemToResponse);
                    }

                }
                //Add to dictionary
                traineeAvarageGrades.Add(key, traineeAvarageGradeList);
                traineeAvarageGradeList = new List<TraineeGrades>();
                if (nextKeyMonth.Year == endDate.Year && nextKeyMonth.Month == endDate.Month)
                {
                    key = startDate.AddMonths(-1);
                    nextKeyMonth = startDate.AddMonths(-1);
                }
                if (nextKeyMonth.Year <= endDate.Year || nextKeyMonth.Month < endDate.Month)
                {
                    nextKeyMonth = nextKeyMonth.AddMonths(1);
                    key = nextKeyMonth;
                }
            }


            List<TraineeGrades> finalMarks = new List<TraineeGrades>();
            //Calculate final max score
            var finalMaxScore = topicInfo.Sum(m => m.MaxScore * m.WeightNumber);
            foreach (var item in marks)
            {
                var itemToResponse = new TraineeGrades
                {
                    TopicId = 0,
                    TraineeId = item.TraineeId,
                    //Score * WeightNumber
                    Score = item.Score * (topicInfo.Where(m => m.TopicId == item.ModuleId).Select(w => w.WeightNumber).FirstOrDefault())
                };
                //Score = score/final max score
                // itemToResponse.Score = itemToResponse.Score / finalMaxScore;
                //If that traineeId exist in list
                if (finalMarks.Select(m => m.TraineeId).Contains(itemToResponse.TraineeId))
                {
                    //Sum the existed and adding mark
                    finalMarks.Where(m => m.TraineeId == itemToResponse.TraineeId).FirstOrDefault().Score = finalMarks.Where(m => m.TraineeId == itemToResponse.TraineeId).FirstOrDefault().Score + itemToResponse.Score;
                }
                else
                {
                    finalMarks.Add(itemToResponse);
                }
            }
            FinalMarksInfo finalMarksInfo = new FinalMarksInfo()
            {
                MaxScore = finalMaxScore,
                PassingScore = averageScoreInfo.Select(m => m.PassingScore).Sum(),
                WeightNumber = averageScoreInfo.Select(m => m.WeightNumber).Sum()
            };
            foreach (var item in finalMarks)
            {
                item.Score = item.Score / finalMarksInfo.WeightNumber;
            }

            TopicGrades topicGrades = new TopicGrades()
            {
                TopicInfos = topicInfo,
                TraineeTopicGrades = traineeGrades,
                AverageScoreInfos = averageScoreInfo,
                TraineeAverageGrades = traineeAvarageGrades,
                FinalMarksInfo = finalMarksInfo,
                FinalMarks = finalMarks
            };
            return topicGrades;
        }

        /// <summary>
        /// Call GetTopicGrades then summary number of trainee per classifications
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <returns>Report object with number of trainees per classifications</returns>
        public async Task<CheckpointReport> GetCheckpointReport(int classId)
        {
            IEnumerable<TraineeGPA> traineeGPAList = await GetTraineeGPAs(classId);

            if (traineeGPAList.Count() == 0)
                return null;

            CheckpointReport result = new CheckpointReport();
            foreach (var trainee in traineeGPAList)
            {
                switch (trainee.Level)
                {
                    case "A+":
                        result.Aplus++;
                        break;
                    case "A":
                        result.A++;
                        break;
                    case "B":
                        result.B++;
                        break;
                    case "C":
                        result.C++;
                        break;
                    case "D":
                        result.D++;
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Collect all pieces of data in every sheet of report then send it to export excel helper
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <returns>An excel report file</returns>
        public async Task<byte[]> GenerateTotalClassReport(int classId)
        {
            string path = "\\wwwroot\\ReportTemplate\\template.xlsx";
            string workingDirectory = Environment.CurrentDirectory;
            string pathToTest = workingDirectory + path;

            // using var stream = File.OpenRead(pathToTest);

            using var stream = await _megaHelper.Download(new Uri("https://mega.nz/file/gAxEiS5Q#0fghqJb5u17XOMgHE-MoorwG4_q6Q5MRK2uOE6p7eCw"));
            using (var package = new ExcelPackage())
            {
                await package.LoadAsync(stream);

                //Fill data to trainee general Info sheet
                var generalInfoSheet = package.Workbook.Worksheets[0];
                var traineeList = this.GetTraineesInfo(classId);
                var traineeInfoFill = generalInfoSheet.Cells[$"A4:T{traineeList.Count() + 4}"];
                traineeInfoFill.FillDataToCells(traineeList, (trainee, cells) =>
                {
                    cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                    cells[0].Value = trainee.EmpId;
                    cells[1].Value = trainee.Account;
                    cells[2].Value = trainee.Name;
                    cells[3].Value = "FA";
                    cells[4].Value = "Đại học FPT";
                    cells[5].Value = "ICT";
                    cells[6].Value = trainee.DOB;
                    cells[7].Value = trainee.Gender;
                    cells[8].Value = trainee.Email;
                    cells[9].Value = trainee.Phone;
                    cells[10].Value = trainee.Facebook;
                    cells[11].Value = "ĐH FPT";
                    cells[12].Value = (trainee.Status == null) ? "Learning" : trainee.Status;
                    cells[13].Value = trainee.StartDate;
                    cells[14].Value = trainee.EndDate;
                    cells[15].Formula = $"VLOOKUP(\"{trainee.Account}\",GPA!B4:I{5 + traineeList.Count - 1},8,0)";
                    cells[16].Formula = $"VLOOKUP(\"{trainee.Account}\",GPA!B4:L{5 + traineeList.Count - 1},9,0)";
                    cells[17].Value = (trainee.SalaryPaid == true) ? "Yes" : "No";
                    cells[18].Value = (trainee.OJT == true) ? "Yes" : "No";
                });

                //Fill data to reward and penalty sheet
                var rewardPenaltyList = this.GetRewardAndPenaltyScore(classId);
                var rewardPenaltySheet = package.Workbook.Worksheets[3];
                var rewardPenaltyFill = rewardPenaltySheet.Cells[$"A3:F{traineeList.Count() + 3}"];
                rewardPenaltyFill.FillDataToCells(rewardPenaltyList, (rewardPenalty, cells) =>
                {
                    cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                    var trainee = _dataContext.Trainees.FirstOrDefault(t => t.TraineeId == rewardPenalty.TraineeId);
                    cells[0].Value = trainee.TraineeId;
                    cells[1].Value = trainee.Username;
                    cells[2].Value = trainee.Fullname;
                    cells[3].Value = rewardPenalty.Date;
                    cells[4].Value = rewardPenalty.BonusAndPenaltyPoint;
                    cells[5].Value = rewardPenalty.Reason;
                });

                // Fill data to GPA sheet
                var GPAList = await this.GetTraineeGPAs(classId);
                var GPASheet = package.Workbook.Worksheets[4];
                var GPAFill = GPASheet.Cells["A4:L4"];
                // fill to trainee GPA list
                int i = 3;
                foreach (var item in GPAList)
                {
                    i++;
                    ICollection<TraineeGPA> list = new List<TraineeGPA>();
                    list.Add(item);
                    GPAFill = GPAFill.CreateNewRows(1);
                    GPAFill.FillDataToCells(list, (GPA, cells) =>
                    {
                        cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                        cells[0].Formula = $"'Trainee general info'!A{i}";
                        cells[1].Formula = $"'Trainee general info'!B{i}";
                        cells[2].Formula = $"'Trainee general info'!C{i}";
                        cells[3].Formula = $"'Trainee general info'!M{i}";
                        cells[4].Value = GPA.AcademicMark;
                        cells[5].Value = GPA.DisciplinaryPoint;
                        cells[6].Value = GPA.Bonus;
                        cells[7].Value = GPA.Penalty;
                        cells[8].Value = GPA.GPA;
                        cells[9].Value = GPA.Level;

                    });
                }
                // fill formula to count trainee and trainee status
                var countTraineeCell = GPASheet.Cells[$"C{6 + GPAList.Count()}"];
                countTraineeCell.Formula = $"COUNTA(C5:C{5 + GPAList.Count() - 1})";
                var countStatusCell = GPASheet.Cells[$"D{6 + GPAList.Count()}"];
                countStatusCell.Formula = $"COUNTIF(D5:D{5 + GPAList.Count() - 1},\"Passed\")";
                // fill formula to count trainee Checkpoint
                var aPlusCountCell = GPASheet.Cells[$"B{9 + GPAList.Count()}"];
                aPlusCountCell.Formula = $"COUNTIF(J5:J{5 + GPAList.Count() - 1},\"A+\")";
                var aCountCell = GPASheet.Cells[$"B{10 + GPAList.Count()}"];
                aCountCell.Formula = $"COUNTIF(J5:J{5 + GPAList.Count() - 1},\"A\")";
                var bCountCell = GPASheet.Cells[$"B{11 + GPAList.Count()}"];
                bCountCell.Formula = $"COUNTIF(J5:J{5 + GPAList.Count() - 1},\"B\")";
                var cCountCell = GPASheet.Cells[$"B{12 + GPAList.Count()}"];
                cCountCell.Formula = $"COUNTIF(J5:J{5 + GPAList.Count() - 1},\"C\")";
                var dCountCell = GPASheet.Cells[$"B{13 + GPAList.Count()}"];
                dCountCell.Formula = $"COUNTIF(J5:J{5 + GPAList.Count() - 1},\"D\")";
                // fill formula to calculate trainee Checkpoint rate
                var aPlusRateCell = GPASheet.Cells[$"C{9 + GPAList.Count()}"];
                aPlusRateCell.Formula = $"{aPlusCountCell.Address}/{countTraineeCell.Address}";
                var aRateCell = GPASheet.Cells[$"C{10 + GPAList.Count()}"];
                aRateCell.Formula = $"{aCountCell.Address}/{countTraineeCell.Address}";
                var bRateCell = GPASheet.Cells[$"C{11 + GPAList.Count()}"];
                bRateCell.Formula = $"{bCountCell.Address}/{countTraineeCell.Address}";
                var cRateCell = GPASheet.Cells[$"C{12 + GPAList.Count()}"];
                cRateCell.Formula = $"{cCountCell.Address}/{countTraineeCell.Address}";
                var dRateCell = GPASheet.Cells[$"C{13 + GPAList.Count()}"];
                dRateCell.Formula = $"{dCountCell.Address}/{countTraineeCell.Address}";

                // fill formula to count trainee status
                var learningCountCell = GPASheet.Cells[$"B{17 + GPAList.Count()}"];
                learningCountCell.Formula = $"COUNTIF(D5:D{5 + GPAList.Count() - 1},\"Learning\")";
                var passCountCell = GPASheet.Cells[$"B{18 + GPAList.Count()}"];
                passCountCell.Formula = $"COUNTIF(D5:D{5 + GPAList.Count() - 1},\"Passed\")";
                var failCountCell = GPASheet.Cells[$"B{19 + GPAList.Count()}"];
                failCountCell.Formula = $"COUNTIF(D5:D{5 + GPAList.Count() - 1},\"Failed\")";
                var deferCountCell = GPASheet.Cells[$"B{20 + GPAList.Count()}"];
                deferCountCell.Formula = $"COUNTIF(D5:D{5 + GPAList.Count() - 1},\"Deferred\")";
                var dropCountCell = GPASheet.Cells[$"B{21 + GPAList.Count()}"];
                dropCountCell.Formula = $"COUNTIF(D5:D{5 + GPAList.Count() - 1},\"Drop-out\")";
                var cancelCountCell = GPASheet.Cells[$"B{22 + GPAList.Count()}"];
                cancelCountCell.Formula = $"COUNTIF(D5:D{5 + GPAList.Count() - 1},\"Cancel\")";
                // fill formula to calculate trainee status rate
                var learningRateCell = GPASheet.Cells[$"C{17 + GPAList.Count()}"];
                learningRateCell.Formula = $"{learningCountCell.Address}/{countTraineeCell.Address}";
                var passRateCell = GPASheet.Cells[$"C{18 + GPAList.Count()}"];
                passRateCell.Formula = $"{passCountCell.Address}/{countTraineeCell.Address}";
                var failRateCell = GPASheet.Cells[$"C{19 + GPAList.Count()}"];
                failRateCell.Formula = $"{failCountCell.Address}/{countTraineeCell.Address}";
                var deferRateCell = GPASheet.Cells[$"C{20 + GPAList.Count()}"];
                deferRateCell.Formula = $"{deferCountCell.Address}/{countTraineeCell.Address}";
                var dropRateCell = GPASheet.Cells[$"C{21 + GPAList.Count()}"];
                dropRateCell.Formula = $"{dropCountCell.Address}/{countTraineeCell.Address}";
                var cancelRateCell = GPASheet.Cells[$"C{22 + GPAList.Count()}"];
                cancelRateCell.Formula = $"{cancelCountCell.Address}/{countTraineeCell.Address}";
                // draw pie chart for trainee check point
                var GPALabel = GPASheet.Cells[$"A{9 + GPAList.Count()}:A{13 + GPAList.Count()}"];
                var GPAValue = GPASheet.Cells[$"{aPlusCountCell.Address}:{dCountCell.Address}"];
                var pieChart = Helper.ExcelHelper.GeneratePieChart(GPASheet, "Tỉ lệ checkpoint", GPALabel, GPAValue);
                pieChart.Title.Text = "Tỉ lệ checkpoint";
                pieChart.SetPosition(8 + GPAList.Count(), 0, 5, 0);
                pieChart.SetSize(500, 350);

                //Fill data to Feedback total report
                var fbTotalReportSheet = package.Workbook.Worksheets[5];
                var feedbackAvgFill = fbTotalReportSheet.Cells["C11:N11"];
                var feedbackReportGet = this.GetFeedbackReport(classId);
                var fbReportTotal = feedbackReportGet.Where(f => f.IsSumary == true);
                feedbackAvgFill.FillDataToCells(fbReportTotal, (fb, cell) =>
                {
                    cell[0].Value = fb.TopicContent;
                    cell[1].Value = fb.TopicObjective;
                    cell[2].Value = fb.ApproriateTopicLevel;
                    cell[3].Value = fb.TopicUsefulness;
                    cell[4].Value = fb.TrainingMaterial;
                    cell[5].Value = fb.TrainerKnowledge;
                    cell[6].Value = fb.SubjectCoverage;
                    cell[7].Value = fb.InstructionAndCommunicate;
                    cell[8].Value = fb.TrainerSupport;
                    cell[9].Value = fb.Logistics;
                    cell[10].Value = fb.InformationToTrainees;
                    cell[11].Value = fb.AdminSupport;
                });
                //draw 2 chart for training feedback total
                var chart1Label = fbTotalReportSheet.Cells[$"C{20}:N{21}"];
                var chart1Value = fbTotalReportSheet.Cells[$"C{23}:N{23}"];
                var chart1 = Helper.ExcelHelper.GenerateColumnClusterChart(fbTotalReportSheet, "Course Evaluation", chart1Label, chart1Value);
                chart1.Title.Text = "Course Evaluation";
                chart1.SetPosition(26, 0, 1, 0);
                chart1.SetSize(600, 500);

                var chart2Label = fbTotalReportSheet.Cells[$"C{20}:O{20}"];
                var chart2Value = fbTotalReportSheet.Cells[$"C{24}:O{24}"];
                var chart2 = Helper.ExcelHelper.GenerateBarClusterChart(fbTotalReportSheet, "Brief Course Evaluation", chart2Label, chart2Value);
                chart2.Title.Text = "Brief Course Evaluation";
                chart2.SetPosition(26, 0, 8, 0);
                chart2.SetSize(600, 500);
                // Fill data to training Feedback each month
                var fbMonthReport = feedbackReportGet.Where(f => f.IsSumary == false);
                foreach (var monthFb in fbMonthReport)
                {
                    var fbMonthGet = this.GetTraineeFeedbacks(classId, monthFb.ReportAt);
                    if (fbMonthGet.Contains(null))
                    {
                        continue;
                    }
                    var fbMonthSheet = package.Workbook.Worksheets.Copy("Training  Feedback", $"Training Feedback {monthFb.ReportAt.Month}/{monthFb.ReportAt.Year}");
                    var fbFill = fbMonthSheet.Cells["B11:O11"];
                    foreach (var item in fbMonthGet)
                    {
                        fbFill = fbFill.CreateNewRows(1);
                        ICollection<TraineeFeedback> list = new List<TraineeFeedback>();
                        list.Add(item);
                        fbFill.FillDataToCells(list, (fb, cell) =>
                        {
                            cell.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                            cell[0].Value = fb.Email;
                            cell[1].Value = fb.TopicContent;
                            cell[2].Value = fb.TopicObjective;
                            cell[3].Value = fb.ApproriateTopicLevel;
                            cell[4].Value = fb.TopicUsefulness;
                            cell[5].Value = fb.TrainingMaterial;
                            cell[6].Value = fb.TrainerKnowledge;
                            cell[7].Value = fb.SubjectCoverage;
                            cell[8].Value = fb.InstructionAndCommunicate;
                            cell[9].Value = fb.TrainerSupport;
                            cell[10].Value = fb.Logistics;
                            cell[11].Value = fb.InformationToTrainees;
                            cell[12].Value = fb.AdminSupport;
                            cell[13].Value = fb.OtherComment;

                        });
                    }
                    // Add formula to calculate Average feedback point from C11 to N11
                    fbMonthSheet.Cells["C11"].Formula = $"AVERAGE(C12:C{12 + fbMonthGet.Count - 1})";
                    fbMonthSheet.Cells["D11"].Formula = $"AVERAGE(D12:D{12 + fbMonthGet.Count - 1})";
                    fbMonthSheet.Cells["F11"].Formula = $"AVERAGE(F12:F{12 + fbMonthGet.Count - 1})";
                    fbMonthSheet.Cells["E11"].Formula = $"AVERAGE(E12:E{12 + fbMonthGet.Count - 1})";
                    fbMonthSheet.Cells["G11"].Formula = $"AVERAGE(G12:G{12 + fbMonthGet.Count - 1})";
                    fbMonthSheet.Cells["H11"].Formula = $"AVERAGE(H12:H{12 + fbMonthGet.Count - 1})";
                    fbMonthSheet.Cells["I11"].Formula = $"AVERAGE(I12:I{12 + fbMonthGet.Count - 1})";
                    fbMonthSheet.Cells["J11"].Formula = $"AVERAGE(J12:J{12 + fbMonthGet.Count - 1})";
                    fbMonthSheet.Cells["K11"].Formula = $"AVERAGE(K12:K{12 + fbMonthGet.Count - 1})";
                    fbMonthSheet.Cells["L11"].Formula = $"AVERAGE(L12:L{12 + fbMonthGet.Count - 1})";
                    fbMonthSheet.Cells["M11"].Formula = $"AVERAGE(M12:M{12 + fbMonthGet.Count - 1})";
                    fbMonthSheet.Cells["N11"].Formula = $"AVERAGE(N12:N{12 + fbMonthGet.Count - 1})";

                    //draw Bar Chart for training feedback of each month
                    var monthChart1Label = fbMonthSheet.Cells[$"C{20 + fbMonthGet.Count}:N{21 + fbMonthGet.Count}"];
                    var monthChart1Value = fbMonthSheet.Cells[$"C{23 + fbMonthGet.Count}:N{23 + fbMonthGet.Count}"];
                    var monthChart1 = Helper.ExcelHelper.GenerateColumnClusterChart(fbMonthSheet, "Course Evaluation", monthChart1Label, monthChart1Value);
                    monthChart1.Title.Text = "Course Evaluation";
                    monthChart1.SetPosition(26 + fbMonthGet.Count, 0, 1, 0);
                    monthChart1.SetSize(600, 500);

                    var monthChart2Label = fbMonthSheet.Cells[$"C{20 + fbMonthGet.Count}:O{20 + fbMonthGet.Count}"];
                    var monthChart2Value = fbMonthSheet.Cells[$"C{24 + fbMonthGet.Count}:O{24 + fbMonthGet.Count}"];
                    var monthChart2 = Helper.ExcelHelper.GenerateBarClusterChart(fbMonthSheet, "Brief Course Evaluation", monthChart2Label, monthChart2Value);
                    monthChart2.Title.Text = "Brief Course Evaluation";
                    monthChart2.SetPosition(26 + fbMonthGet.Count, 0, 8, 0);
                    monthChart2.SetSize(600, 500);

                }

                // Fill data to trainees status of each day
                var attSheet = package.Workbook.Worksheets[1];
                var attFill = attSheet.Cells[$"E2:E{4 + traineeList.Count - 1}"];
                var attList = await this.GetAttendanceInfo(classId);
                foreach (var item in attList)
                {
                    var date = attFill.SelectSubRange(2, 1, 2, 1);
                    //     var a = item.Key;
                    date.Value = item.Key;
                    attFill.FillDataToCellsColumn(item.Value, (att, cell) =>
                    {
                        cell.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                        int i = 2;
                        foreach (var t in item.Value)
                        {
                            cell[i].Value = t.Status;
                            i++;
                        }
                    });
                    if (item.Equals(attList.Last()))
                    {
                        break;
                    }
                    var attNext = attFill.CreateNewColumns(1);
                    attFill.Copy(attNext);
                    attFill = attNext;
                }

                // Fill data to trainee info from A to D columnn
                var traineeInAttFill = attSheet.Cells[$"A4:D{4 + traineeList.Count - 1}"];
                List<int> traineePosition = new List<int>();
                for (int it = 4; it <= 4 + traineeList.Count - 1; it++)
                {
                    traineePosition.Add(it);
                }
                traineeInAttFill.FillDataToCells(traineePosition, (pos, cells) =>
                {
                    cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                    cells[0].Formula = $"'Trainee general info'!A{pos}";
                    cells[1].Formula = $"'Trainee general info'!B{pos}";
                    cells[2].Formula = $"'Trainee general info'!C{pos}";
                    cells[3].Formula = $"'Trainee general info'!M{pos}";
                });
                // Fill data to attendance report each month (Fill one month and create new 4 column to fill next month)
                var monthAttRpRange = attSheet.Cells[2, 5 + attList.Count, 4 + traineeList.Count - 1, 5 + attList.Count + 3]; //position of month Att report part in template
                var listMonth = this.GetMonthListOfClass(classId);
                foreach (var month in listMonth)
                {
                    var monthAttRpFill = monthAttRpRange.SelectSubRange(3, 1, 3 + traineeList.Count - 1, 4);
                    var monthFill = monthAttRpRange.SelectSubRange(1, 1, 1, 4);
                    var monthRp = this.GetAttendanceReportEachMonth(classId, month);

                    monthAttRpFill.FillDataToCells(monthRp.First().Value, (rp, cells) =>
                     {
                         cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                         cells[0].Value = rp.NumberOfAbsent;
                         cells[1].Value = rp.NumberOfLateInAndEarlyOut;
                         cells[2].Value = rp.NoPermissionRate;
                         cells[3].Value = rp.DisciplinaryPoint;
                     });
                    monthFill.Value = month;
                    if (month.Equals(listMonth.Last()))
                    {
                        break;
                    }
                    var nextMonthAttRange = monthAttRpRange.CreateNewColumns(4);
                    monthAttRpRange.Copy(nextMonthAttRange);
                    monthAttRpRange = nextMonthAttRange;

                }
                //Fill data to Attendance to total report
                var attRpTotal = this.GetTotalAttendanceReports(classId);
                var attRpTotalFill = attSheet.Cells[4, 5 + attList.Count + 4 * listMonth.Count(), 4 + traineeList.Count - 1, 5 + attList.Count + 4 * listMonth.Count() + 3];
                attRpTotalFill.FillDataToCells(attRpTotal, (rp, cells) =>
                {
                    cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                    cells[0].Value = rp.NumberOfAbsent;
                    cells[1].Value = rp.NumberOfLateInAndEarlyOut;
                    cells[2].Value = rp.NoPermissionRate;
                    cells[3].Value = rp.DisciplinaryPoint;
                });

                var topicGradeList = this.GetTopicGrades(classId);
                var topicGradeSheet = package.Workbook.Worksheets[2];
                // Fill data to topic Info (E2:E7)
                var topicInfoRange = topicGradeSheet.Cells[$"E1:E{7 + traineeList.Count}"];
                var topicInfoList = topicGradeList.TopicInfos;
                foreach (var item in topicInfoList)
                {
                    // var topicInfoFill = topicInfoRange.SelectSubRange(2, 1, 7+ traineeList.Count, 1);
                    ICollection<TopicInfo> list = new List<TopicInfo>();
                    list.Add(item);
                    topicInfoRange.FillDataToCellsColumn(list, (topic, cells) =>
                    {
                        cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                        cells[1].Value = topic.Month;
                        cells[2].Value = topic.Name;
                        cells[4].Value = topic.MaxScore;
                        cells[5].Value = topic.PassingScore;
                        cells[6].Value = topic.WeightNumber;
                    });
                    if (item.Equals(topicInfoList.Last()))
                    {
                        break;
                    }
                    var nextTopicInfoRange = topicInfoRange.CreateNewColumns(1);
                    topicInfoRange.Copy(nextTopicInfoRange);
                    topicInfoRange = nextTopicInfoRange;
                }
                //Fill data to trainee topic score
                var traineeGradesFill = topicGradeSheet.Cells[1, 5, 7 + traineeList.Count, 5].SelectSubRange(8, 1, 7 + traineeList.Count, 1);
                foreach (var dateScore in topicGradeList.TraineeTopicGrades)
                {
                    traineeGradesFill.FillDataToCellsColumn(dateScore.Value, (s, cells) =>
                    {

                        int i = 0;
                        foreach (var score in dateScore.Value)
                        {
                            cells[i].Value = score.Score;
                            i++;
                        }
                    });
                    traineeGradesFill = traineeGradesFill.MoveRight(1);
                }
                //Fill data to Average Score Info
                var avgScoreInfoRange = topicGradeSheet.Cells[1, 5 + topicInfoList.Count, 7 + traineeList.Count, 5 + topicInfoList.Count];
                var avgScoreInfoList = topicGradeList.AverageScoreInfos;
                foreach (var item in avgScoreInfoList)
                {
                    ICollection<AverageScoreInfo> list = new List<AverageScoreInfo>();
                    list.Add(item);
                    avgScoreInfoRange.FillDataToCellsColumn(list, (topic, cells) =>
                    {
                        cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                        cells[2].Value = topic.Month;
                        cells[4].Value = topic.MaxScore;
                        cells[5].Value = topic.PassingScore;
                        cells[6].Value = topic.WeightNumber;
                    });
                    if (item.Equals(avgScoreInfoList.Last()))
                    {
                        break;
                    }
                    var nextAvgScoreInfoRange = avgScoreInfoRange.CreateNewColumns(1);
                    avgScoreInfoRange.Copy(nextAvgScoreInfoRange);
                    avgScoreInfoRange = nextAvgScoreInfoRange;
                }
                //Merge Average Score title
                var avgTitleRange = topicGradeSheet.Cells[2, 5 + topicInfoList.Count, 2, 5 + topicInfoList.Count + avgScoreInfoList.Count - 1];
                avgTitleRange.Merge = true;
                //Fill data to trainee average grade
                var traineeAvgGradeFill = topicGradeSheet.Cells[8, 5 + topicInfoList.Count, 7 + traineeList.Count, 5 + topicInfoList.Count];
                foreach (var dateScore in topicGradeList.TraineeAverageGrades)
                {
                    traineeAvgGradeFill.FillDataToCellsColumn(dateScore.Value, (s, cells) =>
                    {

                        int i = 0;
                        foreach (var score in dateScore.Value)
                        {
                            cells[i].Value = score.Score;
                            i++;
                        }
                    });
                    traineeAvgGradeFill = traineeAvgGradeFill.MoveRight(1);
                }
                //Fill data to final mark info
                var finalMarkInfoRange = topicGradeSheet.Cells[1, 5 + topicInfoList.Count + avgScoreInfoList.Count, 7 + traineeList.Count, 5 + topicInfoList.Count + avgScoreInfoList.Count];
                var tempList = new List<FinalMarksInfo>();
                tempList.Add(topicGradeList.FinalMarksInfo);
                finalMarkInfoRange.FillDataToCellsColumn(tempList, (info, cells) =>
                {
                    cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                    cells[4].Value = info.MaxScore;
                    cells[5].Value = info.PassingScore;
                    cells[6].Value = info.WeightNumber;
                });
                //fill data to trainee final mark
                var finalMarkFill = topicGradeSheet.Cells[8, 5 + topicInfoList.Count + avgScoreInfoList.Count, 7 + traineeList.Count, 5 + topicInfoList.Count + avgScoreInfoList.Count];
                finalMarkFill.FillDataToCellsColumn(topicGradeList.FinalMarks, (s, cells) =>
                {
                    int i = 0;
                    foreach (var sc in topicGradeList.FinalMarks)
                    {
                        cells[i].Value = sc.Score;
                        i++;
                    }
                });
                // Fill formula to trainee in topic grade
                var traineeTopicGradeFill = topicGradeSheet.Cells[$"A8:D{8 + traineeList.Count - 1}"];
                traineeTopicGradeFill.FillDataToCells(traineePosition, (pos, cells) =>
                {
                    cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                    cells[0].Formula = $"'Trainee general info'!A{pos}";
                    cells[1].Formula = $"'Trainee general info'!B{pos}";
                    cells[2].Formula = $"'Trainee general info'!C{pos}";
                    cells[3].Formula = $"'Trainee general info'!M{pos}";
                });
                return await package.GetAsByteArrayAsync();
            }

        }

        public async Task<byte[]> GenerateClassReportEachMonth(int classId, DateTime reportAt)
        {
            string path = "\\wwwroot\\ReportTemplate\\template.xlsx";
            string workingDirectory = Environment.CurrentDirectory;
            string pathToTest = workingDirectory + path;

            // using var stream = File.OpenRead(pathToTest);
            using var stream = await _megaHelper.Download(new Uri("https://mega.nz/file/gAxEiS5Q#0fghqJb5u17XOMgHE-MoorwG4_q6Q5MRK2uOE6p7eCw"));
            using (var package = new ExcelPackage())
            {
                await package.LoadAsync(stream);

                //Fill data to trainee general Info sheet
                var generalInfoSheet = package.Workbook.Worksheets[0];
                var traineeList = this.GetTraineesInfo(classId);
                var traineeInfoFill = generalInfoSheet.Cells[$"A4:T{traineeList.Count() + 4}"];
                traineeInfoFill.FillDataToCells(traineeList, (trainee, cells) =>
                {
                    cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                    cells[0].Value = trainee.EmpId;
                    cells[1].Value = trainee.Account;
                    cells[2].Value = trainee.Name;
                    cells[3].Value = "FA";
                    cells[4].Value = "Đại học FPT";
                    cells[5].Value = "ICT";
                    cells[6].Value = trainee.DOB;
                    cells[7].Value = trainee.Gender;
                    cells[8].Value = trainee.Email;
                    cells[9].Value = trainee.Phone;
                    cells[10].Value = trainee.Facebook;
                    cells[11].Value = "ĐH FPT";
                    cells[12].Value = (trainee.Status == null) ? "Learning" : trainee.Status;
                    cells[13].Value = trainee.StartDate;
                    cells[14].Value = trainee.EndDate;
                    cells[15].Formula = $"VLOOKUP(\"{trainee.Account}\",GPA!B4:I{5 + traineeList.Count - 1},8,0)";
                    cells[16].Formula = $"VLOOKUP(\"{trainee.Account}\",GPA!B4:L{5 + traineeList.Count - 1},9,0)";
                    cells[17].Value = (trainee.SalaryPaid == true) ? "Yes" : "No";
                    cells[18].Value = (trainee.OJT == true) ? "Yes" : "No";
                });

                //Fill data to reward and penalty sheet
                var rewardPenaltyList = this.GetRewardAndPenaltyScore(classId, reportAt);
                var rewardPenaltySheet = package.Workbook.Worksheets[3];
                var rewardPenaltyFill = rewardPenaltySheet.Cells[$"A3:F{traineeList.Count() + 3}"];
                rewardPenaltyFill.FillDataToCells(rewardPenaltyList, (rewardPenalty, cells) =>
                {
                    cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                    var trainee = _dataContext.Trainees.FirstOrDefault(t => t.TraineeId == rewardPenalty.TraineeId);
                    cells[0].Value = trainee.TraineeId;
                    cells[1].Value = trainee.Username;
                    cells[2].Value = trainee.Fullname;
                    cells[3].Value = rewardPenalty.Date;
                    cells[4].Value = rewardPenalty.BonusAndPenaltyPoint;
                    cells[5].Value = rewardPenalty.Reason;
                });

                // Fill data to GPA sheet
                var GPAList = await this.GetTraineeGPAs(classId);
                var GPASheet = package.Workbook.Worksheets[4];
                var GPAFill = GPASheet.Cells["A4:L4"];
                // fill to trainee GPA list
                int i = 3;
                foreach (var item in GPAList)
                {
                    i++;
                    ICollection<TraineeGPA> list = new List<TraineeGPA>();
                    list.Add(item);
                    GPAFill = GPAFill.CreateNewRows(1);
                    GPAFill.FillDataToCells(list, (GPA, cells) =>
                    {
                        cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                        cells[0].Formula = $"'Trainee general info'!A{i}";
                        cells[1].Formula = $"'Trainee general info'!B{i}";
                        cells[2].Formula = $"'Trainee general info'!C{i}";
                        cells[3].Formula = $"'Trainee general info'!M{i}";
                        cells[4].Value = GPA.AcademicMark;
                        cells[5].Value = GPA.DisciplinaryPoint;
                        cells[6].Value = GPA.Bonus;
                        cells[7].Value = GPA.Penalty;
                        cells[8].Value = GPA.GPA;
                        cells[9].Value = GPA.Level;

                    });
                }
                // fill formula to count trainee and trainee status
                var countTraineeCell = GPASheet.Cells[$"C{6 + GPAList.Count()}"];
                countTraineeCell.Formula = $"COUNTA(C5:C{5 + GPAList.Count() - 1})";
                var countStatusCell = GPASheet.Cells[$"D{6 + GPAList.Count()}"];
                countStatusCell.Formula = $"COUNTIF(D5:D{5 + GPAList.Count() - 1},\"Passed\")";
                // fill formula to count trainee Checkpoint
                var aPlusCountCell = GPASheet.Cells[$"B{9 + GPAList.Count()}"];
                aPlusCountCell.Formula = $"COUNTIF(J5:J{5 + GPAList.Count() - 1},\"A+\")";
                var aCountCell = GPASheet.Cells[$"B{10 + GPAList.Count()}"];
                aCountCell.Formula = $"COUNTIF(J5:J{5 + GPAList.Count() - 1},\"A\")";
                var bCountCell = GPASheet.Cells[$"B{11 + GPAList.Count()}"];
                bCountCell.Formula = $"COUNTIF(J5:J{5 + GPAList.Count() - 1},\"B\")";
                var cCountCell = GPASheet.Cells[$"B{12 + GPAList.Count()}"];
                cCountCell.Formula = $"COUNTIF(J5:J{5 + GPAList.Count() - 1},\"C\")";
                var dCountCell = GPASheet.Cells[$"B{13 + GPAList.Count()}"];
                dCountCell.Formula = $"COUNTIF(J5:J{5 + GPAList.Count() - 1},\"D\")";
                // fill formula to calculate trainee Checkpoint rate
                var aPlusRateCell = GPASheet.Cells[$"C{9 + GPAList.Count()}"];
                aPlusRateCell.Formula = $"{aPlusCountCell.Address}/{countTraineeCell.Address}";
                var aRateCell = GPASheet.Cells[$"C{10 + GPAList.Count()}"];
                aRateCell.Formula = $"{aCountCell.Address}/{countTraineeCell.Address}";
                var bRateCell = GPASheet.Cells[$"C{11 + GPAList.Count()}"];
                bRateCell.Formula = $"{bCountCell.Address}/{countTraineeCell.Address}";
                var cRateCell = GPASheet.Cells[$"C{12 + GPAList.Count()}"];
                cRateCell.Formula = $"{cCountCell.Address}/{countTraineeCell.Address}";
                var dRateCell = GPASheet.Cells[$"C{13 + GPAList.Count()}"];
                dRateCell.Formula = $"{dCountCell.Address}/{countTraineeCell.Address}";

                // fill formula to count trainee status
                var learningCountCell = GPASheet.Cells[$"B{17 + GPAList.Count()}"];
                learningCountCell.Formula = $"COUNTIF(D5:D{5 + GPAList.Count() - 1},\"Learning\")";
                var passCountCell = GPASheet.Cells[$"B{18 + GPAList.Count()}"];
                passCountCell.Formula = $"COUNTIF(D5:D{5 + GPAList.Count() - 1},\"Passed\")";
                var failCountCell = GPASheet.Cells[$"B{19 + GPAList.Count()}"];
                failCountCell.Formula = $"COUNTIF(D5:D{5 + GPAList.Count() - 1},\"Failed\")";
                var deferCountCell = GPASheet.Cells[$"B{20 + GPAList.Count()}"];
                deferCountCell.Formula = $"COUNTIF(D5:D{5 + GPAList.Count() - 1},\"Deferred\")";
                var dropCountCell = GPASheet.Cells[$"B{21 + GPAList.Count()}"];
                dropCountCell.Formula = $"COUNTIF(D5:D{5 + GPAList.Count() - 1},\"Drop-out\")";
                var cancelCountCell = GPASheet.Cells[$"B{22 + GPAList.Count()}"];
                cancelCountCell.Formula = $"COUNTIF(D5:D{5 + GPAList.Count() - 1},\"Cancel\")";
                // fill formula to calculate trainee status rate
                var learningRateCell = GPASheet.Cells[$"C{17 + GPAList.Count()}"];
                learningRateCell.Formula = $"{learningCountCell.Address}/{countTraineeCell.Address}";
                var passRateCell = GPASheet.Cells[$"C{18 + GPAList.Count()}"];
                passRateCell.Formula = $"{passCountCell.Address}/{countTraineeCell.Address}";
                var failRateCell = GPASheet.Cells[$"C{19 + GPAList.Count()}"];
                failRateCell.Formula = $"{failCountCell.Address}/{countTraineeCell.Address}";
                var deferRateCell = GPASheet.Cells[$"C{20 + GPAList.Count()}"];
                deferRateCell.Formula = $"{deferCountCell.Address}/{countTraineeCell.Address}";
                var dropRateCell = GPASheet.Cells[$"C{21 + GPAList.Count()}"];
                dropRateCell.Formula = $"{dropCountCell.Address}/{countTraineeCell.Address}";
                var cancelRateCell = GPASheet.Cells[$"C{22 + GPAList.Count()}"];
                cancelRateCell.Formula = $"{cancelCountCell.Address}/{countTraineeCell.Address}";
                // draw pie chart for trainee check point
                var GPALabel = GPASheet.Cells[$"A{9 + GPAList.Count()}:A{13 + GPAList.Count()}"];
                var GPAValue = GPASheet.Cells[$"{aPlusCountCell.Address}:{dCountCell.Address}"];
                var pieChart = Helper.ExcelHelper.GeneratePieChart(GPASheet, "Tỉ lệ checkpoint", GPALabel, GPAValue);
                pieChart.Title.Text = "Tỉ lệ checkpoint";
                pieChart.SetPosition(8 + GPAList.Count(), 0, 5, 0);
                pieChart.SetSize(500, 350);

                // Fill data to training Feedback each month
                var fbMonthSheet = package.Workbook.Worksheets[5];
                var fbMonthGet = this.GetTraineeFeedbacks(classId, reportAt);

                var fbFill = fbMonthSheet.Cells["B11:O11"];
                foreach (var item in fbMonthGet)
                {
                    fbFill = fbFill.CreateNewRows(1);
                    ICollection<TraineeFeedback> list = new List<TraineeFeedback>();
                    list.Add(item);
                    fbFill.FillDataToCells(list, (fb, cell) =>
                    {
                        cell.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                        cell[0].Value = fb.Email;
                        cell[1].Value = fb.TopicContent;
                        cell[2].Value = fb.TopicObjective;
                        cell[3].Value = fb.ApproriateTopicLevel;
                        cell[4].Value = fb.TopicUsefulness;
                        cell[5].Value = fb.TrainingMaterial;
                        cell[6].Value = fb.TrainerKnowledge;
                        cell[7].Value = fb.SubjectCoverage;
                        cell[8].Value = fb.InstructionAndCommunicate;
                        cell[9].Value = fb.TrainerSupport;
                        cell[10].Value = fb.Logistics;
                        cell[11].Value = fb.InformationToTrainees;
                        cell[12].Value = fb.AdminSupport;
                        cell[13].Value = fb.OtherComment;

                    });
                }
                // Add formula to calculate Average feedback point from C11 to N11
                fbMonthSheet.Cells["C11"].Formula = $"AVERAGE(C12:C{12 + fbMonthGet.Count - 1})";
                fbMonthSheet.Cells["D11"].Formula = $"AVERAGE(D12:D{12 + fbMonthGet.Count - 1})";
                fbMonthSheet.Cells["F11"].Formula = $"AVERAGE(F12:F{12 + fbMonthGet.Count - 1})";
                fbMonthSheet.Cells["E11"].Formula = $"AVERAGE(E12:E{12 + fbMonthGet.Count - 1})";
                fbMonthSheet.Cells["G11"].Formula = $"AVERAGE(G12:G{12 + fbMonthGet.Count - 1})";
                fbMonthSheet.Cells["H11"].Formula = $"AVERAGE(H12:H{12 + fbMonthGet.Count - 1})";
                fbMonthSheet.Cells["I11"].Formula = $"AVERAGE(I12:I{12 + fbMonthGet.Count - 1})";
                fbMonthSheet.Cells["J11"].Formula = $"AVERAGE(J12:J{12 + fbMonthGet.Count - 1})";
                fbMonthSheet.Cells["K11"].Formula = $"AVERAGE(K12:K{12 + fbMonthGet.Count - 1})";
                fbMonthSheet.Cells["L11"].Formula = $"AVERAGE(L12:L{12 + fbMonthGet.Count - 1})";
                fbMonthSheet.Cells["M11"].Formula = $"AVERAGE(M12:M{12 + fbMonthGet.Count - 1})";
                fbMonthSheet.Cells["N11"].Formula = $"AVERAGE(N12:N{12 + fbMonthGet.Count - 1})";

                //draw Bar Chart for training feedback of each month
                var monthChart1Label = fbMonthSheet.Cells[$"C{20 + fbMonthGet.Count}:N{21 + fbMonthGet.Count}"];
                var monthChart1Value = fbMonthSheet.Cells[$"C{23 + fbMonthGet.Count}:N{23 + fbMonthGet.Count}"];
                var monthChart1 = Helper.ExcelHelper.GenerateColumnClusterChart(fbMonthSheet, "Course Evaluation", monthChart1Label, monthChart1Value);
                monthChart1.Title.Text = "Course Evaluation";
                monthChart1.SetPosition(26 + fbMonthGet.Count, 0, 1, 0);
                monthChart1.SetSize(600, 500);

                var monthChart2Label = fbMonthSheet.Cells[$"C{20 + fbMonthGet.Count}:O{20 + fbMonthGet.Count}"];
                var monthChart2Value = fbMonthSheet.Cells[$"C{24 + fbMonthGet.Count}:O{24 + fbMonthGet.Count}"];
                var monthChart2 = Helper.ExcelHelper.GenerateBarClusterChart(fbMonthSheet, "Brief Course Evaluation", monthChart2Label, monthChart2Value);
                monthChart2.Title.Text = "Brief Course Evaluation";
                monthChart2.SetPosition(26 + fbMonthGet.Count, 0, 8, 0);
                monthChart2.SetSize(600, 500);

                // Fill data to trainees status of each day
                var attSheet = package.Workbook.Worksheets[1];
                var attFill = attSheet.Cells[$"E2:E{4 + traineeList.Count - 1}"];
                var attList = await this.GetAttendanceInfo(classId, reportAt);
                foreach (var item in attList)
                {
                    var date = attFill.SelectSubRange(2, 1, 2, 1);
                    date.Value = item.Key;
                    attFill.FillDataToCellsColumn(item.Value, (att, cell) =>
                    {
                        cell.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                        int i = 2;
                        foreach (var t in item.Value)
                        {
                            cell[i].Value = t.Status;
                            i++;
                        }
                    });
                    if (item.Equals(attList.Last()))
                    {
                        break;
                    }
                    var attNext = attFill.CreateNewColumns(1);
                    attFill.Copy(attNext);
                    attFill = attNext;
                }

                // Fill data to trainee info from A to D columnn
                var traineeInAttFill = attSheet.Cells[$"A4:D{4 + traineeList.Count - 1}"];
                List<int> traineePosition = new List<int>();
                for (int it = 4; it <= 4 + traineeList.Count - 1; it++)
                {
                    traineePosition.Add(it);
                }
                traineeInAttFill.FillDataToCells(traineePosition, (pos, cells) =>
                {
                    cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                    cells[0].Formula = $"'Trainee general info'!A{pos}";
                    cells[1].Formula = $"'Trainee general info'!B{pos}";
                    cells[2].Formula = $"'Trainee general info'!C{pos}";
                    cells[3].Formula = $"'Trainee general info'!M{pos}";
                });
                // Fill data to attendance report each month 
                var monthAttRpRange = attSheet.Cells[2, 5 + attList.Count, 4 + traineeList.Count - 1, 5 + attList.Count + 3]; //position of month Att report part in template

                var monthAttRpFill = monthAttRpRange.SelectSubRange(3, 1, 3 + traineeList.Count - 1, 4);
                var monthFill = monthAttRpRange.SelectSubRange(1, 1, 1, 4);
                var monthRp = this.GetAttendanceReportEachMonth(classId, reportAt);

                monthAttRpFill.FillDataToCells(monthRp.First().Value, (rp, cells) =>
                 {
                     cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                     cells[0].Value = rp.NumberOfAbsent;
                     cells[1].Value = rp.NumberOfLateInAndEarlyOut;
                     cells[2].Value = rp.NoPermissionRate;
                     cells[3].Value = rp.DisciplinaryPoint;
                 });
                monthFill.Value = reportAt;

                //Fill data to Attendance to total report
                var attRpTotal = this.GetTotalAttendanceReports(classId);
                var attRpTotalFill = attSheet.Cells[4, 5 + attList.Count + 4, 4 + traineeList.Count - 1, 5 + attList.Count + 4 + 3];
                attRpTotalFill.FillDataToCells(attRpTotal, (rp, cells) =>
                {
                    cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                    cells[0].Value = rp.NumberOfAbsent;
                    cells[1].Value = rp.NumberOfLateInAndEarlyOut;
                    cells[2].Value = rp.NoPermissionRate;
                    cells[3].Value = rp.DisciplinaryPoint;
                });

                var topicGradeList = this.GetTopicGrades(classId);
                var topicGradeSheet = package.Workbook.Worksheets[2];
                // Fill data to topic Info (E2:E7)
                var topicInfoRange = topicGradeSheet.Cells[$"E1:E{7 + traineeList.Count}"];
                var topicInfoList = topicGradeList.TopicInfos;
                foreach (var item in topicInfoList)
                {
                    // var topicInfoFill = topicInfoRange.SelectSubRange(2, 1, 7+ traineeList.Count, 1);
                    ICollection<TopicInfo> list = new List<TopicInfo>();
                    list.Add(item);
                    topicInfoRange.FillDataToCellsColumn(list, (topic, cells) =>
                    {
                        cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                        cells[1].Value = topic.Month;
                        cells[2].Value = topic.Name;
                        cells[4].Value = topic.MaxScore;
                        cells[5].Value = topic.PassingScore;
                        cells[6].Value = topic.WeightNumber;
                    });
                    if (item.Equals(topicInfoList.Last()))
                    {
                        break;
                    }
                    var nextTopicInfoRange = topicInfoRange.CreateNewColumns(1);
                    topicInfoRange.Copy(nextTopicInfoRange);
                    topicInfoRange = nextTopicInfoRange;
                }
                //Fill data to trainee topic score
                var traineeGradesFill = topicGradeSheet.Cells[1, 5, 7 + traineeList.Count, 5].SelectSubRange(8, 1, 7 + traineeList.Count, 1);
                foreach (var dateScore in topicGradeList.TraineeTopicGrades)
                {
                    traineeGradesFill.FillDataToCellsColumn(dateScore.Value, (s, cells) =>
                    {

                        int i = 0;
                        foreach (var score in dateScore.Value)
                        {
                            cells[i].Value = score.Score;
                            i++;
                        }
                    });
                    traineeGradesFill = traineeGradesFill.MoveRight(1);
                }
                //Fill data to Average Score Info
                var avgScoreInfoRange = topicGradeSheet.Cells[1, 5 + topicInfoList.Count, 7 + traineeList.Count, 5 + topicInfoList.Count];
                var avgScoreInfoList = topicGradeList.AverageScoreInfos;
                foreach (var item in avgScoreInfoList)
                {
                    ICollection<AverageScoreInfo> list = new List<AverageScoreInfo>();
                    list.Add(item);
                    avgScoreInfoRange.FillDataToCellsColumn(list, (topic, cells) =>
                    {
                        cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                        cells[2].Value = topic.Month;
                        cells[4].Value = topic.MaxScore;
                        cells[5].Value = topic.PassingScore;
                        cells[6].Value = topic.WeightNumber;
                    });
                    if (item.Equals(avgScoreInfoList.Last()))
                    {
                        break;
                    }
                    var nextAvgScoreInfoRange = avgScoreInfoRange.CreateNewColumns(1);
                    avgScoreInfoRange.Copy(nextAvgScoreInfoRange);
                    avgScoreInfoRange = nextAvgScoreInfoRange;
                }
                //Merge Average Score title
                var avgTitleRange = topicGradeSheet.Cells[2, 5 + topicInfoList.Count, 2, 5 + topicInfoList.Count + avgScoreInfoList.Count - 1];
                avgTitleRange.Merge = true;
                //Fill data to trainee average grade
                var traineeAvgGradeFill = topicGradeSheet.Cells[8, 5 + topicInfoList.Count, 7 + traineeList.Count, 5 + topicInfoList.Count];
                foreach (var dateScore in topicGradeList.TraineeAverageGrades)
                {
                    traineeAvgGradeFill.FillDataToCellsColumn(dateScore.Value, (s, cells) =>
                    {

                        int i = 0;
                        foreach (var score in dateScore.Value)
                        {
                            cells[i].Value = score.Score;
                            i++;
                        }
                    });
                    traineeAvgGradeFill = traineeAvgGradeFill.MoveRight(1);
                }
                //Fill data to final mark info
                var finalMarkInfoRange = topicGradeSheet.Cells[1, 5 + topicInfoList.Count + avgScoreInfoList.Count, 7 + traineeList.Count, 5 + topicInfoList.Count + avgScoreInfoList.Count];
                var tempList = new List<FinalMarksInfo>();
                tempList.Add(topicGradeList.FinalMarksInfo);
                finalMarkInfoRange.FillDataToCellsColumn(tempList, (info, cells) =>
                {
                    cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                    cells[4].Value = info.MaxScore;
                    cells[5].Value = info.PassingScore;
                    cells[6].Value = info.WeightNumber;
                });
                //fill data to trainee final mark
                var finalMarkFill = topicGradeSheet.Cells[8, 5 + topicInfoList.Count + avgScoreInfoList.Count, 7 + traineeList.Count, 5 + topicInfoList.Count + avgScoreInfoList.Count];
                finalMarkFill.FillDataToCellsColumn(topicGradeList.FinalMarks, (s, cells) =>
                {
                    int i = 0;
                    foreach (var sc in topicGradeList.FinalMarks)
                    {
                        cells[i].Value = sc.Score;
                        i++;
                    }
                });
                // Fill formula to trainee in topic grade
                var traineeTopicGradeFill = topicGradeSheet.Cells[$"A8:D{8 + traineeList.Count - 1}"];
                traineeTopicGradeFill.FillDataToCells(traineePosition, (pos, cells) =>
                {
                    cells.ToList().ForEach(c => c.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin));
                    cells[0].Formula = $"'Trainee general info'!A{pos}";
                    cells[1].Formula = $"'Trainee general info'!B{pos}";
                    cells[2].Formula = $"'Trainee general info'!C{pos}";
                    cells[3].Formula = $"'Trainee general info'!M{pos}";
                });
                return await package.GetAsByteArrayAsync();
            }

        }
    }
}