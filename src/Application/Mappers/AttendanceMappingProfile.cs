using AutoMapper;
using Contract.Services.Attendance.ShareDto;
using Contract.Services.Attendance.ShareDtos;
using Domain.Entities;

namespace Application.Mappers;

public class AttendanceMappingProfile : Profile
{
    public AttendanceMappingProfile()
    {
        CreateMap<Attendance, AttendanceResponse>()
        .ForCtorParam("FullName", opt => opt.MapFrom(a => a.User.FirstName + " " + a.User.LastName))
        .ForCtorParam("EmployeeProductResponses", opt => opt.MapFrom(a => a.User.EmployeeProducts));
        CreateMap<Attendance, AttendanceUserDetailResponse>()
        .ForCtorParam("EmployeeProductResponses", opt => opt.MapFrom(a => a.User.EmployeeProducts));
        

        //CreateMap<Attendance, AttedanceDateReport>()
        //       .ForCtorParam("SlotId", opt => opt.MapFrom(src => src.SlotId))
        //       .ForCtorParam("IsPresent", opt => opt.MapFrom(src => src.IsAttendance))
        //       .ForCtorParam("isSalaryByProduct", opt => opt.MapFrom(src => src.IsSalaryByProduct))
        //       .ForCtorParam("isOverTime", opt => opt.MapFrom(src => src.IsOverTime));

        //CreateMap<IGrouping<string, Attendance>, AttendanceUserReportResponse>()
        //    .ForCtorParam("Date", opt => opt.MapFrom(src => src.Key))
        //    .ForCtorParam("AttedanceDateReports", opt => opt.MapFrom(src => src.ToList()));

        //CreateMap<IEnumerable<Attendance>, AttendanceUserReponse>()
        //    .ForCtorParam("Month", opt => opt.MapFrom(src => src.First().Date.Month))
        //    .ForCtorParam("Year", opt => opt.MapFrom(src => src.First().Date.Year))
        //    .ForCtorParam("UserId", opt => opt.MapFrom(src => src.First().UserId))
        //    .ForCtorParam("Attendances", opt => opt.MapFrom(src => src.GroupBy(a => a.Date.ToString("dd/MM/yyyy"))));
    }
}
