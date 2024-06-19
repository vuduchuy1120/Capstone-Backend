using AutoMapper;
using Contract.Services.Attendance.ShareDto;
using Domain.Entities;

namespace Application.Mappers;

public class AttendanceMappingProfile : Profile
{
    public AttendanceMappingProfile()
    {
        CreateMap<Attendance, AttendanceResponse>()
        .ForCtorParam("FullName", opt => opt.MapFrom(a => a.User.FirstName + " " + a.User.LastName))
        .ForCtorParam("EmployeeProductResponses", opt => opt.MapFrom(a => a.User.EmployeeProducts));
        ;
    }
}
