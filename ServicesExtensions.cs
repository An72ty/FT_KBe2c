using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql.Replication;

namespace ft_kbe2c;

public static class ServicesExtensions
{
    public static void AddApiAuthentication(this IServiceCollection services, JwtOptions jwtOptions)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => { options.TokenValidationParameters = new() { ValidateIssuer = false, ValidateAudience = false, ValidateLifetime = true, ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)) }; options.Events = new JwtBearerEvents { OnMessageReceived = context => { context.Token = context.Request.Cookies["zpftfkczsukf"]; return Task.CompletedTask; } }; });
        services.AddAuthorization();
    }
}