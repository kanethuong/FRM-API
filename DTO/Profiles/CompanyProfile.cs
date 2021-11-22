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
            CreateMap<Module, string>().ConvertUsing(m => m.ModuleName);
            //CreateMap<Module, string>().ForMember(dest=>dest,m=>m.MapFrom(src=>src.ModuleName));
            CreateMap<Trainee, TraineeSearchResponse>()
                .ForMember(tsr => tsr.ModuleNames, t => t.MapFrom(e => e.Class.Modules))
                .ForMember(tsr => tsr.RoleName, t => t.MapFrom(e => e.Role.RoleName));
            CreateMap<Module, TraineeSkillResponse>();
            CreateMap<int, CompanyRequestDetail>().ForMember(dest => dest.TraineeId, m => m.MapFrom(src => src));
            CreateMap<decimal, CompanyRequestDetail>().ForMember(dest => dest.Wage, m => m.MapFrom(src => src));
            CreateMap<TraineeInRequestDetail, CompanyRequestDetail>()
                .ForMember(cr => cr.TraineeId, s => s.MapFrom(rt => rt.TraineeId))
                .ForMember(cr => cr.Wage, s => s.MapFrom(rt => rt.Wage));
            CreateMap<RequestTraineeInput, CompanyRequest>()
                .ForMember(cr => cr.CompanyRequestDetails, s => s.MapFrom(rt => rt.Trainees));
            CreateMap<CompanyRequest, RequestTraineeResponse>()
                .ForMember(rt => rt.Id, s => s.MapFrom(cr => cr.CompanyRequestId))
                .ForMember(rt => rt.NumberOfTrainee, s => s.MapFrom(cr => cr.CompanyRequestDetails.Count()));
            CreateMap<CompanyRequestDetail, TraineeInResponse>()
                .ForMember(tr => tr.AvatarURL, s => s.MapFrom(crd => crd.Trainee.AvatarURL))
                .ForMember(tr => tr.Fullname, s => s.MapFrom(crd => crd.Trainee.Fullname))
                .ForMember(tr => tr.Email, s => s.MapFrom(crd => crd.Trainee.Email))
                .ForMember(tr => tr.Phone, s => s.MapFrom(crd => crd.Trainee.Phone))
                .ForMember(tr => tr.Wage, s => s.MapFrom(crd => crd.Wage));
            CreateMap<CompanyRequest, RequestTraineeDetailResponse>()
                .ForMember(rt => rt.Trainees, s => s.MapFrom(cr => cr.CompanyRequestDetails));
            CreateMap<CompanyProfileDetailInput, Company>();
        }
    }
}