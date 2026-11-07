using DonationHub.Application.Interfaces.Services;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DonationHub.Infrastructure.Implementaion.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly TimeProvider _timeProvider;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public JwtTokenService(TimeProvider timeProvider, IOptions<JwtOptions> jwtOptions)
        {
            _timeProvider = timeProvider;
            _jwtOptions = jwtOptions;
        }
        public string GenerateJwtToken(Guid id, string email, string userName, string fullName, IReadOnlyList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, userName),
                new Claim("FullName", fullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: claims,
                expires: _timeProvider.GetUtcNow().AddMinutes(_jwtOptions.Value.ExpiryMinutes).UtcDateTime,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
