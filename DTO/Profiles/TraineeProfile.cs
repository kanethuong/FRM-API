using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.TraineeDTO;

namespace kroniiapi.DTO.Profiles
{
    public class TraineeProfile : Profile
    {
        public TraineeProfile()
        {
            CreateMap<TraineeProfileDetail,Trainee>();
            CreateMap<CertificateInput,Certificate>();
            
        }
    }
}