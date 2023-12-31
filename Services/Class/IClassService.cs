using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.PaginationDTO;

namespace kroniiapi.Services
{
    public interface IClassService
    {
        Task<Tuple<int, IEnumerable<Class>>> GetClassList(PaginationParameter paginationParameter);
        Task<Tuple<int, IEnumerable<DeleteClassRequest>>> GetRequestDeleteClassList(PaginationParameter paginationParameter);
        Task<int> UpdateDeletedClass(ConfirmDeleteClassInput confirmDeleteClassInput, int deleteClassRequestId);
        Task<Tuple<int, IEnumerable<Class>>> GetDeletedClassList(PaginationParameter paginationParameter);
        Task<Class> GetClassByClassName(string className);
        Task<Class> GetClassDetail(int id);
        Task<Tuple<int, IEnumerable<Trainee>>> GetTraineesByClassId(int id, PaginationParameter paginationParameter);
        Task<Tuple<int, IEnumerable<Trainee>>> GetTraineesByClassIdOfTrainer(int id, PaginationParameter paginationParameter);
        Task<int> InsertNewRequestDeleteClass(DeleteClassRequest deleteClassRequest);
        Task AddClassIdToTrainee(int classId, ICollection<int> traineeIdList);
        Task AddDataToClassModule(int classId, ICollection<TrainerModule> moduleIdList);
        Task<int> InsertNewClass(NewClassInput newClass);
        Task<int> InsertNewClassNoSave(NewClassInput newClass);
        Task<int> RejectAllOtherDeleteRequest(int deleteRequestId);
        Task<Class> GetClassByClassID(int classId);
        Task<int> SaveChange();
        Task<FeedbackViewForTrainee> GetFeedbackViewForTrainee(int traineeId);
        Task<int> DeleteTraineeClass(int deleteRequestId);
        void DiscardChanges();
        Task<int> RemoveModuleFromClass(int classId, int moduleId);
        Task<Tuple<int, IEnumerable<Class>>> GetClassListByTrainerId(int trainerId, PaginationParameter paginationParameter);
        bool CheckClassExist(int id);
        Task<ClassModule> GetClassModule(int classId, int moduleId);
        Task<int> AssignModuleToClass(ClassModule classModule);
        Task<ICollection<Class>> GetClassListByAdminId(int adminId, DateTime at = default(DateTime));
    }
}