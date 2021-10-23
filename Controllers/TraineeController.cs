using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ApplicationDTO;
using kroniiapi.DTO.ClassDetailDTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TraineeDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TraineeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IClassService _classService;
        private readonly IFeedbackService _feedbackService;
        private readonly ITraineeService _traineeService;

        public TraineeController(IMapper mapper, IClassService classService, IFeedbackService feedbackService, ITraineeService traineeService)
        {
            _mapper = mapper;
            _classService = classService;
            _feedbackService = feedbackService;
            _traineeService = traineeService;
        }

        /// <summary>
        /// View Trainee module in two day, exam in 1 1 week
        /// </summary>
        /// <param name="id"> trainee id</param>
        /// <returns>trainee module (two day) and exam (one week)</returns>
        [HttpGet("{id:int}/dashboard")]
        public async Task<ActionResult<TraineeDashboard>> ViewTraineeDashboard(int id)
        {
            return null;
        }

        /// <summary>
        /// View Trainer and admin information to be ready to send feedback
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <returns>trainee and admin info </returns>
        [HttpGet("{id:int}/feedback")]
        public async Task<ActionResult<FeedbackViewForTrainee>> ViewFeedback(int id)
        {
            var whoToFeedback = await _classService.GetFeedbackViewForTrainee(id);
            if (whoToFeedback == null)
            {
                return NotFound(new ResponseDTO(404, "There are no Trainee"));
            }
            return whoToFeedback;
        }

        /// <summary>
        /// send trainer feedback
        /// </summary>
        /// <param name="trainerFeedbackInput">detail of feedback</param>
        /// <returns>201: created / </returns>
        [HttpPost("feedback/trainer")]
        public async Task<ActionResult> SendTrainerFeedback([FromBody] TrainerFeedbackInput trainerFeedbackInput)
        {
            TrainerFeedback trainerFeedback = _mapper.Map<TrainerFeedback>(trainerFeedbackInput);
            int rs = await _feedbackService.InsertNewTrainerFeedback(trainerFeedback);
            if (rs == -1)
            {
                return NotFound(new ResponseDTO(404, "Duplicated TrainerId and TraineeId"));
            }
            if (rs == 0)
            {
                return NotFound(new ResponseDTO(404, "Don't have Trainee or Trainer"));
            }
            if (rs == 1)
            {
                return Ok(new ResponseDTO(200, "Feedback Success"));
            }
            return BadRequest(new ResponseDTO(400, "Failed To Insert"));
        }

        /// <summary>
        /// send admin feedback
        /// </summary>
        /// <param name="adminFeedbackInput">detail of feedback</param>
        /// <returns>201: created / </returns>
        [HttpPost("feedback/admin")]
        public async Task<ActionResult> SendAdminFeedback([FromBody] AdminFeedbackInput adminFeedbackInput)
        {
            AdminFeedback adminFeedback = _mapper.Map<AdminFeedback>(adminFeedbackInput);
            int rs = await _feedbackService.InsertNewAdminFeedback(adminFeedback);
            if (rs == -1)
            {
                return NotFound(new ResponseDTO(404, "Duplicated TrainerId and TraineeId"));
            }
            if (rs == 0)
            {
                return NotFound(new ResponseDTO(404, "Don't have Trainee or Trainer"));
            }
            if (rs == 1)
            {
                return Ok(new ResponseDTO(200, "Feedback Success"));
            }
            return BadRequest(new ResponseDTO(400, "Failed To Insert"));
        }
        /// <summary>
        /// View trainee profile
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <returns>Trainee profile</returns>
        [HttpGet("{id:int}/profile")]
        public async Task<ActionResult<TraineeProfileDetail>> ViewProfile(int id)
        {
            return null;
        }
        /// <summary>
        /// Edit trainee profile
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <param name="traineeProfileDetail">detail trainee profile</param>
        /// <returns>201: Updated / 409: Bad request </returns>
        [HttpPut("{id:int}/profile")]
        public async Task<ActionResult> EditProfile(int id, [FromBody] TraineeProfileDetail traineeProfileDetail)
        {
            return null;
        }

        /// <summary>
        /// Get the detail information of a class 
        /// </summary>
        /// <param name="id"> id of class</param>
        /// <returns> 200: Detail of class  / 404: class not found </returns>
        [HttpGet("{id:int}/class")]
        public async Task<ActionResult<ClassDetailResponse>> ViewClassDetail(int id)
        {
            return null;
        }
        /// <summary>
        /// Get trainee list with pagination
        /// </summary>
        /// <param name="id"> id of trainee</param>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of trainee list in a class with pagination / 404: search trainee name not found</returns>
        [HttpGet("{id:int}/class/trainee")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TraineeInClassDetail>>>> GetTraineeListInClass(int id, [FromQuery] PaginationParameter paginationParameter)
        {
            return null;
        }

        /// <summary>
        /// View trainee mark and skill
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <returns>Trainee mark and skill</returns>
        [HttpGet("{id:int}/mark")]
        public async Task<ActionResult<IEnumerable<TraineeMarkAndSkill>>> ViewMarkAndSkill(int id)
        {
            return null;
        }
        /// <summary>
        /// submit trainee certificate (upload to mega)
        /// </summary>
        /// <param name="certificateInput">detail of certificate input</param>
        /// <returns>201: created / 409: bad request</returns>
        [HttpPost("{traineeId:int}/certificate/{moduleId:int}")]
        public async Task<ActionResult> SubmitCertificate(IFormFile file, int traineeId, int moduleId)
        {
            return null;
        }

        /// <summary>
        /// View trainee attendance report
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>trainee attendance report in pagination</returns>
        [HttpGet("{id:int}/attendance")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TraineeAttendanceReport>>>> ViewAttendanceReport(int id, [FromQuery] PaginationParameter paginationParameter)
        {
            if (await _traineeService.GetTraineeById(id) == null)
            {
                return BadRequest(new ResponseDTO(404, "id not found"));
            }
            try
            {
                (int totalRecord, IEnumerable<TraineeAttendanceReport> result) = await _traineeService.GetAttendanceReports(id, paginationParameter);
                return Ok(new PaginationResponse<IEnumerable<TraineeAttendanceReport>>(totalRecord, result));
            }
            catch
            {
                return BadRequest(new ResponseDTO(404, "Undefined error, trainee may not in any class"));
            }
        }

        /// <summary>
        /// Get the uri from redis, call download from mega and return file stream
        /// </summary>
        /// <returns>FileContentResult giống trong cái controller action của cái API Mega nhe Tiên :v </returns>
        [HttpGet("rule")]
        public async Task<ActionResult<Stream>> ViewRule()
        {
            return null;
        }

        /// <summary>
        /// Get the list event in 1 month, include module and exam
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <returns>list event in 1 month, include module and exam</returns>
        [HttpGet("{id:int}/timetable")]
        public async Task<ActionResult<EventInTimeTable>> ViewTimeTable(int id)
        {

            return null;
        }

        /// <summary>
        /// get application list of trainee
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: application list </returns>
        [HttpGet("{id:int}/application")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<ApplicationResponse>>>> ViewApplicationList(int id, [FromQuery] PaginationParameter paginationParameter)
        {
            if (await _traineeService.GetTraineeById(id) == null)
            {
                return BadRequest(new ResponseDTO(404, "id not found"));
            }
            try
            {
                (int totalRecord, IEnumerable<Application> application) = await _traineeService.GetApplicationListByTraineeId(id, paginationParameter);
                IEnumerable<ApplicationResponse> applicationDTO = _mapper.Map<IEnumerable<Application>,IEnumerable<ApplicationResponse>>(application);
                return Ok(new PaginationResponse<IEnumerable<ApplicationResponse>>(totalRecord, applicationDTO));
            }
            catch
            {
                return BadRequest(new ResponseDTO(404, "Undefined error, trainee doesn't have any applicaiton"));
            }
        }

        /// <summary>
        /// Trainee submit application form (mega upload)
        /// </summary>
        /// <param name="applicationInput">detail of applcation input </param>
        /// <returns>201: created</returns>
        [HttpPost("application")]
        public async Task<ActionResult> SubmitApplicationForm(ApplicationInput applicationInput)
        {
            return null;
        }

        /// <summary>
        /// get all application type
        /// </summary>
        /// <returns>all applcation type</returns>
        [HttpGet("application")]
        public async Task<ActionResult<IEnumerable<ApplicationCategoryResponse>>> ViewApplicationType()
        {
            return null;
        }

    }

}