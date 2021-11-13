using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DTO;
using kroniiapi.DTO.ReportDTO;
using kroniiapi.Services.Report;
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
        public async Task<ActionResult> GenerateReport(int classId, [FromQuery] DateTimeOffset? at = null)
        {
            return null;
        }
    }
}