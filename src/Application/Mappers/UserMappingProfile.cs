using AutoMapper;
using Contract.Services.User.Command;
using Contract.Services.User.CreateUser;
using Contract.Services.User.SharedDto;
using Domain.Entities;

namespace Application.Helpers;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserResponse>();
    }
}
