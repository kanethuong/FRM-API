using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
<<<<<<< HEAD
    public class CalendarService : ICalendarService
=======
    public class CalendarService:ICalendarService
>>>>>>> a5a429610f5b3f4dfb9809115eb027d031c68f90
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