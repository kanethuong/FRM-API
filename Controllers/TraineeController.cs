using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ApplicationDTO;
using kroniiapi.DTO.ClassDetailDTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TraineeDTO;
using kroniiapi.Helper;
using kroniiapi.Helper.UploadDownloadFile;
using kroniiapi.Helper.Upload;
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
        private readonly IImgHelper _imgHelper;
        private readonly ICalendarService _calendarService;
        private readonly IModuleService _moduleService;
        private readonly ITrainerService _trainerService;
        private readonly IRoomService _roomService;
        private readonly IExamService _examService;

        private readonly ICertificateService _certificateService;
        private readonly IApplicationService _applicationService;
        private readonly IMegaHelper _megaHelper;
        public TraineeController(IMapper mapper,
                                 IClassService classService,
                                 IFeedbackService feedbackService,
                                 ITraineeService traineeService,
                                 ICalendarService calendarService,
                                 IModuleService moduleService,
                                 ITrainerService trainerService,
                                 IRoomService roomService,
                                 IExamService examService,
                                 ICertificateService certificateService,
                                 IApplicationService applicationService,
                                 IMegaHelper megaHelper,
                                 IImgHelper imgHelper)
        {
            _mapper = mapper;
            _classService = classService;
            _feedbackService = feedbackService;
            _traineeService = traineeService;
            _imgHelper = imgHelper;
            _calendarService = calendarService;
            _moduleService = moduleService;
            _trainerService = trainerService;
            _roomService = roomService;
            _examService = examService;
            _certificateService = certificateService;
            _applicationService = applicationService;
            _megaHelper = megaHelper;
        }

        /// <summary>
        /// View Trainee module in two day, exam in 1 1 week
        /// </summary>
        /// <param name="id"> trainee id</param>
        /// <returns>trainee module (two day) and exam (one week)</returns>
        [HttpGet("{id:int}/dashboard")]
        public async Task<ActionResult<TraineeDashboard>> ViewTraineeDashboard(int id)
        {
            var calenders = await _calendarService.GetCalendarsByTraineeId(id,DateTime.Today,DateTime.UtcNow.AddDays(2));
            Trainer trainer = await _trainerService.GetTrainerById(calenders.FirstOrDefault().Class.TrainerId);
            Room room = await _roomService.GetRoomById(calenders.FirstOrDefault().Class.RoomId);
            var exam = await _examService.GetExamListByModuleId(calenders.ToList(),DateTime.Today,DateTime.UtcNow.AddDays(2));
            //Trainee trainee = await _traineeService.GetTraineeById(id);
            
            foreach (var item in calenders)
            {
                item.Class.Trainer = trainer;
                item.Class.Room = room;
            }
            var moduleInDashboard = _mapper.Map<IEnumerable<ModuleInTraineeDashboard>>(calenders);
            var examInDashboard = _mapper.Map<IEnumerable<ExamInTraineeDashboard>>(exam);
            TraineeDashboard dashboard = new TraineeDashboard();
            dashboard.moduleInTraineeDashboards = moduleInDashboard;
            dashboard.examInTraineeDashboards = examInDashboard;
            return Ok(dashboard);
        }

        /// <summary>
        /// View trainee profile
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <returns>Trainee profile / 404: Profile not found</returns>
        [HttpGet("{id:int}/profile")]
        public async Task<ActionResult<TraineeProfileDetail>> ViewProfile(int id)
        {
            Trainee trainee = await _traineeService.GetTraineeById(id);
            if (trainee == null)
            {
                return NotFound(new ResponseDTO(404, "Trainee profile cannot be found"));
            }
            return Ok(_mapper.Map<TraineeProfileDetail>(trainee));
        }
        /// <summary>
        /// Edit trainee profile
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <param name="traineeProfileDetail">detail trainee profile</param>
        /// <returns>200: Updated / 409: Bad request / 404: Profile not found</returns>
        [HttpPut("{id:int}/profile")]
        public async Task<ActionResult> EditProfile(int id, [FromBody] TraineeProfileDetailInput traineeProfileDetail)
        {
            Trainee trainee = _mapper.Map<Trainee>(traineeProfileDetail);
            int rs = await _traineeService.UpdateTrainee(id, trainee);
            if (rs == -1)
            {
                return NotFound(new ResponseDTO(404, "Trainee profile cannot be found"));
            }
            else if (rs == 0)
            {
                return BadRequest(new ResponseDTO(409, "Fail to update trainee profile"));
            }
            else
            {
                return Ok(new ResponseDTO(200, "Update profile success"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPut("{id:int}/avatar")]
        public async Task<ActionResult> UpdateAvatar(int id, [FromForm] IFormFile image)
        {
            (bool isImage, string errorMsg) = FileHelper.CheckImageExtension(image);
            if (isImage == false)
            {
                return BadRequest(new ResponseDTO(409, errorMsg));
            }

            string fileName = ContentDispositionHeaderValue.Parse(image.ContentDisposition).FileName.Trim('"');
            Stream stream = image.OpenReadStream();
            long fileLength = image.Length;
            string fileType = image.ContentType;

            string avatarUrl = await _imgHelper.Upload(stream, fileName, fileLength, fileType);
            int rs = await _traineeService.UpdateAvatar(id,avatarUrl);
            if (rs == -1)
            {
                return NotFound(new ResponseDTO(404, "Trainee profile cannot be found"));
            }
            else if (rs == 0)
            {
                return BadRequest(new ResponseDTO(409, "Fail to update trainee avatar"));
            }
            else
            {
                return Ok(new ResponseDTO(200, "Update avatar success"));
            }
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
        /// Get the list event in 1 month, include module and exam
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <returns>list event in 1 month, include module and exam</returns>
        [HttpGet("{id:int}/timetable")]
        public async Task<ActionResult<EventInTimeTable>> ViewTimeTable(int id)
        {

            return null;
        }

        

    }

}