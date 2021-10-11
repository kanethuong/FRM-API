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
        /// <summary>
        /// Get admin using id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Admin data</returns>
        public async Task<Admin> GetAdminById(int id)
        {
            return await _dataContext.Admins.Where(a => a.AdminId == id && a.IsDeactivated == false).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Get admin using username
        /// </summary>
        /// <param name="username"></param>
        /// <returns> Admin data</returns>
        public async Task<Admin> GetAdminByUsername(string username)
        {
            return await _dataContext.Admins.Where(a => a.Username.Equals(username) && a.IsDeactivated == false).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Get admin using email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Admin data</returns>
        public async Task<Admin> GetAdminByEmail(string email)
        {
            return await _dataContext.Admins.Where(a => a.Email.Equals(email) && a.IsDeactivated == false).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Insert new admin to database
        /// </summary>
        /// <param name="admin"></param>
        /// <returns>-1: existed / 0: fail / 1: done</returns>
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
        /// <summary>
        /// Update admin with new input data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="admin"></param>
        /// <returns>-1: not found / 0: fail / 1: done</returns>
        public async Task<int> UpdateAdmin(int id, Admin admin)
        {
            var existedAdmin = await _dataContext.Admins.Where(a => a.AdminId == id && a.IsDeactivated == false).FirstOrDefaultAsync();
            if (existedAdmin == null)
            {
                return -1;
            }
            existedAdmin.Fullname = admin.Fullname;
            existedAdmin.AvatarURL = admin.AvatarURL;
            existedAdmin.Phone = admin.Phone;
            existedAdmin.DOB = admin.DOB;
            existedAdmin.Address = admin.Address;
            existedAdmin.Gender = admin.Gender;
            var rowUpdated = 0;
            rowUpdated = await _dataContext.SaveChangesAsync();
            return rowUpdated;
        }
        /// <summary>
        /// Delete admin in database by upadating isDeactivated to "true"
        /// </summary>
        /// <param name="id"></param>
        /// <returns>-1: not found / 0: fail / 1: done<</returns>
        public async Task<int> DeleteAdmin(int id)
        {
            var existedAdmin = await _dataContext.Admins.Where(a => a.AdminId == id && a.IsDeactivated == false).FirstOrDefaultAsync();
            if (existedAdmin == null)
            {
                return -1;
            }
            existedAdmin.IsDeactivated = true;
            existedAdmin.DeactivatedAt = DateTime.Now;
            var rowUpdated = 0;
            rowUpdated = await _dataContext.SaveChangesAsync();
            return rowUpdated;
        }
    }
}