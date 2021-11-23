using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ApplicationDTO;
using kroniiapi.DTO.CompanyDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TraineeDTO;
using kroniiapi.Helper;
using kroniiapi.Services.Report;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace kroniiapi.Services
{
    public class TraineeService : ITraineeService
    {
        private DataContext _dataContext;
        private readonly IReportService _reportService;
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
            var existedTrainee = await _dataContext.Trainees.Where(t => t.TraineeId == id && t.IsDeactivated == false).FirstOrDefaultAsync();
            if (existedTrainee == null)
            {
                return -1;
            }
            existedTrainee.Fullname = trainee.Fullname;
            existedTrainee.Phone = trainee.Phone;
            existedTrainee.DOB = trainee.DOB;
            existedTrainee.Address = trainee.Address;
            existedTrainee.Gender = trainee.Gender;
            existedTrainee.Wage = trainee.Wage;
            existedTrainee.Facebook = trainee.Facebook;
            var rowUpdated = await _dataContext.SaveChangesAsync();

            return rowUpdated;
        }

        /// <summary>
        /// Update trainee's avatar method
        /// </summary>
        /// <param name="id"></param>
        /// <param name="avatarUrl"></param>
        /// <returns>-1:not existed / 0:fail / 1:success</returns>
        public async Task<int> UpdateAvatar(int id, string avatarUrl)
        {
            var existedTrainee = await _dataContext.Trainees.Where(t => t.TraineeId == id && t.IsDeactivated == false).FirstOrDefaultAsync();
            if (existedTrainee == null)
            {
                return -1;
            }
            existedTrainee.AvatarURL = avatarUrl;

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

        /// <summary>
        /// Get Application List by Trainee id
        /// </summary>
        /// <param name="id">Trainee id</param>
        /// <param name="paginationParameter">Pagination</param>
        /// <returns>Tuple Application list as Pagination</returns>
        public async Task<Tuple<int, IEnumerable<TraineeApplicationResponse>>> GetApplicationListByTraineeId(int id, PaginationParameter paginationParameter)
        {

            IQueryable<Application> application = _dataContext.Applications.Where(m => m.TraineeId == id);
            if (paginationParameter.SearchName != "")
            {
                application = application.Where(c => EF.Functions.ToTsVector("simple", EF.Functions.Unaccent(c.ApplicationCategory.CategoryName.ToLower()))
                    .Matches(EF.Functions.ToTsQuery("simple", EF.Functions.Unaccent(paginationParameter.SearchName.ToLower()))));
            }
            List<Application> rs = await application
                .GetCount(out var totalRecords)
                .OrderByDescending(e => e.CreatedAt)
                .Select(a => new Application
                {
                    TraineeId = a.TraineeId,
                    Description = a.Description,
                    ApplicationURL = a.ApplicationURL,
                    ApplicationId = a.ApplicationId,
                    ApplicationCategoryId = a.ApplicationCategoryId,
                    ApplicationCategory = a.ApplicationCategory,
                    IsAccepted = a.IsAccepted,
                })
                .ToListAsync();
            List<TraineeApplicationResponse> applicationReponse = new List<TraineeApplicationResponse>();

            foreach (var item in rs)
            {
                var itemToResponse = new TraineeApplicationResponse
                {
                    Description = item.Description,
                    ApplicationURL = item.ApplicationURL,
                    Type = item.ApplicationCategory.CategoryName,
                    IsAccepted = item.IsAccepted
                };
                applicationReponse.Add(itemToResponse);
            }
            return Tuple.Create(totalRecords, applicationReponse.GetPage(paginationParameter));
        }

        /// <summary>
        /// Get Class Id using trainee id
        /// </summary>
        /// <param name="id">TraineeId</param>
        /// <returns>-1: Message / {classId}</returns>
        public async Task<(int, string)> GetClassIdByTraineeId(int id)
        {
            var trainee = await _dataContext.Trainees.FirstOrDefaultAsync(t => t.TraineeId == id);

            if (trainee == null)
            {
                return (-1, "Trainee not found");
            }

            if (trainee.ClassId == null)
            {
                return (-1, "Trainee does not have class");
            }

            return ((int)trainee.ClassId, "");
        }

        /// <summary>
        /// Get Mark and Skills by Trainee id
        /// </summary>
        /// <param name="id">Trainee id</param>
        /// <param name="paginationParameter"></param>
        /// <returns>Tuple Mark and Skill data</returns>
        public async Task<Tuple<int, IEnumerable<TraineeMarkAndSkill>>> GetMarkAndSkillByTraineeId(int id, PaginationParameter paginationParameter)
        {
            IQueryable<Mark> markList = _dataContext.Marks.Where(m => m.TraineeId == id);
            if (paginationParameter.SearchName != "")
            {
                markList = markList.Where(c => EF.Functions.ToTsVector("simple", EF.Functions.Unaccent(c.Module.ModuleName.ToLower()))
                    .Matches(EF.Functions.ToTsQuery("simple", EF.Functions.Unaccent(paginationParameter.SearchName.ToLower()))));
            }
            List<Mark> rs = await markList
                .GetCount(out var totalRecords)
                .OrderByDescending(e => e.PublishedAt)
                .Select(ma => new Mark
                {
                    ModuleId = ma.ModuleId,
                    Module = new Module
                    {
                        ModuleName = ma.Module.ModuleName,
                        Description = ma.Module.Description,
                        IconURL = ma.Module.IconURL,
                        Certificates = ma.Module.Certificates.ToList(),
                    }
                })
                .ToListAsync();

            List<TraineeMarkAndSkill> markAndSkills = new List<TraineeMarkAndSkill>();
            foreach (var item in rs)
            {
                var itemToResponse = new TraineeMarkAndSkill
                {
                    ModuleId = item.ModuleId,
                    ModuleName = item.Module.ModuleName,
                    Description = item.Module.Description,
                    IconURL = item.Module.IconURL,
                    Score = await this.GetScoreByTraineeIdAndModuleId(id, item.ModuleId),
                    CertificateURL = await this.GetCertificatesURLByTraineeIdAndModuleId(id, item.ModuleId),
                };
                markAndSkills.Add(itemToResponse);
            }
            return Tuple.Create(totalRecords, markAndSkills.GetPage(paginationParameter));

        }

        /// <summary>
        /// GetCertificatesURLByTraineeIdAndModuleId
        /// </summary>
        /// <param name="Traineeid"></param>
        /// <param name="Moduleid"></param>
        /// <returns>Certificate URL</returns>
        private async Task<string> GetCertificatesURLByTraineeIdAndModuleId(int Traineeid, int Moduleid)
        {
            var certificate = await _dataContext.Certificates.Where(m => m.TraineeId == Traineeid && m.ModuleId == Moduleid).FirstOrDefaultAsync();
            if (certificate == null)
            {
                return null;
            }
            return certificate.CertificateURL;
        }

        /// <summary>
        /// GetScoreByTraineeIdAndModuleId
        /// </summary>
        /// <param name="Traineeid"></param>
        /// <param name="Moduleid"></param>
        /// <returns>Score</returns>
        private async Task<float> GetScoreByTraineeIdAndModuleId(int Traineeid, int Moduleid)
        {
            var score = await _dataContext.Marks.Where(m => m.TraineeId == Traineeid && m.ModuleId == Moduleid).FirstOrDefaultAsync();
            if (score == null)
            {
                return 0;
            }
            return score.Score;
        }
        public async Task<Tuple<int, IEnumerable<Trainee>>> GetAllTraineeWithoutClass(PaginationParameter paginationParameter)
        {
            IQueryable<Trainee> trainees = _dataContext.Trainees.Where(t => t.IsDeactivated == false && t.ClassId == null);
            if (paginationParameter.SearchName != "")
            {
                trainees = trainees.Where(e => EF.Functions.ToTsVector("simple", EF.Functions.Unaccent(e.Fullname.ToLower())
                                                                    + " "
                                                                    + EF.Functions.Unaccent(e.Username.ToLower())
                                                                    + " "
                                                                    + EF.Functions.Unaccent(e.Email.ToLower()))
                    .Matches(EF.Functions.ToTsQuery("simple", EF.Functions.Unaccent(paginationParameter.SearchName.ToLower()))));
            }
            IEnumerable<Trainee> rs = await trainees
                .GetCount(out var totalRecords)
                .OrderByDescending(e => e.CreatedAt)
                .GetPage(paginationParameter)
                .ToListAsync();
            return Tuple.Create(totalRecords, rs);
        }
        public bool CheckTraineeExist(int id)
        {
            return _dataContext.Trainees.Any(t => t.TraineeId == id &&
           t.IsDeactivated == false);
        }
        public async Task<Tuple<int, IEnumerable<Trainee>>> GetAllTrainee(PaginationParameter paginationParameter)
        {
            IQueryable<Trainee> trainees = _dataContext.Trainees.Where(t => t.IsDeactivated == false);
            if (paginationParameter.SearchName != "")
            {
                trainees = trainees.Where(e => EF.Functions.ToTsVector("simple", EF.Functions.Unaccent(e.Fullname.ToLower())
                                                                    + " "
                                                                    + EF.Functions.Unaccent(e.Username.ToLower())
                                                                    + " "
                                                                    + EF.Functions.Unaccent(e.Email.ToLower()))
                    .Matches(EF.Functions.ToTsQuery("simple", EF.Functions.Unaccent(paginationParameter.SearchName.ToLower()))));
            }
            IEnumerable<Trainee> rs = await trainees
                .GetCount(out var totalRecords)
                .OrderByDescending(e => e.CreatedAt)
                .GetPage(paginationParameter)
                .ToListAsync();
            return Tuple.Create(totalRecords, rs);
        }

        public async Task<List<TraineeSkillResponse>> GetTraineeSkillByTraineeId(int traineeId)
        {
            var cers = await _dataContext.Certificates.Where(c => c.TraineeId == traineeId).Select(c => new Certificate
            {
                CreatedAt = c.CreatedAt,
                Module = new Module
                {
                    ModuleName = c.Module.ModuleName
                }
            }).ToListAsync();
            List<TraineeSkillResponse> tsr = new List<TraineeSkillResponse>();
            foreach (var item in cers)
            {
                TraineeSkillResponse temp = new();
                temp.ModuleName = item.Module.ModuleName;
                temp.FinishDate = item.CreatedAt;
                tsr.Add(temp);
            }
            return tsr;
        }
        public async Task<bool> AutoUpdateTraineesStatus(int classId)
        {
            var clazz = await _dataContext.Classes.Where(c => c.ClassId == classId).Select(c => new Class
            {
                ClassId = c.ClassId,
                EndDay = c.EndDay
            }).FirstOrDefaultAsync();
            if (clazz.EndDay > DateTime.Now)
            {
                return false;
            }
            DateTime tempTime = DateTime.Now;
            var traineeGPAs = await _reportService.GetTraineeGPAs(classId, tempTime);
            foreach (var item in traineeGPAs)
            {
                var trainee = await _dataContext.Trainees.Where(t => t.TraineeId == item.TraineeId).FirstOrDefaultAsync();
                if (item.Level == "D")
                {
                    trainee.Status = "Failed";
                }
                else
                {
                    trainee.Status = "Passed";
                }
                await _dataContext.SaveChangesAsync();
            }
            return true;
        }

    }
}