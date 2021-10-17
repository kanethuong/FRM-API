using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public class MarkService : IMarkService
    {
        private DataContext _dataContext;
        public MarkService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<ICollection<Mark>> GetMarkByTraineeId(int id, DateTime? startDate, DateTime? endDate)
        {
            return null;
        }
        public async Task<ICollection<Mark>> GetMarkByModuleId(int id, DateTime? startDate, DateTime? endDate)
        {
            return null;
        }
        public async Task<int> InsertNewMark(int moduleId, int traineeId){
            return 0;
        }
        public async Task<int> UpdateMark(int moduleId, int traineeId, Mark mark){
            return 0;
        }
    }
}