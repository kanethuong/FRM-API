using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO.AttendanceDTO;

namespace kroniiapi.DTO.Profiles
{
    public class AttendanceProfile : Profile
    {
        public AttendanceProfile()
        {
            CreateMap<Attendance, AttendanceResponse>()
                .ForMember(at => at.Name, a => a.MapFrom(s => s.Trainee.Fullname))
                .ForMember(at => at.Id, a => a.MapFrom(s => s.AttendanceId));
            CreateMap<AttendanceInput, Attendance>()
                .ForMember(a => a.AttendanceId, a => a.MapFrom(s => s.Id));
        }
    }
}