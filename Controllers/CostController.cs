using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.CostDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CostController : ControllerBase
    {
        private readonly ICostService _costService;
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;
        public CostController(ICostService costService, IMapper mapper, IAdminService adminService)
        {
            _costService = costService;
            _mapper = mapper;
            _adminService = adminService;
        }
        /// <summary>
        /// Get all cost with pagination
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns>200: Total record, list of cost / 404: Searched content cannot be found</returns>
        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<CostResponse>>>> ViewCostList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<Cost> costList) = await _costService.GetCostList(paginationParameter);

            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Cost type cannot be found"));
            }

            var costResponse = _mapper.Map<IEnumerable<CostResponse>>(costList);
            return Ok(new PaginationResponse<IEnumerable<CostResponse>>(totalRecord, costResponse));
        }
        /// <summary>
        /// create new cost
        /// </summary>
        /// <param name="costInput"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateNewCost([FromBody] CostInput costInput)
        {
            if (_adminService.CheckAdminExist(costInput.AdminId) is false)
            {
                return NotFound(new ResponseDTO
                {
                    Status = 404,
                    Message = "Admin does not exist"
                });
            }
            if (costInput.Amount <= 0)
            {
                return BadRequest(new ResponseDTO(400, "Cost must be greater than 0"));
            }
            Cost cost = _mapper.Map<Cost>(costInput);
            int rs = await _costService.InsertNewCost(cost);
            if (rs == -1)
            {
                return BadRequest(new ResponseDTO(400, "Cost type is not found"));
            }
            return Created("", new ResponseDTO(201, "Successfully inserted"));
        }
        [HttpGet("type")]
        public async Task<ActionResult<IEnumerable<CostTypeResponse>>> GetCostType()
        {
            var costTypeList = await _costService.GetCostTypeList();
            var rs = _mapper.Map<IEnumerable<CostTypeResponse>>(costTypeList);
            return Ok(rs);
        }

    }
}