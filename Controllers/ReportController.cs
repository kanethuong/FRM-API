using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DTO;
using kroniiapi.DTO.ReportDTO;
using kroniiapi.Services;
using kroniiapi.Services.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IClassService _classService;

        public ReportController(IReportService reportService, IClassService classService)
        {
            _reportService = reportService;
            _classService = classService;
        }

        /// <summary>
        /// Export class report detail to an excel file then return it to user
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <param name="at">Choose time to export report</param>
        /// <returns></returns>
        [HttpGet("{classId:int}")]
        // [Authorize(Policy = "ReportGet")]
        public async Task<ActionResult> GenerateReport(int classId, [FromQuery] DateTime reportAt = default(DateTime))
        {
            var classGet = await _classService.GetClassByClassID(classId);
            if (classGet == null)
            {
                return NotFound(new ResponseDTO(404, "Class cannot be found"));
            };
            if (reportAt == default(DateTime))
            {
                var stream = await _reportService.GenerateTotalClassReport(classId);
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{classGet.ClassName}_Report.xlsx");
            }
            if ((reportAt.Month > classGet.EndDay.Month && reportAt.Year > classGet.EndDay.Year)
                || (reportAt.Month < classGet.StartDay.Month && reportAt.Year < classGet.StartDay.Year))
            {
                return BadRequest(new ResponseDTO(400, "Time report at is out of range"));
            }
            var stream1 = await _reportService.GenerateClassReportEachMonth(classId, reportAt);
            return File(stream1, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{classGet.ClassName}_Report {reportAt.Month}/{reportAt.Year}.xlsx");

        }
        [HttpGet("attendance/{classId:int}")]
        public async Task<ActionResult> GetAttReport(int classId, DateTime month = default(DateTime))
        {
            // var rs = new Dictionary<int, List<AttendanceReport>>();
            if (month == default(DateTime))
            {
                return Ok(_reportService.GetTotalAttendanceReports(classId));
            }
            else
                return Ok(_reportService.GetAttendanceReportEachMonth(classId, month));
        }
        [HttpGet("info/{classId:int}")]
        public async Task<ActionResult> GetTraineesInfo(int classId)
        {

            return Ok(_reportService.GetTraineesInfo(classId));
        }
        [HttpGet("status/{classId:int}")]
        public async Task<ActionResult> GetClassStatusReport(int classId)
        {

            return Ok(_reportService.GetClassStatusReport(classId));
        }
        [HttpGet("GetAttendanceInfo/{classId:int}")]
        public async Task<ActionResult> GetAttendanceInfo(int classId, DateTime reportAt = default(DateTime))
        {

            return Ok(await _reportService.GetAttendanceInfo(classId, reportAt));
        }
        [HttpGet("GetRewardAndPenaltyScore/{classId:int}")]
        public async Task<ActionResult> GetRewardAndPenaltyScore(int classId, DateTime reportAt = default(DateTime))
        {

            return Ok(_reportService.GetRewardAndPenaltyScore(classId, reportAt));
        }
        [HttpGet("GetTraineeGPAs/{classId:int}")]
        public async Task<ActionResult> GetTraineeGPAs(int classId, DateTime reportAt = default(DateTime))
        {

            return Ok(await _reportService.GetTraineeGPAs(classId, reportAt));
        }
        [HttpGet("GetTopicGrades/{classId:int}")]
        public async Task<ActionResult> GetTopicGrades(int classId, DateTime reportAt = default(DateTime))
        {

            return Ok(_reportService.GetTopicGrades(classId));
        }
        [HttpGet("GetTraineeFeedbacks/{classId:int}")]
        public async Task<ActionResult> GetTraineeFeedbacks(int classId, DateTime reportAt = default(DateTime))
        {

            return Ok(_reportService.GetTraineeFeedbacks(classId, reportAt));
        }
        [HttpGet("GetAllTraineeFeedbacks/{classId:int}")]
        public async Task<ActionResult> GetAllTraineeFeedbacks(int classId)
        {

            return Ok(_reportService.GetAllTraineeFeedbacks(classId));
        }
        [HttpGet("GetFeedbackReport/{classId:int}")]
        public async Task<ActionResult> GetFeedbackReport(int classId, DateTime reportAt = default(DateTime))
        {

            return Ok(_reportService.GetFeedbackReport(classId, reportAt));
        }
    }
}