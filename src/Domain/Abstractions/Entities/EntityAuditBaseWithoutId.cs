﻿using Domain.Abstractions.Entities.Base;

namespace Domain.Abstractions.Entities;

public abstract class EntityAuditBaseWithoutId : IAuditable
{
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
