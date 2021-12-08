using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Helper;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class ClassService : IClassService
    {
        private DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly ITraineeService _traineeService;
        private readonly ITimetableService _timetableService;
        public ClassService(DataContext dataContext,
                            IMapper mapper,
                            ITraineeService traineeService,
                            ITimetableService timetableService
        )
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _traineeService = traineeService;
            _timetableService = timetableService;
        }
        /// <summary>
        /// Get Class List
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns> Tuple List of Class List </returns>
        public async Task<Tuple<int, IEnumerable<Class>>> GetClassList(PaginationParameter paginationParameter)
        {
            IQueryable<Class> classList = _dataContext.Classes.Where(c => c.IsDeactivated == false).Select(c => new Class
            {
                ClassId = c.ClassId,
                ClassName = c.ClassName,
                Admin = new Admin
                {
                    Fullname = c.Admin.Fullname
                },
                CreatedAt = c.CreatedAt,
                Description = c.Description,
                ClassModules = c.ClassModules
            });
            if (paginationParameter.SearchName != "")
            {
                classList = classList.Where(e => EF.Functions.ToTsVector("simple", EF.Functions.Unaccent(e.ClassName.ToLower()))
                    .Matches(EF.Functions.ToTsQuery("simple", EF.Functions.Unaccent(paginationParameter.SearchName.ToLower()))));
            }
            IEnumerable<Class> rs = await classList
                .GetCount(out var totalRecords)
                .OrderByDescending(e => e.CreatedAt)
                .GetPage(paginationParameter)
                .ToListAsync();
            return Tuple.Create(totalRecords, rs);
        }

        /// <summary>
        /// Get Class By ClassName
        /// </summary>
        /// <param name="className"></param>
        /// <returns> Class </returns>
        public async Task<Class> GetClassByClassName(string className)
        {
            return await _dataContext.Classes.Where(c => c.ClassName == className).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get Request Deleted Class List
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns>Tuple List of  Class Delete Request</returns>
        public async Task<Tuple<int, IEnumerable<DeleteClassRequest>>> GetRequestDeleteClassList(PaginationParameter paginationParameter)
        {
            IQueryable<DeleteClassRequest> listRequest = _dataContext.DeleteClassRequests
                                    .Where(c => c.IsAccepted == null)
                                    .Select(c => new DeleteClassRequest
                                    {
                                        DeleteClassRequestId = c.DeleteClassRequestId,
                                        Reason = c.Reason,
                                        CreatedAt = c.CreatedAt,
                                        IsAccepted = c.IsAccepted,
                                        AcceptedAt = c.AcceptedAt,
                                        ClassId = c.ClassId,
                                        Class = new Class
                                        {
                                            ClassId = c.ClassId,
                                            ClassName = c.Class.ClassName,
                                            Description = c.Class.Description,
                                            CreatedAt = c.Class.CreatedAt,
                                        },
                                        Admin = new Admin
                                        {
                                            AdminId = c.AdminId,
                                            Username = c.Admin.Username,
                                            Fullname = c.Admin.Fullname,
                                            AvatarURL = c.Admin.AvatarURL,
                                            Email = c.Admin.Email,
                                        }
                                    });
            if (paginationParameter.SearchName != "")
            {
                listRequest = listRequest.Where(e => EF.Functions.ToTsVector("simple", EF.Functions.Unaccent(e.Class.ClassName.ToLower()))
                    .Matches(EF.Functions.ToTsQuery("simple", EF.Functions.Unaccent(paginationParameter.SearchName.ToLower()))));
            }

            IEnumerable<DeleteClassRequest> rs = await listRequest
                .GetCount(out var totalRecords)
                .OrderByDescending(e => e.CreatedAt)
                .GetPage(paginationParameter)
                .ToListAsync();
            return Tuple.Create(totalRecords, rs);
        }

        /// <summary>
        /// Update Deleted Class
        /// </summary>
        /// <param name="confirmDeleteClassInput"></param>
        /// <returns>1 if Success to Change & 0 if false to change & -1 if invalid & 2 if is rejected</returns>
        public async Task<int> UpdateDeletedClass(ConfirmDeleteClassInput confirmDeleteClassInput, int deleteClassRequestId)
        {
            if (confirmDeleteClassInput.IsDeactivate == true)
            {
                var existedClass = await _dataContext.Classes.Where(i => i.ClassId == confirmDeleteClassInput.ClassId).FirstOrDefaultAsync();
                if (existedClass == null)
                {
                    return -1;
                }
                var existedRequest = await _dataContext.DeleteClassRequests.Where(d => d.DeleteClassRequestId == deleteClassRequestId).FirstOrDefaultAsync();
                if (existedRequest == null)
                {
                    return -1;
                }
                if (existedClass.IsDeactivated == true || existedRequest.IsAccepted == true)
                {
                    return 0;
                }
                existedClass.IsDeactivated = true;
                existedClass.DeactivatedAt = DateTime.Now;
                existedRequest.IsAccepted = true;
                existedRequest.AcceptedAt = DateTime.Now;
                // Delete Class Module
                var classModuleList = _dataContext.ClassModules.Where(cm => cm.ClassId == confirmDeleteClassInput.ClassId).ToList();
                _dataContext.ClassModules.RemoveRange(classModuleList);
                // Delete Calendars and Attendance
                var calendars = _dataContext.Calendars.Where(cl => cl.ClassId == confirmDeleteClassInput.ClassId).ToList();
                _dataContext.Calendars.RemoveRange(calendars);
                var attendances = _dataContext.Attendances.Where(at => at.Trainee.ClassId == confirmDeleteClassInput.ClassId).ToList();
                _dataContext.Attendances.RemoveRange(attendances);
                // Save Change 
                await _dataContext.SaveChangesAsync();
                return 1;
            }
            else if (confirmDeleteClassInput.IsDeactivate == false)
            {
                var existedRequest = await _dataContext.DeleteClassRequests.Where(d => d.DeleteClassRequestId == deleteClassRequestId).FirstOrDefaultAsync();
                existedRequest.IsAccepted = false;
                await _dataContext.SaveChangesAsync();
                return 2;
            }
            return -1;
        }

        public async Task<int> DeleteTraineeClass(int deleteClassId)
        {
            var traineeList = await _dataContext.Trainees.Where(c => c.ClassId == deleteClassId).ToListAsync();
            foreach (var item in traineeList)
            {
                item.ClassId = null;
            }
            int status = await _dataContext.SaveChangesAsync();
            return status;
        }

        public async Task<int> RejectAllOtherDeleteRequest(int deleteRequestId)
        {
            int classId = await _dataContext.DeleteClassRequests.Where(t => t.DeleteClassRequestId == deleteRequestId)
            .Select(t => t.ClassId).FirstOrDefaultAsync();
            var listRequest = await _dataContext.DeleteClassRequests.Where(t => t.ClassId == classId).ToListAsync();
            foreach (var i in listRequest)
            {
                i.IsAccepted = false;
            };
            var currentReq = await _dataContext.DeleteClassRequests.Where(t => t.DeleteClassRequestId == deleteRequestId).FirstOrDefaultAsync();
            currentReq.IsAccepted = true;
            int rs = await _dataContext.SaveChangesAsync();
            return rs;
        }

        /// <summary>
        ///  Get Deleted Class List
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns> Tuple List of Deleted Class </returns>
        public async Task<Tuple<int, IEnumerable<Class>>> GetDeletedClassList(PaginationParameter paginationParameter)
        {
            IQueryable<Class> classes = _dataContext.Classes.Where(c => c.IsDeactivated == true);
            if (paginationParameter.SearchName != "")
            {
                classes = classes.Where(c => EF.Functions.ToTsVector("simple", EF.Functions.Unaccent(c.ClassName.ToLower()))
                    .Matches(EF.Functions.ToTsQuery("simple", EF.Functions.Unaccent(paginationParameter.SearchName.ToLower()))));
            }

            IEnumerable<Class> rs = await classes
                .GetCount(out var totalRecords)
                .OrderByDescending(e => e.CreatedAt)
                .GetPage(paginationParameter)
                .ToListAsync();

            return Tuple.Create(totalRecords, rs);
        }

        /// <summary>
        /// Get detail of a class 
        /// </summary>
        /// <param name="id">id of the class</param>
        /// <returns>if found return class and if not return 0</returns>
        public async Task<Class> GetClassDetail(int id)
        {
            var classGet = await _dataContext.Classes.Where(c => c.ClassId == id && c.IsDeactivated == false)
            .Select(c => new Class
            {
                ClassId = c.ClassId,
                ClassName = c.ClassName,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                StartDay = c.StartDay,
                EndDay = c.EndDay,
                IsDeactivated = c.IsDeactivated,
                DeactivatedAt = c.DeactivatedAt,
                Trainees = c.Trainees,
                AdminId = c.AdminId,
                Admin = new Admin
                {
                    AdminId = c.AdminId,
                    Fullname = c.Admin.Fullname,
                    AvatarURL = c.Admin.AvatarURL,
                    Email = c.Admin.Email,
                },
                // TrainerId = c.TrainerId,
                // Trainer = new Trainer
                // {
                //     Fullname = c.Trainer.Fullname,
                //     AvatarURL = c.Trainer.AvatarURL,
                //     Email = c.Trainer.Email,
                // },
                // RoomId = c.RoomId,
                // Room = new Room
                // {
                //     RoomId = c.Room.RoomId,
                //     RoomName = c.Room.RoomName,
                //     Classes = c.Room.Classes,
                // },
                //ClassModules = c.ClassModules,
                //Modules = c.Modules,
                DeleteClassRequests = c.DeleteClassRequests,
                Calendars = c.Calendars,
            })
            .FirstOrDefaultAsync();

            return classGet;
        }

        /// <summary>
        /// Get Trainee List in a class with pagination
        /// </summary>
        /// <param name="id">id of the class</param>
        /// <param name="paginationParameter">pagination param to get approriate trainee in a page</param>
        /// <returns>tuple list of trainee</returns>
        public async Task<Tuple<int, IEnumerable<Trainee>>> GetTraineesByClassId(int id, PaginationParameter paginationParameter)
        {
            IQueryable<Trainee> traineeList = _dataContext.Trainees.Where(t => t.ClassId == id && t.IsDeactivated == false);
            if (paginationParameter.SearchName != "")
            {
                traineeList = traineeList.Where(c => EF.Functions.ToTsVector("simple", EF.Functions.Unaccent(c.Fullname.ToLower())
                                                                                        + " "
                                                                                        + EF.Functions.Unaccent(c.Username.ToLower())
                                                                                        + " "
                                                                                        + EF.Functions.Unaccent(c.Email.ToLower()))
                    .Matches(EF.Functions.ToTsQuery("simple", EF.Functions.Unaccent(paginationParameter.SearchName.ToLower()))));
            }
            IEnumerable<Trainee> rs = await traineeList
                .GetCount(out var totalRecords)
                .OrderBy(e => e.Fullname)
                .GetPage(paginationParameter)
                .ToListAsync();
            return Tuple.Create(totalRecords, rs);
        }

        /// <summary>
        /// Get Trainee List in a class with pagination
        /// </summary>
        /// <param name="id">id of the class</param>
        /// <param name="paginationParameter">pagination param to get approriate trainee in a page</param>
        /// <returns>tuple list of trainee</returns>
        public async Task<Tuple<int, IEnumerable<Trainee>>> GetTraineesByClassIdOfTrainer(int id, PaginationParameter paginationParameter)
        {
            IQueryable<Trainee> traineeList = _dataContext.Trainees.Where(t => t.ClassId == id && t.IsDeactivated == false);
            if (paginationParameter.SearchName != "")
            {
                traineeList = traineeList.Where(c => EF.Functions.ToTsVector("simple", EF.Functions.Unaccent(c.Fullname.ToLower())
                                                                                        + " "
                                                                                        + EF.Functions.Unaccent(c.Username.ToLower())
                                                                                        + " "
                                                                                        + EF.Functions.Unaccent(c.Email.ToLower()))
                    .Matches(EF.Functions.ToTsQuery("simple", EF.Functions.Unaccent(paginationParameter.SearchName.ToLower()))));
            }
            IEnumerable<Trainee> rs = await traineeList
                .GetCount(out var totalRecords)
                .OrderBy(e => e.Fullname)
                .ToListAsync();
            return Tuple.Create(totalRecords, rs);
        }

        /// <summary>
        /// Insert New Request Delete Class to db
        /// </summary>
        /// <param name="requestDeleteClassInput"></param>
        /// <returns> -2: Admin is not exist / -1: Class is not exist / 0: Insert fail / 1: Insert success </returns>
        public async Task<int> InsertNewRequestDeleteClass(DeleteClassRequest deleteClassRequest)
        {
            var isAdminExist = _dataContext.Admins.Any(a => a.AdminId == deleteClassRequest.AdminId && a.IsDeactivated == false);
            var isClassExist = _dataContext.Classes.Any(c => c.ClassId == deleteClassRequest.ClassId && c.IsDeactivated == false);

            if (isAdminExist == false)
            {
                return -2;
            }
            else if (isClassExist == false)
            {
                return -1;
            }

            int rowInserted = 0;
            _dataContext.DeleteClassRequests.Add(deleteClassRequest);
            rowInserted = await _dataContext.SaveChangesAsync();
            return rowInserted;
        }

        /// <summary>
        /// Get Class By ClassID
        /// </summary>
        /// <param name="classID"></param>
        /// <returns> Class </returns>
        public async Task<Class> GetClassByClassID(int classId)
        {
            return await _dataContext.Classes.Where(c => c.ClassId == classId && c.IsDeactivated == false)
                                            .Select(c => new Class
                                            {
                                                ClassId = c.ClassId,
                                                ClassName = c.ClassName,
                                                Description = c.Description,
                                                CreatedAt = c.CreatedAt,
                                                StartDay = c.StartDay,
                                                EndDay = c.EndDay,
                                                IsDeactivated = c.IsDeactivated,
                                                DeactivatedAt = c.DeactivatedAt,
                                                Modules = c.Modules
                                            })
                                            .FirstOrDefaultAsync();
        }

        /// <summary>
        /// add Class Id to Trainee model (after add new class)
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="traineeIdList"></param>
        /// <returns></returns>
        public async Task AddClassIdToTrainee(int classId, ICollection<int> traineeIdList)
        {
            foreach (var traineeId in traineeIdList)
            {
                if (await _traineeService.IsTraineeHasClass(traineeId)) continue;
                var trainee = await _dataContext.Trainees.Where(t => t.TraineeId == traineeId &&
                                                            t.IsDeactivated == false).FirstOrDefaultAsync();
                trainee.ClassId = classId;
            }
        }

        /// <summary>
        /// add module id and class id to class module model (after add new class)
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="moduleIdList"></param>
        public async Task AddDataToClassModule(int classId, ICollection<TrainerModule> trainerModuleList)
        {
            var classGet = await this.GetClassByClassID(classId);
            foreach (var trainerModule in trainerModuleList)
            {
                var startDay = _timetableService.GetStartDayforClassToInsertModule(classId);
                (int roomId, DateTime date) = _timetableService.GetRoomIdAvailableForModule(startDay, classGet.EndDay, trainerModule.ModuleId);
                ClassModule classModule = await _dataContext.ClassModules.Where(cm => cm.ClassId == classId
                                                                                      && cm.ModuleId == trainerModule.ModuleId).FirstOrDefaultAsync();
                if (classModule is not null) continue;
                classModule = new ClassModule()
                {
                    ClassId = classId,
                    ModuleId = trainerModule.ModuleId,
                    TrainerId = trainerModule.TrainerId,
                    WeightNumber = trainerModule.WeightNumber,
                    RoomId = roomId
                };
                _dataContext.ClassModules.Add(classModule);
                _dataContext.SaveChanges();
                int status = await _timetableService.InsertCalendarsToClass(classId, trainerModule.ModuleId);
            }
        }

        /// <summary>
        /// Insert new class and save change
        /// </summary>
        /// <param name="newClassInput">detail of class input</param>
        /// <returns> -1: duplicate class name / -2: trainee already have class / 0: some unpredicted error / 1: insert succesfully </returns>
        public async Task<int> InsertNewClass(NewClassInput newClassInput)
        {
            var classSave = await InsertNewClassNoSave(newClassInput);
            if (classSave == -1)
            {
                return -1;
            }
            else if (classSave == -2)
            {
                return -2;
            }
            int rowInserted = 0;
            rowInserted = await SaveChange();
            if (rowInserted != 0)
            {
                var newClass = await GetClassByClassName(newClassInput.ClassName);
                await AddClassIdToTrainee(newClass.ClassId, newClassInput.TraineeIdList);
                await AddDataToClassModule(newClass.ClassId, newClassInput.TrainerModuleList);
                await SaveChange();
            }
            return rowInserted;
        }

        /// <summary>
        /// Insert class but not saving, check if trainee already have class
        /// </summary>
        /// <param name="newClassInput"></param>
        /// <returns>-1: duplicate class name / -2: trainee already have class / 1: insert succesfully</returns>
        public async Task<int> InsertNewClassNoSave(NewClassInput newClassInput)
        {
            var newClass = _mapper.Map<Class>(newClassInput);

            // Check duplicate class name
            if (_dataContext.Classes.Any(c => c.ClassName.Equals(newClass.ClassName) && c.IsDeactivated == false))
            {
                return -1;
            }

            // Check if trainee input already have class
            var traineeListId = newClassInput.TraineeIdList;
            foreach (var traineeId in traineeListId)
            {
                if (await _traineeService.IsTraineeHasClass(traineeId))
                {
                    return -2;
                }
            }

            _dataContext.Classes.Add(newClass);

            return 1;
        }

        /// <summary>
        /// save change to database
        /// </summary>
        /// <returns>number of row effeted</returns>
        public async Task<int> SaveChange()
        {
            return await _dataContext.SaveChangesAsync();
        }

        /// <summary>
        /// discard all change
        /// </summary>
        public void DiscardChanges()
        {
            _dataContext.ChangeTracker.Clear();
        }

        /// <summary>
        /// Get Trainer and Admin using TraineeId
        /// </summary>
        /// <param name="traineeId"></param>
        /// <returns>FeedbackViewForTrainee</returns>
        public async Task<FeedbackViewForTrainee> GetFeedbackViewForTrainee(int traineeId)
        {
            var traineeToView = await _dataContext.Trainees
                                        .Where(t => t.TraineeId == traineeId)
                                        .Select(c => new Trainee
                                        {
                                            TraineeId = c.TraineeId,
                                            Username = c.Username,
                                            Fullname = c.Fullname,
                                            AvatarURL = c.AvatarURL,
                                            Class = new Class
                                            {
                                                ClassId = c.Class.ClassId,
                                                AdminId = c.Class.Admin.AdminId,
                                                Admin = new Admin
                                                {
                                                    Fullname = c.Class.Admin.Fullname,
                                                    Email = c.Class.Admin.Email,
                                                    AvatarURL = c.Class.Admin.AvatarURL,
                                                },
                                                // TrainerId = c.Class.TrainerId,
                                                // Trainer = new Trainer
                                                // {
                                                //     Fullname = c.Class.Trainer.Fullname,
                                                //     Email = c.Class.Trainer.Email,
                                                //     AvatarURL = c.Class.Trainer.AvatarURL,
                                                // }
                                            }
                                        }).FirstOrDefaultAsync();
            if (traineeToView == null)
            {
                return null;
            }
            var returnThing = new FeedbackViewForTrainee
            {
                trainer = new TrainerInFeedbackResponse
                {
                    // TrainerId = traineeToView.Class.TrainerId,
                    // Fullname = traineeToView.Class.Trainer.Fullname,
                    // Email = traineeToView.Class.Trainer.Email,
                    // AvatarURL = traineeToView.Class.Trainer.AvatarURL
                },
                admin = new AdminInFeedbackResponse
                {
                    AdminId = traineeToView.Class.AdminId,
                    Fullname = traineeToView.Class.Admin.Fullname,
                    Email = traineeToView.Class.Admin.Email,
                    AvatarURL = traineeToView.Class.Admin.AvatarURL
                }
            };
            return returnThing;
        }

        /// <summary>
        /// Remove class module in calendar table
        /// </summary>
        /// <returns>1 if success/ 0 if fail</returns>
        public async Task<int> RemoveClassModuleFromCalendar(int classId, int moduleId)
        {
            IEnumerable<Calendar> listForDelete = _dataContext.Calendars.Where(t => t.ClassId == classId && t.ModuleId == moduleId);
            int numberOfDeleteRecord = listForDelete.Count();
            _dataContext.Calendars.RemoveRange(listForDelete);
            if (await _dataContext.SaveChangesAsync() == numberOfDeleteRecord)
            {
                return 1;
            }
            else
            {
                _dataContext.ChangeTracker.Clear();
                return 0;
            }
        }

        /// <summary>
        /// remove record from table class-module
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="moduleId"></param>
        /// <returns>-1:not found / 0:fail / 1:success</returns>
        public async Task<int> RemoveModuleFromClass(int classId, int moduleId)
        {
            var classModuleForDelete = _dataContext.ClassModules.Where(t => t.ClassId == classId && t.ModuleId == moduleId).FirstOrDefault();
            int deleteFromCalendarStatus = await RemoveClassModuleFromCalendar(classId, moduleId);
            if (deleteFromCalendarStatus == 0)
            {
                return 0;
            }
            if (classModuleForDelete != null)
            {
                _dataContext.ClassModules.RemoveRange(classModuleForDelete);
            }
            else
            {
                return -1;
            }
            return await _dataContext.SaveChangesAsync();
        }

        /// <summary>
        /// Get all module of that class
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="moduleId"></param>
        /// <returns>Module list</returns>
        public async Task<ClassModule> GetClassModule(int classId, int moduleId)
        {
            return await _dataContext.ClassModules.Where(t => t.ClassId == classId && t.ModuleId == moduleId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get Class list of Trainer with pagination
        /// </summary>
        /// <param name="trainerId">id of the class</param>
        /// <param name="paginationParameter">pagination param to get approriate class in a page</param>
        /// <returns>tuple list of classes</returns>
        public async Task<Tuple<int, IEnumerable<Class>>> GetClassListByTrainerId(int trainerId, PaginationParameter paginationParameter)
        {
            IQueryable<Class> result = _dataContext.Classes.Where(c => c.ClassModules.Any(cm => cm.TrainerId == trainerId) && c.IsDeactivated == false).Distinct();
            if (paginationParameter.SearchName != "")
            {
                result = result.Where(e => EF.Functions.ToTsVector("simple", EF.Functions.Unaccent(e.ClassName.ToLower()))
                    .Matches(EF.Functions.ToTsQuery("simple", EF.Functions.Unaccent(paginationParameter.SearchName.ToLower()))));
            }
            IEnumerable<Class> rs = await result
            .GetCount(out var totalRecords)
            .OrderBy(c => c.CreatedAt)
            .GetPage(paginationParameter)
            .Select(c => new Class
            {
                ClassName = c.ClassName,
                ClassId = c.ClassId,
                Description = c.Description,
                ClassModules = c.ClassModules
            })
            .ToListAsync();
            return Tuple.Create(totalRecords, rs);
        }

        /// <summary>
        /// Check there is any class with input id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bool</returns>
        public bool CheckClassExist(int id)
        {
            return _dataContext.Classes.Any(c => c.ClassId == id &&
           c.IsDeactivated == false);
        }

        /// <summary>
        /// Assign a module to class
        /// </summary>
        /// <param name="classModule"></param>
        /// <returns>0: assign fail / 1: assign success</returns>
        public async Task<int> AssignModuleToClass(ClassModule classModule)
        {
            int rowInserted = 0;
            _dataContext.ClassModules.Add(classModule);
            rowInserted = await _dataContext.SaveChangesAsync();
            return rowInserted;
        }

        /// <summary>
        /// Get classes belonged to admin in every year by input
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="at"></param>
        /// <returns>Class list</returns>
        public async Task<ICollection<Class>> GetClassListByAdminId(int adminId, DateTime at = default(DateTime))
        {
            List<Class> classes;
            if (at == default(DateTime)) {
                classes = await _dataContext.Admins.Where(a => a.AdminId == adminId && a.IsDeactivated == false)
                .Select(a => a.Classes
                    .Where(c => c.IsDeactivated == false)
                    .OrderByDescending(c => c.StartDay)
                    .ToList()
                )
                .FirstOrDefaultAsync();
            } else {
                classes = await _dataContext.Admins.Where(a => a.AdminId == adminId && a.IsDeactivated == false)
                .Select(a => a.Classes
                    .Where(c => c.IsDeactivated == false && c.StartDay.Year == at.Year || c.EndDay.Year == at.Year)
                    .OrderByDescending(c => c.StartDay)
                    .ToList()
                )
                .FirstOrDefaultAsync();
            }
            
            return (ICollection<Class>)classes;
        }


    }
}