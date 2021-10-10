using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.AuthDTO;

namespace kroniiapi.DTO.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<Admin, AccountResponse>().ForMember(ac => ac.Role, act => act.Ignore())
                .ForMember(ac => ac.Role, a => a.MapFrom(s => s.Role.RoleName));
            CreateMap<Trainer, AccountResponse>().ForMember(ac => ac.Role, act => act.Ignore())
                .ForMember(ac => ac.Role, t => t.MapFrom(s => s.Role.RoleName));
            CreateMap<Trainee, AccountResponse>().ForMember(ac => ac.Role, act => act.Ignore())
                .ForMember(ac => ac.Role, t => t.MapFrom(s => s.Role.RoleName));
            CreateMap<Company, AccountResponse>().ForMember(ac => ac.Role, act => act.Ignore())
                .ForMember(ac => ac.Role, c => c.MapFrom(s => s.Role.RoleName));
        }
    }
}