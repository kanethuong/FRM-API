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
            CreateMap<CertificateInput,Certificate>();
            //TraineeDashboard Mapping
            CreateMap<Trainee,TraineeProfileDetail>();
            CreateMap<Calendar,ModuleInTraineeDashboard>()
                .ForMember(md => md.ModuleId,c => c.MapFrom(s => s.Module.ModuleId))
                .ForMember(md => md.ModuleName,c => c.MapFrom(s => s.Module.ModuleName))
                .ForMember(md => md.SlotDuration,c => c.MapFrom(s => s.Module.SlotDuration.TotalMinutes));
            CreateMap<Class,TraineeClassInfo>()
                .ForMember(ti => ti.TrainerAvatarURL,c => c.MapFrom(s => s.Trainer.AvatarURL))
                .ForMember(ti => ti.TrainerName,c => c.MapFrom(s => s.Trainer.Fullname))
                .ForMember(ti => ti.TrainerEmail,c => c.MapFrom(s => s.Trainer.Email))
                .ForMember(ti => ti.RoomName,c => c.MapFrom(s => s.Room.RoomName));
            
            //TraineeTimetable Mapping
            CreateMap<Calendar,ModuleInTimeTable>()
                .ForMember(md => md.ModuleId,c => c.MapFrom(s => s.Module.ModuleId))
                .ForMember(md => md.ModuleName,c => c.MapFrom(s => s.Module.ModuleName))
                .ForMember(md => md.SlotDuration,c => c.MapFrom(s => s.Module.SlotDuration.TotalMinutes));

            CreateMap<TraineeProfileDetailInput,Trainee>();
            CreateMap<Trainee,TraineeResponse>();
        }
    }
}