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
        Task<IEnumerable<Exam>> GetExamListByModuleId(List<Calendar> calendars, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Exam>> GetExamListByTraineeId(int traineeId, DateTime startDate, DateTime endDate);
        Task<(bool? status, string message)> CancelExam(int id);
        Task<bool> CheckDateExam(int classId,int moduleId,DateTime time,int lastSlot);
        
    }
}