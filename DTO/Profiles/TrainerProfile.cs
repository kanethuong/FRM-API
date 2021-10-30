using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.TrainerDTO;

namespace kroniiapi.DTO.Profiles
{
    public class TrainerProfile : Profile
    {
        public TrainerProfile()
        {
             CreateMap<Trainer,TrainerResponse>();
             CreateMap<Trainer,TrainerProfileDetail>();
        }
    }
}