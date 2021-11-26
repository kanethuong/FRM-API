using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private string defaultAvatar = "https://i.ibb.co/kxHf8Yv/default-avatar.png";

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

        private async Task<IEnumerable<AccountResponse>> addAccountToTotalList(
        IEnumerable<Admin> admins, IEnumerable<Trainer> trainers, IEnumerable<Trainee> trainees, IEnumerable<Company> companies)
        {
            List<AccountResponse> totalAccount = new List<AccountResponse>();

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
            totalAccount.Sort((x, y) => y.CreatedAt.CompareTo(x.CreatedAt));
            return totalAccount;
        }
        private string ConvertToUnsign(string str)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = str.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty)
                        .Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        /// <summary>
        /// get all account in 5 role
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        public async Task<Tuple<int, IEnumerable<AccountResponse>>> GetAccountList(PaginationParameter paginationParameter)
        {
            string searchName = paginationParameter.SearchName;
            searchName = searchName.Replace(":*|", " ").Replace(":*", "");
            searchName = ConvertToUnsign(searchName);

            IEnumerable<Admin> admins = _dataContext.Admins.ToList().Where(t
                 => t.IsDeactivated == false && (t.Email.ToUpper().Contains(searchName.ToUpper())
                || t.Username.ToUpper().Contains(searchName.ToUpper())
                || ConvertToUnsign(t.Fullname).ToUpper().Contains(searchName.ToUpper())));
            IEnumerable<Trainer> trainers = _dataContext.Trainers.ToList().Where(t
                 => t.IsDeactivated == false && (t.Email.ToUpper().Contains(searchName.ToUpper())
                || t.Username.ToUpper().Contains(searchName.ToUpper())
                || ConvertToUnsign(t.Fullname).ToUpper().Contains(searchName.ToUpper())));
            IEnumerable<Trainee> trainees = _dataContext.Trainees.ToList().Where(t
                 => t.IsDeactivated == false && (t.Email.ToUpper().Contains(searchName.ToUpper())
                || t.Username.ToUpper().Contains(searchName.ToUpper())
                || ConvertToUnsign(t.Fullname).ToUpper().Contains(searchName.ToUpper())));
            IEnumerable<Company> companies = _dataContext.Companies.ToList().Where(t
                 => t.IsDeactivated == false && (t.Email.ToUpper().Contains(searchName.ToUpper())
                || t.Username.ToUpper().Contains(searchName.ToUpper())
                || ConvertToUnsign(t.Fullname).ToUpper().Contains(searchName.ToUpper())));

            IEnumerable<AccountResponse> totalAccount = await addAccountToTotalList(admins, trainers, trainees, companies);


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

        private void sendEmail(string email, string password)
        {
            EmailContent emailContent = new EmailContent();
            emailContent.IsBodyHtml = true;
            emailContent.ToEmail = email;
            emailContent.Subject = "[Kronii] Your password";
            emailContent.Body = password;
            _emailService.SendEmailAsync(emailContent);
        }

        private string processPasswordAndSendEmail(string email)
        {
            string password = AutoGeneratorPassword.passwordGenerator(15, 5, 5, 5);

            sendEmail(email, password);
            password = BCrypt.Net.BCrypt.HashPassword(password);

            return password;
        }

        /// <summary>
        /// Insert new account method (remember to call SaveChange to insert to database)
        /// </summary>
        /// <param name="accountInput"></param>
        /// <returns>-1:existed  / 1:success</returns>
        public async Task<int> InsertNewAccount(AccountInput accountInput)
        {
            if (await GetAccountByEmail(accountInput.Email) != null)
            {
                return -1;
            }

            if (await GetAccountByUsername(accountInput.Username) != null)
            {
                return -1;
            }

            accountInput.Role = accountInput.Role.ToLower();
            bool insertedStatus = false;                             // true if success, false if duplicate
            switch (accountInput.Role)
            {
                case "administrator":
                    var adminstratorToAdd = _mapper.Map<Administrator>(accountInput);
                    adminstratorToAdd.RoleId = 1;

                    adminstratorToAdd.Password = processPasswordAndSendEmail(accountInput.Email);

                    if (_dataContext.Administrators.Any(a =>
                        a.Username.Equals(adminstratorToAdd.Username) ||
                        a.Email.Equals(adminstratorToAdd.Email)
                    ))
                    {
                        return -1;
                    }
                    adminstratorToAdd.AvatarURL = defaultAvatar;
                    _dataContext.Administrators.Add(adminstratorToAdd);
                    insertedStatus = true;
                    break;
                case "admin":
                    var adminToAdd = _mapper.Map<Admin>(accountInput);
                    adminToAdd.RoleId = 2;
                    adminToAdd.Password = processPasswordAndSendEmail(accountInput.Email);
                    adminToAdd.AvatarURL = defaultAvatar;
                    insertedStatus = _adminService.InsertNewAdminNoSaveChange(adminToAdd);
                    break;
                case "trainer":
                    var trainerToAdd = _mapper.Map<Trainer>(accountInput);
                    trainerToAdd.RoleId = 3;
                    trainerToAdd.Password = processPasswordAndSendEmail(accountInput.Email);
                    trainerToAdd.AvatarURL = defaultAvatar;
                    insertedStatus = _trainerService.InsertNewTrainerNoSaveChange(trainerToAdd);
                    break;
                case "trainee":
                    var traineeToAdd = _mapper.Map<Trainee>(accountInput);
                    traineeToAdd.RoleId = 4;
                    traineeToAdd.Password = processPasswordAndSendEmail(accountInput.Email);
                    traineeToAdd.AvatarURL = defaultAvatar;
                    insertedStatus = _traineeService.InsertNewTraineeNoSaveChange(traineeToAdd);
                    break;
                case "company":
                    var companyToAdd = _mapper.Map<Company>(accountInput);
                    companyToAdd.RoleId = 5;
                    companyToAdd.Password = processPasswordAndSendEmail(accountInput.Email);
                    companyToAdd.AvatarURL = defaultAvatar;
                    insertedStatus = _companyService.InsertNewCompanyNoSaveChange(companyToAdd);
                    break;
                default:
                    break;
            }

            if (insertedStatus)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Insert a list of accounts to the database
        /// </summary>
        /// <param name="accountInputs">the list of accounts</param>
        /// <returns>
        /// A dictionary contains failed accounts with error code
        /// - Invalid Email : 0
        /// - Account Existed : -1
        /// - Invalid Role : 1
        /// </returns>
        /// <exception cref="System.Data.DataException">If there is an error when saving data</exception>
        public async Task<IDictionary<int, int>> InsertNewAccount(IList<AccountInput> accountInputs)
        {
            Dictionary<int, int> failAccounts = new();

            Dictionary<string, string> passwordByEmail = new(); // email key, password value
            string passwordBeforeHash;

            for (int processingIndex = 0; processingIndex < accountInputs.Count; processingIndex++)
            {
                var account = accountInputs[processingIndex];
                //switch and add to context (noSaveChange) base on role
                switch (account.Role.ToLower())
                {
                    case "administrator":
                        var adminstratorToAdd = _mapper.Map<Administrator>(account);
                        adminstratorToAdd.RoleId = 1;

                        passwordBeforeHash = AutoGeneratorPassword.passwordGenerator(15, 5, 5, 5);
                        try
                        {
                            passwordByEmail.Add(account.Email, passwordBeforeHash);
                        }
                        catch (ArgumentException)
                        {
                            failAccounts.Add(processingIndex, 0);
                            break;
                        }

                        adminstratorToAdd.Password = BCrypt.Net.BCrypt.HashPassword(passwordBeforeHash);

                        if (_dataContext.Administrators.Any(a =>
                            a.Username.Equals(adminstratorToAdd.Username) ||
                            a.Email.Equals(adminstratorToAdd.Email)
                        ))
                        {
                            failAccounts.Add(processingIndex, -1);
                            break;
                        }
                        adminstratorToAdd.AvatarURL = defaultAvatar;
                        _dataContext.Administrators.Add(adminstratorToAdd);
                        break;
                    case "admin":
                        var adminToAdd = _mapper.Map<Admin>(account);
                        adminToAdd.RoleId = 2;
                        adminToAdd.AvatarURL = defaultAvatar;
                        passwordBeforeHash = AutoGeneratorPassword.passwordGenerator(15, 5, 5, 5);
                        try
                        {
                            passwordByEmail.Add(account.Email, passwordBeforeHash);
                        }
                        catch (ArgumentException)
                        {
                            failAccounts.Add(processingIndex, 0);
                            break;
                        }

                        adminToAdd.Password = BCrypt.Net.BCrypt.HashPassword(passwordBeforeHash);
                        if (!_adminService.InsertNewAdminNoSaveChange(adminToAdd))
                        {
                            failAccounts.Add(processingIndex, -1);
                            break;
                        }
                        break;
                    case "trainer":
                        var trainerToAdd = _mapper.Map<Trainer>(account);
                        trainerToAdd.RoleId = 3;
                        trainerToAdd.AvatarURL = defaultAvatar;
                        passwordBeforeHash = AutoGeneratorPassword.passwordGenerator(15, 5, 5, 5);
                        try
                        {
                            passwordByEmail.Add(account.Email, passwordBeforeHash);
                        }
                        catch (ArgumentException)
                        {
                            failAccounts.Add(processingIndex, 0);
                            break;
                        }

                        trainerToAdd.Password = BCrypt.Net.BCrypt.HashPassword(passwordBeforeHash);

                        if (!_trainerService.InsertNewTrainerNoSaveChange(trainerToAdd))
                        {
                            failAccounts.Add(processingIndex, -1);
                            break;
                        }
                        break;
                    case "trainee":
                        var traineeToAdd = _mapper.Map<Trainee>(account);
                        traineeToAdd.RoleId = 4;
                        traineeToAdd.AvatarURL = defaultAvatar;
                        passwordBeforeHash = AutoGeneratorPassword.passwordGenerator(15, 5, 5, 5);
                        try
                        {
                            passwordByEmail.Add(account.Email, passwordBeforeHash);
                        }
                        catch (ArgumentException)
                        {
                            failAccounts.Add(processingIndex, 0);
                            break;
                        }

                        traineeToAdd.Password = BCrypt.Net.BCrypt.HashPassword(passwordBeforeHash);

                        if (!_traineeService.InsertNewTraineeNoSaveChange(traineeToAdd))
                        {
                            failAccounts.Add(processingIndex, -1);
                            break;
                        }
                        break;
                    case "company":
                        var companyToAdd = _mapper.Map<Company>(account);
                        companyToAdd.RoleId = 5;
                        companyToAdd.AvatarURL = defaultAvatar;
                        passwordBeforeHash = AutoGeneratorPassword.passwordGenerator(15, 5, 5, 5);
                        try
                        {
                            passwordByEmail.Add(account.Email, passwordBeforeHash);
                        }
                        catch (ArgumentException)
                        {
                            failAccounts.Add(processingIndex, 0);
                            break;
                        }

                        companyToAdd.Password = BCrypt.Net.BCrypt.HashPassword(passwordBeforeHash);

                        if (!_companyService.InsertNewCompanyNoSaveChange(companyToAdd))
                        {
                            failAccounts.Add(processingIndex, -1);
                            break;
                        }
                        break;
                    default:
                        failAccounts.Add(processingIndex, 1);
                        break;
                }
            }

            // Can throw exception, catch in Controller
            int rowInserted = await _dataContext.SaveChangesAsync();

            foreach (var account in passwordByEmail)
            {
                sendEmail(account.Key, account.Value);
            }

            return failAccounts;
        }

        /// <summary>
        /// save change to database
        /// </summary>
        /// <returns>number of row effeted</returns>
        public async Task<int> SaveChange()
        {
            return await _dataContext.SaveChangesAsync();
            //_dataContext.ChangeTracker.Clear();
        }

        /// <summary>
        /// discard all change
        /// </summary>
        public void DiscardChanges()
        {
            _dataContext.ChangeTracker.Clear();
        }

        /// <summary>
        /// get all account which is deactivated in 5 role
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        public async Task<Tuple<int, IEnumerable<DeletedAccountResponse>>> GetDeactivatedAccountList(PaginationParameter paginationParameter)
        {
            string searchName = paginationParameter.SearchName;
            searchName = searchName.Replace(":*|", " ").Replace(":*", "");
            searchName = ConvertToUnsign(searchName);

            IEnumerable<Admin> admins = _dataContext.Admins.ToList().Where(t
                 => t.IsDeactivated == true && (t.Email.ToUpper().Contains(searchName.ToUpper())
                || t.Username.ToUpper().Contains(searchName.ToUpper())
                || ConvertToUnsign(t.Fullname).ToUpper().Contains(searchName.ToUpper())));
            IEnumerable<Trainer> trainers = _dataContext.Trainers.ToList().Where(t
                 => t.IsDeactivated == true && (t.Email.ToUpper().Contains(searchName.ToUpper())
                || t.Username.ToUpper().Contains(searchName.ToUpper())
                || ConvertToUnsign(t.Fullname).ToUpper().Contains(searchName.ToUpper())));
            IEnumerable<Trainee> trainees = _dataContext.Trainees.ToList().Where(t
                 => t.IsDeactivated == true && (t.Email.ToUpper().Contains(searchName.ToUpper())
                || t.Username.ToUpper().Contains(searchName.ToUpper())
                || ConvertToUnsign(t.Fullname).ToUpper().Contains(searchName.ToUpper())));
            IEnumerable<Company> companies = _dataContext.Companies.ToList().Where(t
                 => t.IsDeactivated == true && (t.Email.ToUpper().Contains(searchName.ToUpper())
                || t.Username.ToUpper().Contains(searchName.ToUpper())
                || ConvertToUnsign(t.Fullname).ToUpper().Contains(searchName.ToUpper())));

            List<DeletedAccountResponse> totalAccount = new List<DeletedAccountResponse>();

            foreach (var item in admins)
            {
                var itemToResponse = _mapper.Map<DeletedAccountResponse>(item);
                itemToResponse.Role = await getRoleName(item.RoleId);
                totalAccount.Add(itemToResponse);
            }

            foreach (var item in trainers)
            {
                var itemToResponse = _mapper.Map<DeletedAccountResponse>(item);
                itemToResponse.Role = await getRoleName(item.RoleId);
                totalAccount.Add(itemToResponse);
            }

            foreach (var item in trainees)
            {
                var itemToResponse = _mapper.Map<DeletedAccountResponse>(item);
                itemToResponse.Role = await getRoleName(item.RoleId);
                totalAccount.Add(itemToResponse);
            }

            foreach (var item in companies)
            {
                var itemToResponse = _mapper.Map<DeletedAccountResponse>(item);
                itemToResponse.Role = await getRoleName(item.RoleId);
                totalAccount.Add(itemToResponse);
            }
            totalAccount.Sort((x, y) => y.DeactivatedAt.CompareTo(x.DeactivatedAt));

            return Tuple.Create(totalAccount.Count(), PaginationHelper.GetPage(totalAccount,
                paginationParameter.PageSize, paginationParameter.PageNumber));
        }

        public async Task<int> UpdateAccountPassword(string email, string password)
        {
            Tuple<AccountResponse, string> tupleResponse = await GetAccountByEmail(email);
            if (tupleResponse == null)
            {
                return 0;
            }
            AccountResponse account = tupleResponse.Item1;
            if (password == null || password == "" || account.Role == "Administrator")
            {
                return 0;
            }

            EmailContent emailContent = new EmailContent();
            emailContent.IsBodyHtml = true;
            emailContent.ToEmail = email;
            emailContent.Subject = "[Kronii] Your password was reset";
            emailContent.Body = password;

            await _emailService.SendEmailAsync(emailContent);

            password = BCrypt.Net.BCrypt.HashPassword(password);

            int rowInserted = 0;
            switch (account.Role.ToLower())
            {
                case "admin":
                    var admin = await _adminService.GetAdminByEmail(email);
                    admin.Password = password;
                    rowInserted = await _dataContext.SaveChangesAsync();
                    break;
                case "trainer":
                    var trainer = await _trainerService.GetTrainerByEmail(email);
                    trainer.Password = password;
                    rowInserted = await _dataContext.SaveChangesAsync();
                    break;
                case "trainee":
                    var trainee = await _traineeService.GetTraineeByEmail(email);
                    trainee.Password = password;
                    rowInserted = await _dataContext.SaveChangesAsync();
                    break;
                case "company":
                    var company = await _companyService.GetCompanyByEmail(email);
                    company.Password = password;
                    rowInserted = await _dataContext.SaveChangesAsync();
                    break;
                default:
                    return 0;
            }
            return rowInserted;
        }
    }
}