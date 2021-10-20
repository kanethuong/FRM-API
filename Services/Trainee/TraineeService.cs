using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;
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
        /// <returns>Trainee data</returns>
        public async Task<Trainee> GetTraineeById(int id)
        {
            return await _dataContext.Trainees.Where(t => t.TraineeId == id &&
            t.IsDeactivated == false).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get trainee method by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Trainee data</returns>
        public async Task<Trainee> GetTraineeByUsername(string username)
        {
            return await _dataContext.Trainees.Where(t => t.Username == username &&
            t.IsDeactivated == false).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get trainee method by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Trainee data</returns>
        public async Task<Trainee> GetTraineeByEmail(string email)
        {
            return await _dataContext.Trainees.Where(t => t.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
            t.IsDeactivated == false).FirstOrDefaultAsync();
        }


        /// <summary>
        /// Insert new trainee method
        /// </summary>
        /// <param name="trainee"></param>
        /// <returns>-1:existed / 0:fail / 1:success</returns>
        public async Task<int> InsertNewTrainee(Trainee trainee)
        {
            if (_dataContext.Trainees.Any(t =>
                 t.TraineeId == trainee.TraineeId ||
                 t.Username == trainee.Username ||
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
            if (existedTrainee == null)
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
            var existedTrainee = await _dataContext.Trainees.Where(t => t.TraineeId == id && t.IsDeactivated == false).FirstOrDefaultAsync();
            if (existedTrainee == null)
            {
                return -1;
            }
            existedTrainee.IsDeactivated = true;
            existedTrainee.DeactivatedAt = DateTime.Now;
            var rowDeleted = await _dataContext.SaveChangesAsync();

            return rowDeleted;
        }

        /// <summary>
        /// Insert new (trainee) account to DbContext without save change to DB
        /// </summary>
        /// <param name="trainee">Trainee data</param>
        /// <returns>true: insert done / false: dupplicate data</returns>
        public bool InsertNewTraineeNoSaveChange(Trainee trainee)
        {
            if (_dataContext.Trainees.Any(t =>
                 t.TraineeId == trainee.TraineeId ||
                 t.Username == trainee.Username ||
                 t.Email == trainee.Email
            ))
            {
                return false;
            }

            _dataContext.Trainees.Add(trainee);            

            return true;
        }

        /// <summary>
        ///Get trainee list by class id with pagination
        /// </summary>
        /// <param name="id"></param>
        /// <param name="paginationParameter"></param>
        /// <returns>Tuple of trainee list</returns>
        public async Task<Tuple<int, IEnumerable<Trainee>>> GetTraineeListByClassId(int id, PaginationParameter paginationParameter)
        {
            var listTrainee = await _dataContext.Trainees.Where(t => t.ClassId == id && t.IsDeactivated == false && t.Username.ToUpper().Contains(paginationParameter.SearchName.ToUpper())).ToListAsync();

            int totalRecords = listTrainee.Count();

            var rs = listTrainee.OrderBy(c => c.ClassId)
                     .Skip((paginationParameter.PageNumber - 1) * paginationParameter.PageSize)
                     .Take(paginationParameter.PageSize);

            return Tuple.Create(totalRecords, rs);
        }

        /// <summary>
        /// Get Trainee by class id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of trainee</returns>
        public async Task<ICollection<Trainee>> GetTraineeByClassId(int id)
        {
            return await _dataContext.Trainees.Where(t => t.ClassId == id && t.IsDeactivated == false).ToListAsync();
        }
    }
}