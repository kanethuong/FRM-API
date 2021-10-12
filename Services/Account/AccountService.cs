using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.AccountDTO;
using kroniiapi.DTO.PaginationDTO;

namespace kroniiapi.Services
{
    public class AccountService : IAccountService
    {
        private DataContext _dataContext;
        private IAdminService _adminService;
        private IAdministratorService _administratorService;
        private ICompanyService _companyService;
        private ITrainerService _trainerService;
        private ITraineeService _traineeService;
        private IMapper _mapper;

        public AccountService(DataContext dataContext, IMapper mapper, IAdminService adminService
        ,IAdministratorService administratorService, ICompanyService companyService, ITraineeService traineeService
        ,ITrainerService trainerService)
        {
            _dataContext = dataContext;
            _adminService = adminService;
            _administratorService = administratorService;
            _companyService = companyService;
            _traineeService = traineeService;
            _trainerService = trainerService;
            _mapper = mapper;
        }

        public async Task<Tuple<int, IEnumerable<AccountResponse>>> GetAccountList(PaginationParameter paginationParameter)
        {
            return null;
        }

        /// <summary>
        /// get account  in 5 role by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<AccountResponse> GetAccountByUsername(string username)
        {
            AccountResponse accountToReponse;

            async Task<string> getRoleName(int id)
            {
                Role role = await _dataContext.Roles.FindAsync(id);
                return role.RoleName;
            } 

            Administrator administrator = await _administratorService.GetAdministratorByUsername(username);
            if(administrator != null)
            {                 
                accountToReponse = _mapper.Map<AccountResponse>(administrator);
                accountToReponse.Role = await getRoleName(administrator.RoleId);
                return accountToReponse;
            }

            Company company = await _companyService.GetCompanyByUsername(username);
            if(company != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(company);
                accountToReponse.Role = await getRoleName(company.RoleId);
                return accountToReponse;
            }

            Trainer trainer = await _trainerService.GetTrainerByUsername(username);
            if(trainer != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(trainer);
                accountToReponse.Role = await getRoleName(trainer.RoleId);
                return accountToReponse;
            }

            Trainee trainee = await _traineeService.GetTraineeByUsername(username);
            if(trainee != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(trainee);
                accountToReponse.Role = await getRoleName(trainee.RoleId);
                return accountToReponse;
            }
            
            Admin admin = await _adminService.GetAdminByUsername(username);
            if(admin != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(admin);
                accountToReponse.Role = await getRoleName(admin.RoleId);
                return accountToReponse;
            }
            return null;
        }

        /// <summary>
        /// get account and password in 5 role by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Tuple<AccountResponse, string>> GetAccountByEmail(string email)
        {
            string password = "";
            AccountResponse accountToReponse;

            async Task<string> getRoleName(int id)
            {
                Role role = await _dataContext.Roles.FindAsync(id);
                return role.RoleName;
            } 

            Administrator administrator = await _administratorService.GetAdministratorByEmail(email);
            if(administrator != null)
            {                 
                accountToReponse = _mapper.Map<AccountResponse>(administrator);
                accountToReponse.Role = await getRoleName(administrator.RoleId);
                password = administrator.Password;
                return Tuple.Create(accountToReponse, password);
            }

            Company company = await _companyService.GetCompanyByEmail(email);
            if(company != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(company);
                accountToReponse.Role = await getRoleName(company.RoleId);
                password = company.Password;
                return Tuple.Create(accountToReponse, password);
            }

            Trainer trainer = await _trainerService.GetTrainerByEmail(email);
            if(trainer != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(trainer);
                accountToReponse.Role = await getRoleName(trainer.RoleId);
                password = trainer.Password;
                return Tuple.Create(accountToReponse, password);
            }

            Trainee trainee = await _traineeService.GetTraineeByEmail(email);
            if(trainee != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(trainee);
                accountToReponse.Role = await getRoleName(trainee.RoleId);
                password = trainee.Password;
                return Tuple.Create(accountToReponse, password);
            }
            
            Admin admin = await _adminService.GetAdminByEmail(email);
            if(admin != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(admin);
                accountToReponse.Role = await getRoleName(admin.RoleId);
                password = admin.Password;
                return Tuple.Create(accountToReponse, password);
            }
            return null;
        }

        /// <summary>
        /// Deactivate base on role
        /// </summary>
        /// <param name="id">id of user</param>
        /// <param name="role">role of user</param>
        /// <returns>-1:existed / 0:fail / 1:success</returns>
        public async Task<int> DeactivateAccount(int id, string role)
        {
            switch (role.ToLower())
            {
                case "administrator":
                    return 0;
                case "admin":
                    return await _adminService.DeleteAdmin(id);
                case "trainer":    
                    return await _trainerService.DeleteTrainer(id);
                case "trainee":
                    return await _traineeService.DeleteTrainee(id);
                case "company":
                    return await _companyService.DeleteCompany(id);
                default:
                    return 0;
            }
        }
        
        /// <summary>
        /// Insert new account method
        /// </summary>
        /// <param name="accountInput"></param>
        /// <returns>-1:existed / 0:fail / 1:success</returns>
        public async Task<int> InsertNewAccount(AccountInput accountInput)
        {
            accountInput.Role = accountInput.Role.ToLower();
            int rowInserted = 0;
            switch (accountInput.Role)
            {
                case "administrator":
                    break;
                case "admin":
                    var adminToAdd = _mapper.Map<Admin>(accountInput);
                    adminToAdd.RoleId = 2;
                    rowInserted = await _adminService.InsertNewAdmin(adminToAdd);
                    break;
                case "trainer":
                    var trainerToAdd = _mapper.Map<Trainer>(accountInput);
                    trainerToAdd.RoleId = 3;
                    rowInserted = await _trainerService.InsertNewTrainer(trainerToAdd);
                    break;
                case "trainee":
                    var traineeToAdd = _mapper.Map<Trainee>(accountInput);
                    traineeToAdd.RoleId = 4;
                    rowInserted = await _traineeService.InsertNewTrainee(traineeToAdd);
                    break;
                case "company":
                     var companyToAdd = _mapper.Map<Company>(accountInput);
                    companyToAdd.RoleId = 5;
                    rowInserted = await _companyService.InsertNewCompany(companyToAdd);
                    break;
                default:
                    break;
            }
            
            return rowInserted;
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