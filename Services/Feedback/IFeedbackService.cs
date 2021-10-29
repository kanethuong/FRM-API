using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public interface IFeedbackService
    {
        Task<ICollection<AdminFeedback>> GetAdminFeedbacksByAdminId(int id);

        Task<(int,string)> InsertNewAdminFeedback(AdminFeedback adminFeedback);

        Task<ICollection<TrainerFeedback>> GetTrainerFeedbacksByAdminId(int id);

        Task<(int,string)> InsertNewTrainerFeedback(TrainerFeedback trainerFeedback);

    }
}