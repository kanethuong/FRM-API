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
        /// <returns>200 :trainee module (two day) and exam (one week)/ 404: Trainee not found or Class has been deactivated</returns>
        [HttpGet("{id:int}/dashboard")]
        public async Task<ActionResult<List<Object>>> ViewTraineeDashboard(int id)
        {
            var checkTrainee = await _traineeService.GetTraineeById(id);
            if (checkTrainee == null) {
                return NotFound(new ResponseDTO(404,"Trainee not found!"));
            }
            var classCheck = await _classService.GetClassByClassID(checkTrainee.ClassId.GetValueOrDefault());
            if(classCheck == null || classCheck.IsDeactivated == true){
                return NotFound(new ResponseDTO(404,"Class not found"));
            }
            TimeSpan oneSecond = new TimeSpan(00, 00, -1);
            var calenders = await _calendarService.GetCalendarsByTraineeId(id, DateTime.Today, DateTime.Today.AddDays(2).Add(oneSecond));
            var exam = await _examService.GetExamListByTraineeId(id, DateTime.Today, DateTime.Today.AddDays(7).Add(oneSecond));
            //Trainee trainee = await _traineeService.GetTraineeById(id);

            if (calenders.Count() != 0)
            {
                Trainer trainer = await _trainerService.GetTrainerById(calenders.FirstOrDefault().Class.TrainerId);
                Room room = await _roomService.GetRoomById(calenders.FirstOrDefault().Class.RoomId);
                foreach (var item in calenders)
                {
                    item.Class.Trainer = trainer;
                    item.Class.Room = room;
                }
            }
            
            var moduleInDashboard = _mapper.Map<IEnumerable<ModuleInTraineeDashboard>>(calenders);
            var examInDashboard = _mapper.Map<IEnumerable<ExamInTraineeDashboard>>(exam);
            if(examInDashboard.Count() != 0){
                Room room = await _roomService.GetRoomByTraineeId(id);
                foreach (var item in examInDashboard)
                {
                    item.RoomId = room.RoomId;
                    item.RoomName = room.RoomName;
                }
            }
            List<Object> listObject = new List<Object>();
            foreach (var item in moduleInDashboard)
            {
                var o = (Object)item;
                listObject.Add(o);
            }
            foreach (var item in examInDashboard)
            {
                var o = (Object)item;
                listObject.Add(o);
            }
            return Ok(listObject);
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
        /// <returns>200: Updated / 409: Conflict / 404: Profile not found</returns>
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
                return Conflict(new ResponseDTO(409, "Fail to update trainee profile"));
            }
            else
            {
                return Ok(new ResponseDTO(200, "Update profile success"));
            }
        }

        /// <summary>
        /// Update trainee avatar
        /// </summary>
        /// <param name="id">Trainee id</param>
        /// <param name="image">The avatar to update</param>
        /// <returns>200: Update avatar success / 404: Trainee profile cannot be found / 409: Conflict</returns>
        [HttpPut("{id:int}/avatar")]
        public async Task<ActionResult> UpdateAvatar(int id, [FromForm] IFormFile image)
        {
            (bool isImage, string errorMsg) = FileHelper.CheckImageExtension(image);
            if (isImage == false)
            {
                return Conflict(new ResponseDTO(409, errorMsg));
            }
            string fileName = ContentDispositionHeaderValue.Parse(image.ContentDisposition).FileName.Trim('"');
            Stream stream = image.OpenReadStream();
            long fileLength = image.Length;
            string fileType = image.ContentType;

            string avatarUrl = await _imgHelper.Upload(stream, fileName, fileLength, fileType);
            int rs = await _traineeService.UpdateAvatar(id, avatarUrl);
            if (rs == -1)
            {
                return NotFound(new ResponseDTO(404, "Trainee profile cannot be found"));
            }
            else if (rs == 0)
            {
                return Conflict(new ResponseDTO(409, "Fail to update trainee avatar"));
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
                return NotFound(new ResponseDTO(404, "id not found"));
            }
            try
            {
                (int totalRecord, IEnumerable<TraineeAttendanceReport> result) = await _traineeService.GetAttendanceReports(id, paginationParameter);
                return Ok(new PaginationResponse<IEnumerable<TraineeAttendanceReport>>(totalRecord, result));
            }
            catch
            {
                return NotFound(new ResponseDTO(404, "Undefined error, trainee may not in any class"));
            }
        }

        /// <summary>
        /// Get the list event in 1 month, include module and exam
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <returns>list event in 1 month, include module and exam</returns>
        [HttpGet("{id:int}/timetable")]
        public async Task<ActionResult<List<Object>>> ViewTimeTable(int id, DateTime date)
        {

            TimeSpan oneday = new TimeSpan(23, 59, 59);
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            endDate = endDate.Add(oneday);

            var checkTrainee = await _traineeService.GetTraineeById(id);
            if (checkTrainee == null) {
                return NotFound(new ResponseDTO(404,"Trainee not found!"));
            }
            var classCheck = await _classService.GetClassByClassID(checkTrainee.ClassId.GetValueOrDefault());
            if(classCheck == null || classCheck.IsDeactivated == true){
                return NotFound(new ResponseDTO(404,"Class not found"));
            }
            var calenders = await _calendarService.GetCalendarsByTraineeId(id, startDate, endDate);
            var exam = await _examService.GetExamListByTraineeId(id, startDate, endDate);
            //Trainee trainee = await _traineeService.GetTraineeById(id);

            if (calenders.Count() != 0)
            {
                Trainer trainer = await _trainerService.GetTrainerById(calenders.FirstOrDefault().Class.TrainerId);
                Room room = await _roomService.GetRoomById(calenders.FirstOrDefault().Class.RoomId);
                foreach (var item in calenders)
                {
                    item.Class.Trainer = trainer;
                    item.Class.Room = room;
                }
            }

            var moduleInTimeTable = _mapper.Map<IEnumerable<ModuleInTimeTable>>(calenders);
            var examInTimeTable = _mapper.Map<IEnumerable<ExamInTimeTable>>(exam);
            if(examInTimeTable.Count() != 0){
                Room room = await _roomService.GetRoomByTraineeId(id);
                foreach (var item in examInTimeTable)
                {
                    item.RoomId = room.RoomId;
                    item.RoomName = room.RoomName;
                }
            }

            List<Object> listObject = new List<Object>();
            foreach (var item in moduleInTimeTable)
            {
                var o = (Object)item;
                listObject.Add(o);
            }
            foreach (var item in examInTimeTable)
            {
                var o = (Object)item;
                listObject.Add(o);
            }
            return Ok(listObject);
        }
        [HttpGet("free_trainee")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TraineeResponse>>>> GetTraineeWithoutClass([FromQuery]PaginationParameter paginationParameter)
        {
            
            (int totalRecord, IEnumerable<Trainee> listTrainee) = await _traineeService.GetAllTraineeWithoutClass(paginationParameter);

            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Search trainee email not found"));
            }
            var trainees = _mapper.Map<IEnumerable<TraineeResponse>>(listTrainee);

            return Ok(new PaginationResponse<IEnumerable<TraineeResponse>>(totalRecord, trainees));
        }
        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TraineeResponse>>>> GetTraineeList([FromQuery]PaginationParameter paginationParameter)
        {
            return null;
        }
    }
}