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
        public async Task<(int,string)> InsertNewAdminFeedback(AdminFeedback adminFeedback)
        {
            if (!_dataContext.Trainees.Any(t => t.TraineeId == adminFeedback.TraineeId))
            {
                return (0,"Don't have this Trainee");
            }
            if (!_dataContext.Admins.Any(t => t.AdminId == adminFeedback.AdminId))
            {
                return (0,"Don't have this Admin");
            }
            var classId = await _dataContext.Trainees.Where(c => c.TraineeId == adminFeedback.TraineeId).FirstOrDefaultAsync();
            if (!_dataContext.Classes.Any(c => c.AdminId == adminFeedback.AdminId && c.ClassId == classId.ClassId))
            {
                return (-1, "Admin doesn't train Trainee");
            }
            if (_dataContext.AdminFeedbacks.Any(
                f => f.AdminId == adminFeedback.AdminId && f.TraineeId == adminFeedback.TraineeId
            ))
            {
                return (-1,"Trainee has feedback this Admin");
            }
            _dataContext.Add(adminFeedback);
            await _dataContext.SaveChangesAsync();
            return (1, "Success");
        }
        /// <summary>
        /// Get Trainer Feedback
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of Trainer Feedback</returns>
        public async Task<ICollection<TrainerFeedback>> GetTrainerFeedbacksByTrainerId(int id)
        {
            var feedTrainer = await _dataContext.TrainerFeedbacks.Where(a => a.TrainerId == id).ToListAsync();
            return feedTrainer;
        }
        /// <summary>
        /// Insert New Trainer Feedback
        /// </summary>
        /// <param name="trainerFeedback"></param>
        /// <returns>-1 if invalid input & 0 if failed to insert & 1 if success</returns>
        public async Task<(int, string)> InsertNewTrainerFeedback(TrainerFeedback trainerFeedback)
        {
            if (!_dataContext.Trainees.Any(t => t.TraineeId == trainerFeedback.TraineeId))
            {
                return (0, "Don't have this Trainee");
            }
            if (!_dataContext.Trainers.Any(t => t.TrainerId == trainerFeedback.TrainerId))
            {
                return (0,"Don't have this Trainer");
            }
            var trainee = await _dataContext.Trainees.Where(c => c.TraineeId == trainerFeedback.TraineeId).FirstOrDefaultAsync();
            if (!_dataContext.Classes.Any(c => c.TrainerId == trainerFeedback.TrainerId && c.ClassId == trainee.ClassId))
            {
                return (-1, "Trainer doesn't train Trainee");
            }
            if (_dataContext.TrainerFeedbacks.Any(
                f => f.TrainerId == trainerFeedback.TrainerId && f.TraineeId == trainerFeedback.TraineeId
            ))
            {
                return (-1,"Trainee has feedback this Trainer");
            }
            _dataContext.Add(trainerFeedback);
            await _dataContext.SaveChangesAsync();
            return (1, "Success");
        }
    }
}