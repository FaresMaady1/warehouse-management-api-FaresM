namespace Warehouse.Api.IntegrationTests;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public static class TestJwt
{
    public static readonly SymmetricSecurityKey SigningKey =
        new(Encoding.UTF8.GetBytes("IntegrationTestsOnlySigningKey1234567890"));

    // CreateToken
    public static string CreateToken(string uid, string role)
    {
        var claims = new[] { new Claim("sub", uid), new Claim("role", role) };
        var credentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}