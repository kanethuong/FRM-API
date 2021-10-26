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

        
    }
}