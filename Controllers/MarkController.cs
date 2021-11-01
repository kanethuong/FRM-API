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

        public MarkController(ITraineeService traineeService,
                            ICertificateService certificateService,
                            IMegaHelper megaHelper,
                            IMapper mapper,
                            IModuleService moduleService,
                            IMarkService markService,
                            IClassService classService
                            )
        {
            _traineeService = traineeService;
            _certificateService = certificateService;
            _megaHelper = megaHelper;
            _mapper = mapper;
            _moduleService = moduleService;
            _markService = markService;
            _classService = classService;
        }

        /// <summary>
        /// View trainee mark and skill
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <returns>Trainee mark and skill</returns>
        [HttpGet("trainee/{traineeId:int}")]
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
        public async Task<ActionResult> SubmitCertificate([FromForm] IFormFile file, [FromForm] CertificateInput certificateInput)
        {
            string message_img, message_pdf;
            bool success_img, success_pdf;
            (success_img, message_img) = FileHelper.CheckImageExtension(file);
            (success_pdf, message_pdf) = FileHelper.CheckPDFExtension(file);
            if (!success_img && !success_pdf)
            {
                return BadRequest(new ResponseDTO(400, "Your submission file must be PDF or image"));
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
            return Created("",new ResponseDTO(201, "Your submission was successful!"));
        }
        /// <summary>
        /// Get the student mark with pagination of a class
        /// </summary>
        /// <param name="id">id of class</param>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: List of student mark in a class with pagination / 404: search student name not found</returns>
        [HttpGet("class/{classId:int}")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<MarkResponse>>>> ViewClassScore(int classId, [FromQuery] PaginationParameter paginationParameter)
        {
            var class1 = await _classService.GetClassByClassID(classId);
            if (class1 == null || class1.IsDeactivated == true)
            {
                return NotFound(new ResponseDTO(404, "Class not found!"));
            }
            (int totalRecords, IEnumerable<Trainee> trainees) = await _classService.GetTraineesByClassId(classId, paginationParameter);
            List<MarkResponse> markResponses = new List<MarkResponse>();
            foreach (Trainee trainee in trainees)
            {
                MarkResponse markResponse = new MarkResponse();
                markResponse.TraineeName = trainee.Fullname;
                IEnumerable<Mark> markList = await _markService.GetMarkByTraineeId(trainee.TraineeId, DateTime.MinValue, DateTime.Now);
                foreach (Mark m in markList)
                {
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

    }
}