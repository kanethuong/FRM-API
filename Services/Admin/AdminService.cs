using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public class AdminService : IAdminService
    {
        private DataContext _dataContext;

        public AdminService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Admin> GetAdminById(int id)
        {
            return null;
        }

        public async Task<Admin> GetAdminByUsername(string username)
        {
            return null;
        }

        public async Task<Admin> GetAdminByEmail(string email)
        {
            return null;
        }

        public async Task<int> InsertNewAdmin(Admin admin)
        {
            return 0;
        }

        public async Task<int> UpdateAdmin(int id, Admin admin)
        {
            return 0;
        }

        public async Task<int> DeleteAdmin(int id)
        {
            return 0;
        }
    }
}