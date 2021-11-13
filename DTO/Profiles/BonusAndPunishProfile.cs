using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.BonusAndPunishDTO;

namespace kroniiapi.DTO.Profiles
{
    public class BonusAndPunishProfile :Profile
    {
        public BonusAndPunishProfile()
        {
            CreateMap<BonusAndPunishInput,BonusAndPunish>();
            CreateMap<BonusAndPunish,BonusAndPunishResponse>();
            //Map className
            CreateMap<Trainee,TraineeInBP>()
                .ForMember(t => t.ClassName, m => m.MapFrom(a => a.Class.ClassName));
        }
    }
}