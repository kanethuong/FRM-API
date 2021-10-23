using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace kroniiapi.Helper
{
    public class JwtGenerator : IJwtGenerator
    {
        private readonly string key = "this is my custom Secret key for authnetication";
        public JwtGenerator(string key)
        {
            this.key = key;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            // Declare token and properties
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(key);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature
                )
            };

            // Generate token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, // We only get payload token.
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero, // Disable default 5 mins of Microsoft
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSercurityToken = securityToken as JwtSecurityToken;
            if (jwtSercurityToken == null || !jwtSercurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}