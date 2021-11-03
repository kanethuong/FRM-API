using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public interface IFeedbackService
    {
        Task<ICollection<AdminFeedback>> GetAdminFeedbacksByClassId(int classId);

        Task<(int,string)> InsertNewAdminFeedback(AdminFeedback adminFeedback);

        Task<ICollection<TrainerFeedback>> GetTrainerFeedbacksByClassId(int classId);

        Task<(int,string)> InsertNewTrainerFeedback(TrainerFeedback trainerFeedback);

        Task<ICollection<TrainerFeedback>> GetTrainerFeedbackByTrainerId(int trainerId);

    }
}