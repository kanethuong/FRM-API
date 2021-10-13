using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.AccountDTO;

namespace kroniiapi.DTO.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<AccountInput, Admin>().ForMember(a => a.Role, act => act.Ignore());
            CreateMap<AccountInput, Trainer>().ForMember(t => t.Role, act => act.Ignore());
            CreateMap<AccountInput, Trainee>().ForMember(t => t.Role, act => act.Ignore());
            CreateMap<AccountInput, Company>().ForMember(c => c.Role, act => act.Ignore());
            CreateMap<AccountInput, Administrator>().ForMember(c => c.Role, act => act.Ignore());

            CreateMap<Admin, AccountResponse>().ForMember(ac => ac.Role, act => act.Ignore())
                .ForMember(ac => ac.Role, a => a.MapFrom(s => s.Role.RoleName));
            CreateMap<Trainer, AccountResponse>().ForMember(ac => ac.Role, act => act.Ignore())
                .ForMember(ac => ac.Role, t => t.MapFrom(s => s.Role.RoleName));
            CreateMap<Trainee, AccountResponse>().ForMember(ac => ac.Role, act => act.Ignore())
                .ForMember(ac => ac.Role, t => t.MapFrom(s => s.Role.RoleName));
            CreateMap<Company, AccountResponse>().ForMember(ac => ac.Role, act => act.Ignore())
                .ForMember(ac => ac.Role, c => c.MapFrom(s => s.Role.RoleName));

            CreateMap<Admin, DeletedAccountResponse>();
            CreateMap<Trainer, DeletedAccountResponse>();
            CreateMap<Trainee, DeletedAccountResponse>();
            CreateMap<Company, DeletedAccountResponse>();
        }
    }
}