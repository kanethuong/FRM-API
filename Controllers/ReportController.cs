using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DTO.ReportDTO;
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

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Export class report detail to an excel file then return it to user
        /// </summary>
        /// <param name="classId">Id of class</param>
        /// <param name="at">Choose time to export report</param>
        /// <returns></returns>
        [HttpGet("{classId:int}")]
        [Authorize(Policy = "ReportGet")]
        public async Task<ActionResult> GenerateReport(int classId, [FromQuery] DateTimeOffset? at = null)
        {
            return null;
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

            return Ok(_reportService.GetRewardAndPenaltyScore(classId,reportAt));
        }
        [HttpGet("GetTraineeGPAs/{classId:int}")]
        public async Task<ActionResult> GetTraineeGPAs(int classId, DateTime reportAt = default(DateTime))
        {

            return Ok(await _reportService.GetTraineeGPAs(classId));
        }
        [HttpGet("GetTopicGrades/{classId:int}")]
        public async Task<ActionResult> GetTopicGrades(int classId, DateTime reportAt = default(DateTime))
        {

            return Ok(_reportService.GetTopicGrades(classId));
        }
        [HttpGet("GetTraineeFeedbacks/{classId:int}")]
        public async Task<ActionResult> GetTraineeFeedbacks(int classId, DateTime reportAt = default(DateTime))
        {

            return Ok(_reportService.GetTraineeFeedbacks(classId,reportAt));
        }
        [HttpGet("GetAllTraineeFeedbacks/{classId:int}")]
        public async Task<ActionResult> GetAllTraineeFeedbacks(int classId)
        {

            return Ok(_reportService.GetAllTraineeFeedbacks(classId));
        }
    }
}