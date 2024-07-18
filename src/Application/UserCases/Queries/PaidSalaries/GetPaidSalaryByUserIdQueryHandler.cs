using Application.Abstractions.Data;
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

        var result = _mapper.Map<List<PaidSalaryResponse>>(paidSalaries);
        var searchResponse = new SearchResponse<List<PaidSalaryResponse>>(request.PageIndex, totalPage, result);

        return Result.Success<SearchResponse<List<PaidSalaryResponse>>>.Get(searchResponse);
    }
}
