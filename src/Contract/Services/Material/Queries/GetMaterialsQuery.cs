using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Attendance.ShareDto;
using Contract.Services.Material.ShareDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.Material.Get;

public record GetMaterialsQuery
(
    string? SearchTerm,
    int PageIndex = 1,
    int PageSize = 10) : IQuery<SearchResponse<List<MaterialResponse>>>;