using Application.Abstractions.Services;
using Application.UserCases.Commands.Users.Login;
using Contract.Services.User.Login;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace WebApi.Middlewares;

public class CheckTokenMiddleware
{
    private readonly RequestDelegate _next;

    public CheckTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var user = context.User;

        if (user.Identity?.IsAuthenticated ?? false)
        {
            var userId = user.FindFirst("UserID").Value;
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var redisService = context.RequestServices.GetRequiredService<IRedisService>();

            var loginResponse = await redisService.GetAsync<LoginResponse>(LoginCommandHandler.User_Redis_Prefix + userId);

            if(loginResponse is null || !loginResponse.AccessToken.Equals(token))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
        }

        await _next(context);
    }
}
