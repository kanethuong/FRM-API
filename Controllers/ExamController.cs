using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DTO.ExamDTO;
using kroniiapi.DTO.PaginationDTO;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamController : ControllerBase
    {
        public ExamController()
        {
            
        }
        [HttpPost]
        public async Task<ActionResult> CreateNewExam()
        {
            return null;
        }
        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<ExamResponse>>>> ViewExamList()
        {
            return null;
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ChangeExamInfo(int id, int duration, DateTime ExamDay)
        {
            return null;
        }
        [HttpPut("cancel/{id:int}")]
        public async Task<ActionResult> CancelExam(int id)
        {
            return null;
        }
    }
}