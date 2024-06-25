using Application.Abstractions.Data;
using Application.Utils;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.EmployeeProduct.Queries;
using Contract.Services.EmployeeProduct.ShareDto;
using Domain.Exceptions.Users;

namespace Application.UserCases.Queries.EmployeeProducts;

public sealed class GetEmployeeProductsByEmployeeIdDateAndSlotIdQueryHandler
    (IEmployeeProductRepository _employeeProductRepository,
    IMapper _mapper)
    : IQueryHandler<GetEmployeeProductsByEmployeeIdDateAndSlotIdQuery, List<EmployeeProductResponse>>
{
    public async Task<Result.Success<List<EmployeeProductResponse>>> Handle(GetEmployeeProductsByEmployeeIdDateAndSlotIdQuery request, CancellationToken cancellationToken)
    {
        var date = DateUtil.ConvertStringToDateTimeOnly(request.getRequest.Date);
        var roleName = request.RoleName;
        var companyId = request.getRequest.CompanyId;
        var userId = request.getRequest.UserId;
        var userIdClaim = request.UserIdClaim;
        if (roleName != "MAIN_ADMIN" && request.CompanyIdClaim != companyId)
        {
            throw new UserNotPermissionException("You dont have permission to get employee products of other user companyID");
        }
        if ((roleName != "MAIN_ADMIN" && roleName != "BRANCH_ADMIN" && roleName != "COUNTER") && userId != userIdClaim)
        {
            throw new UserNotPermissionException("You don't have permission to get employee products of other user companyID");
        }

        var searchResult = await _employeeProductRepository.GetEmployeeProductsByEmployeeIdDateAndSlotId(request.getRequest.UserId, request.getRequest.SlotId, date, companyId);
        var result = _mapper.Map<List<EmployeeProductResponse>>(searchResult);

        return Result.Success<List<EmployeeProductResponse>>.Get(result);
    }

}
