using Application.Abstractions.Data;
using Application.Utils;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.PaidSalary.Queries;
using Contract.Services.PaidSalary.ShareDtos;
using Domain.Exceptions.Users;

namespace Application.UserCases.Queries.PaidSalaries;

internal sealed class GetPaidSalaryByUserIdQueryHandler
    (IPaidSalaryRepository _paidSalaryRepository,
    IMapper _mapper
    ) : IQueryHandler<GetPaidSalaryByUserIdQuery, SearchResponse<List<PaidSalaryResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<PaidSalaryResponse>>>> Handle(GetPaidSalaryByUserIdQuery request, CancellationToken cancellationToken)
    {
        if (request.roleNameClaim != "MAIN_ADMIN" && request.UserIdClaim != request.UserId)
        {
            throw new UserNotPermissionException("Bạn không có quyền xem thông tin này.");
        }
        var query = await _paidSalaryRepository.GetPaidSalariesByUserIdAsync(request);

        var paidSalaries = query.Item1;
        var totalPage = query.Item2;

        var result = paidSalaries.Select(
                paidSalary => new PaidSalaryResponse
                (
                    Id: paidSalary.Id,
                    UserId: paidSalary.UserId,
                    Salary: paidSalary.Salary,
                    Note: paidSalary.Note,
                    CreatedAt: DateUtil.ConvertStringToDateTimeOnly(paidSalary.CreatedDate.Date.ToString("dd/MM/yyyy"))
                )).ToList();


        var searchResponse = new SearchResponse<List<PaidSalaryResponse>>(request.PageIndex, totalPage, result);

        return Result.Success<SearchResponse<List<PaidSalaryResponse>>>.Get(searchResponse);
    }
}
