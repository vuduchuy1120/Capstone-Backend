using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.MonthlyCompanySalary.Queries;
using Contract.Services.MonthlyCompanySalary.ShareDtos;

namespace Application.UserCases.Queries.MonthlyCompanySalaries;

internal sealed class SearchMonthlyCompanySalaries
    (IMonthlyCompanySalaryRepository _monthlyCompanySalaryRepository,
    IMapper _mapper) : IQueryHandler<GetMonthlyCompanySalaryQuery, SearchResponse<List<MonthlyCompanySalaryResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<MonthlyCompanySalaryResponse>>>> Handle(GetMonthlyCompanySalaryQuery request, CancellationToken cancellationToken)
    {
        var query = await _monthlyCompanySalaryRepository.SearchMonthlyCompanySalary(request);
        var monthlyCompanySalaries = query.Item1;
        var totalPage = query.Item2;

        var data = _mapper.Map<List<MonthlyCompanySalaryResponse>>(monthlyCompanySalaries);

        var response = new SearchResponse<List<MonthlyCompanySalaryResponse>>(request.PageIndex, totalPage, data);

        return Result.Success<SearchResponse<List<MonthlyCompanySalaryResponse>>>.Get(response);

    }
}
