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
        private enum _attendanceStatus
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

            if(attendanceList.Count() == 0)
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
    }
}