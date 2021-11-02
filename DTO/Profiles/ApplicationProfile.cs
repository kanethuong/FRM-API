using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ApplicationDTO;
using kroniiapi.DTO.TraineeDTO;

namespace kroniiapi.DTO.Profiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Application, TraineeApplicationResponse>();
            CreateMap<Application, ApplicationResponse>()
                .ForMember(ti => ti.TraineeName, c => c.MapFrom(s => s.Trainee.Fullname))
                .ForMember(ti => ti.Category, c => c.MapFrom(s => s.ApplicationCategory.CategoryName));

            CreateMap<Application, ApplicationDetail>()
                .ForMember(ti => ti.TraineeName, c => c.MapFrom(s => s.Trainee.Fullname))
                .ForMember(ti => ti.Category, c => c.MapFrom(s => s.ApplicationCategory.CategoryName));
                
            CreateMap<ApplicationInput, Application>();
            CreateMap<ApplicationCategory, ApplicationCategoryResponse>();
        }
    }
}