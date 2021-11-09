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
        /// send feedback
        /// </summary>
        /// <param name="feedbackInput">detail of feedback</param>
        /// <returns>201: created / </returns>
        [HttpPost]
        public async Task<ActionResult> SendFeedback([FromBody] FeedbackInput feedbackInput)
        {
            var (classId, message) = await _traineeService.GetClassIdByTraineeId(feedbackInput.TraineeId);
            if (classId == -1)
            {
                return NotFound(new ResponseDTO(404, message));
            }
            Feedback feedback = _mapper.Map<Feedback>(feedbackInput);
            var (rs, feedbackMessage) = await _feedbackService.InsertNewFeedback(feedback);
            if (rs == -1)
            {
                return NotFound(new ResponseDTO(404, feedbackMessage));
            }
            if (rs == 1)
            {
                return Created("",new ResponseDTO(201, feedbackMessage));
            }
            return BadRequest(new ResponseDTO(400, "Failed to send feedback"));
        }

        /// <summary>
        /// Get the trainer feedbacks and admin feedbacks of a class
        /// </summary>
        /// <param name="id">id of a class</param>
        /// <returns>200: List of trainer feedback and admin feedback</returns>
        [HttpGet("{classId:int}")]
        public async Task<ActionResult<FeedbackResponse>> ViewClassFeedback(int classId)
        {
            // var class1 = await _classService.GetClassByClassID(classId);
            // if (class1.IsDeactivated == true)
            // {
            //     return NotFound(new ResponseDTO(404, "Class not found!"));
            // }
            // Admin admin = await _adminService.getAdminByClassId(classId);
            // Trainer trainer = await _trainerService.getTrainerByClassId(classId);

            // if ((admin == null || trainer == null) || (admin.IsDeactivated == true || trainer.IsDeactivated == true))
            // {
            //     return NotFound(new ResponseDTO(404, "Admin or trainer not found!"));
            // }

            // FeedbackResponse feedbackResponses = new FeedbackResponse();

            // IEnumerable<TrainerFeedback> trainerFeedbacks = await _feedbackService.GetTrainerFeedbacksByClassId(classId);
            // IEnumerable<AdminFeedback> adminFeedbacks = await _feedbackService.GetAdminFeedbacksByClassId(classId);


            // IEnumerable<FeedbackContent> TrainerfeedbackContents = _mapper.Map<IEnumerable<FeedbackContent>>(trainerFeedbacks);
            // IEnumerable<FeedbackContent> AdminfeedbackContents = _mapper.Map<IEnumerable<FeedbackContent>>(adminFeedbacks);


            // TrainerFeedbackResponse trainerFeedbackResponse = new();
            // trainerFeedbackResponse.Trainer = _mapper.Map<TrainerInFeedbackResponse>(trainer);
            // trainerFeedbackResponse.Feedbacks = TrainerfeedbackContents;

            // AdminFeedbackResponse adminFeedbackResponse = new();
            // adminFeedbackResponse.Admin = _mapper.Map<AdminInFeedbackResponse>(admin);
            // adminFeedbackResponse.Feedbacks = AdminfeedbackContents;

            // feedbackResponses.TrainerFeedback = trainerFeedbackResponse;
            // feedbackResponses.AdminFeedback = adminFeedbackResponse;

            // return feedbackResponses;
            return null;
        }
    }
}