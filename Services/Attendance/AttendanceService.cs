using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TraineeDTO;
using kroniiapi.Helper;
using kroniiapi.Services.Attendance;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.AttendanceServicesss
{
    public class AttendanceService : IAttendanceService
    {
        public enum _attendanceStatus
        {
            A,
            P,
            L,
            Ln,
            E,
            En,
            An,
            Ob
        }
        private DataContext _datacontext;
        public AttendanceService(DataContext dataContext)
        {
            _datacontext = dataContext;
        }
        public async Task<int> AddNewAttendance(int calendarId, IEnumerable<Trainee> traineesList)
        {
            foreach (var item in traineesList)
            {
                var attendanceNew = new Attendance
                {
                    // CalendarId = calendarId,
                    TraineeId = item.TraineeId,
                    // IsAbsent = false,
                };
                _datacontext.Attendances.Add(attendanceNew);
            }
            int rowinserted = await _datacontext.SaveChangesAsync();
            return rowinserted;
        }

        /// <summary>
        /// get attendace list by trainee id
        /// </summary>
        /// <param name="traineeId"></param>
        /// <returns>attendance list</returns>
        public async Task<IEnumerable<Attendance>> GetAttendanceListByTraineeId(int traineeId)
        {
            return await _datacontext.Attendances.Where(t => t.TraineeId == traineeId).ToListAsync();
        }

        /// <summary>
        /// get number of day per status code (ex: 5 day absent)
        /// </summary>
        /// <param name="traineeId"></param>
        /// <returns>Attendance report</returns>
        public async Task<TraineeAttendanceReport> GetTraineeAttendanceReport(int traineeId)
        {
            TraineeAttendanceReport attendanceReport = new TraineeAttendanceReport
            {
                NumberOfSlotAbsent = 0,
                NumberOfSlotPresent = 0,
                NumberOfSlotLate = 0,
                NumberOfSlotLateNoReason = 0,
                NumberOfSlotEarly = 0,
                NumberOfSlotEarlyNoReason = 0,
                NumberOfSlotAbsentNoReason = 0
            };

            IEnumerable<Attendance> attendanceList = await GetAttendanceListByTraineeId(traineeId);

            if (attendanceList.Count() == 0)
                return null;

            foreach (var day in attendanceList)
            {
                switch (day.Status)
                {
                    case nameof(_attendanceStatus.A):
                        attendanceReport.NumberOfSlotAbsent++;
                        break;
                    case nameof(_attendanceStatus.An):
                        attendanceReport.NumberOfSlotAbsentNoReason++;
                        break;
                    case nameof(_attendanceStatus.E):
                        attendanceReport.NumberOfSlotEarly++;
                        break;
                    case nameof(_attendanceStatus.En):
                        attendanceReport.NumberOfSlotEarlyNoReason++;
                        break;
                    case nameof(_attendanceStatus.L):
                        attendanceReport.NumberOfSlotLate++;
                        break;
                    case nameof(_attendanceStatus.Ln):
                        attendanceReport.NumberOfSlotLateNoReason++;
                        break;
                    case nameof(_attendanceStatus.P):
                        attendanceReport.NumberOfSlotPresent++;
                        break;
                }
            }
            return attendanceReport;
        }

        List<DateTime> holidayss = new List<DateTime> {
            // New Year
            new DateTime(2000, 1, 1),
            //
            new DateTime(2000, 4, 30),
            //
            new DateTime(2000, 5, 1),
            //
            new DateTime(2000,9,1),
            //
            new DateTime(2000, 9, 2),
            //
            new DateTime(2000,9,3),
            //
        };

        /// <summary>
        /// Check if Date is a Day Off
        /// </summary>
        /// <param name="date"></param>
        /// <returns>true or false</returns>
        private bool DayOffCheck(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }
            foreach (var item in holidayss)
            {
                if (date.Day == item.Day && date.Month == item.Month)
                {
                    return true;
                }
            }

            DateTime lunarDate = VietnameseLunarDateConverter.LunarDate(date.Day,date.Month,date.Year);

            if(lunarDate.Month == 5 && lunarDate.Day == 10)
            {
                return true;
            }
            if(lunarDate.Month == 12 && lunarDate.Day == 30)
            {
                return true;
            }
            if(lunarDate.Month == 1 && (lunarDate.Day == 1 || lunarDate.Day == 2 || lunarDate.Day == 2))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <returns>-2: already have attendance/-1:not existed / 0:fail / 1:success</returns>
        public async Task<int> InitAttendanceWhenCreateClass(int classId)
        {
            Class classInfor = await _datacontext.Classes.Where(c => c.ClassId == classId && c.IsDeactivated == false).FirstOrDefaultAsync();

            if (classInfor == null)
                return -1;

            IEnumerable<Trainee> listTraineeInClass = await _datacontext.Trainees.Where(
                t => t.ClassId == classId && t.IsDeactivated == false).ToListAsync();

            if (listTraineeInClass.Count() == 0)
                return -1;


            //because time alway 0 o'clock add 1 day to add final day to attendance
            DateTime EndDay = classInfor.EndDay.AddDays(1.0);

            foreach (var trainee in listTraineeInClass)
            {
                for (DateTime date = classInfor.StartDay; date <= EndDay; date = date.AddDays(1.0))
                {
                    if (!DayOffCheck(date))
                    {
                        var attendanceForCheckDuplicate = await _datacontext.Attendances.Where(
                            t => t.Date == date.AddHours(8) && t.TraineeId == trainee.TraineeId).FirstOrDefaultAsync();
                        if (attendanceForCheckDuplicate != null)
                        {
                            _datacontext.ChangeTracker.Clear();
                            return -2;
                        }
                        _datacontext.Attendances.AddRange(new Attendance
                        {
                            Status = nameof(_attendanceStatus.P),
                            Reason = "",
                            Date = new DateTime(date.Year, date.Month, date.Day, 8, 0, 0),
                            TraineeId = trainee.TraineeId
                        });
                    }
                }
            }

            int rowInserted = await _datacontext.SaveChangesAsync();

            if (rowInserted != 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}