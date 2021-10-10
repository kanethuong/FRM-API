using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.DTO.PaginationDTO;

namespace kroniiapi.Services
{
    public class ClassService : IClassService
    {
        private DataContext _dataContext;

        public ClassService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Tuple<int, IEnumerable<Class>>> GetClassList(PaginationParameter paginationParameter)
        {
            return null;
        }

        public async Task<Class> GetClassByClassName(string className)
        {
            return null;
        }

        public async Task<Tuple<int, IEnumerable<Class>>> GetRequestDeletedClassList(PaginationParameter paginationParameter)
        {
            return null;
        }

        public async Task<Tuple<int, IEnumerable<Class>>> UpdateDeletedClass(ConfirmDeleteClassInput confirmDeleteClassInput)
        {
            return null;
        }

        public async Task<Tuple<int, IEnumerable<Class>>> GetDeletedClassList(PaginationParameter paginationParameter)
        {
            return null;
        }
    }
}