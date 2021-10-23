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
            CreateMap<Application,ApplicationResponse>();
            CreateMap<ApplicationInput,Application>();
            CreateMap<ApplicationCategory,ApplicationCategoryResponse>();
        }
    }
}