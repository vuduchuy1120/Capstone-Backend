using Domain.Abstractions.Entities.Base;

namespace Domain.Abstractions.Entities;

public abstract class EntityBase<T> : IEntityBase<T>
{
    public T Id {  get; set; }
}
