using Application.Abstractions.Services;
using Infrastructure.AuthOptions;
using Infrastructure.Options;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration
        )
    {
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();
                 options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = jwtOptions.Issuer,
                     ValidAudience = jwtOptions.Audience,
                     IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                 };
             });
        services.AddAuthorization();

        services.ConfigureOptions<JwtOptionsSetup>();

        return services;
    }
}
