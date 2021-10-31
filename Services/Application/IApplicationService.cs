using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ApplicationDTO;
using kroniiapi.DTO.PaginationDTO;
using Microsoft.AspNetCore.Http;

namespace kroniiapi.Services
{
    public interface IApplicationService
    {
        Task<Application> GetApplicationById(int id);

        Task<int> InsertNewApplication(Application application,IFormFile form);
        Task<Tuple<int, IEnumerable<TraineeApplicationResponse>>> GetApplicationList(PaginationParameter paginationParameter);
        Task<IEnumerable<ApplicationCategory>> GetApplicationCategoryList ();
        Task<ApplicationCategory> GetApplicationCategory(int id);
    }
}