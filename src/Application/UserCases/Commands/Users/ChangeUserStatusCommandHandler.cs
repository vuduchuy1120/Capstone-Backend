using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.BanUser;
using Domain.Entities;
using Domain.Exceptions.Users;

namespace Application.UserCases.Commands.Users;

internal sealed class ChangeUserStatusCommandHandler(IUserRepository _userRepository, IUnitOfWork _unitOfWork)
    : ICommandHandler<ChangeUserStatusCommand>
{
    public async Task<Result.Success> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.userId) 
            ?? throw new UserNotFoundException(request.userId);

        user.UpdateStatus(request);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }
}
