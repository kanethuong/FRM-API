using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.CompanyDTO;

namespace kroniiapi.DTO.Profiles
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<CompanyRequest, CompanyRequestResponse>()
                .ForMember(ti => ti.CompanyName, c => c.MapFrom(s => s.Company.Fullname));
            CreateMap<CompanyRequest, CompanyReport>()
                .ForMember(ti => ti.CompanyName, c => c.MapFrom(s => s.Company.Fullname));
            CreateMap<CompanyRequest, RequestDetail>()
            .ForMember(ti => ti.CompanyName, c => c.MapFrom(s => s.Company.Fullname));
            CreateMap<Trainee, TraineeInRequest>();
        }
    }
}