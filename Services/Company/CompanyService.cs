using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public class CompanyService : ICompanyService
    {
        private DataContext _dataContext;

        public CompanyService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Company> GetCompanyById(int id)
        {
            return null;
        }

        public async Task<Company> GetCompanyByUsername(string username)
        {
            return null;
        }

        public async Task<Company> GetCompanyByEmail(string email)
        {
            return null;
        }

        public async Task<int> InsertNewCompany(Company company)
        {
            return 0;
        }

        public async Task<int> UpdateCompany(int id, Company company)
        {
            return 0;
        }

        public async Task<int> DeleteCompany(int id)
        {
            return 0;
        }
    }
}