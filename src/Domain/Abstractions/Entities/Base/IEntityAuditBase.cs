namespace Domain.Abstractions.Entities.Base;

internal interface IEntityAuditBase<T> : IEntityBase<T>, IAuditable
{
}
