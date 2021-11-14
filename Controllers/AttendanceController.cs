using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.AttendanceDTO;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.Services.Attendance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Authorize(Policy = "Attendance")]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IMapper _mapper;
        public AttendanceController(IAttendanceService attendanceService, IMapper mapper)
        {
            _attendanceService = attendanceService;
            _mapper = mapper;
        }

        /// <summary>
        /// View all class of admin to take attendance
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns>200: A list of classes / 404: Admin does not have class || Admin id for found</returns>
        [HttpGet("{adminId:int}")]
        public async Task<ActionResult<IEnumerable<ClassResponse>>> ViewClassList(int adminId)
        {
            var rs = await _attendanceService.GetClassList(adminId);
            if (rs == null || rs.Count() == 0)
            {
                return NotFound(new ResponseDTO(404, "There are no classes you can take attendance"));
            }
            return Ok(_mapper.Map<IEnumerable<ClassResponse>>(rs));
        }

        /// <summary>
        /// View attendance detail of a class
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="classId"></param>
        /// <returns>200: A list of attendance / 404: ClassId or AdminId not found</returns>
        [HttpGet("{adminId:int}/{classId:int}")]
        public async Task<ActionResult<IEnumerable<AttendanceResponse>>> ViewAttendanceList(int adminId, int classId)
        {
            var rs = await _attendanceService.GetAttendanceList(adminId, classId);
            if (rs == null || rs.Count() == 0)
            {
                return NotFound(new ResponseDTO(400, "Class attendance details not found"));
            }
            var result = _mapper.Map<IEnumerable<AttendanceResponse>>(rs);
            return Ok(result);
        }

        /// <summary>
        /// Update trainee attendance status
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="classId"></param>
        /// <param name="attendanceInputs"></param>
        /// <returns>200: Updated / 400: Error message</returns>
        [HttpPut("{adminId:int}/{classId:int}")]
        public async Task<ActionResult<ResponseDTO>> TakeAttendance(int adminId, int classId, [FromBody] ArrayBodyInput<AttendanceInput> listAttendanceInputs)
        {
            var attendances = _mapper.Map<IEnumerable<Attendance>>(listAttendanceInputs.arrayData);
            (bool status, string message) = await _attendanceService.TakeAttendance(attendances);
            if (status == false)
            {
                return BadRequest(new ResponseDTO(400, message));
            }

            return Ok(new ResponseDTO(200, "Attendance is updated."));
        }
    }
}