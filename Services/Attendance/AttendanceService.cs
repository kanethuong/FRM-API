using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Helper;
using kroniiapi.Services.Attendance;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.AttendanceServicesss
{
    public class AttendanceService : IAttendanceService
    {
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
                    CalendarId = calendarId,
                    TraineeId = item.TraineeId,
                    IsAbsent = false,
                };
                _datacontext.Attendances.Add(attendanceNew);
            }
            int rowinserted = await _datacontext.SaveChangesAsync();
            return rowinserted;
        }
    }
}