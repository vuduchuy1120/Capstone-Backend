using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.UpdateUser;
using Contract.Abstractions.Exceptions;
using Domain.Exceptions.Users;
using FluentValidation;

namespace Application.UserCases.Commands.Users.UpdateUser;

internal sealed class UpdateUserCommandHandler(
    IUserRepository _userRepository, 
    IUnitOfWork _unitOfWork,
    IValidator<UpdateUserRequest> _validator) : ICommandHandler<UpdateUserCommand>
{
    public async Task<Result.Success> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = request.UpdateUserRequest;

        var validationResult = await _validator.ValidateAsync(updateRequest);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var user = await _userRepository.GetUserByIdAsync(updateRequest.Id)
            ?? throw new UserNotFoundException(updateRequest.Id);

        user.Update(updateRequest, request.UpdatedBy);

        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }
}
