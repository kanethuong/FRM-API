using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.CostDTO;

namespace kroniiapi.DTO.Profiles
{
    public class CostProfile : Profile
    {
        public CostProfile()
        {
            CreateMap<Cost, CostResponse>()
                .ForMember(ti => ti.Creator, c => c.MapFrom(s => s.Admin.Fullname))
                .ForMember(ti => ti.CostType, c => c.MapFrom(s => s.CostType.CostTypeName));
            CreateMap<CostInput, Cost>();

        }
    }
}