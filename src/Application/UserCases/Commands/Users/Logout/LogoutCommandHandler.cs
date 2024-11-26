using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.Logout;
using Domain.Exceptions.Users;

namespace Application.UserCases.Commands.Users.Logout;

internal sealed class LogoutCommandHandler(
    IRedisService _redisService, 
    ITokenRepository _tokenRepository,
    IUnitOfWork _unitOfWork)
    : ICommandHandler<LogoutCommand>
{
    public async Task<Result.Success> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (!request.logoutUserId.Equals(request.currentUserId))
        {
            throw new UserIdConflictException();
        }

        var token = await _tokenRepository.GetByUserIdAsync(request.logoutUserId);
        
        if(token != null)
        {
            _tokenRepository.Delete(token);
            await _unitOfWork.SaveChangesAsync();
        }

        //await _redisService.RemoveAsync(ConstantUtil.User_Redis_Prefix + request.logoutUserId);

        return Result.Success.Logout();
    }
}
