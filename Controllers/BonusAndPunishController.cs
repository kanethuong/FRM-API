using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.BonusAndPunishDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BonusAndPunishController : ControllerBase
    {
        private readonly IBonusAndPunishService _bonusAndPunishService;
        private readonly IMapper _mapper;

        public BonusAndPunishController(IBonusAndPunishService bonusAndPunishService, IMapper mapper)
        {
            _bonusAndPunishService = bonusAndPunishService;
            _mapper = mapper;
        }
        /// <summary>
        /// Create new Bonus and punish , bonus > 0, punish < 0
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateNewBonusOrPunish([FromBody] BonusAndPunishInput input)
        {
            BonusAndPunish bonusAndPunishInput = _mapper.Map<BonusAndPunish>(input);
            int status = await _bonusAndPunishService.InsertNewBonusAndPunish(bonusAndPunishInput);
            if (status == 0) {
                return BadRequest(new ResponseDTO(400,"Failed to create new Bonus or Punish"));
            }
            return Ok(new ResponseDTO(201,"Created!"));
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