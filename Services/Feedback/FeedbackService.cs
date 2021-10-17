using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public class FeedbackService :IFeedbackService
    {
        private DataContext _dataContext;
        public FeedbackService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ICollection<AdminFeedback>> GetAdminFeedbacksByAdminId(int id)
        {
            return null;
        }
        public async Task<int> InsertNewAdminFeedback(AdminFeedback adminFeedback)
        {
            return 0;
        }
        public async Task<ICollection<TrainerFeedback>> GetTrainerFeedbacksByAdminId(int id)
        {
            return null;
        }
        public async Task<int> InsertNewTrainerFeedback(TrainerFeedback trainerFeedback)
        {
            return 0;
        }
    }
}