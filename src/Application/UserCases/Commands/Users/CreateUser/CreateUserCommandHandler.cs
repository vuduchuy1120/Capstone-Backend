using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.User.Command;
using Contract.Services.User.CreateUser;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Users;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.UserCases.Commands.Users.CreateUser;

internal sealed class CreateUserCommandHandler(
    IUserRepository _userRepository,
    IUnitOfWork _unitOfWork,
    IPasswordService _passwordService,
    IValidator<CreateUserRequest> _validator) : ICommandHandler<CreateUserCommand>
{
    
    public async Task<Result.Success> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var createUserRequest = request.CreateUserRequest;

        var validationResult = await _validator.ValidateAsync(createUserRequest);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        //var isUserExisted = await _userRepository.IsUserExistAsync(createUserRequest.Id);

        //if (isUserExisted)
        //{
        //    throw new UserAlreadyExistedException(createUserRequest.Id);
        //}

        string hashPassword = _passwordService.Hash(request.CreateUserRequest.Password);
        var user = User.Create(createUserRequest, hashPassword, request.CreatedBy);

        _userRepository.AddUser(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Create();
    }
}
