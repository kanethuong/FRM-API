using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Helper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace kroniiapi.Services
{
    public class TrainerService : ITrainerService
    {
        private DataContext _dataContext;

        public TrainerService(DataContext dataContext, IClassService classService)
        {
            _dataContext = dataContext;
            _classService = classService;
        }
        private IClassService _classService;
        /// <summary>
        /// Get Trainer method by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>null:fail / Trainer data</returns>
        public async Task<Trainer> GetTrainerById(int id)
        {
            return await _dataContext.Trainers.Where(t => t.TrainerId == id &&
            t.IsDeactivated == false).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get Trainer method by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Trainer data</returns>
        public async Task<Trainer> GetTrainerByUsername(string username)
        {
            return await _dataContext.Trainers.Where(t => t.Username == username &&
            t.IsDeactivated == false).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get Trainer method by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Trainer data</returns>
        public async Task<Trainer> GetTrainerByEmail(string email)
        {
            return await _dataContext.Trainers.Where(t => t.Email.ToLower().Equals(email.ToLower()) &&
            t.IsDeactivated == false).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Insert new trainer method
        /// </summary>
        /// <param name="trainer"></param>
        /// <returns>-1:existed / 0:fail / 1:success</returns>
        public async Task<int> InsertNewTrainer(Trainer trainer)
        {
            if (_dataContext.Trainers.Any(t =>
                 t.TrainerId == trainer.TrainerId ||
                 t.Username == trainer.Username ||
                 t.Email == trainer.Email
            ))
            {
                return -1;
            }
            int rowInserted = 0;

            _dataContext.Trainers.Add(trainer);
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

            var existedTrainer = await _dataContext.Trainers.Where(t => t.TrainerId == id).FirstOrDefaultAsync();
            if (existedTrainer == null)
            {
                return -1;
            }
            existedTrainer.Fullname = trainer.Fullname;
            existedTrainer.Phone = trainer.Phone;
            existedTrainer.DOB = trainer.DOB;
            existedTrainer.Address = trainer.Address;
            existedTrainer.Gender = trainer.Gender;
            existedTrainer.Facebook = trainer.Facebook;
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
            var existedTrainer = await _dataContext.Trainers.Where(t => t.TrainerId == id && t.IsDeactivated == false).FirstOrDefaultAsync();
            if (existedTrainer == null)
            {
                return -1;
            }
            existedTrainer.IsDeactivated = true;
            existedTrainer.DeactivatedAt = DateTime.Now;
            var rowDeleted = await _dataContext.SaveChangesAsync();

            return rowDeleted;
        }

        /// <summary>
        /// Insert new (trainer) account to DbContext without save change to DB
        /// </summary>
        /// <param name="trainer">Trainer data</param>
        /// <returns>true: insert done / false: dupplicate data</returns>
        public bool InsertNewTrainerNoSaveChange(Trainer trainer)
        {
            if (_dataContext.Trainers.Any(t =>
                 t.TrainerId == trainer.TrainerId ||
                 t.Username == trainer.Username ||
                 t.Email == trainer.Email
            ))
            {
                return false;
            }

            _dataContext.Trainers.Add(trainer);

            return true;
        }
        /// <summary>
        /// Get all trainer with pagination
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        public async Task<Tuple<int, IEnumerable<Trainer>>> GetAllTrainer(PaginationParameter paginationParameter)
        {
            IQueryable<Trainer> trainers = _dataContext.Trainers.Where(t => t.IsDeactivated == false);
            if (paginationParameter.SearchName != "")
            {
                trainers = trainers.Where(e => EF.Functions.ToTsVector("simple", EF.Functions.Unaccent(e.Fullname.ToLower())
                                                                                 + " "
                                                                                 + EF.Functions.Unaccent(e.Username.ToLower())
                                                                                 + " "
                                                                                 + EF.Functions.Unaccent(e.Email.ToLower()))
                    .Matches(EF.Functions.ToTsQuery("simple", EF.Functions.Unaccent(paginationParameter.SearchName.ToLower()))));
            }
            IEnumerable<Trainer> rs = await trainers
                .GetCount(out var totalRecords)
                .OrderByDescending(e => e.CreatedAt)
                .GetPage(paginationParameter)
                .ToListAsync();
            return Tuple.Create(totalRecords, rs);
        }
        public bool CheckTrainerExist(int id)
        {
            return _dataContext.Trainers.Any(t => t.TrainerId == id &&
           t.IsDeactivated == false);
        }
        public async Task<Trainer> GetTrainerByCalendarId(int id)
        {
            Calendar calendar = await _dataContext.Calendars.Where(c => c.CalendarId == id).FirstOrDefaultAsync();
            var tId = await _dataContext.ClassModules.Where(m => m.ModuleId == calendar.ModuleId && m.ClassId == calendar.ClassId).Select(t => t.TrainerId).FirstOrDefaultAsync();
            Trainer t = await _dataContext.Trainers.Where(m => m.TrainerId == tId).FirstOrDefaultAsync();
            return t;
        }
        public async Task<IEnumerable<Trainer>> GetTrainerListByClassId(int id)
        {
            var trainerIdList = await _dataContext.ClassModules.Where(t => t.ClassId == id).Select(t => t.TrainerId).ToListAsync();
            //Delete duplicated trainerId
            trainerIdList = trainerIdList.Distinct().ToList();
            List<Trainer> trainers = new List<Trainer>();
            foreach (var item in trainerIdList)
            {
                trainers.Add(await _dataContext.Trainers.Where(t => t.TrainerId == item).FirstOrDefaultAsync());
            }
            return trainers;
        }
    }
}
