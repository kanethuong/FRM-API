using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DTO.AccountDTO;
using kroniiapi.DTO.PaginationDTO;

namespace kroniiapi.Services
{
    public interface IAccountService
    {
        Task<Tuple<int, IEnumerable<AccountResponse>>> GetAccountList(PaginationParameter paginationParameter);
        Task<AccountResponse> GetAccountByUsername(string username);
        Task<Tuple<AccountResponse, string>> GetAccountByEmail(string email);
        Task<int> DeactivateAccount(int id, string role);
        Task<int> InsertNewAccount(AccountInput accountInput);
        Task<Tuple<int, IEnumerable<AccountResponse>>> GetDeactivatedAccountList(PaginationParameter paginationParameter);
        Task<int> UpdateAccountPassword(string email, string password);
        Task<int> SaveChange();
        void DiscardChanges();
    }
}