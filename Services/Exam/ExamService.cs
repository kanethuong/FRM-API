using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;

namespace kroniiapi.Services
{
    public class ExamService : IExamService
    {
        private DataContext _dataContext;
        public ExamService(DataContext dataContext )
        {
            _dataContext = dataContext;
        }
        public async Task<Exam> GetExamById(int id)
        {
            return null;
        }
        public async Task<int> InsertNewExam(Exam exam){
            return 0;
        }
        public async Task<int> UpdateExam(int id, Exam exam){
            return 0;
        }
        public async Task<Tuple<int, IEnumerable<Exam>>> GetExamList(PaginationParameter paginationParameter){
            return null;
        }
    }
}