using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.MarkDTO;

namespace kroniiapi.DTO.Profiles
{
    public class MarkProfile : Profile
    {
        public MarkProfile()
        {
            CreateMap<Mark,ModuleMark>()
                .ForMember(mm=>mm.ModuleName,x => x.MapFrom(m => m.Module.ModuleName));
            CreateMap<TraineeMarkInput,Mark>();
                
        }
    }
}