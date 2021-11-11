using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ModuleDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Helper;
using kroniiapi.Helper.Upload;
using kroniiapi.Helper.UploadDownloadFile;
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
        private readonly IImgHelper _imgHelper;
        private readonly IMegaHelper _megaHelper;
        private readonly IMapper _mapper;
        public ModuleController(IMapper mapper, IModuleService moduleService, IImgHelper imgHelper, IMegaHelper megaHelper)
        {
            _mapper = mapper;
            _moduleService = moduleService;
            _imgHelper = imgHelper;
            _megaHelper = megaHelper;
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
        /// <returns>400 : The File/Icon is invalid / 409 : Fail to create / 201 : Created</returns>
        [HttpPost]
        public async Task<ActionResult> CreateNewModule([FromForm] ModuleInput moduleInput)
        {
            // Map to module
            Module module = _mapper.Map<Module>(moduleInput);

            // Check File Extension on Icon & Syllabus
            var icon = moduleInput.Icon;
            var syllabus = moduleInput.Syllabus;
            bool success;
            string message;
            (success, message) = FileHelper.CheckImageExtension(icon);
            if (!success) return BadRequest(new ResponseDTO(400, "The Icon is invalid: " + message));
            (success, message) = FileHelper.CheckExcelExtension(syllabus);
            if (!success) return BadRequest(new ResponseDTO(400, "The Syllabus is invalid: " + message));

            // Upload & assign URL to Module
            using (var stream = icon.OpenReadStream())
            {
                module.IconURL = await _imgHelper.Upload(stream, icon.FileName, icon.Length, icon.ContentType);
            }
            using (var stream = syllabus.OpenReadStream())
            {
                module.SyllabusURL = await _megaHelper.Upload(stream, syllabus.FileName, "Syllabus");
            }

            // Insert module & Return Result
            int result = await _moduleService.InsertNewModule(module);
            if (result > 0)
            {
                return CreatedAtAction(nameof(ViewModuleList), new ResponseDTO(201, "Created"));
            }
            else
            {
                return Conflict(new ResponseDTO(409, "Fail to create new module"));
            }
        }

        /// <summary>
        /// Update module
        /// </summary>
        /// <param name="moduleId">module id to update</param>
        /// <param name="moduleInput">module input to update</param>
        /// <returns>404 : The module is not found / 409 : Fail to create / 200 : Updated</returns>
        [HttpPut("{moduleId:int}")]
        public async Task<ActionResult> UpdateModule(int moduleId, [FromForm] ModuleUpdateInput moduleInput)
        {
            List<string> errors = new();

            // Map input to module
            Module module = await _moduleService.GetModuleById(moduleId);
            if (module is null)
                return NotFound(new ResponseDTO(404, "The module id is not found"));
            _mapper.Map(moduleInput, module);

            // If File is not null, check file extension & assign URL to Module
            bool success;
            string message;
            if (moduleInput.Syllabus is not null)
            {
                var syllabus = moduleInput.Syllabus;
                (success, message) = FileHelper.CheckExcelExtension(syllabus);
                if (success)
                {
                    using (var stream = syllabus.OpenReadStream())
                    {
                        module.SyllabusURL = await _megaHelper.Upload(stream, syllabus.FileName, "Syllabus");
                    }
                }
                else
                {
                    errors.Add($"Error on Syllabus: {message}");
                }
            }
            if (moduleInput.Icon is not null)
            {
                var icon = moduleInput.Icon;
                (success, message) = FileHelper.CheckImageExtension(icon);
                if (success)
                {
                    using (var stream = icon.OpenReadStream())
                    {
                        module.IconURL = await _imgHelper.Upload(stream, icon.FileName, icon.Length, icon.ContentType);
                    }
                }
                else
                {
                    errors.Add($"Error on Icon: {message}");
                }
            }

            // Update Module & Return Result
            int result = await _moduleService.UpdateModule(moduleId, module);
            if (result > 0)
                return Ok(new ResponseDTO(200, "Successfully updated")
                {
                    Errors = errors
                });
            else
                return Conflict(new ResponseDTO(409, "Fail to update the module")
                {
                    Errors = errors
                });
        }
        [HttpGet("test")]
        public async Task<ActionResult> Test(int moduleId) {
            var listTest = await _moduleService.GetModuleLessonDetail(moduleId);
            return Ok(listTest);
        }
    }
}