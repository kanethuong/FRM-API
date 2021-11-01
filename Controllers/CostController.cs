using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DTO.CostDTO;
using kroniiapi.DTO.PaginationDTO;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CostController : ControllerBase
    {
        public CostController()
        {
            
        }
        /// <summary>
        /// Get all cost with pagination
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<CostResponse>>>> ViewCostList([FromQuery] PaginationParameter paginationParameter)
        {
            return null;
        }
        /// <summary>
        /// create new cost
        /// </summary>
        /// <param name="costInput"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateNewCost(CostInput costInput)
        {
            return null;
        }
        
    }
}