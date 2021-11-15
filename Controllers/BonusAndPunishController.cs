using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DTO.BonusAndPunishDTO;
using kroniiapi.DTO.PaginationDTO;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BonusAndPunishController : ControllerBase
    {
        /// <summary>
        /// Create new Bonus and punish , bonus > 0, punish < 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateNewBonusOrPunish([FromBody] BonusAndPunishInput input)
        {


            return null;
        }
        /// <summary>
        /// Get Bonus and punish list in pagination
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns>00: Total record, list of B&P / 404: Searched trainee name cannot be found</returns>
        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<BonusAndPunishResponse>>>> ViewBonusAndPunish([FromQuery] PaginationParameter paginationParameter)
        {

            return null;
        }

    }
}