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
        Task<Room> GetRoomByTraineeId(int traineeId);
    }
}