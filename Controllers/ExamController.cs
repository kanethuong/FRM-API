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
        /// <summary>
        /// create new exam 
        /// </summary>
        /// <param name="newExamInput">new exam input, include trainee list id< to take exam/param>
        /// <param name="classId">classId to take exam (optional), if user send classId, get all trainee in the class</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateNewExam(NewExamInput newExamInput, int? classId = null)
        {
            return null;
        }
        /// <summary>
        /// View all exam with pagination
        /// </summary>
        /// <returns></returns>
        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<ExamResponse>>>> ViewExamList([FromQuery] PaginationParameter paginationParameter)
        {
            return null;
        }
        /// <summary>
        /// Change Exam infomation, only duration and exam day
        /// </summary>
        /// <param name="id">id of exam</param>
        /// <param name="duration"></param>
        /// <param name="ExamDay"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ChangeExamInfo(int id, int duration, DateTime ExamDay)
        {
            return null;
        }
        /// <summary>
        /// Cancel an exam
        /// </summary>
        /// <param name="id">id of exam</param>
        /// <returns></returns>
        [HttpPut("cancel/{id:int}")]
        public async Task<ActionResult> CancelExam(int id)
        {
            return null;
        }
    }
}