using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityService.Data;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Services;

public sealed class TokenService(IConfiguration configuration)
{
    public string GenerateToken(AppUser user, IList<string> roles)
    {
        var secret = configuration["Jwt:Secret"]!;
        var issuer = configuration["Jwt:Issuer"]!;
        var audience = configuration["Jwt:Audience"]!;
        var expiryMinutes = int.Parse(configuration["Jwt:ExpiryMinutes"] ?? "480");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.FullName),
            new("firstName", user.FirstName),
            new("lastName", user.LastName),
        };

        if (user.DoctorId.HasValue)
            claims.Add(new Claim("doctorId", user.DoctorId.Value.ToString()));
        if (user.ClinicId.HasValue)
            claims.Add(new Claim("clinicId", user.ClinicId.Value.ToString()));

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
