using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;
using Microsoft.EntityFrameworkCore;
using kroniiapi.Helper;

namespace kroniiapi.Services
{
    public class AdminService : IAdminService
    {
        private DataContext _dataContext;

        public AdminService(DataContext dataContext, IClassService classService)
        {
            _dataContext = dataContext;
            _classService = classService;
        }
        private IClassService _classService;
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
            return await _dataContext.Admins.Where(a => a.Email.ToLower().Equals(email.ToLower()) && a.IsDeactivated == false).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Get admin list
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns> Tuple list of all admin</returns>
        public async Task<Tuple<int, IEnumerable<Admin>>> GetAdminList(PaginationParameter paginationParameter)
        {
            IQueryable<Admin> admins = _dataContext.Admins.Where(t => t.IsDeactivated == false);
            if (paginationParameter.SearchName != "")
            {
                admins = admins.Where(e => EF.Functions.ToTsVector("simple", EF.Functions.Unaccent(e.Fullname.ToLower())
                                                                    + " "
                                                                    + EF.Functions.Unaccent(e.Username.ToLower())
                                                                    + " "
                                                                    + EF.Functions.Unaccent(e.Email.ToLower()))
                    .Matches(EF.Functions.ToTsQuery("simple", EF.Functions.Unaccent(paginationParameter.SearchName.ToLower()))));
            }
            IEnumerable<Admin> rs = await admins
                .GetCount(out var totalRecords)
                .OrderByDescending(e => e.CreatedAt)
                .GetPage(paginationParameter)
                .ToListAsync();
            return Tuple.Create(totalRecords, rs);
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

        /// <summary>
        /// Insert new (admin) account to DbContext without save change to DB
        /// </summary>
        /// <param name="admin">Admin data</param>
        /// <returns>true: insert done / false: dupplicate data</returns>
        public bool InsertNewAdminNoSaveChange(Admin admin)
        {
            if (_dataContext.Admins.Any(a =>
                a.Username.Equals(admin.Username) ||
                a.Email.Equals(admin.Email)
            ))
            {
                return false;
            }
            _dataContext.Admins.Add(admin);
            return true;
        }
        /// <summary>
        /// Get Admin by ClassID
        /// </summary>
        /// <param name="id">Admin ID</param>
        /// <returns>Admin data</returns>
        public async Task<Admin> getAdminByClassId(int id)
        {
            Class class1 = await _classService.GetClassByClassID(id);
            if (class1 == null)
            {
                return null;
            }
            return await _dataContext.Admins.Where(a => a.AdminId == class1.AdminId && a.IsDeactivated == false).FirstOrDefaultAsync();

        }
        /// <summary>
        /// Check admin existed in DB and isDeactivated == false
        /// </summary>
        /// <param name="id">Admin ID</param>
        /// <returns>true: Admin exist in DB and isDeactivated == false / false: Admin doesn't exist in DB or isDeactivated == true</returns>
        public bool CheckAdminExist(int id)
        {
            return _dataContext.Admins.Any(t => t.AdminId == id &&
           t.IsDeactivated == false);
        }
    }
}