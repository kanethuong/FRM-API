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
        /// <summary>
        /// Get Trainer method by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Trainer data</returns>
        public async Task<Trainer> GetTrainerById(int id)
        {
            result = await _dataContext.Trainer.Where(t => t.TrainerID == id).FirstOrDefaultAsync();
            if(result.IsDeactivated == "true"))
            {
                return 0;
            }
            return result;
        }

        /// <summary>
        /// Get Trainer method by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Trainer data</returns>
        public async Task<Trainer> GetTrainerByUsername(string username)
        {
            result =  await _dataContext.Trainer.Where(t => t.Username == username).FirstOrDefaultAsync();
            if(result.IsDeactivated == "true"))
            {
                return 0;
            }
            return result;
        }

        /// <summary>
        /// Get Trainer method by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Trainer data</returns>
        public async Task<Trainer> GetTrainerByEmail(string email)
        {
            result await _dataContext.Trainer.Where(t => t.Email == email).FirstOrDefaultAsync();
            if(result.IsDeactivated == "true")
            {
                return 0;
            }
            return result;
        }

        /// <summary>
        /// Insert new trainer method
        /// </summary>
        /// <param name="trainer"></param>
        /// <returns>-1:existed / 0:fail / 1:success</returns>
        public async Task<int> InsertNewTrainer(Trainer trainer)
        {
            if(_dataContext.Trainer.Any(t => 
                t.TrainerID == trainer.TrainerID &&
                t.Username == trainer.Username &&
                t.Email == trainer.Email
            ))
            {
                return -1;
            }
            int rowInserted = 0;

            _dataContext.Trainer.Add(trainer);
            rowInserted = await _dataContext.SaveChangesAsync();

            return rowInserted;
            
        }

        /// <summary>
        /// Update trainer method
        /// </summary>
        /// <param name="id"></param>
        /// <param name="trainer"></param>
        /// <returns>-1:not existed / 0:fail / 1:success</returns>
        public async Task<int> UpdateTrainer(int id, Trainer trainer)
        {

            var existedTrainer = await _dataContext.Trainer.Where(t => t.TrainerID == id).FirstOrDefaultAsync();
            if(existedTrainer == null)
            {
                return -1;
            }
            existedTrainer.Email = trainer.Email;
            if(_dataContext.Trainer.Any(t =>
                t.TrainerID != trainer.ID &&
                t.Email == trainer.Email
            ))
            {
                return 0;
            }
            existedTrainer.FullName = trainer.FullName;
            existedTrainer.Phone = trainer.Phone;
            existedTrainer.DOB = trainer.DOB;
            existedTrainer.Address = trainer.Address;
            existedTrainer.Gender = trainer.Gender;

            var rowUpdated = await _dataContext.SaveChangesAsync();

            return rowUpdated;
        }

        /// <summary>
        /// Delete Trainer method
        /// </summary>
        /// <param name="id"></param>
        /// <returns>-1:not existed / 0:fail / 1:success</returns>
        public async Task<int> DeleteTrainer(int id)
        {
            var existedTrainer = await _dataContext.Trainer.Where(t => t.TrainerID == id).FirstOrDefaultAsync();
            if(existedTrainer == null)
            {
                return -1;
            }
            existedTrainer.isDeactivated = "true";
            var rowDeleted = await _dataContext.SaveChangesAsync();

            return rowDeleted;
        }
    }
}