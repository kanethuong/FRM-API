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
using Microsoft.AspNetCore.Mvc;
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
            IQueryable<Application> applicationList = _dataContext.Applications;
            if (paginationParameter.SearchName != "")
            {
                applicationList = applicationList.Where(c => EF.Functions.ToTsVector("simple", EF.Functions.Unaccent(c.ApplicationCategory.CategoryName.ToLower())
                                                                                        + " "
                                                                                        + EF.Functions.Unaccent(c.Trainee.Fullname.ToLower()))
                    .Matches(EF.Functions.ToTsQuery("simple", EF.Functions.Unaccent(paginationParameter.SearchName.ToLower()))));
            }

            List<Application> rs = await applicationList
                .GetCount(out var totalRecords)
                .OrderByDescending(e => e.CreatedAt)
                .Select(a => new Application
                {
                    TraineeId = a.TraineeId,
                    Trainee = a.Trainee,
                    Description = a.Description,
                    ApplicationURL = a.ApplicationURL,
                    ApplicationId = a.ApplicationId,
                    ApplicationCategoryId = a.ApplicationCategoryId,
                    ApplicationCategory = a.ApplicationCategory,
                    IsAccepted = a.IsAccepted,
                    CreatedAt = a.CreatedAt,
                    AcceptedAt = a.AcceptedAt
                })
                .ToListAsync();

            List<ApplicationResponse> applicationReponse = new List<ApplicationResponse>();

            foreach (var item in rs)
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

            return Tuple.Create(totalRecords, applicationReponse.GetPage(paginationParameter));
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
                                                                Response = a.Response,
                                                                ApplicationCategoryId = a.ApplicationCategoryId,
                                                                ApplicationCategory = new ApplicationCategory
                                                                {
                                                                    ApplicationCategoryId = a.ApplicationCategoryId,
                                                                    CategoryName = a.ApplicationCategory.CategoryName,
                                                                },
                                                                ApplicationURL = a.ApplicationURL,
                                                                IsAccepted = a.IsAccepted,
                                                                AcceptedAt = a.AcceptedAt,
                                                            })
                                                            .FirstOrDefaultAsync();

            return appDetail;
        }

        /// <summary>
        /// Confirm application method
        /// </summary>
        /// <param name="confirmApplicationInput">Confirm Application Input DTO</param>
        /// <param name="response">message of admin to response</param>
        /// <param name="isAccepted">true/false/null</param>
        /// <returns>-1,-2:Not found / 0:Fail to confirm / 1:Confirmed</returns>
        public async Task<int> ConfirmApplication(int id, [FromBody] ConfirmApplicationInput confirmApplicationInput)
        {
            var existedApplication = await _dataContext.Applications.Where(a => a.ApplicationId == id && a.IsAccepted == null).FirstOrDefaultAsync();
            var checkAdmin = await _dataContext.Admins.Where(a => a.AdminId == confirmApplicationInput.AdminId && a.IsDeactivated == false).FirstOrDefaultAsync();
            if (existedApplication == null)
            {
                return -1;
            }
            if (checkAdmin == null)
            {
                return -2;
            }
            existedApplication.Response = confirmApplicationInput.Response;
            existedApplication.IsAccepted = confirmApplicationInput.IsAccepted;
            existedApplication.AcceptedAt = DateTime.Now;
            existedApplication.AdminId = confirmApplicationInput.AdminId;
            var rowUpdated = await _dataContext.SaveChangesAsync();

            return rowUpdated;
        }

    }
}