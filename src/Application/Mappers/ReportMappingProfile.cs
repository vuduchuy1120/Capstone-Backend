using AutoMapper;
using Contract.Services.Report.ShareDtos;
using Domain.Entities;

namespace Application.Mappers;

public class ReportMappingProfile : Profile
{
    public ReportMappingProfile()
    {
        CreateMap<Report, ReportResponse>()
            .ConstructUsing(src => new ReportResponse(
                src.Id,
                src.User.Id,
                src.User.FirstName + " " + src.User.LastName,
                src.User.Avatar,
                src.Description,
                src.Status,
                src.Status.ToString(),
                src.Status.GetDescription(),
                src.ReportType,
                src.ReportType.ToString(),
                src.ReportType.GetDescription(),
                src.ReplyMessage,
                src.User.CompanyId,
                DateOnly.FromDateTime(src.CreatedDate.Date)
                ));
    }
}
