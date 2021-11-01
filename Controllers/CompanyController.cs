using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DTO.CompanyDTO;
using kroniiapi.DTO.PaginationDTO;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        public CompanyController()
        {

        }
        /// <summary>
        /// View all company request with pagination
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        [HttpGet("request")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<CompanyRequestResponse>>>> ViewCompanyRequestList([FromQuery] PaginationParameter paginationParameter)
        {
            return null;
        }
        /// <summary>
        /// View all company report with pagination (CompanyRequest with isAccepted == true)
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        [HttpGet("report")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<CompanyReport>>>> ViewCompanyReportList([FromQuery] PaginationParameter paginationParameter)
        {
            return null;
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
        public async Task<ActionResult> ConfirmCompanyRequest(int id, bool isAccepted)
        {
            return null;
        }
        
    }
}