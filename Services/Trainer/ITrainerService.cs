using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;

namespace kroniiapi.Services
{
    public interface ITrainerService
    {
        Task<Trainer> GetTrainerById(int id);
        Task<Trainer> GetTrainerByUsername(string username);
        Task<Trainer> GetTrainerByEmail(string email);
        Task<int> InsertNewTrainer(Trainer trainer);
        bool InsertNewTrainerNoSaveChange(Trainer trainer);
        Task<int> UpdateTrainer(int id, Trainer trainer);
        Task<int> DeleteTrainer(int id);
        Task<Tuple<int, IEnumerable<Trainer>>> GetAllTrainer(PaginationParameter paginationParameter);
        bool CheckTrainerExist(int id);
        Task<Trainer> GetTrainerByCalendarId(int id);
    }
}