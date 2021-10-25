using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ApplicationDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TraineeDTO;
using kroniiapi.Helper;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class TraineeService : ITraineeService
    {
        private DataContext _dataContext;
        private IMapper _mapper;
        private IApplicationService _applicationService;

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
            return await _dataContext.Trainees.Where(t => t.Email.ToLower().Equals(email.ToLower()) &&
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


        /// <summary>
        /// Check if the trainee has a class
        /// </summary>
        /// <param name="traineeId">the trainee id</param>
        /// <returns>whether the trainee has a class</returns>
        public async Task<bool> IsTraineeHasClass(int traineeId)
        {
            var trainee = await GetTraineeById(traineeId);
            var traineeClassId = trainee?.ClassId;
            return traineeClassId is not null;
        }

        public async Task<Tuple<int, IEnumerable<TraineeAttendanceReport>>> GetAttendanceReports(int id, PaginationParameter paginationParameter)
        {
            Trainee trainee = await GetTraineeById(id);
            if (trainee.ClassId == null)
            {
                return null;
            }

            List<TraineeAttendanceReport> resultList = new List<TraineeAttendanceReport>();

            Dictionary<int, int> NumberOfAbsentByModule = new Dictionary<int, int>();

            IEnumerable<int> listModule = await _dataContext.ClassModules.
                Where(t => t.ClassId == trainee.ClassId).Select(t => t.ModuleId).ToListAsync();

            foreach (var module in listModule)         //init absent tracker by module id
            {
                NumberOfAbsentByModule.Add(module, 0);
            }

            IEnumerable<int> listCalendarIdAbsent = await _dataContext.Attendances.Where(
                t => t.IsAbsent == true && t.TraineeId == id).Select(i => i.CalendarId).ToListAsync();

            foreach (var calenderId in listCalendarIdAbsent) //count number of absent slot in each module id
            {
                int moduleId = _dataContext.Calendars.Where(t => t.CalendarId == calenderId).FirstOrDefault().ModuleId;
                try
                {
                    NumberOfAbsentByModule[moduleId] += 1;
                }
                catch
                {
                    NumberOfAbsentByModule.Add(moduleId, 0);
                }
            }


            foreach (var moduleId in listModule)
            {
                Module module = _dataContext.Modules.Where(t => t.ModuleId == moduleId).FirstOrDefault();
                resultList.Add(new TraineeAttendanceReport
                {
                    NoOfSlot = module.NoOfSlot,
                    ModuleName = module.ModuleName,
                    NumberSlotAbsent = NumberOfAbsentByModule[moduleId]
                });
            }

            return Tuple.Create(resultList.Count(), PaginationHelper.GetPage(resultList.Where(t =>
                t.ModuleName.ToLower().Contains(paginationParameter.SearchName.ToLower())), paginationParameter));
        }

        /// <summary>
        /// Get Application List by Trainee id
        /// </summary>
        /// <param name="id">Trainee id</param>
        /// <param name="paginationParameter">Pagination</param>
        /// <returns>Tuple Application list as Pagination</returns>
        public async Task<Tuple<int, IEnumerable<ApplicationResponse>>> GetApplicationListByTraineeId(int id, PaginationParameter paginationParameter)
        {

            List<Application> application = await _dataContext.Applications
                                                .Where(app => app.TraineeId == id)
                                                    .Select(a => new Application
                                                    {
                                                        TraineeId = a.TraineeId,
                                                        Description = a.Description,
                                                        ApplicationURL = a.ApplicationURL,
                                                        ApplicationId = a.ApplicationId,
                                                        ApplicationCategoryId = a.ApplicationCategoryId,
                                                        ApplicationCategory = new ApplicationCategory
                                                        {
                                                            ApplicationCategoryId = a.ApplicationCategoryId,
                                                            CategoryName = a.ApplicationCategory.CategoryName,
                                                        },
                                                    })
                                                    .ToListAsync();
            List<ApplicationResponse> applicationReponse = new List<ApplicationResponse>();

            foreach (var item in application)
            {
                var itemToResponse = new ApplicationResponse{
                    Description = item.Description,
                    ApplicationURL = item.ApplicationURL,
                    Type = item.ApplicationCategory.CategoryName,
                    IsAccepted = item.IsAccepted
                };
                applicationReponse.Add(itemToResponse);
            }

            return Tuple.Create(applicationReponse.Count(), PaginationHelper.GetPage(applicationReponse,
                paginationParameter.PageSize, paginationParameter.PageNumber));

        }
    }
}