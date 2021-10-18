using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class FeedbackService :IFeedbackService
    {
        private DataContext _dataContext;
        public FeedbackService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        /// <summary>
        /// Get Admin Feddback By Admin ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of adminfeedback</returns>
        public async Task<ICollection<AdminFeedback>> GetAdminFeedbacksByAdminId(int id)
        {
            var feedAdmin = await _dataContext.AdminFeedbacks.Where(a => a.AdminId == id).ToListAsync();
            return feedAdmin;
        }
        /// <summary>
        /// Insert New Admin Feedback
        /// </summary>
        /// <param name="adminFeedback"></param>
        /// <returns>-1 if invalid input & 0 if failed to insert & 1 if success</returns>
        public async Task<int> InsertNewAdminFeedback(AdminFeedback adminFeedback)
        {
            if (_dataContext.AdminFeedbacks.Any(
                f => f.TraineeId == adminFeedback.TraineeId
            ))
            {
                return 0;
            }
            _dataContext.Add(adminFeedback);

            return await _dataContext.SaveChangesAsync();
        }
        /// <summary>
        /// Get Trainer Feedback
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of Trainer Feedback</returns>
        public async Task<ICollection<TrainerFeedback>> GetTrainerFeedbacksByAdminId(int id)
        {
            var feedTrainer = await _dataContext.TrainerFeedbacks.Where(a => a.TrainerId == id).ToListAsync();
            return feedTrainer;
        }
        /// <summary>
        /// Insert New Trainer Feedback
        /// </summary>
        /// <param name="trainerFeedback"></param>
        /// <returns>-1 if invalid input & 0 if failed to insert & 1 if success</returns>
        public async Task<int> InsertNewTrainerFeedback(TrainerFeedback trainerFeedback)
        {
            if (_dataContext.AdminFeedbacks.Any(
                f => f.TraineeId == trainerFeedback.TraineeId
            ))
            {
                return 0;
            }
            _dataContext.Add(trainerFeedback);

            return await _dataContext.SaveChangesAsync();
        }
    }
}