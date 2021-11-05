using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ApplicationDTO;
using kroniiapi.DTO.PaginationDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Services
{
    public interface IApplicationService
    {
        Task<Application> GetApplicationById(int id);

        Task<int> InsertNewApplication(Application application,IFormFile form);
        Task<Tuple<int, IEnumerable<ApplicationResponse>>> GetApplicationList(PaginationParameter paginationParameter);
        Task<IEnumerable<ApplicationCategory>> GetApplicationCategoryList ();
        Task<ApplicationCategory> GetApplicationCategory(int id);
        Task<Application> GetApplicationDetail(int id);
        Task<int> ConfirmApplication(int id, [FromBody]ConfirmApplicationInput confirmApplicationInput);
    }
}