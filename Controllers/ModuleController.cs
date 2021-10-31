using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ModuleDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService _moduleService;
        private readonly IMapper _mapper;
        public ModuleController(IMapper mapper, IModuleService moduleService)
        {
            _mapper = mapper;
            _moduleService = moduleService;
        }

        /// <summary>
        /// Get all module with pagination
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns>All module response with pagination</returns>
        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<ModuleResponse>>>> ViewModuleList([FromQuery] PaginationParameter paginationParameter)
        {

            (int totalRecord, IEnumerable<Module> listModule) = await _moduleService.GetAllModule(paginationParameter);

            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Search module name not found"));
            }
            var modules = _mapper.Map<IEnumerable<ModuleResponse>>(listModule);

            return Ok(new PaginationResponse<IEnumerable<ModuleResponse>>(totalRecord, modules));
        }
        /// <summary>
        /// Create new module 
        /// </summary>
        /// <param name="moduleInput">Module input to add to DB</param>
        /// <param name="syllabus">Syllabus file, upload to mega </param>
        /// <param name="icon">module icon (image), upload to mega</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateNewModule(ModuleInput moduleInput, IFormFile syllabus, IFormFile icon)
        {
            return null;
        }

        /// <summary>
        /// Update module
        /// </summary>
        /// <param name="moduleId">module id to update</param>
        /// <param name="moduleInput">module input to update</param>
        /// <param name="syllabus">Syllabus file, upload to mega (optional)</param>
        /// <param name="icon">module icon (image), upload to mega (optional)</param>
        /// <returns></returns>
        [HttpPut("{moduleId:int}")]
        public async Task<ActionResult> UpdateModule(int moduleId, ModuleInput moduleInput, IFormFile syllabus = null, IFormFile icon = null)
        {
            return null;
        }
    }
}