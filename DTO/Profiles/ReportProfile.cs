using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ReportDTO;

namespace kroniiapi.DTO.Profiles
{
    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            CreateMap<Trainee, TraineeGeneralInfo>()
                .ForMember(ti => ti.EmpId, a => a.MapFrom(s => s.TraineeId))
                .ForMember(ti => ti.Account, a => a.MapFrom(s => s.Username))
                .ForMember(ti => ti.Name, a => a.MapFrom(s => s.Fullname))
                .ForMember(ti => ti.SalaryPaid, a => a.MapFrom(s => s.OnBoard))
                .ForMember(ti => ti.OJT, a => a.MapFrom(s => s.OnBoard))
                .ForMember(ti => ti.StartDate, a => a.MapFrom(s => s.Class.StartDay))
                .ForMember(ti => ti.EndDate, a => a.MapFrom(s => s.Class.EndDay));

            CreateMap<Feedback, TraineeFeedback>();

            CreateMap<BonusAndPunish, RewardAndPenalty>()
                .ForMember(rw => rw.BonusAndPenaltyPoint, a => a.MapFrom(s => s.Score));
        }
    }
}