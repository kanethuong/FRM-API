using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public class TrainerService : ITrainerService
    {
        private DataContext _dataContext;

        public TrainerService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Trainer> GetTrainerById(int id)
        {
            return null;
        }

        public async Task<Trainer> GetTrainerByUsername(string username)
        {
            return null;
        }

        public async Task<Trainer> GetTrainerByEmail(string email)
        {
            return null;
        }

        public async Task<int> InsertNewTrainer(Trainer trainer)
        {
            return 0;
        }

        public async Task<int> UpdateTrainer(int id, Trainer trainer)
        {
            return 0;
        }

        public async Task<int> DeleteTrainer(int id)
        {
            return 0;
        }
    }
}