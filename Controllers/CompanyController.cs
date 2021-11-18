using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.CompanyDTO;
//using kroniiapi.DTO.PaginationCompanyDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TraineeDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Authorize(Policy = "Company")]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {

        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        public CompanyController(IMapper mapper,
                                ICompanyService companyService)
        {
            _mapper = mapper;
            _companyService = companyService;
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

        
        // public async Task<ActionResult<PaginationCompanyResponse<IEnumerable<TraineeResponse>>>> ViewTraineeList([FromQuery]PaginationCompanyParameter paginationCompanyParameter){
        //     return null;
        // }
    }
}