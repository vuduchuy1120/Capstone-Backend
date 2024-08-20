using Contract.Abstractions.Messages;
using Contract.Services.MaterialHistory.ShareDto;

namespace Contract.Services.MaterialHistory.Queries;

public record GetMaterialHistoryByIdQuery(Guid Id) : IQuery<MaterialHistoryResponse>;