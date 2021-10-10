using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public class TraineeService : ITraineeService
    {
        private DataContext _dataContext;

        public TraineeService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Trainee> GetTraineeById(int id)
        {
            return null;
        }

        public async Task<Trainee> GetTraineeByUsername(string username)
        {
            return null;
        }

        public async Task<Trainee> GetTraineeByEmail(string email)
        {
            return null;
        }

        public async Task<int> InsertNewTrainee(Trainee trainee)
        {
            return 0;
        }

        public async Task<int> UpdateTrainee(int id, Trainee trainee)
        {
            return 0;
        }

        public async Task<int> DeleteTrainee(int id)
        {
            return 0;
        }
    }
}