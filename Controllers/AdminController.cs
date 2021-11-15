using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.AdminDTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Authorize(Policy = "Admin")]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {

        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;

        public AdminController(IAdminService adminService, IMapper mapper)
        {
            _adminService = adminService;
            _mapper = mapper;
        }
        /// <summary>
        /// Get all admin with pagination
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns>all admin with pagination</returns>
        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<AdminResponse>>>> ViewAdminList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecords, IEnumerable<Admin> adminList) = await _adminService.GetAdminList(paginationParameter);

            IEnumerable<AdminResponse> adminResponses = _mapper.Map<IEnumerable<AdminResponse>>(adminList);
            if (totalRecords == 0)
            {
                return NotFound(new ResponseDTO(404, "Admin not found!"));
            }
            return Ok(new PaginationResponse<IEnumerable<AdminResponse>>(totalRecords, adminResponses));
        }
        /// <summary>
        /// Get admin profile
        /// </summary>
        /// <param name="id">admin id</param>
        /// <returns>200 :Admin profile /404: not found</returns>
        [HttpGet("{id:int}/profile")]
        public async Task<ActionResult<AdminProfileDetail>> ViewAdminProfile(int id)
        {
            Admin admin = await _adminService.GetAdminById(id);
            if (admin == null)
            {
                return NotFound(new ResponseDTO(404, "Admin not found!"));
            }
            AdminProfileDetail adminResponse = _mapper.Map<AdminProfileDetail>(admin);
            return adminResponse;
        }
    }
}