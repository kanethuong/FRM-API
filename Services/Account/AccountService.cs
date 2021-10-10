using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB;
using kroniiapi.DTO.AccountDTO;
using kroniiapi.DTO.PaginationDTO;

namespace kroniiapi.Services
{
    public class AccountService : IAccountService
    {
        private DataContext _dataContext;
        private AdminService _adminService;
        private AdministratorService _administratorService;
        private CompanyService _companyService;
        private TrainerService _trainerService;
        private TraineeService _traineeService;
        private IMapper _mapper;

        public AccountService(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            // _adminService = adminService;
            // _administratorService = administratorService;
            // _companyService = companyService;
            // _traineeService = traineeService;
            // _trainerService = trainerService;
            _mapper = mapper;
        }

        public async Task<Tuple<int, IEnumerable<AccountResponse>>> GetAccountList(PaginationParameter paginationParameter)
        {
            return null;
        }

        public async Task<AccountResponse> GetAccountByUsername(string username)
        {
            return null;
        }

        public async Task<AccountResponse> GetAccountByEmail(string email)
        {
            return null;
        }

        public async Task<bool> DeactivateAccount(int id, string role)
        {
            return true;
        }

        public async Task<int> InsertNewAccount(AccountInput accountInput)
        {
            return 0;
        }

        public async Task<Tuple<int, IEnumerable<AccountResponse>>> GetDeactivatedAccountList(PaginationParameter paginationParameter)
        {
            return null;
        }

        public async Task<int> UpdateAccountPassword(string email, string password)
        {
            return 0;
        }
    }
}