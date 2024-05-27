using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.ChangePassword;

namespace Application.UserCases.Commands.Users;

internal sealed class ChangePasswordCommandHandler(
    IUserRepository _userRepository,
    IUnitOfWork _unitOfWork,
    IPasswordService _passwordService) : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Result.Success> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
