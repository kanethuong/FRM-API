using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.MarkDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TraineeDTO;
using kroniiapi.Helper.Upload;
using kroniiapi.Helper.UploadDownloadFile;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using kroniiapi.Helper;
using Microsoft.AspNetCore.Authorization;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarkController : ControllerBase
    {
        private readonly ITraineeService _traineeService;
        private readonly ICertificateService _certificateService;
        private readonly IMapper _mapper;
        private readonly IMegaHelper _megaHelper;
        private readonly IModuleService _moduleService;
        private readonly IClassService _classService;
        private readonly IMarkService _markService;
        private readonly ITrainerService _trainerService;
        public MarkController(ITraineeService traineeService,
                            ICertificateService certificateService,
                            IMegaHelper megaHelper,
                            IMapper mapper,
                            IModuleService moduleService,
                            IMarkService markService,
                            IClassService classService,
                            ITrainerService trainerService
                            )
        {
            _traineeService = traineeService;
            _certificateService = certificateService;
            _megaHelper = megaHelper;
            _mapper = mapper;
            _moduleService = moduleService;
            _markService = markService;
            _classService = classService;
            _trainerService = trainerService;
        }

        /// <summary>
        /// View trainee mark and skill
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <returns>Trainee mark and skill</returns>
        [HttpGet("trainee/{traineeId:int}")]
        [Authorize(Policy = "MarkGet")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TraineeMarkAndSkill>>>> ViewMarkAndSkill(int traineeId, [FromQuery] PaginationParameter paginationParameter)
        {
            var existedTrainee = await _traineeService.GetTraineeById(traineeId);
            if (existedTrainee == null)
            {
                return NotFound(new ResponseDTO(404, "id not found"));
            }
            (int totalRecord, IEnumerable<TraineeMarkAndSkill> markAndSkills) = await _traineeService.GetMarkAndSkillByTraineeId(traineeId, paginationParameter);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Trainee doesn't have any module"));
            }
            return Ok(new PaginationResponse<IEnumerable<TraineeMarkAndSkill>>(totalRecord, markAndSkills));
        }

        /// <summary>
        /// submit trainee certificate (upload to mega)
        /// </summary>
        /// <param name="certificateInput">detail of certificate input</param>
        /// <returns>201: Created / 400: Bad request / 404: Module or Trainee not found</returns>
        [HttpPost("certificate")]
        [Authorize(Policy = "MarkPost")]
        public async Task<ActionResult> SubmitCertificate([FromForm] IFormFile file, [FromForm] CertificateInput certificateInput)
        {
            string message_img;
            bool success_img;
            (success_img, message_img) = FileHelper.CheckImageExtension(file);
            if (!success_img)
            {
                return BadRequest(new ResponseDTO(400, "Your submission file must be image"));
            }
            Stream stream = file.OpenReadStream();
            string Uri = await _megaHelper.Upload(stream, file.FileName, "Certificate");
            Certificate certificate = _mapper.Map<Certificate>(certificateInput);
            certificate.CertificateURL = Uri;
            int status = await _certificateService.InsertCertificate(certificate);
            if (status == -1 || status == -2)
            {
                return NotFound(new ResponseDTO(404, "Cannot find module and trainee or trainee was deactivated!"));
            }
            else if (status == 0)
            {
                return BadRequest(new ResponseDTO(400, "Your submission failed!"));
            }
            return Created("", new ResponseDTO(201, "Your submission was successful!"));
        }

        /// <summary>
        /// Get the student mark with pagination of a class
        /// </summary>
        /// <param name="id">id of class</param>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of student mark in a class with pagination / 404: search student name not found</returns>
        [HttpGet("class/{classId:int}")]
        //[Authorize(Policy = "MarkGet")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<MarkResponse>>>> ViewClassScore(int classId, [FromQuery] PaginationParameter paginationParameter)
        {
            var class1 = await _classService.GetClassByClassID(classId);
            if (class1 == null || class1.IsDeactivated == true)
            {
                return NotFound(new ResponseDTO(404, "Class not found!"));
            }
            (int totalRecords, IEnumerable<Trainee> trainees) = await _classService.GetTraineesByClassId(classId, paginationParameter);
            var moduleList = await _moduleService.GetModulesByClassId(classId);
            List<MarkResponse> markResponses = new List<MarkResponse>();
            foreach (Trainee trainee in trainees)
            {
                MarkResponse markResponse = new MarkResponse();
                markResponse.TraineeName = trainee.Fullname;

                var mark_empty = new List<Mark>();
                var markList = new List<Mark>();
                foreach (var module in moduleList)
                {
                    var traineeMark = await _markService.GetMarkByTraineeIdAndModuleId(trainee.TraineeId, module.ModuleId, class1.StartDay, class1.EndDay);
                    if (traineeMark == null)
                    {
                        Mark mark_zero = new Mark();
                        mark_zero.TraineeId = trainee.TraineeId;
                        mark_zero.ModuleId = module.ModuleId;
                        mark_zero.Score = 0;
                        markList.Add(mark_zero);
                    }
                    else
                    {
                        markList.Add(traineeMark);
                    }
                }
                foreach (Mark m in markList)
                {
                    m.Module = await _moduleService.GetModuleById(m.ModuleId);
                }
                markList.OrderBy(m => m.Module.ModuleId);
                markResponse.ScoreList = _mapper.Map<List<ModuleMark>>(markList);
                markResponses.Add(markResponse);
            }
            if (totalRecords == 0)
            {
                return NotFound(new ResponseDTO(404, "Search student name not found!"));
            }
            return Ok(new PaginationResponse<IEnumerable<MarkResponse>>(totalRecords, markResponses));
        }

        /// <summary>
        /// Get score of a trainer's class
        /// </summary>
        /// <param name="classId">class id</param>
        /// <param name="trainerId">trainer id</param>
        /// <param name="paginationParameter">pagination</param>
        /// <returns>200: List of student mark in a class with pagination / 404:not found</returns>
        [HttpGet("trainer/class/{classId:int}")]
        [Authorize(Policy = "MarkGet")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<MarkResponse>>>> ViewClassScoreByTrainerId(int classId, int trainerId, [FromQuery] PaginationParameter paginationParameter)
        {
            var class1 = await _classService.GetClassByClassID(classId);
            if (!_trainerService.CheckTrainerExist(trainerId) || !_classService.CheckClassExist(classId))
            {
                return NotFound(new ResponseDTO(404, "Trainer not found or Class not found"));
            }
            var moduleList = await _moduleService.GetModulesByClassIdAndTrainerId(classId, trainerId);
            if (moduleList.Count() == 0)
            {
                return NotFound(new ResponseDTO(404, "Cannot find that Trainer in this Class"));
            }

            (int totalRecords, IEnumerable<Trainee> trainees) = await _classService.GetTraineesByClassId(classId, paginationParameter);

            List<MarkResponse> markResponses = new List<MarkResponse>();
            foreach (Trainee trainee in trainees)
            {
                MarkResponse markResponse = new MarkResponse();
                markResponse.TraineeName = trainee.Fullname;

                var mark_empty = new List<Mark>();
                var markList = new List<Mark>();
                foreach (var module in moduleList)
                {
                    var traineeMark = await _markService.GetMarkByTraineeIdAndModuleId(trainee.TraineeId, module.ModuleId, class1.StartDay, class1.EndDay);
                    if (traineeMark == null)
                    {
                        Mark mark_zero = new Mark();
                        mark_zero.TraineeId = trainee.TraineeId;
                        mark_zero.ModuleId = module.ModuleId;
                        mark_zero.Score = 0;
                        markList.Add(mark_zero);
                    }
                    else
                    {
                        markList.Add(traineeMark);
                    }
                }
                foreach (Mark m in markList)
                {
                    m.Module = await _moduleService.GetModuleById(m.ModuleId);
                }
                markList.OrderBy(m => m.Module.ModuleId);
                markResponse.ScoreList = _mapper.Map<List<ModuleMark>>(markList);
                markResponses.Add(markResponse);
            }
            if (totalRecords == 0)
            {
                return NotFound(new ResponseDTO(404, "Search student name not found!"));
            }
            return Ok(new PaginationResponse<IEnumerable<MarkResponse>>(totalRecords, markResponses));
        }

        /// <summary>
        /// Change class sore
        /// </summary>
        /// <param name="traineeId">trainee id</param>
        /// <param name="traineeMarkInput">Trainee Module and Mark</param>
        /// <returns>200: Updated / 409: Conflict / 404: trainee not found</returns>
        [HttpPut("trainee")]
        [Authorize(Policy = "MarkPut")]
        public async Task<ActionResult> ChangeClassScore([FromBody] TraineeMarkInput traineeMarkInput)
        {
            Mark mark = _mapper.Map<Mark>(traineeMarkInput);
            Trainee trainee = await _traineeService.GetTraineeById(mark.TraineeId);
            if (trainee == null)
            {
                return NotFound(new ResponseDTO(404, "Trainee not found"));
            }
            Module module = await _moduleService.GetModuleById(mark.ModuleId);
            if (module == null)
            {
                return NotFound(new ResponseDTO(404, "Module not found"));
            }
            var checkExist = await _markService.GetMarkByTraineeIdAndModuleId(mark.TraineeId, mark.ModuleId);
            if (checkExist == null)
            {
                return NotFound(new ResponseDTO(404, "Trainee does not study this module"));
            }
            if (checkExist.ModuleId == mark.ModuleId && checkExist.TraineeId == mark.TraineeId && checkExist.Score == mark.Score)
            {
                return Ok(new ResponseDTO(200, "Update trainee's score success"));
            }
            if (mark.Score < 0)
            {
                return BadRequest(new ResponseDTO(400, "Score cannot be negative"));
            }
            if (await _markService.UpdateMark(mark) == 1)
            {
                return Ok(new ResponseDTO(200, "Update trainee's score success"));
            }
            else
            {
                return Conflict(new ResponseDTO(409, "Fail to update trainee score"));
            }

        }

        /// <summary>
        /// View trainee mark
        /// </summary>
        /// <param name="traineeId">trainee id</param>
        /// <returns>200: Trainee mark response/ 404: Trainee not found</returns>
        [HttpGet("{traineeId:int}/Score")]
        [Authorize(Policy = "MarkGet")]
        public async Task<ActionResult<MarkResponse>> ViewTraineeMark(int traineeId)
        {
            Trainee trainee = await _traineeService.GetTraineeById(traineeId);
            if (trainee == null)
            {
                return NotFound(new ResponseDTO(404, "Trainee not found"));
            }
            var moduleList = await _moduleService.GetModulesByTraineeId(traineeId);

            MarkResponse markResponse = new MarkResponse();
            markResponse.TraineeName = trainee.Fullname;
            var markList = new List<Mark>();
            foreach (var module in moduleList)
            {
                var traineeMark = await _markService.GetMarkByTraineeIdAndModuleId(trainee.TraineeId, module.ModuleId);
                if (traineeMark == null)
                {
                    Mark mark_zero = new Mark();
                    mark_zero.TraineeId = trainee.TraineeId;
                    mark_zero.ModuleId = module.ModuleId;
                    mark_zero.Score = 0;
                    markList.Add(mark_zero);
                }
                else
                {
                    markList.Add(traineeMark);
                }
            }
            foreach (Mark m in markList)
            {
                m.Module = await _moduleService.GetModuleById(m.ModuleId);
            }
            markList.OrderBy(m => m.Module.ModuleId);
            markResponse.ScoreList = _mapper.Map<List<ModuleMark>>(markList);
            return Ok(_mapper.Map<MarkResponse>(markResponse));
        }

    }
}