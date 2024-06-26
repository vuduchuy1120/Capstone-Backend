using Application.Abstractions.Services;
using Infrastructure.AuthOptions;
using Infrastructure.Options;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;

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

                 options.Events = new JwtBearerEvents
                 {
                     OnAuthenticationFailed = context =>
                     {
                         context.Response.StatusCode = 401;
                         context.Response.ContentType = "application/json";
                         var result = JsonSerializer.Serialize(new { message = "Authentication failed" });
                         return context.Response.WriteAsync(result);
                     }
                 };
             });
        services.AddAuthorization();

        services.AddAuthorizationBuilder()
            .AddPolicy("Require-Admin", policy => policy.RequireClaim("Role", "MAIN_ADMIN"))
            .AddPolicy("Require-User", policy => policy.RequireClaim("Role", "USER"))
            .AddPolicy("Require-Counter", policy => policy.RequireClaim("Role", "COUNTER"))
            .AddPolicy("Require-Driver", policy => policy.RequireClaim("Role", "DRIVER"))
            .AddPolicy("Require-Branch-Admin", policy => policy.RequireClaim("Role", "BRANCH_ADMIN"))
            .AddPolicy("RequireAdminOrBranchAdmin", policy =>
                        policy.RequireAssertion(
                            context =>
                            context.User.HasClaim(
                                c => c.Type == "Role" &&
                                (c.Value == "MAIN_ADMIN" || c.Value == "BRANCH_ADMIN"))))
            .AddPolicy("RequireAdminOrCounter", policy => policy.RequireAssertion(
                                           context =>
                                            context.User.HasClaim(
                                            c => c.Type == "Role" &&
                                            (c.Value == "MAIN_ADMIN" || c.Value == "COUNTER" || c.Value == "BRANCH_ADMIN"))))
            .AddPolicy("RequireAdminOrDriver", policy => policy.RequireAssertion(
                context => context.User.HasClaim(
                    c=> c.Type == "Role" &&
                    (c.Value == "MAIN_ADMIN" || c.Value == "DRIVER" || c.Value == "BRANCH_ADMIN"))))
            .AddPolicy("RequireAnyRole", policy => policy.RequireAssertion(
                               context => context.User.HasClaim(
                                c => c.Type == "Role")));



        services.AddStackExchangeRedisCache(options =>
        {
            var connection = configuration.GetConnectionString("Redis");
            options.Configuration = connection;

        });

        services.AddScoped<IRedisService, RedisService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ICloudStorage, GoogleCloudStorage>();

        var apiKeySid = "SK.0.DjKijVdL1BKmr4ktbhuk84ugDaBWb498";
        var apiKeySecret = "MVZtVzh1TWhpZTZuY2cwV3g2WmZyZjZxbnFFTnJCcFE=";

        services.AddSingleton<ISmsService>(new SmsService(apiKeySid, apiKeySecret));

        services.ConfigureOptions<JwtOptionsSetup>();

        return services;
    }
}
