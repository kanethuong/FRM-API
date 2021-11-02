using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public interface ITimetableService
    {
        Task<int> InsertModuleToClass(int moduleId, int classId,  int numSlotWeek);
        bool CheckAvailableModule(ICollection<Module> modulesList, DateTime startDay, DateTime endDay);
        Task<ICollection<Module>> GetModuleListlByClassId(int classId);
        int CalculateSlotForWeek(int moduleSlot, DateTime startDay, DateTime endDay);
        bool CheckAvailabeSlotsForRoom(int slotsNeed, int roomId, DateTime startDay, DateTime endDay);
        Task<IEnumerable<string>> GetOtherRoomsForClass(int slotsNeed, int roomId, DateTime startDay, DateTime endDay);
        int GetTotalSlotsNeed(ICollection<Module> modulesList);
        bool CheckAvailabeSlotsForTrainer(int slotsNeed, int trainerId, DateTime startDay, DateTime endDay);
        Task<(int, string )> GenerateTimetable(int classId);
    }
}