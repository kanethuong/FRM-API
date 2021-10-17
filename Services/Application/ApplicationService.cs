using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;

namespace kroniiapi.Services
{
    public class ApplicationService : IApplicationService
    {
        private DataContext _dataContext;
        public ApplicationService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<Application> GetExamById(int id)
        {
            return null;
        }
        public async Task<int> InsertNewApplication(Application application){
            return 0;
        }
        public async Task<Tuple<int, IEnumerable<Exam>>> GetApplicationList(PaginationParameter paginationParameter){
            return null;
        }
    }
}