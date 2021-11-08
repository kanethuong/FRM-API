using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.MarkDTO;
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
        private readonly ICalendarService _calendarService;
        private readonly IMapper _mapper;
        public TrainerController(IMapper mapper, ITrainerService trainerService, IFeedbackService feedbackService,ICalendarService calendarService)
        {
            _mapper = mapper;
            _trainerService = trainerService;
            _feedbackService = feedbackService;
            _calendarService = calendarService;
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
            // if(_trainerService.CheckTrainerExist(id)==false){
            //     return NotFound(new ResponseDTO(404,"Trainer cannot be found"));
            // }
            // IEnumerable<TrainerFeedback> listFeedback = await _feedbackService.GetTrainerFeedbackByTrainerId(id);
            // IEnumerable<FeedbackContent> feedbackContents=_mapper.Map<IEnumerable<FeedbackContent>>(listFeedback);
            // if(!feedbackContents.Any()){
            //     return NotFound(new ResponseDTO(404,"Currently there is no feedback about this trainer"));
            // }
            // return Ok(feedbackContents);
            return null;
        }

        /// <summary>
        /// Update trainer wage
        /// </summary>
        /// <param name="id">trainer id</param>
        /// <param name="wage">wage of trainer</param>
        /// <returns>200: Update trainer wage success / 404: Trainer cannot be found / 400,409: Fail to update trainer wage</returns>
        [HttpPut("{id:int}/wage")]
        public async Task<ActionResult> UpdateTrainerWage(int id, [FromBody] decimal wage)
        {
            // Trainer trainer = await _trainerService.GetTrainerById(id);
            // if(trainer==null){
            //     return NotFound(new ResponseDTO(404,"Trainer cannot be found"));
            // }
            // if(trainer.Wage==wage){
            //     return Ok(new ResponseDTO(200,"Update trainer wage success"));
            // }
            // if(wage<0){
            //     return BadRequest(new ResponseDTO(400,"Fail to update trainer wage"));
            // }

            // trainer.Wage=wage;
            // if(await _trainerService.UpdateTrainer(id,trainer)==1){
            //     return Ok(new ResponseDTO(200,"Update trainer wage success"));
            // }else{
            //     return Conflict(new ResponseDTO(409,"Fail to update trainer wage"));
            // }
            return null;
        }

        /// <summary>
        /// Get the list module in 1 month
        /// </summary>
        /// <param name="id">Trainer id</param>
        /// <param name="date"></param>
        /// <returns>list module in 1 month</returns>
        [HttpGet("{id:int}/timetable")]
        public async Task<ActionResult<IEnumerable<TrainerTimeTable>>> ViewTimeTable(int id, DateTime date)
        {
            return null;
        }

        /// <summary>
        /// View Trainee module in two day
        /// </summary>
        /// <param name="id">Trainer id</param>
        /// <returns>200 :trainee module (two day) / 404: Trainee not found or Class has been deactivated</returns>
        [HttpGet("{id:int}/dashboard")]
        public async Task<ActionResult<IEnumerable<TrainerDashboard>>> ViewTrainerDashboard(int id)
        {
            if(id==0 || _trainerService.CheckTrainerExist(id) == false){
                return NotFound(new ResponseDTO(404,"Trainer cannot be found"));
            }
            TimeSpan oneSecond = new TimeSpan(00, 00, -1);
            var calendars = await _calendarService.GetCalendarsByTrainerId(id,DateTime.Today, DateTime.Today.AddDays(2).Add(oneSecond));
            IEnumerable<TrainerDashboard> tdbDto = _mapper.Map<IEnumerable<TrainerDashboard>>(calendars);
            return Ok(tdbDto);
        }


        /// <summary>
        /// Edit trainer profile
        /// </summary>
        /// <param name="id">trainer id</param>
        /// <param name="traineeProfileDetail">detail trainer profile</param>
        /// <returns>200: Updated / 409: Conflict / 404: Profile not found</returns>
        [HttpPut("{id:int}/profile")]
        public async Task<ActionResult> EditProfile(int id, [FromBody] TrainerProfileDetailInput trainerProfileDetail)
        {
            return null;
        }


    }
}