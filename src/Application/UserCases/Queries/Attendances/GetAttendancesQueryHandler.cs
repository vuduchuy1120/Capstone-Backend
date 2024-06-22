using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Attendance.Query;
using Contract.Services.Attendance.ShareDto;
using Contract.Services.EmployeeProduct.ShareDto;
using Domain.Exceptions.Attendances;
using MediatR;

namespace Application.UserCases.Queries.Attendances;

internal sealed class GetAttendancesQueryHandler
    (IAttendanceRepository _attendanceRepository,
    IMapper _mapper) : IQueryHandler<GetAttendancesQuery, SearchResponse<List<AttendanceResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<AttendanceResponse>>>> Handle(GetAttendancesQuery request, CancellationToken cancellationToken)
    {
        var searchResult = await _attendanceRepository.SearchAttendancesAsync(request);

        var attendances = searchResult.Item1;
        var totalPage = searchResult.Item2;

        if (attendances is null || attendances.Count <= 0 || totalPage <= 0)
        {
            throw new AttendanceNotFoundException();
        }
        var data = attendances.Select(attendance => new AttendanceResponse(
                   attendance.UserId,
                   attendance.Date,
                   attendance.User.FirstName + " " + attendance.User.LastName,
                   attendance.HourOverTime,
                   attendance.IsAttendance,
                   attendance.IsOverTime,
                   attendance.IsSalaryByProduct,
                   attendance.IsManufacture,
                   attendance.User.EmployeeProducts
                       .Where(ep => ep.Date == attendance.Date && ep.SlotId == attendance.SlotId)
                       .Select(ep => new EmployeeProductResponse(
                           ep.Product.Images.FirstOrDefault()?.ImageUrl ?? string.Empty,
                           ep.Product.Name,
                           ep.Product.Id,
                           ep.Phase.Id,
                           ep.Phase.Name,
                           ep.Quantity
                       ))
                       .ToList()
               )).ToList();
        var searchResponse = new SearchResponse<List<AttendanceResponse>>(totalPage, request.PageIndex, data);

        return Result.Success<SearchResponse<List<AttendanceResponse>>>.Get(searchResponse);
    }
}
