using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TraineeDTO;
using kroniiapi.Helper.Upload;
using kroniiapi.Helper.UploadDownloadFile;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public MarkController(ITraineeService traineeService,
                            ICertificateService certificateService,
                            IMegaHelper megaHelper,
                            IMapper mapper)
        {
            _traineeService = traineeService;
            _certificateService = certificateService;
            _megaHelper = megaHelper;
            _mapper = mapper;
        }

         /// <summary>
        /// View trainee mark and skill
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <returns>Trainee mark and skill</returns>
        [HttpGet("{traineeId:int}")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TraineeMarkAndSkill>>>> ViewMarkAndSkill(int traineeId, [FromQuery] PaginationParameter paginationParameter)
        {
            if (await _traineeService.GetTraineeById(traineeId) == null)
            {
                return BadRequest(new ResponseDTO(404, "id not found"));
            }
            (int totalRecord, IEnumerable<TraineeMarkAndSkill> markAndSkills) = await _traineeService.GetMarkAndSkillByTraineeId(traineeId, paginationParameter);
            if (totalRecord == 0)
            {
                return BadRequest(new ResponseDTO(404, "Trainee doesn't have any module"));
            }
            return Ok(new PaginationResponse<IEnumerable<TraineeMarkAndSkill>>(totalRecord, markAndSkills));
        }
        /// <summary>
        /// submit trainee certificate (upload to mega)
        /// </summary>
        /// <param name="certificateInput">detail of certificate input</param>
        /// <returns>201: created / 400: bad request</returns>
        [HttpPost("certificate")]
        public async Task<ActionResult> SubmitCertificate(IFormFile file, int traineeId, int moduleId)
        {
            Stream stream = file.OpenReadStream();
            string Uri = await _megaHelper.Upload(stream, file.FileName, "Certificate");
            CertificateInput certificateInput = new();
            certificateInput.ModuleId = moduleId;
            certificateInput.TraineeId = traineeId;
            certificateInput.CertificateURL = Uri;
            Certificate certificate = _mapper.Map<Certificate>(certificateInput);
            int status = await _certificateService.InsertCertificate(certificate);
            if (status == 0)
            {
                return BadRequest(new ResponseDTO(400, "Your submit failed!"));
            }
            return Ok(new ResponseDTO(201, "Your submission was successful!"));
        }
    }
}