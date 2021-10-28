using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ExamDTO;

namespace kroniiapi.DTO.Profiles
{
    public class ExamProfile : Profile
    {
        public ExamProfile()
        {
            CreateMap<NewExamInput,Exam>();
            CreateMap<Exam,ExamResponse>()
                .ForMember(mi => mi.ModuleName,c => c.MapFrom(s => s.Module.ModuleName))
                .ForMember(mi => mi.AdminName,c => c.MapFrom(s => s.Admin.Fullname));
        }
    }
}