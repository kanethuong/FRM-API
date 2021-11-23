using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.MarkDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Helper;
using kroniiapi.Helper.Timetable;
using kroniiapi.Services;
using kroniiapi.Services.Attendance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimetableController : ControllerBase
    {
        private readonly ITimetableService _timetableService;
        private readonly IClassService _classService;
        private readonly IAttendanceService _attendanceService;
        private readonly ICalendarService _calendarService;
        private DataContext _datacontext;
        public TimetableController(DataContext dataContext, ITimetableService timetableService, IClassService classService, IAttendanceService attendanceService, ICalendarService calendarService)
        {
            _timetableService = timetableService;
            _classService = classService;
            _attendanceService = attendanceService;
            _calendarService = calendarService;
            _datacontext = dataContext;
        }
        [HttpPost("create")]
        public async Task<ActionResult> CreateTimetableForClass(int year)
        {
            var startDay = new DateTime(year, 1, 1);
            var endDay = new DateTime(year + 1, 12, 31);
            var holiday = new List<DateTime>{
                new DateTime(1,1,1)
            };
            var lunarholi = TimetableHelper.GetLunarHoliday(year);
            lunarholi.AddRange(TimetableHelper.GetLunarHoliday(year + 1));
            return Ok(lunarholi);
        }

    }
}
