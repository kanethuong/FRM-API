using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using Microsoft.EntityFrameworkCore;

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
            return await _dataContext.Admins.Where(a => a.AdminId == id && a.IsDeactivated == false).FirstOrDefaultAsync();
        }

        public async Task<Admin> GetAdminByUsername(string username)
        {
            return await _dataContext.Admins.Where(a => a.Username.Equals(username) && a.IsDeactivated == false).FirstOrDefaultAsync();
        }

        public async Task<Admin> GetAdminByEmail(string email)
        {
            return await _dataContext.Admins.Where(a => a.Email.Equals(email) && a.IsDeactivated == false).FirstOrDefaultAsync();
        }

        public async Task<int> InsertNewAdmin(Admin admin)
        {
            if (_dataContext.Admins.Any(a =>
                a.Username.Equals(admin.Username) ||
                a.Email.Equals(admin.Email)
            ))
            {
                return -1;
            }
            int rowInserted = 0;
            _dataContext.Admins.Add(admin);
            rowInserted = await _dataContext.SaveChangesAsync();
            return rowInserted;
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