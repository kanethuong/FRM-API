using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.DTO.ModuleDTO;

namespace kroniiapi.DTO.Profiles
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {
            CreateMap<Module, ModuleResponse>();
            CreateMap<ModuleInput, Module>()
                .ForMember(output => output.SlotDuration, opt => opt.MapFrom(src => TimeSpan.FromMinutes(src.SlotDuration)))
                .ForSourceMember(input => input.Syllabus, opt => opt.DoNotValidate())
                .ForSourceMember(input => input.Icon, opt => opt.DoNotValidate());
            CreateMap<ModuleUpdateInput, Module>()
                .ForMember(output => output.SlotDuration, opt => opt.MapFrom(src => TimeSpan.FromMinutes(src.SlotDuration)))
                .ForSourceMember(input => input.Syllabus, opt => opt.DoNotValidate())
                .ForSourceMember(input => input.Icon, opt => opt.DoNotValidate());
            CreateMap<AssignModuleInput, ClassModule>();
        }
    }
}