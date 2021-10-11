using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using Microsoft.EntityFrameworkCore;

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
        /// <returns>null:fail / Trainee data</returns>
        public async Task<Trainee> GetTraineeById(int id)
        {
            var result = await _dataContext.Trainees.Where(t => t.TraineeId == id).FirstOrDefaultAsync();
            if(result.IsDeactivated == true)
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// Get trainee method by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>null:fail / Trainee data</returns>
        public async Task<Trainee> GetTraineeByUsername(string username)
        {
            var result =  await _dataContext.Trainees.Where(t => t.Username == username).FirstOrDefaultAsync();
            if(result.IsDeactivated == true)
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// Get trainee method by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>null:fail / Trainee data</returns>
        public async Task<Trainee> GetTraineeByEmail(string email)
        {
            var result = await _dataContext.Trainees.Where(t => t.Email == email).FirstOrDefaultAsync();
            if(result.IsDeactivated == true)
            {
                return null;
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
            if(_dataContext.Trainees.Any(t => 
                t.TraineeId == trainee.TraineeId &&
                t.Username == trainee.Username &&
                t.Email == trainee.Email
            ))
            {
                return -1;
            }
            int rowInserted = 0;

            _dataContext.Trainees.Add(trainee);
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
            var existedTrainee = await _dataContext.Trainees.Where(t => t.TraineeId == id).FirstOrDefaultAsync();
            if(existedTrainee == null)
            {
                return -1;
            }
            existedTrainee.Fullname = trainee.Fullname;
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
            var existedTrainee = await _dataContext.Trainees.Where(t => t.TraineeId == id).FirstOrDefaultAsync();
            if(existedTrainee == null)
            {
                return -1;
            }
            existedTrainee.IsDeactivated = true;
            var rowDeleted = await _dataContext.SaveChangesAsync();

            return rowDeleted;
        }
    }
}