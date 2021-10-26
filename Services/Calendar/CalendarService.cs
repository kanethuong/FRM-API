using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly DataContext _dataContext;
        public CalendarService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IEnumerable<Calendar>> GetCalendarsByTraineeId(int traineeId,DateTime startDate, DateTime endDate){
            Trainee trainee = await _dataContext.Trainees.Where(t => t.TraineeId == traineeId).FirstOrDefaultAsync();
            IEnumerable<Calendar> calendars = await _dataContext.Calendars.Where(t => t.ClassId == trainee.ClassId && t.Date >= startDate && t.Date <= endDate )
            .Select(m => new Calendar{
                CalendarId = m.CalendarId,
                Date = m.Date,
                ClassId = m.ClassId,
                SlotInDay = m.SlotInDay,
                SyllabusSlot = m.SyllabusSlot,
                ModuleId = m.ModuleId,
                Module = m.Module,
                Class = m.Class
            }
            ).ToListAsync();
            return calendars;
        }
    }
}