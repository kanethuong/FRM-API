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
using Microsoft.AspNetCore.Http;
using kroniiapi.Helper;
using System.Net.Http.Headers;
using System.IO;
using kroniiapi.Helper.UploadDownloadFile;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Authorize(Policy = "Admin")]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {

        private readonly IAdminService _adminService;
        private readonly IReportService _reportService;
        private readonly IClassService _classService;
        private readonly IMapper _mapper;
        private readonly IImgHelper _imgHelper;

        public AdminController(IAdminService adminService, IMapper mapper, IReportService reportService, IClassService classService, IImgHelper imgHelper)
        {
            _adminService = adminService;
            _classService = classService;
            _mapper = mapper;
            _reportService = reportService;
            _imgHelper = imgHelper;
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
        /// Edit admin profile
        /// </summary>
        /// <param name="id">admin id</param>
        /// <param name="adminProfileDetail">detail admin profile</param>
        /// <returns>200: Updated / 409: Conflict / 404: Profile not found</returns>
        [HttpPut("{id:int}/profile")]
        public async Task<ActionResult> EditProfile(int id, [FromBody] AdminProfileDetailInput adminProfileDetailInput)
        {
            Admin admin = _mapper.Map<Admin>(adminProfileDetailInput);
            Admin existedAdmin = await _adminService.GetAdminById(id);
            if (existedAdmin == null)
            {
                return NotFound(new ResponseDTO(404, "Admin profile cannot be found"));
            }
            else if (
               existedAdmin.Fullname.ToLower().Equals(admin.Fullname.ToLower()) &&
               existedAdmin.Phone.ToLower().Equals(admin.Phone.ToLower()) &&
               existedAdmin.DOB.ToString().ToLower().Equals(admin.DOB.ToString().ToLower()) &&
               existedAdmin.Address.ToLower().Equals(admin.Address.ToLower()) &&
               existedAdmin.Gender.ToLower().Equals(admin.Gender.ToLower()) &&
               existedAdmin.Facebook.ToLower().Equals(admin.Facebook.ToLower())
            )
            {
                return Ok(new ResponseDTO(200, "Update profile success"));
            }
            int rs = await _adminService.UpdateAdmin(id, admin);
            if (rs == 0)
            {
                return Conflict(new ResponseDTO(409, "Fail to update admin profile"));
            }
            else
            {
                return Ok(new ResponseDTO(200, "Update profile success"));
            }
        }

        /// <summary>
        /// Update admin avatar
        /// </summary>
        /// <param name="id">admin id</param>
        /// <param name="image">The avatar to update</param>
        /// <returns>200: Update avatar success / 404: Admin profile cannot be found / 409: Conflict</returns>
        [HttpPut("{id:int}/avatar")]
        public async Task<ActionResult> UpdateAvatar(int id, [FromForm] IFormFile image)
        {
            (bool isImage, string errorMsg) = FileHelper.CheckImageExtension(image);
            if (isImage == false)
            {
                return Conflict(new ResponseDTO(409, errorMsg));
            }

            if (_adminService.CheckAdminExist(id)==false)
            {
                return NotFound(new ResponseDTO(404, "Admin profile cannot be found"));
            }
            string fileName = ContentDispositionHeaderValue.Parse(image.ContentDisposition).FileName.Trim('"');
            Stream stream = image.OpenReadStream();
            long fileLength = image.Length;
            string fileType = image.ContentType;

            string avatarUrl = await _imgHelper.Upload(stream, fileName, fileLength, fileType);
            int rs = await _adminService.UpdateAvatar(id, avatarUrl);
            if (rs == 0)
            {
                return Conflict(new ResponseDTO(409, "Fail to update admin avatar"));
            }
            else
            {
                return Ok(new ResponseDTO(200, "Update avatar success"));
            }
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
                return BadRequest(new ResponseDTO(400, "Cannot find class was studying in input time"));
            }

            // Check report time is in range from start to end date of class
            var clazz = classList.Where(c => c.ClassId == classId).FirstOrDefault();

            // Re-format start day and end day of class to check yyyy-MM only
            var classStartDay = new DateTime(clazz.StartDay.Year, clazz.StartDay.Month, 1);
            var classEndDay = new DateTime(clazz.EndDay.Year, clazz.EndDay.Month, DateTime.DaysInMonth(clazz.EndDay.Year, clazz.EndDay.Month), 23, 59, 59);

            if (reportAt != default(DateTime) && !(reportAt >= classStartDay && reportAt <= classEndDay))
            {
                return BadRequest(new ResponseDTO(400, "Report time can be only from " + clazz.StartDay.ToString("MMM-yyyy") + " to " + clazz.EndDay.ToString("MMM-yyyy")));
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
                ClassStatus = (classStatus == null) ? new ClassStatusReport() : classStatus,
                Checkpoint = (checkPoint == null) ? new CheckpointReport() : checkPoint,
                Feedback = (feedbackReports.Count > 1) ? feedbackReports.Where(f => f.IsSumary == true).FirstOrDefault() : feedbackReports.FirstOrDefault()
            };
            return Ok(adminDashBoard);
        }
    }
}