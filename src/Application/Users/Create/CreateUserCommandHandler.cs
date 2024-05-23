using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.Shared.Dtos;
using Application.Shared.Dtos.Users;
using Domain.Users;
using MediatR;
using System.Net;

namespace Application.Users.Create;

internal sealed class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, SuccessResponse<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;

    public CreateUserCommandHandler(
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork, 
        IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
    }

    public async Task<SuccessResponse<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var isUserExisted = await _userRepository.IsUserExistAsync(request.Id);

        if (isUserExisted)
        {
            throw new UserAlreadyExistedException(request.Id);
        }

        var hashPassword = _passwordService.Hash(request.Password);
        var createdBy = "Nguyen dinh son";
        var user = User.Create(
            request.Id, 
            request.Fullname, 
            request.Phone, 
            request.Address,
            hashPassword,
            1,
            createdBy);

        _userRepository.AddUser(user);
        await _unitOfWork.SaveChangesAsync();

        var userResponse = new UserResponse()
        {
            Id = user.Id,
            Fullname = user.Fullname,
            Address = user.Address,
            Phone = user.Phone
        };

        return new SuccessResponse<UserResponse>()
        {
            Status = (int)HttpStatusCode.Created,
            Message = $"Create user success",
            Data = userResponse
        };
    }
}
