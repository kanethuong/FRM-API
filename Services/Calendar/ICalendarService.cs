using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public interface ICalendarService
    {
        Task<IEnumerable<Calendar>> GetCalendarsByTraineeId(int traineeId,DateTime startDate, DateTime endDate);
        Task<List<int>> GetCalendarsIdListByModuleAndClassId(int moduleId, int classId);
    }
}