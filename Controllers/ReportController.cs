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
       // [Authorize(Policy = "ReportGet")]
        public async Task<ActionResult> GenerateReport(int classId, [FromQuery] DateTimeOffset? at = null)
        {
            return null;
        }
        [HttpGet("attendance/{classId:int}")]
        public async Task<ActionResult> GetAttReport(int classId, DateTime reportAt = default(DateTime))
        {
            // var rs = new Dictionary<int, List<AttendanceReport>>();
            var rs =  _reportService.GetRewardAndPenaltyScore(classId,reportAt);
            return Ok(rs);
        }

    }
}