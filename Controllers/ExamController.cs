using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ExamDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Authorize(Policy = "Exam")]
    [Route("api/[controller]")]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;
        private readonly ITraineeService _traineeService;
        private readonly IAdminService _adminService;
        private readonly IModuleService _moduleService;
        private readonly IClassService _classService;
        private readonly IRoomService _roomService;
        private readonly ITimetableService _timeTableService;

        private readonly IMapper _mapper;

        public ExamController(IExamService examService, IMapper mapper, ITraineeService traineeService, IAdminService adminService, IModuleService moduleService, IClassService classService,IRoomService roomService,TimetableService timeTableService)
        {
            _examService = examService;
            _mapper = mapper;
            _traineeService = traineeService;
            _adminService = adminService;
            _moduleService = moduleService;
            _classService = classService;
            _roomService = roomService;
            _timeTableService = timeTableService;
        }

        /// <summary>
        /// create new exam 
        /// </summary>
        /// <param name="newExamInput">new exam input, include trainee list id< to take exam/param>
        /// <param name="classId">classId to take exam (optional), if user send classId, get all trainee in the class</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateNewExam(NewExamInput newExamInput)
        {
            if(_timeTableService.DayOffCheck(newExamInput.ExamDay)){
                return BadRequest(new ResponseDTO(400,"Exam time can't be in day off"));
            }
            var errorNotfound = new List<String>();
            var errorNotModule = new List<String>();
            if (newExamInput.ExamDay <= DateTime.Now)
            {
                return BadRequest(new ResponseDTO(400, "Exam time can't be smaller than current time"));
            }
            //Gắn traineeID trong newExamInput.class vào newExamInput.TraineeIdList và loại bỏ những trainee trùng
            // if (newExamInput.classId != null)
            // {
            List<int> traineeIdList = new List<int>();
            var traineeList = await _traineeService.GetTraineeByClassId(newExamInput.classId);
            foreach (Trainee item in traineeList)
            {
                traineeIdList.Add(item.TraineeId);
            }
            newExamInput.TraineeIdList = newExamInput.TraineeIdList.Concat(traineeIdList);
            newExamInput.TraineeIdList = newExamInput.TraineeIdList.Distinct();
            //}
            //Check Class deactivated
            var classCheck = await _classService.GetClassByClassID(newExamInput.classId);
            if (newExamInput.classId != 0 && classCheck == null)
            {
                //errorNotfound.Add("Class not found");
                return NotFound(new ResponseDTO(404, "Class not found"));
            }
            //Check admin deactivated == false
            var adminCheck = await _adminService.GetAdminById(newExamInput.AdminId);
            if (adminCheck == null)
            {
                return NotFound(new ResponseDTO(404, "Admin not found"));
            }
            //map
            Exam exam = _mapper.Map<Exam>(newExamInput);

            //Gan trainee vao traineeList1 va check trainee deactivated
            var traineeList1 = new List<Trainee>();
            foreach (var item in newExamInput.TraineeIdList)
            {
                var traineeCheck = await _traineeService.GetTraineeById(item);

                if (traineeCheck == null)
                {
                    errorNotfound.Add(item.ToString());
                    //return NotFound(new ResponseDTO(404, "Trainee(s) not found"));
                }
                else
                {
                    traineeList1.Add(traineeCheck);
                }
            }
            exam.Trainees = traineeList1;
            //Check if trainee have right module
            foreach (var item in exam.Trainees)
            {
                var modules = await _moduleService.GetModulesIdByTraineeId(item.TraineeId);
                bool checkModule = modules.Contains(exam.ModuleId);
                if (checkModule == false)
                {
                    errorNotModule.Add(item.Fullname + "(" + item.Email + ")");
                }

            }
            if (errorNotfound.Count() != 0 || errorNotModule.Count() != 0)
            {
                var temp = await _moduleService.GetModuleById(newExamInput.ModuleId);
                string rp = "";
                if (errorNotfound.Count() != 0)
                {
                    rp += "There are (" + errorNotfound.Count() + ") problem about student id not found :";
                    foreach (var item in errorNotfound)
                    {
                        rp += item;
                        if (item != errorNotfound[errorNotfound.Count - 1])
                        {
                            rp += ", ";
                        }
                    }
                    rp += "\n";
                }
                if (errorNotModule.Count() != 0)
                {
                    rp += "There are (" + errorNotModule.Count() + ") problem about student name not in module (" + temp.ModuleName + ") :";
                    foreach (var item in errorNotModule)
                    {
                        rp += item;
                        if (item != errorNotModule[errorNotModule.Count - 1])
                        {
                            rp += ", ";
                        }
                    }
                }
                return NotFound(new ResponseDTO(404, rp));
            }
            //Set variable cho traineeExams
            List<TraineeExam> traineeExams = new List<TraineeExam>();
            foreach (var item in exam.Trainees)
            {
                TraineeExam tempTraineeExam = new TraineeExam();
                tempTraineeExam.Trainee = item;
                tempTraineeExam.TraineeId = item.TraineeId;
                tempTraineeExam.Exam = exam;
            }
            int status = await _examService.InsertNewExam(exam);

            return Ok(new ResponseDTO(200, "Success"));
        }

        /// <summary>
        /// create new exam last
        /// </summary>
        /// <param name="newExamInput">new exam input, include trainee list id< to take exam/param>
        /// <param name="classId">classId to take exam (optional), if user send classId, get all trainee in the class</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> CreateNewExamLast(NewExamInput newExamInput)
        {
            if (await _examService.CheckDateExam(newExamInput.classId, newExamInput.ModuleId, newExamInput.ExamDay, 3) == false)
            {
                return BadRequest(new ResponseDTO(400, "Datetime not right"));
            }
            var classCheck = await _classService.GetClassByClassID(newExamInput.classId);
            var traineeList = await _traineeService.GetTraineeByClassId(newExamInput.classId);
            List<int> traineeIdList = new List<int>();
            foreach (var item in traineeList)
            {
                traineeIdList.Add(item.TraineeId);
            }
            newExamInput.TraineeIdList = traineeIdList;
            Exam exam = _mapper.Map<Exam>(newExamInput);
            var Room = await _roomService.GetRoom(newExamInput.classId,newExamInput.ModuleId);
            exam.RoomId = Room.RoomId;
            exam.Trainees = traineeList;
            var rs = await _examService.InsertNewExam(exam);
            return Ok(new ResponseDTO(200, "Input success"));
        }

        /// <summary>
        /// View all exam with pagination
        /// /// </summary>
        /// <returns>200: All exams list / 404: Exam name not found</returns>
        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<ExamResponse>>>> ViewExamList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<Exam> examList) = await _examService.GetExamList(paginationParameter);

            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Exam not found"));
            }

            var rs = new PaginationResponse<ICollection<ExamResponse>>()
            {
                TotalRecords = totalRecord,
                Payload = _mapper.Map<ICollection<ExamResponse>>(examList)
            };

            return Ok(rs);
        }

        /// <summary>
        /// Change Exam infomation, only duration and exam day
        /// </summary>
        /// <param name="id">id of exam</param>
        /// <param name="duration"></param>
        /// <param name="ExamDay"></param>
        /// <returns>200: Update succcessfully / 400: Exam was cancelled / 404: Exam not found</returns>
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ChangeExamInfo(int id, [FromBody] UpdateExamInput updateExamInput)
        {
            var exam = await _examService.GetExamById(id);

            if (exam == null)
            {
                return NotFound(new ResponseDTO(404, "Exam not found"));
            }

            if (exam.DurationInMinute == updateExamInput.Duration && exam.ExamDay == updateExamInput.ExamDay)
            {
                return Ok(new ResponseDTO(200, "Update exam successfully"));
            }

            exam.DurationInMinute = updateExamInput.Duration;
            exam.ExamDay = updateExamInput.ExamDay;

            var rs = await _examService.UpdateExam(id, exam);

            if (rs == -1)
            {
                return BadRequest(new ResponseDTO(400, "Exam was cancelled"));
            }

            if (rs == 0)
            {
                return BadRequest(new ResponseDTO(400, "There is an error when update exam"));
            }

            return Ok(new ResponseDTO(200, "Update exam successfully"));
        }

        /// <summary>
        /// Cancel an exam
        /// </summary>
        /// <param name="id">id of exam</param>
        /// <returns>200: Cancelled / 404: Id not found, 409: Exam was cancelled, Delete fail</returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> CancelExam(int id)
        {
            (bool? status, string message) = await _examService.CancelExam(id);

            if (status == null)
            {
                return NotFound(new ResponseDTO(404, message));
            }

            if (status == false)
            {
                return Conflict(new ResponseDTO(409, message));
            }

            return Ok(new ResponseDTO(200, message));
        }
    }
}