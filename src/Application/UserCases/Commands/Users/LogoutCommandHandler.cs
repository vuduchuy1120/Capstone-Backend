using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.Logout;
using Domain.Exceptions.Users;

namespace Application.UserCases.Commands.Users;

internal sealed class LogoutCommandHandler(IRedisService _redisService)
    : ICommandHandler<LogoutCommand>
{
    public async Task<Result.Success> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (!request.logoutUserId.Equals(request.currentUserId))
        {
            throw new UserIdConflictException();
        }

        await _redisService.RemoveAsync(LoginCommandHandler.User_Redis_Prefix + request.logoutUserId);

        return Result.Success.Logout();
    }
}
