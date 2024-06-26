using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.MaterialHistory.ShareDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.MaterialHistory.Queries;

public record GetMaterialHistoriesByMaterialQuery(
    string? SearchTerms,
    string? StartDateImport,
    string? EndDateImport,
    int PageIndex = 1,
    int PageSize = 10
    ) : IQuery<SearchResponse<List<MaterialHistoryResponse>>>;