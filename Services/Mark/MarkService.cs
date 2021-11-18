using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class MarkService : IMarkService
    {
        private DataContext _dataContext;
        public MarkService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        /// <summary>
        /// Get mark by trainee id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<ICollection<Mark>> GetMarkByTraineeId(int id, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (startDate == null)
            {
                startDate = DateTime.MinValue;
            }
            if (endDate == null)
            {
                endDate = DateTime.MaxValue;
            }
            return await _dataContext.Marks.Where(m => m.TraineeId == id && m.PublishedAt >= startDate && m.PublishedAt <= endDate).ToListAsync();
        }

        public async Task<Mark> GetMarkByTraineeIdAndModuleId(int traineeId, int moduleId, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (startDate == null)
            {
                startDate = DateTime.MinValue;
            }
            if (endDate == null)
            {
                endDate = DateTime.MaxValue;
            }
            return await _dataContext.Marks.Where(m => m.TraineeId == traineeId && m.ModuleId == moduleId && m.PublishedAt >= startDate && m.PublishedAt <= endDate).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get mark by module id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>list of marks</returns>
        public async Task<ICollection<Mark>> GetMarkByModuleId(int id, DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null)
            {
                startDate = DateTime.MinValue;
            }
            if (endDate == null)
            {
                endDate = DateTime.MaxValue;
            }
            return await _dataContext.Marks.Where(m => m.ModuleId == id && m.PublishedAt > startDate && m.PublishedAt < endDate).ToListAsync();
        }
        /// <summary>
        /// Insert New Mark
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="traineeId"></param>
        /// <returns>-1 if invalid input & 0 if failed to insert & 1 if success</returns>
        public async Task<int> InsertNewMark(Mark mark)
        {
            if (_dataContext.Marks.Any(m => m.ModuleId == mark.ModuleId && m.TraineeId == mark.TraineeId))
            {
                return -1;
            }
            var newMark = new Mark
            {
                ModuleId = mark.ModuleId,
                TraineeId = mark.TraineeId,
                Score = mark.Score,
            };
            _dataContext.Add(newMark);
            return await _dataContext.SaveChangesAsync();
        }
        /// <summary>
        /// Update Mark
        /// </summary>
        /// <param name="mark"></param>
        /// <returns>-1 if invalid input & 0 if failed to update & 1 if success</returns>
        public async Task<int> UpdateMark(Mark mark)
        {
            var markUpdate = await _dataContext.Marks.Where(m => m.ModuleId == mark.ModuleId && m.TraineeId == mark.TraineeId).FirstOrDefaultAsync();
            if (markUpdate == null)
            {
                return -1;
            }
            markUpdate.Score = mark.Score;
            return await _dataContext.SaveChangesAsync();
        }


        /// <summary>
        /// Update list marks
        /// </summary>
        /// <param name="status"></param>
        /// <param name="listMarks"></param>
        /// <returns>true: update success, false: fail at id(s)</returns>
        public async Task<(bool status, string message)> UpdateMarks(IEnumerable<Mark> listMarks)
        {
            List<string> errorTraineeIds = new List<string>();
            List<string> errorModuleIds = new List<string>();
            foreach (var item in listMarks)
            {
                var existedTrainee = _dataContext.Trainees.FirstOrDefault(m => m.TraineeId == item.TraineeId && m.IsDeactivated == false);
                if (existedTrainee == null)
                {
                    errorTraineeIds.Add(item.TraineeId.ToString());
                }
                else if (_dataContext.Modules.FirstOrDefault(m => m.ModuleId == item.ModuleId) == null)
                {
                    errorModuleIds.Add(item.ModuleId.ToString());
                }
                else if(_dataContext.ClassModules.FirstOrDefault(m => m.ClassId == existedTrainee.ClassId && m.ModuleId == item.ModuleId) == null){
                    return(false, "Trainee with id " + existedTrainee.TraineeId + "does not studying module " + item.ModuleId);
                }
                else
                {
                    var existMark = await _dataContext.Marks.Where(m => m.TraineeId == item.TraineeId && m.ModuleId == item.ModuleId).FirstOrDefaultAsync();
                    if (existMark == null)
                    {
                        await this.InsertNewMark(item);
                    }
                    else
                    {
                        existMark.Score = item.Score;
                    }
                }
            }
            if (errorTraineeIds.Count() != 0)
            {
                return (false, "There is(are) error(s) with trainee id: " + errorTraineeIds.Aggregate((i, j) => i + ", " + j));
            }
            else if (errorModuleIds.Count() != 0)
            {
                return (false, "This(these) module id(s) is(are) not exist: " + errorModuleIds.Aggregate((i, j) => i + ", " + j));
            }
            var rowUpdated = await _dataContext.SaveChangesAsync();
            // if(rowUpdated == 0)
            // {
            //     return(false, "Nothing updated");
            // }
            return (true, "");

        }
    }
}