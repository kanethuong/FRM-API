using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ClassDTO;

namespace kroniiapi.Services
{
    public interface ITimetableService
    {
        Task<(int, string)> GenerateTimetable(int classId);
        (int, DateTime) GetRoomIdAvailableForModule(DateTime startDay, DateTime endDay, int moduleId);
        DateTime GetStartDayforClassToInsertModule(int classId);
        (bool, int) CheckAvailableForClass(DateTime startDay, DateTime endDay, int classId);
        (bool, string) CheckRoomsNewClass(List<int> moduleIds, DateTime startDay, DateTime endDay);
        (bool, string) CheckTrainersNewClass(List<TrainerModule> trainerModules, DateTime startDay, DateTime endDay);
        bool DayLeftAvailableCheck(int moduleId, int classId);
        (bool, DateTime) CheckTrainerAvailableForModule(DateTime startDay, DateTime endDay, int trainerId, int moduleId);
        Task<int> InsertCalendarsToClass( int classId, int moduleId);
        bool DayOffCheck(DateTime date);
    }   
}