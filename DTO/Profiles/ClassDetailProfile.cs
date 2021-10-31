using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.AdminDTO;
using kroniiapi.DTO.ClassDetailDTO;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.DTO.ModuleDTO;

namespace kroniiapi.DTO.Profiles
{
    public class ClassDetailProfile : Profile
    {
        public ClassDetailProfile()
        {
            CreateMap<Class,ClassDetailResponse>();
            CreateMap<Trainee,TraineeResponse>();
            CreateMap<Admin,AdminResponse>();
            CreateMap<Trainer,TrainerFeedback>();
            CreateMap<Module,ModuleResponse>();
           

        }
    }
}