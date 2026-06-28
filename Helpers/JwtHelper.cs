using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskTracker.Models.Entities;

namespace TaskTracker.Helpers
{
  public class JwtHelper
  {
    private readonly IConfiguration _config;

    public JwtHelper(IConfiguration config)
    {
      _config=config;
    }

    public string GenerateToken(User user)
    {
      // Claims = data we embed inside the token
      // When a request comes in, we read these without hitting the DB
      var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

      // Sign the token with our secret key
      var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!)
            );
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(
                    int.Parse(_config["JwtSettings:ExpiryInDays"]!)
                ),
                signingCredentials: creds
            );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }


  }
}
