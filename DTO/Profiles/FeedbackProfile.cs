using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.FeedbackDTO;

namespace kroniiapi.DTO.Profiles
{
    public class FeedbackProfile : Profile
    {
        public FeedbackProfile()
        {
            CreateMap<TrainerFeedback, FeedbackContent>();  //Map rate and content of trainerfeedback to feedbackcontent
            CreateMap<Trainer, TrainerInFeedbackResponse>();   // Map trainer name and avatar of trainer model to trainer in feedback


            CreateMap<Admin, AdminInFeedbackResponse>(); // Map admin name and avatar of admin model to admin in feedback
            CreateMap<AdminFeedback, FeedbackContent>(); //Map rate and content of adminfeedback to feedbackcontent

<<<<<<< HEAD
            CreateMap<AdminFeedback, AdminFeedback>();
            CreateMap<TrainerFeedback, TrainerFeedback>();
=======
            CreateMap<AdminFeedbackInput, AdminFeedback>();
            CreateMap<TrainerFeedbackInput, TrainerFeedback>();
>>>>>>> 7295c15b751c3fefafa78f15f325dc59cb6806f0
        }
    }
}