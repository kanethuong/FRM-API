using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using kroniiapi.DTO;
using kroniiapi.DTO.TokenDTO;
using kroniiapi.Helper;
using kroniiapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IRefreshToken _refreshToken;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IAccountService _accountService;

        public TokenController(IRefreshToken refreshToken, IJwtGenerator jwtGenerator, IAccountService accountService)
        {
            _refreshToken = refreshToken;
            _jwtGenerator = jwtGenerator;
            _accountService = accountService;
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
        public async Task<ActionResult> RefreshAccessToken([FromBody] Token token)
        {
            // Get payload data in access token
            var principal = _jwtGenerator.GetPrincipalFromExpiredToken(token.AccessToken);

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
    }
}