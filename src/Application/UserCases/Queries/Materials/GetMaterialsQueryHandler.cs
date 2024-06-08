using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Material.Get;
using Contract.Services.Material.ShareDto;
using Contract.Services.User.SharedDto;
using Domain.Entities;
using Domain.Exceptions.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.UserCases.Queries.Materials;
public sealed class GetMaterialsQueryHandler(
    IMaterialRepository _materialRepository,
    IMapper _mapper) : IQueryHandler<GetMaterialsQuery, SearchResponse<List<MaterialResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<MaterialResponse>>>> Handle(GetMaterialsQuery request, CancellationToken cancellationToken)
    {
        var searchResult = await _materialRepository.SearchMaterialsAsync(request);

        var materials = searchResult.Item1;
        var totalPage = searchResult.Item2;

        if (materials is null || materials.Count <= 0 || totalPage <= 0)
        {
            throw new MaterialNotFoundException();
        }

        var result = _mapper.Map<List<MaterialResponse>>(materials);
        var searchResponse = new SearchResponse<List<MaterialResponse>>(totalPage, request.PageIndex, result);

        return Result.Success<SearchResponse<List<MaterialResponse>>>.Get(searchResponse);
    }
}
