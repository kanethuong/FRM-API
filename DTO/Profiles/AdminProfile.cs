using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.AdminDTO;

namespace kroniiapi.DTO.Profiles
{
    public class AdminProfile :Profile
    {
        public AdminProfile()
        {
            CreateMap<Admin,AdminResponse>();
            CreateMap<Admin,AdminProfileDetail>();
            CreateMap<AdminProfileDetailInput,Admin>();
        }
    }
}