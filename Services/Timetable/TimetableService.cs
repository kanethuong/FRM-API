using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
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
        private bool DayOffCheck (DateTime date)
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
        /// Get Number of All Modules Slot
        /// </summary>
        /// <param name="modulesList"></param>
        /// <returns></returns>
        public int CalculateSlotForWeek(int moduleSlot, DateTime startDay, DateTime endDay)
        {
            int availableDays = (endDay - startDay).Days;
            int availableWeeks = availableDays / 7;
            int slot4week =  moduleSlot / availableWeeks ;
            return slot4week;
        }
        /// <summary>
        /// Check The Weekend
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private bool WeekendCheck(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Check if Day is valid or not
        /// </summary>
        /// <param name="calendar"></param>
        /// <returns>true or false</returns>
        private bool DayCheck(Calendar calendar,  int roomId)
        {
            if (_datacontext.Calendars.Any(c => c.Date == calendar.Date)
             && _datacontext.Calendars.Any(c => c.SlotInDay == calendar.SlotInDay)
             && _datacontext.Calendars.Any(c => c.Class.RoomId == roomId)
             )
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Check Trainer with the time
        /// </summary>
        /// <param name="calendar"></param>
        /// <param name="trainerId"></param>
        /// <returns></returns>
        private bool TrainerCheck(Calendar calendar, int trainerId)
        {
            if (_datacontext.Calendars.Any(c => c.Date == calendar.Date)
             && _datacontext.Calendars.Any(c => c.SlotInDay == calendar.SlotInDay)
             && _datacontext.Calendars.Any(c => c.Class.TrainerId == trainerId))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Get Number of Slot in that day
        /// </summary>
        /// <param name="date"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        private int GetDaySlot(DateTime date, int classId)
        {
            var daySlot = _datacontext.Calendars.Where(c => c.Date.Day == date.Day && c.Date.Month == date.Month && c.Date.Year == date.Year  && c.ClassId == classId).ToList();
            int slotsDay = daySlot.Count();
            return slotsDay;
        }
        /// <summary>
        /// Check Available Day for Module
        /// </summary>
        /// <param name="modulesList"></param>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <returns></returns>
        public bool CheckAvailableModule(ICollection<Module> modulesList, DateTime startDay, DateTime endDay)
        {
            int record = 0;
            foreach (var item in modulesList)
            {
                record += item.NoOfSlot;
            }
            int availableDays = TimetableHelper.BusinessDaysUntil(startDay,endDay,holidayss);
            if (record > availableDays * 3)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Nhu cai ten
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public async Task<ICollection<Module>> GetModuleListlByClassId(int classId)
        {
            var moduleList = await _datacontext.Classes.Where(m => m.ClassId == classId).Select(c => c.Modules).FirstOrDefaultAsync();
            return moduleList;
        }
        /// <summary>
        /// Check available slots for the room in the among of time
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public bool CheckAvailabeSlotsForRoom(int slotsNeed, int roomId, DateTime startDay, DateTime endDay)
        {
            DateTime dateCount = startDay;
            int slotsAvai = 0;
            while (dateCount < endDay)
            {
                var count = _datacontext.Calendars.Where(c => c.Date.Day == dateCount.Day && c.Date.Month == dateCount.Month && c.Date.Year == dateCount.Year && c.Class.RoomId == roomId).Count();
                if (count <= 3)
                {
                    slotsAvai += 3;
                }
                else
                {
                    slotsAvai += 6 - count;
                }
                dateCount = dateCount.AddDays(1);
            }
            if (slotsAvai < slotsNeed)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Check available slots for the trainer in the among of time
        /// </summary>
        /// <param name="slotsNeed"></param>
        /// <param name="trainerId"></param>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <returns></returns>
        public bool CheckAvailabeSlotsForTrainer(int slotsNeed, int trainerId, DateTime startDay, DateTime endDay)
        {
            DateTime dateCount = startDay;
            int slotsAvai = 0;
            while (dateCount < endDay)
            {
                var count = _datacontext.Calendars.Where(c => c.Date.Day == dateCount.Day && c.Date.Month == dateCount.Month && c.Date.Year == dateCount.Year && c.Class.TrainerId == trainerId).Count();
                if (count <= 3)
                {
                    slotsAvai += 3;
                }
                else
                {
                    slotsAvai += 6 - count;
                }
                dateCount = dateCount.AddDays(1);
            }
            if (slotsAvai < slotsNeed)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Get Other Rooms For Class
        /// </summary>
        /// <param name="slotsNeed"></param>
        /// <param name="roomId"></param>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <returns>Room Name List</returns>
        public async Task<IEnumerable<string>> GetOtherRoomsForClass(int slotsNeed,int roomId, DateTime startDay, DateTime endDay)
        {
            var roomList = await _datacontext.Rooms.Where(r => r.RoomId != roomId).ToListAsync();
            List<String> roomName = new List<string>();
            foreach (var item in roomList)
            {
                if (CheckAvailabeSlotsForRoom(slotsNeed,roomId,startDay,endDay))
                {
                    roomName.Add(item.RoomName);
                }
            }
            return roomName;
        }
        /// <summary>
        /// Get Total Slots in ModulesList
        /// </summary>
        /// <param name="modulesList"></param>
        /// <returns></returns>
        public int GetTotalSlotsNeed(ICollection<Module> modulesList)
        {
            int totalRecord = 0;
            foreach (var item in modulesList)
            {
                totalRecord += item.NoOfSlot;
            }
            return totalRecord;
        }
        /// <summary>
        /// Insert Module To Class
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="classId"></param>
        /// <param name="numSlotWeek"></param>
        /// <returns>status</returns>
        public async Task<int> InsertModuleToClass(int moduleId, int classId, int numSlotWeek)
        {
            var module = await _moduleService.GetModuleById(moduleId);
            var classGet = await _classService.GetClassByClassID(classId);
            int moduleSlot = module.NoOfSlot;
            int slotCount = 1;
            int slotInWeek = 0;
            DateTime dateCount = classGet.StartDay;
            while (slotCount <= moduleSlot)
            {
                var ts = new TimeSpan(7, 0, 0);
                var slotTime = new TimeSpan(1, 30, 0);
                var breaktime = new TimeSpan(0, 15, 0);
                while (DayOffCheck(dateCount))
                {
                    if (WeekendCheck(dateCount))
                    {
                        slotInWeek = 0;
                    }
                    dateCount = dateCount.AddDays(1);
                }
                int slotModuleInDay = 0;
                int slotInDay = GetDaySlot(dateCount,classId); 
                for (var i = 1; i <= 6; i++)
                {
                    if (i == 4)
                    {
                        breaktime = new TimeSpan(0, 45, 0);
                    }
                    else
                    {
                        breaktime = new TimeSpan(0, 15, 0);
                    }
                    Calendar calendarToAdd = new Calendar
                    {
                        SyllabusSlot = slotCount,
                        Date = dateCount + ts + (i -1) * slotTime + (i-1) * breaktime,
                        SlotInDay = i,
                        ModuleId = module.ModuleId,
                        Module = module,
                        ClassId = classGet.ClassId,
                        Class = classGet,
                    };
                    if (slotCount > moduleSlot)
                    {
                        break;
                    }
                    if (slotInDay == 3)
                    {
                        break;
                    }
                    if (slotModuleInDay == 2)
                    {
                        if (slotInWeek == numSlotWeek)
                        {
                            slotInWeek = 0;
                            dateCount = TimetableHelper.NextMonday(dateCount); 
                        }
                        break;
                    }
                    if (slotInWeek == numSlotWeek)
                    {
                        slotInWeek = 0;
                        dateCount = TimetableHelper.NextMonday(dateCount);
                        break;
                    }
                    if (DayCheck(calendarToAdd, classGet.RoomId) && TrainerCheck(calendarToAdd,classGet.TrainerId))
                    {
                        slotCount += 1;
                        slotModuleInDay += 1;
                        slotInDay += 1;
                        slotInWeek += 1;
                        _datacontext.Calendars.Add(calendarToAdd);
                    };
                }
                dateCount = dateCount.AddDays(1);
            }
            if (dateCount.Day > classGet.EndDay.Day && dateCount.Month > classGet.EndDay.Month && dateCount.Year > classGet.EndDay.Year)
            {
                return -1;
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
            var moduleList = await GetModuleListlByClassId(classId);
            var classGet = await _classService.GetClassByClassID(classId);
            if (CheckAvailableModule(moduleList,classGet.StartDay,classGet.EndDay))
            {
                return (-1, "Too much Module");
            }
            int slotsNeed = GetTotalSlotsNeed(moduleList);
            if (!CheckAvailabeSlotsForRoom(slotsNeed,classGet.RoomId,classGet.StartDay,classGet.EndDay))
            {
                var otherRooms = await GetOtherRoomsForClass(slotsNeed, classGet.RoomId, classGet.StartDay, classGet.EndDay);
                string message = "Room is already full of slot, Recommend:";
                foreach (var item in otherRooms)
                {
                    message += " "  + item;
                }
                return (-1, message);
            }
            if (!CheckAvailabeSlotsForTrainer(slotsNeed,classGet.TrainerId,classGet.StartDay,classGet.EndDay))
            {
                return (-1,"This trainer is too bussy at that moment");
            }
            foreach (var item in moduleList)
            {
                var slotForWeek = CalculateSlotForWeek(item.NoOfSlot, classGet.StartDay, classGet.EndDay);
                var slotss = item.NoOfSlot;
                var status = await InsertModuleToClass(item.ModuleId, classId, slotForWeek);
                if (status == -1)
                {
                    return (0, "Failed To Insert");
                }
                var idList = await _calendarService.GetCalendarsIdListByModuleAndClassId(item.ModuleId, classId);
                foreach (var id in idList)
                {
                    await _attendaceService.AddNewAttendance(id, classGet.Trainees);
                }
            }
            return (1, "Succes");            
        }
    }
}