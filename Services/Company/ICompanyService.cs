using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO.CompanyDTO;
using kroniiapi.DTO.PaginationDTO;

namespace kroniiapi.Services
{
    public interface ICompanyService
    {
        Task<Company> GetCompanyById(int id);
        Task<Company> GetCompanyByUsername(string username);
        Task<Company> GetCompanyByEmail(string email);
        Task<int> InsertNewCompany(Company company);
        bool InsertNewCompanyNoSaveChange(Company company);
        Task<int> UpdateCompany(int id, Company company);
        Task<int> DeleteCompany(int id);
        Task<CompanyRequest> GetCompanyRequestById(int id);
        Task<int> ConfirmCompanyRequest(int id, bool isAccepted);
        Task<Tuple<int, IEnumerable<CompanyRequestResponse>>> GetCompanyRequestList(PaginationParameter paginationParameter);
        Task<CompanyRequest> GetCompanyRequestDetail(int requestId);
    }
}