using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public interface IAdministratorService
    {
        Task<Administrator> GetAdministratorById(int id);
        Task<Administrator> GetAdministratorByUsername(string username);
        Task<Administrator> GetAdministratorByEmail(string email);
    }
}