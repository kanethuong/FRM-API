using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ApplicationDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TraineeDTO;

namespace kroniiapi.Services
{
    public interface ITraineeService
    {
        Task<Trainee> GetTraineeById(int id);
        Task<Trainee> GetTraineeByUsername(string username);
        Task<Trainee> GetTraineeByEmail(string email);
        Task<int> InsertNewTrainee(Trainee trainee);
        bool InsertNewTraineeNoSaveChange(Trainee trainee);
        Task<int> UpdateTrainee(int id, Trainee trainee);
        Task<int> UpdateAvatar(int id, string avatarUrl);
        Task<int> DeleteTrainee(int id);
        Task<Tuple<int, IEnumerable<Trainee>>> GetTraineeListByClassId(int id, PaginationParameter paginationParameter);
        Task<ICollection<Trainee>> GetTraineeByClassId(int id);
        Task<bool> IsTraineeHasClass(int traineeId);
        Task<Tuple<int, IEnumerable<TraineeApplicationResponse>>> GetApplicationListByTraineeId(int id, PaginationParameter paginationParameter);
        Task<(int, string)> GetClassIdByTraineeId(int id);
        Task<Tuple<int, IEnumerable<TraineeMarkAndSkill>>> GetMarkAndSkillByTraineeId(int id, PaginationParameter paginationParameter);
        Task<Tuple<int, IEnumerable<Trainee>>> GetAllTraineeWithoutClass(PaginationParameter paginationParameter);
        bool CheckTraineeExist(int id);
        Task<Tuple<int, IEnumerable<Trainee>>> GetAllTrainee(PaginationParameter paginationParameter);
    }
}