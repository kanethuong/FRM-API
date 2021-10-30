using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public interface IAdminService
    {
        Task<Admin> GetAdminById(int id);
        Task<Admin> GetAdminByUsername(string username);
        Task<Admin> GetAdminByEmail(string email);
        Task<int> InsertNewAdmin(Admin admin);
        bool InsertNewAdminNoSaveChange(Admin admin);
        Task<int> UpdateAdmin(int id, Admin admin);
        Task<int> DeleteAdmin(int id);
        Task<Admin> getAdminByClassId(int id);
        bool CheckAdminExist(int id);
    }
}