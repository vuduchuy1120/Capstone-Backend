﻿using Contract.Abstractions.Messages;
using Contract.Services.Phase.ShareDto;

namespace Contract.Services.Phase.Queries;

public record GetPhaseByIdQuery(Guid Id) : IQueryHandler<PhaseResponse>;

