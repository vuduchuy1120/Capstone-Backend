using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Abstractions.Shared.Utils;
using Contract.Services.EmployeeProduct.Queries;
using Contract.Services.EmployeeProduct.ShareDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserCases.Queries.EmployeeProducts;

public sealed class GetEmployeeProductsByEmployeeIdDateAndSlotIdQueryHandler
    (IEmployeeProductRepository _employeeProductRepository,
    IMapper _mapper)
    : IQueryHandler<GetEmployeeProductsByEmployeeIdDateAndSlotIdQuery, List<EmployeeProductResponse>>
{
    public async Task<Result.Success<List<EmployeeProductResponse>>> Handle(GetEmployeeProductsByEmployeeIdDateAndSlotIdQuery request, CancellationToken cancellationToken)
    {
        var date = DateUtils.ConvertStringToDateTimeOnly(request.date);
        var searchResult = await _employeeProductRepository.GetEmployeeProductsByEmployeeIdDateAndSlotId(request.userId, request.slotId, date);
        var result = _mapper.Map<List<EmployeeProductResponse>>(searchResult);

        return Result.Success<List<EmployeeProductResponse>>.Get(result);
    }

}
