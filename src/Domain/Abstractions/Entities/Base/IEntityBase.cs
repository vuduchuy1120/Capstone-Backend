namespace Domain.Abstractions.Entities.Base;

internal interface IEntityBase<T>
{
    T Id { get; set; }
}
