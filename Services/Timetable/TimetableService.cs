using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.TimetableDTO;
using kroniiapi.Helper.Timetable;
using kroniiapi.Services.Attendance;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class TimetableService : ITimetableService
    {
        private DataContext _datacontext;
        private ICalendarService _calendarService;
        private IModuleService _moduleService;
        private IClassService _classService;
        private IAttendanceService _attendaceService;


        public TimetableService(DataContext dataContext, ICalendarService calendarService, IModuleService moduleService, IClassService classService, IAttendanceService attendanceService)
        {
            _datacontext = dataContext;
            _calendarService = calendarService;
            _moduleService = moduleService;
            _classService = classService;
            _attendaceService = attendanceService;
        }

        List<DateTime> holidayss = new List<DateTime> {
            // New Year
            new DateTime(1, 1, 1),
            //
            new DateTime(1, 4, 30),
            //
            new DateTime(1, 5, 1),
            //
            new DateTime(1, 9, 1),
            //
            new DateTime(1, 9, 2),
            //
            new DateTime(1, 9, 3),
            //
        };

        /// <summary>
        /// Check if Date is a Day Off
        /// </summary>
        /// <param name="date"></param>
        /// <returns>true or false</returns>
        public bool DayOffCheck(DateTime date)
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
            return false;
        }
        /// <summary>
        /// Get Total Day need for all Module in Class
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        private int GetTotalDaysNeed(int classId)
        {
            ICollection<Module> moduleList = _datacontext.Classes.Where(c => c.ClassId == classId).Select(c => c.Modules).FirstOrDefault();
            int totalDay = 0;
            foreach (var item in moduleList)
            {
                totalDay += item.NoOfSlot;
            }
            return totalDay;
        }
        /// <summary>
        /// Check Available for Class
        /// </summary>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        public (bool, int) CheckAvailableForClass(DateTime startDay, DateTime endDay, int classId)
        {
            int daysAvailable = TimetableHelper.BusinessDaysUntil(startDay, endDay, holidayss);
            int daysNeed = GetTotalDaysNeed(classId);
            if (daysAvailable >= daysNeed)
            {
                return (true, 0);
            }
            return (false, daysNeed - daysAvailable);
        }
        /// <summary>
        /// Get Room Available
        /// </summary>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <param name="slotNeed"></param>
        /// <returns></returns>
        public int GetRoomIdAvailableForModule(int classId , int moduleId)
        {
            var classGet = _datacontext.Classes.Where(cl => cl.ClassId == classId).FirstOrDefault();
            var startDay = GetStartDayforClassToInsertModule(classId);
            var slotNeed = _datacontext.Modules.Where(md => md.ModuleId == moduleId).Select(md => md.NoOfSlot).FirstOrDefault();
            DateTime lastDay = startDay;
            while (TimetableHelper.BusinessDaysUntil(startDay,lastDay,holidayss) < slotNeed)
            {
                lastDay = lastDay.AddDays(1);
                if (lastDay == classGet.EndDay)
                {
                    return 0;
                }
            }
            for (int i = 1; i <= 10; i++)
            {
                if(!_datacontext.Calendars.Any(c => c.Class.ClassModules.Any(cm => cm.RoomId == i) && c.Date >= startDay&& c.Date <= lastDay))
                {
                    return i;
                }
            }
            return 0;
        }
        /// <summary>
        /// Check Trainer Available 
        /// </summary>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <param name="trainerId"></param>
        /// <param name="daysNeed"></param>
        /// <returns></returns>
        public bool CheckTrainerAvailableForModule(int classId, int trainerId, int moduleId)
        {
            var classGet = _datacontext.Classes.Where(cl => cl.ClassId == classId).FirstOrDefault();
            var startDay = GetStartDayforClassToInsertModule(classId);
            var trainerDays = _datacontext.Calendars.Where(c => c.Class.ClassModules.Any(cm => cm.TrainerId == trainerId) && c.Date >= startDay && c.Date <= classGet.EndDay).Count() / 2;
            var daysNeed = _datacontext.Modules.Where(md => md.ModuleId == moduleId).Select(md => md.NoOfSlot).FirstOrDefault();
            if (TimetableHelper.BusinessDaysUntil(startDay,classGet.EndDay,holidayss) - trainerDays >= daysNeed)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Lay ngay cuoi cung cua cai lop dang hoc
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        private DateTime GetStartDayforClassToInsertModule(int classId)
        {
            if (!_datacontext.Calendars.Any(c => c.ClassId == classId))
            {
                return _datacontext.Classes.Where(c => c.ClassId == classId).Select(c => c.StartDay).FirstOrDefault();
            }
            DateTime startDay = _datacontext.Calendars.Where(cl => cl.ClassId == classId).Select(cl => cl.Date).OrderBy(cl => cl).LastOrDefault().AddDays(1);
            DateTime returnDay = new DateTime(startDay.Year, startDay.Month, startDay.Day, 0,0,0);
            while (DayOffCheck(returnDay))
            {
                returnDay = returnDay.AddDays(1);
            }
            return returnDay;
        }
        /// <summary>
        /// Check day left available
        /// </summary>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <returns></returns>
        public bool DayLeftAvailableCheck(int moduleId, int classId)
        {
            var classGet = _datacontext.Classes.Where(cl => cl.ClassId == classId).FirstOrDefault();
            var moduleGet = _datacontext.Modules.Where(cl => cl.ModuleId == moduleId).FirstOrDefault();
            var startDay = GetStartDayforClassToInsertModule(classId);
            int businessday = TimetableHelper.BusinessDaysUntil(startDay, classGet.EndDay, holidayss);
            if (moduleGet.NoOfSlot > businessday)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Insert Module To Class
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="classId"></param>
        /// <param name="numSlotWeek"></param>
        /// <returns>status 1: Success / 0: Not enough Day</returns>
        public async Task<int> InsertCalendarsToClass( int classId, int moduleId)
        {
            var classGet = await _classService.GetClassByClassID(classId);
            var noOfSlot = _datacontext.Modules.Where(md => md.ModuleId == moduleId).Select(md => md.NoOfSlot).FirstOrDefault();
            DateTime dateCount = GetStartDayforClassToInsertModule(classId);
            if (dateCount == new DateTime(1,1,1))
            {
                dateCount = classGet.StartDay;
            }
            int slotCount = 1;
            while (slotCount <= noOfSlot)
            {
                while (DayOffCheck(dateCount))
                {
                    dateCount =  dateCount.AddDays(1);
                }
                Calendar calendarMor = new Calendar
                {
                    SyllabusSlot = slotCount,
                    Date = dateCount + new TimeSpan(8, 0, 0),
                    ModuleId = moduleId,
                    ClassId = classId,
                };
                _datacontext.Calendars.Add(calendarMor);
                Calendar calendarAft = new Calendar
                {
                    SyllabusSlot = slotCount,
                    Date = dateCount + new TimeSpan(13, 0, 0),
                    ModuleId = moduleId,
                    ClassId = classId,
                };
                _datacontext.Calendars.Add(calendarAft);
                slotCount += 1;
                dateCount = dateCount.AddDays(1);
            }
            await _datacontext.SaveChangesAsync();
            return 1;
        }
        /// <summary>
        /// Generate Timetable
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public async Task<(int, string)> GenerateTimetable(int classId)
        {
            var classGet = _datacontext.Classes.Where(cl => cl.ClassId == classId).FirstOrDefault();
            var listModuleClass = _datacontext.ClassModules.Where(cm => cm.ClassId == classId).Select(cl => cl.Module).ToList();
            var (check, message) = CheckAvailableForClass(GetStartDayforClassToInsertModule(classId), classGet.EndDay, classId);
            if (!check)
            {
                return (0, "Missing: " + message + " Day ");
            }
            foreach (var item in listModuleClass)
            {
                int noOfSlot = _datacontext.Modules.Where(m => m.ModuleId == item.ModuleId).Select(m => m.NoOfSlot).FirstOrDefault();
                int status = await InsertCalendarsToClass(classId, item.ModuleId);
            }
            return (1, "Succes");
        }
    }
}