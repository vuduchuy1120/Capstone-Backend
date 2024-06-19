using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.MaterialHistory.Queries;
using Contract.Services.MaterialHistory.ShareDto;
using Domain.Exceptions.MaterialHistories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserCases.Queries.MaterialHistories;

public class GetMaterialHistoryByIdQueryHandler
    (IMaterialHistoryRepository _materialHistoryRepository,
    IMapper _mapper) : IQueryHandler<GetMaterialHistoryByIdQuery, MaterialHistoryResponse>
{
    public async Task<Result.Success<MaterialHistoryResponse>> Handle(GetMaterialHistoryByIdQuery request, CancellationToken cancellationToken)
    {
        var materialHistory = await _materialHistoryRepository.GetMaterialHistoryByIdAsync(request.Id);
        if (materialHistory == null)
        {
            throw new MaterialHistoryNotFoundException();
        }
        var result = _mapper.Map<MaterialHistoryResponse>(materialHistory);
        return Result.Success<MaterialHistoryResponse>.Get(result);
    }
}
