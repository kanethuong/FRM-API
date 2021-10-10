using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DTO.AuthDTO;
using kroniiapi.DTO.TokenDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AuthenticationController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        /// <summary>
        /// Login function to return account data and access token in body and refresh token in cookies
        /// </summary>
        /// <param name="loginInput">Login data</param>
        /// <returns>200: Account detail with access token / 404: Login fail</returns>
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginInput loginInput)
        {
            return Ok();
        }

        /// <summary>
        /// Revoke refresh token of that user
        /// </summary>
        /// <param name="token">Access token</param>
        /// <returns>200: Logout success</returns>
        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody] Token token)
        {
            return Ok();
        }
    }
}