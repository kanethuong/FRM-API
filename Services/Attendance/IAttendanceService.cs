using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO.TraineeDTO;

namespace kroniiapi.Services.Attendance
{
    public interface IAttendanceService
    {
        Task<int> AddNewAttendance(int calendarId, IEnumerable<Trainee> traineesList);
        Task<IEnumerable<DB.Models.Attendance>> GetAttendanceListByTraineeId(int traineeId);
        Task<TraineeAttendanceReport> GetTraineeAttendanceReport(int traineeId);
        Task<int> InitAttendanceWhenCreateClass(int classId);
        
    }
}