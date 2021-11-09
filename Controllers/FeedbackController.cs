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
        /// Send new feedback to system. Every trainee can send upto a number of month they are learning
        /// If trainee resend a feedbacck in a month update it not insert.
        /// </summary>
        /// <param name="trainerFeedbackInput">detail of feedback</param>
        /// <returns>201: created / 400: / 404: trainee not found</returns>
        [HttpPost]
        public async Task<ActionResult> SendTrainer([FromBody] FeedbackInput FeedbackInput)
        {
            return null;
        }

        /// <summary>
        /// Get the trainer feedbacks and admin feedbacks of a class
        /// </summary>
        /// <param name="id">id of a class</param>
        /// <returns>200: List of feedback / 404: Class id not found</returns>
        [HttpGet("{classId:int}")]
        public async Task<ActionResult<ICollection<FeedbackResponse>>> ViewClassFeedback(int classId)
        {
            return null;
        }
    }
}