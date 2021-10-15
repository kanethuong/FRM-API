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
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginInput loginInput)
        {
            try
            {
                // Try to get account and password by input email 
                (var account, string password) = await _accountService.GetAccountByEmail(loginInput.Email);

                if (account == null)
                {
                    return NotFound(new ResponseDTO(404, "Wrong credentials"));
                }

                // Verify password with bcrypt
                bool verified = false;

                verified = BCrypt.Net.BCrypt.Verify(loginInput.Password, password);

                if (verified == false)
                {
                    return NotFound(new ResponseDTO(404, "Wrong credentials"));
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
                Response.Cookies.Append("X-Refresh-Token", refreshToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true });

                // Add access token to account response
                var authResponse = _mapper.Map<AuthResponse>(account);

                authResponse.AccessToken = accessToken;

                return Ok(authResponse);
            }
            catch
            {
                return NotFound(new ResponseDTO(404, "Wrong credentials"));
            }
        }

        /// <summary>
        /// Revoke refresh token of that user
        /// </summary>
        /// <param name="token">Access token</param>
        /// <returns>200: Logout success</returns>
        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody] Token token)
        {
            if (!Request.Cookies.TryGetValue("X-Refresh-Token", out var refreshToken))
            {
                return BadRequest(new ResponseDTO(400, "Invalid token request"));
            }
            var tokenEmail = _refreshToken.GetEmailByToken(refreshToken);
            try
            {
                _refreshToken.RemoveTokenByEmail(tokenEmail);
            }
            catch
            {
                return BadRequest(new ResponseDTO(400, "Invalid token request"));
            }
            return Ok(new ResponseDTO(200, "Logout Success!"));

        }
    }
}