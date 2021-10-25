using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;

namespace kroniiapi.Services
{
    public interface IApplicationService
    {
        Task<Application> GetExamById(int id);

        Task<int> InsertNewApplication(Application application);
        Task<Tuple<int, IEnumerable<Application>>> GetApplicationList(PaginationParameter paginationParameter);
        Task<IEnumerable<ApplicationCategory>> GetApplicationCategoryList ();
    }
}