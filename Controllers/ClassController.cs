using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.DTO.ExcelDTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.MarkDTO;
using kroniiapi.DTO.ModuleDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TraineeDTO;
using kroniiapi.DTO.TrainerDTO;
using kroniiapi.Helper;
using kroniiapi.Helper.Timetable;
using kroniiapi.Services;
using kroniiapi.Services.Attendance;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IModuleService _moduleService;
        private readonly IMapper _mapper;
        private readonly ITraineeService _traineeService;
        private readonly ITimetableService _timetableService;

        private readonly IMarkService _markService;

        private readonly ICertificateService _certificateService;
        private readonly IAttendanceService _attendanceServices;
        private readonly IRoomService _roomService;
        private DataContext _datacontext;
        public ClassController(IClassService classService,
                               ITraineeService traineeService,
                               IAdminService adminService,
                               IModuleService moduleService,
                               ITrainerService trainerService,
                               IMapper mapper,
                               ITimetableService timetableService,
                               IMarkService markService,
                               ICertificateService certificateService,
                               IAttendanceService attendanceServices,
                               IRoomService roomService,
                               DataContext datacontext)
        {
            _classService = classService;
            _adminService = adminService;
            _trainerService = trainerService;
            _moduleService = moduleService;
            _mapper = mapper;
            _timetableService = timetableService;
            _adminService = adminService;
            _traineeService = traineeService;
            _markService = markService;
            _certificateService = certificateService;
            _attendanceServices = attendanceServices;
            _roomService = roomService;
            _datacontext = datacontext;
        }

        /// <summary>
        /// Get list of class in db with pagination
        /// </summary>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List with pagination / 404: class name not found</returns>
        [HttpGet("page")]
        [Authorize(Policy = "ClassGet")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<ClassResponse>>>> GetClassList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<Class> classList) = await _classService.GetClassList(paginationParameter);
            IEnumerable<ClassResponse> classListDto = _mapper.Map<IEnumerable<ClassResponse>>(classList);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Classes not found"));
            }
            foreach (var item in classListDto)
            {
                var trainers = await _trainerService.GetTrainerListByClassId(item.ClassId);
                item.Trainer = _mapper.Map<List<TrainerInClassResponse>>(trainers);
            }
            return Ok(new PaginationResponse<IEnumerable<ClassResponse>>(totalRecord, classListDto));
        }

        /// <summary>
        /// Get list of request delete class with pagination
        /// </summary>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of class with pagination / 404: search class name not found</returns>
        [HttpGet("request")]
        [Authorize(Policy = "ClassGetDeleted")]
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
        [HttpPut("request/{deleteClassRequestId:int}")]
        [Authorize(Policy = "ClassPut")]
        public async Task<ActionResult> ConfirmDeleteClassRequest([FromBody] ConfirmDeleteClassInput confirmDeleteClassInput, int deleteClassRequestId)
        {
            int status = await _classService.UpdateDeletedClass(confirmDeleteClassInput, deleteClassRequestId);
            if (status == -1)
            {
                return NotFound(new ResponseDTO(404, "Class or request not found"));
            }
            if (status == 0)
            {
                return Conflict(new ResponseDTO(409, "Class or request deactivated"));
            }
            // if (status == 2)
            // {
            //     return BadRequest(new ResponseDTO(400, "Request is rejected"));
            // }
            int rejectAllStatus = await _classService.RejectAllOtherDeleteRequest(deleteClassRequestId);
            int deleteTraineeClass = await _classService.DeleteTraineeClass(confirmDeleteClassInput.ClassId);
            return Ok(new ResponseDTO(200, "Update done"));
        }

        /// <summary>
        /// Get list of request delete class with pagination
        /// </summary>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of class with pagination / 404: search class name not found</returns>
        [HttpGet("deleted")]
        [Authorize(Policy = "ClassGetDeleted")]
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
        /// Get the module information of a class 
        /// </summary>
        /// <param name="id"> id of class</param>
        /// <returns> 200: Detail of class  / 404: class not found </returns>
        [HttpGet("{id:int}/modules")]
        [Authorize(Policy = "ClassGet")]
        public async Task<ActionResult<ICollection<ModuleResponse>>> ViewClassModule(int id)
        {
            var modules = await _moduleService.GetModulesByClassId(id);
            var moduleDto = _mapper.Map<ICollection<ModuleResponse>>(modules);
            return moduleDto.ToList();
        }
        /// <summary>
        /// Get the detail information of a class and student list with pagination
        /// </summary>
        /// <param name="id"> id of class</param>
        /// <returns> 200: Detail of class  / 404: class not found </returns>
        [HttpGet("{id:int}")]
        [Authorize(Policy = "ClassGet")]
        public async Task<ActionResult<ClassDetailResponse>> ViewClassDetail(int id)
        {
            Class s = await _classService.GetClassDetail(id);
            if (s == null)
            {
                return NotFound(new ResponseDTO(404, "Class not found"));
            }

            var cdr = _mapper.Map<ClassDetailResponse>(s);
            cdr.Trainer = _mapper.Map<List<TrainerResponse>>(await _trainerService.GetTrainerListByClassId(id));
            var rooms = await _roomService.GetRoomByClassId(id);
            foreach (var item in rooms)
            {
                cdr.RoomName.Add(item.RoomName);
            }
            return Ok(cdr);
        }

        /// <summary>
        /// Get the detail information of a class 
        /// </summary>
        /// <param name="id"> id of class</param>
        /// <returns> 200: Detail of class  / 404: class not found </returns>
        [HttpGet("trainee/{traineeId:int}")]
        [Authorize(Policy = "ClassGet")]
        public async Task<ActionResult<ClassDetailResponse>> ViewClassDetailByTraineeId(int traineeId)
        {
            var (classId, message) = await _traineeService.GetClassIdByTraineeId(traineeId);

            if (classId == -1)
            {
                return NotFound(new ResponseDTO(404, message));
            }

            var clazz = await _classService.GetClassDetail(classId);

            if (clazz == null)
            {
                return NotFound(new ResponseDTO(404, "Class not found"));
            }
            ClassDetailResponse cdr = _mapper.Map<ClassDetailResponse>(clazz);
            cdr.Trainer = _mapper.Map<List<TrainerResponse>>(await _trainerService.GetTrainerListByClassId(classId));
            var rooms = await _roomService.GetRoomByClassId(classId);
            foreach (var item in rooms)
            {
                cdr.RoomName.Add(item.RoomName);
            }
            return Ok(cdr);

        }

        /// <summary>
        /// Get trainee list with pagination
        /// </summary>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of trainee list in a class with pagination / 404: search trainee name not found</returns>
        [HttpGet("{id:int}/trainee")]
        [Authorize(Policy = "ClassGet")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TraineeResponse>>>> GetTraineeListByClassId(int id, [FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<Trainee> trainees) = await _classService.GetTraineesByClassId(id, paginationParameter);
            IEnumerable<TraineeResponse> traineeDTO = _mapper.Map<IEnumerable<TraineeResponse>>(trainees);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Search trainee name not found"));
            }
            return Ok(new PaginationResponse<IEnumerable<TraineeResponse>>(totalRecord, traineeDTO));
        }

        /// <summary>
        /// Insert the request delete class to db
        /// </summary>
        /// <param name="requestDeleteClassInput">Request detail</param>
        /// <returns>201: Request is created / 404: Class/Admin is not exist / 409: Fail to request delete class</returns>
        [HttpPost("request")]
        [Authorize(Policy = "ClassPost")]
        public async Task<ActionResult> CreateRequestDeleteClass(RequestDeleteClassInput requestDeleteClassInput)
        {
            DeleteClassRequest deleteClassRequest = _mapper.Map<DeleteClassRequest>(requestDeleteClassInput);
            int rs = await _classService.InsertNewRequestDeleteClass(deleteClassRequest);
            if (rs == -2)
            {
                return NotFound(new ResponseDTO(404, "Admin is not exist"));
            }
            else if (rs == -1)
            {
                return NotFound(new ResponseDTO(404, "Class is not exist"));
            }
            else if (rs == 0)
            {
                return Conflict(new ResponseDTO(409, "Fail to request delete class"));
            }
            else
            {
                return Ok(new ResponseDTO(201, "Request delete class success"));
            }
        }

        /// <summary>
        /// Insert new class to db
        /// </summary>
        /// <param name="newClassInput">Detail of new class</param>
        /// <returns>201: Class is created / 409: Classname exist || Trainees or trainers already have class</returns>
        [HttpPost]
        [Authorize(Policy = "ClassPost")]
        public async Task<ActionResult> CreateNewClass([FromBody] NewClassInput newClassInput)
        {
            // Check number of module must be equal or less than number of month
            var start = newClassInput.StartDay;
            var end = newClassInput.EndDay;
            end = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month));// set end-date to end of month
            var listMonth = Enumerable.Range(0, Int32.MaxValue)
                                .Select(e => start.AddMonths(e))
                                .TakeWhile(e => e <= end)
                                .Select(e => e);     // get the list of month of class duration
            if (listMonth.Count() < newClassInput.TrainerModuleList.Count())
            {
                return BadRequest(new ResponseDTO(400, "Number of modules must be equal or less than number of months"));
            }
            //Check admin exist
            if (_adminService.CheckAdminExist(newClassInput.AdminId) is false)
            {
                return NotFound(new ResponseDTO
                {
                    Status = 404,
                    Message = "Admin does not exist"
                });
            }
            //Check Trainer exist to teach
            foreach (var item in newClassInput.TrainerModuleList)
            {
                if (_trainerService.CheckTrainerExist(item.TrainerId) is false)
                {
                    return NotFound(new ResponseDTO
                    {
                        Status = 404,
                        Message = "Trainer does not exist"
                    });
                }
            }
            // Check trainee exist
            foreach (var traineeId in newClassInput.TraineeIdList)
            {
                if (_traineeService.CheckTraineeExist(traineeId) is false)
                {
                    return NotFound(new ResponseDTO
                    {
                        Status = 404,
                        Message = "Trainee does not exist"
                    });
                }
            }
            //Check module exist
            foreach (var module in newClassInput.TrainerModuleList)
            {
                var moduleToAssign = await _moduleService.GetModuleById(module.ModuleId);
                if (moduleToAssign == null)
                {
                    return NotFound(new ResponseDTO(404, "Module is not exist"));
                }
            }
            // Check Trainer available  (Trainer is free in timetable or not)
            (bool isTrainerAvailable, string message) = _timetableService.CheckTrainersNewClass(newClassInput.TrainerModuleList, newClassInput.StartDay, newClassInput.EndDay);
            if (isTrainerAvailable is false)
            {
                return BadRequest(new ResponseDTO(400, message));
            }
            // Insert class and attendance in transaction, if attendance fail, roll back before insert classs
            using (var dbContextTransaction = _datacontext.Database.BeginTransaction())
            {
                try
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
                        return Conflict(new ResponseDTO(409, "One or more trainees already have class "));
                    }
                    else if (rs == 0)
                    {
                        return BadRequest(new ResponseDTO(400, "Some error occur"));
                    }
                    var classGet = await _classService.GetClassByClassName(newClassInput.ClassName);
                    int attRs = await _attendanceServices.InitAttendanceWhenCreateClass(classGet.ClassId);
                    if (attRs != 1)
                    {
                        throw new Exception();
                    }
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback();
                    return BadRequest(new ResponseDTO(400, "Can't create attendance"));
                }
            }

            return Created("", new ResponseDTO(201, "Successfully inserted class with timetable"));
        }

        /// <summary>
        /// Insert a list class to db using excel file
        /// </summary>
        /// <param name="file">Excel file to stores class data</param>
        /// <returns>201: Class is created / 400: File content inapproriate / 409: There are errors</returns>
        [HttpPost("excel")]
        [Authorize(Policy = "ClassPost")]
        public async Task<ActionResult<ExcelResponseDTO>> CreateNewClassByExcel([FromForm] IFormFile file)
        {
            bool success;
            string message;
            List<ExcelErrorDTO> excelErrors = new();

            (success, message) = FileHelper.CheckExcelExtension(file);
            if (!success)
            {
                return BadRequest(new ExcelResponseDTO(400, message)
                {
                    Errors = excelErrors
                });
            }

            // Initialize Excel Package
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);

            // Check sheets
            ExcelWorkbook workbook = package.Workbook;
            ExcelWorksheet classSheet = workbook.Worksheets["Class"];
            ExcelWorksheet moduleSheet = workbook.Worksheets["Module"];
            ExcelWorksheet traineeSheet = workbook.Worksheets["Trainee"];
            if (classSheet is null || moduleSheet is null || traineeSheet is null)
            {
                return BadRequest(new ExcelResponseDTO(400, "Missing required sheets")
                {
                    Errors = excelErrors
                });
            }

            // Export Class-Module dictionary
            Dictionary<string, ICollection<TrainerModule>> classModulesDict = new();
            Dictionary<TrainerModule, int> moduleRowDict = new();
            static bool moduleChecker(List<string> list) => list.ContainsAll("class", "module", "trainer", "weight");
            List<Dictionary<string, object>> moduleDictList = moduleSheet.ExportDataFromExcel(moduleChecker, out success, out message);
            if (!success)
            {
                return BadRequest(new ExcelResponseDTO(400, "Error on Module: " + message)
                {
                    Errors = excelErrors
                });
            }
            for (int i = 0; i < moduleDictList.Count; i++)
            {
                int row = i + 2; // row starts from 1, but we skip the first row as it's the column name
                Dictionary<string, object> dict = moduleDictList[i];

                // Get from cells
                string className = dict["class"]?.ToString();
                string module = dict["module"]?.ToString();
                string trainerEmail = dict["trainer"]?.ToString();
                string weight = dict["weight"]?.ToString();
                if (className is null || module is null || trainerEmail is null || weight is null)
                {
                    continue;
                }

                // Validate module & weight number format
                if (!int.TryParse(module, out int moduleId))
                {
                    excelErrors.Add(new()
                    {
                        Sheet = moduleSheet.Name,
                        Row = row,
                        ColumnName = "module",
                        Value = module,
                        Error = "Module is in invalid format"
                    });
                    continue;
                }
                if (!float.TryParse(weight, out float weightNumber))
                {
                    excelErrors.Add(new()
                    {
                        Sheet = moduleSheet.Name,
                        Row = row,
                        ColumnName = "weight",
                        Value = weight,
                        Error = "Weight is in invalid format"
                    });
                    continue;
                }

                // Get the trainer module list or create one
                ICollection<TrainerModule> list;
                if (classModulesDict.ContainsKey(className))
                {
                    list = classModulesDict[className];
                }
                else
                {
                    list = new List<TrainerModule>();
                    classModulesDict[className] = list;
                }

                // Validate and get trainer id
                Trainer trainer = await _trainerService.GetTrainerByEmail(trainerEmail);
                if (trainer is null)
                {
                    excelErrors.Add(new()
                    {
                        Sheet = moduleSheet.Name,
                        Row = row,
                        ColumnName = "trainer",
                        Value = trainerEmail,
                        Error = "Trainer is not found"
                    });
                    continue;
                }
                int trainerId = trainer.TrainerId;

                // Validate module id
                if (await _moduleService.GetModuleById(moduleId) is null)
                {
                    excelErrors.Add(new()
                    {
                        Sheet = moduleSheet.Name,
                        Row = row,
                        ColumnName = "module",
                        Value = moduleId,
                        Error = "Module is not found"
                    });
                    continue;
                }

                // Add to list
                TrainerModule trainerModule = new()
                {
                    TrainerId = trainerId,
                    ModuleId = moduleId,
                    WeightNumber = weightNumber
                };
                list.Add(trainerModule);
                moduleRowDict[trainerModule] = row;
            }


            // Export Class-Trainee dictionary
            Dictionary<string, HashSet<int>> classTraineesDict = new();
            Dictionary<int, int> traineeRowDict = new();
            static bool traineeChecker(List<string> list) => list.ContainsAll("class", "email");
            List<Dictionary<string, object>> traineeDictList = traineeSheet.ExportDataFromExcel(traineeChecker, out success, out message);
            if (!success)
            {
                return BadRequest(new ExcelResponseDTO(400, "Error on Trainee: " + message)
                {
                    Errors = excelErrors
                });
            }
            for (int i = 0; i < traineeDictList.Count; i++)
            {
                int row = i + 2; // row starts from 1, but we skip the first row as it's the column name
                Dictionary<string, object> dict = traineeDictList[i];

                // Get from cells
                string className = dict["class"]?.ToString();
                string email = dict["email"]?.ToString();
                if (className is null || email is null)
                {
                    continue;
                }

                // Get Trainee Id
                int? traineeId = (await _traineeService.GetTraineeByEmail(email))?.TraineeId;
                if (!traineeId.HasValue)
                {
                    excelErrors.Add(new()
                    {
                        Sheet = traineeSheet.Name,
                        Row = row,
                        ColumnName = "email",
                        Value = email,
                        Error = "Trainee is not found"
                    });
                    continue;
                }

                // Get the trainee list or create one
                HashSet<int> list;
                if (classTraineesDict.ContainsKey(className))
                {
                    list = classTraineesDict[className];
                }
                else
                {
                    list = new HashSet<int>();
                    classTraineesDict[className] = list;
                }

                // Add to list
                list.Add(traineeId.Value);
                traineeRowDict[traineeId.Value] = row;
            }

            // Export Class list
            Dictionary<string, NewClassInput> classInputNameDict = new();
            Dictionary<int, NewClassInput> classInputRowDict = new();
            static bool classChecker(List<string> list) => list.ContainsAll("name", "description", "admin", "start", "end");
            List<Dictionary<string, object>> classDictList = classSheet.ExportDataFromExcel(classChecker, out success, out message);
            if (!success)
            {
                return BadRequest(new ExcelResponseDTO(400, "Error on Class: " + message)
                {
                    Errors = excelErrors
                });
            }

            // Export Classes
            for (int i = 0; i < classDictList.Count; i++)
            {
                int row = i + 2; // row starts from 1, but we skip the first row as it's the column name
                Dictionary<string, object> dict = classDictList[i];

                // Create new class input
                NewClassInput classInput = new();

                // Set class name
                classInput.ClassName = dict["name"]?.ToString();
                if (classInput.ClassName is null)
                {
                    excelErrors.Add(new()
                    {
                        Sheet = classSheet.Name,
                        Row = row,
                        ColumnName = "name",
                        Value = classInput.ClassName,
                        Error = "Class name is empty"
                    });
                    continue;
                }

                // Set description
                classInput.Description = dict["description"]?.ToString();

                // Set start time
                object start = dict["start"];
                if (start is not null && start is DateTime startTine)
                {
                    classInput.StartDay = startTine;
                }

                // Set end time
                object end = dict["end"];
                if (end is not null && end is DateTime endTime)
                {
                    classInput.EndDay = endTime;
                }

                // Set Admin
                string adminEmail = dict["admin"]?.ToString();
                if (adminEmail is not null)
                {
                    Admin admin = await _adminService.GetAdminByEmail(adminEmail);
                    if (admin is null)
                    {
                        excelErrors.Add(new()
                        {
                            Sheet = classSheet.Name,
                            Row = row,
                            ColumnName = "admin",
                            Value = adminEmail,
                            Error = "Admin is not found"
                        });
                        continue;
                    }
                    else
                    {
                        classInput.AdminId = admin.AdminId;
                    }
                }

                // Add class input to dictionary
                classInputNameDict[classInput.ClassName] = classInput;
                classInputRowDict[row] = classInput;
            }

            // Add Modules to Class
            foreach (var modulePair in classModulesDict)
            {
                if (!classInputNameDict.ContainsKey(modulePair.Key)) continue;
                var clazz = classInputNameDict[modulePair.Key];
                var trainerModules = modulePair.Value;
                if (clazz is null)
                {
                    continue;
                }

                // Check number of module must be equal or less than number of month
                var start = clazz.StartDay;
                var end = clazz.EndDay;
                end = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month));// set end-date to end of month
                var listMonth = Enumerable.Range(0, int.MaxValue)
                                    .Select(e => start.AddMonths(e))
                                    .TakeWhile(e => e <= end)
                                    .Select(e => e);     // get the list of month of class duration
                if (listMonth.Count() < trainerModules.Count)
                {
                    foreach (var trainerModule in trainerModules)
                    {
                        excelErrors.Add(new()
                        {
                            Sheet = moduleSheet.Name,
                            Row = moduleRowDict[trainerModule],
                            Value = trainerModule,
                            Error = "Number of modules must be equal or less than number of months"
                        });
                    }
                    continue;
                }

                // Check if trainers are available
                (bool isAvailable, string errorMessage) = _timetableService.CheckTrainersNewClass(trainerModules, clazz.StartDay, clazz.EndDay);

                // Add if trainers are available
                if (isAvailable)
                    clazz.TrainerModuleList = trainerModules;
                else
                    foreach (var trainerModule in trainerModules)
                    {
                        excelErrors.Add(new()
                        {
                            Sheet = moduleSheet.Name,
                            Row = moduleRowDict[trainerModule],
                            Value = trainerModule,
                            Error = "Number of modules must be equal or less than number of months"
                        });
                    }
            }

            // Add Trainees to Class
            foreach (var traineePair in classTraineesDict)
            {
                // Check if trainees are free
                foreach (var traineeId in traineePair.Value)
                {
                    if (await _traineeService.IsTraineeHasClass(traineeId))
                    {
                        var trainee = await _traineeService.GetTraineeById(traineeId);
                        if (trainee is not null)
                        {
                            excelErrors.Add(new()
                            {
                                Sheet = traineeSheet.Name,
                                Row = traineeRowDict[traineeId],
                                ColumnName = "email",
                                Value = trainee.Email,
                                Error = $"Trainee {trainee.Username} ({trainee.Email}) has a class"
                            });
                        }
                    }
                }

                // Add trainees to class
                if (!classInputNameDict.ContainsKey(traineePair.Key)) continue;
                var clazz = classInputNameDict[traineePair.Key];
                if (clazz is null) continue;
                clazz.TraineeIdList = traineePair.Value;
            }

            // Start Transaction
            using var dbContextTransaction = _datacontext.Database.BeginTransaction();

            // Insert Classes
            try
            {
                foreach (var rowClassInput in classInputRowDict)
                {
                    var row = rowClassInput.Key;
                    var classInput = rowClassInput.Value;

                    // Validate class input
                    if (!classInput.Validate(out List<ValidationResult> validateResults))
                    {
                        excelErrors.Add(new()
                        {
                            Sheet = classSheet.Name,
                            Row = row,
                            Value = classInput,
                            Errors = validateResults.Select(x => x.ErrorMessage).ToList()
                        });
                        continue;
                    }

                    int result = await _classService.InsertNewClass(classInput);
                    if (result <= 0)
                    {
                        excelErrors.Add(new()
                        {
                            Sheet = classSheet.Name,
                            Row = row,
                            Value = classInput,
                            Error = result switch
                            {
                                -1 => "Duplicate class name",
                                -2 => "Trainee already have class",
                                _ => "Unknown Error"
                            }
                        });
                        continue;
                    }

                    // Initialize Attendance
                    var className = classInput.ClassName;
                    var clazz = await _classService.GetClassByClassName(className);
                    int attRs = await _attendanceServices.InitAttendanceWhenCreateClass(clazz.ClassId);
                    if (attRs != 1)
                    {
                        excelErrors.Add(new()
                        {
                            Sheet = traineeSheet.Name,
                            Row = row,
                            Value = classInput,
                            Error = attRs switch
                            {
                                -2 => $"Class {className} already have attendance",
                                -1 => $"Class {className} did not exist",
                                0 => $"Class {className} failed to init attendance",
                                _ => "Unknown Error"
                            }
                        });
                    }
                }
            }
            catch
            {
                dbContextTransaction.Rollback();
                throw;
            }

            // Rollback if there are errors
            if (excelErrors.Count > 0)
            {
                dbContextTransaction.Rollback();
                return Conflict(new ExcelResponseDTO(409, "There are errors. Check Errors for Details")
                {
                    Errors = excelErrors
                });
            }
            else
            {
                dbContextTransaction.Commit();
                return CreatedAtAction(nameof(GetClassList), new ExcelResponseDTO(201, "Created")
                {
                    Errors = excelErrors
                });
            }
        }

        /// <summary>
        /// Get class list of trainer
        /// </summary>
        /// <param name="id">Trainer id</param>
        /// <param name="paginationParameter"></param>
        /// <returns>List with pagination/ 404: Not found</returns>
        [HttpGet("trainer/{id:int}")]
        [Authorize(Policy = "ClassGet")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TrainerClassListResponse>>>> GetClassListByTrainerId(int id, [FromQuery] PaginationParameter paginationParameter)
        {
            if (!_trainerService.CheckTrainerExist(id))
            {
                return NotFound(new ResponseDTO(404, "Trainer not found"));
            }
            (int totalRecord, IEnumerable<Class> classList) = await _classService.GetClassListByTrainerId(id, paginationParameter);
            IEnumerable<TrainerClassListResponse> classListDto = _mapper.Map<IEnumerable<TrainerClassListResponse>>(classList);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Classes not found"));
            }
            return Ok(new PaginationResponse<IEnumerable<TrainerClassListResponse>>(totalRecord, classListDto));
        }

        /// <summary>
        /// Assign a module to a trainer class
        /// </summary>
        /// <param name="trainerId">trainer id</param>
        /// <param name="assignModuleInput">AssignModuleInput</param>
        /// <returns>200: Assigned / 404: Class/Trainer is not exist / 409: Fail to assign</returns>
        [HttpPost("module")]
        [Authorize(Policy = "ClassModule")]
        public async Task<ActionResult> AssignModuleToClass([FromBody] AssignModuleInput assignModuleInput)
        {
            var moduleToAssign = await _moduleService.GetModuleById(assignModuleInput.ModuleId);
            if (moduleToAssign == null)
            {
                return NotFound(new ResponseDTO(404, "Module is not exist"));
            }

            var isClassExist = _classService.CheckClassExist(assignModuleInput.ClassId);
            if (isClassExist == false)
            {
                return NotFound(new ResponseDTO(404, "Class is not exist"));
            }

            var isTrainerExist = _trainerService.CheckTrainerExist(assignModuleInput.TrainerId);
            if (isTrainerExist == false)
            {
                return NotFound(new ResponseDTO(404, "Trainer is not exist"));
            }

            bool isDayleftAvailable = _timetableService.DayLeftAvailableCheck(assignModuleInput.ModuleId, assignModuleInput.ClassId);
            if (isDayleftAvailable == false)
            {
                return Conflict(new ResponseDTO(409, "Not enough day left to assign this module to class"));
            }

            Class classGet = await _classService.GetClassByClassID(assignModuleInput.ClassId);
            var startDay = _timetableService.GetStartDayforClassToInsertModule(assignModuleInput.ClassId);
            (bool isTrainerAvailable, DateTime date) = _timetableService.CheckTrainerAvailableForModule(startDay, classGet.EndDay, assignModuleInput.TrainerId, assignModuleInput.ModuleId);
            if (isTrainerAvailable == false)
            {
                return Conflict(new ResponseDTO(409, "Trainer is not available for teaching this module"));
            }

            var classModuleInfor = await _classService.GetClassModule(assignModuleInput.ClassId, assignModuleInput.ModuleId);
            if (classModuleInfor != null)
            {
                return Conflict(new ResponseDTO(409, "Module is already assigned to class"));
            }

            ClassModule classModule = _mapper.Map<ClassModule>(assignModuleInput);
            (int roomId, DateTime date2) = _timetableService.GetRoomIdAvailableForModule(startDay, classGet.EndDay, assignModuleInput.ModuleId);
            classModule.RoomId = roomId;
            int insertCalendar = await _timetableService.InsertCalendarsToClass(assignModuleInput.ClassId, assignModuleInput.ModuleId);
            int rs = await _classService.AssignModuleToClass(classModule);
            if (rs == 0)
            {
                return Conflict(new ResponseDTO(409, "Fail to assign module to class"));
            }
            else
            {
                return Ok(new ResponseDTO(200, "Assign module to class success"));
            }
        }

        /// <summary>
        /// Delete a module of class by trainer
        /// </summary>
        /// <param name="classId">class id</param>
        /// <param name="trainerId">trainer id</param>
        /// <param name="moduleId">module id</param>
        /// <returns>200: Deleted / 404: Class/Trainer/Module is not exist / 409: Fail to delete</returns>
        [HttpDelete("module/{moduleId:int}")]
        [Authorize(Policy = "ClassModule")]
        public async Task<ActionResult> RemoveModule(RemoveModuleInput assignModuleInput)
        {
            var classInfor = await _classService.GetClassByClassID(assignModuleInput.ClassId);
            if (classInfor == null)
            {
                return NotFound(new ResponseDTO(404, "Class is not exist"));
            }
            var moduleMarkState = await _markService.GetMarkByModuleId(assignModuleInput.ModuleId, null, null);
            if (moduleMarkState.Count() != 0)
            {
                return NotFound(new ResponseDTO(404, "Trainee in this class has mark with this module"));
            }

            var moduleCertificateState = await _certificateService.GetCertificatesURLByModuleId(assignModuleInput.ModuleId);
            if (moduleCertificateState.Count() != 0)
            {
                return NotFound(new ResponseDTO(404, "Trainee in this class has certificate with this module"));
            }

            var classModuleInfor = await _classService.GetClassModule(assignModuleInput.ClassId, assignModuleInput.ModuleId);
            if (classModuleInfor == null)
            {
                return NotFound(new ResponseDTO(404, "Class-Module not found"));
            }
            if (classModuleInfor.TrainerId != assignModuleInput.TrainerId)
            {
                return BadRequest(new ResponseDTO(409, "This trainer dont teach this class"));
            }

            int removeStatus = await _classService.RemoveModuleFromClass(assignModuleInput.ClassId, assignModuleInput.ModuleId);
            if (removeStatus == -1)
            {
                return BadRequest(new ResponseDTO(409, "Class does not have this module"));
            }
            else if (removeStatus == 1)
            {
                return Ok(new ResponseDTO(200, "Deleted!"));
            }
            else
            {
                return BadRequest(new ResponseDTO(409, "Fail to delete! Unexpected error"));
            }
        }

        [HttpGet("admin/{id:int}")]
        public async Task<ActionResult<ICollection<AdminDashboardClassResponse>>> GetAdminClassByYear(int id, [FromQuery] DateTime at)
        {
            if (at == default(DateTime))
            {
                at = DateTime.Now;
            }

            var verify = _adminService.CheckAdminExist(id);
            if (verify == false)
            {
                return NotFound(new ResponseDTO(404, "Admin id not found"));
            }

            var classList = await _classService.GetClassListByAdminId(id, at);

            if (classList == null)
            {
                return NotFound(new ResponseDTO(404, "Admin does not have any class"));
            }

            return Ok(_mapper.Map<ICollection<AdminDashboardClassResponse>>(classList));
        }
    }
}