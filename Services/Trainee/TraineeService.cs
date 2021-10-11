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

        /// <summary>
        /// Get trainee method by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>0:fail / Trainee data</returns>
        public async Task<Trainee> GetTraineeById(int id)
        {
            result = await _dataContext.Trainee.Where(t => t.TraineeID == id).FirstOrDefaultAsync();
            if(result.IsDeactivated == "true"))
            {
                return 0;
            }
            return result;
        }

        /// <summary>
        /// Get trainee method by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>0:fail / Trainee data</returns>
        public async Task<Trainee> GetTraineeByUsername(string username)
        {
            result =  await _dataContext.Trainee.Where(t => t.Username == username).FirstOrDefaultAsync();
            if(result.IsDeactivated == "true"))
            {
                return 0;
            }
            return result;
        }

        /// <summary>
        /// Get trainee method by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>0:fail / Trainee data</returns>
        public async Task<Trainee> GetTraineeByEmail(string email)
        {
            result await _dataContext.Trainee.Where(t => t.Email == email).FirstOrDefaultAsync();
            if(result.IsDeactivated == "true")
            {
                return 0;
            }
            return result;
        }


        /// <summary>
        /// Insert new trainee method
        /// </summary>
        /// <param name="trainee"></param>
        /// <returns>-1:existed / 0:fail / 1:success</returns>
        public async Task<int> InsertNewTrainee(Trainee trainee)
        {
            if(_dataContext.Trainee.Any(t => 
                t.TraineeID == trainee.TraineeID &&
                t.Username == trainee.Username &&
                t.Email == trainee.Email
            ))
            {
                return -1;
            }
            int rowInserted = 0;

            _dataContext.Trainee.Add(trainee);
            rowInserted = await _dataContext.SaveChangesAsync();

            return rowInserted;
        }

        /// <summary>
        /// Update trainee method
        /// </summary>
        /// <param name="id"></param>
        /// <param name="trainee"></param>
        /// <returns>-1:not existed / 0:fail / 1:success</returns>
        public async Task<int> UpdateTrainee(int id, Trainee trainee)
        {
            var existedTrainee = await _dataContext.Trainee.Where(t => t.TraineeID == id).FirstOrDefaultAsync();
            if(existedTrainee == null)
            {
                return -1;
            }
            existedTrainee.FullName = trainee.FullName;
            existedTrainee.Phone = trainee.Phone;
            existedTrainee.DOB = trainee.DOB;
            existedTrainee.Address = trainee.Address;
            existedTrainee.Gender = trainee.Gender;

            var rowUpdated = await _dataContext.SaveChangesAsync();

            return rowUpdated;
        }

        /// <summary>
        /// Delete trainee method
        /// </summary>
        /// <param name="id"></param>
        /// <returns>-1:not existed / 0:fail / 1:success</returns>
        public async Task<int> DeleteTrainee(int id)
        {
            var existedTrainee = await _dataContext.Trainee.Where(t => t.TraineeID == id).FirstOrDefaultAsync();
            if(existedTrainee == null)
            {
                return -1;
            }
            existedTrainee.IsDeactivated = "true";
            var rowDeleted = await _dataContext.SaveChangesAsync();

            return rowDeleted;
        }
    }
}