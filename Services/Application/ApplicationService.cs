using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ApplicationDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Helper;
using kroniiapi.Helper.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class ApplicationService : IApplicationService
    {
        private DataContext _dataContext;
        private readonly IMegaHelper _megaHelper;
        public ApplicationService(DataContext dataContext, IMegaHelper megaHelper)
        {
            _dataContext = dataContext;
            _megaHelper = megaHelper;
        }
        /// <summary>
        /// Get Application By id
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Application </returns>
        public async Task<Application> GetApplicationById(int id)
        {

            return await _dataContext.Applications.Where(c => c.ApplicationId == id).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Insert new application
        /// </summary>
        /// <param name="application"></param>
        /// <returns> Return 1 if insert success and 0 if insert fail </returns>
        public async Task<int> InsertNewApplication(Application application, IFormFile form)
        {
            //check if trainee in DB
            var checkTrainee = _dataContext.Trainees.Any(t => t.TraineeId == application.TraineeId && t.IsDeactivated == false);
            if (checkTrainee is false)
            {
                return -1;
            }
            var checkCategory = _dataContext.ApplicationCategories.Any(t => t.ApplicationCategoryId == application.ApplicationCategoryId);
            if (checkCategory is false)
            {
                return -2;
            }
            var stream = form.OpenReadStream();
            String formURL = await _megaHelper.Upload(stream, form.FileName, "ApplicationForm");
            application.ApplicationURL = formURL;
            _dataContext.Applications.Add(application);
            int rs = await _dataContext.SaveChangesAsync();
            return rs;
        }
        /// <summary>
        /// Get a list of application
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns> Tuple List of application </returns>
        public async Task<Tuple<int, IEnumerable<ApplicationResponse>>> GetApplicationList(PaginationParameter paginationParameter)
        {
            var applicationList = await _dataContext.Applications.Where(app => app.ApplicationCategory.CategoryName.ToUpper().Contains(paginationParameter.SearchName.ToUpper()) && app.Trainee.Fullname.ToUpper().Contains(paginationParameter.SearchName.ToUpper()))
                                                    .Select(a => new Application
                                                    {
                                                        TraineeId = a.TraineeId,
                                                        Trainee = new Trainee
                                                        {
                                                            TraineeId = a.TraineeId,
                                                            Fullname = a.Trainee.Fullname
                                                        },
                                                        Description = a.Description,
                                                        ApplicationURL = a.ApplicationURL,
                                                        ApplicationId = a.ApplicationId,
                                                        ApplicationCategoryId = a.ApplicationCategoryId,
                                                        ApplicationCategory = new ApplicationCategory
                                                        {
                                                            ApplicationCategoryId = a.ApplicationCategoryId,
                                                            CategoryName = a.ApplicationCategory.CategoryName,
                                                        },
                                                        IsAccepted = a.IsAccepted,
                                                        CreatedAt = a.CreatedAt,
                                                        AcceptedAt = a.AcceptedAt
                                                    })
                                                    .OrderByDescending(c => c.CreatedAt)
                                                    .ToListAsync();
            List<ApplicationResponse> applicationReponse = new List<ApplicationResponse>();

            foreach (var item in applicationList)
            {
                var itemToResponse = new ApplicationResponse
                {
                    ApplicationId = item.ApplicationId,
                    TraineeName = item.Trainee.Fullname,
                    Category = item.ApplicationCategory.CategoryName,
                    IsAccepted = item.IsAccepted,
                    AcceptedAt = item.AcceptedAt,
                    CreatedAt = item.CreatedAt

                };
                applicationReponse.Add(itemToResponse);
            }

            return Tuple.Create(applicationReponse.Count(), PaginationHelper.GetPage(applicationReponse,
                paginationParameter.PageSize, paginationParameter.PageNumber));
        }

        public async Task<IEnumerable<ApplicationCategory>> GetApplicationCategoryList()
        {
            return await _dataContext.ApplicationCategories.ToListAsync();
        }
        /// <summary>
        /// Get Application Category by id
        /// </summary>
        /// <param name="id">Application Category Id</param>
        /// <returns>Application Category data</returns>
        public async Task<ApplicationCategory> GetApplicationCategory(int id)
        {
            return await _dataContext.ApplicationCategories.Where(a => a.ApplicationCategoryId == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get Appliaction Detail
        /// </summary>
        /// <param name="id">Application Id</param>
        /// <returns>Application</returns>
        public async Task<Application> GetApplicationDetail(int id)
        {
            var appDetail = await _dataContext.Applications.Where(a => a.ApplicationId == id)
                                                            .Select(a => new Application
                                                            {
                                                                ApplicationId = a.ApplicationId,
                                                                Description = a.Description,
                                                                CreatedAt = a.CreatedAt,
                                                                TraineeId = a.TraineeId,
                                                                Trainee = new Trainee
                                                                {
                                                                    TraineeId = a.TraineeId,
                                                                    Fullname = a.Trainee.Fullname,
                                                                    AvatarURL = a.Trainee.AvatarURL,
                                                                    Email = a.Trainee.Email
                                                                },
                                                                AdminId = a.AdminId,

                                                                Admin = new Admin
                                                                {
                                                                    AdminId = (int)a.AdminId,
                                                                    Fullname = a.Admin.Fullname,
                                                                    AvatarURL = a.Admin.AvatarURL,
                                                                    Email = a.Admin.Email,
                                                                },
                                                                Response = a.Response,
                                                                ApplicationCategoryId = a.ApplicationCategoryId,
                                                                ApplicationCategory = new ApplicationCategory
                                                                {
                                                                    ApplicationCategoryId = a.ApplicationCategoryId,
                                                                    CategoryName = a.ApplicationCategory.CategoryName,
                                                                },
                                                                ApplicationURL = a.ApplicationURL
                                                            })
                                                            .FirstOrDefaultAsync();

            return appDetail;
        }

<<<<<<< HEAD
        /// <summary>
        /// Confirm application method
        /// </summary>
        /// <param name="id">application id</param>
        /// <param name="response">message of admin to response</param>
        /// <param name="isAccepted">true/false/null</param>
        /// <returns>-1:Not found / 0:Fail to confirm / 1:Confirmed</returns>
        public async Task<int> ConfirmApplication(int id, string response, bool isAccepted)
        {
            var existedApplication = await _dataContext.Applications.Where(a => a.ApplicationId == id).FirstOrDefaultAsync();
            if (existedApplication == null)
            {
                return -1;
            }
            existedApplication.Response = response;
            existedApplication.IsAccepted = isAccepted;
            existedApplication.AcceptedAt = DateTime.Now;
            var rowUpdated = await _dataContext.SaveChangesAsync();

            return rowUpdated;
        }

=======
>>>>>>> c7d153a0a818166fad55d694f6b96d9966cab0a0
    }
}