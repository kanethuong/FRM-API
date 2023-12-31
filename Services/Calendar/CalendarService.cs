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

        public async Task<List<Calendar>> GetCalendarsByTraineeId(int traineeId, DateTime startDate, DateTime endDate)
        {
            Trainee trainee = await _dataContext.Trainees.Where(t => t.TraineeId == traineeId).FirstOrDefaultAsync();
            List<Calendar> calendars = await _dataContext.Calendars.Where(t => t.ClassId == trainee.ClassId && t.Date >= startDate && t.Date <= endDate)
            .Select(m => new Calendar
            {
                CalendarId = m.CalendarId,
                Date = m.Date,
                ClassId = m.ClassId,
                // SlotInDay = m.SlotInDay,
                SyllabusSlot = m.SyllabusSlot,
                ModuleId = m.ModuleId,
                Module = m.Module,
                Class = m.Class
            }
            ).OrderBy(c => c.Date).ToListAsync();
            return calendars;
        }
        public async Task<string> GetRoomNameByCalendarId(int calendarId)
        {
            var calendarRoom = await _dataContext.Calendars
                                .Where(c => c.CalendarId == calendarId)
                                .Select(c => new Calendar
                                {
                                    CalendarId = c.CalendarId,
                                    SyllabusSlot = c.SyllabusSlot,
                                    // SlotInDay = c.SlotInDay,
                                    Class = new Class
                                    {
                                        ClassId = c.ClassId,
                                        ClassName = c.Class.ClassName,
                                        // Room = new Room {
                                        //     RoomId = c.Class.RoomId,
                                        //     RoomName = c.Class.Room.RoomName,
                                        // }
                                    }
                                })
                                .FirstOrDefaultAsync();
            // return calendarRoom.Class.Room.RoomName;
            return null;
        }
        /// <summary>
        /// Get Calendars Id List using Module Id and Class Id
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        public async Task<List<int>> GetCalendarsIdListByModuleAndClassId(int moduleId, int classId)
        {
            var idList = await _dataContext.Calendars.Where(c => c.ModuleId == moduleId && c.ClassId == classId).Select(c => c.CalendarId).ToListAsync();
            return idList;
        }
        public async Task<IEnumerable<Calendar>> GetCalendarsByTrainerId(int trainerId, DateTime startDate, DateTime endDate)
        {
            var classesModules = await _dataContext.ClassModules.Where(c => c.TrainerId == trainerId).ToListAsync();
            //Add and check class if deactivated
            var classes = new List<Class>();
            foreach (var item in classesModules)
            {
                classes.Add(await _dataContext.Classes.Where(c => c.ClassId == item.ClassId && c.IsDeactivated == false).FirstOrDefaultAsync());
            }
            classes.RemoveAll(i => i == null);
            //Delete duplicated class
            classes = classes.Distinct().ToList();
            List<ClassModule> classesModulesLegit = new List<ClassModule>();
            foreach (var item in classes)
            {
                classesModulesLegit.AddRange(classesModules.Where(t => t.ClassId == item.ClassId).ToList());
            }
            List<Calendar> calendars = new List<Calendar>();

            foreach (var item in classesModulesLegit)
            {
                calendars.AddRange(await _dataContext.Calendars.Where(t => t.ClassId == item.ClassId && t.ModuleId == item.ModuleId && t.Date >= startDate && t.Date <= endDate)
           .Select(m => new Calendar
           {
               CalendarId = m.CalendarId,
               Date = m.Date,
               ClassId = m.ClassId,
               SyllabusSlot = m.SyllabusSlot,
               ModuleId = m.ModuleId,
               Module = new Module
               {
                   ModuleName = m.Module.ModuleName,
                   ModuleId = m.ModuleId,
                   SlotDuration = m.Module.SlotDuration
               },
               Class = new Class
               {
                   ClassName = m.Class.ClassName
               }
           }
           ).OrderBy(c => c.Date).ToListAsync());
            }
            //delete null calendar
            calendars.RemoveAll(t => t == null);
            return calendars;
        }
    }
}