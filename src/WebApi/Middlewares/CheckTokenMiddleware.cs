using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.Utils;
using Contract.Services.User.Login;
using System.Net;

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
            var tokenRepository = context.RequestServices.GetRequiredService<ITokenRepository>();

            var tokenDb = await tokenRepository.GetByUserIdAsync(userId);

            //var loginResponse = await redisService.GetAsync<LoginResponse>(ConstantUtil.User_Redis_Prefix + userId);

            if(tokenDb is null || !tokenDb.AccessToken.Equals(token))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
        }
         
        await _next(context);
    }
}
