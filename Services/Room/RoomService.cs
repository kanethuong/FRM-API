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
        public async Task<Room> GetRoomByTraineeId(int traineeId)
        {
             //var classId = await _dataContext.Trainees.Where(e => e.TraineeId == traineeId).Select(e => e.ClassId).FirstOrDefaultAsync();
             //var roomId = await _dataContext.Classes.Where(c => c.ClassId == classId).Select(r => r.RoomId).FirstOrDefaultAsync();
            // var room = await _dataContext.Rooms.Where(c => c.RoomId == roomId).FirstOrDefaultAsync();

            // return room;
            return null;
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


    }
}