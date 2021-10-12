using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.AuthDTO;
using kroniiapi.DTO.AccountDTO;

namespace kroniiapi.DTO.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<Admin, AuthDTO.AccountResponse>().ForMember(ac => ac.Role, act => act.Ignore())
                .ForMember(ac => ac.Role, a => a.MapFrom(s => s.Role.RoleName));
            CreateMap<Trainer, AuthDTO.AccountResponse>().ForMember(ac => ac.Role, act => act.Ignore())
                .ForMember(ac => ac.Role, t => t.MapFrom(s => s.Role.RoleName));
            CreateMap<Trainee, AuthDTO.AccountResponse>().ForMember(ac => ac.Role, act => act.Ignore())
                .ForMember(ac => ac.Role, t => t.MapFrom(s => s.Role.RoleName));
            CreateMap<Company, AuthDTO.AccountResponse>().ForMember(ac => ac.Role, act => act.Ignore())
                .ForMember(ac => ac.Role, c => c.MapFrom(s => s.Role.RoleName));

            CreateMap<AccountDTO.AccountResponse, AuthDTO.AccountResponse>();
        }
    }
}