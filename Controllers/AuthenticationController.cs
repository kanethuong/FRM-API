using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DTO;
using kroniiapi.DTO.AuthDTO;
using kroniiapi.DTO.AccountDTO;
using kroniiapi.DTO.TokenDTO;
using kroniiapi.Helper;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IRefreshToken _refreshToken;

        public AuthenticationController(IAccountService accountService, IMapper mapper, IJwtGenerator jwtGenerator, IRefreshToken refreshToken)
        {
            _accountService = accountService;
            _mapper = mapper;
            _jwtGenerator = jwtGenerator;
            _refreshToken = refreshToken;
        }

        /// <summary>
        /// Login function to return account data and access token in body and refresh token in cookies
        /// </summary>
        /// <param name="loginInput">Login data</param>
        /// <returns>200: Account detail with access token / 404: Login fail</returns>
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginInput loginInput)
        {
            // Try to get account and password by input email 
            (var account, string password) = await _accountService.GetAccountByEmail(loginInput.Email);

            if (account == null)
            {
                return NotFound(new ResponseDTO(404, "Login fail"));
            }

            // Verify password with bcrypt
            bool verified = false;

            try
            {
                verified = BCrypt.Net.BCrypt.Verify(loginInput.Password, password);
            }
            catch
            {
                return NotFound(new ResponseDTO(404, "login fail"));
            }

            if (verified == false)
            {
                return NotFound(new ResponseDTO(404, "Login fail"));
            }

            // Create claims and roles to set to token
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Email, account.Email)
            };

            claims.Add(new Claim("role", account.Role));

            // Create access and refresh token
            string accessToken = _jwtGenerator.GenerateAccessToken(claims);

            string refreshToken = _refreshToken.CreateRefreshToken(account.Email);

            if (accessToken == null || refreshToken == null)
            {
                return NotFound(new ResponseDTO(404, "Token generate fail"));
            }

            // Add cookie with refresh token
            Response.Cookies.Append("X-Refresh-Token", refreshToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });

            // Add access token to account response
            var authResponse = _mapper.Map<DTO.AuthDTO.AccountResponse>(account);

            authResponse.AccessToken = accessToken;

            return Ok(authResponse);
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