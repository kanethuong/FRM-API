using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.TrainerDTO;

namespace kroniiapi.DTO.Profiles
{
    public class TrainerProfile : Profile
    {
        public TrainerProfile()
        {
             CreateMap<Trainer,TrainerResponse>();
             CreateMap<Trainer,TrainerProfileDetail>();

             //TrainerDashboard mapping
             CreateMap<Calendar,TrainerDashboard>()
                .ForMember(md => md.ModuleId,c => c.MapFrom(s => s.Module.ModuleId))
                .ForMember(md => md.ModuleName,c => c.MapFrom(s => s.Module.ModuleName))
                .ForMember(md => md.SlotDuration,c => c.MapFrom(s => s.Module.SlotDuration.TotalMinutes));
             CreateMap<Class,ClassRoom>()
                .ForMember(ti => ti.RoomName,c => c.MapFrom(s => s.Room.RoomName));
            
            //TrainerTimeTable mapping
             CreateMap<Calendar,TrainerTimeTable>()
                .ForMember(md => md.ModuleId,c => c.MapFrom(s => s.Module.ModuleId))
                .ForMember(md => md.ModuleName,c => c.MapFrom(s => s.Module.ModuleName))
                .ForMember(md => md.SlotDuration,c => c.MapFrom(s => s.Module.SlotDuration.TotalMinutes));
             CreateMap<Class,ClassRoom>()
                .ForMember(ti => ti.RoomName,c => c.MapFrom(s => s.Room.RoomName));
             CreateMap<Trainer,TrainerProfileDetailInput>();
        }
    }
}