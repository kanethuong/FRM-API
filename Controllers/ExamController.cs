using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ExamDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;
        private readonly IMapper _mapper;

        public ExamController(IExamService examService, IMapper mapper)
        {
            _examService = examService;
            _mapper = mapper;
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
        /// /// </summary>
        /// <returns>200: All exams list / 404: Exam name not found</returns>
        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<ExamResponse>>>> ViewExamList([FromQuery] PaginationParameter paginationParameter)
        {
            var tuple = await _examService.GetExamList(paginationParameter);
            var totalRecord = tuple.Item1;
            var examList = tuple.Item2;

            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Exam not found"));
            }

            var rs = new PaginationResponse<ICollection<ExamResponse>>()
            {
                TotalRecords = totalRecord,
                Payload = _mapper.Map<ICollection<ExamResponse>>(examList)
            };

            return Ok(rs);
        }

        /// <summary>
        /// Change Exam infomation, only duration and exam day
        /// </summary>
        /// <param name="id">id of exam</param>
        /// <param name="duration"></param>
        /// <param name="ExamDay"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ChangeExamInfo(int id, [FromBody] UpdateExamInput updateExamInput)
        {
            var exam = await _examService.GetExamById(id);

            if (exam == null)
            {
                return NotFound(new ResponseDTO(404, "Exam not found"));
            }

            exam.DurationInMinute = updateExamInput.Duration;
            exam.ExamDay = updateExamInput.ExamDay;

            var rs = await _examService.UpdateExam(id, exam);

            if (rs == -1)
            {
                return BadRequest(new ResponseDTO(400, "Exam was cancelled"));
            }

            if (rs == 0)
            {
                return BadRequest(new ResponseDTO(400, "There is an error when update exam"));
            }

            return Ok(new ResponseDTO(200, "Update exam successfully"));
        }

        /// <summary>
        /// Cancel an exam
        /// </summary>
        /// <param name="id">id of exam</param>
        /// <returns>200: Cancelled / 404: Id not found, 409: Exam was cancelled, Delete fail</returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> CancelExam(int id)
        {
            (bool? status, string message) = await _examService.CancelExam(id);

            if (status == null)
            {
                return NotFound(new ResponseDTO(404, message));
            }

            if (status == false)
            {
                return Conflict(new ResponseDTO(409, message));
            }

            return Ok(new ResponseDTO(200, message));
        }
    }
}