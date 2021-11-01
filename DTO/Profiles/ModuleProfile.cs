using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ModuleDTO;

namespace kroniiapi.DTO.Profiles
{
    public class ModuleProfile: Profile
    {
        public ModuleProfile()
        {
            CreateMap<Module,ModuleResponse>();
            CreateMap<ModuleInput,Module>();
        }
    }
}