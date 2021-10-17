using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ClassDetailDTO;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.MarkDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;
        private readonly IMapper _mapper;

        public ClassController(IClassService classService, IMapper mapper)
        {
            _classService = classService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of class in db with pagination
        /// </summary>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List with pagination / 404: class name not found</returns>
        [HttpGet("page")]
        public async Task<ActionResult> GetClassList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<Class> classList) = await _classService.GetClassList(paginationParameter);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Classes not found"));
            }
            return Ok(new PaginationResponse<IEnumerable<Class>>(totalRecord, classList));
        }

        /// <summary>
        /// Get list of request delete class with pagination
        /// </summary>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of class with pagination / 404: search class name not found</returns>
        [HttpGet("request")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<RequestDeleteClassResponse>>>> GetDeleteClassRequestList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecords, IEnumerable<DeleteClassRequest> deleteClassRequests) = await _classService.GetRequestDeleteClassList(paginationParameter);
            IEnumerable<RequestDeleteClassResponse> RequestDeleteClassDTO = _mapper.Map<IEnumerable<RequestDeleteClassResponse>>(deleteClassRequests);
            if (totalRecords == 0)
            {
                return NotFound(new ResponseDTO(404, "Class name not found!"));
            }
            return Ok(new PaginationResponse<IEnumerable<RequestDeleteClassResponse>>(totalRecords, RequestDeleteClassDTO));
        }

        /// <summary>
        /// Update delete class request and if accept delete request then deactivate that class
        /// </summary>
        /// <param name="confirmDeleteClassInput">Confirm detail</param>
        /// <returns>200: Update done / 404: Class or request not found / 409: Class or request deactivated</returns>
        [HttpPut("request")]
        public async Task<ActionResult> ConfirmDeleteClassRequest([FromBody] ConfirmDeleteClassInput confirmDeleteClassInput)
        {
            int status = await _classService.UpdateDeletedClass(confirmDeleteClassInput);
            if (status == -1)
            {
                return NotFound(new ResponseDTO(404, "Class or request not found"));
            }
            else if (status == 1)
            {
                return Ok(new ResponseDTO(200, "Update done"));
            }
            else
            {
                return Ok(new ResponseDTO(409, "Class or request deactivated"));
            }
        }

        /// <summary>
        /// Get list of request delete class with pagination
        /// </summary>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of class with pagination / 404: search class name not found</returns>
        [HttpGet("deleted")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<DeleteClassResponse>>>> GetDeactivatedClass([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<Class> deletedClass) = await _classService.GetDeletedClassList(paginationParameter);
            IEnumerable<DeleteClassResponse> deletedClassDTO = _mapper.Map<IEnumerable<Class>, IEnumerable<DeleteClassResponse>>(deletedClass);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "List empty"));
            }
            return Ok(new PaginationResponse<IEnumerable<DeleteClassResponse>>(totalRecord, deletedClassDTO));
        }

        /// <summary>
        /// Get the detail information of a class and student list with pagination
        /// </summary>
        /// <param name="id"> id of class</param>
        /// <returns> 200: Detail of class and student list with pagination / 404: class not found </returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClassDetailResponse>> ViewClassDetail(int id)
        {
            return null;
        }
        /// <summary>
        /// Get trainee list with pagination
        /// </summary>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of trainee list in a class with pagination / 404: search trainee name not found</returns>
        [HttpGet("trainee/{id:int}")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TraineeInClassDetail>>>> GetTraineeListByClassId(int id, [FromQuery] PaginationParameter paginationParameter)
        {
            return null;
        }
        
        /// <summary>
        /// Insert the request delete class to db
        /// </summary>
        /// <param name="requestDeleteClassInput">Request detail</param>
        /// <returns>201: Request is created / 409: Class is already deactivated</returns>
        [HttpPost("request")]
        public async Task<ActionResult> CreateRequestDeleteClass(RequestDeleteClassInput requestDeleteClassInput)
        {
            return null;
        }

        /// <summary>
        /// Get the student mark with pagination of a class
        /// </summary>
        /// <param name="id">id of class</param>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of student mark in a class with pagination / 404: search student name not found</returns>
        [HttpGet("score/{id:int}")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<MarkResponse>>>> ViewClassScore(int id, [FromQuery] PaginationParameter paginationParameter)
        {
            return null;
        }

        /// <summary>
        /// Get the trainer feedbacks and admin feedbacks of a class
        /// </summary>
        /// <param name="id">id of a class</param>
        /// <returns>200: List of trainer feedback and admin feedback</returns>
        [HttpGet("feedback/{id:int}")]
        public async Task<ActionResult<FeedbackResponse>> ViewClassFeedback(int id)
        {
            return null;
        }
        
        /// <summary>
        /// Insert new class to db
        /// </summary>
        /// <param name="newClassInput">Detail of new class</param>
        /// <returns>201: Class is created / 409: Classname exist || Trainees or trainers already have class</returns>
        [HttpPost]
        public async Task<ActionResult> CreateNewClass(NewClassInput newClassInput)
        {
            return null;
        }
        
        /// <summary>
        /// Insert a list class to db using excel file
        /// </summary>
        /// <param name="file">Excel file to stores class data</param>
        /// <returns>201: Class is created /400: File content inapproriate /409: Classname exist || Trainees or trainers already have class</returns>
        [HttpPost("excel")]
        public async Task<ActionResult> CreateNewClassByExcel([FromForm] IFormFile file)
        {
            return null;
        }
    }
}