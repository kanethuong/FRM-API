using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ClassDTO;

namespace kroniiapi.DTO.Profiles
{
    public class ClassProfile : Profile
    {
        public ClassProfile()
        {
            CreateMap<Class, ClassResponse>();
            CreateMap<Class, DeleteClassResponse>();
        }
    }
}