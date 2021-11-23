using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TrainerDTO;
using kroniiapi.Helper;
using kroniiapi.Helper.UploadDownloadFile;
using kroniiapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        private readonly IImgHelper _imgHelper;
        public TrainerController(IMapper mapper, ITrainerService trainerService, IFeedbackService feedbackService, ICalendarService calendarService, IRoomService roomService, IImgHelper imgHelper)
        {
            _mapper = mapper;
            _trainerService = trainerService;
            _feedbackService = feedbackService;
            _calendarService = calendarService;
            _roomService = roomService;
            _imgHelper = imgHelper;
        }

        /// <summary>
        /// Get all trainer 
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        [HttpGet("page")]
        [Authorize(Policy = "TrainerGet")]
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
        [Authorize(Policy = "TrainerGet")]
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
        /// Get the list module in 1 month
        /// </summary>
        /// <param name="id">Trainer id</param>
        /// <param name="date"></param>
        /// <returns>list module in 1 month / 404: Trainer Not found</returns>
        [HttpGet("{id:int}/timetable")]
        [Authorize(Policy = "TrainerGet")]
        public async Task<ActionResult<IEnumerable<TrainerTimeTable>>> ViewTimeTable(int id, DateTime date)
        {
            TimeSpan oneday = new TimeSpan(23, 59, 59);
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            startDate = startDate.AddMonths(-1);
            endDate = endDate.AddMonths(1);
            endDate = endDate.Add(oneday);

            var checkTrainer = await _trainerService.GetTrainerById(id);
            if (checkTrainer == null)
            {
                return NotFound(new ResponseDTO(404, "Trainer not found!"));
            }
            var calendars = await _calendarService.GetCalendarsByTrainerId(id, startDate, endDate);

            IEnumerable<TrainerTimeTable> trainerTimeTables = _mapper.Map<IEnumerable<TrainerTimeTable>>(calendars);
            if (trainerTimeTables.Count() != 0)
            {
                foreach (var item in trainerTimeTables)
                {
                    Room r = await _roomService.GetRoom(item.ClassId, item.ModuleId);
                    if (r == null)
                    {
                        item.RoomName = null;
                    }
                    else
                    {
                        item.RoomName = r.RoomName;
                    }
                }
            }
            return Ok(trainerTimeTables);
        }

        /// <summary>
        /// View Trainee module in two day
        /// </summary>
        /// <param name="id">Trainer id</param>
        /// <returns>200 :trainee module (two day) / 404: Trainee not found or Class has been deactivated</returns>
        [HttpGet("{id:int}/dashboard")]
        [Authorize(Policy = "TrainerGet")]
        public async Task<ActionResult<IEnumerable<TrainerDashboard>>> ViewTrainerDashboard(int id)
        {
            if (id == 0 || _trainerService.CheckTrainerExist(id) == false)
            {
                return NotFound(new ResponseDTO(404, "Trainer cannot be found"));
            }
            TimeSpan oneSecond = new TimeSpan(00, 00, -1);
            var calendars = await _calendarService.GetCalendarsByTrainerId(id, DateTime.Today, DateTime.Today.AddDays(2).Add(oneSecond));
            IEnumerable<TrainerDashboard> tdbDto = _mapper.Map<IEnumerable<TrainerDashboard>>(calendars);
            foreach (var item in tdbDto)
            {
                Room r = await _roomService.GetRoom(item.ClassId, item.ModuleId);
                item.RoomName = r.RoomName;
            }
            return Ok(tdbDto);
        }

        /// <summary>
        /// Edit trainer profile
        /// </summary>
        /// <param name="id">trainer id</param>
        /// <param name="trainerProfileDetail">detail trainer profile</param>
        /// <returns>200: Updated / 409: Conflict / 404: Profile not found</returns>
        [HttpPut("{id:int}/profile")]
        [Authorize(Policy = "TrainerPut")]
        public async Task<ActionResult> EditProfile(int id, [FromBody] TrainerProfileDetailInput trainerProfileDetail)
        {
            Trainer trainer = _mapper.Map<Trainer>(trainerProfileDetail);
            Trainer existedTrainer = await _trainerService.GetTrainerById(id);
            if (existedTrainer == null)
            {
                return NotFound(new ResponseDTO(404, "Trainer profile cannot be found"));
            }
            else if (
               existedTrainer.Fullname.ToLower().Equals(trainer.Fullname.ToLower()) &&
               existedTrainer.Phone.ToLower().Equals(trainer.Phone.ToLower()) &&
               existedTrainer.DOB.ToString().ToLower().Equals(trainer.DOB.ToString().ToLower()) &&
               existedTrainer.Address.ToLower().Equals(trainer.Address.ToLower()) &&
               existedTrainer.Gender.ToLower().Equals(trainer.Gender.ToLower()) &&
               existedTrainer.Facebook.ToLower().Equals(trainer.Facebook.ToLower())
           )
            {
                return Ok(new ResponseDTO(200, "Update profile success"));
            }
            int rs = await _trainerService.UpdateTrainer(id, trainer);
            if (rs == 0)
            {
                return Conflict(new ResponseDTO(409, "Fail to update trainer profile"));
            }
            else
            {
                return Ok(new ResponseDTO(200, "Update profile success"));
            }
        }

        /// <summary>
        /// Update trainer avatar
        /// </summary>
        /// <param name="id">Trainer id</param>
        /// <param name="image">The avatar to update</param>
        /// <returns>200: Update avatar success / 404: Trainer profile cannot be found / 409: Conflict</returns>
        [HttpPut("{id:int}/avatar")]
        [Authorize(Policy = "TrainerPut")]
        public async Task<ActionResult> UpdateAvatar(int id, [FromForm] IFormFile image)
        {
            (bool isImage, string errorMsg) = FileHelper.CheckImageExtension(image);
            if (isImage == false)
            {
                return Conflict(new ResponseDTO(409, errorMsg));
            }

            if (_trainerService.CheckTrainerExist(id) == false)
            {
                return NotFound(new ResponseDTO(404, "Trainer profile cannot be found"));
            }
            string fileName = ContentDispositionHeaderValue.Parse(image.ContentDisposition).FileName.Trim('"');
            Stream stream = image.OpenReadStream();
            long fileLength = image.Length;
            string fileType = image.ContentType;

            string avatarUrl = await _imgHelper.Upload(stream, fileName, fileLength, fileType);
            int rs = await _trainerService.UpdateAvatar(id, avatarUrl);
            if (rs == 0)
            {
                return Conflict(new ResponseDTO(409, "Fail to update trainer avatar"));
            }
            else
            {
                return Ok(new ResponseDTO(200, "Update avatar success"));
            }
        }
    }
}