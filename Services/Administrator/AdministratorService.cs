using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class AdministratorService : IAdministratorService
    {
        private DataContext _dataContext;

        public AdministratorService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// Get Administrator by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Administrator </returns>
        public async Task<Administrator> GetAdministratorById(int id)
        {
            return await _dataContext.Administrators.Where(a => a.AdministratorId == id).FirstOrDefaultAsync(); 
        }

        /// <summary>
        /// Get Administrator by Username
        /// </summary>
        /// <param name="username"></param>
        /// <returns> Administrator </returns>
        public async Task<Administrator> GetAdministratorByUsername(string username)
        {
            return await _dataContext.Administrators.Where(a => a.Username == username).FirstOrDefaultAsync(); 
        }
        /// <summary>
        ///  Get Administrator by Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns> Administrator </returns>
        public async Task<Administrator> GetAdministratorByEmail(string email)
        {
            return await _dataContext.Administrators.Where(a => a.Email == email).FirstOrDefaultAsync(); 
        }
    }
}