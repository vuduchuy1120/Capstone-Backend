using Application.Abstractions.Data;
using Application.Shared.Dtos;
using Application.Shared.Dtos.Users;
using AutoMapper;
using Domain.Users;
using MediatR;
using System.Net;

namespace Application.Users.GetById;

internal sealed class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, SuccessResponse<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<SuccessResponse<UserResponse>> Handle(
        GetByIdQuery request, 
        CancellationToken cancellationToken)
    {
        string id = request.Id;
        var user = await _userRepository.GetUserByIdAsync(id);

        if (user is null)
        {
            throw new UserNotFoundException(id);
        }

        var userResponse = _mapper.Map<UserResponse>(user);

        return new SuccessResponse<UserResponse>()
        {
            Status = (int)HttpStatusCode.OK,
            Message = $"Get user has id: {id} success",
            Data = userResponse
        };
    }
}
