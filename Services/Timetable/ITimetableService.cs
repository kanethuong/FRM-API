using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public interface ITimetableService
    {
        Task<(int, string)> GenerateTimetable(int classId);
        int GetRoomIdAvailableForModule(DateTime startDay, DateTime endDay, int slotNeed);
        int CheckTrainerAvailableForModule(DateTime startDay, DateTime endDay, int trainerId, int daysNeed);
        DateTime GetStartDayforClassToInsertModule(int classId);
        Task<int> InsertModuleToClass(int classId, int moduleId, int noOfSlot);
    }   
}