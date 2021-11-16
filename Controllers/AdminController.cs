using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.AdminDTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Services;
using kroniiapi.Services.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using kroniiapi.DTO.ReportDTO;
using kroniiapi.Services.Attendance;

namespace kroniiapi.Controllers
{
    [ApiController]
    // [Authorize(Policy = "Admin")]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {

        private readonly IAdminService _adminService;
        private readonly IReportService _reportService;
        private readonly IClassService _classService;
        private readonly IMapper _mapper;

        public AdminController(IAdminService adminService, IMapper mapper, IReportService reportService, IClassService classService)
        {
            _adminService = adminService;
            _classService = classService;
            _mapper = mapper;
            _reportService = reportService;
        }
        /// <summary>
        /// Get all admin with pagination
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns>all admin with pagination</returns>
        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<AdminResponse>>>> ViewAdminList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecords, IEnumerable<Admin> adminList) = await _adminService.GetAdminList(paginationParameter);

            IEnumerable<AdminResponse> adminResponses = _mapper.Map<IEnumerable<AdminResponse>>(adminList);
            if (totalRecords == 0)
            {
                return NotFound(new ResponseDTO(404, "Admin not found!"));
            }
            return Ok(new PaginationResponse<IEnumerable<AdminResponse>>(totalRecords, adminResponses));
        }
        /// <summary>
        /// Get admin profile
        /// </summary>
        /// <param name="id">admin id</param>
        /// <returns>200 :Admin profile /404: not found</returns>
        [HttpGet("{id:int}/profile")]
        public async Task<ActionResult<AdminProfileDetail>> ViewAdminProfile(int id)
        {
            Admin admin = await _adminService.GetAdminById(id);
            if (admin == null)
            {
                return NotFound(new ResponseDTO(404, "Admin not found!"));
            }
            AdminProfileDetail adminResponse = _mapper.Map<AdminProfileDetail>(admin);
            return adminResponse;
        }

        /// <summary>
        /// Admin dashboard with class detail report
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="classId"></param>
        /// <param name="reportAt"></param>
        /// <returns>200: Report data / 400: There is a problem with client request</returns>
        [HttpGet("dashboard/{adminId:int}")]
        public async Task<ActionResult<AdminDashboard>> ViewAdminDashboard(int adminId, [FromQuery] int classId, [FromQuery] DateTime reportAt)
        {
            // Check admin id
            var adminDetail = await _adminService.GetAdminById(adminId);
            if (adminDetail == null)
            {
                return BadRequest(new ResponseDTO(400, "Admin id not found"));
            }

            // Check is class belong to admin
            List<Class> classList = (List<Class>)await _classService.GetClassListByAdminId(adminId, reportAt);
            if (classId == 0)
            {
                classId = classList.Select(c => c.ClassId).FirstOrDefault();
                if (classId == 0)
                {
                    return BadRequest(new ResponseDTO(400, "Admin does not have any classes are studying"));
                }
            }
            else if (classList == null || classList.Where(c => c.ClassId == classId).FirstOrDefault() == null)
            {
                return BadRequest(new ResponseDTO(400, "Class id does not belong to admin id"));
            }

            // Check report time is in range from start to end date of class
            var clazz = classList.Where(c => c.ClassId == classId).FirstOrDefault();
            if (!(reportAt >= clazz.StartDay && reportAt <= clazz.EndDay) && reportAt != default(DateTime))
            {
                return BadRequest(new ResponseDTO(400, "Report time can be only from " + clazz.StartDay + " to " + clazz.EndDay));
            }

            // Get dashboard value for chart
            var classStatus = _reportService.GetClassStatusReport(classId);
            var checkPoint = await _reportService.GetCheckpointReport(classId);
            List<FeedbackReport> feedbackReports = new List<FeedbackReport>();
            if (reportAt != default(DateTime))
            {
                feedbackReports = _reportService.GetFeedbackReport(classId, reportAt);
            }
            else
            {
                feedbackReports = _reportService.GetFeedbackReport(classId);
            }
            var adminDashBoard = new AdminDashboard
            {
                ClassStatus = classStatus,
                Checkpoint = checkPoint,
                Feedback = (feedbackReports.Count > 1) ? feedbackReports.Where(f => f.IsSumary == true).FirstOrDefault() : feedbackReports.FirstOrDefault()
            };
            return Ok(adminDashBoard);
        }
    }
}