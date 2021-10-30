using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ExamDTO;
using kroniiapi.DTO.TraineeDTO;

namespace kroniiapi.DTO.Profiles
{
    public class ExamProfile : Profile
    {
        public ExamProfile()
        {
            CreateMap<Exam, ExamInTraineeDashboard>()
                .ForMember(mi => mi.ModuleName, c => c.MapFrom(s => s.Module.ModuleName))
                .ForMember(ti => ti.Date, c => c.MapFrom(s => s.ExamDay))
                .ForMember(ti => ti.AdminAvatarURL, c => c.MapFrom(s => s.Admin.AvatarURL))
                .ForMember(ti => ti.AdminName, c => c.MapFrom(s => s.Admin.Fullname))
                .ForMember(ti => ti.AdminEmail, c => c.MapFrom(s => s.Admin.Email));
                
            CreateMap<Exam, ExamInTimeTable>()
            .ForMember(ti => ti.Date, c => c.MapFrom(s => s.ExamDay))
            .ForMember(ti => ti.AdminAvatarURL, c => c.MapFrom(s => s.Admin.AvatarURL))
            .ForMember(ti => ti.AdminName, c => c.MapFrom(s => s.Admin.Fullname))
            .ForMember(ti => ti.AdminEmail, c => c.MapFrom(s => s.Admin.Email));

            CreateMap<NewExamInput, Exam>();
            CreateMap<Exam, ExamResponse>()
                .ForMember(mi => mi.ModuleName, c => c.MapFrom(s => s.Module.ModuleName))
                .ForMember(mi => mi.AdminName, c => c.MapFrom(s => s.Admin.Fullname));
        }
    }
}