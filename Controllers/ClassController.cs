using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ClassDetailDTO;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.MarkDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Helper;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;
        private readonly IAdminService _adminService;
        private readonly ITrainerService _trainerService;
        private readonly IMarkService _markService;
        private readonly IModuleService _moduleService;
        private readonly IFeedbackService _feedbackService;
        private readonly IMapper _mapper;
        private readonly ITraineeService _traineeService;

        public ClassController(IClassService classService,ITraineeService traineeService, IMarkService markService, IAdminService adminService, IModuleService moduleService, ITrainerService trainerService, IFeedbackService feedbackService, IMapper mapper)
        {
            _classService = classService;
            _adminService = adminService;
            _trainerService = trainerService;
            _feedbackService = feedbackService;
            _markService = markService;
            _moduleService = moduleService;
            _mapper = mapper;
            _adminService = adminService;
            _traineeService = traineeService;
        }

        /// <summary>
        /// Get list of class in db with pagination
        /// </summary>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List with pagination / 404: class name not found</returns>
        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<ClassResponse>>>> GetClassList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<Class> classList) = await _classService.GetClassList(paginationParameter);

            foreach (Class c in classList)
            {
                c.Trainer = await _trainerService.GetTrainerById(c.TrainerId);
                c.Admin = await _adminService.GetAdminById(c.AdminId);
            }
            IEnumerable<ClassResponse> classListDto = _mapper.Map<IEnumerable<ClassResponse>>(classList);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Classes not found"));
            }
            return Ok(new PaginationResponse<IEnumerable<ClassResponse>>(totalRecord, classListDto));
        }

        /// <summary>
        /// Get list of request delete class with pagination
        /// </summary>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of class with pagination / 404: search class name not found</returns>
        [HttpGet("request")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<RequestDeleteClassResponse>>>> GetDeleteClassRequestList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecords, IEnumerable<DeleteClassRequest> deleteClassRequests) = await _classService.GetRequestDeleteClassList(paginationParameter);
            IEnumerable<RequestDeleteClassResponse> RequestDeleteClassDTO = _mapper.Map<IEnumerable<RequestDeleteClassResponse>>(deleteClassRequests);
            if (totalRecords == 0)
            {
                return NotFound(new ResponseDTO(404, "Class name not found!"));
            }
            return Ok(new PaginationResponse<IEnumerable<RequestDeleteClassResponse>>(totalRecords, RequestDeleteClassDTO));
        }

        /// <summary>
        /// Update delete class request and if accept delete request then deactivate that class
        /// </summary>
        /// <param name="confirmDeleteClassInput">Confirm detail</param>
        /// <returns>200: Update done / 404: Class or request not found / 409: Class or request deactivated</returns>
        [HttpPut("request")]
        public async Task<ActionResult> ConfirmDeleteClassRequest([FromBody] ConfirmDeleteClassInput confirmDeleteClassInput)
        {
            int status = await _classService.UpdateDeletedClass(confirmDeleteClassInput);
            if (status == -1)
            {
                return NotFound(new ResponseDTO(404, "Class or request not found"));
            }
            if (status == 0)
            {
                return BadRequest(new ResponseDTO(409, "Class or request deactivated"));
            }
            return Ok(new ResponseDTO(200, "Update done"));
        }

        /// <summary>
        /// Get list of request delete class with pagination
        /// </summary>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of class with pagination / 404: search class name not found</returns>
        [HttpGet("deleted")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<DeleteClassResponse>>>> GetDeactivatedClass([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<Class> deletedClass) = await _classService.GetDeletedClassList(paginationParameter);
            IEnumerable<DeleteClassResponse> deletedClassDTO = _mapper.Map<IEnumerable<Class>, IEnumerable<DeleteClassResponse>>(deletedClass);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "List empty"));
            }
            return Ok(new PaginationResponse<IEnumerable<DeleteClassResponse>>(totalRecord, deletedClassDTO));
        }

        /// <summary>
        /// Get the detail information of a class and student list with pagination
        /// </summary>
        /// <param name="id"> id of class</param>
        /// <returns> 200: Detail of class  / 404: class not found </returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClassDetailResponse>> ViewClassDetail(int id)
        {
            Class s = await _classService.GetClassDetail(id);
            if (s == null)
            {
                return NotFound(new ResponseDTO(404, "Class not found"));
            }

            var cdr = _mapper.Map<ClassDetailResponse>(s);
            return Ok(cdr);
        }
        /// <summary>
        /// Get trainee list with pagination
        /// </summary>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of trainee list in a class with pagination / 404: search trainee name not found</returns>
        [HttpGet("trainee/{id:int}")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TraineeInClassDetail>>>> GetTraineeListByClassId(int id, [FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<Trainee> trainees) = await _classService.GetTraineesByClassId(id, paginationParameter);
            IEnumerable<TraineeInClassDetail> traineeDTO = _mapper.Map<IEnumerable<TraineeInClassDetail>>(trainees);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Search trainee name not found"));
            }
            return Ok(new PaginationResponse<IEnumerable<TraineeInClassDetail>>(totalRecord, traineeDTO));
        }

        /// <summary>
        /// Insert the request delete class to db
        /// </summary>
        /// <param name="requestDeleteClassInput">Request detail</param>
        /// <returns>201: Request is created / 409: Class is already deactivated</returns>
        [HttpPost("request")]
        public async Task<ActionResult> CreateRequestDeleteClass(RequestDeleteClassInput requestDeleteClassInput)
        {
            DeleteClassRequest deleteClassRequest=_mapper.Map<DeleteClassRequest>(requestDeleteClassInput);
            int rs=await _classService.InsertNewRequestDeleteClass(deleteClassRequest);
            if(rs==-1){
                return Conflict(new ResponseDTO(409,"Class is already deactivated"));
            }else if(rs==0){
                return StatusCode(StatusCodes.Status500InternalServerError,new ResponseDTO(500,"Fail to request delete class"));
            }else{
                return Ok(new ResponseDTO(200,"Request delete class success"));
            }
        }

        /// <summary>
        /// Get the student mark with pagination of a class
        /// </summary>
        /// <param name="id">id of class</param>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of student mark in a class with pagination / 404: search student name not found</returns>
        [HttpGet("score/{id:int}")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<MarkResponse>>>> ViewClassScore(int id, [FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecords, IEnumerable<Trainee> trainees) = await _classService.GetTraineesByClassId(id,paginationParameter);
            List<MarkResponse> markResponses = new List<MarkResponse>(); 
            foreach (Trainee trainee in trainees) {
                    MarkResponse markResponse = new MarkResponse();
                    markResponse.TraineeName = trainee.Fullname;
                    IEnumerable<Mark> markList = await _markService.GetMarkByTraineeId(trainee.TraineeId, new DateTime(2021,2,5),new DateTime(2021,7,11));
                    foreach (Mark m in markList) {
                        m.Module = await _moduleService.GetModuleById(m.ModuleId);
                    }
                    markResponse.ScoreList = _mapper.Map<IEnumerable<ModuleMark>>(markList);
                    markResponses.Add(markResponse);
            }
            if (totalRecords == 0)
            {
                return NotFound(new ResponseDTO(404, "Search student name not found!"));
            }
            return Ok(new PaginationResponse<IEnumerable<MarkResponse>>(totalRecords, markResponses));
        }

        /// <summary>
        /// Get the trainer feedbacks and admin feedbacks of a class
        /// </summary>
        /// <param name="id">id of a class</param>
        /// <returns>200: List of trainer feedback and admin feedback</returns>
        [HttpGet("feedback/{id:int}")]
        public async Task<ActionResult<FeedbackResponse>> ViewClassFeedback(int id)
        {
            Admin admin1 = await _adminService.getAdminByClassId(id);
            Trainer trainer1 = await _trainerService.getTrainerByClassId(id);
            FeedbackResponse feedbackResponses = new FeedbackResponse();

            IEnumerable<TrainerFeedback> trainerFeedbacks = await _feedbackService.GetTrainerFeedbacksByAdminId(trainer1.TrainerId);
            IEnumerable<AdminFeedback> adminFeedbacks = await _feedbackService.GetAdminFeedbacksByAdminId(admin1.AdminId);
            
            IEnumerable<FeedbackContent> TrainerfeedbackContents = _mapper.Map<IEnumerable<FeedbackContent>>(trainerFeedbacks);
            IEnumerable<FeedbackContent> AdminfeedbackContents = _mapper.Map<IEnumerable<FeedbackContent>>(adminFeedbacks);
            
            
            TrainerFeedbackResponse trainerFeedbackResponse = new ();
            trainerFeedbackResponse.Trainer = _mapper.Map<TrainerInFeedbackResponse>(trainer1);
            trainerFeedbackResponse.Feedbacks = TrainerfeedbackContents;

            AdminFeedbackResponse adminFeedbackResponse = new();
            adminFeedbackResponse.Admin = _mapper.Map<AdminInFeedbackResponse>(admin1);
            adminFeedbackResponse.Feedbacks = AdminfeedbackContents;

            feedbackResponses.TrainerFeedback = trainerFeedbackResponse;
            feedbackResponses.AdminFeedback = adminFeedbackResponse;

            return feedbackResponses;

        }

        /// <summary>
        /// Insert new class to db
        /// </summary>
        /// <param name="newClassInput">Detail of new class</param>
        /// <returns>201: Class is created / 409: Classname exist || Trainees or trainers already have class</returns>
        [HttpPost]
        public async Task<ActionResult> CreateNewClass([FromBody] NewClassInput newClassInput)
        {
            var rs = await _classService.InsertNewClass(newClassInput);
            if (rs == -1)
            {
                return Conflict(new ResponseDTO
                {
                    Status = 409,
                    Message = "Class is already exist"
                });
            }
            else if (rs == -2)
            {
                return BadRequest(new ResponseDTO(409, "One or more trainees already have class "));
            }
            else if (rs == 0)
            {
                return StatusCode(500);
            }
            return CreatedAtAction(nameof(GetClassList), new ResponseDTO(201, "Successfully inserted"));
        }

        /// <summary>
        /// Insert a list class to db using excel file
        /// </summary>
        /// <param name="file">Excel file to stores class data</param>
        /// <returns>201: Class is created /400: File content inapproriate /409: Classname exist || Trainees or trainers already have class</returns>
        [HttpPost("excel")]
        public async Task<ActionResult<ResponseDTO>> CreateNewClassByExcel([FromForm] IFormFile file)
        {
            bool success;
            string message;
            List<string> errors = new List<string>();

            (success, message) = FileHelper.CheckExcelExtension(file);
            if (!success)
            {
                return BadRequest(new ResponseDTO(400, message));
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    Dictionary<string, HashSet<int>> classModulesDict = new Dictionary<string, HashSet<int>>();
                    Dictionary<string, HashSet<int>> classTraineesDict = new Dictionary<string, HashSet<int>>();
                    List<NewClassInput> classInputList = new List<NewClassInput>();

                    // Check sheets
                    ExcelWorkbook workbook = package.Workbook;
                    ExcelWorksheet classSheet = workbook.Worksheets["Class"];
                    ExcelWorksheet moduleSheet = workbook.Worksheets["Module"];
                    ExcelWorksheet traineeSheet = workbook.Worksheets["Trainee"];
                    if (classSheet is null || moduleSheet is null || traineeSheet is null)
                    {
                        return BadRequest(new ResponseDTO(400, "Missing required sheets"));
                    }

                    // Export Class-Module dictionary
                    Predicate<List<string>> moduleChecker = list => list.ContainsAll("class", "module");
                    List<Dictionary<string, object>> moduleDictList = moduleSheet.ExportDataFromExcel(moduleChecker, out success, out message);
                    if (!success) {
                        return BadRequest(new ResponseDTO(400, "Error on Module: " + message));
                    }
                    foreach (var dict in moduleDictList) {
                        string className = dict["class"]?.ToString();
                        string module = dict["module"]?.ToString();
                        if (className is null || module is null) {
                            continue;
                        }
                        int moduleId;
                        if (!int.TryParse(module, out moduleId)) {
                            continue;
                        }
                        HashSet<int> list;
                        if (classModulesDict.ContainsKey(className)) {
                            list = classModulesDict[className];
                        } else {
                            list = new HashSet<int>();
                            classModulesDict[className] = list;
                        }
                        if (await _moduleService.GetModuleById(moduleId) is not null) {
                            list.Add(moduleId);
                        } else {
                            errors.Add("Invalid module: " + module);
                        }
                    }


                    // Export Class-Trainee dictionary
                    Predicate<List<string>> traineeChecker = list => list.ContainsAll("class", "email");
                    List<Dictionary<string, object>> traineeDictList = traineeSheet.ExportDataFromExcel(traineeChecker, out success, out message);
                    if (!success) {
                        return BadRequest(new ResponseDTO(400, "Error on Trainee: " + message));
                    }
                    foreach (var dict in traineeDictList) {
                        string className = dict["class"]?.ToString();
                        string email = dict["email"]?.ToString();
                        if (className is null || email is null) {
                            continue;
                        }
                        int? traineeId = (await _traineeService.GetTraineeByEmail(email))?.TraineeId;
                        if (!traineeId.HasValue) {
                            errors.Add("Invalid email: " + email);
                            continue;
                        }
                        HashSet<int> list;
                        if (classTraineesDict.ContainsKey(className)) {
                            list = classTraineesDict[className];
                        } else {
                            list = new HashSet<int>();
                            classTraineesDict[className] = list;
                        }
                        list.Add(traineeId.Value);
                    }

                    // Export Class list
                    Predicate<List<string>> classChecker = list => list.ContainsAll("name", "description", "trainer", "admin", "room", "start", "end");
                    List<Dictionary<string, object>> classDictList = classSheet.ExportDataFromExcel(classChecker, out success, out message);
                    if (!success) {
                        return BadRequest(new ResponseDTO(400, "Error on Class: " + message));
                    }
                    foreach (var dict in classDictList) {
                        NewClassInput classInput = new();
                        classInput.ClassName = dict["name"]?.ToString();
                        if (classInput.ClassName is not null) {
                            if (classModulesDict.ContainsKey(classInput.ClassName)) {
                                classInput.ModuleIdList = classModulesDict[classInput.ClassName];
                            }
                            if (classTraineesDict.ContainsKey(classInput.ClassName)) {
                                classInput.TraineeIdList = classTraineesDict[classInput.ClassName];
                            }
                        }
                        classInput.Description = dict["description"]?.ToString();
                        object room = dict["room"];
                        if (room is not null) {
                            int roomId = 0;
                            int.TryParse(room.ToString(), out roomId);
                            classInput.RoomId = roomId; // TODO: RoomService ?
                        }
                        object startTime = dict["start"];
                        if (startTime is not null && startTime is DateTime) {
                            classInput.StartDay = (DateTime) startTime;
                        }
                        object endTime = dict["end"];
                        if (endTime is not null && endTime is DateTime) {
                            classInput.EndDay = (DateTime) endTime;
                        }
                        string trainerEmail = dict["trainer"]?.ToString();
                        if (trainerEmail is not null) {
                            Trainer trainer = await _trainerService.GetTrainerByEmail(trainerEmail);
                            if (trainer is null) {
                                errors.Add("Invalid trainer email: " + trainerEmail);
                            } else {
                                classInput.TrainerId = trainer.TrainerId;
                            }
                        }
                        string adminEmail = dict["admin"]?.ToString();
                        if (adminEmail is not null) {
                            Admin admin = await _adminService.GetAdminByEmail(adminEmail);
                            if (admin is null) {
                                errors.Add("Invalid admin email: " + adminEmail);
                            } else {
                                classInput.AdminId = admin.AdminId;
                            }
                        }
                        classInputList.Add(classInput);
                    }

                    // Validate the class inputs
                    foreach (var classInput in classInputList) {
                        if (!classInputList.Validate(out List<ValidationResult> validateResults)) {
                            return BadRequest(new ResponseDTO(400, "Error when validating class") {
                                Errors = new {
                                    value = classInput,
                                    errors = validateResults
                                }
                            });
                        }
                    }

                    // Throw remain errors
                    if (errors.Count > 0) {
                        return Conflict(new ResponseDTO(409, "Some errors when creating classes") {
                            Errors = errors
                        });
                    }

                    // Insert the classes
                    foreach (var classInput in classInputList) {
                        int result = await _classService.InsertNewClassNoSave(classInput);
                        if (result < 0) {
                            _classService.DiscardChanges();
                            if (result == -1) {
                                return Conflict(new ResponseDTO(409, "Duplicate class name") {
                                    Errors = new {
                                        value = classInput
                                    }
                                });
                            } else if (result == -2) {
                                return Conflict(new ResponseDTO(409, "Trainee already have class") {
                                    Errors = new {
                                        value = classInput
                                    }
                                });
                            }
                        }
                    }
                    try {
                        await _classService.SaveChange();
                    } catch (Exception e) {
                        _classService.DiscardChanges();
                        return BadRequest(new ResponseDTO(400, "Failed to add class") {
                            Errors = e
                        });
                    }

                    // Insert Class-Module and Class-Trainee
                    foreach(var modulePair in classModulesDict) {
                        var clazz = await _classService.GetClassByClassName(modulePair.Key);
                        if (clazz is not null) {
                            _classService.AddDataToClassModule(clazz.ClassId, modulePair.Value);
                        }
                    }
                    foreach(var traineePair in classTraineesDict) {
                        var clazz = await _classService.GetClassByClassName(traineePair.Key);
                        if (clazz is not null) {
                            await _classService.AddClassIdToTrainee(clazz.ClassId, traineePair.Value);
                        }
                    }
                    try {
                        await _classService.SaveChange();
                    } catch (Exception e) {
                        _classService.DiscardChanges();
                        return BadRequest(new ResponseDTO(400, "Failed to save changes on module or trainee on class") {
                            Errors = e
                        });
                    }
                }
            }
            return CreatedAtAction(nameof(GetClassList), new ResponseDTO(201, "Created"));
        }
    }
}