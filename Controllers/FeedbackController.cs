using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IClassService _classService;
        private readonly IFeedbackService _feedbackService;
        private readonly IAdminService _adminService;
        private readonly ITrainerService _trainerService;
        private readonly ITraineeService _traineeService;
        public FeedbackController(IClassService classService,
                                 IFeedbackService feedbackService,
                                 IMapper mapper,
                                 IAdminService adminService,
                                 ITrainerService trainerService, ITraineeService traineeService)
        {
            _classService = classService;
            _feedbackService = feedbackService;
            _mapper = mapper;
            _adminService = adminService;
            _trainerService = trainerService;
            _traineeService = traineeService;
        }
        /// <summary>
        /// View Trainer and admin information to be ready to send feedback
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <returns>trainee and admin info </returns>
        [HttpGet("{traineeId:int}/feedbackInfo")]
        public async Task<ActionResult<FeedbackViewForTrainee>> ViewFeedback(int traineeId)
        {
            var (classId, message) = await _traineeService.GetClassIdByTraineeId(traineeId);
            if (classId == -1)
            {
                return NotFound(new ResponseDTO(404, message));
            }
            var whoToFeedback = await _classService.GetFeedbackViewForTrainee(traineeId);
            if (whoToFeedback == null)
            {
                return NotFound(new ResponseDTO(404, "There are no Trainee"));
            }
            return Ok( whoToFeedback);
        }

        /// <summary>
        /// send trainer feedback
        /// </summary>
        /// <param name="trainerFeedbackInput">detail of feedback</param>
        /// <returns>201: created / </returns>
        [HttpPost("trainer")]
        public async Task<ActionResult> SendTrainerFeedback([FromBody] TrainerFeedbackInput trainerFeedbackInput)
        {
            var (classId, message) = await _traineeService.GetClassIdByTraineeId(trainerFeedbackInput.TraineeId);
            if (classId == -1)
            {
                return NotFound(new ResponseDTO(404, message));
            }
            TrainerFeedback trainerFeedback = _mapper.Map<TrainerFeedback>(trainerFeedbackInput);
            var (rs, feedbackMessage) = await _feedbackService.InsertNewTrainerFeedback(trainerFeedback);
            if (trainerFeedbackInput.Rate < 0 || trainerFeedback.Rate >5)
            {
                return BadRequest(new ResponseDTO(400, "Rate must be between 1 and 5"));
            }
            if (rs == -1)
            {
                return NotFound(new ResponseDTO(404, feedbackMessage));
            }
            if (rs == 0)
            {
                return NotFound(new ResponseDTO(404, feedbackMessage));
            }
            if (rs == 1)
            {
                return Ok(new ResponseDTO(200, feedbackMessage));
            }
            return BadRequest(new ResponseDTO(400, "Failed To Insert"));
        }

        /// <summary>
        /// send admin feedback
        /// </summary>
        /// <param name="adminFeedbackInput">detail of feedback</param>
        /// <returns>201: created / </returns>
        [HttpPost("admin")]
        public async Task<ActionResult> SendAdminFeedback([FromBody] AdminFeedbackInput adminFeedbackInput)
        {
            var (classId, message) = await _traineeService.GetClassIdByTraineeId(adminFeedbackInput.TraineeId);
            if (classId == -1)
            {
                return NotFound(new ResponseDTO(404, message));
            }
            if (adminFeedbackInput.Rate < 0 || adminFeedbackInput.Rate > 5)
            {
                return BadRequest(new ResponseDTO(400, "Rate must be between 1 and 5"));
            }
            AdminFeedback adminFeedback = _mapper.Map<AdminFeedback>(adminFeedbackInput);
            var (rs, feedbackMessage) = await _feedbackService.InsertNewAdminFeedback(adminFeedback);
            if (rs == -1)
            {
                return NotFound(new ResponseDTO(404, feedbackMessage));
            }
            if (rs == 0)
            {
                return NotFound(new ResponseDTO(404, feedbackMessage));
            }
            if (rs == 1)
            {
                return Ok(new ResponseDTO(200, feedbackMessage));
            }
            return BadRequest(new ResponseDTO(400, "Failed To Insert"));
        }
        /// <summary>
        /// Get the trainer feedbacks and admin feedbacks of a class
        /// </summary>
        /// <param name="id">id of a class</param>
        /// <returns>200: List of trainer feedback and admin feedback</returns>
        [HttpGet("{classId:int}")]
        public async Task<ActionResult<FeedbackResponse>> ViewClassFeedback(int classId)
        {
            Admin admin1 = await _adminService.getAdminByClassId(classId);
            Trainer trainer1 = await _trainerService.getTrainerByClassId(classId);

            if (admin1 == null || trainer1 == null)
            {
                return NotFound(new ResponseDTO(404, "Class not found"));
            }

            FeedbackResponse feedbackResponses = new FeedbackResponse();

            IEnumerable<TrainerFeedback> trainerFeedbacks = await _feedbackService.GetTrainerFeedbacksByAdminId(trainer1.TrainerId);
            IEnumerable<AdminFeedback> adminFeedbacks = await _feedbackService.GetAdminFeedbacksByAdminId(admin1.AdminId);

            IEnumerable<FeedbackContent> TrainerfeedbackContents = _mapper.Map<IEnumerable<FeedbackContent>>(trainerFeedbacks);
            IEnumerable<FeedbackContent> AdminfeedbackContents = _mapper.Map<IEnumerable<FeedbackContent>>(adminFeedbacks);


            TrainerFeedbackResponse trainerFeedbackResponse = new();
            trainerFeedbackResponse.Trainer = _mapper.Map<TrainerInFeedbackResponse>(trainer1);
            trainerFeedbackResponse.Feedbacks = TrainerfeedbackContents;

            AdminFeedbackResponse adminFeedbackResponse = new();
            adminFeedbackResponse.Admin = _mapper.Map<AdminInFeedbackResponse>(admin1);
            adminFeedbackResponse.Feedbacks = AdminfeedbackContents;

            feedbackResponses.TrainerFeedback = trainerFeedbackResponse;
            feedbackResponses.AdminFeedback = adminFeedbackResponse;

            return feedbackResponses;

        }
    }
}