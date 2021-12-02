using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public interface IFeedbackService
    {
        Task<(int,string)> InsertNewFeedback(Feedback feedback);
        Task<Feedback> GetFeedbackByTraineeId(int traineeId);
        bool CheckTraineeHasFeedbackThisMonth(int traineeId);


    }
}