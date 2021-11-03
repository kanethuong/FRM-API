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
        private readonly IFeedbackService _feedbackService;
        private readonly IMapper _mapper;
        public TrainerController(IMapper mapper, ITrainerService trainerService, IFeedbackService feedbackService)
        {
            _mapper = mapper;
            _trainerService = trainerService;
            _feedbackService = feedbackService;
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
            Trainer trainer = await _trainerService.GetTrainerById(id);
            if (trainer == null)
            {
                return NotFound(new ResponseDTO(404, "Trainer cannot be found"));
            }
            return Ok(_mapper.Map<TrainerProfileDetail>(trainer));
        }

        /// <summary>
        /// Get list of trainer feedbacks
        /// </summary>
        /// <param name="id">trainer id</param>
        /// <returns>200: list of feedback / 404: trainer not found</returns>
        [HttpGet("{id:int}/feedback")]
        public async Task<ActionResult<IEnumerable<FeedbackContent>>> ViewTrainerFeedback(int id)
        {
            if(_trainerService.CheckTrainerExist(id)==false){
                return NotFound(new ResponseDTO(404,"Trainer cannot be found"));
            }
            IEnumerable<TrainerFeedback> listFeedback = await _feedbackService.GetTrainerFeedbackByTrainerId(id);
            IEnumerable<FeedbackContent> feedbackContents=_mapper.Map<IEnumerable<FeedbackContent>>(listFeedback);
            if(!feedbackContents.Any()){
                return NotFound(new ResponseDTO(404,"Currently there is no feedback about this trainer"));
            }
            return Ok(feedbackContents);
        }

        /// <summary>
        /// Update trainer wage
        /// </summary>
        /// <param name="id">trainer id</param>
        /// <param name="wage">wage of trainer</param>
        /// <returns>200: Update trainer wage success / 404: Trainer cannot be found / 400,409: Fail to update trainer wage</returns>
        [HttpPut("{id:int}/wage")]
        public async Task<ActionResult> UpdateTrainerWage(int id, [FromBody]decimal wage)
        {
            Trainer trainer = await _trainerService.GetTrainerById(id);
            if(trainer==null){
                return NotFound(new ResponseDTO(404,"Trainer cannot be found"));
            }

            if(wage<=0){
                return BadRequest(new ResponseDTO(400,"Fail to update trainer wage"));
            }

            trainer.Wage=wage;
            if(await _trainerService.UpdateTrainer(id,trainer)==1){
                return Ok(new ResponseDTO(200,"Update trainer wage success"));
            }else{
                return Conflict(new ResponseDTO(409,"Fail to update trainer wage"));
            }
        }

    }
}