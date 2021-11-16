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
        int GetRoomIdAvailableForModule(int classId , int moduleId);
        (bool, int) CheckAvailableForClass(DateTime startDay, DateTime endDay, int classId);
        bool DayLeftAvailableCheck(int moduleId, int classId);
        bool CheckTrainerAvailableForModule(int classId, int trainerId, int moduleId);
        Task<int> InsertCalendarsToClass( int classId, int moduleId);
        bool DayOffCheck(DateTime date);
    }   
}