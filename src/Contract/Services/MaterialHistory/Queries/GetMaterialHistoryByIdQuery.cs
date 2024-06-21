using Contract.Abstractions.Messages;
using Contract.Services.MaterialHistory.ShareDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Services.MaterialHistory.Queries;

public record GetMaterialHistoryByIdQuery(Guid Id) : IQueryHandler<MaterialHistoryResponse>;