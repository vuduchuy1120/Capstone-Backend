using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.Command;
using Domain.Entities;
using Domain.Exceptions.Users;

namespace Application.UserCases.Commands.Users;

internal sealed class CreateUserCommandHandler(
    IUserRepository _userRepository,
    IUnitOfWork _unitOfWork,
    IPasswordService _passwordService) : ICommandHandler<CreateUserCommand>
{
    public async Task<Result.Success> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var createUserRequest = request.CreateUserRequest;
        var isUserExisted = await _userRepository.IsUserExistAsync(createUserRequest.Id);
        
        if (isUserExisted)
        {
            throw new UserAlreadyExistedException(createUserRequest.Id);
        }

        string hashPassword = _passwordService.Hash(request.CreateUserRequest.Password);
        var user = User.Create(createUserRequest, hashPassword, request.CreatedBy);

        _userRepository.AddUser(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Create();
    }
}
