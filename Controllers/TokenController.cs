using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DTO;
using kroniiapi.DTO.AuthDTO;
using kroniiapi.DTO.TokenDTO;
using kroniiapi.Helper;
using kroniiapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IRefreshToken _refreshToken;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public TokenController(IRefreshToken refreshToken, IJwtGenerator jwtGenerator, IAccountService accountService, IMapper mapper)
        {
            _refreshToken = refreshToken;
            _jwtGenerator = jwtGenerator;
            _accountService = accountService;
            _mapper = mapper;
        }

        /// <summary>
        /// Refresh access token using refresh token in cookie
        /// </summary>
        /// <param name="token">Access token</param>
        /// <returns>200: New accessToken / 
        /// 400: Not found refresh token in client cookies, 
        /// token is expired, 
        /// payload in access token and refresh token not the same</returns>
        [HttpPost("refresh")]
        public async Task<ActionResult<Token>> RefreshAccessToken()
        {
            // Get bearer token from Header
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

            // Get payload data in access token
            var principal = _jwtGenerator.GetPrincipalFromExpiredToken(_bearer_token);

            // Get email, role claim in user access token (payload)
            var claims = principal.Identities.First().Claims.ToList();
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var role = claims?.FirstOrDefault(c => c.Type.EndsWith("role", StringComparison.CurrentCultureIgnoreCase))?.Value;

            // Check existed email in db with that role
            var account = await _accountService.GetAccountByEmail(email);

            if (account == null || !account.Item1.Role.Equals(role, StringComparison.CurrentCultureIgnoreCase))
            {
                return BadRequest(new ResponseDTO(400, "Invalid access token"));
            }

            // Get refresh token from cookies
            if (!Request.Cookies.TryGetValue("X-Refresh-Token", out var refreshToken))
            {
                return BadRequest(new ResponseDTO(400, "Invalid token request"));
            }

            // Try to check expired and get email from refresh token
            var tokenMail = _refreshToken.GetEmailByToken(refreshToken); // Included expired check

            if (tokenMail == null || tokenMail != refreshToken)
            {
                return BadRequest(new ResponseDTO(400, "Invalid client request"));
            }

            // Generate new tokens
            var newAccessToken = _jwtGenerator.GenerateAccessToken(principal.Claims);

            return Ok(new Token { AccessToken = newAccessToken });
        }

        /// <summary>
        /// Gett access token from header then get account data and return it to user
        /// </summary>
        /// <returns>200: Account data</returns>
        [Authorize]
        [HttpPost("account")]
        public async Task<ActionResult<AuthResponse>> GetAccountData()
        {
            try
            {
                // Get bearer token from Header
                var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

                // Get payload data in access token
                var principal = _jwtGenerator.GetPrincipalFromExpiredToken(_bearer_token);

                // Get email, role claim in user access token (payload)
                var claims = principal.Identities.First().Claims.ToList();
                var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var role = claims?.FirstOrDefault(c => c.Type.EndsWith("role", StringComparison.CurrentCultureIgnoreCase))?.Value;

                // Check existed email in db with that role
                var account = await _accountService.GetAccountByEmail(email);

                if (account == null)
                {
                    return BadRequest(new ResponseDTO(400, "Invalid access token"));
                }

                // Add access token to account response
                var authResponse = _mapper.Map<AuthResponse>(account.Item1);

                authResponse.AccessToken = _bearer_token;

                return Ok(authResponse);
            }
            catch
            {
                return BadRequest(new ResponseDTO(400, "Invalid access token"));
            }
        }
    }
}