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
        public async Task<Room> GetRoomByTraineeId(int traineeId){
            var classId = await _dataContext.Trainees.Where(e => e.TraineeId == traineeId).Select(e => e.ClassId).FirstOrDefaultAsync();
            var roomId = await _dataContext.Classes.Where(c => c.ClassId == classId).Select(r => r.RoomId).FirstOrDefaultAsync();
            var room = await _dataContext.Rooms.Where(c => c.RoomId == roomId).FirstOrDefaultAsync();
            return room;
        }

        
    }
}