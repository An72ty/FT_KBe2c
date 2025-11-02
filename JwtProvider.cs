

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ft_kbe2c;

public class JwtProvider(IOptions<JwtOptions> options)
{
    JwtOptions _options = options.Value;
    
    public string GenerateToken(params KeyValuePair<string, string>[] data)
    {
        List<Claim> claims = [];
        foreach (KeyValuePair<string, string> dataEntry in data.ToDictionary())
        {
            Claim claim = new Claim(dataEntry.Key, dataEntry.Value);
            claims.Add(claim);
        }
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(claims: claims, signingCredentials: signingCredentials, expires: DateTime.UtcNow.AddHours(_options.ExpiresHours));
        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;
    }
}