using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services.Attendance
{
    public interface IAttendanceService
    {
        Task<int> AddNewAttendance(int calendarId, IEnumerable<Trainee> traineesList);
    }
}