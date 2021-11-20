using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public interface IRoomService
    {
        Task<Room> GetRoomById(int id);
        Task<Room> GetRoom(int classId,int moduleId);
        Task<List<Room>> GetRoomByClassId(int classId);
    }
}