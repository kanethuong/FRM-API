using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.AccountDTO;
using kroniiapi.DTO.Email;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Helper;

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
        private IEmailService _emailService;
        private IMapper _mapper;

        public AccountService(DataContext dataContext, IMapper mapper, IAdminService adminService
        , IAdministratorService administratorService, ICompanyService companyService, ITraineeService traineeService
        , ITrainerService trainerService, IEmailService emailService)
        {
            _dataContext = dataContext;
            _adminService = adminService;
            _administratorService = administratorService;
            _companyService = companyService;
            _traineeService = traineeService;
            _trainerService = trainerService;
            _emailService = emailService;
            _mapper = mapper;
        }

        private async Task<string> getRoleName(int id)
        {
            Role role = await _dataContext.Roles.FindAsync(id);
            return role.RoleName;
        }

        private async Task<IEnumerable<AccountResponse>> addAccountToTotalList(IEnumerable<Administrator> administrators,
        IEnumerable<Admin> admins, IEnumerable<Trainer> trainers, IEnumerable<Trainee> trainees, IEnumerable<Company> companies)
        {
            List<AccountResponse> totalAccount = new List<AccountResponse>();

            foreach (var item in administrators)
            {
                var itemToResponse = _mapper.Map<AccountResponse>(item);
                itemToResponse.Role = await getRoleName(item.RoleId);
                itemToResponse.AccountId = item.AdministratorId;
                totalAccount.Add(itemToResponse);
            }

            foreach (var item in admins)
            {
                var itemToResponse = _mapper.Map<AccountResponse>(item);
                itemToResponse.Role = await getRoleName(item.RoleId);
                itemToResponse.AccountId = item.AdminId;
                totalAccount.Add(itemToResponse);
            }

            foreach (var item in trainers)
            {
                var itemToResponse = _mapper.Map<AccountResponse>(item);
                itemToResponse.Role = await getRoleName(item.RoleId);
                itemToResponse.AccountId = item.TrainerId;
                totalAccount.Add(itemToResponse);
            }

            foreach (var item in trainees)
            {
                var itemToResponse = _mapper.Map<AccountResponse>(item);
                itemToResponse.Role = await getRoleName(item.RoleId);
                itemToResponse.AccountId = item.TraineeId;
                totalAccount.Add(itemToResponse);
            }

            foreach (var item in companies)
            {
                var itemToResponse = _mapper.Map<AccountResponse>(item);
                itemToResponse.Role = await getRoleName(item.RoleId);
                itemToResponse.AccountId = item.CompanyId;
                totalAccount.Add(itemToResponse);
            }

            return totalAccount;
        }

        /// <summary>
        /// get all account in 5 role
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        public async Task<Tuple<int, IEnumerable<AccountResponse>>> GetAccountList(PaginationParameter paginationParameter)
        {
            IEnumerable<Administrator> administrators = _dataContext.Administrators.ToList().Where(t
                 => t.Email.ToUpper().Contains(paginationParameter.MyProperty.ToUpper()));
            IEnumerable<Admin> admins = _dataContext.Admins.ToList().Where(t
                 => t.Email.ToUpper().Contains(paginationParameter.MyProperty.ToUpper()));
            IEnumerable<Trainer> trainers = _dataContext.Trainers.ToList().Where(t
                 => t.Email.ToUpper().Contains(paginationParameter.MyProperty.ToUpper()));
            IEnumerable<Trainee> trainees = _dataContext.Trainees.ToList().Where(t
                 => t.Email.ToUpper().Contains(paginationParameter.MyProperty.ToUpper()));
            IEnumerable<Company> companies = _dataContext.Companies.ToList().Where(t
                 => t.Email.ToUpper().Contains(paginationParameter.MyProperty.ToUpper()));

            IEnumerable<AccountResponse> totalAccount = await addAccountToTotalList(administrators, admins, trainers, trainees, companies);


            return Tuple.Create(totalAccount.Count(), PaginationHelper.GetPage(totalAccount,
                paginationParameter.PageSize, paginationParameter.PageNumber));
        }

        /// <summary>
        /// get account  in 5 role by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<AccountResponse> GetAccountByUsername(string username)
        {
            AccountResponse accountToReponse;

            Administrator administrator = await _administratorService.GetAdministratorByUsername(username);
            if (administrator != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(administrator);
                accountToReponse.Role = await getRoleName(administrator.RoleId);
                accountToReponse.AccountId = administrator.AdministratorId;
                return accountToReponse;
            }

            Company company = await _companyService.GetCompanyByUsername(username);
            if (company != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(company);
                accountToReponse.Role = await getRoleName(company.RoleId);
                accountToReponse.AccountId = company.CompanyId;
                return accountToReponse;
            }

            Trainer trainer = await _trainerService.GetTrainerByUsername(username);
            if (trainer != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(trainer);
                accountToReponse.Role = await getRoleName(trainer.RoleId);
                accountToReponse.AccountId = trainer.TrainerId;
                return accountToReponse;
            }

            Trainee trainee = await _traineeService.GetTraineeByUsername(username);
            if (trainee != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(trainee);
                accountToReponse.Role = await getRoleName(trainee.RoleId);
                accountToReponse.AccountId = trainee.TraineeId;
                return accountToReponse;
            }

            Admin admin = await _adminService.GetAdminByUsername(username);
            if (admin != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(admin);
                accountToReponse.Role = await getRoleName(admin.RoleId);
                accountToReponse.AccountId = admin.AdminId;
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
            if (administrator != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(administrator);
                accountToReponse.Role = await getRoleName(administrator.RoleId);
                accountToReponse.AccountId = administrator.AdministratorId;
                password = administrator.Password;
                return Tuple.Create(accountToReponse, password);
            }

            Company company = await _companyService.GetCompanyByEmail(email);
            if (company != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(company);
                accountToReponse.Role = await getRoleName(company.RoleId);
                accountToReponse.AccountId = company.CompanyId;
                password = company.Password;
                return Tuple.Create(accountToReponse, password);
            }

            Trainer trainer = await _trainerService.GetTrainerByEmail(email);
            if (trainer != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(trainer);
                accountToReponse.Role = await getRoleName(trainer.RoleId);
                accountToReponse.AccountId = trainer.TrainerId;
                password = trainer.Password;
                return Tuple.Create(accountToReponse, password);
            }

            Trainee trainee = await _traineeService.GetTraineeByEmail(email);
            if (trainee != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(trainee);
                accountToReponse.Role = await getRoleName(trainee.RoleId);
                accountToReponse.AccountId = trainee.TraineeId;
                password = trainee.Password;
                return Tuple.Create(accountToReponse, password);
            }

            Admin admin = await _adminService.GetAdminByEmail(email);
            if (admin != null)
            {
                accountToReponse = _mapper.Map<AccountResponse>(admin);
                accountToReponse.Role = await getRoleName(admin.RoleId);
                accountToReponse.AccountId = admin.AdminId;
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

        async private Task<string> processPasswordAndSendEmail(string email)
        {
            string password = AutoGeneratorPassword.passwordGenerator(15, 5, 5, 5);

            EmailContent emailContent = new EmailContent();
            emailContent.IsBodyHtml = true;
            emailContent.ToEmail = email;
            emailContent.Subject = "Your Password";
            emailContent.Body = password;

            await _emailService.SendEmailAsync(emailContent);

            password = BCrypt.Net.BCrypt.HashPassword(password);
            return password;
        }

        /// <summary>
        /// Insert new account method
        /// </summary>
        /// <param name="accountInput"></param>
        /// <returns>-1:existed / 0:fail / 1:success</returns>
        public async Task<int> InsertNewAccount(AccountInput accountInput)
        {
            if(await GetAccountByEmail(accountInput.Email) != null )
            {
                return -1;
            }

            if(await GetAccountByUsername(accountInput.Username) != null )
            {
                return -1;
            }

            accountInput.Role = accountInput.Role.ToLower();
            int rowInserted = 0;
            switch (accountInput.Role)
            {
                case "administrator":
                    break;
                case "admin":
                    var adminToAdd = _mapper.Map<Admin>(accountInput);
                    adminToAdd.RoleId = 2;
                    adminToAdd.Password = await processPasswordAndSendEmail(accountInput.Email);
                    rowInserted = await _adminService.InsertNewAdmin(adminToAdd);
                    break;
                case "trainer":
                    var trainerToAdd = _mapper.Map<Trainer>(accountInput);
                    trainerToAdd.RoleId = 3;
                    trainerToAdd.Password = await processPasswordAndSendEmail(accountInput.Email);
                    rowInserted = await _trainerService.InsertNewTrainer(trainerToAdd);
                    break;
                case "trainee":
                    var traineeToAdd = _mapper.Map<Trainee>(accountInput);
                    traineeToAdd.RoleId = 4;
                    traineeToAdd.Password = await processPasswordAndSendEmail(accountInput.Email);
                    rowInserted = await _traineeService.InsertNewTrainee(traineeToAdd);
                    break;
                case "company":
                    var companyToAdd = _mapper.Map<Company>(accountInput);
                    companyToAdd.RoleId = 5;
                    companyToAdd.Password = await processPasswordAndSendEmail(accountInput.Email);
                    rowInserted = await _companyService.InsertNewCompany(companyToAdd);
                    break;
                default:
                    break;
            }

            return rowInserted;
        }

        /// <summary>
        /// get all account which is deactivated in 5 role
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        public async Task<Tuple<int, IEnumerable<AccountResponse>>> GetDeactivatedAccountList(PaginationParameter paginationParameter)
        {
            IEnumerable<Administrator> administrators = _dataContext.Administrators.ToList().Where(t =>
                 t.Email.ToUpper().Contains(paginationParameter.MyProperty.ToUpper())); ;
            IEnumerable<Admin> admins = _dataContext.Admins.ToList().Where(t =>
                 t.IsDeactivated == true && t.Email.ToUpper().Contains(paginationParameter.MyProperty.ToUpper()));
            IEnumerable<Trainer> trainers = _dataContext.Trainers.ToList().Where(t =>
                 t.IsDeactivated == true && t.Email.ToUpper().Contains(paginationParameter.MyProperty.ToUpper()));
            IEnumerable<Trainee> trainees = _dataContext.Trainees.ToList().Where(t =>
                 t.IsDeactivated == true && t.Email.ToUpper().Contains(paginationParameter.MyProperty.ToUpper()));
            IEnumerable<Company> companies = _dataContext.Companies.ToList().Where(t =>
                 t.IsDeactivated == true && t.Email.ToUpper().Contains(paginationParameter.MyProperty.ToUpper()));

            IEnumerable<AccountResponse> totalAccount = await addAccountToTotalList(administrators, admins, trainers, trainees, companies);

            return Tuple.Create(totalAccount.Count(), PaginationHelper.GetPage(totalAccount,
                paginationParameter.PageSize, paginationParameter.PageNumber));
        }

        public async Task<int> UpdateAccountPassword(string email, string password)
        {
            return 0;
        }
    }
}