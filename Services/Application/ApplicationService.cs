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
            var checkTrainee = _dataContext.Trainees.Any(t => t.TraineeId == application.TraineeId && t.IsDeactivated==false);
            if(checkTrainee is false){
                return -1;
            }
            var checkCategory =_dataContext.ApplicationCategories.Any(t => t.ApplicationCategoryId == application.ApplicationCategoryId);
            if(checkCategory is false){
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
        public async Task<Tuple<int, IEnumerable<TraineeApplicationResponse>>> GetApplicationList(PaginationParameter paginationParameter)
        {
            var applicationList = await _dataContext.Applications.Where(app => app.ApplicationCategory.CategoryName.ToUpper().Contains(paginationParameter.SearchName.ToUpper()))
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
                                                        IsAccepted = a.IsAccepted,
                                                    })
                                                    .ToListAsync();
            List<TraineeApplicationResponse> applicationReponse = new List<TraineeApplicationResponse>();

            foreach (var item in applicationList)
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

    }
}