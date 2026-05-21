using Application.Auth.Settings;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Auth.Service
{
    public class JwtService(IOptions<AuthSettings> options)
    {
        public string GenerateToken(User user )
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString(), user.Role.ToString())
            };


            var jwtToken = new JwtSecurityToken(
                expires: DateTime.UtcNow.Add(options.Value.Expires),
                         claims: claims,
                         signingCredentials:
                            new SigningCredentials(new SymmetricSecurityKey(
                                                    Encoding
                                                    .UTF8
                                                    .GetBytes(options.Value.SecretKey)),
                                                    SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
