using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.FeedbackDTO;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class FeedbackService : IFeedbackService
    {
        private DataContext _dataContext;
        public FeedbackService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        /// <summary>
        /// Get Admin Feddback By Class ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of adminfeedback</returns>
        // public async Task<ICollection<AdminFeedback>> GetAdminFeedbacksByClassId(int classId)
        // {
        //     var checkAdmin = await _dataContext.Classes.Where(c => c.ClassId == classId).Select(c => c.AdminId).FirstOrDefaultAsync();
        //     var traineeIds = await _dataContext.Trainees.Where(t => t.ClassId == classId).Select(t => t.TraineeId).ToListAsync();
        //     List<AdminFeedback> adminFeedbacks = new List<AdminFeedback>();
        //     foreach (var item in traineeIds)
        //     {
        //         adminFeedbacks.AddRange(await _dataContext.AdminFeedbacks.Where(a => a.TraineeId == item && a.AdminId == checkAdmin).ToListAsync()); 
        //     }

        //     return adminFeedbacks;
        // }
        /// <summary>
        /// Insert New Admin Feedback
        /// </summary>
        /// <param name="adminFeedback"></param>
        /// <returns>-1 if invalid input & 0 if failed to insert & 1 if success</returns>
        // public async Task<(int,string)> InsertNewAdminFeedback(AdminFeedback adminFeedback)
        // {
        //     if (!_dataContext.Trainees.Any(t => t.TraineeId == adminFeedback.TraineeId))
        //     {
        //         return (0,"Don't have this Trainee");
        //     }
        //     if (!_dataContext.Admins.Any(t => t.AdminId == adminFeedback.AdminId))
        //     {
        //         return (0,"Don't have this Admin");
        //     }
        //     var classId = await _dataContext.Trainees.Where(c => c.TraineeId == adminFeedback.TraineeId).FirstOrDefaultAsync();
        //     if (!_dataContext.Classes.Any(c => c.AdminId == adminFeedback.AdminId && c.ClassId == classId.ClassId))
        //     {
        //         return (-1, "Admin doesn't train Trainee");
        //     }
        //     if (_dataContext.AdminFeedbacks.Any(
        //         f => f.AdminId == adminFeedback.AdminId && f.TraineeId == adminFeedback.TraineeId
        //     ))
        //     {
        //         return (-1,"Trainee has feedback this Admin");
        //     }
        //     _dataContext.Add(adminFeedback);
        //     await _dataContext.SaveChangesAsync();
        //     return (1, "Success");
        // }
        /// <summary>
        /// Get Trainer Feedback
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of Trainer Feedback</returns>
        // public async Task<ICollection<TrainerFeedback>> GetTrainerFeedbacksByClassId(int classId)
        // { 
        //     var checkTrainer = await _dataContext.Classes.Where(c => c.ClassId == classId).Select(c => c.TrainerId).FirstOrDefaultAsync();
        //     var traineeIds = await _dataContext.Trainees.Where(t => t.ClassId == classId).Select(t => t.TraineeId).ToListAsync();
        //     List<TrainerFeedback> trainerFeedbacks = new List<TrainerFeedback>();
        //     foreach (var item in traineeIds)
        //     {
        //         trainerFeedbacks.AddRange(await _dataContext.TrainerFeedbacks.Where(a => a.TraineeId == item && a.TrainerId == checkTrainer).ToListAsync()); 
        //     }

        //     return trainerFeedbacks;
        // }
        /// <summary>
        /// Insert New Trainer Feedback
        /// </summary>
        /// <param name="trainerFeedback"></param>
        /// <returns>-1 if invalid input & 0 if failed to insert & 1 if success</returns>
        public async Task<(int, string)> InsertNewFeedback(Feedback feedback)
        {
            if (!_dataContext.Trainees.Any(t => t.TraineeId == feedback.TraineeId))
            {
                return (-1, "Don't have this Trainee");
            }
            // if trainer has feedback this month, update feedback
            var fbInMonthBefore = await _dataContext.Feedbacks.Where(
                        f => f.TraineeId == feedback.TraineeId && f.CreatedAt.Month == DateTime.Now.Month).FirstOrDefaultAsync();
            if (fbInMonthBefore != null)
            {
                fbInMonthBefore.TopicContent = feedback.TopicContent;
                fbInMonthBefore.TopicObjective = feedback.TopicObjective;
                fbInMonthBefore.ApproriateTopicLevel = feedback.ApproriateTopicLevel;
                fbInMonthBefore.TopicUsefulness = feedback.TopicUsefulness;
                fbInMonthBefore.TrainingMaterial = feedback.TrainingMaterial;
                fbInMonthBefore.TrainerKnowledge = feedback.TrainerKnowledge;
                fbInMonthBefore.SubjectCoverage = feedback.SubjectCoverage;
                fbInMonthBefore.InstructionAndCommunicate = feedback.InstructionAndCommunicate;
                fbInMonthBefore.TrainerSupport = feedback.TrainerSupport;
                fbInMonthBefore.Logistics = feedback.Logistics;
                fbInMonthBefore.InformationToTrainees = feedback.InformationToTrainees;
                fbInMonthBefore.AdminSupport = feedback.AdminSupport;
                fbInMonthBefore.OtherComment = feedback.OtherComment;
                fbInMonthBefore.CreatedAt = DateTime.Now;
            }
            else _dataContext.Feedbacks.Add(feedback);
            int row = await _dataContext.SaveChangesAsync();
            return (row, "Send feedback successfully");

        }

        public async Task<Feedback> GetFeedbackByTraineeId(int traineeId)
        {
            return await _dataContext.Feedbacks.Where(fb => fb.TraineeId == traineeId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get trainer feedback by trainer ID
        /// </summary>
        /// <param name="trainerId"></param>
        /// <returns>List of feedback</returns>
        // public async Task<ICollection<TrainerFeedback>> GetTrainerFeedbackByTrainerId(int trainerId){
        //     return await _dataContext.TrainerFeedbacks.Where(t => t.TrainerId==trainerId).ToListAsync();
        // }
    }
}