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
        public FeedbackController(IClassService classService,
                                 IFeedbackService feedbackService,
                                 IMapper mapper)
        {
            _classService = classService;
            _feedbackService = feedbackService;
            _mapper = mapper;
        }
        /// <summary>
        /// View Trainer and admin information to be ready to send feedback
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <returns>trainee and admin info </returns>
        [HttpGet("{traineeId:int}/feedbackInfo")]
        public async Task<ActionResult<FeedbackViewForTrainee>> ViewFeedback(int traineeId)
        {
            var whoToFeedback = await _classService.GetFeedbackViewForTrainee(traineeId);
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
        [HttpPost("trainer")]
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
        [HttpPost("admin")]
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
    }
}