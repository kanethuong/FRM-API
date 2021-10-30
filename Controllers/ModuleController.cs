using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ClassDetailDTO;
using kroniiapi.DTO.ModuleDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService _moduleService;
        private readonly IMapper _mapper;
        public ModuleController(IMapper mapper,IModuleService moduleService)
        {
            _mapper = mapper;
            _moduleService = moduleService;
        }

        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<ModuleResponse>>>> GetAllModule([FromQuery]PaginationParameter paginationParameter)
        {
            
            (int totalRecord, IEnumerable<Module> listModule) = await _moduleService.GetAllModule(paginationParameter);

            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Search module name not found"));
            }
            var modules = _mapper.Map<IEnumerable<ModuleResponse>>(listModule);

            return Ok(new PaginationResponse<IEnumerable<ModuleResponse>>(totalRecord, modules));
        }
    }
}