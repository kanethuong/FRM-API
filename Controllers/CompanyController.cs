using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.CompanyDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
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
        /// <returns></returns>
        [HttpGet("report")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<CompanyReport>>>> ViewCompanyReportList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<CompanyRequest> reportList) = await _companyService.GetCompanyReportList(paginationParameter);

            if(totalRecord==0){
                return NotFound(new ResponseDTO(404,"Searched company cannot be found"));
            }

            var reports=_mapper.Map<IEnumerable<CompanyReport>>(reportList);
            
            return Ok(new PaginationResponse<IEnumerable<CompanyReport>>(totalRecord,reports));
        }
        /// <summary>
        /// View Company Request Detail 
        /// </summary>
        /// <param name="id">company request id</param>
        /// <returns></returns>
        [HttpGet("request/{id:int}")]
        public async Task<ActionResult<RequestDetail>> ViewCompanyRequestDetail(int id)
        {
            return null;
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
            return null;
        }
        /// <summary>
        /// Send accept or reject company request
        /// </summary>
        /// <param name="id">company request id</param>
        /// <param name="isAccepted">accept or reject</param>
        /// <returns></returns>
        [HttpPut("request/{id:int}")]
        public async Task<ActionResult> ConfirmCompanyRequest(int id,[FromBody] bool isAccepted)
        {
            var rs = await _companyService.ConfirmCompanyRequest(id, isAccepted);
            if (rs == -1)
            {
                return NotFound(new ResponseDTO(404, "Company request cannot be found"));
            }
            else if (rs == -2)
            {
                return BadRequest(new ResponseDTO(400, "Company request had been confirmed before"));
            }
            else if (rs == 1)
            {
                return Ok(new ResponseDTO(200, "The company request is comfirmed"));
            }
            else return BadRequest(new ResponseDTO(400, "Fail to update"));
        }

    }
}