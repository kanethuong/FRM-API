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
    public class ApplicationService : IApplicationService
    {
        private DataContext _dataContext;
        public ApplicationService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        /// <summary>
        /// Get Application By id
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Application </returns>
        public async Task<Application> GetExamById(int id)
        {   

            return await _dataContext.Applications.Where(c => c.ApplicationId == id ).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Insert new application
        /// </summary>
        /// <param name="application"></param>
        /// <returns> Return 1 if insert success and 0 if insert fail </returns>
        public async Task<int> InsertNewApplication(Application application){
            _dataContext.Applications.Add(application);
            int rs = await _dataContext.SaveChangesAsync();
            return rs;
        }
        /// <summary>
        /// Get a list of application
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns> Tuple List of application List </returns>
        public async Task<Tuple<int, IEnumerable<Application>>> GetApplicationList(PaginationParameter paginationParameter){
            var applicationList = await _dataContext.Applications.ToListAsync();
            int totalRecords = applicationList.Count();
            var rs = applicationList.OrderBy(c => c.ApplicationId)
                                            .Skip((paginationParameter.PageNumber - 1) * paginationParameter.PageSize)
                                            .Take(paginationParameter.PageSize);
            return Tuple.Create(totalRecords, rs);
        }

        
       
    }
}