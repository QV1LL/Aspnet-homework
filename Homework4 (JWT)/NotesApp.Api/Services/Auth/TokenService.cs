using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NotesApp.Api.Models;

namespace NotesApp.Api.Services.Auth;

public class TokenService(IConfiguration config)
{
    public string GenerateAccessToken(User user)
    {
        var handler = new JsonWebTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Sub] = user.Id.ToString(),
            [JwtRegisteredClaimNames.Name] = user.Name,
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
            [ClaimTypes.Role] = "User"
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"],
            Claims = claims,
            Expires = DateTime.UtcNow.AddMinutes(1),
            SigningCredentials = credentials
        };

        return handler.CreateToken(descriptor);
    }

    public RefreshToken CreateRefreshToken(Guid userId)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "")
                .Replace("+", ""),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = userId,
            IsRevoked = false
        };
    }
}