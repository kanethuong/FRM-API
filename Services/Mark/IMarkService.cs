using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public interface IMarkService
    {
        Task<ICollection<Mark>> GetMarkByTraineeId(int id, DateTime? startDate = null, DateTime? endDate = null);
        
    
        Task<ICollection<Mark>> GetMarkByModuleId(int id, DateTime? startDate, DateTime? endDate);
        
    
        Task<int> InsertNewMark(Mark mark);
        
        Task<int> UpdateMark(Mark mark);
        
    }
}