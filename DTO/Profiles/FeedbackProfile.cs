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
            // CreateMap<TrainerFeedback, FeedbackContent>();  //Map rate and content of trainerfeedback to feedbackcontent
            // CreateMap<Trainer, TrainerInFeedbackResponse>();   // Map trainer name and avatar of trainer model to trainer in feedback


            // CreateMap<Admin, AdminInFeedbackResponse>(); // Map admin name and avatar of admin model to admin in feedback
            // CreateMap<AdminFeedback, FeedbackContent>(); //Map rate and content of adminfeedback to feedbackcontent

            // CreateMap<AdminFeedbackInput, AdminFeedback>();
            // CreateMap<TrainerFeedbackInput, TrainerFeedback>();

            CreateMap<FeedbackInput, Feedback>();
            CreateMap<Feedback, FeedbackResponse>();
        }
    }
}