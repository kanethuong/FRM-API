using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;

namespace kroniiapi.Services
{
    public interface IExamService
    {
        Task<Exam> GetExamById(int id);
       
        Task<int> InsertNewExam(Exam exam);
        
        Task<int> UpdateExam(int id, Exam exam);
       
        Task<Tuple<int, IEnumerable<Exam>>> GetExamList(PaginationParameter paginationParameter);
        
    }
}