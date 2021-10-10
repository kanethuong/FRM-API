using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public class AdministratorService : IAdministratorService
    {
        private DataContext _dataContext;

        public AdministratorService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Administrator> GetAdministratorById(int id)
        {
            return null;
        }

        public async Task<Administrator> GetAdministratorByUsername(string username)
        {
            return null;
        }

        public async Task<Administrator> GetAdministratorByEmail(string email)
        {
            return null;
        }
    }
}