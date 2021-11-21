using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.CompanyDTO;
using kroniiapi.DTO.PaginationCompanyDTO;
//using kroniiapi.DTO.PaginationCompanyDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TraineeDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Authorize(Policy = "Company")]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {

        private readonly ICompanyService _companyService;
        private readonly ITraineeService _traineeService;
        private readonly IMapper _mapper;
        public CompanyController(IMapper mapper,
                                ICompanyService companyService,
                                ITraineeService traineeService)
        {
            _mapper = mapper;
            _companyService = companyService;
            _traineeService = traineeService;
        }
        /// <summary>
        /// View all company request with pagination
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        [HttpGet("request")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<CompanyRequestResponse>>>> ViewCompanyRequestList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecords, IEnumerable<CompanyRequestResponse> companyRequestResponses) = await _companyService.GetCompanyRequestList(paginationParameter);
            if (totalRecords == 0)
            {
                return NotFound(new ResponseDTO(404, "Company not found!"));
            }
            return Ok(new PaginationResponse<IEnumerable<CompanyRequestResponse>>(totalRecords, companyRequestResponses));
        }

        /// <summary>
        /// View all company report with pagination (CompanyRequest with isAccepted == true)
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns>200: Total record, list of report / 404: Searched company cannot be found</returns>
        [HttpGet("report")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<CompanyReport>>>> ViewCompanyReportList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<CompanyReport> reportList) = await _companyService.GetCompanyReportList(paginationParameter);

            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Company report not found"));
            }

            return Ok(new PaginationResponse<IEnumerable<CompanyReport>>(totalRecord, reportList));
        }

        /// <summary>
        /// View Company Request Detail 
        /// </summary>
        /// <param name="id">company request id</param>
        /// <returns></returns>
        [HttpGet("request/{id:int}")]
        public async Task<ActionResult<RequestDetail>> ViewCompanyRequestDetail(int id)
        {
            var companyRequest = await _companyService.GetCompanyRequestDetail(id);
            if (companyRequest == null)
            {
                return NotFound(new ResponseDTO(404, "Company request not found!"));
            }
            RequestDetail requestDetail = _mapper.Map<RequestDetail>(companyRequest);
            return Ok(requestDetail);
        }

        /// <summary>
        /// View all trainee in company request with pagination
        /// </summary>
        /// <param name="id">company request id</param>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        [HttpGet("request/{id:int}/trainee")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TraineeInRequest>>>> ViewTraineeListInRequest(int id, [FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecords, IEnumerable<Trainee> trainees) = await _companyService.GetTraineesByCompanyRequestId(id, paginationParameter);
            IEnumerable<TraineeInRequest> traineeDTO = _mapper.Map<IEnumerable<TraineeInRequest>>(trainees);
            if (totalRecords == 0)
            {
                return NotFound(new ResponseDTO(404, "Trainee not found"));
            }
            return Ok(new PaginationResponse<IEnumerable<TraineeInRequest>>(totalRecords, traineeDTO));
        }

        /// <summary>
        /// Send accept or reject company request
        /// </summary>
        /// <param name="id">company request id</param>
        /// <param name="isAccepted">accept or reject</param>
        /// <returns></returns>
        [HttpPut("request/{id:int}")]
        public async Task<ActionResult> ConfirmCompanyRequest(int id, [FromBody] ConfirmCompanyRequestInput input)
        {
            var rs = await _companyService.ConfirmCompanyRequest(id, input.isAccepted);
            if (rs.Item1 == -1)
            {
                return NotFound(new ResponseDTO(404, "Company request cannot be found"));
            }
            else if (rs.Item1 == -2)
            {
                return BadRequest(new ResponseDTO(400, "Company request had been confirmed before"));
            }
            else if (rs.Item1 == -3)
            {
                return BadRequest(new ResponseDTO(400, $"There are trainees in this list is in onboard: {string.Join(", ", rs.Item2)}"));
            }
            else if (rs.Item1 != 0)
            {
                return Ok(new ResponseDTO(200, "The company request is processed"));
            }
            else return BadRequest(new ResponseDTO(400, "Fail to update"));
        }

        /// <summary>
        /// Search and show trainee list (show all trainees by default)
        /// </summary>
        /// <param name="paginationCompanyParameter">Pagination has 3 search fields</param>
        /// <returns> 200: Total record, list results / 404: Not found </returns>
        [HttpGet("trainee")]
        public async Task<ActionResult<PaginationCompanyResponse<IEnumerable<TraineeSearchResponse>>>> SearchTraineeList([FromQuery] PaginationCompanyParameter paginationCompanyParameter)
        {
            return null;
        }

        /// <summary>
        /// List all skills (module) of trainee and the finish date of modules
        /// </summary>
        /// <param name="traineeId"></param>
        /// <returns> 200: List of skills / 404: Trainee not found </returns>
        [HttpGet("trainee/{traineeId:int}/skill")]
        public async Task<ActionResult<IEnumerable<TraineeSkillResponse>>> ViewTraineeSkill(int traineeId)
        {
            if(await _traineeService.GetTraineeById(traineeId) == null){
                return NotFound(new ResponseDTO(404,"Trainee not found"));
            }
            var tSRList = await _traineeService.GetTraineeSkillByTraineeId(traineeId);
            return Ok(tSRList);
        }

        /// <summary>
        /// Send request a list of trainees
        /// </summary>
        /// <param name="requestTraineeInput"></param>
        /// <returns> 200: Send success / 409: Fail to send request</returns>
        [HttpPost("request")]
        public async Task<ActionResult> SendTraineeRequest(RequestTraineeInput requestTraineeInput)
        {
            CompanyRequest companyRequest = _mapper.Map<CompanyRequest>(requestTraineeInput);

            var rs = await _companyService.InsertNewCompanyRequestIncludeTrainee(companyRequest);
            if (rs == 0) {
                return BadRequest(new ResponseDTO(409, "Failed to send request"));
            }
            return Ok(new ResponseDTO(200, "Send Success"));
        }

        /// <summary>
        /// View list of trainee request by company id
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="paginationParameter"></param>
        /// <returns> 200: Total record,list of request / 404: Company not found</returns>
        [HttpGet("{companyId:int}/request")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<RequestTraineeResponse>>>> ViewTraineeRequestList(int companyId, [FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<CompanyRequest> companyRequests) = await _companyService.GetTraineeListInRequestByCompanyId(companyId, paginationParameter);
            IEnumerable<RequestTraineeResponse> requestTraineeResponses = _mapper.Map<IEnumerable<RequestTraineeResponse>>(companyRequests);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Company not found"));
            }
            return Ok(new PaginationResponse<IEnumerable<RequestTraineeResponse>>(totalRecord, requestTraineeResponses));
        }

        /// <summary>
        /// View detail of a request of company
        /// </summary>
        /// <param name="companyId">Company id</param>
        /// <param name="requestId">Company Request id</param>
        /// <returns> 200: Request detail / 404: ID not found </returns>
        [HttpGet("{companyId:int}/{requestId:int}")]
        public async Task<ActionResult<RequestTraineeDetailResponse>> ViewRequestDetail(int companyId, int requestId)
        {
            if(await _companyService.GetCompanyById(companyId) == null)
            {
                return NotFound(new ResponseDTO(404, "Company not found!"));
            }
            if(await _companyService.GetCompanyRequestById(requestId) == null)
            {
                return NotFound(new ResponseDTO(404, "Request not found!"));
            }
            var companyRequest = await _companyService.GetRequestDetailByCompanyIdAndRequestId(companyId,requestId);
            if (companyRequest == null)
            {
                return NotFound(new ResponseDTO(404, "Company request not found!"));
            }
            RequestTraineeDetailResponse requestDetail = _mapper.Map<RequestTraineeDetailResponse>(companyRequest);
            return Ok(requestDetail);
        }

        /// <summary>
        /// Upload report of a request
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="requestId"></param>
        /// <param name="file"></param>
        /// <returns> 200: Upload success / 404: ID not found / 409: Fail to upload </returns>
        [HttpPost("{companyId:int}/{requestId:int}/report")]
        public async Task<ActionResult> UploadReport(int companyId, int requestId, [FromForm] IFormFile file)
        {
            return null;
        }
    }
}