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
            CreateMap<Class, ClassResponse>()
                .ForMember(cs => cs.Creator, m => m.MapFrom(a => a.Admin.Fullname)) //map fullname of admin in class model to fullname of classresponse
                .ForMember(cm => cm.NoOfModule,m => m.MapFrom(a => a.ClassModules.Count()));
            CreateMap<Trainer, TrainerInClassResponse>();
            CreateMap<Class, DeleteClassResponse>();
            CreateMap<Admin, CreatorDTO>();
            /*
            Mapping from model to DTO, using ForMember to specify which field is selected
            */
            CreateMap<DeleteClassRequest, RequestDeleteClassResponse>()
            /*
            get ClassName MapFrom class Class set to ClassName in DTO 
            */
                .ForMember(rs => rs.ClassName, m => m.MapFrom(d => d.Class.ClassName));
            CreateMap<NewClassInput, Class>();
            CreateMap<RequestDeleteClassInput, DeleteClassRequest>();
            CreateMap<Class, ClassDetailResponse>()
            .ForMember(rs => rs.RoomName, m => m.MapFrom(d => ""));
            CreateMap<Trainee, TraineeInclassResponse>();
            CreateMap<Class, TrainerClassListResponse>()
            .ForMember(c => c.NoOfModule, m => m.MapFrom(a => a.ClassModules.Count()));
            CreateMap<Class, TrainerClassDetailResponse>();
            CreateMap<AssignModuleInput, ClassModule>();
            CreateMap<Class, AdminDashboardClassResponse>();
        }
    }
}