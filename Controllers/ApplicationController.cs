using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ApplicationDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Helper;
using kroniiapi.Helper.Upload;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly ITraineeService _traineeService;
        private readonly IApplicationService _applicationService;
        private readonly IMapper _mapper;
        private readonly IMegaHelper _megaHelper;
        public ApplicationController(IMapper mapper,

                                 ITraineeService traineeService,

                                 IApplicationService applicationService,
                                 IMegaHelper megaHelper
                                 )
        {
            _mapper = mapper;
            _traineeService = traineeService;
            _applicationService = applicationService;
            _megaHelper = megaHelper;
        }
        /// <summary>
        /// get application list of trainee
        /// </summary>
        /// <param name="id">trainee id</param>
        /// <param name="paginationParameter">Pagination parameters from client</param>
        /// <returns>200: application list/ 400: Not found</returns>
        [HttpGet("trainee/{traineeId:int}")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TraineeApplicationResponse>>>> ViewApplicationList(int traineeId, [FromQuery] PaginationParameter paginationParameter)
        {
            if (_traineeService.CheckTraineeExist(traineeId) is false)
            {
                return NotFound(new ResponseDTO(404, "id not found"));
            }
            (int totalRecord, IEnumerable<TraineeApplicationResponse> application) = await _traineeService.GetApplicationListByTraineeId(traineeId, paginationParameter);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Trainee doesn't have any application"));
            }
            return Ok(new PaginationResponse<IEnumerable<TraineeApplicationResponse>>(totalRecord, application));
        }

        /// <summary>
        /// Trainee submit application form (mega upload)
        /// </summary>
        /// <param name="applicationInput">detail of applcation input </param>
        /// <returns>201: created</returns>
        [HttpPost]
        public async Task<ActionResult> SubmitApplicationForm([FromForm] ApplicationInput applicationInput, [FromForm] IFormFile form)
        {
            (bool isDoc, string errorMsg) = FileHelper.CheckDocExtension(form);
            if (isDoc == false)
            {
                return BadRequest(new ResponseDTO(400, errorMsg));
            }
            Application app = _mapper.Map<Application>(applicationInput);
            var rs = await _applicationService.InsertNewApplication(app, form);
            if (rs == -1)
            {
                return BadRequest(new ResponseDTO(400, "Trainee is not found"));
            }
            else if (rs == -2)
            {
                return BadRequest(new ResponseDTO(400, "Application category is not found"));
            }
            return Created("", new ResponseDTO(201, "Successfully inserted")); ;
        }

        /// <summary>
        /// get all application type
        /// </summary>
        /// <returns>all applcation type</returns>
        [HttpGet("category")]
        public async Task<ActionResult<IEnumerable<ApplicationCategoryResponse>>> ViewApplicationCategory()
        {
            var applicationTypeList = await _applicationService.GetApplicationCategoryList();
            var rs = _mapper.Map<IEnumerable<ApplicationCategoryResponse>>(applicationTypeList);
            return Ok(rs);
        }

        /// <summary>
        /// Get all application with pagination
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns>Pagination of all ApplicationResponse, not TraineeApplicationResponse</returns>
        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<ApplicationResponse>>>> ViewAllApplication([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<ApplicationResponse> appList) = await _applicationService.GetApplicationList(paginationParameter);
            //IEnumerable<TraineeApplicationResponse> appListDTO = _mapper.Map<IEnumerable<Application>, IEnumerable<TraineeApplicationResponse>>(appList);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "List empty"));
            }
            return Ok(new PaginationResponse<IEnumerable<ApplicationResponse>>(totalRecord, appList));
        }

        /// <summary>
        /// Get applcation detail
        /// </summary>
        /// <param name="id">applcation id</param>
        /// <returns>200: An applcation detail with corresponding id / 404: not found</returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApplicationDetail>> ViewApplicationDetail(int id)
        {
            Application app = await _applicationService.GetApplicationDetail(id);
            if (app == null)
            {
                return NotFound(new ResponseDTO(404, "Application not found"));
            }

            var appDTO = _mapper.Map<ApplicationDetail>(app);
            return Ok(appDTO);
        }

        /// <summary>
        /// Send accept or reject to application form
        /// </summary>
        /// <param name="id">application id</param>
        /// <param name="response">Response to application form</param>
        /// <param name="isAccepted">accept or reject</param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ConfirmApplication(int id, string response, bool isAccepted)
        {
            return null;
        }

    }
}