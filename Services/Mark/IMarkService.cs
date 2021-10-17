using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public interface IMarkService
    {
        Task<ICollection<Mark>> GetMarkByTraineeId(int id, DateTime? startDate, DateTime? endDate);
        
    
        Task<ICollection<Mark>> GetMarkByModuleId(int id, DateTime? startDate, DateTime? endDate);
        
    
        Task<int> InsertNewMark(int moduleId, int traineeId);
        
        Task<int> UpdateMark(int moduleId, int traineeId, Mark mark);
        
    }
}