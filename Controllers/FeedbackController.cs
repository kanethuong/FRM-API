using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Policy = "FeedbackPost")]
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
                return Created("", new ResponseDTO(201, feedbackMessage));
            }
            return BadRequest(new ResponseDTO(400, "Failed to send feedback"));
        }

        /// <summary>
        /// Get the trainer feedbacks and admin feedbacks of a class
        /// </summary>
        /// <param name="id">id of a class</param>
        /// <returns>200: List of feedback / 404: Class id not found</returns>
        [HttpGet("{classId:int}")]
        public async Task<ActionResult<ICollection<FeedbackResponse>>> ViewClassFeedback(int classId)
        {
            // if(_classService.CheckClassExist(classId)==false){
            //     return NotFound(new ResponseDTO(404,"Class not found"));
            // }
            // ICollection<Trainee> traineeList=await _traineeService.GetTraineeByClassId(classId);
            return null;
        }
    }
}