using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DTO.AdminDTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.PaginationDTO;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        public AdminController()
        {
            
        }
        /// <summary>
        /// Get all admin with pagination
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns>all admin with pagination</returns>
        [HttpGet("page")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<AdminResponse>>>> ViewAdminList([FromQuery] PaginationParameter paginationParameter)
        {
            return null;
        }
        /// <summary>
        /// Get admin profile
        /// </summary>
        /// <param name="id">admin id</param>
        /// <returns>200 :Admin profile /404: not found</returns>
        [HttpGet("{id:int}/profile")]
        public async Task<ActionResult<AdminProfileDetail>> ViewAdminProfile(int id)
        {
            return null;
        }
        /// <summary>
        /// Get all feedback of an admin
        /// </summary>
        /// <param name="id">admin id</param>
        /// <returns>All feedback of admin with list</returns>
        [HttpGet("{id:int}/feedback")]
        public async Task<ActionResult<IEnumerable<FeedbackContent>>> ViewAdminFeedback(int id)
        {
            return null;
        }
    }
}