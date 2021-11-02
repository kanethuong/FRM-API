using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TrainerDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainerController : ControllerBase
    {
        private readonly ITrainerService _trainerService;
        private readonly IMapper _mapper;
        public TrainerController(IMapper mapper, ITrainerService trainerService)
        {
            _mapper = mapper;
            _trainerService = trainerService;
        }

        /// <summary>
        /// Get all trainer 
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TrainerResponse>>>> ViewTrainerList([FromQuery] PaginationParameter paginationParameter)
        {

            (int totalRecord, IEnumerable<Trainer> listTrainer) = await _trainerService.GetAllTrainer(paginationParameter);

            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Search trainer not found"));
            }
            var trainers = _mapper.Map<IEnumerable<TrainerResponse>>(listTrainer);

            return Ok(new PaginationResponse<IEnumerable<TrainerResponse>>(totalRecord, trainers));
        }

        /// <summary>
        /// get trainer profile
        /// </summary>
        /// <param name="id">trainer id</param>
        /// <returns>200: trainer profile / 404: not found</returns>
        [HttpGet("{id:int}/profile")]
        public async Task<ActionResult<TrainerProfileDetail>> ViewTrainerProfile(int id)
        {
            return null;
        }

        /// <summary>
        /// Get list of trainer feedbacks
        /// </summary>
        /// <param name="id">trainer id</param>
        /// <returns></returns>
        [HttpGet("{id:int}/feedback")]
        public async Task<ActionResult<IEnumerable<FeedbackContent>>> ViewTrainerFeedback(int id)
        {
            return null;
        }

        /// <summary>
        /// Update trainer qage
        /// </summary>
        /// <param name="id">trainer id</param>
        /// <param name="wage"></param>
        /// <returns></returns>
        [HttpPut("{id:int}/wage")]
        public async Task<ActionResult> UpdateTrainerWage(int id, decimal wage)
        {
            return null;
        }

    }
}