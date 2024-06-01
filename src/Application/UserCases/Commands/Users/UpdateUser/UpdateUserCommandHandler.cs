using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.UpdateUser;
using Domain.Exceptions.Users;

namespace Application.UserCases.Commands.Users.UpdateUser;

internal sealed class UpdateUserCommandHandler(IUserRepository _userRepository, IUnitOfWork _unitOfWork)
    : ICommandHandler<UpdateUserCommand>
{
    public async Task<Result.Success> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = request.UpdateUserRequest;
        var user = await _userRepository.GetUserByIdAsync(updateRequest.Id)
            ?? throw new UserNotFoundException(updateRequest.Id);

        user.Update(updateRequest, request.UpdatedBy);

        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }
}
