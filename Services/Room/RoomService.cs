using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class RoomService : IRoomService
    {
        private DataContext _dataContext;

        public RoomService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Room> GetRoomById(int id)
        {
            return await _dataContext.Rooms.Where(r => r.RoomId == id).FirstOrDefaultAsync();
        }
        public async Task<Room> GetRoom(int classId,int moduleId){
            var room = await _dataContext.ClassModules.Where(e => e.ClassId == classId && e.ModuleId == moduleId ).Select(r => r.Room).FirstOrDefaultAsync();
            return room;
        }
        public async Task<List<Room>> GetRoomByClassId(int classId){
            var rooms = await _dataContext.ClassModules.Where(e => e.ClassId == classId).Select(r => r.Room).ToListAsync();
            rooms = rooms.Distinct().ToList();
            rooms.RemoveAll(x => x == null);
            return rooms;
        }
        /// <summary>
        /// Get Room ID to Create new Exam
        /// </summary>
        /// <param name="examDay"></param>
        /// <param name="traineesList"></param>
        /// <returns></returns>
        public (int,string) GetRoomIdToCreateNewExam(DateTime examDay, IEnumerable<Trainee> traineesList)
        {
            string allThename = " ";
            int traineeCount = 0;
            foreach (var item in traineesList)
            {
                if (_dataContext.Calendars.Any(cl => cl.Class.Trainees.Contains(item)))
                {
                    allThename += item.Fullname + " ";
                    traineeCount += 0;
                }
            }
            if (traineeCount != 0)
            {
                return (-1, "Trainee: " + allThename + " already have thing to do that day");
            }
            for (var i = 0; i < 10; i++)
            {
                if ((!_dataContext.Calendars.Any(cl => cl.Class.ClassModules.Any(cm => cm.RoomId == i) && cl.Date.Year == examDay.Year && cl.Date.Month == examDay.Month && cl.Date.Day == examDay.Day))
                    
                )
                {
                    return (i, "This is the room u need");
                }
            }
            return (0, "There are no more room Available on Day:" + examDay.Day +"/"+examDay.Month+"/"+examDay.Year );
        }

        
    }
}