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
        Task<bool> UpdateDeletedClass(ConfirmDeleteClassInput confirmDeleteClassInput);
        Task<Tuple<int, IEnumerable<Class>>> GetDeletedClassList(PaginationParameter paginationParameter);
    }
}