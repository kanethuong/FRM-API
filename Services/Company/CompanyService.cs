using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Helper;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class CompanyService : ICompanyService
    {
        private DataContext _dataContext;

        public CompanyService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// Get a company by its ID
        /// </summary>
        /// <param name="id">ID of company</param>
        /// <returns>Company data</returns>
        public async Task<Company> GetCompanyById(int id)
        {
            return await _dataContext.Companies.Where(c => c.CompanyId == id && c.IsDeactivated == false).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get a company by its username
        /// </summary>
        /// <param name="username">Username of company</param>
        /// <returns>Company data</returns>
        public async Task<Company> GetCompanyByUsername(string username)
        {
            return await _dataContext.Companies.Where(c => c.Username.Equals(username) && c.IsDeactivated == false).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get a company by its email
        /// </summary>
        /// <param name="username">Email of company</param>
        /// <returns>Company data</returns>
        public async Task<Company> GetCompanyByEmail(string email)
        {
            return await _dataContext.Companies.Where(c => c.Email.ToLower().Equals(email.ToLower()) && c.IsDeactivated == false).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get a report list of companies
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns>Total record, report list</returns>
        public async Task<Tuple<int, IEnumerable<CompanyRequest>>> GetCompanyReportList(PaginationParameter paginationParameter)
        {
            var reportList = await _dataContext.CompanyRequests.Where(c => c.IsAccepted == true
              && (c.Company.Fullname.ToLower().Contains(paginationParameter.SearchName.ToLower()) ||
                  c.Company.Username.ToLower().Contains(paginationParameter.SearchName.ToLower()) ||
                  c.Company.Email.ToLower().Contains(paginationParameter.SearchName.ToLower())))
                .Select(c => new CompanyRequest{
                            CompanyRequestId=c.CompanyRequestId,
                            Company=new Company{
                                Fullname=c.Company.Fullname
                            }


                })
                .OrderByDescending(c => c.AcceptedAt)
                .ToListAsync();

            return Tuple.Create(reportList.Count(),
                                PaginationHelper.GetPage(reportList, paginationParameter.PageSize, paginationParameter.PageNumber));
        }

        /// <summary>
        /// Insert new company to database
        /// </summary>
        /// <param name="company">Company data</param>
        /// <returns>-1: existed / 0: fail / 1: done</returns>
        public async Task<int> InsertNewCompany(Company company)
        {
            if (_dataContext.Companies.Any(c => c.Username == company.Username || c.Email == company.Email))
            {
                return -1;
            }

            int rowInserted = 0;

            _dataContext.Companies.Add(company);
            rowInserted = await _dataContext.SaveChangesAsync();

            return rowInserted;
        }

        /// <summary>
        /// Insert new (company) account to DbContext without save change to DB
        /// </summary>
        /// <param name="company">Company data</param>
        /// <returns>true: insert done / false: dupplicate data</returns>
        public bool InsertNewCompanyNoSaveChange(Company company)
        {
            if (_dataContext.Companies.Any(c => c.Username == company.Username || c.Email == company.Email))
            {
                return false;
            }

            _dataContext.Companies.Add(company);

            return true;
        }

        /// <summary>
        /// Update a company with new input data
        /// </summary>
        /// <param name="id">ID of company to update</param>
        /// <param name="company">New company data</param>
        /// <returns>-1: not found / 0: fail / 1: done</returns>
        public async Task<int> UpdateCompany(int id, Company company)
        {
            var existedCompany = await _dataContext.Companies.Where(c => c.CompanyId == id && c.IsDeactivated == false).FirstOrDefaultAsync();

            if (existedCompany == null)
            {
                return -1;
            }

            existedCompany.Fullname = company.Fullname;
            existedCompany.AvatarURL = company.AvatarURL;
            existedCompany.Phone = company.Phone;
            existedCompany.Address = company.Address;
            existedCompany.Gender = company.Gender;

            int rowUpdated = 0;
            rowUpdated = await _dataContext.SaveChangesAsync();
            return rowUpdated;
        }

        /// <summary>
        /// Delete a company in database
        /// </summary>
        /// <param name="id">ID of company to delete</param>
        /// <returns>-1: not found / 0: fail / 1: done</returns>
        public async Task<int> DeleteCompany(int id)
        {
            var existedCompany = await _dataContext.Companies.Where(c => c.CompanyId == id && c.IsDeactivated == false).FirstOrDefaultAsync();

            if (existedCompany == null)
            {
                return -1;
            }

            existedCompany.IsDeactivated = true;
            existedCompany.DeactivatedAt = DateTime.Now;

            int rowDeleted = 0;
            rowDeleted = await _dataContext.SaveChangesAsync();
            return rowDeleted;
        }
    }
}