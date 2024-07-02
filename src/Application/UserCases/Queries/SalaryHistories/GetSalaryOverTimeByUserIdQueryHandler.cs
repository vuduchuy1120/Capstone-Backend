using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.SalaryHistory.Queries;
using Contract.Services.SalaryHistory.ShareDtos;
using Domain.Exceptions.Users;

namespace Application.UserCases.Queries.SalaryHistories;

public sealed class GetSalaryOverTimeByUserIdQueryHandler
    (ISalaryHistoryRepository _salaryHistoryRepository,
    IUserRepository _userRepository
    ) : IQueryHandler<GetSalaryOverTimeByUserIdQuery, SearchResponse<List<SalaryByOverTimeResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<SalaryByOverTimeResponse>>>> Handle(GetSalaryOverTimeByUserIdQuery request, CancellationToken cancellationToken)
    {
        if (request.RoleName != "MAIN_ADMIN" && request.UserIdClaims != request.UserId)
        {
            throw new UserNotPermissionException("Bạn không có quyền xem thông tin lương nhân viên khác.");
        }
        var isExistUser = await _userRepository.IsUserActiveAsync(request.UserId);
        if (!isExistUser)
        {
            throw new UserNotFoundException(request.UserId);
        }
        var query = await _salaryHistoryRepository.GetSalaryHistoryByUserId(request.UserId, SalaryType.SALARY_OVER_TIME, request.PageIndex, request.PageSize);
        var salaryOverTimeHistories = query.Item1;
        var totalPage = query.Item2;

        var data = salaryOverTimeHistories.ConvertAll(s => new SalaryByOverTimeResponse(s.Salary, s.StartDate));
        var searchResponse = new SearchResponse<List<SalaryByOverTimeResponse>>(request.PageIndex, totalPage, data);
        return Result.Success<SearchResponse<List<SalaryByOverTimeResponse>>>.Get(searchResponse);

    }
}
