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
using kroniiapi.DTO.TraineeDTO;
using kroniiapi.Helper;
using kroniiapi.Helper.UploadDownloadFile;
using kroniiapi.Helper.Upload;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using kroniiapi.Services.Attendance;
using kroniiapi.Services.Report;
using Microsoft.AspNetCore.Authorization;

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

        private readonly IAttendanceService _attendanceService;
        private readonly IReportService reportService;

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
                                 IImgHelper imgHelper,
                                 IAttendanceService attendanceService,
                                 IReportService reportService)
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
            _attendanceService = attendanceService;
            this.reportService = reportService;
        }

        /// <summary>
        /// View Trainee module in two day, exam in 1 1 week
        /// </summary>
        /// <param name="id"> trainee id</param>
        /// <returns>200 :trainee module (two day) and exam (one week)/ 404: Trainee not found or Class has been deactivated</returns>
        [HttpGet("{id:int}/dashboard")]
        [Authorize(Policy = "TraineeGet")]
        public async Task<ActionResult<List<Object>>> ViewTraineeDashboard(int id)
        {
            var checkTrainee = await _traineeService.GetTraineeById(id);
            if (checkTrainee == null)
            {
                return NotFound(new ResponseDTO(404, "Trainee not found!"));
            }
            var classCheck = await _classService.GetClassByClassID(checkTrainee.ClassId.GetValueOrDefault());
            if (classCheck == null || classCheck.IsDeactivated == true)
            {
                return NotFound(new ResponseDTO(404, "Class not found"));
            }
            TimeSpan oneSecond = new TimeSpan(00, 00, -1);
            List<Calendar> calenders = await _calendarService.GetCalendarsByTraineeId(id, DateTime.Today, DateTime.Today.AddDays(2).Add(oneSecond));
            var exam = await _examService.GetExamListByTraineeId(id, DateTime.Today, DateTime.Today.AddDays(7).Add(oneSecond));
            Trainee trainee = await _traineeService.GetTraineeById(id);
            var moduleInDashboard = _mapper.Map<IEnumerable<ModuleInTraineeDashboard>>(calenders);
            var examInDashboard = _mapper.Map<IEnumerable<ExamInTraineeDashboard>>(exam);
            if (moduleInDashboard.Count() != 0)
            {
                int i = 0;
                foreach (var item in moduleInDashboard)
                {
                    Trainer tempTrainer = await _trainerService.GetTrainerByCalendarId(calenders[i].CalendarId);
                    item.Class.TrainerName = tempTrainer.Fullname;
                    item.Class.TrainerAvatarURL = tempTrainer.AvatarURL;
                    item.Class.TrainerEmail = tempTrainer.Email;
                    Room tempRoom = await _roomService.GetRoom(classCheck.ClassId, item.ModuleId);
                    item.Class.RoomName = tempRoom.RoomName;
                    item.SlotDuration = item.SlotDuration/2;
                    i++;
                }
            }
            if (examInDashboard.Count() != 0)
            {
                foreach (var item in examInDashboard)
                {
                    Room room = await _roomService.GetRoom(classCheck.ClassId, item.ModuleId);
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
        [Authorize(Policy = "TraineeGet")]
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
        [Authorize(Policy = "TraineePut")]
        public async Task<ActionResult> EditProfile(int id, [FromBody] TraineeProfileDetailInput traineeProfileDetail)
        {
            Trainee trainee = _mapper.Map<Trainee>(traineeProfileDetail);
            Trainee existedTrainee = await _traineeService.GetTraineeById(id);
            if (existedTrainee == null)
            {
                return NotFound(new ResponseDTO(404, "Trainee profile cannot be found"));
            }
            if (
                existedTrainee.Fullname.ToLower().Equals(trainee.Fullname.ToLower()) &&
                existedTrainee.Phone.ToLower().Equals(trainee.Phone.ToLower()) &&
                existedTrainee.DOB.ToString().ToLower().Equals(trainee.DOB.ToString().ToLower()) &&
                existedTrainee.Address.ToLower().Equals(trainee.Address.ToLower()) &&
                existedTrainee.Gender.ToLower().Equals(trainee.Gender.ToLower()) &&
                existedTrainee.Facebook.ToLower().Equals(trainee.Facebook.ToLower())
            )
            {
                return Ok(new ResponseDTO(200, "Update profile success"));
            }
            int rs = await _traineeService.UpdateTrainee(id, trainee);
            if (rs == 0)
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
        [Authorize(Policy = "TraineePut")]
        public async Task<ActionResult> UpdateAvatar(int id, [FromForm] IFormFile image)
        {
            (bool isImage, string errorMsg) = FileHelper.CheckImageExtension(image);
            if (isImage == false)
            {
                return Conflict(new ResponseDTO(409, errorMsg));
            }

            if (_traineeService.CheckTraineeExist(id) == false)
            {
                return NotFound(new ResponseDTO(404, "Trainee profile cannot be found"));
            }
            string fileName = ContentDispositionHeaderValue.Parse(image.ContentDisposition).FileName.Trim('"');
            Stream stream = image.OpenReadStream();
            long fileLength = image.Length;
            string fileType = image.ContentType;

            string avatarUrl = await _imgHelper.Upload(stream, fileName, fileLength, fileType);
            int rs = await _traineeService.UpdateAvatar(id, avatarUrl);
            if (rs == 0)
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
        [Authorize(Policy = "TraineeGet")]
        public async Task<ActionResult<TraineeAttendanceReport>> ViewAttendanceReport(int id)
        {
            if (await _traineeService.GetTraineeById(id) == null)
            {
                return NotFound(new ResponseDTO(404, "id not found"));
            }
            TraineeAttendanceReport attendanceReport = await _attendanceService.GetTraineeAttendanceReport(id);
            if (attendanceReport == null)
            {
                return NotFound(new ResponseDTO(404, "Attendance Report NotFound"));
            }
            return Ok(attendanceReport);
        }

        /// <summary>
        /// Get the list event in 1 month, include module and exam
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <returns>list event in 1 month, include module and exam</returns>
        [HttpGet("{id:int}/timetable")]
        [Authorize(Policy = "TraineeGet")]
        public async Task<ActionResult<List<Object>>> ViewTimeTable(int id, DateTime date)
        {
            TimeSpan oneday = new TimeSpan(23, 59, 59);
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            endDate = endDate.Add(oneday);

            var checkTrainee = await _traineeService.GetTraineeById(id);
            if (checkTrainee == null)
            {
                return NotFound(new ResponseDTO(404, "Trainee not found!"));
            }
            var classCheck = await _classService.GetClassByClassID(checkTrainee.ClassId.GetValueOrDefault());
            if (classCheck == null || classCheck.IsDeactivated == true)
            {
                return NotFound(new ResponseDTO(404, "Class not found"));
            }
            var calenders = await _calendarService.GetCalendarsByTraineeId(id, startDate, endDate);
            var exam = await _examService.GetExamListByTraineeId(id, startDate, endDate);

            var moduleInTimeTable = _mapper.Map<IEnumerable<ModuleInTimeTable>>(calenders);
            var examInTimeTable = _mapper.Map<IEnumerable<ExamInTimeTable>>(exam);
            if (moduleInTimeTable.Count() != 0)
            {
                int i = 0;
                foreach (var item in moduleInTimeTable)
                {
                    Trainer tempTrainer = await _trainerService.GetTrainerByCalendarId(calenders[i].CalendarId);
                    item.Class.TrainerName = tempTrainer.Fullname;
                    item.Class.TrainerAvatarURL = tempTrainer.AvatarURL;
                    item.Class.TrainerEmail = tempTrainer.Email;
                    Room tempRoom = await _roomService.GetRoom(classCheck.ClassId, item.ModuleId);
                    item.Class.RoomName = tempRoom.RoomName;
                    i++;
                }
            }
            if (examInTimeTable.Count() != 0)
            {
                foreach (var item in examInTimeTable)
                {
                    Room room = await _roomService.GetRoom(classCheck.ClassId, item.ModuleId);
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
        /// <summary>
        /// Get all trainee not have class
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        [HttpGet("free_trainee")]
        [Authorize(Policy = "TraineeGet")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TraineeResponse>>>> GetTraineeWithoutClass([FromQuery] PaginationParameter paginationParameter)
        {

            (int totalRecord, IEnumerable<Trainee> listTrainee) = await _traineeService.GetAllTraineeWithoutClass(paginationParameter);

            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Trainee not found"));
            }
            var trainees = _mapper.Map<IEnumerable<TraineeResponse>>(listTrainee);

            return Ok(new PaginationResponse<IEnumerable<TraineeResponse>>(totalRecord, trainees));
        }
        /// <summary>
        /// Get all trainee List with pagination
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        [HttpGet("page")]
        [Authorize(Policy = "TraineeGet")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TraineeResponse>>>> ViewTraineeList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecords, IEnumerable<Trainee> trainees) = await _traineeService.GetAllTrainee(paginationParameter);
            IEnumerable<TraineeResponse> traineesDto = _mapper.Map<IEnumerable<TraineeResponse>>(trainees);
            if (totalRecords == 0)
            {
                return NotFound(new ResponseDTO(404, "Search trainee name not found"));
            }
            return Ok(new PaginationResponse<IEnumerable<TraineeResponse>>(totalRecords, traineesDto));
        }
        /// <summary>
        /// Update trainee wage
        /// </summary>
        /// <param name="id">id of trainee</param>
        /// <param name="wage"></param>
        /// <returns>200: Update wage success / 404: Trainee profile cannot be found / 409: Conflict</returns>
        [HttpPut("{id:int}/wage")]
        [Authorize(Policy = "TraineePutAdmin")]
        public async Task<ActionResult> UpdateTraineeWage(int id, [FromBody] decimal wage)
        {
            Trainee trainee = await _traineeService.GetTraineeById(id);
            if (trainee is null)
            {
                return NotFound(new ResponseDTO(404, "Trainee profile cannot be found"));
            }
            if (wage <= 0)
            {
                return BadRequest(new ResponseDTO(400, "Fail to update trainee wage"));
            }
            trainee.Wage = wage;
            if (await _traineeService.UpdateTrainee(id, trainee) == 1)
            {
                return Ok(new ResponseDTO(200, "Update trainee's wage success"));
            }
            else
            {
                return Conflict(new ResponseDTO(409, "Fail to update trainee wage"));
            }
        }

        /// <summary>
        /// Update trainee status
        /// </summary>
        /// <param name="id">id of trainee</param>
        /// <param name="status"></param>
        /// <returns>200: Update status success / 404: Trainee cannot be found</returns>
        [HttpPut("{id:int}/status")]
        [Authorize(Policy = "TraineePutAdmin")]
        public async Task<ActionResult> UpdateTraineeStatus(int id, [FromBody] UpdateTraineeStatus updateTraineeStatus)
        {
            Trainee trainee = await _traineeService.GetTraineeById(id);
            if (trainee is null)
            {
                return NotFound(new ResponseDTO(404, "Trainee profile cannot be found"));
            }
            if (updateTraineeStatus.status <= 2 || updateTraineeStatus.status >= 6)
            {
                return BadRequest(new ResponseDTO(400, "Fail to update trainee status"));
            }
            switch (updateTraineeStatus.status)
            {
                case 1:
                    {
                        trainee.Status = "Passed";
                        break;
                    }
                case 2:
                    {
                        trainee.Status = "Failed";
                        break;
                    }
                case 3:
                    {
                        trainee.Status = "Deferred";
                        break;
                    }
                case 4:
                    {
                        trainee.Status = "Drop-out";
                        break;
                    }
                case 5:
                    {
                        trainee.Status = "Cancel";
                        break;
                    }
            }
            if (await _traineeService.UpdateTrainee(id, trainee) == 1)
            {
                return Ok(new ResponseDTO(200, "Update trainee's status success"));
            }
            else
            {
                return Conflict(new ResponseDTO(409, "Fail to update trainee status"));
            }
        }
    }
}