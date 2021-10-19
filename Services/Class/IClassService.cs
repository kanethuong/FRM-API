using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.DTO.PaginationDTO;

namespace kroniiapi.Services
{
    public interface IClassService
    {
        Task<Tuple<int, IEnumerable<Class>>> GetClassList(PaginationParameter paginationParameter);
        Task<Tuple<int, IEnumerable<DeleteClassRequest>>> GetRequestDeleteClassList(PaginationParameter paginationParameter);
        Task<int> UpdateDeletedClass(ConfirmDeleteClassInput confirmDeleteClassInput);
        Task<Tuple<int, IEnumerable<Class>>> GetDeletedClassList(PaginationParameter paginationParameter);
        Task<Class> GetClassByClassName(string className);
        Task<Class> GetClassDetail(int id);
        Task<Tuple<int, IEnumerable<Trainee>>> GetTraineesByClassId(int id, PaginationParameter paginationParameter);
        Task<int> InsertNewRequestDeleteClass(DeleteClassRequest deleteClassRequest);
        Task<Class> GetClassByClassID(int classId);
    }
}